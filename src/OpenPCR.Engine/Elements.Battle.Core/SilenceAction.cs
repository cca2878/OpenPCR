using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class SilenceAction : ActionParameter
	{
		[global::OpenPCR.Migration("ffb04a21-dbc3-4c95-ab13-04463737850a")]
		public SilenceAction() : base()
		{
		}

		[global::OpenPCR.Migration("efba3e13-1ff2-4e3e-9cf4-d379f42879b2")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("531ae091-8102-4945-86a5-f34bbc837cf5")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			bool forceHit = _target.Owner.ComponentAbnormal.IsForceHit(eActionType.SILENCE, ActionDetail1);
			if (ActionDetail1 == 1)
				forceHit |= ActionDetail2 == 1;
			float random = Elements.Battle.BattleManager.Random(0f, 1f);
			if (forceHit || random < BattleUtil.CalcDodgeByLevelDiffUseParam(_skill.Level, _source, _target, IsExEquipSkillAction, (float)_valueDictionary[eValueNumber.VALUE_3]))
			{
				eAbnormalState state = ActionDetail1 == 1 ? eAbnormalState.UB_SILENCE : eAbnormalState.SILENCE;
				AppendIsAlreadyExeced(_target.Owner, _num);
				_target.Owner.ComponentAbnormal.SetAbnormalState(_source, state, AbnormalStateFieldAction == null ? (float)_valueDictionary[eValueNumber.VALUE_1] : 90f, this, _skill);
			}
			else
			{
				ActionExecedData data = AlreadyExecedData[_target.Owner][_num];
				if (data.ExecedPartsNumber == data.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_SILENCE, eDamageEffectType.NORMAL, data.TargetPartsNumber == 1 ? _target : null, 1f);
			}
		}

		[global::OpenPCR.Migration("97c78683-683a-4917-92f5-30cfe52ebc09")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
