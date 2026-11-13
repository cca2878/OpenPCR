using global::Elements;
using global::OpenPCR.Foundation;
using global::OpenPCR.Foundation.Audio;
using global::OpenPCR.Foundation.Audio.Movie;
using global::OpenPCR.Runtime.Api;
using Elements.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public partial class PrincessFormProcessor
	{
		[global::OpenPCR.Migration("fe8b93c0-4dad-420a-b961-4718c8c87020")]
		private System.Collections.IEnumerator waitStopComplete(System.Action _callback, eMovieType _movieType, long _movieId, long _movieIndex)
		{
			while (movieManager.GetStopIsNotComplete(_movieType, _movieId, _movieIndex))
				yield return null;
			_callback.Call();
		}

		private bool forceNormalSD;
		private AudioPlayback sePlayback;
		private Dictionary<int, Dictionary<int, MovieVoiceData>> MovieVoiceDataDictionary;
		private long currentMovieIndex;
		private long currentMovieId;
		private eMovieType currentMovieType;
		private IBattleTimeScaleForUnitCtrl battleTimeScale;
		private UnitActionController unitActionController;
		private UnitCtrl owner;
		private MovieManager movieManager;
		private BattleManager battleManager;
		[global::OpenPCR.Migration("45acdd4d-c7a8-442d-94ce-76d73f5ab5bb")]
		public PrincessFormProcessor()
		{
			currentMovieType = eMovieType.PRINCESS_FORM_SKILL_NORMAL;
			MovieVoiceDataDictionary = new Dictionary<int, Dictionary<int, MovieVoiceData>>();
			sePlayback = AudioPlayback.Error(SoundGroup.Se);
		}

		[global::OpenPCR.Migration("c46591a5-411b-4c46-ace4-96f73760d238")]
		public void Initialize(UnitCtrl _unitCtrl, UnitActionController _unitActionController, BattleManager _battleManager, IBattleTimeScaleForUnitCtrl _battleTimeScale)
		{
			movieManager = ManagerSingleton<MovieManager>.Instance;
			battleManager = _battleManager;
			owner = _unitCtrl;
			unitActionController = _unitActionController;
			battleTimeScale = _battleTimeScale;
			forceNormalSD = _battleManager.GetForceNormalSD();
		}

		[global::OpenPCR.Migration("09f0e054-d425-42df-a6ea-d589b5781228")]
		public void StartPrincessFormSkill(List<PrincessSkillMovieData> _princessSkillMovieDatas)
		{
			int unionBurstSkillId = owner.UnionBurstSkillId;
			eAnimKitCharacterAnimeId spineCharacterAnimeId = ((unitActionController.GetSkillMotionType(unionBurstSkillId) == SkillMotionType.EVOLUTION) ? eAnimKitCharacterAnimeId.PRINCESS_SKILL_EVOLUTION : eAnimKitCharacterAnimeId.PRINCESS_SKILL);
			unitActionController.StartAction(unionBurstSkillId);
			int current = 0;
			Action func = null;
			func = delegate
			{
				bool flag = battleManager.CanCreateBattleEffect();
				current++;
				if (current > _princessSkillMovieDatas.Count)
				{
					finishSkill();
				}
				else
				{
					if ((current == 1) & flag)
					{
						battleManager.IsPrincessSkillMovieFirstHalfRunning = true;
					}

					if (!flag)
					{
						playSpinePart();
					}
					else
					{
						battleManager.IsPrincessSkillMovieRunning = true;
						playPrincessFormMovie(current, _princessSkillMovieDatas, delegate
						{
							playSpinePart();
						});
					}
				}
			};
			func.Call();
			void playSpinePart()
			{
				battleManager.IsPrincessSkillMovieRunning = false;
				if (!owner.GetCurrentSpineCtrl().IsAnimation(spineCharacterAnimeId, current, -1, owner.MotionPrefix))
				{
					finishSkill();
				}
				else
				{
					owner.ComponentAnimation.PlayAnime(spineCharacterAnimeId, current, -1, owner.MotionPrefix, _isLoop: false);
					owner.AppendCoroutine(waitAnimationEnd(delegate
					{
						func.Call();
					}), ePauseType.SYSTEM, owner);
				}
			}
		}

		[global::OpenPCR.Migration("c84d290a-2721-4ea2-8762-7ebd3feb5fbf")]
		private void finishSkill()
		{
			battleManager.IsPrincessSkillRunning = false;
			movieManager.SetMirrorMode(_enable: false);
			battleManager.SetBlackoutTimeZero();
			owner.ComponentState.SkillEndProcess();
		}

		[global::OpenPCR.Migration("b474b959-9fe5-4c04-bb57-856f428a4287")]
		private void playPrincessFormMovie(int _movieIndex, List<PrincessSkillMovieData> _movieDataList, Action _callback)
		{
			if (global::OpenPCR.PreparedPrincessMoviePlayback.IsBound(movieManager))
			{
				PrincessSkillMovieData preparedSegment = _movieDataList[_movieIndex - 1];
				if (preparedSegment.UseFade) throw new global::OpenPCR.OutOfBoundaryException("Offline princess fade callbacks require their own decoder producer.");
				int preparedSkillId = owner.UnionBurstSkillId;
				bool completed = false;
				Action finish = () =>
				{
					if (completed) return;
					completed = true;
					_callback.Call();
				};
				// Use explicit offline decoder completion inputs. The original wait
				// owns midpoint animation update, movie flags and the completion callback.
				global::OpenPCR.PreparedPrincessMoviePlayback.Start(movieManager, (int)currentMovieType, preparedSkillId, _movieIndex);
				battleManager.StartCoroutine(waitMovieEndFrame(finish, preparedSkillId, currentMovieType, _movieIndex));
				currentMovieId = preparedSkillId;
				currentMovieIndex = _movieIndex;
				return;
			}

			bool isPrincessUpMovieSpeedUp = ManagerSingleton<SaveDataManager>.Instance.IsPrincessUpMovieSpeedUp;
			PrincessSkillMovieData princessSkillMovieData = _movieDataList[_movieIndex - 1];
			bool flag = !forceNormalSD && owner.IsShadow;
			if (!isPrincessUpMovieSpeedUp)
			{
				if (battleTimeScale.IsSpeedQuadruple)
				{
					currentMovieType = (flag ? eMovieType.PRINCESS_FORM_SKILL_SHADOW_4X : eMovieType.PRINCESS_FORM_SKILL_NORMAL_4X);
				}
				else
				{
					currentMovieType = (flag ? eMovieType.PRINCESS_FORM_SKILL_SHADOW : eMovieType.PRINCESS_FORM_SKILL_NORMAL);
				}
			}
			else if (!battleTimeScale.SpeedupFlag)
			{
				currentMovieType = (flag ? eMovieType.PRINCESS_FORM_SKILL_SHADOW : eMovieType.PRINCESS_FORM_SKILL_NORMAL);
			}
			else
			{
				currentMovieType = (flag ? eMovieType.PRINCESS_FORM_SKILL_SHADOW_4X : eMovieType.PRINCESS_FORM_SKILL_NORMAL_4X);
			}

			int unionBurstSkillId = owner.UnionBurstSkillId;
			float num = 0.75f;
			ViewManager instance = ManagerSingleton<ViewManager>.Instance;
			SoundManager instance2 = ManagerSingleton<SoundManager>.Instance;
			if (_movieIndex != 1)
			{
				movieManager.Load(currentMovieType, unionBurstSkillId, _isShowSubtitle: false, null, _isWaitPrepare: true, _movieIndex);
			}

			movieManager.SetScreenScale(currentMovieType, unionBurstSkillId, new Vector2(instance.ActiveScreenWidthInUIRoot, instance.ActiveScreenWidthInUIRoot * num), _isPanScan: true, _movieIndex);
			movieManager.SetCollider(currentMovieType, unionBurstSkillId, _isEnable: false, _movieIndex);
			movieManager.SetSortOrder(currentMovieType, unionBurstSkillId, 0, _movieIndex);
			movieManager.SetMoviePlayerLocalPosition(currentMovieType, unionBurstSkillId, WrapperEngineApiScreen.GetRestoreNGUIOffsetYVector3(), _movieIndex);
			if (MovieVoiceDataDictionary.ContainsKey(_movieIndex))
			{
				foreach (KeyValuePair<int, MovieVoiceData> item in MovieVoiceDataDictionary[_movieIndex])
				{
					owner.StartCoroutine(playUbVoiceWithDelay(item.Value));
				}
			}

			if (battleManager.CanPlaySe())
			{
				sePlayback = instance2.PlaySeFromName(BattleVoiceUtility.ConvertToSkillCueSheetName(owner.ComponentSound.SoundUnitId), buildSkillMovieName(unionBurstSkillId, _movieIndex));
			}

			bool changeFPS = false;
			if (battleManager.FrameRate == 30)
			{
				changeFPS = battleTimeScale.SpeedupFlag | isPrincessUpMovieSpeedUp;
			}

			if (changeFPS)
			{
				OverrideApplication.targetFrameRate = (int)(battleTimeScale.SpeedUpRate * 30f);
			}

			bool alreadyCalled = false;
			Action action = delegate
			{
				if (!alreadyCalled)
				{
					alreadyCalled = true;
					if (changeFPS)
					{
						OverrideApplication.targetFrameRate = 30;
					}

					_callback.Call();
				}
			};
			Action setSpeed = delegate
			{
				if (isPrincessUpMovieSpeedUp)
				{
					if (battleTimeScale.IsSpeedQuadruple)
					{
						movieManager.SetPlaySpeed(currentMovieType, unionBurstSkillId, 1.5f, _movieIndex);
					}
					else if (!battleTimeScale.SpeedupFlag)
					{
						movieManager.SetPlaySpeed(currentMovieType, unionBurstSkillId, 2f, _movieIndex);
					}
				}
				else if (!battleTimeScale.IsSpeedQuadruple)
				{
					movieManager.SetPlaySpeed(currentMovieType, unionBurstSkillId, battleTimeScale.SpeedupFlag ? battleTimeScale.SpeedUpRate : 1f, _movieIndex);
				}
			};
			if (princessSkillMovieData.UseFade)
			{
				movieManager.Play(currentMovieType, unionBurstSkillId, eMoviePlayType.NORMAL, action, PlayEndType.Stop, princessSkillMovieData.FadeStartTime, princessSkillMovieData.FadeDuration, _movieIndex);
				battleManager.StartCoroutine(waitMovieEndFrame(null, unionBurstSkillId, currentMovieType, _movieIndex));
				setSpeed.Call();
			}
			else if (movieManager.GetStopIsNotComplete(currentMovieType, unionBurstSkillId, _movieIndex))
			{
				battleManager.StartCoroutine(waitStopComplete(delegate
				{
					movieManager.Play(currentMovieType, unionBurstSkillId, eMoviePlayType.NORMAL, null, PlayEndType.Stop, 0f, 0f, _movieIndex);
					battleManager.StartCoroutine(waitMovieEndFrame(action, unionBurstSkillId, currentMovieType, _movieIndex));
					setSpeed.Call();
				}, currentMovieType, unionBurstSkillId, _movieIndex));
			}
			else
			{
				movieManager.Play(currentMovieType, unionBurstSkillId, eMoviePlayType.NORMAL, null, PlayEndType.Stop, 0f, 0f, _movieIndex);
				battleManager.StartCoroutine(waitMovieEndFrame(action, unionBurstSkillId, currentMovieType, _movieIndex));
				setSpeed.Call();
			}

			currentMovieId = unionBurstSkillId;
			currentMovieIndex = _movieIndex;
		}

		[global::OpenPCR.Migration("7693fa9a-97f5-4bfc-a990-3f15e9007639")]
		public IEnumerator waitMovieEndFrame(Action _callback, long _movieId, eMovieType _movieType, int _movieIndex)
		{
			MovieManager movieManager = ManagerSingleton<MovieManager>.Instance;
			float oneFrame = 1f / 30f;
			float duration;
			while (true)
			{
				duration = movieManager.GetDurationTime(_movieType, _movieId, _movieIndex);
				if (duration != 0f)
				{
					break;
				}

				yield return null;
			}

			bool resetAnime = false;
			float previous = -1f;
			while (true)
			{
				float seekPositionTime = movieManager.GetSeekPositionTime(_movieType, _movieId, _movieIndex);
				if (!resetAnime && seekPositionTime >= duration / 2f)
				{
					movieManager.SetPlayEndTypeDestroy(_movieType, _movieId, _movieIndex);
					resetAnime = true;
					eAnimKitCharacterAnimeId animeId = ((unitActionController.GetSkillMotionType((int)_movieId) == SkillMotionType.EVOLUTION) ? eAnimKitCharacterAnimeId.PRINCESS_SKILL_EVOLUTION : eAnimKitCharacterAnimeId.PRINCESS_SKILL);
					BattleAnimKitController currentSpineCtrl = owner.GetCurrentSpineCtrl();
					if (currentSpineCtrl.IsAnimation(animeId, _movieIndex, -1, owner.MotionPrefix))
					{
						owner.ComponentAnimation.PlayAnime(animeId, _movieIndex, -1, owner.MotionPrefix, _isLoop: false);
					}
					else
					{
						owner.ComponentAnimation.PlayAnime(eAnimKitCharacterAnimeId.IDLE, owner.MotionPrefix);
					}

					currentSpineCtrl.RealUpdate();
					currentSpineCtrl.RealLateUpdate();
					if (_movieIndex == 1)
					{
						battleManager.IsPrincessSkillMovieFirstHalfRunning = false;
					}

					if (_callback == null)
					{
						yield break;
					}
				}

				if (!(seekPositionTime >= previous && seekPositionTime + oneFrame + oneFrame * 0.5f < duration))
				{
					break;
				}

				previous = seekPositionTime;
				yield return null;
			}

			_callback.Call();
		}

		[global::OpenPCR.Migration("901a1e5f-686b-4be0-b562-3680a7b4250a")]
		public void Prepare()
		{
			battleManager.IsPrincessSkillRunning = true;
			if (!battleManager.CanCreateBattleEffect())
			{
				return;
			}

			// Bound offline mode projects movie type/load and muted voice output only.
			// Native IsPrincessSkillRunning and CanCreateBattleEffect remain above.
			if (global::OpenPCR.PreparedPrincessMoviePlayback.IsBound(movieManager)) return;

			bool flag = !forceNormalSD && owner.IsShadow;
			if (!ManagerSingleton<SaveDataManager>.Instance.IsPrincessUpMovieSpeedUp)
			{
				if (battleTimeScale.IsSpeedQuadruple)
				{
					currentMovieType = (flag ? eMovieType.PRINCESS_FORM_SKILL_SHADOW_4X : eMovieType.PRINCESS_FORM_SKILL_NORMAL_4X);
				}
				else
				{
					currentMovieType = (flag ? eMovieType.PRINCESS_FORM_SKILL_SHADOW : eMovieType.PRINCESS_FORM_SKILL_NORMAL);
				}
			}
			else if (!battleTimeScale.SpeedupFlag)
			{
				currentMovieType = (flag ? eMovieType.PRINCESS_FORM_SKILL_SHADOW : eMovieType.PRINCESS_FORM_SKILL_NORMAL);
			}
			else
			{
				currentMovieType = (flag ? eMovieType.PRINCESS_FORM_SKILL_SHADOW_4X : eMovieType.PRINCESS_FORM_SKILL_NORMAL_4X);
			}

			int unionBurstSkillId = owner.UnionBurstSkillId;
			movieManager.Load(currentMovieType, unionBurstSkillId, _isShowSubtitle: false, null, _isWaitPrepare: true, 1L);
			foreach (KeyValuePair<int, Dictionary<int, MovieVoiceData>> item in MovieVoiceDataDictionary)
			{
				foreach (KeyValuePair<int, MovieVoiceData> item2 in item.Value)
				{
					item2.Value.VoiceDelayAndEnable.Enable = false;
				}
			}
		}

		[global::OpenPCR.Migration("cc051daf-9c91-4a24-9973-d2f21641d86b")]
		private IEnumerator waitAnimationEnd(Action _callback)
		{
			do
			{
				yield return null;
			}
			while (owner.GetCurrentSpineCtrl().IsPlayAnimeBattle);
			_callback.Call();
		}

		[global::OpenPCR.Migration("c8042bce-510d-4d17-9370-f0bfd035a55e")]
		public void FadeOutSe(float _fadeSec)
		{
			ManagerSingleton<SoundManager>.Instance.StopSe(sePlayback, _fadeSec);
		}

		[global::OpenPCR.Migration("08505168-ec7c-4c6a-b3fa-e99a523ef40b")]
		private IEnumerator playUbVoiceWithDelay(MovieVoiceData _data)
		{
			float time = 0f;
			Skill skill = (_data.Rarity6 ? unitActionController.UnionBurstEvolutionList[0] : unitActionController.UnionBurstList[0]);
			float delay = _data.VoiceDelayAndEnable.Delay;
			if (!_data.VoiceDelayAndEnable.Enable)
			{
				yield break;
			}

			while (true)
			{
				if (!SingletonMonoBehaviour<BattleHeaderController>.Instance.IsPaused)
				{
					time += Time.deltaTime;
				}

				if (time >= delay)
				{
					break;
				}

				yield return null;
			}

			if (skill.EffectBranchId == _data.Index % 100)
			{
				owner.ComponentSound.PlayVoice(SoundManager.eVoiceType.UNION_BURST, _data.Index, _lastSuffixIsRandom: false);
			}
		}

		[global::OpenPCR.Migration("b72b6678-3c15-4b29-aace-ac054370608d")]
		private string buildSkillMovieName(int _a, int _b)
		{
			return string.Format("p_skill_{0}_{1}", _a, _b);
		}

		[global::OpenPCR.Migration("bfa06a47-a5f2-4ece-9fd1-45bbd1266fa3")]
		public void Pause(bool _pause)
		{
			global::OpenPCR.Presentation.Noop();
		}

		private class MovieVoiceData
		{
			public VoiceDelayAndEnable VoiceDelayAndEnable;
			public int Index;
			public bool Rarity6;

		}
	}
}
