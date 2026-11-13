using global::OpenPCR.Foundation;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class UnableStateGuardActionBase : ActionParameter
	{
		[global::OpenPCR.Migration("5a84e21f-4b90-4a0a-a715-c6949e1e3b60")]
		public UnableStateGuardActionBase() : base()
		{
		}

		[global::OpenPCR.Migration("b10e82f8-3ba9-4dcc-aa89-5534a50b874a")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			UnitCtrl owner = _target.Owner;
			int value = OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]);
			bool timeOnly = value == -1;
			int maxCount = timeOnly ? 1 : OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]);
			UnitComponentAbnormal abnormal = owner.ComponentAbnormal;
			if (getSealData(abnormal) != null)
			{
				SealData seal = getSealData(abnormal);
				seal.RemoveSeal(seal.Count, true);
				seal.Max = maxCount;
				seal.DisplayCount = !timeOnly;
			}
			else
				createSeal(abnormal, maxCount, timeOnly);
			global::OpenPCR.Foundation.EventExtension.Call(owner.ComponentAbnormal.OnChangeState, getIconType(), true);
			getSealData(abnormal).AddSeal((float)_valueDictionary[eValueNumber.VALUE_3], owner, getIconType(), maxCount);
		}

		[global::OpenPCR.Migration("c357d705-f2df-41bc-95cc-b2e4a1f0b1ae")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}

		[global::OpenPCR.Migration("191a92a5-7671-4a8d-845b-ab87d3a6f035")]
		protected virtual global::Elements.eStateIconType getIconType()
		{
			return eStateIconType.UNABLE_STATE_GUARD;
		}

		[global::OpenPCR.Migration("cb329050-a244-4819-8876-9b0e92d9c0df")]
		protected virtual global::Elements.Battle.Core.SealData getSealData(global::Elements.Battle.Core.UnitComponentAbnormal _componentAbnormal)
		{
			return _componentAbnormal.UnableStateGuardData;
		}

		[global::OpenPCR.Migration("31598d29-e48e-4b7b-aea8-44d908950bef")]
		protected virtual void createSeal(global::Elements.Battle.Core.UnitComponentAbnormal _componentAbnormal, int _maxCount, bool _isReleaseTimeLimitOnly)
		{
			var seal = new SealData();
			seal.Max = _maxCount;
			seal.DisplayCount = !_isReleaseTimeLimitOnly;
			_componentAbnormal.UnableStateGuardData = seal;
		}
	}
}
