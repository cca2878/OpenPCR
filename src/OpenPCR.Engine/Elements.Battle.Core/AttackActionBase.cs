using System;
using global::OpenPCR.Foundation;
using Elements.Battle;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AttackActionBase : ActionParameter
	{
		public enum eCriticalReference
		{
			NORMAL,
			PHYSICAL,
			MAGIC,
			SUM
		}

		public eCriticalReference CriticalReference { get; set; }
		public long AdditionalCritical { get; set; }

		public enum eAttackType
		{
			PHYSICAL = 1,
			MAGIC,
			INEVITABLE_PHYSICAL
		}

		private BasePartsData parts;
		public bool UseCopyAtkParam;
		public long CopyAtk;
		[global::OpenPCR.Migration("94c2d300-ba53-4835-94fa-3e50adc59070")]
		public AttackActionBase() : base()
		{
		}

		[global::OpenPCR.Migration("c2b4a01b-5b82-484e-8840-7ebc0d5f922a")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			for (int i = 0; i < base.HitOnceKeyList.Count; i++)
			{
				base.HitOnceDic[base.HitOnceKeyList[i]] = false;
			}

			base.CriticalDataDictionary.Clear();
			base.TotalDamageDictionary.Clear();
		}

		[global::OpenPCR.Migration("4a277c88-9388-4728-ae35-35b8bdb64410")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget);
		}

		[global::OpenPCR.Migration("ae311731-557b-4790-9d7e-cf543c4ca9a3")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
		}

		[global::OpenPCR.Migration("e1d1ee36-0fdc-4bd7-ac2e-748a05579be4")]
		protected bool judgeMiss(BasePartsData _target, UnitCtrl _source, int _num, Skill _skill, eAttackType _actionDetail1)
		{
			float num = BattleManager.Random(0f, 1f);
			long num2 = 0;
			num2 = (_source.IsPartsBoss ? parts.GetAccuracyZero() : ((long)_source.ComponentParameterClamped.AccuracyZero));
			if (_skill.CountBlind && _skill.CountBlindType == _actionDetail1)
			{
				_target.Owner.ComponentDamageControl.OnDodge.Call();
				_target.SetMissAtk(_source, eMissLogType.DODGE_ATTACK_DARK, base.ActionExecTimeList[_num].DamageNumType, base.ActionExecTimeList[_num].DamageNumScale);
				return true;
			}

			var abnormal = _source.ComponentAbnormal;
			if (abnormal.IsAbnormalState(eAbnormalState.COUNT_BLIND))
			{
				_skill.CountBlindType = (eAttackType)global::OpenPCR.NativeNumeric.ToInt32(abnormal.GetAbnormalStateSubValue(eAbnormalStateCategory.COUNT_BLIND));
				if (_skill.CountBlindType == _actionDetail1)
				{
					_target.Owner.ComponentDamageControl.OnDodge.Call();
					_skill.CountBlind = true;
					abnormal.DecreaseCountBlind();
					_target.SetMissAtk(_source, eMissLogType.DODGE_ATTACK_DARK, base.ActionExecTimeList[_num].DamageNumType, base.ActionExecTimeList[_num].DamageNumScale);
					return true;
				}
			}

			if (num < _target.GetDodgeRate(num2) && _actionDetail1 == eAttackType.PHYSICAL)
			{
				_target.SetMissAtk(_source, eMissLogType.DODGE_ATTACK, base.ActionExecTimeList[_num].DamageNumType, base.ActionExecTimeList[_num].DamageNumScale);
				_target.Owner.ComponentDamageControl.OnDodge.Call();
				return true;
			}

			if (!judgeDarkMiss(eAbnormalState.PHYSICS_DARK, _actionDetail1, _target, _source, _num))
			{
				return judgeDarkMiss(eAbnormalState.MAGIC_DARK, _actionDetail1, _target, _source, _num);
			}

			return true;
		}

		[global::OpenPCR.Migration("fd60056d-c6ab-448f-837e-020ba87ae5c1")]
		private bool judgeDarkMiss(eAbnormalState _abnormalState, eAttackType _attackType, BasePartsData _target, UnitCtrl _source, int _num)
		{
			UnitComponentAbnormal abnormal = _source.ComponentAbnormal;
			if (!abnormal.IsAbnormalState(_abnormalState))
				return false;
			eAttackType darkType = _abnormalState == eAbnormalState.PHYSICS_DARK ? eAttackType.PHYSICAL : eAttackType.MAGIC;
			if (darkType != _attackType)
				return false;
			eAbnormalStateCategory category = _abnormalState == eAbnormalState.PHYSICS_DARK ? eAbnormalStateCategory.PHYSICAL_DARK : eAbnormalStateCategory.MAGIC_DARK;
			float roll = Elements.Battle.BattleManager.Random(0f, 1f);
			if (abnormal.GetAbnormalStateMainValue(category) / -100.0 + 1.0 <= roll)
				return false;
			ActionExecTime timing = ActionExecTimeList[_num];
			_target.SetMissAtk(_source, eMissLogType.DODGE_ATTACK_DARK, timing.DamageNumType, timing.DamageNumScale);
			_target.Owner.ComponentDamageControl.OnDodge.Call();
			return true;
		}

		[global::OpenPCR.Migration("a67ed5b2-8e66-4f28-ad93-98c6dbf538df")]
		protected virtual DamageData createDamageData(UnitCtrl _source, BasePartsData _target, int _num, Dictionary<eValueNumber, double> _valueDictionary, eAttackType _actionDetail1, bool _isCritical, Skill _skill, eActionType _actionTypeForSource, bool _isPhysicalForTarget)
		{
			bool physical = judgeIsPhysical(_actionDetail1);
			bool everyCritical = _source.ComponentAbnormal.JudgeEveryAttackCritical(physical, TargetAssignment == eTargetAssignment.OWNER_SITE);
			var clamped = _source.ComponentParameterClamped;
			var parameter = _source.ComponentParameter;
			long attack;
			long critical = 0L;
			int level;
			if (!_source.IsPartsBoss)
			{
				attack = physical ? (long)clamped.AtkZero : (long)clamped.MagicStrZero;
				switch (CriticalReference)
				{
					case eCriticalReference.NORMAL:
						critical = physical ? (long)clamped.PhysicalCriticalZero : (long)clamped.MagicCriticalZero;
						break;
					case eCriticalReference.PHYSICAL:
						critical = unchecked((long)clamped.PhysicalCriticalZero + AdditionalCritical);
						break;
					case eCriticalReference.MAGIC:
						critical = unchecked((long)clamped.MagicCriticalZero + AdditionalCritical);
						break;
					case eCriticalReference.SUM:
						critical = unchecked((long)clamped.PhysicalCriticalZero + (long)clamped.MagicCriticalZero + AdditionalCritical);
						break;
				}

				level = parameter.Level;
			}
			else
			{
				attack = physical ? parts.GetAtkZero() : parts.GetMagicStrZero();
				switch (CriticalReference)
				{
					case eCriticalReference.NORMAL:
						critical = physical ? parts.GetPhysicalCriticalZero() : parts.GetMagicCriticalZero();
						break;
					case eCriticalReference.PHYSICAL:
						critical = unchecked(parts.GetPhysicalCriticalZero() + AdditionalCritical);
						break;
					case eCriticalReference.MAGIC:
						critical = unchecked(parts.GetMagicCriticalZero() + AdditionalCritical);
						break;
					case eCriticalReference.SUM:
						critical = unchecked(parts.GetPhysicalCriticalZero() + parts.GetMagicCriticalZero() + AdditionalCritical);
						break;
				}

				level = parts.GetLevel();
			}

			if (UseCopyAtkParam)
				attack = CopyAtk;
			long totalDamage = calcTotalDamage(_source, _target, _num, _valueDictionary, attack);
			long hitDamage = calcDamage(_source, _target, _num, _valueDictionary, attack);
			var targetClamped = _target.Owner.ComponentParameterClamped;
			double receiveRate = (_isPhysicalForTarget ? (double)targetClamped.PhysicalReceiveDamagePercentOrMin : (double)targetClamped.MagicReceiveDamagePercentOrMin) / 100.0;
			double damageUpRate = (_isPhysicalForTarget ? (double)clamped.PhysicalDamageUpPercentOrMin : (double)clamped.MagicDamageUpPercentOrMin) / 100.0;
			totalDamage = BattleUtil.DoubleToLong(receiveRate * (double)totalDamage * damageUpRate);
			hitDamage = BattleUtil.DoubleToLong(receiveRate * (double)hitDamage * damageUpRate);
			var abnormal = _target.Owner.ComponentAbnormal;
			if (abnormal.AccumulativeDamageDataDictionary.TryGetValue(_source, out var accumulated))
				accumulated.DamagedCount++;
			if (_source.IsOther != _target.Owner.IsOther)
			{
				var sourceDataDictionary = _source.ComponentAbnormal.AccumulativeDamageSourceDataDictionary;
				var allEnemyData = abnormal.AccumulativeDamageDataForAllEnenmyDictionary;
				foreach (var sourceData in sourceDataDictionary.Values)
				{
					if (!allEnemyData.TryGetValue(sourceData, out var data))
					{
						data = sourceData.CreateAccumulativeDamageData();
						allEnemyData[sourceData] = data;
					}

					data.DamagedCount++;
				}
			}

			double criticalDamageRate = getCriticalDamageRate(_valueDictionary);
			double criticalRate = (physical ? (double)clamped.PhysicalCriticalDamageRateOrMin : (double)clamped.MagicCriticalDamageRateOrMin) / 100.0;
			double receiveCriticalRate = (double)targetClamped.ReceiveCriticalDamageRateOrMin / 100.0;
			criticalDamageRate *= Math.Max(0.5, criticalRate * receiveCriticalRate);
			var damageData = new DamageData
			{
				TotalDamageForLogBarrier = totalDamage,
				Damage = hitDamage,
				Target = _target,
				Source = _source,
				DamageType = _isPhysicalForTarget ? DamageData.eDamageType.ATK : DamageData.eDamageType.MGC,
				CriticalRate = everyCritical || _isCritical ? 1f : BattleUtil.GetCriticalRate((float)critical, level, _target.GetLevel()),
				CriticalRateForLogBarrier = everyCritical ? 1f : BattleUtil.GetCriticalRate((float)critical, level, _target.GetLevel()),
				DamegeEffectType = ActionExecTimeList[_num].DamageNumType,
				DamegeNumScale = ActionExecTimeList[_num].DamageNumScale,
				DefPenetrate = _isPhysicalForTarget ? (long)_source.ComponentParameterClamped.PhysicalPenetrateZero : (long)_source.ComponentParameterClamped.MagicPenetrateZero,
				LifeSteal = _source.IsPartsBoss ? parts.GetLifeStealZero() : (long)_source.ComponentParameterClamped.LifeStealZero,
				CriticalDamageRate = criticalDamageRate,
				ActionType = _actionTypeForSource,
				TargetIncludeMyTeam = TargetAssignment != eTargetAssignment.OTHER_SIDE || IsEnvironmentAbnormalState,
				IsAlwaysChargeEnegry = IsAlwaysChargeEnegry
			};
			if (_isPhysicalForTarget)
			{
				if (LimitDamageDictionaryAtk.ContainsKey(_target))
					setupLimitDamageData(damageData, LimitDamageDictionaryAtk, _target);
			}
			else if (LimitDamageDictionaryMgc.ContainsKey(_target))
				setupLimitDamageData(damageData, LimitDamageDictionaryMgc, _target);
			return damageData;
		}

		[global::OpenPCR.Migration("73123911-9dc6-4653-a74b-577cf826d244")]
		private void setupLimitDamageData(DamageData _damageData, Dictionary<BasePartsData, long> _limitDamageDictionary, BasePartsData _target)
		{
			_damageData.LimitDamageFunc = damage =>
			{
				long limit = _limitDamageDictionary[_target];
				if (limit == 0L)
					return 0.0;
				if (limit > 0L && (double)limit < damage)
				{
					_limitDamageDictionary[_target] = 0L;
					return (double)limit;
				}

				_limitDamageDictionary[_target] = unchecked(_limitDamageDictionary[_target] - BattleUtil.DoubleToLong(damage));
				return damage;
			};
		}

		[global::OpenPCR.Migration("6c1594a9-c698-4905-9310-7a32c989f5e2")]
		private long calcTotalDamage(UnitCtrl _source, BasePartsData _target, int _num, Dictionary<eValueNumber, double> _valueDictionary, long _atk)
		{
			double fixedDamage = _valueDictionary[eValueNumber.VALUE_1];
			double coefficient = _valueDictionary[eValueNumber.VALUE_3];
			double debuffRate = _target.Owner.ComponentAbnormal.GetDebuffDamageUpValue();
			bool enemySide = _source.IsOther != _target.Owner.IsOther;
			var abnormal = _target.Owner.ComponentAbnormal;
			bool found = abnormal.AccumulativeDamageDataDictionary.TryGetValue(_source, out var data);
			double value = (fixedDamage + coefficient * (double)_atk) * debuffRate;
			if (!found && (!enemySide || _source.ComponentAbnormal.AccumulativeDamageSourceDataDictionary.Count == 0))
				return BattleUtil.DoubleToLong(value);
			long total = 0L;
			for (int i = 0; i < ActionExecTimeList.Count; i++)
			{
				long damage = BattleUtil.DoubleToLong(value * ActionExecTimeList[i].Weight / ActionWeightSum);
				long additional = data != null ? data.CalcAdditionalDamage(i - _num + data.DamagedCount, damage) : 0L;
				long allEnemyAdditional = 0L;
				if (enemySide)
				{
					foreach (var sourceData in _source.ComponentAbnormal.AccumulativeDamageSourceDataDictionary.Values)
					{
						var allEnemyData = _target.Owner.ComponentAbnormal.AccumulativeDamageDataForAllEnenmyDictionary;
						if (!allEnemyData.TryGetValue(sourceData, out var targetData))
						{
							targetData = sourceData.CreateAccumulativeDamageData();
							allEnemyData[sourceData] = targetData;
						}

						allEnemyAdditional = unchecked(allEnemyAdditional + targetData.CalcAdditionalDamage(i - _num + targetData.DamagedCount, damage));
					}
				}

				total = unchecked(total + damage + additional + allEnemyAdditional);
			}

			return total;
		}

		[global::OpenPCR.Migration("bb5dfbce-dedd-4283-b76f-842f5ae6bc0f")]
		private long calcDamage(UnitCtrl _source, BasePartsData _target, int _num, Dictionary<eValueNumber, double> _valueDictionary, long _atk)
		{
			double fixedDamage = _valueDictionary[eValueNumber.VALUE_1];
			double coefficient = _valueDictionary[eValueNumber.VALUE_3];
			long damage = BattleUtil.DoubleToLong((fixedDamage + coefficient * (double)_atk) * ActionExecTimeList[_num].Weight / ActionWeightSum);
			UnitComponentAbnormal abnormal = _target.Owner.ComponentAbnormal;
			long additional = 0L;
			if (abnormal.AccumulativeDamageDataDictionary.TryGetValue(_source, out AccumulativeDamageData data))
				additional = data.CalcAdditionalDamage(data.DamagedCount, damage);
			long allEnemyAdditional = 0L;
			if (_source.IsOther != _target.Owner.IsOther)
			{
				Dictionary<AccumulativeDamageSourceData, AccumulativeDamageData> allEnemyData = abnormal.AccumulativeDamageDataForAllEnenmyDictionary;
				foreach (AccumulativeDamageSourceData sourceData in _source.ComponentAbnormal.AccumulativeDamageSourceDataDictionary.Values)
				{
					if (allEnemyData.TryGetValue(sourceData, out AccumulativeDamageData targetData))
						allEnemyAdditional = unchecked(allEnemyAdditional + targetData.CalcAdditionalDamage(targetData.DamagedCount, damage));
				}
			}

			return unchecked(additional + damage + allEnemyAdditional);
		}

		[global::OpenPCR.Migration("40ffcfef-c84b-4e67-9130-11aa343e97a0")]
		protected virtual double getCriticalDamageRate(Dictionary<eValueNumber, double> _valueDictionary)
		{
			return 1f;
		}

		[global::OpenPCR.Migration("a4906087-06a0-467a-950c-cc07f994bf5a")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}

		[global::OpenPCR.Migration("decfe1a7-d704-4761-819e-4752ee44ef6a")]
		protected bool judgeIsPhysical(eAttackType _attackType)
		{
			if (_attackType != eAttackType.INEVITABLE_PHYSICAL)
			{
				return _attackType == eAttackType.PHYSICAL;
			}

			return true;
		}
	}
}
