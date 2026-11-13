using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class AccumulativeDamageAction : ActionParameter
	{
		[global::OpenPCR.Migration("8bac7012-d265-4a44-92a3-1d6cef74fd80")]
		public AccumulativeDamageAction() : base()
		{
		}

		[global::OpenPCR.Migration("c6f0cffe-f6d5-4968-94a0-907bdf1d46f9")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("b399e82b-d94c-41a3-8640-eacf0ef0d6d6")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			eAccumulativeDamageType kind = (eAccumulativeDamageType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]);
			UnitComponentAbnormal abnormal = _target.Owner.ComponentAbnormal;
			float duration = battleManager.TimeLimit;
			abnormal.SetAbnormalState(_source, eAbnormalState.ACCUMULATIVE_DAMAGE, duration, this, _skill, 0.0, 0.0, false, false, 1.0, true, false, false);
			AccumulativeDamageData data = new AccumulativeDamageData
			{
				AccumulativeDamageType = kind,
				FixedValue = kind == eAccumulativeDamageType.FIXED ? _valueDictionary[eValueNumber.VALUE_2] : 0.0,
				PercentageValue = kind == eAccumulativeDamageType.PERCENTAGE ? _valueDictionary[eValueNumber.VALUE_2] : 0.0,
				CountLimit = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_4])
			};
			Dictionary<UnitCtrl, AccumulativeDamageData> dictionary = _target.Owner.ComponentAbnormal.AccumulativeDamageDataDictionary;
			if (!dictionary.ContainsKey(_source))
				dictionary.Add(_source, data);
		}

		[global::OpenPCR.Migration("e5d4dbac-74bd-4d0a-8162-1765b1e9d813")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
			Value[eValueNumber.VALUE_4] = MasterData.action_value_4 + MasterData.action_value_5 * _level;
		}
	}
}
