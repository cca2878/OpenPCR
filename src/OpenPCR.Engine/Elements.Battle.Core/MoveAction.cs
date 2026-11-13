using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class MoveAction : ActionParameter
	{
		private ActionParameter endAction { get; set; }
		private ActionState initialState { get; set; }

		[global::OpenPCR.Migration("d6ce8e98-7034-4a92-92d9-2c64b5f05f59")]
		public MoveAction() : base()
		{
		}

		[global::OpenPCR.Migration("c45f90a4-0c4d-41a2-b36c-c9a4242f5e1c")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			endAction = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2);
		}

		[global::OpenPCR.Migration("61bf4ab7-0f67-4077-a753-488577a7008b")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (base.ActionDetail2 != 0)
			{
				endAction.CancelByIfForAll = true;
			}

			FixedTransformMonoBehavior.FixedTransform sourceTransform = _sourceActionController.transform;
			switch (base.ActionDetail1)
			{
				case 1:
					if (_num % 2 == 0)
					{
						sourceTransform.SetLocalPosX(_target.GetBottomTransformLocalPosition().x + _target.GetLocalPosition().x);
						if (BattleUtil.Approximately(_valueDictionary[eValueNumber.VALUE_3], 1.0))
						{
							sourceTransform.SetLocalPosY(_target.GetLocalPosition().y);
						}

						sourceTransform.localPosition += calculatePosotion(_source, _target, _sourceActionController, _valueDictionary);
					}
					else
					{
						sourceTransform.localPosition = _skill.OwnerReturnPosition;
					}

					break;
				case 2:
					if (_num % 2 == 0)
					{
						sourceTransform.localPosition += new Vector3(_source.IsLeftDir ? (-(float)_valueDictionary[eValueNumber.VALUE_1]) : (float)_valueDictionary[eValueNumber.VALUE_1], 0f, 0f);
						OnActionEnd = delegate
						{
							sourceTransform.localPosition = _skill.OwnerReturnPosition;
							OnActionEnd = null;
						};
					}
					else
					{
						sourceTransform.localPosition = _skill.OwnerReturnPosition;
						OnActionEnd = null;
					}

					break;
				case 3:
					_source.transform.SetLocalPosX(_target.GetBottomTransformLocalPosition().x + _target.GetLocalPosition().x);
					_source.transform.localPosition += calculatePosotion(_source, _target, _sourceActionController, _valueDictionary);
					_sourceActionController.AppendCoroutine(resetPositionY(_skill, _source), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
					break;
				case 4:
					_source.transform.localPosition += new Vector3(_source.IsLeftDir ? (-(float)_valueDictionary[eValueNumber.VALUE_1]) : (float)_valueDictionary[eValueNumber.VALUE_1], 0f, 0f);
					break;
				case 5:
					_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 1, -1, _isLoop: true);
					_sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, 1, _first: false);
					initialState = _source.ComponentState.CurrentState;
					_sourceActionController.AppendCoroutine(targetMoveByVerocity((float)_valueDictionary[eValueNumber.VALUE_1], (float)_valueDictionary[eValueNumber.VALUE_2], _target, _sourceActionController, _source, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
					break;
				case 6:
					_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 1, -1, _isLoop: true);
					initialState = _source.ComponentState.CurrentState;
					_sourceActionController.AppendCoroutine(absoluteMoveByVerocity((float)_valueDictionary[eValueNumber.VALUE_1], (float)_valueDictionary[eValueNumber.VALUE_2], _sourceActionController, _source, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
					break;
				case 7:
					if (_num % 2 == 0)
					{
						sourceTransform.localPosition += new Vector3(_source.IsOther ? (-(float)_valueDictionary[eValueNumber.VALUE_1]) : (float)_valueDictionary[eValueNumber.VALUE_1], 0f, 0f);
						_source.SetDirectionAuto();
					}
					else
					{
						sourceTransform.localPosition = _skill.OwnerReturnPosition;
					}

					break;
			}

			int actionDetail = base.ActionDetail1;
			if ((uint)(actionDetail - 1) <= 3u || actionDetail == 7)
			{
				if (sourceTransform.localPosition.x > BattleDefine.BATTLE_FIELD_SIZE)
				{
					sourceTransform.SetLocalPosX(BattleDefine.BATTLE_FIELD_SIZE);
				}

				if (sourceTransform.localPosition.x < -BattleDefine.BATTLE_FIELD_SIZE)
				{
					sourceTransform.SetLocalPosX(-BattleDefine.BATTLE_FIELD_SIZE);
				}
			}
		}

		[global::OpenPCR.Migration("2c87101e-e019-48f8-a3dc-175285589c05")]
		private IEnumerator absoluteMoveByVerocity(float _distance, float _speed, UnitActionController _sourceUnitActionController, UnitCtrl _source, Skill _skill)
		{
			_sourceUnitActionController.MoveEnd = false;
			FixedTransformMonoBehavior.FixedTransform sourceTransform = _sourceUnitActionController.transform;
			float startPosx = sourceTransform.localPosition.x;
			while (true)
			{
				if (_source.ComponentState.CurrentState != initialState)
				{
					_sourceUnitActionController.CancelAction(_skill.SkillId);
					yield break;
				}

				sourceTransform.localPosition += new Vector3(((!_source.IsLeftDir) ? _speed : (-_speed)) * base.battleManager.DeltaTime, 0f, 0f);
				if (Mathf.Abs(startPosx - sourceTransform.localPosition.x) >= _distance)
				{
					break;
				}

				yield return null;
			}

			_sourceUnitActionController.MoveEnd = true;
			_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 2);
			_sourceUnitActionController.AppendCoroutine(moveByVerocityEnd(_sourceUnitActionController, _source, _skill, _speed), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
		}

		[global::OpenPCR.Migration("196771b7-d48c-4946-888b-da80dbeb46ea")]
		private void killLoopEffectObjects(Skill _skill)
		{
			for (int i = 0; i < _skill.LoopEffectObjs.Count; i++)
				_skill.LoopEffectObjs[i].SetTimeToDie(true);
			_skill.LoopEffectObjs.Clear();
			_skill.LoopEffectAlreadyDone = true;
		}

		[global::OpenPCR.Migration("4c7fc20e-a7f1-43e5-9d63-41210b1976cb")]
		private IEnumerator targetMoveByVerocity(float _main, float _sub, BasePartsData _target, UnitActionController _sourceUnitActionController, UnitCtrl _source, Skill _skill)
		{
			_main += _target.GetBodyWidth() / 2f;
			_sourceUnitActionController.MoveEnd = false;
			FixedTransformMonoBehavior.FixedTransform sourceTransform = _sourceUnitActionController.transform;
			bool isRight = sourceTransform.position.x < _target.GetPosition().x;
			bool isUnionBurst = _skill.SkillId == _source.UnionBurstSkillId;
			var abnormal = _source.ComponentAbnormal;
			while (true)
			{
				if (_source.ComponentState.CurrentState != initialState || _source.ComponentState.IsCancelActionState(isUnionBurst) || (!isUnionBurst && abnormal.IsUnableActionState()))
				{
					killLoopEffectObjects(_skill);
					_sourceUnitActionController.CancelAction(_skill.SkillId);
					yield break;
				}

				sourceTransform.localPosition += new Vector3(((!isRight) ? (-_sub) : _sub) * base.battleManager.DeltaTime, 0f, 0f);
				if ((_target.GetLocalPosition().x - sourceTransform.localPosition.x) * (float)(isRight ? 1 : (-1)) <= _main)
				{
					break;
				}

				yield return null;
			}

			_sourceUnitActionController.MoveEnd = true;
			_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 2);
			killLoopEffectObjects(_skill);
			_sourceUnitActionController.AppendCoroutine(moveByVerocityEnd(_sourceUnitActionController, _source, _skill, _sub), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
		}

		[global::OpenPCR.Migration("edcdbab8-d64a-4d2d-a18b-ca244e49ad5c")]
		private IEnumerator moveByVerocityEnd(UnitActionController _sourceActionController, UnitCtrl _source, Skill _skill, float _velocity)
		{
			if (endAction != null)
			{
				endAction.CancelByIfForAll = false;
				_sourceActionController.ExecUnitActionWithDelay(endAction, _skill, _first: false, _boneCount: false);
			}

			_sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, 2, _first: false);
			while (true)
			{
				if (_source.ComponentState.CurrentState != initialState)
				{
					_sourceActionController.CancelAction(_skill.SkillId);
					yield break;
				}

				if (!_source.UnitSpineCtrl.IsPlayAnimeBattle)
					break;
				yield return null;
			}

			if (_source.GetCurrentSpineCtrl().IsAnimation(_skill.AnimId, _skill.SkillNum, 3))
			{
				_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 3, -1, _isLoop: true);
				_sourceActionController.AppendCoroutine(moveByVerocityReturn(_sourceActionController, _source, _skill, _velocity), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
			}
			else
			{
				_source.ComponentState.SkillEndProcess();
			}
		}

		[global::OpenPCR.Migration("ce1e2e87-1cc1-42ae-8ffb-ad458fcbfd40")]
		private IEnumerator moveByVerocityReturn(UnitActionController _sourceActionController, UnitCtrl _source, Skill _skill, float _velocity)
		{
			FixedTransformMonoBehavior.FixedTransform sourceTransform = _sourceActionController.transform;
			bool isRight = sourceTransform.localPosition.x < _skill.OwnerReturnPosition.x;
			while (true)
			{
				if (_source.ComponentState.CurrentState != initialState)
				{
					_sourceActionController.CancelAction(_skill.SkillId);
					yield break;
				}

				sourceTransform.localPosition += new Vector3(((!isRight) ? (-_velocity) : _velocity) * base.battleManager.DeltaTime, 0f, 0f);
				if ((_skill.OwnerReturnPosition.x - sourceTransform.localPosition.x) * (float)(isRight ? 1 : (-1)) < 0f || BattleUtil.Approximately(_skill.OwnerReturnPosition.x, sourceTransform.localPosition.x))
				{
					break;
				}

				yield return null;
			}

			Vector3 localPosition = sourceTransform.localPosition;
			localPosition.x = _skill.OwnerReturnPosition.x;
			sourceTransform.localPosition = localPosition;
			_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 4);
			_sourceActionController.AppendCoroutine(moveType4ReturnEnd(_sourceActionController, _source, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
		}

		[global::OpenPCR.Migration("7a599fe8-288c-4ea8-bd0e-53ed8e9d9f08")]
		private IEnumerator moveType4ReturnEnd(UnitActionController _sourceActionController, UnitCtrl _source, Skill _skill)
		{
			var state = _source.ComponentState;
			while (state.CurrentState == initialState && _source.UnitSpineCtrl.IsPlayAnimeBattle)
			{
				yield return null;
			}

			if (state.CurrentState == initialState)
				_source.ComponentState.SkillEndProcess();
			else
				_sourceActionController.CancelAction(_skill.SkillId);
			if (_skill.BlackoutEndWithMotion)
			{
				base.battleManager.SetBlackoutTimeZero();
			}
		}

		[global::OpenPCR.Migration("2603f515-53bd-46ad-9389-82387dc5b341")]
		private IEnumerator resetPositionY(Skill _skill, UnitCtrl _source)
		{
			var abnormal = _source.ComponentAbnormal;
			var state = _source.ComponentState;
			while (true)
			{
				if (state.IdleOnly)
					yield break;
				if (!(_source.GetCurrentSpineCtrl().IsPlayAnimeBattle && !abnormal.IsUnableActionState() && state.CurrentState != ActionState.DAMAGE))
					break;
				yield return null;
			}

			_source.transform.localPosition += new Vector3(0f, _skill.OwnerReturnPosition.y - _source.transform.localPosition.y, 0f);
			List<UnitCtrl> list = (_source.IsOther ? base.battleManager.UnitList : base.battleManager.EnemyList);
			for (int i = 0; i < list.Count; i++)
			{
				UnitCtrl unitCtrl = list[i];
				if (unitCtrl.ComponentState.CurrentState == ActionState.IDLE || unitCtrl.ComponentState.CurrentState == ActionState.WALK)
				{
					unitCtrl.SetDirectionAuto();
				}
			}
		}

		[global::OpenPCR.Migration("7a251af9-48f2-4033-a676-be10e390233e")]
		private static Vector3 calculatePosotion(UnitCtrl _source, BasePartsData _target, UnitActionController _sourceActionController, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return new Vector3((_source.IsLeftDir ? 1f : (-1f)) * ((float)_valueDictionary[eValueNumber.VALUE_1] + (float)Math.Sign(_valueDictionary[eValueNumber.VALUE_1]) * (_target.GetBodyWidth() / 2f + _source.BodyWidth / 2f)), 0f, 0f);
		}
	}
}
