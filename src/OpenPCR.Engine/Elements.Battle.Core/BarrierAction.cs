using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class BarrierAction : ActionParameter
	{

		[global::OpenPCR.Migration("4c86d541-8c94-4924-8f55-c8c957ea155c")]
		public BarrierAction() : base()
		{
		}

		[global::OpenPCR.Migration("59e4a1bf-ea31-49f9-af98-369dd3dce67f")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("3e4db591-3320-4137-9f46-56ca07dc5b75")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			int shield = BattleUtil.DoubleToIntReverseTruncate(_valueDictionary[eValueNumber.VALUE_1]);
			float duration = (float)_valueDictionary[eValueNumber.VALUE_3];
			switch (ActionDetail1)
			{
				case 1: _target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.GUARD_ATK, duration, this, _skill, shield, 0.0, false, false, 1.0, true, false, false); break;
				case 2: _target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.GUARD_MGC, duration, this, _skill, shield, 0.0, false, false, 1.0, true, false, false); break;
				case 3: _target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.DRAIN_ATK, duration, this, _skill, shield, 0.0, false, false, 1.0, true, false, false); break;
				case 4: _target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.DRAIN_MGC, duration, this, _skill, shield, 0.0, false, false, 1.0, true, false, false); break;
				case 5: _target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.GUARD_BOTH, duration, this, _skill, shield, 0.0, false, false, 1.0, true, false, false); break;
				case 6: _target.Owner.ComponentAbnormal.SetAbnormalState(_source, eAbnormalState.DRAIN_BOTH, duration, this, _skill, shield, 0.0, false, false, 1.0, true, false, false); break;
			}
		}

		[global::OpenPCR.Migration("c5713de2-8f4e-4108-a8ae-3bac06cac982")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
