using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChangeEnergyRecoveryRatioByDamage : ActionParameter
	{
		[global::OpenPCR.Migration("a05b95ad-3af2-4570-92d0-4042761b9c4f")]
		public ChangeEnergyRecoveryRatioByDamage()
		{
		}

		[global::OpenPCR.Migration("ac6a2cf9-f372-482d-8377-fac2bb764296")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			Queue<List<int>> val = new Queue<List<int>>();
			val.Enqueue(base.ActionChildrenIndexes);
			while (val.Count > 0)
			{
				List<int> list = val.Dequeue();
				for (int i = 0; i < list.Count; i++)
				{
					_skill.ActionParameters[list[i]].EnergyChargeMultiple = _valueDictionary[eValueNumber.VALUE_1];
					val.Enqueue(_skill.ActionParameters[list[i]].ActionChildrenIndexes);
				}
			}
		}
	}
}
