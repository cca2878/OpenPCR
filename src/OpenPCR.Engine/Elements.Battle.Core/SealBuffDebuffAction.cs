using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class SealBuffDebuffAction : ActionParameter
	{
		[global::OpenPCR.Migration("ecab471d-d87c-430e-a7e0-30a0c5d966c7")]
		public SealBuffDebuffAction() : base()
		{
		}

		[global::OpenPCR.Migration("506723a3-6d49-4190-8319-35faa0821a4c")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("ff1e5ff0-9c1f-47b2-bfe7-a12ad5941a06")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			eStateIconType icon = (eStateIconType)ActionDetail2;
			UnitComponentAbnormal abnormal = _target.Owner.ComponentAbnormal;
			BuffParamKind kind = BuffDebuffAction.GetChangeParamKind(ActionDetail1);
			Dictionary<BasePartsData, long> values = new Dictionary<BasePartsData, long>();
			if (_target.Owner.IsPartsBoss && !AbnormalStateDefine.IS_EFFECT_BOSS_BODY_ONLY[kind])
			{
				List<PartsData> parts = _target.Owner.ComponentMultiPartsBoss.BossPartsListForBattle;
				for (int i = 0; i < parts.Count; i++)
					values.Add(parts[i], BuffDebuffAction.CalculateBuffDebuffParam(parts[i], _valueDictionary[eValueNumber.VALUE_2], (BuffDebuffAction.eChangeParameterType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]), kind, ActionDetail1 % 10 == 1));
			}
			else
				values.Add(_target.Owner.ComponentMultiPartsBoss.DummyPartsData, BuffDebuffAction.CalculateBuffDebuffParam(_target, _valueDictionary[eValueNumber.VALUE_2], (BuffDebuffAction.eChangeParameterType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]), kind, ActionDetail1 % 10 == 1));
			SealBuffDebuffData data = new SealBuffDebuffData
			{
				Source = _source,
				TargetComponent = _target.Owner.ComponentAbnormal,
				ValuePerSealDictionary = values,
				TargetParameter = kind,
				Target = _target.Owner,
				Duration = _valueDictionary[eValueNumber.VALUE_4]
			};
			if (abnormal.SealDictionary.ContainsKey(icon))
			{
				if (abnormal.SealDictionary[icon].SealBuffDebuffDataDictionary != null)
				{
					if (abnormal.SealDictionary[icon].SealBuffDebuffDataDictionary.ContainsKey(ActionDetail1))
						abnormal.SealDictionary[icon].SealBuffDebuffDataDictionary[ActionDetail1].DeleteValue();
				}
				else
					abnormal.SealDictionary[icon].SealBuffDebuffDataDictionary = new Dictionary<int, SealBuffDebuffData>();
				abnormal.SealDictionary[icon].SealBuffDebuffDataDictionary[ActionDetail1] = data;
				data.ParentSealData = abnormal.SealDictionary[icon];
			}
			else
			{
				SealData seal = new SealData();
				Dictionary<int, SealBuffDebuffData> dictionary = new Dictionary<int, SealBuffDebuffData>();
				dictionary.Add(ActionDetail1, data);
				seal.SealBuffDebuffDataDictionary = dictionary;
				seal.DisplayCount = true;
				abnormal.SealDictionary.Add(icon, seal);
				data.ParentSealData = seal;
			}

			data.UpdateValue(abnormal.SealDictionary[icon].GetCurrentCount());
			data.StartTimer();
		}

		[global::OpenPCR.Migration("f2bc9282-1201-4484-aafb-41bc9762b581")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
		}

		[global::OpenPCR.Migration("c327827d-687e-468b-bbed-a3eaa7233e67")]
		public override double GetActionBaseValue(UnitCtrl _source, eValueNumber _type)
		{
			Value.TryGetValue(_type, out double value);
			if (_type != eValueNumber.VALUE_2)
				return value;
			BuffParamKind kind = BuffDebuffAction.GetChangeParamKind(ActionDetail1);
			if (!BuffDebuffAction.BUFF_DEBUFF_ENHANCE_TARGET_TYPE.Contains(kind) || IsExEquipSkillAction)
				return value;
			long enhance = ActionDetail1 % 10 == 1
				? (long)_source.ComponentParameterClamped.DebuffEnhanceZero
				: (long)_source.ComponentParameterClamped.BuffEnhanceZero;
			return value * ((double)enhance / 10000.0 + 1.0);
		}
	}
}
