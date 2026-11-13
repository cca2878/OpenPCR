using global::OpenPCR.UICompat;
using global::Elements;
using global::Elements.Battle;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Foundation;
using global::OpenPCR.ImageEffects;
using global::OpenPCR.Runtime.Api;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;
using Elements.Data;
using Elements.Uek;

namespace Elements.Battle
{
	public class BattleManager : MonoBehaviour, ISingletonField, IBattleManagerForBattleUnionBurstController, IBattleManagerForDungeonBattleProcessor, IBattleManagerForBaseBattleProcessor, IBattleManagerForTimeUpDeadEffect, IBattleManagerForDungeonSpecialBattleProcessor, IBattleManagerForArenaBattleProcessor, IBattleManagerForArenaReplayBattleProcessor, IBattleManagerForGrandArenaBattleProcessor, IBattleManagerForGrandArenaReplayBattleProcessor, IBattleManagerForQuestBattleProcessor, IBattleManagerForOnlyBaseBattleProcessor, IBattleManagerForTrainingBattleProcessor, IBattleManagerForQuestReplayBattleProcessor, IBattleManagerForStoryBattleProcessor, IBattleManagerForHatsuneBattleProcessor, IBattleManagerForHatsuneBossBattleProcessor, IBattleManagerForClanBattleProcessor, IBattleManagerForTutorialBattleProcessor, IBattleManagerForTowerBattleProcessor, IBattleManagerForTowerExBattleProcessor, IBattleManagerForBattleLog, IBattleManagerForCameraEffect, IBattleManagerForBattleTimeScale, IBattleManagerForDamageEffectCtrlBase, IBattleManagerForSkillEffectCtrl, IBattleManagerForBattleVoiceUtility, IBattleManagerForAbromalStateIconController, IBattleManagerForBattleHeaderController, IBattleManagerForPartsBossGauge, IBattleManagerForUnitUiCtrl, IBattleManagerForLifeGaugeController, IBattleManagerForActionParameter, IBattleManagerForFieldDataBase, IBattleManagerForUbAbnormalData, IBattleManagerForUnitActionController, IBattleManagerForSocketIOManager, IBattleManagerForViewManager, IBattleManagerForHatsuneTask, IBattleManagerForBattleAnimKitController, IBattleManagerForDialogBossResult, IBattleManagerForDialogHatsuneBossResult, IBattleManagerForDialogQuestFailed, IBattleManagerForDialogNormalArenaResult, IBattleManagerForDialogGrandArenaResult, IBattleManagerForDialogQuestResultWin, IBattleManagerForDialogTowerExResult, IBattleManagerForPartsQuestResultLater, IBattleManagerForPartsQuestResultFormer, IBattleManagerForDialogManager, IBattleManagerForTowerReplayBattleProcessor, IBattleManagerForGeneralReplayBattleProcessor, IBattleManagerForTowerReplayExBattleProcessor, IBattleManagerForTowerReplayExSingleBattleProcessor, IBattleManagerForHighRarityQuestBattleProcessor, IBattleManagerForTrainingQuestFinishSequence, IBattleManagerForKaiserSubBattleProcessor, IBattleManagerForUekBattleProcessor, IBattleManagerForSpaceBattleProcessor, IBattleManagerForReplayClanBattleProcessor, IBattleManagerForAutoLogic, IBattleManagerForTrialBattleProcessor, IBattleManagerForDialogTrialBattleResult, IBattleManagerForBaseDimensionFaultBattleProcessor, IBattleManagerForDimensionFaultBattleProcessor, IBattleManagerForDimensionFaultRehearsalBattleProcessor, IBattleManagerForDimensionFaultTreasureBattleProcessor, IBattleManagerForBywayQuestBattleProcessor, IBattleManagerForBywayQuestReplayBattleProcessor, IBattleManagerForColosseumBattleProcessor, IBattleManagerForTalentQuestBattleProcessor, IBattleManagerForAbyssBattleProcessor, IBattleManagerForAbyssBossBattleProcessor, IBattleManagerForDomeBattleProcessor, IBattleManagerForAcnEndlessBattleProcessor, IBattleManagerForAcnBossBattleProcessor, IBattleManagerForAcnSpecialBattleProcessor, IBattleManagerForAcnUnknownBattleProcesssor, IBattleManagerForMirageBattleProcessor, IBattleManagerForSevenQuestBattleProcessor, IBattleManagerForSevenScenarioBossBattleProcessor, IBattleManagerForLabyrinthBattleProcessor, IBattleManagerForLabyrinthExBattleProcessor, IBattleManagerForDialogLabyrinthExResult, IBattleManagerForMreBattleProcessor
	{

		[global::OpenPCR.Migration("739d364f-2719-4062-97f0-38e3e2ca60f7")]
		private void execBattleBonus(List<BonusEnemyData> _bonusEnemyDatas)
		{
			foreach (BonusEnemyData bonus in _bonusEnemyDatas)
			{
				UnitCtrl enemy = EnemyList.Find(e => e.UnitId == bonus.Id);
				foreach ((int, int) skill in bonus.SkillIdAndBonusIdList)
					enemy.ExecSkillBySkillId(skill.Item1, skill.Item2);
			}
		}


		private bool isAllActionDone = true;

		[global::OpenPCR.Runtime.Api.SerializeField]
		private double energyStackValueDefeat = 300d;

		[global::OpenPCR.Migration("3f5ee294-6f98-45fd-9d7b-6b29a2aed41e")]
		[global::OpenPCR.Runtime.Api.SerializeField]
		private double energyStackRatioDamagedFront = 0.5d;

		[global::OpenPCR.Migration("d4ceeaad-c1dc-48cb-8ea4-e5cafdd67a4a")]
		[global::OpenPCR.Runtime.Api.SerializeField]
		private double energyStackRatioDamagedMiddle = 0.75d;

		[global::OpenPCR.Runtime.Api.SerializeField]
		private double energyStackRationDamageBack = 1d;

		private List<SkillEffectCtrl> effectUpdateList = new List<SkillEffectCtrl>();

		private Dictionary<int, UnitCtrl> currentEnemyUnitCtrlDictionary = new Dictionary<int, UnitCtrl>();

		public List<UnitCtrl> BossList { get; set; } = new List<UnitCtrl>();

		private List<Vector2> questWinPositionList = new List<Vector2>();

		private List<BattleAnimKitController> unitSpineControllerList = new List<BattleAnimKitController>();

		private SaveDataManager.eBattleMaxSpeed settingBattleMaxSpeed { get; set; } = SaveDataManager.eBattleMaxSpeed.QUADRUPLE;

		public Dictionary<int, int> EnvironmentCounterDic { get; set; } = new Dictionary<int, int>();

		private Dictionary<eStateIconType, AllUnitSealDamageCutData> unitAllUnitSealDamageCutDataDic { get; set; } = new Dictionary<eStateIconType, AllUnitSealDamageCutData>();

		private Dictionary<eStateIconType, AllUnitSealDamageCutData> enemyAllUnitSealDamageCutDataDic { get; set; } = new Dictionary<eStateIconType, AllUnitSealDamageCutData>();

		public float BattleStartPos = 560f;

		public float LatestSkillVoiceTime = -0.5f;

		public bool HasWarnedSuspendAbuse { get; set; } = true;

		private ViewManager viewManager = ManagerSingleton<ViewManager>.Instance;

		private BlockLayerManager blockLayerManager = ManagerSingleton<BlockLayerManager>.Instance;

		private ResourceManager resourceManager = ManagerSingleton<ResourceManager>.Instance;

		private ApiManager apiManager = ManagerSingleton<ApiManager>.Instance;

		private SoundManager soundManager = ManagerSingleton<SoundManager>.Instance;

		private DialogManager dialogManager = ManagerSingleton<DialogManager>.Instance;

		private LoadingManager loadingManager = ManagerSingleton<LoadingManager>.Instance;

		private TempData tempData = Singleton<TempData>.Instance;

		private float fadeoutDuration = 0.5f;

		private Dictionary<BattleDefine.eExtraEffectExecTiming, bool> isExtraEffectCalled = new Dictionary<BattleDefine.eExtraEffectExecTiming, bool>
		{
			{
				BattleDefine.eExtraEffectExecTiming.EX_SKILL,
				false
			},
			{
				BattleDefine.eExtraEffectExecTiming.ALL_UNIT_ACTION,
				false
			},
			{
				BattleDefine.eExtraEffectExecTiming.MY_TEAM_ACTION,
				false
			},
			{
				BattleDefine.eExtraEffectExecTiming.ENEMY_ACTION,
				false
			}
		};

		private Dictionary<eBonusType, List<BonusData>> bonusDictionary = new Dictionary<eBonusType, List<BonusData>>();

		private Dictionary<int, int> bonusIdIconIndexPairsLeft { get; set; } = new Dictionary<int, int>();

		private Dictionary<int, int> bonusIdIconIndexPairsRight { get; set; } = new Dictionary<int, int>();

		public Dictionary<int, int> PlayerHpDic { get; private set; } = new Dictionary<int, int>();

		public Dictionary<int, int> EnemyHpDic { get; private set; } = new Dictionary<int, int>();

		private bool isValid = true;

		public bool PrincessFormMoviePausable { get; set; } = true;


		private bool wasJudgeAllUnitPuppet;

		[global::OpenPCR.ImplicitMigration("62cea572-bbff-4084-85c9-ac1559ad9107")][global::OpenPCR.Migration("85b1b33f-7863-4ef0-bd94-52482726d657")] public bool IsTrialClanBattle => BattleCategory == eBattleCategory.TRIAL_CLAN_BATTLE;


		[global::OpenPCR.Migration("991d7ab0-a2f7-4cb6-a96f-c282d1d678f2")]
		[global::OpenPCR.Runtime.Api.SerializeField]
		private double energyGainValueSkillFront;

		[global::OpenPCR.ImplicitMigration("5fd133af-c9a3-4c43-abf4-6625ec966f35")][global::OpenPCR.Migration("17775c1e-b376-41ed-a858-38099a9fd08c")] public double EnergyGainValieSkillFront => energyGainValueSkillFront;

		[global::OpenPCR.Migration("0abb88b6-544b-4580-aae9-d59b8a85aca5")]
		[global::OpenPCR.Runtime.Api.SerializeField]
		private double energyGainValueSkillMiddle;

		[global::OpenPCR.ImplicitMigration("b6bcaceb-def5-4548-a912-adc11c91a01f")][global::OpenPCR.Migration("c877fa78-98c4-45a8-a260-bf436a956a4f")] public double EnergyGainValieSkillMiddle => energyGainValueSkillMiddle;

		[global::OpenPCR.Migration("cf05a43a-01a0-4331-aa50-291294ce9ef1")]
		[global::OpenPCR.Runtime.Api.SerializeField]
		private double energyGainValueSkillBack;

		[global::OpenPCR.ImplicitMigration("787ae692-0406-4b17-9f6b-93acc448d373")][global::OpenPCR.Migration("3d300dff-6ce9-4677-8b30-9092103a3f13")] public double EnergyGainValieSkillBack => energyGainValueSkillBack;



		[global::OpenPCR.ImplicitMigration("f819479f-316b-4971-90c4-d0a85cec3573")][global::OpenPCR.Migration("ce852dc5-d810-4971-a05f-6d493dd05dbc")] public double EnergyStackRatioDamagedFront => energyStackRatioDamagedFront;



		[global::OpenPCR.ImplicitMigration("3d4412cd-cb1b-4b29-80fc-d4b2816ed80b")][global::OpenPCR.Migration("37ed2039-940f-4600-862a-24d781cf4d6f")] public double EnergyStackRatioDamagedMiddle => energyStackRatioDamagedMiddle;



		[global::OpenPCR.ImplicitMigration("301c34a1-b2eb-4de8-90cd-4beca3c4e2a3")][global::OpenPCR.Migration("ec0a2811-19a4-45b9-8d4f-ad11b00fb89b")] public double EnergyStackRatioDamagedBack => energyStackRationDamageBack;

		[global::OpenPCR.Migration("fbbd1e26-2945-4cd7-8864-ed1ef8612599")] public int GetTriggerHpPercent() => battleProcessor.GetTriggerHpPercent();


		private global::System.Collections.Generic.List<Elements.UnitCtrl> extraEnemyList;


		private global::Elements.ePlaybackType cutInPlaybackType { get; set; }




		[global::OpenPCR.ImplicitMigration("0206ad01-0be6-49fe-b5aa-f75cb21dd6a8")][global::OpenPCR.Migration("ff3a98a6-5bb9-4058-bd50-049cc3fae340")] public double EnergyStackValueDefeat => energyStackValueDefeat;


		[global::OpenPCR.Migration("82c97313-e55a-4891-9f80-2f972ecedd5a")]
		public void IncrementDefeatCountForEndleeBattle()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::IncrementDefeatCountForEndleeBattle():System.Void (outside first-phase endless boundary)"); }


		[global::OpenPCR.Migration("ac717d53-7573-46e8-a3fc-d7eae1e9c2d5")]
		public void StartExtraEffect(global::Elements.BattleDefine.eExtraEffectExecTiming _timing)
		{
			if (isExtraEffectCalled[_timing]) return;
			isExtraEffectCalled[_timing] = true;
			List<BonusEnemyData> data = battleProcessor.GetExtraEffectEnemyData(_timing);
			if (data.Count == 0) return;
			if (_timing == BattleDefine.eExtraEffectExecTiming.EX_SKILL)
			{
				startBonusEffectImpl(data);
				return;
			}
			IEnumerator routine = IsMultiTargetAppearPlaying ? waitMultiTargetAppearEnd(data) : waitPlayFirstWaveStartStoryEnd(data);
			AppendCoroutine(routine, ePauseType.SYSTEM, null);
		}


		[global::OpenPCR.Migration("14e1df96-045f-4874-b930-86862631c3b9")]
		public double TryApplyAllUnitDamageSeal(bool _enemySide, UnitCtrl _target, UnitCtrl _effectTarget, ActionParameter _actionParameter)
		{
			if (_actionParameter.TargetAssignment != eTargetAssignment.OTHER_SIDE)
				return 0.0;
			var dictionary = _enemySide ? enemyAllUnitSealDamageCutDataDic : unitAllUnitSealDamageCutDataDic;
			if (dictionary.Count == 0)
				return 0.0;
			if (_actionParameter.AllSealDamageCutExecValueDic.TryGetValue(_effectTarget, out double value))
				return value;
			double rate = _actionParameter.AllSealDamageCutValueForCountZero;
			if (rate != -1.0)
				return rate;
			foreach (var entry in dictionary.OrderBy(c => c.Value.Priority))
			{
				AllUnitSealDamageCutData data = entry.Value;
				SealData seal = data.Source.ComponentAbnormal.SealDictionary[data.IconType];
				if (seal.GetCurrentCount() < 1)
					continue;
				rate = data.DamageCutRate;
				if (seal.GetCurrentCount() == 1)
					_actionParameter.AllSealDamageCutValueForCountZero = rate;
				seal.RemoveSeal(1, true);
				data.Exec(_target, _effectTarget);
				_actionParameter.AllSealDamageCutExecValueDic[_effectTarget] = rate;
				return rate;
			}
			return 0.0;
		}



		[global::OpenPCR.Migration("53211243-12ba-47d2-aa4e-70fb7bd24b92")]
		public bool IsSpecialBattleModeChangeOnHpChange(int _unitId)
		{

			if ((int)BattleCategory == 58)
				throw new global::OpenPCR.OutOfBoundaryException("Seven multi-boss destruction-target input/lookup belongs to the Seven battle mode outside guild/training scope.");
			if ((int)BattleCategory == 30)
				throw new global::OpenPCR.OutOfBoundaryException("Dungeon special multi-boss destruction-target input/lookup belongs to Dungeon mode outside guild/training scope.");
			if (HatsuneUtility.IsNowEventMultiBoss())
				return HatsuneUtility.IsMultiBossDestructionTarget(_unitId);
			return true;

		}


		[global::OpenPCR.ImplicitMigration("1ffb2619-2c8c-4352-a960-f57512553a97")][global::OpenPCR.Migration("af368bb2-2ad8-4283-90b4-5b6ddbcb16b4")] public bool IsMiniRaidBattle => BattleCategory == (eBattleCategory)66;

		public bool IsMiniRaidExtraPhase { get; private set; }


		public global::System.Collections.Generic.Dictionary<Elements.eDamageEffectType, System.Collections.Generic.Dictionary<Elements.eDamageEffctTypeDetail, global::OpenPCR.Runtime.Api.GameObject>> DamageNumberEffectDictionary { get; set; }

		public class BonusEnemyData
		{
			[global::OpenPCR.Migration("b8a69395-54d2-4afd-be9c-c11797391d9b")]
			public BonusEnemyData()
			{
			}

			public int Id { get; set; }
			public List<(int, int)> SkillIdAndBonusIdList { get; set; } = new List<(int, int)>();
		}

		public enum eBonusType
		{
			DEFEAT_ENEMY,
			DOMINANCE_ENEMY,
			DEFEAT_ALLIES,
			DOMINANCE_ALLIES
		}

		public class BonusData
		{

		}

		private enum eIgnoreAllActionDoneState
		{
			NOT_RUNNING,
			RUNNING,
			STOP_PULSE
		}
		[SerializeField]
		private UITexture stageBgTex;
		[SerializeField]
		private UITexture stageForegroundTex;
		[SerializeField]
		private Transform unitParentTransform;
		[SerializeField]
		private Transform enemyParentTransform;
		[SerializeField]
		private Camera playCamera;
		[SerializeField]
		private Transform bgTransform;
		[SerializeField]
		private MotionBlur motionBlur;
		[SerializeField]
		private BlurOptimized playCameraBlurOptimized;
		private ViewBattle viewBattle;
		private BattleEffectManager battleEffectManager;
		private float deltaTimeAccumulated;
		private float cutinSkipTimeAccumulated;
		private bool isToPauseOnFrameEnd;
		private bool isToResumeOnFrameEnd;
		private bool idleonlyDone;

		private bool playerAllDead;
		private bool otherAllDead;

		private Vector3 playCameraPos0;
		private UnitCtrl voiceOnUnit;
		private bool isAdminChanging;
		private global::OpenPCR.Runtime.Api.Random.State tempRandomState;

		private List<UnitCtrl>[] allWaveEnemyUnitCtrlListArray;
		public UnitCtrl BossUnit;


		private bool isUpdateFrameExecuted;
		private TowerTempData towerTempData;
		private SpaceBattleTempData spaceBattleTempData;
		private ReplayTempData replayTempData;
		private bool isSkipCoroutineRunning;
		private float startRemainTime;
		private bool isPauseTimeLimit;
		public bool IsPlayCutin;

		private int noneVoiceCount;
		private int lastVoiceId;
		private eBattleClearType clearType;
		public float ActionStartTimeCounter;
		private int currentWaveOffset;
		private int suspendCount;







		[SerializeField]
		private UIPanel skillExeScreen;
		private bool isStarted;
		private float blackoutTime;
		private float blackoutTimeCounter;

		private Action onEndFadeOut;
		[SerializeField]
		private UITexture skillExeScreenTexture;
		private float startalpha;

		private bool isDefeatBonusOnly;
		private bool timeUpWin;
		[SerializeField]
		private UIPanel backgroundPanel;
		[SerializeField]
		private UIPanel foregroundPanel;
		[global::OpenPCR.Migration("4224a66a-9438-4bc4-a61a-27e4a162acd7")] private BaseBattleProcessor battleProcessor;
		private BattleLog battleLog;
		private BattleCameraEffect battleCameraEffect;
		private BattleEffectPool battleEffectPool;
		private BattleTimeScale battleTimeScale;
		private Yggdrasil<BattleManager> singletonTree;
		private BattleUnionBurstController battleUnionBurstController;
		private ISingletonNodeForInstanceEditing battleManagerNode;

		private bool forcePauseNoDialogUpdate;
		private StoryBattleData storyBattleDataPrefab;
		private GameObject focusObject;
		private CustomUIButton focusButton;
		[global::OpenPCR.ImplicitMigration("884184b2-0746-42b7-8949-48403ee17724")][global::OpenPCR.Migration("f0b0c358-6a09-45d3-abf7-cd24871ca48e")] public Vector3 PlayCameraPos0 => throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::get_PlayCameraPos0():OpenPCR.Runtime.Api.Vector3: runtime capability is unavailable.");

		private float specialBattleStartStoryCounter { get; set; }
		private Dictionary<int, List<FieldDataBase>> fieldDataDictionary { get; set; }
		public int ChangeSpeedOverwriteFieldCounter { get; private set; }


		public int CurrentEnvironmentId { get; set; }
		public long CurrentEnemyPoint { get; private set; }
		public int CurrentDefeatCount { get; private set; }
		public SpBattleShieldEffect SpBattleShieldEffect { get; set; }
		public List<int> KillOrder { get; private set; }
		public global::System.Int32 FrameCount { get; private set; }
		public eBattleGameState GameState { get; private set; }
		[global::OpenPCR.ImplicitMigration("2837d68d-7efb-487b-b557-d4562055d812")][global::OpenPCR.Migration("cdf47191-ccfd-46f8-98ed-c3d2dc3b8541")] public UnitUiCtrl UnitUiCtrl => viewBattle.UnitUiCtrl;

