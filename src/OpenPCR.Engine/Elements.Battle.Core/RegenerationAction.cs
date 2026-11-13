using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public partial class RegenerationAction : ActionParameter
	{
		public enum eTargetType { HP = 1, TP, HP2, TP2 }
		private static readonly Dictionary<eTargetType, eAbnormalState> ABNORMAL_STATE_DIC = new Dictionary<eTargetType, eAbnormalState> {
			{ eTargetType.HP, eAbnormalState.REGENERATION }, { eTargetType.TP, eAbnormalState.TP_REGENERATION },
			{ eTargetType.HP2, eAbnormalState.REGENERATION2 }, { eTargetType.TP2, eAbnormalState.TP_REGENERATION2 }
		};

		private BasePartsData parts;
		[global::OpenPCR.Migration("ca0e418e-3e93-4734-b60b-9f9ffc9d2e3f")]
		public RegenerationAction() : base()
		{
		}

		[global::OpenPCR.Migration("2b6858cc-c355-41bd-863f-b61ae5737042")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget);
		}

		[global::OpenPCR.Migration("9bf5e0ff-e5f5-468a-87f1-c24784771c12")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			long status = ActionDetail1 == 1 ? (_source.IsPartsBoss ? parts.GetAtkZero() : (long)_source.ComponentParameterClamped.AtkZero) : (_source.IsPartsBoss ? parts.GetMagicStrZero() : (long)_source.ComponentParameterClamped.MagicStrZero);
			UnitComponentAbnormal abnormal = _target.Owner.ComponentAbnormal;
			eAbnormalState state = ABNORMAL_STATE_DIC[(eTargetType)ActionDetail2];
			double duration = _valueDictionary[eValueNumber.VALUE_5];
			long value = calculateHealValue(_source, status, _valueDictionary);
			int parameter = ActionDetail1;
			bool showsIcon = _valueDictionary[eValueNumber.VALUE_7] == 0.0;
			abnormal.SetAbnormalState(_source, state, (float)duration, this, _skill, (double)value, (double)parameter, false, false, 1.0, showsIcon, false, false);
		}

		[global::OpenPCR.Migration("41b69f86-cb24-40b3-9e33-4ec712c1568a")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
			Value[eValueNumber.VALUE_5] = MasterData.action_value_5 + MasterData.action_value_6 * _level;
		}

		[global::OpenPCR.Migration("9d8c57fd-8903-43c5-9aa2-7e541683c353")]
		private long calculateHealValue(UnitCtrl _source, long _statusValue, Dictionary<eValueNumber, double> _valueDictionary)
		{
			switch (ActionDetail2)
			{
				case 1:
				case 3:
					double value = _valueDictionary[eValueNumber.VALUE_1];
					double rate = _valueDictionary[eValueNumber.VALUE_3];
					long enhancement = IsExEquipSkillAction ? 0L : (long)_source.ComponentParameterClamped.HealEnhanceZero;
					return BattleUtil.DoubleToLong((value + rate * (double)_statusValue) * ((double)enhancement + 10000.0) / 10000.0);
				case 2:
				case 4:
					return BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_1]);
				default:
					return 0L;
			}
		}
	}
}
