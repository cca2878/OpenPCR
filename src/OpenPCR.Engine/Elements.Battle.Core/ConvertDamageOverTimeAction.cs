using global::OpenPCR.Foundation;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ConvertDamageOverTimeAction : ActionParameter
	{
		[global::OpenPCR.Migration("2c73db2b-0cca-4d78-9714-49daf8860bc7")]
		public ConvertDamageOverTimeAction() : base()
		{
		}

		[global::OpenPCR.Migration("7fe7d34d-f29c-47be-87f5-a148ad4c9e3d")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("dcbadc3d-f4ec-461a-9efe-878106e02691")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			if (_target.Owner.ComponentAbnormal.ConvertDamageOverTimeData != null)
				_target.Owner.ComponentAbnormal.ConvertDamageOverTimeData.Disable(true);
			_target.Owner.ComponentAbnormal.ConvertDamageOverTimeData = new ConvertDamageOverTimeData
			{
				Enable = true,
				Target = _target.Owner,
				Source = _source,
				DamageCutValue = _valueDictionary[eValueNumber.VALUE_1] / 100.0,
				Timer = _valueDictionary[eValueNumber.VALUE_2],
				NumberOfDamage = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_3]),
				OwnerAction = this
			};
			_target.Owner.ComponentAbnormal.OnChangeState.Call((eStateIconType)204, true);
			_target.Owner.ComponentAbnormal.ConvertDamageOverTimeData.StartTimer();
		}

		[global::OpenPCR.Migration("35725ac3-7eb8-4dc6-92e6-832e80441d3d")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
