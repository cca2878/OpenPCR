using System;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChargeEnergyAction : ActionParameter
	{
		[global::OpenPCR.Migration("32abdfb1-597b-443d-863c-72e0b9b7ce1b")]
		public ChargeEnergyAction() : base()
		{
		}

		[global::OpenPCR.Migration("a8483c3f-18df-4b27-873d-112d3297aa74")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("44f77b44-cadf-4272-8674-e53c9b617995")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			UnitCtrl owner = _target.Owner;
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(owner, _num);
			double value = 0.0;
			switch ((eChargeType)ActionDetail1)
			{
				case eChargeType.INCREASE:
				case eChargeType.INCREASE_WITHOUT_RECOVERY_RATE:
					value = _valueDictionary[eValueNumber.VALUE_1];
					break;
				case eChargeType.DECREASE:
					value = -_valueDictionary[eValueNumber.VALUE_1];
					break;
				case eChargeType.DECREASE_BASED_ON_CURRENT_TP:
					value = owner.ComponentParameter.Energy * _valueDictionary[eValueNumber.VALUE_1] / 100.0 - _target.Owner.ComponentParameter.Energy;
					break;
			}

			if (owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.ENERGY_DAMAGE_REDUCE) && value < 0.0)
				value *= owner.ComponentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.ENERGY_DAMAGE_REDUCE);
			if (_skill.BonusId != 0)
				battleManager.DeleteBonusIcon(_skill.BonusId);
			owner.ComponentDamageControl.ChargeEnergy(eSetEnergyType.BY_CHANGE_ENERGY, value, false, null, hasRecoveryEffect((eChargeType)ActionDetail1), _source, ActionDetail2 != -1, (eEffectType)EffectType, ActionDetail1 != 4, false, 1.0);
		}

		[global::OpenPCR.Migration("798605a0-5423-4d6f-8c5d-c1d7e7cd0691")]
		private bool hasRecoveryEffect(eChargeType _chargeType)
		{
			return _chargeType == eChargeType.INCREASE || _chargeType == eChargeType.INCREASE_WITHOUT_RECOVERY_RATE;
		}

		[global::OpenPCR.Migration("62134630-7f67-483c-999e-79394de3f188")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + System.Math.Ceiling(MasterData.action_value_2 * _level);
		}
	}
}
