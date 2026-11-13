using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class CountBlindAction : ActionParameter
	{
		[global::OpenPCR.Migration("15cd07d3-e760-4c10-99c4-bb38b4f4951c")]
		public CountBlindAction() : base()
		{
		}

		[global::OpenPCR.Migration("731f58f7-f83c-4cf3-a654-35cb3ebc1cf6")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("cde80909-cbd4-45e7-9121-0e2452f755b5")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			float roll = BattleManager.Random(0f, 1f);
			if (BattleUtil.LessThanOrApproximately(roll, BattleUtil.GetDodgeByLevelDiff(_skill.Level, _target)))
			{
				AppendIsAlreadyExeced(_target.Owner, _num);
				int kind = global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]);
				if (kind == 1)
					_target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.COUNT_BLIND,
						(float)_valueDictionary[eValueNumber.VALUE_2], this, _skill, 99999.0, (double)ActionDetail1,
						false, false, 1.0, true, false, false);
				else if (kind == 2)
					_target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.COUNT_BLIND,
						90f, this, _skill, _valueDictionary[eValueNumber.VALUE_2], (double)ActionDetail1,
						false, false, 1.0, true, false, false);
			}
			else
			{
				var executed = AlreadyExecedData[_target.Owner][_num];
				if (executed.ExecedPartsNumber == executed.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_DARK, eDamageEffectType.NORMAL,
						executed.TargetPartsNumber == 1 ? _target : null, 1f);
			}
		}

		[global::OpenPCR.Migration("9a7dbd4a-0867-4b3b-ba5a-7035502c88f9")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
		}
	}
}
