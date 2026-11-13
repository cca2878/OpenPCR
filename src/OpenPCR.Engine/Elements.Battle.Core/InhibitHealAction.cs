using Elements.Battle;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public partial class InhibitHealAction : ActionParameter
	{
		private enum eInhibitType
		{
			INHIBIT = 0,
			DECREASE = 1,
			DISABLE = 2
		}

		private Dictionary<eInhibitType, eAbnormalState> abnormalStateDic = new Dictionary<eInhibitType, eAbnormalState>
		{
			{
				eInhibitType.INHIBIT,
				eAbnormalState.INHIBIT_HEAL
			},
			{
				eInhibitType.DECREASE,
				eAbnormalState.DECREASE_HEAL
			},
			{
				eInhibitType.DISABLE,
				eAbnormalState.DISABLE_HEAL
			}
		};

		[global::OpenPCR.Migration("c4c419f6-2cb4-4fd1-b4db-618a0bac1a87")]
		public InhibitHealAction() : base()
		{
		}

		[global::OpenPCR.Migration("5de5c5d2-728d-4294-939c-abc91cab4850")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("fbc45d80-00e6-4918-b015-ec91545c1dab")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			float roll = BattleManager.Random(0f, 1f);
			if (roll < BattleUtil.GetDodgeByLevelDiff(_skill.Level, _target) || (int)TargetAssignment == 2)
			{
				AppendIsAlreadyExeced(_target.Owner, _num);
				_target.Owner.ComponentAbnormal.SetAbnormalState(_source, abnormalStateDic[(eInhibitType)ActionDetail1],
					AbnormalStateFieldAction != null ? 9999f : (float)_valueDictionary[eValueNumber.VALUE_2],
					this, _skill, _valueDictionary[eValueNumber.VALUE_1], 0.0, false, false, 1.0, true, false, false);
			}
			else
			{
				var executed = AlreadyExecedData[_target.Owner][_num];
				if (executed.ExecedPartsNumber == executed.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_SLIP_DAMAGE, eDamageEffectType.NORMAL,
						executed.TargetPartsNumber == 1 ? _target : null, 1f);
			}
		}

		[global::OpenPCR.Migration("decd8532-c3ac-443c-943b-37f187e3e48c")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = System.Math.Max(0.0, MasterData.action_value_1 + MasterData.action_value_4 * _level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
		}
	}
}
