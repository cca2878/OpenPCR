using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class ActionByHitCountAction : ActionParameter
	{

		private ActionParameter execAction1;
		private ActionParameter execAction2;
		[global::OpenPCR.Migration("1b164fec-2719-43b3-90b6-96b7cfe9df7d")]
		public ActionByHitCountAction() : base()
		{
		}

		[global::OpenPCR.Migration("ef874ee7-93e9-4618-a3d8-f9c3b53b4ccd")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			execAction1 = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2);
			execAction2 = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail3);
		}

		[global::OpenPCR.Migration("12824b90-94c3-4d3a-9da5-cdec3db7b5ee")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			if (execAction1 != null)
			{
				execAction1.CancelByIfForAll = true;
			}

			if (execAction2 != null)
			{
				execAction2.CancelByIfForAll = true;
			}
		}

		[global::OpenPCR.Migration("675554b9-f1b5-499a-86a1-d8116c5fe940")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			if (_target.Owner != _source)
				return;
			Action callback = null;
			var data = new ActionByHitCountData
			{
				LimitTime = (float)_valueDictionary[eValueNumber.VALUE_3],
				ConditionCount = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]),
				ExecLimit = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_5]),
				StateIconType = (eStateIconType)global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_2]),
				Enable = true
			};
			if (data.StateIconType != eStateIconType.NONE)
				_source.ComponentAbnormal.OnChangeState.Call(data.StateIconType, true);
			callback += () =>
			{
				if (data.LimitTime >= 0f && data.Enable)
				{
					data.HitCounter++;
					if (data.ConditionCount == data.HitCounter && (data.ExecLimit == 0 || data.ExecLimit != data.ExecCounter))
					{
						data.ExecCounter++;
						if (data.ExecLimit != 0 && data.ExecLimit == data.ExecCounter)
						{
							if (data.StateIconType != eStateIconType.NONE)
								_source.ComponentAbnormal.OnChangeState.Call(data.StateIconType, false);
							data.Enable = false;
						}
						_sourceActionController.SearchTargetByAction(_skill.SkillId);
						data.HitCounter = 0;
						if (execAction1 != null)
						{
							execAction1.CancelByIfForAll = false;
							execAction1.ResetHitData();
							_sourceActionController.ExecUnitActionNoDelay(execAction1, _skill);
						}
						if (execAction2 != null)
						{
							execAction2.CancelByIfForAll = false;
							execAction2.ResetHitData();
							_sourceActionController.ExecUnitActionNoDelay(execAction2, _skill);
						}
					}
				}
			};
			switch (ActionDetail1)
			{
				case 3: _source.ComponentDamageControl.OnActionByDamage += callback; break;
				case 4: _source.ComponentDamageControl.OnActionByCritical += callback; break;
				case 1: _source.ComponentDamageControl.OnActionByDamageOnce += callback; break;
			}
			_sourceActionController.AppendCoroutine(updateAttackSealData(data, _source), ePauseType.SYSTEM, _source);
		}

		[global::OpenPCR.Migration("544991f4-9df6-460b-a536-21914ae7d055")]
		private IEnumerator updateAttackSealData(ActionByHitCountData _actionByHitCountData, UnitCtrl _unitCtrl)
		{
			var state = _unitCtrl.ComponentState;
			while (true)
			{
				if (state.IdleOnly)
				{
					if (_actionByHitCountData.StateIconType != eStateIconType.NONE)
						_unitCtrl.ComponentAbnormal.OnChangeState.Call(_actionByHitCountData.StateIconType, false);
					_actionByHitCountData.Enable = false;
					yield break;
				}
				_actionByHitCountData.LimitTime -= _unitCtrl.DeltaTimeForPause;
				if (!(_actionByHitCountData.LimitTime >= 0f))
					break;
				yield return null;
			}
			if (_actionByHitCountData.StateIconType != eStateIconType.NONE)
				_unitCtrl.ComponentAbnormal.OnChangeState.Call(_actionByHitCountData.StateIconType, false);
			_actionByHitCountData.Enable = false;
		}

		[global::OpenPCR.Migration("c7600fbc-80d0-48b0-86e8-1fc298e5d70a")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
