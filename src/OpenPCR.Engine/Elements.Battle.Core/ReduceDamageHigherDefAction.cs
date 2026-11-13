using global::OpenPCR.Foundation;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ReduceDamageHigherDefAction : ActionParameter
	{
		[global::OpenPCR.Migration("0919f01b-9919-4447-b5c7-b43af8580fb4")]
		public ReduceDamageHigherDefAction() : base()
		{
		}

		[global::OpenPCR.Migration("87143c55-ab48-4b52-a159-cf2dc9afe6a8")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("7a99ab3f-66a0-4848-a5dd-3a457820bef8")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			var abnormal = _target.Owner.ComponentAbnormal;
			if (abnormal.ReduceDamageByHigherDefData != null)
				abnormal.ReduceDamageByHigherDefData.Disable();
			_target.Owner.ComponentAbnormal.ReduceDamageByHigherDefData = new ReduceDamageByHigherDefData
			{
				Enable = true,
				Target = _target.Owner,
				Timer = _valueDictionary[eValueNumber.VALUE_1]
			};
			_target.Owner.ComponentAbnormal.OnChangeState.Call((eStateIconType)203, true);
			_target.Owner.ComponentAbnormal.ReduceDamageByHigherDefData.StartTimer();
		}

		[global::OpenPCR.Migration("881a59b0-878a-490e-8fbf-555b98102195")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
