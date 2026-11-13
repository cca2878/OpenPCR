using global::Elements;
using global::OpenPCR.UICompat;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle;

namespace Elements
{
	public partial class AnimKitNormalEffect : SkillEffectCtrl
	{
		[SerializeField]
		private int index;
		private float startTime;
		[SerializeField]
		private eSortOrderType sortOrderType = eSortOrderType.BACK_OF_FX;
		[SerializeField]
		private float frontToBackTime;
		[SerializeField]
		private bool useFrontToBack;
		[SerializeField]
		private eSortOrder sortOrder;
		[SerializeField]
		private bool isFrontSort;
		public List<PrefabWithTime> BoneFollowEffectPrefab;
		private static IBattleEffectPool staticBattleEffectPool;
		private static bool isStaticInit;
		public bool Roop;
		protected BattleAnimKitController unitSpineController;
		public TweenPosition TweenPosition;
		[SerializeField]
		protected int skillId;
		public bool ConsiderCutInSkip;
		[global::OpenPCR.Migration("dd24448a-ea5d-4574-949a-5b939ef17474")]
		public AnimKitNormalEffect()
		{

		}

		[global::OpenPCR.Migration("38c3f93c-7479-4799-9a6d-8038e57f8b35")]
		public void SetIsIndependentBattleSync()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("3f048d55-7765-4c37-ba7e-b5e30386d446")]
		public override void ResetParameter(GameObject _prefab, int _skinId = 0, bool _isShadow = false)
		{
			base.ResetParameter(_prefab);
			if (unitSpineController != null)
			{
				unitSpineController.Resume();
				playAnime();
			}
		}

		[global::OpenPCR.Migration("e66bca52-4857-49de-9691-6c70c62445ad")]
		private void playAnime()
		{
			if (source != null && base.battleManager.IsDefenseReplayMode != source.IsLeftDir && unitSpineController.IsAnimation(unitSpineController.ConvertAnimeIdToAnimeName(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_REVERSE, skillId)))
			{
				unitSpineController.PlayAnime(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_REVERSE, skillId, Roop);
			}
			else
			{
				unitSpineController.PlayAnime(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT, skillId, Roop);
			}
		}

