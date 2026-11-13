using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChangeSpeedOverlapAction : ActionParameter
	{
		public static readonly Dictionary<ChangeSpeedAction.eChangeSpeedType, eAbnormalState> AbnormalStateDic = new Dictionary<ChangeSpeedAction.eChangeSpeedType, eAbnormalState>
		{
			{
				ChangeSpeedAction.eChangeSpeedType.SLOW,
				(eAbnormalState)65
			},
			{
				ChangeSpeedAction.eChangeSpeedType.HASTE,
				(eAbnormalState)66
			}
		};
		[global::OpenPCR.Migration("555b3a29-a16b-4398-bf7c-ac24ce877b4c")]
		public ChangeSpeedOverlapAction() : base()
		{
		}

		[global::OpenPCR.Migration("6acb3a25-327a-4291-95c5-2cb6493d0c14")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (BattleManager.Random(0f, 1f) < BattleUtil.GetDodgeByLevelDiff(_skill.Level, _target) || base.TargetAssignment == eTargetAssignment.OWNER_SITE)
			{
				AppendIsAlreadyExeced(_target.Owner, _num);
				eAbnormalState abnormalState = AbnormalStateDic[(ChangeSpeedAction.eChangeSpeedType)base.ActionDetail1];
				var abnormal = _target.Owner.ComponentAbnormal;
				double num = _valueDictionary[eValueNumber.VALUE_1];
				if (abnormalState == eAbnormalState.SLOW_OVERLAP)
				{
					num *= -1.0;
				}
				else if (abnormalState == eAbnormalState.HASTE_OVERLAP && !BattleUtil.LessThanOrApproximately(num + abnormal.CalcAbnormalStateSpeed(), 1.0))
				{
					abnormal.SetBuffParam(BuffParamKind.NUM, BuffParamKind.NUM, new Dictionary<BasePartsData, long> { { _target, 0 } }, 0.5f, _skill.SkillId, _source, _despelable: true, eEffectType.COMMON, _isBuff: true, _additional: false, _isShowIcon: false, _bonusId: 0, _environmentId: -1, _triggerCall: true);
				}

				ChangeSpeedAction.eCancelTriggerType actionDetail = (ChangeSpeedAction.eCancelTriggerType)base.ActionDetail2;
				if (actionDetail == ChangeSpeedAction.eCancelTriggerType.DAMAGED && !_target.Owner.ComponentDamageControl.OnDamageListForChangeSpeedDisableByAttack.ContainsKey(base.ActionId))
				{
					_target.Owner.ComponentDamageControl.OnDamageListForChangeSpeedDisableByAttack.Add(base.ActionId, delegate (bool byAttack)
					{
						if (byAttack)
						{
							_target.Owner.ComponentAbnormal.DisableAbnormalStateById(abnormalState, base.ActionId, _isReleasedByDamage: true);
						}
					});
				}

				abnormal.SetAbnormalState(_source, abnormalState, (base.AbnormalStateFieldAction == null) ? (float)_valueDictionary[eValueNumber.VALUE_3] : 90f, this, _skill, num, 0f, _reduceEnergy: false, base.ActionDetail2 == 1, 1f, _showsIcon: false);
				return;
			}

			ActionExecedData actionExecedData = base.AlreadyExecedData[_target.Owner][_num];
			if (actionExecedData.ExecedPartsNumber == actionExecedData.TargetPartsNumber)
			{
				if (actionExecedData.TargetPartsNumber == 1)
				{
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_SLIP_DAMAGE, eDamageEffectType.NORMAL, _target);
				}
				else
				{
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_SLIP_DAMAGE);
				}
			}
		}

		[global::OpenPCR.Migration("b3bb18e5-7f5b-425c-a160-fc256cc0eb9c")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
