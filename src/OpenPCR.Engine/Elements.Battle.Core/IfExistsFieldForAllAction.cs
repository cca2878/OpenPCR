using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class IfExistsFieldForAllAction : ActionParameter
	{
		[global::OpenPCR.Migration("c41d75f8-a540-4032-9258-4ac994104ebe")]
		public IfExistsFieldForAllAction() : base()
		{
		}

		[global::OpenPCR.Migration("af4a4166-575d-410b-8995-8af4d12f2097")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("5a26fc26-3014-496a-83cf-d9269921b82c")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictinary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictinary);
			if (!base.battleManager.ExistsField(base.ActionDetail1, _source.IsOther))
			{
				_skill.EffectBranchId = BattleUtil.DoubleToInt(base.Value[eValueNumber.VALUE_2]);
				if (base.ActionDetail2 != 0)
				{
					cancelAction(_skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2), _skill);
				}
			}
			else
			{
				_skill.EffectBranchId = BattleUtil.DoubleToInt(base.Value[eValueNumber.VALUE_1]);
				if (base.ActionDetail3 != 0)
				{
					cancelAction(_skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail3), _skill);
				}
			}

			_sourceActionController.AppendCoroutine(_sourceActionController.UpdateBranchMotion(this, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
		}

		[global::OpenPCR.Migration("d91c0a77-005f-4b88-a30c-b8d4ccd4968d")]
		private void cancelAction(ActionParameter _action, Skill _skill)
		{
			_action.CancelByIfForAll = true;
			if (_action is IfForAllAction)
			{
				(_action as IfForAllAction).CancelBoth(_skill);
			}
		}
	}
}