		[global::OpenPCR.Migration("4d508fd6-4fb8-4000-8a45-b1559e15d2e8")]
		public void SetDepth(int _depth)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("88f2bec3-bbfe-4bf4-bd91-693479da14b6")]
		public new static void StaticRelease()
		{
			isStaticInit = false;
			staticBattleEffectPool = null;
		}

		[global::OpenPCR.Migration("c114b50a-685c-4b8b-a76a-0c4f4827b154")]
		private void Awake()
		{
			awakeImpl();
			if (index != 0 && unitSpineController == null)
			{
				loadSpine();
			}

			if (TweenPosition != null)
			{
				EventDelegate.Set(TweenPosition.onFinished, onFinish);
			}

			if (!isStaticInit)
			{
				staticBattleEffectPool = base.singletonTree.Get<BattleEffectPool>();
				isStaticInit = true;
			}
		}

		[global::OpenPCR.Migration("facd4212-a315-4519-8de6-8a1e5c40a8d3")]
		protected override void onDestroy()
		{
			unitSpineController = null;
			TweenPosition = null;
			base.onDestroy();
		}

		[global::OpenPCR.Migration("6c11efe5-2e51-4f3c-a1b6-afeda6795120")]
		public override void ExecAppendCoroutine(UnitCtrl _unit = null, bool _isAbnormal = false)
		{
			base.ExecAppendCoroutine(_unit);
			AppendCoroutine(updateMotion(), ePauseType.VISUAL, _unit);
			if (source != null)
			{
				source.AppendCoroutine(createPrefabWithTime(BoneFollowEffectPrefab), ePauseType.NO_DIALOG);
			}
		}

		[global::OpenPCR.Migration("95963da8-b0a8-488c-a96e-b0e37a4b6231")]
		protected virtual void loadSpine(Action endAction = null)
		{
			BattleAnimKitController.LoadCreateImmidiateBySkinId(eAnimKitType.SKILL_EFFECT, skillId, skillId, index, base.transform, delegate (BattleAnimKitController unit)
			{
				unitSpineController = unit;
				playAnime();
				// Rendered layer output omitted.
				if (endAction != null)
				{
					endAction();
				}
			});
		}

		[global::OpenPCR.Migration("5d9aa21b-b49c-4d69-9502-3f4310156a42")]
		protected virtual void onFinish()
		{
			timeToDie = true;
		}

		[global::OpenPCR.Migration("dc50307f-31e2-4662-b27a-e346ff309c2c")]
		private IEnumerator updateMotion()
		{
			float timer = startTime;
			while (true)
			{
				if (useFrontToBack)
				{
					timer += source.DeltaTimeForPause;
					if (timer > frontToBackTime)
					{
						sortOrder = eSortOrder.BACK;
						if (base.IsFront)
						{
							SetSortOrderFront();
						}
						else
						{
							SetSortOrderBack();
						}
					}
				}

				if (!unitSpineController.IsPlayAnime)
				{
					onFinish();
					break;
				}

				if (!base.IsPlaying)
				{
					break;
				}

				yield return null;
			}
		}

		[global::OpenPCR.Migration("b017cbbb-7aed-4dcb-aa88-fb32f2792ab0")]
		public override void SetSortOrderBack()
		{
			base.SetSortOrderBack();
			if (unitSpineController != null)
			{
				setSortOrderBackImpl();
			}
			else
			{
				loadSpine(SetSortOrderBack);
			}
		}

		[global::OpenPCR.Migration("0efc22f9-b906-4475-a193-9be327f0c257")]
		private void setSortOrderBackImpl()
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("5451efb0-f079-4f45-be22-60124e0f5f14")]
		public override void SetSortOrderFront()
		{
			base.SetSortOrderFront();
			if (unitSpineController != null)
			{
				setSortOrderBackImpl();
				// Rendered depth output omitted.
			}
			else
			{
				loadSpine(SetSortOrderFront);
			}
		}

		[global::OpenPCR.Migration("0b73442e-f47c-41de-9fc8-7054f0f27875")]
		public override void Pause()
		{
			unitSpineController.Pause();
			if (TweenPosition != null)
			{
				TweenPosition.enabled = false;
			}

			isPause = true;
		}

		[global::OpenPCR.Migration("d5e79699-b732-4a15-9265-508deb530e9d")]
		public override void Resume()
		{
			unitSpineController.Resume();
			if (TweenPosition != null)
			{
				TweenPosition.enabled = true;
			}

			isPause = false;
		}

		[global::OpenPCR.Migration("8fceb7f1-5e0f-4388-b8dd-6bbb840122e8")]
		public override void SetStartTime(float _starttime)
		{
			startTime = _starttime;
			if (ConsiderCutInSkip)
			{
				TrackEntry current = unitSpineController.state.GetCurrent(0);
				current.lastTime = _starttime;
				current.time = _starttime;
			}
		}

		[global::OpenPCR.Migration("389e1659-087c-4668-b6b2-1689413819d3")]
		public override void OnAwakeWhenSkipCutIn()
		{
			if (ConsiderCutInSkip)
			{
				source.EffectSpineControllerList.Add(unitSpineController);
			}
		}

		[global::OpenPCR.Migration("030b460f-68e4-45a1-8ffd-4e1d08304fb8")]
		private IEnumerator createPrefabWithTime(List<PrefabWithTime> _data)
		{
			float time = startTime;
			bool[] effectCreated = new bool[_data.Count];
			while (true)
			{
				time += source.DeltaTimeForPause;
				int i = 0;
				for (int count = _data.Count; i < count; i++)
				{
					if (effectCreated[i])
					{
						if (i == count - 1)
						{
							yield break;
						}

						continue;
					}

					float time2 = _data[i].Time;
					GameObject prefab = _data[i].Prefab;
					if (time > time2)
					{
						SkillEffectCtrl effect = battleEffectPool.GetEffect(prefab, source);
						effect.InitializeSort();
						effect.PlaySe(source.ComponentSound.SoundUnitId, source.IsLeftDir);
						effect.transform.position = base.transform.position;
						effect.transform.parent = ExceptUIRoot.Transform;
						effect.SortTarget = source;
						effect.ExecAppendCoroutine(source, _isAbnormal: true);
						if (time < startTime)
						{
							effect.SetStartTime(startTime - time);
						}

						base.battleManager.StartCoroutine(effect.TrackTarget(unitSpineController, Vector3.zero, _followX: true, _followY: true, null, _data[i].IsTrackRotation));
						effectCreated[i] = true;
					}
				}

				yield return null;
			}
		}

		private enum eSortOrder
		{
			DEFAULT,
			FOREMOST,
			FRONT,
			BACK,
			TOP_BACK
		}

		private enum eSortOrderType
		{
			FRONT_OF_FX,
			BACK_OF_FX
		}

		protected IBattleEffectPool battleEffectPool
		{
			[global::OpenPCR.Migration("66db1c45-34ce-4bcc-8faf-3f7c9e71dcd0")]
			get
			{
				return staticBattleEffectPool;
			}
		}
	}
}
