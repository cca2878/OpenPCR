using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChargeEnergyByDamage : ActionParameter
	{
		private enum eChargeType
		{
			INCREASE = 1
		}

		[global::OpenPCR.Migration("92e10fe5-f831-4d9c-8ccd-9ae206993c97")]
		public ChargeEnergyByDamage()
		{
		}

		[global::OpenPCR.Migration("f18e114e-64b4-40cf-9baf-d5070f6cbd99")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			UnitCtrl targetUnit = _target.Owner;
			AppendIsAlreadyExeced(targetUnit, _num);
			eStateIconType actionDetail = (eStateIconType)base.ActionDetail2;
			if (!targetUnit.ComponentAbnormal.SealDictionary.ContainsKey(actionDetail))
			{
				targetUnit.ComponentAbnormal.SealDictionary.Add(actionDetail, new SealData { Max = (int)_valueDictionary[eValueNumber.VALUE_4], DisplayCount = true });
			}
			else
			{
				targetUnit.ComponentAbnormal.SealDictionary[actionDetail].Max = System.Math.Max((int)_valueDictionary[eValueNumber.VALUE_4], targetUnit.ComponentAbnormal.SealDictionary[actionDetail].Max);
			}

			SealData sealData = targetUnit.ComponentAbnormal.SealDictionary[actionDetail];
			if (sealData.GetCurrentCount() == 0)
			{
				targetUnit.ComponentAbnormal.OnChangeState.Call(actionDetail, arg2: true);
			}

			sealData.AddSeal((float)_valueDictionary[eValueNumber.VALUE_5], targetUnit, actionDetail, (int)_valueDictionary[eValueNumber.VALUE_3]);
			eChargeType chargeType = (eChargeType)base.ActionDetail1;
			double chargeValue = 0.0;
			eChargeType eChargeType2 = chargeType;
			if (eChargeType2 == eChargeType.INCREASE)
			{
				chargeValue = System.Math.Min(1000.0, _valueDictionary[eValueNumber.VALUE_1]);
			}

			targetUnit.ComponentAbnormal.ChargeEnergyByReceiveDamageDictionary[actionDetail] = delegate
			{
				if (sealData.GetCurrentCount() != 0)
				{
					targetUnit.ComponentDamageControl.ChargeEnergy(eSetEnergyType.BY_CHANGE_ENERGY, chargeValue, _slipDamage: false, null, chargeType == eChargeType.INCREASE, _source);
					sealData.RemoveSeal(1, _isPassiveSeal: true);
				}
			};
		}

		[global::OpenPCR.Migration("b5a44afa-d111-47c6-be38-cad31c579193")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
