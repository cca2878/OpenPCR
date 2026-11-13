using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class AddAbnormalEffectTimeAction : ActionParameter
	{
		[global::OpenPCR.Migration("3a5651c8-ca67-4f03-9d9f-7d7df392bfa3")]
		public AddAbnormalEffectTimeAction() : base()
		{
		}

		[global::OpenPCR.Migration("926e747f-ab42-492e-b2d6-859b9681b22e")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("9b0941d4-fcc1-4499-bfcb-06e9b2d305be")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			foreach (eAbnormalState state in createTargetStateList(_valueDictionary))
			{
				var dictionary = _target.Owner.ComponentAbnormal.AddAbnormalEffectTimeDataDictionary;
				var data = new AddAbnormalEffectTimeData();
				data.Value = (float)_valueDictionary[eValueNumber.VALUE_2];
				data.ValueType = (AddAbnormalEffectTimeData.eValueType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]);
				dictionary[state] = data;
			}
		}

		[global::OpenPCR.Migration("c487bd9f-cc80-464c-9824-eea53a7b7fbf")]
		private List<eAbnormalState> createTargetStateList(Dictionary<eValueNumber, double> _valueDictionary)
		{
			var result = new List<eAbnormalState>();
			System.Action<double> add = value =>
			{
				if (value == -1.0)
					return;
				int key = double.IsNaN(value) ? 0 : value >= int.MaxValue ? int.MaxValue : value <= int.MinValue ? int.MinValue : (int)value;
				switch (ActionDetail1)
				{
					case 8:
						result.Add(ChangeSpeedAction.AbnormalStateDic[(ChangeSpeedAction.eChangeSpeedType)key]);
						break;
					case 9:
						result.Add(SlipDamageAction.AbnormalStateDic[(eSlipDamageType)key]);
						break;
					case 11:
						switch (key)
						{
							case 0:
								result.Add(eAbnormalState.CONVERT);
								break;
							case 1:
								result.Add(eAbnormalState.CONFUSION);
								break;
							case 2:
								result.Add(eAbnormalState.CONFUSION2);
								break;
						}

						break;
					case 12:
						result.Add(BlindAction.AbnormalStateDic[(BlindAction.eBlindType)key]);
						break;
					case 13:
						switch (key)
						{
							case 0:
								result.Add(eAbnormalState.SILENCE);
								break;
							case 1:
								result.Add(eAbnormalState.UB_SILENCE);
								break;
						}

						break;
				}
			};
			add(_valueDictionary[eValueNumber.VALUE_4]);
			add(_valueDictionary[eValueNumber.VALUE_5]);
			add(_valueDictionary[eValueNumber.VALUE_6]);
			add(_valueDictionary[eValueNumber.VALUE_7]);
			return result;
		}

		[global::OpenPCR.Migration("abde632c-5224-489f-b161-2bab13a2106b")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
		}
	}
}
