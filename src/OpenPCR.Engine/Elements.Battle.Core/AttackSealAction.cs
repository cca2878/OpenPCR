using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AttackSealAction : ActionParameter
	{
		[global::OpenPCR.Migration("f1904f82-ef60-48c0-b237-3a94207bd9de")]
		public AttackSealAction() : base()
		{
		}

		[global::OpenPCR.Migration("35f78d1b-e30b-4281-b733-758655b40967")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("6841570c-354c-4553-9b72-f22261959c40")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			var icon = (eStateIconType)global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_2]);
			var abnormal = _target.Owner.ComponentAbnormal;
			if (!abnormal.SealDictionary.ContainsKey(icon))
				abnormal.SealDictionary.Add(icon, new SealData { Max = global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]), DisplayCount = ActionDetail2 == 1 });
			Dictionary<UnitCtrl, Dictionary<int, AttackSealData>> dictionary = null;
			if (ActionDetail1 == 1)
			{
				if (ActionDetail3 == 1)
					dictionary = abnormal.DamageOnceOwnerSealDateDictionary;
			}
			else if (ActionDetail1 >= 3 && ActionDetail1 <= 5)
			{
				if (ActionDetail3 == 1) dictionary = abnormal.DamageOwnerSealDataDictionary;
				else if (ActionDetail3 == 0) dictionary = abnormal.DamageSealDataDictionary;
			}
			if (dictionary.ContainsKey(_source) && dictionary[_source].ContainsKey(ActionId))
			{
				dictionary[_source][ActionId].LimitTime = (float)_valueDictionary[eValueNumber.VALUE_5];
				return;
			}
			var data = new AttackSealData
			{
				LimitTime = (float)_valueDictionary[eValueNumber.VALUE_5],
				SealEffectTime = (float)_valueDictionary[eValueNumber.VALUE_3],
				IconType = icon,
				ActionId = ActionId,
				ExecConditionType = (AttackSealData.eExecConditionType)ActionDetail1
			};
			if (dictionary.ContainsKey(_source))
				dictionary[_source].Add(ActionId, data);
			else
				dictionary.Add(_source, new Dictionary<int, AttackSealData> { { ActionId, data } });
			_sourceActionController.AppendCoroutine(updateAttackSealData(data, _source, () =>
			{
				if (dictionary.ContainsKey(_source))
					dictionary[_source].Remove(ActionId);
			}), ePauseType.SYSTEM, _source);
		}

		[global::OpenPCR.Migration("01055de0-6f19-47dd-8f96-ed06a5738787")]
		private IEnumerator updateAttackSealData(AttackSealData _attackSealData, UnitCtrl _unitCtrl, Action _callback)
		{
			var state = _unitCtrl.ComponentState;
			while (true)
			{
				if (state.IdleOnly)
					yield break;
				_attackSealData.LimitTime -= _unitCtrl.DeltaTimeForPause;
				if (_attackSealData.LimitTime < 0f)
					break;
				yield return null;
			}
			_callback.Call();
		}

		[global::OpenPCR.Migration("6b1a03fd-d6a3-4c36-8adb-71d922378644")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
			Value[eValueNumber.VALUE_5] = MasterData.action_value_5 + MasterData.action_value_6 * _level;
		}
	}
}
