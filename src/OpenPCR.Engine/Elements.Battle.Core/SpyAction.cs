using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public partial class SpyAction : ActionParameter
	{
		public enum eSpyType
		{
			SPY = 1
		}

		public enum eCancelTriggerType
		{
			NONE,
			DAMAGED
		}

		public class eSpyType_DictComparer : IEqualityComparer<eSpyType>
		{
			public bool Equals(eSpyType _x, eSpyType _y) => _x == _y;
			public int GetHashCode(eSpyType _obj) => (int)_obj;
		}

		public static readonly Dictionary<eSpyType, eAbnormalState> AbnormalStateDic = new Dictionary<eSpyType, eAbnormalState>(new eSpyType_DictComparer())
		{
			{
				eSpyType.SPY,
				eAbnormalState.SPY
			}
		};
		[global::OpenPCR.Migration("e5098bfb-7488-4f10-9a1e-5c66e28f5d3b")]
		public SpyAction() : base()
		{
		}

		[global::OpenPCR.Migration("7ff47751-59f0-4f27-bc90-3e7fae7ced6b")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("a48c02f8-4b73-4b50-b22e-20a707837e23")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			eAbnormalState abnormalState = AbnormalStateDic[(eSpyType)base.ActionDetail1];
			List<UnitCtrl> targetList = (_target.Owner.IsOther ? base.battleManager.EnemyList : base.battleManager.UnitList);
			var abnormal = _target.Owner.ComponentAbnormal;
			foreach (var tauntTarget in abnormal.TauntTargetList)
				tauntTarget.ComponentAbnormal.EnableAbnormalState(eAbnormalState.TAUNT, false, false, false, false);
			abnormal.TauntTargetList.Clear();
			abnormal.SetAbnormalState(_source, abnormalState, (base.AbnormalStateFieldAction == null) ? (float)_valueDictionary[eValueNumber.VALUE_1] : 90f, this, _skill);
			_target.Owner.AppendCoroutine(updateSpy(_target.Owner, targetList), ePauseType.SYSTEM, _target.Owner);
			eCancelTriggerType actionDetail = (eCancelTriggerType)base.ActionDetail2;
			if (actionDetail != eCancelTriggerType.DAMAGED || _target.Owner.ComponentDamageControl.OnDamageListForSpyDisableByAttack.ContainsKey(base.ActionId))
			{
				return;
			}

			_target.Owner.ComponentDamageControl.OnDamageListForSpyDisableByAttack.Add(base.ActionId, delegate
			{
				if (abnormal.IsAbnormalState(abnormalState))
				{
					abnormal.DisableAbnormalStateById(abnormalState, base.ActionId, _isReleasedByDamage: true);
				}
			});
		}

		[global::OpenPCR.Migration("10438382-3f47-462a-8c3d-90c866b120d5")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}

		[global::OpenPCR.Migration("9be058da-1e23-46f4-a473-4f527d67e3ee")]
		private IEnumerator updateSpy(UnitCtrl _target, List<UnitCtrl> _targetList)
		{
			yield return null;
			while (true)
			{
				if (!_target.ComponentAbnormal.IsAbnormalState(eAbnormalState.SPY) || _target.ComponentState.IsDead)
				{
					yield break;
				}

				if (!_targetList.Exists((UnitCtrl e) => ((!e.ComponentState.IsDead && (long)e.ComponentParameter.Hp > 0) || e.ComponentState.HasUnDeadTime) && !e.ComponentAbnormal.IsStealth && !e.ComponentAbnormal.IsAbnormalState(eAbnormalState.SPY)))
				{
					break;
				}

				yield return null;
			}

			foreach (UnitCtrl _target2 in _targetList)
			{
				_target2.ComponentAbnormal.DispelAbnormalState(eAbnormalState.SPY);
				foreach (var (_, sealData) in _target2.ComponentAbnormal.SealDictionary)
				{
					if (sealData.IsEtherealization)
					{
						sealData.RemoveSeal(sealData.Count, true);
					}
				}
			}
		}
	}
}
