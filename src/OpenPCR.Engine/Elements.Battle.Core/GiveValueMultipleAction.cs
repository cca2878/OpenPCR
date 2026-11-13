using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class GiveValueMultipleAction : GiveValueAction
	{
		[global::OpenPCR.Migration("2f1cfb83-db20-41af-8179-6cb5e9434697")]
		public GiveValueMultipleAction() : base()
		{
		}

		[global::OpenPCR.Migration("305faa72-bbc5-4c6d-bc07-52cbd80040af")]
		protected override void setValue(Dictionary<eValueNumber, double> _value, ActionParameter _action)
		{
			base.setValue(_value, _action);
			_action.MultipleValue = _value;
		}

		[global::OpenPCR.Migration("8d2e6275-1c61-4350-ad89-3893fbc5998b")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
		}
		[global::OpenPCR.Migration("0a280303-83b6-45a5-8e72-17e462f6e882")]
		protected override double calcDamageValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return (double)((long)_source.ComponentParameter.MaxHp - (long)_source.ComponentParameter.Hp) / (double)(long)_source.ComponentParameter.MaxHp * _valueDictionary[eValueNumber.VALUE_2];
		}

		[global::OpenPCR.Migration("6f9efd8e-655e-4bf3-834f-2e45458439b0")]
		protected override double calcHpValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return (double)(long)_source.ComponentParameter.Hp / (double)(long)_source.ComponentParameter.MaxHp * _valueDictionary[eValueNumber.VALUE_2];
		}

		[global::OpenPCR.Migration("13d4ad26-949b-43af-8593-8f140e00c532")]
		protected override double calcMaxHpValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return (double)(long)_source.ComponentParameter.MaxHp * _valueDictionary[eValueNumber.VALUE_2];
		}
	}
}
