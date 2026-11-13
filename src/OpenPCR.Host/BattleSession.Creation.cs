using global::Elements;
using global::OpenPCR;
using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Impl;
#nullable disable
using System;
using Elements.Battle;
using Elements.Data;

namespace OpenPCR.Host;

public sealed partial class BattleSession
{
	/// <summary>Create a session from complete prepared products.</summary>
	public static BattleSession Create(PreparedBattleInputs input, BattleRunOptions options, BattleExecutionInputs execution = null,
		Action<BattleProgress> progress = null, Action<System.Collections.ICollection> battleLogRows = null, BattleUpdateClock clock = null)
	{
		ArgumentNullException.ThrowIfNull(input);
		input.Products.Validate(input.Definition);
		return CreateCore(input.Definition.MasterVersion, options, _ => input, execution ?? BattleExecutionInputs.Manual(), progress, battleLogRows, clock);
	}
	private static BattleSession CreateCore(string masterVersion, BattleRunOptions options, Func<ResourceDatabases, PreparedBattleInputs> prepare,
		BattleExecutionInputs execution, Action<BattleProgress> progress, Action<System.Collections.ICollection> battleLogRows, BattleUpdateClock clock)
	{
		ArgumentNullException.ThrowIfNull(options); ArgumentNullException.ThrowIfNull(execution);
		if (options.MaxEngineFrames <= 0) throw new ArgumentOutOfRangeException(nameof(options.MaxEngineFrames));
		execution = execution.Copy();
		if (PreparedAnimKitLoads.IsInitialized || SceneScripting.IsInitialized || ScriptingTime.IsInitialized || PreparedSceneOperations.IsInitialized || PreparedTransformInputs.IsInitialized || PreparedCoroutineOperations.IsInitialized || ManagerSingleton<ApiManager>.Instance != null || ManagerSingleton<DialogManager>.Instance != null || ManagerSingleton<MovieManager>.Instance != null || ManagerSingleton<ViewManager>.Instance != null || ManagerSingleton<SaveDataManager>.Instance != null || Singleton<TempData>.Instance != null || Singleton<ClanBattleTempData>.Instance != null || Singleton<BattleResourceManager>.Instance != null || Singleton<BattleUnitLoader>.Instance != null || SingletonMonoBehaviour<ExceptUIRoot>.Instance != null || SingletonMonoBehaviour<BattleHeaderController>.Instance != null || ManagerSingleton<MasterDataManager>.Instance != null || Singleton<UserData>.Instance != null || ManagerSingleton<ResourceManager>.Instance != null || RuntimeBinding.Get(typeof(BaseSingletonTree), "rootNode") != null) throw new InvalidOperationException("Battle requires an unowned runtime session.");
		if (global::OpenPCR.Foundation.Toolbox.ResourcesManager is not null) throw new InvalidOperationException("Resource bundle mode is already owned.");
		if (System.Threading.Interlocked.CompareExchange(ref active, 1, 0) != 0) throw new InvalidOperationException("Only one battle session may run in a process.");
		var session = new BattleSession(options, progress);
		try
		{
			session.previousRandom = ScriptingRandom.Current;
			var database = session.Own(new ResourceDatabases(options.Resources, masterVersion));
			var master = session.Own(new RuntimeMasterData(database));
			master.LoadBattleTables();
			var input = prepare(database);
			if (input.Labyrinth is not null)
			{
				master.LoadLabyrinthRelicTables();
				master.PrepareLabyrinthQuestLookups(input.Definition.Scene.Category, input.Labyrinth.QuestId);
			}
			var party = input.Consume(master);
			session.Initialize(input.Definition, input.Products, party, database, master, execution, battleLogRows, input.Labyrinth, clock ?? BattleUpdateClock.FixedRate(60));
			return session;
		}
		catch { session.Dispose(); throw; }
	}
}
