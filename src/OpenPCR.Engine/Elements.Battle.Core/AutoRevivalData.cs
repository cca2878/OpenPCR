using System;
using global::OpenPCR.Foundation;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AutoRevivalData
	{
		public enum eValueElementType
		{
			PHYSICS = 1,
			MAGIC
		}

		public enum eValueType
		{
			NORMAL = 1,
			PERCENT
		}

		private const int ENCHANT_UNBEATABLE_VALUE = 99;
		private const int NO_DISP_NUM = 1;
		private BasePartsData partsData;
		private eValueElementType valueElementType = eValueElementType.PHYSICS;
		private double value;
		private double parameterRate;
		private int counter;
		private double timer;
		private eStateIconType iconType;
		private eValueType valueType = eValueType.NORMAL;
		private eEffectType effectType;
		private IBattleManagerForActionParameter battleManager;
		[global::OpenPCR.Migration("a68d2d40-f55b-40ba-887f-c0a67c7a576d")]
		public AutoRevivalData(BasePartsData _partsData, int _detail1, int _detail3, Dictionary<eValueNumber, double> _valueDictionary, UnitCtrl _owner, eStateIconType _iconType, eEffectType _effectType, Skill _skill, IBattleManagerForActionParameter _battleManager)
		{
			partsData = _partsData;
			owner = _owner;
			valueElementType = (eValueElementType)_detail1;
			valueType = (eValueType)global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]);
			value = _valueDictionary[eValueNumber.VALUE_2];
			parameterRate = _valueDictionary[eValueNumber.VALUE_4];
			counter = global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_6]);
			timer = _valueDictionary[eValueNumber.VALUE_5];
			iconType = _iconType;
			effectType = _effectType;
			isEnchantUnbeatable = _detail3 == ENCHANT_UNBEATABLE_VALUE;
			if (_iconType != eStateIconType.NONE)
			{
				_owner.ComponentAbnormal.OnChangeState.Call(_iconType, true);
				if (counter != NO_DISP_NUM)
					_owner.ComponentAbnormal.OnChangeStateNum(iconType, counter);
			}

			_owner.AppendCoroutine(updateTimer(_owner), ePauseType.SYSTEM, null);
			skill = _skill;
			battleManager = _battleManager;
		}

		[global::OpenPCR.Migration("229e5e1e-d914-4327-b18e-6ef1c21e85cc")]
		private IEnumerator updateTimer(UnitCtrl _target)
		{
			while (!(timer <= 0.0) && counter > 0 && !_target.ComponentState.IdleOnly && !_target.ComponentState.IsDead)
			{
				timer -= (double)_target.DeltaTimeForPause;
				yield return null;
			}

			if (counter > 0)
				_target.ComponentAbnormal.OnChangeState(iconType, false);
			counter = 0;
			timer = 0.0;
		}

		[global::OpenPCR.Migration("ffa73c9e-600c-4c10-92b8-45f77c180011")]
		public bool TryExec()
		{
			if (counter == 0 || timer <= 0.0)
				return false;
			counter = unchecked(counter - 1);
			if (counter == 0)
				owner.ComponentAbnormal.OnChangeState(iconType, false);
			else
				owner.ComponentAbnormal.OnChangeStateNum(iconType, counter);
			cureUnableAbnormalState();
			applyHeal();
			if (isEnchantUnbeatable)
				owner.ComponentAbnormal.IsUnbeatableForAutoRevival = true;
			execSkill();
			return true;
		}

		[global::OpenPCR.Migration("d355a60a-86d2-49a0-85a0-3d35678a0e0c")]
		private void cureUnableAbnormalState()
		{
			foreach (eAbnormalState state in Enum.GetValues(typeof(eAbnormalState)))
			{
				if (state == eAbnormalState.NONE)
					continue;
				if (owner.ComponentAbnormal.IsAbnormalState(state) && judgeReleaseTargetState(state))
					owner.ComponentAbnormal.EnableAbnormalState(state, false, false, false, true);
			}

			if (owner.ComponentAbnormal.ToadDatas.Count > 0)
				owner.ComponentAbnormal.ToadDatas[0].Enable = false;
		}

		[global::OpenPCR.Migration("d864fbf2-5699-41f0-960b-6bc3c28713ea")]
		private bool judgeReleaseTargetState(eAbnormalState _state)
		{
			if (!AbnormalStateUtility.IsUnableAbnormalState(_state))
				return _state == eAbnormalState.SILENCE;
			return owner.ComponentAbnormal.AbnormalStateCategoryDataDictionary[AbnormalStateUtility.GetAbnormalStateCategory(_state)].ActionType != eActionType.FORCE_CHANGE_SPEED;
		}

		[global::OpenPCR.Migration("08da0ea8-d480-4901-9db9-54ec0a8047a7")]
		private void applyHeal()
		{
			UnitCtrl unit = owner;
			eValueType type = valueType;
			eValueElementType element = valueElementType;
			long result;
			if (type == eValueType.PERCENT)
			{
				result = BattleUtil.DoubleToLong(value * (double)(long)unit.ComponentParameter.MaxHp / 100.0);
			}
			else if (type == eValueType.NORMAL)
			{
				UnitComponentParameterClamped clamped = unit.ComponentParameterClamped;
				long stat;
				if (unit.IsPartsBoss)
					stat = element == eValueElementType.PHYSICS ? partsData.GetAtkZero() : partsData.GetMagicStrZero();
				else
					stat = element == eValueElementType.PHYSICS ? (long)clamped.AtkZero : (long)clamped.MagicStrZero;
				result = BattleUtil.DoubleToLong(value + parameterRate * (double)stat);
			}
			else
				result = 0;
			UnitCtrl target = owner;
			UnitComponentDamageControl damageControl = target.ComponentDamageControl;
			double healDown = target.ComponentAbnormal.GetHealDownValue();
			damageControl.SetRecovery(result, 0, (eInhibitHealType)(element != eValueElementType.PHYSICS ? 1 : 0), target, false, null, healDown, effectType == eEffectType.COMMON, true, false, false, true, null, true);
		}

		[global::OpenPCR.Migration("f38dc69e-8a47-48bc-9623-bf9426995508")]
		private void execSkill()
		{
			if (battleManager.ChargeSkillTurn != eChargeSkillTurn.NONE)
				owner.AppendCoroutine(waitChargeSkillTurnNone(), ePauseType.SYSTEM, owner);
			else
				trySkill();
		}

		[global::OpenPCR.Migration("543c2ec6-234c-4528-b7bd-ede895fa4838")]
		private void trySkill()
		{
			global::Elements.UnitCtrl nativeValue1 = (this).owner;
			if (global::System.Object.ReferenceEquals(nativeValue1, null))
				throw new global::System.NullReferenceException();
			global::Elements.Battle.Core.UnitComponentAbnormal nativeValue2 = (nativeValue1).ComponentAbnormal;
			if (global::System.Object.ReferenceEquals(nativeValue2, null))
				throw new global::System.NullReferenceException();
			int nativeValue3 = (nativeValue2).KnockBackEnableCount;
			if ((unchecked((int)(unchecked((uint)(nativeValue3)))) < unchecked((int)(1u))))
			{
				(this).execSkillImpl();
				return;
			}
			else
			{
				global::System.Collections.IEnumerator nativeResult2 = (this).waitKnockBackEnd();
				global::Elements.UnitCtrl nativeValue4 = (this).owner;
				(nativeValue1).AppendCoroutine((global::System.Collections.IEnumerator)(nativeResult2), unchecked((global::Elements.ePauseType)(1u)), (global::Elements.UnitCtrl)(nativeValue4));
				return;
			}
		}

		[global::OpenPCR.Migration("1b3ddfe8-7a7e-4005-9313-15818d4d245b")]
		private void execSkillImpl()
		{
			global::Elements.UnitCtrl nativeValue1 = (this).owner;
			if (global::System.Object.ReferenceEquals(nativeValue1, null))
				throw new global::System.NullReferenceException();
			global::Elements.Battle.Core.UnitComponentState nativeValue2 = (nativeValue1).ComponentState;
			if (global::System.Object.ReferenceEquals(nativeValue2, null))
				throw new global::System.NullReferenceException();
			bool nativeValue3 = (nativeValue2).IsDead;
			if (!(((nativeValue3 ? 1u : 0u) == 0)))
			{
				return;
			}
			else
			{
				global::Elements.Battle.Core.UnitComponentAbnormal nativeValue4 = (nativeValue1).ComponentAbnormal;
				if (global::System.Object.ReferenceEquals(nativeValue4, null))
					throw new global::System.NullReferenceException();
				bool nativeResult1 = (nativeValue4).IsUnableActionState();
				if (((nativeResult1 ? 1u : 0u) & 1u) != 0)
				{
					return;
				}
				else
				{
					global::Elements.UnitCtrl nativeValue5 = (this).owner;
					if (global::System.Object.ReferenceEquals(nativeValue5, null))
						throw new global::System.NullReferenceException();
					global::Elements.Battle.Core.UnitComponentState nativeValue6 = (nativeValue5).ComponentState;
					if (global::System.Object.ReferenceEquals(nativeValue6, null))
						throw new global::System.NullReferenceException();
					global::Elements.Skill nativeValue7 = (this).skill;
					(nativeValue6).CancelByAwake = true;
					if (global::System.Object.ReferenceEquals(nativeValue7, null))
						throw new global::System.NullReferenceException();
					global::Elements.Battle.Core.ActionState nativeValue9 = (nativeValue6).CurrentState;
					int nativeValue10 = (nativeValue7).SkillId;
					(nativeValue6).CurrentTriggerSkillId = unchecked((int)(unchecked((uint)(nativeValue10))));
					if (!((unchecked((uint)(nativeValue9)) == 5u)))
					{
						global::Elements.Skill nativeValue12 = (this).skill;
						if (global::System.Object.ReferenceEquals(nativeValue12, null))
							throw new global::System.NullReferenceException();
						global::Elements.Battle.Core.UnitComponentState nativeValue13 = (nativeValue5).ComponentState;
						if (global::System.Object.ReferenceEquals(nativeValue13, null))
							throw new global::System.NullReferenceException();
						int nativeValue14 = (nativeValue12).SkillId;
						(nativeValue13).SetState(unchecked((global::Elements.Battle.Core.ActionState)(3u)), unchecked((int)(0u)), unchecked((int)(unchecked((uint)(nativeValue14)))), false);
						bool nativeValue15 = (this).isEnchantUnbeatable;
						if (((nativeValue15 ? 1u : 0u) == 0))
						{
							return;
						}
						else
						{
							global::Elements.UnitCtrl nativeValue16 = (this).owner;
							global::System.Collections.IEnumerator nativeResult3 = (this).waitSkillEndAndReleaseUnbeatable();
							if (global::System.Object.ReferenceEquals(nativeValue16, null))
								throw new global::System.NullReferenceException();
							global::Elements.UnitCtrl nativeValue17 = (this).owner;
							(nativeValue16).AppendCoroutine((global::System.Collections.IEnumerator)(nativeResult3), unchecked((global::Elements.ePauseType)(1u)), (global::Elements.UnitCtrl)(nativeValue17));
							return;
						}
					}
					else
					{
						(nativeValue6).StopUpdateDamage();
						global::Elements.UnitCtrl nativeValue18 = (this).owner;
						if (global::System.Object.ReferenceEquals(nativeValue18, null))
							throw new global::System.NullReferenceException();
						global::Elements.Skill nativeValue19 = (this).skill;
						if (global::System.Object.ReferenceEquals(nativeValue19, null))
							throw new global::System.NullReferenceException();
						global::Elements.Battle.Core.UnitComponentState nativeValue20 = (nativeValue18).ComponentState;
						if (global::System.Object.ReferenceEquals(nativeValue20, null))
							throw new global::System.NullReferenceException();
						int nativeValue21 = (nativeValue19).SkillId;
						(nativeValue20).SetState(unchecked((global::Elements.Battle.Core.ActionState)(3u)), unchecked((int)(0u)), unchecked((int)(unchecked((uint)(nativeValue21)))), false);
						bool nativeValue22 = (this).isEnchantUnbeatable;
						if (((nativeValue22 ? 1u : 0u) == 0))
						{
							return;
						}
						else
						{
							global::Elements.UnitCtrl nativeValue23 = (this).owner;
							global::System.Collections.IEnumerator nativeResult7 = (this).waitSkillEndAndReleaseUnbeatable();
							if (global::System.Object.ReferenceEquals(nativeValue23, null))
								throw new global::System.NullReferenceException();
							global::Elements.UnitCtrl nativeValue24 = (this).owner;
							(nativeValue23).AppendCoroutine((global::System.Collections.IEnumerator)(nativeResult7), unchecked((global::Elements.ePauseType)(1u)), (global::Elements.UnitCtrl)(nativeValue24));
							return;
						}
					}
				}
			}
		}

		[global::OpenPCR.Migration("99265219-3b17-46a1-ac6e-db79c189723f")]
		private IEnumerator waitChargeSkillTurnNone()
		{
			do
			{
				yield return null;
			}
			while (battleManager.ChargeSkillTurn != eChargeSkillTurn.NONE);
			trySkill();
		}

		[global::OpenPCR.Migration("2e417155-81b7-4422-b1d1-a739fb466f4b")]
		private IEnumerator waitKnockBackEnd()
		{
			while (owner.ComponentAbnormal.KnockBackEnableCount > 0)
				yield return null;
			if (!owner.ComponentState.IsDead)
				execSkillImpl();
		}

		[global::OpenPCR.Migration("fbafda3c-bde0-4fb9-b52e-826010f31133")]
		private IEnumerator waitSkillEndAndReleaseUnbeatable()
		{
			while (owner.ComponentState.CurrentState == ActionState.SKILL && !owner.ComponentState.IdleOnly && !owner.ComponentState.IsDead)
				yield return null;
			owner.ComponentAbnormal.IsUnbeatableForAutoRevival = false;
			battleManager.UpdateAbnormalStateField();
		}

		private global::Elements.UnitCtrl owner;
		private global::Elements.Skill skill;
		private bool isEnchantUnbeatable;
	}
}
