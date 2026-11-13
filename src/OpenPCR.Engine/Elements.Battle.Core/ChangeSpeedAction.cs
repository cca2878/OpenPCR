using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public partial class ChangeSpeedAction : ActionParameter
	{
		public enum eChangeSpeedType
		{
			SLOW = 1,
			HASTE,
			PARALYSIS,
			FREEZE,
			CHAINED,
			SLEEP,
			STUN,
			STONE,
			DETAIN,
			FAINT,
			PAUSE_ACTION,
			NPC_STUN,
			CRYSTALIZE,
			STUN2,
			CHAINED_ACN
		}

		public enum eCancelTriggerType
		{
			NONE,
			DAMAGED,
			MAX_HP
		}

		public class eChangeSpeedType_DictComparer : IEqualityComparer<eChangeSpeedType>
		{


			public bool Equals(eChangeSpeedType _x, eChangeSpeedType _y)
			{
				return _x == _y;
			}

			public int GetHashCode(eChangeSpeedType _obj)
			{
				return (int)_obj;
			}
		}

		public static readonly Dictionary<eChangeSpeedType, eAbnormalState> AbnormalStateDic = new Dictionary<eChangeSpeedType, eAbnormalState>(new eChangeSpeedType_DictComparer())
		{
			{
				eChangeSpeedType.SLOW,
				(eAbnormalState)11
			},
			{
				eChangeSpeedType.HASTE,
				(eAbnormalState)7
			},
			{
				eChangeSpeedType.PARALYSIS,
				(eAbnormalState)12
			},
			{
				eChangeSpeedType.FREEZE,
				(eAbnormalState)13
			},
			{
				eChangeSpeedType.CHAINED,
				(eAbnormalState)17
			},
			{
				eChangeSpeedType.SLEEP,
				(eAbnormalState)18
			},
			{
				eChangeSpeedType.STUN,
				(eAbnormalState)19
			},
			{
				eChangeSpeedType.STONE,
				(eAbnormalState)28
			},
			{
				eChangeSpeedType.DETAIN,
				(eAbnormalState)20
			},
			{
				eChangeSpeedType.FAINT,
				(eAbnormalState)38
			},
			{
				eChangeSpeedType.PAUSE_ACTION,
				(eAbnormalState)47
			},
			{
				eChangeSpeedType.NPC_STUN,
				(eAbnormalState)51
			},
			{
				eChangeSpeedType.CRYSTALIZE,
				(eAbnormalState)54
			},
			{
				eChangeSpeedType.STUN2,
				(eAbnormalState)58
			},
			{
				eChangeSpeedType.CHAINED_ACN,
				(eAbnormalState)77
			}
		};
		[global::OpenPCR.Migration("8b406509-7e44-4b36-9eaf-94c1f32586f7")]
		public ChangeSpeedAction() : base()
		{
		}

		[global::OpenPCR.Migration("c03e3f6d-b6c0-41a8-bebf-91934fe16c37")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("75c9babc-8ab8-4bfb-88e6-fa16fa28f17c")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			if (AbnormalStateFieldAction != null)
				AbnormalStateFieldAction.IsTargetAbnormalStateBuff = AbnormalStateDefine.ABNORMAL_CONST_DATA[AbnormalStateDic[(eChangeSpeedType)ActionDetail1]].IsBuff;
		}

		[global::OpenPCR.Migration("d0884a66-9558-48c8-b899-a2c3fbab0b2d")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if ((ActionDetail1 >= 3 && ActionDetail1 <= 13) || ActionDetail1 == 15)
			{
				if (_target.Owner.ComponentState.ModeChangeUnableStateBarrier || _target.Owner.ComponentState.TriggerStealthUnableStateBarrier)
					return;
			}
			else if (ActionDetail1 == 1 || ActionDetail1 == 2)
			{
				if (_target.Owner.ComponentAbnormal.OverwriteSpeedDataCount > 0 && !_target.Owner.ComponentAbnormal.IsAbnormalState(AbnormalStateDic[(eChangeSpeedType)ActionDetail1]))
					return;
			}

			float roll = BattleManager.Random(0f, 1f);
			bool blocked = false;
			if (ActionDetail2 == (int)eCancelTriggerType.MAX_HP && BattleUtil.LessThanOrApproximately(_valueDictionary[eValueNumber.VALUE_6] / 100.0, (double)_target.Owner.ComponentParameter.Hp / (double)_target.Owner.ComponentParameter.MaxHp))
				blocked = true;
			eAbnormalState abnormalState = AbnormalStateDic[(eChangeSpeedType)ActionDetail1];
			var owner = _target.Owner;
			var abnormal = owner.ComponentAbnormal;
			if (_source.IsOther != owner.IsOther && AbnormalStateUtility.IsUnableAbnormalState(abnormalState) && abnormal.OnSetCrowndControlForLoopTriggerListCall())
				return;
			var guard = abnormal.UnableStateGuardDataForExtraEffect;
			if (guard == null || guard.GetCurrentCount() == 0)
				guard = abnormal.UnableStateGuardData;
			if (_source.IsOther != owner.IsOther && AbnormalStateUtility.IsUnableAbnormalState(abnormalState) && guard != null && guard.GetCurrentCount() >= 1 && !abnormal.IsNoDamageMotion() && !abnormal.IsAbnormalState(eAbnormalState.NO_ABNORMAL))
			{
				blocked = true;
				if (guard.DisplayCount)
					guard.RemoveSeal(1, true);
			}

			bool hit = roll < BattleUtil.CalcDodgeByLevelDiffUseParam(_skill.Level, _source, _target, IsExEquipSkillAction, 1f) || TargetAssignment == eTargetAssignment.OWNER_SITE;
			bool force = abnormal.IsForceHit(eActionType.CHANGE_SPEED, ActionDetail1);
			if (!blocked && (hit || force))
			{
				AppendIsAlreadyExeced(owner, _num);
				double value = _valueDictionary[eValueNumber.VALUE_1];
				if (abnormalState == eAbnormalState.HASTE && !BattleUtil.LessThanOrApproximately(value + abnormal.CalcAbnormalStateSpeed(), 1.0))
					abnormal.SetBuffParam(BuffParamKind.NUM, BuffParamKind.NUM, new Dictionary<BasePartsData, long> { { _target, 0L } }, 0.5f, _skill.SkillId, _source, true, eEffectType.COMMON, true, false, false, 0, -1, true);
				float extend = abnormalState == eAbnormalState.CRYSTALIZE ? abnormal.GetSlipDamageExtendValue() : 1f;
				float duration = AbnormalStateFieldAction != null ? 90f : extend * (float)_valueDictionary[eValueNumber.VALUE_3];
				abnormal.SetAbnormalState(_source, abnormalState, duration, this, _skill, value, 0.0, false, ActionDetail2 == (int)eCancelTriggerType.DAMAGED, 1.0, BattleUtil.Approximately(_valueDictionary[eValueNumber.VALUE_5], 0.0), false, false);
				if (ActionDetail2 == (int)eCancelTriggerType.DAMAGED)
				{
					if (!owner.ComponentDamageControl.OnDamageListForChangeSpeedDisableByAttack.ContainsKey(ActionId))
						owner.ComponentDamageControl.OnDamageListForChangeSpeedDisableByAttack.Add(ActionId, byAttack =>
						{
							if (byAttack && _target.Owner.ComponentAbnormal.IsAbnormalState(abnormalState))
								_target.Owner.ComponentAbnormal.DisableAbnormalStateById(abnormalState, ActionId, true);
						});
				}
				else if (ActionDetail2 == (int)eCancelTriggerType.MAX_HP)
				{
					double threshold = _valueDictionary[eValueNumber.VALUE_6] / 100.0;
					if (!owner.ComponentDamageControl.OnRecoverListForChangeSpeedDisableByMaxHp.ContainsKey(ActionId))
						owner.ComponentDamageControl.OnRecoverListForChangeSpeedDisableByMaxHp.Add(ActionId, hpAmount =>
						{
							if (BattleUtil.LessThanOrApproximately(threshold, hpAmount) && _target.Owner.ComponentAbnormal.IsAbnormalState(abnormalState))
								_target.Owner.ComponentAbnormal.DisableAbnormalStateById(abnormalState, ActionId, false);
						});
				}

				return;
			}

			ActionExecedData actionExecedData = AlreadyExecedData[_target.Owner][_num];
			if (actionExecedData.ExecedPartsNumber == actionExecedData.TargetPartsNumber)
			{
				if (actionExecedData.TargetPartsNumber == 1)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_CHANGE_SPEED, eDamageEffectType.NORMAL, _target);
				else
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_CHANGE_SPEED);
			}
		}

		[global::OpenPCR.Migration("85bab031-db25-4374-865a-7fe89bda9f47")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
