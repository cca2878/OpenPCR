using global::Elements;
using global::OpenPCR.UICompat;
using global::OpenPCR;
using global::OpenPCR.Foundation;
using global::OpenPCR.Host;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Elements.Data;
using Elements.Battle;
using Elements.Battle.Core;
using System.IO;
namespace OpenPCR.Host;

/// <summary>One process-owned battle, paused only between complete engine frames.</summary>
public sealed partial class BattleSession : IDisposable
{
	private static int active;
	private sealed class SessionState
	{
		internal int EngineFrame;
		internal long NextHandle;
		internal List<UnitCtrl> CreatedUnits;
		internal BattlePlayerInputs PlayerInputs;
		internal bool UbExploration;
		internal long UbDecisionId;
		internal readonly List<BattleScriptException> ScriptExceptions = new();
	}
	private readonly SessionState state = new();
	private readonly List<IDisposable> owned = new();
	private readonly BattleRunOptions options;
	private Action<BattleProgress> progress;
	private Action stepFrame, finishDestruction;
	private Func<BattleProgress> readProgress;
	private Func<BattleResult> readResult;
	private Func<ClanBattleSettlement> readClanSettlement;
	private Func<bool> isCompleted;
	private ScriptingRandom.State previousRandom;
	private ApiManager outputOwner;
	private DialogManager terminalDialogOwner;
	private MovieManager movieOwner;
	private BlockLayerManager blockLayerOwner;
	private SoundManager soundOwner;
	private ResourceManager resourceOwner;
	private global::OpenPCR.Foundation.ResourcesManager bundleModeOwner;
	private object parameterOwner;
	private LabyrinthTempData labyrinthOwner;
	private bool disposed, busy, faulted;
	private BattleSession(BattleRunOptions options, Action<BattleProgress> progress) { this.options = options; this.progress = progress; }
	private T Own<T>(T value) where T : class, IDisposable { if (value != null) owned.Add(value); return value; }
	public BattleProgress Progress { get { RequireAvailable(); return readProgress(); } }
	public bool Completed { get { RequireAvailable(); return isCompleted(); } }
	/// <summary>Detached observations at original output boundaries, including before the current Update returns.</summary>
	public BattleTerminalTrace ReadTerminalTrace()
	{
		RequireAvailable();
		return new("openpcr-battle-terminal/v1", isCompleted(), PreparedBattleResultOutput.ReadObservations(outputOwner)
			.Select(item => new BattleTerminalEvent(item.Kind, new(item.Point.UpdateIndex, item.Point.BattleFrame, item.Point.Wave, item.Point.GameState, item.Point.Result, item.Point.MiliLimitTime))).ToArray());
	}
	public bool IsFaulted => faulted;
	public bool Exhausted { get { RequireAvailable(); return state.EngineFrame >= options.MaxEngineFrames; } }
	private void RequireAvailable()
	{
		ObjectDisposedException.ThrowIf(disposed, this);
		if (busy) throw new InvalidOperationException("Battle session operation is already in progress.");
		if (faulted) throw new InvalidOperationException("Battle session is faulted; dispose it before creating another.");
	}
	public void Step(int engineFrames)
	{
		RequireAvailable();
		if (engineFrames <= 0) throw new ArgumentOutOfRangeException(nameof(engineFrames));
		if (state.UbExploration) throw new InvalidOperationException("Use AdvanceToUbDecision/ChooseUb, or end UB exploration before ordinary stepping.");
		busy = true;
		try
		{
			for (int i = 0; i < engineFrames && state.EngineFrame < options.MaxEngineFrames && !isCompleted(); i++)
			{
				stepFrame();
				if (state.EngineFrame % 60 == 0) progress?.Invoke(readProgress());
			}
		}
		catch { faulted = true; throw; }
		finally { busy = false; }
	}
	public void RunToEnd() => Step(int.MaxValue);
	public BattleResult ReadBattleResult()
	{
		RequireAvailable(); busy = true;
		try { return readResult(); }
		catch { faulted = true; throw; }
		finally { busy = false; }
	}
	/// <summary>Settlement of a completed clan battle or trial.</summary>
	public ClanBattleSettlement ReadClanSettlement()
	{
		RequireAvailable();
		return readClanSettlement();
	}
	public LabyrinthBattleSettlement ReadLabyrinthSettlement()
	{
		RequireAvailable();
		var finish = PreparedBattleResultOutput.ReadLabyrinth(outputOwner) ?? throw new InvalidOperationException("No completed Labyrinth finish request.");
		return new("openpcr-labyrinth-settlement/v2", finish.QuestId, finish.RemainTime, finish.AutoClear,
			((UnitHpInfo[])finish.Units).Select(unit => new BattleSettlementHp(unit.unit_id, unit.viewer_id, unit.hp)).ToArray());
	}
	private void Initialize(BattleDefinition definition, PreparedBattleProducts products, PreparedPartyInputs scene,
		ResourceDatabases db, RuntimeMasterData md, BattleExecutionInputs execution, Action<System.Collections.ICollection> battleLogRows, PreparedLabyrinthContext labyrinth, BattleUpdateClock clock)
	{
		const BindingFlags flags = RuntimeBinding.Members;
		void Set(object o, string n, object v) => RuntimeBinding.Set(o, n, v);
		var parameterProducts = products.Parameters;
		var accountGrowth = products.Account;
		var teamGrowth = products.Team;
		var players = scene.Support is null ? scene.Owned : scene.Owned.Append(scene.Support).ToList();
		// PartsData resolves the indexed enemy_m_parts child row, independently of the parent unit product.
		var partIds = labyrinth is null ? Array.Empty<int>() : md.EnemyPartLookupIds(scene.Enemies.Select(unit => (int)unit.Unique.Id).Concat(parameterProducts.ResourceUnitIds));
		md.PrepareEnemyLookupMisses(players.Concat(scene.Enemies).Select(u => (int)u.Unique.Id).Concat(scene.Limited.Keys.Select(key => key.Id)).Concat(parameterProducts.ResourceUnitIds)
			.Concat(labyrinth is null ? Array.Empty<int>() : labyrinth.Units.Select(unit => (int)unit.Unique.Id)).Concat(partIds), allowLabyrinth: labyrinth is not null);
		var settings = Own(new BattleSettingsBinding(definition.Settings, definition.Scene.Trial?.InfinityHpModeKey ?? 0));
		var env = new BattleEnvironmentInputs(definition.Account.TeamLevel, definition.Account.Values.ViewerId, definition.Account.Values.EnergyRecoveryRateMax, definition.Account.Values.CurrentClanBattleMode);
		var account = env.BindAccount();
		accountGrowth.Bind(account);
		BattleEnvironmentInputs.BindAccountBattleInputs(account, definition, clock.InitialFrame.FrameCount);
		parameterOwner = md.Manager;
		PreparedParameters.Bind(parameterOwner, (owner, id, level, equipment, evolved) => parameterProducts.CalculateSummon(owner, id, level, equipment, evolved).CreateParameter(md), id => parameterProducts.Enemy(id).CreateParameter(md),
			(unique, isSupport) =>
			{
				var data = (UnitData)unique;
				if (!scene.Limited.TryGetValue(((int)data.Id, isSupport), out var product)) throw new InvalidDataException("No prepared Limited product for " + (int)data.Id + (isSupport ? " (support)" : ""));
				if ((int)data.UnitRarity != (int)product.Limited.UnitRarity || (int)data.PromotionLevel != (int)product.Limited.PromotionLevel)
					throw new InvalidDataException("Limited refresh input differs from the prepared login unit: " + (int)data.Id);
				PreparedUnitInputBinding.Bind(product.Limited, product.Inputs);
				return product.Limited.CreateCopyData();
			});
		// Login state the Native party producer reads: owned parameters by original id and the raw deck.
		Set(account, "UnitParameterDictionary", scene.Owned.ToDictionary(u => (int)u.Unique.Id, u => u.CreateParameter(md)));
		Set(account, "partyDictionary", new Dictionary<ePartyType, int[]> { { scene.PartyType, scene.Deck } });
		var sceneEnemies = scene.Enemies.Select(u => u.CreateParameter(md)).ToList();
		var stage = Own(new BattleStageBinding(definition.Scene, sceneEnemies, teamGrowth));
		if (stage.Stage.CurrentBattleCategory != eBattleCategory.TRIAL_CLAN_BATTLE && stage.Stage.CurrentBattleCategory != eBattleCategory.CLAN_BATTLE && !((stage.Stage.CurrentBattleCategory is eBattleCategory.LABYRINTH or eBattleCategory.LABYRINTH_EX) && labyrinth is not null)) throw new NotSupportedException("Unsupported prepared battle category.");
		if (labyrinth is not null) labyrinthOwner = labyrinth.Bind(md, stage.Temp);
		// Party selection navigation input: the clan and training party screens select these party types.
		stage.Temp.SelectedPartyType = scene.PartyType;
		var clan = Own(stage.Stage.CurrentBattleCategory == eBattleCategory.CLAN_BATTLE ? new ClanBattleBinding(definition.Scene.Clan, account) : null);
		// Receives offline finish products without starting the network client.
		outputOwner = (ApiManager)RuntimeHelpers.GetUninitializedObject(typeof(ApiManager));
		if (labyrinth?.Ex is not null) PreparedBattleResultOutput.BindLabyrinthEx(outputOwner);
		else PreparedBattleResultOutput.Bind(outputOwner);
		RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(ApiManager), outputOwner);
		// Native BattleManager field initializers read the manager singletons at construction.
		// Scene-wide view singleton; BattleManager/UnitUiCtrl read it, the battle scene supplies its UI root.
		var viewManager = (ViewManager)RuntimeHelpers.GetUninitializedObject(typeof(ViewManager));
		RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(ViewManager), viewManager);
		if (ManagerSingleton<BlockLayerManager>.Instance is not null) throw new InvalidOperationException("A UI input-block owner is already active.");
		blockLayerOwner = (BlockLayerManager)RuntimeHelpers.GetUninitializedObject(typeof(BlockLayerManager));
		RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(BlockLayerManager), blockLayerOwner);
		if (ManagerSingleton<SoundManager>.Instance is not null) throw new InvalidOperationException("A sound output owner is already active.");
		soundOwner = (SoundManager)RuntimeHelpers.GetUninitializedObject(typeof(SoundManager));
		RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(SoundManager), soundOwner);
		BattleManager bm = null;
		// Explicit offline movie completion inputs; actual authored
		// princess segments remain required and the existing Native wait runs.
		movieOwner = (MovieManager)RuntimeHelpers.GetUninitializedObject(typeof(MovieManager));
		PreparedPrincessMoviePlayback.Bind(movieOwner, (type, skillId, index) =>
			type == (int)eMovieType.PRINCESS_FORM_SKILL_NORMAL && index > 0 &&
			bm.UnitList.Concat(bm.EnemyList).Any(unit => unit.UnionBurstSkillId == skillId &&
				unit.UnitActionController.GetIsSkillPrincessForm((int)skillId) &&
				unit.UnitActionController.GetPrincessFormMovieData((int)skillId).Count >= index));
		RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(MovieManager), movieOwner);
		terminalDialogOwner = (DialogManager)RuntimeHelpers.GetUninitializedObject(typeof(DialogManager));
		PreparedTerminalDialog.Bind(terminalDialogOwner, callback =>
		{
			// Offline terminal display completion. Gameplay sealing
			// and the caller's 0.5-second coroutine remain live before this output.
			if (bm.GameState != eBattleGameState.WAIT_WAVE_END || bm.TimeLimit != 0f || !(bool)RuntimeBinding.Get(bm, "isPauseTimeLimit"))
				throw new InvalidOperationException("TIME_UP presentation preceded gameplay sealing.");
			callback();
		});
		RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(DialogManager), terminalDialogOwner);
		var resources = (ResourceManager)RuntimeHelpers.GetUninitializedObject(typeof(ResourceManager));
		resourceOwner = resources;
		bundleModeOwner = (global::OpenPCR.Foundation.ResourcesManager)RuntimeHelpers.GetUninitializedObject(typeof(global::OpenPCR.Foundation.ResourcesManager));
		Set(bundleModeOwner, "useAssetBundle", true); // Explicit extracted AssetBundle deployment.
		Set(typeof(global::OpenPCR.Foundation.Toolbox), "resourcesManager", bundleModeOwner);
		typeof(ManagerSingleton<ResourceManager>).GetProperty("Instance", flags).SetValue(null, resources);
		var battleResources = new BattleResourceManager();
		typeof(Singleton<BattleResourceManager>).GetProperty("Instance", flags).SetValue(null, battleResources);
		battleResources.Init();
		var loader = new BattleUnitLoader();
		typeof(Singleton<BattleUnitLoader>).GetProperty("Instance", flags).SetValue(null, loader);
		// Host bootstrap replacing the out-of-boundary Yggdrasil.setupTree: the root, the
		// BattleManager node and the child nodes whose battle consumers resolve through it.
		var root = new SingletonNode();
		Set(typeof(BaseSingletonTree), "rootNode", root);
		Set(typeof(BaseSingletonTree), "dicNode", new Dictionary<string, SingletonNode> { { "Root", root } });
		var tree = Yggdrasil<BattleManager>.Create();
		var node = tree.AddNode(root, typeof(BattleManager));
		tree.AddNode(node, typeof(SkillEffectCtrl));
		tree.AddNode(node, typeof(UnitCtrl));
		tree.AddNode(node, typeof(UnitActionController));
		tree.AddNode(node, typeof(ActionParameter));
		tree.AddNode(node, typeof(EnvironmentData));
		tree.AddNode(node, typeof(FieldDataBase));
		tree.AddNode(node, typeof(AllUnitSealDamageCutData));
		tree.AddNode(node, typeof(StrikeBackData));
		tree.AddNode(node, typeof(PartsData));
		tree.AddNode(node, typeof(UnitUiCtrl));
		if (labyrinth is not null)
		{
			node.AddInstance(labyrinthOwner);
			tree.AddNode(node, typeof(UnitComponentRelicSkill));
			tree.AddNode(node, typeof(GiveValueAdditiveForLabyrinthAction));
		}
		else if (clan is not null) tree.AddNode(node, typeof(ClanBattleProcessor));
		else
		{
			tree.AddNode(node, typeof(TrialBattleProcessor));
			var trial = new TrialBattleTempData();
			var input = definition.Scene.Trial;
			trial.SupportUnitId = definition.Support?.UnitId ?? 0;
			trial.SupportOwnerId = definition.Support?.OwnerViewerId ?? 0;
			trial.InitClanBattleSelectDifficultyParam = new() { ClanBattleId = input.ClanBattleId, OrderNum = input.OrderNum };
			trial.DuringClanBattleModeParam = new() { InfinityHpModeKey = input.InfinityHpModeKey, BossResultPosY = input.BossResultPosY, Phase = input.Phase };
			// The selected support entry; UnitUiCtrl.Initialize finds it by the selected owner/id pair.
			var supports = new List<TrialBattleSupportUnit>();
			if (scene.Support is { } selected)
			{
				var entry = new TrialBattleSupportUnit();
				Set(entry, "OwnerViewerId", (global::System.Int64)scene.SupportOwner);
				PreparedUnitInputBinding.Bind(selected.Unique, selected.UniqueInputs);
				Set(entry, "UnitParamData", selected.Unique);
				supports.Add(entry);
			}
			trial.SupportUnitList = supports;
			node.AddInstance(trial);
		}
		var header = new BattleHeaderController();
		Set(header, "partsBossTotalDamage", new PartsBossTotalDamage());
		Set(header, "partsBossGauge", new PartsBossGauge());
		Set(header, "partsSkillWindow", RuntimeHelpers.GetUninitializedObject(typeof(PartsSkillWindow)));
		typeof(SingletonMonoBehaviour<BattleHeaderController>).GetProperty("Instance", flags).SetValue(null, header);
		bm = new BattleManager();
		Set(bm, "MuteVoice", true);
		env.BindManager(bm, db);
		var effects = new SkillEffectResources(db);
		var effectTemplates = new Dictionary<GameObject, SkillEffectResources.Resource>();
		var unitTemplates = new Dictionary<GameObject, int>();
		var unitResources = new Dictionary<int, GameObject>();
		var sceneReady = false;
		var sceneNodes = new List<EngineSceneNode>();
		var poses = new List<EngineTransformPose>();
		var activityPublisher = new SceneActivityPublisher();
		state.NextHandle = 1;
		(GameObject, Transform) Node(object[] components, Transform parent, EngineVector local, EngineVector scale, bool enabled = true, GameObject owner = null)
		{
			var go = owner ?? (GameObject)RuntimeHelpers.GetUninitializedObject(typeof(GameObject));
			var tr = (Transform)Activator.CreateInstance(typeof(Transform), true);
			var list = new List<EngineSceneObject> { new(tr, sceneReady ? SceneScripting.AllocateHandle() : (IntPtr)state.NextHandle++) };
			var behaviours = new List<EngineBehaviourRegistration>();
			foreach (var c in components)
			{
				list.Add(new(c, sceneReady ? SceneScripting.AllocateHandle() : (IntPtr)state.NextHandle++));
				if (c is Behaviour) { activityPublisher.Add(c, enabled); behaviours.Add(new(c, enabled)); }
			}
			sceneNodes.Add(new(new(go, sceneReady ? SceneScripting.AllocateHandle() : (IntPtr)state.NextHandle++), list, parent, new(go, true, false, 255, behaviours)));
			poses.Add(new(tr, local, scale, new(0, 0, 0, 1)));
			return (go, tr);
		}
		var scales = new List<float>();
		{
			var query = (Microsoft.Data.Sqlite.SqliteConnection)typeof(ResourceDatabases).GetProperty("Prefab", flags).GetValue(db);
			using var command = query.CreateCommand();
			command.CommandText = "SELECT local_scale_x FROM battle_manager_transform ORDER BY depth DESC";
			using var reader = command.ExecuteReader();
			while (reader.Read()) scales.Add((float)BitConverter.ToDouble((byte[])reader[0]));
		}
		// BattleManager -> Scaler -> UnitParent; the manager component lives on the scene root.
		Transform parent = null;
		foreach (var scaleValue in scales)
			parent = Node(parent is null ? new object[] { bm } : Array.Empty<object>(), parent, new(0, 0, 0), new(scaleValue, scaleValue, scaleValue)).Item2;
		Set(bm, "unitParentTransform", parent);
		Set(bm, "enemyParentTransform", parent);
		var playCamera = (Camera)RuntimeHelpers.GetUninitializedObject(typeof(Camera));
		Node(new object[] { playCamera }, null, new(0, 0, 0), new(1, 1, 1));
		Set(bm, "playCamera", playCamera);
		var ui = new UnitUiCtrl();
		var uiTransform = Node(new object[] { ui }, null, new(0, 0, 0), new(1, 1, 1)).Item2;
		// Footer prefab objects UnitUiCtrl.Initialize touches, including its speed restoration gate.
		GameObject UiObject() => Node(Array.Empty<object>(), uiTransform, new(0, 0, 0), new(1, 1, 1)).Item1;
		T UiComponent<T>(T component) { Node(new object[] { component }, uiTransform, new(0, 0, 0), new(1, 1, 1)); return component; }
		CustomUIButton UiButton()
		{
			var button = UiComponent(new CustomUIButton());
			// Initialize the enable-state bit from initEnable before UnitUiCtrl reads the buttons.
			// These synthetic buttons have no visual children; retain this state before UnitUiCtrl.Initialize.
			((System.Collections.BitArray)RuntimeBinding.Get(button, "flag")!)[0] = (bool)RuntimeBinding.Get(button, "initEnable")!;
			return button;
		}
		Set(ui, "effectiveIcons", Enumerable.Range(0, 5).Select(_ => UiObject()).ToArray());
		Set(ui, "buttonArray", Enumerable.Range(0, 5).Select(_ => UiButton()).ToArray());
		foreach (string name in new[] { "gameSpeedButton", "skillModeButton", "allSetModeButton", "skipBattleButton" }) Set(ui, name, UiButton());
		Set(ui, "buttonLayoutGrid", UiComponent((UIGrid)RuntimeHelpers.GetUninitializedObject(typeof(UIGrid))));
		Set(ui, "fullScreenFade", UiComponent(new FullScreenFade()));
		Set(ui, "skillReserveExecuteEffectBaseObj", UiObject());
		Set(viewManager, "curUIRoot", UiComponent((UIRoot)RuntimeHelpers.GetUninitializedObject(typeof(UIRoot))));
		var view = (ViewBattle)RuntimeHelpers.GetUninitializedObject(typeof(ViewBattle));
		var transformEffect = new TransformEffectCtrl();
		var transformEffectForBoss = (TransformEffectCtrlForBoss)RuntimeHelpers.GetUninitializedObject(typeof(TransformEffectCtrlForBoss));
		Node(new object[] { view }, null, new(0, 0, 0), new(1, 1, 1));
		Node(new object[] { transformEffect }, null, new(0, 0, 0), new(1, 1, 1));
		Node(new object[] { transformEffectForBoss }, null, new(0, 0, 0), new(1, 1, 1));
		Set(view, "unitUiCtrl", ui);
		Set(view, "transformEffectCtrl", transformEffect);
		Set(view, "transformEffectCtrlForBoss", transformEffectForBoss);
		Node(new object[] { header, header.BossTotalDamage, RuntimeBinding.Get(header, "partsBossGauge") }, null, new(0, 0, 0), new(1, 1, 1));
		Set(typeof(BattleAnimKitController), "staticBattleManager", bm);
		ExceptUIRoot except = (ExceptUIRoot)RuntimeHelpers.GetUninitializedObject(typeof(ExceptUIRoot));
		Node(new object[] { except }, null, new(0, 0, 0), new(1, 1, 1));
		typeof(SingletonMonoBehaviour<ExceptUIRoot>).GetProperty("Instance", flags).SetValue(null, except);
		GameObject CreateGauge()
		{
			int begin = sceneNodes.Count;
			var pair = Node(new object[] { new LifeGaugeController() }, null, new(0, 0, 0), new(1, 1, 1));
			if (sceneReady)
			{
				SceneScripting.RegisterNodes(sceneNodes.GetRange(begin, sceneNodes.Count - begin));
				PreparedTransformInputs.Register(poses[^1]);
			}
			return pair.Item1;
		}
		GameObject CreateUiInstance(object component, Transform parent = null)
		{
			int begin = sceneNodes.Count;
			var pair = Node(component is null ? Array.Empty<object>() : new[] { component }, parent, new(0, 0, 0), new(1, 1, 1));
			SceneScripting.RegisterNodes(sceneNodes.GetRange(begin, sceneNodes.Count - begin));
			PreparedTransformInputs.Register(poses[^1]);
			return pair.Item1;
		}
		GameObject CreateCursor(Transform parent)
		{
			// Finite prefab projection: actual component/scene identity is consumed by
			// PartsData's null guards. Animator/widget/panel members only render output.
			int begin = sceneNodes.Count;
			var cursor = (MultiTargetCursor)RuntimeHelpers.GetUninitializedObject(typeof(MultiTargetCursor));
			var pair = Node(new object[] { cursor }, parent, new(0, 0, 0), new(1, 1, 1));
			if (sceneReady)
			{
				SceneScripting.RegisterNodes(sceneNodes.GetRange(begin, sceneNodes.Count - begin));
				PreparedTransformInputs.Register(poses[^1]);
			}
			return pair.Item1;
		}
		var commonFields = new Dictionary<string, string>
			{
				{ "DebuffFieldEffectPrefab", "fxsk_0047_CUT_fx_debuff_field_magiccircle" },
				{ "SealActionEffectPrefab", "fxsk_0087_CUT_fx_state_seal" }
			};
		foreach (var field in commonFields)
		{
			var conn = (Microsoft.Data.Sqlite.SqliteConnection)typeof(ResourceDatabases).GetProperty("Prefab", flags).GetValue(db);
			using var command = conn.CreateCommand();
			command.CommandText = "SELECT prefab_path_id FROM skill_effect_component WHERE prefab_name=$name";
			command.Parameters.AddWithValue("$name", field.Value);
			var key = command.ExecuteScalar() ?? throw new Exception("Missing " + field.Value);
			var data = effects.Read((long)key);
			var template = Node(Array.Empty<object>(), null, new(data.Position.X, data.Position.Y, data.Position.Z), new(1, 1, 1)).Item1;
			effectTemplates.Add(template, data);
			Set(battleResources, field.Key, template);
		}
		var commonInputs = new CommonEffectInputs(db);
		var commonTemplates = new Dictionary<long, GameObject>();
		var fieldTemplates = new Dictionary<string, GameObject>();
		var pendingFields = new Dictionary<GameObject, Func<SkillEffectResources.Resource>>();
		GameObject EffectTemplate(SkillEffectResources.Resource data)
		{
			if (commonTemplates.TryGetValue(data.PathId, out var existing))
			{
				if (effectTemplates[existing] != data) throw new InvalidDataException("Conflicting effect template identity: " + data.PathId);
				return existing;
			}
			int begin = sceneNodes.Count;
			var pair = Node(Array.Empty<object>(), null, new(data.Position.X, data.Position.Y, data.Position.Z), new(1, 1, 1));
			commonTemplates.Add(data.PathId, pair.Item1);
			effectTemplates.Add(pair.Item1, data);
			if (sceneReady)
			{
				SceneScripting.RegisterNodes(sceneNodes.GetRange(begin, sceneNodes.Count - begin));
				PreparedTransformInputs.Register(poses[^1]);
				PreparedSceneOperations.BindName(pair.Item1, data.Name);
			}
			return pair.Item1;
		}
		// Footer UI resources UnitUiCtrl.Initialize loads; their instances only feed footer presentation.
		var uiResourceTypes = new Dictionary<int, Type>
			{
				{ (int)eResourceId.ABNORMAL_STATEICON, typeof(AbromalStateIconController) }, { (int)eResourceId.FX_SKILLON_1, null },
				{ (int)eResourceId.FX_UB_TAP, typeof(AnimKitNormalEffect) }, { (int)eResourceId.FX_SKILL_EXECUTE, null },
				{ (int)eResourceId.FX_BUFF_STATE, null }, { (int)eResourceId.FX_DEBUFF_STATE, null }, { (int)eResourceId.FX_BUFF_DEBUFF_STATE, null }
			};
		var uiTemplates = new Dictionary<GameObject, Type>();
		GameObject UiTemplate(Type component)
		{
			int begin = sceneNodes.Count;
			var pair = Node(component is null ? Array.Empty<object>() : new[] { RuntimeHelpers.GetUninitializedObject(component) }, null, new(0, 0, 0), new(1, 1, 1));
			if (sceneReady) { SceneScripting.RegisterNodes(sceneNodes.GetRange(begin, sceneNodes.Count - begin)); PreparedTransformInputs.Register(poses[^1]); }
			uiTemplates.Add(pair.Item1, component);
			return pair.Item1;
		}
		var equipmentSkills = new EquipmentSkillResources(db);
		object LoadCommon(int id, long index, bool diagnostic)
		{
			if (id == (int)eResourceId.EQUIPMANT_SKILL) return equipmentSkills.Read(index);
			if (index == 0 && uiResourceTypes.TryGetValue(id, out var uiType)) return UiTemplate(uiType);
			string fieldName = commonInputs.FieldName(id, index);
			if (fieldName is not null)
			{
				if (fieldTemplates.TryGetValue(fieldName, out var existing)) return existing;
				// This boundary supplies a resource reference,
				// with no component or numerical pose until actual effect instantiation.
				var template = (GameObject)RuntimeHelpers.GetUninitializedObject(typeof(GameObject));
				var transform = (Transform)Activator.CreateInstance(typeof(Transform), true);
				var node = new EngineSceneNode(new(template, sceneReady ? SceneScripting.AllocateHandle() : (IntPtr)state.NextHandle++), new[] { new EngineSceneObject(transform, sceneReady ? SceneScripting.AllocateHandle() : (IntPtr)state.NextHandle++) }, null, new(template, true, false, 255, Array.Empty<EngineBehaviourRegistration>()));
				sceneNodes.Add(node);
				fieldTemplates.Add(fieldName, template);
				pendingFields.Add(template, () =>
				{
					long fieldKey = commonInputs.Resolve(id, index, diagnostic) ?? throw new InvalidDataException("Missing required field resource: " + fieldName);
					var data = effects.Read(fieldKey);
					if (data.Name != fieldName) throw new InvalidDataException("Conflicting field resource identity: " + fieldName);
					return data;
				});
				if (sceneReady)
				{
					SceneScripting.RegisterNodes(new[] { node });
					PreparedSceneOperations.BindName(template, fieldName);
				}
				return template;
			}
			var key = commonInputs.Resolve(id, index, diagnostic);
			return key.HasValue ? EffectTemplate(effects.Read(key.Value)) : null;
		}
		GameObject UnitTemplate(int id)
		{
			if (unitResources.TryGetValue(id, out var existing)) return existing;
			db.Prefabs.ReadTree(id); // Validate the deployed resource before publishing a reference.
			int begin = sceneNodes.Count;
			var pair = Node(Array.Empty<object>(), null, new(0, 0, 0), new(1, 1, 1));
			unitResources.Add(id, pair.Item1); unitTemplates.Add(pair.Item1, id);
			if (sceneReady)
			{
				SceneScripting.RegisterNodes(sceneNodes.GetRange(begin, sceneNodes.Count - begin));
				PreparedTransformInputs.Register(poses[^1]);
			}
			return pair.Item1;
		}
		PreparedBattleResourceLoads.Bind(loader, resources,
			(id, enemy, instantiate) => instantiate ? global::OpenPCR.Runtime.Api.Object.Instantiate(UnitTemplate(id)) : UnitTemplate(id), LoadCommon);
		// Native loadOtherPrefab producers needed by the retained buff arrow lifecycle.
		resources.AddLoadResourceId(eResourceId.FX_BUFF, value => Set(battleResources, "EnbuffEffectPrefab", value));
		resources.AddLoadResourceId(eResourceId.FX_DEBUFF, value => Set(battleResources, "EndebuffEffectPrefab", value));
		RuntimeBinding.Call(battleResources, "loadAbnormalEffectPrefab");
		RuntimeBinding.Call(battleResources, "loadAttackHitCenterEffectPrefab");
		// Native LoadResources schedules the max-HP debuff pair after its prefab loaders.
		resources.AddLoadResourceId(eResourceId.MAX_HP_DEBUFF_EFFECT, value => Set(battleResources, "MaxHpDebuffPrefabRight", value));
		resources.AddLoadResourceId(eResourceId.MAX_HP_DEBUFF_EFFECT_L, value => Set(battleResources, "MaxHpDebuffPrefabLeft", value));
		resources.StartLoad();
		PreparedResourceInputBinding.Bind(resources, (id, index, owner) =>
		{
			if (id == (int)eResourceId.UNIT_LIFE_GAUGE && index == 0 && owner == null) return CreateGauge();
			// Footer abnormal-state icon and the per-slot buff/debuff state effects: UI-only instances.
			if (id == (int)eResourceId.ABNORMAL_STATEICON && index == 0 && owner == null) return CreateUiInstance(RuntimeHelpers.GetUninitializedObject(typeof(AbromalStateIconController)));
			if (id is (int)eResourceId.FX_BUFF_STATE or (int)eResourceId.FX_DEBUFF_STATE or (int)eResourceId.FX_BUFF_DEBUFF_STATE && index == 0 && owner == null) return CreateUiInstance(null);
			if ((id == (int)eResourceId.ANIM_MULTI_TARGET_CURSOR || id == (int)eResourceId.ANIM_MULTI_TARGET_CURSOR_OVER) &&
				index == 0 && ReferenceEquals(owner, ui.transform)) return CreateCursor((Transform)owner);
			// The retained Engine call owns GamePause and its Timer(1f) completion.
			// This component only receives the banner's output transform; Animator playback is not a clock source.
			if (id == (int)eResourceId.ANIM_MULTI_TARGET_APPEAR && index == 0 && ReferenceEquals(owner, ui.transform))
				return CreateUiInstance(RuntimeHelpers.GetUninitializedObject(typeof(Animator)), (Transform)owner);
			throw new NotSupportedException("Resource " + id);
		},
			(id, index) =>
			{
				if (id != (int)eBundleId.SPINE_SD_NORMAL || index <= 0 || index > int.MaxValue)
					throw new NotSupportedException("Unprepared animation bundle lookup: " + id + ":" + index);
				// Skin availability is read from the extracted deployment table.
				using var lookup = db.Prefab.CreateCommand();
				lookup.CommandText = "SELECT 1 FROM unit_spine_data WHERE skin_key=$skin";
				lookup.Parameters.AddWithValue("$skin", index.ToString("D6", System.Globalization.CultureInfo.InvariantCulture));
				return lookup.ExecuteScalar() is not null;
			});
		foreach (var name in new[] { "RunSmokeLPrefab", "RunSmokeRPrefab" }
)
		{
			var data = effects.Read(name == "RunSmokeLPrefab" ? "fx_run_smoke_l" : "fx_run_smoke_r");
			var template = Node(new object[] { effects.Create(data) }, null, new(data.Position.X, data.Position.Y, data.Position.Z), new(1, 1, 1)).Item1;
			Set(battleResources, name, template);
			effectTemplates.Add(template, data);
		}
		SceneScripting.Initialize(sceneNodes);
		sceneReady = true;
		PreparedSceneOperations.Initialize(original =>
		{
			if (uiTemplates.TryGetValue((GameObject)original, out var uiComponent)) return UiTemplate(uiComponent);
			if (unitTemplates.TryGetValue((GameObject)original, out int prefabId))
			{
				var unit = new UnitCtrl(); var actions = new UnitActionController();
				new UnitPrefabBindings((id, pointer) => EffectTemplate(effects.Read(pointer.PathId)), db.Prefabs).Bind(db.Prefabs.ReadTree(prefabId), unit, actions);
				int unitBegin = sceneNodes.Count;
				var unitPair = Node(new object[] { unit, actions }, null, new(0, 0, 0), new(1, 1, 1));
				SceneScripting.RegisterNodes(sceneNodes.GetRange(unitBegin, sceneNodes.Count - unitBegin));
				PreparedTransformInputs.Register(poses[^1]);
				SkeletalColliderProducer.Bind(unitPair.Item1);
				RuntimeBinding.Call(unit, "Awake");
				return unitPair.Item1;
			}
			if (pendingFields.TryGetValue((GameObject)original, out var resolveField))
			{
				var field = resolveField();
				effectTemplates.Add((GameObject)original, field);
				PreparedTransformInputs.Register(new((Transform)SceneScripting.Transform(original), new(field.Position.X, field.Position.Y, field.Position.Z), new(1, 1, 1), new(0, 0, 0, 1)));
				pendingFields.Remove((GameObject)original);
			}
			var data = effectTemplates[(GameObject)original];
			var effect = effects.Create(data, (owner, id) => EffectTemplate(effects.ReadChild(owner, id)));
			int begin = sceneNodes.Count;
			int poseBegin = poses.Count;
			var pair = Node(new object[] { effect }, null, new(data.Position.X, data.Position.Y, data.Position.Z), new(1, 1, 1), effects.Enabled(data));
			if (effect is CircleEffectController circle)
				for (int i = 0; i < circle.Children.Count; i++)
					// Before the first orbit update, Core reads only the authored local X.
					Node(new object[] { circle.Children[i] }, pair.Item2, new(effects.CircleChildX(data, i), 0, 0), new(1, 1, 1));
			SceneScripting.RegisterNodes(sceneNodes.GetRange(begin, sceneNodes.Count - begin));
			foreach (var pose in poses.GetRange(poseBegin, poses.Count - poseBegin)) PreparedTransformInputs.Register(pose);
			PreparedSceneOperations.BindName(pair.Item1, data.Name + "(Clone)");
			RuntimeBinding.Call(effect, "Awake"); return pair.Item1;
		}
, activityPublisher.Publish
, owner =>
{
	int begin = sceneNodes.Count;
	Node(Array.Empty<object>(), null, new(0, 0, 0), new(1, 1, 1), owner: (GameObject)owner);
	SceneScripting.RegisterNodes(sceneNodes.GetRange(begin, sceneNodes.Count - begin));
	PreparedTransformInputs.Register(poses[^1]);
}
, activityPublisher.Enabled, (behaviour, value) =>
{
	if (activityPublisher.Enabled(behaviour) == value) return;
	// Unity calls OnEnable/OnDisable on a registered state change; no such lifecycle producer is modeled.
	if (behaviour.GetType().GetMethod("OnEnable", RuntimeBinding.Members) is not null || behaviour.GetType().GetMethod("OnDisable", RuntimeBinding.Members) is not null)
		throw new NotSupportedException("Unmodeled OnEnable/OnDisable lifecycle: " + behaviour.GetType());
	activityPublisher.Set(behaviour, value);
	var go = SceneScripting.GameObject(behaviour);
	activityPublisher.Publish(go, SceneScripting.ActiveSelf(go));
}
);
		foreach (var pair in effectTemplates) PreparedSceneOperations.BindName(pair.Key, pair.Value.Name);
		foreach (var pair in fieldTemplates) PreparedSceneOperations.BindName(pair.Value, pair.Key);
		var coroutineDriver = Own(new PreparedCoroutineDriver());
		Set(typeof(SkillEffectCtrl), "staticBattleManager", bm);
		SceneScripting.ConfigureComponentProducer((owner, type) =>
		{
			object component = type == typeof(CoroutineManager) ? new CoroutineManager()
				: type == typeof(BattleEffectManager) ? new BattleEffectManager()
				: type == typeof(Rigidbody2D) ? new Rigidbody2D()
				: type == typeof(PolygonCollider2D) ? new PolygonCollider2D()
				: type == typeof(global::OpenPCR.AudioStubs.AudioAtomSource) ? RuntimeHelpers.GetUninitializedObject(type)
				: throw new NotSupportedException(type.FullName);
			return new EngineComponentCreation(component, component is Behaviour ? true : null, value =>
			{
				if (value is Behaviour) activityPublisher.Add(value, true);
			});
		});
		var destroy = new HashSet<object>();
		SceneScripting.ConfigureDestructionProducer(wrapper =>
		{
			if (wrapper is not Collider2D && wrapper is not Rigidbody2D) throw new NotSupportedException("Unwired destruction " + wrapper.GetType()); destroy.Add(wrapper);
		}
	   );
		PreparedAnimKitLoads.Bind((type, skin, controllerSkin, enemyColor, parentTransform, prefabSkin) =>
		{
			BattleAnimKitController controller;
			if (type == (int)eAnimKitType.SD_MODE_CHANGE && skin == controllerSkin && enemyColor == 0 && prefabSkin == 0)
				controller = new AnimKitInputs(db).CreateMode(skin, md);
			else if (type == (int)eAnimKitType.SKILL_EFFECT && skin == controllerSkin && prefabSkin == 0)
				controller = new AnimKitInputs(db).CreateEffect(skin, enemyColor);
			else if ((type == (int)eAnimKitType.SD_NORMAL_BOUND || type == (int)eAnimKitType.SD_SHADOW_BOUND) && controllerSkin == (prefabSkin != 0 ? prefabSkin : skin) && enemyColor == 0)
				controller = new AnimKitInputs(db).CreateUnitSkin(skin, md, (eAnimKitType)type, prefabSkin);
			else throw new NotSupportedException("animation resource identity");
			int begin = sceneNodes.Count; var pair = Node(new object[] { controller }, (Transform)parentTransform, new(0, 0, 0), new(1, 1, 1)); SceneScripting.RegisterNodes(sceneNodes.GetRange(begin, sceneNodes.Count - begin)); PreparedTransformInputs.Register(poses[^1]); bm.AddUnitSpineControllerList(controller); controller.IsStopState = false; return controller;
		}
, AnimKitInputs.RequireUnitAnimation);
		PreparedTransformInputs.Initialize(poses);
		PreparedTransformInputs.BindPlanarRotationMath((a, b) =>
		{
			var q = new Quaternion(a.X, a.Y, a.Z, a.W) * new Quaternion(b.X, b.Y, b.Z, b.W);
			return new(q.x, q.y, q.z, q.w);
		}, (q, v) =>
		{
			var point = new Quaternion(q.X, q.Y, q.Z, q.W) * new Vector3(v.X, v.Y, v.Z);
			return new(point.x, point.y, point.z);
		});
		ScriptingTime.Initialize(clock.InitialFrame, clock.InitialTimeScale);
		// Native lifecycle: Awake creates the runtime services, processor, lists and component managers.
		RuntimeBinding.Call(bm, "Awake");
		RuntimeBinding.Call(ui, "Awake");
		var log = (BattleLog)RuntimeBinding.Get(bm, "battleLog");
		battleLogRows?.Invoke((System.Collections.ICollection)RuntimeBinding.Get(log, "battleLogList"));
		// Offline log switches must preserve the any-enabled result used during initialization.
		bool accountLogEnabled = ((Dictionary<eBattleLogType, bool>)RuntimeBinding.Get(typeof(BattleLog), "BATTLE_LOG_SETTING")).Values.Any(value => value);
		var logging = Own(new BattleLoggingBinding(definition.Logging));
		if (logging.Enabled != accountLogEnabled) throw new NotSupportedException("Captured log switches change the Awake log-enable result.");
		// Native scene entry: ViewBattle starts the manager's coroutineStartProcess.
		PreparedBattleResultOutput.BindObservationSource(outputOwner, () => new(state.EngineFrame, (int)bm.FrameCount, bm.CurrentWave, (int)bm.GameState, (int)bm.BattleResult, bm.GetMiliLimitTime()));
		bm.Init(view);
		// Match UnitUiCtrl's rarity-six identity conversion and prepare operations before startup checkpoints.
		state.PlayerInputs = new BattlePlayerInputs(stage.Stage.BattleTimeLimit,
			players.Select(unit => UnitUtility.GetConversionUnit(unit.Unique.Id,
				ReferenceEquals(unit, scene.Support) ? unit.Unique.UnitRarity : unit.Limited.UnitRarity)).ToArray(), execution);
		ubBridge = new UbBattleManager(this, bm, state.PlayerInputs.Read().PlayerUnitIds);
		RuntimeBinding.Set(RuntimeBinding.Get(bm, "battleUnionBurstController"), "battleManager", ubBridge);
		Action updateManager = () => RuntimeBinding.Call(bm, "Update");
		Action<Exception> reportScriptException = error => state.ScriptExceptions.Add(new(state.EngineFrame,
			(int)bm.FrameCount, bm.TimeLimit, "Elements.Battle.BattleManager.Update", error.GetType().FullName,
			error.Message, BattleScriptCallbacks.ReadStackTrace(error)));
		stepFrame = () =>
		{

			var updateTime = clock.ReadUpdate(checked(state.EngineFrame + 1));
			int frame = ++state.EngineFrame;
			coroutineDriver.BeginFrame(frame);
			ScriptingTime.PublishUpdate(updateTime);
			if (bm.enabled)
			{
				// Captured operations apply on the battle clock, which starts with the first enabled Update.
				state.CreatedUnits ??= bm.UnitList.Concat(bm.EnemyList).ToList();
				state.PlayerInputs.Apply(bm, ui);
				if (state.UbExploration) ubBridge.NormalizeControls();
				foreach (BattleAnimKitController spine in (List<BattleAnimKitController>)RuntimeBinding.Get(bm, "unitSpineControllerList")) AnimKitInputs.RequireUnsubscribedEvents(spine);
				BattleScriptCallbacks.Update(updateManager, reportScriptException);
			}
			coroutineDriver.Update();
			coroutineDriver.EndFrame();
			// ViewBattle's fade-in starts once the manager reports it ready; the coroutine then enables the manager.
			if (bm.BattleReady && !view.StartFadeInCalled) Set(view, "StartFadeInCalled", true);
			state.PlayerInputs.UpdateBoundary(bm);
		};
		readProgress = () => new(state.EngineFrame, (int)bm.FrameCount, bm.TimeLimit, bm.BattleResult.ToString());
		isCompleted = () => PreparedBattleResultOutput.Read(outputOwner) is not null || PreparedBattleResultOutput.ReadLabyrinth(outputOwner) is not null || PreparedBattleResultOutput.IsLabyrinthExComplete(outputOwner);
		readResult = () =>
		{


			return new BattleResult(isCompleted(), state.EngineFrame, (int)bm.FrameCount, bm.TimeLimit, bm.BattleResult.ToString(),
				(state.CreatedUnits ?? new List<UnitCtrl>()).Select(unit => new BattleUnitResult(unit.UnitId, unit.IsOther, (long)unit.ComponentParameter.Hp, (long)unit.ComponentParameter.MaxHp, (long)unit.ComponentParameter.Energy)).ToArray(), bm.FrameRate, bm.GetSystemId(), log.GetBattleLogListCsv(), ScriptingRandom.Current)
			{ ScriptExceptions = state.ScriptExceptions.Count == 0 ? null : state.ScriptExceptions.ToArray() };
		};
		readClanSettlement = () =>
		{
			var finish = PreparedBattleResultOutput.Read(outputOwner) ?? throw new InvalidOperationException("No completed clan battle finish request.");
			{
				if (bm.BattleResult == eBattleResult.INVALID_VALUE) throw new InvalidOperationException("Battle result lacks original terminal finish output.");
				UnitHpInfo[] hp;
				UnitDamageInfo[] damage;
				if (finish.Units is TrialBattleFinishUnit trialPayload) { hp = trialPayload.unit_hp_list; damage = trialPayload.unit_damage_list; }
				else if (finish.Units is ClanBattleFinishUnit clanPayload) { hp = clanPayload.unit_hp_list; damage = clanPayload.unit_damage_list; }
				else throw new InvalidOperationException("Unknown original finish unit payload.");
				int? battleId = finish switch
				{
					PreparedBattleResultOutput.TrialFinish trialFinish => trialFinish.ClanBattleId,
					PreparedBattleResultOutput.ClanBattleFinish battleFinish => battleFinish.ClanBattleId,
					PreparedBattleResultOutput.ClanRehearsalFinish rehearsalFinish => rehearsalFinish.ClanBattleId,
					PreparedBattleResultOutput.ClanTrainingFinish => null,
					_ => throw new InvalidOperationException("Unknown finish protocol.")
				};
				return new ClanBattleSettlement(battleId, finish.Phase, finish.OrderNum, finish.TotalDamage,
					hp.Select(u => new BattleSettlementHp(u.unit_id, u.viewer_id, u.hp)).ToArray(),
					damage.Select(u => new BattleSettlementDamage(u.unit_id, u.viewer_id, u.damage, u.rarity)).ToArray())
				{ ClanRequest = finish as PreparedBattleResultOutput.ClanFinish, BattleLogId = finish.BattleLogId };
			}
		};
		finishDestruction = () =>
		{
			foreach (var wrapper in destroy)
			{
				PreparedColliderBoundsX.Release(wrapper);
				SceneScripting.CompleteComponentDestruction(wrapper);
			}
		};
	}

	public void Dispose()
	{
		if (disposed) return;
		if (busy) throw new InvalidOperationException("Cannot dispose a running session.");
		disposed = true;
		checkpoints?.Dispose();
		try
		{
			finishDestruction?.Invoke();
			for (int i = owned.Count - 1; i >= 0; i--) owned[i].Dispose();
		}
		finally
		{

			if (parameterOwner is not null) { PreparedParameters.Release(parameterOwner); PreparedLabyrinthInputs.Release(parameterOwner); }
			if (labyrinthOwner is not null) { PreparedLabyrinthInputs.Release(labyrinthOwner); labyrinthOwner = null; }
			if (resourceOwner is not null) PreparedResourceInputBinding.Release(resourceOwner);
			if (bundleModeOwner is not null) RuntimeBinding.Set(typeof(global::OpenPCR.Foundation.Toolbox), "resourcesManager", null);
			if (movieOwner is not null)
			{
				PreparedPrincessMoviePlayback.Release(movieOwner);
				RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(MovieManager), null);
			}
			if (blockLayerOwner is not null && ReferenceEquals(ManagerSingleton<BlockLayerManager>.Instance, blockLayerOwner))
				RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(BlockLayerManager), null);
			if (soundOwner is not null && ReferenceEquals(ManagerSingleton<SoundManager>.Instance, soundOwner))
				RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(SoundManager), null);
			if (terminalDialogOwner is not null)
			{
				PreparedTerminalDialog.Release(terminalDialogOwner);
				RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(DialogManager), null);
			}
			if (outputOwner is not null)
			{
				PreparedBattleResultOutput.Release(outputOwner);
				RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(ApiManager), null);
			}
			ScriptingRandom.Current = previousRandom;
			ActionParameter.StaticRelease();
			EnvironmentData.StaticRelease();
			BattleVoiceUtility.StaticRelease();
			PreparedAnimKitLoads.Release();
			PreparedSceneOperations.EndSession();
			PreparedTransformInputs.EndSession();
			ScriptingTime.EndSession();
			SceneScripting.EndSession();
			RuntimeBinding.Instance(typeof(Singleton<>), typeof(UserData), null);
			RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(ResourceManager), null);
			RuntimeBinding.Instance(typeof(Singleton<>), typeof(BattleResourceManager), null);
			RuntimeBinding.Instance(typeof(Singleton<>), typeof(BattleUnitLoader), null);
			RuntimeBinding.Instance(typeof(SingletonMonoBehaviour<>), typeof(ExceptUIRoot), null);
			RuntimeBinding.Instance(typeof(SingletonMonoBehaviour<>), typeof(BattleHeaderController), null);
			// Scene teardown (SingletonMonoBehaviour OnDestroy -> SingletonManager.OnDeleteInstance) is outside the
			// battle; drop this session's registration so the process-wide list does not retain the whole scene.
			if (SingletonMonoBehaviour<UnitUiCtrl>.Instance is { } registeredUi) ((List<Common.IDestroyInstance>)RuntimeBinding.Get(typeof(Common.SingletonManager), "singletonList")).Remove(registeredUi);
			RuntimeBinding.Instance(typeof(SingletonMonoBehaviour<>), typeof(UnitUiCtrl), null);
			RuntimeBinding.Instance(typeof(ManagerSingleton<>), typeof(ViewManager), null);
			RuntimeBinding.Set(typeof(BaseSingletonTree), "rootNode", null);
			RuntimeBinding.Set(typeof(BaseSingletonTree), "dicNode", null);
			UnitCtrl.StaticRelease();
			UnitActionController.StaticRelease();
			SkillEffectCtrl.StaticRelease();
			BattleAnimKitController.StaticRelease();
			FieldDataBase.StaticRelease();
			StrikeBackData.StaticRelease();
			AllUnitSealDamageCutData.StaticRelease();
			UbAbnormalData.StaticRelease();
			PartsBossTotalDamage.StaticRelease();
			AnimKitNormalEffect.StaticRelease();
			AnimKitFireArm.StaticRelease();
			AnimKitCowFireArm.StaticRelease();
			BlurEffect.StaticRelease();
			System.Threading.Interlocked.Exchange(ref active, 0);
			owned.Clear(); checkpointParticipants.Clear(); checkpoints = null;
			ClearUbAnchor(); ubDecisions.Clear(); ubBridge = null;
			state.CreatedUnits = null; state.PlayerInputs = null;
			outputOwner = null; terminalDialogOwner = null; movieOwner = null; resourceOwner = null; bundleModeOwner = null; parameterOwner = null;
			progress = null;
			stepFrame = null; readResult = null; readProgress = null; isCompleted = null; finishDestruction = null;
		}
	}
}
