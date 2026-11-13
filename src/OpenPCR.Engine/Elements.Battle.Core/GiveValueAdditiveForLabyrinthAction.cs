using System;
using System.Collections.Generic;
using Elements.Data;

namespace Elements.Battle.Core
{
	public class GiveValueAdditiveForLabyrinthAction : GiveValueAction, ISingletonField
	{
		private LabyrinthTempData labyrinthTempData;
		[global::OpenPCR.Migration("ed8051d2-3e3e-42fe-9e5c-c83400102fcf")]
		public GiveValueAdditiveForLabyrinthAction() : base()
		{
		}

		[global::OpenPCR.Migration("4a025593-8d48-4be1-85ba-fb60ff1647ad")]
		public override void Initialize()
		{
			base.Initialize();
			labyrinthTempData = this.CreateSingletonTree().Get<LabyrinthTempData>();
		}
		[global::OpenPCR.Migration("7ed3370b-65e5-46a2-8b4f-fbcbd465bec2")]
		protected override void setValue(Dictionary<eValueNumber, double> _value, ActionParameter _action)
		{
			base.setValue(_value, _action);
			_action.AdditionalValue = _value;
		}
		[global::OpenPCR.Migration("4b0c09f4-a030-462e-8a93-acba25dd8828")]
		protected override void createValue(UnitCtrl _source, Skill _skill, Dictionary<eValueNumber, double> _valueDictionary, Dictionary<eValueNumber, double> _addValue, eValueNumber _evalue, BasePartsData _target)
		{
			int type = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]);
			int count = global::OpenPCR.PreparedLabyrinthInputs.Require(labyrinthTempData).TargetValue(type);
			_addValue[_evalue] = _valueDictionary[eValueNumber.VALUE_2] * (double)count;
			if ((double)MasterData.action_value_2 >= 0.0) createPositiveValue(_addValue, _evalue);
			else createNegativeValue(_addValue, _evalue);
		}
		[global::OpenPCR.Migration("678599b1-109a-434a-ae94-c6bcbdd7af01")]
		private void createPositiveValue(Dictionary<eValueNumber, double> _addValue, eValueNumber _evalue)
		{
			if (Value[eValueNumber.VALUE_3] != 0.0) _addValue[_evalue] = Math.Min(_addValue[_evalue], Value[eValueNumber.VALUE_3]);
			_addValue[_evalue] = Math.Max(_addValue[_evalue], 0.0);
		}
		[global::OpenPCR.Migration("17f0bad9-ce78-421a-81f6-13ffe2312dcd")]
		private void createNegativeValue(Dictionary<eValueNumber, double> _addValue, eValueNumber _evalue)
		{
			if (Value[eValueNumber.VALUE_3] != 0.0) _addValue[_evalue] = Math.Max(_addValue[_evalue], Value[eValueNumber.VALUE_3]);
			_addValue[_evalue] = Math.Min(_addValue[_evalue], 0.0);
		}
	}
}
