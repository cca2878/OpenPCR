using Elements.Battle;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class DamageByBehaviourAction : ActionParameter
	{
		public enum eDamageByBehaviourEffectType
		{
			NONE,
			WORLD_LIGHTNING
		}

		public static readonly Dictionary<eSlipDamageType, eAbnormalState> abnormalStateDic;
		public static readonly Dictionary<eAbnormalState, bool> abnormalStateDodgeByLevel;
		static DamageByBehaviourAction()
		{
			abnormalStateDic = new Dictionary<eSlipDamageType, eAbnormalState>
			{
				{ (eSlipDamageType)1, (eAbnormalState)53 },
				{ (eSlipDamageType)10, (eAbnormalState)73 }
			};
			abnormalStateDodgeByLevel = new Dictionary<eAbnormalState, bool>
			{
				{ (eAbnormalState)53, true },
				{ (eAbnormalState)73, false }
			};
		}

		public eDamageByBehaviourEffectType DamageEffectType { get; set; }

		[global::OpenPCR.Migration("fcd91fc9-fc78-43e0-aff4-3c0f9750e3e7")]
		public DamageByBehaviourAction() : base()
		{
		}

		[global::OpenPCR.Migration("716e1db8-b90d-4e61-8514-2b9ff135c28a")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			DamageEffectType = (eDamageByBehaviourEffectType)global::OpenPCR.NativeNumeric.ToInt32((double)MasterData.action_value_6);
			if (DamageEffectType != eDamageByBehaviourEffectType.NONE)
				BattleResourceManager.Instance.LoadDamageByBehaviourEffect(DamageEffectType);
		}

		[global::OpenPCR.Migration("74cf4bed-f54f-4291-8be4-71a48ef7dd27")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (ActionDetail3 != 0)
				EnergyChargeMultiple = (double)ActionDetail3 / -100.0 + 1.0;
			float roll = Elements.Battle.BattleManager.Random(0f, 1f);
			eAbnormalState state = abnormalStateDic[(eSlipDamageType)ActionDetail1];
			if (roll < BattleUtil.CalcDodgeByLevelDiffUseParam(_skill.Level, _source, _target, IsExEquipSkillAction, 1f) || !abnormalStateDodgeByLevel[state])
			{
				AppendIsAlreadyExeced(_target.Owner, _num);
				_target.Owner.ComponentAbnormal.SetAbnormalState(_source, state, AbnormalStateFieldAction != null ? 90f : (float)_valueDictionary[eValueNumber.VALUE_3], this, _skill, BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]), _valueDictionary[eValueNumber.VALUE_5], false, false, 0.0, true, IsEnvironmentAbnormalState, false);
			}
			else
			{
				ActionExecedData executed = AlreadyExecedData[_target.Owner][_num];
				if (executed.ExecedPartsNumber == executed.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_DAMAGE_BY_BEHAVIOUR, eDamageEffectType.NORMAL, executed.ExecedPartsNumber == 1 ? _target : null, 1f);
			}
		}

		[global::OpenPCR.Migration("f87783d8-301b-49bd-9e94-fb19b86dc96a")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
