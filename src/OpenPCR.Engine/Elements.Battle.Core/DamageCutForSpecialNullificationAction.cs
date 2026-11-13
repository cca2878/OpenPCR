using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class DamageCutForSpecialNullificationAction : ActionParameter
	{
		[global::OpenPCR.Migration("fd1e4161-45e1-40b4-8e12-cd1496f7e3c3")]
		public DamageCutForSpecialNullificationAction() : base()
		{
		}

		[global::OpenPCR.Migration("035dbd77-efc0-4cac-bcb1-dd17f6a176d4")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("046c355d-858c-4c23-b042-566079552d9e")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			if (_target.Owner.ComponentAbnormal.DamageCutForSpecialNullificationData != null)
				_target.Owner.ComponentAbnormal.DamageCutForSpecialNullificationData.Disable();
			_target.Owner.ComponentAbnormal.DamageCutForSpecialNullificationData = new DamageCutForSpecialNullificationData(
				(DamageCutForSpecialNullificationData.eDamageCutType)ActionDetail1,
				Value[eValueNumber.VALUE_1] / 100.0,
				Value[eValueNumber.VALUE_3], _target.Owner, (eStateIconType)ActionDetail2,
				(DamageCutForSpecialNullificationData.eReleaseType)BattleUtil.DoubleToInt(Value[eValueNumber.VALUE_5]),
				battleManager, battleEffectPool, ActionEffectList, _skill);
		}

		[global::OpenPCR.Migration("22788de0-4d77-4a0d-b2fd-9b7da82796aa")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
