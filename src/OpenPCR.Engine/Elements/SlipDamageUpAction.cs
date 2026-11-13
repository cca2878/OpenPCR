using System.Collections.Generic;
using Elements.Battle.Core;

namespace Elements
{
	public class SlipDamageUpAction : ActionParameter
	{
		[global::OpenPCR.Migration("30e13eff-962d-4c03-979f-0344fe2bb0c6")]
		public SlipDamageUpAction()
		{ }

		[global::OpenPCR.Migration("11dfe7de-df8f-48d5-b71d-819d034c1feb")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart((global::Elements.Skill)(@_skill), (global::Elements.UnitCtrl)(@_source), (global::Elements.UnitActionController)(@_sourceActionController));
		}

		[global::OpenPCR.Migration("ca4826b0-3ac6-486c-81cd-6a96d4af936a")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			int detail = ActionDetail1;
			AppendIsAlreadyExeced(_target.Owner, _num);
			var data = new SlipDamageUpData();
			data.Value = _valueDictionary[eValueNumber.VALUE_1] / 100.0;
			data.Timer = (float)_valueDictionary[eValueNumber.VALUE_2];
			data.LimitDamage = BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_7]);
			data.Enable = true;
			data.DisplayIcon = detail == 0;
			data.StartTimer(_target.Owner);
			if (global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_3]) == -1)
			{
				_target.Owner.ComponentAbnormal.EnchantAllSlipDamageUp(data);
			}
			else
			{
				var types = new List<eSlipDamageType>();
				System.Action<int> append = value =>
				{
					if (value == -1) return;
					types.Add((eSlipDamageType)value);
					switch (value)
					{
						case 1: types.Add((eSlipDamageType)7); break;
						case 3: types.Add((eSlipDamageType)8); break;
						case 7: types.Add((eSlipDamageType)1); break;
						case 8: types.Add((eSlipDamageType)3); break;
					}
				};
				append(global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_3]));
				append(global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_4]));
				append(global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_5]));
				append(global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_6]));
				foreach (var type in types)
					_target.Owner.ComponentAbnormal.EnchantSlipDamageUp(data, SlipDamageAction.AbnormalStateDic[type]);
			}
		}

		[global::OpenPCR.Migration("81017ee2-98d6-4b7e-a9cf-11a1d538912a")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(unchecked((int)(unchecked((uint)(@_level)))));
		}
	}
}
