using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
using global::OpenPCR.AnimKit.Scene.Modules;
using global::OpenPCR.Runtime.Api;
using System.Linq;
using global::OpenPCR.Foundation;
using global::OpenPCR.Foundation.Audio.Movie;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle;
using Elements.Battle.Core;
using Elements.Data;

namespace Elements
{
	public class UnitCtrl : FixedTransformMonoBehavior, ISingletonField, IUnitComponentContainer
	{

		[global::OpenPCR.Migration("bb230f22-d337-4335-8913-9f73bdfd9b0f")]
		public bool JudgeCanPlayVoice(global::Elements.SoundManager.eVoiceType _voiceType, int _enemyVoiceId)
		{
			if (ComponentAbnormal.ToadDatas.Count > 0) return false;
			if (IsShadow && !BattleDefine.VoiceJudgeDataDictionary[_voiceType].PlayIfShadow) return false;
			if (!battleManager.CanPlayVoice()) return false;
			if ((!battleManager.IsDefenseReplayMode && _enemyVoiceId != 0 && IsOther) || IsPlayerUnit) return true;
			return BattleDefine.VoiceJudgeDataDictionary[_voiceType].PlayIfOther;
		}


		[global::OpenPCR.Migration("0d72070b-9818-4147-9351-37292697c9f5")]
		public global::Elements.ContinuousUnit SearchContinuousUnit()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.UnitCtrl::SearchContinuousUnit():Elements.ContinuousUnit (outside supported first-phase lifecycle/input scope)"); }

		public Dictionary<int, List<(SkillEffectCtrl effect, string boneName)>> DamagedHpEffectDictionary { get; set; } = new Dictionary<int, List<(SkillEffectCtrl effect, string boneName)>>();

		public Dictionary<int, int> SkillUseCount = new Dictionary<int, int>();
		public List<BattleAnimKitController> EffectSpineControllerList { get; private set; } = new List<BattleAnimKitController>();

