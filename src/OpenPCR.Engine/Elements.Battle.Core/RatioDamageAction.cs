using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class RatioDamageAction : ActionParameter
	{
		private BasePartsData parts;
		private Dictionary<BasePartsData, long> targetCurrentHps;
		[global::OpenPCR.Migration("3816d49c-1ca7-4d02-893c-b47ef829ea35")]
		public RatioDamageAction() : base()
		{
		}

		[global::OpenPCR.Migration("ceff846e-dba5-4ef5-a4f0-05b5b0a5acb8")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			for (int i = 0; i < base.HitOnceKeyList.Count; i++)
			{
				base.HitOnceDic[base.HitOnceKeyList[i]] = false;
			}
		}

		[global::OpenPCR.Migration("eaae0b31-4693-4e48-8102-eb3a3a9382c4")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget);
			if (targetCurrentHps == null)
			{
				targetCurrentHps = new Dictionary<BasePartsData, long>();
			}
		}

		[global::OpenPCR.Migration("59fa1753-9c2a-4cad-ba04-24be74b45575")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (ActionDetail3 != 0)
				EnergyChargeMultiple = (double)ActionDetail3 / -100.0 + 1.0;
			_target.UbAttackHitCount++;
			if (!base.HitOnceDic.ContainsKey(_target))
			{
				base.HitOnceDic.Add(_target, value: false);
				base.HitOnceKeyList.Add(_target);
			}

			var parameter = _target.Owner.ComponentParameter;
			if (!base.HitOnceDic[_target])
			{
				targetCurrentHps[_target] = parameter.Hp;
			}

			long num = 0L;
			switch (base.ActionDetail1)
			{
				case 1:
					{
						long hp = parameter.MaxHp;
						num = BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_1] * (double)hp / 100.0 * ActionExecTimeList[_num].Weight / ActionWeightSum);
						break;
					}
				case 2:
					{
						long hp = targetCurrentHps[_target];
						num = BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_1] * (double)hp / 100.0 * ActionExecTimeList[_num].Weight / ActionWeightSum);
						break;
					}
				case 3:
					{
						long hp = parameter.StartMaxHP;
						num = BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_1] * (double)hp / 100.0 * ActionExecTimeList[_num].Weight / ActionWeightSum);
						break;
					}
			}

			long overrideDamageLimit = _valueDictionary[eValueNumber.VALUE_3] == 0.0 ? -1L : BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_3] * ActionExecTimeList[_num].Weight / ActionWeightSum);
			DamageData damageData = new DamageData
			{
				Target = _target,
				Damage = num,
				DamageType = (DamageData.eDamageType)base.ActionDetail2,
				Source = _source,
				DamageSoundType = DamageData.eDamageSoundType.HIT,
				DamegeEffectType = base.ActionExecTimeList[_num].DamageNumType,
				DamegeNumScale = base.ActionExecTimeList[_num].DamageNumScale,
				IgnoreDef = true,
				LifeSteal = (_source.IsPartsBoss ? parts.GetLifeStealZero() : ((long)_source.ComponentParameterClamped.LifeStealZero)),
				ActionType = eActionType.RATIO_DAMAGE,
				IsAlwaysChargeEnegry = IsAlwaysChargeEnegry,
				OverrideDamageLimit = overrideDamageLimit,
				IgnoreStrikeBackAndAllUnitDamageCutTrigger = IgnoreTriggerCausedByTrigger
			};
			UnitCtrl owner = _target.Owner;
			OnDamageHitDelegate onDamageHit = base.OnDamageHit;
			if (owner.ComponentDamageControl.SetDamage(damageData, _byAttack: true, base.ActionId, onDamageHit, _hasEffect: true, _skill, _energyAdd: true, null, _noMotion: false, base.ActionExecTimeList[_num].Weight, base.ActionWeightSum, null, EnergyChargeMultiple, base.UsedChargeEnergyByReceiveDamage) != 0L)
			{
				base.HitOnceDic[_target] = true;
			}
		}

		[global::OpenPCR.Migration("034c2ea7-02f8-40bb-8bf0-7f4373a4424b")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
