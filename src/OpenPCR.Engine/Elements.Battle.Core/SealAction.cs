using global::Elements;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class SealAction : ActionParameter
	{
		public enum eCancelTriggerType
		{
			NONE = 0,
			DAMAGED_COUNT = 1
		}
		[global::OpenPCR.Migration("3551e373-63a2-4fac-a97c-e3d2cd53d7c0")]
		public SealAction() : base()
		{
		}

		[global::OpenPCR.Migration("e1277c1e-6da0-4d50-b77b-333c9372b8ff")]
		public override void Initialize()
		{
			base.Initialize();
			BattleResourceManager.Instance.LoadSealActionEffect();
		}

		[global::OpenPCR.Migration("c7f1291f-727d-42bb-8c5c-bc7da81a6574")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("9e9c24d5-0099-4207-a039-ae9006bfb385")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			eStateIconType icon = (eStateIconType)global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_2]);
			var abnormal = _target.Owner.ComponentAbnormal;
			if (!abnormal.SealDictionary.ContainsKey(icon))
			{
				var value = new SealData
				{
					Max = global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]),
					DisplayCount = ActionDetail1 == 1
				};
				abnormal.SealDictionary.Add(icon, value);
				if (ActionDetail3 == 1)
				{
					var cancel = new SealDamageCountCancelData(global::OpenPCR.NativeNumeric.ToInt32(Value[eValueNumber.VALUE_5]), () => value.RemoveSeal(1, true));
					if (!abnormal.SealCancelDictionary.ContainsKey(eCancelTriggerType.DAMAGED_COUNT))
						abnormal.SealCancelDictionary.Add(eCancelTriggerType.DAMAGED_COUNT, new Dictionary<eStateIconType, SealCancelData>());
					abnormal.SealCancelDictionary[eCancelTriggerType.DAMAGED_COUNT][icon] = cancel;
					value.SealCancelData = cancel;
				}
			}
			else
				abnormal.SealDictionary[icon].Max = Mathf.Max(global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]), abnormal.SealDictionary[icon].Max);
			if (_valueDictionary[eValueNumber.VALUE_4] == 0.0)
				return;
			SealData seal = abnormal.SealDictionary[icon];
			if (seal.GetCurrentCount() == 0 && _valueDictionary[eValueNumber.VALUE_4] > 0.0)
			{
				abnormal.OnChangeState.Call(icon, true);
				if (ActionEffectList.Count != 0)
				{
					var data = ActionEffectList[0];
					var effect = seal.Effect = battleEffectPool.GetEffect(_target.Owner.IsLeftDir ? data.PrefabLeft : data.Prefab);
					effect.transform.parent = ExceptUIRoot.Transform;
					effect.SortTarget = _target.Owner;
					effect.InitializeSort();
					effect.SetPossitionAppearanceType(data, _target, _target.Owner, _skill);
					effect.ExecAppendCoroutine();
				}

				seal.SealCancelData?.OnActivate();
			}

			var addEffect = ActionSubEffectList.Count > 0 ? ActionSubEffectList[0] : null;
			var removeEffect = ActionSubEffectList.Count >= 2 ? ActionSubEffectList[1] : null;
			if (_valueDictionary[eValueNumber.VALUE_4] > 0.0)
			{
				int count = seal.GetCurrentCount();
				seal.AddSeal((float)_valueDictionary[eValueNumber.VALUE_3], _target.Owner, icon, BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_4]));
				if (addEffect != null && count != seal.GetCurrentCount())
				{
					var effect = battleEffectPool.GetEffect(_target.Owner.IsLeftDir ? addEffect.PrefabLeft : addEffect.Prefab, _source);
					effect.transform.parent = ExceptUIRoot.Transform;
					effect.SortTarget = _target.Owner;
					effect.PlaySe(_source.ComponentSound.SoundUnitId, _source.IsLeftDir);
					effect.InitializeSort();
					effect.SetPossitionAppearanceType(addEffect, _target, _target.Owner, _skill);
					effect.ExecAppendCoroutine();
				}
			}

			if (_valueDictionary[eValueNumber.VALUE_4] < 0.0)
			{
				if (removeEffect != null && seal.GetCurrentCount() > 0)
				{
					var effect = battleEffectPool.GetEffect(_target.Owner.IsLeftDir ? removeEffect.PrefabLeft : removeEffect.Prefab, _source);
					effect.transform.parent = ExceptUIRoot.Transform;
					effect.PlaySe(_source.ComponentSound.SoundUnitId, _source.IsLeftDir);
					effect.SortTarget = _target.Owner;
					effect.InitializeSort();
					effect.SetPossitionAppearanceType(removeEffect, _target, _target.Owner, _skill);
					effect.ExecAppendCoroutine();
				}

				seal.RemoveSeal(-BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_4]), false);
			}
		}
	}
}
