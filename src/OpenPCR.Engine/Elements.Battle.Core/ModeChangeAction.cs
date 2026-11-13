using global::Elements;
using global::OpenPCR.Runtime.Api;
using Elements.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class ModeChangeAction : ActionParameter
	{
		public enum eModeChangeType
		{
			TIME = 1,
			ENERGY,
			RELEASE
		}

		public enum eAdditionalAbnormalType
		{
			NONE,
			FLIGHT
		}

		private const int MODE_CHANGE1 = 1;
		private const int MODE_CHANGE_END_MOTION_NUMBER = 1;
		private enum eCancelTriggerType
		{
			NONE,
			UNABLE_ABNORMAL_OR_CHARM
		}

		private static readonly Dictionary<eAdditionalAbnormalType, eAbnormalState> ABNORMAL_STATE_DIC = new Dictionary<eAdditionalAbnormalType, eAbnormalState>
		{
			{
				eAdditionalAbnormalType.FLIGHT,
				eAbnormalState.FLIGHT
			}
		};
		private int oldUnionBurstId { get; set; }

		public bool ReleaseReady;
		[global::OpenPCR.Migration("952a6c6b-8420-4343-a9b2-f906b3b8d8f3")]
		public ModeChangeAction() : base()
		{
		}

		[global::OpenPCR.Migration("43fc29f1-b938-418b-a073-26cec0b63825")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			if (base.ActionDetail1 != 3)
			{
				eAnimKitType spineType = eAnimKitType.SD_MODE_CHANGE;
				if (_source.IsShadow)
				{
					spineType = ((base.battleManager.BattleCategory == eBattleCategory.FRIEND) ? eAnimKitType.SD_MODE_CHANGE : eAnimKitType.SD_SHADOW_MODE_CHANGE);
				}

				BattleAnimKitController.LoadCreate(spineType, _source.ComponentSound.SoundUnitId, _source.SkinRarity, 0, _source.transform.TargetTransform, delegate (BattleAnimKitController _obj)
				{
					_obj.gameObject.SetActive(value: false);
					_obj.transform.localScale = new Vector3(_source.Scale, _source.Scale, 1f);
					_obj.Owner = _source;
					_obj.SetAnimeEventDelegateForBattle(delegate
					{
						_obj.IsStopState = true;
					}, 1);
					_source.UnitSpineCtrlModeChange = _obj;
					_source.StateBoneModeChange = _obj.skeleton.FindBone("State");
					_source.CenterBoneModeChange = _obj.skeleton.FindBone("Center");
				});
				_skill.IsModeChange = true;
			}

			_source.ComponentActionPattern.CreateAttackPattern(ManagerSingleton<MasterDataManager>.Instance.masterUnitSkillData[_source.ComponentSound.SoundUnitId], base.ActionDetail2);
			if (!_sourceActionController.UnionBurstIsChargeTime)
			{
				_sourceActionController.UnionBurstIsChargeTime = _skill.SkillId == _source.UnionBurstSkillId && base.ActionDetail1 == 2;
			}

			_source.ComponentAbnormal.OnActiveUnableState += () => _source.ComponentAbnormal.OnActiveUnableStateForModeChange?.Invoke();
			_source.ComponentAbnormal.OnActiveCharmState += () => _source.ComponentAbnormal.OnActiveCharmForModeChange?.Invoke();
		}

		[global::OpenPCR.Migration("a6e9bf2f-a74c-4e11-b690-dcb2af51bf82")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			eModeChangeType actionDetail = (eModeChangeType)base.ActionDetail1;
			if (actionDetail == eModeChangeType.RELEASE)
			{
				ReleaseReady = false;
			}
		}

		[global::OpenPCR.Migration("2bb650ef-4f2e-4e76-ac3c-c6bd020485ba")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			switch ((eModeChangeType)base.ActionDetail1)
			{
				case eModeChangeType.TIME:
				case eModeChangeType.ENERGY:
					{
						if (BattleUtil.Approximately(_valueDictionary[eValueNumber.VALUE_4], 1.0))
						{
							_source.ComponentState.ModeChangeUnableStateBarrier = true;
						}

						if (base.ActionDetail3 == 0)
						{
							_sourceActionController.DisableUBByModeChange = true;
						}

						_sourceActionController.ModeChanging = true;
						ActionParameter actionParameter = null;
						int i = 0;
						for (int count = _skill.ActionParameters.Count; i < count; i++)
						{
							ActionParameter actionParameter2 = _skill.ActionParameters[i];
							if (actionParameter2.ActionType == eActionType.MODE_CHANGE && actionParameter2.ActionDetail1 == 3)
							{
								actionParameter = actionParameter2;
								actionParameter.TargetList = new List<BasePartsData>(base.TargetList);
							}
						}

						var additionalAbnormalType = (eAdditionalAbnormalType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_5]);
						if (additionalAbnormalType != eAdditionalAbnormalType.NONE)
							_source.ComponentAbnormal.SetAbnormalState(_source, ABNORMAL_STATE_DIC[additionalAbnormalType], 90f, this, _skill, 0.0, 0.0, false, false, 1.0, true, false, false);
						_sourceActionController.IsCancelModeChangeByTrigger = false;
						_source.ComponentState.OnMotionPrefixChanged = delegate
						{
							if (!BattleUtil.Approximately(_valueDictionary[eValueNumber.VALUE_3], 0.0))
							{
								oldUnionBurstId = _source.UnionBurstSkillId;
								_source.ChangeChargeSkill(BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_3]), 0f);
								_source.ComponentState.IsSubUnionBurstMode = true;
							}

							if (BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_6]) == (int)eCancelTriggerType.UNABLE_ABNORMAL_OR_CHARM)
							{
								_source.ComponentAbnormal.OnActiveUnableStateForModeChange = () => modeChangeEndByCancelTrigger(_skill, actionParameter, _source, _sourceActionController, additionalAbnormalType);
								_source.ComponentAbnormal.OnActiveCharmForModeChange = () => modeChangeEndByCancelTrigger(_skill, actionParameter, _source, _sourceActionController, additionalAbnormalType);
							}

							_source.ComponentActionPattern.ChangeAttackPattern(base.ActionDetail2, _skill.Level, -1f);
							_source.ComponentState.OnMotionPrefixChanged = null;
						};
						if (actionParameter != null)
						{
							_sourceActionController.AppendCoroutine(updateModeChange(_valueDictionary[eValueNumber.VALUE_1], (eModeChangeType)base.ActionDetail1, actionParameter, _skill, _source, _sourceActionController, additionalAbnormalType), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
						}

						break;
					}

				case eModeChangeType.RELEASE:
					if (ReleaseReady)
					{
						_source.ComponentActionPattern.ChangeAttackPattern(base.ActionDetail2, _skill.Level);
					}
					else
					{
						ReleaseReady = true;
					}

					break;
			}
		}

		[global::OpenPCR.Migration("d3c80adb-c1a0-40e1-b109-29cdc9ec7050")]
		private IEnumerator updateModeChange(double _value, eModeChangeType _type, ActionParameter _action, Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController, eAdditionalAbnormalType _additionalAbnormalType)
		{
			float timer = 0f;
			bool endFlag = false;
			var parameter = _source.ComponentParameter;
			while (true)
			{
				if ((long)parameter.Hp == 0L || _source.ComponentAbnormal.IsUnableActionState())
				{
					BasePartsData firstParts = _source.ComponentMultiPartsBoss.GetFirstParts(_owner: true);
					_source.ActionsTargetOnMe.Remove((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					base.battleManager.CallbackActionEnd((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					endModeChangeFlags(_sourceActionController, _source);
					yield break;
				}

				if (_source.MotionPrefix == 1)
				{
					break;
				}

				yield return null;
			}

			while (true)
			{
				if (_sourceActionController.IsCancelModeChangeByTrigger)
					yield break;
				switch (_type)
				{
					case eModeChangeType.TIME:
						if (base.battleManager.GetBlackOutUnitLength() == 0)
						{
							timer += _source.DeltaTimeForPause;
						}

						if (timer >= _value && _source.ComponentState.CurrentState == ActionState.IDLE)
						{
							modeChangeEnd(_skill, _action, _source, _sourceActionController, _additionalAbnormalType);
							yield break;
						}

						if (_source.ComponentAbnormal.ToadDatas.Count > 0)
						{
							modeChangeEnd(_skill, _action, _source, _sourceActionController, _additionalAbnormalType);
							yield break;
						}

						break;
					case eModeChangeType.ENERGY:
						_source.ComponentUnionBurst.SetReduceEnergyAction(eReduceEnergyType.MODE_CHANGE, true);
						parameter.SetEnergy(parameter.Energy - _source.DeltaTimeForPause * _value, eSetEnergyType.BY_MODE_CHANGE);
						if (BattleUtil.Approximately(parameter.Energy, 0.0))
						{
							endFlag = true;
						}

						if (_source.ComponentAbnormal.ToadDatas.Count > 0)
						{
							modeChangeEnd(_skill, _action, _source, _sourceActionController, _additionalAbnormalType);
							yield break;
						}

						if ((_source.ComponentState.CurrentState == ActionState.IDLE) & endFlag)
						{
							modeChangeEnd(_skill, _action, _source, _sourceActionController, _additionalAbnormalType);
							yield break;
						}

						break;
				}

				if ((long)parameter.Hp == 0L)
				{
					BasePartsData firstParts = _source.ComponentMultiPartsBoss.GetFirstParts(_owner: true);
					_source.ActionsTargetOnMe.Remove((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					base.battleManager.CallbackActionEnd((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					yield break;
				}

				if (_source.ComponentState.IdleOnly && (long)parameter.Hp > 0)
				{
					break;
				}

				yield return null;
			}

			modeChangeEnd(_skill, _action, _source, _sourceActionController, _additionalAbnormalType);
		}

		[global::OpenPCR.Migration("e3c25941-630f-4e00-973c-16a72411e827")]
		private void modeChangeEnd(Skill _skill, ActionParameter _action, UnitCtrl _source, UnitActionController _sourceUnitActionController, eAdditionalAbnormalType _additionalAbnormalType)
		{
			if (_sourceUnitActionController.IsCancelModeChangeByTrigger)
				return;
			var abnormal = _source.ComponentAbnormal;
			var state = _source.ComponentState;
			abnormal.OnActiveUnableStateForModeChange = null;
			abnormal.OnActiveCharmForModeChange = null;
			_source.ComponentUnionBurst.SetReduceEnergyAction(eReduceEnergyType.MODE_CHANGE, false);
			if (_additionalAbnormalType != eAdditionalAbnormalType.NONE)
				abnormal.DispelAbnormalState(ABNORMAL_STATE_DIC[_additionalAbnormalType]);
			if (abnormal.ToadDatas.Count > 0)
			{
				var firstParts = _source.ComponentMultiPartsBoss.GetFirstParts(_owner: true);
				_source.ActionsTargetOnMe.Remove((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
				base.battleManager.CallbackActionEnd((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
				_source.MotionPrefix = -1;
				if (_source.ComponentState.IsSubUnionBurstMode)
				{
					_source.ChangeChargeSkill(oldUnionBurstId, 0f);
				}

				_source.ComponentState.IsSubUnionBurstMode = false;
				_source.UnitSpineCtrl.CurColor = _source.UnitSpineCtrl.CurColor;
				Color curColor = _source.UnitSpineCtrl.CurColor;
				curColor.a = 1f;
				_source.UnitSpineCtrl.CurColor = curColor;
				_source.UnitSpineCtrl.SetActive(_isActive: false);
				if (_source.ComponentSortOrder.IsFront)
				{
					_source.ComponentSortOrder.SetSortOrderFront();
				}
				else
				{
					_source.ComponentSortOrder.SetSortOrderBack();
				}

				state.CancelByToad = false;
				state.CancelByConvert = false;
				_sourceUnitActionController.DisableUBByModeChange = false;
				_sourceUnitActionController.ExecUnitActionWithDelay(_action, _skill, _first: false, _boneCount: false);
				return;
			}

			if (!_source.Pause)
			{
				_source.GetCurrentSpineCtrl().Resume();
			}

			_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 1, 1);
			_source.ComponentState.IsModeChangeEnd = true;
			_sourceUnitActionController.AppendCoroutine(updateModeChangeEnd(_skill, _source, _sourceUnitActionController), ePauseType.SYSTEM);
			endModeChangeFlags(_sourceUnitActionController, _source);
			_sourceUnitActionController.ExecUnitActionWithDelay(_action, _skill, _first: false, _boneCount: false);
			int num = 1;
			abnormal.ModeChangeEndEffectList.Clear();
			_sourceUnitActionController.StopModeChangeEndEffectCalled = false;
			_sourceUnitActionController.CreateNormalPrefabWithTargetMotion(_skill, num, _first: false, _useStartCoroutine: false, _modechangeEndEffect: true);
			for (int i = 0; i < _skill.ShakeEffects.Count; i++)
			{
				if (_skill.ShakeEffects[i].TargetMotion == num)
				{
					_sourceUnitActionController.AppendCoroutine(_sourceUnitActionController.StartShakeWithDelay(_skill.ShakeEffects[i], _skill), ePauseType.VISUAL, (_skill.BlackOutTime > 0f) ? _source : null);
				}
			}
		}

		[global::OpenPCR.Migration("ba7c98c5-1355-491a-a3c7-b42b18bf7c6a")]
		private IEnumerator updateModeChangeEnd(Skill skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			BasePartsData firstParts;
			while (true)
			{
				firstParts = _source.ComponentMultiPartsBoss.GetFirstParts(_owner: true);
				if ((long)_source.ComponentParameter.Hp == 0L)
				{
					_source.ActionsTargetOnMe.Remove((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					base.battleManager.CallbackActionEnd((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					endModeChangeFlags(_sourceActionController, _source);
					_source.ComponentState.CurrentState = ActionState.IDLE;
					_source.ComponentState.SetState(ActionState.DIE);
					yield break;
				}

				if (_source.ComponentAbnormal.ToadDatas.Count > 0)
				{
					_source.ActionsTargetOnMe.Remove((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					base.battleManager.CallbackActionEnd((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					_source.MotionPrefix = -1;
					if (_source.ComponentState.IsSubUnionBurstMode)
					{
						_source.ChangeChargeSkill(oldUnionBurstId, 0f);
					}

					_source.ComponentState.IsSubUnionBurstMode = false;
					_source.UnitSpineCtrl.CurColor = _source.UnitSpineCtrl.CurColor;
					Color curColor = _source.UnitSpineCtrl.CurColor;
					curColor.a = 1f;
					_source.UnitSpineCtrl.CurColor = curColor;
					_source.UnitSpineCtrl.SetActive(_isActive: false);
					if (_source.ComponentSortOrder.IsFront)
					{
						_source.ComponentSortOrder.SetSortOrderFront();
					}
					else
					{
						_source.ComponentSortOrder.SetSortOrderBack();
					}

					_source.ComponentState.IsModeChangeEnd = false;
					_source.ComponentState.CancelByConvert = false;
					_source.ComponentState.CancelByToad = false;
					_sourceActionController.DisableUBByModeChange = false;
					stopEndEffects(_source, _sourceActionController);
					yield break;
				}

				if (_source.ComponentAbnormal.IsUnableActionState())
				{
					stopEndEffects(_source, _sourceActionController);
				}

				if (!_source.UnitSpineCtrlModeChange.IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			_source.ActionsTargetOnMe.Remove((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
			base.battleManager.CallbackActionEnd((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
			_source.MotionPrefix = -1;
			if (_source.ComponentState.IsSubUnionBurstMode)
			{
				_source.ChangeChargeSkill(oldUnionBurstId, 0f);
			}

			_source.ComponentState.IsSubUnionBurstMode = false;
			_source.UnitSpineCtrlModeChange.gameObject.SetActive(value: false);
			_source.UnitSpineCtrl.IsAlphaZero = false;
			_source.UnitSpineCtrl.SetCurColor(_source.UnitSpineCtrlModeChange.CurColor);
			if (_source.ComponentSortOrder.IsFront)
			{
				_source.ComponentSortOrder.SetSortOrderFront();
			}
			else
			{
				_source.ComponentSortOrder.SetSortOrderBack();
			}

			if (!_source.UnitSpineCtrl.IsAnimation(_source.UnitSpineCtrl.ConvertAnimeIdToAnimeName(skill.AnimId, skill.SkillNum, 1)))
			{
				_source.ComponentState.IsModeChangeEnd = false;
				_source.ComponentState.SkillEndProcess();
				_source.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE, _source.MotionPrefix);
				yield break;
			}

			if (!_source.Pause)
			{
				_source.GetCurrentSpineCtrl().Resume();
			}

			_source.ComponentAnimation.PlayAnimeNoOverlap(skill.AnimId, skill.SkillNum, 1);
			_sourceActionController.StopModeChangeEndEffectCalled = false;
			_sourceActionController.AppendCoroutine(updateModeChangeEnd2(_source, _sourceActionController), ePauseType.VISUAL);
		}

		[global::OpenPCR.Migration("e18f753e-2849-44a0-aaf5-29e15f0c36e1")]
		private void stopEndEffects(UnitCtrl _source, UnitActionController _unitActionController)
		{
			_unitActionController.StopModeChangeEndEffectCalled = true;
			for (int num = _source.ComponentAbnormal.ModeChangeEndEffectList.Count - 1; num >= 0; num--)
			{
				_source.ComponentAbnormal.ModeChangeEndEffectList[num].SetTimeToDie(value: true);
				_source.ComponentAbnormal.ModeChangeEndEffectList.RemoveAt(num);
			}
		}

		[global::OpenPCR.Migration("4ca2eaef-e9a3-46c4-a2b1-ba56814c75ff")]
		private IEnumerator updateModeChangeEnd2(UnitCtrl _source, UnitActionController _sourceActionController)
		{
			while (true)
			{
				if ((long)_source.ComponentParameter.Hp == 0L)
				{
					BasePartsData firstParts = _source.ComponentMultiPartsBoss.GetFirstParts(_owner: true);
					_source.ActionsTargetOnMe.Remove((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					base.battleManager.CallbackActionEnd((long)base.ActionId * 100L + base.IdOffsetDictionary[firstParts]);
					endModeChangeFlags(_sourceActionController, _source);
					_source.ComponentState.CurrentState = ActionState.IDLE;
					_source.ComponentState.SetState(ActionState.DIE);
					yield break;
				}

				if (_source.ComponentAbnormal.IsUnableActionState())
				{
					stopEndEffects(_source, _sourceActionController);
				}

				if (!_source.UnitSpineCtrl.IsPlayAnimeBattle || _source.ComponentAbnormal.ToadDatas.Count > 0)
				{
					break;
				}

				yield return null;
			}

			_source.ComponentState.IsModeChangeEnd = false;
			_source.ComponentState.SkillEndProcess();
			_source.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE, _source.MotionPrefix);
			stopEndEffects(_source, _sourceActionController);
		}

		[global::OpenPCR.Migration("795d23c6-4cfa-459c-b489-06cabfb52fff")]
		private void modeChangeEndByCancelTrigger(Skill _skill, ActionParameter _action, UnitCtrl _source, UnitActionController _sourceActionController, eAdditionalAbnormalType _additionalAbnormalType)
		{
			var abnormal = _source.ComponentAbnormal;
			_sourceActionController.IsCancelModeChangeByTrigger = true;
			if (_additionalAbnormalType != eAdditionalAbnormalType.NONE)
				abnormal.DispelAbnormalState(ABNORMAL_STATE_DIC[_additionalAbnormalType]);
			_source.ComponentUnionBurst.SetReduceEnergyAction(eReduceEnergyType.MODE_CHANGE, false);
			endModeChangeFlags(_sourceActionController, _source);
			_sourceActionController.ExecUnitActionWithDelay(_action, _skill, false, false);
			abnormal.ModeChangeEndEffectList.Clear();
			_sourceActionController.StopModeChangeEndEffectCalled = false;
			_sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, 1, false, false, true);
			var firstParts = _source.ComponentMultiPartsBoss.GetFirstParts(true);
			_source.ActionsTargetOnMe.Remove((long)ActionId * 100L + IdOffsetDictionary[firstParts]);
			battleManager.CallbackActionEnd((long)ActionId * 100L + IdOffsetDictionary[firstParts]);
			_source.MotionPrefix = -1;
			var state = _source.ComponentState;
			if (state.IsSubUnionBurstMode)
				_source.ChangeChargeSkill(oldUnionBurstId, 0f);
			state.IsSubUnionBurstMode = false;
			BattleAnimKitController modeSpine = _source.UnitSpineCtrlModeChange;
			modeSpine.gameObject.SetActive(false);
			BattleAnimKitController spine = _source.UnitSpineCtrl;
			spine.IsAlphaZero = false;
			spine.SetCurColor(_source.UnitSpineCtrlModeChange.CurColor);
			if (_source.ComponentSortOrder.IsFront)
				_source.ComponentSortOrder.SetSortOrderFront();
			else
				_source.ComponentSortOrder.SetSortOrderBack();
			state.IsModeChangeEnd = false;
			state.CancelByConvert = false;
			state.CancelByToad = false;
			abnormal.OnActiveUnableStateForModeChange = null;
			abnormal.OnActiveCharmForModeChange = null;
			if (state.CurrentState == ActionState.DAMAGE)
			{
				if (abnormal.IsContinueIdleForPauseAction)
				{
					spine.IsStopState = true;
					spine.Resume();
					spine.PlayAnime(eAnimKitCharacterAnimeId.IDLE, -1, true);
					spine.Pause();
				}
				else
				{
					_source.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.DAMEGE, -1, -1, -1, false, spine, false, modeSpine.StopStateTime, false);
					spine.Pause();
					spine.IsPlayAnimeBattle = modeSpine.IsPlayAnimeBattle;
					spine.IsStopState = modeSpine.IsStopState;
				}
			}
		}

		[global::OpenPCR.Migration("3087a60c-03e0-4163-8d6c-b4b1a3cbaec9")]
		private void endModeChangeFlags(UnitActionController _unitActionController, UnitCtrl _unit)
		{
			_unitActionController.ModeChanging = false;
			UnitComponentState state = _unit.ComponentState;
			if (state == null)
				throw new System.NullReferenceException();
			_unitActionController.DisableUBByModeChange = false;
			state.ModeChangeUnableStateBarrier = false;
		}

		[global::OpenPCR.Migration("e870dfaa-b709-49a8-89ba-f8d64891b5b5")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
