using global::Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class StealthAction : ActionParameter
	{
		private ActionParameter action { get; set; }

		[global::OpenPCR.Migration("1aacf825-b617-42ca-a2da-0b7f747999d8")]
		public StealthAction() : base()
		{
		}

		[global::OpenPCR.Migration("0df11855-e037-4c28-b2fc-16cb7e8872f4")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			action = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail1);
		}

		[global::OpenPCR.Migration("9abb5c87-f0d9-456d-a87c-9217733b60be")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			if (action != null)
			{
				action.CancelByIfForAll = true;
			}
		}

		[global::OpenPCR.Migration("3db8d6c1-3c2b-4073-8bb6-a34c146deca8")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			_target.Owner.ComponentAbnormal.IsStealth = true;
			_target.Owner.AppendCoroutine(updateStealth((float)_valueDictionary[eValueNumber.VALUE_1], _target.Owner), ePauseType.SYSTEM);
			if (_skill.AnimId != eAnimKitCharacterAnimeId.NONE && _source.GetCurrentSpineCtrl().IsAnimation(_skill.AnimId, _skill.SkillNum, 1))
			{
				_sourceActionController.AppendCoroutine(updateStartMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
			}
		}

		[global::OpenPCR.Migration("1b84832d-a8d3-4228-a853-51132302cab0")]
		private IEnumerator updateStartMotion(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill))
				{
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
			_sourceActionController.AppendCoroutine(updateLoopMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
		}

		[global::OpenPCR.Migration("a4b3c1be-eb34-4c0e-a187-f23d5a34a3cd")]
		private IEnumerator updateLoopMotion(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			yield return null;
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill))
				{
					yield break;
				}

				if (_source.ActionsTargetOnMe.Count == 0)
				{
					break;
				}

				yield return null;
			}

			for (int i = 0; i < _skill.LoopEffectObjs.Count; i++)
			{
				_skill.LoopEffectObjs[i].SetTimeToDie(value: true);
			}

			_skill.LoopEffectObjs.Clear();
			action.CancelByIfForAll = false;
			_sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, 2, _first: false);
			_sourceActionController.ExecUnitActionWithDelay(action, _skill, _first: false, _boneCount: false);
			_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 2);
			_sourceActionController.AppendCoroutine(updateEndMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
		}

		[global::OpenPCR.Migration("c46b7952-4500-4524-abf3-800826cbea53")]
		private IEnumerator updateEndMotion(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			var state = _source.ComponentState;
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill))
				{
					yield break;
				}

				if (!_source.UnitSpineCtrl.IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			state.IdleOnly = true;
			state.CurrentState = ActionState.IDLE;
			base.battleManager.CallbackIdleOnlyDone(_source);
			List<UnitCtrl> list = (_source.IsOther ? base.battleManager.EnemyList : base.battleManager.UnitList);
			if (list.Contains(_source))
			{
				list.Remove(_source);
			}

			state.IsDead = true;
			base.battleManager.CheckAcnLosingBattleFinish();
			_source.ComponentAbnormal.CureAllAbnormalState();
			base.battleManager.CallbackFadeOutDone(_source);
			base.battleManager.CallbackDead(_source);
			_source.gameObject.SetActive(value: false);
		}

		[global::OpenPCR.Migration("dd475677-29ae-48a6-b256-84eac7f38f5f")]
		private IEnumerator updateStealth(float _duration, UnitCtrl _target)
		{
			float timer = _duration;
			while (timer > 0f)
			{
				timer -= base.battleManager.DeltaTime;
				yield return null;
			}

			_target.ComponentAbnormal.IsStealth = false;
		}

		[global::OpenPCR.Migration("574ae86e-7d91-4407-8073-dc2e49fa502b")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}

		[global::OpenPCR.Migration("f40175f1-89a6-4adc-ae5c-03d1d76e2f76")]
		private bool isActionCancel(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			if (_source.ComponentAbnormal.IsUnableActionState() || _source.ComponentState.IsCancelActionState(_skill.SkillId == _source.UnionBurstSkillId))
			{
				var state = _source.ComponentState;
				state.CancelByAwake = false;
				state.CancelByConvert = false;
				state.CancelByToad = false;
				_sourceActionController.CancelAction(_skill.SkillId);
				return true;
			}

			return _skill.Cancel;
		}
	}
}