		[global::OpenPCR.Migration("2703b8bd-a95e-45bb-aaf5-909385a7f306")]
		public BattleDefineData BattleDefineData
		{
			[global::OpenPCR.Migration("68f9ee9d-4727-472f-a27a-1b102ab0cab0")]
			get
			{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::get_BattleDefineData():Elements.BattleDefineData (outside supported first-phase lifecycle/input scope)"); }
		}

		public float TimeLimit { get; set; }
		[global::OpenPCR.ImplicitMigration("586f4cac-c942-4c3c-95e9-356062a2e78a")][global::OpenPCR.Migration("68af79a8-46f7-4100-aba5-8ff328f28afb")] public float StartLimitTimeForDefense => battleProcessor.GetStartTimeLimitForDefense();
		[global::OpenPCR.ImplicitMigration("ab42ad00-8350-405f-ba73-05319a660895")][global::OpenPCR.Migration("313e461f-98c1-48b8-8b8c-d173d1042b8e")] public float TakeOverTimeForDefense => battleProcessor.GetTakeOverTimeForDefense();
		[global::OpenPCR.ImplicitMigration("009f8faf-1280-43c6-8d44-3752bc01fa7d")][global::OpenPCR.Migration("7a5f81ac-93b9-4904-98cf-a0a3c4435116")] public bool IsShowBossGauge => battleProcessor.IsShowBossGauge();
		[global::OpenPCR.ImplicitMigration("cbec07b9-a270-4306-9173-ba6db05eb395")][global::OpenPCR.Migration("e7fb4096-ea97-4e91-922a-a5278ff6a928")] private bool isTimeUp => TimeLimit <= 0f;
		public int CurrentWave { get; set; }
		public List<UnitCtrl> FadeOutUnitList { get; set; }
		public List<UnitCtrl> UnitList { get; set; }
		public List<UnitCtrl> EnemyList { get; set; }
		public UnitCtrl RelicEnemyUnit { get; private set; }
		public bool IsDefenseReplayMode { get; set; }
		public bool IsSortBackBossBattle { get; set; }
		public UnitCtrl DecoyUnit { get; set; }
		public UnitCtrl DecoyOther { get; set; }
		public UnitCtrl DecoyUnitFire { get; set; }
		public UnitCtrl DecoyOtherFire { get; set; }
		public eBattleCategory BattleCategory { get; private set; }
		public CoroutineManager CoroutineManager { get; private set; }
		public eChargeSkillTurn ChargeSkillTurn { get; set; }
		public UnitCtrl UnionBurstUseUnit { get; set; }
		public int ServerDeadFrame { get; set; }
		[global::OpenPCR.Migration("dc28a45e-6196-4843-9963-858cf342694a")] public int FrameRate { [global::OpenPCR.Migration("ff69757e-3577-4e7f-90c0-6a1eae3ec660")] get; [global::OpenPCR.Migration("27c0faa1-2df5-4811-96b0-78b36ebf9f95")] private set; }
		public float DeltaTime { get; private set; }

		[global::OpenPCR.Migration("aafe2afe-b5ad-4ee7-a4c7-c37189cc31c7")]
		public bool IsPlayingPauseEffect
		{
			[global::OpenPCR.Migration("bfceb63a-1645-4508-975d-e3270e5da066")]
			get
			{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::get_IsPlayingPauseEffect():System.Boolean (outside supported first-phase lifecycle/input scope)"); }
		}

		public List<long> ExecutedActionList { get; private set; }
		public eBattleResult BattleResult { get; private set; }
		public int PlayVoiceUnitId { get; set; }
		public int PlayVoiceIndex { get; set; }
		public bool PlayVoiceUnitIsRarity6 { get; set; }
		public int PlayVoiceGroupId { get; set; }
		public int PlayVoiceGroupUnitId { get; set; }
		public bool IsPauseFrameCount { get; set; }
		public bool IsPlayingNoCutInMotion { get; set; }
		public bool IsNoCutInMotionRightAfter { get; set; }
		public bool IsPlayingNoCutInMotionForPSkill { get; set; }
		public List<UnitCtrl> BlackoutUnitTargetList { get; private set; }
		public Dictionary<string, GameObject> ReActionEffectDictionary { get; private set; }
		public bool IsEnemyCreated { get; private set; }
		public List<RewardData> RewardList { get; private set; }
		public bool IsBossBattle { get; private set; }

		[global::OpenPCR.Migration("9b7e0001-0b87-4e46-bac3-e48d6dece803")]
		public bool IsClanBattle
		{
			[global::OpenPCR.Migration("1c7ae911-4ed8-4576-9dc4-f9cf76a5b0ba")]
			get
			{
				if (BattleCategory != eBattleCategory.CLAN_BATTLE)
				{
					return BattleCategory == eBattleCategory.REPLAY_CLAN_BATTLE;
				}

				return true;
			}
		}

		[global::OpenPCR.Migration("8baf1c63-20e2-4f5b-b06c-7f053419c0e0")]
		public bool IsDispTotalDamageBattle
		{
			[global::OpenPCR.Migration("ce6b35a0-0a5e-4251-bdd2-8c08870916f6")]
			get
			{
				uint category = unchecked((uint)BattleCategory);
				return category <= 48U && ((0x1800190100200UL >> (int)category) & 1UL) != 0;
			}
		}

		public long TotalBossDamage { get; set; }
		public long SekaiAccumulateDamage { get; set; }
		[global::OpenPCR.ImplicitMigration("bfeeb733-d578-4327-b2f8-457540fb1f4c")][global::OpenPCR.Migration("a5a123f4-afcc-4d53-8450-dd36a3ca73ca")] public bool IsHatsuneQuest => BattleCategory == eBattleCategory.HATSUNE_BATTLE;
		[global::OpenPCR.ImplicitMigration("ae6a12ea-3a7f-4deb-aab1-172173298d90")][global::OpenPCR.Migration("613469dd-0680-41ea-8739-d28c193e0392")] public bool IsHatsuneBoss => BattleCategory == eBattleCategory.HATSUNE_BOSS_BATTLE;

		[global::OpenPCR.Migration("990098e0-7d5f-4bcc-b5de-37fb04eaa2a6")]
		public bool IsHatsuneBattle
		{
			[global::OpenPCR.Migration("778983db-8b53-4f3f-8c55-ae0b0390f46a")]
			get
			{
				return BattleCategory == (eBattleCategory)10 || BattleCategory == (eBattleCategory)11 || BattleCategory == (eBattleCategory)16;
			}
		}

		[global::OpenPCR.ImplicitMigration("f1961a94-873e-4a29-807a-792f91299ca1")][global::OpenPCR.Migration("83780aec-1bf2-4006-947c-892dfa185df6")] public bool IsSpecialBattle => battleProcessor.IsSpecialBattle();

		[global::OpenPCR.Migration("de07546f-7465-484d-b0e9-a5a1cf48db72")]
		public bool IsEndlessBattle
		{
			[global::OpenPCR.Migration("de9fb8f4-95f9-4373-9e56-7ebb84c258da")]
			get
			{
				return BattleCategory == (eBattleCategory)49;
			}
		}

		[global::OpenPCR.ImplicitMigration("7254b1b6-8d6b-4881-8925-80efb1e597a2")][global::OpenPCR.Migration("e7832ada-74c2-498c-ac1a-23dd4b86a888")] public bool IsDefenseBattle => battleProcessor.IsDefenseBattle();
		[global::OpenPCR.ImplicitMigration("776625ad-bc17-4d8c-ade8-357b51b35aa6")][global::OpenPCR.Migration("dc2b8ee9-69e1-4090-97f0-7d3314570c62")] public bool IsDefenseBattleInMode => battleProcessor.IsDefenseBattleInMode();

		[global::OpenPCR.Migration("b1061f93-0833-4050-b5a0-a60fe8fae921")]
		public bool IsTowerContent
		{
			[global::OpenPCR.Migration("d0bed498-b780-4d48-931a-886e2b212138")]
			get
			{
				if (BattleCategory != eBattleCategory.TOWER && BattleCategory != eBattleCategory.TOWER_EX && BattleCategory != eBattleCategory.TOWER_REHEARSAL)
				{
					return BattleCategory == eBattleCategory.TOWER_CLOISTER;
				}

				return true;
			}
		}

		[global::OpenPCR.ImplicitMigration("6d1691b7-9510-4e0d-9777-fdd83ce2a54c")][global::OpenPCR.Migration("18548423-2b98-4eeb-a04c-42c39758015a")] public bool IsHighRarityQuest => throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::get_IsHighRarityQuest():System.Boolean (outside supported first-phase input/lifecycle scope)");

		[global::OpenPCR.Migration("81c24a03-c563-417a-a76b-0553569a4c32")]
		public bool IsTowerReplayContent
		{
			[global::OpenPCR.Migration("ebe6155c-30b9-4cde-96de-9775e3e2f1a0")]
			get
			{
				eBattleCategory battleCategory = BattleCategory;
				if ((uint)(battleCategory - 17) <= 1u || battleCategory == eBattleCategory.TOWER_CLOISTER_REPLAY || (uint)(battleCategory - 33) <= 1u)
				{
					return true;
				}

				return false;
			}
		}

		[global::OpenPCR.ImplicitMigration("89b78e6e-5ad8-44a9-bceb-ece861ee01ca")][global::OpenPCR.Migration("06f7a3c5-bc6a-4d80-b244-064a5afd7c32")] public bool IsDungeonSpecialBattle => BattleCategory == eBattleCategory.DUNGEON_SPECIAL;

		[global::OpenPCR.Migration("ead9b5ae-df33-4348-ac77-0d586727a8a7")]
		public bool IsMainStoryRaidBattle
		{
			[global::OpenPCR.Migration("bb160d76-6be0-4f04-9654-97a25ed18a3a")]
			get
			{
				if (BattleCategory != eBattleCategory.KAISER_BATTLE_MAIN && BattleCategory != eBattleCategory.LEGION_BATTLE_MAIN)
				{
					return BattleCategory == eBattleCategory.MIROKU_BATTLE;
				}

				return true;
			}
		}

		[global::OpenPCR.ImplicitMigration("dc678c9c-4d6e-4131-a60c-b373d0982a00")][global::OpenPCR.Migration("fbcf94f5-32db-40f9-af80-d9c78f98c13b")] public bool IsSubStoryRaidBattle => BattleCategory == eBattleCategory.LEGION_BATTLE_SUB;

		[global::OpenPCR.ImplicitMigration("57045210-c9ec-40fc-9609-9ed7f532d7d3")][global::OpenPCR.Migration("b2d779b8-15ef-4d83-9fa7-be99f2ab9c8d")] public bool IsSevenBoss => unchecked((uint)((int)BattleCategory - 57)) < 2u;
		[global::OpenPCR.ImplicitMigration("75da0dbd-fe93-4cf8-add0-3882d7550617")][global::OpenPCR.Migration("c4b0ddf8-7789-4565-82be-04c01b0c521b")] public bool IsAcnBossBattle => throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::get_IsAcnBossBattle():System.Boolean (outside supported first-phase input/lifecycle scope)");
		[global::OpenPCR.ImplicitMigration("7671a403-71f5-411c-8cc5-c70fbf90891f")][global::OpenPCR.Migration("28503b71-1745-4ad7-a73b-fc644f0e0076")] public bool IsAcnSpeicalBattle => throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::get_IsAcnSpeicalBattle():System.Boolean (outside supported first-phase input/lifecycle scope)");

		[global::OpenPCR.Migration("ab16c4dd-5dfc-4876-9c8b-21b5e45340ad")]
		public bool IsAcnUnknownBattle
		{
			[global::OpenPCR.Migration("568a67f6-86dd-4f75-8bc5-4f5658c20387")]
			get
			{
				return ((uint)BattleCategory & 0xfffffffeU) == 52U;
			}
		}

		public float MultiTimeEffectTimer { get; set; }
		private bool multiTargetEffectDone { get; set; }
		public bool IsPausingEffectSkippedInThisFrame { get; set; }
		public bool IsPrincessSkillMovieRunning { get; set; }
		public bool IsPrincessSkillMovieFirstHalfRunning { get; set; }
		public bool IsPrincessSkillRunning { get; set; }
		public bool IsSuspendLose { get; set; }
		public long StartBattleUnixTime { get; set; }

		[global::OpenPCR.Migration("d8fe49da-9aee-44eb-ade9-1bec8023ee14")]
		private bool isArenaContents
		{
			[global::OpenPCR.Migration("71380c5f-5b67-46eb-80ae-51c7b6b6712e")]
			get
			{
				if (!IsArenaBattle)
				{
					return isArenaReplay;
				}

				return true;
			}
		}

		[global::OpenPCR.Migration("b670e99d-bd12-42c8-b306-26502599f21c")]
		public bool IsArenaBattle
		{
			[global::OpenPCR.Migration("86a2392c-2086-47bb-aa63-190cd07459a6")]
			get
			{
				if (BattleCategory != eBattleCategory.ARENA)
				{
					return BattleCategory == eBattleCategory.GRAND_ARENA;
				}

				return true;
			}
		}

		[global::OpenPCR.Migration("858aa890-34c4-4962-8977-96718fe7b5b4")]
		private bool isArenaReplay
		{
			[global::OpenPCR.Migration("6c9f1f0c-0678-4307-9c8d-8f5dd7053be2")]
			get
			{
				if (BattleCategory != eBattleCategory.ARENA_REPLAY)
				{
					return BattleCategory == eBattleCategory.GRAND_ARENA_REPLAY;
				}

				return true;
			}
		}

		[global::OpenPCR.Migration("1b0447ae-6235-4c87-94e0-42603682e5ed")]
		public bool CanAllSetMode
		{
			[global::OpenPCR.Migration("4f8f4134-3d3d-436a-ac73-63dd6365f909")]
			get
			{
				if (TutorialManager.IsStartTutorial || isArenaContents || IsColosseumBattle || IsDomeBattle) return false;
				if (BattleCategory == eBattleCategory.BYWAY_QUEST && BywayQuestUtility.IsFixAutoBattle(Singleton<TempData>.Instance.SelectedQuestId)) return false;
				uint category = unchecked((uint)BattleCategory);
				return category > 24U || ((0xffff9fU >> (int)category) & 1U) != 0U;
			}
		}

		[global::OpenPCR.Migration("d25fcede-ffed-40c5-9be4-1ff1e1de1b06")]
		public bool CanReserveSkill
		{
			[global::OpenPCR.Migration("387da0c9-6235-4c76-8fd3-c6a3a4eb9256")]
			get
			{
				if (!CanAllSetMode)
					return false;
				uint category = unchecked((uint)BattleCategory);
				return category > 40U || ((0xF9EFB9DFFFUL >> (int)category) & 1UL) != 0;
			}
		}

		[global::OpenPCR.Migration("00407b65-c48d-432d-8e20-51865f5bf80a")]
		public bool IsColosseumBattle
		{
			[global::OpenPCR.Migration("09e9a1a1-12bc-494e-86e2-6e3360603574")]
			get
			{
				return unchecked((uint)((int)BattleCategory - 41)) < 2U;
			}
		}

		[global::OpenPCR.Migration("806f10e6-c850-43bf-9125-ca1f9f03e600")]
		public bool IsDomeBattle
		{
			[global::OpenPCR.Migration("75dd292c-06a8-4f29-bcad-9880fcbb804c")]
			get
			{
				return ((uint)BattleCategory & 0xfffffffeU) == 44U;
			}
		}

		[global::OpenPCR.Migration("6ec55939-2328-4e4a-889f-fa83b96c0472")]
		public bool IsLabyrinth
		{
			[global::OpenPCR.Migration("8be05df9-772d-4ad6-a8d4-cfd984bf6221")]
			get
			{
				return (uint)(BattleCategory - 59) < 2u;
			}
		}

		[global::OpenPCR.Migration("b7889829-296a-4306-aa8a-1604666d9985")]
		public bool IsReplayContents
		{
			[global::OpenPCR.Migration("a21e0bba-e959-4562-a5a4-49e3a0a70a4d")]
			get
			{
				return battleProcessor.IsReplay();
			}
		}

		[global::OpenPCR.Migration("6e61b232-12ad-4cd2-8d2e-8ead6f75b80d")]
		public bool CanQuadruple
		{
			[global::OpenPCR.Migration("5b4cbffc-d129-4886-b170-34ead754de97")]
			get
			{
				switch (BattleCategory)
				{
					case eBattleCategory.QUEST:
					case eBattleCategory.QUEST_REPLAY:
						return ManagerSingleton<MasterDataManager>.Instance.masterContentReleaseData.CheckReleaseContents(
							QuestDefine.IsUniqueQuest(tempData.SelectedQuestId) || QuestDefine.IsHighRarityQuest(tempData.SelectedQuestId)
								? eSystemId.EQUIPMENT_QUEST_QUADSPEED : eSystemId.QUEST_QUADSPEED);
					case eBattleCategory.TRAINING:
						return ManagerSingleton<MasterDataManager>.Instance.masterContentReleaseData.CheckReleaseContents(eSystemId.TRAINING_QUEST_QUADSPEED);
					case eBattleCategory.CLAN_BATTLE:
						return Singleton<ClanBattleTempData>.Instance.IsRehearsal && (int)settingBattleMaxSpeed == 4;
					case eBattleCategory.HATSUNE_BATTLE:
					case eBattleCategory.HATSUNE_BOSS_BATTLE:
					case eBattleCategory.HATSUNE_SPECIAL_BATTLE:
					case eBattleCategory.SEVEN_QUEST:
					case eBattleCategory.SEVEN_SCENARIO_BOSS:
					case eBattleCategory.SEVEN_SPECIAL_BOSS:
						throw new global::OpenPCR.OutOfBoundaryException("Hatsune/Seven content-release and selected boss qd_mode inputs are outside clan battle and training.");
					case eBattleCategory.DIMENSION_FAULT:
					case eBattleCategory.DIMENSION_FAULT_REHEARSAL:
					case eBattleCategory.DIMENSION_FAULT_TREASURE:
						return (int)settingBattleMaxSpeed == 4;
					case eBattleCategory.DUNGEON:
					case eBattleCategory.HIGH_RARITY:
					case eBattleCategory.FRIEND:
					case eBattleCategory.LEGION_BATTLE_SUB:
					case eBattleCategory.REPLAY_CLAN_BATTLE:
					case eBattleCategory.DUNGEON_SPECIAL:
					case eBattleCategory.TRIAL_BATTLE:
					case eBattleCategory.TRIAL_CLAN_BATTLE:
					case eBattleCategory.BYWAY_QUEST:
					case eBattleCategory.BYWAY_QUEST_REPLAY:
					case eBattleCategory.COLOSSEUM:
					case eBattleCategory.COLOSSEUM_REPLAY:
					case eBattleCategory.TALENT_QUEST:
					case eBattleCategory.DOME:
					case eBattleCategory.DOME_REPLAY:
					case eBattleCategory.ABYSS_BATTLE:
					case eBattleCategory.ABYSS_BOSS_BATTLE:
					case eBattleCategory.ABYSS_BOSS_BATTLE_REHEARSAL:
					case eBattleCategory.ACN_ENDLESS:
					case eBattleCategory.ACN_BOSS:
					case eBattleCategory.ACN_SPECIAL:
					case eBattleCategory.MIRAGE_FLOOR_QUEST:
					case eBattleCategory.MIRAGE_NEMESIS:
					case eBattleCategory.LABYRINTH:
					case eBattleCategory.LABYRINTH_EX:
					case eBattleCategory.MRE_BOSS:
						return true;
					default:
						if (IsTowerContent || IsTowerReplayContent) return (int)settingBattleMaxSpeed == 4;
						return isArenaContents;
				}
			}
		}

		[global::OpenPCR.Migration("df95b081-fdf3-49cd-bf65-a77eeff00487")]
		public bool IsTalentBonusEffectPlaying
		{
			[global::OpenPCR.Migration("3876d733-786e-48f4-bfd1-78459f2a2797")]
			get
			{
				return componentTalent.IsTalentBonusEffectPlaying;
			}
		}

		[global::OpenPCR.Migration("27581252-31f4-4ea6-9826-4ab75e6ad1ed")]
		public bool IsActiveUnitTalentFormationBonus
		{
			[global::OpenPCR.Migration("7421d226-f1ca-4253-ab78-06fbcb662ed9")]
			get
			{
				return UnitTalentFormationBonusDictionary.Any(pair => pair.Value > 0L);
			}
		}

		public bool IsReleasedKnightEnhance { get; private set; }

		private bool enemySetUp { get; set; }
		public int FieldIndex { get; set; }
		public List<UnitCtrl> BlackOutUnitList { get; private set; }
		public UnitCtrl SkillReadyUnit { get; set; }
		public UnitCtrl ChainVoicePlayUnit { get; set; }
		public bool HideDamageNumFlag { get; set; }
		private UnitCtrl skillExeUnitForBlackoutFadeout { get; set; }
		public bool CancelByAwake { get; set; }
		private int ubUsedCount { get; set; }
		public bool IsMultiTargetAppearPlaying { get; set; }




		public bool IsCurtain { get; set; }
		private bool isWinTeamFadeOutWaitMode { get; set; }
		private bool loseTeamfadeOutDone { get; set; }
		private bool allTeamFadeoutDone { get; set; }
		private bool ignoreAllActionDone { get; set; }
		private eIgnoreAllActionDoneState ignoreAllActionDoneCoroutineState { get; set; }
		public GameObject TotalDamageEffectPrefabLarge { get; set; }
		public GameObject TotalDamageEffectPrefabSmall { get; set; }
		public int UnitCount { get; set; }
		public bool BattleLogEnable { get; set; }
		public BattleVoiceFlagManager BattleVoiceFlagMgr { get; set; }
		[global::OpenPCR.Migration("9715869a-b673-4cc0-bdbd-aa6e0e9fed54")] public double DodgeTPRecoverRatio { [global::OpenPCR.Migration("00287d56-a1e4-45b7-bea6-a47ff13d62d7")] get; [global::OpenPCR.Migration("4abeb640-1cb6-455d-a995-726d5e423516")] private set; }
		private GameObject backgroundEffect { get; set; }

		[global::OpenPCR.Migration("5ffe48d8-0e01-449c-a68f-ee80230731ad")]
		public BattleLog BattleLog
		{
			[global::OpenPCR.Migration("7f87e948-c265-4842-b20c-c52578ea7db9")]
			get
			{
				return battleLog;
			}
		}

		public bool BattleReady { get; private set; }
		[global::OpenPCR.ImplicitMigration("53f8c0af-3e19-4949-894d-db1695d053bd")][global::OpenPCR.Migration("aeba26bf-f0fe-42c6-9341-8f140d5e2ff6")] public bool IsValid => isValid;
		public BattleAnimKitController MiddleGroundSpine { get; set; }
		public BattleAnimKitController ForeGroundSpine { get; set; }
		[global::OpenPCR.Migration("8f7b45cd-47b7-433e-8ffc-14738ad11421")] public int EnergyRecoveryRateMax { [global::OpenPCR.Migration("2b682287-7fa7-4409-87fb-9a4fb4a5d826")] get; [global::OpenPCR.Migration("4dc65cc9-3495-4df1-84aa-e68b6ae2e3de")] set; }

		[global::OpenPCR.Migration("a89e68ac-2228-4ad1-907a-bff8c549d344")]
		public long AllLogBarrierEffectValue
		{
			[global::OpenPCR.Migration("0e942def-cef7-4438-a7a9-bcacbc71d0e6")]
			get
			{
				return allLogBarrierEffectValue;
			}
		}

		[global::OpenPCR.Migration("4ebb609f-44c7-474f-8d99-262760154ede")]
		public long AllLogBarrierStartValue
		{
			[global::OpenPCR.Migration("87df2a1f-0cab-4837-97d8-bc7735f07ca7")]
			get
			{
				return allLogBarrierStartValue;
			}
		}

		[global::OpenPCR.Migration("592eb798-19ed-4948-bdca-fc5bddf04cf4")] public bool NewAllLogBarrierEnabled { [global::OpenPCR.Migration("7e78e895-bdb8-4445-b6f6-ea55910f7216")] get; [global::OpenPCR.Migration("8c3432a8-74f2-4bb4-a2fa-4004d2a98b12")] set; }

		[global::OpenPCR.Migration("5c1031c1-17b8-4ed1-a148-33616120467f")]
		public bool IsReleaseSlipDamageLogBarrier
		{
			[global::OpenPCR.Migration("c00517a7-cb73-42b4-834f-cd18e3522264")]
			get
			{
				return isReleaseSlipDamageLogBarrier;
			}
		}

		private bool allStandByDone { get; set; }
		private bool isPlayGuestCutin { get; set; }
		[global::OpenPCR.Migration("de44dfe9-e93a-4691-a689-0e203aec4a93")] public bool IsGuestCutinPlaying { [global::OpenPCR.Migration("e35ae0c5-e160-4254-977e-9ca419c133d7")] get; [global::OpenPCR.Migration("ea287f07-a8cc-4911-856b-32566950e621")] set; }
		public long AbsorberValue { get; set; }

		public bool MuteVoice { get; set; }
		public bool IsStartSkillExec { get; private set; }

		[global::OpenPCR.Migration("b03a1ddc-8ba2-4984-945f-ee6ff0b36f10")]
		public BattleManager()
		{
		}

		[global::OpenPCR.Migration("fbc25a5d-dc88-455f-b6e8-01807077ed33")]
		public void PlayBattleStart(Action hxAction)
		{
			bool quadruple = battleTimeScale.IsSpeedQuadruple;
			float rate = battleTimeScale.SpeedUpRate;
			this.Timer(quadruple ? 0.5f / rate : 0.5f, delegate
			{
				soundManager.PlaySe(eSE.BTL_BATTLE_START);
				if (!BootSystem.GetSkipBoot() && !dialogManager.IsUse(eDialogId.BATTLE_MENU))
				{
					if (MasterContentReleaseData.HasLock(999000))
					{
						dialogManager.OpenFlatoutPlayer(DialogFlatoutPlayer.FlatoutType.BATTLE_START, 0, 0, 0, 1f);
						return;
					}
					hxAction();
				}
			});
		}

		[global::OpenPCR.ImplicitMigration("faba200c-3478-44ae-bcd5-3725e410939e")][global::OpenPCR.Migration("97b92a5e-c7e2-459e-9e53-650a7089b803")] public bool IsAdminChanging => isAdminChanging;

		[global::OpenPCR.ImplicitMigration("15aebd4a-b46b-45ad-98f1-89e52240a6b3")][global::OpenPCR.Migration("f8bedb22-47bf-48c9-8470-89729b6252bf")] public bool IsUekTowerBattleContent => BattleCategory == eBattleCategory.UEK_BOSS_BATTLE;

		[global::OpenPCR.ImplicitMigration("fa346e7c-c55c-471c-8a61-8a391b09ac25")][global::OpenPCR.Migration("ff2080b2-5431-4fa8-8ced-1327f73ded30")] public bool IsSpaceBattle => BattleCategory == eBattleCategory.SPACE_BATTLE;

		[global::OpenPCR.Migration("e4f457bb-2301-435c-9c0a-194829fafe3f")]
		public UnitCtrl GetBossUnit()
		{
			return BossUnit;
		}

		[global::OpenPCR.Migration("90ffe634-b7f4-4efa-b1cd-669c6427b597")]
		public int GetStartRemainTime()
		{
			return OpenPCR.NativeNumeric.ToInt32(startRemainTime);
		}

		[global::OpenPCR.Migration("b81ee27f-affe-4570-8f24-bdd6f76047d3")]
		public int GetBattleElapsedTime()
		{
			return OpenPCR.NativeNumeric.ToInt32(startRemainTime - TimeLimit);
		}

		[global::OpenPCR.Migration("1faeefd9-dfb7-4011-b332-1c43797aa669")]
		public int GetMiliLimitTime()
		{
			return OpenPCR.NativeNumeric.ToInt32(TimeLimit * 1000f);
		}

		[global::OpenPCR.Migration("4325b5d9-69d4-4fac-9256-92d034867884")]
		public int GetRoundUpTimeLimit()
		{
			float timeLimit = TimeLimit;
			if (timeLimit - Mathf.Floor(timeLimit) > 0f)
				return unchecked(OpenPCR.NativeNumeric.ToInt32(timeLimit) + 1);
			return OpenPCR.NativeNumeric.ToInt32(timeLimit);
		}

		[global::OpenPCR.Migration("d33a1ece-934f-4b22-af66-2faf8fae51fb")]
		public void SetPauseTimeLimit(bool _flag)
		{
			isPauseTimeLimit = _flag;
		}

		[global::OpenPCR.Migration("42142830-e130-4f35-8547-9d07eb6f5a18")]
		public int GetUnitCtrlLength()
		{
			return UnitList.Count;
		}

		[global::OpenPCR.Migration("d0ee903f-eba1-4daa-a918-24f566f39491")]
		public UnitCtrl GetUnitCtrl(int _idx)
		{
			return UnitList[_idx];
		}

		[global::OpenPCR.Migration("ef5354b9-835b-4803-879b-92b947f2a419")]
		public bool IsShowEffectiveIcon(int _unitId)
		{ return global::OpenPCR.Presentation.Noop<bool>(); }

		[global::OpenPCR.Migration("554089f8-ec95-495b-8f6d-c4072b2dfe59")]
		public int GetEnemyCtrlLength()
		{
			return EnemyList.Count;
		}

		[global::OpenPCR.Migration("6969a91d-c37e-4c86-b62b-48f3aa26910d")]
		public UnitCtrl GetEnemyCtrl(int _idx)
		{
			return EnemyList[_idx];
		}

		[global::OpenPCR.Migration("75ead600-b91c-425f-8fe1-2b319c98be8b")]
		public UnitCtrl FindEnemy(int _id)
		{
			return EnemyList.Find((UnitCtrl JNOIHMMFADD) => JNOIHMMFADD.UnitId == _id);
		}

		[global::OpenPCR.Migration("bd8871c3-3760-4e16-91af-5090357f91f6")]
		public void SetPartsTalentWeakness(int _unitId, BasePartsData _partsData)
		{
			battleProcessor.SetPartsTalentWeakness(_unitId, _partsData);
		}

		[global::OpenPCR.Migration("89ce61cf-8fa1-42bc-84f7-622c5ce41667")]
		public bool GetIsPlayCutin()
		{
			return IsPlayCutin;
		}

		[global::OpenPCR.Migration("73c8e3ed-229c-4644-b5bf-70dc0167ef60")]
		public void SetBattleResult(eBattleResult result)
		{
			BattleResult = result;
		}

		[global::OpenPCR.Migration("746efe24-390a-4f35-9bc0-3742ce8bb764")]
		public int GetBlackoutUnitTargetLength()
		{
			return BlackoutUnitTargetList.Count;
		}

		[global::OpenPCR.Migration("8c222ba7-5120-44f9-bc81-d968691ff00d")]
		public UnitCtrl GetBlackoutUnitTarget(int _idx)
		{
			return BlackoutUnitTargetList[_idx];
		}

		[global::OpenPCR.Migration("d458d9cf-f318-4087-a069-37ce4b08e176")]
		public eBattleClearType GetAutoClearType()
		{
			return clearType;
		}

		[global::OpenPCR.Migration("38f6626b-14d3-49b3-944c-fcb93e21fffa")]
		public void InitOnlyAutoClearFlag(bool _isUseAllSet)
		{
			if (((@_isUseAllSet ? 1u : 0u) & 1u) == 0)
			{
				(this).clearType = unchecked((global::Elements.eBattleClearType)(1u));
				return;
			}
			else
			{
				(this).clearType = unchecked((global::Elements.eBattleClearType)(3u));
				return;
			}
		}

		[global::OpenPCR.Migration("1b855be4-a5d2-4e4d-bed2-5acc6f54e495")]
		public void DisableAutoClear()
		{
			clearType = eBattleClearType.MANUAL;
		}

		[global::OpenPCR.Migration("bb1e1fbd-495f-46ee-998d-5602d6921b91")]
		public bool NeedsSuspendAbuseWarning()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::NeedsSuspendAbuseWarning():System.Boolean (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("28efd599-f56c-48e1-b4c9-16c1cd5c6e07")]
		private void incrementWave(Action _finishCallback)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::incrementWave(System.Action):System.Void (outside first-phase wave boundary)"); }

		[global::OpenPCR.Migration("9c0c6196-87ca-4547-8183-f1beac75b2a4")]
		private void Update()
		{
			isUpdateFrameExecuted = false;
			if (IsSkipping())
			{
				if (!isSkipCoroutineRunning && !IsSuspendLose)
				{
					StartCoroutine(skipBattle());
				}

				return;
			}

			int i = 0;
			for (int num = UnitUiCtrl.UnitCtrls.Length; i < num; i++)
			{
				UnitCtrl unitCtrl = UnitUiCtrl.UnitCtrls[i];
				if (!(unitCtrl == null))
				{
					while (FrameCount < unitCtrl.CutInFrameSet.ServerCutInFrame)
					{
						updateFrameWithSkip(unitCtrl);
					}
				}
			}

			if (!SingletonMonoBehaviour<BattleHeaderController>.Instance.IsPaused && !forcePauseNoDialogUpdate)
			{
				cutinSkipTimeAccumulated += Time.deltaTime;
			}

			while (cutinSkipTimeAccumulated >= DeltaTime)
			{
				CoroutineManager.NoDialogUpdate();
				cutinSkipTimeAccumulated -= DeltaTime;
			}

			if (IsPrincessSkillMovieRunning)
			{
				unitSpineControllerList.ForEach(delegate (BattleAnimKitController ACFHIKDFIOJ)
				{
					ACFHIKDFIOJ.UpdateIndependentBattleSync();
				});
				unitSpineControllerList.ForEach(delegate (BattleAnimKitController ACFHIKDFIOJ)
				{
					ACFHIKDFIOJ.LateUpdateIndependentBattleSync();
				});
				return;
			}

			if (!IsPauseFrameCount)
			{
				deltaTimeAccumulated += Time.deltaTime;
			}

			do
			{
				if (IsPauseFrameCount && !CoroutineManager.VisualPause)
				{
					CoroutineManager.VisualPause = true;
					int num2 = 0;
					for (int count = UnitList.Count; num2 < count; num2++)
					{
						if (UnitList[num2] != voiceOnUnit)
						{
							UnitList[num2].ComponentSound.StopVoiceExceptUbVoice();
						}
					}

					int num3 = 0;
					for (int count2 = EnemyList.Count; num3 < count2; num3++)
					{
						if (EnemyList[num3] != voiceOnUnit)
						{
							EnemyList[num3].ComponentSound.StopVoiceExceptUbVoice();
						}
					}

					if (voiceOnUnit != null)
					{
						voiceOnUnit.ComponentSound.PauseSound(_isPause: false);
					}

					battleEffectManager.Pause();
				}

				if (IsPauseFrameCount || deltaTimeAccumulated < 0f)
				{
					break;
				}

				updateFrame();
				deltaTimeAccumulated -= DeltaTime;
			}
			while (!IsPrincessSkillMovieRunning && deltaTimeAccumulated >= DeltaTime);
			while (FrameCount < ServerDeadFrame && !IsPauseFrameCount)
			{
				updateFrame();
				if (IsPrincessSkillMovieRunning)
				{
					break;
				}
			}

			if (isToPauseOnFrameEnd && !isToResumeOnFrameEnd)
			{
				GamePause(_pause: true);
			}
			else if (!isToPauseOnFrameEnd && isToResumeOnFrameEnd)
			{
				GamePause(_pause: false);
			}

			if (!isUpdateFrameExecuted)
			{
				unitSpineControllerList.ForEach(delegate (BattleAnimKitController ACFHIKDFIOJ)
				{
					ACFHIKDFIOJ.UpdateIndependentBattleSync();
				});
				unitSpineControllerList.ForEach(delegate (BattleAnimKitController ACFHIKDFIOJ)
				{
					ACFHIKDFIOJ.LateUpdateIndependentBattleSync();
				});
			}
		}

		[global::OpenPCR.Migration("a39192c2-34b6-4e42-8155-bd69a3a501c1")]
		private void updateFrame()
		{
			bool flag = !isPauseTimeLimit && BlackOutUnitList.Count == 0;
			BattleHeaderController instance = SingletonMonoBehaviour<BattleHeaderController>.Instance;
			instance.PauseNoMoreTimeUp(BlackoutUnitTargetList.Count != 0);
			if (ActionStartTimeCounter > 0f)
			{
				ActionStartTimeCounter -= DeltaTime;
			}

			if (MultiTimeEffectTimer > 0f)
			{
				MultiTimeEffectTimer -= DeltaTime;
			}
			else if (!multiTargetEffectDone)
			{
				multiTargetEffectDone = true;
				int num = BossList.FindLastIndex((UnitCtrl JNOIHMMFADD) => JNOIHMMFADD.IsPartsBoss);
				for (int num2 = 0; num2 < BossList.Count; num2++)
				{
					UnitCtrl unitCtrl = BossList[num2];
					if (unitCtrl.IsPartsBoss)
					{
						unitCtrl.ComponentState.PlayAndSetUpMultiTarget(num2 == num);
					}
				}
			}

			if (specialBattleStartStoryCounter > 0f)
			{
				specialBattleStartStoryCounter -= DeltaTime;
			}
			else if (hasReservedTimeStartStory)
			{
				int storyId = battleProcessor.GetTimeStartStoryId();
				if (storyId == 0)
					IsPausingEffectSkippedInThisFrame = true;
				PlayStory(null, storyId, _end: false);
				hasReservedTimeStartStory = false;
			}

			if (flag)
			{
				if (IsDefenseBattle)
				{
					if (allStandByDone)
					{
						TimeLimit -= DeltaTime;
					}
				}
				else
				{
					TimeLimit -= DeltaTime;
				}

				instance.SetRestTime(TimeLimit);
				if (isTimeUp)
				{
					TimeLimit = 0f;
					isPauseTimeLimit = true;
					stopAllAbnormalEffect(UnitList);
					stopAllAbnormalEffect(EnemyList);
					appendWaveEndLog();
					SingletonMonoBehaviour<BattleHeaderController>.Instance.SetRestTime(TimeLimit);
					GamePause(_pause: true);
					if (ServerDeadFrame > 0)
					{
						ServerDeadFrame = FrameCount;
					}

					SingletonMonoBehaviour<BattleHeaderController>.Instance.gameObject.SetActive(value: false);
					UnitUiCtrl.HideSettingButtons();
					UnitUiCtrl.HidePopUp();
					GameState = eBattleGameState.WAIT_WAVE_END;
					StartCoroutine(waitActionWhile(() => tempData.BattleSkipState == eBattleSkipState.PREPARING_TO_SKIP, delegate
					{
						if (tempData.BattleSkipState == eBattleSkipState.SKIPPING)
						{
							finishBattle(eBattleResult.TIME_OVER);
						}
						else
						{
							this.Timer(0.5f, delegate
							{
								battleProcessor.PlayTimeup(delegate
								{
									finishBattle(eBattleResult.TIME_OVER);
								});
							});
						}
					}));
					return;
				}
			}

			FrameCount++;
			if (TimeLimit <= 0f)
			{
				if (BattleResult != eBattleResult.WIN)
					return;
			}
			else if (BattleResult == eBattleResult.LOSE && (int)BattleCategory == 49)
				return;
			UnitUiCtrl._Update(flag);
			eBattleGameState gameState = GameState;
			if (gameState == eBattleGameState.PLAY || gameState == eBattleGameState.WAIT_WAVE_END)
			{
				int num3 = 0;
				for (int count = UnitList.Count; num3 < count; num3++)
				{
					UnitList[num3]._Update();
				}

				for (int num4 = 0; num4 < EnemyList.Count; num4++)
				{
					EnemyList[num4]._Update();
				}

				if (RelicEnemyUnit != null)
					RelicEnemyUnit.ComponentRelicSkill.TryExec(getBattleElapsedFloatTime());
			}

			battleCameraEffect.UpdateShake();
			for (int num5 = effectUpdateList.Count - 1; num5 >= 0; num5--)
			{
				if (!effectUpdateList[num5]._Update())
				{
					effectUpdateList.RemoveAt(num5);
				}
			}

			if (FrameCount != 0 && UnitUiCtrl.UnitCtrls != null && EnemyList != null && !IsPausingEffectSkippedInThisFrame && !callStartCutIn())
			{
				CoroutineManager._Update();
			}

			unitSpineControllerList.ForEach(delegate (BattleAnimKitController ACFHIKDFIOJ)
			{
				ACFHIKDFIOJ.RealUpdate();
			});
			if (!IsSkipping())
			{
				unitSpineControllerList.ForEach(delegate (BattleAnimKitController ACFHIKDFIOJ)
				{
					ACFHIKDFIOJ.RealLateUpdate();
				});
			}

			battleUnionBurstController.TryExecUnionBurst();
			IsPausingEffectSkippedInThisFrame = false;
			isUpdateFrameExecuted = true;
		}

		[global::OpenPCR.Migration("c3d1a29b-08e5-4be7-b2de-8a4f1ee122f3")]
		private bool callStartCutIn()
		{
			if (GameState != eBattleGameState.PLAY)
			{
				return false;
			}

			if (updateMainUnit())
			{
				return true;
			}

			if (updateEnemyUnit())
			{
				return true;
			}

			return false;
		}

		[global::OpenPCR.Migration("17af3361-faae-4d4b-883c-4506bea4d898")]
		public void PauseAbnormalEffect()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::PauseAbnormalEffect():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("9bad8e99-552f-42b6-aea4-d22c4d82f35c")]
		public void ResumeAbnormalEffect()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::ResumeAbnormalEffect():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("5cf41ee6-98ae-44a5-8bf1-2a63064f1336")]
		public void SetSkillEffectSortOrderBack()
		{
			battleEffectManager.SetSkillEffectSortOrderBack();
		}

