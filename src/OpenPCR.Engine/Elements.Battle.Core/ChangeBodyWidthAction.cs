using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChangeBodyWidthAction : ActionParameter
	{
		[global::OpenPCR.Migration("a2bec5ac-688c-4982-b81f-022a062f66ef")]
		public ChangeBodyWidthAction() : base()
		{
		}

		[global::OpenPCR.Migration("2a11959e-130b-4154-8025-7483f9504f77")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			switch (ActionDetail1)
			{
				case 0:
					_target.Owner.BodyWidth = (float)_valueDictionary[eValueNumber.VALUE_1];
					break;
				case 1:
					_target.Owner.ComponentMultiPartsBoss.BossPartsListForBattle.Find(e => e.Index == ActionDetail2).BodyWidthValue = (float)_valueDictionary[eValueNumber.VALUE_1];
					break;
			}
		}

		[global::OpenPCR.Migration("ec790718-7aa3-4c4d-a086-901904b50f16")]
		public override void SetLevel(int _level)
		{
		}
	}
}
