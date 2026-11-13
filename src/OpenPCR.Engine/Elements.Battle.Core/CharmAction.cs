using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class CharmAction : ActionParameter
	{
		[global::OpenPCR.Migration("97426f18-696d-4c52-9f5f-1014cac7e0a8")]
		public CharmAction() : base()
		{
		}

		[global::OpenPCR.Migration("ae74a27f-f3b8-47f3-ac8d-c48b2b3de6e8")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("2541f0ad-7d99-4e74-88c2-d230bf6f0099")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (_target.Owner.ComponentState.ModeChangeUnableStateBarrier || _target.Owner.ComponentState.TriggerStealthUnableStateBarrier)
				return;
			float random = Elements.Battle.BattleManager.Random(0f, 1f);
			float dodge = BattleUtil.CalcDodgeByLevelDiffUseParam(_skill.Level, _source, _target, IsExEquipSkillAction, (float)_valueDictionary[eValueNumber.VALUE_3]);
			bool forceHit = _target.Owner.ComponentAbnormal.IsForceHit(eActionType.CHARM, ActionDetail1);
			if (random < dodge || forceHit)
			{
				AppendIsAlreadyExeced(_target.Owner, _num);
				eAbnormalState state = ActionDetail1 == 1 ? eAbnormalState.CONFUSION : ActionDetail1 == 2 ? eAbnormalState.CONFUSION2 : ActionDetail1 == 3 ? (eAbnormalState)78 : eAbnormalState.CONVERT;
				_target.Owner.ComponentAbnormal.SetAbnormalState(_source, state, AbnormalStateFieldAction == null ? (float)_valueDictionary[eValueNumber.VALUE_1] : 90f, this, _skill, 0.0, 0.0, false, false, 1.0, _valueDictionary[eValueNumber.VALUE_5] == 0.0, false, IsIgnoreDebuffImmune());
			}
			else
			{
				ActionExecedData data = AlreadyExecedData[_target.Owner][_num];
				if (data.ExecedPartsNumber == data.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_CHARM, eDamageEffectType.NORMAL, data.TargetPartsNumber == 1 ? _target : null, 1f);
			}
		}

		[global::OpenPCR.Migration("b85b31ff-e0f1-48e1-b00f-30ac4a6a6677")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}

		[global::OpenPCR.Migration("8cc244ea-482c-46b6-a3e6-26851c5dad2a")]
		public override bool IsIgnoreDebuffImmune()
		{
			return BattleUtil.Approximately(Value[eValueNumber.VALUE_6], 1.0);
		}
	}
}