		[global::OpenPCR.Migration("0e4d2397-c02d-451f-9896-26f44a716bc4")]
		public void GamePause(bool _pause, bool _isBlackoutEnd = false)
		{
			if (_pause)
			{
				battleEffectManager.Pause();
			}
			else
			{
				battleEffectManager.Resume();
			}

			if (!IsPlayingNoCutInMotion)
			{
				setPauseAllUnit(_pause);
				if (!_isBlackoutEnd)
				{
					IsPauseFrameCount = _pause;
				}

				CoroutineManager.SystemPause = _pause;
				CoroutineManager.VisualPause = _pause;
				SingletonMonoBehaviour<BattleHeaderController>.Instance.SkillWindow.Pause(_pause);
				isToPauseOnFrameEnd = false;
				isToResumeOnFrameEnd = false;
			}
		}

		[global::OpenPCR.Migration("5108796a-b668-45a3-8030-eeb7b6e248d9")]
		public void PauseAllSe()
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("11c959d7-4021-44ad-9960-2d2b779bb51d")]
		public void FrameCountPause(UnitCtrl _unit)
		{
			voiceOnUnit = _unit;
			IsPauseFrameCount = true;
		}

		[global::OpenPCR.Migration("97e60eea-8947-4057-ab9a-740d5ddd4fdc")]
		private IEnumerator resumeFrameCount()
		{
			if (this == null || UnitUiCtrl == null)
			{
				yield break;
			}

			UnitUiCtrl.UpdateAllEnemyHp0();
			if (GameState != eBattleGameState.PLAY || UnitUiCtrl.AllEnemyHp0)
			{
				GamePause(_pause: false);
				yield break;
			}

			while (true)
			{
				bool flag = true;
				int i = 0;
				for (int count = UnitList.Count; i < count; i++)
				{
					if (UnitList[i].CutInFrameSet.ServerCutInFrame == -2)
					{
						flag = false;
					}
				}

				if (flag)
				{
					break;
				}

				yield return null;
			}

			IsPauseFrameCount = false;
		}

