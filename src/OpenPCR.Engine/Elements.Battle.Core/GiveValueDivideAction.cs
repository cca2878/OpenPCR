using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class GiveValueDivideAction : GiveValueAction
	{
		[global::OpenPCR.Migration("33b1d5ae-12a5-486e-b9ab-ed954bacd8c5")]
		public GiveValueDivideAction() : base()
		{
		}

		[global::OpenPCR.Migration("118ff528-c874-4c33-8887-a7e3380be950")]
		protected override void setValue(Dictionary<eValueNumber, double> _value, ActionParameter _action)
		{
			base.setValue(_value, _action);
			_action.DivideValue = _value;
		}

		[global::OpenPCR.Migration("8ca33627-93a6-4436-82ac-097c0b224532")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
		}
		[global::OpenPCR.Migration("01e4607d-5623-4cb5-bebd-3854eaf81010")]
		protected override double calcDamageValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return (double)((long)_source.ComponentParameter.MaxHp - (long)_source.ComponentParameter.Hp) / (double)(long)_source.ComponentParameter.MaxHp * _valueDictionary[eValueNumber.VALUE_2];
		}

		[global::OpenPCR.Migration("95797ea0-c961-4181-a1a4-387c87f11887")]
		protected override double calcHpValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return (double)(long)_source.ComponentParameter.Hp / (double)(long)_source.ComponentParameter.MaxHp * _valueDictionary[eValueNumber.VALUE_2];
		}

		[global::OpenPCR.Migration("f583eb11-5353-4267-8955-b1119cf97d7c")]
		protected override double calcMaxHpValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return (double)(long)_source.ComponentParameter.MaxHp * _valueDictionary[eValueNumber.VALUE_2];
		}
	}
}
