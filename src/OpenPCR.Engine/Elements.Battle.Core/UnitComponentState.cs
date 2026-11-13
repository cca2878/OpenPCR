using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using System.Linq;
using global::OpenPCR.Foundation;
using Elements.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public partial class UnitComponentState : UnitComponentBase, IUnitComponent
	{
		public delegate void OnDeadDelegate(UnitCtrl owner);
		private SkillEffectCtrl runSmokeEffect;
		public Action<bool> OnStartErrorUndoDivision;
		public Action<UnitCtrl> OnDieForZeroHp;
		public Action<UnitCtrl> OnDieFadeOutEnd;
		private global::Elements.Battle.BattleManager battleManager { get; set; }
		public bool IdleOnly { get; set; }
		public bool IsDead { get; set; }
		public bool HasUnDeadTime { get; set; }
		public bool CancelByConvert { get; set; }
		public bool CancelByToad { get; set; }
		public bool CancelByAwake { get; set; }
		public bool GameStartDone { get; set; }
		public global::System.Action<float> OnUpdateWhenIdle { get; set; }
		public global::System.Action OnBreakAll { get; set; }
		public int EnemyPoint { get; set; }
		public bool MultiTargetDone { get; set; }
		public bool ModeChangeUnableStateBarrier { get; set; }
		private global::System.Collections.Generic.List<global::Elements.SkillEffectCtrl> idleEffectsObjs { get; set; }

		[global::OpenPCR.Migration("9bb6fd16-c4d0-4821-88ae-92b6755c1023")]
		public UnitComponentState(UnitCtrl _unitCtrl, BattleManager _battleManager, IBattleLog _battleLog, PresetBattleEffectData _battleEffectData) : base(_unitCtrl)
		{
			soundManager = ManagerSingleton<SoundManager>.Instance;
			battleManager = _battleManager;
			battleLog = _battleLog;
			battleEffectData = _battleEffectData;
			idleEffectsObjs = new List<SkillEffectCtrl>();
			voiceTypeDictionary = new Dictionary<int, VoiceTypeAndSkillNumber>();
		}

		[global::OpenPCR.Migration("186810e0-ec3f-4677-830d-adee1f1ebd2c")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAbnormal = _container.ComponentAbnormal;
			componentAnimation = _container.ComponentAnimation;
			componentAttackPattern = _container.ComponentActionPattern;
			componentColor = _container.ComponentColor;
			componentDamageControl = _container.ComponentDamageControl;
			componentMultiPartsBoss = _container.ComponentMultiPartsBoss;
			componentParameter = _container.ComponentParameter;
			componentPassiveSkill = _container.ComponentPassiveSkill;
			componentRelicSkill = _container.ComponentRelicSkill;
			componentSearchArea = _container.ComponentSearchArea;
			componentSortOrder = _container.ComponentSortOrder;
			componentSound = _container.ComponentSound;
			componentUnionBurst = _container.ComponentUnionBurst;
		}

		[global::OpenPCR.Migration("d0b2d7eb-96e8-4219-8435-3834dfce70b4")]
		public void SetState(ActionState _state, int _nextSkillId = 0, int _skillId = 0, bool _quiet = false)
		{
			HasSetStateCalled = true;
			if (!MultiTargetByTime && GameStartDone && unitController.IsPartsBoss && !MultiTargetDone)
			{
				PlayAndSetUpMultiTarget(_isFirst: true);
			}

			if ((battleManager.IsSubStoryRaidBattle && unitController.IsBoss && GameStartDone) ||
				(!battleManager.IsSubStoryRaidBattle && IsStartActionForExtraEffect))
			{
				battleManager.StartExtraEffect(BattleDefine.eExtraEffectExecTiming.ALL_UNIT_ACTION);
				battleManager.StartExtraEffect(unitController.IsOther ? BattleDefine.eExtraEffectExecTiming.ENEMY_ACTION : BattleDefine.eExtraEffectExecTiming.MY_TEAM_ACTION);
			}

			bool flag = false;
			if (unitController.IsPartsBoss && GameStartDone && IsNotPartsBossReady)
			{
				IsNotPartsBossReady = false;
				if (IsOnBreakAllCallWhenGameStartDone)
				{
					flag = true;
					IsOnBreakAllCallWhenGameStartDone = false;
				}

				battleManager.CountIsNotPartsBossReady--;
			}

			if (unitController.IsBoss && GameStartDone && unitController.ComponentMultiPartsBoss.MultiBossPartsData != null && componentMultiPartsBoss.MultiBossPartsData.MultiTargetCursor == null)
			{
				tryPlayMultiBossAppear();
				battleManager.CountIsNotPartsBossReady--;
				IsNotPartsBossReady = false;
				ResourceManager instance = ManagerSingleton<ResourceManager>.Instance;
				MultiTargetCursor cursor = null;
				if (isMultiBossDestructionTarget(unitController.UnitId))
				{
					cursor = instance.InstantiateAndGetComponent<MultiTargetCursor>(eResourceId.ANIM_MULTI_TARGET_CURSOR_LEADER, battleManager.UnitUiCtrl.transform, 0L);
				}
				else
				{
					cursor = instance.InstantiateAndGetComponent<MultiTargetCursor>(unitController.UseTargetCursorOver ? eResourceId.ANIM_MULTI_TARGET_CURSOR_OVER : eResourceId.ANIM_MULTI_TARGET_CURSOR, battleManager.UnitUiCtrl.transform, 0L);
				}

				PartsData multiBossPartsData = unitController.ComponentMultiPartsBoss.MultiBossPartsData;
				cursor.transform.position = multiBossPartsData.GetBottomTransformPosition();
				cursor.Panel.sortingOrder = (unitController.ComponentSortOrder.IsFront ? 11500 : 0) + 690;
				multiBossPartsData.MultiTargetCursor = cursor;
				battleManager.AppendCoroutine(multiBossPartsData.TrackBottomCoroutine(_topBack: true), ePauseType.SYSTEM, unitController);
				if (multiBossPartsData.IsBreak)
				{
					unitController.AppendBreakLog(multiBossPartsData.BreakSource);
					multiBossPartsData.OnBreak.Call();
					multiBossPartsData.SetBreak(_enable: true, battleManager.UnitUiCtrl.transform, _displayEffect: true);
					unitController.AppendCoroutine(unitController.ComponentMultiPartsBoss.UpdateBreak(multiBossPartsData.BreakTime, multiBossPartsData), ePauseType.SYSTEM);
				}

				unitController.OnIsFrontFalse = (Action)Delegate.Combine(unitController.OnIsFrontFalse, (Action)delegate
				{
					cursor.Panel.sortingOrder = 690;
				});
			}

			if (_state != ActionState.IDLE && _state != ActionState.WALK)
			{
				CancelByConvert = false;
				CancelByToad = false;
			}

			UnitCtrl lIMEKPEENOB;
			UnitCtrl jELADBAMFKH;
			if (_state == ActionState.UNION_BURST)
			{
				_skillId = unitController.UnionBurstSkillId;
				IBattleLog mLEGMHAOCON = battleLog;
				jELADBAMFKH = unitController;
				lIMEKPEENOB = unitController;
				mLEGMHAOCON.AppendBattleLog(eBattleLogType.BUTTON_TAP, 0, _skillId, (long)CurrentState, 0, 0, 1, 1, jELADBAMFKH, lIMEKPEENOB);
			}

			IBattleLog mLEGMHAOCON2 = battleLog;
			lIMEKPEENOB = unitController;
			jELADBAMFKH = unitController;
			mLEGMHAOCON2.AppendBattleLog(eBattleLogType.SET_STATE, (int)_state, _skillId, (long)CurrentState, 0, 0, 1, 1, lIMEKPEENOB, jELADBAMFKH);
			for (int num = idleEffectsObjs.Count - 1; num >= 0; num--)
			{
				idleEffectsObjs[num].SetTimeToDie(value: true);
				idleEffectsObjs.RemoveAt(num);
			}

			if (StandByDone && !unitController.Pause)
			{
				if (_state == ActionState.IDLE && unitController.ComponentAbnormal.IsHasteSpeed())
				{
					unitController.GetCurrentSpineCtrl().SetTimeScale(2f);
				}
				else
				{
					unitController.GetCurrentSpineCtrl().Resume();
				}
			}

			unitController.SetRecastTime(_nextSkillId);
			unitController.SetDirectionAuto();
			switch (_state)
			{
				case ActionState.WALK:
					setStateWalk();
					break;
				case ActionState.IDLE:
					setStateIdle();
					break;
				case ActionState.ATK:
					setStateAttack();
					break;
				case ActionState.DAMAGE:
					setStateDamage(_quiet);
					break;
				case ActionState.DIE:
					setStateDie();
					break;
				case ActionState.UNION_BURST:
					setStateUnionBurst();
					break;
				case ActionState.SKILL:
					setStateSkill(_skillId);
					break;
				case ActionState.GAME_START:
					{
						unitController.IsAwakeMotion = true;
						unitController.SetLeftDirection(bLeftDir: true);
						unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.GAME_START, -1, -1, -1, _isLoop: false, null, _quiet: true);
						CurrentState = _state;
						eBattleCategory battleCategory = battleManager.BattleCategory;
						if ((battleCategory == eBattleCategory.TOWER_EX || battleCategory == eBattleCategory.TOWER_EX_REPLAY || battleCategory == eBattleCategory.TOWER_EX_SINGLE_REPLAY) && battleManager.GetCurrentTowerExPartyIndex() > 0)
						{
							unitController.AppendCoroutine(updateGameStartMotionIdle(), ePauseType.SYSTEM);
						}
						else
						{
							unitController.AppendCoroutine(updateGameStart(), ePauseType.SYSTEM);
						}

						break;
					}
			}

			if (flag)
			{
				OnBreakAll.Call();
			}
		}

		[global::OpenPCR.Migration("1d538c21-7521-4fa3-a157-94d2f2aaeed9")]
		public void PlayAndSetUpMultiTarget(bool _isFirst)
		{
			this.MultiTargetDone = true;
			if (shouldPlayMultiTarget())

			{
				var blockLayerManager = ManagerSingleton<BlockLayerManager>.Instance;
				ManagerSingleton<SaveDataManager>.Instance.MultiTargetFirst = _isFirst;
				blockLayerManager.SetBlockCommon(true);
				SingletonMonoBehaviour<UnitUiCtrl>.Instance.HidePopUp();
				this.battleManager.SetSkillScreen(_enable: true);
				this.battleManager.GamePause(_pause: true);
				componentSortOrder.SetSortOrderFront();
				this.battleManager.IsMultiTargetAppearPlaying = true;
				this.soundManager.PlaySe(eSE.MULTI_TARGETS_START);
				if (!unitController.OneRemainingDisableEffect || componentMultiPartsBoss.BossPartsListForBattle.FindAll((PartsData e) => !e.IsBreak).Count >= 2)
				{
					ManagerSingleton<ResourceManager>.Instance.InstantiateAndGetComponent<Animator>(eResourceId.ANIM_MULTI_TARGET_APPEAR, this.battleManager.UnitUiCtrl.transform, 0L).transform.position = unitController.BottomTransform.position + unitController.FixedCenterPos;
				}

				unitController.Timer(1f, delegate
				{
					this.battleManager.SetSkillScreen(_enable: false);
					componentSortOrder.SetSortOrderBack();
					this.battleManager.IsMultiTargetAppearPlaying = false;
					if (!SingletonMonoBehaviour<BattleHeaderController>.Instance.IsPaused)
					{
						this.battleManager.GamePause(_pause: false);
					}
					blockLayerManager.SetBlockCommon(false);
				});
			}
			else
			{
				this.battleManager.IsPausingEffectSkippedInThisFrame = true;
			}

			for (int num = 0; num < componentMultiPartsBoss.BossPartsListForBattle.Count; num++)
			{
				ResourceManager instance2 = ManagerSingleton<ResourceManager>.Instance;
				MultiTargetCursor cursor = instance2.InstantiateAndGetComponent<MultiTargetCursor>(unitController.UseTargetCursorOver ? eResourceId.ANIM_MULTI_TARGET_CURSOR_OVER : eResourceId.ANIM_MULTI_TARGET_CURSOR, this.battleManager.UnitUiCtrl.transform, 0L);
				PartsData partsData = componentMultiPartsBoss.BossPartsListForBattle[num];
				// Retain the actual cursor identity: SetBreak and multi-boss readiness consume
				// its presence. Scale, panel depth/position and tracking only render that cursor.
				partsData.MultiTargetCursor = cursor;
				partsData.TrackBottom(_isTopBack: false);
				this.battleManager.AppendCoroutine(partsData.TrackBottomCoroutine(_topBack: false), ePauseType.SYSTEM, unitController);
				if (partsData.IsBreak)
				{
					unitController.AppendBreakLog(partsData.BreakSource);
					partsData.OnBreak.Call();
					partsData.SetBreak(_enable: true, this.battleManager.UnitUiCtrl.transform, !partsData.DisableStartBreakEffect);
					unitController.AppendCoroutine(componentMultiPartsBoss.UpdateBreak(partsData.BreakTime, partsData), ePauseType.SYSTEM);
				}

				// OnIsFrontFalse's cursor-panel sorting callback has no gameplay consumer.

			}
		}

		[global::OpenPCR.Migration("b2cd9cfc-b33f-4578-9520-0292d7bed4ad")]
		private void tryPlayMultiBossAppear()
		{
			if (!this.MultiTargetDone)
			{
				this.MultiTargetDone = true;
			}

			if (this.battleManager.BossList.Exists((UnitCtrl e) => !e.ComponentState.MultiTargetDone))
			{
				return;
			}

			if (shouldPlayMultiTarget())

			{
				ManagerSingleton<SaveDataManager>.Instance.MultiTargetFirst = true;
				SingletonMonoBehaviour<UnitUiCtrl>.Instance.HidePopUp();
				this.battleManager.SetSkillScreen(_enable: true);
				this.battleManager.GamePause(_pause: true);
				foreach (UnitCtrl boss in this.battleManager.BossList)
				{
					boss.ComponentSortOrder.SetSortOrderFront();
					if (boss.ComponentMultiPartsBoss.MultiBossPartsData.MultiTargetCursor != null)
					{
						boss.ComponentMultiPartsBoss.MultiBossPartsData.MultiTargetCursor.Panel.sortingOrder = 12190;
					}
				}

				this.soundManager.PlaySe(eSE.MULTI_TARGETS_START);
				ManagerSingleton<ResourceManager>.Instance.InstantiateAndGetComponent<Animator>(eResourceId.ANIM_MULTI_TARGET_APPEAR, this.battleManager.UnitUiCtrl.transform, 0L).transform.position = this.battleManager.BossUnit.BottomTransform.position + this.battleManager.BossUnit.FixedCenterPos;
				unitController.Timer(1f, delegate
				{
					this.battleManager.SetSkillScreen(_enable: false);
					foreach (UnitCtrl boss2 in this.battleManager.BossList)
					{
						boss2.ComponentSortOrder.SetSortOrderBack();
					}

					if (!SingletonMonoBehaviour<BattleHeaderController>.Instance.IsPaused)
					{
						this.battleManager.GamePause(_pause: false);
					}
				});
			}
			else
			{
				this.battleManager.IsPausingEffectSkippedInThisFrame = true;
			}
		}

		[global::OpenPCR.Migration("35cf14a0-1a3a-4046-8c46-033dff5d2b84")]
		private bool isMultiBossDestructionTarget(int _enemyId)
		{
			eBattleCategory currentBattleCategory = Singleton<TempData>.Instance.BattleStageData.CurrentBattleCategory;
			if (currentBattleCategory == eBattleCategory.HATSUNE_BOSS_BATTLE || currentBattleCategory == eBattleCategory.HATSUNE_SPECIAL_BATTLE)
			{
				return HatsuneUtility.IsMultiBossDestructionTarget(_enemyId);
			}

			return false;
		}

		[global::OpenPCR.Migration("65563e3f-2b1d-4720-8e82-effba780904f")]
		private IEnumerator updateGameStartMotionIdle()
		{
			BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
			global::OpenPCR.AnimKit.Animation animation = currentSpineCtrl.state.Data.skeletonData.FindAnimation(currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.GAME_START));
			float endTime = animation.Duration + unitController.BossAppearDelay;
			currentSpineCtrl.PlayAnime(eAnimKitCharacterAnimeId.IDLE);
			while (endTime > 0f)
			{
				endTime -= unitController.DeltaTimeForPause;
				yield return null;
			}

			this.GameStartDone = true;
			IsStartActionForExtraEffect = true;
			this.SetState(ActionState.IDLE);
		}

		[global::OpenPCR.Migration("78f8fbc3-73b0-4621-84dc-994c78edeeaf")]
		private IEnumerator updateGameStart()
		{
			float time = 0f;
			bool[] shakeStarted = new bool[battleEffectData.GameStartShakes.Count];
			if (battleManager.GetPurpose() == eSpecialBattlePurpose.SHIELD || (battleManager.GetPurpose() == eSpecialBattlePurpose.ABSORBER && battleManager.CurrentEnemyPoint < battleManager.GetPurposeCount()))
			{
				unitController.AppendCoroutine(CreatePrefabWithTime(battleEffectData.GameStartEffects, _isIdle: false, -1, _isAura: false, _isShieldEffect: true), ePauseType.SYSTEM);
			}

			while (true)
			{
				unitController.GetCurrentSpineCtrl().Pause();
				time += unitController.DeltaTimeForPause;
				if (time > unitController.BossAppearDelay)
				{
					break;
				}

				yield return null;
			}

			if (unitController.ComponentSound.EnemyVoiceId != 0)
			{
				BattleVoiceUtility.PlayWaveStartVoice(unitController);
			}

			unitController.GetCurrentSpineCtrl().Resume();
			time = 0f;
			unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.GAME_START, -1, -1, -1, _isLoop: false);
			unitController.AppendCoroutine(CreatePrefabWithTime(battleEffectData.GameStartEffects), ePauseType.SYSTEM);
			unitController.AppendCoroutine(CreatePrefabWithTime(battleEffectData.AuraEffects, _isIdle: false, -1, _isAura: true), ePauseType.SYSTEM);
			while (true)
			{
				time += unitController.DeltaTimeForPause;
				for (int i = 0; i < battleEffectData.GameStartShakes.Count; i++)
				{
					if (!shakeStarted[i] && battleEffectData.GameStartShakes[i].StartTime < time)
					{
						unitController.StartShake(battleEffectData.GameStartShakes[i], null, unitController);
						shakeStarted[i] = true;
					}
				}

				if (IsDead)
				{
					GameStartDone = true;
					IsStartActionForExtraEffect = true;
					if (unitController.IsDepthBack)
					{
						unitController.IsDepthBack = false;
						componentSortOrder.SetSortOrderBack();
					}

					yield break;
				}

				if (!unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle || CancelByAwake)
				{
					break;
				}

				yield return null;
			}

			GameStartDone = true;
			IsStartActionForExtraEffect = true;
			if (unitController.IsDepthBack)
			{
				unitController.IsDepthBack = false;
				componentSortOrder.SetSortOrderBack();
			}

			if (!CancelByAwake)
			{
				SetState(ActionState.IDLE);
			}
		}

		[global::OpenPCR.Migration("03922220-9de1-4266-bd7b-6d56231ff85a")]
		private void setStateSkill(int skillId)
		{
			if (skillId == 0)
				return;
			var controller = unitController.UnitActionController;
			if (controller == null)
				return;
			CurrentState = ActionState.SKILL;
			unitController.CurrentSkillId = skillId;
			if (componentAbnormal.IsAbnormalState(eAbnormalState.SILENCE) && componentAbnormal.AttackWhenSilence)
			{
				SetState(ActionState.ATK);
				return;
			}

			if (componentAbnormal.ToadDatas.Count > 0)
			{
				SetState(ActionState.ATK);
				return;
			}

			if (!controller.StartAction(skillId) && !unitController.IsBoss)
			{
				SetState(ActionState.IDLE);
				return;
			}

			componentDamageControl.ChargeEnergy(eSetEnergyType.BY_ATK, (double)unitController.SkillStackVal, false, null, false, unitController, false, eEffectType.COMMON, true, false, 1.0);
			unitController.AppendCoroutine(updateSkill(skillId), ePauseType.SYSTEM, unitController);
			TryPlaySkillVoice(skillId);
		}

		[global::OpenPCR.Migration("eb7d48a2-9b82-408f-9ba5-60e4523d30f1")]
		private IEnumerator updateSkill(int skillId)
		{
			var controller = unitController.UnitActionController;
			while (true)
			{
				if (CancelByConvert || CancelByToad)
				{
					controller.CancelAction(skillId);
					CancelByConvert = false;
					CancelByAwake = false;
					CancelByToad = false;
					yield break;
				}

				ActionState currentState = CurrentState;
				if (currentState == ActionState.UNION_BURST || currentState == ActionState.DAMAGE || currentState == ActionState.DIE)
				{
					controller.CancelAction(skillId);
					CancelByConvert = false;
					CancelByAwake = false;
					yield break;
				}

				if (CancelByAwake && skillId != CurrentTriggerSkillId)
				{
					controller.CancelAction(skillId);
					CancelByAwake = false;
					CancelByConvert = false;
					CancelByToad = false;
					yield break;
				}

				if (controller.HasNextAnime(skillId) && controller.IsLoopMotionPlaying(skillId))
				{
					yield break;
				}

				if (!unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle || controller.GetSkillMotionType(skillId) == SkillMotionType.NONE)
				{
					break;
				}

				yield return null;
			}

			if (controller.IsModeChange(skillId))
			{
				Color color = unitController.UnitSpineCtrl.CurColor;
				unitController.UnitSpineCtrl.IsAlphaZero = true;
				unitController.UnitSpineCtrl.SetCurColor(color);
				unitController.UnitSpineCtrlModeChange.IsAlphaZero = false;
				unitController.UnitSpineCtrlModeChange.SetCurColor(color);
				unitController.UnitSpineCtrlModeChange.gameObject.SetActive(value: true);
				unitController.MotionPrefix = 1;
				OnMotionPrefixChanged.Call();
				foreach (KeyValuePair<int, List<(SkillEffectCtrl, string)>> item in unitController.DamagedHpEffectDictionary)
				{
					foreach (var item2 in item.Value)
					{
						battleManager.StartCoroutine(item2.Item1.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero, _followX: true, _followY: true, unitController.GetCurrentSpineCtrl().skeleton.FindBone(item2.Item2)));
					}
				}

				if (unitController.ComponentSortOrder.IsFront)
				{
					unitController.ComponentSortOrder.SetSortOrderFront();
				}
				else
				{
					componentSortOrder.SetSortOrderBack();
				}

				unitController.SetLeftDirection(unitController.IsLeftDir);
				int skillNum = controller.GetSkillNum(skillId);
				eAnimKitCharacterAnimeId animeId = controller.GetAnimeId(skillId);
				if (unitController.UnitSpineCtrlModeChange.IsAnimation(unitController.UnitSpineCtrlModeChange.ConvertAnimeIdToAnimeName(animeId, skillNum, -1, 1)))
				{
					unitController.ComponentAnimation.PlayAnimeNoOverlap(animeId, skillNum, -1, 1, _isLoop: false, unitController.UnitSpineCtrlModeChange);
					unitController.AppendCoroutine(updateModeChange(), ePauseType.SYSTEM, unitController);
				}
				else
				{
					ModeChangeUnableStateBarrier = false;
					SetState(ActionState.IDLE);
					battleManager.UpdateAbnormalStateField();
				}
			}
			else if (!controller.HasNextAnime(skillId))
			{
				SetState(ActionState.IDLE);
				CancelByConvert = false;
				CancelByAwake = false;
				CancelByToad = false;
			}
		}

		[global::OpenPCR.Migration("da867ddd-c70b-461a-986d-4043037c43af")]
		private void playSubUbVoiceWithDelay(Skill _skill)
		{
			List<VoiceDelayAndEnable> list = null;
			List<VoiceDelayAndEnable> list2 = null;
			if (unitController.GetSpeedUpFlag())
			{
				if (componentUnionBurst.PlayCutInFlag)
				{
					list = _skill.SpeedUpWithCutinVoice.Voice1List;
					list2 = _skill.SpeedUpWithCutinVoice.Voice2List;
				}
				else
				{
					list = _skill.SpeedUpNoCutinVoice.Voice1List;
					list2 = _skill.SpeedUpNoCutinVoice.Voice2List;
				}
			}
			else if (componentUnionBurst.PlayCutInFlag)
			{
				list = _skill.NormalWithCutinVoice.Voice1List;
				list2 = _skill.NormalWithCutinVoice.Voice2List;
			}
			else
			{
				list = _skill.NormalNoCutinVoice.Voice1List;
				list2 = _skill.NormalNoCutinVoice.Voice2List;
			}

			playVoiceDataList(list, 100, _skill.SkillNum);
			playVoiceDataList(list2, 200, _skill.SkillNum);
		}

		[global::OpenPCR.Migration("579d53eb-b113-47c9-acc1-7b044daea3b6")]
		private void playUbVoiceWithDelay()
		{
			List<VoiceDelayAndEnable> list = null;
			list = (unitController.GetSpeedUpFlag() ? ((!componentUnionBurst.PlayCutInFlag) ? unitController.SpeedUpSkillVoiceDelay : unitController.SpeedUpSkillVoiceDelayWithCutIn) : ((!componentUnionBurst.PlayCutInFlag) ? unitController.NormalSkillVoiceDelay : unitController.NormalSkillVoiceDelayWithCutIn));
			playVoiceDataList(list, 100);
		}

		[global::OpenPCR.Migration("9b59fc5b-c760-425d-9077-f121e5ef494c")]
		private void playUbVoice3WithDelay()
		{
			List<VoiceDelayAndEnable> list = null;
			list = (unitController.GetSpeedUpFlag() ? ((!componentUnionBurst.PlayCutInFlag) ? unitController.SpeedUpSkillVoice3Delay : unitController.SpeedUpSkillVoice3DelayWithCutIn) : ((!componentUnionBurst.PlayCutInFlag) ? unitController.NormalSkillVoice3Delay : unitController.NormalSkillVoice3DelayWithCutIn));
			playVoiceDataList(list, 300);
		}

		[global::OpenPCR.Migration("0f17b354-3c56-4b46-9689-428a90034649")]
		private void playUbVoice4WithDelay()
		{
			List<VoiceDelayAndEnable> list = null;
			list = (unitController.GetSpeedUpFlag() ? ((!componentUnionBurst.PlayCutInFlag) ? unitController.SpeedUpSkillVoice4Delay : unitController.SpeedUpSkillVoice4DelayWithCutIn) : ((!componentUnionBurst.PlayCutInFlag) ? unitController.NormalSkillVoice4Delay : unitController.NormalSkillVoice4DelayWithCutIn));
			playVoiceDataList(list, 400);
		}

		[global::OpenPCR.Migration("96298c48-ce3a-47b9-b6a4-66f801b06310")]
		private void playVoiceDataList(List<VoiceDelayAndEnable> _dataList, int _id, int _skillNum = 0)
		{
			if (!unitController.GetIsMuteVoice())
				throw new global::OpenPCR.OutOfBoundaryException("Offline UB voice output requires explicitly muted voice; authored voice timing/cue outputs are outside this scope.");
		}

		[global::OpenPCR.Migration("bb8d1fdd-89d3-41c5-ad2f-93fc83b3810a")]
		private void playUbNameVoiceWithDelay()
		{
			List<VoiceDelayAndEnable> list = null;
			list = (unitController.GetSpeedUpFlag() ? ((!componentUnionBurst.PlayCutInFlag) ? unitController.SpeedUpSkillNameVoiceDelay : unitController.SpeedUpSkillNameVoiceDelayWithCutIn) : ((!componentUnionBurst.PlayCutInFlag) ? unitController.NormalSkillNameVoiceDelay : unitController.NormalSkillNameVoiceDelayWithCutIn));
			playVoiceDataList(list, 200);
		}

		[global::OpenPCR.Migration("8dae5891-5d0b-4b3e-b147-c5c68c6f984c")]
		private IEnumerator updateUnionBurst()
		{
			var controller = unitController.UnitActionController;
			int unionBurstSkillId;
			while (true)
			{
				unionBurstSkillId = unitController.UnionBurstSkillId;
				if (CancelByConvert || CancelByToad || CurrentState == ActionState.DAMAGE || CurrentState == ActionState.DIE)
				{
					controller.CancelAction(unionBurstSkillId);
					CancelByConvert = false;
					CancelByToad = false;
					yield break;
				}
				if (!unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle || HasEndUnionBurstAnimeForIfAction) break;
				yield return null;
			}
			HasEndUnionBurstAnimeForIfAction = false;
			if (controller.IsModeChange(unionBurstSkillId))
			{
				BattleAnimKitController main = unitController.UnitSpineCtrl;
				BattleAnimKitController mode = unitController.UnitSpineCtrlModeChange;
				Color color = main.CurColor;
				main.IsAlphaZero = true;
				main.SetCurColor(color);
				mode.IsAlphaZero = false;
				mode.SetCurColor(color);
				mode.gameObject.SetActive(true);
				unitController.MotionPrefix = 1;
				OnMotionPrefixChanged.Call();
				if (componentSortOrder.IsFront) componentSortOrder.SetSortOrderFront();
				else componentSortOrder.SetSortOrderBack();
				unitController.SetLeftDirection(unitController.IsLeftDir);
				if (mode.IsAnimation(mode.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.SKILL, 0, -1, 1)))
				{
					componentAnimation.PlayAnimeNoOverlap(eAnimKitCharacterAnimeId.SKILL, 0, -1, 1, false, unitController.UnitSpineCtrlModeChange);
					unitController.AppendCoroutine(updateModeChange(), ePauseType.SYSTEM, unitController);
				}
				else SetState(ActionState.IDLE);
				yield break;
			}
			if (!controller.HasNextAnime(unionBurstSkillId))
			{
				SkillEndProcess();
				yield break;
			}
			while (CurrentState != ActionState.IDLE)
			{
				if (CurrentState == ActionState.DAMAGE || CurrentState == ActionState.DIE)
				{
					controller.CancelAction(unionBurstSkillId);
					CancelByConvert = false;
					CancelByToad = false;
					yield break;
				}
				yield return null;
			}
		}

		[global::OpenPCR.Migration("66c428a6-2400-4ef1-b763-67b49b4f258c")]
		public void SkillEndProcess()
		{
			SetState(ActionState.IDLE);
			CancelByConvert = false;
			CancelByToad = false;
		}

		[global::OpenPCR.Migration("29b63969-93fc-4aea-a98a-ea2ca03fec0e")]
		private IEnumerator updateModeChange()
		{
			while (unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle)
			{
				yield return null;
			}

			this.ModeChangeUnableStateBarrier = false;
			this.SetState(ActionState.IDLE);
		}

		[global::OpenPCR.Migration("f9a91019-0a85-49ef-83bd-3c304b553192")]
		private void setStateDie()
		{
			if (battleManager.GetPurpose() == eSpecialBattlePurpose.GET_POINT && !EnemyPointDone)
			{
				EnemyPointDone = true;
				if (EnemyPoint != 0)
				{
					battleManager.KillOrder.Add(unitController.UnitId);
					battleManager.SubstructEnemyPoint(EnemyPoint);
				}
			}

			if (battleManager.IsEndlessBattle && unitController.IsOther && !WasDefeatCount)
			{
				WasDefeatCount = true;
				battleManager.IncrementDefeatCountForEndleeBattle();
				var header = Singleton<BattleHeaderController>.Instance;
				header.SetGoldNum(unchecked(header.Gold + (int)unitController.Rupee));
			}

			switch (CurrentState)
			{
				case ActionState.DAMAGE:
					if (OnDeadForRevival == null && !IsDead && UnDeadTimeHitCount <= 1)
					{
						IsDead = true;
						battleManager.CallbackDead(unitController);
						if (runSmokeEffect != null)
						{
							runSmokeEffect.SetTimeToDie(value: true);
						}
					}

					return;
				case ActionState.DIE:
					return;
			}

			if (OnDeadForRevival == null && !IsDead && UnDeadTimeHitCount <= 1)
			{
				IsDead = true;
				battleManager.CallbackDead(unitController);
				if (runSmokeEffect != null)
				{
					runSmokeEffect.SetTimeToDie(value: true);
				}
			}

			if ((unitController.ActionsTargetOnMe.Count > 0 || unitController.FirearmCtrlsOnMe.Count > 0) && UnDeadTimeHitCount == 0)
			{
				SetState(ActionState.DAMAGE, 0, 0, _quiet: true);
				return;
			}

			if (componentAbnormal.ToadDatas.Count > 0)
			{
				DieInToad = true;
				unitController.ComponentAbnormal.ToadDatas[0].Enable = false;
				return;
			}

			battleManager.CheckAcnLosingBattleFinish();
			componentAbnormal.CureAllAbnormalState();
			if (unitController.ComponentDamageControl.KillBonusTarget != null)
			{
				componentDamageControl.KillBonusTarget.ComponentDamageControl.ChargeEnergy(eSetEnergyType.KILL_BONUS, battleManager.EnergyStackValueDefeat, _slipDamage: false, null, _hasEffect: false, null, _hasNumberEffect: false);
			}

			for (int num = unitController.RepeatEffectList.Count; num > 0; num--)
			{
				SkillEffectCtrl skillEffectCtrl = unitController.RepeatEffectList[num - 1];
				if (skillEffectCtrl != null && !(skillEffectCtrl is FirearmCtrl))
				{
					skillEffectCtrl.SetTimeToDie(value: true);
					skillEffectCtrl.OnEffectEnd.Call(skillEffectCtrl);
				}
			}

			if (UnDeadTimeHitCount == 1)
			{
				unitController.ComponentAnimation.PlayAnimeNoOverlap(eAnimKitCharacterAnimeId.DIE, 1);
			}
			else if (unitController.IsDivisionSourceForDie)
			{
				OnStartErrorUndoDivision.Call(arg1: false);
			}
			else
			{
				unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.DIE, -1, unitController.MotionPrefix, -1, _isLoop: false);
			}

			if (battleEffectData.DieEffects.Count > 0 && !unitController.IsDivisionSourceForDie)
			{
				unitController.AppendCoroutine(CreatePrefabWithTime(battleEffectData.DieEffects, _isIdle: false, -1, _isAura: false, _isShieldEffect: false, _ignorePause: true), ePauseType.IGNORE_BLACK_OUT);
			}

			int i = 0;
			for (int count = battleEffectData.DieShakes.Count; i < count; i++)
			{
				unitController.AppendCoroutine(startShakeWithDelay(battleEffectData.DieShakes[i], _ignorePause: true), ePauseType.IGNORE_BLACK_OUT);
			}

			if (OnDieForZeroHp != null && OnDeadForRevival == null)
			{
				OnDieForZeroHp(unitController);
			}

			unitController.ComponentSound.PlayRetireVoice();
			componentDamageControl.ResetAccumulateDamage();
			componentSearchArea.ResetTargetList();
			CurrentState = ActionState.DIE;
			if (unitController.IsPartsBoss)
			{
				for (int j = 0; j < componentMultiPartsBoss.BossPartsListForBattle.Count; j++)
				{
					unitController.ComponentMultiPartsBoss.BossPartsListForBattle[j].DisableCursor();
				}
			}

			if (unitController.ComponentMultiPartsBoss.MultiBossPartsData != null)
			{
				componentMultiPartsBoss.MultiBossPartsData.DisableCursor();
			}

			foreach (KeyValuePair<int, UnitCtrl> item in unitController.SummonUnitDictionary)
			{
				item.Value.ComponentState.IdleOnly = true;
				item.Value.ComponentAbnormal.CureAllAbnormalState();
				battleManager.CallbackDead(item.Value);
			}

			if (unitController.HasDieLoop)
			{
				unitController.RespawnPos = eUnitRespawnPos.MAIN_POS_1;
				if (!unitController.ComponentSortOrder.IsFront)
				{
					componentSortOrder.SetSortOrderBack();
				}
			}

			if (unitController.IsDivisionSourceForDie)
			{
				unitController.AppendCoroutine(updateUndoDivision(), ePauseType.SYSTEM);
			}
			else
			{
				unitController.AppendCoroutine(updateDie(++dieCoroutineId), ePauseType.SYSTEM);
			}
		}

		[global::OpenPCR.Migration("6c3d78d1-8b7f-437f-b424-967dd2231634")]
		private IEnumerator updateDie(int _thisId)
		{
			while (true)
			{
				if (_thisId != dieCoroutineId)
				{
					yield break;
				}

				BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
				if (currentSpineCtrl == null)
				{
					yield break;
				}

				if (!currentSpineCtrl.IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			_ = battleManager.CanPlaySe(); // Die-fade sound output only.

			if (unitController.HasDieLoop)
			{
				unitController.GetCurrentSpineCtrl().PlayAnime(eAnimKitCharacterAnimeId.DIE_LOOP);
				battleManager.CallbackFadeOutDone(unitController);
				yield break;
			}

			float fAlpha = (unitController.IsDeadBySetCurrentHp ? unitController.GetCurrentSpineCtrl().CurColor.a : 1f);
			while (_thisId == dieCoroutineId)
			{
				fAlpha -= unitController.DeltaTimeForPause;
				if (fAlpha <= 0f)
				{
					if (OnDieFadeOutEnd != null && OnDeadForRevival == null)
					{
						OnDieFadeOutEnd(unitController);
						OnDieFadeOutEnd = null;
					}

					if (OnDeadForRevival != null)
					{
						OnDeadForRevival(unitController);
						break;
					}

					if (UnitUtility.JudgeIsSummon(unitController.UnitId))
					{
						List<UnitCtrl> list = (unitController.IsOther ? battleManager.EnemyList : battleManager.UnitList);
						if (list.Contains(unitController))
						{
							list.Remove(unitController);
						}

						if (unitController.gameObject.activeSelf)
						{
							unitController.gameObject.SetActive(value: false);
							battleManager.CallbackFadeOutDone(unitController);
						}

						break;
					}

					if (unitController.IsPartsBoss)
					{
						foreach (PartsData bossParts in unitController.BossPartsList)
						{
							bossParts.FixAttachment(unitController);
						}
					}

					unitController.gameObject.SetActive(value: false);
					battleManager.CallbackFadeOutDone(unitController);
					break;
				}

				if (unitController.GetCurrentSpineCtrl() == null)
				{
					break;
				}

				unitController.GetCurrentSpineCtrl().CurColor = new Color(1f, 1f, 1f, fAlpha);
				yield return null;
			}
		}

		[global::OpenPCR.Migration("d3c60858-955a-4ba9-aff8-be364b3aaf4c")]
		private IEnumerator updateUndoDivision()
		{
			while (unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle)
			{
				yield return null;
			}

			componentAnimation.PlayAnime(eAnimKitCharacterAnimeId.DIE, -1, unitController.MotionPrefix, -1, _isLoop: false);
			unitController.AppendCoroutine(this.CreatePrefabWithTime(battleEffectData.DieEffects), ePauseType.VISUAL, unitController);
			unitController.AppendCoroutine(this.updateDie(++this.dieCoroutineId), ePauseType.SYSTEM);
		}

		[global::OpenPCR.Migration("57ddc20c-aaa6-40e7-a507-e977c6b3e97a")]
		private void setStateDamage(bool _quiet)
		{
			ActionState currentState = this.CurrentState;
			IsModeChangeEnd = false;
			if (currentState == ActionState.DAMAGE || currentState == ActionState.DIE)
			{
				return;
			}

			if (this.ToadRelease)
			{
				this.ToadReleaseDamage = true;
				return;
			}

			if (battleEffectData.DamageEffects.Count > 0)
			{
				unitController.AppendCoroutine(this.CreatePrefabWithTime(battleEffectData.DamageEffects), ePauseType.SYSTEM);
			}

			int i = 0;
			for (int count = battleEffectData.DamageShakes.Count; i < count; i++)
			{
				unitController.AppendCoroutine(this.startShakeWithDelay(battleEffectData.DamageShakes[i]), ePauseType.SYSTEM);
			}

			if (!_quiet)
			{
				componentSound.PlayDamageVoice();
			}

			componentAnimation.PlayAnime(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix, -1, -1, _isLoop: false, null, _quiet: false, 0f, _ignoreBlackout: true);
			this.CurrentState = ActionState.DAMAGE;
			unitController.AppendCoroutine(this.updateDamage(++this.damageCoroutineId), ePauseType.IGNORE_BLACK_OUT);
		}

		[global::OpenPCR.Migration("d9d8cb0d-d876-45d0-8955-768a42b31ffb")]
		private IEnumerator updateDamage(int _thisId)
		{
			float time = 0f;
			while (true)
			{
				if (stopUpdateDamageFlag || _thisId != damageCoroutineId)
				{
					stopUpdateDamageFlag = false;
					yield break;
				}

				if ((long)unitController.ComponentParameter.Hp <= 0)
				{
					if (unitController.ActionsTargetOnMe.Count > 0 || unitController.FirearmCtrlsOnMe.Count > 0 || unitController.ComponentAbnormal.KnockBackEnableCount > 0)
					{
						time += battleManager.DeltaTime;
						if (time > 10f)
						{
							bool flag = false;
							for (int num = unitController.ActionsTargetOnMe.Count - 1; num >= 0; num--)
							{
								flag = true;
								unitController.ActionsTargetOnMe.RemoveAt(num);
							}

							for (int num2 = unitController.FirearmCtrlsOnMe.Count - 1; num2 >= 0; num2--)
							{
								flag = true;
								unitController.FirearmCtrlsOnMe.RemoveAt(num2);
							}

							if (flag)
							{
								continue;
							}
						}

						BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
						TrackEntry current = currentSpineCtrl.state.GetCurrent(0);
						if (current != null && current.lastTime >= currentSpineCtrl.StopStateTime)
						{
							current.lastTime = currentSpineCtrl.StopStateTime;
							current.time = currentSpineCtrl.StopStateTime;
							currentSpineCtrl.Pause();
						}

						yield return null;
						continue;
					}

					unitController.GetCurrentSpineCtrl().Resume();
					CurrentState = ActionState.IDLE;
					SetState(ActionState.DIE);
					stopUpdateDamageFlag = false;
					yield break;
				}

				if (isPauseDamageMotion())
				{
					resumeIsStopState = true;
					BattleAnimKitController currentSpineCtrl2 = unitController.GetCurrentSpineCtrl();
					TrackEntry current2 = currentSpineCtrl2.state.GetCurrent(0);
					if (current2 != null)
					{
						current2.lastTime = currentSpineCtrl2.StopStateTime;
						current2.time = currentSpineCtrl2.StopStateTime;
					}

					bool isInitPause = !unitController.ComponentAbnormal.IsSpecialSleepStatus;
					if (isInitPause)
					{
						unitController.GetCurrentSpineCtrl().Pause();
					}

					while (unitController.ComponentAbnormal.IsUnableActionState() && (long)unitController.ComponentParameter.Hp != 0L)
					{
						BattleAnimKitController currentSpineCtrl3 = unitController.GetCurrentSpineCtrl();
						if (stopUpdateDamageFlag || _thisId != damageCoroutineId)
						{
							stopUpdateDamageFlag = false;
							yield break;
						}

						if (!isInitPause && !unitController.ComponentAbnormal.IsSpecialSleepStatus)
						{
							currentSpineCtrl3.Pause();
							isInitPause = true;
						}

						if (unitController.ComponentAbnormal.IsPlayDamageAnimForAbnormal)
						{
							unitController.ComponentAbnormal.IsPlayDamageAnimForAbnormal = false;
							unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix, -1, -1, _isLoop: false, null, _quiet: false, 0f, _ignoreBlackout: true);
							break;
						}

						yield return null;
					}
				}

				if (unitController.ComponentAbnormal.IsSpecialSleepStatus)
				{
					resumeIsStopState = true;
				}

				unitController.GetCurrentSpineCtrl().Resume();
				if ((long)unitController.ComponentParameter.Hp == 0L)
				{
					CurrentState = ActionState.IDLE;
					SetState(ActionState.DIE);
					stopUpdateDamageFlag = false;
					yield break;
				}

				if (unitController.ComponentAbnormal.IsSpecialSleepStatus && unitController.GetCurrentSpineCtrl().HasSpecialSleepAnimatilon(unitController.MotionPrefix))
				{
					unitController.ComponentAbnormal.ReleaseSleepAnime();
				}

				if (!unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle && !unitController.ComponentAbnormal.IsUnableActionState())
				{
					break;
				}

				yield return null;
			}

			if (!ToadRelease || !IdleOnly)
			{
				SetState(ActionState.IDLE);
			}
		}

		[global::OpenPCR.Migration("fd014807-0dc4-46b3-be6f-308cec548239")]
		private bool isPauseDamageMotion()
		{
			if (!componentAbnormal.IsUnableActionState())
			{
				return false;
			}

			if (!unitController.GetCurrentSpineCtrl().IsStopState)
			{
				return false;
			}

			return true;
		}

		[global::OpenPCR.Migration("10647088-8ba7-4df8-88e1-243708c43cfc")]
		private void setStateAttack()
		{
			CurrentState = ActionState.ATK;
			unitController.CurrentSkillId = 1;
			var controller = componentAbnormal.ToadDatas.Count > 0 ? componentAbnormal.ToadDatas[0].UnitActionController : unitController.UnitActionController;
			if (!controller.StartAction(1))
			{
				SetState(ActionState.IDLE);
				return;
			}

			componentDamageControl.ChargeEnergy(eSetEnergyType.BY_ATK, (double)unitController.SkillStackVal, false, null, false, unitController, false, eEffectType.COMMON, true, false, 1.0);
			if (judgePlayVoice("attack", 1, false, 0f))
				componentSound.PlayVoice(SoundManager.eVoiceType.ATTACK, 0, true);
			unitController.AppendCoroutine(updateAttack(controller), ePauseType.SYSTEM, unitController);
		}

		[global::OpenPCR.Migration("ca43242e-c6aa-483c-99c2-915e349bac26")]
		private bool judgePlayVoice(string _voiceName, int _maxNum, bool _isSkill, float _voiceOffsetRate)
		{
			bool playVoice = true;
			int counter = 0;
			if (unitController.ComponentSound.EnemyVoiceId != 0)
			{
				return true;
			}

			if (!unitController.IsPlayerUnit)
			{
				return false;
			}

			if (!battleManager.CanPlayVoice())
			{
				return false;
			}

			Action<List<UnitCtrl>> action = delegate (List<UnitCtrl> list)
			{
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					if (list[i].ComponentSound.IsVoicePlaying(_voiceName))
					{
						counter++;
						if (counter == _maxNum)
						{
							playVoice = false;
						}

						break;
					}
				}
			};
			action(battleManager.UnitList);
			if (playVoice)
			{
				action(battleManager.EnemyList);
			}

			if (_isSkill && Time.time - battleManager.LatestSkillVoiceTime < 0.5f)
			{
				playVoice = false;
			}

			if (playVoice)
			{
				if (battleManager.JudgeVoicePlay(unitController.ComponentSound.SoundUnitId, _maxNum, _voiceOffsetRate))
				{
					if (_isSkill)
					{
						battleManager.LatestSkillVoiceTime = Time.time;
					}

					return true;
				}
			}
			else
			{
				battleManager.IncrementNoneVoiceAttackCount();
			}

			return false;
		}

		[global::OpenPCR.Migration("d3b45fe6-35d8-4dac-9c86-7d3f64f4ebb2")]
		private IEnumerator updateAttack(UnitActionController _actionController)
		{
			while (true)
			{
				if (this.IsCancelActionState(_isUnionBurst: false))
				{
					_actionController.CancelAction(1);
					this.CancelByAwake = false;
					this.CancelByConvert = false;
					this.CancelByToad = false;
					yield break;
				}

				if (!unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			this.SetState(ActionState.IDLE);
			this.CancelByAwake = false;
			this.CancelByConvert = false;
			this.CancelByToad = false;
		}

		[global::OpenPCR.Migration("8ccf2419-f449-4b06-97e7-d5084a4278c0")]
		private void setStateWalk()
		{
			if (CurrentState != ActionState.IDLE && CurrentState != ActionState.SUMMON)
				return;
			ExecStartSkill();
			unitController.SetDirectionAuto();
			CurrentState = ActionState.WALK;
			unitController.AppendCoroutine(updateWalk(++walkCoroutineId), ePauseType.SYSTEM, unitController);
			eAnimKitCharacterAnimeId animeId = unitController.MoveRate == 0f || IsForceStartStay ? eAnimKitCharacterAnimeId.IDLE : isRunForCatchUp || StandByDone ? eAnimKitCharacterAnimeId.RUN : eAnimKitCharacterAnimeId.RUN_GAME_START;
			componentAnimation.PlayAnime(animeId, unitController.MotionPrefix);
		}

		[global::OpenPCR.Migration("b04c402c-9967-45fe-b88d-1eee120361f7")]
		private IEnumerator updateWalk(int coroutineId)
		{
			while (!judgeWalkEnd(coroutineId))
			{
				var enemies = unitController.IsOther ? battleManager.UnitList : battleManager.EnemyList;
				bool noneAlive = enemies.FindIndex(e => (long)e.ComponentParameter.Hp > 0L) == -1;
				bool allHidden = enemies.TrueForAll(e => e.ComponentAbnormal.IsStealth || (long)e.ComponentParameter.Hp == 0L || e.ComponentAbnormal.IsSpyOrEtherialization(unitController));
				var gameState = battleManager.GameState;
				BattleAnimKitController spine = unitController.GetCurrentSpineCtrl();
				var position = unitController.transform.localPosition;
				float rate = IsForceStartStay ? 0f : unitController.MoveRate;
				position.x += rate * unitController.DeltaTimeForPause * (isRunForCatchUp ? 1f : 1.6f);
				if (gameState == eBattleGameState.NEXT_WAVE_PROCESS || !(noneAlive | allHidden))
				{
					if (spine.AnimationName == spine.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix) && rate != 0f)
						componentAnimation.PlayAnime(eAnimKitCharacterAnimeId.RUN, unitController.MotionPrefix);
					unitController.transform.localPosition = position;
				}
				else if (spine.AnimationName == spine.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.RUN, unitController.MotionPrefix))
					componentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix);
				unitController.SetDirectionAuto();
				yield return null;
			}
		}

		[global::OpenPCR.Migration("cee5958f-4dfd-42d7-b15a-5521f57c2ca8")]
		private void setStateIdle()
		{
			if (runSmokeEffect != null)
			{
				runSmokeEffect.SetTimeToDie(value: true);
				runSmokeEffect = null;
			}

			if (unitController.MotionPrefix == 1 && !unitController.UnitActionController.ModeChanging)
			{
				return;
			}

			unitController.IsAwakeMotion = false;
			unitController.SetDirectionAuto();
			if (battleEffectData.IdleEffects.Count > 0 && !IsDead)
			{
				unitController.AppendCoroutine(CreatePrefabWithTime(battleEffectData.IdleEffects, _isIdle: true), ePauseType.VISUAL, unitController);
			}

			if (unitController.CurrentSkillId == unitController.UnionBurstSkillId)
			{
				unitController.PlayUbChainVoice();
				for (int num = componentAbnormal.UbAbnormalDataList.Count - 1; num >= 0; num--)
				{
					componentAbnormal.UbAbnormalDataList[num].Exec(unitController);
					componentAbnormal.UbAbnormalDataList.RemoveAt(num);
					componentAbnormal.OnChangeState.Call(eStateIconType.UB_DISABLE, arg2: false);
				}
			}

			foreach (KeyValuePair<eAbnormalState, Action<bool>> item in unitController.ComponentAbnormal.DamageByBehaviourDictionary)
			{
				item.Value.Call(arg1: false);
			}

			ActionState currentState = CurrentState;
			if ((uint)(currentState - 1) > 5u && currentState != ActionState.GAME_START)
			{
				return;
			}

			CurrentState = ActionState.IDLE;
			if (IdleOnly && !ToadRelease)
			{
				battleManager.CallbackIdleOnlyDone(unitController);
			}

			if (StandByDone)
			{
				BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
				if (SpecialIdleMotionId == 0)
				{
					if (currentSpineCtrl.AnimationName != currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE) && !ToadRelease)
					{
						unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix);
					}
				}
				else if (currentSpineCtrl.AnimationName != currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.SPECIAL_IDLE, SpecialIdleMotionId) && !ToadRelease)
				{
					unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.SPECIAL_IDLE, SpecialIdleMotionId);
				}

				battleManager.CallbackStartDashDone(unitController);
			}
			else
			{
				unitController.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.STAND_BY, unitController.MotionPrefix, -1, -1, _isLoop: false);
				if (IsStartVoicePlay && battleManager.CanPlayVoice())
				{
					BattleVoiceUtility.PlayWaveStartVoice(unitController);
				}

				unitController.AppendCoroutine(updateStandBy(), ePauseType.VISUAL, unitController);
			}

			ExecStartSkill();
			unitController.AppendCoroutine(updateIdle(), ePauseType.SYSTEM, unitController);
		}

		[global::OpenPCR.Migration("64d6b4c1-2c01-49b9-a0d5-c2b2712917cf")]
		public void ExecStartSkill()
		{
			if (unitController.IsStartSkillExeced || IdleOnly)
				return;
			var skills = componentPassiveSkill.StartExSkillList;
			var controller = unitController.UnitActionController;
			foreach (var skill in skills)
				skill.Exec(controller);
			unitController.IsStartSkillExeced = true;
			unitController.GetCurrentSpineCtrl().SetTimeScale(1f);
		}

		[global::OpenPCR.Migration("df66eb8f-8fb7-4cc6-bda4-d292dbfc9550")]
		private IEnumerator updateStandBy()
		{
			while (unitController.GetCurrentSpineCtrl().IsPlayAnimeBattle)
			{
				if (this.CurrentState != ActionState.IDLE)
				{
					this.StandByDone = true;
					this.battleManager.CallbackStanbyDone(unitController);
					yield break;
				}

				yield return null;
			}

			this.StandByDone = true;
			this.battleManager.CallbackStanbyDone(unitController);
			if (this.CurrentState == ActionState.IDLE)
			{
				componentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix);
			}
		}

		[global::OpenPCR.Migration("c908a86b-24ea-48f2-9e75-ad430c112aaf")]
		private IEnumerator updateIdle()
		{
			if (IdleStartAfterWaitFrame)
			{
				IdleStartAfterWaitFrame = false;
				yield return null;
			}

			float fAlpha = 1f;
			while (true)
			{
				if (IsModeChangeEnd)
				{
					yield return null;
					continue;
				}

				if (IdleOnly && EnemyPoint != 0 && battleManager.CurrentEnemyPoint >= battleManager.GetSpecialBattleData().PurposeCount)
				{
					SetState(ActionState.DIE);
				}

				if (IdleOnly || WaveStartIdle)
				{
					if (CurrentState != ActionState.IDLE)
					{
						yield break;
					}

					if (unitController.IsSummonOrPhantom || unitController.IsGuestFadeout)
					{
						List<UnitCtrl> list = (unitController.IsOther ? battleManager.EnemyList : battleManager.UnitList);
						if (list.Contains(unitController))
						{
							if (!unitController.IsGuestFadeout)
							{
								list.Remove(unitController);
							}

							if (!unitController.IsOther)
							{
								battleManager.FadeOutUnitList.Add(unitController);
							}
						}

						fAlpha -= battleManager.DeltaTime;
						if (fAlpha <= 0f || BattleUtil.Approximately(fAlpha, 0f))
						{
							if (OnDieFadeOutEnd != null && OnDeadForRevival == null)
							{
								OnDieFadeOutEnd(unitController);
								OnDieFadeOutEnd = null;
							}

							if (OnDeadForRevival != null)
							{
								OnDeadForRevival(unitController);
								OnDeadForRevival = null;
								yield break;
							}

							for (int num = idleEffectsObjs.Count - 1; num >= 0; num--)
							{
								idleEffectsObjs[num].SetTimeToDie(value: true);
								idleEffectsObjs.RemoveAt(num);
							}

							for (int i = componentAbnormal.CharaAuraEffectList.Count - 1; i >= 0; i--)
							{
								var effect = componentAbnormal.CharaAuraEffectList[i];
								effect.left.SetTimeToDie(true);
								effect.left.SetActive(false);
								effect.right.SetTimeToDie(true);
								effect.right.SetActive(false);
								componentAbnormal.CharaAuraEffectList.RemoveAt(i);
							}
							componentAbnormal.CureAllAbnormalState();
							battleManager.CallbackFadeOutDone(unitController);
							unitController.gameObject.SetActive(value: false);
							if (!unitController.IsOther)
							{
								battleManager.FadeOutUnitList.Remove(unitController);
							}

							yield break;
						}

						unitController.GetCurrentSpineCtrl().CurColor = new Color(1f, 1f, 1f, fAlpha);
					}

					yield return null;
					continue;
				}

				if (!componentAttackPattern.HasLoopPattern())
				{
					yield return null;
					continue;
				}

				if (CurrentState == ActionState.IDLE)
				{
					OnUpdateWhenIdle.Call(battleManager.TimeLimit);
				}

				if (CurrentState != ActionState.IDLE)
				{
					yield break;
				}

				if (battleManager.BlackOutUnitList.Contains(unitController))
				{
					yield return null;
					continue;
				}

				if (judgeStateIsWalk())
				{
					yield break;
				}

				if ((battleManager.ActionStartTimeCounter <= 0f || unitController.IsOther) && !ToadRelease)
				{
					unitController.CastTimer = (float)unitController.CastTimer - unitController.DeltaTimeForPause;
				}

				if (battleManager.CountIsNotPartsBossReady > 0)
				{
					yield return null;
					continue;
				}

				if (ToadRelease)
				{
					yield return null;
					continue;
				}

				if ((float)unitController.CastTimer <= 0f && componentAttackPattern.ActionPatternDictionary != null && !unitController.IsBonusEnemy)
				{
					componentSearchArea.UpdateAttackTargetImpl(componentAbnormal.GetTauntSource());
					if (judgeStateIsWalk())
					{
						yield break;
					}

					int num2 = componentAttackPattern.GetSkillId();
					ActionState state = num2 == 1 ? ActionState.ATK : ActionState.SKILL;
					int nextSkillId = componentAttackPattern.MoveToNext();
					CancelByAwake = false;
					SetState(state, nextSkillId, num2);
					yield break;
				}

				if ((long)unitController.ComponentParameter.Hp <= 0)
				{
					break;
				}

				yield return null;
			}

			SetState(ActionState.DIE);
		}

		[global::OpenPCR.Migration("63ce15f1-995e-47bc-a550-54037ea289f4")]
		private bool judgeStateIsWalk()
		{
			if (!componentAbnormal.IsConfusionOrConvert())
			{
				if (componentAbnormal.IsTauntState() ? componentSearchArea.IsTauntSourceInSearchArea : componentSearchArea.TargetEnemyList.Count > 0)
					return false;
				isRunForCatchUp = true;
				SetState(ActionState.WALK);
				return true;
			}

			bool noTarget = true;
			var targets = componentSearchArea.TargetPlayerList;
			for (int i = 0; i < targets.Count; i++)
			{
				var target = targets[i];
				if (!target.ComponentState.IsDead && target != unitController && !target.ComponentAbnormal.IsStealth && !target.ComponentAbnormal.IsRevivaling)
					noTarget &= target.ComponentAbnormal.IsSpyOrEtherialization(unitController);
			}

			var list = unitController.IsOther ? battleManager.EnemyList : battleManager.UnitList;
			int count = 0;
			for (int i = 0; i < list.Count; i++)
			{
				var target = list[i];
				if (target == unitController)
					count++;
				else if (!target.ComponentState.IsDead && !target.ComponentAbnormal.IsStealth && !target.ComponentAbnormal.IsRevivaling && !target.ComponentAbnormal.IsAbnormalState(eAbnormalState.SPY))
					count++;
			}

			if (list.Count > 0)
				noTarget &= count != 1;
			if (noTarget)
			{
				SetState(ActionState.WALK);
				return true;
			}

			return false;
		}

		[global::OpenPCR.Migration("c05f5cb5-209b-4f95-9599-df1282d3ed0f")]
		public void PlayDamageWhenIdle(bool _damageMotionWhenUnionBurst = false, bool _pauseStopState = false)
		{
			if (this.battleManager == null || ((this.CurrentState != ActionState.IDLE || IsModeChangeEnd) && !this.battleManager.BlackoutUnitTargetList.Contains(unitController)) || this.battleManager.BlackOutUnitList.Contains(unitController) || componentAbnormal.IsAbnormalState(eAbnormalState.STONE) || componentAbnormal.IsAbnormalState(eAbnormalState.PAUSE_ACTION))
			{
				return;
			}

			BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
			TrackEntry current = currentSpineCtrl.state.GetCurrent(0);
			if (!this.isIdleDamage)
			{
				if (current != null)
				{
					this.resumeTime = current.AnimationTime;
				}

				this.animeLoop = currentSpineCtrl.loop;
				this.resumeIsStopState = currentSpineCtrl.IsStopState;
				this.animeName = currentSpineCtrl.AnimationName;
				this.isIdleDamage = true;
			}

			currentSpineCtrl.loop = false;
			string text = "";
			text = ((!unitController.IsPartsBoss || unitController.PartsMotionPrefix == 0) ? currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix) : currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMAGE_MULTI_TARGET, -1, unitController.PartsMotionPrefix));
			if (this.battleManager.BlackoutUnitTargetList.Contains(unitController))
			{
				if (_damageMotionWhenUnionBurst)
				{
					if (currentSpineCtrl.AnimationName != text)
					{
						currentSpineCtrl.AnimationName = text;
						currentSpineCtrl.SetTimeScale(1f);
						current = currentSpineCtrl.state.GetCurrent(0);
						current.lastTime = this.battleManager.DeltaTime;
						current.time = this.battleManager.DeltaTime;
						current.nextAnimationLast = this.battleManager.DeltaTime;
						if (_pauseStopState)
						{
							current.lastTime = currentSpineCtrl.StopStateTime;
							current.time = currentSpineCtrl.StopStateTime;
							currentSpineCtrl.SetTimeScale(0f);
							unitController.AppendCoroutine(this.updateDamageWhenIdle(_damageMotionWhenUnionBurst), ePauseType.IGNORE_BLACK_OUT);
							return;
						}
					}

					if (_pauseStopState)
					{
						return;
					}
				}
				else
				{
					currentSpineCtrl.loop = true;
					if (unitController.IsPartsBoss && unitController.PartsMotionPrefix != 0)
					{
						currentSpineCtrl.AnimationName = currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE_MULTI_TARGET, -1, unitController.PartsMotionPrefix);
					}
					else
					{
						currentSpineCtrl.AnimationName = currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix);
					}

					currentSpineCtrl.SetTimeScale(1f);
					current = currentSpineCtrl.state.GetCurrent(0);
					current.lastTime = 0f;
					current.time = 0f;
					if (!this.battleManager.IsSkipping())
					{
						currentSpineCtrl.state.Apply(currentSpineCtrl.skeleton);
					}
				}
			}
			else
			{
				currentSpineCtrl.AnimationName = text;
				currentSpineCtrl.SetTimeScale(1f);
			}

			unitController.AppendCoroutine(this.updateDamageWhenIdle(_damageMotionWhenUnionBurst), ePauseType.IGNORE_BLACK_OUT);
		}

		[global::OpenPCR.Migration("f4d65301-4b71-4052-a4ba-2a69fae4aed8")]
		private IEnumerator updateDamageWhenIdle(bool _damageMotionWhenUnionBurst)
		{
			bool blackoutUnitContained = this.battleManager.BlackoutUnitTargetList.Contains(unitController);
			BattleAnimKitController currentSpineCtrl;
			while (true)
			{
				currentSpineCtrl = unitController.GetCurrentSpineCtrl();
				if (blackoutUnitContained && !this.battleManager.BlackoutUnitTargetList.Contains(unitController) && (long)componentParameter.Hp != 0L)
				{
					this.resumeSpineWithTime();
					if (componentAbnormal.IsContinueIdleForPauseAction)
					{
						currentSpineCtrl.IsStopState = true;
						currentSpineCtrl.Resume();
						currentSpineCtrl.PlayAnime(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix);
						currentSpineCtrl.Pause();
					}
					else if (this.isResumeSpecialSleepAnime())
					{
						currentSpineCtrl.PlayAnime(eAnimKitCharacterAnimeId.SLEEP, unitController.MotionPrefix, 1, 0);
						currentSpineCtrl.Resume();
						componentAbnormal.SpecialSleepStatus = eSpecialSleepStatus.LOOP;
					}
					else if (currentSpineCtrl.HasSpecialSleepAnimatilon(unitController.MotionPrefix) && componentAbnormal.IsAbnormalState(eAbnormalState.STONE) && componentAbnormal.IsAbnormalState(eAbnormalState.SLEEP))
					{
						componentAbnormal.IsPlayDamageAnimForAbnormal = false;
						currentSpineCtrl.AnimationName = currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix);
						TrackEntry current = currentSpineCtrl.state.GetCurrent(0);
						if (current != null)
						{
							current.lastTime = currentSpineCtrl.StopStateTime;
							current.time = currentSpineCtrl.StopStateTime;
						}

						currentSpineCtrl.SetTimeScale(0f);
						currentSpineCtrl.IsStopState = this.resumeIsStopState;
					}

					this.isIdleDamage = false;
					yield break;
				}

				if (this.CurrentState != ActionState.IDLE && !this.battleManager.BlackoutUnitTargetList.Contains(unitController))
				{
					this.isIdleDamage = false;
					yield break;
				}

				if (currentSpineCtrl.AnimationName == currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.JOY_LONG, unitController.MotionPrefix))
				{
					yield break;
				}

				if (this.CurrentState == ActionState.IDLE && !currentSpineCtrl.IsPlayAnime)
				{
					currentSpineCtrl.loop = true;
					if (unitController.IsPartsBoss && unitController.PartsMotionPrefix != 0)
					{
						currentSpineCtrl.AnimationName = currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE_MULTI_TARGET, -1, unitController.PartsMotionPrefix);
					}
					else
					{
						currentSpineCtrl.AnimationName = currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix);
					}

					this.isIdleDamage = false;
					yield break;
				}

				if ((!currentSpineCtrl.IsPlayAnime & _damageMotionWhenUnionBurst) && (long)componentParameter.Hp != 0L && currentSpineCtrl.AnimationName == currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix))
				{
					break;
				}

				yield return null;
			}

			currentSpineCtrl.loop = true;
			if (unitController.IsPartsBoss && unitController.PartsMotionPrefix != 0)
			{
				currentSpineCtrl.AnimationName = currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE_MULTI_TARGET, -1, unitController.PartsMotionPrefix);
			}
			else
			{
				currentSpineCtrl.AnimationName = currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix);
			}

			if (this.battleManager.IsSkipping())
			{
				this.isIdleDamage = false;
			}
		}

		[global::OpenPCR.Migration("8cf2d186-9218-4404-beb3-29516f43e538")]
		private bool isResumeSpecialSleepAnime()
		{
			if (componentAbnormal.IsAbnormalState(eAbnormalState.STONE))
			{
				return false;
			}

			if (!componentAbnormal.IsSpecialSleepStatus)
			{
				return false;
			}

			if (!unitController.GetCurrentSpineCtrl().HasSpecialSleepAnimatilon(unitController.MotionPrefix))
			{
				return false;
			}

			return true;
		}

		[global::OpenPCR.Migration("f9069ca3-4b05-4350-8c0e-db126f2770b4")]
		private bool isResumeDamageAnimationFromStopStateTime()
		{
			if (!this.resumeIsStopState && !componentAbnormal.IsDamageReleaseSpecialSleepAnimForUnionBurst)
			{
				return false;
			}

			if (!componentAbnormal.IsUnableActionState())
			{
				return false;
			}

			if (componentAbnormal.IsContinueIdleForPauseAction)
			{
				return false;
			}

			if (this.isResumeSpecialSleepAnime() && !componentAbnormal.IsDamageReleaseSpecialSleepAnimForUnionBurst)
			{
				return false;
			}

			return true;
		}

		[global::OpenPCR.Migration("3a1263c9-417c-4a1f-9aaf-322e9e0680c2")]
		private void resumeSpineWithTime()
		{
			BattleAnimKitController currentSpineCtrl = unitController.GetCurrentSpineCtrl();
			if (!(currentSpineCtrl.AnimationName == currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix)) && !(currentSpineCtrl.AnimationName == currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix)) && !(currentSpineCtrl.AnimationName == currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMAGE_MULTI_TARGET, -1, unitController.PartsMotionPrefix)) && !(currentSpineCtrl.AnimationName == currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE_MULTI_TARGET, -1, unitController.PartsMotionPrefix)) && !currentSpineCtrl.AnimationName.IsNullOrEmpty())
			{
				return;
			}

			if (this.isResumeDamageAnimationFromStopStateTime())
			{
				if (componentAbnormal.IsDamageReleaseSpecialSleepAnimForUnionBurst)
				{
					currentSpineCtrl.PlayAnime(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix, _playLoop: false);
				}
				else
				{
					currentSpineCtrl.AnimationName = currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMEGE, unitController.MotionPrefix);
				}

				TrackEntry current = currentSpineCtrl.state.GetCurrent(0);
				if (current != null)
				{
					current.lastTime = currentSpineCtrl.StopStateTime;
					current.time = currentSpineCtrl.StopStateTime;
				}

				currentSpineCtrl.SetTimeScale(0f);
				currentSpineCtrl.IsStopState = true;
				componentAbnormal.IsDamageReleaseSpecialSleepAnimForUnionBurst = false;
				componentAbnormal.IsPlayDamageAnimForAbnormal = false;
				componentAbnormal.SpecialSleepStatus = eSpecialSleepStatus.INVALID;
				return;
			}

			bool flag = this.animeName == currentSpineCtrl.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.IDLE, unitController.MotionPrefix);
			if ((this.CurrentState != ActionState.IDLE && this.CurrentState != ActionState.DAMAGE) || (flag && !componentAbnormal.IsUnableActionState()))
			{
				currentSpineCtrl.loop = this.animeLoop;
				currentSpineCtrl.AnimationName = this.animeName;
				TrackEntry current2 = currentSpineCtrl.state.GetCurrent(0);
				if (current2 != null)
				{
					current2.lastTime = this.resumeTime;
					current2.time = this.resumeTime;
				}

				currentSpineCtrl.SetTimeScale(1f);
			}

			currentSpineCtrl.IsStopState = this.resumeIsStopState;
		}

		[global::OpenPCR.Migration("c4799933-27d2-4453-a432-1e2d8cd26485")]
		public IEnumerator SetStateWithDelayForRevival(float delay, ActionState state, int nextSkillId = 0, int skillId = 0)
		{
			float time = 0f;
			while (true)
			{
				time += unitController.DeltaTimeForPause;
				if (!(time <= delay) || battleManager.GameState != eBattleGameState.PLAY)
					break;
				yield return null;
			}

			componentColor.SetNormalColor();
			SetState(state, nextSkillId, skillId);
		}

		[global::OpenPCR.Migration("de79ead6-ad68-4969-b442-a17d8d0135c4")]
		public IEnumerator CreatePrefabWithTime(List<PrefabWithTime> _data, bool _isIdle = false, int _summonIndex = -1, bool _isAura = false, bool _isShieldEffect = false, bool _ignorePause = false, int _damagedTriggerActionId = -1, bool _useTimeDelta = false)
		{
			_data = _data.FindAll((PrefabWithTime e) => e.EffectDifficlty == PrefabWithTime.eEffectDifficulty.ALL || unitController.EffectDifficulty == e.EffectDifficlty);
			if (_isShieldEffect)
			{
				_data = _data.FindAll((PrefabWithTime e) => e.Prefab.GetComponent<SpBattleShieldEffect>() != null);
			}

			float time = 0f;
			bool[] effectCreated = new bool[_data.Count];
			while (true)
			{
				time = ((!_useTimeDelta) ? (time + (_ignorePause ? battleManager.DeltaTime : unitController.DeltaTimeForPause)) : (time + Time.deltaTime));
				int num = 0;
				for (int count = _data.Count; num < count; num++)
				{
					if (effectCreated[num])
					{
						if (num == count - 1)
						{
							yield break;
						}

						continue;
					}

					float time2 = _data[num].Time;
					if ((_summonIndex != -1 && _data[num].SummonSkillIndex != _summonIndex) || (!_isShieldEffect && _data[num].Prefab.GetComponent<SpBattleShieldEffect>() != null))
					{
						effectCreated[num] = true;
						continue;
					}

					GameObject mDOJNMEMHLN = (unitController.IsLeftDir ? _data[num].PrefabLeft : _data[num].Prefab);
					if (!(time > time2))
					{
						continue;
					}

					if (IsDead & _isIdle)
					{
						break;
					}

					SkillEffectCtrl effect = unitController.GetEffect(mDOJNMEMHLN, unitController);
					effect.InitializeSort();
					effect.PlaySe(unitController.ComponentSound.SoundUnitId, unitController.IsLeftDir);
					effect.transform.position = unitController.transform.position;
					effect.transform.parent = ExceptUIRoot.Transform;
					effect.SortTarget = unitController;
					effect.ExecAppendCoroutine(_isAura ? unitController : null, _isAbnormal: true);
					effect.StartUpdateDisappear(_isUnionBurst: false, unitController.ComponentMultiPartsBoss.DummyPartsData);
					effect.SetTimeToDieByStartHp(unitController.StartHpPercent);
					if (effect is SpBattleShieldEffect)
					{
						if (battleManager.CurrentEnemyPoint >= battleManager.GetPurposeCount())
						{
							effect.SetActive(_isActive: false);
						}
						else
						{
							battleManager.SpBattleShieldEffect = effect as SpBattleShieldEffect;
							battleManager.SpBattleShieldEffect.SetEnemyPoint(battleManager.CurrentEnemyPoint, unitController, _init: true, battleManager.GetPurposeCount());
						}
					}

					if (_data[num].TargetBoneName.IsNullOrEmpty())
					{
						battleManager.StartCoroutine(effect.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero));
					}
					else
					{
						battleManager.StartCoroutine(effect.TrackTarget(unitController.ComponentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero, _followX: true, _followY: true, unitController.GetCurrentSpineCtrl().skeleton.FindBone(_data[num].TargetBoneName), _data[num].IsTrackRotation));
					}

					if (_summonIndex != -1)
					{
						battleManager.StartCoroutine(effect.TrackTargetSortForSummon(unitController));
					}

					if (_isAura | _isShieldEffect)
					{
						battleManager.StartCoroutine(effect.TrackTargetSort(unitController));
						unitController.RepeatEffectList.Add(effect);
						unitController.AuraEffectList.Add(effect);
						effect.IsAura = true;
					}

					if (_damagedTriggerActionId != -1)
					{
						if (unitController.DamagedHpEffectDictionary.ContainsKey(_damagedTriggerActionId))
						{
							unitController.DamagedHpEffectDictionary[_damagedTriggerActionId].Add((effect, _data[num].TargetBoneName));
						}
						else
						{
							unitController.DamagedHpEffectDictionary.Add(_damagedTriggerActionId, new List<(SkillEffectCtrl, string)> { (effect, _data[num].TargetBoneName) });
						}
					}

					effectCreated[num] = true;
					if (_isIdle)
					{
						idleEffectsObjs.Add(effect);
					}
				}

				yield return null;
			}
		}

		[global::OpenPCR.Migration("a665cf59-21f2-4dd1-910f-257ffdb5590f")]
		public void DeleteDamagedHpAuraEffect(int _actionId)
		{
			if (unitController.DamagedHpEffectDictionary.TryGetValue(_actionId, out List<(SkillEffectCtrl, string)> value))
			{
				for (int num = value.Count - 1; num >= 0; num--)
				{
					value[num].Item1.SetTimeToDie(value: true);
					unitController.AuraEffectList.Remove(value[num].Item1);
					value.RemoveAt(num);
				}
			}
		}

		[global::OpenPCR.Migration("7bf8d0f7-f790-4db7-b655-8ed8cd5f502f")]
		public void CreateRunSmoke()
		{
			if (componentParameter.MoveSpeed != 0f && !unitController.IsMoveSpeedForceZero && !this.IsDead && !(this.runSmokeEffect != null) && UnitUtility.IsPersonUnit(componentSound.SoundUnitId) && this.battleManager.CanCreateBattleEffect())
			{
				SkillEffectCtrl effect = unitController.GetEffect(unitController.IsOther ? Singleton<BattleResourceManager>.Instance.RunSmokeLPrefab : Singleton<BattleResourceManager>.Instance.RunSmokeRPrefab);
				effect.InitializeSort();
				effect.PlaySe(componentSound.SoundUnitId, unitController.IsLeftDir);
				effect.transform.position = unitController.transform.position;
				effect.transform.parent = ExceptUIRoot.Transform;
				effect.SortTarget = unitController;
				effect.SetSortOrderBack();
				this.battleManager.StartCoroutine(effect.TrackTarget(componentMultiPartsBoss.GetFirstParts(_owner: true), Vector3.zero));
				effect.ExecAppendCoroutine();
				this.runSmokeEffect = effect;
			}
		}

		[global::OpenPCR.Migration("1ae85367-f500-4cd5-bc90-d665ed9df4fe")]
		private IEnumerator startShakeWithDelay(ShakeEffect shake, bool _ignorePause = false)
		{
			float time = 0f;
			while (true)
			{
				time += _ignorePause ? battleManager.DeltaTime : unitController.DeltaTimeForPause;
				if (time > shake.StartTime)
					break;
				yield return null;
			}

			unitController.StartShake(shake, null, unitController);
		}

		[global::OpenPCR.Migration("ba3330fd-c367-4685-8c14-fdb6eca1234d")]
		public bool IsCancelActionState(bool _isUnionBurst, int _skillId = -1)
		{
			bool result = false;
			if ((this.CancelByAwake && this.CurrentTriggerSkillId != _skillId) || this.CancelByConvert || this.CancelByToad)
			{
				result = true;
			}

			switch (this.CurrentState)
			{
				case ActionState.DAMAGE:
				case ActionState.DIE:
				case ActionState.LOSE:
					result = true;
					break;
				case ActionState.UNION_BURST:
					result = !_isUnionBurst;
					break;
			}

			return result;
		}
		private bool isIdleDamage;
		private float resumeTime;
		private bool resumeIsStopState;
		private bool animeLoop;
		private string animeName = "";
		private global::Elements.Battle.IBattleLog battleLog { get; set; }
		private global::Elements.SoundManager soundManager { get; set; }
		private global::Elements.Battle.Core.UnitComponentAbnormal componentAbnormal { get; set; }
		private global::Elements.Battle.Core.UnitComponentAnimation componentAnimation { get; set; }
		private global::Elements.Battle.Core.UnitComponentActionPattern componentAttackPattern { get; set; }
		private global::Elements.Battle.Core.UnitComponentColor componentColor { get; set; }
		private global::Elements.Battle.Core.UnitComponentMultiPartsBoss componentMultiPartsBoss { get; set; }
		private global::Elements.Battle.Core.UnitComponentDamageControl componentDamageControl { get; set; }
		private global::Elements.Battle.Core.UnitComponentParameter componentParameter { get; set; }
		private global::Elements.Battle.Core.UnitComponentPassiveSkill componentPassiveSkill { get; set; }
		private global::Elements.UnitComponentRelicSkill componentRelicSkill { get; set; }
		private global::Elements.Battle.Core.UnitComponentSearchArea componentSearchArea { get; set; }
		private global::Elements.Battle.Core.UnitComponentSortOrder componentSortOrder { get; set; }
		private global::Elements.Battle.Core.UnitComponentSound componentSound { get; set; }
		private global::Elements.Battle.Core.UnitComponentUnionBurst componentUnionBurst { get; set; }
		private global::Elements.Battle.Core.PresetBattleEffectData battleEffectData { get; set; }
		private bool stopUpdateDamageFlag { get; set; }
		public global::Elements.Battle.Core.ActionState CurrentState { get; set; }
		public bool StartStateIsWalk { get; set; }
		public bool StandByDone { get; set; }
		public bool WaveStartIdle { get; set; }
		public int UnDeadTimeHitCount { get; set; }
		public bool HasEndUnionBurstAnimeForIfAction { get; set; }
		public bool IsSubUnionBurstMode { get; set; }
		public bool IsStartVoicePlay { get; set; }
		public bool IsModeChangeEnd { get; set; }
		public bool ToadRelease { get; set; }
		public bool ToadReleaseDamage { get; set; }
		public bool DieInToad { get; set; }
		public bool IdleStartAfterWaitFrame { get; set; }
		public int CurrentTriggerSkillId { get; set; }
		public global::Elements.Battle.Core.UnitComponentState.OnDeadDelegate OnDeadForRevival { get; set; }
		public bool IsStartActionForExtraEffect { get; set; }
		private int dieCoroutineId { get; set; }
		public bool IsOnBreakAllCallWhenGameStartDone { get; set; }
		private int damageCoroutineId { get; set; }
		private int walkCoroutineId { get; set; }
		public bool EnemyPointDone { get; set; }
		public bool WasDefeatCount { get; set; }
		public bool RevivalFlagForEnvironment { get; set; }
		public long SpecialBattlePurposeHp { get; set; }
		public global::System.Action OnMotionPrefixChanged { get; set; }
		public bool TriggerStealthUnableStateBarrier { get; set; }
		public bool MultiTargetByTime { get; set; }
		public int SpecialIdleMotionId { get; set; }
		private bool isRunForCatchUp { get; set; }
		public bool IsForceStartStay { get; set; }
		public global::Elements.CutinVoiceStatus CutinVoiceStatus { get; set; }
		public bool HasSetStateCalled { get; private set; }
		public bool IsNotPartsBossReady { get; set; }
		private global::System.Collections.Generic.Dictionary<int, global::Elements.Battle.Core.VoiceTypeAndSkillNumber> voiceTypeDictionary { get; set; }

		[global::OpenPCR.Migration("b9efb84e-d259-48a8-ba29-891593233cbd")]
		private bool shouldPlayMultiTarget()
		{
			if (Singleton<TempData>.Instance.BattleSkipState != eBattleSkipState.NOT_SKIPPING)
				return false;
			var save = ManagerSingleton<SaveDataManager>.Instance;
			return save.MultiTargetConfirm == 0 || (save.MultiTargetConfirm == 2 && !save.MultiTargetFirst);
		}

		[global::OpenPCR.Migration("7dca63d0-26d8-464d-8b7c-c315e5f473f7")]
		private void setStateUnionBurst()
		{
			var controller = unitController.UnitActionController;
			int unionBurstSkillId = unitController.UnionBurstSkillId;
			if (IsSubUnionBurstMode)
				playSubUbVoiceWithDelay(controller.SubUnionBurstList.Find(skill => skill.SkillId == unionBurstSkillId));
			else
			{
				playUbVoiceWithDelay();
				playUbNameVoiceWithDelay();
				if (unitController.UseUbVoice3and4)
				{
					playUbVoice3WithDelay();
					playUbVoice4WithDelay();
				}
			}
			foreach (var pair in componentAbnormal.UbTriggerSealActionDictionary.ToArray()) pair.Value.Call();
			CurrentState = ActionState.UNION_BURST;
			unitController.CurrentSkillId = unitController.UnionBurstSkillId;
			battleManager.ChainVoicePlayUnit = unitController;
			if (controller.GetIsSkillPrincessForm(unionBurstSkillId))
			{
				unitController.PrincessFormProcessor.StartPrincessFormSkill(controller.GetPrincessFormMovieData(unionBurstSkillId));
				battleManager.TriggerUnionBurstSeal(unitController.IsOther);
			}
			else if (controller.StartAction(unionBurstSkillId))
			{
				battleManager.TriggerUnionBurstSeal(unitController.IsOther);
				unitController.AppendCoroutine(updateUnionBurst(), ePauseType.SYSTEM, unitController);
			}
			else SetState(ActionState.IDLE);
		}

		[global::OpenPCR.Migration("04f2eebc-104b-4747-a3af-188c286972f6")]
		public IEnumerator PlayCutInVoiceWithDelay(VoiceDelayAndEnable _data, int _voiceId, bool _withCutin)
		{
			if (!unitController.GetIsMuteVoice())
				throw new global::OpenPCR.OutOfBoundaryException("Offline UB voice output requires explicitly muted voice; authored voice timing/cue outputs are outside this scope.");
			yield break;
		}

		[global::OpenPCR.Migration("19784366-7ff0-4048-aac6-c3df1be0d193")]
		public void StopUpdateDamage()
		{
			stopUpdateDamageFlag = true;
		}

		[global::OpenPCR.Migration("1881e7fe-d42c-4d40-b131-26353a199742")]
		public void TryPlaySkillVoice(int _skillId)
		{
			if (voiceTypeDictionary.ContainsKey(_skillId) && judgePlayVoice("skill", 2, true, 0.5f))
				componentSound.PlayVoice(voiceTypeDictionary[_skillId].VoiceType, voiceTypeDictionary[_skillId].SkillNumber * 100, false);
		}

		[global::OpenPCR.Migration("6449bfbe-c186-4eff-8b2b-78a63d4731f9")]
		private bool judgeWalkEnd(int _coroutineId)
		{
			if (walkCoroutineId != _coroutineId)
				return true;
			if (CurrentState == ActionState.UNION_BURST || CurrentState == ActionState.DAMAGE || CurrentState == ActionState.DIE)
				return true;
			if (!componentAbnormal.IsConfusionOrConvert())
			{
				if (componentAbnormal.IsTauntState() ? componentSearchArea.IsTauntSourceInSearchArea : componentSearchArea.TargetEnemyList.Count > 0)
				{
					changeStateIdleFromWalk();
					return true;
				}

				if (!IdleOnly && !WaveStartIdle)
					return false;
				changeStateIdleFromWalk();
				return true;
			}

			var list = unitController.IsOther ? battleManager.EnemyList : battleManager.UnitList;
			int count = 0;
			for (int i = 0; i < list.Count; i++)
			{
				var target = list[i];
				if (target == unitController)
					count++;
				else if (!target.ComponentState.IsDead && !target.ComponentAbnormal.IsStealth && !target.ComponentAbnormal.IsRevivaling && !target.ComponentAbnormal.IsSpyOrEtherialization(unitController))
					count++;
			}

			if (count == 1)
			{
				changeStateIdleFromWalk();
				return true;
			}

			if (componentSearchArea.TargetPlayerList.Count > 1 || IdleOnly || WaveStartIdle)
			{
				SetState(ActionState.IDLE);
				return true;
			}

			return false;
		}

		[global::OpenPCR.Migration("dfd58e6c-4b02-44e7-acf2-689d5cc59e01")]
		private void changeStateIdleFromWalk()
		{
			isRunForCatchUp = false;
			if (!unitController.IsBoss && !unitController.IsBonusEnemy && !IsStartActionForExtraEffect)
				IsStartActionForExtraEffect = true;
			SetState(ActionState.IDLE);
		}

		[global::OpenPCR.Migration("ecb98c4f-001c-484b-9acd-9fd30278a7a4")]
		public void AddVoiceTypeAndSkillNumber(int _key, VoiceTypeAndSkillNumber _data)
		{
			voiceTypeDictionary.Add(_key, _data);
		}
	}
}
