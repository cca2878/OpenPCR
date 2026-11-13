using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class StopFieldAction : ActionParameter
	{
		[global::OpenPCR.Migration("316e56e2-d175-4832-b717-70d0f103847d")]
		public StopFieldAction() : base()
		{
		}

		[global::OpenPCR.Migration("03bf0072-9243-448a-948c-3fe613222f4a")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			base.battleManager.StopField(base.ActionDetail1, base.TargetAssignment, _source.IsOther);
		}

		[global::OpenPCR.Migration("6e95f8f3-df79-467a-b4c0-8063d53b32ca")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
