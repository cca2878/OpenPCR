using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class GiveValueAdditiveAction : GiveValueAction
	{
		[global::OpenPCR.Migration("b7b6e115-4cc9-4623-9aed-bb3150347dd9")]
		public GiveValueAdditiveAction() : base()
		{
		}

		[global::OpenPCR.Migration("3b0b3799-aaa8-4b72-92a4-ca6f3858c9d4")]
		protected override void setValue(Dictionary<eValueNumber, double> _value, ActionParameter _action)
		{
			base.setValue(_value, _action);
			_action.AdditionalValue = _value;
		}

		[global::OpenPCR.Migration("990c3c81-ad4d-4e2e-b5b9-73c85ec9217b")]
		protected override void createValue(UnitCtrl _source, Skill _skill, Dictionary<eValueNumber, double> _valueDictionary, Dictionary<eValueNumber, double> _addValue, eValueNumber _evalue, BasePartsData _target)
		{
			base.createValue(_source, _skill, _valueDictionary, _addValue, _evalue, _target);
			if ((double)base.MasterData.action_value_2 >= 0.0)
			{
				createPositiveValue(_addValue, _evalue);
			}
			else
			{
				createNegativeValue(_addValue, _evalue);
			}
		}

		[global::OpenPCR.Migration("6ff4ebe6-2a56-4a3f-b936-43e965daabc3")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
			Value[eValueNumber.VALUE_4] = MasterData.action_value_4 + MasterData.action_value_5 * _level;
		}

		[global::OpenPCR.Migration("de8b0e54-6dec-4e75-9378-13c3a2b08452")]
		protected override double calcDamageValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return (double)((long)_source.ComponentParameter.MaxHp - (long)_source.ComponentParameter.Hp) * _valueDictionary[eValueNumber.VALUE_2];
		}

		[global::OpenPCR.Migration("c5aa8084-c527-4ced-924b-e46b828f01f3")]
		protected override double calcHpValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return (double)(long)_source.ComponentParameter.Hp * _valueDictionary[eValueNumber.VALUE_2];
		}

		[global::OpenPCR.Migration("cc6a4292-4284-4598-b650-0469bf6a45ea")]
		protected override double calcMaxHpValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return (double)(long)_source.ComponentParameter.MaxHp * _valueDictionary[eValueNumber.VALUE_2];
		}

		[global::OpenPCR.Migration("63a3dc1c-d1c5-4e2c-ad5b-301aba6848a0")]
		private void createPositiveValue(Dictionary<eValueNumber, double> _addValue, eValueNumber _evalue)
		{
			if ((double)MasterData.action_value_4 != 0.0 || (double)MasterData.action_value_5 != 0.0)
				_addValue[_evalue] = System.Math.Min(_addValue[_evalue], Value[eValueNumber.VALUE_4]);
			_addValue[_evalue] = System.Math.Max(_addValue[_evalue], 0.0);
		}

		[global::OpenPCR.Migration("af83637d-71e9-486c-837d-22955ee81d08")]
		private void createNegativeValue(Dictionary<eValueNumber, double> _addValue, eValueNumber _evalue)
		{
			if ((double)MasterData.action_value_4 != 0.0 || (double)MasterData.action_value_5 != 0.0)
				_addValue[_evalue] = System.Math.Max(_addValue[_evalue], Value[eValueNumber.VALUE_4]);
			_addValue[_evalue] = System.Math.Min(_addValue[_evalue], 0.0);
		}
	}
}
