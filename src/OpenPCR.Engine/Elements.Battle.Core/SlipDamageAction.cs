using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public partial class SlipDamageAction : ActionParameter
	{
		private class eSlipDamageType_DictComparer : IEqualityComparer<eSlipDamageType>
		{
			[global::OpenPCR.Migration("2406ab32-1efc-49ed-bbec-06f759020d4c")]
			public eSlipDamageType_DictComparer() { }
			[global::OpenPCR.Migration("55c1ccee-6803-486c-91d9-13cdda27236b")]
			public bool Equals(eSlipDamageType _x, eSlipDamageType _y) { return _x == _y; }
			[global::OpenPCR.Migration("bfb7d088-3245-4213-88e2-bd4e14ed39dc")]
			public int GetHashCode(eSlipDamageType _obj) { return (int)_obj; }
		}
		public static readonly Dictionary<eSlipDamageType, eAbnormalState> AbnormalStateDic;
		[global::OpenPCR.Migration("1d77f452-769c-4251-9945-4e825751d039")]
		static SlipDamageAction()
		{
			AbnormalStateDic = new Dictionary<eSlipDamageType, eAbnormalState>(new eSlipDamageType_DictComparer())
			{
				{ (eSlipDamageType)1, (eAbnormalState)8 }, { (eSlipDamageType)2, (eAbnormalState)9 },
				{ (eSlipDamageType)3, (eAbnormalState)10 }, { (eSlipDamageType)0, (eAbnormalState)21 },
				{ (eSlipDamageType)4, (eAbnormalState)32 }, { (eSlipDamageType)5, (eAbnormalState)37 },
				{ (eSlipDamageType)6, (eAbnormalState)40 }, { (eSlipDamageType)7, (eAbnormalState)59 },
				{ (eSlipDamageType)8, (eAbnormalState)60 }, { (eSlipDamageType)9, (eAbnormalState)69 },
				{ (eSlipDamageType)11, (eAbnormalState)80 }
			};
		}

		[global::OpenPCR.Migration("c49f3c0f-900b-4ffc-b9b2-93a710058ce8")]
		public SlipDamageAction() : base()
		{
		}

		[global::OpenPCR.Migration("ae7bbe70-68a7-46ca-8a9b-6a8a6ec8430d")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("65f280f4-e973-4902-88f4-025a74993831")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (ActionDetail3 != 0)
				EnergyChargeMultiple = (double)ActionDetail3 / -100.0 + 1.0;
			float roll = BattleManager.Random(0f, 1f);
			var owner = _target.Owner;
			bool cancel = ActionDetail2 == 2 && BattleUtil.LessThanOrApproximately(
				_valueDictionary[eValueNumber.VALUE_7] / 100.0,
				(double)(long)owner.ComponentParameter.Hp / (double)(long)owner.ComponentParameter.MaxHp);
			float chance = BattleUtil.CalcDodgeByLevelDiffUseParam(_skill.Level, _source, _target, IsExEquipSkillAction, 1f);
			bool forceHit = _target.Owner.ComponentAbnormal.IsForceHit(eActionType.SLIP_DAMAGE, ActionDetail1);
			if (cancel || (roll >= chance && !forceHit))
			{
				var executed = AlreadyExecedData[_target.Owner][_num];
				if (executed.ExecedPartsNumber == executed.TargetPartsNumber)
					_target.Owner.ComponentDamageControl.SetMissAtk(_source, eMissLogType.DODGE_SLIP_DAMAGE, eDamageEffectType.NORMAL,
						executed.TargetPartsNumber == 1 ? _target : null, 1f);
			}
			else
			{
				AppendIsAlreadyExeced(owner, _num);
				var state = AbnormalStateDic[(eSlipDamageType)ActionDetail1];
				var abnormal = owner.ComponentAbnormal;
				abnormal.IsReleaseSlipDamageDic[state] = null;
				if (ActionDetail2 == 2)
					abnormal.IsReleaseSlipDamageDic[state] = () => BattleUtil.LessThanOrApproximately(
						_valueDictionary[eValueNumber.VALUE_7] / 100.0,
						(double)(long)owner.ComponentParameter.Hp / (double)(long)owner.ComponentParameter.MaxHp);
				double extend = abnormal.GetSlipDamageExtendValue();
				float duration = AbnormalStateFieldAction != null ? 90f : (float)(extend * _valueDictionary[eValueNumber.VALUE_3]);
				abnormal.SetAbnormalState(_source, state, duration, this, _skill,
					(double)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]),
					_valueDictionary[eValueNumber.VALUE_5], false, false, 0.0,
					_valueDictionary[eValueNumber.VALUE_6] == 0.0, IsEnvironmentAbnormalState, false);
			}
		}

		[global::OpenPCR.Migration("05c6d096-b52f-4c00-8051-31edc0c16ad0")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
			Value[eValueNumber.VALUE_5] = MasterData.action_value_5;
		}
	}
}
