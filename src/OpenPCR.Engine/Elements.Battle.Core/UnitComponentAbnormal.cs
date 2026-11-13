using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System.Linq;
using Elements.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UnitComponentAbnormal : UnitComponentBase, IUnitComponent
	{
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalState, bool> abnormalStateFlagDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalState, System.Boolean>(new eAbnormalState_DictComparer());
		public global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalStateCategory, global::Elements.Battle.Core.AbnormalStateCategoryData> AbnormalStateCategoryDataDictionary { get; private set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalStateCategory, Elements.Battle.Core.AbnormalStateCategoryData>(new eAbnormalStateCategory_DictComparer());
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalState, global::System.Collections.Generic.List<int>> overlapAbnormalStateIndexList { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalState, System.Collections.Generic.List<System.Int32>>
		{
			{
				eAbnormalState.HASTE_OVERLAP,
				new List<int>()
			},
			{
				eAbnormalState.SLOW_OVERLAP,
				new List<int>()
			},
			{
				eAbnormalState.HASTE,
				new List<int>()
			},
			{
				eAbnormalState.SLOW,
				new List<int>()
			}
		};
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalState, bool> overlapAbnormalStateIconDisplayFlag { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalState, System.Boolean>
		{
			{
				eAbnormalState.HASTE,
				false
			},
			{
				eAbnormalState.SLOW,
				false
			}
		};
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalState, int> overlapAbnormalStateShowIconCount { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalState, System.Int32>
		{
			{
				eAbnormalState.HASTE,
				0
			},
			{
				eAbnormalState.SLOW,
				0
			}
		};
		private global::System.Collections.Generic.Dictionary<int, global::Elements.Battle.Core.AbnormalStateCategoryData> overlapAbnormalStateData { get; set; } = new System.Collections.Generic.Dictionary<System.Int32, Elements.Battle.Core.AbnormalStateCategoryData>();
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalState, global::System.Collections.Generic.List<global::Elements.SlipDamageUpData>> slipDamageUpDataDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalState, System.Collections.Generic.List<Elements.SlipDamageUpData>>();
		private global::System.Collections.Generic.List<global::Elements.SlipDamageUpData> allSlipDamageUpDataList { get; set; } = new System.Collections.Generic.List<Elements.SlipDamageUpData>();
		private global::System.Collections.Generic.Dictionary<global::OpenPCR.Runtime.Api.GameObject, int> statusEffectOrderDictionary { get; set; } = new System.Collections.Generic.Dictionary<global::OpenPCR.Runtime.Api.GameObject, System.Int32>();
		public global::Elements.Battle.Core.eSpecialSleepStatus SpecialSleepStatus { get; set; } = eSpecialSleepStatus.INVALID;
		public bool IsAbnormalEffectEnable { get; private set; } = true;
		public global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalState, global::System.Func<bool>> IsReleaseSlipDamageDic { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalState, System.Func<System.Boolean>>();
		private global::System.Collections.Generic.List<global::Elements.Battle.Core.AweAction.AweData> aweDatas { get; set; } = new System.Collections.Generic.List<Elements.Battle.Core.AweAction.AweData>();
		private global::System.Collections.Generic.Dictionary<global::Elements.eStateIconType, global::System.Collections.Generic.List<global::Elements.Battle.Core.KnightGuardData>> knightGuardDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.eStateIconType, System.Collections.Generic.List<Elements.Battle.Core.KnightGuardData>>();
		private global::System.Collections.Generic.Dictionary<global::Elements.eStateIconType, global::Elements.Battle.Core.GutsData> gutsDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.eStateIconType, Elements.Battle.Core.GutsData>();
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.PassiveSealAction.ePassiveTiming, global::System.Collections.Generic.List<global::Elements.Battle.Core.PassiveSealData>> passiveSealDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.PassiveSealAction.ePassiveTiming, System.Collections.Generic.List<Elements.Battle.Core.PassiveSealData>>();
		private global::System.Collections.Generic.List<global::Elements.Battle.Core.DebuffDamageUpData> debuffDamageUpDataList { get; set; } = new System.Collections.Generic.List<Elements.Battle.Core.DebuffDamageUpData>();
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.EveryAttackCriticalAction.eEveryAtkCriticalType, global::Elements.Battle.Core.EveryAttackCriticalData> everyAttackCriticalData { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.EveryAttackCriticalAction.eEveryAtkCriticalType, Elements.Battle.Core.EveryAttackCriticalData>();
		public global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalState, global::Elements.Battle.Core.AddAbnormalEffectTimeData> AddAbnormalEffectTimeDataDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalState, Elements.Battle.Core.AddAbnormalEffectTimeData>();
		public global::System.Collections.Generic.List<global::Elements.UnitCtrl> TauntTargetList { get; set; } = new System.Collections.Generic.List<Elements.UnitCtrl>();
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.BuffParamKind, int> debuffCounterDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.BuffParamKind, System.Int32>
		{
			{
				BuffParamKind.ATK,
				0
			},
			{
				BuffParamKind.DEF,
				0
			},
			{
				BuffParamKind.MAGIC_STR,
				0
			},
			{
				BuffParamKind.MAGIC_DEF,
				0
			},
			{
				BuffParamKind.DODGE,
				0
			},
			{
				BuffParamKind.PHYSICAL_CRITICAL,
				0
			},
			{
				BuffParamKind.MAGIC_CRITICAL,
				0
			},
			{
				BuffParamKind.ENERGY_RECOVER_RATE,
				0
			},
			{
				BuffParamKind.LIFE_STEAL,
				0
			},
			{
				BuffParamKind.MOVE_SPEED,
				0
			},
			{
				BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE,
				0
			},
			{
				BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE,
				0
			},
			{
				BuffParamKind.ACCURACY,
				0
			},
			{
				BuffParamKind.RECEIVE_CRITICAL_DAMAGE_RATE,
				0
			},
			{
				BuffParamKind.RECEIVE_PHYSICAL_AND_MAGIC_DAMAGE_PERCENT,
				0
			},
			{
				BuffParamKind.RECEIVE_PHYSICAL_DAMAGE_PERCENT,
				0
			},
			{
				BuffParamKind.RECEIVE_MAGIC_DAMAGE_PERCENT,
				0
			},
			{
				BuffParamKind.MAX_HP,
				0
			},
			{
				BuffParamKind.PHYSICAL_DAMAGE_UP_PERCENT,
				0
			},
			{
				BuffParamKind.MAGIC_DAMAGE_UP_PERCENT,
				0
			},
			{
				BuffParamKind.BUFF_ENHANCE,
				0
			}
		};
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.BuffParamKind, int> buffCounterDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.BuffParamKind, System.Int32>
		{
			{
				BuffParamKind.ATK,
				0
			},
			{
				BuffParamKind.DEF,
				0
			},
			{
				BuffParamKind.MAGIC_STR,
				0
			},
			{
				BuffParamKind.MAGIC_DEF,
				0
			},
			{
				BuffParamKind.DODGE,
				0
			},
			{
				BuffParamKind.PHYSICAL_CRITICAL,
				0
			},
			{
				BuffParamKind.MAGIC_CRITICAL,
				0
			},
			{
				BuffParamKind.ENERGY_RECOVER_RATE,
				0
			},
			{
				BuffParamKind.LIFE_STEAL,
				0
			},
			{
				BuffParamKind.MOVE_SPEED,
				0
			},
			{
				BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE,
				0
			},
			{
				BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE,
				0
			},
			{
				BuffParamKind.ACCURACY,
				0
			},
			{
				BuffParamKind.RECEIVE_CRITICAL_DAMAGE_RATE,
				0
			},
			{
				BuffParamKind.RECEIVE_PHYSICAL_AND_MAGIC_DAMAGE_PERCENT,
				0
			},
			{
				BuffParamKind.RECEIVE_PHYSICAL_DAMAGE_PERCENT,
				0
			},
			{
				BuffParamKind.RECEIVE_MAGIC_DAMAGE_PERCENT,
				0
			},
			{
				BuffParamKind.MAX_HP,
				0
			},
			{
				BuffParamKind.MAGIC_DAMAGE_UP_PERCENT,
				0
			},
			{
				BuffParamKind.PHYSICAL_DAMAGE_UP_PERCENT,
				0
			},
			{
				BuffParamKind.BUFF_ENHANCE,
				0
			}
		};
		public global::System.Collections.Generic.List<(global::Elements.SkillEffectCtrl right, global::Elements.SkillEffectCtrl left)> CharaAuraEffectList { get; set; } = new System.Collections.Generic.List<System.ValueTuple<Elements.SkillEffectCtrl, Elements.SkillEffectCtrl>>();
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalStateCategory, int> slipDamageIdDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalStateCategory, System.Int32>
		{
			{
				eAbnormalStateCategory.POISON,
				0
			},
			{
				eAbnormalStateCategory.CURSE,
				0
			},
			{
				eAbnormalStateCategory.BURN,
				0
			},
			{
				eAbnormalStateCategory.NO_EFFECT_SLIP_DAMAGE,
				0
			},
			{
				eAbnormalStateCategory.VENOM,
				0
			},
			{
				eAbnormalStateCategory.HEX,
				0
			},
			{
				eAbnormalStateCategory.COMPENSATION,
				0
			},
			{
				eAbnormalStateCategory.POISON2,
				0
			},
			{
				eAbnormalStateCategory.CURSE2,
				0
			},
			{
				eAbnormalStateCategory.BLACK_FRAME,
				0
			},
			{
				eAbnormalStateCategory.RATIO_SLIP,
				0
			}
		};
		public global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalState, global::System.Action<bool>> DamageByBehaviourDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalState, System.Action<System.Boolean>>();
		private global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eAbnormalState, int> abnormalStateToCurrentId { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eAbnormalState, System.Int32>();
		private global::System.Collections.Generic.Dictionary<int, global::Elements.Battle.Core.OverwriteAbnormalStateData> overwriteSpeedData { get; set; } = new System.Collections.Generic.Dictionary<System.Int32, Elements.Battle.Core.OverwriteAbnormalStateData>();
		public global::System.Collections.Generic.List<global::Elements.SkillEffectCtrl> ModeChangeEndEffectList { get; set; } = new System.Collections.Generic.List<Elements.SkillEffectCtrl>();
		public bool IsCharaAuraEnable { get; set; } = true;
		public global::System.Collections.Generic.Dictionary<global::Elements.UnitCtrl, global::System.Collections.Generic.Dictionary<int, global::Elements.Battle.Core.AttackSealData>> DamageOnceOwnerSealDateDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.UnitCtrl, System.Collections.Generic.Dictionary<System.Int32, Elements.Battle.Core.AttackSealData>>();
		public global::System.Collections.Generic.Dictionary<global::Elements.UnitCtrl, global::System.Collections.Generic.Dictionary<int, global::Elements.Battle.Core.AttackSealData>> DamageOwnerSealDataDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.UnitCtrl, System.Collections.Generic.Dictionary<System.Int32, Elements.Battle.Core.AttackSealData>>();
		public global::System.Collections.Generic.Dictionary<global::Elements.eStateIconType, global::System.Action> ChargeEnergyByReceiveDamageDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.eStateIconType, System.Action>();
		public global::System.Collections.Generic.Dictionary<global::Elements.eStateIconType, global::System.Action> UbTriggerSealActionDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.eStateIconType, System.Action>();
		public global::System.Collections.Generic.List<global::Elements.Battle.Core.ToadData> ToadDatas { get; set; } = new System.Collections.Generic.List<Elements.Battle.Core.ToadData>();
		public global::System.Collections.Generic.List<global::System.Func<bool>> OnSetCrowndControlForLoopTriggerList { get; set; } = new System.Collections.Generic.List<System.Func<System.Boolean>>();
		public global::System.Collections.Generic.List<global::Elements.Battle.Core.TranscendDamageDataForSkillAction> TranscendDamageListForSkillAction { get; set; } = new System.Collections.Generic.List<Elements.Battle.Core.TranscendDamageDataForSkillAction>();
		private global::System.Collections.Generic.Dictionary<int, int> BuffDebuffTypeIndex { get; set; } = new System.Collections.Generic.Dictionary<System.Int32, System.Int32>();
		public global::System.Collections.Generic.List<global::Elements.SkillEffectCtrl> BuffDebuffEffectList { get; set; } = new System.Collections.Generic.List<Elements.SkillEffectCtrl>();
		public global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.BuffParamKind, long> AdditionalBuffDictionary { get; set; } = new System.Collections.Generic.Dictionary<Elements.Battle.Core.BuffParamKind, System.Int64>();

		private List<int> buffDebuffSkilIds = new System.Collections.Generic.List<System.Int32>();
		public List<BuffDebuffCoroutineDataSet> BuffDebuffCoroutineList = new System.Collections.Generic.List<Elements.Battle.Core.UnitComponentAbnormal.BuffDebuffCoroutineDataSet>();
		private global::System.Collections.Generic.List<global::Elements.Battle.Core.ChangeParameterFieldData> buffDebuffFieldDataList = new System.Collections.Generic.List<Elements.Battle.Core.ChangeParameterFieldData>();
		public class BuffParamKind_DictComparer : IEqualityComparer<BuffParamKind>
		{


			public bool Equals(BuffParamKind _x, BuffParamKind _y)
			{
				return _x == _y;
			}

			public int GetHashCode(BuffParamKind _obj)
			{
				return (int)_obj;
			}
		}

		public class BuffDebuffCoroutineDataSet
		{
			public IEnumerator Coroutine { get; set; }
			public bool Enable { get; set; }
			public int EnvironmentId { get; set; }

			[global::OpenPCR.Migration("6bca60de-164b-4b8a-ba49-ae08faaf5189")]
			public BuffDebuffCoroutineDataSet()
			{
			}
		}

		static UnitComponentAbnormal()
		{
			BUFF_DEBUFF_ICON_DIC = new Dictionary<BuffParamKind, BuffDebuffConstData>(new BuffParamKind_DictComparer())
			{
				{
					BuffParamKind.NUM,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.NONE,
						DebuffIcon = eStateIconType.NONE
					}
				},
				{
					BuffParamKind.ATK,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_PHYSICAL_ATK,
						DebuffIcon = eStateIconType.DEBUFF_PHYSICAL_ATK
					}
				},
				{
					BuffParamKind.DEF,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_PHYSICAL_DEF,
						DebuffIcon = eStateIconType.DEBUFF_PHYSICAL_DEF
					}
				},
				{
					BuffParamKind.MAGIC_STR,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_MAGIC_ATK,
						DebuffIcon = eStateIconType.DEBUFF_MAGIC_ATK
					}
				},
				{
					BuffParamKind.MAGIC_DEF,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_MAGIC_DEF,
						DebuffIcon = eStateIconType.DEBUFF_MAGIC_DEF
					}
				},
				{
					BuffParamKind.DODGE,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_DODGE,
						DebuffIcon = eStateIconType.DEBUFF_DODGE
					}
				},
				{
					BuffParamKind.PHYSICAL_CRITICAL,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_CRITICAL,
						DebuffIcon = eStateIconType.DEBUFF_CRITICAL
					}
				},
				{
					BuffParamKind.MAGIC_CRITICAL,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_CRITICAL,
						DebuffIcon = eStateIconType.DEBUFF_CRITICAL
					}
				},
				{
					BuffParamKind.ENERGY_RECOVER_RATE,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_ENERGY_RECOVERY,
						DebuffIcon = eStateIconType.DEBUFF_ENERGY_RECOVERY
					}
				},
				{
					BuffParamKind.LIFE_STEAL,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_LIFE_STEAL,
						DebuffIcon = eStateIconType.DEBUFF_LIFE_STEAL
					}
				},
				{
					BuffParamKind.MOVE_SPEED,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.NONE,
						DebuffIcon = eStateIconType.DEBUFF_MOVE_SPEED
					}
				},
				{
					BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_PHYSICAL_CRITICAL_DAMAGE,
						DebuffIcon = eStateIconType.DEBUFF_PHYSICAL_CRITICAL_DAMAGE
					}
				},
				{
					BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_MAGIC_CRITICAL_DAMAGE,
						DebuffIcon = eStateIconType.DEBUFF_MAGIC_CRITICAL_DAMAGE
					}
				},
				{
					BuffParamKind.ACCURACY,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_ACCURACY,
						DebuffIcon = eStateIconType.DEBUFF_ACCURACY
					}
				},
				{
					BuffParamKind.RECEIVE_CRITICAL_DAMAGE_RATE,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_RECEIVE_CRITICAL_DAMAGE,
						DebuffIcon = eStateIconType.DEBUFF_RECEIVE_CRITICAL_DAMAGE
					}
				},
				{
					BuffParamKind.RECEIVE_PHYSICAL_AND_MAGIC_DAMAGE_PERCENT,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.NONE,
						DebuffIcon = eStateIconType.DEBUFF_RECEIVE_PHYSICAL_AND_MAGIC_DAMAGE_PERCENT
					}
				},
				{
					BuffParamKind.RECEIVE_PHYSICAL_DAMAGE_PERCENT,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.NONE,
						DebuffIcon = eStateIconType.DEBUFF_RECEIVE_PHYSICAL_DAMAGE_PERCENT
					}
				},
				{
					BuffParamKind.RECEIVE_MAGIC_DAMAGE_PERCENT,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.NONE,
						DebuffIcon = eStateIconType.DEBUFF_RECEIVE_MAGIC_DAMAGE_PERCENT
					}
				},
				{
					BuffParamKind.MAX_HP,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.NONE,
						DebuffIcon = eStateIconType.DEBUFF_MAX_HP
					}
				},
				{
					BuffParamKind.PHYSICAL_DAMAGE_UP_PERCENT,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_PHYSICAL_DAMAGE,
						DebuffIcon = eStateIconType.DEBUFF_PHYSICAL_DAMAGE
					}
				},
				{
					BuffParamKind.MAGIC_DAMAGE_UP_PERCENT,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_MAGIC_DAMAGE,
						DebuffIcon = eStateIconType.DEBUFF_MAGIC_DAMAGE
					}
				},
				{
					BuffParamKind.BUFF_ENHANCE,
					new BuffDebuffConstData
					{
						BuffIcon = eStateIconType.BUFF_ENHANCE,
						DebuffIcon = eStateIconType.NONE
					}
				},
			};
			ADDITIONAL_BUFF_ICON_DIC = new Dictionary<BuffParamKind, eStateIconType>
			{
				{
					BuffParamKind.NUM,
					eStateIconType.NONE
				},
				{
					BuffParamKind.DEF,
					eStateIconType.ADDITIONAL_BUFF_PHYSICAL_DEF
				},
				{
					BuffParamKind.MAGIC_DEF,
					eStateIconType.ADDITIONAL_BUFF_MAGIC_DEF
				},
			};
		}

		public static readonly Dictionary<BuffParamKind, BuffDebuffConstData> BUFF_DEBUFF_ICON_DIC;
		public static readonly Dictionary<BuffParamKind, eStateIconType> ADDITIONAL_BUFF_ICON_DIC;
		public const float PREFAB_CREATE_OFFSET_TIME = 1f;
		[global::OpenPCR.Migration("c4199ea2-3044-4df2-88b5-8c192bba9d87")]
		public bool IsSpecialSleepStatus
		{
			[global::OpenPCR.Migration("c21a28f9-8809-4506-8e6f-07c03874a163")]
			get
			{
				return (uint)SpecialSleepStatus < 3u;
			}
		}

		[global::OpenPCR.Migration("d06654c5-d0d0-4ec8-a7f4-957a5bdbf482")]
		public bool IsRevivaling
		{
			[global::OpenPCR.Migration("7265df42-70be-4004-9ecb-897f04329abe")]
			get
			{
				return (long)componentParameter.Hp <= 0 && componentState.OnDeadForRevival != null;
			}
		}
		private global::System.Collections.IEnumerator knightGuardCoroutine { get; set; }
		private global::System.Collections.IEnumerator debuffDamageUpCoroutine { get; set; }

		[global::OpenPCR.Migration("194fbac5-2a2c-48ec-80dc-3528c5e8c922")]
		public int OverwriteSpeedDataCount
		{
			[global::OpenPCR.Migration("c5984e3f-f700-4662-b3b5-432f6d303f9e")]
			get
			{
				return overwriteSpeedData.Count;
			}
		}

		public global::System.Collections.Generic.List<global::System.Collections.Generic.Queue<int>> LifeStealQueueList { get; private set; }
		public int KnockBackEnableCount { [global::OpenPCR.Migration("96286bb7-1129-4eab-8154-7ef979745b6b")] get; set; }
		public global::System.Collections.Generic.Dictionary<int, int> SkillExecCountDictionary { get; set; }
		public bool IsStealth { get; set; }
		public global::System.Action<global::Elements.eStateIconType, bool> OnChangeState { get; set; }
		public global::System.Action<global::Elements.eStateIconType, int> OnChangeStateNum { get; set; }
		public global::System.Action<global::Elements.eStateIconType, bool> OnChangeStatePassiveEnable { get; set; }
		private int buffDebuffIndex { get; set; }
		private int clearedBuffIndex { get; set; }
		public int ClearedBuffFieldIndex { get; private set; }
		public int ClearedDebuffFieldIndex { get; private set; }

		[global::OpenPCR.Migration("a69585bb-b959-42d7-94f2-52bbe3f66a08")]
		public UnitComponentAbnormal(UnitCtrl _unitController, BattleManager _battleManager, IBattleLog _battleLog) : base(_unitController)
		{
			battleManager = _battleManager;
			battleLog = _battleLog;
			LifeStealQueueList = new System.Collections.Generic.List<System.Collections.Generic.Queue<System.Int32>>();
			StrikeBackDictionary = new System.Collections.Generic.Dictionary<Elements.Battle.Core.EnchantStrikeBackAction.eStrikeBackEffectType, Elements.Battle.Core.StrikeBackDataSet>(new EnchantStrikeBackAction.eStrikeBackEffectType_DictComparer());
			AccumulativeDamageDataDictionary = new System.Collections.Generic.Dictionary<Elements.UnitCtrl, Elements.Battle.Core.AccumulativeDamageData>();
			AccumulativeDamageSourceDataDictionary = new System.Collections.Generic.Dictionary<System.Int32, Elements.Battle.Core.AccumulativeDamageSourceData>();
			AccumulativeDamageDataForAllEnenmyDictionary = new System.Collections.Generic.Dictionary<Elements.Battle.Core.AccumulativeDamageSourceData, Elements.Battle.Core.AccumulativeDamageData>();
			DamageSealDataDictionary = new System.Collections.Generic.Dictionary<Elements.UnitCtrl, System.Collections.Generic.Dictionary<System.Int32, Elements.Battle.Core.AttackSealData>>();
			AttackSealDataDictionary = new System.Collections.Generic.Dictionary<System.Int32, Elements.Battle.Core.AttackSealDataForAllEnemy>();
			SealDictionary = new System.Collections.Generic.Dictionary<Elements.eStateIconType, Elements.Battle.Core.SealData>();
			SealCancelDictionary = new System.Collections.Generic.Dictionary<Elements.Battle.Core.SealAction.eCancelTriggerType, System.Collections.Generic.Dictionary<Elements.eStateIconType, Elements.Battle.Core.SealCancelData>>();
			UbAbnormalDataList = new System.Collections.Generic.List<Elements.Battle.Core.UbAbnormalData>();
			SkillExecCountDictionary = new System.Collections.Generic.Dictionary<System.Int32, System.Int32>();
			AbnormalStateForceHitData = new System.Collections.Generic.Dictionary<Elements.Battle.Core.eActionType, System.Collections.Generic.List<System.Int32>>();
			for (eAbnormalStateCategory category = eAbnormalStateCategory.NONE; category < eAbnormalStateCategory.NUM; ++category)
				AbnormalStateCategoryDataDictionary.Add(category, new AbnormalStateCategoryData());
			foreach (eAbnormalState state in Enum.GetValues(typeof(eAbnormalState)))
			{
				if (state != eAbnormalState.NONE)
					abnormalStateFlagDictionary.Add(state, false);
			}

			if (unitController.IsBonusEnemy)
				IsStealth = true;
		}

		[global::OpenPCR.Migration("1484e513-cb9b-44e1-a1f1-661cec73ea39")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAnimation = _container.ComponentAnimation;
			componentColor = _container.ComponentColor;
			componentDamageControl = _container.ComponentDamageControl;
			componentMultiPartsBoss = _container.ComponentMultiPartsBoss;
			componentParameter = _container.ComponentParameter;
			componentParameterClamped = _container.ComponentParameterClamped;
			componentPassiveSkill = _container.ComponentPassiveSkill;
			componentSortOrder = _container.ComponentSortOrder;
			componentSound = _container.ComponentSound;
			componentState = _container.ComponentState;
			componentUnionBurst = _container.ComponentUnionBurst;
		}

		[global::OpenPCR.Migration("1c314093-8ba0-4b5e-9db8-9b0a7d9964cf")]
		public void SetAbnormalState(UnitCtrl _source, eAbnormalState _abnormalState, float _effectTime, ActionParameter _action, Skill _skill, double _value = 0.0, double _value2 = 0.0, bool _reduceEnergy = false, bool _isDamageRelease = false, double _reduceEnergyRate = 1.0, bool _showsIcon = true, bool _isEnvironmentAbnormalState = false, bool _isIgnoreDebuffImmune = false)
		{
			if (battleManager.GameState != eBattleGameState.PLAY)
			{
				return;
			}

			AbnormalStateEffectPrefabData specialEffectData = _action?.CreateAbnormalEffectData();
			if (_action != null && _action.AbnormalStateFieldAction != null)
			{
				_action.AbnormalStateFieldAction.TargetAbnormalState = _abnormalState;
			}

			if (_action != null) _action.EnvironmentTargetAbnormalState = _abnormalState;

			if ((IsNoDamageMotion() || IsAbnormalState(eAbnormalState.NO_ABNORMAL)) && !(_action?.IsEnvironmentAbnormalState ?? false) && !AbnormalStateDefine.ABNORMAL_CONST_DATA[_abnormalState].IsBuff && !_isIgnoreDebuffImmune)
			{
				battleLog.AppendBattleLog(eBattleLogType.MISS, 7, _source: _source, _target: unitController, _value: (int)_value, _currentValue: 0L, _duration: 0, _actionId: _action?.ActionId ?? 0);
				if (IsAbnormalState(eAbnormalState.NO_ABNORMAL))
				{
					unitController.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_BY_NO_DAMAGE_MOTION);
				}
			}
			else
			{
				if (AddAbnormalEffectTimeDataDictionary.TryGetValue(_abnormalState, out var extendTime))
					_effectTime = extendTime.GetResultTime(_effectTime);
				if (tryApplyConditionExSkill(_abnormalState, ref _effectTime))
				{
					return;
				}

				switch (_abnormalState)
				{
					case eAbnormalState.POISON:
					case eAbnormalState.BURN:
					case eAbnormalState.CURSE:
					case eAbnormalState.NO_EFFECT_SLIP_DAMAGE:
					case eAbnormalState.VENOM:
					case eAbnormalState.HEX:
					case eAbnormalState.COMPENSATION:
					case eAbnormalState.POISON_BY_BEHAVIOUR:
					case eAbnormalState.POISON2:
					case eAbnormalState.CURSE2:
					case eAbnormalState.RATIO_SLIP:
					case eAbnormalState.BLACK_FRAME:
						OnSlipDamage.Call();
						break;
					case eAbnormalState.CONVERT:
					case eAbnormalState.CONFUSION:
					case eAbnormalState.CONFUSION2:
					case eAbnormalState.PUPPET:
						OnActiveCharmState.Call();
						break;
					default:
						if (AbnormalStateUtility.IsUnableAbnormalState(_abnormalState)) OnActiveUnableState.Call();
						break;
				}

				battleLog.AppendBattleLog(eBattleLogType.SET_ABNORMAL, _source: _source, _target: unitController, _type: (int)_abnormalState, _value: (int)_value, _currentValue: (int)(_effectTime * (float)battleManager.FrameRate), _duration: 0, _actionId: _action?.ActionId ?? 0);
				eAbnormalStateCategory abnormalStateCategory = AbnormalStateUtility.GetAbnormalStateCategory(_abnormalState);
				AbnormalStateCategoryData abnormalStateCategoryData = AbnormalStateCategoryDataDictionary[abnormalStateCategory];
				if (_abnormalState == eAbnormalState.GUARD_ATK && IsAbnormalState(eAbnormalState.DRAIN_ATK))
				{
					abnormalStateCategoryData.Effects.Add(new AbnormalStateEffectGameObject { RightEffect = CreateAbnormalStateEffect(_abnormalState, _isRight: true, specialEffectData), LeftEffect = CreateAbnormalStateEffect(_abnormalState, _isRight: false, specialEffectData) });
				}
				else if (_abnormalState == eAbnormalState.GUARD_MGC && IsAbnormalState(eAbnormalState.DRAIN_MGC))
				{
					abnormalStateCategoryData.Effects.Add(new AbnormalStateEffectGameObject { RightEffect = CreateAbnormalStateEffect(_abnormalState, _isRight: true, specialEffectData), LeftEffect = CreateAbnormalStateEffect(_abnormalState, _isRight: false, specialEffectData) });
				}
				else if (_abnormalState == eAbnormalState.GUARD_BOTH && IsAbnormalState(eAbnormalState.DRAIN_BOTH))
				{
					abnormalStateCategoryData.Effects.Add(new AbnormalStateEffectGameObject { RightEffect = CreateAbnormalStateEffect(_abnormalState, _isRight: true, specialEffectData), LeftEffect = CreateAbnormalStateEffect(_abnormalState, _isRight: false, specialEffectData) });
				}
				else
				{
					if (unitController.ComponentState.IsDead)
					{
						return;
					}

					if (AbnormalStateUtility.CanOverlapAbnormalState(_abnormalState))
					{
						var data = new AbnormalStateCategoryData
						{
							CurrentAbnormalState = _abnormalState,
							enable = true,
							MainValue = _value,
							SubValue = _value2,
							Time = _effectTime,
							ActionId = _action?.ActionId ?? 0,
							ActionType = _action?.ActionType ?? 0,
							IsDamageRelease = _isDamageRelease,
							ShowsIcon = false,
							Skill = _skill,
							Source = _source,
							ActionDetail1 = _action?.ActionDetail1 ?? 0,
							ActionDetail2 = _action?.ActionDetail2 ?? 0,
							IsEnvironmentAbnormalState = _isEnvironmentAbnormalState,
							SourceFieldIndex = _action?.SourceFieldIndex ?? -1
						};
						int index = prepareForUpdateOverlapAbnormalState(_abnormalState, data, _showsIcon);
						overlapAbnormalStateData[index].UpdateCoroutine = updateOverlapAbnormalState(_abnormalState, index, _effectTime, _showsIcon);
						battleManager.AppendCoroutine(overlapAbnormalStateData[index].UpdateCoroutine, ePauseType.SYSTEM);
						return;
					}

					if (IsAbnormalState(abnormalStateCategory))
					{
						switchAbnormalState(_abnormalState, specialEffectData, _showsIcon);
					}

					abnormalStateCategoryData.Duration = _effectTime;
					abnormalStateCategoryData.Time = _effectTime;
					abnormalStateCategoryData.MainValue = _value;
					abnormalStateCategoryData.SubValue = _value2;
					abnormalStateCategoryData.EnergyReduceRate = _reduceEnergyRate;
					abnormalStateCategoryData.ActionId = _action?.ActionId ?? 0;
					abnormalStateCategoryData.ActionType = _action?.ActionType ?? 0;
					abnormalStateCategoryData.IsEnergyReduceMode = _reduceEnergy;
					abnormalStateCategoryData.Skill = _skill;
					abnormalStateCategoryData.Source = _source;
					abnormalStateCategoryData.IsDamageRelease = _isDamageRelease;
					abnormalStateCategoryData.IsReleasedByDamage = false;
					abnormalStateCategoryData.ShowsIcon = _showsIcon;
					if (_action != null)
					{
						abnormalStateCategoryData.EnergyChargeMultiple = _action.EnergyChargeMultiple;
					}

					abnormalStateCategoryData.ActionDetail1 = _action?.ActionDetail1 ?? 0;
					abnormalStateCategoryData.ActionDetail2 = _action?.ActionDetail2 ?? 0;
					abnormalStateCategoryData.IsEnvironmentAbnormalState = _isEnvironmentAbnormalState;
					abnormalStateCategoryData.SourceFieldIndex = _action?.SourceFieldIndex ?? -1;
					abnormalStateCategoryData.AbsorberValue = battleManager.AbsorberValue;
					if (!IsAbnormalState(abnormalStateCategory))
					{
						abnormalStateCategoryData.CurrentAbnormalState = _abnormalState;
						IEnumerator enumerator = UpdateAbnormalState(_abnormalState, specialEffectData);
						if (enumerator.MoveNext())
						{
							unitController.AppendCoroutine(enumerator, ePauseType.SYSTEM);
						}
					}

					if (abnormalStateCategory == eAbnormalStateCategory.SPEED)
					{
						updateEffectForSpeedAbnormalState(_isEnable: true);
					}
				}
			}
		}

		[global::OpenPCR.Migration("94fc2ec3-a0e3-4b2e-af85-c06544d3f877")]
		public void AddOverwriteAbnormalState(int _counter, OverwriteAbnormalStateData _data)
		{
			overwriteSpeedData.Add(_counter, _data);
			eAbnormalState abnormalState = AbnormalStateUtility.GetAbnormalState(_data.AbnormalState);
			SetAbnormalState(_data.Source, abnormalState, 9999f, _data.ActionParameter, _data.Skill, _data.EffectValue, 0.0, false, false, 1.0, true, false, false);
			if (abnormalState == eAbnormalState.HASTE && !BattleUtil.LessThanOrApproximately(CalcAbnormalStateSpeed(), 1f))
			{
				Dictionary<BasePartsData, long> values = new Dictionary<BasePartsData, long>();
				values.Add(_data.TargetParts, 0L);
				SetBuffParam(BuffParamKind.NUM, BuffParamKind.NUM, values, 0.5f, _data.Skill.SkillId, _data.Source, true, eEffectType.COMMON, true, false, false, 0, -1, true);
			}
		}

		[global::OpenPCR.Migration("468cf889-70aa-4c78-b729-314889d44382")]
		public void RemoveOverwriteAbnormalState(int _counter, int _fieldIndex)
		{
			if (overwriteSpeedData.Keys.Count == 0)
				return;
			OverwriteAbnormalStateData data = overwriteSpeedData[_counter];
			overwriteSpeedData.Remove(_counter);
			DisableAbnormalStateById(AbnormalStateUtility.GetAbnormalState(data.AbnormalState), data.ActionParameter.ActionId, false, _fieldIndex);
		}

		[global::OpenPCR.Migration("6c422d2d-1343-40d9-9876-25b8ec661182")]
		private bool tryApplyConditionExSkill(eAbnormalState _abnormalState, ref float _effectTime)
		{
			switch (_abnormalState)
			{
				case eAbnormalState.PARALYSIS:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.PARALYSIS], ref _effectTime);
				case eAbnormalState.FREEZE:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.FREEZE], ref _effectTime);
				case eAbnormalState.CHAINED:
				case eAbnormalState.CHAINED_ACN:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.CHAINED], ref _effectTime);
				case eAbnormalState.SLEEP:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.SLEEP], ref _effectTime);
				case eAbnormalState.STUN:
				case eAbnormalState.NPC_STUN:
				case eAbnormalState.STUN2:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.STUN], ref _effectTime);
				case eAbnormalState.STONE:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.STONE], ref _effectTime);
				case eAbnormalState.PAUSE_ACTION:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.PAUSE_ACTION], ref _effectTime);
				case eAbnormalState.POISON:
				case eAbnormalState.POISON_BY_BEHAVIOUR:
				case eAbnormalState.POISON2:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.POISON], ref _effectTime);
				case eAbnormalState.BURN:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.BURN], ref _effectTime);
				case eAbnormalState.CURSE:
				case eAbnormalState.CURSE2:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.CURSE], ref _effectTime);
				case eAbnormalState.VENOM:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.VENOM], ref _effectTime);
				case eAbnormalState.HEX:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.HEX], ref _effectTime);
				case eAbnormalState.CONVERT:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.CONVERT], ref _effectTime);
				case eAbnormalState.CONFUSION:
				case eAbnormalState.CONFUSION2:
					return tryConditionExSkill(ExSkillData.ABNORMAL_CONDITION_PAIR[eAbnormalState.CONFUSION], ref _effectTime);
				default:
					return false;
			}
		}

		[global::OpenPCR.Migration("245a0f43-6aa5-4ca7-96a4-e08052887bea")]
		private bool tryConditionExSkill(List<eExSkillCondition> _conditions, ref float _effectTime)
		{
			tryConditionExSkillImpl(_conditions, ExConditionPassiveData.eEffectType.NORMAL, ref _effectTime);
			if (tryConditionExSkillImpl(_conditions, ExConditionPassiveData.eEffectType.CANCEL, ref _effectTime))
				return true;
			tryConditionExSkillImpl(_conditions, ExConditionPassiveData.eEffectType.SHORTEN, ref _effectTime);
			return false;
		}

		[global::OpenPCR.Migration("04f5567a-d6c4-4b89-a8c2-44bd59d9b660")]
		private bool tryConditionExSkillImpl(List<eExSkillCondition> _conditions, ExConditionPassiveData.eEffectType _targetEffectType, ref float _effectTime)
		{
			List<ExSkillData> value = null;
			foreach (eExSkillCondition condition in _conditions)
			{
				if (!componentPassiveSkill.ConditionExSkillDictionary.TryGetValue(condition, out value))
					continue;
				foreach (ExSkillData data in value)
				{
					switch (_targetEffectType)
					{
						case ExConditionPassiveData.eEffectType.NORMAL:
							data.TryExSkill(condition, unitController.UnitActionController, false, ExConditionPassiveData.eEffectType.NORMAL);
							break;
						case ExConditionPassiveData.eEffectType.CANCEL:
						case ExConditionPassiveData.eEffectType.SHORTEN:
							switch (data.TryExSkill(condition, unitController.UnitActionController, false, _targetEffectType))
							{
								case ExSkillData.eTryExResult.CANCEL:
									return true;
								case ExSkillData.eTryExResult.SHORTEN:
									_effectTime *= (float)data.ExSkillConditionData[condition].Value;
									return false;
							}

							break;
					}
				}
			}

			return false;
		}

		[global::OpenPCR.Migration("0c57c035-fb11-4730-a742-430992474e87")]
		private int prepareForUpdateOverlapAbnormalState(eAbnormalState _abnormalState, AbnormalStateCategoryData _data, bool _showsIcon)
		{
			if (_abnormalState == eAbnormalState.SLOW)
			{
				List<int> oldIndexes = new List<int>(overlapAbnormalStateIndexList[eAbnormalState.HASTE]);
				overlapAbnormalStateIndexList[eAbnormalState.HASTE].Clear();
				foreach (int index in oldIndexes)
				{
					overlapAbnormalStateData[index].DisableBySwitch = true;
					overlapAbnormalStateData[index].SwitchingAbnormalState = eAbnormalState.SLOW;
					overlapAbnormalStateData[index].UpdateCoroutine.MoveNext();
				}

				if (_showsIcon)
					++overlapAbnormalStateShowIconCount[eAbnormalState.SLOW];
			}
			else if (_abnormalState == eAbnormalState.HASTE)
			{
				List<int> oldIndexes = new List<int>(overlapAbnormalStateIndexList[eAbnormalState.SLOW]);
				overlapAbnormalStateIndexList[eAbnormalState.SLOW].Clear();
				foreach (int index in oldIndexes)
				{
					overlapAbnormalStateData[index].DisableBySwitch = true;
					overlapAbnormalStateData[index].SwitchingAbnormalState = eAbnormalState.HASTE;
					overlapAbnormalStateData[index].UpdateCoroutine.MoveNext();
				}

				if (_showsIcon)
					++overlapAbnormalStateShowIconCount[eAbnormalState.HASTE];
			}

			int currentBehaviourIndex = overlapAbnormalStateCount++;
			overlapAbnormalStateIndexList[_abnormalState].Add(currentBehaviourIndex);
			overlapAbnormalStateData[currentBehaviourIndex] = _data;
			abnormalStateFlagDictionary[_abnormalState] = true;
			if (AbnormalStateUtility.CanOverlapAbnormalState(_abnormalState))
				updateEffectForSpeedAbnormalState(true);
			return currentBehaviourIndex;
		}

		[global::OpenPCR.Migration("ce27d5d8-6438-418b-82c7-52e16c4647fc")]
		private IEnumerator updateOverlapAbnormalState(eAbnormalState _abnormalState, int _currentBehaviourIndex, float _effectTime, bool _showIcon)
		{
			float time = 0f;
			while (time < _effectTime)
			{
				yield return null;
				if (!overlapAbnormalStateIndexList[_abnormalState].Contains(_currentBehaviourIndex))
					break;
				time += unitController.DeltaTimeForPause;
			}

			overlapAbnormalStateIndexList[_abnormalState].Remove(_currentBehaviourIndex);
			overlapAbnormalStateData[_currentBehaviourIndex].enable = false;
			bool disableBySwitch = overlapAbnormalStateData[_currentBehaviourIndex].DisableBySwitch;
			eAbnormalState switchingAbnormalState = overlapAbnormalStateData[_currentBehaviourIndex].SwitchingAbnormalState;
			overlapAbnormalStateData.Remove(_currentBehaviourIndex);
			if (overlapAbnormalStateShowIconCount.ContainsKey(_abnormalState) && _showIcon)
				--overlapAbnormalStateShowIconCount[_abnormalState];
			abnormalStateFlagDictionary[_abnormalState] = overlapAbnormalStateIndexList[_abnormalState].Count > 0;
			bool remainsEnabled = abnormalStateFlagDictionary[_abnormalState];
			if (disableBySwitch || remainsEnabled)
			{
				if (disableBySwitch)
					battleManager.RestartAbnormalStateField(unitController, switchingAbnormalState, true);
			}
			else
			{
				battleManager.RestartAbnormalStateField(unitController, _abnormalState, false);
			}

			if (AbnormalStateUtility.CanOverlapAbnormalState(_abnormalState))
				updateEffectForSpeedAbnormalState(false);
		}

		[global::OpenPCR.Migration("47fe5eab-74b7-4c29-b60a-316d5b7bfbb0")]
		private void updateEffectForSpeedAbnormalState(bool _isEnable)
		{
			int slowCount = overlapAbnormalStateIndexList[eAbnormalState.SLOW].Count;
			if (slowCount > 0)
			{
				if (overlapAbnormalStateIconDisplayFlag[eAbnormalState.SLOW] && overlapAbnormalStateShowIconCount[eAbnormalState.SLOW] == 0)
				{
					overlapAbnormalStateIconDisplayFlag[eAbnormalState.SLOW] = false;
					EventExtension.Call(OnChangeState, eStateIconType.SLOW, false);
				}

				if (!overlapAbnormalStateIconDisplayFlag[eAbnormalState.SLOW] && overlapAbnormalStateShowIconCount[eAbnormalState.SLOW] > 0)
				{
					overlapAbnormalStateIconDisplayFlag[eAbnormalState.SLOW] = true;
					EventExtension.Call(OnChangeState, eStateIconType.SLOW, slowCount > 0);
				}
			}
			else if (overlapAbnormalStateIconDisplayFlag[eAbnormalState.SLOW])
			{
				overlapAbnormalStateIconDisplayFlag[eAbnormalState.SLOW] = false;
				EventExtension.Call(OnChangeState, eStateIconType.SLOW, slowCount > 0);
			}

			int hasteCount = overlapAbnormalStateIndexList[eAbnormalState.HASTE].Count;
			if (hasteCount > 0)
			{
				if (overlapAbnormalStateIconDisplayFlag[eAbnormalState.HASTE] && overlapAbnormalStateShowIconCount[eAbnormalState.SLOW] == 0)
				{
					overlapAbnormalStateIconDisplayFlag[eAbnormalState.HASTE] = false;
					EventExtension.Call(OnChangeState, eStateIconType.HASTE, false);
				}

				if (!overlapAbnormalStateIconDisplayFlag[eAbnormalState.HASTE] && overlapAbnormalStateShowIconCount[eAbnormalState.HASTE] > 0)
				{
					overlapAbnormalStateIconDisplayFlag[eAbnormalState.HASTE] = true;
					EventExtension.Call(OnChangeState, eStateIconType.HASTE, hasteCount > 0);
				}
			}
			else if (overlapAbnormalStateIconDisplayFlag[eAbnormalState.HASTE])
			{
				overlapAbnormalStateIconDisplayFlag[eAbnormalState.HASTE] = false;
				EventExtension.Call(OnChangeState, eStateIconType.HASTE, hasteCount > 0);
			}

			bool isSlow = IsSlowSpeed();
			if (_isEnable)
			{
				if (isSlow)
				{
					if (componentState.CurrentState == ActionState.IDLE)
						unitController.GetCurrentSpineCtrl().SetTimeScale(0.5f);
					componentColor.SetWeakColor();
					return;
				}

				if (IsHasteSpeed() && componentState.CurrentState == ActionState.IDLE)
					unitController.GetCurrentSpineCtrl().SetTimeScale(2f);
				componentColor.SetNormalColor();
				return;
			}

			if (isSlow)
				componentColor.SetWeakColor();
			else
				componentColor.SetNormalColor();
		}

		[global::OpenPCR.Migration("e97551e6-7208-4b4e-bfca-0939632c746e")]
		private double calcOverlapSpeed()
		{
			double value = 0.0;
			foreach (var entry in overlapAbnormalStateIndexList)
			{
				if (entry.Key != eAbnormalState.HASTE_OVERLAP && entry.Key != eAbnormalState.SLOW_OVERLAP)
					continue;
				for (int index = 0; index < entry.Value.Count; ++index)
					value += overlapAbnormalStateData[entry.Value[index]].MainValue;
			}

			return value;
		}

		[global::OpenPCR.Migration("41627c66-154c-4fd1-bd8d-1f0b8b0960b1")]
		public float CalcAbnormalStateSpeed()
		{
			return (float)Math.Max((double)0.1f, getSpeed() + calcOverlapSpeed());
		}

		[global::OpenPCR.Migration("cb5163a4-6271-4319-826a-89771b52e305")]
		public bool IsHasteSpeed()
		{
			return !BattleUtil.LessThanOrApproximately(this.CalcAbnormalStateSpeed(), 1f);
		}

		[global::OpenPCR.Migration("17950bf0-04bf-45c5-8305-5972527cafd3")]
		public bool IsSlowSpeed()
		{
			float num = this.CalcAbnormalStateSpeed();
			if (BattleUtil.Approximately(num, 1f))
			{
				return false;
			}

			return num < 1f;
		}

		[global::OpenPCR.Migration("f651353c-53f2-4d8d-8442-f527b809d5cc")]
		private void switchAbnormalState(eAbnormalState abnormalState, AbnormalStateEffectPrefabData _specialEffectData, bool _showsIconAfterSwitch)
		{
			AbnormalStateCategoryData data = AbnormalStateCategoryDataDictionary[AbnormalStateUtility.GetAbnormalStateCategory(abnormalState)];
			EnableAbnormalState(data.CurrentAbnormalState, false, false, true, false);
			data.ShowsIcon = _showsIconAfterSwitch;
			EnableAbnormalState(abnormalState, true, false, false, false);
			data.CurrentAbnormalState = abnormalState;
			for (int i = 0; i < data.Effects.Count; ++i)
			{
				if (data.Effects[i].RightEffect != null)
					data.Effects[i].RightEffect.SetTimeToDie(true);
				if (data.Effects[i].LeftEffect != null)
					data.Effects[i].LeftEffect.SetTimeToDie(true);
			}

			data.Effects.Add(new AbnormalStateEffectGameObject { RightEffect = CreateAbnormalStateEffect(abnormalState, true, _specialEffectData), LeftEffect = CreateAbnormalStateEffect(abnormalState, false, _specialEffectData) });
		}

		[global::OpenPCR.Migration("71dec293-0876-413b-b4ca-1733521d899f")]
		private IEnumerator UpdateAbnormalState(eAbnormalState _abnormalState, AbnormalStateEffectPrefabData _specialEffectData)
		{
			eAbnormalStateCategory category = AbnormalStateUtility.GetAbnormalStateCategory(_abnormalState);
			AbnormalStateCategoryData data = AbnormalStateCategoryDataDictionary[category];
			data.Time = data.Duration;
			data.Effects.Add(new AbnormalStateEffectGameObject { RightEffect = CreateAbnormalStateEffect(_abnormalState, true, _specialEffectData), LeftEffect = CreateAbnormalStateEffect(_abnormalState, false, _specialEffectData) });
			EnableAbnormalState(_abnormalState, true, data.IsEnergyReduceMode, false, false);
			while (true)
			{
				if (!IsAbnormalState(category))
					yield break;
				_abnormalState = data.CurrentAbnormalState;
				if (data.IsEnergyReduceMode)
				{
					componentParameter.SetEnergy(componentParameter.Energy - data.EnergyReduceRate * unitController.DeltaTimeForPause, eSetEnergyType.BY_MODE_CHANGE, null);
					if (BattleUtil.Approximately(componentParameter.Energy, 0.0))
						break;
				}
				else
				{
					data.Time -= unitController.DeltaTimeForPause;
					if (data.Time <= 0f)
						break;
				}

				if (componentState.IsDead)
					break;
				yield return null;
			}

			EnableAbnormalState(_abnormalState, false, false, false, false);
		}

		[global::OpenPCR.Migration("be1f4afa-9012-4fe9-9407-8ebfec4caefe")]
		private void DestroyAbnormalEffect(eAbnormalStateCategory abnormalStateCategory)
		{
			for (int i = 0; i < AbnormalStateCategoryDataDictionary[abnormalStateCategory].Effects.Count; ++i)
			{
				AbnormalStateEffectGameObject effects = AbnormalStateCategoryDataDictionary[abnormalStateCategory].Effects[i];
				if (effects.RightEffect != null)
					effects.RightEffect.SetTimeToDie(true);
				if (effects.LeftEffect != null)
					effects.LeftEffect.SetTimeToDie(true);
			}

			AbnormalStateCategoryDataDictionary[abnormalStateCategory].Effects.Clear();
		}

		[global::OpenPCR.Migration("4f7257c5-8da5-4970-b7be-74ea028a0855")]
		public void DisableAbnormalStateById(eAbnormalState _abnormalState, int _actionId, bool _isReleasedByDamage, int _sourceFieldIndex = -1)
		{
			if (AbnormalStateUtility.CanOverlapAbnormalState(_abnormalState))
			{
				List<int> indexes = overlapAbnormalStateIndexList[_abnormalState];
				for (int i = indexes.Count - 1; i >= 0; --i)
				{
					int index = indexes[i];
					AbnormalStateCategoryData data = overlapAbnormalStateData[index];
					if (data.enable && data.ActionId == _actionId && (_sourceFieldIndex == -1 || data.SourceFieldIndex == _sourceFieldIndex))
					{
						overlapAbnormalStateData[index].IsReleasedByDamage = _isReleasedByDamage;
						indexes.RemoveAt(i);
					}
				}
			}
			else
			{
				eAbnormalStateCategory category = AbnormalStateUtility.GetAbnormalStateCategory(_abnormalState);
				if (AbnormalStateCategoryDataDictionary[category].ActionId == _actionId && (_sourceFieldIndex == -1 || AbnormalStateCategoryDataDictionary[category].SourceFieldIndex == _sourceFieldIndex))
				{
					AbnormalStateCategoryDataDictionary[category].IsReleasedByDamage = _isReleasedByDamage;
					EnableAbnormalState(_abnormalState, false, false, false, false);
				}
			}
		}

		[global::OpenPCR.Migration("4f7486fc-bf83-4101-977b-2d21eecaddd4")]
		public void EnableAbnormalState(eAbnormalState _abnormalState, bool _enable, bool _reduceEnergy = false, bool _switch = false, bool _skipRestartField = false)
		{
			eAbnormalStateCategory abnormalStateCategory = AbnormalStateUtility.GetAbnormalStateCategory(_abnormalState);
			AbnormalStateCategoryData abnormalStateCategoryData = AbnormalStateCategoryDataDictionary[abnormalStateCategory];
			if (!_enable)
			{
				DestroyAbnormalEffect(abnormalStateCategory);
				abnormalStateCategoryData.MainValue = 0f;
				abnormalStateCategoryData.Time = 0f;
				abnormalStateCategoryData.Duration = 0f;
				abnormalStateCategoryData.EnergyChargeMultiple = 1f;
				if (abnormalStateCategoryData.Skill != null && abnormalStateCategoryData.Skill.BonusId != 0 && !_switch && !unitController.ComponentState.IsDead)
				{
					battleManager.DeleteBonusIcon(abnormalStateCategoryData.Skill.BonusId);
				}
			}

			abnormalStateCategoryData.enable = _enable;
			abnormalStateFlagDictionary[_abnormalState] = _enable;
			switch (_abnormalState)
			{
				case eAbnormalState.SLOW:
					if (!_enable)
					{
						if (!IsUnableActionState())
						{
							unitController.GetCurrentSpineCtrl().Resume();
						}

						updateEffectForSpeedAbnormalState(_isEnable: false);
					}

					break;
				case eAbnormalState.HASTE:
					if (!_enable)
					{
						if (!IsUnableActionState() && !unitController.Pause)
						{
							unitController.GetCurrentSpineCtrl().Resume();
						}

						updateEffectForSpeedAbnormalState(_isEnable: false);
					}

					break;
				case eAbnormalState.PARALYSIS:
				case eAbnormalState.FREEZE:
				case eAbnormalState.CHAINED_ACN:
				case eAbnormalState.CHAINED:
				case eAbnormalState.STUN:
				case eAbnormalState.DETAIN:
				case eAbnormalState.FAINT:
				case eAbnormalState.NPC_STUN:
				case eAbnormalState.CRYSTALIZE:
				case eAbnormalState.STUN2:
					if (_enable)
					{
						if (unitController.ComponentState.CurrentState != ActionState.DAMAGE)
						{
							unitController.ComponentState.SetState(ActionState.DAMAGE);
						}
					}
					else if (!IsUnableActionState() && !_switch)
					{
						BattleAnimKitController currentSpineCtrl2 = unitController.GetCurrentSpineCtrl();
						if (currentSpineCtrl2.AnimationName == currentSpineCtrl2.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE))
						{
							currentSpineCtrl2.IsPlayAnimeBattle = false;
							currentSpineCtrl2.IsStopState = false;
						}

						unitController.SetMotionResume();
						IsContinueIdleForPauseAction = false;
					}

					break;
				case eAbnormalState.SLEEP:
					{
						BattleAnimKitController currentSpineCtrl3 = unitController.GetCurrentSpineCtrl();
						if (_enable)
						{
							bool isDamageAnimBeforeSleep = currentSpineCtrl3.AnimationName == currentSpineCtrl3.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix) || currentSpineCtrl3.AnimationName == currentSpineCtrl3.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMAGE_MULTI_TARGET, -1, unitController.PartsMotionPrefix);
							if (unitController.ComponentState.CurrentState != ActionState.DAMAGE)
							{
								unitController.ComponentState.SetState(ActionState.DAMAGE, 0, 0, _quiet: true);
							}

							if (!isUnableActionState(eAbnormalState.SLEEP) && currentSpineCtrl3.HasSpecialSleepAnimatilon(unitController.MotionPrefix) && !currentSpineCtrl3.CheckPlaySpecialSleepAnimeExceptRelease(unitController.MotionPrefix))
							{
								SpecialSleepStatus = eSpecialSleepStatus.START;
								battleManager.AppendCoroutine(playSleepAnime(isDamageAnimBeforeSleep, AbnormalStateCategoryDataDictionary[eAbnormalStateCategory.SLEEP].IsDamageRelease), ePauseType.IGNORE_BLACK_OUT);
							}
						}
						else if (!IsUnableActionState() && !_switch)
						{
							IsContinueIdleForPauseAction = false;
							if (AbnormalStateCategoryDataDictionary[eAbnormalStateCategory.SLEEP].IsReleasedByDamage && battleManager.ChargeSkillTurn != eChargeSkillTurn.NONE)
							{
								IsDamageReleaseSpecialSleepAnimForUnionBurst = true;
							}
						}

						break;
					}

				case eAbnormalState.STONE:
					unitController.GetCurrentSpineCtrl().IsColorStone = _enable;
					if (_enable)
					{
						if (unitController.ComponentState.CurrentState != ActionState.DAMAGE)
						{
							unitController.ComponentState.SetState(ActionState.DAMAGE);
						}

						if (IsSpecialSleepStatus)
						{
							IsPlayDamageAnimForAbnormal = true;
						}

						SpecialSleepStatus = eSpecialSleepStatus.INVALID;
					}
					else
					{
						if (!IsUnableActionState() && !_switch && IsContinueIdleForPauseAction)
						{
							unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle = false;
							IsContinueIdleForPauseAction = false;
						}

						shiftAbnormalColor();
					}

					break;
				case eAbnormalState.REGENERATION:
				case eAbnormalState.REGENERATION2:
					if (_enable)
					{
						unitController.AppendCoroutine(UpdateHpRegeneration(_abnormalState), ePauseType.SYSTEM);
					}

					break;
				case eAbnormalState.TP_REGENERATION:
				case eAbnormalState.TP_REGENERATION2:
					if (_enable)
					{
						unitController.AppendCoroutine(UpdateTpRegeneration(_abnormalState), ePauseType.SYSTEM);
					}

					break;
				case eAbnormalState.POISON:
				case eAbnormalState.BURN:
				case eAbnormalState.CURSE:
				case eAbnormalState.NO_EFFECT_SLIP_DAMAGE:
				case eAbnormalState.VENOM:
				case eAbnormalState.HEX:
				case eAbnormalState.COMPENSATION:
				case eAbnormalState.POISON2:
				case eAbnormalState.CURSE2:
				case eAbnormalState.RATIO_SLIP:
				case eAbnormalState.BLACK_FRAME:
					if (_enable)
					{
						unitController.AppendCoroutine(UpdateSlipDamage(_abnormalState, ++slipDamageIdDictionary[abnormalStateCategory]), ePauseType.SYSTEM);
					}

					break;
				case eAbnormalState.WORLD_LIGHTNING:
				case eAbnormalState.POISON_BY_BEHAVIOUR:
					if (_enable)
					{
						DamageByBehaviourDictionary[_abnormalState] = delegate (bool _isForce)
						{
							damageByBehaviour(_abnormalState, _isForce);
						};
					}

					break;
				case eAbnormalState.MIFUYU:
					if (!_enable)
					{
					}

					break;
				case eAbnormalState.CONVERT:
				case eAbnormalState.CONFUSION:
				case eAbnormalState.CONFUSION2:
				case eAbnormalState.PUPPET:
					{
						if (!IsAbnormalState(eAbnormalState.PAUSE_ACTION))
						{
							unitController.SetDirectionAuto();
						}

						ActionState currentState = unitController.ComponentState.CurrentState;
						if ((uint)(currentState - 1) <= 2u)
						{
							unitController.ComponentState.CancelByConvert = true;
							unitController.ComponentState.IdleStartAfterWaitFrame = (float)unitController.CastTimer <= unitController.DeltaTimeForPause;
						}

						if ((long)unitController.ComponentParameter.Hp > 0 && !IsUnableActionState() && unitController.ComponentState.CurrentState != ActionState.DAMAGE)
						{
							unitController.ComponentState.SetState(ActionState.IDLE);
						}

						if (_abnormalState == eAbnormalState.PUPPET && _enable) battleManager.CheckAcnLosingBattleFinish();
						break;
					}

				case eAbnormalState.DECOY:
					{
						UnitCtrl unitCtrl = (unitController.IsOther ? battleManager.DecoyOther : battleManager.DecoyUnit);
						if (_enable)
						{
							if (unitCtrl != null && unitCtrl != unitController)
							{
								unitCtrl.ComponentAbnormal.EnableAbnormalState(eAbnormalState.DECOY, _enable: false);
							}

							if (unitController.IsOther)
							{
								battleManager.DecoyOther = unitController;
							}
							else
							{
								battleManager.DecoyUnit = unitController;
							}
						}
						else if (unitCtrl == unitController)
						{
							if (unitController.IsOther)
							{
								battleManager.DecoyOther = null;
							}
							else
							{
								battleManager.DecoyUnit = null;
							}
						}

						break;
					}

				case eAbnormalState.PAUSE_ACTION:
					{
						BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
						currentSpineCtrl.IsColorPauseAction = _enable;
						if (_enable)
						{
							if (unitController.ComponentState.CurrentState != ActionState.DAMAGE)
							{
								unitController.ComponentState.SetState(ActionState.DAMAGE);
							}

							currentSpineCtrl.IsStopState = true;
							unitController.SetMotionResume();
							unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix);
							unitController.SetMotionPause();
							IsContinueIdleForPauseAction = true;
							SpecialSleepStatus = eSpecialSleepStatus.INVALID;
						}
						else if (!IsUnableActionState() && !_switch)
						{
							IsContinueIdleForPauseAction = false;
							currentSpineCtrl.IsPlayAnimeBattle = false;
							currentSpineCtrl.IsStopState = false;
							currentSpineCtrl.Resume();
							unitController.SetDirectionAuto();
						}
						else
						{
							if (IsAbnormalState(eAbnormalState.SLEEP) && unitController.GetCurrentSpineCtrl().HasSpecialSleepAnimatilon(unitController.MotionPrefix) && currentSpineCtrl.IsPlayAnimeBattle)
							{
								currentSpineCtrl.IsPlayAnimeBattle = false;
							}

							shiftAbnormalColor();
						}

						break;
					}
			}

			eStateIconType jEOCPILJNAD = (abnormalStateCategoryData.ShowsIcon ? AbnormalStateDefine.ABNORMAL_CONST_DATA[_abnormalState].IconType : eStateIconType.NONE);
			OnChangeState.Call(jEOCPILJNAD, _enable);
			if (!_enable && !_switch && !_skipRestartField)
			{
				battleManager.RestartAbnormalStateField(unitController, _abnormalState, false);
			}
		}

		[global::OpenPCR.Migration("a28d29ea-b9fe-439e-8850-03b57705001c")]
		private void shiftAbnormalColor()
		{
			if (this.IsUnableActionState())
			{
				if (this.IsAbnormalState(eAbnormalState.STONE))
				{
					unitController.GetCurrentSpineCtrl().IsColorStone = true;
				}
				else if (this.IsAbnormalState(eAbnormalState.PAUSE_ACTION))
				{
					unitController.GetCurrentSpineCtrl().IsColorPauseAction = true;
				}
			}
		}

		[global::OpenPCR.Migration("4e7c10e9-2ccc-459e-9f96-84b9cc083d91")]
		private IEnumerator playSleepAnime(bool _isDamageAnimBeforeSleep, bool _isDamageRelease)
		{
			BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
			if (!currentSpineCtrl.HasSpecialSleepAnimatilon(unitController.MotionPrefix) || currentSpineCtrl.AnimationName == currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 1, 0))
			{
				yield break;
			}

			if (this.SpecialSleepStatus != eSpecialSleepStatus.START)
			{
				this.SpecialSleepStatus = eSpecialSleepStatus.INVALID;
				yield break;
			}

			if (_isDamageAnimBeforeSleep)
			{
				currentSpineCtrl.PlayAnime(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 1, 0);
				this.SpecialSleepStatus = eSpecialSleepStatus.LOOP;
				yield break;
			}

			currentSpineCtrl.IsStopState = true;
			currentSpineCtrl.PlayAnime(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 0, 0, _playLoop: false);
			currentSpineCtrl.Resume();
			string sleepStartAnimName = currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 0, 0);
			TrackEntry trackEntry = currentSpineCtrl.state.GetCurrent(0);
			this.SpecialSleepStatus = eSpecialSleepStatus.WAIT_START_END;
			BattleAnimKitController currentSpineCtrl2;
			while (true)
			{
				currentSpineCtrl2 = unitController.GetCurrentSpineCtrl();
				if ((long)componentParameter.Hp <= 0)
				{
					currentSpineCtrl2.IsStopState = false;
					yield break;
				}

				if (!currentSpineCtrl2.HasSpecialSleepAnimatilon(unitController.MotionPrefix))
				{
					this.SpecialSleepStatus = eSpecialSleepStatus.INVALID;
					yield break;
				}

				if (this.SpecialSleepStatus == eSpecialSleepStatus.LOOP)
				{
					yield break;
				}

				if (this.SpecialSleepStatus != eSpecialSleepStatus.WAIT_START_END)
				{
					this.SpecialSleepStatus = eSpecialSleepStatus.INVALID;
					if (!this.IsAbnormalState(eAbnormalState.PAUSE_ACTION))
					{
						currentSpineCtrl2.IsStopState = false;
					}

					yield break;
				}

				if (!this.IsAbnormalState(eAbnormalState.SLEEP))
				{
					currentSpineCtrl2.IsStopState = false;
					this.SpecialSleepStatus = eSpecialSleepStatus.INVALID;
					if (_isDamageRelease)
					{
						componentAnimation.PlayAnime(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix, -1, -1, _isLoop: false, null, _quiet: false, 0f, _ignoreBlackout: true);
					}

					yield break;
				}

				if (componentState.CurrentState != ActionState.DAMAGE)
				{
					currentSpineCtrl2.IsStopState = false;
					this.SpecialSleepStatus = eSpecialSleepStatus.INVALID;
					yield break;
				}

				if (currentSpineCtrl2.AnimeName != sleepStartAnimName)
				{
					if (!currentSpineCtrl2.HasSpecialSleepAnimatilon(unitController.MotionPrefix))
					{
						currentSpineCtrl2.IsStopState = false;
						this.SpecialSleepStatus = eSpecialSleepStatus.INVALID;
					}

					bool flag = currentSpineCtrl2.AnimationName == currentSpineCtrl2.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix) || currentSpineCtrl2.AnimationName == currentSpineCtrl2.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMAGE_MULTI_TARGET, -1, unitController.PartsMotionPrefix);
					if (this.battleManager.ChargeSkillTurn == eChargeSkillTurn.NONE)
					{
						if (flag)
						{
							currentSpineCtrl2.PlayAnime(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 1, 0);
							this.SpecialSleepStatus = eSpecialSleepStatus.LOOP;
						}
						else if (currentSpineCtrl2.AnimeName != currentSpineCtrl2.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 1, 0))
						{
							currentSpineCtrl2.IsStopState = false;
						}

						yield break;
					}

					if (flag)
					{
						if (_isDamageRelease)
						{
							currentSpineCtrl2.IsStopState = false;
							this.SpecialSleepStatus = eSpecialSleepStatus.INVALID;
						}
						else
						{
							this.SpecialSleepStatus = eSpecialSleepStatus.LOOP;
						}

						yield break;
					}
				}

				if (trackEntry.IsComplete)
				{
					break;
				}

				yield return null;
			}

			if (!currentSpineCtrl2.HasSpecialSleepAnimatilon(unitController.MotionPrefix))
			{
				currentSpineCtrl2.IsStopState = false;
				this.SpecialSleepStatus = eSpecialSleepStatus.INVALID;
			}
			else
			{
				currentSpineCtrl2.PlayAnime(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 1, 0);
				currentSpineCtrl2.Resume();
				this.SpecialSleepStatus = eSpecialSleepStatus.LOOP;
			}
		}

		[global::OpenPCR.Migration("c78e4592-d30c-4995-b308-848a40796f62")]
		public void ReleaseSleepAnime()
		{
			BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
			if (!currentSpineCtrl.HasSpecialSleepAnimatilon(unitController.MotionPrefix))
			{
				currentSpineCtrl.IsPlayAnimeBattle = false;
				SpecialSleepStatus = eSpecialSleepStatus.INVALID;
				return;
			}

			if (SpecialSleepStatus != eSpecialSleepStatus.LOOP)
			{
				currentSpineCtrl.IsPlayAnimeBattle = false;
				SpecialSleepStatus = eSpecialSleepStatus.INVALID;
				return;
			}

			if (AbnormalStateCategoryDataDictionary[eAbnormalStateCategory.SLEEP].IsReleasedByDamage)
			{
				currentSpineCtrl.IsPlayAnimeBattle = false;
				SpecialSleepStatus = eSpecialSleepStatus.INVALID;
				unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix, -1, -1, _isLoop: false, null, _quiet: false, 0f, _ignoreBlackout: true);
				return;
			}

			unitController.ComponentDamageControl.OnDamageForSpecialSleepRelease = delegate (bool _byAttack)
			{
				if (_byAttack)
				{
					unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle = false;
					unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix, -1, -1, _isLoop: false, null, _quiet: false, 0f, _ignoreBlackout: true);
					unitController.ComponentDamageControl.OnDamageForSpecialSleepRelease = null;
				}
			};
			SpecialSleepStatus = eSpecialSleepStatus.INVALID;
			unitController.StartCoroutine(endSleepReleaseAnim());
		}

		[global::OpenPCR.Migration("ffe96d44-28dd-44cd-952b-f1fcb5507fbc")]
		private IEnumerator endSleepReleaseAnim()
		{
			BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
			currentSpineCtrl.PlayAnime(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 2, 0, _playLoop: false);
			TrackEntry trackEntry = currentSpineCtrl.state.GetCurrent(0);
			while (true)
			{
				BattleAnimKitController currentSpineCtrl2 = unitController.GetCurrentSpineCtrl();
				if (trackEntry.IsComplete || !currentSpineCtrl2.HasSpecialSleepAnimatilon(unitController.MotionPrefix) || currentSpineCtrl2.AnimeName != currentSpineCtrl2.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 2, 0))
				{
					break;
				}

				yield return null;
			}

			componentDamageControl.OnDamageForSpecialSleepRelease = null;
		}

		[global::OpenPCR.Migration("ce7e2e77-c79d-4228-b75e-b0be96055332")]
		private IEnumerator UpdateHpRegeneration(eAbnormalState _regenerationState)
		{
			abnormalStateToCurrentId.TryGetValue(_regenerationState, out int currentId);
			currentId++;
			abnormalStateToCurrentId[_regenerationState] = currentId;
			eAbnormalStateCategory category = AbnormalStateUtility.GetAbnormalStateCategory(_regenerationState);
			float time = 0f;
			while (true)
			{
				yield return null;
				if (!IsAbnormalState(_regenerationState) || abnormalStateToCurrentId[_regenerationState] != currentId) yield break;
				time += unitController.DeltaTimeForPause;
				if (BattleUtil.LessThanOrApproximately(1f, time))
				{
					var data = AbnormalStateCategoryDataDictionary[category];
					long value = BattleUtil.DoubleToLong(data.MainValue);
					componentDamageControl.SetRecovery(value, 0L,
						(int)data.SubValue == 1 ? eInhibitHealType.PHYSICS : eInhibitHealType.MAGIC,
						data.Source, false, null, data.Source.ComponentAbnormal.GetHealDownValue(), true, false, false, true, true, null, true);
					time = 0f;
				}
			}
		}

		[global::OpenPCR.Migration("2af8f7b6-6038-4fee-9bba-2a65b13c404c")]
		public double GetHealDownValue()
		{
			return IsAbnormalState(eAbnormalState.HEAL_DOWN) ? AbnormalStateCategoryDataDictionary[eAbnormalStateCategory.HEAL_DOWN].MainValue : 1.0;
		}

		[global::OpenPCR.Migration("c164f7e7-15fe-454d-aeca-75528de6b949")]
		private IEnumerator UpdateTpRegeneration(eAbnormalState _regenerationState)
		{
			abnormalStateToCurrentId.TryGetValue(_regenerationState, out int currentId);
			currentId++;
			abnormalStateToCurrentId[_regenerationState] = currentId;
			eAbnormalStateCategory category = AbnormalStateUtility.GetAbnormalStateCategory(_regenerationState);
			float time = 0f;
			while (true)
			{
				yield return null;
				if (!IsAbnormalState(_regenerationState) || abnormalStateToCurrentId[_regenerationState] != currentId) yield break;
				time += unitController.DeltaTimeForPause;
				if (BattleUtil.LessThanOrApproximately(1f, time))
				{
					var data = AbnormalStateCategoryDataDictionary[category];
					componentDamageControl.ChargeEnergy(eSetEnergyType.BY_CHANGE_ENERGY, data.MainValue, false, null,
						true, unitController, true, eEffectType.COMMON, true, false, 1.0);
					time = 0f;
				}
			}
		}

		[global::OpenPCR.Migration("f0ac702a-237d-4406-8499-5af50d1b18d8")]
		private IEnumerator UpdateSlipDamage(eAbnormalState _abnormalState, int _slipDamageId)
		{
			eAbnormalStateCategory category = AbnormalStateUtility.GetAbnormalStateCategory(_abnormalState);
			AbnormalStateCategoryData categoryData = AbnormalStateCategoryDataDictionary[category];
			float time = 0f;
			long damage;
			long incrementDamage = 0L;
			if (_abnormalState == eAbnormalState.RATIO_SLIP)
				damage = BattleUtil.DoubleToLong(categoryData.MainValue / 100d * (double)(long)componentParameter.MaxHp);
			else
			{
				damage = BattleUtil.DoubleToLong(categoryData.MainValue);
				incrementDamage = BattleUtil.DoubleToLong(categoryData.MainValue * categoryData.SubValue / 100d);
			}
			IsReleaseSlipDamageDic.TryGetValue(_abnormalState, out var isReleaseSlipDamage);
			float interval = GetSlipDamageExtendValue();
			while (true)
			{
				yield return null;
				if (!IsAbnormalState(category) || slipDamageIdDictionary[category] != _slipDamageId)
				{
					yield break;
				}

				if (isReleaseSlipDamage != null && isReleaseSlipDamage())
				{
					break;
				}

				time += unitController.DeltaTimeForPause;
				if (!BattleUtil.LessThanOrApproximately(interval, time))
				{
					continue;
				}

				long maxLimit = -1L;
				long boostedDamage = applySlipDamageUp(damage, _abnormalState, ref maxLimit);
				if (_abnormalState == eAbnormalState.RATIO_SLIP)
				{
					long limit = BattleUtil.DoubleToLong(categoryData.SubValue);
					maxLimit = maxLimit == -1L ? limit : Math.Min(maxLimit, limit);
				}
				BasePartsData target = componentMultiPartsBoss.GetFirstParts();
				DamageData damageData = new DamageData
				{
					Target = target,
					Damage = boostedDamage,
					TargetIncludeMyTeam = target.Owner.IsOther == categoryData.Source.IsOther || categoryData.IsEnvironmentAbnormalState,
					IgnoreTalentBonusForRatioSlip = _abnormalState == eAbnormalState.RATIO_SLIP,
					OverrideDamageLimit = maxLimit,
					DamageType = DamageData.eDamageType.NONE,
					Source = categoryData.Source,
					DamageSoundType = DamageData.eDamageSoundType.SLIP,
					IsSlipDamage = true,
					ActionType = eActionType.SLIP_DAMAGE,
					ExecAbsorber = delegate (long _damage)
					{
						if (_damage > categoryData.AbsorberValue)
						{
							long result = _damage - categoryData.AbsorberValue;
							battleManager.SubstructEnemyPoint(categoryData.AbsorberValue);
							categoryData.AbsorberValue = 0;
							return result;
						}

						categoryData.AbsorberValue -= _damage;
						battleManager.SubstructEnemyPoint(_damage);
						return 0;
					}
				};
				unitController.ComponentDamageControl.SetDamage(damageData, _skill: categoryData.Skill, _byAttack: false, _actionId: categoryData.ActionId, _onDamageHit: null, _hasEffect: true, _energyAdd: true, _onDefeat: null, _noMotion: false, _damageWeight: 1f, _damageWeightSum: 1f, _upperLimitFunc: null, _energyChargeMultiple: categoryData.EnergyChargeMultiple);
				time = 0f;
				damage += incrementDamage;
			}

			DisableAbnormalStateById(_abnormalState, categoryData.ActionId, _isReleasedByDamage: false);
		}

		[global::OpenPCR.Migration("b0c69332-c09c-4ba9-b059-15d9b3034f77")]
		public float GetSlipDamageExtendValue()
		{
			return ExtendSlipDamageData != null && ExtendSlipDamageData.Enable ? ExtendSlipDamageData.ExtendValue : 1f;
		}

		[global::OpenPCR.Migration("7a157818-71f0-4286-b15e-299a92f987bd")]
		private void damageByBehaviour(eAbnormalState _abnormalState, bool _isForce)
		{
			if (IsAbnormalState(_abnormalState) && !unitController.ComponentState.IsDead && !IsRevivaling && (_isForce || ((unitController.ComponentState.CurrentState == ActionState.ATK || unitController.ComponentState.CurrentState == ActionState.SKILL) && battleManager.ChargeSkillTurn == eChargeSkillTurn.NONE)))
			{
				unitController.AppendCoroutine(damageByBehaviourCoroutine(_abnormalState, AbnormalStateCategoryDataDictionary[AbnormalStateUtility.GetAbnormalStateCategory(_abnormalState)].Clone()), ePauseType.SYSTEM);
			}
		}

		[global::OpenPCR.Migration("be848f7b-9145-49a7-a5c9-7cd9fd2e0377")]
		private IEnumerator damageByBehaviourCoroutine(eAbnormalState _abnormalState, AbnormalStateCategoryData _categoryData)
		{
			if (componentState.IsDead) yield break;
			long damage = 0L;
			bool ignoreLogBarrier = false;
			if (_categoryData.ActionDetail2 == 0) damage = BattleUtil.DoubleToLong(_categoryData.MainValue);
			else if (_categoryData.ActionDetail2 >= 1 && _categoryData.ActionDetail2 <= 3)
			{
				long baseValue = _categoryData.ActionDetail2 == 1 ? (long)componentParameter.MaxHp :
					_categoryData.ActionDetail2 == 2 ? (long)componentParameter.Hp : (long)componentParameter.StartMaxHP;
				damage = Math.Min(BattleUtil.DoubleToLong(_categoryData.MainValue / 100d * (double)baseValue), BattleUtil.DoubleToLong(_categoryData.SubValue));
				ignoreLogBarrier = true;
			}
			BasePartsData target = componentMultiPartsBoss.GetFirstParts();
			UnitCtrl source = _categoryData.Source;
			DamageData damageData = new DamageData
			{
				Target = target,
				Source = source,
				Damage = damage,
				DamageType = DamageData.eDamageType.NONE,
				DamageSoundType = DamageData.eDamageSoundType.SLIP,
				IsSlipDamage = true,
				ActionType = eActionType.DAMAGE_BY_ATTACK,
				TargetIncludeMyTeam = target.Owner.IsOther == source.IsOther || _categoryData.IsEnvironmentAbnormalState,
				ExecAbsorber = _damage =>
				{
					if (_damage > _categoryData.AbsorberValue)
					{
						long remaining = _damage - _categoryData.AbsorberValue;
						battleManager.SubstructEnemyPoint(_categoryData.AbsorberValue);
						_categoryData.AbsorberValue = 0L;
						return remaining;
					}
					_categoryData.AbsorberValue -= _damage;
					battleManager.SubstructEnemyPoint(_damage);
					return 0L;
				},
				IgnoreLogBarrier = ignoreLogBarrier
			};
			componentDamageControl.SetDamage(damageData, false, _categoryData.ActionId, null, true, _categoryData.Skill, true, null, false, 1d, 1d, null, _categoryData.EnergyChargeMultiple, null);
			var action = (DamageByBehaviourAction)_categoryData.Skill.ActionParameters.Find(candidate => candidate.ActionId == _categoryData.ActionId);
			if (Singleton<BattleResourceManager>.Instance.DamageByBehaviourEffectPrefab.TryGetValue(action.DamageEffectType, out var prefab))
			{
				var effect = unitController.GetEffect(prefab);
				effect.ResetParameter(prefab);
				effect.InitializeSort();
				effect.PlaySe(componentSound.SoundUnitId, unitController.IsLeftDir);
				effect.transform.position = unitController.transform.position;
				effect.transform.parent = ExceptUIRoot.Transform;
				effect.SortTarget = unitController;
				effect.ExecAppendCoroutine();
			}
			yield break;
		}

		[global::OpenPCR.Migration("edd9c40b-3b30-41d8-a656-ed9689a5abaa")]
		private SkillEffectCtrl CreateAbnormalStateEffect(eAbnormalState _abnormalState, bool _isRight, AbnormalStateEffectPrefabData _specialEffectData)
		{
			if (!battleManager.CanCreateBattleEffect())
			{
				return null;
			}

			AbnormalStateEffectPrefabData abnormalStateEffectPrefabData = null;
			if (_specialEffectData == null)
			{
				if ((uint)(_abnormalState - 1) <= 5u || (uint)(_abnormalState - 23) <= 1u)
				{
					abnormalStateEffectPrefabData = Singleton<BattleResourceManager>.Instance.GetAbnormalStateEffectPrefabData(eAbnormalState.GUARD_ATK, unitController.UnitId);
				}
				else
				{
					abnormalStateEffectPrefabData = Singleton<BattleResourceManager>.Instance.GetAbnormalStateEffectPrefabData(_abnormalState, unitController.UnitId);
					if (abnormalStateEffectPrefabData == null)
					{
						return null;
					}
				}
			}
			else
			{
				abnormalStateEffectPrefabData = _specialEffectData;
			}

			GameObject gameObject = ((_isRight || abnormalStateEffectPrefabData.LeftEffectPrefab == null) ? abnormalStateEffectPrefabData.RightEffectPrefab : abnormalStateEffectPrefabData.LeftEffectPrefab);
			if (gameObject == null)
			{
				return null;
			}

			GameObject gameObject2 = unitController.GetEffect(gameObject).gameObject;
			gameObject2.transform.localScale = gameObject.transform.localScale;
			gameObject2.transform.position = new Vector3(unitController.BottomTransform.transform.position.x, abnormalStateEffectPrefabData.IsHead ? unitController.LifeGauge.transform.position.y : unitController.BottomTransform.transform.position.y, 0f) + gameObject.transform.position;
			gameObject2.transform.parent = ExceptUIRoot.Transform;
			bool flag = _isRight != !unitController.IsLeftDir;
			SkillEffectCtrl component = gameObject2.GetComponent<SkillEffectCtrl>();
			if (component != null)
			{
				component.ResetParameter(gameObject);
				component.InitializeSort();
				if (!flag)
				{
					component.PlaySe(unitController.ComponentSound.SoundUnitId, unitController.IsLeftDir);
				}

				int num = 0;
				if (statusEffectOrderDictionary.Count > 0)
				{
					num = statusEffectOrderDictionary.OrderByDescending((KeyValuePair<GameObject, int> x) => x.Value).First().Value + 1;
				}

				if (statusEffectOrderDictionary.ContainsKey(gameObject2))
				{
					statusEffectOrderDictionary[gameObject2] = num;
				}
				else
				{
					statusEffectOrderDictionary.Add(gameObject2, num);
				}

				component.SortTarget = unitController;
				component.SetSortOrderStatus(num);
				component.ExecAppendCoroutine(null, _isAbnormal: true);
				unitController.StartCoroutine(component.TrackTargetSort(unitController, false));
				switch (_abnormalState)
				{
					case eAbnormalState.POISON:
					case eAbnormalState.BURN:
					case eAbnormalState.CURSE:
					case eAbnormalState.CONVERT:
					case eAbnormalState.PHYSICS_DARK:
					case eAbnormalState.SILENCE:
					case eAbnormalState.STUN:
					case eAbnormalState.CONFUSION:
					case eAbnormalState.VENOM:
					case eAbnormalState.HEX:
					case eAbnormalState.FAINT:
					case eAbnormalState.COMPENSATION:
					case eAbnormalState.UB_SILENCE:
					case eAbnormalState.MAGIC_DARK:
					case eAbnormalState.NPC_STUN:
					case eAbnormalState.POISON_BY_BEHAVIOUR:
					case eAbnormalState.STUN2:
					case eAbnormalState.POISON2:
					case eAbnormalState.CURSE2:
					case eAbnormalState.CONFUSION2:
					case eAbnormalState.PUPPET:
					case eAbnormalState.BLACK_FRAME:
						unitController.StartCoroutine(component.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero, _followX: true, _followY: true, unitController.StateBone, _trackRotation: false, SkillEffectCtrl.eToadTrackType.HEAD));
						break;
					case eAbnormalState.FREEZE:
					case eAbnormalState.DETAIN:
						unitController.StartCoroutine(component.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero));
						break;
					case eAbnormalState.RATIO_SLIP:
					case eAbnormalState.CRYSTALIZE:
					case eAbnormalState.SPY:
						unitController.StartCoroutine(component.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero, _followX: true, _followY: true, unitController.CenterBone, _trackRotation: false, SkillEffectCtrl.eToadTrackType.CENTER));
						break;
					default:
						unitController.StartCoroutine(component.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero, _followX: true, _followY: true, abnormalStateEffectPrefabData.IsHead ? unitController.StateBone : unitController.CenterBone, _trackRotation: false, abnormalStateEffectPrefabData.IsHead ? SkillEffectCtrl.eToadTrackType.HEAD : SkillEffectCtrl.eToadTrackType.CENTER));
						break;
				}
			}

			if (flag)
			{
				gameObject2.SetActive(value: false);
			}

			return component;
		}

		[global::OpenPCR.Migration("8ae73aa6-989f-4745-aa79-ad9bcbc5b16c")]
		public bool IsAbnormalState(eAbnormalState abnormalState)
		{
			return abnormalStateFlagDictionary[abnormalState];
		}

		[global::OpenPCR.Migration("b2ba5555-d238-43aa-bd7d-9833006799c3")]
		public bool IsNoDamageMotion()
		{
			return IsAbnormalState(eAbnormalState.NO_DAMAGE_MOTION) || IsAbnormalState(eAbnormalState.NO_DAMAGE_MOTION2) || IsUnbeatableForAutoRevival;
		}

		[global::OpenPCR.Migration("2066e5e1-e253-4a4c-8ff6-de5d9c4e5b8f")]
		public bool IsConfusionOrConvert()
		{
			return abnormalStateFlagDictionary[eAbnormalState.CONFUSION] || abnormalStateFlagDictionary[eAbnormalState.CONVERT] || abnormalStateFlagDictionary[eAbnormalState.CONFUSION2] || abnormalStateFlagDictionary[eAbnormalState.PUPPET];
		}

		[global::OpenPCR.Migration("1f8f801d-d91d-42fe-bad5-a3a1b8629234")]
		public bool IsPoisonState()
		{
			return IsAbnormalState(eAbnormalState.POISON) || IsAbnormalState(eAbnormalState.POISON2) || IsAbnormalState(eAbnormalState.POISON_BY_BEHAVIOUR);
		}

		[global::OpenPCR.Migration("01168fd6-dd4a-4d30-8d27-6402e9a63c4c")]
		public bool IsCurseState()
		{
			return IsAbnormalState(eAbnormalState.CURSE) || IsAbnormalState(eAbnormalState.CURSE2);
		}

		[global::OpenPCR.Migration("5e5800a3-b0af-4526-b535-70c4e6d33980")]
		public bool IsSlipDamageState()
		{
			return abnormalStateFlagDictionary[eAbnormalState.BURN] || abnormalStateFlagDictionary[eAbnormalState.POISON] || abnormalStateFlagDictionary[eAbnormalState.VENOM] || abnormalStateFlagDictionary[eAbnormalState.CURSE] || abnormalStateFlagDictionary[eAbnormalState.DETAIN] || abnormalStateFlagDictionary[eAbnormalState.HEX] || abnormalStateFlagDictionary[eAbnormalState.COMPENSATION] || abnormalStateFlagDictionary[eAbnormalState.POISON_BY_BEHAVIOUR] || abnormalStateFlagDictionary[eAbnormalState.POISON2] || abnormalStateFlagDictionary[eAbnormalState.CURSE2] || abnormalStateFlagDictionary[eAbnormalState.BLACK_FRAME] || abnormalStateFlagDictionary[eAbnormalState.RATIO_SLIP];
		}

		[global::OpenPCR.Migration("9bd155b6-fe5d-40f4-8c53-4dce23a1f2d6")]
		public bool IsBarrierState()
		{
			return IsAbnormalState(eAbnormalState.GUARD_ATK) || IsAbnormalState(eAbnormalState.GUARD_MGC) || IsAbnormalState(eAbnormalState.DRAIN_ATK) || IsAbnormalState(eAbnormalState.DRAIN_MGC) || IsAbnormalState(eAbnormalState.GUARD_BOTH) || IsAbnormalState(eAbnormalState.DRAIN_BOTH);
		}

		[global::OpenPCR.Migration("05552c6c-cec7-48b8-8aa5-f79f18d2ba18")]
		public bool IsAbnormalState(eAbnormalStateCategory abnormalStateCategory)
		{
			return AbnormalStateCategoryDataDictionary[abnormalStateCategory].enable;
		}

		[global::OpenPCR.Migration("54d1fd84-7f1b-4d8f-a100-47305e06ddb6")]
		public double GetAbnormalStateMainValue(eAbnormalStateCategory _abnormalStateCategory)
		{
			return AbnormalStateCategoryDataDictionary[_abnormalStateCategory].MainValue;
		}

		[global::OpenPCR.Migration("c12e1647-8157-415e-8270-8e2cbba486c0")]
		public void DecreaseCountBlind()
		{
			AbnormalStateCategoryDataDictionary[eAbnormalStateCategory.COUNT_BLIND].MainValue -= 1.0;
			if (AbnormalStateCategoryDataDictionary[eAbnormalStateCategory.COUNT_BLIND].MainValue == 0.0)
				EnableAbnormalState(eAbnormalState.COUNT_BLIND, false, false, false, false);
		}

		[global::OpenPCR.Migration("b94e5296-3389-4ac2-95ea-852882c9aeb1")]
		public double GetAbnormalStateSubValue(eAbnormalStateCategory abnormalStateCategory)
		{
			return AbnormalStateCategoryDataDictionary[abnormalStateCategory].SubValue;
		}

		[global::OpenPCR.Migration("bc949cbd-d366-4f10-aab7-689addfec894")]
		public bool IsUnableActionState()
		{
			return IsAbnormalState(eAbnormalState.PARALYSIS) || IsAbnormalState(eAbnormalState.FREEZE) || IsAbnormalState(eAbnormalState.SLEEP) || IsAbnormalState(eAbnormalState.CHAINED) || IsAbnormalState(eAbnormalState.CHAINED_ACN) || IsAbnormalState(eAbnormalState.STUN) || IsAbnormalState(eAbnormalState.STUN2) || IsAbnormalState(eAbnormalState.NPC_STUN) || IsAbnormalState(eAbnormalState.STONE) || IsAbnormalState(eAbnormalState.DETAIN) || IsAbnormalState(eAbnormalState.FAINT) || IsAbnormalState(eAbnormalState.PAUSE_ACTION) || IsAbnormalState(eAbnormalState.CRYSTALIZE) || KnockBackEnableCount > 0;
		}

		[global::OpenPCR.Migration("0472f5bf-54de-438f-aa03-6ab156d04fcb")]
		public void CureAllAbnormalState()
		{
			foreach (eAbnormalState state in Enum.GetValues(typeof(eAbnormalState)))
				if (state != eAbnormalState.NONE && IsAbnormalState(state) && !AbnormalStateUtility.CanOverlapAbnormalState(state))
					EnableAbnormalState(state, false);

			overlapAbnormalStateIndexList[eAbnormalState.SLOW_OVERLAP].Clear();
			overlapAbnormalStateIndexList[eAbnormalState.HASTE_OVERLAP].Clear();
			overlapAbnormalStateIndexList[eAbnormalState.HASTE].Clear();
			overlapAbnormalStateIndexList[eAbnormalState.SLOW].Clear();
			DamageByBehaviourDictionary.Clear();
			overwriteSpeedData.Clear();
			foreach (eStateIconType key in ChargeEnergyByReceiveDamageDictionary.Keys)
			{
				SealData sealData = SealDictionary[key];
				sealData.RemoveSeal(sealData.Count, _isPassiveSeal: true);
			}

			UnableStateGuardData?.RemoveSeal(UnableStateGuardData.Count, _isPassiveSeal: true);
			UnableStateGuardDataForExtraEffect?.RemoveSeal(UnableStateGuardDataForExtraEffect.Count, true);
			ChargeEnergyByReceiveDamageDictionary.Clear();
			UbTriggerSealActionDictionary.Clear();
			for (int num = LifeStealQueueList.Count - 1; num >= 0; num--)
			{
				LifeStealQueueList[num].Dequeue();
				if (LifeStealQueueList[num].Count == 0)
				{
					LifeStealQueueList.RemoveAt(num);
					OnChangeState.Call(eStateIconType.BUFF_ADD_LIFE_STEAL, arg2: false);
				}
			}

			foreach (KeyValuePair<EnchantStrikeBackAction.eStrikeBackEffectType, StrikeBackDataSet> item in StrikeBackDictionary)
			{
				item.Value.SetTimeToDie();
				for (int i = 0; i < item.Value.DataList.Count; i++)
				{
					OnChangeState.Call(eStateIconType.STRIKE_BACK, arg2: false);
				}
			}

			StrikeBackDictionary.Clear();
			ClearKnightGuard();
			ClearGuts();
			ClearAwe();
			OnSetBuffForEnchantTrigger = null;
			OnGiveCriticalDamage = null;
			AccumulativeDamageDataDictionary.Clear();
			AccumulativeDamageSourceDataDictionary.Clear();
			AccumulativeDamageDataForAllEnenmyDictionary.Clear();
			DamageSealDataDictionary.Clear();
			DamageOnceOwnerSealDateDictionary.Clear();
			DamageOwnerSealDataDictionary.Clear();
			AttackSealDataDictionary.Clear();
			AddAbnormalEffectTimeDataDictionary.Clear();
			componentUnionBurst.SetIsUbDisabledByChangePattern(false);
			passiveSealDictionary.Clear();
			AbnormalStateForceHitData.Clear();
			foreach (var item in everyAttackCriticalData)
			{
				item.Value.Enable = false;
				item.Value.Timer = 0f;
				OnChangeState.Call(item.Value.IconType, false);
			}
			foreach (KeyValuePair<eExSkillCondition, List<ExSkillData>> item2 in unitController.ComponentPassiveSkill.ConditionExSkillDictionary)
			{
				foreach (ExSkillData item3 in item2.Value)
				{
					item3.DeleteIcon(unitController);
				}
			}
			foreach (var target in TauntTargetList) target.ComponentAbnormal.EnableAbnormalState(eAbnormalState.TAUNT, false);
			TauntTargetList.Clear();
		}

		[global::OpenPCR.Migration("0ef4918d-03d3-4f68-a2d4-b55235bfbab6")]
		public void ClearKnightGuard()
		{
			foreach (KeyValuePair<eStateIconType, List<KnightGuardData>> knightGuardData in this.knightGuardDictionary)
			{
				if (knightGuardData.Value.Count != 0)
				{
					this.OnChangeState.Call(knightGuardData.Key, arg2: false);
					knightGuardData.Value.Clear();
				}
			}
		}

		[global::OpenPCR.Migration("b6d8dbe4-80d1-447a-93c0-e1c7db53c8e4")]
		public void ClearGuts()
		{
			foreach (var key in gutsDictionary.Keys)
				OnChangeState.Call(key, false);
			gutsDictionary.Clear();
		}

		[global::OpenPCR.Migration("50b6dd05-cae5-44f5-a831-1e99648e2efb")]
		public void EnableAbnormalEffect(bool _active)
		{
			IsAbnormalEffectEnable = _active;
			foreach (KeyValuePair<eAbnormalStateCategory, AbnormalStateCategoryData> item in AbnormalStateCategoryDataDictionary)
			{
				if (!IsAbnormalState(item.Key))
				{
					continue;
				}

				for (int i = 0; i < item.Value.Effects.Count; i++)
				{
					AbnormalStateEffectGameObject abnormalStateEffectGameObject = item.Value.Effects[i];
					if (abnormalStateEffectGameObject.LeftEffect != null)
					{
						abnormalStateEffectGameObject.LeftEffect.SetActive(_active && unitController.IsLeftDir);
					}

					if (abnormalStateEffectGameObject.RightEffect != null)
					{
						abnormalStateEffectGameObject.RightEffect.SetActive(_active && !unitController.IsLeftDir);
					}
				}
			}

			for (int j = 0; j < BuffDebuffEffectList.Count; j++)
			{
				BuffDebuffEffectList[j].SetActive(_active);
			}

			if (MaxHpDebufCounter != 0)
			{
				if (MaxHpDebufEffectLeft != null)
				{
					MaxHpDebufEffectLeft.SetActive(_active && unitController.IsLeftDir);
				}

				if (MaxHpDebufEffectRight != null)
				{
					MaxHpDebufEffectRight.SetActive(_active && !unitController.IsLeftDir);
				}
			}

			unitController.LifeGauge.SetActive(_active);
			if (!unitController.ComponentSortOrder.IsFront)
			{
				return;
			}

			Dictionary<EnchantStrikeBackAction.eStrikeBackEffectType, StrikeBackDataSet>.Enumerator enumerator2 = StrikeBackDictionary.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				CircleEffectController circleEffectController = enumerator2.Current.Value.SkillEffect as CircleEffectController;
				if (_active)
				{
					for (int k = 0; k < circleEffectController.Children.Count; k++)
					{
						circleEffectController.Children[k].transform.localPosition += new Vector3(0f, BattleDefine.BATTLE_FIELD_SIZE, 0f);
					}
				}
				else
				{
					for (int l = 0; l < circleEffectController.Children.Count; l++)
					{
						circleEffectController.Children[l].transform.localPosition -= new Vector3(0f, BattleDefine.BATTLE_FIELD_SIZE, 0f);
					}
				}
			}
		}

		[global::OpenPCR.Migration("f8f48e4f-8b42-4c8e-b9e0-064f17544025")]
		public void EnableAuraEffect(bool _active)
		{
			if (componentState.IsDead & _active)
			{
				return;
			}

			for (int i = 0; i < unitController.AuraEffectList.Count; i++)
			{
				SkillEffectCtrl skillEffectCtrl = unitController.AuraEffectList[i];
				if (skillEffectCtrl != null)
				{
					skillEffectCtrl.SetActive(_active);
				}

				if (skillEffectCtrl is SpBattleShieldEffect && !_active)
				{
					(skillEffectCtrl as SpBattleShieldEffect).DisableEndParticle();
				}
			}

			foreach (var charaAuraEffect in this.CharaAuraEffectList)
			{
				if (charaAuraEffect.right.gameObject.activeSelf)
				{
					charaAuraEffect.right.SetActive(!unitController.IsLeftDir && this.IsCharaAuraEnable);
				}

				if (charaAuraEffect.left.gameObject.activeSelf)
				{
					charaAuraEffect.left.SetActive(unitController.IsLeftDir && this.IsCharaAuraEnable);
				}
			}
		}

		[global::OpenPCR.Migration("28d30c66-2a87-4e53-aaf5-8310d98992a6")]
		public void StopAbnormalEffect()
		{
			foreach (KeyValuePair<eAbnormalStateCategory, AbnormalStateCategoryData> item in this.AbnormalStateCategoryDataDictionary)
			{
				if (!this.IsAbnormalState(item.Key))
				{
					continue;
				}

				for (int i = 0; i < item.Value.Effects.Count; i++)
				{
					AbnormalStateEffectGameObject abnormalStateEffectGameObject = item.Value.Effects[i];
					if (abnormalStateEffectGameObject.LeftEffect != null)
					{
						abnormalStateEffectGameObject.LeftEffect.Pause();
					}

					if (abnormalStateEffectGameObject.RightEffect != null)
					{
						abnormalStateEffectGameObject.RightEffect.Pause();
					}
				}
			}
		}

		[global::OpenPCR.Migration("2ff5fb9f-18c6-414e-9f57-081e94f0abb9")]
		public double CalcAweValue(bool _isUnionBurst, bool _isAttack)
		{
			double value = 1.0;
			if (aweDatas.Count == 0)
				return value;
			for (int i = aweDatas.Count - 1; i >= 0; i--)
			{
				AweAction.AweData data = aweDatas[i];
				bool applies = data.AweType == AweAction.eAweType.UB_AND_SKILL ? !_isAttack : data.AweType == AweAction.eAweType.UB_ONLY && !_isAttack && _isUnionBurst;
				if (!applies)
					continue;
				value -= data.Value;
				if (data.CountLimit > 0)
				{
					data.CountLimit--;
					if (data.CountLimit == 0)
					{
						if (data.Effect != null)
							data.Effect.SetTimeToDie(true);
						aweDatas.RemoveAt(i);
					}
				}
			}

			if (aweDatas.Count == 0)
				OnChangeState.Call(eStateIconType.AWE, false);
			return Math.Max(value, 0.0);
		}

		[global::OpenPCR.Migration("320fe2db-8318-4c66-b2e4-dbcb56f3fa26")]
		public void ClearAwe()
		{
			if (this.aweDatas.Count != 0)
			{
				this.OnChangeState.Call(eStateIconType.AWE, arg2: false);
				this.aweDatas.Clear();
			}
		}

		[global::OpenPCR.Migration("7d93f139-3e0f-4ee3-99d8-5775c8f5397f")]
		public void AddKnightGuard(KnightGuardData _knightGuardData)
		{
			if (!knightGuardDictionary.TryGetValue(_knightGuardData.StateIconType, out var values))
			{
				values = new List<KnightGuardData>();
				knightGuardDictionary.Add(_knightGuardData.StateIconType, values);
			}

			if (values.Count == 0)
				OnChangeState.Call(_knightGuardData.StateIconType, true);
			values.Add(_knightGuardData);
			if (knightGuardCoroutine == null)
			{
				knightGuardCoroutine = updateKnightGuard();
				unitController.AppendCoroutine(knightGuardCoroutine, ePauseType.SYSTEM, null);
			}
		}

		[global::OpenPCR.Migration("a77d1d28-8d49-4f40-8bda-1444de9e0055")]
		private IEnumerator updateKnightGuard()
		{
			while (this.knightGuardCoroutine != null)
			{
				foreach (KeyValuePair<eStateIconType, List<KnightGuardData>> knightGuardData2 in this.knightGuardDictionary)
				{
					if (knightGuardData2.Value.Count == 0)
					{
						continue;
					}

					for (int num = knightGuardData2.Value.Count - 1; num >= 0; num--)
					{
						KnightGuardData knightGuardData = knightGuardData2.Value[num];
						knightGuardData.LifeTime -= unitController.DeltaTimeForPause;
						if (knightGuardData.LifeTime < 0f)
						{
							knightGuardData2.Value.RemoveAt(num);
							if (knightGuardData.Effect != null)
							{
								knightGuardData.Effect.SetTimeToDie(value: true);
							}
						}
					}

					if (knightGuardData2.Value.Count == 0)
					{
						this.OnChangeState.Call(knightGuardData2.Key, arg2: false);
					}
				}

				yield return null;
			}
		}

		[global::OpenPCR.Migration("42b1ff58-577d-4c68-a1d0-4f5446c38d3c")]
		public bool ExecKnightGuard()
		{
			bool result = false;
			foreach (var entry in knightGuardDictionary)
			{
				Action recovery = null;
				foreach (var data in entry.Value)
				{
					result = true;
					recovery += () => componentDamageControl.SetRecovery(data.Value, 0L, data.InhibitHealType, unitController, false, null, 1f, true, true, false, false, true, null, false);
					if (data.ExecEffectData != null)
					{
						var prefab = unitController.IsLeftDir ? data.ExecEffectData.PrefabLeft : data.ExecEffectData.Prefab;
						var effect = unitController.GetEffect(prefab);
						effect.transform.parent = ExceptUIRoot.Transform;
						effect.SortTarget = unitController;
						effect.InitializeSort();
						effect.PlaySe(data.Source.ComponentSound.SoundUnitId, data.Source.IsLeftDir);
						effect.SetPossitionAppearanceType(data.ExecEffectData, componentMultiPartsBoss.GetFirstParts(), unitController, data.Skill);
						effect.ExecAppendCoroutine();
					}
				}
				entry.Value.Clear();
				recovery.Call();
				OnChangeState.Call(entry.Key, false);
			}
			return result;
		}

		[global::OpenPCR.Migration("b8820de5-3d47-4587-a57c-5fdcf76df153")]
		public void AddPassiveSeal(PassiveSealAction.ePassiveTiming _timing, PassiveSealData _data)
		{
			if (passiveSealDictionary.ContainsKey(_timing))
			{
				passiveSealDictionary[_timing].Add(_data);
				return;
			}

			passiveSealDictionary.Add(_timing, new List<PassiveSealData> { _data });
		}

		[global::OpenPCR.Migration("dfa6c974-49cc-4fbf-9544-0ae628854554")]
		public void AddDebuffDamageUpData(DebuffDamageUpData _debuffDamageUpData)
		{
			debuffDamageUpDataList.Add(_debuffDamageUpData);
			if (debuffDamageUpCoroutine == null)
			{
				debuffDamageUpCoroutine = updateDebuffDamageUp();
				unitController.AppendCoroutine(debuffDamageUpCoroutine, ePauseType.SYSTEM, null);
			}
		}

		[global::OpenPCR.Migration("31caa860-ae39-461b-a3d3-3bb1e8e92983")]
		private IEnumerator updateDebuffDamageUp()
		{
			while (true)
			{
				for (int num = this.debuffDamageUpDataList.Count - 1; num >= 0; num--)
				{
					DebuffDamageUpData debuffDamageUpData = this.debuffDamageUpDataList[num];
					debuffDamageUpData.DebuffDamageUpTimer -= unitController.DeltaTimeForPause;
					if (debuffDamageUpData.DebuffDamageUpTimer < 0f)
					{
						this.debuffDamageUpDataList.RemoveAt(num);
					}
				}

				yield return null;
			}
		}

		[global::OpenPCR.Migration("22bca91e-ea05-4f9e-bbfd-ae8e890c9ab5")]
		public double GetDebuffDamageUpValue()
		{
			if (debuffDamageUpDataList.Count == 0)
			{
				return 1.0;
			}

			int num = 0;
			if (IsAbnormalState(eAbnormalState.SLOW))
			{
				num++;
			}

			if (overlapAbnormalStateIndexList[eAbnormalState.SLOW_OVERLAP].Count > 0)
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.PARALYSIS))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.FREEZE))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.CHAINED))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.CHAINED_ACN))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.SLEEP))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.STUN))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.STUN2))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.STONE))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.DETAIN))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.FAINT))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.PAUSE_ACTION))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.NPC_STUN))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.NO_EFFECT_SLIP_DAMAGE))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.POISON))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.POISON2))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.RATIO_SLIP))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.BURN))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.BLACK_FRAME))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.CURSE))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.CURSE2))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.VENOM))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.HEX))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.COMPENSATION))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.CONVERT))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.CONFUSION))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.CONFUSION2))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.PUPPET))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.MAGIC_DARK))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.PHYSICS_DARK))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.SILENCE))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.UB_SILENCE))
			{
				num++;
			}

			if (UbAbnormalDataList.Count > 0)
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.COUNT_BLIND))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.INHIBIT_HEAL))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.DISABLE_HEAL))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.FEAR))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.DECREASE_HEAL))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.TAUNT))
			{
				num++;
			}

			if (aweDatas.Count > 0)
			{
				num++;
			}

			if (ToadDatas.Count > 0)
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.HEAL_DOWN))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.POISON_BY_BEHAVIOUR))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.CRYSTALIZE))
			{
				num++;
			}

			if (IsAbnormalState(eAbnormalState.ALL_FEAR))
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.ATK] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.DEF] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.MAGIC_STR] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.MAGIC_DEF] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.DODGE] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.PHYSICAL_CRITICAL] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.MAGIC_CRITICAL] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.ENERGY_RECOVER_RATE] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.LIFE_STEAL] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.MOVE_SPEED] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.ACCURACY] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.RECEIVE_CRITICAL_DAMAGE_RATE] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.RECEIVE_PHYSICAL_AND_MAGIC_DAMAGE_PERCENT] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.RECEIVE_PHYSICAL_DAMAGE_PERCENT] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.RECEIVE_MAGIC_DAMAGE_PERCENT] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.MAX_HP] > 0)
			{
				num++;
			}

			if (debuffCounterDictionary[BuffParamKind.BUFF_ENHANCE] > 0) num++;

			double num2 = 1.0;
			foreach (DebuffDamageUpData debuffDamageUpData in debuffDamageUpDataList)
			{
				double num3 = 0.0;
				switch (debuffDamageUpData.EffectType)
				{
					case DebuffDamageUpData.eEffectType.ADD:
						num3 = Math.Min(1.0 + debuffDamageUpData.DebuffDamageUpValue * (double)num, debuffDamageUpData.DebuffDamageUpLimitValue);
						break;
					case DebuffDamageUpData.eEffectType.SUBTRACT:
						num3 = Math.Max(1.0 - debuffDamageUpData.DebuffDamageUpValue * (double)num, debuffDamageUpData.DebuffDamageUpLimitValue);
						break;
				}

				num2 *= num3;
			}

			return num2;
		}

		[global::OpenPCR.Migration("798a14ff-c296-45d2-b354-0124d71fb951")]
		public bool JudgeEveryAttackCritical(bool _isPhysical, bool _targetSideOwners)
		{
			if (_targetSideOwners)
				return false;
			if (everyAttackCriticalData.ContainsKey(EveryAttackCriticalAction.eEveryAtkCriticalType.BOTH) && everyAttackCriticalData[EveryAttackCriticalAction.eEveryAtkCriticalType.BOTH].Enable)
				return true;
			if (_isPhysical)
				return everyAttackCriticalData.ContainsKey(EveryAttackCriticalAction.eEveryAtkCriticalType.PHYSICAL) && everyAttackCriticalData[EveryAttackCriticalAction.eEveryAtkCriticalType.PHYSICAL].Enable;
			return everyAttackCriticalData.ContainsKey(EveryAttackCriticalAction.eEveryAtkCriticalType.MAGIC) && everyAttackCriticalData[EveryAttackCriticalAction.eEveryAtkCriticalType.MAGIC].Enable;
		}

		[global::OpenPCR.Migration("ccc405b0-f482-4a4a-8e8e-b84fda646e4f")]
		public bool GetIsProtected()
		{
			if (ProtectionTargetData == null || !ProtectionTargetData.Enable)
				return false;
			var protector = ProtectionTargetData.ProtectionParts.Owner.ComponentAbnormal;
			return !protector.IsUnableActionState() && !protector.IsConfusionOrConvert() && protector.ToadDatas.Count < 1;
		}

		[global::OpenPCR.Migration("71128f84-a943-4f2c-bc4a-2685f47bd4ef")]
		public bool IsSpecialSleepEnabled()
		{
			return SpecialSleepStatus != eSpecialSleepStatus.INVALID;
		}

		[global::OpenPCR.Migration("74813727-c199-4da4-81da-d555e0424ad5")]
		public bool HasBuffDebuff(bool _isBuff)
		{
			return (_isBuff ? buffCounter : debuffCounter) == 0;
		}

		[global::OpenPCR.Migration("9291692f-797f-4343-8704-6c476fcafc5a")]
		public bool IsBuffDebuffEnable(BuffParamKind _kind, bool _isBuff)
		{
			return (_isBuff ? buffCounterDictionary : debuffCounterDictionary)[_kind] > 0;
		}

		[global::OpenPCR.Migration("7985e653-2c71-4bcf-8c02-53e2a9a31fdc")]
		public bool HasDebuffDamageUpData()
		{
			return debuffDamageUpDataList.Count > 0;
		}

		[global::OpenPCR.Migration("d5a6b8ff-b853-4ede-905b-0611596b5844")]
		public long GetAdditionalBuffDictionary(BuffParamKind _kind)
		{
			long value = 0;
			return AdditionalBuffDictionary.TryGetValue(_kind, out value) ? value : 0L;
		}

		[global::OpenPCR.Migration("3d7eae53-da50-4482-83d6-0f28db385c76")]
		public void SetBuffParam(BuffParamKind _kind, BuffParamKind _resistMissCheckParamKind, Dictionary<BasePartsData, long> _value, float _time, int _skillId, UnitCtrl _source, bool _despelable, eEffectType _effectType, bool _isBuff, bool _additional, bool _isShowIcon, int _bonusId = 0, int _environmentId = -1, bool _triggerCall = true)
		{
			if (!_isBuff && (IsAbnormalState(eAbnormalState.NO_DEBUF) ||
				_value.All(value => (float)value.Key.GetDebuffResistPercent(_resistMissCheckParamKind) == 100f)))
			{
				componentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_BY_NO_DAMAGE_MOTION);
				return;
			}
			if (_effectType == eEffectType.COMMON) createBuffDebuffEffect(_skillId, _isBuff, _source);
			var data = new BuffDebuffCoroutineDataSet { Enable = true, EnvironmentId = _environmentId };
			var coroutine = UpdateBuffParam(_kind, _value, _time, _skillId, _source, _despelable,
				++buffDebuffIndex, _isBuff, _additional, _isShowIcon, _bonusId, data, _triggerCall);
			data.Coroutine = coroutine;
			BuffDebuffCoroutineList.Add(data);
			if (coroutine.MoveNext()) unitController.AppendCoroutine(coroutine, ePauseType.SYSTEM);
		}

		[global::OpenPCR.Migration("98d7132e-d238-417a-858b-fd91772c11e7")]
		private IEnumerator UpdateBuffParam(BuffParamKind _kind, Dictionary<BasePartsData, long> _value, float _maxTime, int _skillId, UnitCtrl _source, bool _despelable, int _buffDebuffId, bool _isBuff, bool _additional, bool _isShowIcon, int _bonusId, BuffDebuffCoroutineDataSet _buffDebuffCoroutineDataSet, bool _triggerCall)
		{
			if (_kind == BuffParamKind.MAX_HP)
			{
				if (MaxHpDebufCounter == 0)
				{
					if (MaxHpDebufEffectRight == null)
					{
						MaxHpDebufEffectRight = createMaxHpDebufEffect(_left: false);
						MaxHpDebufEffectLeft = createMaxHpDebufEffect(_left: true);
					}

					if (unitController.IsLeftDir)
					{
						MaxHpDebufEffectRight.SetActive(_isActive: false);
					}
					else
					{
						MaxHpDebufEffectLeft.SetActive(_isActive: false);
					}
				}

				MaxHpDebufCounter++;
			}

			EnableBuffParam(_kind, _value, true, _source, _isBuff, _additional, _isShowIcon, _triggerCall);
			float time = 0f;
			bool flag3;
			while (true)
			{
				if (!isStopBuffDebuffEffectTimer)
				{
					time += unitController.DeltaTimeForPause;
				}

				if (time > 1f)
				{
					buffDebuffSkilIds.Remove(_skillId);
				}

				bool flag = (_isBuff ? (_buffDebuffId <= clearedBuffIndex) : (_buffDebuffId <= clearedDebuffIndex));
				BuffDebuffTypeIndex.TryGetValue(getBuffDebuffParamValue(_kind, _isBuff, _additional), out var value);
				bool flag2 = _buffDebuffId <= value;
				flag3 = _despelable && (flag | flag2);
				if ((time >= _maxTime || unitController.ComponentState.IdleOnly || (long)unitController.ComponentParameter.Hp <= 0) | flag3)
				{
					break;
				}

				if (!_buffDebuffCoroutineDataSet.Enable) break;
				yield return null;
			}

			buffDebuffSkilIds.Remove(_skillId);
			if (_bonusId != 0 && (long)unitController.ComponentParameter.Hp > 0)
			{
				battleManager.DeleteBonusIcon(_bonusId);
			}

			if (((time >= _maxTime) | flag3) || unitController.ComponentState.IdleOnly || _kind != BuffParamKind.MAX_HP)
			{
				EnableBuffParam(_kind, _value, false, _source, _isBuff, _additional, _isShowIcon, true);
			}
			else
			{
				OnChangeState.Call(eStateIconType.DEBUFF_MAX_HP, arg2: false);
				unitController.ComponentParameter.MaxHp = unitController.ComponentParameter.MaxHpAfterPassive;
			}

			if (_kind == BuffParamKind.MAX_HP)
			{
				MaxHpDebufCounter--;
				if (MaxHpDebufCounter == 0)
				{
					MaxHpDebufEffectRight.SetActive(_isActive: false);
					MaxHpDebufEffectLeft.SetActive(_isActive: false);
				}
			}
		}

		[global::OpenPCR.Migration("ec6808a4-adb4-40be-b325-e0d8c558ce1d")]
		private int getBuffDebuffParamValue(BuffParamKind _kind, bool _isBuff, bool _additional)
		{
			return unchecked((_additional ? 1000 : 0) + (int)_kind * 10 + (_isBuff ? 0 : 1));
		}

		[global::OpenPCR.Migration("3afddb87-fc0a-4f07-9e15-3c2def065fe3")]
		private SkillEffectCtrl createMaxHpDebufEffect(bool _left)
		{
			SkillEffectCtrl effect = unitController.GetEffect(_left ? Singleton<BattleResourceManager>.Instance.MaxHpDebuffPrefabLeft : Singleton<BattleResourceManager>.Instance.MaxHpDebuffPrefabRight);
			effect.transform.parent = ExceptUIRoot.Transform;
			effect.SortTarget = unitController;
			effect.InitializeSort();
			effect.SetSortOrderStatus(0);
			effect.ExecAppendCoroutine(null, _isAbnormal: true);
			unitController.StartCoroutine(effect.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero, _followX: true, _followY: true, unitController.StateBone, _trackRotation: false, SkillEffectCtrl.eToadTrackType.HEAD));
			unitController.StartCoroutine(effect.TrackTargetSort(unitController));
			return effect;
		}

		[global::OpenPCR.Migration("99c20a33-af93-4c0b-8845-db2061143bc1")]
		public void DespeleBuffDebuff(bool _isBuff, AbnormalStateEffectPrefabData _prefabData)
		{
			(this).createBuffDebuffClearEffect((((@_isBuff ? 1u : 0u) & 1u) != 0), (global::Elements.Battle.Core.AbnormalStateEffectPrefabData)(@_prefabData));
			int nativeValue1 = (this).buffDebuffIndex;
			if (((@_isBuff ? 1u : 0u) & 1u) == 0)
			{
				global::Elements.Battle.BattleManager nativeValue2 = (this).battleManager;
				(this).clearedDebuffIndex = unchecked((int)(unchecked((uint)(nativeValue1))));
				if (global::System.Object.ReferenceEquals(nativeValue2, null))
					throw new global::System.NullReferenceException();
				int nativeValue4 = (nativeValue2).FieldIndex;
				(this).ClearedDebuffFieldIndex = unchecked((int)(unchecked((uint)(nativeValue4))));
				(this).dispelBuffDebuffImmediately();
				return;
			}
			else
			{
				global::Elements.Battle.BattleManager nativeValue6 = (this).battleManager;
				(this).clearedBuffIndex = unchecked((int)(unchecked((uint)(nativeValue1))));
				if (global::System.Object.ReferenceEquals(nativeValue6, null))
					throw new global::System.NullReferenceException();
				int nativeValue8 = (nativeValue6).FieldIndex;
				(this).ClearedBuffFieldIndex = unchecked((int)(unchecked((uint)(nativeValue8))));
				(this).dispelBuffDebuffImmediately();
				return;
			}
		}

		[global::OpenPCR.Migration("0c75eb59-1aed-401c-8ec6-75d08cbfc52f")]
		public void CancelStatusUpBuff()
		{
			this.clearedBuffIndex = this.buffDebuffIndex;
			this.dispelBuffDebuffImmediately();
		}

		[global::OpenPCR.Migration("ce616e18-b7e9-4a99-a3b8-be41242f36da")]
		public void CancelBuffDebuffType(int _buffDebuffCancelType)
		{
			this.BuffDebuffTypeIndex[_buffDebuffCancelType] = this.buffDebuffIndex;
			this.dispelBuffDebuffImmediately();
		}

		[global::OpenPCR.Migration("d64b5134-d9c0-41e3-8936-a707b58cc97e")]
		private void dispelBuffDebuffImmediately()
		{
			isStopBuffDebuffEffectTimer = true;
			for (int num = BuffDebuffCoroutineList.Count - 1; num >= 0; num--)
			{
				if (!BuffDebuffCoroutineList[num].Coroutine.MoveNext())
				{
					BuffDebuffCoroutineList.RemoveAt(num);
				}
			}

			isStopBuffDebuffEffectTimer = false;
			for (int num2 = buffDebuffFieldDataList.Count - 1; num2 >= 0; num2--)
			{
				buffDebuffFieldDataList[num2].DispelImmediately(unitController);
			}
		}

		[global::OpenPCR.Migration("d8739b07-32b6-4251-b05a-4416d04907a9")]
		public void EnableBuffDebuffField(bool _isEnable, ChangeParameterFieldData _data)
		{
			if (_isEnable)
				buffDebuffFieldDataList.Add(_data);
			else
				buffDebuffFieldDataList.Remove(_data);
		}

		[global::OpenPCR.Migration("cd60344d-3e86-46b1-ab38-06a3f58d2f49")]
		public void DispelAbnormalState(eAbnormalState _abnormalState)
		{
			if (IsAbnormalState(_abnormalState))
				EnableAbnormalState(_abnormalState, false, false, false, false);
		}

		[global::OpenPCR.Migration("7c0893c4-f02c-4a1b-8777-cd63ef2e2d04")]
		private void createBuffDebuffClearEffect(bool _isBuff, AbnormalStateEffectPrefabData _prefabData)
		{
			if (_prefabData == null || HasBuffDebuff(_isBuff)) return;
			_ = Singleton<BattleResourceManager>.Instance;
			SkillEffectCtrl effect = unitController.GetEffect(unitController.IsLeftDir ? _prefabData.LeftEffectPrefab : _prefabData.RightEffectPrefab);
			effect.InitializeSort();
			effect.SortTarget = unitController;
			effect.SetSortOrderBack();
			effect.ExecAppendCoroutine(unitController);
			if (unitController.gameObject.activeSelf)
			{
				if (_prefabData.IsHead)
				{
					unitController.StartCoroutine(effect.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero, _followX: true, _followY: true, unitController.StateBone));
				}
				else
				{
					unitController.StartCoroutine(effect.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), unitController.FixedCenterPos));
				}
			}

			effect.PlaySe(unitController.ComponentSound.SoundUnitId, unitController.IsLeftDir);
		}

		[global::OpenPCR.Migration("8a30e12b-0c9b-4d0f-8b21-25117f2cce4c")]
		public void EnableBuffParam(BuffParamKind _kind, Dictionary<BasePartsData, long> _value, bool _enable, UnitCtrl _source, bool _isBuff, bool _additional, bool _showsIcon = true, bool _triggerCall = true)
		{
			if (_value.Values.All(value => value == 0L)) return;
			eStateIconType icon = _showsIcon ? findBuffDebuffIcon(_kind, _isBuff, _additional) : eStateIconType.NONE;
			OnChangeState.Call(icon, _enable);
			if (icon != eStateIconType.NONE)
			{
				if (_enable)
				{
					if (_isBuff)
					{
						IncrementBuffCount(_kind);
						if (_triggerCall)
						{
							ExecPassiveSeal(PassiveSealAction.ePassiveTiming.BUFF);
							OnSetBuffForEnchantTrigger.Call();
						}
					}
					else
					{
						IncrementDebuffCount(_kind);
						if (_triggerCall) ExecPassiveSeal(PassiveSealAction.ePassiveTiming.DEBUFF);
					}
				}
				else if (_isBuff) DecrementBuffCount(_kind);
				else DecrementDebuffCount(_kind);
			}
			foreach (var value in _value)
				value.Key.SetBuffDebuff(_enable, value.Value, _kind, _source, battleLog, _additional);
		}

		[global::OpenPCR.Migration("0852fe10-753b-4a38-8723-97bf5a9ece7c")]
		private eStateIconType findBuffDebuffIcon(BuffParamKind _kind, bool _isBuff, bool _additional)
		{
			if (_additional)
				return ADDITIONAL_BUFF_ICON_DIC[_kind];
			if (_isBuff)
				return BUFF_DEBUFF_ICON_DIC[_kind].BuffIcon;
			return BUFF_DEBUFF_ICON_DIC[_kind].DebuffIcon;
		}

		[global::OpenPCR.Migration("fe6536a7-9c39-4266-9dd6-6fc1b79160fd")]
		public void EnchantSlipDamageUp(SlipDamageUpData _data, eAbnormalState _abnormalState)
		{
			if (slipDamageUpDataDictionary.TryGetValue(_abnormalState, out var values))
				values.Add(_data);
			else
			{
				var added = new List<SlipDamageUpData>
				{
					_data
				};
				slipDamageUpDataDictionary[_abnormalState] = added;
			}
		}

		[global::OpenPCR.Migration("06c84eb7-2a4c-49a2-887e-60240f83d60a")]
		public void EnchantAllSlipDamageUp(SlipDamageUpData _data)
		{
			allSlipDamageUpDataList.Add(_data);
		}

		[global::OpenPCR.Migration("0864bcc9-2c7c-4bb5-9b07-e19838ea5187")]
		private long applySlipDamageUp(long _current, eAbnormalState _abnormalState, ref long _maxLimit)
		{
			double rate = 1.0;
			bool unlimited = false;
			if (slipDamageUpDataDictionary.TryGetValue(_abnormalState, out var values))
			{
				foreach (var data in values)
					if (data.Enable)
					{
						rate += data.Value;
						_maxLimit = Math.Max(_maxLimit, data.LimitDamage);
						if (data.LimitDamage == 0)
							unlimited = true;
					}
			}

			foreach (var data in allSlipDamageUpDataList)
				if (data.Enable)
					rate += data.Value;
			foreach (var data in allSlipDamageUpDataList)
				if (data.Enable)
				{
					_maxLimit = Math.Max(_maxLimit, data.LimitDamage);
					if (data.LimitDamage == 0)
					{
						unlimited = true;
						break;
					}
				}

			if (unlimited)
				_maxLimit = -1;
			return BattleUtil.DoubleToLong(rate * (double)_current);
		}

		[global::OpenPCR.Migration("7d49583c-f8e2-4951-844a-661a92745ad2")]
		public bool IsForceHit(eActionType _actionType, int _id)
		{
			return AbnormalStateForceHitData.ContainsKey(_actionType) && AbnormalStateForceHitData[_actionType].Contains(_id);
		}

		[global::OpenPCR.Migration("0ed53b8e-33d8-42d5-851c-ac6df7e2a38b")]
		public bool IsSpyOrEtherialization(UnitCtrl _source)
		{
			return IsAbnormalState(eAbnormalState.SPY) || JudgeIsEtherialization(_source);
		}

		[global::OpenPCR.Migration("6d6f1af3-3abc-4966-8a4c-48aef92eb81a")]
		public bool JudgeIsEtherialization(UnitCtrl _source)
		{
			if (_source.IsOther == unitController.IsOther || _source.IsBonusEnemy)
				return false;
			if (EtherealizationBothCount > 0)
				return true;
			if (_source.AtkType == 2 && EtherealizationMagicCount > 0)
				return true;
			return _source.AtkType != 2 && EtherealizationPhysicalCount > 0;
		}

		[global::OpenPCR.Migration("c7d40a39-c1ae-44bc-aef4-e73e8b7f091f")]
		public bool JudgeIsEtherializationConsiderDebuffImmune(UnitCtrl _source, ActionParameter _actionParameter)
		{
			return !_actionParameter.IsIgnoreDebuffImmune() && JudgeIsEtherialization(_source);
		}

		[global::OpenPCR.Migration("11624987-20a8-4cf4-815f-f1cddf79b9ef")]
		public bool IsEtherealization()
		{
			return EtherealizationBothCount > 0 || EtherealizationMagicCount > 0 || EtherealizationPhysicalCount > 0;
		}

		[global::OpenPCR.Migration("2abf1c73-eab2-4274-9319-523603a404c9")]
		public void AddEtherealizationCount(EtherealizationAction.eTargetAttackType _targetAttackType)
		{
			switch (_targetAttackType)
			{
				case EtherealizationAction.eTargetAttackType.BOTH:
					++EtherealizationBothCount;
					break;
				case EtherealizationAction.eTargetAttackType.MAGIC:
					++EtherealizationMagicCount;
					break;
				case EtherealizationAction.eTargetAttackType.PHYSICAL:
					++EtherealizationPhysicalCount;
					break;
			}
		}

		private bool isStopBuffDebuffEffectTimer;
		private global::Elements.Battle.BattleManager battleManager { get; set; }
		private global::Elements.Battle.IBattleLog battleLog { get; set; }
		private global::Elements.Battle.Core.UnitComponentAnimation componentAnimation { get; set; }
		private global::Elements.Battle.Core.UnitComponentColor componentColor { get; set; }
		private global::Elements.Battle.Core.UnitComponentDamageControl componentDamageControl { get; set; }
		private global::Elements.Battle.Core.UnitComponentMultiPartsBoss componentMultiPartsBoss { get; set; }
		private global::Elements.Battle.Core.UnitComponentParameter componentParameter { get; set; }
		private global::Elements.Battle.Core.UnitComponentParameterClamped componentParameterClamped { get; set; }
		private global::Elements.Battle.Core.UnitComponentPassiveSkill componentPassiveSkill { get; set; }
		private global::Elements.Battle.Core.UnitComponentSortOrder componentSortOrder { get; set; }
		private global::Elements.Battle.Core.UnitComponentSound componentSound { get; set; }
		private global::Elements.Battle.Core.UnitComponentState componentState { get; set; }
		private global::Elements.Battle.Core.UnitComponentUnionBurst componentUnionBurst { get; set; }
		public global::System.Action OnSetBuffForEnchantTrigger { get; set; }
		private int overlapAbnormalStateCount { get; set; }
		public int SlipDamageUpIconCounter { get; set; }
		public bool IsDamageReleaseSpecialSleepAnimForUnionBurst { get; set; }
		public bool IsPlayDamageAnimForAbnormal { get; set; }
		public global::System.Action OnSlipDamage { get; set; }
		public global::System.Action OnActiveUnableStateForModeChange { get; set; }
		public global::System.Action OnActiveCharmForModeChange { get; set; }
		public int EtherealizationPhysicalCount { get; set; }
		public int EtherealizationMagicCount { get; set; }
		public int EtherealizationBothCount { get; set; }
		public global::Elements.Battle.Core.AutoRevivalData AutoRevivalData { get; set; }
		public bool IsUnbeatableForAutoRevival { get; set; }
		public global::Elements.BasePartsData TauntSource { get; set; }
		public global::Elements.Battle.Core.DamageCutForSpecialNullificationData DamageCutForSpecialNullificationData { get; set; }
		public global::Elements.Battle.Core.ExtendSlipDamageData ExtendSlipDamageData { get; set; }
		private int buffCounter { get; set; }
		private int debuffCounter { get; set; }
		public bool IsContinueIdleForPauseAction { get; private set; }
		public global::Elements.Battle.Core.SealData UnableStateGuardData { get; set; }
		public global::Elements.Battle.Core.SealData UnableStateGuardDataForExtraEffect { get; set; }
		public bool AttackWhenSilence { get; set; }
		public global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.EnchantStrikeBackAction.eStrikeBackEffectType, global::Elements.Battle.Core.StrikeBackDataSet> StrikeBackDictionary { get; private set; }
		public global::System.Collections.Generic.Dictionary<global::Elements.UnitCtrl, global::Elements.Battle.Core.AccumulativeDamageData> AccumulativeDamageDataDictionary { get; private set; }
		public global::System.Collections.Generic.Dictionary<int, global::Elements.Battle.Core.AccumulativeDamageSourceData> AccumulativeDamageSourceDataDictionary { get; private set; }
		public global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.AccumulativeDamageSourceData, global::Elements.Battle.Core.AccumulativeDamageData> AccumulativeDamageDataForAllEnenmyDictionary { get; private set; }
		public global::System.Collections.Generic.Dictionary<global::Elements.eStateIconType, global::Elements.Battle.Core.SealData> SealDictionary { get; private set; }
		public global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.SealAction.eCancelTriggerType, global::System.Collections.Generic.Dictionary<global::Elements.eStateIconType, global::Elements.Battle.Core.SealCancelData>> SealCancelDictionary { get; private set; }
		public global::System.Collections.Generic.List<global::Elements.Battle.Core.UbAbnormalData> UbAbnormalDataList { get; private set; }
		public bool IsTough { get; set; }
		public global::System.Collections.Generic.Dictionary<global::Elements.UnitCtrl, global::System.Collections.Generic.Dictionary<int, global::Elements.Battle.Core.AttackSealData>> DamageSealDataDictionary { get; set; }
		public global::System.Collections.Generic.Dictionary<int, global::Elements.Battle.Core.AttackSealDataForAllEnemy> AttackSealDataDictionary { get; set; }
		public global::Elements.Battle.Core.ProtectionData ProtectionTargetData { get; set; }
		public global::Elements.Battle.Core.ProtectionData ProtectionSourceData { get; set; }
		public global::System.Action OnGiveCriticalDamage { get; set; }
		public global::System.Action OnActiveUnableState { get; set; }
		public global::System.Action OnActiveCharmState { get; set; }
		public global::Elements.Battle.Core.LimitEnergyRecoveryRateData LimitEnergyRecoveryRateData { get; set; }
		public global::System.Collections.Generic.Dictionary<global::Elements.Battle.Core.eActionType, global::System.Collections.Generic.List<int>> AbnormalStateForceHitData { get; set; }
		public global::Elements.Battle.Core.ReduceDamageByHigherDefData ReduceDamageByHigherDefData { get; set; }
		public global::Elements.Battle.Core.ConvertDamageOverTimeData ConvertDamageOverTimeData { get; set; }
		private int clearedDebuffIndex { get; set; }
		public int MaxHpDebufCounter { get; set; }
		public global::Elements.SkillEffectCtrl MaxHpDebufEffectRight { get; set; }
		public global::Elements.SkillEffectCtrl MaxHpDebufEffectLeft { get; set; }

		[global::OpenPCR.Migration("e70f9f29-ac8d-40fc-9e00-9647e9420329")]
		private double getSpeed()
		{
			foreach (var entry in overlapAbnormalStateIndexList)
			{
				if (entry.Value.Count == 0)
					continue;
				if (entry.Key == eAbnormalState.HASTE)
					return entry.Value.Max(index => overlapAbnormalStateData[index].MainValue);
				if (entry.Key == eAbnormalState.SLOW)
					return entry.Value.Min(index => overlapAbnormalStateData[index].MainValue);
			}

			return 1.0;
		}

		[global::OpenPCR.Migration("78a96638-1b19-48dc-bcd1-1e5631097130")]
		public bool IsTauntState()
		{
			return IsAbnormalState(eAbnormalState.TAUNT);
		}

		[global::OpenPCR.Migration("9c0b0d6d-c6bb-443a-a71e-c1bc854d752b")]
		private bool isUnableActionState(eAbnormalState _removeCheckState)
		{
			if (IsAbnormalState(eAbnormalState.PARALYSIS) && _removeCheckState != eAbnormalState.PARALYSIS)
				return true;
			if (IsAbnormalState(eAbnormalState.FREEZE) && _removeCheckState != eAbnormalState.FREEZE)
				return true;
			if (IsAbnormalState(eAbnormalState.SLEEP) && _removeCheckState != eAbnormalState.SLEEP)
				return true;
			if (IsAbnormalState(eAbnormalState.CHAINED) && _removeCheckState != eAbnormalState.CHAINED)
				return true;
			if (IsAbnormalState(eAbnormalState.CHAINED_ACN) && _removeCheckState != eAbnormalState.CHAINED_ACN)
				return true;
			if (IsAbnormalState(eAbnormalState.STUN) && _removeCheckState != eAbnormalState.STUN)
				return true;
			if (IsAbnormalState(eAbnormalState.STUN2) && _removeCheckState != eAbnormalState.STUN2)
				return true;
			if (IsAbnormalState(eAbnormalState.NPC_STUN) && _removeCheckState != eAbnormalState.NPC_STUN)
				return true;
			if (IsAbnormalState(eAbnormalState.STONE) && _removeCheckState != eAbnormalState.STONE)
				return true;
			if (IsAbnormalState(eAbnormalState.DETAIN) && _removeCheckState != eAbnormalState.DETAIN)
				return true;
			if (IsAbnormalState(eAbnormalState.FAINT) && _removeCheckState != eAbnormalState.FAINT)
				return true;
			if (IsAbnormalState(eAbnormalState.PAUSE_ACTION) && _removeCheckState != eAbnormalState.PAUSE_ACTION)
				return true;
			if (IsAbnormalState(eAbnormalState.CRYSTALIZE) && _removeCheckState != eAbnormalState.CRYSTALIZE)
				return true;
			return KnockBackEnableCount > 0;
		}

		[global::OpenPCR.Migration("81eb7710-b7d9-4fe3-b33e-eb78a37d15ef")]
		public bool OnSetCrowndControlForLoopTriggerListCall()
		{
			bool result = false;
			foreach (var callback in OnSetCrowndControlForLoopTriggerList)
				result |= callback.Call();
			return result;
		}

		[global::OpenPCR.Migration("83c57a2f-68fd-4387-8b52-2e229ceb6579")]
		public bool ExecGuts()
		{
			bool executed = false;
			foreach (GutsData data in gutsDictionary.Values)
			{
				if (data.IsEnable) { executed = true; data.Exec(); }
			}
			return executed;
		}

		[global::OpenPCR.Migration("85be605c-a8e3-4768-8259-66776c0e0216")]
		public void ExecPassiveSeal(PassiveSealAction.ePassiveTiming _timing)
		{
			if (!passiveSealDictionary.TryGetValue(_timing, out var values))
				return;
			for (int i = values.Count - 1; i >= 0; i--)
			{
				PassiveSealData data = values[i];
				if (data.LifeTime < 0f)
				{
					values.RemoveAt(i);
					continue;
				}

				UnitCtrl target;
				if (data.SealTarget == PassiveSealAction.eSealTarget.SOURCE)
					target = data.Source;
				else if (data.SealTarget == PassiveSealAction.eSealTarget.TARGET)
					target = data.Target;
				else
					throw new NullReferenceException();
				UnitComponentAbnormal abnormal = target.ComponentAbnormal;
				eStateIconType icon = data.TargetStateIcon;
				if (abnormal.SealDictionary.ContainsKey(icon))
					abnormal.SealDictionary[icon].Max = Math.Max(data.SealNumLimit, abnormal.SealDictionary[icon].Max);
				else
					abnormal.SealDictionary.Add(icon, new SealData { Max = data.SealNumLimit, DisplayCount = data.DisplayCount });
				SealData seal = abnormal.SealDictionary[icon];
				if (data.SealNum > 0)
				{
					if (seal.GetCurrentCount() == 0)
						abnormal.OnChangeState.Call(icon, true);
					seal.AddSeal(data.SealDuration, target, icon, data.SealNum);
				}
				else
					seal.RemoveSeal(unchecked(-data.SealNum), true);
			}
		}

		[global::OpenPCR.Migration("fa05e722-341f-4dd3-b4ba-e3490b8abefd")]
		public void EnchantProtection(ProtectionData _data)
		{
			ProtectionData old = ProtectionTargetData;
			if (old != null && old.Enable)
			{
				if (old.IconType != _data.IconType)
				{
					ProtectionTargetData.DisableIcon();
					_data.EnableIcon();
				}

				_data.UpdateIconCount();
				old.Enable = false;
				old.Timer = 0f;
			}
			else
			{
				_data.EnableIcon();
				_data.UpdateIconCount();
			}

			ProtectionTargetData = _data;
			_data.StartTimer();
		}

		[global::OpenPCR.Migration("e7a942af-0d6d-42e9-b563-2b42c93ce9ac")]
		public void ApplyLifeSteal(Skill _skill)
		{
			for (int i = LifeStealQueueList.Count - 1; i >= 0; i--)
			{
				_skill.LifeSteal += LifeStealQueueList[i].Dequeue();
				if (LifeStealQueueList[i].Count == 0)
				{
					LifeStealQueueList.RemoveAt(i);
					OnChangeState.Call(eStateIconType.BUFF_ADD_LIFE_STEAL, false);
				}
			}
		}

		[global::OpenPCR.Migration("66668ef2-41f5-460c-878a-ef1ab8aaa936")]
		public void IncrementBuffCount(BuffParamKind _kind)
		{
			unchecked
			{
				buffCounterDictionary[_kind]++;
				buffCounter++;
			}
		}

		[global::OpenPCR.Migration("31bb28d3-f3c1-480d-8fd5-ce67ec00ba50")]
		public void DecrementBuffCount(BuffParamKind _kind)
		{
			unchecked
			{
				buffCounterDictionary[_kind]--;
				buffCounter--;
			}
		}

		[global::OpenPCR.Migration("4a1a148f-c74b-416b-a994-c27cb5de6008")]
		public void IncrementDebuffCount(BuffParamKind _kind)
		{
			unchecked
			{
				debuffCounterDictionary[_kind]++;
				debuffCounter++;
			}
		}

		[global::OpenPCR.Migration("46a471d5-5d5c-43bb-b2d2-3f6b70aab399")]
		public void DecrementDebuffCount(BuffParamKind _kind)
		{
			unchecked
			{
				debuffCounterDictionary[_kind]--;
				debuffCounter--;
			}
		}

		[global::OpenPCR.Migration("b17903a4-4c6b-46aa-a8f8-9891cbc47da0")]
		private void createBuffDebuffEffect(int skillId, bool isBuff, UnitCtrl source)
		{
			if (!battleManager.CanCreateBattleEffect()) return;
			int size = Elements.Utility.BattleEffectUtility.GetBuffArrowEffectSize(battleManager.BattleCategory);
			if (size < 1 || buffDebuffSkilIds.Contains(skillId)) return;
			GameObject prefab = isBuff ? Singleton<BattleResourceManager>.Instance.EnbuffEffectPrefab : Singleton<BattleResourceManager>.Instance.EndebuffEffectPrefab;
			var gameObject = unitController.GetEffect(prefab).gameObject;
			var scale = prefab.transform.localScale;
			double rate = (double)size / 100.0;
			gameObject.transform.localScale = new Vector3((float)((double)scale.x * rate), (float)((double)scale.y * rate), (float)((double)scale.z * rate));
			gameObject.transform.parent = ExceptUIRoot.Transform;
			var effect = gameObject.GetComponent<SkillEffectCtrl>();
			battleManager.StartCoroutine(effect.TrackTarget(componentMultiPartsBoss.GetFirstParts(true), Vector3.zero, true, true, unitController.CenterBone));
			effect.InitializeSort();
			effect.SortTarget = unitController;
			if (battleManager.IsFinishStartFadeIn()) effect.PlaySe(componentSound.SoundUnitId, unitController.IsLeftDir);
			effect.ExecAppendCoroutine(unitController);
			BuffDebuffEffectList.Add(effect);
			effect.OnEffectEnd += obj => BuffDebuffEffectList.Remove(obj);
			battleManager.StartCoroutine(effect.TrackTargetSort(unitController));
			buffDebuffSkilIds.Add(skillId);
		}

		[global::OpenPCR.Migration("54277f18-1aa2-43fd-854e-f18ef114a1f8")]
		public void SetBuffDebuffWithMultiplyValue(BuffParamKind _kind, Dictionary<BasePartsData, long> _partsToBaseValue, bool _enable, UnitCtrl _source, int _multiplyValue)
		{
			foreach (var item in _partsToBaseValue)
				item.Key.SetBuffDebuff(_enable, unchecked(item.Value * _multiplyValue), _kind, _source, battleLog, false);
		}

		[global::OpenPCR.Migration("cf0e4cd0-4314-4506-9a3d-8a188ca68035")]
		public void DisableEnvironmentCoroutine(int _environmentId)
		{
			foreach (var data in BuffDebuffCoroutineList)
				if (data.EnvironmentId == _environmentId)
					data.Enable = false;
			if (ExtendSlipDamageData != null && ExtendSlipDamageData.Enable && ExtendSlipDamageData.EnvironmentId == _environmentId)
				ExtendSlipDamageData.Enable = false;
		}

		[global::OpenPCR.Migration("0d402dcd-8247-4e8b-b485-080481ee9bc9")]
		public BasePartsData GetTauntSource()
		{
			return IsTauntState() ? TauntSource : null;
		}
	}
}