		[global::OpenPCR.Migration("c3e3ccc3-94d1-4dbc-9fbb-74452c10074e")]
		public void RetireBattle()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::RetireBattle():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("a4128868-a2bc-40c9-b5a0-7a1c36d56cb3")]
		public bool IsSkillExeUnit(UnitCtrl unitCtrl)
		{ return global::OpenPCR.Presentation.Noop<bool>(); }

		[global::OpenPCR.Migration("8d67798a-6c59-4a65-95fa-a81f3bf2ab28")]
		public void AddUnitSpineControllerList(BattleAnimKitController _unitSpineController)
		{
			unitSpineControllerList.Add(_unitSpineController);
		}

		[global::OpenPCR.Migration("9f76c490-5384-4a5c-8785-63c8ef267e10")]
		public void RemoveUnitSpineControllerList(BattleAnimKitController _unitSpineController)
		{
			if (unitSpineControllerList != null)
			{
				unitSpineControllerList.Remove(_unitSpineController);
			}
		}

		[global::OpenPCR.Migration("d01bb575-d9b9-443d-a610-240fea55d83c")]
		public void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit = null)
		{
			CoroutineManager.AppendCoroutine(cr, pauseType, unit);
		}

		[global::OpenPCR.Migration("0fd89cac-07d9-4375-bb27-5a887496974f")]
		public void StartCoroutineIgnoreFps(IEnumerator _cr)
		{
			StartCoroutine(updateIgnoreFps(_cr));
		}

		[global::OpenPCR.Migration("2f264a51-d58b-4de0-9277-8bb57065e1e9")]
		private IEnumerator updateIgnoreFps(IEnumerator _coroutine)
		{
			float deltaTimeAccumulated = 0f;
			while (true)
			{
				for (deltaTimeAccumulated += Time.deltaTime; deltaTimeAccumulated > 0f; deltaTimeAccumulated -= DeltaTime)
				{
					if (!_coroutine.MoveNext())
					{
						yield break;
					}
				}

				yield return null;
			}
		}

		[global::OpenPCR.Migration("73449e4e-43bf-4e0c-8e5e-e67ff5cd7234")]
		public void RemoveAllCoroutines()
		{
			StopAllCoroutines();
			CoroutineManager.RemoveAll();
		}

		[global::OpenPCR.Migration("34973b3e-a3bf-467e-abc0-b9c994f4a1cd")]
		public void AppendEffect(SkillEffectCtrl _skillEffectCtrl, UnitCtrl _unit = null, bool _isAbnormal = false)
		{
			battleEffectManager.AppendEffect(_skillEffectCtrl, _unit, _isAbnormal);
		}

		[global::OpenPCR.Migration("12d2fd94-8ac7-4399-ba42-f8c06ad72fbf")]
		public void AddEffectToUpdateList(SkillEffectCtrl ef)
		{
			if (!effectUpdateList.Contains(ef))
			{
				effectUpdateList.Add(ef);
			}
		}

		[global::OpenPCR.Migration("32b52b94-f665-46b8-af99-6b83bf9154f0")]
		public void RemoveEffectToUpdateList(SkillEffectCtrl ef)
		{
			if (effectUpdateList != null)
			{
				effectUpdateList.Remove(ef);
			}
		}

		[global::OpenPCR.Migration("fac1f28b-bb77-4c7c-ae23-727e0b2bb43f")]
		public static float Random(float _min, float _max)
		{
			return (float)Random(OpenPCR.NativeNumeric.ToInt32(_min * 1000f), OpenPCR.NativeNumeric.ToInt32(_max * 1000f)) / 1000f;
		}

		[global::OpenPCR.Migration("72204cf5-4ad0-4389-b41c-7ef89450af61")]
		public static int Random(int _min, int _max)
		{
			if (_min == _max)
			{
				return _min;
			}

			return global::OpenPCR.Runtime.Api.Random.Range(_min, _max);
		}

		[global::OpenPCR.Migration("41496c00-e98b-445a-b6ce-46f17602a76d")]
		public static int HeldRandom(int _min, int _max)
		{
			if (_min == _max)
			{
				return _min;
			}

			global::OpenPCR.Runtime.Api.Random.State state = global::OpenPCR.Runtime.Api.Random.state;
			int result = global::OpenPCR.Runtime.Api.Random.Range(_min, _max);
			global::OpenPCR.Runtime.Api.Random.state = state;
			return result;
		}

		[global::OpenPCR.Migration("ff4aa874-f0ba-4b3e-82e1-1ca307fedc2c")]
		public void CreateUnit(int _unitId, Action<UnitCtrl> _callback, bool _addUnitList = true)
		{
			_ = ManagerSingleton<ResourceManager>.Instance;
			Singleton<BattleUnitLoader>.Instance.AddLoadResource(_unitId, delegate (GameObject EHONOEKJKAA)
			{
				UnitCtrl component = EHONOEKJKAA.GetComponent<UnitCtrl>();
				if (_addUnitList)
				{
					UnitList.Add(component);
				}
				_callback.Call(component);
			}, _isWaveEnemy: false, _isInstantiate: true);
		}

		[global::OpenPCR.Migration("54d1b59e-6335-4987-801d-79ab43064df2")]
		public void CreateUnitSpine(UnitCtrl _unit, eUnitRespawnPos _respawnPos, UnitParameter _pParam, Action<UnitCtrl> _callback = null, bool _isOther = false)
		{
			if (_unit.IsGuest)
			{
				global::System.Int32 unit_id = ManagerSingleton<MasterDataManager>.Instance.masterEnemyParameter.GetFromAllKind(_pParam.UniqueData.Id).unit_id;
				isPlayGuestCutin = ResourceManager.IsExistResource(eResourceId.ANIM_GUEST_CUT_IN, (int)unit_id);
			}
			int unitId = _pParam.MasterData.PrefabId;
			int setSkinNo = UnitUtility.GetSetSkinNo(_pParam.UniqueData, UnitDefine.eSkinType.Sd, _isOther, _unit);
			int prefabId = _pParam.MasterData.prefab_id_battle;
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(_unit.transform.TargetTransform, worldPositionStays: false);
			gameObject.name = "rotate_center";
			_unit.RotateCenter = gameObject.transform;
			BattleAnimKitController.LoadCreate((!IsDefenseReplayMode) ? eAnimKitType.SD_NORMAL_BOUND : eAnimKitType.SD_SHADOW_BOUND, unitId, setSkinNo, prefabId, gameObject.transform, delegate (BattleAnimKitController JPBMABCKONG)
			{
				if (IsDefenseReplayMode)
				{
					_unit.IsShadow = true;
				}
				JPBMABCKONG.Owner = _unit;
				if (!UnitUtility.JudgeIsGuest(unitId))
				{
					JPBMABCKONG.LoadAnimationIDImmediately(eAnimKitBinaryAnimationId.COMMON_BATTLE);
				}
				JPBMABCKONG.LoadAnimationIDImmediately(eAnimKitBinaryAnimationId.BATTLE);
				_unit.CenterBone = JPBMABCKONG.skeleton.FindBone("Center");
				_unit.StateBone = JPBMABCKONG.skeleton.FindBone("State");
				JPBMABCKONG.SetAnimeEventDelegateForBattle(delegate
				{
					JPBMABCKONG.IsStopState = true;
				}, -1);
				_unit.transform.parent = unitParentTransform;
				TransformUtility.ResetLocal(_unit.transform);
				_unit.transform.localPosition = new Vector3(-1400f, GetRespawnPos(_respawnPos), 0f);
				battleProcessor.AddUnitSkillUseCount(ref _unit.SkillUseCount, unitId);
				_unit.Initialize(_pParam, false, true, false, UnitExtraEnhanceInfo, JPBMABCKONG, cutInPlaybackType, SummonAction.eSummonType.NONE);
				_unit.BattleStartProcess(_respawnPos);
				_callback.Call(_unit);
			}, 0);
		}

		[global::OpenPCR.Migration("8e4214af-0d93-43d2-a247-42a9df72f6b0")]
		private void appendWaveEndLog()
		{
			Action<List<UnitCtrl>> action = delegate (List<UnitCtrl> MMGJMMGDPGM)
			{
				if (MMGJMMGDPGM.Count > 0)
				{
					for (int i = 0; i < MMGJMMGDPGM.Count; i++)
					{
						if (MMGJMMGDPGM[i] != null && MMGJMMGDPGM[i].ComponentDamageControl.UnitDamageInfo != null)
						{
							battleLog.AppendBattleLog(eBattleLogType.WAVE_END_HP, 0, _source: MMGJMMGDPGM[i], _target: MMGJMMGDPGM[i], _value: MMGJMMGDPGM[i].ComponentParameter.Hp, _currentValue: 0L, _duration: 0, _actionId: 0);
							battleLog.AppendBattleLog(eBattleLogType.WAVE_END_DAMAGE_AMOUNT, 0, _source: MMGJMMGDPGM[i], _target: MMGJMMGDPGM[i], _value: MMGJMMGDPGM[i].ComponentDamageControl.UnitDamageInfo.damage, _currentValue: 0L, _duration: 0, _actionId: 0);
						}
					}
				}
			};
			action(UnitList);
			action(EnemyList);
			battleLog.AppendBattleLog(eBattleLogType.SUSPEND_COUNT, 0, suspendCount, 0L, 0, 0);
			suspendCount = 0;
		}

		[global::OpenPCR.Migration("8553032a-d680-4679-a614-27144fd6d745")]
		public void NextWaveProcess()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::NextWaveProcess():System.Void (outside first-phase wave boundary)"); }

		[global::OpenPCR.Migration("2b9798dc-36f4-455b-b5ce-f83d5a67371f")]
		private void onBossStateChange(eStateIconType _iconType, bool _enable)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("66f18e1c-642e-4c20-bba5-d250de527541")]
		private void onBossStateNumChange(eStateIconType _iconType, int _num)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("84d8003d-cdc4-4b1a-b8f8-374e3bbb6456")]
		private void onBossStatePassiveEnableChange(eStateIconType _iconType, bool _enable)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("26f2fc6f-311f-442f-9f1c-a4a95f172b93")]
		private void sortAndSetPositionOneWave(Dictionary<int, UnitCtrl> _pEnemyDataDic)
		{
			List<UnitCtrl> list = new List<UnitCtrl>(_pEnemyDataDic.Values);
			int count = list.Count;
			float[] array = new float[count];
			int[] array2 = new int[count];
			for (int i = 0; i < count; i++)
			{
				UnitCtrl unitCtrl = list[i];
				array[i] = unitCtrl.ComponentSearchArea.SearchAreaSize;
				array2[i] = i;
			}

			battleProcessor.SortPosition(list, array, array2);
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int num = 0;
			EnemyList.Clear();
			for (int j = 0; j < count; j++)
			{
				num = list[array2[j]].UnitId;
				if (dictionary.ContainsKey(num))
				{
					int key = num;
					int value = dictionary[key] + 1;
					dictionary[key] = value;
				}
				else
				{
					dictionary.Add(num, 0);
				}

				float overlapPos = 100f * (float)dictionary[num];
				UnitCtrl unitCtrl2 = list[array2[j]];
				unitCtrl2.SetOverlapPos(overlapPos);
				if (IsPauseFrameCount)
				{
					unitCtrl2.Pause = true;
				}

				EnemyList.Add(unitCtrl2);
			}
		}

		[global::OpenPCR.Migration("2d7d057a-5897-45c5-b0b1-c859cafbc753")]
		private void setupEnemyProcess()
		{
			if (EnemyList.FindIndex((UnitCtrl JNOIHMMFADD) => JNOIHMMFADD.BossSortIsBack) != -1)
			{
				IsSortBackBossBattle = true;
			}
			int normalEnemyIndex = 0;
			for (int num = 0; num < EnemyList.Count; num++)
			{
				UnitCtrl unitCtrl = EnemyList[num];
				// onDieEnemy gold/treasure presentation excluded; core death state remains above.
				bool bonus = unitCtrl.IsBonusEnemy;
				battleProcessor.SetupEnemyStartProcess(unitCtrl, bonus ? 0 : normalEnemyIndex);
				if (!bonus) normalEnemyIndex++;
			}
			// Boss gauge/icon presentation is excluded; enemy setup and load-completion callback stay real.
			enemySetUp = false;
			resourceManager.StartLoad(delegate
			{
				enemySetUp = true;
			});
			CoroutineManager.ReorderByEnemyList(ePauseType.SYSTEM, EnemyList);
		}

		[global::OpenPCR.Migration("e9f7b9f8-64d2-4a07-b962-50e81dd38209")]
		public bool IsDisplayBossHpUi()
		{ return global::OpenPCR.Presentation.Noop<bool>(); }

		[global::OpenPCR.Migration("19453ad6-fd66-4483-a704-3e853b5dd94e")]
		public SpecialBattleData GetSpecialBattleData()
		{
			return battleProcessor.GetSpecialBattleData();
		}

		[global::OpenPCR.Migration("14e3366f-fceb-44e7-ac53-5e4982221678")]
		public bool IsBossInfinityHpBattle()
		{
			return battleProcessor.IsBossInfinityHpBattle();
		}

		[global::OpenPCR.Migration("0b69ed11-b4ab-42fc-a404-b9785b9d615b")]
		public bool IsBossInfinityHpMode()
		{
			return battleProcessor.IsBossInfinityHpMode();
		}

		[global::OpenPCR.Migration("06d916a3-4f8d-463d-bb46-8f3f5aa29e77")]
		public long GetInvalidLifeAmount(long _maxHp)
		{
			return battleProcessor.GetInvalidLifeAmount(_maxHp);
		}

		[global::OpenPCR.Migration("61404c86-725c-4714-936f-cc87ab6a4bae")]
		private void setPauseAllUnit(bool _pause)
		{
			Action<List<UnitCtrl>> action = delegate (List<UnitCtrl> JCLJGFKMBJM)
			{
				int i = 0;
				for (int count = JCLJGFKMBJM.Count; i < count; i++)
				{
					UnitCtrl unitCtrl = JCLJGFKMBJM[i];
					if (!_pause && BlackOutUnitList.Count > 0)
					{
						if (BlackOutUnitList.Contains(unitCtrl))
						{
							unitCtrl.Pause = _pause;
							unitCtrl.ComponentSound.PauseVoice(_pause);
						}
					}
					else
					{
						unitCtrl.Pause = _pause;
						unitCtrl.ComponentSound.PauseVoice(_pause);
					}
				}
			};
			action(UnitList);
			action(EnemyList);
		}

		[global::OpenPCR.Migration("53b2beae-6721-46e3-9992-d5428f2bb602")]
		private void checkBossBattle(Dictionary<int, UnitCtrl> _dic)
		{
			IsBossBattle = false;
			List<UnitCtrl> list = new List<UnitCtrl>(_dic.Values);
			for (int i = 0; i < list.Count; i++)
			{
				if (UnitUtility.JudgeIsBoss(list[i].UnitId))
				{
					IsBossBattle = true;
					break;
				}
			}
		}

		[global::OpenPCR.Migration("c6da8c60-f101-4583-95e6-61a49466cd45")]
		private bool updateMainUnit()
		{
			int i = 0;
			for (int num = UnitUiCtrl.UnitCtrls.Length; i < num; i++)
			{
				UnitCtrl unitCtrl = UnitUiCtrl.UnitCtrls[i];
				if (unitCtrl == null)
				{
					continue;
				}

				if (FrameCount == unitCtrl.CutInFrameSet.CutInFrame)
				{
					var unionBurst = unitCtrl.ComponentUnionBurst;
					if (!unionBurst.CheckIsSkillReady())
					{
						unitCtrl.CutInFrameSet.CutInFrame = -1;
						unitCtrl.CutInFrameSet.ServerCutInFrame = -1;
						ChargeSkillTurn = eChargeSkillTurn.NONE;
						return false;
					}

					unionBurst.StartCutIn();
					return true;
				}

				if (FrameCount == unitCtrl.CutInFrameSet.ServerCutInFrame + 1)
				{
					onSyncEnd(unitCtrl);
					if (unitCtrl.ComponentUnionBurst.ConsumeEnergy() == eConsumeResult.FAILED || GameState != eBattleGameState.PLAY)
					{
						unitCtrl.CutInFrameSet.ServerCutInFrame = -3;
					}
					else
					{
						unitCtrl.ComponentState.SetState(ActionState.UNION_BURST);
					}

					HideDamageNumFlag = false;
				}

				if (unitCtrl.CutInFrameSet.ServerCutInFrame == -3)
				{
					HideDamageNumFlag = false;
					unitCtrl.CutInFrameSet.CutInFrame = -1;
					unitCtrl.CutInFrameSet.ServerCutInFrame = -1;
					OnBlackOutEnd(unitCtrl);
				}
			}

			return false;
		}

		[global::OpenPCR.Migration("42f512b9-4802-4750-9e84-f0e779a40fc4")]
		private bool updateEnemyUnit()
		{
			int i = 0;
			for (int count = EnemyList.Count; i < count; i++)
			{
				UnitCtrl unitCtrl = EnemyList[i];
				if (unitCtrl == null)
				{
					continue;
				}

				if (FrameCount == unitCtrl.CutInFrameSet.CutInFrame)
				{
					var unionBurst = unitCtrl.ComponentUnionBurst;
					if (!unionBurst.CheckIsSkillReady())
					{
						unitCtrl.CutInFrameSet.CutInFrame = -1;
						unitCtrl.CutInFrameSet.ServerCutInFrame = -1;
						ChargeSkillTurn = eChargeSkillTurn.NONE;
						return false;
					}

					unionBurst.StartCutIn();
					return true;
				}

				if (FrameCount == unitCtrl.CutInFrameSet.CutInFrame + 1)
				{
					onSyncEnd(unitCtrl);
					HideDamageNumFlag = false;
					if (unitCtrl.ComponentUnionBurst.ConsumeEnergy() == eConsumeResult.FAILED)
					{
						unitCtrl.CutInFrameSet.CutInFrame = -1;
						unitCtrl.CutInFrameSet.ServerCutInFrame = -1;
						OnBlackOutEnd(unitCtrl);
					}
					else
					{
						unitCtrl.ComponentState.SetState(ActionState.UNION_BURST);
					}
				}

				if (unitCtrl.CutInFrameSet.ServerCutInFrame == -3)
				{
					HideDamageNumFlag = false;
					unitCtrl.CutInFrameSet.CutInFrame = -1;
					unitCtrl.CutInFrameSet.ServerCutInFrame = -1;
					OnBlackOutEnd(unitCtrl);
				}
			}

			return false;
		}

		[global::OpenPCR.Migration("6fccba76-c39b-4c1e-9e44-380392a67fe5")]
		private void onSyncEnd(UnitCtrl _unit)
		{
			_unit.ComponentSortOrder.SetSortOrderFront();
			setPauseAllUnit(_pause: true);
		}

		[global::OpenPCR.Migration("4db801f2-9b2b-4e2b-92d8-376c244daec2")]
		public void DisplayUnitFrontOfDialog()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::DisplayUnitFrontOfDialog():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("44d4fa64-d00e-42c9-b169-ea8adf7ac219")]
		public void DisplayEnemyFrontOfDialog()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::DisplayEnemyFrontOfDialog():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("855f4291-ac86-4019-ac7c-a3eff9ab9620")]
		public IEnumerator UpdateZoom(float _scale, float _posY)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::UpdateZoom(System.Single,System.Single):System.Collections.IEnumerator (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("a1c6ab7e-ed61-4e9e-8932-5b13f578367d")]
		public void DisplayAliveUnitFront()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::DisplayAliveUnitFront():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("da97d375-fe05-47da-a7d5-81a9b1ce9971")]
		public void RunOutBattleResult()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::RunOutBattleResult():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("95615afc-5505-4562-a475-b3b1180ea83f")]
		public IEnumerator RecreateDeadUnits(Action _callback, eBattleResult _battleResult)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::RecreateDeadUnits(System.Action,Elements.eBattleResult):System.Collections.IEnumerator (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("703ffa44-2eef-4e63-bc13-036db962cf07")]
		private void updateFrameWithSkip(UnitCtrl _unitCtrl)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::updateFrameWithSkip(Elements.UnitCtrl):System.Void (outside first-phase cutin boundary)"); }

		[global::OpenPCR.Migration("a87bf067-a124-41db-88cb-47dff5f3d84e")]
		public void PlayBattleStart()
		{
			bool isSpeedQuadruple = battleTimeScale.IsSpeedQuadruple;
			float speedUpRate = battleTimeScale.SpeedUpRate;
			speedUpRate = (isSpeedQuadruple ? speedUpRate : 1f);
			this.Timer(0.5f / speedUpRate, delegate
			{
				soundManager.PlaySe(eSE.BTL_BATTLE_START);
				if (!BootSystem.GetSkipBoot() && !dialogManager.IsUse(eDialogId.BATTLE_MENU))
				{
					StartFlatoutEffect(DialogFlatoutPlayer.FlatoutType.BATTLE_START);
				}
			});
		}

		[global::OpenPCR.Migration("772625c7-d319-4bfa-bf4b-5feb800a35f2")]
		public int GetLatestClearStarNum()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::GetLatestClearStarNum():System.Int32 (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("ec962591-9e56-45eb-8adb-19127b2063d6")]
		public List<UnitCtrl> GetAliveUnitList()
		{
			return UnitList.FindAll((UnitCtrl IMDKENFNOMF) => !IMDKENFNOMF.ComponentState.IsDead && !IMDKENFNOMF.IsSummonOrPhantom);
		}

		[global::OpenPCR.Migration("e897c026-817c-4d4a-962a-2b095facf345")]
		public IEnumerator CallTimer(float _time, Action _onComplete)
		{
			return ExtensionMethods.Timer(this, _time, _onComplete);
		}

		[global::OpenPCR.Migration("7aed9f3b-2a82-4b6f-b4c7-38a1d4492aaa")]
		public IEnumerator CallFade(float _from, float _to, float _duration, Action<float> _setFade, Action _onFinish)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::CallFade(System.Single,System.Single,System.Single,System.Action\u00601\u003CSystem.Single\u003E,System.Action):System.Collections.IEnumerator (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("fce25b9d-2814-486e-885e-c70ea17c5211")]
		public int GetCurrentTowerExPartyIndex()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::GetCurrentTowerExPartyIndex():System.Int32 (outside first-phase tower-index boundary)"); }

		[global::OpenPCR.Migration("f1e46225-a49b-4a4b-a6ad-1c052dba4470")]
		public UnitCtrl FindUnitForSoundUnitId(int _soundUnitId)
		{
			return UnitList.Find((UnitCtrl BHGHCPANDJB) => BHGHCPANDJB.ComponentSound.SoundUnitId == _soundUnitId);
		}

		[global::OpenPCR.Migration("c83602de-e8c2-4955-8064-80f147ce14be")]
		public UnitCtrl FindEnemyForResourceId(int _resourceId)
		{
			return EnemyList.Find((UnitCtrl BHGHCPANDJB) => UnitUtility.GetUnitResourceId(BHGHCPANDJB.UnitId) == _resourceId);
		}

		[global::OpenPCR.Migration("e36a15ac-1c94-401d-b5db-8007929fba1b")]
		public void IncrementNoneVoiceAttackCount()
		{
			noneVoiceCount++;
		}

		[global::OpenPCR.Migration("a8b1839b-e0ce-44ff-a735-caab44b5b425")]
		public bool JudgeVoicePlay(int _unitId, int _maxNum, float _voiceOffsetRate)
		{
			int num = UnitList.FindAll((UnitCtrl JNOIHMMFADD) => (long)JNOIHMMFADD.ComponentParameter.Hp > 0 && !JNOIHMMFADD.IsSummonOrPhantom).Count;
			bool flag = UnitList.Any((UnitCtrl JNOIHMMFADD) => JNOIHMMFADD.IsGuest && (long)JNOIHMMFADD.ComponentParameter.Hp > 0);
			if (num == 1 || (flag && num <= 2))
			{
				return true;
			}

			if (lastVoiceId != 0)
			{
				num--;
			}

			bool flag2 = false;
			flag2 = (float)HeldRandom(0, Mathf.Max(num - noneVoiceCount, 1) * 1000) <= ((float)_maxNum - _voiceOffsetRate) * 1000f;
			if (_unitId != lastVoiceId && ((num == 0) | flag2))
			{
				lastVoiceId = _unitId;
				noneVoiceCount = 0;
				return true;
			}

			noneVoiceCount++;
			return false;
		}

		[global::OpenPCR.Migration("653b36e1-1697-4bf3-9271-4dab1110346c")]
		public void TurnOffAllEffects()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::TurnOffAllEffects():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("12f1ffa3-1037-4a19-b696-c8039122b81c")]
		public void OffSkillExeScreen()
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("1d982126-977e-40c6-a05f-df14cf3a9bf1")]
		public void ExecAllUnitSealDamageCut(AllUnitSealDamageCutData _data, bool _enemySide)
		{
			int priority = allUnitSealDamageCutCounter++;
			_data.Priority = priority;
			if (_enemySide)
			{
				if (enemyAllUnitSealDamageCutDataDic.ContainsKey(_data.IconType))
					enemyAllUnitSealDamageCutDataDic[_data.IconType].Clear();
				enemyAllUnitSealDamageCutDataDic[_data.IconType] = _data;
			}
			else
			{
				if (unitAllUnitSealDamageCutDataDic.ContainsKey(_data.IconType))
					unitAllUnitSealDamageCutDataDic[_data.IconType].Clear();
				unitAllUnitSealDamageCutDataDic[_data.IconType] = _data;
			}

			_data.Start();
		}

		[global::OpenPCR.Migration("ef58c3c7-300e-42a8-905a-e5cd31e80ce5")]
		public void ExecField(FieldDataBase _fieldData, int _actionId)
		{
			if (fieldDataDictionary.ContainsKey(_actionId))
			{
				fieldDataDictionary[_actionId].Add(_fieldData);
			}
			else
			{
				fieldDataDictionary.Add(_actionId, new List<FieldDataBase> { _fieldData });
			}

			_fieldData.AllTargetList = ((_fieldData.FieldTargetType == eFieldTargetType.ENEMY) ? EnemyList : UnitList);
			_fieldData.StartField();
		}

		[global::OpenPCR.Migration("0891097c-14cd-4fc6-8ac9-ed53bafa9564")]
		public void ExecEnvironment(EnvironmentData _environmentData, bool _isEnemySide)
		{
			EnvironmentData current = _isEnemySide ? enemyEnvironment : unitEnvironment;
			if (current != null && current.Enable)
			{
				if (current.EnvironmentType == _environmentData.EnvironmentType)
				{
					current.ResetTimer();
					return;
				}

				current.StopEnvironmentImmediately();
			}

			if (_isEnemySide)
				enemyEnvironment = _environmentData;
			else
				unitEnvironment = _environmentData;
			_environmentData.UnitList = UnitList;
			_environmentData.EnemyUnitList = EnemyList;
			_environmentData.StartEnvironment();
			SingletonMonoBehaviour<BattleHeaderController>.Instance.AppearEnvironmentIcon(_isEnemySide, _environmentData.EnvironmentType);
		}

		[global::OpenPCR.Migration("11fe560c-bc61-4abb-9fb6-bf18482be895")]
		public int GetEnvironmentId(bool _isEnemy)
		{
			EnvironmentData environment = _isEnemy ? enemyEnvironment : unitEnvironment;
			return environment != null && environment.Enable ? environment.EnvironmentType : -1;
		}

		[global::OpenPCR.Migration("1f52f0ae-5bed-432f-bde9-a5316b5e1ff7")]
		public void SetEnvironmentResume(bool _isEnemy)
		{
			EnvironmentData environment = _isEnemy ? enemyEnvironment : unitEnvironment;
			if (environment != null && environment.Enable && environment.WaitingAnotherEnvironmentEnd)
			{
				environment.WaitingAnotherEnvironmentEnd = false;
				environment.InitializeSkillEffect();
			}
		}

		[global::OpenPCR.Migration("1a5310db-a62a-4b8d-b000-7d9049d36a7e")]
		public bool ExistsEnvironment(int _id)
		{
			return GetEnvironmentId(true) == _id || GetEnvironmentId(false) == _id;
		}

		[global::OpenPCR.Migration("a90cdef6-758a-4a86-94a4-c80ffce54922")]
		public void StopField(int _actionId, eTargetAssignment _targetAssignment, bool _isOther)
		{
			Dictionary<int, List<FieldDataBase>>.Enumerator enumerator = fieldDataDictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				int key = enumerator.Current.Key;
				for (int i = 0; i < fieldDataDictionary[key].Count; i++)
				{
					if (_actionId == 0 || _actionId == key)
					{
						fieldDataDictionary[key][i].StopField(_targetAssignment, _isOther);
					}
				}
			}
		}

		[global::OpenPCR.Migration("6d14032e-26e3-421d-a281-44ce945c145c")]
		public void RestartAbnormalStateField(UnitCtrl _unit, eAbnormalState _abnormalState, bool _strict)
		{
			var enumerator = fieldDataDictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				List<FieldDataBase> list = fieldDataDictionary[enumerator.Current.Key];
				for (int i = 0; i < list.Count; i++)
					list[i].ResetTarget(_unit, _abnormalState, _strict);
			}
		}

		[global::OpenPCR.Migration("14d8cb85-b1d0-459e-81d0-85a613bd8593")]
		public void UpdateAbnormalStateField()
		{
			foreach (List<FieldDataBase> list in fieldDataDictionary.Values)
				foreach (FieldDataBase data in list)
					if (data is AbnormalStateFieldData abnormal)
						abnormal.CallUpdateTarget();
		}

		[global::OpenPCR.Migration("2b6cef22-8554-4489-811d-4b8c37becc93")]
		public bool ExistsField(int _actionId, bool _isOther)
		{
			if (!fieldDataDictionary.ContainsKey(_actionId))
			{
				return false;
			}

			return fieldDataDictionary[_actionId].FindIndex((FieldDataBase JNOIHMMFADD) => JNOIHMMFADD.Enable && JNOIHMMFADD.Source.IsOther == _isOther) != -1;
		}

		[global::OpenPCR.Migration("93869bbf-f4c7-4ccc-9db4-01a49432e26b")]
		public void AddChangeSpeedOverwriteFieldCounter()
		{
			ChangeSpeedOverwriteFieldCounter++;
		}

		[global::OpenPCR.Migration("93aa5bd6-114f-49b7-8430-8625569f0bb6")]
		public IBattleCameraEffectForBattleProcessor GetBattleCameraEffectForBattleProcessor()
		{
			return battleCameraEffect;
		}

		[global::OpenPCR.Migration("a3c13dff-733f-4a01-b38c-76d78e04c3e2")]
		public IBattleTimeScaleForBattleProcessor GetIBattleTimeScaleForBattleProcessor()
		{
			return battleTimeScale;
		}

		[global::OpenPCR.Migration("db2dff40-22cf-4c0c-ae2a-7baf97bfc710")]
		public int GetSystemId()
		{
			return battleProcessor.GetSystemId();
		}

		[global::OpenPCR.Migration("2a287d5e-4751-4df3-b327-57a3520dde0f")]
		public bool GetEnableShadowEffect()
		{
			return battleProcessor.GetEnableShadowEffect();
		}

		[global::OpenPCR.Migration("adf75c9f-5c97-48a7-af21-d2a4a4d9af06")]
		public void StartChangeScale(Skill _skill, float _startTime)
		{
			foreach (UnitCtrl unit in BlackoutUnitTargetList)
			{
				UnitComponentChangeScale component = unit.ComponentScaleChange;
				if (component.IsScaleChangeTarget)
				{
					component.StartScaleChange(_skill.ScaleChangers, _startTime, _skill.BlackOutTime);
					component.SetScaleChangeTarget(false);
				}
			}
		}

		[global::OpenPCR.Migration("06203f15-f183-4b95-9351-102dc6f628ec")]
		public void StopScaleChange()
		{
			foreach (UnitCtrl blackoutUnitTarget in BlackoutUnitTargetList)
			{
				blackoutUnitTarget.ComponentScaleChange.StopScaleChange();
			}
		}

		[global::OpenPCR.Migration("e21e0927-4302-4578-82fb-c1680a7a2505")]
		public void SetForegroundEnable(bool _enable)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("55f5378e-1b87-4406-872f-3658d9adfc74")]
		public float GetRespawnPos(eUnitRespawnPos _unitRespawnPos)
		{
			if (BossUnit != null)
			{
				return BattleDefine.RESPAWN_POS[_unitRespawnPos] + BossUnit.AllUnitCenter;
			}

			return BattleDefine.RESPAWN_POS[_unitRespawnPos];
		}

		[global::OpenPCR.Migration("dc3930a8-a933-4393-89ef-96bcd4c0eeb3")]
		private void resetAllUnitY()
		{
			foreach (UnitCtrl unit in UnitList)
			{
				unit.transform.SetLocalPosY(GetRespawnPos(unit.RespawnPos));
			}
		}

		[global::OpenPCR.Migration("071b7974-1b04-4cc3-8a3f-0df7f8f06c89")]
		public void ChangeAllUnitDefaultShader()
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("fe9c5e74-0d6f-4ce0-a351-51d4f6d5dcc0")]
		private IEnumerator skipBattle()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::skipBattle():System.Collections.IEnumerator (outside first-phase skip boundary)"); }

		[global::OpenPCR.Migration("2f8ba92f-c44d-4360-bce9-8bb4c91e6b40")]
		public void FadeOutAllVoices(float _fadeSec)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::FadeOutAllVoices(System.Single):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("a309ca2a-1b17-450e-9b6a-485701fa1a80")]
		public void FadeOutAllSe(float _fadeSec)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::FadeOutAllSe(System.Single):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("9f9644a4-7020-4c2b-a91f-89497fcb730f")]
		public bool CanCreateBattleEffect()
		{
			return tempData.BattleSkipState != eBattleSkipState.SKIPPING;
		}

		[global::OpenPCR.Migration("a83aef8f-738a-416c-867d-edfa2276d44f")]
		public bool IsSkipping()
		{
			if (tempData == null || tempData.BattleSkipState != eBattleSkipState.SKIPPING)
			{
				return false;
			}

			if (!viewBattle.FinishFadeInCalled)
			{
				return false;
			}

			return CanSkipBattle();
		}

		[global::OpenPCR.Migration("3b841d93-ee23-4aa7-95e1-d01601ad588b")]
		public bool IsLocalReplay()
		{
			if (replayTempData == null)
			{
				return false;
			}

			return replayTempData.IsLocalReplay;
		}

		[global::OpenPCR.Migration("bd4611b7-d38a-46f0-992e-1648477f7ee3")]
		public void ExecuteSkipProcess(Action _callback)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::ExecuteSkipProcess(System.Action):System.Void (outside first-phase skip boundary)"); }

		[global::OpenPCR.Migration("71d54a21-93b7-4ecc-949c-69d2f1ce1355")]
		public bool CanSkipBattle()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::CanSkipBattle():System.Boolean (outside first-phase skip boundary)"); }

		[global::OpenPCR.Migration("5a8bd919-8cd0-4065-ac24-88ac7d75a116")]
		public bool CanPlaySe()
		{
			eBattleSkipState battleSkipState = tempData.BattleSkipState;
			if ((uint)(battleSkipState - 1) <= 1u)
			{
				return false;
			}

			return true;
		}

		[global::OpenPCR.Migration("e136a20f-c51f-4c7d-82f4-d5c12cff6960")]
		public bool CanPlayVoice()
		{
			eBattleSkipState battleSkipState = tempData.BattleSkipState;
			if ((uint)(battleSkipState - 1) <= 1u)
			{
				return false;
			}

			return true;
		}

		[global::OpenPCR.Migration("85af8c8b-47fe-45ba-b185-5bb4b17b2bc2")]
		public double CalcPlayerDamageTpReduceRate(int _promotionLevel)
		{
			if (ManagerSingleton<MasterDataManager>.Instance.masterExceptEr.HasKey((int)BattleCategory))
				return 1.0;
			return battleProcessor.CalcPlayerDamageTpReduceRate(_promotionLevel);
		}

		[global::OpenPCR.Migration("b2fa45a7-747f-48ef-9575-a630bafecd29")]
		public double CalcEnemyDamageTpReduceRate(int _promotionLevel)
		{
			if (ManagerSingleton<MasterDataManager>.Instance.masterExceptEr.HasKey((int)BattleCategory))
				return 1.0;
			return battleProcessor.CalcEnemyDamageTpReduceRate(_promotionLevel);
		}

		[global::OpenPCR.Migration("9e3d67f3-4f6d-4c79-93bd-ed7418f60370")]
		public (long currentHp, long maxHp) GetTotalBossHp()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::GetTotalBossHp():System.ValueTuple\u00602\u003CSystem.Int64,System.Int64\u003E (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("664a629e-cc30-4289-9a9c-f3bf7d539f41")]
		public bool IsEnableQuestAutoProgress()
		{
			global::Elements.eBattleCategory nativeValue1 = (this).BattleCategory;
			bool nativeResult1 = global::Elements.QuestAutoProgressUtility.IsEnableBattleCategory(unchecked((global::Elements.eBattleCategory)(unchecked((uint)(nativeValue1)))));
			if (((nativeResult1 ? 1u : 0u) & 1u) == 0)
			{
				return false;
			}
			else
			{
				global::Elements.Data.TempData nativeValue2 = (this).tempData;
				if (global::System.Object.ReferenceEquals(nativeValue2, null))
					throw new global::System.NullReferenceException();
				global::Elements.QuestAutoProgressData nativeValue3 = (nativeValue2).QuestAutoProgressData;
				if (global::System.Object.ReferenceEquals(nativeValue3, null))
					throw new global::System.NullReferenceException();
				bool nativeValue4 = (nativeValue3).IsEnable;
				return ((!(((nativeValue4 ? 1u : 0u) == 0u)) ? 1u : 0u) != 0);
			}
		}

		[global::OpenPCR.Migration("22c54b8d-42d5-47ac-9dc3-11203d5fa629")]
		public bool IsQuestAutoProgressing()
		{
			QuestAutoProgressData questAutoProgressData = tempData.QuestAutoProgressData;
			if (IsEnableQuestAutoProgress())
			{
				return !questAutoProgressData.IsStopped;
			}

			return false;
		}

		[global::OpenPCR.Migration("d357aecb-04f0-493b-a8b4-d5f0a0d30350")]
		public void StartFlatoutEffect(DialogFlatoutPlayer.FlatoutType _type)
		{
			dialogManager.OpenFlatoutPlayer(_type, 9000, 0, 0, 0, 1f);
		}

		[global::OpenPCR.Migration("e2e71bac-8d21-47a2-9f65-11cbab4e47f8")]
		public bool IsFinishStartFadeIn()
		{
			return viewBattle.FinishFadeInCalled;
		}

		[global::OpenPCR.Migration("f339c0ca-7633-4327-be6b-7d3fd3b7d5e0")]
		private void calcTalentFormationBonus()
		{
			if (!IsReleasedKnightEnhance)
				return;
			var counts = new Dictionary<int, int>();
			for (int i = 0; i < UnitList.Count; i++)
			{
				List<int> talentIds = UnitList[i].TalentIds;
				for (int j = 0; j < talentIds.Count; j++)
				{
					counts.TryGetValue(talentIds[j], out int value);
					counts[talentIds[j]] = value + 1;
				}
			}

			foreach (var pair in counts)
			{
				Dictionary<int, long> bonus = UnitTalentFormationBonusDictionary;
				long value = battleProcessor.GetTalentBonusPermyriad(pair.Key, pair.Value);
				bonus[pair.Key] = value;
			}
		}

		[global::OpenPCR.Migration("10901f11-1353-4299-bfda-0a41b17de643")]
		public void TriggerUnionBurstSeal(bool _isEnemy)
		{
			foreach (UnitCtrl unit in _isEnemy ? EnemyList : UnitList)
				if (!unit.ComponentState.IdleOnly && !unit.ComponentState.IsDead)
					unit.ComponentAbnormal.ExecPassiveSeal(PassiveSealAction.ePassiveTiming.TEAM_UB);
		}

		[global::OpenPCR.Migration("ecf440af-5e59-4a48-be6c-7a6b2e1024b9")]
		public void StartAllUnitEtherealizationCheck(UnitCtrl _source)
		{
			AppendCoroutine(allUnitEtherealizationCheckUpdate(_source), ePauseType.SYSTEM, null);
		}

		[global::OpenPCR.Migration("33c6c077-f238-4974-b0a3-9e0b3c5400c4")]
		private IEnumerator allUnitEtherealizationCheckUpdate(UnitCtrl _source)
		{
			while (_source.ComponentAbnormal.IsEtherealization())
			{
				AllUnitEtherealizationCheck(_source.IsOther);
				yield return null;
			}
		}

		[global::OpenPCR.Migration("9d2cc734-6054-4f4a-93e6-7fa45c0d9a13")]
		public void AllUnitEtherealizationCheck(bool _isOther)
		{
			List<UnitCtrl> units = _isOther ? EnemyList : UnitList;
			foreach (UnitCtrl unit in units)
				if (!unit.ComponentAbnormal.IsStealth && (long)unit.ComponentParameter.Hp != 0L && !unit.ComponentAbnormal.IsEtherealization() && !unit.ComponentAbnormal.IsAbnormalState(eAbnormalState.SPY))
					return;
			foreach (UnitCtrl unit in units)
			{
				foreach (KeyValuePair<eStateIconType, SealData> pair in unit.ComponentAbnormal.SealDictionary)
					if (pair.Value.IsEtherealization)
						pair.Value.RemoveSeal(pair.Value.Count, true);
				unit.ComponentAbnormal.DispelAbnormalState(eAbnormalState.SPY);
			}
		}

		[global::OpenPCR.Migration("44b45c3a-2af4-42ce-8b12-a82a85018dbb")]
		public bool GetNeedsCommonRehearsalIcon()
		{
			return battleProcessor.GetNeedsCommonRehearsalIcon();
		}

		[global::OpenPCR.Migration("5d8d8450-10f8-4fde-b2dd-ed8ab74628c7")]
		public int GetBlackOutUnitLength()
		{
			return BlackOutUnitList.Count;
		}

		[global::OpenPCR.Migration("b280e4f2-3f16-4a29-a061-7be9e0605f68")]
		public void OnBlackOutEnd(UnitCtrl _unit, bool _isForce = false)
		{
			if (_unit == UnionBurstUseUnit)
			{
				foreach (KeyValuePair<eAbnormalState, Action<bool>> item in _unit.ComponentAbnormal.DamageByBehaviourDictionary)
					item.Value.Call(true);
				UnionBurstUseUnit = null;
			}
			ChargeSkillTurn = eChargeSkillTurn.NONE;
			if (CanCreateBattleEffect()) showTotalDamage(_unit);
			Action<List<UnitCtrl>> action = units =>
			{
				for (int i = 0; i < units.Count; i++)
				{
					UnitCtrl unit = units[i];
					unit.CutInFrameSet.CutInFrame = -1;
					ProtectionData protection = unit.ComponentAbnormal.ProtectionTargetData;
					if (protection != null) protection.DisableIfOwnerDead();
					onEndFadeOut += () =>
					{
						if (!BlackoutUnitTargetList.Contains(unit) && !BlackOutUnitList.Contains(unit))
							unit.ComponentSortOrder.SetSortOrderBack();
					};
				}
			};
			action(BlackOutUnitList);
			action(BlackoutUnitTargetList);
			onEndFadeOut += () => battleEffectManager.SetSortOrderBack();
			BlackOutUnitList.Clear();
			BlackoutUnitTargetList.Clear();
			GamePause(false, true);
			AppendCoroutine(updateBlackOutFadeOut(_unit, _isForce), ePauseType.IGNORE_BLACK_OUT, null);
		}

		[global::OpenPCR.Migration("43fd8fd6-30db-426e-aff9-ab4bd09f18ed")]
		public void OnCutInEnd(bool _isOther, UnitCtrl _unit)
		{
			SkillReadyUnit = null;
			if (this != null)
			{
				StartCoroutine(resumeFrameCount());
				onSyncEnd(_unit);
				if (_unit.ComponentUnionBurst.ConsumeEnergy() != eConsumeResult.FAILED && GameState == eBattleGameState.PLAY)
					_unit.ComponentState.SetState(ActionState.UNION_BURST, 0, 0, false);
				else
					_unit.CutInFrameSet.ServerCutInFrame = -3;
				HideDamageNumFlag = false;
				IsPlayCutin = false;
			}
		}

		[global::OpenPCR.Migration("3da0f5b6-b577-4ef6-ad02-b05c68ba4ae7")]
		public void SetSkillExeScreen(UnitCtrl _cc, float _exeTime, Color _color, bool _endWithMotion)
		{
			bool isOther = _cc.IsOther;
			skillExeUnitForBlackoutFadeout = _cc;
			ChargeSkillTurn = isOther ? eChargeSkillTurn.ENEMY : eChargeSkillTurn.PLAYER;
			_cc.ComponentUnionBurst.IncrementUsedCount();
			ubUsedCount = _cc.ComponentUnionBurst.UsedCount;
			startalpha = _color.a;
			if (!BlackOutUnitList.Contains(_cc)) BlackOutUnitList.Add(_cc);
			if (blackoutTime - blackoutTimeCounter < _exeTime)
			{
				blackoutTimeCounter = 0f;
				blackoutTime = _exeTime;
			}
			if (battleCameraEffect.BattleCameraWaveScale != 1f)
				battleCameraEffect.TurnOffZoomStopFlag(_cc);
			if (!isStarted)
			{
				isStarted = true;
				AppendCoroutine(updateBlackoutUnit(_cc, _endWithMotion), ePauseType.IGNORE_BLACK_OUT, null);
			}
			GamePause(false, false);
		}

		[global::OpenPCR.Migration("9b512530-9a88-450d-96be-b466673d75b8")]
		public void StartBackgroundSpineAnimation(int _skillId, float _skipTime)
		{
			if (ForeGroundSpine != null)
			{
				string text = "battle_foreground_ub_" + _skillId;
				if (ForeGroundSpine.IsAnimation(text))
				{
					ForeGroundSpine.Depth = 22400;
					ForeGroundSpine.PlayAnime(text, _playLoop: false, 0f, _ignoreBlackout: true);
					TrackEntry current = ForeGroundSpine.state.GetCurrent(0);
					current.lastTime = _skipTime;
					current.time = _skipTime;
				}
			}

			if (MiddleGroundSpine != null)
			{
				string text2 = "battle_middleground_ub_" + _skillId;
				if (MiddleGroundSpine.IsAnimation(text2))
				{
					MiddleGroundSpine.Depth = 12180;
					MiddleGroundSpine.PlayAnime(text2, _playLoop: false, 0f, _ignoreBlackout: true);
					TrackEntry current2 = MiddleGroundSpine.state.GetCurrent(0);
					current2.lastTime = _skipTime;
					current2.time = _skipTime;
				}
			}
		}

		[global::OpenPCR.Migration("7d6df238-fc46-45bc-af55-d3d5a80bab00")]
		public void SetSkillExeScreenActive(UnitCtrl _unit, Color _color)
		{
			skillExeUnitForBlackoutFadeout = _unit;
			_unit.ComponentUnionBurst.IncrementUsedCount();
			ubUsedCount = _unit.ComponentUnionBurst.UsedCount;
		}

		[global::OpenPCR.Migration("9fb4d6df-8918-4bd0-a30f-954466b082b2")]
		private IEnumerator updateBlackoutUnit(UnitCtrl _unit, bool _endWithMotion)
		{
			UnitCtrl blackOutUnit = _unit;
			UnitComponentState state = _unit.ComponentState;
			while (true)
			{
				if (_endWithMotion && state.IsDead)
				{
					blackoutTime = 0f;
				}

				blackoutTimeCounter += DeltaTime;
				if (blackOutUnit != skillExeUnitForBlackoutFadeout)
				{
					blackOutUnit = skillExeUnitForBlackoutFadeout;
				}

				if (blackoutTimeCounter > blackoutTime)
				{
					break;
				}

				yield return null;
			}

			blackoutTime = 0f;
			OnBlackOutEnd(blackOutUnit);
			isStarted = false;
		}

		[global::OpenPCR.Migration("c92aae88-6475-4a10-9023-8fb7410af7c3")]
		public void SetBlackoutTimeZero()
		{
			blackoutTime = 0f;
		}

		[global::OpenPCR.Migration("e74cfa34-200d-4ddd-b782-d1008d90298a")]
		private IEnumerator updateBlackOutFadeOut(UnitCtrl _unitCtrl, bool _isForce)
		{
			int initialUsedCount = ubUsedCount;
			float fade = fadeoutDuration;
			while (true)
			{
				fade -= DeltaTime;
				bool restarted = _unitCtrl != null && skillExeUnitForBlackoutFadeout == _unitCtrl && initialUsedCount != _unitCtrl.ComponentUnionBurst.UsedCount;
				if (((skillExeUnitForBlackoutFadeout != _unitCtrl) | restarted | _isForce) || CancelByAwake)
				{
					CancelByAwake = false;
					FinishBlackFadeOut(_unitCtrl, restarted || skillExeUnitForBlackoutFadeout != _unitCtrl);
					yield break;
				}
				if (!(fade > 0f)) break;
				yield return null;
			}
			FinishBlackFadeOut(_unitCtrl, false);
		}

		[global::OpenPCR.Migration("7dcc2ba6-041a-47e6-b94e-4c662db00a9f")]
		public void AddBlackOutTarget(UnitCtrl _source, UnitCtrl _target, BasePartsData _parts)
		{
			_parts.IsBlackoutTarget = true;
			if (!_target.DisableSortOrderFrontOnBlackoutTarget)
				_target.ComponentSortOrder.SetSortOrderFront();
			_target.ComponentDamageControl.ResetTotalDamage();
			if (!BlackoutUnitTargetList.Contains(_target))
			{
				BlackoutUnitTargetList.Add(_target);
				if (_source.ComponentSound.UnionBurstResponceVoiceType == eUbResponceVoiceType.THANKS)
					_target.ComponentUnionBurst.ThanksTargetUnitId = _source.UnitId;
				if (_target != _source)
					_target.ComponentState.PlayDamageWhenIdle(false, false);
			}
		}

		[global::OpenPCR.Migration("fecaee3b-902e-4440-983b-b6293d9a3565")]
		public void FinishBlackFadeOut(UnitCtrl _unit, bool _skipBackgroundAnimeReset)
		{
			if (SpBattleShieldEffect != null)
			{
				SpBattleShieldEffect.SetSortOrderBack();
			}

			if (onEndFadeOut != null)
			{
				onEndFadeOut();
				onEndFadeOut = null;
				if (battleCameraEffect.BattleCameraWaveScale != 1f)
				{
					AppendCoroutine(cameraSizeUpdate(_unit), ePauseType.NO_DIALOG);
				}
			}

			if (!_skipBackgroundAnimeReset)
			{
				if (ForeGroundSpine != null && ForeGroundSpine.Depth >= 11500)
				{
					ForeGroundSpine.Depth = 10900;
					ForeGroundSpine.PlayAnime("battle_foreground_loop");
				}

				if (MiddleGroundSpine != null && MiddleGroundSpine.Depth >= 11500)
				{
					MiddleGroundSpine.Depth = 680;
					MiddleGroundSpine.PlayAnime("battle_middleground_loop");
				}
			}
		}

		[global::OpenPCR.Migration("10e40166-c0e7-451a-b0e0-ac0074cd35a6")]
		private IEnumerator cameraSizeUpdate(UnitCtrl _unit)
		{
			float time = 0.2f;
			while (time > 0f)
			{
				if (ChargeSkillTurn != eChargeSkillTurn.NONE || (_unit != null && battleCameraEffect.JudgeStopEndZoom(_unit)))
					yield break;
				time -= DeltaTime;
				yield return null;
			}
		}

		[global::OpenPCR.Migration("5623ab7a-24be-427e-8e52-72027e3855ed")]
		private void showTotalDamage(UnitCtrl _unit)
		{
			int totalCount = 0;
			for (int i = 0; i < BlackOutUnitList.Count; i++)
				BlackOutUnitList[i].ComponentDamageControl.ShowLifeStealNum();
			for (int i = 0; i < BlackoutUnitTargetList.Count; i++)
			{
				UnitCtrl target = BlackoutUnitTargetList[i];
				if (!target.IsPartsBoss)
				{
					totalCount += target.ComponentMultiPartsBoss.GetFirstParts(true, 0f).JudgeShowTotalDamage() ? 1 : 0;
					continue;
				}
				for (int j = 0; j < target.ComponentMultiPartsBoss.BossPartsListForBattle.Count; j++)
					totalCount += target.ComponentMultiPartsBoss.BossPartsListForBattle[j].JudgeShowTotalDamage() ? 1 : 0;
			}
			List<UnitCtrl> sorted = new List<UnitCtrl>(BlackoutUnitTargetList);
			sorted.Sort((a, b) => b.RespawnPos.CompareTo(a.RespawnPos));
			float delay = 0f;
			for (int i = 0; i < sorted.Count; i++)
			{
				UnitCtrl target = sorted[i];
				if (!target.IsPartsBoss)
				{
					BasePartsData parts = target.ComponentMultiPartsBoss.GetFirstParts(true, 0f);
					if (parts.JudgeShowTotalDamage())
					{
						StartCoroutine(parts.StartTotalDamage(totalCount == 1, delay, _unit));
						delay += 0.06f;
					}
					continue;
				}
				for (int j = 0; j < target.ComponentMultiPartsBoss.BossPartsListForBattle.Count; j++)
				{
					PartsData parts = target.ComponentMultiPartsBoss.BossPartsListForBattle[j];
					if (parts.JudgeShowTotalDamage())
					{
						StartCoroutine(parts.StartTotalDamage(totalCount == 1, delay, _unit));
						delay += 0.06f;
					}
				}
			}
		}

		[global::OpenPCR.Migration("cc07ea79-7cf9-4c64-88d5-4453794df97c")]
		public void HideDamageNumBySkillScreen()
		{
			HideDamageNumFlag = true;
		}

		[global::OpenPCR.Migration("198bc82a-8826-4b1c-b3c9-3944d3fb71f9")]
		public void SetSkillScreen(bool _enable)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("063100d6-6648-4480-8bd8-28c8bf497068")]
		private void displayBonusIcon()
		{
			isDefeatBonusOnly = (bonusDictionary.ContainsKey((eBonusType)2) || bonusDictionary.ContainsKey((eBonusType)0))
				&& !bonusDictionary.ContainsKey((eBonusType)3) && !bonusDictionary.ContainsKey((eBonusType)1);
			// Icon sprites and their index maps have only presentation consumers.
		}

		[global::OpenPCR.Migration("005c33ff-e47d-4a94-bc33-0c5f8ac26597")]
		private IEnumerator waitMultiTargetAppearEnd(List<BonusEnemyData> _bonusEnemyDatas)
		{
			while (IsMultiTargetAppearPlaying) yield return null;
			AppendCoroutine(waitPlayFirstWaveStartStoryEnd(_bonusEnemyDatas), ePauseType.SYSTEM, null);
		}

		[global::OpenPCR.Migration("becd1a03-9c56-4aab-96ab-2a041d9e3272")]
		private IEnumerator waitPlayFirstWaveStartStoryEnd(List<BonusEnemyData> _bonusEnemyDatas)
		{
			bool needWait = battleProcessor.HasWaveStartStory();
			while (needWait && !GetAdvPlayed())
				yield return null;
			startBonusEffectImpl(_bonusEnemyDatas);
		}

		[global::OpenPCR.Migration("9ed5a277-b104-4f8a-a880-75ab2f92d8fb")]
		private void startBonusEffectImpl(List<BonusEnemyData> _bonusEnemyDatas)
		{
			bonusDictionary = battleProcessor.GetExtraEffectDirectionDictionary();
			if (bonusDictionary == null || !ManagerSingleton<SaveDataManager>.Instance.AdvantageAndBreakdownBonusShows)
			{
				if (bonusDictionary != null) displayBonusIcon();
				execBattleBonus(_bonusEnemyDatas);
				return;
			}
			UnitUiCtrl.HidePopUp();
			SetSkillScreen(true);
			GamePause(true, false);
			BonusAppearEffect effect = ManagerSingleton<ResourceManager>.Instance.InstantiateAndGetComponent<BonusAppearEffect>(eResourceId.ANIM_BATTLE_BONUS_APPEAR, UnitUiCtrl.transform, 0L);
			TransformUtility.ResetLocal(effect.transform);
			effect.Play(bonusDictionary, () =>
			{
				SetSkillScreen(false);
				displayBonusIcon();
				execBattleBonus(_bonusEnemyDatas);
				if (!SingletonMonoBehaviour<BattleHeaderController>.Instance.IsPaused) GamePause(false, false);
			}, UnitList, EnemyList);
		}

		[global::OpenPCR.Migration("59783c9f-fa84-445d-8b53-f63115da3dac")]
		public void OpenBonusDetailDialog()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::OpenBonusDetailDialog():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("2350da6e-b3c4-4be1-b2e3-896e1b49acec")]
		public void DeleteBonusIcon(int _bonusId)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("05b532c1-d9ce-44bd-a82f-fd766204417c")]
		public void RevivalAtFinishBattle()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::RevivalAtFinishBattle():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("59264724-3523-457a-bea4-04ac2846abf7")]
		private void finishBattle(eBattleResult _battleResult)
		{
			if (SingletonMonoBehaviour<BattleHeaderController>.Instance.IsPaused && dialogManager.IsUse(eDialogId.BATTLE_MENU))
			{
				dialogManager.ForceCloseOne(eDialogId.BATTLE_MENU);
			}

			foreach (UnitCtrl unit in UnitList)
			{
				unit.ComponentSortOrder.SetPartsSortForResult();
			}

			eBattleSkipState battleSkipState = tempData.BattleSkipState;
			if ((uint)(battleSkipState - 1) <= 1u)
			{
				blockLayerManager.SetBlockButton(_isEnabled: false);
			}

			adjustUnitDamageInfo();
			adjustEnemyDamageInfo();
			global::OpenPCR.Runtime.Api.Random.state = tempRandomState;
			BattleResult = _battleResult;
			isPauseTimeLimit = true;
			battleTimeScale.SetBaseTimeScale(1f);
			battleTimeScale.SpeedupFlag = false;
			for (int i = 0; i < EnemyList.Count; i++)
			{
				EnemyList[i].ComponentState.IdleOnly = true;
			}

			if (BattleCategory == eBattleCategory.LEGION_BATTLE_SUB)
			{
				for (int num = EnemyList.Count - 1; num >= 0; num--)
				{
					if (!EnemyList[num].IsBoss)
					{
						EnemyList.RemoveAt(num);
					}
				}
			}

			for (int j = 0; j < UnitList.Count; j++)
			{
				UnitList[j].ComponentState.IdleOnly = true;
			}

			createHpList();
			battleProcessor.FinishBattle();
		}

		[global::OpenPCR.Migration("32836e08-3d6b-4e1e-9210-7f4855b68241")]
		public List<UnitHpInfo> CreateUnitHpInfoList(long _enemyViewerId, long _myViewerId)
		{
			List<UnitHpInfo> hpList = new List<UnitHpInfo>();
			Action<List<UnitCtrl>, long> action = delegate (List<UnitCtrl> JCLJGFKMBJM, long IJMOJNCOBPF)
			{
				for (int i = 0; i < JCLJGFKMBJM.Count; i++)
				{
					UnitCtrl unitCtrl = JCLJGFKMBJM[i];
					global::System.Int64 hp = unitCtrl.ComponentParameter.Hp;
					if (!unitCtrl.IsSummonOrPhantom && !unitCtrl.IsBonusEnemy)
					{
						UnitHpInfo unitHpInfo = new UnitHpInfo();
						unitHpInfo.SetUnitId(unitCtrl.OriginalUnitId);
						unitHpInfo.SetViewerId(IJMOJNCOBPF);
						unitHpInfo.SetHp((long)hp);
						hpList.Add(unitHpInfo);
					}
				}
			};
			action(UnitList, _myViewerId);
			action(EnemyList, _enemyViewerId);
			return hpList;
		}

		[global::OpenPCR.Migration("b1e491db-85cf-4e78-9fac-6c64214397ca")]
		public List<UnitHpInfoForFriendBattle> CreateUnitHpInfoListForFriendBattle(long _enemyViewerId, long _myViewerId)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::CreateUnitHpInfoListForFriendBattle(System.Int64,System.Int64):System.Collections.Generic.List\u00601\u003CElements.UnitHpInfoForFriendBattle\u003E (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("ce8cf889-19b9-4d8b-a033-5de703d626b8")]
		public void ResultApiSendExec(Action _execCallback = null, Action<int> _errorCallback = null)
		{
			Action exec = () =>
			{
				if (global::OpenPCR.PreparedBattleResultOutput.IsBound(apiManager))
				{
					if ((BattleCategory != eBattleCategory.TRIAL_CLAN_BATTLE && BattleCategory != eBattleCategory.CLAN_BATTLE && BattleCategory != eBattleCategory.LABYRINTH && BattleCategory != eBattleCategory.LABYRINTH_EX) || BattleResult == eBattleResult.INVALID_VALUE)
						throw new global::OpenPCR.OutOfBoundaryException("Offline output requires a completed clan, trial-clan or prepared Labyrinth battle.");
					if (BattleCategory == eBattleCategory.LABYRINTH_EX) global::OpenPCR.PreparedBattleResultOutput.RequestLabyrinthExSend(apiManager);
					else global::OpenPCR.PreparedBattleResultOutput.Complete(apiManager, (int)BattleCategory);
					return;
				}
				Action finish = () => _execCallback.Call();
				if (BattleLogEnable) battleLog.AddSendBattleLogCsvPostParam();
				if (apiManager.TaskCount > 0)
					apiManager.Exec(finish, _errorCallback, false);
				else
					_execCallback.Call();
			};
			if (battleCameraEffect.BattleCameraWaveScale == 1f) exec.Call();
			else StartCameraZoomReset(exec);
		}

		[global::OpenPCR.Migration("8c0fa90a-d238-46ed-9747-b13d875bf267")]
		public void StartCameraZoomReset(Action _callback)
		{
			StartCoroutine(cameraZoomReset(_callback));
		}

		[global::OpenPCR.Migration("5f1e41ba-dead-46b4-9421-ed75357fc004")]
		private IEnumerator cameraZoomReset(Action _callback)
		{
			float time = 0.2f;
			var easing = new CustomEasing(CustomEasing.eType.outQuad, battleCameraEffect.BattleCameraWaveScale, 1f, 0.2f);
			while (time > 0f)
			{
				var effect = battleCameraEffect;
				float value = easing.GetCurVal(DeltaTime, false);
				effect.BattleCameraWaveScale = value;
				onChangeBattleCameraScale(true);
				time -= DeltaTime;
				yield return null;
			}
			_callback.Call();
		}

		[global::OpenPCR.Migration("8bfc67d2-f8ad-4627-abd5-101f39d82577")]
		public void CallbackRequestFinishBattle()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::CallbackRequestFinishBattle():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("5d9c1222-ee32-44d7-ace4-14c90c24f87f")]
		private void createHpList()
		{
			eBattleCategory battleCategory = BattleCategory;
			if (battleCategory != eBattleCategory.FRIEND)
			{
				return;
			}

			List<UnitCtrl> list = UnitList.FindAll((UnitCtrl IEODDJAPMND) => !IEODDJAPMND.IsSummonOrPhantom);
			List<UnitCtrl> list2 = EnemyList.FindAll((UnitCtrl IEODDJAPMND) => !IEODDJAPMND.IsSummonOrPhantom && !IEODDJAPMND.IsBonusEnemy);
			PlayerHpDic.Clear();
			EnemyHpDic.Clear();
			foreach (UnitCtrl item in list)
			{
				PlayerHpDic.Add(item.OriginalUnitId, BattleUtil.DoubleToInt(Math.Ceiling((double)(long)item.ComponentParameter.Hp / (double)(long)item.ComponentParameter.MaxHp * 1000.0)));
			}

			foreach (UnitCtrl item2 in list2)
			{
				EnemyHpDic.Add(item2.OriginalUnitId, BattleUtil.DoubleToInt(Math.Ceiling((double)(long)item2.ComponentParameter.Hp / (double)(long)item2.ComponentParameter.MaxHp * 1000.0)));
			}
		}

		[global::OpenPCR.Migration("e48ab512-90fc-49b5-b0bb-72e4f6ca04ff")]
		public void SetupLosePlayVoiceUnitId()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::SetupLosePlayVoiceUnitId():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("7dbcce74-7088-4592-bc59-c87211bbf385")]
		public void StartCoroutineBossLose()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::StartCoroutineBossLose():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("ad85bc0e-c239-478e-8872-a7ad810678ce")]
		public void OpenTowerExResultDialog(bool _isVictory, int _finalWaveIndex, bool _isFirst)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::OpenTowerExResultDialog(System.Boolean,System.Int32,System.Boolean):System.Void (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("ef456004-ef53-4d4e-a8e7-6addb8ca7170")]
		public void OpenLabyrinthExResultDialog(bool _isVictory, int _finalWaveIndex)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Labyrinth EX result dialog production is outside clan battle and corresponding training.");
		}

		[global::OpenPCR.Migration("f75fb556-a322-415c-a1f7-faaf1e7f452d")]
		public void OpenResultWinDialog()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::OpenResultWinDialog():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("b64a9324-30c4-4aae-ac67-9a27de4e266f")]
		public void PlayFlatoutAfterMove(bool _isPlayUnitJoyMotion)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::PlayFlatoutAfterMove(System.Boolean):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("9099d88b-5164-4e39-a820-93a7a256e25b")]
		public IEnumerator InitializeAndUpdateWalkPlayer(Action _onWalkEnd, bool _isOutOfSystemUpdate = false)
		{
			yield return StartCoroutine(battleProcessor.LoadWinTextResource());
			if (MiddleGroundSpine != null)
			{
				string animeName = "battle_middleground_result";
				if (MiddleGroundSpine.IsAnimation(animeName))
				{
					MiddleGroundSpine.PlayAnime(animeName, _playLoop: false);
					MiddleGroundSpine.state.Complete += delegate
					{
						MiddleGroundSpine.PlayAnime(animeName + "_loop");
					};
				}
			}
			if (ForeGroundSpine != null)
			{
				string animeName2 = "battle_foreground_result";
				if (ForeGroundSpine.IsAnimation(animeName2))
				{
					ForeGroundSpine.PlayAnime(animeName2, _playLoop: false);
					ForeGroundSpine.state.Complete += delegate
					{
						ForeGroundSpine.PlayAnime(animeName2 + "_loop");
					};
				}
			}
			float[][] speedRandArray = new float[3][];
			float guestSpeed = Random(0.8f, 1.2f);
			speedRandArray[0] = new float[UnitUiCtrl.UnitCtrls.Length];
			float num = ((BattleCategory == eBattleCategory.GRAND_ARENA || BattleCategory == eBattleCategory.GRAND_ARENA_REPLAY) ? 302f : 280f);
			int num2 = 0;
			for (int num3 = UnitUiCtrl.UnitCtrls.Length; num2 < num3; num2++)
			{
				UnitCtrl unitCtrl = UnitUiCtrl.UnitCtrls[num2];
				questWinPositionList.Add(new Vector2(0f, -100f));
				if (unitCtrl == null || unitCtrl.ComponentState.IsDead)
				{
					if (num2 != 0)
					{
						questWinPositionList[num2] = questWinPositionList[num2 - 1];
					}
					else
					{
						questWinPositionList[0] = new Vector2(num / 2f, 0f);
					}
					continue;
				}
				for (int num4 = 0; num4 < questWinPositionList.Count - 1; num4++)
				{
					Vector2 value = questWinPositionList[num4];
					value.x += num / 2f;
					questWinPositionList[num4] = value;
				}
				if (num2 != 0)
				{
					Vector2 value2 = questWinPositionList[questWinPositionList.Count - 1];
					value2.x += questWinPositionList[questWinPositionList.Count - 2].x - num;
					questWinPositionList[questWinPositionList.Count - 1] = value2;
				}
			}
			if (UnitUiCtrl.GuestUnit != null && !UnitUiCtrl.GuestUnit.ComponentState.IsDead)
			{
				questWinPositionList.Add(new Vector2(0f, -100f));
				for (int num5 = 0; num5 < questWinPositionList.Count - 1; num5++)
				{
					Vector2 value3 = questWinPositionList[num5];
					value3.x += num / 2f;
					questWinPositionList[num5] = value3;
				}
				Vector2 value4 = questWinPositionList[questWinPositionList.Count - 1];
				value4.x += questWinPositionList[questWinPositionList.Count - 2].x - num;
				questWinPositionList[questWinPositionList.Count - 1] = value4;
				UnitCtrl guestUnit = UnitUiCtrl.GuestUnit;
				float x = questWinPositionList[questWinPositionList.Count - 1].x;
				bool leftDirection = guestUnit.transform.localPosition.x > x;
				guestUnit.SetLeftDirection(leftDirection);
				guestUnit.GetCurrentSpineCtrl().CurColor = Color.white;
				guestUnit.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.RUN, guestUnit.MotionPrefix);
			}
			int num6 = 0;
			for (int num7 = UnitUiCtrl.UnitCtrls.Length; num6 < num7; num6++)
			{
				UnitCtrl unitCtrl2 = UnitUiCtrl.UnitCtrls[num6];
				if (unitCtrl2 != null)
				{
					speedRandArray[0][num6] = Random(0.8f, 1.2f);
					float x2 = questWinPositionList[num6].x;
					bool leftDirection2 = unitCtrl2.transform.localPosition.x > x2;
					unitCtrl2.SetLeftDirection(leftDirection2);
					unitCtrl2.GetCurrentSpineCtrl().SetTimeScale(1f);
					unitCtrl2.GetCurrentSpineCtrl().CurColor = Color.white;
					unitCtrl2.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.RUN, unitCtrl2.MotionPrefix);
				}
			}
			float deltaTimeAccumulated = 0f;
			while (true)
			{
				bool bHAOIOGGDDC = false;
				if (_isOutOfSystemUpdate)
				{
					deltaTimeAccumulated += Time.deltaTime - DeltaTime;
					while (deltaTimeAccumulated >= DeltaTime)
					{
						bool more = unitCtrlsWalk(UnitUiCtrl.UnitCtrls, speedRandArray, bHAOIOGGDDC, 0, UnitUiCtrl.GuestUnit, ref guestSpeed);
						bHAOIOGGDDC = true;
						deltaTimeAccumulated -= DeltaTime;
						if (!more)
						{
							bHAOIOGGDDC = false;
							break;
						}
					}
				}
				if (!unitCtrlsWalk(UnitUiCtrl.UnitCtrls, speedRandArray, bHAOIOGGDDC, 0, UnitUiCtrl.GuestUnit, ref guestSpeed))
				{
					break;
				}
				yield return null;
			}
			float time = 0f;
			while (true)
			{
				time += DeltaTime;
				if (time > 0.6f)
				{
					break;
				}
				yield return null;
			}
			_onWalkEnd.Call();
		}

		[global::OpenPCR.Migration("d1db9350-899c-4d7a-937e-2004b5e9a583")]
		private bool unitCtrlsWalk(UnitCtrl[] _unitCtrls, float[][] _speedRandArray, bool _keepWalkForMain, int _index, UnitCtrl _guestUnit, ref float _guestSpeed)
		{
			int i = 0;
			for (int num = _unitCtrls.Length; i < num; i++)
			{
				UnitCtrl unitCtrl = _unitCtrls[i];
				if (!(unitCtrl != null))
				{
					continue;
				}
				Vector2 vector = questWinPositionList[i];
				Vector3 localPosition = unitCtrl.transform.localPosition;
				if (!isReached(unitCtrl.IsLeftDir, localPosition, vector))
				{
					Vector3 vector2 = localPosition + (Vector3)(vector - (Vector2)localPosition).normalized * 1600f * DeltaTime * _speedRandArray[_index][i];
					if (isReached(unitCtrl.IsLeftDir, vector2, vector))
					{
						vector2 = vector;
					}
					unitCtrl.transform.localPosition = vector2;
					_keepWalkForMain = true;
				}
				else if (_speedRandArray[_index][i] != 0f)
				{
					unitCtrl.transform.localPosition = vector;
					if (unitCtrl.GetCurrentSpineCtrl().IsAnimation(eAnimKitCharacterAnimeId.IDLE_RESULT))
					{
						unitCtrl.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE_RESULT, unitCtrl.MotionPrefix);
					}
					else
					{
						unitCtrl.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE, unitCtrl.MotionPrefix);
					}
					_speedRandArray[_index][i] = 0f;
				}
			}
			if (_guestUnit != null)
			{
				Vector2 vector3 = questWinPositionList[questWinPositionList.Count - 1];
				Vector3 localPosition2 = _guestUnit.transform.localPosition;
				if (!isReached(_guestUnit.IsLeftDir, localPosition2, vector3))
				{
					Vector3 vector4 = localPosition2 + (Vector3)(vector3 - (Vector2)localPosition2).normalized * 1600f * DeltaTime * _guestSpeed;
					if (isReached(_guestUnit.IsLeftDir, vector4, vector3))
					{
						vector4 = vector3;
					}
					_guestUnit.transform.localPosition = vector4;
					_keepWalkForMain = true;
				}
				else if (_guestSpeed != 0f)
				{
					_guestUnit.transform.localPosition = vector3;
					if (_guestUnit.GetCurrentSpineCtrl().IsAnimation(eAnimKitCharacterAnimeId.IDLE_RESULT))
					{
						_guestUnit.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE_RESULT, _guestUnit.MotionPrefix);
					}
					else
					{
						_guestUnit.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE, _guestUnit.MotionPrefix);
					}
					_guestSpeed = 0f;
				}
			}
			return _keepWalkForMain;
		}

		[global::OpenPCR.Migration("8e13a4ba-bc45-4cae-93da-54b7483db8dc")]
		private bool isReached(bool _isLeft, Vector2 _fromPos, Vector2 _toPos)
		{
			bool flag = (_isLeft ? (_toPos.x >= _fromPos.x) : (_fromPos.x >= _toPos.x));
			return (Mathf.Abs(_fromPos.x - _toPos.x) <= 0.1f) | flag;
		}

		[global::OpenPCR.Migration("c41d5e4d-e430-4634-967d-8f92c75ea118")]
		public void PlayPartyUnitJoyMotion()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::PlayPartyUnitJoyMotion():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("7644e07e-b0a7-4448-b370-01f14966de6b")]
		public void StartResult()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::StartResult():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("0fc99a6c-ffbc-4608-8de7-2cd9b71396b1")]
		public void ShowResultDialog()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::ShowResultDialog():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("bb97b8e9-bddc-4a47-b4f7-2ff8bb71bae4")]
		public void ShowResultDialogCoroutine(bool _isWin)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::ShowResultDialogCoroutine(System.Boolean):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("1ac4a7a2-c84e-4b07-80f4-4a68a6c12334")]
		public void ResetFieldTimeUp()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::ResetFieldTimeUp():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("f9c3b178-0101-4c9d-a0c1-9f4d5853d071")]
		public IEnumerator UpdateBossIdleMotion(BattleAnimKitController _unitSpineController, int _motionPrefix)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::UpdateBossIdleMotion(Elements.BattleAnimKitController,System.Int32):System.Collections.IEnumerator: runtime capability is unavailable."); }

		[global::OpenPCR.Migration("86fac3b9-0a7a-4004-a8d0-830a08ff68dc")]
		public void ForceFadeUnitOut()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::ForceFadeUnitOut():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("36e6650f-8192-41d4-8420-ad8751b611c9")]
		private void stopAllAbnormalEffect(List<UnitCtrl> _unitList)
		{
			foreach (UnitCtrl item in _unitList)
			{
				item.ComponentAbnormal.StopAbnormalEffect();
				var charaAuraEffects = item.ComponentAbnormal.CharaAuraEffectList;
				foreach (var (skillEffectCtrl, skillEffectCtrl2) in charaAuraEffects)
				{
					skillEffectCtrl2.SetTimeToDie(value: true);
					skillEffectCtrl2.SetActive(_isActive: false);
					skillEffectCtrl.SetTimeToDie(value: true);
					skillEffectCtrl.SetActive(_isActive: false);
				}

				charaAuraEffects.Clear();
			}
		}

		[global::OpenPCR.Migration("eb8f67a7-0f41-4260-aa3c-6a2dae3b6067")]
		public List<TowerQueryUnit> CreateTowerQueryUnitList(bool _isEx, bool _isPlayer)
		{
			throw new OpenPCR.OutOfBoundaryException("Tower result-unit HP/TP conversion, dispatch persistence and skill-limit serialization are outside clan battle and corresponding training.");
		}

		[global::OpenPCR.Migration("8d409039-6cc3-4ff9-89f6-633a8fb052f3")]
		public void AddTempDataBattleLog()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::AddTempDataBattleLog():System.Void (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("d471da2b-3982-4e91-b809-04fcfa42b3a1")]
		private IEnumerator waitActionWhile(Func<bool> _condition, Action _action)
		{
			while (_condition())
			{
				yield return null;
			}

			_action();
		}

		[global::OpenPCR.Migration("c5236695-da1c-4ad2-9640-b2b94f307549")]
		public IEnumerator SetupPlayerResultPosition(Action _finishCallBack)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::SetupPlayerResultPosition(System.Action):System.Collections.IEnumerator (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("ba6f7ea2-03d2-4807-921d-748d35a6eb04")]
		public void CallbackActionEnd(long actionId)
		{
			ExecutedActionList.Remove(actionId);
			if (!isAllActionDone && ExecutedActionList.Count == 0)
			{
				isAllActionDone = true;
				onExecutedActionListCountZero();
			}
		}

		[global::OpenPCR.Migration("5e6f1f30-181c-487d-956d-e4d6e1ec4f28")]
		private void onExecutedActionListCountZero()
		{
			if (GameState != eBattleGameState.PLAY && allTeamFadeoutDone && (isAllActionDone || ignoreAllActionDone) && idleonlyDone)
			{
				finishWave(otherAllDead);
			}
		}

		[global::OpenPCR.Migration("0a9629b3-4e93-437d-bb7b-584c763b6eed")]
		public void CallbackIdleOnlyDone(UnitCtrl unit)
		{
			if (GameState != eBattleGameState.WAIT_WAVE_END || !(unit.IsOther ? EnemyList : UnitList).Contains(unit))
			{
				return;
			}

			int i = 0;
			for (int count = UnitList.Count; i < count; i++)
			{
				if (!UnitList[i].ComponentState.IsDead)
				{
					idleonlyDone = UnitList[i].ComponentState.CurrentState == ActionState.IDLE;
					if (!idleonlyDone)
					{
						return;
					}
				}
			}

			int j = 0;
			for (int count2 = EnemyList.Count; j < count2; j++)
			{
				if (!EnemyList[j].ComponentState.IsDead)
				{
					idleonlyDone = EnemyList[j].ComponentState.CurrentState == ActionState.IDLE || EnemyList[j].ComponentState.CurrentState == ActionState.LOSE;
					if (!idleonlyDone)
					{
						return;
					}
				}
			}

			if (allTeamFadeoutDone && (isAllActionDone || ignoreAllActionDone) && idleonlyDone)
			{
				GameState = eBattleGameState.IDLE;
				this.Timer(delegate
				{
					finishWave(otherAllDead);
				});
			}
		}

		[global::OpenPCR.Migration("fe8d8c10-6998-47f0-8f07-27bec21ea55d")]
		public void CallbackFadeOutDone(UnitCtrl unit)
		{
			if (allTeamFadeoutDone)
			{
				return;
			}

			if (isWinTeamFadeOutWaitMode)
			{
				if ((!otherAllDead || !unit.IsOther) && (!playerAllDead || unit.IsOther))
				{
					judgeWinUnitFadeOutDone(unit.IsOther);
				}

				return;
			}

			List<UnitCtrl> list = (unit.IsOther ? EnemyList : UnitList);
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				UnitCtrl unitCtrl = list[i];
				if (!battleProcessor.JudgeIsIgnoreUnit(unitCtrl))
				{
					if (unitCtrl.HasDieLoop)
					{
						BattleUnitBaseAnimKitController currentSpineCtrl = unitCtrl.GetCurrentSpineCtrl();
						loseTeamfadeOutDone = currentSpineCtrl.AnimationName == currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DIE_LOOP);
					}
					else
					{
						loseTeamfadeOutDone = !unitCtrl.gameObject.activeSelf;
					}

					if (!loseTeamfadeOutDone)
					{
						return;
					}
				}
			}

			judgeWinUnitFadeOutDone(!unit.IsOther);
		}

		[global::OpenPCR.Migration("38bd1409-2b77-425c-b1f6-d01aa9316a7d")]
		private void judgeWinUnitFadeOutDone(bool _winUnitIsOther)
		{
			List<UnitCtrl> list = (_winUnitIsOther ? EnemyList : UnitList);
			for (int i = 0; i < list.Count; i++)
			{
				UnitCtrl unitCtrl = list[i];
				if (unitCtrl.ComponentState.IsDead && unitCtrl.gameObject.activeSelf)
				{
					isWinTeamFadeOutWaitMode = true;
					return;
				}
			}

			isWinTeamFadeOutWaitMode = false;
			allTeamFadeoutDone = true;
			if (allTeamFadeoutDone && (isAllActionDone || ignoreAllActionDone) && idleonlyDone)
			{
				finishWave(!_winUnitIsOther);
			}
		}

		[global::OpenPCR.Migration("d75c7829-7361-4d23-9d3b-3e0d3bf5b468")]
		private bool isCheckDeadAllUnit(UnitCtrl _unit)
		{
			if ((BattleCategory == eBattleCategory.HATSUNE_BOSS_BATTLE || BattleCategory == eBattleCategory.HATSUNE_SPECIAL_BATTLE) && HatsuneUtility.IsNowEventMultiBoss())
			{
				eSpecialBattlePurpose purpose = GetPurpose();
				if ((uint)(purpose - 3) <= 1u && HatsuneUtility.GetNowEventBossEnemySettingForEnemyId(_unit.UnitId) != null)
				{
					return false;
				}
			}

			return true;
		}

		[global::OpenPCR.Migration("2e328687-0229-4427-8976-fe86fbfee4fe")]
		public void CallbackDead(UnitCtrl _unit)
		{
			CallbackIdleOnlyDone(_unit);
			UnitUiCtrl.UpdateAllEnemyHp0();
			List<UnitCtrl> list = (_unit.IsOther ? EnemyList : UnitList);
			if (list == null || !isCheckDeadAllUnit(_unit))
			{
				return;
			}

			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				UnitCtrl unitCtrl = list[i];
				if (unitCtrl.IsSummonOrPhantom || unitCtrl.IsBonusEnemy || unitCtrl.IsGuest || battleProcessor.JudgeIsIgnoreUnit(unitCtrl))
				{
					continue;
				}

				if (_unit.IsOther)
				{
					otherAllDead = unitCtrl.ComponentState.IsDead;
					if (!otherAllDead)
					{
						return;
					}
				}
				else
				{
					playerAllDead = unitCtrl.ComponentState.IsDead;
					if (!playerAllDead)
					{
						return;
					}
				}
			}

			if (UnitUiCtrl.GuestUnit != null && !UnitUiCtrl.GuestUnit.ComponentState.IsDead && playerAllDead)
			{
				UnitUiCtrl.GuestUnit.ComponentState.IdleOnly = true;
				UnitUiCtrl.GuestUnit.IsGuestFadeout = true;
			}

			battleProcessor.OnDeadLastUnit(_unit);
			bool num = GameState == eBattleGameState.PLAY;
			GameState = eBattleGameState.WAIT_WAVE_END;
			if (ExecutedActionList.Count > 0)
			{
				isAllActionDone = false;
				if (ignoreAllActionDoneCoroutineState == eIgnoreAllActionDoneState.NOT_RUNNING)
				{
					ignoreAllActionDoneCoroutineState = eIgnoreAllActionDoneState.RUNNING;
					StartCoroutine(updateIgnoreAllActionDoneCounter());
				}
			}

			CallbackIdleOnlyDone(_unit);
			List<UnitCtrl> list2 = (_unit.IsOther ? UnitList : EnemyList);
			isPauseTimeLimit = true;
			if (num)
			{
				int j = 0;
				for (int count2 = list2.Count; j < count2; j++)
				{
					UnitCtrl unitCtrl2 = list2[j];
					unitCtrl2.ComponentState.IdleOnly = true;
					unitCtrl2.ComponentAbnormal.CureAllAbnormalState();
				}
			}

			if (!_unit.IsOther)
			{
				return;
			}

			bool num2 = (BattleCategory == eBattleCategory.HATSUNE_BOSS_BATTLE || BattleCategory == eBattleCategory.HATSUNE_SPECIAL_BATTLE) && HatsuneUtility.IsNowEventMultiBoss();
			bool flag = IsDungeonSpecialBattle && DungeonUtility.IsNowSpecialMultiBoss();
			if (num2 | flag)
			{
				EnemyList.ForEach(delegate (UnitCtrl JNOIHMMFADD)
				{
					if (battleProcessor.JudgeIsIgnoreUnit(JNOIHMMFADD))
					{
						JNOIHMMFADD.ComponentState.IdleOnly = true;
						JNOIHMMFADD.ComponentAbnormal.CureAllAbnormalState();
						CallbackFadeOutDone(JNOIHMMFADD);
					}
				});
				return;
			}

			UnitCtrl unitCtrl3 = EnemyList.Find((UnitCtrl JNOIHMMFADD) => !JNOIHMMFADD.IsBonusEnemy && battleProcessor.JudgeIsIgnoreUnit(JNOIHMMFADD));
			if (unitCtrl3 != null)
			{
				unitCtrl3.ComponentState.IdleOnly = true;
				unitCtrl3.ComponentAbnormal.CureAllAbnormalState();
				CallbackFadeOutDone(unitCtrl3);
			}
		}

		[global::OpenPCR.Migration("67347794-5060-4a9b-9d61-ba4f1d235aa4")]
		private IEnumerator updateIgnoreAllActionDoneCounter()
		{
			float timer = 10f;
			while (true)
			{
				timer -= Time.deltaTime;
				if (ignoreAllActionDoneCoroutineState == eIgnoreAllActionDoneState.STOP_PULSE)
				{
					ignoreAllActionDoneCoroutineState = eIgnoreAllActionDoneState.NOT_RUNNING;
					yield break;
				}

				if (timer < 0f)
				{
					break;
				}

				yield return null;
			}

			ignoreAllActionDone = true;
			CallbackActionEnd((ExecutedActionList.Count == 0) ? 0 : ExecutedActionList[0]);
			ignoreAllActionDoneCoroutineState = eIgnoreAllActionDoneState.NOT_RUNNING;
		}

		[global::OpenPCR.Migration("6ed24701-ef5d-4955-944f-05eae5c6eaf9")]
		public void UekOnDeadLastUnit(UnitCtrl _unit, int _area)
		{ throw new global::OpenPCR.OutOfBoundaryException("UEK reward/treasure production is outside clan battle and training."); }

		[global::OpenPCR.Migration("1da8ffb2-a001-4f03-8b50-6f134be23518")]
		public void TowerOnDeadLastUnit(UnitCtrl _unit)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::TowerOnDeadLastUnit(Elements.UnitCtrl):System.Void (outside first-phase tower-reward boundary)"); }

		[global::OpenPCR.Migration("2ab8bf79-0d83-43e6-9a92-b5617c9b0dec")]
		public void HighRarityOnDeadLastUnit(UnitCtrl _unit, eBattleTreasureBoxType _treasureType)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::HighRarityOnDeadLastUnit(Elements.UnitCtrl,Elements.eBattleTreasureBoxType):System.Void (outside first-phase high-rarity boundary)"); }

		[global::OpenPCR.Migration("b2a47840-792f-48cf-ba2e-54a138a837d1")]
		private void finishWave(bool _win)
		{
			if (ignoreAllActionDoneCoroutineState == eIgnoreAllActionDoneState.RUNNING)
			{
				ignoreAllActionDoneCoroutineState = eIgnoreAllActionDoneState.STOP_PULSE;
			}

			ignoreAllActionDone = false;
			for (int i = 0; i < UnitList.Count; i++)
			{
				UnitCtrl unitCtrl = UnitList[i];
				unitCtrl.ComponentAbnormal.CureAllAbnormalState();
				unitCtrl.CutInFrameSet.CutInFrame = -1;
			}

			if (!_win && otherAllDead)
			{
				_win = otherAllDead;
			}

			ChargeSkillTurn = eChargeSkillTurn.NONE;
			UnitUiCtrl.HidePopUp();
			playerAllDead = false;
			otherAllDead = false;
			allTeamFadeoutDone = false;
			loseTeamfadeOutDone = false;
			idleonlyDone = false;
			GameState = eBattleGameState.IDLE;
			appendWaveEndLog();
			StartCoroutine(waitActionWhile(() => tempData.BattleSkipState == eBattleSkipState.PREPARING_TO_SKIP, delegate
			{
				if (_win)
				{
					if (CurrentWave < tempData.BattleStageData.CurrentBattleWaves.Count - 1)
					{
						battleProcessor.FinishWave();
					}
					else
					{
						SingletonMonoBehaviour<BattleHeaderController>.Instance.gameObject.SetActive(value: false);
						UnitUiCtrl.HideSettingButtons();
						int waveEndStoryId = battleProcessor.GetWaveEndStoryId();
						PlayStory(delegate
						{
							finishBattle(eBattleResult.WIN);
						}, waveEndStoryId, _end: true);
					}
				}
				else
				{
					SingletonMonoBehaviour<BattleHeaderController>.Instance.gameObject.SetActive(value: false);
					UnitUiCtrl.HideSettingButtons();
					finishBattle(eBattleResult.LOSE);
				}
			}));
		}

		[global::OpenPCR.Migration("b7c6fa8c-bc90-45f0-9d03-8a7162476a06")]
		private void Awake()
		{
			isValid = true;
			componentTalent = new BattleComponentTalent();
			singletonTree = this.CreateSingletonTree();
			UekTempData uek = singletonTree.Get<UekTempData>();
			towerTempData = singletonTree.Get<TowerTempData>();
			spaceBattleTempData = singletonTree.Get<SpaceBattleTempData>();
			replayTempData = singletonTree.Get<ReplayTempData>();
			// Colosseum-only singleton consumer is outside clan/training scope.
			battleManagerNode = singletonTree.GetCurrentNodeForInstanceEditing();
			battleLog = new BattleLog(this);
			battleUnionBurstController = new BattleUnionBurstController(this);
			battleCameraEffect = new BattleCameraEffect(this, playCamera, motionBlur, playCameraBlurOptimized);
			battleEffectPool = new BattleEffectPool();
			battleTimeScale = new BattleTimeScale(this);
			KillOrder = new List<int>();
			battleManagerNode.AddInstance(this);
			battleManagerNode.AddInstance(battleLog);
			battleManagerNode.AddInstance(battleCameraEffect);
			battleManagerNode.AddInstance(battleEffectPool);
			battleManagerNode.AddInstance(battleTimeScale);
			battleManagerNode.AddInstance(battleUnionBurstController);
			DialogBase.StaticRelease();
			singletonTree.AddNode(battleManagerNode, typeof(DialogBase));
			SkillEffectCtrl.StaticInitSingletonTree();
			BattleVoiceUtility.StaticInit(this);
			dialogManager.SetBattleManager(this);
			if (Singleton<UserData>.Instance != null &&
				Singleton<UserData>.Instance.InitSettingParameter != null &&
				Singleton<UserData>.Instance.InitSettingParameter.BattleLogType != null)
				BattleLogEnable = battleLog.GetLogEnable(Singleton<UserData>.Instance.InitSettingParameter.BattleLogType);
			EnergyRecoveryRateMax = Singleton<UserData>.Instance.EnergyRecoveryRateMax;
			BattleCategory = tempData.BattleStageData.CurrentBattleCategory;
			battleProcessor = createBattleProcessor(uek);
			DodgeTPRecoverRatio = battleProcessor.GetDodgeTPRecoveryRatio();
			BattleEffectPool pool = battleEffectPool;
			pool.ForceNormalSD = battleProcessor.GetForceNormalSD();
			CurrentEnemyPoint = battleProcessor.GetEnemyPoint();
			currentWaveOffset = battleProcessor.GetCurrentWaveOffset();
			battleTimeScale.SetBaseTimeScale(1f);
			UnitList = new List<UnitCtrl>();
			EnemyList = new List<UnitCtrl>();
			UnitTalentFormationBonusDictionary = new Dictionary<int, long>();
			FadeOutUnitList = new List<UnitCtrl>();
			ExecutedActionList = new List<long>();
			BlackoutUnitTargetList = new List<UnitCtrl>();
			BlackOutUnitList = new List<UnitCtrl>();
			FrameCount = 0;
			IsEnemyCreated = false;
			ReActionEffectDictionary = new Dictionary<string, GameObject>();
			fieldDataDictionary = new Dictionary<int, List<FieldDataBase>>();
			SaveDataManager saved = ManagerSingleton<SaveDataManager>.Instance;
			uint category = unchecked((uint)BattleCategory);
			if (category <= 38u)
			{
				long categoryMask = 1L << (int)category;
				if ((categoryMask & 0x6006EC000L) != 0)
					settingBattleMaxSpeed = saved.TowerBattleMaxSpeed;
				else if ((categoryMask & 0x7000000000L) != 0)
					settingBattleMaxSpeed = saved.DimensionFaultBattleMaxSpeed;
				else if ((categoryMask & 0x10000200L) != 0)
					settingBattleMaxSpeed = ManagerSingleton<SaveDataManager>.Instance.ClanReharsalBattleMaxSpeed;
			}
			BattleLogEnable = !TutorialManager.IsStartTutorial && battleProcessor.GetBattleLogEnable(BattleLogEnable);
			FrameRate = battleProcessor.GetFrameRate();
			IsDefenseReplayMode = battleProcessor.GetIsDefenseReplayMode();
			cutInPlaybackType = saved.CutInPlaybackType;
			DeltaTime = 0.016666668f;
			CoroutineManager = gameObject.AddComponent<CoroutineManager>();
			CoroutineManager.Init(BlackOutUnitList);
			battleEffectManager = gameObject.AddComponent<BattleEffectManager>();
			battleEffectManager.Init(BlackOutUnitList, UnitList, EnemyList);
			// Independent skill-screen renderer order/scale writes are outside battle inference.
			enabled = false;
			IsBossBattle = false;
			// Independent motion-blur and background-blur renderer writes are outside battle inference.
			BattleResult = eBattleResult.INVALID_VALUE;
			battleCameraEffect.BattleCameraWaveScale = 1f;
			IsSuspendLose = false;
			battleCameraEffect.BossBattleScale = 0f;
			battleCameraEffect.BattleCameraUbScale = 1f;
			IsReleasedKnightEnhance = battleProcessor.IsReleasedKnightEnhance();
			UserData user = Singleton<UserData>.Instance;
			StartBattleUnixTime = user.NowUnixTime;
			isReleaseSlipDamageLogBarrier = user.IsReleaseSlipDamageLogBarrier();
			LogBarrierIniSetting logBarrier = user.InitSettingParameter.LogBarrierIniSetting;
			if (logBarrier != null)
			{
				allLogBarrierEffectValue = (int)logBarrier.Coefficient;
				allLogBarrierStartValue = (int)logBarrier.Threshold;
			}
			NewAllLogBarrierEnabled = ManagerSingleton<MasterDataManager>.Instance.masterLabyrinthSetting.HasKey(10);
		}


		[global::OpenPCR.Migration("c3128bc4-d863-43c8-9270-c9d359197a6e")]
		private BaseBattleProcessor createBattleProcessor(UekTempData _uekTempData)
		{
			return CreateBattleProcessorImpl(_uekTempData);
		}

		[global::OpenPCR.Migration("c1cc6759-334a-4c77-bd68-1561add18fd3")]
		public BaseBattleProcessor CreateBattleProcessorImpl(UekTempData _uekTempData)
		{
			Screen.sleepTimeout = -1;
			int category = (int)BattleCategory;
			if (category == 9) return new ClanBattleProcessor(this);
			if (category == 32) return new TrialBattleProcessor(this, eBattleCategory.TRIAL_CLAN_BATTLE);
			if (category == 59) return new LabyrinthBattleProcessor(this, singletonTree.Get<LabyrinthTempData>());
			if (category == 60) return new LabyrinthExBattleProcessor(this, singletonTree.Get<LabyrinthTempData>());
			if (unchecked((uint)category) <= 20u || (category >= 23 && category <= 60) || category == 66)
				throw new global::OpenPCR.OutOfBoundaryException("Battle processor scene mode outside clan battle and corresponding training: " + category);
			return null;
		}

		[global::OpenPCR.Migration("b9326b35-d356-477d-870e-2051070a30cf")]
		private void OnDestroy()
		{
			tempData.UnitDamageInfoList.Clear();
			tempData.EnemyDamageInfoList.Clear();
			tempData.EnemyColorIdList.Clear();
			if (viewManager.NextViewId != eViewId.BATTLE)
			{
				replayTempData.ResetData();
			}
			skillExeScreen = null;
			BlackOutUnitList = null;
			SkillReadyUnit = null;
			onEndFadeOut = null;
			skillExeScreenTexture = null;
			ChainVoicePlayUnit = null;
			skillExeUnitForBlackoutFadeout = null;
			UnitPassiveDictionaryByUnit = null;
			EnemyPassiveDictionaryByUnit = null;
			EnemyPassiveDictionaryByEnemy = null;
			UnitPassiveDictionaryByEnemy = null;
			DamageNumberEffectDictionary = null;
			TotalDamageEffectPrefabLarge = null;
			TotalDamageEffectPrefabSmall = null;
			storyBattleDataPrefab = null;
			motionBlur = null;
			playCameraBlurOptimized = null;
			focusObject = null;
			focusButton = null;
			stageBgTex = null;
			stageForegroundTex = null;
			unitParentTransform = null;
			enemyParentTransform = null;
			playCamera = null;
			bgTransform = null;
			viewBattle = null;
			battleEffectManager = null;
			effectUpdateList = null;
			voiceOnUnit = null;
			currentEnemyUnitCtrlDictionary = null;
			allWaveEnemyUnitCtrlListArray = null;
			BossUnit = null;
			questWinPositionList = null;
			UnitList = null;
			EnemyList = null;
			UnitTalentFormationBonusDictionary = null;
			FadeOutUnitList = null;
			DecoyUnit = null;
			DecoyOther = null;
			DecoyUnitFire = null;
			DecoyOtherFire = null;
			CoroutineManager = null;
			ExecutedActionList = null;
			BlackoutUnitTargetList = null;
			ReActionEffectDictionary = null;
			RewardList = null;
			unitSpineControllerList = null;
			fieldDataDictionary = null;
			enemyEnvironment = null;
			unitEnvironment = null;
			viewManager = null;
			blockLayerManager = null;
			resourceManager = null;
			apiManager = null;
			soundManager = null;
			loadingManager = null;
			BattleVoiceFlagMgr = null;
			tempData = null;
			replayTempData = null;
			KillOrder = null;
			dialogManager.ReleaseBattleManager();
			dialogManager = null;
			if (battleProcessor != null)
			{
				battleProcessor.Release();
				battleProcessor = null;
			}
			BlurEffect.StaticRelease();
			DamageEffectCtrlBase.StaticRelease();
			AnimKitCowFireArm.StaticRelease();
			AnimKitFireArm.StaticRelease();
			AnimKitNormalEffect.StaticRelease();
			PartsBossTotalDamage.StaticRelease();
			ActionParameter.StaticRelease();
			FieldDataBase.StaticRelease();
			EnvironmentData.StaticRelease();
			StrikeBackData.StaticRelease();
			AllUnitSealDamageCutData.StaticRelease();
			UnitCtrl.StaticRelease();
			SkillEffectCtrl.StaticRelease();
			AbromalStateIconController.StaticRelease();
			BonusIconController.StaticRelease();
			LifeGaugeController.StaticRelease();
			UbAbnormalData.StaticRelease();
			UnitActionController.StaticRelease();
			BattleAnimKitController.StaticRelease();
			// DialogBossResult result-dialog renderer singleton omitted.
			// DialogAbyssBossResult result-dialog renderer singleton omitted.
			// DialogHatsuneBossResult result-dialog renderer singleton omitted.
			// DialogQuestFailed result-dialog renderer singleton omitted.
			// DialogQuestResultWin result-dialog renderer singleton omitted.
			// DialogTowerExResult result-dialog renderer singleton omitted.
			// PartsQuestResultLater result-dialog renderer singleton omitted.
			// PartsQuestResultFormer result-dialog renderer singleton omitted.
			// DialogNormalArenaResult result-dialog renderer singleton omitted.
			// DialogGrandArenaResult result-dialog renderer singleton omitted.
			// Pure result-dialog renderer singleton reset is omitted.
			// DialogTrialBattleResult result-dialog renderer singleton omitted.
			singletonTree.RemoveNode(battleManagerNode, typeof(DialogBase));
			DialogBase.StaticRelease();
			battleManagerNode.RemoveInstance(battleLog);
			battleManagerNode.RemoveInstance(battleCameraEffect);
			battleManagerNode.RemoveInstance(battleEffectPool);
			battleManagerNode.RemoveInstance(battleTimeScale);
			battleManagerNode.RemoveInstance(battleUnionBurstController);
			battleManagerNode.RemoveInstance(this);
			battleLog.Release();
			battleLog = null;
			battleCameraEffect.Release();
			battleCameraEffect = null;
			battleEffectPool.Release();
			battleEffectPool = null;
			battleTimeScale.Release();
			battleTimeScale = null;
			battleUnionBurstController.Release();
			battleUnionBurstController = null;
			battleManagerNode = null;
			BattleVoiceUtility.StaticRelease();
			singletonTree = null;
			componentTalent = null;
			isValid = false;
		}

		[global::OpenPCR.Migration("40747b1c-2cb5-4263-8736-77feb66ce6e7")]
		public void Init(ViewBattle _viewBattle)
		{
			StartCoroutine(coroutineStartProcess(_viewBattle));
		}

		[global::OpenPCR.Migration("acb0fdac-02c2-452b-982c-4d964a45cea3")]
		private IEnumerator coroutineStartProcess(ViewBattle _viewBattle)
		{
			viewBattle = _viewBattle;
			BattleCameraEffect camera = battleCameraEffect;
			int height = Screen.height;
			int width = Screen.width;
			camera.CurrentZoom = ((float)height * 2.0444443f) / (float)width;
			viewBattle.TransformEffectCtrl.SetHalfDurationCallback(incrementWave);
			viewBattle.TransformEffectCtrlForBoss.SetHalfDurationCallback(incrementWave);
			// Audio-listener outputs have no supplied battle callback; camera/RNG state below remains.
			playCamera.orthographicSize = battleCameraEffect.CurrentZoom;
			playCameraPos0 = playCamera.transform.position;
			tempRandomState = global::OpenPCR.Runtime.Api.Random.state;
			int seed = battleProcessor.GetSeed();
			global::OpenPCR.Runtime.Api.Random.InitState(seed);
			ActionStartTimeCounter = battleProcessor.GetActionStartTime();
			specialBattleStartStoryCounter = battleProcessor.GetStoryStartTime();
			hasReservedTimeStartStory = battleProcessor.HasTimeStartStoryId();
			int currentWave = 0;
			int background = tempData.BattleStageData.CurrentBattleBackgrounds[currentWave];
			if (tempData.BattleSkipState != eBattleSkipState.SKIPPING) addLoadDamageNumEffects();
			if (tempData.BattleSkipState != eBattleSkipState.SKIPPING || IsTowerContent) addLoadLandscapeResources(background);
			TimeLimit = IsDefenseBattle ? TakeOverTimeForDefense : tempData.BattleStageData.BattleTimeLimit;
			startRemainTime = TimeLimit;
			SingletonMonoBehaviour<BattleHeaderController>.Instance.SetRestTime(TimeLimit);
			initializePassiveSkill();
			initializeTeamEnhanceInfo();
			UnitUiCtrl.Initialize(tempData.SelectedPartyType);
			while (!UnitUiCtrl.IsUnitCreated) yield return null;
			calcTalentFormationBonus();
			for (int i = 0; i < UnitList.Count; i++)
				UnitList[i].ComponentPassiveSkill.SetTalentFormationBonus(UnitTalentFormationBonusDictionary);
			UnitUiCtrl.SetTalentFormationBonus(UnitTalentFormationBonusDictionary);
			yield return InitializeEnemyFirst();
			// Pure wave viewport scale output is omitted.
			IsEnemyCreated = true;
			stageBgTex.SetActiveWithCheck(true);
			sortAndSetPositionOneWave(currentEnemyUnitCtrlDictionary);
			if (BossUnit != null) resetAllUnitY();
			if (IsDefenseReplayMode) throw new global::OpenPCR.OutOfBoundaryException("Defense replay scene production is outside clan/training scope.");
			setupEnemyProcess();
			while (!enemySetUp) yield return null;
			checkBossBattle(currentEnemyUnitCtrlDictionary);
			for (int i = 0; i < EnemyList.Count; i++) EnemyList[i].WaveStartProcess(true);
			battleProcessor.SetupHpAndEnergy();
			MultiTimeEffectTimer = battleProcessor.GetMultiTargetEffectTimer();
			if (MultiTimeEffectTimer <= 0f) multiTargetEffectDone = true;
			bool isResourceLoaded = false;
			BattleResourceManager resources = Singleton<BattleResourceManager>.Instance;
			if ((int)BattleCategory == 51 || IsAcnUnknownBattle)
			{
				BattleUnitLoader.AddLoadAcnEffectAndPrefab();
				resources.LoadAcnEffectPrefab();
			}
			for (int i = 0; i < UnitList.Count; i++) resources.LoadMultiUnitAbnormalEffectPrefab(UnitList[i].UnitId);
			for (int i = 0; i < EnemyList.Count; i++) resources.LoadMultiUnitAbnormalEffectPrefab(EnemyList[i].UnitId);
			resourceManager.StartLoad(() => isResourceLoaded = true);
			yield return new WaitUntil(() => isResourceLoaded);
			for (int i = 0; i < UnitList.Count; i++)
			{
				UnitCtrl unit = UnitList[i];
				unit.WaveStartProcess(true);
				unit.ComponentDamageControl.ResetChargedFlagByAtk();
				UnitComponentState state = unit.ComponentState;
				state.SetState(state.IdleOnly || state.WaveStartIdle ? ActionState.IDLE : ActionState.WALK, 0, 0, false);
			}
			for (int i = 0; i < EnemyList.Count; i++)
			{
				UnitCtrl enemy = EnemyList[i];
				enemy.ComponentDamageControl.ResetChargedFlagByAtk();
				UnitComponentState state = enemy.ComponentState;
				if (state.StartStateIsWalk)
					state.SetState(state.IdleOnly || state.WaveStartIdle ? ActionState.IDLE : ActionState.WALK, 0, 0, false);
			}
			battleProcessor.SetupHpAndEnergyAfterApplyPassiveSkill();
			tempData.InitXorShiftRandom(seed);
			BattleVoiceFlagMgr = new BattleVoiceFlagManager(GetMyUnitList(), GetOpponentUnitList());
			List<UnitCtrl> startVoiceUnits = UnitList.Where(unit => !unit.IsGuest).ToList();
			startVoiceUnits[HeldRandom(0, startVoiceUnits.Count)].ComponentState.IsStartVoicePlay = true;
			for (int i = 0, count = UnitList.Count; i < count; i++) UnitList[i].ExecActionOnStartAndDetermineInstanceID();
			for (int i = 0, count = EnemyList.Count; i < count; i++) EnemyList[i].ExecActionOnStartAndDetermineInstanceID();
			StartExtraEffect(BattleDefine.eExtraEffectExecTiming.EX_SKILL);
			if (RelicEnemyUnit != null) RelicEnemyUnit.ComponentRelicSkill.TryExec(0f);
			tempData = Singleton<TempData>.Instance;
			long mainPlayerViewerId = Singleton<UserData>.Instance.UserInfo.ViewerId;
			if (battleProcessor.IsFirstWave())
			{
				tempData.UnitDamageInfoList.Clear();
				UnitCtrl[] unitCtrls = UnitUiCtrl.UnitCtrls;
				foreach (UnitCtrl unit in unitCtrls)
				{
					if (unit == null) continue;
					int unitId = unit.OriginalUnitId;
					long viewerId = BattleUtil.IsQuestSupportUnit(BattleCategory, unitId)
						? tempData.SupportQuestStartParam.ViewerId
						: battleProcessor.GetMainPlayerViewerId(mainPlayerViewerId, unitId);
					UnitComponentDamageControl damageControl = unit.ComponentDamageControl;
					damageControl.UnitDamageInfo = tempData.CreateDamageData(unitId, viewerId,
						unit.UnitParameter.UniqueData.GetCurrentRarity(), false, 0);
					damageControl.UnitDamageInfo.SetSkinData(unit.UnitParameter.UniqueData.SkinData);
					if (!battleProcessor.IsInitialMode())
					{
						UnitDamageInfo info = damageControl.UnitDamageInfo;
						info.SetDamage(battleProcessor.GetUnitDamageInfos().Find(e => e.unit_id == unitId).damage);
					}
				}
				UnitCtrl guest = UnitUiCtrl.GuestUnit;
				if (guest != null)
				{
					MasterEnemyParameter.EnemyParameter parameter = ManagerSingleton<MasterDataManager>.Instance.masterEnemyParameter.GetFromAllKind(guest.UnitId);
					UnitComponentDamageControl damageControl = guest.ComponentDamageControl;
					TempData data = tempData;
					int unitId = parameter.unit_id;
					long viewerId = battleProcessor.GetMainPlayerViewerId(mainPlayerViewerId, guest.UnitId);
					damageControl.UnitDamageInfo = data.CreateDamageData(unitId, viewerId,
						guest.UnitParameter.UniqueData.GetCurrentRarity(), false, 0);
				}
			}
			else
			{
				UnitCtrl[] unitCtrls = UnitUiCtrl.UnitCtrls;
				for (int i = 0; i < unitCtrls.Length; i++)
				{
					if (unitCtrls[i] == null) continue;
					UnitComponentDamageControl damageControl = unitCtrls[i].ComponentDamageControl;
					damageControl.UnitDamageInfo = tempData.UnitDamageInfoList.Find(a =>
						a.unit_id == unitCtrls[i].OriginalUnitId && a.viewer_id == mainPlayerViewerId);
				}
			}
			tempData.EnemyDamageInfoList.Clear();
			tempData.EnemyColorIdList.Clear();
			if (battleProcessor.IsFirstWave())
			{
				tempData.UnitUnionBurstTimelineList.Clear();
				foreach (UnitCtrl unit in UnitList) unit.UnitUnionBurstTimelineList = tempData.UnitUnionBurstTimelineList;
				foreach (UnitCtrl enemy in EnemyList) enemy.UnitUnionBurstTimelineList = tempData.UnitUnionBurstTimelineList;
			}
			battleProcessor.PrepareBattle();
			BattleReady = true;
			yield return new WaitUntil(() => viewBattle.StartFadeInCalled);
			enabled = true;
			battleProcessor.CleanUpData();
			// BGM playback and audio sequence-block outputs are outside battle inference.
			if (ManagerSingleton<ViewManager>.Instance.IsSnsBattlePause)
			{
				GamePause(true, false);
				ManagerSingleton<ViewManager>.Instance.IsSnsBattlePause = false;
			}
		}


		[global::OpenPCR.Migration("a51d6e9d-6620-4938-9756-cfc276a8b3f7")]
		public List<UnitCtrl> GetMyUnitList()
		{
			if ((BattleCategory != eBattleCategory.ARENA_REPLAY && BattleCategory != eBattleCategory.GRAND_ARENA_REPLAY) || !IsDefenseReplayMode)
			{
				return UnitList;
			}

			return EnemyList;
		}

		[global::OpenPCR.Migration("9391d321-1e6a-4dce-a861-2ae7bfa2c325")]
		public List<UnitCtrl> GetOpponentUnitList()
		{
			if ((BattleCategory != eBattleCategory.ARENA_REPLAY && BattleCategory != eBattleCategory.GRAND_ARENA_REPLAY) || !IsDefenseReplayMode)
			{
				return EnemyList;
			}

			return UnitList;
		}

		[global::OpenPCR.Migration("ca11fda7-3add-4f24-ae49-b68b219a2bd9")]
		private void initializePassiveSkill()
		{
			UnitPassiveDictionaryByUnit = new List<Dictionary<eParamType, PassiveActionValue>>
			{
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer()),
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer()),
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer())
			};
			EnemyPassiveDictionaryByUnit = new List<Dictionary<eParamType, PassiveActionValue>>
			{
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer()),
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer()),
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer())
			};
			UnitPassiveDictionaryByEnemy = new List<Dictionary<eParamType, PassiveActionValue>>
			{
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer()),
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer()),
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer())
			};
			EnemyPassiveDictionaryByEnemy = new List<Dictionary<eParamType, PassiveActionValue>>
			{
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer()),
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer()),
				new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer())
			};
			passiveSkillInitializer = new PassiveSkillInitializer(UnitPassiveDictionaryByUnit, UnitPassiveDictionaryByEnemy, EnemyPassiveDictionaryByUnit, EnemyPassiveDictionaryByEnemy);
			int i = 0;
			for (int count = tempData.BattleStageData.CurrentBattleWaves.Count; i < count; i++)
			{
				InitializePassiveSkillUnitList(tempData.BattleStageData.CurrentBattleWaves[i], ePassiveInitType.ALL_BY_ENEMY, i);
			}
		}

		[global::OpenPCR.Migration("706dc31e-48aa-4f11-9c83-dabe50a1f314")]
		private void initializeTeamEnhanceInfo()
		{
			TempData.BattleStage stage = tempData.BattleStageData;
			LabyrinthEnhanceParameter labyrinth = IsLabyrinth ? stage.LabyrinthEnhanceParameter : null;
			UnitExtraEnhanceInfo = new ExtraEnhanceParameterContainer(stage.UnitTeamEnhanceInfo, labyrinth);
			EnemyExtraEnhanceInfo = new ExtraEnhanceParameterContainer(stage.EnemyTeamEnhanceInfo, null);
		}

		[global::OpenPCR.Migration("78070dc4-a9e6-4bac-9a70-8eb0447212e5")]
		public void InitializePassiveSkillUnitList(List<UnitParameter> _unitList, ePassiveInitType _passiveInitType, int _waveNumber)
		{
			int count = _unitList.Count;
			for (int i = 0; i < count; i++)
				passiveSkillInitializer.InitializeExAndFreeSkill(_unitList[i].UniqueData, _passiveInitType, _waveNumber, null);
		}

		[global::OpenPCR.Migration("9a8233bb-8886-4feb-943c-310412276172")]
		public void InitializeExAndFreeSkill(UnitData _unitData, ePassiveInitType _passiveInitType, int _waveNumber, UnitCtrl _unitController)
		{
			global::Elements.Battle.Core.PassiveSkillInitializer nativeValue1 = (this).passiveSkillInitializer;
			if (global::System.Object.ReferenceEquals(nativeValue1, null))
				throw new global::System.NullReferenceException();
			(nativeValue1).InitializeExAndFreeSkill((global::Elements.UnitData)(@_unitData), unchecked((global::Elements.Battle.Core.ePassiveInitType)(unchecked((uint)(@_passiveInitType)))), unchecked((int)(unchecked((uint)(@_waveNumber)))), (global::Elements.UnitCtrl)(@_unitController));
		}

		[global::OpenPCR.Migration("1841ca22-cfdf-4fd7-a85a-b2bd60e8ea6d")]
		private IEnumerator InitializeEnemyFirst()
		{
			TempData.BattleStage battleStageData = tempData.BattleStageData;
			int count = battleStageData.CurrentBattleWaves.Count;
			allWaveEnemyUnitCtrlListArray = new List<UnitCtrl>[count];
			for (int i = 0; i < count; i++)
			{
				allWaveEnemyUnitCtrlListArray[i] = new List<UnitCtrl>();
			}
			for (int j = 0; j < count; j++)
			{
				bool isCallbackCalled = false;
				loadEnemy(j, delegate
				{
					isCallbackCalled = true;
				});
				while (!isCallbackCalled)
				{
					yield return null;
				}
			}
			setupCurrentEnemy(0);
		}

		[global::OpenPCR.Migration("22083e09-64b5-4212-87cb-9462272e592d")]
		private void setupCurrentEnemy(int _currentWaveIndex)
		{
			for (int i = 0; i < allWaveEnemyUnitCtrlListArray.Length; i++)
			{
				if (i == _currentWaveIndex)
				{
					continue;
				}
				for (int j = 0; j < allWaveEnemyUnitCtrlListArray[i].Count; j++)
				{
					UnitCtrl unitCtrl = allWaveEnemyUnitCtrlListArray[i][j];
					if (unitCtrl != null)
					{
						unitCtrl.SetActive(_isActive: false);
					}
				}
			}
			Dictionary<int, UnitCtrl> dictionary = new Dictionary<int, UnitCtrl>();
			for (int k = 0; k < allWaveEnemyUnitCtrlListArray[_currentWaveIndex].Count; k++)
			{
				UnitCtrl unitCtrl2 = allWaveEnemyUnitCtrlListArray[_currentWaveIndex][k];
				unitCtrl2.SetActive(_isActive: true);
				dictionary.Add(k, unitCtrl2);
			}
			currentEnemyUnitCtrlDictionary = dictionary;
		}

		[global::OpenPCR.Migration("98456056-4fcc-4abd-99bc-242f78fa8a64")]
		private void loadEnemy(int _waveIndex, Action finishCallback)
		{
			TempData.BattleStage stage = tempData.BattleStageData;
			List<UnitParameter> deck = null;
			battleProcessor.GetEnemyAtWaveIdx(_waveIndex, ref deck);
			List<UnitParameter> extraDeck = battleProcessor.GetGenerateExtraEffectEnemy(_waveIndex);
			loadEnemyPrime(_waveIndex, stage.CurrentBattleWaves.Count, deck, extraDeck, stage.CurrentBattleReward,
				stage.CurrentContinuousUnit != null ? stage.CurrentContinuousUnit[_waveIndex] : null, finishCallback);
		}

		[global::OpenPCR.Migration("3bc05154-7346-46b7-819e-0b3b9250ec31")]
		private void loadEnemyPrime(int _waveIndex, int _waveMax, List<UnitParameter> _enemyDeck, List<UnitParameter> _enemyExtraDeck, List<InventoryInfo> _questReward, List<ContinuousUnit> _continuousUnitList, Action _finishCallback)
		{
			List<int> list = new List<int>();
			for (int num = _enemyDeck.Count - 1; num >= 0; num--)
			{
				_ = _enemyDeck[num];
				if (_continuousUnitList == null || !battleProcessor.CheckContinuousUnitDead(_continuousUnitList, _enemyDeck[num].UniqueData))
				{
					list.Add(num);
				}
			}
			List<UnitParameter> convertedEnemyDeck = _enemyDeck.ConvertAll((UnitParameter FNHGFDNICFG) => UnitUtility.MightConvertRarity6Unit(in FNHGFDNICFG));
			List<int> enemyPrefabIdList = new List<int>();
			list.ForEach(delegate (int NEKDADGFOHE)
			{
				enemyPrefabIdList.Add(convertedEnemyDeck[NEKDADGFOHE].MasterData.PrefabId);
			});
			Dictionary<int, UnitCtrl> currentEnemyUnitCtrlDictionay = new Dictionary<int, UnitCtrl>();
			List<UnitCtrl> currentEnemyList = new List<UnitCtrl>();
			allWaveEnemyUnitCtrlListArray[_waveIndex] = currentEnemyList;
			list.ForEach(delegate (int enemyDeckIndex)
			{
				int unitPrefabId = convertedEnemyDeck[enemyDeckIndex].MasterData.PrefabId;
				Singleton<BattleUnitLoader>.Instance.AddLoadResource(unitPrefabId, delegate (GameObject loaded)
				{
					UnitCtrl unit = loadEnemyCallback(loaded, _enemyDeck[enemyDeckIndex].UniqueData, _continuousUnitList);
					currentEnemyUnitCtrlDictionay.Add(enemyDeckIndex, unit);
					currentEnemyList.Add(unit);
				}, true, true);
			});
			int extraEnemyIndex = _enemyDeck.Count;
			extraEnemyList = new List<UnitCtrl>();
			_enemyExtraDeck.ForEach(delegate (UnitParameter extraEnemy)
			{
				int prefabId = extraEnemy.MasterData.PrefabId;
				convertedEnemyDeck.Add(UnitUtility.MightConvertRarity6Unit(in extraEnemy));
				Singleton<BattleUnitLoader>.Instance.AddLoadResource(prefabId, delegate (GameObject loaded)
				{
					UnitCtrl unit = loadEnemyCallback(loaded, extraEnemy.UniqueData, null);
					currentEnemyUnitCtrlDictionay.Add(extraEnemyIndex, unit);
					extraEnemyIndex++;
					currentEnemyList.Add(unit);
					extraEnemyList.Add(unit);
				}, true, true);
			});
			if (IsLabyrinth)
			{
				UnitParameter relic = UnitUtility.CreateUnitParameterFromEnemyParameter(770999901);
				convertedEnemyDeck.Add(UnitUtility.MightConvertRarity6Unit(in relic));
				Singleton<BattleUnitLoader>.Instance.AddLoadResource(relic.MasterData.PrefabId, delegate (GameObject loaded)
				{
					UnitCtrl unit = loadEnemyCallback(loaded, relic.UniqueData, null);
					currentEnemyUnitCtrlDictionay.Add(_enemyDeck.Count + _enemyExtraDeck.Count, unit);
					currentEnemyList.Add(unit);
					RelicEnemyUnit = unit;
				}, true, true);
			}
			resourceManager.StartLoad(delegate
			{
				if (_waveIndex == _waveMax - 1)
				{
					Dictionary<int, UnitCtrl> dictionary = currentEnemyUnitCtrlDictionay;
					int key = convertedEnemyDeck.FindIndex((UnitParameter JNOIHMMFADD) => UnitUtility.JudgeIsBoss(JNOIHMMFADD.MasterData.UnitId));
					if (!dictionary.TryGetValue(key, out BossUnit) && currentEnemyList.Count > 0)
					{
						BossUnit = currentEnemyList[0];
					}
					if (_questReward != null && _questReward.Count > 0)
					{
						MasterItemData masterItemData = ManagerSingleton<MasterDataManager>.Instance.masterItemData;
						RewardList = new List<RewardData>();
						int num2 = 0;
						for (int count = _questReward.Count; num2 < count; num2++)
						{
							InventoryInfo inventoryInfo = _questReward[num2];
							MasterItemData.ItemData value = null;
							bool isRare = masterItemData.dictionary.TryGetValue(inventoryInfo.Id, out value) && (int)value.Type == 18;
							RewardList.Add(new RewardData(inventoryInfo.Id, (int)inventoryInfo.Type, inventoryInfo.Count, isRare));
						}
					}
				}
				List<WaveEnemyInfo> waveEnemyInfo = Singleton<UserData>.Instance.QuestStageParameter.GetWaveEnemyInfo(_waveIndex);
				int remainLoadSpineCount = currentEnemyUnitCtrlDictionay.Count;
				foreach (KeyValuePair<int, UnitCtrl> item in currentEnemyUnitCtrlDictionay)
				{
					int key2 = item.Key;
					UnitCtrl unit = currentEnemyUnitCtrlDictionay[key2];
					UnitParameter unitParameter = convertedEnemyDeck[key2];
					ContinuousUnit continuousUnit = null;
					if (_continuousUnitList != null)
					{
						continuousUnit = battleProcessor.IdentifyContinuous(_continuousUnitList, unitParameter.UniqueData);
					}
					if (waveEnemyInfo != null && waveEnemyInfo.Count > key2)
					{
						battleProcessor.SetupDropItem(unit, waveEnemyInfo[key2]);
					}
					battleProcessor.SetupTreasureBoxEffect(unit);
					eAnimKitType spineType = eAnimKitType.SD_NORMAL_BOUND;
					MasterUnitData.UnitData unitData = ManagerSingleton<MasterDataManager>.Instance.masterUnitData.Get(UnitUtility.GetUnitResourceId(unitParameter.MasterData.UnitId));
					bool otherUser = battleProcessor.IsNecessaryOtherUserInfo();
					int setSkinNo = UnitUtility.GetSetSkinNo(unitParameter.UniqueData.UnitRarity, unitParameter.UniqueData.SkinData, UnitDefine.eSkinType.Sd, otherUser, unit);
					bool flag = setSkinNo >= 6;
					int num3 = (flag ? unitData.cutin1_star6 : unitData.CutIn1);
					if (UnitUtility.IsPersonUnit(unitParameter.MasterData.UnitId) && !IsDefenseReplayMode)
					{
						spineType = ((uint)(BattleCategory - 41) <= 1u || (uint)(BattleCategory - 44) <= 1u || BattleCategory == eBattleCategory.FRIEND) ? eAnimKitType.SD_NORMAL_BOUND : eAnimKitType.SD_SHADOW_BOUND;
						unit.IsShadow = true;
						num3 = (flag ? unitData.cutin2_star6 : unitData.CutIn2);
					}
					else if (unitParameter.MasterData.VisualChangeFlag == 1)
					{
						spineType = eAnimKitType.SD_SHADOW_BOUND;
						unit.IsShadow = true;
						num3 = (flag ? unitData.cutin2_star6 : unitData.CutIn2);
					}
					if (unitParameter.EnemyColor != 0)
					{
						spineType = eAnimKitType.SD_COLOR_ENEMY_BOUND;
					}
					if (num3 != 0 && tempData.BattleSkipState != eBattleSkipState.SKIPPING)
					{
						ManagerSingleton<MovieManager>.Instance.Load(eMovieType.CUT_IN, num3, _isShowSubtitle: false, null, _isWaitPrepare: true, 0L, false, false, false, 0f, 1f, 0);
					}
					int unitId = unitParameter.MasterData.PrefabId;
					GameObject gameObject = new GameObject();
					gameObject.transform.SetParent(currentEnemyUnitCtrlDictionay[key2].transform.TargetTransform, worldPositionStays: false);
					gameObject.name = "rotate_center";
					currentEnemyUnitCtrlDictionay[key2].RotateCenter = gameObject.transform;
					BattleAnimKitController.LoadCreate(spineType, UnitUtility.GetUnitResourceId(unitId), setSkinNo, unitData.prefab_id_battle, gameObject.transform, delegate (BattleAnimKitController JPBMABCKONG)
					{
						JPBMABCKONG.Owner = unit;
						JPBMABCKONG.LoadAnimationIDImmediately(eAnimKitBinaryAnimationId.COMMON_BATTLE);
						JPBMABCKONG.LoadAnimationIDImmediately(eAnimKitBinaryAnimationId.BATTLE);
						unit.CenterBone = JPBMABCKONG.skeleton.FindBone("Center");
						unit.StateBone = JPBMABCKONG.skeleton.FindBone("State");
						JPBMABCKONG.SetAnimeEventDelegateForBattle(delegate
						{
							JPBMABCKONG.IsStopState = true;
						}, -1);
						JPBMABCKONG.SetDropTreasureBoxTime();
						unit.Initialize(unitParameter, true, _waveIndex == 0, false, EnemyExtraEnhanceInfo, JPBMABCKONG, cutInPlaybackType, SummonAction.eSummonType.NONE);
						unit.ComponentUnionBurst.MovieId = num3;
						if (extraEnemyList.Contains(unit))
						{
							unit.BonusEnemyType = UnitCtrl.eBonusEnemyType.EXTRA_EFFECT;
							unit.ComponentAbnormal.IsStealth = true;
						}
						if (unit == RelicEnemyUnit)
						{
							unit.BonusEnemyType = UnitCtrl.eBonusEnemyType.RELIC;
							unit.ComponentAbnormal.IsStealth = true;
							unit.ComponentRelicSkill.Initialize();
						}
						if (unit.IsBoss)
						{
							BossList.Add(unit);
						}
						if (continuousUnit != null)
						{
							unit.ComponentParameter.SetCurrentHp(continuousUnit.Hp);
							unit.ComponentParameter.SetEnergy((double)continuousUnit.Energy, eSetEnergyType.INITIALIZE, null);
						}
						remainLoadSpineCount--;
						if (remainLoadSpineCount == 0) _finishCallback.Call();
					}, unitParameter.EnemyColor);
				}
				resourceManager.StartLoad(null);
			});
		}

		[global::OpenPCR.Migration("32ac4899-881a-4dd2-b34a-2259e026b4a7")]
		private UnitCtrl loadEnemyCallback(GameObject _loadedObject, UnitData _uniqueData, List<ContinuousUnit> _continuousUnitList)
		{
			_loadedObject.transform.parent = enemyParentTransform;
			_loadedObject.transform.localScale = Vector3.one;
			_loadedObject.transform.localPosition = new Vector3(0f, -3000f, 0f);
			UnitCtrl unit = _loadedObject.GetComponent<UnitCtrl>();
			if (_continuousUnitList != null) battleProcessor.UpdateContinuousUnitSkillCounter(_continuousUnitList, _uniqueData, unit);
			battleProcessor.ModifyUnitCtrl(ref unit, _uniqueData);
			unit.Index = 0;
			return unit;
		}

		[global::OpenPCR.Migration("98ec3cff-2e47-4b73-b8a3-f40e0bddb126")]
		public void UpdateSkillCounter(ContinuousUnit _continuousUnit, UnitCtrl _unitCtrl)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::UpdateSkillCounter(Elements.ContinuousUnit,Elements.UnitCtrl):System.Void (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("836d2ca8-07b1-4182-8237-13ff246a7d89")]
		private void loadAnimationTexture()
		{
			resourceManager.RegistCommonAsset(eBundleId.BG_ANIMATION_TEXTURE, 0L);
		}

		[global::OpenPCR.Migration("446c7600-273a-4d98-929f-39b2f3ced6dd")]
		public bool GetForceNormalSD()
		{
			return battleProcessor.GetForceNormalSD();
		}

		[global::OpenPCR.Migration("748f7022-a65d-40f3-86a6-1d4b598dfa1e")]
		private void addLoadDamageNumEffects()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("ccc7a574-1bf3-4aac-9110-c46198234326")]
		private void addLoadLandscapeResources(int _bgId)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("cc3f302c-8bf1-4106-bccb-86bd80ad8931")]
		public bool GetAdvPlayed()
		{
			return waveStartAdvPlayState == eWaveStartAdvState.FINISHED;
		}

		[global::OpenPCR.Migration("c7691c89-21dc-4fec-a5f7-c78ff36fd868")]
		public void CallbackStanbyDone(UnitCtrl _unit)
		{
			if (allStandByDone)
				return;
			for (int i = 0; i < UnitList.Count; i++)
				if (!UnitList[i].ComponentState.StandByDone)
					return;
			allStandByDone = true;
			if (battleProcessor.HasWaveStartStory() || isPlayGuestCutin || hasTalentFormationBonus())
			{
				if (tempData.BattleSkipState != eBattleSkipState.NOT_SKIPPING)
				{
					waveStartAdvPlayState = eWaveStartAdvState.FINISHED;
					IsPausingEffectSkippedInThisFrame = true;
				}
				else
				{
					int storyId = battleProcessor.GetWaveStartStoryId();
					bool playTalent = battleProcessor.IsPlayTalentFormationBonusEffect();
					IsPausingEffectSkippedInThisFrame |= storyId == 0 && !isPlayGuestCutin && !playTalent;
					waveStartAdvPlayState = eWaveStartAdvState.PLAYING;
					componentTalent.IsPlayTalentBonusEffect = playTalent;
					PlayStory(() =>
					{
						battleProcessor.NextWaveStartStoryFinishCallback();
						waveStartAdvPlayState = eWaveStartAdvState.FINISHED;
						playGuestBattle(playTalentBonusEffect);
					}, storyId, false);
				}
			}
			else
				waveStartAdvPlayState = eWaveStartAdvState.FINISHED;
		}

		[global::OpenPCR.Migration("88a74854-37ac-4094-a0bb-2f203dda9bc1")]
		public void CallbackStartDashDone(UnitCtrl _unit)
		{
			if (!battleProcessor.HasWaveStartStory() && !isPlayGuestCutin)
				return;
			_unit.StartDashDone = true;
			for (int i = 0; i < UnitList.Count; i++)
				if (!UnitList[i].ComponentState.IsDead && !UnitList[i].StartDashDone)
					return;
			for (int i = 0; i < UnitList.Count; i++)
				UnitList[i].StartDashDone = false;
			if (waveStartAdvPlayState != eWaveStartAdvState.STAND_BY)
				return;
			if (tempData.BattleSkipState != eBattleSkipState.NOT_SKIPPING)
			{
				waveStartAdvPlayState = eWaveStartAdvState.FINISHED;
				IsPausingEffectSkippedInThisFrame = true;
			}
			else
			{
				int storyId = battleProcessor.GetWaveStartStoryId();
				IsPausingEffectSkippedInThisFrame |= storyId == 0 && !isPlayGuestCutin;
				waveStartAdvPlayState = eWaveStartAdvState.PLAYING;
				PlayStory(() =>
				{
					waveStartAdvPlayState = eWaveStartAdvState.FINISHED;
					playGuestBattle(null);
				}, storyId, false);
			}
		}

		[global::OpenPCR.Migration("de9bdf38-881e-4dc2-8bdf-d4ecba74016c")]
		public void PlayStory(Action _callback, int _storyId, bool _end)
		{
			if (_storyId != 0)
			{
				UnitUiCtrl.HidePopUp();
				GamePause(true, false);
				battleProcessor.PlayStory(
					() => battleProcessor.PlayStoryAfter(_callback, _storyId, _end, viewBattle),
					_storyId, _end, viewBattle);
			}
			else
			{
				battleProcessor.PlayStoryAfter(() => _callback.Call(), 0, _end, viewBattle);
			}
		}

		[global::OpenPCR.Migration("733e5232-9e6c-4038-ba07-c8bca748e3b9")]
		private void playGuestBattle(Action _callback = null)
		{
			if (!isPlayGuestCutin)
			{
				_callback.Call();
				return;
			}
			IsGuestCutinPlaying = true;
			blockLayerManager.SetBlockCommon(true);
			GamePause(_pause: true);
			UnitCtrl unitCtrl = UnitList.Find((UnitCtrl JNOIHMMFADD) => JNOIHMMFADD.IsGuest);
			global::System.Int32 unit_id = ManagerSingleton<MasterDataManager>.Instance.masterEnemyParameter.GetFromAllKind(unitCtrl.UnitId).unit_id;
			if (ResourceManager.IsExistResource(eBundleId.UI_MATERIALSET_GUEST, (int)unit_id))
			{
				resourceManager.LoadAssetBundleImmediately(eBundleId.UI_MATERIALSET_GUEST, (int)unit_id);
			}
			// Guest cut-in renderer creation/viewport scaling omitted; 2s pause timer and completion callback remain real.
			isPlayGuestCutin = false;
			this.Timer(2f, delegate
			{
				IsGuestCutinPlaying = false;
				if (!SingletonMonoBehaviour<BattleHeaderController>.Instance.IsPaused)
				{
					GamePause(_pause: false);
				}
				blockLayerManager.SetBlockCommon(false);
				_callback.Call();
			});
		}

		[global::OpenPCR.Migration("2b4005d7-119b-4ca1-b700-f731b21b4c40")]
		public long GetPurposeCount()
		{
			return battleProcessor.GetPurposeCount();
		}

		[global::OpenPCR.Migration("4c68acb1-bc6e-41dd-b1d2-4ce6daeb5b7e")]
		public eSpecialBattlePurpose GetPurpose()
		{
			return battleProcessor.GetPurpose();
		}

		[global::OpenPCR.Migration("47411b1b-512c-4156-969f-ac5ad5f5f661")]
		public void InstantiatePointCounter(Transform _transform, ref PartsHastunePointCounter _counter)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::InstantiatePointCounter(OpenPCR.Runtime.Api.Transform,ref Elements.PartsHastunePointCounter):System.Void: runtime capability is unavailable."); }

		[global::OpenPCR.Migration("99324e5b-69ea-45c1-8d91-f9a948e3a2f6")]
		public void InstantiateDefenseTimeGauge(Transform _parent, ref PartsHatsuneDefenseTimeGauge _timeGauge)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::InstantiateDefenseTimeGauge(OpenPCR.Runtime.Api.Transform,ref Elements.PartsHatsuneDefenseTimeGauge):System.Void: runtime capability is unavailable."); }

		[global::OpenPCR.Migration("4170a513-2a7d-4bd3-99f3-8af77e892b08")]
		public void InstantiateSpeicalBattleModeWithoutGauge(Transform _parent, ref PartsSpecialBattleMode _specialBattleMode)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::InstantiateSpeicalBattleModeWithoutGauge(OpenPCR.Runtime.Api.Transform,ref Elements.PartsSpecialBattleMode):System.Void: runtime capability is unavailable."); }

		[global::OpenPCR.Migration("edbd57e5-fb67-457b-b3b8-a8f9b33150c0")]
		public int GetEnemyPoint(int _unitId)
		{
			return battleProcessor.GetEnemyPoint(_unitId);
		}

		[global::OpenPCR.Migration("9a50258c-8370-4605-8e9d-8090c552f551")]
		public int GetInitialPosition(int _unitId)
		{
			return battleProcessor.GetInitialPosition(_unitId);
		}

		[global::OpenPCR.Migration("817cab71-6474-45f6-b3e9-220b643d5849")]
		public int GetCurrentWaveOffset()
		{
			return battleProcessor.GetCurrentWaveOffset();
		}

		[global::OpenPCR.Migration("495e54a7-f5c8-46e6-a7f4-dd64b49d84b6")]
		public bool GetBossGaugeColorChange()
		{
			return battleProcessor.GetBossGaugeColorChange();
		}

		[global::OpenPCR.Migration("cc4f6d3e-2e23-44f8-99cd-c7d355a174ac")]
		public float GetBossGaugeValue(long _currentHp)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::GetBossGaugeValue(System.Int64):System.Single (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("a07d7656-f3c3-43ed-a8f7-aab01484f103")]
		public void SetSpecialBattleGaugeColor(UIProgressBar _gauge)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::SetSpecialBattleGaugeColor(UIProgressBar):System.Void (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("a98310bf-1816-4de8-a8c5-e6cef8741ca8")]
		public void SetDamageGoldModeMaxHpAttackNum(ref long _damage, ref int _gold, ref int _mode, ref long _maxHp, ref int _attackNum)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::SetDamageGoldModeMaxHpAttackNum(ref System.Int64,ref System.Int32,ref System.Int32,ref System.Int64,ref System.Int32):System.Void (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("1aaadb8c-cc3b-45f5-8fbe-c01fef2519f1")]
		public void SubstructEnemyPoint(long _point)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::SubstructEnemyPoint(System.Int64):System.Void (outside first-phase point-settlement boundary)"); }

		[global::OpenPCR.Migration("bf39395d-d101-4eb2-ad42-88dd04e49602")]
		public void CheckAcnLosingBattleFinish()
		{
			if (BattleCategory != eBattleCategory.ACN_LOSING_UNKNOWN || wasJudgeAllUnitPuppet) return;
			bool any = false;
			for (int index = 0; index < UnitList.Count; index++)
			{
				UnitCtrl unit = UnitList[index];
				if (!unit.ComponentState.IsDead && !unit.IsSummonOrPhantom && !unit.IsDivision)
				{
					if (!unit.ComponentAbnormal.IsAbnormalState(eAbnormalState.PUPPET)) return;
					any = true;
				}
			}
			if (!any) return;
			for (int index = 0; index < EnemyList.Count; index++) EnemyList[index].ComponentState.IdleOnly = true;
			for (int index = 0; index < UnitList.Count; index++) UnitList[index].ComponentState.IdleOnly = true;
			BattleResult = eBattleResult.WIN;
			wasJudgeAllUnitPuppet = true;
			isPauseTimeLimit = true;
			AppendCoroutine(waitAndFinishBattle(), ePauseType.SYSTEM, null);
		}

		[global::OpenPCR.Migration("bf8dac4c-dc75-42c6-8dac-c7f1e648c239")]
		private IEnumerator waitAndFinishBattle()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::waitAndFinishBattle():System.Collections.IEnumerator (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("1202d627-e293-4958-af04-87fb9af861d3")]
		public void SpecialBattleModeChangeOnHpChange()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::SpecialBattleModeChangeOnHpChange():System.Void (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("60b17e97-6419-4c8b-92a1-8bc5927a4f4f")]
		public void DeterminePositionOrder()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::DeterminePositionOrder():System.Void (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("5c417279-024a-4f98-87f2-d92e8429e32a")]
		public void SetStoryBattleDataPrefab(StoryBattleData _storyBattleDataPrefab)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::SetStoryBattleDataPrefab(Elements.StoryBattleData):System.Void (outside first-phase story boundary)"); }

		[global::OpenPCR.Migration("b1cb365c-b627-4343-beed-6ed217a7e5de")]
		public IEnumerator StoryBattleAnimationUpdate()
		{
			UnitCtrl unitCtrl = EnemyList[0];
			UnitCtrl unitCtrl2 = UnitList[0];
			UnitCtrl unitCtrl3 = ((unitCtrl.ComponentSearchArea.SearchAreaSize < unitCtrl2.ComponentSearchArea.SearchAreaSize) ? unitCtrl2 : unitCtrl);
			unitCtrl3.SetLocalPosX(storyBattleDataPrefab.StartPosition);
			if (storyBattleDataPrefab.IsUseVoiceTimeLine)
			{
				StartCoroutine(storyBattleVoiceTimeLineUpdate());
			}

			if (unitCtrl3.IsOther)
			{
				float num = storyBattleDataPrefab.StartPosition - unitCtrl3.BodyWidth / 2f;
				int i = 0;
				for (int count = UnitList.Count; i < count; i++)
				{
					UnitList[i].SetLocalPosX(num - UnitList[i].ComponentSearchArea.SearchAreaSize - UnitList[i].BodyWidth / 2f);
				}

				num = UnitList[0].transform.localPosition.x + UnitList[0].BodyWidth / 2f;
				int j = 1;
				for (int count2 = EnemyList.Count; j < count2; j++)
				{
					EnemyList[j].SetLocalPosX(num + EnemyList[j].ComponentSearchArea.SearchAreaSize + EnemyList[j].BodyWidth / 2f);
				}
			}
			else
			{
				float num2 = storyBattleDataPrefab.StartPosition + unitCtrl3.BodyWidth / 2f;
				int k = 0;
				for (int count3 = EnemyList.Count; k < count3; k++)
				{
					EnemyList[k].SetLocalPosX(num2 + EnemyList[k].ComponentSearchArea.SearchAreaSize + EnemyList[k].BodyWidth / 2f);
				}

				num2 = EnemyList[0].transform.localPosition.x - EnemyList[0].BodyWidth / 2f;
				int l = 1;
				for (int count4 = UnitList.Count; l < count4; l++)
				{
					UnitList[l].SetLocalPosX(num2 - UnitList[l].ComponentSearchArea.SearchAreaSize - UnitList[l].BodyWidth / 2f);
				}
			}

			int index = 0;
			List<StoryBattleTimeLineData> timeLineData = new List<StoryBattleTimeLineData>(storyBattleDataPrefab.TimeLineData);
			timeLineData.Sort((StoryBattleTimeLineData IPJGCOBNHLB, StoryBattleTimeLineData IMMPDMOKFGC) => IPJGCOBNHLB.Time.CompareTo(IMMPDMOKFGC.Time));
			while (index != timeLineData.Count)
			{
				if ((float)FrameCount * DeltaTime > timeLineData[index].Time)
				{
					execTimeLineData(timeLineData[index]);
					index++;
				}

				yield return null;
			}
		}

		[global::OpenPCR.Migration("2b1c1d81-27a8-4a60-b6fe-a5c77a4dd107")]
		private IEnumerator storyBattleVoiceTimeLineUpdate()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::storyBattleVoiceTimeLineUpdate():System.Collections.IEnumerator (outside first-phase story boundary)"); }

		[global::OpenPCR.Migration("b06f661f-7ae9-4fed-a2ae-dfd5d94b248d")]
		private void execTimeLineData(StoryBattleTimeLineData _timeLineData)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::execTimeLineData(Elements.StoryBattleTimeLineData):System.Void (outside first-phase story boundary)"); }

		[global::OpenPCR.Migration("d75aac7d-8daa-420c-970b-9e4845559ad8")]
		public List<int> GetStoryIdListToStoryBattleData()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::GetStoryIdListToStoryBattleData():System.Collections.Generic.List\u00601\u003CSystem.Int32\u003E (outside first-phase story boundary)"); }

		[global::OpenPCR.Migration("ad698e0a-2457-403e-8ac5-cba6d68eb717")]
		public UnitCtrl Summon(SummonData _summonData)
		{
			UnitCtrl unitCtrl = global::OpenPCR.Runtime.Api.Object.Instantiate(_summonData.Prefab).GetComponent<UnitCtrl>();
			FixedTransformMonoBehavior.FixedTransform ucTransform = unitCtrl.transform;
			ucTransform.parent = enemyParentTransform;
			ucTransform.localScale = Vector3.one;
			SummonAction.eMoveType moveType = ((_summonData.MoveSpeed <= 1f) ? SummonAction.eMoveType.NORMAL : SummonAction.eMoveType.LINEAR);
			if (!(_summonData.MoveSpeed > 1f))
			{
				ucTransform.localPosition = _summonData.TargetPosition;
			}
			else
			{
				ucTransform.localPosition = _summonData.FromPosition;
			}
			int rarity = 0;
			int summonResourceId = UnitUtility.GetSummonResourceId(_summonData.SummonId);
			AnimKitResourceInfo resourceInfo = eAnimKitType.SD_NORMAL_BOUND.GetResourceInfo();
			long num = ResourceManager.CreateIndex(UnitUtility.GetSkinId(eAnimKitType.SD_NORMAL_BOUND, summonResourceId, _summonData.Owner.SDSkin), 0L);
			bool flag = Toolbox.ResourcesManager.IsUseAssetBundle();
			if ((flag && ResourceManager.IsExistResource(resourceInfo.bundleId, num)) || (!flag && ResourceManager.IsExistResource(resourceInfo.skelton, num)))
			{
				rarity = _summonData.Owner.SDSkin;
			}
			eAnimKitType spineType = eAnimKitType.SD_NORMAL_BOUND;
			if (_summonData.Owner.IsShadow && !battleEffectPool.ForceNormalSD)
			{
				resourceInfo = eAnimKitType.SD_SHADOW_BOUND.GetResourceInfo();
				num = ResourceManager.CreateIndex(UnitUtility.GetSkinId(eAnimKitType.SD_SHADOW_BOUND, summonResourceId, rarity), 0L);
				flag = Toolbox.ResourcesManager.IsUseAssetBundle();
				if ((flag && ResourceManager.IsExistResource(resourceInfo.bundleId, num)) || (!flag && ResourceManager.IsExistResource(resourceInfo.skelton, num)))
				{
					spineType = eAnimKitType.SD_SHADOW_BOUND;
				}
			}
			GameObject gameObject = new GameObject();
			gameObject.transform.SetParent(ucTransform.TargetTransform, worldPositionStays: false);
			gameObject.name = "rotate_center";
			unitCtrl.RotateCenter = gameObject.transform;
			BattleAnimKitController.LoadCreateImmidiate(spineType, summonResourceId, rarity, gameObject.transform, delegate (BattleAnimKitController JPBMABCKONG)
			{
				JPBMABCKONG.Owner = unitCtrl;
				JPBMABCKONG.LoadAnimationIDImmediately(eAnimKitBinaryAnimationId.COMMON_BATTLE);
				JPBMABCKONG.LoadAnimationIDImmediately(eAnimKitBinaryAnimationId.BATTLE);
				JPBMABCKONG.SetAnimeEventDelegateForBattle(delegate
				{
					JPBMABCKONG.IsStopState = true;
				}, -1);
				UnitParameter unitParameter = null;
				unitParameter = ((!UnitUtility.IsQuestMonsterUnit(_summonData.SummonId)) ? UnitUtility.CreateSummonUnitParameter(_summonData.Owner.UnitParameter, _summonData.SummonId, _summonData.Skill.Level, _summonData.ConsiderEquipmentAndBonus, _summonData.Owner.MainSkill1Evolved) : UnitUtility.CreateUnitParameterFromEnemyParameter(_summonData.SummonId));
				bool flag2 = ((_summonData.SummonSide == SummonAction.eSummonSide.OURS) ? _summonData.Owner.IsOther : (!_summonData.Owner.IsOther));
				unitCtrl.CenterBone = JPBMABCKONG.skeleton.FindBone("Center");
				unitCtrl.StateBone = JPBMABCKONG.skeleton.FindBone("State");
				unitCtrl.Initialize(unitParameter, flag2, true, false, flag2 ? EnemyExtraEnhanceInfo : UnitExtraEnhanceInfo, JPBMABCKONG, cutInPlaybackType, _summonData.SummonType);
				unitCtrl.ComponentParameter.InitParamValue(true);
				if (flag2)
				{
					unitCtrl.ComponentPassiveSkill.SetTalentFormationBonus(null);
					unitCtrl.ComponentPassiveSkill.ApplyPassiveSkill(true, _summonData.ConsiderEquipmentAndBonus);
					// onDieEnemy gold/treasure presentation excluded; core death state remains above.
					EnemyList.Add(unitCtrl);
				}
				else
				{
					unitCtrl.ComponentPassiveSkill.SetTalentFormationBonus(UnitTalentFormationBonusDictionary);
					unitCtrl.ComponentPassiveSkill.ApplyPassiveSkill(true, _summonData.ConsiderEquipmentAndBonus);
					UnitList.Add(unitCtrl);
				}
				unitCtrl.ExecActionOnStartAndDetermineInstanceID();
				unitCtrl.SetLeftDirection(_summonData.Owner.IsLeftDir);
				int skillNum = _summonData.Skill.SkillNum;
				if (unitCtrl.SummonEffects.Count > 0)
				{
					unitCtrl.AppendCoroutine(unitCtrl.ComponentState.CreatePrefabWithTime(unitCtrl.SummonEffects, false, skillNum, false, false, false, -1, false), ePauseType.VISUAL, (_summonData.Skill.BlackOutTime <= 0f) ? null : _summonData.Owner);
				}
				eUnitRespawnPos eUnitRespawnPos2 = eUnitRespawnPos.MAIN_POS_1;
				switch (_summonData.SummonType)
				{
					case SummonAction.eSummonType.SUMMON:
					case SummonAction.eSummonType.PHANTOM:
						eUnitRespawnPos2 = ((!_summonData.UseRespawnPos) ? SearchRespawnPos(_summonData.Owner.RespawnPos, flag2 ? EnemyList : UnitList, _summonData.SummonId) : _summonData.RespawnPos);
						break;
					case SummonAction.eSummonType.DIVISION:
						eUnitRespawnPos2 = _summonData.RespawnPos;
						break;
				}
				unitCtrl.SummonRespawnPos = eUnitRespawnPos2;
				if (unitCtrl.GetCurrentSpineCtrl().IsAnimation(eAnimKitCharacterAnimeId.SUMMON, skillNum) || moveType == SummonAction.eMoveType.LINEAR)
				{
					unitCtrl.RespawnPos = _summonData.Owner.RespawnPos;
					if (unitCtrl.GetCurrentSpineCtrl().IsAnimation(eAnimKitCharacterAnimeId.SUMMON, skillNum))
					{
						unitCtrl.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.SUMMON, skillNum, -1, -1, _isLoop: false);
					}
					if (_summonData.Skill.BlackOutTime > 0f && BlackOutUnitList.Count > 0)
					{
						unitCtrl.ComponentSortOrder.SetSortOrderFront();
						BlackOutUnitList.Add(unitCtrl);
						AppendCoroutine(unitCtrl.UpdateSummon(skillNum, unitCtrl.SummonRespawnPos, moveType, _summonData.TargetPosition, _summonData.MoveSpeed), ePauseType.SYSTEM, _summonData.Owner);
					}
					else
					{
						unitCtrl.ComponentSortOrder.SetSortOrderBack();
						AppendCoroutine(unitCtrl.UpdateSummon(skillNum, unitCtrl.SummonRespawnPos, moveType, _summonData.TargetPosition, _summonData.MoveSpeed), ePauseType.SYSTEM, null);
					}
					unitCtrl.ComponentSortOrder.SortOrder--;
				}
				else
				{
					unitCtrl.BattleStartProcess(eUnitRespawnPos2);
					ucTransform.SetLocalPosY(GetRespawnPos(eUnitRespawnPos2));
					unitCtrl.ComponentState.SetState(ActionState.WALK, 0, 0, false);
				}
			});
			resourceManager.StartLoad(null);
			return unitCtrl;
		}

		[global::OpenPCR.Migration("4e64eefd-9888-49e7-8d56-a4a256bd888b")]
		public eUnitRespawnPos SearchRespawnPos(eUnitRespawnPos _ownerRespawnPos, List<UnitCtrl> _unitList, int _unitId)
		{
			eUnitRespawnPos result = eUnitRespawnPos.MAIN_POS_3;
			for (int i = 0; i < BattleDefine.SUMMON_RESPAWN_PRIORITY[_ownerRespawnPos].Count; i++)
			{
				eUnitRespawnPos respawnPos = BattleDefine.SUMMON_RESPAWN_PRIORITY[_ownerRespawnPos][i];
				if (i % 2 == 1)
				{
					eUnitRespawnPos evenRPos = BattleDefine.SUMMON_RESPAWN_PRIORITY[_ownerRespawnPos][i - 1];
					if (_unitList.FindIndex((UnitCtrl DEGHPOFNMGC) => DEGHPOFNMGC.SummonRespawnPos == evenRPos) != -1)
					{
						if (_unitList.Find((UnitCtrl DEGHPOFNMGC) => DEGHPOFNMGC.SummonRespawnPos == evenRPos).UnitId == _unitId)
						{
							continue;
						}
					}
					else if (_unitList.Find((UnitCtrl DEGHPOFNMGC) => DEGHPOFNMGC.RespawnPos == evenRPos).UnitId == _unitId)
					{
						continue;
					}
				}

				if (_unitList.FindIndex((UnitCtrl DEGHPOFNMGC) => DEGHPOFNMGC.RespawnPos == respawnPos) == -1 && _unitList.FindIndex((UnitCtrl DEGHPOFNMGC) => DEGHPOFNMGC.SummonRespawnPos == respawnPos) == -1)
				{
					result = respawnPos;
					break;
				}
			}

			return result;
		}

		[global::OpenPCR.Migration("670ff796-5797-46b6-a7a9-bf8f5ea5e8a1")]
		public void SetStartSkillExec(bool _flag)
		{
			IsStartSkillExec = _flag;
		}

		[global::OpenPCR.Migration("0ae1c122-3429-4368-88cc-596491ea90ad")]
		public void TutorialSkillExec(GameObject _focus)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::TutorialSkillExec(OpenPCR.Runtime.Api.GameObject):System.Void: runtime capability is unavailable."); }

		[global::OpenPCR.Migration("eecc8955-0c9d-4f79-8696-1a5b6e33386f")]
		public void TutorialOpenFocusDialog()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::TutorialOpenFocusDialog():System.Void (outside first-phase tutorial boundary)"); }

		[global::OpenPCR.Migration("8b86842d-0896-4f8b-b994-ca73b60f3bc0")]
		public bool IsVoiceDownLoad()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleManager::IsVoiceDownLoad():System.Boolean (outside first-phase tutorial boundary)"); }

		[global::OpenPCR.Migration("d5bb74ef-d643-4edd-9802-d87d11fa22bb")]
		Coroutine IBattleManagerForSkillEffectCtrl.StartCoroutine(IEnumerator _routine)
		{
			return base.StartCoroutine(_routine);
		}

		Coroutine IBattleManagerForAbromalStateIconController.StartCoroutine(IEnumerator _routine)
		{
			return base.StartCoroutine(_routine);
		}

		void IBattleManagerForAbromalStateIconController.StopCoroutine(IEnumerator _routine)
		{
			base.StopCoroutine(_routine);
		}

		Coroutine IBattleManagerForLifeGaugeController.StartCoroutine(IEnumerator _routine)
		{
			return base.StartCoroutine(_routine);
		}

		[global::OpenPCR.Migration("2fc56b85-2919-4e36-9fad-d70ffd828229")]
		Coroutine IBattleManagerForActionParameter.StartCoroutine(IEnumerator routine)
		{
			return base.StartCoroutine(routine);
		}

		[global::OpenPCR.Migration("86450516-53fd-4b8f-843d-d16a50e8e72d")]
		Coroutine IBattleManagerForUnitActionController.StartCoroutine(IEnumerator routine)
		{
			return base.StartCoroutine(routine);
		}

		[global::OpenPCR.Migration("e1427101-8233-4782-8c40-31acfc81c7a6")]
		Coroutine IBattleManagerForBaseBattleProcessor.StartCoroutine(IEnumerator routine)
		{
			return base.StartCoroutine(routine);
		}

		private EnvironmentData unitEnvironment { get; set; }
		private EnvironmentData enemyEnvironment { get; set; }


		private int allUnitSealDamageCutCounter { get; set; }

		private PassiveSkillInitializer passiveSkillInitializer;
		private BattleComponentTalent componentTalent;
		private long allLogBarrierEffectValue;
		private long allLogBarrierStartValue;
		private bool isReleaseSlipDamageLogBarrier;
		private BattleDefineData battleDefineData;
		public ExtraEnhanceParameterContainer UnitExtraEnhanceInfo { get; private set; }
		public ExtraEnhanceParameterContainer EnemyExtraEnhanceInfo { get; private set; }

		[global::OpenPCR.Migration("0aa06f89-bece-4d31-8e80-fd56bfe943fb")]
		private void adjustEnemyDamageInfo()
		{
			List<UnitDamageInfo> list = tempData.EnemyDamageInfoList;
			List<int> colors = tempData.EnemyColorIdList;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				int id = list[i].unit_id;
				int rarity = list[i].rarity;
				UnitCtrl unit = FindEnemy(UnitUtility.GetConversionUnit(id, rarity));
				if (unit == null || unit.IsBonusEnemy)
				{
					list.RemoveAt(i);
					colors.RemoveAt(i);
				}
			}
		}

		[global::OpenPCR.Migration("736ab7c1-fcd0-44bb-9844-f30eaa5a5a63")]
		private void adjustUnitDamageInfo()
		{
			List<UnitDamageInfo> list = tempData.UnitDamageInfoList;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				int id = list[i].unit_id;
				int rarity = list[i].rarity;
				UnitCtrl unit = FindUnit(UnitUtility.GetConversionUnit(id, rarity));
				if (unit == null || unit.IsBonusEnemy)
					list.RemoveAt(i);
			}
		}


		[global::OpenPCR.Migration("d1fd7e24-4fcf-4d8e-9fd7-19a3a893132d")]
		private float getBattleElapsedFloatTime()
		{
			return startRemainTime - TimeLimit;
		}

		private bool hasReservedTimeStartStory { get; set; }

		[global::OpenPCR.Migration("dd0fff07-9fdb-4ae2-9885-5956aa60fa78")]
		public UnitCtrl FindUnit(int _id)
		{
			UnitCtrl found = UnitList.Find(unit => unit.UnitId == _id);
			int guestIndex = UnitList.FindIndex(unit => unit.IsGuest);
			if (guestIndex >= 0 && found == null)
			{
				UnitCtrl guest = GetUnitCtrl(guestIndex);
				var row = ManagerSingleton<MasterDataManager>.Instance.masterEnemyParameter.GetFromAllKind(guest.UnitId);
				if (row != null && (row.unit_id) == _id) return guest;
				return null;
			}
			return found;
		}

		private enum eWaveStartAdvState
		{
			INVALID,
			STAND_BY,
			PLAYING,
			FINISHED
		}

		[global::OpenPCR.Migration("a7fe5713-f36d-4bf1-9e84-1018aedd56c9")]
		private bool hasTalentFormationBonus()
		{
			foreach (int talent in ManagerSingleton<MasterDataManager>.Instance.masterTalent.GetTalentIdListOrderByAsc())
				if (battleProcessor.GetTalentBonusPermyriad(talent, 5) >= 1L)
					return true;
			return false;
		}

		[global::OpenPCR.Migration("6b2fc512-e452-4a4c-8100-49988adf9594")]
		private void playTalentBonusEffect()
		{
			if (!componentTalent.IsPlayTalentBonusEffect)
				return;
			List<int> talents = UnitTalentFormationBonusDictionary.Where(pair => pair.Value > 0L).Select(pair => pair.Key).ToList();
			blockLayerManager.SetBlockCommon(true);
			GamePause(true, false);
			UnitUiCtrl.SetTalentFormationBonusActive(false);
			UnitUiCtrl.HidePopUp();
			SetSkillScreen(true);
			componentTalent.PlayTalentBonusEffect(talents, viewBattle.transform, UnitUiCtrl.TalentBonusIconParentTransform, () => UnitUiCtrl.SetTalentFormationBonusActive(true), () =>
			{
				SetSkillScreen(false);
				if (!SingletonMonoBehaviour<BattleHeaderController>.Instance.IsPaused)
					GamePause(false, false);
				blockLayerManager.SetBlockCommon(false);
			});
		}

		public Dictionary<int, long> UnitTalentFormationBonusDictionary { get; set; }
		private eWaveStartAdvState waveStartAdvPlayState { get; set; }

		[global::OpenPCR.Migration("ca84fafc-9af8-4bbb-a434-7f0609bd6fe9")]
		public bool CanPlayCutIn()
		{
			return !battleTimeScale.IsSpeedQuadruple && !IsQuestAutoProgressing();
		}

		public int CountIsNotPartsBossReady { get; set; }

		[global::OpenPCR.Migration("feb63ffb-43ac-45b4-9ce7-eb69144f675a")]
		public Dictionary<eParamType, PassiveActionValue> GetPassiveDictionaryByEnemy(bool _isOther)
		{
			return (_isOther ? EnemyPassiveDictionaryByEnemy : UnitPassiveDictionaryByEnemy)[CurrentWave];
		}

		[global::OpenPCR.Migration("aa7124a5-29c4-4747-8c35-30835febb0fd")]
		public Dictionary<eParamType, PassiveActionValue> GetPassiveDictionaryByUnit(bool _isOther)
		{
			return (_isOther ? EnemyPassiveDictionaryByUnit : UnitPassiveDictionaryByUnit)[0];
		}

		[global::OpenPCR.Migration("578ce226-82c4-46be-b7bb-54a2411639e3")]
		public bool IsInitialMode()
		{
			return battleProcessor.IsInitialMode();
		}

		public List<Dictionary<eParamType, PassiveActionValue>> UnitPassiveDictionaryByUnit { get; private set; }
		public List<Dictionary<eParamType, PassiveActionValue>> EnemyPassiveDictionaryByUnit { get; private set; }
		public List<Dictionary<eParamType, PassiveActionValue>> EnemyPassiveDictionaryByEnemy { get; private set; }
		public List<Dictionary<eParamType, PassiveActionValue>> UnitPassiveDictionaryByEnemy { get; private set; }
		public float LastDamageVoiceTime { get; set; }
		public float LastRetireVoiceTime { get; set; }

		[global::OpenPCR.Migration("a1d330b1-259f-403d-9523-b6b63a390da0")]
		public void ResetThanksTargetUnit()
		{
			for (int i = 0; i < UnitList.Count; i++)
				UnitList[i].ComponentUnionBurst.ThanksTargetUnitId = 0;
		}
		[global::OpenPCR.Migration("96e9dc58-5d80-4654-b24c-7217904dbdfc")]
		private void onChangeBattleCameraScale(bool _cameraOnly)
		{
			global::OpenPCR.Presentation.Noop();
		}
		public bool IsPlayedFirstWaveStartStory;
	}
}
