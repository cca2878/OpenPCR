using global::Elements;
using global::Elements.Battle;
using global::OpenPCR;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using global::OpenPCR.AnimKit.Scene.Modules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class BattleAnimKitController : BattleUnitBaseAnimKitController, ISingletonField
	{
		private Shader[] currentChildObjectShader = new Shader[0];
		private int startedCoroutineId;
		private bool isIndependentBattleSync;
		private bool isStone;
		private bool isPauseAction;
		private static Yggdrasil<BattleAnimKitController> staticSingletonTree;
		private static IBattleManagerForBattleAnimKitController staticBattleManager;
		public UnitCtrl Owner { get; set; }
		public bool IsStopState { get; set; }
		public bool IsPlayAnimeBattle { get; set; }
		public float StopStateTime { get; set; }
		private Action stopStateEvent { get; set; }
		public float DropTreasureBoxTime { get; set; }
		public bool IsConstantVelocity { get; set; }
		public bool IsAlphaZero { get; set; }
		[global::OpenPCR.ImplicitMigration("43b1eba4-08a6-4e7a-b1c5-281c05e58ec6")]
		private IBattleManagerForBattleAnimKitController battleManager => staticBattleManager;

		public bool IsIndependentBattleSync
		{
			get
			{
				return isIndependentBattleSync;
			}

			[global::OpenPCR.Migration("fb6d36c7-c838-4f20-bd58-c1d095a3e1c9")]
			set
			{
				isIndependentBattleSync = value;
			}
		}

		[global::OpenPCR.Migration("0484e89d-ffd8-46fb-b090-65f4ee1ed527")]
		public BattleAnimKitController()
		{
		}

		[global::OpenPCR.Migration("22c86de3-bc35-4969-8969-8454617be019")]
		public bool HasSpecialSleepAnimatilon(int _motionPrefix)
		{
			return IsAnimation(eAnimKitCharacterAnimeId.SLEEP, _motionPrefix, 0);
		}

		[global::OpenPCR.Migration("5abad5a3-f71e-4552-91f8-09df95054fab")]
		public void SetCurColor(Color _color)
		{
			Color curColor = _color;
			if (IsAlphaZero)
			{
				curColor.a = 0f;
			}

			base.CurColor = curColor;
		}

		[global::OpenPCR.Migration("78dc1593-a678-4d9a-afd6-6bda4e49ae4f")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
		}

		[global::OpenPCR.Migration("991fd3e0-2dd4-4190-9fbb-65e45665ebd4")]
		public static BattleAnimKitController LoadCreateImmidiateBySkinId(eAnimKitType _spineType, int _skinId, int _trgIdx0, int _trgIdx1, Transform _transform, Action<BattleAnimKitController> _callback = null)
		{
			BattleAnimKitController component = (BattleAnimKitController)global::OpenPCR.PreparedAnimKitLoads.Create((int)_spineType, _skinId, _trgIdx0, _trgIdx1, _transform, 0);
			_callback.Call(component);
			return component;
		}

		[global::OpenPCR.Migration("815cbf29-900d-4fba-a8a6-072e81524c6e")]
		public override void Update()
		{
			// Native intentionally suppresses the automatic Update reached by AnimationName's setter.
			// BattleManager drives the same animation state explicitly through RealUpdate.
		}

		[global::OpenPCR.Migration("77d4af4c-d906-4697-a460-40b793050553")]
		public override void LateUpdate()
		{
			// Native suppresses the inherited automatic mesh update; battle rendering is explicitly driven.
		}

		[global::OpenPCR.Migration("00fb7dbf-5ba8-4e1a-ac1f-5196603fc86e")]
		public void UpdateIndependentBattleSync()
		{
			if (IsIndependentBattleSync)
			{
				RealUpdate();
			}
		}

		[global::OpenPCR.Migration("8e421eab-58fb-4fdc-9d8a-7714d04ee10c")]
		public void LateUpdateIndependentBattleSync()
		{
			if (IsIndependentBattleSync)
			{
				RealLateUpdate();
			}
		}

		[global::OpenPCR.Migration("cd2b0279-2da5-4d8b-bf16-44bf545671ea")]
		public void LightUpdate(float deltaTime)
		{
			if (valid)
			{
				deltaTime *= timeScale;
				skeleton.Update(deltaTime);
				state.Update(deltaTime);
				state.LightApply(skeleton);
			}
		}

		[global::OpenPCR.Migration("177e443d-95a6-4f77-867b-c4729a64da77")]
		public void RealUpdate()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}

			if (battleManager != null)
			{
				float deltaTime = (IsConstantVelocity ? (battleManager.DeltaTime / Time.timeScale) : battleManager.DeltaTime);
				if (battleManager.IsSkipping())
				{
					LightUpdate(deltaTime);
				}
				else
				{
					Update(deltaTime);
				}
			}
			else
			{
				base.Update();
			}
		}

		[global::OpenPCR.Migration("52ec45fb-2bdb-438e-93ff-8167fa3fec1c")]
		public void RealLateUpdate()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("2a24b3de-b73a-46e7-84de-4ef153d81223")]
		public void PlayAnime(eAnimKitCharacterAnimeId _animeId, bool _playLoop = true)
		{
			PlayAnime(ConvertAnimeIdToAnimeName(_animeId), _playLoop);
		}

		[global::OpenPCR.Migration("7823a0da-47e8-4398-89cd-fbeec6e5fd4d")]
		public void PlayAnime(eAnimKitCharacterAnimeId _animeId, int _index1, bool _playLoop = true)
		{
			PlayAnime(ConvertAnimeIdToAnimeName(_animeId, _index1), _playLoop);
		}

		[global::OpenPCR.Migration("b837edc6-390e-4736-80fd-bccb79f8bede")]
		public void PlayAnime(string _playAnimeName, bool _playLoop, float _startTime = 0f, bool _ignoreBlackout = false)
		{
			base.PlayAnime(_playAnimeName, _playLoop);
			IsPlayAnimeBattle = true;
			float duration = state.Data.skeletonData.FindAnimation(_playAnimeName).Duration;
			if (!_playLoop && (Owner == null || !Owner.ComponentUnionBurst.PlayingNoCutinMotion))
			{
				setUpdateAnime(_startTime, duration, _ignoreBlackout);
			}
		}

		[global::OpenPCR.Migration("45bc722c-deeb-4b95-b2bd-423b2e4fbb23")]
		public void SetDropTreasureBoxTime()
		{
			global::OpenPCR.AnimKit.Animation animation = state.Data.skeletonData.FindAnimation(ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DIE));
			if (animation == null)
			{
				DropTreasureBoxTime = 0.2f;
				return;
			}
			Timeline timeline = animation.timelines.Find(e => e is EventTimeline);
			if (timeline == null) DropTreasureBoxTime = 0.2f;
			else DropTreasureBoxTime = ((EventTimeline)timeline).frames[0];
		}

		[global::OpenPCR.Migration("0f105b59-b689-4c93-ba3b-891cfc5c3bde")]
		public void RestartPlayAnimeCoroutine(float _startTime, eAnimKitCharacterAnimeId _animeId, int _index, int _prefix)
		{
			float duration = state.Data.skeletonData.FindAnimation(ConvertAnimeIdToAnimeName(_animeId, _index, -1, _prefix)).Duration;
			IsPlayAnimeBattle = true;
			setUpdateAnime(_startTime, duration);
		}

		[global::OpenPCR.Migration("aad4e5bf-a984-450c-afe7-df3b55d0de19")]
		public override void PlayAnime(string _playAnimeName, bool _playLoop = true)
		{
			PlayAnime(_playAnimeName, _playLoop, 0f, false);
		}

		[global::OpenPCR.Migration("be3b759f-1250-4eb7-8035-166449636677")]
		public void PlayAnimeNoOverlap(eAnimKitCharacterAnimeId _animeId, int _index1, bool _playLoop = true)
		{
			PlayAnimeNoOverlap(ConvertAnimeIdToAnimeName(_animeId, _index1), _playLoop);
		}

		[global::OpenPCR.Migration("61d0a387-0c87-44cb-8eef-032b2635adb4")]
		public void PlayAnimeNoOverlap(eAnimKitCharacterAnimeId _animeId, int _index1, int _ndex2, int _index3, bool _playLoop = true)
		{
			base.PlayAnimeNoOverlap(ConvertAnimeIdToAnimeName(_animeId, _index1, _ndex2, _index3), _playLoop);
		}

		[global::OpenPCR.Migration("8602fa00-9e7c-4edc-9ad2-1aa49de63c1a")]
		public void SetAnimeEventDelegateForBattle(Action _callBack, int _motionPrefix = -1)
		{
			Timeline timeline = state.Data.skeletonData.FindAnimation(ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMEGE, _motionPrefix)).timelines.Find((Timeline e) => e is EventTimeline);
			stopStateEvent = _callBack;
			if (timeline != null)
			{
				StopStateTime = (timeline as EventTimeline).frames[0];
			}
		}

		[global::OpenPCR.Migration("18783162-dbd9-4085-872b-0c02ca9424c5")]
		public void PlayAnime(eAnimKitCharacterAnimeId _animeId, int _index1, int _index2, int _index3, bool _playLoop = true, float _startTime = 0f, bool _ignoreBlackout = false)
		{
			PlayAnime(ConvertAnimeIdToAnimeName(_animeId, _index1, _index2, _index3), _playLoop, _startTime, _ignoreBlackout);
		}

		[global::OpenPCR.Migration("1f126646-5f04-4250-a8c4-95bd718b0328")]
		private void setUpdateAnime(float _startTime, float _endTime, bool _ignoreBlackout = false)
		{
			if (_ignoreBlackout)
			{
				battleManager.AppendCoroutine(updateAnime(_startTime, _endTime, ++startedCoroutineId), ePauseType.IGNORE_BLACK_OUT, null);
			}
			else
			{
				battleManager.AppendCoroutine(updateAnime(_startTime, _endTime, ++startedCoroutineId), ePauseType.SYSTEM, Owner);
			}
		}

		[global::OpenPCR.Migration("874a0bf8-cc99-43e8-8417-055b46f26b8f")]
		private IEnumerator updateAnime(float _startTime, float _endTime, int _thisId)
		{
			float time = _startTime;
			bool callDamageEvent = Owner != null && base.AnimationName == ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.DAMEGE, Owner.MotionPrefix);
			while (true)
			{
				if (startedCoroutineId != _thisId) yield break;
				time += getDeltaTimeForPause();
				if (callDamageEvent && time > StopStateTime)
				{
					stopStateEvent.Call();
					callDamageEvent = false;
				}
				if (time > _endTime || BattleUtil.Approximately(time, _endTime)) break;
				yield return null;
			}
			IsPlayAnimeBattle = false;
			IsStopState = false;
		}

		public bool IsColorStone
		{
			get
			{
				return isStone;
			}

			[global::OpenPCR.Migration("570bbbc1-317f-4686-9f42-53264af0590e")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}

		public bool IsColorPauseAction
		{
			get
			{
				return isPauseAction;
			}

			[global::OpenPCR.Migration("cbe49c10-242b-4498-b5ee-fb247aa6d90f")]
			set
			{
				global::OpenPCR.Presentation.Noop();
			}
		}

		[global::OpenPCR.Migration("e15eeab0-efbe-45bd-9bee-0802cd73c1c6")]
		public bool CheckPlaySpecialSleepAnimeExceptRelease(int _motionPrefix)
		{
			if (!HasSpecialSleepAnimatilon(_motionPrefix))
			{
				return false;
			}
			if (base.AnimationName == ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.SLEEP, _motionPrefix, 0))
			{
				return true;
			}
			if (base.AnimationName == ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.SLEEP, _motionPrefix, 1))
			{
				return true;
			}
			return false;
		}

		[global::OpenPCR.Migration("848443be-55f5-470e-a42c-d3ef57983487")]
		public static void LoadCreate(eAnimKitType _spineType, int _unitId, int _rarity, int _prefabId, Transform _transform, Action<BattleAnimKitController> _callback = null, int _enemyColor = 0)
		{
			int num = UnitUtility.GetSkinId(_spineType, _unitId, _rarity);
			int num2 = 0;
			if (_prefabId != 0)
			{
				num2 = UnitUtility.GetSkinId(_spineType, _prefabId, _rarity, _unitId);
			}
			BattleAnimKitController component = (BattleAnimKitController)global::OpenPCR.PreparedAnimKitLoads.Create((int)_spineType, num, (num2 == 0) ? num : num2, _enemyColor, _transform, num2);
			_callback.Call(component);
		}

		[global::OpenPCR.Migration("d4fee0ce-ad71-4496-a2b7-8a629c6190e7")]
		public static BattleAnimKitController LoadCreateImmidiate(eAnimKitType _spineType, int _unitId, int _rarity, Transform _transform, Action<BattleAnimKitController> _callback = null)
		{
			int trgIdx = UnitUtility.GetSkinId(_spineType, _unitId, _rarity);
			return LoadCreateImmidiateBySkinId(_spineType, trgIdx, trgIdx, 0, _transform, delegate (BattleAnimKitController _loadedObject)
			{
				_callback.Call(_loadedObject);
			});
		}
		[global::OpenPCR.Migration("9238f96e-5498-4cc3-a2d1-22063e742ecf")]
		private float getDeltaTimeForPause()
		{
			if (isPause) return 0f;
			if (battleManager == null) return 1f / 30f;
			if (battleManager.IsPausingEffectSkippedInThisFrame) return 0f;
			return battleManager.DeltaTime;
		}
	}
}