		public float Scale = 1.35f;
		[SerializeField]
		public float ShowTitleDelay = 0.5f;
		[SerializeField]
		public float UnitAppearDelay = 3f;
		[SerializeField]
		public float BossAppearDelay = 0.8f;
		[SerializeField]
		public float BattleCameraSize = 1f;
		public string SummonTargetAttachmentName = "";
		public string SummonAppliedAttachmentName = "";
		public List<AttachmentChangeData> SortFrontDiappearAttachmentChangeDataList = new List<AttachmentChangeData>();
		public List<PartsData> BossPartsList = new List<PartsData>();
		public float OverCursorSize = 1f;
		public List<SkillEffectCtrl> HideOtherAuraEffect = new List<SkillEffectCtrl>();
		public List<VoiceDelayAndEnable> SpeedUpSkillNameVoiceDelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = true
			}
		};
		public List<VoiceDelayAndEnable> NormalSkillNameVoiceDelay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 2f,
				Enable = true
			}
		};
		public List<VoiceDelayAndEnable> NormalSkillNameVoiceDelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 2f,
				Enable = true
			}
		};
		public List<VoiceDelayAndEnable> SpeedUpSkillNameVoiceDelay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = true
			}
		};
		public List<VoiceDelayAndEnable> NormalSkillVoiceDelay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = true
			}
		};
		public List<VoiceDelayAndEnable> NormalSkillVoiceDelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = true
			}
		};
		public List<VoiceDelayAndEnable> SpeedUpSkillVoiceDelay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> SpeedUpSkillVoiceDelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> NormalSkillVoice3Delay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> NormalSkillVoice3DelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> SpeedUpSkillVoice3Delay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> SpeedUpSkillVoice3DelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> NormalSkillVoice4Delay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> NormalSkillVoice4DelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> SpeedUpSkillVoice4Delay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> SpeedUpSkillVoice4DelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> SpeedUpCutInVoiceDelay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> NormalCutInVoiceDelay = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = false
			}
		};
		public List<VoiceDelayAndEnable> SpeedUpCutInVoiceDelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = true
			}
		};
		public List<VoiceDelayAndEnable> NormalCutInVoiceDelayWithCutIn = new List<VoiceDelayAndEnable>
		{
			new VoiceDelayAndEnable
			{
				Delay = 0f,
				Enable = true
			}
		};
		public List<int> LevelFixedSpSkillIdList { get; private set; } = new List<int>();
		public global::System.Int32 Rupee { get; set; } = (global::System.Int32)0;
		public global::System.Int32 RewardCount { get; set; } = (global::System.Int32)0;
		public Dictionary<int, int> SkillLevels { get; set; } = new Dictionary<int, int>();
		public Dictionary<int, eAnimKitCharacterAnimeId> AnimeIdDictionary { get; set; } = new Dictionary<int, eAnimKitCharacterAnimeId>();
		public List<int> MainSkillIdList { get; set; } = new List<int>();
		public List<int> SpecialSkillIdList { get; set; } = new List<int>();
		public List<int> MainSkillEvolutionIdList { get; set; } = new List<int>();
		public List<int> SpecialSkillEvolutionIdList { get; set; } = new List<int>();
		public List<int> SubUnionBurstIdList { get; set; } = new List<int>();
		public List<eAnimKitCharacterAnimeId> TreasureAnimeIdList { get; set; } = new List<eAnimKitCharacterAnimeId>();
		private const float START_CAST_TIME_STAND_BY = 2.5f;
		private const float START_CAST_TIME = 0.3f;
		public int CurrentRotateCoroutineId;
		[SerializeField]
		private List<ShakeEffect> gameStartShakes;
		[SerializeField]
		private List<PrefabWithTime> gameStartEffects;
		[SerializeField]
		private List<PrefabWithTime> dieEffects;
		[SerializeField]
		private List<ShakeEffect> dieShakes;
		[SerializeField]
		private List<PrefabWithTime> damageEffects;
		[SerializeField]
		private List<ShakeEffect> damageShakes;
		public List<PrefabWithTime> SummonEffects;
		[SerializeField]
		private List<PrefabWithTime> idleEffects;
		[SerializeField]
		private List<PrefabWithTime> auraEffects;
		public float BossDeltaX;
		public float BossDeltaY;
		public float AllUnitCenter;
		public float BossBodyWidthOffset;
		public bool IsGameStartDepthBack;
		public bool BossSortIsBack;
		public bool DisableFlash;
		public bool IsForceLeftDir;
		public bool UseTargetCursorOver;
		public bool OneRemainingDisableEffect;
		public bool UseUbVoice3and4;
		private static Yggdrasil<UnitCtrl> staticSingletonTree = null;
		private static IBattleLog staticBattleLog = null;
		private static IBattleCameraEffectForUnitCtrl staticBattleCameraEffect = null;
		private static IBattleEffectPool staticBattleEffectPool = null;
		private static IBattleTimeScaleForUnitCtrl staticBattleTimeScale = null;
		private static BattleManager staticBattleManager = null;
		public UnitComponentAbnormal ComponentAbnormal { [global::OpenPCR.Migration("4a081292-8eb9-42da-a778-df7ae01244f7")] get; private set; }
		public UnitComponentActionPattern ComponentActionPattern { get; private set; }
		public UnitComponentAnimation ComponentAnimation { get; private set; }
		public UnitComponentAnnihilation ComponentAnnihilation { get; private set; }
		public UnitComponentBattleResult ComponentBattleResult { get; private set; }
		public UnitComponentColor ComponentColor { get; private set; }
		public UnitComponentCompare ComponentCompare { get; private set; }
		public UnitComponentDamageControl ComponentDamageControl { get; private set; }
		public UnitComponentLog ComponentLog { get; private set; }
		public UnitComponentMultiPartsBoss ComponentMultiPartsBoss { get; private set; }
		public UnitComponentParameter ComponentParameter { get; private set; }
		public UnitComponentParameterClamped ComponentParameterClamped { get; private set; }
		public UnitComponentPassiveSkill ComponentPassiveSkill { get; private set; }
		public UnitComponentRelicSkill ComponentRelicSkill { get; private set; }
		public UnitComponentRelicPassiveSkill ComponentRelicPassiveSkill { get; private set; }
		public UnitComponentChangeScale ComponentScaleChange { get; private set; }
		public UnitComponentSearchArea ComponentSearchArea { get; private set; }
		public UnitComponentSortOrder ComponentSortOrder { get; private set; }
		public UnitComponentSound ComponentSound { get; private set; }
		public UnitComponentState ComponentState { get; private set; }
		public UnitComponentUnionBurst ComponentUnionBurst { get; private set; }
		public Bone StateBone { get; set; }
		public Bone StateBoneModeChange { get; set; }
		public Bone CenterBone { get; set; }
		public Bone CenterBoneModeChange { get; set; }
		public List<SkillEffectCtrl> RepeatEffectList { get; private set; }
		public List<SkillEffectCtrl> AuraEffectList { get; private set; }
		public float BodyWidth { get; set; }
		public BattleAnimKitController UnitSpineCtrl { get; set; }
		public SkeletonRenderSeparator SkeletonRenderSeparator { get; set; }
		public BattleAnimKitController UnitSpineCtrlModeChange { get; set; }
		public bool IsRecreated { get; set; }
		public bool MainSkill1Evolved { get; set; }

		[global::OpenPCR.Migration("9b6a1dba-606c-44e4-b859-7b7b35bc09a1")]
		public float DeltaTimeForPause
		{
			[global::OpenPCR.Migration("91e70a2d-b688-417c-90b0-50d1712d5f0a")]
			get
			{
				if (battleManager == null)
				{
					return 0f;
				}

				if (ComponentUnionBurst != null && ComponentUnionBurst.PlayingNoCutinMotion)
				{
					return battleManager.DeltaTime;
				}

				if (!battleManager.IsPausingEffectSkippedInThisFrame && !isPaused)
				{
					return battleManager.DeltaTime;
				}

				return 0f;
			}
		}

		public int UnitInstanceId { get; set; }
		public bool TimeToDie { get; set; }
		public bool IsLeftDir { get; set; }
		public eUnitRespawnPos RespawnPos { get; set; }
		public eUnitRespawnPos SummonRespawnPos { get; set; }
		public UnitParameter UnitParameter { get; set; }
		public bool IsOther { get; set; }

		[global::OpenPCR.Migration("d5238664-8a96-4ed3-b8ff-5dc2dd97b90a")]
		public bool IsBonusEnemy
		{
			[global::OpenPCR.Migration("d3689c71-e1dc-41e6-b8e6-653ac3ac986f")]
			get
			{
				return BonusEnemyType != eBonusEnemyType.NONE;
			}
		}

		[global::OpenPCR.ImplicitMigration("af17fc5d-4c94-4c69-90ad-f52c0f4d3fbb")][global::OpenPCR.Migration("5688055d-210b-4f2b-9701-624d5b5d9af1")] public bool IsPlayerUnit => IsOther == battleManager.IsDefenseReplayMode;
		public SummonAction.eSummonType SummonType { get; set; }

		[global::OpenPCR.Migration("5300489a-c0fa-49a0-bda3-f15c817040d0")]
		public bool IsSummonOrPhantom
		{
			[global::OpenPCR.Migration("f8127668-1b5c-4e14-af36-00be4118a058")]
			get
			{
				if (SummonType != SummonAction.eSummonType.SUMMON)
				{
					return SummonType == SummonAction.eSummonType.PHANTOM;
				}

				return true;
			}
		}

		public bool IsGuest { get; set; }
		public bool IsGuestFadeout { get; set; }
		[global::OpenPCR.ImplicitMigration("4037c14d-0a63-4247-a517-ddd2d5fe1e5b")][global::OpenPCR.Migration("9172203d-9d6f-4d8a-babf-9389fb2088d9")] public bool IsDivision => SummonType == SummonAction.eSummonType.DIVISION;
		[global::OpenPCR.ImplicitMigration("faec1b87-924c-46b8-b634-23250b8065c1")][global::OpenPCR.Migration("daa32a09-046f-40b7-a089-efff37499d56")] public bool IsPhantom => SummonType == SummonAction.eSummonType.PHANTOM;
		public bool IsShadow { get; set; }
		public float OverlapPosX { get; set; }
		public List<long> ActionsTargetOnMe { get; set; }
		public List<FirearmCtrl> FirearmCtrlsOnMe { get; set; }
		private int motionPrefix { get; set; }

		[global::OpenPCR.Migration("a8af01cb-6701-4828-a4de-4175889113a9")]
		public int MotionPrefix
		{
			[global::OpenPCR.Migration("96b3f3e2-61e7-4ade-8d6e-faa3cd277740")]
			get
			{
				if (ComponentAbnormal.ToadDatas.Count <= 0)
				{
					return motionPrefix;
				}

				return -1;
			}

			[global::OpenPCR.Migration("1462d3b6-3262-43ca-af56-a23f28c5119c")]
			set
			{
				motionPrefix = value;
			}
		}

		public int PartsMotionPrefix { get; set; }
		public bool IsBoss { get; set; }
		public Dictionary<int, UnitCtrl> SummonUnitDictionary { get; set; }
		public UnitCtrl SummonSource { get; set; }
		public CutInFrameData CutInFrameSet { get; set; }
		public int SkillEnableFrame { get; set; }
		public Transform RotateCenter { get; set; }
		[global::OpenPCR.ImplicitMigration("4390c36b-fdda-46e9-81bb-1e749892abab")][global::OpenPCR.Migration("2405f72a-a476-4502-b1e2-04a81d820924")] public Vector3 FixedCenterPos => new Vector3((IsLeftDir || IsForceLeftDirOrPartsBoss) ? (-fixedCenterPos.x) : fixedCenterPos.x, fixedCenterPos.y, fixedCenterPos.z);
		private Vector3 fixedCenterPos { get; set; }
		public Vector3 ColliderCenter { get; private set; }
		public Vector3 ColliderSize { get; private set; }
		public int CurrentSkillId { get; set; }
		public Action OnIsFrontFalse { get; set; }

		[global::OpenPCR.Migration("68e5f7ab-ad61-452b-ba09-43294b299bf1")]
		public bool HasDieLoop
		{
			[global::OpenPCR.Migration("e576b98d-0a6f-43c4-8473-0bdfde576cd0")]
			get
			{
				if (!(GetCurrentSpineCtrl() == null))
				{
					return GetCurrentSpineCtrl().IsAnimation(eAnimKitCharacterAnimeId.DIE_LOOP);
				}

				return false;
			}
		}

		public VoiceTimingData SpecialVoiceTimingData { get; private set; }
		public bool DisableSortOrderFrontOnBlackoutTarget { get; set; }
		public int SkinRarity { get; set; }
		public float BaseX { get; set; }
		public bool IsDepthBack { get; set; }

		[global::OpenPCR.Migration("bc041edc-3cf5-470b-9453-779dc36867a1")]
		public bool IsForceLeftDirOrPartsBoss
		{
			[global::OpenPCR.Migration("2c8196cf-17ad-43be-83c5-80553011a6b6")]
			get
			{
				if (!IsForceLeftDir)
				{
					return IsPartsBoss;
				}

				return true;
			}
		}

		public bool StartDashDone { get; set; }
		private Transform bottomTransform { get; set; }
		private Vector2 leftDirScale { get; set; }
		private Vector2 rightDirScale { get; set; }

		[global::OpenPCR.Migration("8b050557-2102-4f1d-be26-ae67dc947765")]
		protected IBattleLog battleLog
		{
			[global::OpenPCR.Migration("9ce1e32f-f462-48e8-a47b-bd609f34ff5d")]
			get
			{
				return staticBattleLog;
			}
		}

		private Dictionary<int, float> castTimeDictionary { get; set; }
		public int UnitId { get; protected set; }
		[global::OpenPCR.Migration("abcd0c90-6b51-4882-85a9-eaab422a906f")] public int OriginalUnitId { [global::OpenPCR.Migration("93f6e0af-d05b-42ea-a92f-47ad6b9f501b")] get; [global::OpenPCR.Migration("7b164e1a-c7f1-4177-ad3f-84fa84c24302")] private set; }
		public int SDSkin { get; private set; }
		public int AtkType { get; protected set; }
		public int UnionBurstSkillId { get; set; }
		public float StartHpPercent { get; private set; }
		public global::System.Boolean IsMoveSpeedForceZero { get; set; }

		[global::OpenPCR.Migration("e2690679-57ec-429b-b716-cf6649603058")]
		private BattleManager battleManager
		{
			[global::OpenPCR.Migration("34644085-dc15-4814-9f61-7d22afd1192b")]
			get
			{
				return staticBattleManager;
			}
		}

		[global::OpenPCR.Migration("76eaef72-3811-4d1f-bd95-726f7e9ebc0b")]
		public Transform BottomTransform
		{
			[global::OpenPCR.Migration("f5d85f18-fd30-4cf0-be9e-8bc0b079b440")]
			get
			{
				if (bottomTransform == null)
				{
					bottomTransform = ((GetCurrentSpineCtrl() == null) ? null : GetCurrentSpineCtrl().transform);
				}

				return bottomTransform;
			}
		}

		[global::OpenPCR.Migration("f2a1fee9-f450-4af9-bb01-dc4bc46aab6c")]
		public bool Pause
		{
			[global::OpenPCR.Migration("fa906f8f-2da4-4c3f-9d91-712ae1573f2f")]
			get
			{
				return isPaused;
			}

			[global::OpenPCR.Migration("1580a6a4-437f-4aec-89d0-83694e6af973")]
			set
			{
				isPaused = value;
				if (value)
				{
					SetMotionPause();
				}
				else if (!ComponentAbnormal.IsUnableActionState() || ComponentAbnormal.IsSpecialSleepEnabled())
				{
					SetMotionResume();
				}

				ComponentSound.PauseSe(isPaused);
				PrincessFormProcessor.Pause(isPaused);
			}
		}

		[global::OpenPCR.ImplicitMigration("60b22ba2-2c0b-4d61-bab9-3bc9d8123d9e")][global::OpenPCR.Migration("61f949b0-3059-40e9-8a40-6d0ce22c047c")] public bool IsPartsBoss => BossPartsList.Count > 0;

		[global::OpenPCR.Migration("a341ef05-7c48-4b9b-8e7f-bc9324ac393e")]
		public UnitCtrl()
		{
		}

		[global::OpenPCR.Migration("c3e9825d-4169-495e-9c11-0a6505eafcda")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
			staticBattleLog = null;
			staticBattleCameraEffect = null;
			staticBattleEffectPool = null;
			staticBattleTimeScale = null;
		}

		[global::OpenPCR.Migration("b643ab0b-445e-4bc5-b345-5411e1ca5f3f")]
		public BattleAnimKitController GetCurrentSpineCtrl()
		{
			if (ComponentAbnormal.ToadDatas.Count > 0)
			{
				return ComponentAbnormal.ToadDatas[0].BattleSpineController;
			}

			if (MotionPrefix != -1)
			{
				return UnitSpineCtrlModeChange;
			}

			return UnitSpineCtrl;
		}

		[global::OpenPCR.Migration("81e05214-270f-44ed-81b5-c67f14606261")]
		private void Awake()
		{
			base.OnAwake();
			TimeToDie = false;
			ActionsTargetOnMe = new List<long>();
			FirearmCtrlsOnMe = new List<FirearmCtrl>();
			SummonUnitDictionary = new Dictionary<int, UnitCtrl>();
			UnitUnionBurstTimelineList = new List<UnitUnionBurstTimeline>();
			CircleEffectList = new List<CircleEffectController>();
		}

		[global::OpenPCR.Migration("bb543c7e-27fb-4dc0-887a-e90490ddca83")]
		protected override void DestructByOnDestroy()
		{
			base.DestructByOnDestroy();
			StateBone = null;
			CenterBone = null;
			RepeatEffectList = null;
			AuraEffectList = null;
			DamagedHpEffectDictionary = null;
			UnitSpineCtrl = null;
			UnitSpineCtrlModeChange = null;
			UnitUnionBurstTimelineList = null;
			CircleEffectList = null;
			UnitParameter = null;
			ActionsTargetOnMe = null;
			FirearmCtrlsOnMe = null;
			SummonUnitDictionary = null;
			CutInFrameSet = null;
			SkillUseCount = null;
			OnIsFrontFalse = null;
			gameStartShakes = null;
			gameStartEffects = null;
			dieEffects = null;
			dieShakes = null;
			damageEffects = null;
			damageShakes = null;
			SummonEffects = null;
			idleEffects = null;
			LifeGauge = null;
			bottomTransform = null;
			UnitActionController = null;
			SkillLevels = null;
			AnimeIdDictionary = null;
			MainSkillIdList = null;
			MainSkillEvolutionIdList = null;
			SpecialSkillIdList = null;
			SpecialSkillEvolutionIdList = null;
			SubUnionBurstIdList = null;
			SpecialVoiceTimingData = null;
			PrincessFormProcessor = null;
			EffectSpineControllerList = null;
			OnInitCallbackForUi = null;
			ComponentAbnormal = null;
			ComponentAnimation = null;
			ComponentAnnihilation = null;
			ComponentBattleResult = null;
			ComponentColor = null;
			ComponentCompare = null;
			ComponentDamageControl = null;
			ComponentLog = null;
			ComponentMultiPartsBoss = null;
			ComponentParameter = null;
			ComponentParameterClamped = null;
			ComponentPassiveSkill = null;
			ComponentRelicSkill = null;
			ComponentRelicPassiveSkill = null;
			ComponentScaleChange = null;
			ComponentSearchArea = null;
			ComponentSortOrder = null;
			ComponentSound = null;
			ComponentState = null;
			ComponentUnionBurst = null;
		}

		[global::OpenPCR.Migration("d6c1c914-a041-4dc3-9f9b-0129a09acb71")]
		public void DestroyAndCoroutineRemove()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.UnitCtrl::DestroyAndCoroutineRemove():System.Void (TimeToDie has no Native producer; RecreateDeadUnits is outside the first-phase result boundary)"); }

		[global::OpenPCR.Migration("fca0ffbf-1ff4-4429-bf04-89df6e7793c2")]
		private void instantiateResources()
		{
			ResourceManager instance = ManagerSingleton<ResourceManager>.Instance;
			LifeGauge = instance.LoadImmediately(eResourceId.UNIT_LIFE_GAUGE, null, 0L).GetComponent<LifeGaugeController>();
			if (IsPlayerUnit || IsBoss)
			{
				return;
			}

			this.ComponentAbnormal.OnChangeState = (Action<eStateIconType, bool>)Delegate.Combine(this.ComponentAbnormal.OnChangeState, (Action<eStateIconType, bool>)delegate (eStateIconType _iconType, bool _enable)
			{
				if (_enable)
				{
					LifeGauge.AddIcon(_iconType);
				}
				else
				{
					LifeGauge.DeleteIcon(_iconType);
				}
			});
			this.ComponentAbnormal.OnChangeStateNum = (Action<eStateIconType, int>)Delegate.Combine(this.ComponentAbnormal.OnChangeStateNum, (Action<eStateIconType, int>)delegate (eStateIconType _iconType, int _num)
			{
				LifeGauge.SetIconNum(_iconType, _num);
			});
			this.ComponentAbnormal.OnChangeStatePassiveEnable = (Action<eStateIconType, bool>)Delegate.Combine(this.ComponentAbnormal.OnChangeStatePassiveEnable, (Action<eStateIconType, bool>)delegate (eStateIconType _iconType, bool _enable)
			{
				LifeGauge.SetIconPassiveEnable(_iconType, _enable);
			});
		}

		[global::OpenPCR.Migration("7d7a18f2-3f89-45c9-adc1-b031bc4d5f49")]
		public void Initialize(UnitParameter _data, bool _isOther, bool _isFirstWave, bool _isGaugeAlwaysVisible, ExtraEnhanceParameterContainer _extraEnhanceParameterContainer, BattleAnimKitController _battleSpineController, ePlaybackType _cutInPlaybackType, SummonAction.eSummonType _summonType = SummonAction.eSummonType.NONE)
		{
			MasterDataManager masterDataManager = ManagerSingleton<MasterDataManager>.Instance;
			if (staticSingletonTree == null)
			{
				staticSingletonTree = this.CreateSingletonTree();
				staticBattleManager = singletonTree.Get<BattleManager>();
				staticBattleLog = singletonTree.Get<BattleLog>();
				staticBattleCameraEffect = singletonTree.Get<BattleCameraEffect>();
				staticBattleEffectPool = singletonTree.Get<BattleEffectPool>();
				staticBattleTimeScale = singletonTree.Get<BattleTimeScale>();
				FunctionalComparer<BasePartsData>.CreateInstance();
			}

			var battleEffectData = new PresetBattleEffectData(gameStartShakes, gameStartEffects, dieEffects, dieShakes,
				damageEffects, damageShakes, SummonEffects, idleEffects, auraEffects);
			RepeatEffectList = new List<SkillEffectCtrl>();
			AuraEffectList = new List<SkillEffectCtrl>();
			CutInFrameSet = new CutInFrameData();
			leftDirScale = new Vector2(-Scale, Math.Abs(Scale));
			rightDirScale = new Vector2(Scale, Math.Abs(Scale));
			eBattleCategory battleCategory = battleManager.BattleCategory;
			UnitParameter = _data;
			IsOther = _isOther;
			WeaponSeType = (SystemIdDefine.eWeaponSeType)(int)_data.MasterData.SeType;
			WeaponMotionType = (SystemIdDefine.eWeaponMotionType)(int)_data.MasterData.MotionType;
			UnitId = _data.UniqueData.Id;
			IsBoss = UnitUtility.JudgeIsBoss(UnitId);
			CharacterUnitId = _data.MasterData.UnitId;
			OriginalUnitId = UnitUtility.GetOriginalUnitId(UnitId);
			UnitSpineCtrl = _battleSpineController;
			ComponentAbnormal = new UnitComponentAbnormal(this, battleManager, battleLog);
			ComponentActionPattern = new UnitComponentActionPattern(this, UnitId, SkillLevels);
			ComponentAnimation = new UnitComponentAnimation(GetCurrentSpineCtrl, battleManager.IsSkipping, battleManager.CanPlaySe);
			ComponentAnnihilation = new UnitComponentAnnihilation();
			ComponentBattleResult = new UnitComponentBattleResult(this, transform);
			ComponentColor = new UnitComponentColor(this);
			ComponentCompare = new UnitComponentCompare(this);
			ComponentDamageControl = new UnitComponentDamageControl(this, battleManager, battleLog, battleEffectPool);
			ComponentLog = new UnitComponentLog(this, battleLog);
			ComponentMultiPartsBoss = new UnitComponentMultiPartsBoss(this);
			ComponentParameter = new UnitComponentParameter(onHpChanged);
			ComponentParameterClamped = new UnitComponentParameterClamped(battleManager.EnergyRecoveryRateMax,
				IsBoss || (int)_data.MasterData.MoveSpeed == 0);
			ComponentPassiveSkill = new UnitComponentPassiveSkill(this);
			ComponentRelicSkill = new UnitComponentRelicSkill(this);
			ComponentRelicPassiveSkill = new UnitComponentRelicPassiveSkill(this);
			ComponentSearchArea = new UnitComponentSearchArea(this);
			ComponentScaleChange = new UnitComponentChangeScale(this);
			ComponentSortOrder = new UnitComponentSortOrder(this);
			ComponentSound = new UnitComponentSound(this, battleManager, _data.MasterData.PrefabId, gameObject);
			ComponentState = new UnitComponentState(this, battleManager, battleLog, battleEffectData);
			ComponentUnionBurst = new UnitComponentUnionBurst(this, battleManager, battleTimeScale, _cutInPlaybackType);
			registerAllComponentSet();
			OnInitCallbackForUi.Call(this);
			battleManager.InitializeExAndFreeSkill(_data.UniqueData, ePassiveInitType.OWNER, 0, this);
			this.ComponentSound.InitializeVoiceMax();
			MasterEnemyEnableVoice.EnemyEnableVoice enemyEnableVoice = masterDataManager.masterEnemyEnableVoice.Get(CharacterUnitId);
			if (enemyEnableVoice != null)
			{
				this.ComponentSound.EnemyVoiceId = enemyEnableVoice.voice_id;
			}

			SDSkin = UnitUtility.GetSetSkinNo(_data.UniqueData, UnitDefine.eSkinType.Sd, !IsPlayerUnit);
			Rarity = _data.UniqueData.UnitRarity;
			BattleRarity = _data.UniqueData.BattleRarity;
			IconSkin = UnitUtility.GetSetSkinNo(_data.UniqueData, UnitDefine.eSkinType.Icon, false, null);
			PromotionLevel = _data.UniqueData.PromotionLevel;
			PrefabId = _data.MasterData.PrefabId;
			bool flag = BattleUtil.IsClanBattleCategory(battleCategory);
			IsClanBattleOrSekaiEnemy = _isOther && (flag || battleCategory == eBattleCategory.GLOBAL_RAID);
			if (flag && !IsBoss)
			{
				IsClanBattleOrSekaiEnemy = false;
			}

			IsTrialBattleEnemy = _isOther && (battleCategory == eBattleCategory.TRIAL_BATTLE || battleCategory == eBattleCategory.TRIAL_CLAN_BATTLE);
			IsAbyssBossEnemy = _isOther && IsBoss && unchecked((uint)((int)battleCategory - 47)) < 2U;
			this.ComponentParameter.Level = _data.UniqueData.UnitLevel;
			AtkType = _data.MasterData.AtkType;
			MotionPrefix = -1;
			ComponentDamageControl.ResetAccumulateDamage();
			ComponentSearchArea.InitializeSearchArea((float)(int)_data.MasterData.SearchAreaWidth);
			this.ComponentState.IsDead = false;
			AtkRecastTime = (float)(double)_data.MasterData.AtkCastTime;
			SpecialVoiceTimingData = BattleVoiceUtility.GetVoiceTimingData(OriginalUnitId);
			TalentIds = masterDataManager.masterUnitTalent.GetTalentIdsAsc(OriginalUnitId);
			RoleId = masterDataManager.masterUnitRoleData.GetOrInvalidRoleId(OriginalUnitId);
			ComponentPassiveSkill.SetEnhanceData(_extraEnhanceParameterContainer.TeamEnhanceParameter);
			var labyrinthEnhance = _extraEnhanceParameterContainer.LabyrinthEnhanceParameter;
			ComponentPassiveSkill.SetContentsEnhanceData(labyrinthEnhance);
			ComponentRelicPassiveSkill.SetTeamEnhanceData(labyrinthEnhance);
			if (_summonType != SummonAction.eSummonType.NONE)
			{
				SummonType = _summonType;
				ComponentState.CurrentState = ActionState.SUMMON;
			}
			instantiateResources();
			if (IsSummonOrPhantom)
			{
				this.ComponentState.StandByDone = true;
			}
			else if (battleManager.BattleCategory != eBattleCategory.TUTORIAL)
			{
				this.ComponentState.StandByDone = battleManager.CurrentWave != 0 || !GetCurrentSpineCtrl().IsAnimation(eAnimKitCharacterAnimeId.STAND_BY);
			}
			else
			{
				this.ComponentState.StandByDone = true;
			}

			this.ComponentParameter.SetEnergy(0f, eSetEnergyType.INITIALIZE);
			LifeGauge.Initialize(_isOther, StateBone.WorldY * Math.Abs(Scale) + (UnitUtility.IsPersonUnit(this.ComponentSound.SoundUnitId) ? BattleDefine.HEAD_OFFSET : 0f), this, _isGaugeAlwaysVisible);
			if (battleManager.IsHatsuneBoss && IsBoss)
			{
				// Hatsune boss input/resource production is outside the clan/training scope; no default effect difficulty is substituted.
				throw new global::OpenPCR.OutOfBoundaryException("Elements.HatsuneUtility::GetCurrentEventBossDifficulty(System.Int32):Elements.eHatsuneBossDifficulity");
			}
			else if (battleManager.IsClanBattle && IsBoss)
			{
				ClanBattleTempData instance = Singleton<ClanBattleTempData>.Instance;
				bool isSingleModeBoss = instance.ClanBattleTopInfo.IsSingleBattleMode() && !instance.IsTrainingBattle;
				EffectDifficulty = ClanBattleUtil.GetCurrentBossEffectDifficulty(instance.BattleId, instance.BattleLapNum, instance.BattlePhase, isSingleModeBoss);
			}
			else if (battleManager.IsTrialClanBattle && IsBoss)
			{
				TrialBattleTempData trialBattleTempData = this.CreateSingletonTree().Get<TrialBattleTempData>();
				MasterClanBattle2MapData.ClanBattle2MapData clanBattle2MapData = null;
				foreach (MasterClanBattle2MapData.ClanBattle2MapData value2 in masterDataManager.masterClanBattle2MapData.dictionary.Values)
				{
					if ((int)value2.clan_battle_id == trialBattleTempData.InitClanBattleSelectDifficultyParam.ClanBattleId && value2.Phase == trialBattleTempData.DuringClanBattleModeParam.Phase)
					{
						clanBattle2MapData = value2;
						break;
					}
				}

				EffectDifficulty = ClanBattleUtil.GetCurrentBossEffectDifficulty(clanBattle2MapData.clan_battle_id, clanBattle2MapData.LapNumFrom, clanBattle2MapData.Phase, false);
			}
			else if (battleManager.BattleCategory != eBattleCategory.KAISER_BATTLE_MAIN && battleManager.BattleCategory != eBattleCategory.LEGION_BATTLE_MAIN && battleManager.BattleCategory != eBattleCategory.MIROKU_BATTLE && battleManager.BattleCategory != eBattleCategory.MIRAGE_FLOOR_QUEST && battleManager.BattleCategory != eBattleCategory.MIRAGE_NEMESIS && auraEffects.Find((PrefabWithTime e) => e.EffectDifficlty == PrefabWithTime.eEffectDifficulty.ALL_FOR_CHARA) == null)
			{
				auraEffects.Clear();
			}

			// Native SetBossCameraScale writes presentation camera zoom only; battle geometry continues to use animation/bone transforms below.

			UnitSpineCtrl.AnimationName = UnitSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE);
			UnitSpineCtrl.transform.localScale = rightDirScale;
			fixedCenterPos = Vector3.Scale(new Vector3(CenterBone.worldX, CenterBone.worldY, 0f), UnitSpineCtrl.transform.lossyScale);
			FixedStatePos = Vector3.Scale(new Vector3(StateBone.worldX, StateBone.worldY, 0f), UnitSpineCtrl.transform.lossyScale);
			// FX slot separation feeds mesh rendering, child shaders and result sorting only.
			// Skeleton bones, attachments, animation events and combat geometry remain live.

			var dummyPartsData = new BasePartsData
			{
				PositionX = 0f,
				Owner = this
			};
			dummyPartsData.SetBattleManager(battleManager);
			dummyPartsData.SetAbnormalResistId(_data.UniqueData.ResistStatusId, _isInit: true);
			dummyPartsData.SetDebuffResistId(_data.UniqueData.ResistVariationId, _isInit: true);
			ComponentMultiPartsBoss.SetDummyPartsData(dummyPartsData);
			battleManager.SetPartsTalentWeakness(OriginalUnitId, dummyPartsData);
			MasterEnemyParameter.EnemyParameter fromAllKind = ManagerSingleton<MasterDataManager>.Instance.masterEnemyParameter.GetFromAllKind(UnitId);
			if (IsBoss && fromAllKind != null && (int)fromAllKind.break_durability > 0)
			{
				var multiBossPartsData = new PartsData
				{
					Owner = this,
					PositionX = 0f
				};
				multiBossPartsData.BreakPoint = fromAllKind.break_durability;
				multiBossPartsData.SetBattleManager(battleManager);
				ComponentMultiPartsBoss.SetMultiBossPartsData(multiBossPartsData);
				battleManager.CountIsNotPartsBossReady++;
				this.ComponentState.IsNotPartsBossReady = true;
			}

			if (IsPartsBoss)
			{
				MasterEnemyMParts.EnemyMParts enemyMParts = ManagerSingleton<MasterDataManager>.Instance.masterEnemyMParts.Get(UnitId);
				for (int num3 = 0; num3 < BossPartsList.Count; num3++)
				{
					PartsData partsData = BossPartsList[num3];
					PartsData partsData2 = new PartsData
					{
						BodyWidthValue = partsData.BodyWidthValue,
						PositionX = partsData.PositionX,
						InitialPositionX = partsData.PositionX,
						EnemyId = partsData.EnemyId,
						Index = partsData.Index,
						Owner = this,
						PositionY = partsData.PositionY,
						AttachmentNamePairList = partsData.AttachmentNamePairList,
						ChangeAttachmentStartTime = partsData.ChangeAttachmentStartTime,
						ChangeAttachmentEndTime = partsData.ChangeAttachmentEndTime,
						DisableStartBreakEffect = partsData.DisableStartBreakEffect,
						BreakEffectTime = partsData.BreakEffectTime,
						BreakEffectSize = partsData.BreakEffectSize,
						BreakEffectOffsetY = partsData.BreakEffectOffsetY
					};
					partsData2.Initialize(enemyMParts, battleManager);
					partsData2.SetBattleManager(battleManager);
					ComponentMultiPartsBoss.BossPartsListForBattle.Add(partsData2);
				}

				battleManager.CountIsNotPartsBossReady++;
				this.ComponentState.IsNotPartsBossReady = true;
			}

			castTimeDictionary = new Dictionary<int, float>();
			MasterSkillData masterSkillParameter = ManagerSingleton<MasterDataManager>.Instance.masterSkillData;
			Action<List<int>, List<SkillLevelInfo>, eAnimKitCharacterAnimeId, SoundManager.eVoiceType> action = delegate (List<int> idList, List<SkillLevelInfo> skillLevelInfoList, eAnimKitCharacterAnimeId animeId, SoundManager.eVoiceType voiceType)
			{
				int i = 0;
				for (int count2 = idList.Count; i < count2; i++)
				{
					int skillId = idList[i];
					if (skillId != 0)
					{
						ComponentState.AddVoiceTypeAndSkillNumber(skillId, new VoiceTypeAndSkillNumber { VoiceType = voiceType, SkillNumber = i + 1 });
						MasterSkillData.SkillData skillData = masterSkillParameter[skillId];
						if ((int)skillData.skill_area_width < 1)
						{
							ComponentSearchArea.SkillAreaWidthList.Add(skillId, (int)_data.MasterData.SearchAreaWidth);
						}
						else
						{
							ComponentSearchArea.SkillAreaWidthList.Add(skillId, (int)skillData.skill_area_width);
						}

						SkillLevelInfo skillLevelInfo5 = skillLevelInfoList.Find((SkillLevelInfo e) => (int)e.SkillId == skillId);
						if (skillLevelInfo5 == null)
						{
							foreach (MasterUnitSkillDataRf.UnitSkillDataRf skillRf in masterDataManager.masterUnitSkillDataRf.GetListWithSkillId(skillId))
							{
								skillLevelInfo5 = skillLevelInfoList.Find((SkillLevelInfo e) => (int)e.SkillId == (int)skillRf.rf_skill_id);
								if (skillLevelInfo5 != null)
								{
									break;
								}
							}
						}

						SkillLevels.Add(skillId, (skillLevelInfo5 != null) ? ((int)skillLevelInfo5.SkillLevel) : 0);
						if (!SkillUseCount.ContainsKey(skillId))
						{
							SkillUseCount.Add(skillId, 0);
						}

						castTimeDictionary.Add(skillId, (float)(double)skillData.skill_cast_time);
						AnimeIdDictionary.Add(skillId, animeId);
					}
				}
			};
			action(_data.SkillData.UnionBurstIds, _data.UniqueData.UnionBurst, eAnimKitCharacterAnimeId.SKILL, SoundManager.eVoiceType.SKILL);
			action(_data.SkillData.UnionBurstEvolutionIds, _data.UniqueData.UnionBurst, eAnimKitCharacterAnimeId.SKILL, SoundManager.eVoiceType.SKILL);
			action(_data.SkillData.MainSkillIds, _data.UniqueData.MainSkill, eAnimKitCharacterAnimeId.SKILL, SoundManager.eVoiceType.SKILL);
			action(_data.SkillData.MainSkillEvolutionIds, _data.UniqueData.MainSkill, eAnimKitCharacterAnimeId.SKILL, SoundManager.eVoiceType.SKILL);
			List<SkillLevelInfo> list = new List<SkillLevelInfo>();
			MasterSpskillLvInitializeData masterSpskillLvInitializeData = ManagerSingleton<MasterDataManager>.Instance.masterSpskillLvInitializeData;
			for (int num4 = 0; num4 < _data.SkillData.SpSkillIds.Count; num4++)
			{
				SkillLevelInfo skillLevelInfo = new SkillLevelInfo();
				int num5 = _data.SkillData.SpSkillIds[num4];
				skillLevelInfo.SetSkillId(num5);
				MasterSpskillLvInitializeData.SpskillLvInitializeData spskillLvInitializeData = masterSpskillLvInitializeData.Get(num5);
				if (spskillLvInitializeData == null || !SkillLevels.ContainsKey(spskillLvInitializeData.base_skill_id))
				{
					skillLevelInfo.SetSkillLevel(this.ComponentParameter.Level);
					if (num4 < _data.SkillData.SpSkillEvolutionIds.Count)
					{
						SkillLevelInfo skillLevelInfo2 = new SkillLevelInfo();
						skillLevelInfo2.SetSkillId(_data.SkillData.SpSkillEvolutionIds[num4]);
						skillLevelInfo2.SetSkillLevel(this.ComponentParameter.Level);
						list.Add(skillLevelInfo2);
					}
				}
				else
				{
					if (num4 < _data.SkillData.SpSkillEvolutionIds.Count)
					{
						LevelFixedSpSkillIdList.Add(_data.SkillData.SpSkillEvolutionIds[num4]);
						SkillLevelInfo skillLevelInfo3 = new SkillLevelInfo();
						skillLevelInfo3.SetSkillId(_data.SkillData.SpSkillEvolutionIds[num4]);
						skillLevelInfo3.SetSkillLevel(searchSpSkillLevel(spskillLvInitializeData.base_skill_id, _data));
						list.Add(skillLevelInfo3);
					}

					LevelFixedSpSkillIdList.Add(num5);
					skillLevelInfo.SetSkillLevel(searchSpSkillLevel(spskillLvInitializeData.base_skill_id, _data));
				}

				list.Add(skillLevelInfo);
			}

			action(_data.SkillData.SpSkillIds, list, eAnimKitCharacterAnimeId.SPECIAL_SKILL, SoundManager.eVoiceType.SP_SKILL);
			action(_data.SkillData.SpSkillEvolutionIds, list, eAnimKitCharacterAnimeId.SPECIAL_SKILL, SoundManager.eVoiceType.SP_SKILL);
			List<SkillLevelInfo> list2 = new List<SkillLevelInfo>();
			for (int num6 = 0; num6 < _data.SkillData.SubUnionBurstIds.Count; num6++)
			{
				SkillLevelInfo skillLevelInfo4 = new SkillLevelInfo();
				skillLevelInfo4.SetSkillId(_data.SkillData.SubUnionBurstIds[num6]);
				if (_data.UniqueData.UnionBurst.Count > 0)
				{
					skillLevelInfo4.SetSkillLevel(_data.UniqueData.UnionBurst[0].SkillLevel);
				}

				list2.Add(skillLevelInfo4);
			}

			action(_data.SkillData.SubUnionBurstIds, list2, eAnimKitCharacterAnimeId.SUB_UNION_BURST, SoundManager.eVoiceType.SUB_UNION_BURST);
			MainSkillIdList = _data.SkillData.MainSkillIds;
			SpecialSkillIdList = _data.SkillData.SpSkillIds;
			SpecialSkillEvolutionIdList = _data.SkillData.SpSkillEvolutionIds;
			MainSkillEvolutionIdList = _data.SkillData.MainSkillEvolutionIds;
			SubUnionBurstIdList = _data.SkillData.SubUnionBurstIds;
			int num7 = _data.SkillData.UnionBurstEvolutionIds[0];
			if (num7 != 0 && SkillLevels[num7] != 0)
			{
				UnionBurstSkillId = num7;
			}
			else
			{
				UnionBurstSkillId = _data.SkillData.UnionBurstIds[0];
			}

			if (UnionBurstSkillId != 0)
			{
				unionBurstSkillAreaWidth = (int)masterSkillParameter[UnionBurstSkillId].skill_area_width;
			}

			ComponentActionPattern.Setup(_data.SkillData);
			UnitData unitData = _data.UniqueData;
			bool flag2 = UnitUtility.IsPlayableUnit(OriginalUnitId) && !IsSummonOrPhantom && !IsGuest;
			if (flag & flag2)
			{
				unitData = _data.LimitedUnitData;
			}

			global::System.Int64 hp = unitData.TotalHp;
			global::System.Int64 atk = unitData.TotalAtk;
			global::System.Int64 def = unitData.TotalDef;
			global::System.Int64 magicAtk = unitData.TotalMagicAtk;
			global::System.Int64 magicDef = unitData.TotalMagicDef;
			global::System.Int64 waveHpRecovery = unitData.TotalWaveHpRecovery;
			global::System.Int64 waveEnergyRecovery = unitData.TotalWaveEnergyRecovery;
			global::System.Int64 physicalCritical = unitData.TotalCritical;
			global::System.Int64 magicCritical = unitData.TotalMagicCritical;
			global::System.Int64 dodge = unitData.TotalDodge;
			global::System.Int64 accuracy = unitData.TotalAccuracy;
			global::System.Int64 physicalPenetrate = unitData.TotalPhysicalPenetrate;
			global::System.Int64 magicPenetrate = unitData.TotalMagicPenetrate;
			global::System.Int64 lifeSteal = unitData.TotalLifeSteal;
			global::System.Int64 hpRecoveryRate = unitData.TotalHpRecoveryRate;
			global::System.Int64 energyRecoveryRate = unitData.TotalEnergyRecoveryRate;
			global::System.Int64 energyReduceRate = unitData.TotalEnergyReduceRate;
			global::System.Int64 damagedHp = UnitUtility.IsPlayableUnit(OriginalUnitId) ?
				UnitUtility.CalcUnitParameterForDamagedEnergy(eParamType.HP, this) : unitData.TotalHp;
			global::System.Int64 damagedDef = UnitUtility.IsPlayableUnit(OriginalUnitId) ?
				UnitUtility.CalcUnitParameterForDamagedEnergy(eParamType.DEF, this) : unitData.TotalDef;
			global::System.Int64 damagedMagicDef = UnitUtility.IsPlayableUnit(OriginalUnitId) ?
				UnitUtility.CalcUnitParameterForDamagedEnergy(eParamType.MAGIC_DEF, this) : unitData.TotalMagicDef;
			ComponentParameter.InitializeParameter(hp, atk, def, magicAtk, magicDef, waveHpRecovery, waveEnergyRecovery,
				physicalCritical, magicCritical, dodge, accuracy, lifeSteal, physicalPenetrate, magicPenetrate,
				hpRecoveryRate, energyRecoveryRate, energyReduceRate, damagedHp, damagedDef, damagedMagicDef,
				100L, 100L, 0L, 0L, 100L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0L,
				0.0, (float)(int)_data.MasterData.MoveSpeed);
			MoveRate = (IsMoveSpeedForceZero ? 0f : ((float)this.ComponentParameterClamped.MoveSpeedOrMin * (_isOther ? (-1f) : 1f)));
			battleLog.AppendBattleLog(eBattleLogType.SET_DAMAGE, 3, 0L, this.ComponentParameter.Hp, 0, 0, 1, 1, null, this);
			long num8 = this.ComponentParameter.StartMaxHpForDamagedEnergy;
			if (fromAllKind != null && (long)fromAllKind.virtual_hp != 0)
			{
				num8 = (long)fromAllKind.virtual_hp;
			}

			SkillStackValDmg = IsBoss ? 0.0 : 1000.0 / (double)num8;
			if (UnitUtility.IsPlayableUnit(OriginalUnitId))
			{
				int promotionLevel = (int)unitData.PromotionLevel;
				double num9 = (IsPlayerUnit ? battleManager.CalcPlayerDamageTpReduceRate(promotionLevel) : battleManager.CalcEnemyDamageTpReduceRate(promotionLevel));
				SkillStackValDmg = num9 * (double)SkillStackValDmg;
			}

			switch (UnitUtility.GetUnitPosType((float)(int)_data.MasterData.SearchAreaWidth))
			{
				case eUnitBattlePos.FRONT:
					SkillStackValDmg = (double)SkillStackValDmg * battleManager.EnergyStackRatioDamagedFront;
					SkillStackVal = battleManager.EnergyGainValieSkillFront;
					break;
				case eUnitBattlePos.MIDDLE:
					SkillStackValDmg = (double)SkillStackValDmg * battleManager.EnergyStackRatioDamagedMiddle;
					SkillStackVal = battleManager.EnergyGainValieSkillMiddle;
					break;
				case eUnitBattlePos.BACK:
					SkillStackValDmg = (double)SkillStackValDmg * battleManager.EnergyStackRatioDamagedBack;
					SkillStackVal = battleManager.EnergyGainValieSkillBack;
					break;
			}

			UnitActionController = GetComponent<UnitActionController>();
			UnitActionController.Initialize(this, _data, false, null);
			Slot slot2 = GetCurrentSpineCtrl().skeleton.FindSlot("Center");
			Skin skin = GetCurrentSpineCtrl().skeleton.Data.Skins.Items[0];
			List<string> list3 = new List<string>();
			int slotIndex = GetCurrentSpineCtrl().skeleton.FindSlotIndex("Center");
			skin.FindNamesForSlot(slotIndex, list3);
			foreach (string item in list3)
			{
				if (skin.GetAttachment(slotIndex, item) is BoundingBoxAttachment box)
				{
					PolygonCollider2D polygonCollider2D = SkeletonUtility.AddBoundingBoxAsComponent(box, slot2, base.gameObject, isTrigger: false);
					ColliderCenter = polygonCollider2D.bounds.center - base.transform.position;
					ColliderSize = polygonCollider2D.bounds.size;
					BodyWidth = ColliderSize.x / base.transform.lossyScale.x + BossBodyWidthOffset;
					global::OpenPCR.Runtime.Api.Object.Destroy(polygonCollider2D);
					global::OpenPCR.Runtime.Api.Object.Destroy(base.gameObject.GetComponent<Rigidbody2D>());
				}
			}

			if (IsOther && battleManager.GetPurpose() == eSpecialBattlePurpose.GET_POINT)
			{
				this.ComponentState.EnemyPoint = battleManager.GetEnemyPoint(UnitId);
			}

			if (IsBoss && battleManager.IsSpecialBattle)
			{
				bool isDungeonSpecialBattle = battleManager.IsDungeonSpecialBattle;
				bool flag3 = !isDungeonSpecialBattle && (!HatsuneUtility.IsNowEventMultiBoss() || HatsuneUtility.IsMultiBossDestructionTarget(UnitId));
				bool flag4 = isDungeonSpecialBattle && (!DungeonUtility.IsNowSpecialMultiBoss() || DungeonUtility.IsMustKillSpecialMultiBoss(UnitId));
				if (flag3 | flag4)
				{
					int triggerHp = battleManager.GetTriggerHpPercent();
					if (triggerHp != 0)
					{
						this.ComponentState.SpecialBattlePurposeHp = BattleUtil.DoubleToLong((double)unchecked(triggerHp * (long)this.ComponentParameter.MaxHp) / 100.0);
					}
				}
			}

			for (int num10 = 0; num10 < SortFrontDiappearAttachmentChangeDataList.Count; num10++)
			{
				AttachmentChangeData data = SortFrontDiappearAttachmentChangeDataList[num10];
				data.TargetIndex = UnitSpineCtrl.skeleton.slots.FindIndex((Slot e) => e.data.Name == data.TargetAttachmentName);
				data.TargetAttachment = UnitSpineCtrl.skeleton.GetAttachment(data.TargetIndex, data.TargetAttachmentName);
				int slotIndex2 = UnitSpineCtrl.skeleton.slots.FindIndex((Slot e) => e.data.Name == data.AppliedAttachmentName);
				Attachment attachment = UnitSpineCtrl.skeleton.GetAttachment(slotIndex2, data.AppliedAttachmentName);
				data.AppliedAttachment = attachment;
			}

			if (IsShadow && Rarity == 6 && battleManager.GetEnableShadowEffect())
			{
				SkillEffectCtrl component = ManagerSingleton<ResourceManager>.Instance.LoadImmediately(eResourceId.FX_SHADOW_BATTLE, ExceptUIRoot.Transform, 0L).GetComponent<SkillEffectCtrl>();
				component.InitializeSort();
				component.PlaySe(this.ComponentSound.SoundUnitId, IsLeftDir);
				component.transform.position = base.transform.position;
				component.transform.parent = ExceptUIRoot.Transform;
				component.SortTarget = this;
				component.ExecAppendCoroutine(this, _isAbnormal: true);
				battleManager.StartCoroutine(component.TrackTarget(this.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero, _followX: true, _followY: true, CenterBone));
				battleManager.StartCoroutine(component.TrackTargetSort(this));
				RepeatEffectList.Add(component);
				AuraEffectList.Add(component);
			}

			foreach (PrefabWithTime auraEffect in auraEffects)
			{
				if (auraEffect.EffectDifficlty == PrefabWithTime.eEffectDifficulty.ALL_FOR_CHARA)
				{
					SkillEffectCtrl skillEffectCtrl = initializeCharaAuraEffect(auraEffect.Prefab, auraEffect.TargetBoneName, auraEffect.IsTrackRotation);
					SkillEffectCtrl skillEffectCtrl2 = initializeCharaAuraEffect(auraEffect.PrefabLeft, auraEffect.TargetBoneName, auraEffect.IsTrackRotation);
					if (IsLeftDir)
					{
						skillEffectCtrl.SetActive(_isActive: false);
					}
					else
					{
						skillEffectCtrl2.SetActive(_isActive: false);
					}

					ComponentAbnormal.CharaAuraEffectList.Add((skillEffectCtrl, skillEffectCtrl2));
				}
			}

			PrincessFormProcessor = new PrincessFormProcessor();
			PrincessFormProcessor.Initialize(this, UnitActionController, battleManager, battleTimeScale);
			MasterExEquipmentData masterExEquipmentData = ManagerSingleton<MasterDataManager>.Instance.masterExEquipmentData;
			List<int> list4 = new List<int>();
			List<ExtraEquipSlot> list5 = unitData.ExEquipSlot;
			if (list5 == null)
			{
				list5 = new List<ExtraEquipSlot>();
			}

			UserData instance2 = Singleton<UserData>.Instance;
			for (int num11 = 0; num11 < list5.Count; num11++)
			{
				ExtraEquipSlot extraEquipSlot = list5[num11];
				if ((int)extraEquipSlot.SerialId == 0 && (int)extraEquipSlot.ExEquipmentId == 0)
				{
					continue;
				}

				int ex_equipment_id = extraEquipSlot.ExEquipmentId;
				if ((int)extraEquipSlot.SerialId != 0)
				{
					ExtraEquipPair extraEquipPair = instance2.SearchExtraEquip(extraEquipSlot.SerialId);
					if (extraEquipPair != null)
					{
						ex_equipment_id = extraEquipPair.ExEquipmentId;
					}
				}

				MasterExEquipmentData.ExEquipmentData exEquipmentData = masterExEquipmentData.Get(ex_equipment_id);
				if ((int)exEquipmentData.Skill1 != 0)
				{
					list4.Add(exEquipmentData.Skill1);
				}

				if ((int)exEquipmentData.Skill2 != 0)
				{
					list4.Add(exEquipmentData.Skill2);
				}
			}

			foreach (int item2 in list4)
			{
				ExSkillData exSkillData = new ExSkillData();
				exSkillData.Initialize(item2, this, UnitActionController, battleManager);
				ComponentPassiveSkill.RegisterExSkill(exSkillData);
			}
		}

		[global::OpenPCR.Migration("2a848c92-f8f4-4de3-9399-45222f5bd20f")]
		private int searchSpSkillLevel(int _baseSkillId, UnitParameter _unitParameter)
		{
			var data = _unitParameter.SkillData;
			int index = data.UnionBurstIds.FindIndex(skillId => skillId == _baseSkillId);
			if (index >= 0)
			{
				int evolved = data.UnionBurstEvolutionIds[index];
				if (evolved != 0 && SkillLevels[evolved] != 0) return SkillLevels[evolved];
			}
			index = data.MainSkillIds.FindIndex(skillId => skillId == _baseSkillId);
			if (index >= 0)
			{
				int evolved = data.MainSkillEvolutionIds[index];
				if (evolved != 0 && SkillLevels[evolved] != 0) return SkillLevels[evolved];
			}
			index = data.ExSkillIds.FindIndex(skillId => skillId == _baseSkillId);
			if (index >= 0)
			{
				int evolved = data.ExSkillEvolutionIds[index];
				if (evolved != 0 && SkillLevels[evolved] != 0) return SkillLevels[evolved];
			}
			return SkillLevels[_baseSkillId];
		}

		[global::OpenPCR.Migration("c33d853e-2016-4cf6-bc42-6ac89720fe9b")]
		private SkillEffectCtrl initializeCharaAuraEffect(GameObject _effectPrefab, string _targetBoneName, bool _isTrackRotation)
		{
			SkillEffectCtrl effect = battleEffectPool.GetEffect(_effectPrefab, this);
			effect.InitializeSort();
			effect.PlaySe(this.ComponentSound.SoundUnitId, IsLeftDir);
			effect.transform.position = base.transform.position;
			effect.transform.parent = ExceptUIRoot.Transform;
			effect.SortTarget = this;
			effect.ExecAppendCoroutine(this, _isAbnormal: true);
			effect.StartUpdateDisappear(_isUnionBurst: false, this.ComponentMultiPartsBoss.DummyPartsData);
			effect.SetSortOrderBack();
			if (_targetBoneName.IsNullOrEmpty())
			{
				battleManager.StartCoroutine(effect.TrackTarget(this.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero));
			}
			else
			{
				battleManager.StartCoroutine(effect.TrackTarget(this.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero, _followX: true, _followY: true, GetCurrentSpineCtrl().skeleton.FindBone(_targetBoneName), _isTrackRotation));
			}

			battleManager.StartCoroutine(effect.TrackTargetSort(this));
			RepeatEffectList.Add(effect);
			AuraEffectList.Add(effect);
			return effect;
		}

		[global::OpenPCR.Migration("b5340e0a-7628-4475-a3b4-6ef97e61d30c")]
		public void ExecActionOnStartAndDetermineInstanceID()
		{
			UnitInstanceId = battleManager.UnitCount++;
			UnitActionController.ExecActionOnStart();
		}

		[global::OpenPCR.Migration("c9d92ded-d21a-4078-a53d-3cd417948e1d")]
		public void BattleStartProcess(eUnitRespawnPos respawnPos)
		{
			RespawnPos = respawnPos;
			AppendCoroutine(updateAttackTarget(), ePauseType.SYSTEM, this);
			IsDepthBack = IsGameStartDepthBack;
			foreach (var charaAuraEffect in ComponentAbnormal.CharaAuraEffectList)
			{
				charaAuraEffect.left.SetSortOrderBack();
				charaAuraEffect.right.SetSortOrderBack();
			}

			if (battleManager.BlackOutUnitList.Contains(this))
			{
				ComponentSortOrder.SetSortOrderFront();
			}
			else
			{
				ComponentSortOrder.SetSortOrderBack();
			}
		}

		[global::OpenPCR.Migration("c4cc3fad-30b2-43f5-b7aa-1774bac8d270")]
		private IEnumerator updateAttackTarget()
		{
			while (true)
			{
				if ((int)battleManager.GameState == 0)
				{
					ComponentSearchArea.UpdateAttackTargetImpl(ComponentAbnormal.GetTauntSource());
				}

				yield return null;
			}
		}

		[global::OpenPCR.Migration("bbfa1dc8-6f00-4fc3-b1d8-935d41284adf")]
		public void WaveStartProcess(bool _first)
		{
			ComponentParameter.InitParamValue(_first);
			ComponentPassiveSkill.ApplyPassiveSkill(_first, true);
			this.ComponentParameter.MaxHpAfterPassive = this.ComponentParameter.MaxHp;
			ComponentActionPattern.Reset();
			ResetCastTime();
			if (IsOther)
			{
				resetPosForEnemyUnit(RespawnPos);
			}
			else
			{
				ResetPosForUserUnit(BattleDefine.UnitRespawnPosList.IndexOf(RespawnPos));
			}

			this.ComponentState.CreateRunSmoke();
			ComponentRelicSkill.ResetExecedFlag();
			foreach (KeyValuePair<eExSkillCondition, List<ExSkillData>> item in this.ComponentPassiveSkill.ConditionExSkillDictionary)
			{
				foreach (ExSkillData item2 in item.Value)
				{
					item2.ResetLimitNum(this);
				}
			}

			UnitActionController.ExecActionOnWaveStart();
		}

		[global::OpenPCR.Migration("b748cdc8-fb90-42c0-94d0-c7e08c4dffcd")]
		public void ResetPosForUserUnit(int index)
		{
			Vector2 vector = base.transform.localPosition;
			vector.x = -560f;
			if (battleManager.BattleCategory == eBattleCategory.TUTORIAL && battleManager.CurrentWave == 0)
			{
				ComponentState.IdleOnly = true;
				if (battleManager.BattleCategory == eBattleCategory.TUTORIAL)
				{
					vector.x = TutorialDefine.UNIT_DEFAULT_POS[ComponentSound.SoundUnitId];
				}
			}
			else
			{
				vector.x -= 200f * (float)(index + 1);
			}

			if (battleManager.IsBossBattle)
			{
				vector.x -= ComponentParameter.MoveSpeed * battleManager.GetBossUnit().UnitAppearDelay * 1.6f;
			}

			base.transform.localPosition = vector;
		}

		[global::OpenPCR.Migration("f2ff55d2-c59f-445d-8a84-ed8d6bc837e9")]
		private void resetPosForEnemyUnit(eUnitRespawnPos pos)
		{
			int num = BattleDefine.UnitRespawnPosList.IndexOf(pos);
			eBattleCategory battleCategory = battleManager.BattleCategory;
			Vector3 localPosition = new Vector3(560f, battleManager.GetRespawnPos(pos), 0f);
			if (IsBoss)
			{
				localPosition.y = battleManager.GetRespawnPos(eUnitRespawnPos.MAIN_POS_5);
				localPosition.x += BossDeltaX * 540f;
				localPosition.y += BossDeltaY * 540f;
				localPosition.x -= -34f;
			}
			else if (battleCategory == eBattleCategory.TUTORIAL && battleManager.CurrentWave == 0)
			{
				localPosition.x = TutorialDefine.HIKARITAKE_POS;
			}
			else
			{
				localPosition.x += 200f * (float)num;
				if (battleManager.IsBossBattle && !IsBonusEnemy)
				{
					float oldMoveSpeed = ComponentParameter.MoveSpeed;
					float bossDelay = battleManager.GetBossUnit().UnitAppearDelay;
					bool flag = true;
					eSpecialBattlePurpose purpose = battleManager.GetPurpose();
					if (purpose == eSpecialBattlePurpose.DEFEAT_OTHERS || purpose == eSpecialBattlePurpose.GET_POINT)
					{
						int initialPosition = battleManager.GetInitialPosition(UnitId);
						localPosition.x = 100f + (float)initialPosition;
						if (battleManager.GetSpecialBattleData().StartIdleTrigger == 0.0)
						{
							this.ComponentState.SetState(ActionState.GAME_START);
						}
						else
						{
							GetCurrentSpineCtrl().CurColor = new Color(1f, 1f, 1f, 0f);
							float moveSpeed = this.ComponentParameter.MoveSpeed;
							this.ComponentParameter.MoveSpeed = 0f;
							ComponentState.IsForceStartStay = true;
							this.ComponentState.SetState(ActionState.IDLE);
							AppendCoroutine(waitShadowAppear(moveSpeed), ePauseType.SYSTEM);
						}

						flag = false;
					}

					if (IsShadow & flag)
					{
						localPosition.x = 100f + this.ComponentSearchArea.SearchAreaSize;
						AppendCoroutine(waitBossMotionEnd(), ePauseType.SYSTEM);
						IsMoveSpeedForceZero = true;
					}
					else if (flag)
					{
						localPosition.x += oldMoveSpeed * bossDelay * 1.6f;
					}
				}
			}

			base.transform.localPosition = localPosition;
		}

		[global::OpenPCR.Migration("7967ee6e-814f-43a3-a081-8d5632c8d193")]
		private IEnumerator waitShadowAppear(float _oldMoveSpeed)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.UnitCtrl::waitShadowAppear(System.Single):System.Collections.IEnumerator (outside first-phase special-purpose-shadow boundary)"); }

		[global::OpenPCR.Migration("e0c8ce4b-50cd-4d58-bc56-da8bbb7b7e1a")]
		private IEnumerator waitBossMotionEnd()
		{
			while (!battleManager.GetBossUnit().ComponentState.GameStartDone)
			{
				yield return null;
			}

			IsMoveSpeedForceZero = false;
			SetLeftDirection(IsOther);
			ComponentState.SetState(ActionState.IDLE);
		}

		[global::OpenPCR.Migration("5eecee21-b191-4c06-9ed4-46bdce47b108")]
		public void SetOverlapPos(float overlapPosX)
		{
			Vector3 position = BottomTransform.transform.localPosition;
			BottomTransform.transform.localPosition = new Vector3(position.x + overlapPosX, position.y, 0f);
			OverlapPosX = overlapPosX;
			Vector3 gaugePosition = LifeGauge.transform.localPosition;
			LifeGauge.transform.localPosition = new Vector3(gaugePosition.x + overlapPosX, gaugePosition.y, 0f);
		}

		[global::OpenPCR.Migration("e6d3ab5d-a19e-40f1-80ca-8a37688d1028")]
		public void _Update()
		{
			if (TimeToDie)
			{
				DestroyAndCoroutineRemove();
			}
			else
			{
				ComponentUnionBurst.SubtractCoolDownTime(GetBattleDeltaTime(), battleManager.GetBlackOutUnitLength() != 0);
				ComponentDamageControl.ReduceSortOffsetResetTimer(battleManager.DeltaTime);
			}
		}

		[global::OpenPCR.Migration("66b365dd-77ca-4782-97f6-c3621690d3c3")]
		public void SetLeftDirection(bool bLeftDir)
		{
			IsLeftDir = bLeftDir;
			if (ComponentAbnormal.ToadDatas.Count > 0)
			{
				GetCurrentSpineCtrl().transform.localScale = ((IsLeftDir || IsForceLeftDir) ? this.ComponentAbnormal.ToadDatas[0].LeftDirScale : this.ComponentAbnormal.ToadDatas[0].RightDirScale) * this.ComponentScaleChange.ScaleChangeValue;
			}
			else
			{
				GetCurrentSpineCtrl().transform.localScale = ((IsLeftDir || IsForceLeftDirOrPartsBoss) ? leftDirScale : rightDirScale) * this.ComponentScaleChange.ScaleChangeValue;
			}

			MoveRate = (IsMoveSpeedForceZero ? 0f : ((float)this.ComponentParameterClamped.MoveSpeedOrMin * (IsLeftDir ? (-1f) : 1f)));
			bool flag = bLeftDir || IsForceLeftDirOrPartsBoss;
			foreach (KeyValuePair<eAbnormalStateCategory, AbnormalStateCategoryData> item in this.ComponentAbnormal.AbnormalStateCategoryDataDictionary)
			{
				for (int i = 0; i < item.Value.Effects.Count; i++)
				{
					AbnormalStateEffectGameObject abnormalStateEffectGameObject = item.Value.Effects[i];
					if (abnormalStateEffectGameObject.LeftEffect != null)
					{
						abnormalStateEffectGameObject.LeftEffect.SetActive(flag && this.ComponentAbnormal.IsAbnormalEffectEnable);
						if (abnormalStateEffectGameObject.RightEffect != null)
						{
							abnormalStateEffectGameObject.RightEffect.SetActive(!flag && this.ComponentAbnormal.IsAbnormalEffectEnable);
						}
					}
				}
			}

			bool flag2 = this.ComponentAbnormal.IsAbnormalEffectEnable && this.ComponentAbnormal.IsCharaAuraEnable;
			foreach (var charaAuraEffect in this.ComponentAbnormal.CharaAuraEffectList)
			{
				charaAuraEffect.left.SetActive(flag & flag2);
				charaAuraEffect.right.SetActive(!flag & flag2);
			}

			if (this.ComponentAbnormal.MaxHpDebufCounter != 0)
			{
				if (this.ComponentAbnormal.MaxHpDebufEffectLeft != null)
				{
					ComponentAbnormal.MaxHpDebufEffectLeft.SetActive(flag && this.ComponentAbnormal.IsAbnormalEffectEnable);
				}

				if (this.ComponentAbnormal.MaxHpDebufEffectRight != null)
				{
					ComponentAbnormal.MaxHpDebufEffectRight.SetActive(!flag && this.ComponentAbnormal.IsAbnormalEffectEnable);
				}
			}
		}

		[global::OpenPCR.Migration("5ce9dc63-bf66-463d-b0d6-396f2fd09a32")]
		public void SetDirectionAuto()
		{
			SetLeftDirection(isNearestEnemyLeft());
		}

		[global::OpenPCR.Migration("f6b8ff13-7db7-468a-8c39-3cad7d42e9a9")]
		private bool isNearestEnemyLeft()
		{
			List<UnitCtrl> list = (this.ComponentAbnormal.IsConfusionOrConvert() ? ((!IsOther) ? battleManager.UnitList : battleManager.EnemyList) : (IsOther ? battleManager.UnitList : battleManager.EnemyList));
			float num = BattleDefine.BATTLE_FIELD_SIZE * 2f * (float)(IsOther ? 1 : (-1));
			if (!ComponentAbnormal.IsConfusionOrConvert() && ComponentAbnormal.IsTauntState())
				list = new List<UnitCtrl> { ComponentAbnormal.TauntSource.Owner };
			for (int i = 0; i < list.Count; i++)
			{
				UnitCtrl unitCtrl = list[i];
				if (unitCtrl.IsPhantom || unitCtrl.ComponentState.IsDead || unitCtrl == this || (long)unitCtrl.ComponentParameter.Hp == 0L || unitCtrl.ComponentAbnormal.IsStealth || unitCtrl.ComponentAbnormal.IsSpyOrEtherialization(this))
				{
					continue;
				}

				if (!unitCtrl.IsPartsBoss)
				{
					if (Mathf.Abs(num) > Mathf.Abs(base.transform.localPosition.x - unitCtrl.transform.localPosition.x) || BattleUtil.Approximately(Mathf.Abs(num), Mathf.Abs(base.transform.localPosition.x - unitCtrl.transform.localPosition.x)))
					{
						num = base.transform.localPosition.x - unitCtrl.transform.localPosition.x;
					}

					continue;
				}

				for (int j = 0; j < unitCtrl.ComponentMultiPartsBoss.BossPartsListForBattle.Count; j++)
				{
					PartsData partsData = unitCtrl.ComponentMultiPartsBoss.BossPartsListForBattle[j];
					if (Mathf.Abs(num) > Mathf.Abs(base.transform.localPosition.x - unitCtrl.transform.localPosition.x - partsData.PositionX) || BattleUtil.Approximately(Mathf.Abs(num), Mathf.Abs(base.transform.localPosition.x - unitCtrl.transform.localPosition.x - partsData.PositionX)))
					{
						num = base.transform.localPosition.x - unitCtrl.transform.localPosition.x - partsData.PositionX;
					}
				}
			}

			return num > 0f;
		}

		[global::OpenPCR.Migration("8725dfc0-0987-43e8-b3d9-3c0a177ecc8b")]
		public void SetMotionPause()
		{
			if (battleManager != null && battleManager.BlackoutUnitTargetList != null && !battleManager.BlackoutUnitTargetList.Contains(this) && GetCurrentSpineCtrl() != null)
			{
				GetCurrentSpineCtrl().Pause();
			}
		}

		[global::OpenPCR.Migration("230b41b2-a732-444b-9f32-d3027fa63636")]
		public void ChangeChargeSkill(int _skillNum, float _limitTime)
		{
			int oldChargeSkillNum = UnionBurstSkillId;
			UnionBurstSkillId = _skillNum;
			if (_limitTime > 0f)
			{
				AppendCoroutine(updateChangeSkillNum(oldChargeSkillNum, _limitTime), ePauseType.SYSTEM, this);
			}
		}

		[global::OpenPCR.Migration("270c18b6-be16-45ca-988b-e2e92ef184ad")]
		private IEnumerator updateChangeSkillNum(int oldChargeSkillNum, float limitTime)
		{
			float time = 0f;
			while (true)
			{
				time += DeltaTimeForPause;
				if (time > limitTime)
				{
					UnionBurstSkillId = oldChargeSkillNum;
				}

				yield return null;
			}
		}

		[global::OpenPCR.Migration("22a2cb40-3119-4e02-87cb-5fac1fc54a8c")]
		public IEnumerator UpdateSummon(int _skillNum, eUnitRespawnPos _respawnPos, SummonAction.eMoveType _moveType, Vector3 _targetPosition, float _moveSpeed)
		{
			string currentMotionName = GetCurrentSpineCtrl().AnimationName;
			while (true)
			{
				if (!battleManager.CoroutineManager.VisualPause && GetCurrentSpineCtrl().state.TimeScale == 0f)
				{
					GetCurrentSpineCtrl().Resume();
				}

				if (this.ComponentAbnormal.IsUnableActionState())
				{
					break;
				}

				if (GetCurrentSpineCtrl().AnimationName != currentMotionName && this.ComponentState.HasSetStateCalled)
				{
					BattleStartProcess(_respawnPos);
					yield break;
				}

				if (!GetCurrentSpineCtrl().IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			switch (_moveType)
			{
				case SummonAction.eMoveType.NORMAL:
					BattleStartProcess(_respawnPos);
					base.transform.SetLocalPosY(battleManager.GetRespawnPos(_respawnPos));
					this.ComponentState.SetState(ActionState.WALK);
					foreach (SkillEffectCtrl repeatEffect in RepeatEffectList)
					{
						repeatEffect.SetSortOrderBack();
					}

					break;
				case SummonAction.eMoveType.LINEAR:
					{
						if (GetCurrentSpineCtrl().IsAnimation(eAnimKitCharacterAnimeId.SUMMON, _skillNum, 1))
						{
							this.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.SUMMON, _skillNum, 1);
						}

						_targetPosition.y = battleManager.GetRespawnPos(_respawnPos);
						Vector3 v = (_targetPosition - base.transform.localPosition).normalized * _moveSpeed;
						SetLeftDirection(v.x < 0f);
						float duration = (_targetPosition - base.transform.localPosition).x / v.x;
						float time = 0f;
						while (true)
						{
							time += battleManager.DeltaTime;
							base.transform.localPosition += v * battleManager.DeltaTime;
							if (time > duration)
							{
								break;
							}

							yield return null;
						}

						if (GetCurrentSpineCtrl().IsAnimation(eAnimKitCharacterAnimeId.SUMMON, _skillNum, 2))
						{
							this.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.SUMMON, _skillNum, 2, -1, _isLoop: false);
						}

						while (true)
						{
							if (!battleManager.CoroutineManager.VisualPause && GetCurrentSpineCtrl().state.TimeScale == 0f)
							{
								GetCurrentSpineCtrl().Resume();
							}

							if (!GetCurrentSpineCtrl().IsPlayAnimeBattle)
							{
								break;
							}

							yield return null;
						}

						BattleStartProcess(_respawnPos);
						base.transform.localPosition = _targetPosition;
						this.ComponentState.SetState(ActionState.WALK);
						break;
					}
			}
		}

		[global::OpenPCR.Migration("a08695f0-ce37-4400-ae04-8d7996255141")]
		public void RestartPlayAnimeCoroutine(float _startTime, eAnimKitCharacterAnimeId _animeId, int _index, int _prefix)
		{
			GetCurrentSpineCtrl().RestartPlayAnimeCoroutine(_startTime, _animeId, _index, _prefix);
		}

		[global::OpenPCR.Migration("ebc8f769-f667-4df8-bb90-8bcc3e1ebf2d")]
		public void AppendCoroutine(IEnumerator _cr, ePauseType _pauseType, UnitCtrl _unit = null)
		{
			battleManager.AppendCoroutine(_cr, _pauseType, _unit);
		}

		[global::OpenPCR.Migration("8ad9020f-8edc-4568-8719-231fd1ac7075")]
		private void onHpChanged(long _hp)
		{
			if (_hp <= 0L)
			{
				GetCurrentSpineCtrl().CurColor = new Color(1f, 1f, 1f, 0f);
				ComponentState.OnDeadForRevival = null;
				ComponentState.SetState(ActionState.DIE);
				ComponentState.StandByDone = true;
				IsDeadBySetCurrentHp = true;
			}

			if (StartHpPercent == 0f)
			{
				StartHpPercent = (float)_hp / (float)(long)ComponentParameter.MaxHp;
			}
		}

		[global::OpenPCR.Migration("2b5f3af9-5df7-4c27-9d56-3a74e5273bf3")]
		public void IndicateSkillName(string _skillName)
		{
			LifeGauge.IndicateSkillName(_skillName);
		}

		[global::OpenPCR.Migration("110f4d01-91b8-46c2-ab53-6dc34d7074c1")]
		public void ExecSkillBySkillId(int _skillId, int _bonusId)
		{
			UnitActionController.StartAction(_skillId);
			UnitActionController.SetBonusId(_skillId, _bonusId);
		}

		[global::OpenPCR.Migration("edadba66-19be-4f89-b42c-846b121f55a9")]
		public float GetBattleDeltaTime()
		{
			return battleManager.DeltaTime;
		}

		[global::OpenPCR.Migration("5a2e7b1a-9281-4a2b-88f9-2bca421e8d2d")]
		public int GetSortOrderConsiderBlackout()
		{ return global::OpenPCR.Presentation.Noop<int>(); }

		[global::OpenPCR.Migration("89378682-7e1a-44de-a5fe-e0795153edfc")]
		public void PlayUbChainVoice()
		{
			if (battleTimeScale.SpeedupFlag || !battleManager.CanPlayVoice() || battleManager.ChainVoicePlayUnit != this) return;
			var alive = (IsOther ? battleManager.EnemyList : battleManager.UnitList)
				.FindAll(unit => unit != this && (long)unit.ComponentParameter.Hp > 0L && !unit.IsSummonOrPhantom);
			ComponentSound.PlayUbChainVoice(ComponentSound.UnionBurstResponceVoiceType, UnitId, ComponentState.CutinVoiceStatus, alive);
			battleManager.ChainVoicePlayUnit = null;
		}

		[global::OpenPCR.Migration("a500e484-339e-471f-9d97-3b1eed05c8c9")]
		public void AppendBreakLog(UnitCtrl _source)
		{
			battleLog.AppendBattleLog(eBattleLogType.BREAK, 0, 0L, 0L, 0, 0, 1, 1, _source, this);
		}

		[global::OpenPCR.Migration("5830c061-82cb-4838-9940-86e7e07bd4a3")]
		public void PreparePrincessFormProcessor()
		{
			PrincessFormProcessor.Prepare();
		}

		[global::OpenPCR.Migration("a870b9d7-cc33-4225-862b-3298a6a1f14b")]
		public SkillEffectCtrl GetEffect(GameObject _prefab, UnitCtrl _owner = null)
		{
			return battleEffectPool.GetEffect(_prefab, _owner);
		}

		[global::OpenPCR.Migration("18afb152-61ad-4f61-9dbc-70856f92c765")]
		public void StopZoomEffect()
		{
			battleCameraEffect.StopZoomEffect(this);
		}

		[global::OpenPCR.Migration("fe5c7329-ef74-451e-9a7c-38fd0d5cf56c")]
		public void StartShake(ShakeEffect _shake, Skill _skill, UnitCtrl _unit)
		{
			battleCameraEffect.StartShake(_shake, _skill, _unit);
		}

		[global::OpenPCR.Migration("5560e465-5ab9-40f5-b4fc-8d7f9efbd382")]
		private void setIdleCastTime()
		{
			float speed = ComponentAbnormal.CalcAbnormalStateSpeed();
			if (BattleUtil.Approximately(speed, 0f))
			{
				CastTimer = 90f;
			}
			else
			{
				CastTimer = (float)CastTimer / speed;
			}
		}

		public enum eBonusEnemyType
		{
			NONE,
			EXTRA_EFFECT,
			RELIC
		}

		public UnitActionController UnitActionController { get; private set; }
		public PrincessFormProcessor PrincessFormProcessor { get; private set; }
		public SystemIdDefine.eWeaponSeType WeaponSeType { get; set; }
		public SystemIdDefine.eWeaponMotionType WeaponMotionType { get; set; }
		public List<UnitUnionBurstTimeline> UnitUnionBurstTimelineList { get; set; }
		public List<CircleEffectController> CircleEffectList { get; private set; }
		public int PrefabId { get; private set; }
		public eBonusEnemyType BonusEnemyType { get; set; }
		public bool IsRelicEnemy
		{
			[global::OpenPCR.Migration("19e1091e-b204-4b21-9375-b4086812f4b6")]
			get { return BonusEnemyType == eBonusEnemyType.RELIC; }
		}
		public bool IsDivisionSourceForDamage { get; set; }
		public bool IsDivisionSourceForDie { get; set; }
		public bool IsClanBattleOrSekaiEnemy { get; set; }
		public bool IsTrialBattleEnemy { get; set; }
		public bool IsAbyssBossEnemy { get; set; }
		public int CurrentSpineSpeedCoroutineId { get; set; }
		public Vector3 FixedStatePos { get; set; }
		public LifeGaugeController LifeGauge { get; private set; }
		public global::System.Single CastTimer { get; set; }
		public global::System.Double SkillStackValDmg { get; private set; }
		public global::System.Double SkillStackVal { get; private set; }
		public float MoveRate { get; private set; }
		public bool IsAwakeMotion { get; set; }
		public bool IsStartSkillExeced { get; set; }
		public PrefabWithTime.eEffectDifficulty EffectDifficulty { get; private set; }
		public Action<UnitCtrl> OnInitCallbackForUi { get; set; }
		private bool isPaused { get; set; }
		private float unionBurstSkillAreaWidth { get; set; }
		public int Index { get; set; }
		public int CharacterUnitId { get; protected set; }
		public int IconSkin { get; private set; }
		public int Rarity { get; private set; }
		public int BattleRarity { get; private set; }
		[global::OpenPCR.Migration("19742905-a53f-46bb-b51a-7c0dfa45935b")] public ePromotionLevel PromotionLevel { [global::OpenPCR.Migration("befe1457-77d2-4044-be99-04bb058d61c6")] get; [global::OpenPCR.Migration("60d4e13c-cb52-4d07-83d8-369b3b44bd78")] private set; }
		public float AtkRecastTime { get; private set; }
		public List<int> TalentIds { get; private set; }
		public int RoleId { get; private set; }
		public bool IsDeadBySetCurrentHp { get; private set; }

		[global::OpenPCR.Migration("cc2255cd-f7c3-45e9-baf8-9a533caa9ac0")]
		protected Yggdrasil<UnitCtrl> singletonTree
		{
			[global::OpenPCR.Migration("ce06f98f-6a3a-4ced-847b-84c8b5843e57")]
			get
			{
				return staticSingletonTree;
			}
		}

		[global::OpenPCR.Migration("7aa1a307-7252-4ace-89de-65d06e3b5e40")]
		protected IBattleCameraEffectForUnitCtrl battleCameraEffect
		{
			[global::OpenPCR.Migration("f7a0a991-6489-43cb-ae2b-d9c78f4856ed")]
			get
			{
				return staticBattleCameraEffect;
			}
		}

		[global::OpenPCR.Migration("8b729ee8-46e5-4832-acf1-57a8a2980193")]
		protected IBattleEffectPool battleEffectPool
		{
			[global::OpenPCR.Migration("43ee49a7-d40c-4f4a-be73-aeb0acb55995")]
			get
			{
				return staticBattleEffectPool;
			}
		}

		[global::OpenPCR.Migration("d5e31222-3655-4280-b74a-35936e2b5f36")]
		protected IBattleTimeScaleForUnitCtrl battleTimeScale
		{
			[global::OpenPCR.Migration("9ababe56-2e4e-490d-b239-494dbe7e9475")]
			get
			{
				return staticBattleTimeScale;
			}
		}

		[global::OpenPCR.Migration("e4d71c24-1632-41d1-bb47-77fab8316333")]
		public List<UnitCtrl> GetFriendList()
		{
			return IsOther ? battleManager.EnemyList : battleManager.UnitList;
		}

		[global::OpenPCR.Migration("e3293b29-b288-4627-b04c-23f62633a12e")]
		public List<UnitCtrl> GetEnemyList()
		{
			return IsOther ? battleManager.UnitList : battleManager.EnemyList;
		}

		[global::OpenPCR.Migration("6ca06789-5c53-4755-8111-4e833a041142")]
		public int GetBattleFrameCount()
		{
			return (int)battleManager.FrameCount;
		}

		[global::OpenPCR.Migration("c57dd592-0fd6-47bb-8067-93d054678c60")]
		public bool GetIsPrincessFormUnionBurstRunning()
		{
			return battleManager.IsPrincessSkillMovieRunning || battleManager.IsPlayingNoCutInMotionForPSkill;
		}

		[global::OpenPCR.Migration("ba214e89-43d2-453e-992e-37a0e6931ff9")]
		public bool GetIsMuteVoice()
		{
			return battleManager.MuteVoice;
		}

		[global::OpenPCR.Migration("be98a88c-000a-4dce-ac92-12d231b8cfef")]
		public bool IsInitialMode()
		{
			return battleManager.IsInitialMode();
		}

		[global::OpenPCR.Migration("3371aa72-9f96-434c-b934-006f201aec75")]
		public void UpdateLastRetireVoiceTime()
		{
			battleManager.LastRetireVoiceTime = Time.time;
		}

		[global::OpenPCR.Migration("89e6fe34-9584-4f27-8aac-0d5cbec983d2")]
		public void UpdateLastDamageVoiceTime()
		{
			battleManager.LastDamageVoiceTime = Time.time;
		}

		[global::OpenPCR.Migration("4b60c278-0b0e-47e1-aa68-74ccb5afd429")]
		public Dictionary<eParamType, PassiveActionValue> GetPassiveDictionaryByEnemy()
		{
			return battleManager.GetPassiveDictionaryByEnemy(IsOther);
		}

		[global::OpenPCR.Migration("33c30482-1dcd-498d-b8fd-f494583edfcd")]
		public Dictionary<eParamType, PassiveActionValue> GetPassiveDictionaryByUnit()
		{
			return battleManager.GetPassiveDictionaryByUnit(IsOther);
		}

		[global::OpenPCR.Migration("dfa5ebc0-d3a4-47dc-a46e-9aaf9ab6e48b")]
		public void ResetThanksTargetUnit()
		{
			battleManager.ResetThanksTargetUnit();
		}

		[global::OpenPCR.Migration("0fcddb56-3fdf-4d8d-b146-7a97b84fdd94")]
		public void ResetBattleCameraShakeEffect()
		{
			battleCameraEffect.ShakePosition = Vector3.zero;
		}

		[global::OpenPCR.Migration("756c8163-a7ff-453e-85c1-610e58dbd8ac")]
		public bool CanBattleManagerPlayCutIn()
		{
			return battleManager.CanPlayCutIn();
		}

		[global::OpenPCR.Migration("2e0095ac-9d58-4ec5-89e7-31ac8a69f11e")]
		public bool GetSpeedUpFlag()
		{
			return battleTimeScale.SpeedupFlag;
		}

		[global::OpenPCR.Migration("7916a926-22b8-4fcc-9312-020f76bb95f0")]
		public void SetMotionResume()
		{
			if (GetCurrentSpineCtrl() != null)
			{
				GetCurrentSpineCtrl().Resume();
			}
		}

		[global::OpenPCR.Migration("e08341df-c8db-4d37-add7-21462d30a859")]
		public void ResetCastTime()
		{
			switch (battleManager.BattleCategory)
			{
				case (eBattleCategory)6:
					CastTimer = 0.3f;
					break;
				case (eBattleCategory)5:
					CastTimer = 90f;
					break;
				default:
					CastTimer = battleManager.CurrentWave == 0 ? 2.5f : 0.3f;
					break;
			}
		}

		[global::OpenPCR.Migration("ff14728b-75a4-4275-b767-2100b602ef56")]
		public void SetRecastTime(int _skillId)
		{
			if (_skillId != 0)
			{
				CastTimer = _skillId == 1 ? AtkRecastTime : castTimeDictionary[_skillId];
				setIdleCastTime();
			}
		}

		[global::OpenPCR.Migration("9b31b0b5-441a-4e01-b4dc-53a75ddc2aef")]
		public bool JudgeIsRarity6()
		{
			return Rarity == 6;
		}

		[global::OpenPCR.Migration("7186503c-2535-4b19-963c-cc8819205655")]
		private void registerAllComponentSet()
		{
			ComponentAbnormal.RegisterComponentSet(this);
			ComponentActionPattern.RegisterComponentSet(this);
			ComponentAnimation.RegisterComponentSet(this);
			ComponentAnnihilation.RegisterComponentSet(this);
			ComponentBattleResult.RegisterComponentSet(this);
			ComponentColor.RegisterComponentSet(this);
			ComponentCompare.RegisterComponentSet(this);
			ComponentDamageControl.RegisterComponentSet(this);
			ComponentLog.RegisterComponentSet(this);
			ComponentMultiPartsBoss.RegisterComponentSet(this);
			ComponentParameter.RegisterComponentSet(this);
			ComponentParameterClamped.RegisterComponentSet(this);
			ComponentPassiveSkill.RegisterComponentSet(this);
			ComponentRelicPassiveSkill.RegisterComponentSet(this);
			ComponentScaleChange.RegisterComponentSet(this);
			ComponentSearchArea.RegisterComponentSet(this);
			ComponentSortOrder.RegisterComponentSet(this);
			ComponentSound.RegisterComponentSet(this);
			ComponentState.RegisterComponentSet(this);
			ComponentUnionBurst.RegisterComponentSet(this);
		}

		[global::OpenPCR.Migration("673b80cc-c336-4138-a573-8973ab3f3516")]
		public void SetSpineSortOrder(int _value)
		{
			global::OpenPCR.Presentation.Noop();
		}
	}
}
