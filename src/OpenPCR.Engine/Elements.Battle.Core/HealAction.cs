using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class HealAction : ActionParameter
	{
		private BasePartsData parts;
		[global::OpenPCR.Migration("796cd5cf-a85f-400c-8fcf-ab1bed2eb120")]
		public HealAction() : base()
		{
		}

		[global::OpenPCR.Migration("61167dcf-882f-49e9-ba12-b4e2bee27d6d")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget);
		}

		[global::OpenPCR.Migration("278a8814-c902-4749-85bc-c720887c0f7c")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			long amount = 0L;
			bool physical = ActionDetail1 == 1;
			var clamped = _source.ComponentParameterClamped;
			switch (BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]))
			{
				case 1:
					long attack = _source.IsPartsBoss ? (physical ? parts.GetAtkZero() : parts.GetMagicStrZero()) : (physical ? (long)clamped.AtkZero : (long)clamped.MagicStrZero);
					amount = unchecked(BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_2]) + BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_4] * (double)attack));
					break;
				case 2:
					long maxHp = (long)_target.Owner.ComponentParameter.MaxHp;
					amount = BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_2] * (double)maxHp / 100.0);
					break;
			}

			long recoveryRate = _source.IsPartsBoss ? parts.GetHpRecoverRateZero() : (long)clamped.HpRecoveryRateZero;
			long enhance = IsExEquipSkillAction ? 0L : (long)_source.ComponentParameterClamped.HealEnhanceZero;
			amount = BattleUtil.DoubleToLong(((double)recoveryRate + 100.0) * (double)amount * ((double)enhance + 10000.0) / 100.0 / 10000.0);
			int hitCount = ExecTime.Length;
			amount = BattleUtil.DoubleToLong(_skill.AweValue * (double)(amount / hitCount));
			_target.Owner.ComponentDamageControl.SetRecovery(amount, 0L, physical ? eInhibitHealType.PHYSICS : eInhibitHealType.MAGIC, _source, false, null, _source.ComponentAbnormal.GetHealDownValue(), EffectType == eEffectType.COMMON, false, false, false, true, _target, true);
		}

		[global::OpenPCR.Migration("f317eaaf-0fbb-4411-9b46-5b08e4b72a90")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
			Value[eValueNumber.VALUE_4] = MasterData.action_value_4 + MasterData.action_value_5 * _level;
		}
	}
}
