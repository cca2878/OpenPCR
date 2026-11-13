using global::Elements;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using Elements.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public partial class UnitComponentDamageControl : UnitComponentBase, IUnitComponent
	{
		public Action OnActionByDamage { get; set; }
		public Action OnActionByCritical { get; set; }
		public Action OnActionByDamageOnce { get; set; }

		public Action<long> OnDamageForLoopRepeat { get; set; }

		public Dictionary<int, Action<bool>> OnDamageListForSpyDisableByAttack { get; set; } = new Dictionary<int, Action<bool>>();
		public Dictionary<int, Action<double>> OnRecoverListForChangeSpeedDisableByMaxHp { get; set; } = new Dictionary<int, Action<double>>();
		public List<(long triggerHp, bool isActionDone, int actionId, Skill skill)> DamagedTriggerHpAndActionDoneList { get; set; } = new List<(long triggerHp, bool isActionDone, int actionId, Skill skill)>();
		private float sortOffsetResetTimer = 0.6f;

		private Elements.Battle.IBattleLog battleLog { get; set; }
		private UnitComponentAbnormal componentAbnormal { get; set; }
		private UnitComponentColor componentColor { get; set; }
		private UnitComponentMultiPartsBoss componentMultiPartsBoss { get; set; }
		private UnitComponentParameter componentParameter { get; set; }
		private UnitComponentParameterClamped componentParameterClamped { get; set; }
		private UnitComponentSortOrder componentSortOrder { get; set; }
		private UnitComponentSound componentSound { get; set; }
		private UnitComponentState componentState { get; set; }


		public global::System.Action<System.Boolean, System.Int64, System.Boolean> OnDamageForLoopTrigger { get; set; }
		private int comboCount;
		private float lastHitTime;
		private int hitEffectSortOffset;
		private IBattleEffectPool battleEffectPool { get; set; }

		public Action<bool> OnDamageForSpecialSleepRelease { get; set; }

		public Dictionary<int, Action<bool>> OnDamageListForChangeSpeedDisableByAttack { get; set; }


		public Action OnDamageForUIShake { get; set; }

		public Action OnDodge { get; set; }

		public bool IsOnDamageCharge { get; set; }


		public UnitCtrl KillBonusTarget { get; private set; }

		public UnitDamageInfo UnitDamageInfo { get; set; }

		public Action<float> OnLifeAmmountChange { get; set; }

		public PartsMultiBossGauge PartsMultiBossGauge { get; set; }

		[global::OpenPCR.Migration("13f9cd5d-b558-460c-a032-d77085e12c69")]
		public UnitComponentDamageControl(UnitCtrl _unitController, BattleManager _battleManager, IBattleLog _battleLog, IBattleEffectPool _battleEffectPool) : base(_unitController)
		{
			battleManager = _battleManager;
			battleLog = _battleLog;
			battleEffectPool = _battleEffectPool;
			OnDamageListForChangeSpeedDisableByAttack = new Dictionary<int, Action<bool>>();
		}

		[global::OpenPCR.Migration("a4e993f8-61ca-47e6-ac5f-672ecbe82cc8")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAbnormal = _container.ComponentAbnormal;
			componentColor = _container.ComponentColor;
			componentMultiPartsBoss = _container.ComponentMultiPartsBoss;
			componentParameter = _container.ComponentParameter;
			componentParameterClamped = _container.ComponentParameterClamped;
			componentSortOrder = _container.ComponentSortOrder;
			componentSound = _container.ComponentSound;
			componentState = _container.ComponentState;
		}

		[global::OpenPCR.Migration("2c3f86e1-42a0-4423-bfb7-5eb789141a66")]
		public void RecoverDodgeTP(DamageData.eDamageType _damageType, long _damage, eActionType _actionType, bool _targetIncludeMyTeam, long _logBarrierExpectedDamageForEnergyCalc, long _totalDamageForLogBarrier, bool _logBarrierExpectedNoDamageByResist, UnitCtrl _source, bool _ignoreDef, BasePartsData _target, long _defPenetrate, Func<long, double, long> _upperLimitFunc, Skill _skill, double _energyChargeMultiple, bool _ignoreStrikeBackAndAllUnitDamageCutTrigger, ActionParameter _sourceAction)
		{
			if (componentState.IdleOnly || unitController.IsDivisionSourceForDamage || componentAbnormal.IsNoDamageMotion()) return;
			bool physicalDodge = componentAbnormal.IsAbnormalState(eAbnormalState.PHYSICS_DODGE);
			if (_damageType == DamageData.eDamageType.ATK && physicalDodge) return;
			bool magicDodge = componentAbnormal.IsAbnormalState(eAbnormalState.MAGIC_DODGE);
			if (_damageType == DamageData.eDamageType.MGC && magicDodge) return;
			double damage = _damage;
			if (componentAbnormal.HasDebuffDamageUpData()) damage *= componentAbnormal.GetDebuffDamageUpValue();
			if (_actionType == eActionType.ATTACK) damage = _logBarrierExpectedDamageForEnergyCalc;
			if (_damageType == DamageData.eDamageType.ATK && componentAbnormal.IsAbnormalState(eAbnormalState.CUT_ATK_DAMAGE))
				damage *= 1d - componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.CUT_ATK_DAMAGE) / 100d;
			else if (_damageType == DamageData.eDamageType.MGC && componentAbnormal.IsAbnormalState(eAbnormalState.CUT_MGC_DAMAGE))
				damage *= 1d - componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.CUT_MGC_DAMAGE) / 100d;
			if (componentAbnormal.IsAbnormalState(eAbnormalState.CUT_ALL_DAMAGE))
				damage *= 1d - componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.CUT_ALL_DAMAGE) / 100d;
			if (!_ignoreStrikeBackAndAllUnitDamageCutTrigger)
				damage *= 1d - battleManager.TryApplyAllUnitDamageSeal(unitController.IsOther, _source, unitController, _sourceAction);
			if (componentAbnormal.DamageCutForSpecialNullificationData != null)
				damage *= 1d - componentAbnormal.DamageCutForSpecialNullificationData.GetDamageCutValue(_damageType);
			if (componentAbnormal.ConvertDamageOverTimeData != null && componentAbnormal.ConvertDamageOverTimeData.Enable)
				damage *= 1d - componentAbnormal.ConvertDamageOverTimeData.DamageCutValue;
			if (_actionType == eActionType.ATTACK)
			{
				double total = _totalDamageForLogBarrier;
				if (!battleManager.NewAllLogBarrierEnabled && total > (double)battleManager.AllLogBarrierStartValue)
				{
					double start = battleManager.AllLogBarrierStartValue, effect = battleManager.AllLogBarrierEffectValue;
					double ratio = (Math.Log((total - start) / effect + 1d) * effect + start) / total;
					total *= ratio; damage *= ratio;
				}
				if (_damageType == DamageData.eDamageType.ATK && componentAbnormal.IsAbnormalState(eAbnormalState.LOG_ATK_BARRIER))
				{
					double start = componentAbnormal.GetAbnormalStateSubValue(eAbnormalStateCategory.LOG_ATK_BARRIR);
					if (total > start)
					{
						double effect = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.LOG_ATK_BARRIR);
						double ratio = (start + effect * Math.Log((total - start) / effect + 1d)) / total;
						total *= ratio; damage *= ratio;
					}
				}
				else if (_damageType == DamageData.eDamageType.MGC && componentAbnormal.IsAbnormalState(eAbnormalState.LOG_MGC_BARRIER))
				{
					double start = componentAbnormal.GetAbnormalStateSubValue(eAbnormalStateCategory.LOG_MGC_BARRIR);
					if (total > start)
					{
						double effect = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.LOG_MGC_BARRIR);
						double ratio = (start + effect * Math.Log((total - start) / effect + 1d)) / total;
						total *= ratio; damage *= ratio;
					}
				}
				if (componentAbnormal.IsAbnormalState(eAbnormalState.LOG_ALL_BARRIER))
				{
					double start = componentAbnormal.GetAbnormalStateSubValue(eAbnormalStateCategory.LOG_ALL_BARRIR);
					if (total > start)
					{
						double effect = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.LOG_ALL_BARRIR);
						double ratio = (start + effect * Math.Log((total - start) / effect + 1d)) / total;
						total *= ratio; damage *= ratio;
					}
				}
				if (battleManager.NewAllLogBarrierEnabled && total > (double)battleManager.AllLogBarrierStartValue)
				{
					double start = battleManager.AllLogBarrierStartValue, effect = battleManager.AllLogBarrierEffectValue;
					damage *= (Math.Log((total - start) / effect + 1d) * effect + start) / total;
				}
			}
			var strikeBack = searchStrikeBack(new DamageData { Source = _source, ActionType = _actionType, DamageType = _damageType });
			bool noDamageByResist;
			if (_actionType != eActionType.ATTACK)
			{
				if (!_ignoreDef && (_damageType == DamageData.eDamageType.ATK || _damageType == DamageData.eDamageType.MGC))
				{
					double defense = _damageType == DamageData.eDamageType.ATK ? _target.GetDefZeroForDamagedEnergy() : _target.GetMagicDefZeroForDamagedEnergy();
					damage *= 1d - Math.Max(0d, defense - (double)_defPenetrate) / (defense + 100d);
				}
				damage *= CalcTalentDamageRate(_actionType, _source, _target, _targetIncludeMyTeam, false, out noDamageByResist);
			}
			else noDamageByResist = _logBarrierExpectedNoDamageByResist;
			damage = Math.Min(damage, 9999999d);
			if (componentState.CurrentState == ActionState.DIE) return;
			if (_upperLimitFunc != null) damage = _upperLimitFunc(BattleUtil.DoubleToLong(damage), 1d);
			if (strikeBack != null) return;
			if (_actionType == eActionType.ATTACK && _skill != null) damage *= _skill.AweValue;
			ChargeEnergy(eSetEnergyType.BY_SET_DAMAGE, damage * (double)unitController.SkillStackValDmg * battleManager.DodgeTPRecoverRatio, false, _skill, false, unitController, false, eEffectType.COMMON, true, false, _energyChargeMultiple);
		}

		[global::OpenPCR.Migration("250ad6ef-fee9-4247-886c-a57c7a4ace74")]
		public long SetDamage(DamageData _damageData, bool _byAttack, int _actionId, ActionParameter.OnDamageHitDelegate _onDamageHit = null, bool _hasEffect = true, Skill _skill = null, bool _energyAdd = true, Action _onDefeat = null, bool _noMotion = false, double _damageWeight = 1.0, double _damageWeightSum = 1.0, Func<long, double, long> _upperLimitFunc = null, double _energyChargeMultiple = 1.0, Dictionary<eStateIconType, List<UnitCtrl>> _usedChargeEnergyByReceiveDamage = null)
		{
			bool critical = BattleUtil.LessThanOrApproximately(BattleManager.Random(0f, 1f), _damageData.CriticalRate) && !BattleUtil.Approximately(_damageData.CriticalDamageRate, 0d) && !BattleUtil.Approximately(_damageData.CriticalRate, 0d);
			if (_damageData.ActionType == eActionType.ATTACK) critical = _damageData.IsLogBarrierCritical;
			long damage = setDamageImpl(_damageData, _byAttack, _onDamageHit, _hasEffect, _skill, _energyAdd, critical, _onDefeat, _noMotion, _upperLimitFunc, _energyChargeMultiple, _actionId, out bool isHit);
			if (_skill != null && isHit && _byAttack)
			{ var action = _skill.ActionParameters.Find(e => e.ActionId == _actionId); if (action.TargetAssignment == (eTargetAssignment)1) OnDamageForExecToSource.Call(_damageData, action); }
			if (_damageData.Target is PartsData target)
			{
				if (!_damageData.IsSlipDamage) target.SetDamage(damage, _damageData.Source);
				else
				{
					var parts = componentMultiPartsBoss.BossPartsListForBattle;
					for (int i = 0; i < parts.Count; i++) parts[i].SetDamage(damage, _damageData.Source);
				}
			}
			if (componentMultiPartsBoss.MultiBossPartsData != null) componentMultiPartsBoss.MultiBossPartsData.SetDamage(damage, _damageData.Source);
			if (_damageData.Source != null && _damageData.ActionType != eActionType.DESTROY && _damageData.ActionType != eActionType.ATTACK_FIELD && (_skill == null || _skill.IsLifeStealEnabled))
			{
				long lifeSteal = _damageData.LifeSteal, skillLifeSteal = 0L;
				bool split = _skill != null && (_damageData.ActionType == eActionType.ATTACK || _damageData.ActionType == eActionType.RATIO_DAMAGE || _damageData.ActionType == eActionType.UPPER_LIMIT_ATTACK);
				if (split) { skillLifeSteal = _skill.LifeSteal; lifeSteal = unchecked(lifeSteal + skillLifeSteal); }
				if (lifeSteal > 0L)
				{
					long normal = unchecked(lifeSteal * damage) / unchecked(lifeSteal + (int)componentParameter.Level + 100L), fromSkill = 0L;
					if (split)
					{ fromSkill = BattleUtil.DoubleToLongReverseTruncate((double)skillLifeSteal * (double)normal / (double)unchecked(_damageData.LifeSteal + skillLifeSteal)); normal = BattleUtil.DoubleToLongReverseTruncate((double)normal * (double)_damageData.LifeSteal / (double)unchecked(_damageData.LifeSteal + skillLifeSteal)); }
					if ((normal | fromSkill) != 0L)
					{
						var recoverySource = _damageData.Source;
						var recoveryAbnormal = recoverySource.ComponentAbnormal;
						var inhibit = _damageData.DamageType == DamageData.eDamageType.MGC ? eInhibitHealType.MAGIC : eInhibitHealType.PHYSICS;
						var recoveryControl = recoverySource.ComponentDamageControl;
						recoveryControl.SetRecovery(normal, fromSkill, inhibit, recoverySource, true, _skill, recoveryAbnormal.GetHealDownValue(), false, false, battleManager.BlackOutUnitList.Contains(_damageData.Source), false, true, null, false);
					}
				}
			}
			if ((_damageData.Source != null && _damageData.Source.IsOther != unitController.IsOther) || _damageData.StrikeBackSource != null)
			{
				UnitCtrl source = _damageData.Source != null ? _damageData.Source : _damageData.StrikeBackSource;
				if (source.IsSummonOrPhantom || source.IsDivision) source = source.SummonSource;
				var damageInfo = source.ComponentDamageControl.UnitDamageInfo;
				if (damageInfo != null) damageInfo.SetDamage(unchecked(damageInfo.damage + damage));
			}
			accumulateDamage = unchecked(accumulateDamage + damage);
			var log = battleLog;
			var logSource = _damageData.Source;
			var logTarget = unitController;
			var logDamageType = _damageData.DamageType;
			int weight = double.IsPositiveInfinity(_damageWeight) ? 0 : unchecked(global::OpenPCR.NativeNumeric.ToInt32(_damageWeight) * 100);
			int weightSum = double.IsPositiveInfinity(_damageWeightSum) ? 0 : unchecked(global::OpenPCR.NativeNumeric.ToInt32(_damageWeightSum) * 100);
			log.AppendBattleLog(eBattleLogType.SET_DAMAGE, (critical ? 10 : 20) + (int)logDamageType, damage, componentParameter.Hp, 0, _actionId, weight, weightSum, logSource, logTarget);
			if (_skill != null)
			{
				if (_damageData.Source != null && _skill.SkillId == _damageData.Source.UnionBurstSkillId)
				{ if (_byAttack) _damageData.Target.PassiveUbIsMagic = _damageData.DamageType == DamageData.eDamageType.MGC; _damageData.Target.TotalDamage = Math.Min(unchecked(_damageData.Target.TotalDamage + damage), 99999999L); }
				if (damage > 0L && !_skill.DamagedPartsList.Contains(_damageData.Target)) _skill.DamagedPartsList.Add(_damageData.Target);
			}
			if (!componentState.IsDead && _damageData.Source != null && componentAbnormal.DamageSealDataDictionary.ContainsKey(_damageData.Source))
			{
				foreach (var item in componentAbnormal.DamageSealDataDictionary[_damageData.Source])
				{
					var data = item.Value; int condition = (int)data.ExecConditionType;
					if ((condition == 3 && damage > 0L) || (condition == 4 && critical && damage > 0L) || (condition == 5 && isHit)) data.AddSeal(unitController);
				}
			}
			var sealSource = _damageData.Source;
			var sourceAbnormal = sealSource != null ? sealSource.ComponentAbnormal : null;
			if (sourceAbnormal != null)
			{
				bool hasOnceOwnerSeal = sourceAbnormal.DamageOnceOwnerSealDateDictionary.ContainsKey(sealSource);
				if (_skill != null && hasOnceOwnerSeal && damage > 0L && !_skill.AlreadyAddAttackSelfSeal)
				{ foreach (var item in sourceAbnormal.DamageOnceOwnerSealDateDictionary[sealSource]) item.Value.AddSeal(sealSource); _skill.AlreadyAddAttackSelfSeal = true; }
				if (sourceAbnormal.DamageOwnerSealDataDictionary.ContainsKey(sealSource))
				{
					foreach (var data in sourceAbnormal.DamageOwnerSealDataDictionary[sealSource].Values)
					{ int condition = (int)data.ExecConditionType; if ((condition == 3 && damage > 0L) || (condition == 4 && critical && damage > 0L) || (condition == 5 && isHit)) data.AddSeal(sealSource); }
				}
				if (_skill != null && critical && damage > 0L)
				{ var action = _skill.ActionParameters.Find(e => e.ActionId == _actionId); if (action.TargetAssignment != eTargetAssignment.OWNER_SITE && action.TriggerCall) sourceAbnormal.OnGiveCriticalDamage.Call(); }
			}
			if (!componentState.IsDead && sealSource != null && sealSource.IsOther != unitController.IsOther)
			{
				foreach (var data in sourceAbnormal.AttackSealDataDictionary.Values)
				{
					int condition = (int)data.ExecConditionType;
					if (condition == 1)
					{ if (_skill != null && damage > 0L && !_skill.AlreadyAddAttackSealForAllEnemy) { data.AddSeal(unitController); _skill.AlreadyAddAttackSealForAllEnemy = true; } }
					else if ((condition == 3 && damage > 0L) || (condition == 4 && critical && damage > 0L) || (condition == 5 && isHit)) data.AddSeal(unitController);
				}
			}
			if (damage > 0L && _damageData.Source != null)
			{
				var sourceDamageControl = _damageData.Source.ComponentDamageControl;
				sourceDamageControl.OnActionByDamage.Call();
				if (_skill != null && !_skill.AlreadyAddAttackSelfSeal) { sourceDamageControl.OnActionByDamageOnce.Call(); _skill.AlreadyExexActionByHit = true; }
				if (critical) sourceDamageControl.OnActionByCritical.Call();
			}
			applyChargeEnergyByReceiveDamage(_damageData.ActionType, damage, _damageData.IsAlwaysChargeEnegry, _usedChargeEnergyByReceiveDamage);
			applySealByReceiveDamage(_damageData.ActionType, damage);
			if (_skill != null && critical && damage > 0L && _skill.CriticalPartsList != null) _skill.CriticalPartsList.Add(_damageData.Target);
			if (_skill != null) _skill.TotalDamage = unchecked(_skill.TotalDamage + damage);
			return damage;

		}

		[global::OpenPCR.Migration("c236cf20-b07e-4e2b-8677-b39e1325cf10")]
		private bool isDamageIgnore(bool _energyAdd)
		{
			global::Elements.Battle.BattleManager nativeValue1 = (this).battleManager;
			if (global::System.Object.ReferenceEquals(nativeValue1, null))
				throw new global::System.NullReferenceException();
			bool nativeResult1 = (nativeValue1).IsBossInfinityHpMode();
			if (((nativeResult1 ? 1u : 0u) & 1u) == 0)
			{
				return false;
			}
			else
			{
				global::Elements.UnitCtrl nativeValue2 = (this).unitController;
				if (global::System.Object.ReferenceEquals(nativeValue2, null))
					throw new global::System.NullReferenceException();
				bool nativeValue3 = (nativeValue2).IsBoss;
				return ((!(((nativeValue3 ? 1u : 0u) == 0u)) ? 1u : 0u) != 0);
			}
		}

		[global::OpenPCR.Migration("7d36e578-14db-491a-844f-08dac565391b")]
		private Tuple<StrikeBackData, List<StrikeBackData>> searchStrikeBack(DamageData _damageData)
		{
			if (_damageData.IgnoreStrikeBackAndAllUnitDamageCutTrigger) return null;
			if (_damageData.Source == null || _damageData.ActionType == (eActionType)30 || _damageData.ActionType == (eActionType)70) return null;
			foreach (var dataSet in componentAbnormal.StrikeBackDictionary.Values)
			{
				for (int i = dataSet.DataList.Count - 1; i >= 0; i--)
				{
					var data = dataSet.DataList[i];
					if (data.Execed || data.IsDieing) continue;
					switch ((int)data.StrikeBackType)
					{
						case 1:
						case 3:
							if (_damageData.DamageType == DamageData.eDamageType.ATK) return Tuple.Create(data, dataSet.DataList);
							break;
						case 2:
						case 4:
							if (_damageData.DamageType == DamageData.eDamageType.MGC) return Tuple.Create(data, dataSet.DataList);
							break;
						case 5: case 6: return Tuple.Create(data, dataSet.DataList);
					}
				}
			}
			return null;
		}

		[global::OpenPCR.Migration("a0dd0a71-924d-4c28-abee-f9fbb4128dc0")]
		private void execBarrier(DamageData _damageData, ref double _dDamage, ref long _overRecoverValue)
		{
			var categoryData = componentAbnormal.AbnormalStateCategoryDataDictionary;
			if (componentAbnormal.IsAbnormalState(eAbnormalState.GUARD_ATK) && _damageData.DamageType == DamageData.eDamageType.ATK)
			{
				var data = categoryData[eAbnormalStateCategory.DAMAGE_RESISTANCE_ATK];
				double remainder = _dDamage - data.MainValue;
				if (remainder > 0d)
				{
					componentAbnormal.EnableAbnormalState(eAbnormalState.GUARD_ATK, false);
					_dDamage = remainder;
				}
				else
				{
					long absorbed = BattleUtil.DoubleToLong(_dDamage);
					data.MainValue -= (double)absorbed;
					_dDamage = 0d;
				}
			}
			if (componentAbnormal.IsAbnormalState(eAbnormalState.GUARD_MGC) && _damageData.DamageType == DamageData.eDamageType.MGC)
			{
				var data = categoryData[eAbnormalStateCategory.DAMAGE_RESISTANCE_MGK];
				double remainder = _dDamage - data.MainValue;
				if (remainder > 0d)
				{
					componentAbnormal.EnableAbnormalState(eAbnormalState.GUARD_MGC, false);
					_dDamage = remainder;
				}
				else
				{
					long absorbed = BattleUtil.DoubleToLong(_dDamage);
					data.MainValue -= (double)absorbed;
					_dDamage = 0d;
				}
			}
			if (componentAbnormal.IsAbnormalState(eAbnormalState.DRAIN_ATK) && _damageData.DamageType == DamageData.eDamageType.ATK)
			{
				var data = categoryData[eAbnormalStateCategory.DAMAGE_RESISTANCE_ATK];
				double remainder = _dDamage - data.MainValue;
				if (remainder > 0d)
				{
					_overRecoverValue = unchecked(_overRecoverValue + setRecoveryAndGetOverRecovery(BattleUtil.DoubleToLong(data.MainValue), unitController, _damageData.Target, _damageData.DamageType == DamageData.eDamageType.MGC, data.Source));
					componentAbnormal.EnableAbnormalState(eAbnormalState.DRAIN_ATK, false);
					_dDamage = remainder;
				}
				else
				{
					long absorbed = BattleUtil.DoubleToLong(_dDamage);
					_overRecoverValue = unchecked(_overRecoverValue + setRecoveryAndGetOverRecovery(absorbed, unitController, _damageData.Target, _damageData.DamageType == DamageData.eDamageType.MGC, data.Source));
					data = categoryData[eAbnormalStateCategory.DAMAGE_RESISTANCE_ATK];
					data.MainValue -= (double)absorbed;
					_dDamage = 0d;
				}
			}
			if (componentAbnormal.IsAbnormalState(eAbnormalState.DRAIN_MGC) && _damageData.DamageType == DamageData.eDamageType.MGC)
			{
				var data = categoryData[eAbnormalStateCategory.DAMAGE_RESISTANCE_MGK];
				double remainder = _dDamage - data.MainValue;
				if (remainder > 0d)
				{
					_overRecoverValue = unchecked(_overRecoverValue + setRecoveryAndGetOverRecovery(BattleUtil.DoubleToLong(data.MainValue), unitController, _damageData.Target, _damageData.DamageType == DamageData.eDamageType.MGC, data.Source));
					componentAbnormal.EnableAbnormalState(eAbnormalState.DRAIN_MGC, false);
					_dDamage = remainder;
				}
				else
				{
					long absorbed = BattleUtil.DoubleToLong(_dDamage);
					_overRecoverValue = unchecked(_overRecoverValue + setRecoveryAndGetOverRecovery(absorbed, unitController, _damageData.Target, _damageData.DamageType == DamageData.eDamageType.MGC, data.Source));
					data = categoryData[eAbnormalStateCategory.DAMAGE_RESISTANCE_MGK];
					data.MainValue -= (double)absorbed;
					_dDamage = 0d;
				}
			}
			if (componentAbnormal.IsAbnormalState(eAbnormalState.GUARD_BOTH) && _damageData.ActionType != eActionType.DESTROY)
			{
				var data = categoryData[eAbnormalStateCategory.DAMAGE_RESISTANCE_BOTH];
				double remainder = _dDamage - data.MainValue;
				if (remainder > 0d)
				{
					componentAbnormal.EnableAbnormalState(eAbnormalState.GUARD_BOTH, false);
					_dDamage = remainder;
				}
				else
				{
					long absorbed = BattleUtil.DoubleToLong(_dDamage);
					data.MainValue -= (double)absorbed;
					_dDamage = 0d;
				}
			}
			if (componentAbnormal.IsAbnormalState(eAbnormalState.DRAIN_BOTH) && _damageData.ActionType != eActionType.DESTROY)
			{
				var data = categoryData[eAbnormalStateCategory.DAMAGE_RESISTANCE_BOTH];
				double remainder = _dDamage - data.MainValue;
				if (remainder > 0d)
				{
					_overRecoverValue = unchecked(_overRecoverValue + setRecoveryAndGetOverRecovery(BattleUtil.DoubleToLong(data.MainValue), unitController, _damageData.Target, _damageData.DamageType == DamageData.eDamageType.MGC, data.Source));
					componentAbnormal.EnableAbnormalState(eAbnormalState.DRAIN_BOTH, false);
					_dDamage = remainder;
				}
				else
				{
					long absorbed = BattleUtil.DoubleToLong(_dDamage);
					_overRecoverValue = unchecked(_overRecoverValue + setRecoveryAndGetOverRecovery(absorbed, unitController, _damageData.Target, _damageData.DamageType == DamageData.eDamageType.MGC, data.Source));
					data = categoryData[eAbnormalStateCategory.DAMAGE_RESISTANCE_BOTH];
					data.MainValue -= (double)absorbed;
					_dDamage = 0d;
				}
			}
		}

		[global::OpenPCR.Migration("0067dba4-25a5-42e8-8189-17a9b77ac6a3")]
		private void createDamageEffectFromSetDamageImpl(DamageData _damageData, bool _hasEffect, Skill _skill, bool _critical, long _damage, bool _ignoreComboReset)
		{
			if (!_hasEffect || _damage < 0L || battleManager == null) return;
			if (!battleManager.CanCreateBattleEffect()) return;
			// Floating digits use a separate output-only number pool; no authored digit prefab is required.
			CreateDamageNumEffect(_damage, null, _skill, _damageData.DamageNumMagic || _damageData.DamageType == DamageData.eDamageType.MGC,
				_damageData.SourceForDamageNumEffect != null ? _damageData.SourceForDamageNumEffect : _damageData.Source,
				_damageData.DamageSoundType, _damageData.Target, _damageData.DamegeNumScale, _ignoreComboReset, false, false);
		}


		[global::OpenPCR.Migration("2a83784e-d922-407e-b4fe-90f295bf0d02")]
		public DamageEffectCtrl CreateDamageNumEffect(long _damage, GameObject _pPrefab, Skill _skill, bool _isMagic, UnitCtrl _source, DamageData.eDamageSoundType _soundType, BasePartsData _targetParts, float _scale, bool _ignoreComboReset, bool _isTowerTimeUp = false, bool _isTotalDmg = false)
		{
			float time = Time.time;
			bool reset = _skill != null && !_ignoreComboReset && _skill.BlackOutTime > 0f && !_skill.ForceComboDamage;
			if (reset || lastHitTime + 0.45f < time) comboCount = 0;
			else { comboCount = unchecked(comboCount + 1); if (comboCount > 6) comboCount = 0; }
			lastHitTime = time;
			int size = BattleEffectUtility.GetDamageNumEffectSize(battleManager.BlackOutUnitList.Count > 0 || _isTotalDmg, battleManager.BattleCategory);
			if (size < 1) return null;
			hitEffectSortOffset = unchecked(hitEffectSortOffset + 1);
			sortOffsetResetTimer = 0.6f;
			// Return carries only digit renderer order at the sole consuming caller.
			return null;
		}

		[global::OpenPCR.Migration("0b0171dc-1fac-48a7-9090-cfbcb7afd0aa")]
		public void SetMissAtk(UnitCtrl _source, eMissLogType _missLogType, eDamageEffectType _type = eDamageEffectType.NORMAL, BasePartsData _parts = null, float _scale = 1f)
		{
			if (_parts == null) _parts = componentMultiPartsBoss.GetFirstParts(true);
			battleLog.AppendBattleLog(eBattleLogType.MISS, (int)_missLogType, 0L, 0L, 0, 0, 1, 1, _source, unitController);
			if (!battleManager.CanCreateBattleEffect()) return;
			int size = BattleEffectUtility.GetDamageNumEffectSize(battleManager.BlackOutUnitList.Count > 0, battleManager.BattleCategory);
			if (size < 1) return;
			float time = Time.time;
			if (lastHitTime + 0.45f < time) comboCount = 0;
			else { comboCount = unchecked(comboCount + 1); if (comboCount > 6) comboCount = 0; }
			lastHitTime = time;
			sortOffsetResetTimer = 0.6f;
			hitEffectSortOffset = unchecked(hitEffectSortOffset + 1);
		}

		[global::OpenPCR.Migration("01f8b13d-29ee-4eca-8f9d-5d755ae04c78")]
		public void ShowHitEffect(SystemIdDefine.eWeaponSeType _weaponType, Skill _skill, bool _isLeft, BasePartsData _parts)
		{
			eResourceId key = BattleDefine.WEAPON_HIT_EFFECT_DIC[_weaponType];
			if (_isLeft)
			{
				eResourceId left = BattleDefine.WEAPON_HIT_EFFECT_DIC_L[key];
				if (Singleton<BattleResourceManager>.Instance.AttackHitCenterEffectPrefabDic[left] != null) key = left;
			}
			eSE se = BattleDefine.WEAPON_SE_DIC[_weaponType];
			GameObject prefab = Singleton<BattleResourceManager>.Instance.AttackHitCenterEffectPrefabDic[key];
			GameObject effect = battleEffectPool.GetEffect(prefab, null).gameObject;
			effect.transform.parent = ExceptUIRoot.Transform;
			// TrackingObject changes only the particle/digit render position; no numerical projectile or callback consumes this output.
			effect.transform.position = BattleUnitBaseAnimKitController.BoneWorldToGlobalPosConsiderRotate(unitController.CenterBone, unitController.RotateCenter, unitController.BottomTransform.lossyScale, false) + prefab.transform.localPosition;
			effect.transform.localScale = prefab.transform.localScale;
			SkillEffectCtrl controller = effect.GetComponent<SkillEffectCtrl>();
			controller.InitializeSort();
			controller.SortTarget = unitController;
			hitEffectSortOffset++;
			sortOffsetResetTimer = 0.6f;
			controller.ExecAppendCoroutine(null, false);
			controller.SetSortOrderHit(hitEffectSortOffset);
			controller.PlaySe(se, unitController.IsLeftDir);
		}

		[global::OpenPCR.Migration("86a496aa-438d-49f8-b936-62da8c2e9821")]
		private long setRecoveryAndGetOverRecovery(long _value, UnitCtrl _source, BasePartsData _target, bool _isMagic, UnitCtrl _healSource)
		{
			long overRecovery = unchecked((long)componentParameter.Hp + _value - (long)componentParameter.MaxHp);
			double healDown = _healSource.ComponentAbnormal.GetHealDownValue();
			SetRecovery(_value, 0L, (eInhibitHealType)(_isMagic ? 1 : 0), _source, false, null, healDown, true, false, false, false, true, _target, false);
			return overRecovery;
		}

		[global::OpenPCR.Migration("eade7420-40c0-4bde-a896-e7549c051967")]
		public void SetRecovery(long _value, long _valueSkillLifeSteal, eInhibitHealType _inhibitHealType, UnitCtrl _source, bool _isLifeSteal, Skill _skill, double _healDownValue = 1.0, bool _isEffect = true, bool _isRevival = false, bool _isUnionBurstLifeSteal = false, bool _isRegenerate = false, bool _useNumberEffect = true, BasePartsData _target = null, bool _releaseToad = false)
		{
			if (_target == null) _target = componentMultiPartsBoss.GetFirstParts(true);
			if (((componentState.IsDead || (long)componentParameter.Hp <= 0L) && !_isRevival) || unitController.IsClanBattleOrSekaiEnemy)
			{
				battleLog.AppendBattleLog(eBattleLogType.MISS, 8, 0L, 0L, 0, 0, 1, 1, _source, unitController);
				return;
			}
			double decrease = 0d;
			if (componentAbnormal.IsAbnormalState(eAbnormalState.DECREASE_HEAL) && _inhibitHealType != eInhibitHealType.NO_EFFECT)
				decrease = Math.Min(componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.DECREASE_HEAL), 1d);
			double multiplier = (1d - decrease) * _healDownValue;
			_value = BattleUtil.DoubleToLong(multiplier * (double)_value);
			_valueSkillLifeSteal = BattleUtil.DoubleToLong(multiplier * (double)_valueSkillLifeSteal);
			if (componentAbnormal.IsAbnormalState(eAbnormalState.INHIBIT_HEAL) && _inhibitHealType != eInhibitHealType.NO_EFFECT)
			{
				double inhibit = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.INHIBIT_HEAL);
				if (!BattleUtil.Approximately(inhibit, 0d))
				{
					long damage = BattleUtil.DoubleToLong(componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.INHIBIT_HEAL) * (double)unchecked(_value + _valueSkillLifeSteal));
					if (componentAbnormal.IsAbnormalState(eAbnormalState.DISABLE_HEAL)) damage = 1L;
					var data = new DamageData
					{
						Target = componentMultiPartsBoss.GetFirstParts(unitController),
						Damage = damage,
						DamageType = DamageData.eDamageType.NONE,
						Source = componentAbnormal.AbnormalStateCategoryDataDictionary[eAbnormalStateCategory.INHIBIT_HEAL].Source,
						DamageNumMagic = _inhibitHealType == eInhibitHealType.MAGIC,
						ActionType = eActionType.INHIBIT_HEAL
					};
					var state = componentAbnormal.AbnormalStateCategoryDataDictionary[eAbnormalStateCategory.INHIBIT_HEAL];
					SetDamage(data, false, state.ActionId, null, true, state.Skill, true, null, false, 1d, 1d, null, 1d, null);
				}
				return;
			}
			if (_releaseToad && componentAbnormal.ToadDatas.Count > 0 && componentAbnormal.ToadDatas[0].ReleaseByHeal)
				componentAbnormal.ToadDatas[0].Enable = false;
			if (_isLifeSteal)
			{
				long start = 0L;
				if (_skill != null && _skill.UseAccumulatedDamage)
				{
					start = _skill.AccumulatedLifeSteal;
					_skill.AccumulatedLifeSteal = unchecked(start + _value);
				}
				_value = BattleUtil.CalcAttenuationForLifeSteal(start, _value);
			}
			long total = componentAbnormal.IsAbnormalState(eAbnormalState.DISABLE_HEAL) ? 0L : unchecked(_value + _valueSkillLifeSteal);
			componentParameter.Hp = unchecked((long)componentParameter.Hp + total);
			battleLog.AppendBattleLog(eBattleLogType.SET_RECOVERY, 0, total, componentParameter.Hp, 0, 0, 1, 1, _source, unitController);
			if ((long)componentParameter.Hp > (long)componentParameter.MaxHp) componentParameter.Hp = componentParameter.MaxHp;
			double hpRate = (double)(long)componentParameter.Hp / (double)(long)componentParameter.MaxHp;
			foreach (var item in OnRecoverListForChangeSpeedDisableByMaxHp) item.Value.Call(hpRate);
			float lifeRate = (float)(long)componentParameter.Hp / (float)(long)componentParameter.MaxHpAfterPassive;
			if (unitController.LifeGauge != null)
			{
				if (unitController.IsBoss)
				{
					if (battleManager.IsBossInfinityHpBattle() && battleManager.IsBossInfinityHpMode())
						SingletonMonoBehaviour<BattleHeaderController>.Instance.BossGauge.UpdateInvalidLifeAmount(battleManager.GetInvalidLifeAmount(componentParameter.MaxHp));
					else SetBossGaugeValue();
				}
				else if (!_isUnionBurstLifeSteal) unitController.LifeGauge.SetNormalizedLifeAmount(lifeRate, componentParameter.MaxLifeAmount, false, false);
				OnLifeAmmountChange.Call(lifeRate);
			}
			if (_isUnionBurstLifeSteal) unionburstLifeStealNum = Math.Min(unchecked(unionburstLifeStealNum + total), 99999999L);
			else
			{
				if (_useNumberEffect) createHealNumEffect(total, _target, battleManager.BlackOutUnitList.Count > 0);
				if (_isEffect) _target.RecoveryEffect(_source, false, battleEffectPool, _isRegenerate);
			}
		}

		[global::OpenPCR.Migration("0bc7612a-e73f-4c08-8055-aeb3d3f0e193")]
		private void applyChargeEnergyByReceiveDamage(eActionType _actionType, long _damage, bool _isAlwaysChargeEnegry, Dictionary<eStateIconType, List<UnitCtrl>> _usedChargeEnergyByReceiveDamage = null)
		{
			if (_damage < 1L || (long)componentParameter.Hp < 1L) return;
			if (_actionType == eActionType.ENCHANT_STRIKE_BACK || _actionType == eActionType.DIVISION)
			{
				foreach (var item in componentAbnormal.ChargeEnergyByReceiveDamageDictionary) item.Value.Call();
				return;
			}
			if (_actionType != eActionType.ATTACK && _actionType != eActionType.DESTROY && (((int)_actionType & 0x7ffffffe) != 46)) return;
			if (_usedChargeEnergyByReceiveDamage == null) return;
			foreach (var item in componentAbnormal.ChargeEnergyByReceiveDamageDictionary)
			{
				if (!_usedChargeEnergyByReceiveDamage.ContainsKey(item.Key))
					_usedChargeEnergyByReceiveDamage[item.Key] = new List<UnitCtrl>();
				if (!_isAlwaysChargeEnegry && _usedChargeEnergyByReceiveDamage[item.Key].Contains(unitController)) continue;
				item.Value.Call();
				if (!_isAlwaysChargeEnegry) _usedChargeEnergyByReceiveDamage[item.Key].Add(unitController);
			}
		}

		[global::OpenPCR.Migration("957c7590-c985-4d9d-9389-3b64ec34ab07")]
		private void applySealByReceiveDamage(eActionType _actionType, long _damage)
		{
			if (_damage < 1L || componentState.IsDead) return;
			if (!componentAbnormal.SealCancelDictionary.ContainsKey((SealAction.eCancelTriggerType)1)) return;
			if (_actionType != eActionType.ATTACK && _actionType != eActionType.DESTROY && (((int)_actionType & 0x7ffffffe) != 46) && _actionType != eActionType.DIVISION && _actionType != eActionType.ENCHANT_STRIKE_BACK) return;
			foreach (var data in componentAbnormal.SealCancelDictionary[(SealAction.eCancelTriggerType)1].Values) ((SealDamageCountCancelData)data).OnDamage();
		}

		[global::OpenPCR.Migration("5fa26402-18cb-4a2b-af90-2c6b989fa6bb")]
		public void ChargeEnergy(eSetEnergyType _setEnergyType, double _energy, bool _slipDamage, Skill _skill, bool _hasEffect = false, UnitCtrl _source = null, bool _hasNumberEffect = true, eEffectType _effectType = eEffectType.COMMON, bool _useRecoveryRate = true, bool _isRegenerate = false, double _multipleValue = 1.0)
		{
			if (componentAbnormal.IsAbnormalState(eAbnormalState.FEAR) && (_setEnergyType == eSetEnergyType.BY_ATK || _setEnergyType == eSetEnergyType.KILL_BONUS)) return;
			if (componentAbnormal.IsAbnormalState(eAbnormalState.ALL_FEAR) && (_setEnergyType == eSetEnergyType.BY_ATK || _setEnergyType == eSetEnergyType.BY_SET_DAMAGE || _setEnergyType == eSetEnergyType.KILL_BONUS || (_setEnergyType == eSetEnergyType.BY_CHANGE_ENERGY && _energy > 0d))) return;
			double energy = _energy;
			if (_energy > 0d && _useRecoveryRate) energy = ((double)componentParameterClamped.EnergyRecoveryRateZeroWithLimit + 100d) / 100d * _energy;
			energy *= _multipleValue;
			if (_setEnergyType == eSetEnergyType.BY_ATK && !wasChargeByAtk)
			{
				energy += (double)componentParameterClamped.EnergyChargeZero;
				wasChargeByAtk = true;
			}
			else if (_setEnergyType == eSetEnergyType.BY_SET_DAMAGE)
			{
				double accumulated = 0d;
				if (_skill != null && !_slipDamage && _skill.UseAccumulatedDamage)
				{
					_skill.AccumulatedTpRecovery.TryGetValue(unitController, out accumulated);
					_skill.AccumulatedTpRecovery[unitController] = accumulated + energy;
				}
				energy = BattleUtil.CalcAttenuationForEnergy(accumulated, energy);
			}
			componentParameter.SetEnergy(componentParameter.Energy + energy, _setEnergyType, _source);
			if (!battleManager.CanCreateBattleEffect()) return;
			GameObject prefab = !(_energy >= 0d) ? Singleton<BattleResourceManager>.Instance.UnitDamageNumEffectPrefab : Singleton<BattleResourceManager>.Instance.RecoveryNumEffectPrefab;
			if (prefab == null) return;
			if (_hasNumberEffect)
			{
				float time = Time.time;
				if (lastEnergyHealTime + 0.45f < time) energyHealComboCount = 0;
				else { energyHealComboCount = unchecked(energyHealComboCount + 1); if (energyHealComboCount > 6) energyHealComboCount = 0; }
				lastEnergyHealTime = time;
				int size = !(_energy >= 0d) ? BattleEffectUtility.GetDamageNumEffectSize(battleManager.BlackOutUnitList.Count > 0, battleManager.BattleCategory) : BattleEffectUtility.GetHealNumEffectSize(battleManager.BlackOutUnitList.Count > 0, battleManager.BattleCategory);
				if (size < 1) return;
				DamageEffectCtrl effect = battleEffectPool.LoadNumberEffect(prefab, unitController).GetComponent<DamageEffectCtrl>();
				BasePartsData parts = componentMultiPartsBoss.GetFirstParts(true);
				float scale = (float)size / 100f;
				effect.SetPosition(parts, Vector3.up * (scale * 0.1f) * (float)energyHealComboCount, scale);
				effect.SetSortOrder(componentSortOrder.IsFront ? 22750 : 11250);
				effect.SetDamageText(((double)Math.Min(BattleUtil.DoubleToInt(energy), 99999999)).ToString(), (eDamageEffectColor)3, null);
			}
			if (_effectType == eEffectType.COMMON && _hasEffect)
				componentMultiPartsBoss.GetFirstParts(true).RecoveryEffect(unitController, true, battleEffectPool, _isRegenerate);
		}

		[global::OpenPCR.Migration("0d489b1d-1ccd-418d-afd7-2831e926b79f")]
		public void ShowLifeStealNum()
		{
			if (unionburstLifeStealNum == 0L) return;
			createHealNumEffect(unionburstLifeStealNum, componentMultiPartsBoss.GetFirstParts(true), true);
			if (unitController.LifeGauge != null)
				unitController.LifeGauge.SetNormalizedLifeAmount((float)(long)componentParameter.Hp / (float)(long)componentParameter.MaxHpAfterPassive, componentParameter.MaxLifeAmount, false, false);
			unionburstLifeStealNum = 0L;
		}

		[global::OpenPCR.Migration("0b1dfb9c-5799-4020-8b7e-bea7b2e983c2")]
		public void SetBossGaugeValue()
		{
			if (!battleManager.IsShowBossGauge) return;
			if (battleManager.IsSubStoryRaidBattle || (battleManager.IsMiniRaidBattle && !battleManager.IsMiniRaidExtraPhase))
			{ setRaidBossGaugeValue(); return; }
			if (battleManager.IsAcnUnknownBattle)
				throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UnitComponentDamageControl::SetBossGaugeValue AcnUnknown solo gauge presentation");
			if (PartsMultiBossGauge != null) PartsMultiBossGauge.SetNormalizedLifeAmount(componentParameter.MaxHp, componentParameter.Hp);
			else if (battleManager.BossList.Count == 1 && !battleManager.IsDefenseBattle)
				SingletonMonoBehaviour<BattleHeaderController>.Instance.BossGauge.SetNormalizedLifeAmount(componentParameter.MaxHp, componentParameter.Hp);
		}

		[global::OpenPCR.Migration("3c36f879-6020-4a22-8410-592ed4c7ab7a")]
		private void setRaidBossGaugeValue()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.Core.UnitComponentDamageControl::setRaidBossGaugeValue raid HUD presentation outside clan battle and training");
		}

		public delegate void OnDamageDelegate(bool byAttack, long damage, bool critical);
		public OnDamageDelegate OnDamage { get; set; }
		public OnDamageDelegate OnHpChange { get; set; }

		[global::OpenPCR.Migration("e892bf94-3e5d-4e67-a393-f0c8a43a9a77")]
		public void ReduceSortOffsetResetTimer(float _deltaTime)
		{
			if (sortOffsetResetTimer > 0f)
			{
				sortOffsetResetTimer -= _deltaTime;
				if (sortOffsetResetTimer <= 0f) hitEffectSortOffset = 0;
			}
		}

		[global::OpenPCR.Migration("469d211f-0a5c-476c-95de-c17365b66549")]
		public IEnumerator SetColorOffsetDefaultWithDelay()
		{
			for (int i = 0; i < 8 && !battleManager.BlackOutUnitList.Contains(unitController); i++)
			{
				componentColor.SetDamageColor();
				yield return null;
			}
			componentColor.ResetColor(null);
		}

		private global::Elements.Battle.BattleManager battleManager { get; set; }
		private int hpHealComboCount;
		private float lastHpHealTime;
		private int energyHealComboCount;
		private float lastEnergyHealTime;
		private long unionburstLifeStealNum;
		private bool wasChargeByAtk;

		[global::OpenPCR.Migration("c4f79d41-63e2-4ad1-9e85-cc928893ef80")]
		public void ResetAccumulateDamage() { accumulateDamage = 0L; }

		[global::OpenPCR.Migration("04941e9a-ac08-40d5-b3c7-e057dc0202e8")]
		public void ResetChargedFlagByAtk() { wasChargeByAtk = false; }

		[global::OpenPCR.Migration("745df860-64d4-4b74-92b1-dd19a95326aa")]
		public void ResetTotalDamage()
		{
			if (!unitController.IsPartsBoss) { componentMultiPartsBoss.GetFirstParts(true).ResetTotalDamage(); return; }
			for (int i = 0; i < componentMultiPartsBoss.BossPartsListForBattle.Count; i++)
				componentMultiPartsBoss.BossPartsListForBattle[i].ResetTotalDamage();
		}

		[global::OpenPCR.Migration("41c1ac3a-94d7-4be6-8153-cd363bda3014")]
		private void createHealNumEffect(long _value, BasePartsData _targetParts, bool _isUb)
		{
			if (!battleManager.CanCreateBattleEffect()) return;
			int size = BattleEffectUtility.GetHealNumEffectSize(_isUb, battleManager.BattleCategory);
			if (size < 1) return;
			GameObject prefab = Singleton<BattleResourceManager>.Instance.RecoveryNumEffectPrefab;
			if (prefab == null) return;
			var effect = battleEffectPool.LoadNumberEffect(prefab, unitController).GetComponent<DamageEffectCtrl>();
			float time = Time.time;
			if (lastHpHealTime + 0.45f < time) hpHealComboCount = 0;
			else { hpHealComboCount = unchecked(hpHealComboCount + 1); if (hpHealComboCount > 6) hpHealComboCount = 0; }
			lastHpHealTime = time;
			Vector3 prefabScale = prefab.transform.localScale;
			Transform effectTransform = effect.transform;
			float scale = (float)size / 100f;
			effectTransform.localScale = new Vector3(scale * prefabScale.x, scale * prefabScale.y, 1f);
			effect.SetPosition(_targetParts, Vector3.up * (scale * 0.1f) * (float)hpHealComboCount, scale);
			hitEffectSortOffset = unchecked(hitEffectSortOffset + 1);
			sortOffsetResetTimer = 0.6f;
			effect.SetSortOrder((componentSortOrder.IsFront ? 22750 : 11250) + hitEffectSortOffset);
			effect.SetDamageText(Math.Min(_value, 99999999L).ToString(), (eDamageEffectColor)1, null);
		}

		[global::OpenPCR.Migration("db13e83e-c199-419d-9f0e-6e24f7fe435f")]
		public double CalcTalentDamageRate(eActionType _actionType, UnitCtrl _source, BasePartsData _target, bool _targetIncludeMyTeam, bool _isIgnoreTalentBonusForRatioSlip, out bool _isNoDamageByResist)
		{
			double weakness = calcTalentWeaknessPercent(_actionType, _targetIncludeMyTeam, _source, _target, _isIgnoreTalentBonusForRatioSlip);
			_isNoDamageByResist = weakness == 0d;
			long bonus = _source == null ? 0L : (long)_source.ComponentParameterClamped.TalentBonusPermyriadZero;
			long enhance = _source == null ? 0L : (long)_source.ComponentParameterClamped.TalentBonusEnhancePercentZero;
			double rate = ((double)enhance + 100d) / 100d;
			return weakness * (((double)bonus + 10000d) / 10000d * getTalentFormationBonusPermyriad(_actionType, _targetIncludeMyTeam, _source, _isIgnoreTalentBonusForRatioSlip) * rate + 10000d) / 1000000d;
		}

		[global::OpenPCR.Migration("0b06f6e7-b91a-455c-9b20-778cf9303915")]
		private double calcTalentWeaknessPercent(eActionType _actionType, bool _targetIncludeMyTeam, UnitCtrl _source, BasePartsData _target, bool _isIgnoreTalentBonusForRatioSlip)
		{
			if (_isIgnoreTalentBonusForRatioSlip) return 100d;
			if (!SkillDefine.TALENT_CONSIDER_DAMAGE_ACTION.Contains(_actionType) || _targetIncludeMyTeam) return 100d;
			if (!battleManager.IsReleasedKnightEnhance) return 100d;
			return _target.GetTalentWeaknessPercent(_source.TalentIds);
		}

		[global::OpenPCR.Migration("f5b56f48-c6a1-48db-a6b2-72958952819e")]
		private double getTalentFormationBonusPermyriad(eActionType _actionType, bool _targetIncludeMyTeam, UnitCtrl _source, bool _isIgnoreTalentBonusForRatioSlip)
		{
			if (!_isIgnoreTalentBonusForRatioSlip && SkillDefine.TALENT_CONSIDER_DAMAGE_ACTION.Contains(_actionType) && !_targetIncludeMyTeam && !_source.IsOther)
				return (double)_source.ComponentPassiveSkill.TalentFormationBonusPermyriad;
			return 0d;
		}

		[global::OpenPCR.Migration("af101ead-aea7-4ce0-a769-36537c8e8506")]
		private long setDamageImpl(DamageData _damageData, bool _byAttack, ActionParameter.OnDamageHitDelegate _onDamageHit, bool _hasEffect, Skill _skill, bool _energyAdd, bool _critical, Action _onDefeat, bool _noMotion, Func<long, double, long> _upperLimitFunc, double _energyChargeMultiple, int _actionId, out bool _isHit)
		{
			_isHit = true;
			if (componentState.IdleOnly || (unitController.IsDivisionSourceForDamage && !_damageData.IsDivisionDamage))
			{ _isHit = false; return 0L; }
			if (battleManager.GetPurpose() == eSpecialBattlePurpose.SHIELD && unitController.IsBoss) battleManager.SubstructEnemyPoint(1L);
			componentAbnormal.ExecPassiveSeal(PassiveSealAction.ePassiveTiming.DAMAGED);
			if (componentAbnormal.IsNoDamageMotion()) { _isHit = false; return 0L; }
			if ((componentAbnormal.IsAbnormalState(eAbnormalState.PHYSICS_DODGE) && _damageData.DamageType == DamageData.eDamageType.ATK) ||
				(componentAbnormal.IsAbnormalState(eAbnormalState.MAGIC_DODGE) && _damageData.DamageType == DamageData.eDamageType.MGC))
			{ SetMissAtk(_damageData.Source, eMissLogType.DODGE_BY_NO_DAMAGE_MOTION, eDamageEffectType.NORMAL, _damageData.Target, 1f); _isHit = false; return 0L; }
			double damage = _damageData.Damage;
			double criticalMultiplier = _damageData.CriticalDamageRate + _damageData.CriticalDamageRate;
			if (_critical) damage *= criticalMultiplier;
			if (componentAbnormal.HasDebuffDamageUpData()) damage *= componentAbnormal.GetDebuffDamageUpValue();
			double totalForBarrier = damage, energyDamage = damage;
			if (_damageData.ActionType == eActionType.ATTACK)
			{ damage = _damageData.LogBarrierExpectedDamage; energyDamage = _damageData.LogBarrierExpectedDamageForEnergyCalc; }
			if (_damageData.DamageType == DamageData.eDamageType.ATK && componentAbnormal.IsAbnormalState(eAbnormalState.CUT_ATK_DAMAGE))
			{ double rate = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.CUT_ATK_DAMAGE) / -100d + 1d; damage *= rate; energyDamage *= rate; }
			else if (_damageData.DamageType == DamageData.eDamageType.MGC && componentAbnormal.IsAbnormalState(eAbnormalState.CUT_MGC_DAMAGE))
			{ double rate = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.CUT_MGC_DAMAGE) / -100d + 1d; damage *= rate; energyDamage *= rate; }
			bool ignoreDamageCut = SkillDefine.IGNORE_DAMAGE_CUT_ACTION.Contains(_damageData.ActionType);
			if (!ignoreDamageCut & componentAbnormal.IsAbnormalState(eAbnormalState.CUT_ALL_DAMAGE))
			{ double rate = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.CUT_ALL_DAMAGE) / -100d + 1d; damage *= rate; energyDamage *= rate; }
			if (_skill != null && _damageData.Source != null && !_damageData.IgnoreStrikeBackAndAllUnitDamageCutTrigger && _byAttack)
			{ double rate = 1d - battleManager.TryApplyAllUnitDamageSeal(unitController.IsOther, _damageData.Source, unitController, _skill.ActionParameters.Find(e => e.ActionId == _actionId)); damage *= rate; energyDamage *= rate; }
			var specialCut = componentAbnormal.DamageCutForSpecialNullificationData;
			if (!ignoreDamageCut && specialCut != null)
			{ double rate = 1d - specialCut.GetDamageCutValue(_damageData.DamageType); damage *= rate; energyDamage *= rate; }
			var strikeBack = searchStrikeBack(_damageData);
			if (_hasEffect)
			{ OnDamageForUIShake.Call(); if (!unitController.DisableFlash && strikeBack == null) unitController.AppendCoroutine(SetColorOffsetDefaultWithDelay(), ePauseType.NO_DIALOG, null); }
			bool noDamageByResist = false;
			if (_damageData.ActionType == eActionType.ATTACK) noDamageByResist = _damageData.LogBarrierExpectedIsNoDamageByResist;
			else
			{
				if (!_damageData.IgnoreDef && _damageData.DamageType != DamageData.eDamageType.NONE)
				{
					double defense = _damageData.Target.GetDefZero(), magicDefense = _damageData.Target.GetMagicDefZero();
					var higher = _damageData.Target.Owner.ComponentAbnormal.ReduceDamageByHigherDefData;
					bool? physical = higher != null && defense != magicDefense && higher.Enable ? defense > magicDefense :
						_damageData.DamageType == DamageData.eDamageType.MGC ? false : _damageData.DamageType == DamageData.eDamageType.ATK ? true : (bool?)null;
					if (physical.HasValue)
					{
						double def = physical.Value ? _damageData.Target.GetDefZero() : _damageData.Target.GetMagicDefZero();
						double energyDef = physical.Value ? _damageData.Target.GetDefZeroForDamagedEnergy() : _damageData.Target.GetMagicDefZeroForDamagedEnergy();
						double rate = 1d - Math.Max(0d, def - (double)_damageData.DefPenetrate) / (def + 100d);
						damage *= rate; totalForBarrier *= rate;
						energyDamage *= 1d - Math.Max(0d, energyDef - (double)_damageData.DefPenetrate) / (energyDef + 100d);
					}
				}
				double talentRate = CalcTalentDamageRate(_damageData.ActionType, _damageData.ActionType == eActionType.ENCHANT_STRIKE_BACK ? _damageData.StrikeBackSource : _damageData.Source, _damageData.Target, _damageData.TargetIncludeMyTeam, _damageData.IgnoreTalentBonusForRatioSlip, out noDamageByResist);
				damage *= talentRate; totalForBarrier *= talentRate; energyDamage *= talentRate;
			}
			int actionType = (int)_damageData.ActionType;
			bool logTarget = actionType == 1 || actionType == 9 || actionType == 25 || actionType == 31 || actionType == 33 || actionType == 36 || actionType == 79;
			if (battleManager.IsReleaseSlipDamageLogBarrier ? logTarget && !_damageData.IgnoreLogBarrier : actionType == 1)
			{
				double total = actionType == 1 ? _damageData.TotalDamageForLogBarrier : totalForBarrier;
				bool transcend = false;
				if (!battleManager.NewAllLogBarrierEnabled && total > (double)battleManager.AllLogBarrierStartValue)
				{ double start = battleManager.AllLogBarrierStartValue, effect = battleManager.AllLogBarrierEffectValue; double rate = (Math.Log((total - start) / effect + 1d) * effect + start) / total; total *= rate; damage *= rate; energyDamage *= rate; }
				if (_damageData.DamageType == DamageData.eDamageType.ATK && componentAbnormal.IsAbnormalState(eAbnormalState.LOG_ATK_BARRIER))
				{
					double start = componentAbnormal.GetAbnormalStateSubValue(eAbnormalStateCategory.LOG_ATK_BARRIR);
					if (total > start) { double effect = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.LOG_ATK_BARRIR); double rate = (start + effect * Math.Log((total - start) / effect + 1d)) / total; transcend = true; total *= rate; damage *= rate; energyDamage *= rate; }
				}
				else if (_damageData.DamageType == DamageData.eDamageType.MGC && componentAbnormal.IsAbnormalState(eAbnormalState.LOG_MGC_BARRIER))
				{
					double start = componentAbnormal.GetAbnormalStateSubValue(eAbnormalStateCategory.LOG_MGC_BARRIR);
					if (total > start) { double effect = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.LOG_MGC_BARRIR); double rate = (start + effect * Math.Log((total - start) / effect + 1d)) / total; transcend = true; total *= rate; damage *= rate; energyDamage *= rate; }
				}
				if (componentAbnormal.IsAbnormalState(eAbnormalState.LOG_ALL_BARRIER))
				{
					double start = componentAbnormal.GetAbnormalStateSubValue(eAbnormalStateCategory.LOG_ALL_BARRIR);
					if (total > start) { double effect = componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.LOG_ALL_BARRIR); double rate = (start + effect * Math.Log((total - start) / effect + 1d)) / total; transcend = true; total *= rate; damage *= rate; energyDamage *= rate; }
				}
				if (battleManager.NewAllLogBarrierEnabled && total > (double)battleManager.AllLogBarrierStartValue)
				{ double start = battleManager.AllLogBarrierStartValue, effect = battleManager.AllLogBarrierEffectValue; double rate = (Math.Log((total - start) / effect + 1d) * effect + start) / total; transcend = true; total *= rate; damage *= rate; energyDamage *= rate; }
				if (transcend && (_damageData.Source != null || _damageData.StrikeBackSource != null))
				{
					UnitCtrl source = _damageData.Source == null ? _damageData.StrikeBackSource : _damageData.Source;
					double additional = 0d;
					foreach (var data in source.ComponentAbnormal.TranscendDamageListForSkillAction) additional += data.CalcTranscendDamage(damage, total);
					damage += additional; energyDamage += additional;
				}
			}
			damage = Math.Min(damage, _damageData.OverrideDamageLimit == -1L ? 9999999d : (double)_damageData.OverrideDamageLimit);
			energyDamage = Math.Min(energyDamage, _damageData.OverrideDamageLimit == -1L ? 9999999d : (double)_damageData.OverrideDamageLimit);
			if (_damageData.LimitDamageFunc != null) { damage = _damageData.LimitDamageFunc(damage); energyDamage = _damageData.LimitDamageFunc(energyDamage); }
			bool ignoreComboReset = SkillDefine.IGNORE_RESET_DAMAGE_NUM_POS_ACTION.Contains(_damageData.ActionType);
			if (componentState.CurrentState == ActionState.DIE)
			{ createDamageEffectFromSetDamageImpl(_damageData, _hasEffect, _skill, _critical, BattleUtil.DoubleToLong(damage), ignoreComboReset); return 0L; }
			if (_upperLimitFunc != null)
			{ double rate = _critical ? criticalMultiplier : 1d; damage = _upperLimitFunc(BattleUtil.DoubleToLong(damage), rate); energyDamage = _upperLimitFunc(BattleUtil.DoubleToLong(energyDamage), rate); }
			if (strikeBack != null && !noDamageByResist)
			{
				var data = strikeBack.Item1; var list = strikeBack.Item2;
				data.Exec(_damageData.Source, unitController, BattleUtil.DoubleToLong(damage), () => list.Remove(data));
				componentAbnormal.OnChangeState.Call(eStateIconType.STRIKE_BACK, false);
				_isHit = false; return 0L;
			}
			if (_skill != null && _damageData.ActionType == eActionType.ATTACK) { damage *= _skill.AweValue; energyDamage *= _skill.AweValue; }
			var convert = componentAbnormal.ConvertDamageOverTimeData;
			bool convertTarget = SkillDefine.CONVERT_DAMEGE_OVER_TIME_TARGET_ACTION.Contains(_damageData.ActionType);
			if (convert != null && !ignoreDamageCut && convert.Enable && convertTarget)
			{ double cut = convert.DamageCutValue * damage; damage -= cut; energyDamage *= 1d - convert.DamageCutValue; convert.AddDamage(BattleUtil.DoubleToLong(cut / (double)convert.NumberOfDamage)); }
			applyReductionDamage(_damageData, _skill, _actionId, ref damage, ref energyDamage, ignoreDamageCut, ref noDamageByResist);
			if (_energyAdd)
			{ int energyActionType = (int)_damageData.ActionType; bool slip = energyActionType == 9 || energyActionType == 36 || energyActionType == 59 || energyActionType == 79; ChargeEnergy(eSetEnergyType.BY_SET_DAMAGE, energyDamage * (double)unitController.SkillStackValDmg, slip, _skill, false, unitController, false, eEffectType.COMMON, true, false, Math.Max(_energyChargeMultiple, 0d)); }
			if (BattleUtil.LessThanOrApproximately(damage, 0d) && _damageData.ActionType == (eActionType)70)
			{ createDamageEffectFromSetDamageImpl(_damageData, _hasEffect, _skill, _critical, BattleUtil.DoubleToLong(damage), ignoreComboReset); return 0L; }
			if (!noDamageByResist) damage = Math.Max(damage, 1d);
			if (battleManager.GetPurpose() == eSpecialBattlePurpose.ABSORBER && battleManager.AbsorberValue != 0L && unitController.IsBoss)
			{
				long amount = BattleUtil.DoubleToLong(damage);
				int absorberActionType = (int)_damageData.ActionType;
				if (absorberActionType == 1 || absorberActionType == 25 || absorberActionType == 46 || absorberActionType == 47)
				{
					if (amount < _skill.AbsorberValue) { battleManager.SubstructEnemyPoint(amount); damage = 0d; _skill.AbsorberValue = unchecked(_skill.AbsorberValue - amount); }
					else { battleManager.SubstructEnemyPoint(_skill.AbsorberValue); long absorber = _skill.AbsorberValue; _skill.AbsorberValue = 0L; damage -= absorber; }
				}
				else if (absorberActionType == 9 || absorberActionType == 31 || absorberActionType == 36 || absorberActionType == 79) damage = _damageData.ExecAbsorber(amount);
				else if (absorberActionType == 33)
				{ if (amount < battleManager.AbsorberValue) { battleManager.SubstructEnemyPoint(amount); damage = 0d; } else { battleManager.SubstructEnemyPoint(battleManager.AbsorberValue); damage -= _skill.AbsorberValue; } }
			}
			long overRecovery = 0L;
			if (_damageData.ActionType != (eActionType)59 && _damageData.ActionType != (eActionType)70) execBarrier(_damageData, ref damage, ref overRecovery);
			long result = BattleUtil.DoubleToLong(damage);
			long initialHpForHit = componentParameter.Hp;
			long initialHpForVoice = componentParameter.Hp;
			long initialMaxHp = componentParameter.MaxHp;
			long initialHp = componentParameter.Hp;
			if (!isDamageIgnore(_energyAdd)) componentParameter.Hp = unchecked((long)componentParameter.Hp - result + Math.Max(0L, overRecovery));
			if (_onDamageHit != null && initialHpForHit > 0L) _onDamageHit(result);
			if ((long)componentParameter.Hp < 0L) componentParameter.Hp = 0L;
			if (componentAbnormal.IsAbnormalState(eAbnormalState.HP_LOWER_LIMIT))
			{
				long limit = BattleUtil.DoubleToLong(componentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.HP_LOWER_LIMIT));
				componentParameter.Hp = initialHp <= limit ? initialHp : Math.Max((long)componentParameter.Hp, limit);
				result = Math.Max(0L, unchecked(initialHp - (long)componentParameter.Hp));
			}
			if ((long)componentParameter.Hp == 0L && battleManager.IsBossInfinityHpBattle() && battleManager.IsBossInfinityHpMode() && unitController.IsBoss) componentParameter.Hp = 1L;
			if ((long)componentParameter.Hp == 0L)
			{
				bool survived = componentAbnormal.IsTough;
				if (!survived)
				{
					survived = componentAbnormal.ExecKnightGuard();
					if (_damageData.ActionType != (eActionType)59) survived |= componentAbnormal.ExecGuts();
					if (componentAbnormal.AutoRevivalData != null) survived |= componentAbnormal.AutoRevivalData.TryExec();
				}
				if (survived && (long)componentParameter.Hp == 0L) componentParameter.Hp = 1L;
			}
			if ((long)componentParameter.Hp > (long)componentParameter.MaxHp) componentParameter.Hp = componentParameter.MaxHp;
			if ((float)initialHpForVoice > (float)initialMaxHp * 0.2f && (float)(long)componentParameter.Hp < (float)(long)componentParameter.MaxHp * 0.2f) componentSound.PlayDamageVoice();
			if (unitController.LifeGauge != null)
			{
				float life = (float)(long)componentParameter.Hp / (float)(long)componentParameter.MaxHpAfterPassive;
				if (unitController.IsClanBattleOrSekaiEnemy || unitController.IsTrialBattleEnemy || unitController.IsAbyssBossEnemy)
				{
					bool ub = _skill != null && battleManager.UnitList.Exists(unit => unit.UnionBurstSkillId == _skill.SkillId);
					long total;
					if (battleManager.IsBossInfinityHpBattle())
					{
						long counted = result;
						if (counted > initialHp && (!unitController.IsBoss || !battleManager.IsBossInfinityHpMode())) counted = initialHp;
						if (unitController.IsBoss) battleManager.TotalBossDamage = unchecked(battleManager.TotalBossDamage + counted);
						if (battleManager.BattleCategory == (eBattleCategory)20) { battleManager.SekaiAccumulateDamage = unchecked(battleManager.SekaiAccumulateDamage + counted); total = battleManager.SekaiAccumulateDamage; }
						else total = battleManager.TotalBossDamage;
					}
					else total = unchecked(accumulateDamage + result);
					if (ub) SingletonMonoBehaviour<BattleHeaderController>.Instance.BossTotalDamage.SetDamageUB(total, _damageData.DamegeEffectType, unitController, Vector3.zero);
					else SingletonMonoBehaviour<BattleHeaderController>.Instance.BossTotalDamage.SetDamageNormal(total);
				}
				if (unitController.IsBoss)
				{ if (battleManager.IsBossInfinityHpBattle() && battleManager.IsBossInfinityHpMode()) SingletonMonoBehaviour<BattleHeaderController>.Instance.BossGauge.UpdateInvalidLifeAmount(battleManager.GetInvalidLifeAmount(componentParameter.MaxHp)); else SetBossGaugeValue(); }
				else { unitController.LifeGauge.SetNormalizedLifeAmount(life, componentParameter.MaxLifeAmount, false, false); OnLifeAmmountChange.Call(life); }
			}
			createDamageEffectFromSetDamageImpl(_damageData, _hasEffect, _skill, _critical, result, ignoreComboReset);
			OnDamage?.Invoke(_byAttack, result, _critical);
			OnDamageForLoopTrigger.Call(_byAttack, result, _critical);
			OnDamageForLoopRepeat.Call(result);
			OnDamageForDivision.Call(_byAttack, Math.Min(initialHp, result), _critical);
			OnDamageForSpecialSleepRelease.Call(_byAttack);
			foreach (var item in OnDamageListForChangeSpeedDisableByAttack) item.Value.Call(_byAttack);
			if (result > 0L) foreach (var item in OnDamageListForSpyDisableByAttack) if (_damageData.ActionType != (eActionType)9) item.Value.Call(_byAttack);
			OnHpChange?.Invoke(_byAttack, result, _critical);
			OnHpChangeForDamagedHP?.Invoke(_byAttack, result, _critical);
			if (battleManager.IsSpecialBattle && unitController.IsBoss && componentState.SpecialBattlePurposeHp != 0L && (long)componentParameter.Hp < componentState.SpecialBattlePurposeHp && battleManager.IsSpecialBattleModeChangeOnHpChange(unitController.UnitId)) battleManager.SpecialBattleModeChangeOnHpChange();
			if (!componentState.HasUnDeadTime)
			{
				if (!_noMotion) componentState.PlayDamageWhenIdle(true, _skill != null && _skill.PauseStopState);
				if ((long)componentParameter.Hp <= 0L && !componentState.IsDead && componentState.CurrentState < ActionState.DIE)
				{
					if (initialHpForHit > 0L && _damageData.Source != null)
					{ KillBonusTarget = _damageData.Source.ComponentAbnormal.IsAbnormalState(eAbnormalState.FEAR) ? null : _damageData.Source; _onDefeat.Call(); }
					if (initialHpForHit > 0L)
					{
						if (componentState.CurrentState == ActionState.UNION_BURST && _damageData.ActionType == (eActionType)59) { unitController.StopZoomEffect(); battleManager.StopScaleChange(); battleManager.SetBlackoutTimeZero(); }
						componentState.SetState(ActionState.DIE, 0, 0, false);
					}
				}
			}
			return result;
		}

		private long accumulateDamage { get; set; }
		public OnDamageDelegate OnHpChangeForDamagedHP { get; set; }
		public Action<DamageData, ActionParameter> OnDamageForExecToSource { get; set; }
		public Action<bool, long, bool> OnDamageForDivision { get; set; }

		[global::OpenPCR.Migration("88345582-e0a1-44fe-b880-762c0a45b3f8")]
		private void applyReductionDamage(DamageData _damageData, Skill _skill, int _actionId, ref double _dDamage, ref double _damageForEnergy, bool _isIgnoreDamageCutAction, ref bool _isNoDamageByResist)
		{
			if (_isIgnoreDamageCutAction) return;
			ActionParameter action = _skill == null ? null : _skill.ActionParameters.Find(e => e.ActionId == _actionId);
			if (_damageData.DamageType == DamageData.eDamageType.ATK && componentAbnormal.IsAbnormalState(eAbnormalState.REDUCTION_ATK_DAMAGE))
				applyReductionDamageImpl(_damageData, ref _dDamage, ref _damageForEnergy, eAbnormalStateCategory.REDUCTION_ATK_DAMAGE, action == null ? null : action.ReductionDamageAtkDictionary, action == null ? null : action.ReductionDamageAtkDictionaryForEnergy, ref _isNoDamageByResist);
			if (_damageData.DamageType == DamageData.eDamageType.MGC && componentAbnormal.IsAbnormalState(eAbnormalState.REDUCTION_MGC_DAMAGE))
				applyReductionDamageImpl(_damageData, ref _dDamage, ref _damageForEnergy, eAbnormalStateCategory.REDUCTION_MGC_DAMAGE, action == null ? null : action.ReductionDamageMgcDictionary, action == null ? null : action.ReductionDamageMgcDictionaryForEnergy, ref _isNoDamageByResist);
			if (componentAbnormal.IsAbnormalState(eAbnormalState.REDUCTION_ALL_DAMAGE))
				applyReductionDamageImpl(_damageData, ref _dDamage, ref _damageForEnergy, eAbnormalStateCategory.REDUCTION_ALL_DAMAGE, action == null ? null : action.ReductionDamageAllDictionary, action == null ? null : action.ReductionDamageAllDictionaryForEnergy, ref _isNoDamageByResist);
		}

		[global::OpenPCR.Migration("bf776a5b-db1d-4396-b8bd-65d28b7d3565")]
		private void applyReductionDamageImpl(DamageData _damageData, ref double _dDamage, ref double _damageForEnergy, eAbnormalStateCategory _abnormalStateCategory, Dictionary<BasePartsData, double> _reductionDamageDictionary, Dictionary<BasePartsData, double> _reductionDamageDictionaryForEnergy, ref bool _isNoDamageByResist)
		{
			int actionType = (int)_damageData.ActionType;
			double main = componentAbnormal.GetAbnormalStateMainValue(_abnormalStateCategory);
			double prior = 0d, priorEnergy = 0d;
			if (_reductionDamageDictionary != null)
			{
				_reductionDamageDictionary.TryGetValue(_damageData.Target, out prior);
				_reductionDamageDictionaryForEnergy.TryGetValue(_damageData.Target, out priorEnergy);
			}
			double remaining = Math.Max(0d, main - prior), remainingEnergy = Math.Max(0d, main - priorEnergy);
			bool stored = actionType == 1 || actionType == 46 || actionType == 47;
			if (remaining > 0d)
			{
				double reduction = Math.Min(_dDamage, remaining);
				if (BattleUtil.DoubleToLong(_dDamage) <= 0L) _isNoDamageByResist = true;
				if (reduction > 0d)
				{
					_dDamage -= reduction;
					if (_reductionDamageDictionary != null && stored) _reductionDamageDictionary[_damageData.Target] = reduction + prior;
					if (BattleUtil.DoubleToLong(_dDamage) <= 0L) _isNoDamageByResist = true;
				}
			}
			if (remainingEnergy > 0d)
			{
				double reduction = Math.Min(_damageForEnergy, remainingEnergy);
				if (reduction > 0d)
				{
					_damageForEnergy -= reduction;
					if (_reductionDamageDictionaryForEnergy != null && stored) _reductionDamageDictionaryForEnergy[_damageData.Target] = reduction + priorEnergy;
				}
			}
		}

	}
}
