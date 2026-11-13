using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class EnergyDamageReduceAction : ActionParameter
	{
		[global::OpenPCR.Migration("d214aa5d-26fc-485a-b8fe-6d061711100b")]
		public EnergyDamageReduceAction() : base()
		{
		}

		[global::OpenPCR.Migration("3dede80e-c97b-4a9a-a2c1-88f69ab2efbb")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("9545df39-7e57-459a-84d5-9794345c9e40")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			_target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.ENERGY_DAMAGE_REDUCE, (float)_valueDictionary[eValueNumber.VALUE_2], this, _skill, _valueDictionary[eValueNumber.VALUE_1], 0.0, false, false, 1.0, BattleUtil.Approximately(_valueDictionary[eValueNumber.VALUE_4], 0.0), false, false);
		}

		[global::OpenPCR.Migration("4179fe1a-d0d4-40b6-99a1-f5493a259f9a")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
		}
	}
}
