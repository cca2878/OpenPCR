using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class BlindAction : ActionParameter
	{
		public enum eBlindType { PHYSICAL, MAGIC }
		public static Dictionary<eBlindType, eAbnormalState> AbnormalStateDic;
		private const float PERCENTAGE_TO_RATE = 0.01f;
		[global::OpenPCR.Migration("00c59ca7-d3e1-4ae5-9bf5-e9f19e0a7b41")]
		static BlindAction()
		{
			AbnormalStateDic = new Dictionary<eBlindType, eAbnormalState>
			{
				{ eBlindType.PHYSICAL, (eAbnormalState)15 },
				{ eBlindType.MAGIC, (eAbnormalState)49 }
			};
		}

		[global::OpenPCR.Migration("21681d47-5de8-4595-a067-473e47875e81")]
		public BlindAction() : base()
		{
		}

		[global::OpenPCR.Migration("3357f4ad-4d47-495d-a0fb-31e57140153e")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("4cfabcad-135d-4731-b841-691dfaa16ca3")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			float roll = BattleManager.Random(0f, 1f);
			float chance = BattleUtil.CalcDodgeByLevelDiffUseParam(_skill.Level, _source, _target, IsExEquipSkillAction,
				(float)_valueDictionary[eValueNumber.VALUE_3] * 0.01f);
			bool hit = BattleUtil.LessThanOrApproximately(roll, chance);
			if (hit | _target.Owner.ComponentAbnormal.IsForceHit(eActionType.BLIND, ActionDetail2))
			{
				AppendIsAlreadyExeced(_target.Owner, _num);
				_target.Owner.ComponentAbnormal.SetAbnormalState(_source, AbnormalStateDic[(eBlindType)ActionDetail2],
					AbnormalStateFieldAction != null ? 90f : (float)_valueDictionary[eValueNumber.VALUE_1],
					this, _skill, (double)ActionDetail1, 0.0, false, false, 1.0, true, false, false);
			}
			else
			{
				var executed = AlreadyExecedData[_target.Owner][_num];
				if (executed.ExecedPartsNumber == executed.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_DARK, eDamageEffectType.NORMAL,
						executed.TargetPartsNumber == 1 ? _target : null, 1f);
			}
		}

		[global::OpenPCR.Migration("ecaef397-bd98-4509-b1f6-7a37d85ed488")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
