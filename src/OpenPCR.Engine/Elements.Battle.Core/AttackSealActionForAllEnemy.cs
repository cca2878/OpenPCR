using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AttackSealActionForAllEnemy : ActionParameter
	{
		[global::OpenPCR.Migration("6f52ba63-bb97-462a-a444-51e6dae544dd")]
		public AttackSealActionForAllEnemy() : base()
		{
		}

		[global::OpenPCR.Migration("4d3772c1-c266-41e5-9910-bf6cff2643be")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			var owner = _target.Owner;
			var icon = (eStateIconType)global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_2]);
			var dictionary = owner.ComponentAbnormal.AttackSealDataDictionary;
			if (dictionary.ContainsKey(ActionId))
			{
				dictionary[ActionId].LimitTime = (float)_valueDictionary[eValueNumber.VALUE_5];
				return;
			}
			var data = new AttackSealDataForAllEnemy(icon,
				BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]), ActionDetail2 == 1,
				(AttackSealData.eExecConditionType)ActionDetail1,
				(float)_valueDictionary[eValueNumber.VALUE_3], (float)_valueDictionary[eValueNumber.VALUE_5]);
			dictionary[ActionId] = data;
			_sourceActionController.AppendCoroutine(updateAttackSealData(data, owner, () => dictionary.Remove(ActionId)), ePauseType.SYSTEM, _source);
		}

		[global::OpenPCR.Migration("c338a6d7-6163-47c8-9dfe-fc769d650d1b")]
		private IEnumerator updateAttackSealData(AttackSealDataForAllEnemy _attackSealData, UnitCtrl _unitCtrl, Action _callback)
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

		[global::OpenPCR.Migration("e49b80e8-4225-4d65-9dc0-c26b602fc37b")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
			Value[eValueNumber.VALUE_5] = MasterData.action_value_5 + MasterData.action_value_6 * _level;
		}
	}
}
