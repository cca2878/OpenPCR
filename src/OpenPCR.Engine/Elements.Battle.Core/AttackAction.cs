using global::Elements;
using Elements.Battle;
using System;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class AttackAction : AttackActionBase
	{
		private enum eDecideTargetAtkType
		{
			SELECT_ATTACK_TYPE,
			TARGET_LOWER_DEF_TYPE
		}
		[global::OpenPCR.Migration("1d11d7ff-40d9-426b-99ef-17be2e8f0be6")]
		public AttackAction() : base()
		{
		}

		[global::OpenPCR.Migration("d1d79af5-1413-4962-b8a5-32f49ca13f51")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			if (ActionDetail3 != 0)
				EnergyChargeMultiple = (double)ActionDetail3 / -100.0 + 1.0;
			_target.IncrementUbAttackHitCount();
			if (_num == 0)
			{
				var abnormal = _target.Owner.ComponentAbnormal;
				if (abnormal.IsAbnormalState(eAbnormalState.DAMAGE_LIMIT_ATK))
					LimitDamageDictionaryAtk[_target] = BattleUtil.DoubleToLong(abnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.DAMAGE_LIMIT_ATK));
				// Both damage-limit conditions update the ATK dictionary.
				if (abnormal.IsAbnormalState(eAbnormalState.DAMAGE_LIMIT_MGC))
					LimitDamageDictionaryAtk[_target] = BattleUtil.DoubleToLong(abnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.DAMAGE_LIMIT_MGC));
				if (abnormal.IsAbnormalState(eAbnormalState.DAMAGE_LIMIT_ALL))
				{
					double all = abnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.DAMAGE_LIMIT_ALL);
					if (LimitDamageDictionaryAtk.ContainsKey(_target))
						LimitDamageDictionaryAtk[_target] = BattleUtil.DoubleToLong(Math.Min(all, (double)LimitDamageDictionaryAtk[_target]));
					else
						LimitDamageDictionaryAtk.Add(_target, BattleUtil.DoubleToLong(all));
					if (LimitDamageDictionaryMgc.ContainsKey(_target))
						LimitDamageDictionaryMgc[_target] = BattleUtil.DoubleToLong(Math.Min(all, (double)LimitDamageDictionaryMgc[_target]));
					else
						// Native also adds to ATK when MGC has no entry.
						LimitDamageDictionaryAtk.Add(_target, BattleUtil.DoubleToLong(all));
				}
			}

			eAttackType attackType = (eAttackType)ActionDetail1;
			if (!HitOnceDic.ContainsKey(_target))
			{
				HitOnceDic.Add(_target, false);
				HitOnceKeyList.Add(_target);
			}

			bool miss = judgeMiss(_target, _source, _num, _skill, attackType);
			if (miss && BattleUtil.Approximately(battleManager.DodgeTPRecoverRatio, 0.0))
				return;
			double criticalValue = _valueDictionary[eValueNumber.VALUE_5];
			bool critical = criticalValue < 0.0 || BattleUtil.Approximately(criticalValue, (double)(_num + 1));
			bool physical = judgeIsPhysical(attackType);
			if (ActionDetail2 == (int)eDecideTargetAtkType.TARGET_LOWER_DEF_TYPE)
			{
				long def = _target.GetDefZero();
				long magicDef = _target.GetMagicDefZero();
				if (def != magicDef)
					physical = def < magicDef;
			}

			var damageControl = _target.Owner.ComponentDamageControl;
			DamageData damage = createDamageData(_source, _target, _num, _valueDictionary, attackType, critical, _skill, eActionType.ATTACK, physical);
			if (!TotalDamageDictionary.ContainsKey(_target))
			{
				long total = 0L;
				var list = new List<CriticalData>();
				for (int i = 0; i < ActionExecTimeList.Count; i++)
				{
					var data = new CriticalData();
					float roll = BattleManager.Random(0f, 1f);
					long expected = BattleUtil.DoubleToLong(ActionExecTimeList[i].Weight * (double)damage.TotalDamageForLogBarrier / ActionWeightSum);
					data.ExpectedDamageForEnergyCalc = expected;
					data.ExpectedDamage = expected;
					data.ExpectedDamageNotCritical = expected;
					if ((BattleUtil.LessThanOrApproximately(roll, damage.CriticalRateForLogBarrier) && !BattleUtil.Approximately(damage.CriticalDamageRate, 0.0) && !BattleUtil.Approximately(damage.CriticalRateForLogBarrier, 0f)) || criticalValue < 0.0 || (criticalValue != double.PositiveInfinity && unchecked(global::OpenPCR.NativeNumeric.ToInt32(criticalValue) - 1) == i))
					{
						data.IsCritical = true;
						data.ExpectedDamage = BattleUtil.DoubleToLong(damage.CriticalDamageRate * ((double)data.ExpectedDamage + (double)data.ExpectedDamage));
						data.ExpectedDamageForEnergyCalc = BattleUtil.DoubleToLong(damage.CriticalDamageRate * ((double)data.ExpectedDamageForEnergyCalc + (double)data.ExpectedDamageForEnergyCalc));
					}

					if (!damage.IgnoreDef && damage.DamageType != DamageData.eDamageType.NONE)
					{
						var damageTarget = damage.Target;
						long def = damageTarget.GetDefZero();
						long magicDef = damageTarget.GetMagicDefZero();
						var higher = damageTarget.Owner.ComponentAbnormal.ReduceDamageByHigherDefData;
						bool? usePhysical = null;
						if (higher != null && (double)def != (double)magicDef && higher.Enable)
							usePhysical = (double)def > (double)magicDef;
						else if (damage.DamageType == DamageData.eDamageType.ATK)
							usePhysical = true;
						else if (damage.DamageType == DamageData.eDamageType.MGC)
							usePhysical = false;
						if (usePhysical.HasValue)
						{
							long normalDef = usePhysical.Value ? damageTarget.GetDefZero() : damageTarget.GetMagicDefZero();
							long energyDef = usePhysical.Value ? damageTarget.GetDefZeroForDamagedEnergy() : damageTarget.GetMagicDefZeroForDamagedEnergy();
							double afterPenetrate = Math.Max(0.0, (double)normalDef - (double)damage.DefPenetrate);
							double afterEnergyPenetrate = Math.Max(0.0, (double)energyDef - (double)damage.DefPenetrate);
							double rate = 1.0 - afterPenetrate / ((double)normalDef + 100.0);
							data.ExpectedDamage = BattleUtil.DoubleToLong(rate * (double)data.ExpectedDamage);
							data.ExpectedDamageNotCritical = BattleUtil.DoubleToLong(rate * (double)data.ExpectedDamageNotCritical);
							data.ExpectedDamageForEnergyCalc = BattleUtil.DoubleToLong((1.0 - afterEnergyPenetrate / ((double)energyDef + 100.0)) * (double)data.ExpectedDamageForEnergyCalc);
						}
					}

					double talent = damageControl.CalcTalentDamageRate(eActionType.ATTACK, _source, _target, damage.TargetIncludeMyTeam, false, out bool noDamage);
					data.ExpectedDamage = BattleUtil.DoubleToLong(talent * (double)data.ExpectedDamage);
					data.ExpectedDamageNotCritical = BattleUtil.DoubleToLong(talent * (double)data.ExpectedDamageNotCritical);
					data.ExpectedDamageForEnergyCalc = BattleUtil.DoubleToLong(talent * (double)data.ExpectedDamageForEnergyCalc);
					data.ExpectedIsNoDamageByResist = noDamage;
					data.CriticalRate = damage.CriticalDamageRate;
					total += data.ExpectedDamage;
					list.Add(data);
				}

				CriticalDataDictionary.Add(_target, list);
				TotalDamageDictionary.Add(_target, total);
			}

			var current = CriticalDataDictionary[_target][_num];
			damage.IsLogBarrierCritical = current.IsCritical;
			damage.LogBarrierExpectedDamage = current.ExpectedDamage;
			damage.LogBarrierExpectedDamageForEnergyCalc = current.ExpectedDamageForEnergyCalc;
			damage.LogBarrierExpectedIsNoDamageByResist = current.ExpectedIsNoDamageByResist;
			damage.TotalDamageForLogBarrier = TotalDamageDictionary[_target];
			if (miss)
			{
				damageControl.RecoverDodgeTP(damage.DamageType, damage.Damage, damage.ActionType, damage.TargetIncludeMyTeam, current.ExpectedDamageNotCritical, damage.TotalDamageForLogBarrier, damage.LogBarrierExpectedIsNoDamageByResist, damage.Source, damage.IgnoreDef, damage.Target, damage.DefPenetrate, null, _skill, EnergyChargeMultiple, IgnoreTriggerCausedByTrigger, this);
				return;
			}

			damage.IgnoreStrikeBackAndAllUnitDamageCutTrigger = IgnoreTriggerCausedByTrigger;
			if (damageControl.SetDamage(damage, true, ActionId, OnDamageHit, true, _skill, true, OnDefeatEnemy, false, ActionExecTimeList[_num].Weight, ActionWeightSum, null, EnergyChargeMultiple, UsedChargeEnergyByReceiveDamage) != 0L)
				HitOnceDic[_target] = true;
			if (_skill.AnimId == eAnimKitCharacterAnimeId.ATTACK)
			{
				if (ProtectedParts != null)
				{
					ProtectedParts.ShowHitEffect(_source.ComponentAbnormal.ToadDatas.Count > 0 ? (SystemIdDefine.eWeaponSeType)_skill.WeaponType : _source.WeaponSeType, _skill, _source.IsLeftDir);
					ProtectedParts = null;
				}
				else
					_target.ShowHitEffect(_source.ComponentAbnormal.ToadDatas.Count > 0 ? (SystemIdDefine.eWeaponSeType)_skill.WeaponType : _source.WeaponSeType, _skill, _source.IsLeftDir);
			}
		}

		[global::OpenPCR.Migration("c651d81b-0262-4865-bfbb-5dbaceef1996")]
		protected override double getCriticalDamageRate(Dictionary<eValueNumber, double> _valueDictionary)
		{
			if (BattleUtil.Approximately(_valueDictionary[eValueNumber.VALUE_6], 0.0))
				return 1.0;
			return _valueDictionary[eValueNumber.VALUE_6];
		}

		[global::OpenPCR.Migration("2f6d295f-a6fe-48ea-bc3f-63471da9c03f")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}

		[global::OpenPCR.Migration("a72a8bf7-4623-49a9-9f24-25ec68386643")]
		protected override DamageData createDamageData(UnitCtrl _source, BasePartsData _target, int _num, Dictionary<eValueNumber, double> _valueDictionary, eAttackType _actionDetail1, bool _isCritical, Skill _skill, eActionType _actionTypeForSource, bool _isPhysicalForTarget)
		{
			DamageData damageData = base.createDamageData(_source, _target, _num, _valueDictionary, _actionDetail1, _isCritical, _skill, _actionTypeForSource, _isPhysicalForTarget);
			long penetration = BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_7]);
			if (penetration > 0L)
				damageData.DefPenetrate += penetration;
			return damageData;
		}
	}
}
