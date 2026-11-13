using global::OpenPCR.Foundation;
using global::OpenPCR.Foundation.Audio.Movie;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UnitComponentUnionBurst : UnitComponentBase
	{
		private Dictionary<eReduceEnergyType, bool> isReduceEnergyDictionary = new Dictionary<eReduceEnergyType, bool>(new eReduceEnergyType_DictComparer()) { { (eReduceEnergyType)0, false }, { (eReduceEnergyType)1, false }, { (eReduceEnergyType)2, false }, { (eReduceEnergyType)3, false } };
		private bool isUbDisabledByChangePattern;
		private float unionBurstCoolDownTime;
		public bool PlayCutInFlag { get; private set; }
		public bool PlayingNoCutinMotion { get; private set; }
		public int MovieId { get; set; }
		private IBattleTimeScaleForUnitCtrl battleTimeScale { get; set; }

		[global::OpenPCR.Migration("71751b36-61ff-4a44-9b4f-34c9dcd7f1a3")]
		public UnitComponentUnionBurst(UnitCtrl _unitController, BattleManager _battleManager, IBattleTimeScaleForUnitCtrl _battleTimeScale, ePlaybackType _cutInPlaybackType) : base(_unitController)
		{
			dialogManager = ManagerSingleton<DialogManager>.Instance;
			soundManager = ManagerSingleton<SoundManager>.Instance;
			movieManager = ManagerSingleton<MovieManager>.Instance;
			battleManager = _battleManager;
			battleTimeScale = _battleTimeScale;
			cutInPlaybackType = _cutInPlaybackType;
		}

		[global::OpenPCR.Migration("3f11cce4-4e8b-497b-b2c5-ac5e5951191d")]
		public void IncrementUsedCount()
		{
			UsedCount++;
		}

		[global::OpenPCR.Migration("eaf2c818-b4d9-4b4e-a9c1-4450d5966746")]
		public bool ExistsReduceEnergyAction()
		{
			foreach (var item in isReduceEnergyDictionary)
			{
				if (item.Value)
					return true;
			}

			return false;
		}

		[global::OpenPCR.Migration("58d04b0c-3f3a-4ffd-a10d-2651ea3dd00e")]
		public void SetReduceEnergyAction(eReduceEnergyType _type, bool _isEnable)
		{
			isReduceEnergyDictionary[_type] = _isEnable;
		}

		[global::OpenPCR.Migration("ca24f140-301e-4a5a-b85a-75b9e30c8d1d")]
		public eConsumeResult ConsumeEnergy()
		{
			var actionController = unitController.UnitActionController;
			if (!BattleUtil.LessThanOrApproximately(1000.0, componentParameter.Energy))
				return eConsumeResult.FAILED;
			double remainingEnergy = (double)(long)componentParameterClamped.EnergyReduceRateZero * 1000.0 / 100.0;
			if (!actionController.UnionBurstIsChargeTime)
			{
				componentParameter.SetEnergy(remainingEnergy, eSetEnergyType.BY_USE_SKILL, null);
				return eConsumeResult.SKILL_OK;
			}

			if (actionController.DisableUBByModeChange)
				return eConsumeResult.FAILED;
			if (actionController.UnionBurstCharging)
			{
				componentParameter.SetEnergy(remainingEnergy, eSetEnergyType.BY_USE_SKILL, null);
				actionController.UnionBurstCharging = false;
				return eConsumeResult.SKILL_RELEASE;
			}

			componentParameter.SetEnergy((double)(UnitDefine.MAX_ENERGY - 1), eSetEnergyType.BY_USE_SKILL, null);
			return eConsumeResult.SKILL_CHARGE;
		}

		[global::OpenPCR.Migration("8d0b95d9-70de-47ff-b67a-d4c5a45c8df2")]
		public void StartCutIn()
		{
			UnitActionController unitActionController = unitController.UnitActionController;
			addUnionBurstTimelineData(battleManager.GetRoundUpTimeLimit(), false);
			IncrementCounter();
			unitController.ResetBattleCameraShakeEffect();
			unitController.ResetThanksTargetUnit();
			battleManager.FinishBlackFadeOut(null, _skipBackgroundAnimeReset: false);
			unitController.ComponentSortOrder.SetSortOrderFront();
			battleManager.SkillReadyUnit = unitController;
			if (battleManager.CanPlaySe())
			{
				// Render-only button audio.
			}

			SetPlayCutInFlag(needsCutIn());
			MovieManager instance = movieManager;
			bool isSkillPrincessForm = unitActionController.GetIsSkillPrincessForm(unitController.UnionBurstSkillId);
			// Movie mirroring changes rendered playback orientation only; the explicit host NONE branch has no movie resource.
			battleManager.FrameCountPause(unitController);
			if (isSkillPrincessForm)
			{
				SingletonMonoBehaviour<BattleHeaderController>.Instance.PauseNoMoreTimeUp(_pause: true);
				unitController.PreparePrincessFormProcessor();
			}

			if (PlayCutInFlag && MovieId != 0)
			{
				SaveDataManager instance2 = ManagerSingleton<SaveDataManager>.Instance;
				if (instance2.CutInPlaybackType == ePlaybackType.ONCE_A_DAY && unitController.IsPlayerUnit)
				{
					instance2.AddCutInPlayedUnitId(unitController.UnitId);
				}

				battleManager.IsPlayCutin = true;
				int charaId = UnitUtility.GetCharaId(MovieId);
				unitController.ComponentState.CutinVoiceStatus = BattleVoiceUtility.SelectCutinVoice(unitController, battleManager.GetMyUnitList(), 1, battleTimeScale.SpeedupFlag);
				battleManager.StartCoroutine(unitController.ComponentState.PlayCutInVoiceWithDelay(battleTimeScale.SpeedupFlag ? unitController.SpeedUpCutInVoiceDelayWithCutIn[0] : unitController.NormalCutInVoiceDelayWithCutIn[0], 1, _withCutin: true));
				if (battleManager.CanPlaySe())
				{
					// Render-only movie audio.
				}

				battleManager.PauseAllSe();
				SingletonMonoBehaviour<UnitUiCtrl>.Instance.HidePopUp();
				battleManager.HideDamageNumBySkillScreen();
				bool changeFPS = battleManager.FrameRate == 30 && battleTimeScale.SpeedupFlag;
				if (dialogManager.IsUse(eDialogId.FLATOUT_PLAYER))
				{
					dialogManager.ForceCloseOne(eDialogId.FLATOUT_PLAYER);
				}

				ViewManager instance3 = ManagerSingleton<ViewManager>.Instance;
				float num = 0.75f;
				instance.SetScreenScale(eMovieType.CUT_IN, MovieId, new Vector2(instance3.ActiveScreenWidthInUIRoot, instance3.ActiveScreenWidthInUIRoot * num), _isPanScan: true, 0L);
				instance.SetMoviePlayerLocalPosition(eMovieType.CUT_IN, MovieId, WrapperEngineApiScreen.GetRestoreNGUIOffsetYVector3(), 0L);
				bool alreadyCalled = false;
				float fadeStartTime = (isSkillPrincessForm ? 0f : unitActionController.UnionBurstList[0].CutInMovieFadeStartTime);
				float fadeDurationTime = (isSkillPrincessForm ? 0f : unitActionController.UnionBurstList[0].CutInMovieFadeDurationTime);
				Action action = delegate
				{
					if (!alreadyCalled)
					{
						alreadyCalled = true;
						if (!(battleManager == null))
						{
							unitActionController.AddBlackoutTarget(unitController.UnionBurstSkillId);
							MoviePlayed = true;
							if (changeFPS)
							{
								OverrideApplication.targetFrameRate = 30;
							}

							battleManager.OnCutInEnd(unitController.IsOther || unitController.IsSummonOrPhantom, unitController);
						}
					}
				};
				instance.Play(eMovieType.CUT_IN, MovieId, eMoviePlayType.NORMAL, isSkillPrincessForm ? null : action, PlayEndType.PrepareForRendering, fadeStartTime, fadeDurationTime, 0L);
				instance.SetPlaySpeed(eMovieType.CUT_IN, MovieId, battleTimeScale.SpeedupFlag ? battleTimeScale.SpeedUpRate : 1f, 0L);
				if (changeFPS)
				{
					OverrideApplication.targetFrameRate = (int)(battleTimeScale.SpeedUpRate * 30f);
				}

				if (isSkillPrincessForm)
				{
					battleManager.StartCoroutine(waitMovieEndFrame(action, MovieId));
				}
			}
			else
			{
				unitController.ComponentState.CutinVoiceStatus = BattleVoiceUtility.SelectCutinVoice(unitController, battleManager.GetMyUnitList(), 2, battleTimeScale.SpeedupFlag);
				battleManager.StartCoroutine(unitController.ComponentState.PlayCutInVoiceWithDelay(battleTimeScale.SpeedupFlag ? unitController.SpeedUpCutInVoiceDelay[0] : unitController.NormalCutInVoiceDelay[0], 2, _withCutin: false));
				PlayingNoCutinMotion = true;
				battleManager.IsPlayingNoCutInMotion = true;
				battleManager.IsNoCutInMotionRightAfter = true;
				battleManager.IsPlayingNoCutInMotionForPSkill = isSkillPrincessForm;
				SingletonMonoBehaviour<BattleHeaderController>.Instance.PauseNoMoreTimeUp(_pause: true);
				unitController.StartCoroutine(unitActionController.StartActionWithOutCutIn(unitController, unitController.UnionBurstSkillId, delegate
				{
					PlayingNoCutinMotion = false;
					battleManager.IsPlayingNoCutInMotion = false;
					battleManager.IsNoCutInMotionRightAfter = false;
					battleManager.IsPlayingNoCutInMotionForPSkill = false;
					battleManager.OnCutInEnd(unitController.IsOther || unitController.IsSummonOrPhantom, unitController);
				}));
			}
		}

		[global::OpenPCR.Migration("b52f3233-ec98-4f14-8222-9b9a12cc4da1")]
		private bool needsCutIn()
		{
			if (!battleManager.CanCreateBattleEffect()) return false;
			if (unitController.UnitActionController.GetForceCutinOff(unitController.UnionBurstSkillId)) return false;
			if (!unitController.CanBattleManagerPlayCutIn()) return false;
			if (!unitController.IsPlayerUnit) return true;
			if ((int)cutInPlaybackType == 1) return false;
			if ((int)cutInPlaybackType != 2) return true;
			if (ManagerSingleton<SaveDataManager>.Instance.IsCutInPlayedChara(unitController.UnitId)) return false;
			return MovieId != 0;
		}

		[global::OpenPCR.Migration("a2ef1ffb-46ea-423f-a24f-a7aaa4056e8d")]
		public IEnumerator waitMovieEndFrame(Action _callback, int _movieId)
		{
			float oneFrame = 1f / 30f;
			float duration;
			while ((duration = movieManager.GetDurationTime(eMovieType.CUT_IN, _movieId, 0)) == 0f) yield return null;
			float previous = -1f;
			while (true)
			{
				float position = movieManager.GetSeekPositionTime(eMovieType.CUT_IN, _movieId, 0, false);
				if (position < previous || (position + oneFrame) + oneFrame * 0.5f >= duration) break;
				previous = position;
				yield return null;
			}
			global::OpenPCR.Foundation.EventExtension.Call(_callback);
		}

		[global::OpenPCR.Migration("ec845cad-0b46-4c55-85cc-3b5f6b7e33b0")]
		public bool IsAutoOrUbExecTrying()
		{
			if (IsUbExecTrying)
			{
				if (JudgeSkillReadyAndIsMyTurn())
				{
					bool isAutoIdle = battleManager.UnitUiCtrl.IsAutoMode && componentState.CurrentState == ActionState.IDLE;
					if (!battleManager.UnitUiCtrl.IsAllSetMode && !isAutoIdle)
						battleManager.DisableAutoClear();
				}

				if (IsUbExecTrying)
					return true;
			}

			if (componentState.CurrentState != ActionState.IDLE)
				return false;
			if (unitController.IsSummonOrPhantom)
			{
				if (unitController.SummonSource == null || (long)unitController.SummonSource.ComponentParameter.Hp < 1L)
					return false;
			}

			return battleManager.UnitUiCtrl.IsAutoMode || unitController.IsSummonOrPhantom;
		}

		[global::OpenPCR.Migration("d9130b5b-a076-46fc-b766-6d969e6735b5")]
		public bool JudgeSkillReadyAndIsMyTurn()
		{
			return isMyTurn() && CheckIsSkillReady();
		}

		[global::OpenPCR.Migration("fab1f77f-ceee-4cb6-815a-25644aaf6cf5")]
		private bool isMyTurn()
		{
			var actionController = unitController.UnitActionController;
			return battleManager.ChargeSkillTurn == eChargeSkillTurn.NONE || actionController.UnionBurstCharging;
		}

		public int ThanksTargetUnitId { get; set; }
		public int Counter { get; private set; }
		public bool IsCutInSkip { get; private set; }
		public bool IsUbExecTrying { get; private set; }
		public bool MoviePlayed { get; set; }
		public int UsedCount { get; set; }
		private ePlaybackType cutInPlaybackType { get; set; }
		private DialogManager dialogManager { get; set; }
		private SoundManager soundManager { get; set; }
		private MovieManager movieManager { get; set; }
		private BattleManager battleManager { get; set; }
		private UnitComponentAbnormal componentAbnormal { get; set; }
		private UnitComponentState componentState { get; set; }
		private UnitComponentSortOrder componentSortOrder { get; set; }
		private UnitComponentParameter componentParameter { get; set; }
		private UnitComponentParameterClamped componentParameterClamped { get; set; }
		private UnitComponentSearchArea componentSearchArea { get; set; }

		[global::OpenPCR.Migration("9475ad12-3fd2-436f-b317-0933c13ab60a")]
		public void SetIsUbExecTrying(bool _isUbExecTrying)
		{
			IsUbExecTrying = _isUbExecTrying;
		}

		[global::OpenPCR.Migration("fa3d3166-3a7f-4a3a-8283-d9de82e3eea2")]
		public void SetCoolDownTime(float _coolDownTime)
		{
			unionBurstCoolDownTime = _coolDownTime;
		}

		[global::OpenPCR.Migration("86461d2b-463a-4970-9fb5-d2d73753e318")]
		public void SetIsUbDisabledByChangePattern(bool _isDisable)
		{
			isUbDisabledByChangePattern = _isDisable;
		}

		[global::OpenPCR.Migration("f83cfd34-6259-41be-b2dd-99fa0c581343")]
		public void IncrementCounter()
		{
			Counter++;
		}

		[global::OpenPCR.Migration("fc122868-82aa-47b1-a5db-451b49d243fd")]
		public void SetIsCutInSkip(bool _isSkip)
		{
			IsCutInSkip = _isSkip;
		}

		[global::OpenPCR.Migration("6c33a003-2851-42c2-97b4-3579d8776b06")]
		public void SetPlayCutInFlag(bool _isEnable)
		{
			PlayCutInFlag = _isEnable;
		}

		[global::OpenPCR.Migration("5d7a94eb-8d45-41f3-a625-b26ae20aaafe")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAbnormal = _container.ComponentAbnormal;
			componentState = _container.ComponentState;
			componentSortOrder = _container.ComponentSortOrder;
			componentParameter = _container.ComponentParameter;
			componentParameterClamped = _container.ComponentParameterClamped;
			componentSearchArea = _container.ComponentSearchArea;
		}

		[global::OpenPCR.Migration("9cb8215b-0de4-4871-8189-994e9e9370a9")]
		public bool CheckIsSkillReady()
		{
			var actionController = unitController.UnitActionController;
			uint category = (uint)battleManager.BattleCategory;
			if (category <= 60u && ((1UL << (int)category) & 0x18000002406EC004UL) != 0UL && !componentState.StandByDone)
				return false;
			if (unionBurstCoolDownTime > 0f || componentAbnormal.IsUnbeatableForAutoRevival)
				return false;
			if (battleManager.IsSpecialBattle && battleManager.ActionStartTimeCounter > 0f)
				return false;
			if (battleManager.IsGuestCutinPlaying || battleManager.IsTalentBonusEffectPlaying)
				return false;
			if (battleManager.CountIsNotPartsBossReady > 0)
				return false;
			if (componentSearchArea.SkillTargetList.Count == 0)
				return false;
			if (battleManager.GameState != eBattleGameState.PLAY || !battleManager.GetAdvPlayed())
				return false;
			if (componentState.IsModeChangeEnd)
				return false;
			var purpose = battleManager.GetPurpose();
			if ((uint)((int)purpose - 3) <= 2u && componentState.IdleOnly)
				return false;
			if (componentAbnormal.ToadDatas.Count > 0 || componentAbnormal.IsAbnormalState(eAbnormalState.UB_SILENCE) || isUbDisabledByChangePattern)
				return false;
			if (battleManager.IsPausingEffectSkippedInThisFrame)
				return false;
			if (componentState.CurrentState == ActionState.UNION_BURST && !actionController.UnionBurstIsChargeTime)
				return false;
			if (componentAbnormal.IsUnableActionState() || componentState.CurrentState == ActionState.DAMAGE)
				return false;
			if (unitController.UnionBurstSkillId == 0 || componentAbnormal.IsAbnormalState(eAbnormalState.SILENCE))
				return false;
			if (componentState.IsDead || componentState.CurrentState == ActionState.DIE)
				return false;
			if (componentAbnormal.IsTauntState() && componentAbnormal.GetTauntSource().Owner.IsOther == unitController.IsOther)
				return false;
			if (componentAbnormal.IsConfusionOrConvert() || (long)componentParameter.Hp < 1L)
				return false;
			if (unitController.IsAwakeMotion || actionController.DisableUBByModeChange)
				return false;
			if (!BattleUtil.LessThanOrApproximately(1000.0, componentParameter.Energy))
				return false;
			if (unitController.SkillLevels[unitController.UnionBurstSkillId] == 0)
				return false;
			return !ExistsReduceEnergyAction();
		}

		[global::OpenPCR.Migration("8e1ec7cc-5116-47ca-a339-acaccd896848")]
		public void SubtractCoolDownTime(float _subtractTime, bool _isBlackout)
		{
			if (!_isBlackout && unionBurstCoolDownTime > 0f)
				unionBurstCoolDownTime -= _subtractTime;
		}

		[global::OpenPCR.Migration("8b37910d-9ee7-47ec-bc09-f498af024dd7")]
		private void addUnionBurstTimelineData(int _remainTime, bool _isBattleFinish)
		{
			var timelineList = unitController.UnitUnionBurstTimelineList;
			if (timelineList.Count <= 500)
			{
				var data = new UnitUnionBurstTimeline();
				data.SetUnitId(unitController.OriginalUnitId);
				data.SetRemainTime(_remainTime);
				data.SetIsBattleFinish(_isBattleFinish ? 1 : 0);
				timelineList.Add(data);
			}
		}

		[global::OpenPCR.Migration("37c79964-3edb-4f7e-a3e7-426cb0e665b5")] public bool IsCoolDown { [global::OpenPCR.Migration("0acbeff2-7608-4589-a849-b5546492d686")] get { return unionBurstCoolDownTime > 0f; } }
	}
}
