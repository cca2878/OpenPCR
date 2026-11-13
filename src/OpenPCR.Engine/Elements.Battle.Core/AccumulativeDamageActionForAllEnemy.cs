using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class AccumulativeDamageActionForAllEnemy : ActionParameter
	{
		[global::OpenPCR.Migration("e914c2c9-d6d4-4c54-8af4-e8706f214edc")]
		public AccumulativeDamageActionForAllEnemy() : base()
		{
		}

		[global::OpenPCR.Migration("355af9fd-a4b1-4d1e-9b75-252443f82cba")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			UnitCtrl owner = _target.Owner;
			AppendIsAlreadyExeced(owner, _num);
			eAccumulativeDamageType kind = (eAccumulativeDamageType)global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]);
			UnitComponentAbnormal abnormal = owner.ComponentAbnormal;
			float duration = battleManager.TimeLimit;
			abnormal.SetAbnormalState(_source, eAbnormalState.ACCUMULATIVE_DAMAGE_FOR_ALL_ENEMY, duration, this, _skill, 0.0, 0.0, false, false, 1.0, true, false, false);
			if (!owner.ComponentAbnormal.AccumulativeDamageSourceDataDictionary.ContainsKey(ActionId))
				owner.ComponentAbnormal.AccumulativeDamageSourceDataDictionary[ActionId] = new AccumulativeDamageSourceData(kind, Value[eValueNumber.VALUE_2], global::OpenPCR.NativeNumeric.ToInt32(Value[eValueNumber.VALUE_4]));
		}

		[global::OpenPCR.Migration("4ea00a8b-30b7-4f99-9205-1b3b2283c60b")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
			Value[eValueNumber.VALUE_4] = MasterData.action_value_4 + MasterData.action_value_5 * _level;
		}
	}
}
