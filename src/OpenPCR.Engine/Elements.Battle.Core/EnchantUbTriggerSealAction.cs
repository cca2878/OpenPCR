using global::OpenPCR.Foundation;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class EnchantUbTriggerSealAction : ActionParameter
	{
		private ActionParameter targetAction;

		[global::OpenPCR.Migration("308d33d5-b92f-49fc-877d-10f341a4bf0b")]
		public EnchantUbTriggerSealAction() : base()
		{
		}

		[global::OpenPCR.Migration("d9b70f10-4b93-4493-b19d-1d847baab879")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			if (ActionDetail1 != 0)
			{
				targetAction = _skill.ActionParameters.Find(value => value.ActionId == ActionDetail1);
				targetAction.TriggerCall = false;
			}
		}

		[global::OpenPCR.Migration("c73f3df6-947b-41df-bf05-9c48ecfd44c8")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			if (targetAction != null)
			{
				targetAction.NoDelayExecByTriggerOnly = true;
				foreach (int index in targetAction.ActionChildrenIndexes)
					_skill.ActionParameters[index].NoDelayExecByTriggerOnly = true;
			}
		}

		[global::OpenPCR.Migration("a54fb166-0f06-4f5c-88fa-a07c8d1a8b50")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			UnitCtrl owner = _target.Owner;
			AppendIsAlreadyExeced(owner, _num);
			eStateIconType icon = (eStateIconType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]);
			var abnormal = owner.ComponentAbnormal;
			if (abnormal.SealDictionary.ContainsKey(icon))
			{
				var old = abnormal.SealDictionary[icon];
				old.RemoveSeal(old.Count, true);
			}
			double maximum = _valueDictionary[eValueNumber.VALUE_3];
			var seal = new SealData
			{
				SourceForMonitorAlive = _source,
				Max = double.IsPositiveInfinity(_valueDictionary[eValueNumber.VALUE_3]) ? int.MinValue : global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_3]),
				DisplayCount = !double.IsPositiveInfinity(maximum) && global::OpenPCR.NativeNumeric.ToInt32(maximum) > 1
			};
			abnormal.SealDictionary[icon] = seal;
			abnormal.OnChangeState.Call(icon, true);
			seal.AddSeal((float)_valueDictionary[eValueNumber.VALUE_4], owner, icon, BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_3]));
			abnormal.UbTriggerSealActionDictionary[icon] = () =>
			{
				if (_source == null || _source.ComponentState.IsDead)
				{
					if (seal.GetCurrentCount() >= 1) seal.RemoveSeal(seal.Count, true);
				}
				else if (seal.GetCurrentCount() != 0)
				{
					_source.UnitActionController.ExecSkillWithTargetPartsAndActionList(_skill, _target, new List<ActionParameter> { targetAction }, false);
					seal.RemoveSeal(1, true);
				}
			};
		}

		[global::OpenPCR.Migration("aec0dabc-826c-468c-a67b-e8083a42c435")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
