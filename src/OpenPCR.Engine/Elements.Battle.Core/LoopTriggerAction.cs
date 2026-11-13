using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class LoopTriggerAction : ActionParameter
	{
		private ActionParameter actionParameter1 { get; set; }
		private ActionParameter actionParameter2 { get; set; }
		private bool triggerSuccess { get; set; } = true;
		private bool disableCrowdControl { get; set; }
		private float duration { get; set; }

		[global::OpenPCR.Migration("93507ff9-d68b-479e-b9f5-29fe93a516e5")]
		public LoopTriggerAction() : base()
		{
		}

		[global::OpenPCR.Migration("0cb8757d-9ea8-406e-9bb1-312bb0d20215")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			if (base.ActionDetail2 != 0)
			{
				actionParameter1 = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2);
			}

			if (base.ActionDetail3 != 0)
			{
				actionParameter2 = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail3);
			}

			if (ActionDetail1 == 14)
			{
				_source.ComponentAbnormal.OnSetCrowndControlForLoopTriggerList.Add(() =>
				{
					if (disableCrowdControl)
						return true;
					if (triggerSuccess)
						return false;
					triggerSuccess = true;
					disableCrowdControl = true;
					return true;
				});
			}
			else if (ActionDetail1 == 2)
				_source.ComponentDamageControl.OnDamageForLoopTrigger = (bool byAttack, long damage, bool critical) =>
				{
					if (byAttack)
						triggerSuccess = true;
				};
		}

		[global::OpenPCR.Migration("b2282588-a18a-421f-a629-61ad06aa8cf8")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			if (actionParameter1 != null)
			{
				actionParameter1.CancelByIfForAll = true;
			}

			if (actionParameter2 != null)
			{
				actionParameter2.CancelByIfForAll = true;
			}
		}

		[global::OpenPCR.Migration("1a8be529-603a-4fdc-a6f8-1029dc46f7f4")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			triggerSuccess = false;
			duration = (float)_valueDictionary[eValueNumber.VALUE_4];
			_sourceActionController.AppendCoroutine(updateStartMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, _source);
		}

		[global::OpenPCR.Migration("6030ecb6-7579-440d-a454-36f5ef0e276c")]
		private IEnumerator updateStartMotion(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill, _isStartMotion: true))
				{
					triggerSuccess = true;
					disableCrowdControl = false;
					yield break;
				}

				if (!_source.UnitSpineCtrl.IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			_sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, 1, _first: false);
			_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 1, -1, _isLoop: true);
			_sourceActionController.AppendCoroutine(updateLoopMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, _source);
		}

		[global::OpenPCR.Migration("a3df3887-e01c-4be6-aa2d-2410e16ccd16")]
		private IEnumerator updateLoopMotion(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			yield return null;
			float timer = duration;
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill, _isStartMotion: false))
				{
					triggerSuccess = true;
					disableCrowdControl = false;
					yield break;
				}

				if (triggerSuccess)
				{
					if (actionParameter1 != null)
					{
						actionParameter1.CancelByIfForAll = false;
						_sourceActionController.ExecUnitActionWithDelay(actionParameter1, _skill, _first: false, actionParameter1.DepenedActionId != 0);
					}

					if (actionParameter2 != null)
					{
						actionParameter2.CancelByIfForAll = false;
						_sourceActionController.ExecUnitActionWithDelay(actionParameter2, _skill, _first: false, actionParameter2.DepenedActionId != 0);
					}

					for (int i = 0; i < _skill.LoopEffectObjs.Count; i++)
					{
						_skill.LoopEffectObjs[i].SetTimeToDie(value: true);
					}

					_skill.LoopEffectObjs.Clear();
					_sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, 2, _first: false);
					_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 2);
					_sourceActionController.AppendCoroutine(updateEndMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, _source);
					yield break;
				}

				if (battleManager.GetBlackOutUnitLength() == 0)
					timer -= _source.DeltaTimeForPause;
				if (timer < 0f)
				{
					break;
				}

				yield return null;
			}

			triggerSuccess = true;
			_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 3);
			_sourceActionController.AppendCoroutine(updateEndMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, _source);
		}

		[global::OpenPCR.Migration("aa8f44b0-9767-454e-8677-ecfc3eb52ab7")]
		private IEnumerator updateEndMotion(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill, _isStartMotion: false))
				{
					disableCrowdControl = false;
					yield break;
				}

				if (!_source.UnitSpineCtrl.IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			disableCrowdControl = false;
			_source.ComponentState.SetState(ActionState.IDLE);
		}

		[global::OpenPCR.Migration("57993751-7f4f-4276-94a2-65c21d7f17b3")]
		private bool isActionCancel(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill, bool _isStartMotion)
		{
			if (_source.ComponentAbnormal.IsUnableActionState() || _source.ComponentState.IsCancelActionState(_skill.SkillId == _source.UnionBurstSkillId))
			{
				if (!_isStartMotion)
				{
					_source.ComponentState.CancelByAwake = false;
					_source.ComponentState.CancelByConvert = false;
					_source.ComponentState.CancelByToad = false;
				}

				_sourceActionController.CancelAction(_skill.SkillId);
				return true;
			}

			return _skill.Cancel;
		}

		[global::OpenPCR.Migration("7b598afd-75fe-4a83-891b-1c9d47492962")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
