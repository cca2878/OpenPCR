using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class DecoyAction : ActionParameter
	{
		[global::OpenPCR.Migration("7a0c8037-cb32-4d59-8c91-cd3810a97bd3")]
		public DecoyAction() : base()
		{
		}

		[global::OpenPCR.Migration("ea393af4-66e2-45b1-85c1-7cca1356e200")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("3e6c2802-76ea-4d38-955f-52b0c78b6d06")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			_target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.DECOY, (float)_valueDictionary[eValueNumber.VALUE_1], this, _skill, 0.0, 0.0, false, false, 1.0, true, false, false);
		}

		[global::OpenPCR.Migration("2048d01e-9e00-4633-a5a4-9c0bc783ea15")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
