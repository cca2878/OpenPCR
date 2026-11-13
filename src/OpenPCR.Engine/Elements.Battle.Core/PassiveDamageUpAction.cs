using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class PassiveDamageUpAction : ActionParameter
	{

		[global::OpenPCR.Migration("4b87ba72-2a25-48ba-9e4e-90f7d5f16410")]
		public PassiveDamageUpAction() : base()
		{
		}

		[global::OpenPCR.Migration("1a59a99d-16e4-4b3d-b4e7-fba614ffe54f")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (ActionDetail1 == 1)
			{
				var data = new DebuffDamageUpData
				{
					DebuffDamageUpLimitValue = _valueDictionary[eValueNumber.VALUE_2],
					DebuffDamageUpValue = _valueDictionary[eValueNumber.VALUE_1],
					DebuffDamageUpTimer = (float)_valueDictionary[eValueNumber.VALUE_3],
					EffectType = (DebuffDamageUpData.eEffectType)ActionDetail2
				};
				_target.Owner.ComponentAbnormal.AddDebuffDamageUpData(data);
			}
		}

		[global::OpenPCR.Migration("2ac28de9-62fa-4807-95a0-909499e42e1a")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
