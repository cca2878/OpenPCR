using global::OpenPCR.UICompat;
using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.AudioStubs;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Elements.Battle;
using Elements.Battle.Core;

namespace Elements
{
	public class SkillEffectCtrl : MonoBehaviour, ISingletonField
	{
		private const int SORT_ORDER_BOUNDARY = 1000;
		public enum eToadTrackType
		{
			NONE,
			HEAD,
			CENTER
		}

		[Serializable]
		private class RarityUpTextureChangeDataSet
		{

		}

		[Serializable]
		public class BoneTimeLineData
		{
			public string BoneName;
			public float Time;
			public BoneTimeLineData()
			{
				BoneName = string.Empty;
			}
		}

		[StructLayout((LayoutKind)0, Pack = 1, Size = 8)]
		public struct SortOrderAndOffset
		{
			public int SortOrder;
			public int Offset;
		}

		public static readonly eSE[] BattleCommonSeArray = new eSE[2]
		{
			eSE.NONE,
			eSE.BTL_FIREARM_ARROW_1
		};
		[SerializeField]
		protected ParticleSystem particle;
		[SerializeField]
		public TweenPosition[] TweenerList = new TweenPosition[0];
		[SerializeField]
		public TweenRotation[] TweenRotList = new TweenRotation[0];
		[SerializeField]
		public Animator[] AnimatorList = new Animator[0];
		[SerializeField]
		protected AudioAtomSource skillSeSource;
		[SerializeField]
		protected bool isRepeat;
		protected UnitCtrl source;
		[SerializeField]
		private bool isCommon;
		[SerializeField]
		private eBattleSkillSeType seType;
		[SerializeField]
		private List<RarityUpTextureChangeDataSet> textureChangeParticles;
		private List<BoneTimeLineData> boneTimeLine;
		private float boneTimeLineStartTime;
		protected bool timeToDie;
		protected bool isPause;
		public Vector3 BaseScale { get; set; } = Vector3.one;
		private SoundManager soundManager = ManagerSingleton<SoundManager>.Instance;
		protected Dictionary<Renderer, SortOrderAndOffset> particleRendererDictionary;
		private int currentCoroutineId;
		private static readonly string LAYER_NAME = "Default";
		private static Yggdrasil<SkillEffectCtrl> staticSingletonTree = null;
		private static IBattleManagerForSkillEffectCtrl staticBattleManager = null;
		public bool IsAura { get; set; }
		public Action<SkillEffectCtrl> OnEffectEnd { get; set; }
		public UnitCtrl SortTarget { get; set; }
		public UnitCtrl Target { get; set; }

		public bool IsRepeat
		{
			[global::OpenPCR.Migration("a50169ed-f861-48e9-a3d3-d40c526dbeb0")]
			get
			{
				return isRepeat;
			}

			[global::OpenPCR.Migration("5eeea007-7c6f-452c-a532-e3df6755a2c0")]
			set
			{
				isRepeat = value;
			}
		}

		public bool IsPlaying { get; set; }
		private int trackTargetCoroutinueId { get; set; }
		public bool IsFront { get; set; }
		protected ParticleSystem[] particles { get; set; }
		private Dictionary<ParticleSystem, float> particleStartDelayDictionary { get; set; }
		private float resumeTime { get; set; }
		[global::OpenPCR.ImplicitMigration("6487a462-f40b-4fd2-b6f1-ed6a2424963f")]
		protected Yggdrasil<SkillEffectCtrl> singletonTree => staticSingletonTree;
		[global::OpenPCR.ImplicitMigration("0be8b175-5f83-4935-b98a-2411b7f3d467")]
		protected IBattleManagerForSkillEffectCtrl battleManager => staticBattleManager;

		public AudioAtomSource SeSource
		{
			[global::OpenPCR.Migration("cb826ab1-bc27-4bbb-a6be-939d17271f91")]
			get
			{
				return skillSeSource;
			}

			set
			{
				skillSeSource = value;
			}
		}

		[global::OpenPCR.Migration("bcd55df1-2831-4ccc-9cff-4e5fed2e12ee")]
		public SkillEffectCtrl()
		{
		}

		[global::OpenPCR.Migration("9654b4f2-1f25-4431-b01a-4acf184ae6b4")]
		public static void StaticInitSingletonTree()
		{
			staticSingletonTree = null;
		}

		[global::OpenPCR.Migration("e6605ccc-0774-4934-bfba-5528208a5f1f")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
		}

		[global::OpenPCR.Migration("3b3abd62-96ac-406b-97f8-048e907f282a")]
		public virtual void InitializeSort()
		{
			battleManager.AddEffectToUpdateList(this);
			// Bone timelines remain real; particles and renderer sorting are output-only.
			EffectBoneTimeLineData component = GetComponent<EffectBoneTimeLineData>();
			if (component != null) boneTimeLine = component.BoneTimeLine;
		}

		[global::OpenPCR.Migration("8c390dc0-14de-45b7-b07f-c3a170d3e94e")]
		public virtual void ExecAppendCoroutine(UnitCtrl _unit = null, bool _isAbnormal = false)
		{
			if (!isRepeat)
			{
				AppendCoroutine(updateTimer(++currentCoroutineId), ePauseType.VISUAL, _unit);
			}
			else
			{
				AppendCoroutine(UpdateTimerRepeat(), ePauseType.VISUAL);
			}

			battleManager.AppendEffect(this, _unit, _isAbnormal);
		}

		[global::OpenPCR.Migration("a5a88c77-5ff5-4bdb-87ab-1935bc72bc84")]
		private void particleRendererDictionaryInitialize()
		{
			if (particleRendererDictionary != null)
			{
				return;
			}
			particleRendererDictionary = new Dictionary<Renderer, SortOrderAndOffset>();
			particles = base.gameObject.GetComponentsInChildren<ParticleSystem>(includeInactive: true);
			particleStartDelayDictionary = new Dictionary<ParticleSystem, float>();
			EffectBoneTimeLineData component = GetComponent<EffectBoneTimeLineData>();
			if (component != null)
			{
				boneTimeLine = component.BoneTimeLine;
			}
			for (int i = 0; i < particles.Length; i++)
			{
				Renderer component2 = particles[i].GetComponent<Renderer>();
				component2.sortingOrder = Mathf.Min(5000, component2.sortingOrder);
				SortOrderAndOffset value = new SortOrderAndOffset
				{
					SortOrder = component2.sortingOrder
				};
				EffectSortOffsetData component3 = particles[i].GetComponent<EffectSortOffsetData>();
				if (component3 != null)
				{
					value.Offset = component3.Offset;
				}
				particleRendererDictionary.Add(component2, value);
				particleStartDelayDictionary.Add(particles[i], particles[i].main.startDelayMultiplier);
			}
		}

		[global::OpenPCR.Migration("747ba0f9-e057-4123-95f9-a82577d8e0ff")]
		public virtual void SetPossitionAppearanceType(SkillEffectBase _skillEffect, BasePartsData _target, UnitCtrl _owner, Skill _skill, eToadTrackType _toadTrackType = eToadTrackType.NONE)
		{
			if (_skillEffect.EffectBehavior != eEffectBehavior.FIREARM && _skillEffect.EffectBehavior != eEffectBehavior.SERIES_FIREARM)
			{
				source = _owner;
				if ((uint)_skillEffect.EffectTarget <= 6u)
				{
					SortTarget = _target.Owner;
					if (_skillEffect.TargetBone == eTargetBone.ANY_BONE)
					{
						if (_skillEffect.TrackType != eTrackType.BONE_TIME_LINE) _skillEffect.TrackType = eTrackType.BONE;
						_skillEffect.TrackDimension = eTrackDimension.XY;
					}
				}
				if (_skillEffect.EffectBehavior == eEffectBehavior.NORMAL && _skillEffect.EffectTarget == eEffectTarget.TARGET_CENTER_FOR_FIREARM)
					_skillEffect.TrackType = eTrackType.NONE;
				switch (_skillEffect.TrackType)
				{
					case eTrackType.BONE:
					case eTrackType.BOTTOM:
						switch (_skillEffect.TrackDimension)
						{
							case eTrackDimension.X:
								battleManager.StartCoroutine(TrackTarget(_target, Vector3.zero, true, false));
								break;
							case eTrackDimension.Y:
								battleManager.StartCoroutine(TrackTarget(_target, Vector3.zero, false));
								break;
							case eTrackDimension.XY:
								battleManager.StartCoroutine(TrackTarget(_target, Vector3.zero));
								break;
						}
						break;
					case eTrackType.BONE_TIME_LINE:
						bool flag = _skill.SkillId == _owner.UnionBurstSkillId;
						AppendCoroutine(updateBoneTimeLine(_target, _skillEffect.TrackRotation), flag ? ePauseType.NO_DIALOG : ePauseType.VISUAL, flag ? null : _target.Owner);
						break;
				}
				return;
			}

			source = _owner;
			Vector3 vector = new Vector3(0f, 9.259259f, 0f);
			Bone bone = null;
			eEffectTarget effectTarget = _skillEffect.EffectTarget;
			if ((uint)effectTarget <= 6u)
			{
				SortTarget = _target.Owner;
				switch (_skillEffect.TargetBone)
				{
					case eTargetBone.BOTTOM:
						vector = _target.GetBottomTransformPosition();
						break;
					case eTargetBone.HEAD:
						bone = _target.GetStateBone();
						vector = BattleUnitBaseAnimKitController.BoneWorldToGlobalPosConsiderRotate(_target.GetStateBone(), _target.Owner.RotateCenter, _target.GetBottomLossyScale());
						break;
					case eTargetBone.CENTER:
						bone = _target.GetCenterBone();
						vector = BattleUnitBaseAnimKitController.BoneWorldToGlobalPosConsiderRotate(_target.GetCenterBone(), _target.Owner.RotateCenter, _target.GetBottomLossyScale());
						break;
					case eTargetBone.FIXED_CENETER:
						vector = _target.GetBottomTransformPosition() + _target.GetFixedCenterPos();
						break;
					case eTargetBone.ANY_BONE:
						bone = SortTarget.GetCurrentSpineCtrl().skeleton.FindBone(_skillEffect.TargetBoneName);
						if (_skillEffect.TrackType != eTrackType.BONE_TIME_LINE)
						{
							_skillEffect.TrackType = eTrackType.BONE;
						}
						_skillEffect.TrackDimension = eTrackDimension.XY;
						vector = BattleUnitBaseAnimKitController.BoneWorldToGlobalPosConsiderRotate(bone, SortTarget.RotateCenter, SortTarget.BottomTransform.lossyScale);
						break;
				}
			}
			if (_skillEffect.EffectBehavior == eEffectBehavior.SERIES_FIREARM || (_skillEffect.EffectBehavior == eEffectBehavior.FIREARM && _skillEffect.TargetBone != eTargetBone.BOTTOM && global::OpenPCR.PreparedEffectInputs.HasParticle(this)))
			{
				vector = _target.GetBottomTransformPosition() + _target.GetFixedCenterPos();
			}
			base.transform.position += vector;
			eEffectBehavior effectBehavior = _skillEffect.EffectBehavior;
			if (effectBehavior == eEffectBehavior.FIREARM || effectBehavior == eEffectBehavior.SERIES_FIREARM)
			{
				_skillEffect.TrackType = eTrackType.NONE;
			}
			Vector3 absolutePosition = base.transform.position - _target.GetBottomTransformPosition();
			switch (_skillEffect.TrackType)
			{
				case eTrackType.BONE:
					switch (_skillEffect.TrackDimension)
					{
						case eTrackDimension.X:
							battleManager.StartCoroutine(TrackTarget(_target, absolutePosition, _followX: true, _followY: false, bone, _toadTrackType: _toadTrackType));
							break;
						case eTrackDimension.Y:
							battleManager.StartCoroutine(TrackTarget(_target, absolutePosition, _followX: false, _followY: true, bone, _toadTrackType: _toadTrackType));
							break;
						case eTrackDimension.XY:
							battleManager.StartCoroutine(TrackTarget(_target, absolutePosition, _followX: true, _followY: true, bone, _skillEffect.TrackRotation, _toadTrackType));
							break;
					}
					break;
				case eTrackType.BOTTOM:
					switch (_skillEffect.TrackDimension)
					{
						case eTrackDimension.X:
							battleManager.StartCoroutine(TrackTarget(_target, absolutePosition, _followX: true, _followY: false));
							break;
						case eTrackDimension.Y:
							battleManager.StartCoroutine(TrackTarget(_target, absolutePosition, _followX: false));
							break;
						case eTrackDimension.XY:
							battleManager.StartCoroutine(TrackTarget(_target, absolutePosition));
							break;
					}
					break;
				case eTrackType.BONE_TIME_LINE:
					{
						bool flag = _skill.SkillId == _owner.UnionBurstSkillId;
						AppendCoroutine(updateBoneTimeLine(_target, _skillEffect.TrackRotation), flag ? ePauseType.NO_DIALOG : ePauseType.VISUAL, flag ? null : _target.Owner);
						break;
					}
				case eTrackType.NONE:
					break;
			}
		}

		[global::OpenPCR.Migration("5c336779-354d-46db-a634-6aebc2f75861")]
		public virtual void StartUpdateDisappear(bool _isUnionBurst, BasePartsData _target)
		{
		}

		[global::OpenPCR.Migration("1d93f8b3-8a81-49a7-a37d-08cd26703142")]
		public IEnumerator TrackTarget(BasePartsData _trans, Vector3 _absolutePosition, bool _followX = true, bool _followY = true, Bone _bone = null, bool _trackRotation = false, eToadTrackType _toadTrackType = eToadTrackType.NONE)
		{
			trackTargetCoroutinueId++;
			int localId = trackTargetCoroutinueId;
			while (!(this == null) && _trans != null && !timeToDie && !(_trans.Owner == null) && localId == trackTargetCoroutinueId)
			{
				yield return null;
			}
		}

		[global::OpenPCR.Migration("f98aa77e-4722-4f15-9fae-a2d1ac04c2da")]
		private IEnumerator updateTimer(int _coroutineId)
		{
			float time = 0f;
			while (true)
			{
				yield return null;
				if (_coroutineId != currentCoroutineId || this == null)
				{
					yield break;
				}

				if (isPause)
				{
					continue;
				}

				if (Target != null && Target.ComponentState.IsDead)
				{
					if (OnEffectEnd != null)
					{
						OnEffectEnd(this);
					}

					if (!(this is FirearmCtrl))
					{
						timeToDie = true;
					}
				}

				float deltaTime = battleManager.DeltaTime;
				time += deltaTime;
				if (!(time < global::OpenPCR.PreparedEffectInputs.ParticleDuration(this)))
				{
					if (OnEffectEnd != null)
					{
						OnEffectEnd(this);
					}

					if (!(this is FirearmCtrl))
					{
						break;
					}
				}
			}

			timeToDie = true;
		}

		[global::OpenPCR.Migration("c4ee19c9-7aea-4a38-aca5-2790e50f1a27")]
		private IEnumerator UpdateTimerRepeat()
		{
			while (true)
			{
				yield return null;
				if (!isPause && Target != null && Target.ComponentState.IsDead)
				{
					if (OnEffectEnd != null)
					{
						OnEffectEnd(this);
					}

					if (!(this is FirearmCtrl))
					{
						timeToDie = true;
					}
				}
			}
		}

		[global::OpenPCR.Migration("7c18af6b-23e1-4fb4-ae26-4384651c2f9b")]
		public virtual bool _Update()
		{
			if (timeToDie)
			{
				IsPlaying = false;
				base.gameObject.SetActive(value: false);
			}

			return !timeToDie;
		}

		[global::OpenPCR.Migration("0d5f9566-1bbb-4803-9507-000e9fad4ce3")]
		public virtual void Pause()
		{
			if (particle != null)
			{
				particle.Pause(withChildren: true);
				resumeTime = particle.time;
				pauseSe(isPause: true);
			}

			for (int i = 0; i < TweenerList.Length; i++)
			{
				if (!global::OpenPCR.PreparedEffectInputs.IsRendererOnlyTweener(TweenerList[i])) TweenerList[i].enabled = false;
			}

			for (int j = 0; j < TweenRotList.Length; j++)
			{
				TweenRotList[j].enabled = false;
			}

			for (int k = 0; k < AnimatorList.Length; k++)
			{
				AnimatorList[k].enabled = false;
			}

			isPause = true;
		}

		[global::OpenPCR.Migration("5272e6f3-53ea-49c6-8184-1417e377e27a")]
		protected virtual void resetStartDelay()
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("4a7fcf09-03d2-4b45-b875-59245054d7fd")]
		public virtual void Resume()
		{
			if (particle != null && particle.isPaused && base.gameObject != null && base.gameObject.activeSelf)
			{
				particle.Play();
				resetStartDelay();
				pauseSe(isPause: false);
			}

			for (int i = 0; i < TweenerList.Length; i++)
			{
				if (!global::OpenPCR.PreparedEffectInputs.IsRendererOnlyTweener(TweenerList[i])) TweenerList[i].enabled = true;
			}

			for (int j = 0; j < TweenRotList.Length; j++)
			{
				TweenRotList[j].enabled = true;
			}

			for (int k = 0; k < AnimatorList.Length; k++)
			{
				AnimatorList[k].enabled = true;
			}

			isPause = false;
		}

		[global::OpenPCR.Migration("1d0642ba-cdf6-4e0e-a802-54521f4d756f")]
		protected virtual void initTweener(UITweener _tweener)
		{
			_tweener.ResetToBeginning();
			_tweener.PlayForward();
		}

		[global::OpenPCR.Migration("98a4a3a2-d60d-4a0b-80a0-1f64a08b4d2f")]
		public virtual void SetSortOrderBack()
		{
			IsFront = false;
			particleRendererDictionaryInitialize();
			foreach (KeyValuePair<Renderer, SortOrderAndOffset> item in particleRendererDictionary)
			{
				Renderer key = item.Key;
				if (item.Value.Offset != 0)
				{
					key.sortingOrder = BattleDefine.GetUnitSortOrder(SortTarget) + item.Value.Offset;
				}
				else if (item.Value.SortOrder < -1000)
				{
					key.sortingOrder = 690;
				}
				else if (item.Value.SortOrder <= 0)
				{
					key.sortingOrder = BattleDefine.GetUnitSortOrder(SortTarget) + -300;
				}
				else if (item.Value.SortOrder < 1000)
				{
					key.sortingOrder = BattleDefine.GetUnitSortOrder(SortTarget) + 300;
				}
				else
				{
					key.sortingOrder = 11000;
				}
			}
		}

		[global::OpenPCR.Migration("715f9e5e-f93b-490f-b5e3-22276c56be8c")]
		public virtual void SetSortOrderFront()
		{
			IsFront = true;
			particleRendererDictionaryInitialize();
			foreach (KeyValuePair<Renderer, SortOrderAndOffset> item in particleRendererDictionary)
			{
				Renderer key = item.Key;
				if (item.Value.Offset != 0)
				{
					key.sortingOrder = BattleDefine.GetUnitSortOrder(SortTarget) + item.Value.Offset;
				}
				else if (item.Value.SortOrder < -1000)
				{
					key.sortingOrder = 690;
				}
				else if (item.Value.SortOrder <= 0)
				{
					key.sortingOrder = BattleDefine.GetUnitSortOrder(SortTarget) + -300;
				}
				else if (item.Value.SortOrder < 1000)
				{
					key.sortingOrder = BattleDefine.GetUnitSortOrder(SortTarget) + 300;
				}
				else
				{
					key.sortingOrder = 11000;
				}

				key.sortingOrder += 11500;
			}
		}

		[global::OpenPCR.Migration("caf25d2e-a85b-4927-84d8-59a26bfb0122")]
		public void SetSortOrderStatus(int offset)
		{
			particleRendererDictionaryInitialize();
			foreach (KeyValuePair<Renderer, SortOrderAndOffset> item in particleRendererDictionary)
			{
				Renderer key = item.Key;
				if (item.Value.SortOrder < -1000)
				{
					key.sortingOrder = 690;
				}
				else if (item.Value.SortOrder < 0)
				{
					key.sortingOrder = BattleDefine.GetUnitSortOrder(SortTarget) + -300;
				}
				else if (item.Value.SortOrder < 1000)
				{
					key.sortingOrder = BattleDefine.GetUnitSortOrder(SortTarget) + 350 + offset;
				}
				else
				{
					key.sortingOrder = 11000;
				}
			}
		}

		[global::OpenPCR.Migration("9af0d3b1-1518-41c3-b52a-cb537e551a7d")]
		public void SetSortOrderHit(int _offset)
		{
			particleRendererDictionaryInitialize();
			foreach (KeyValuePair<Renderer, SortOrderAndOffset> item in particleRendererDictionary)
			{
				Renderer key = item.Key;
				if (item.Value.SortOrder < -1000)
				{
					key.sortingOrder = 690;
				}
				else if (item.Value.SortOrder < 0)
				{
					key.sortingOrder = BattleDefine.GetUnitSortOrder(SortTarget) + -300;
				}
				else if (item.Value.SortOrder < 1000)
				{
					key.sortingOrder = 11000 + _offset;
				}
				else
				{
					key.sortingOrder = 11000;
				}
			}
		}

		[global::OpenPCR.Migration("05791f6c-5e1f-4937-8e46-758fd7350aea")]
		protected void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit = null)
		{
			battleManager.AppendCoroutine(cr, pauseType, unit);
		}

		[global::OpenPCR.Migration("5501a949-dbd7-4c03-a2c1-fb3d52a4c9b4")]
		public void PlaySe(int soundUnitId, bool isEnemySide)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("13519fe0-67a1-469a-8ce8-c9c10d02c66d")]
		public void PlaySe(eSE se, bool isEnemySide)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("3a7935c9-e3f6-444a-9b91-df9a3c135d13")]
		public IEnumerator FadeoutSe(Action _callback, float _fadeSec)
		{
			CustomEasing easing = new CustomEasing(CustomEasing.eType.outQuad, 1f, 0f, _fadeSec);
			while (easing.IsMoving)
			{
				SeSource.volume = easing.GetCurVal(Time.deltaTime);
				yield return null;
			}
			_callback.Call();
		}

		[global::OpenPCR.Migration("0bfc404a-3b22-40f9-b5df-b7fb8e949d6c")]
		private void pauseSe(bool isPause)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("df3aaa53-ff79-4d61-b77e-3dd92ed5f1df")]
		public void SetSource(UnitCtrl _source)
		{
			source = _source;
		}

		[global::OpenPCR.Migration("eee18edb-bf9a-4658-860d-708944e6ab78")]
		public virtual void ResetParameter(GameObject _prefab, int _skinID = 0, bool _isShadow = false)
		{
			if (battleManager.IsDefenseReplayMode) throw new global::OpenPCR.OutOfBoundaryException("Defense effect position/texture input is outside the prepared clan/training scene.");
			timeToDie = false;
			IsPlaying = true;
			OnEffectEnd = null;
			pauseSe(isPause: false);
			base.transform.localPosition = _prefab.transform.localPosition;
		}

		[global::OpenPCR.Migration("db930f83-e39b-4988-9c32-7940d28a06f4")]
		public void RestartTween()
		{
			int i = 0;
			for (int num = TweenerList.Length; i < num; i++)
			{
				initTweener(TweenerList[i]);
			}

			int j = 0;
			for (int num2 = TweenRotList.Length; j < num2; j++)
			{
				TweenRotList[j].ResetToBeginning();
				TweenRotList[j].PlayForward();
			}
		}

		[global::OpenPCR.Migration("834f91d8-7a71-4585-87a9-739f58c2c899")]
		public void SetTimeToDie(bool value)
		{
			timeToDie = value;
		}

		[global::OpenPCR.Migration("a88ecaf0-f88e-4f92-a159-e683e059aebd")]
		public virtual void SetStartTime(float _starttime)
		{
			boneTimeLineStartTime = _starttime;
		}

		[global::OpenPCR.Migration("269920d2-de0e-4c3f-ac43-294575e57ccf")]
		public virtual void SetTimeToDieByStartHp(float _hpPercent)
		{
		}


		[global::OpenPCR.Migration("434fad7f-fdad-494f-8297-956ed4a37687")]
		private void Awake()
		{
			setBaseScale();
			awakeImpl();
		}

		[global::OpenPCR.Migration("6b8f00ba-8bf9-4e2d-ad35-18c516c0fcd4")]
		protected void awakeImpl()
		{
			if (staticSingletonTree == null)
			{
				staticSingletonTree = this.CreateSingletonTree();
				staticBattleManager = staticSingletonTree.Get<BattleManager>();
			}
		}

		[global::OpenPCR.Migration("91f614a3-e9d4-4b2d-9a89-9f46ab3062cc")]
		private void OnDestroy()
		{
			if (battleManager != null && battleManager.IsValid)
			{
				battleManager.RemoveEffectToUpdateList(this);
			}

			particle = null;
			TweenerList = null;
			TweenRotList = null;
			skillSeSource = null;
			source = null;
			textureChangeParticles = null;
			OnEffectEnd = null;
			SortTarget = null;
			Target = null;
			soundManager = null;
			particleRendererDictionary = null;
			onDestroy();
		}

		[global::OpenPCR.Migration("40b1d08e-6609-451d-89ee-05e0d97c5848")]
		protected virtual void onDestroy()
		{
		}

		[global::OpenPCR.Migration("c80878c8-acc1-470f-b957-37295602acf3")]
		public void SetLayerUI()
		{ transform.ChangeChildObjectLayer(LayerDefine.LAYER_UI); }

		[global::OpenPCR.Migration("d930788c-2115-40b4-8820-0000d27db3c2")]
		private IEnumerator updateBoneTimeLine(BasePartsData _partsData, bool _trackRotation)
		{
			BattleAnimKitController spine = SortTarget.GetCurrentSpineCtrl();
			if (boneTimeLine.Count == 0)
			{
				yield break;
			}
			Bone bone = spine.skeleton.FindBone(boneTimeLine[0].BoneName);
			bool hasNext = boneTimeLine.Count != 1;
			float timer = 0f;
			float nextTime = boneTimeLine[0].Time;
			int currentIndex = 0;
			while (true)
			{
				if (!isPause)
				{
					timer += battleManager.DeltaTime;
				}
				if (this == null || _partsData == null || timeToDie || _partsData.Owner == null)
				{
					break;
				}
				if (_trackRotation)
				{
					float num = ((bone.skeleton.flipX ^ bone.skeleton.flipY ^ (_partsData.Owner.BottomTransform.localScale.x < 0f)) ? (-1f) : 1f);
					Vector3 eulerAngles = base.transform.localRotation.eulerAngles;
					base.transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, bone.WorldRotationX * num);
				}
				base.transform.position = BattleUnitBaseAnimKitController.BoneWorldToGlobalPosConsiderRotate(bone, _partsData.Owner.RotateCenter, _partsData.Owner.BottomTransform.lossyScale);
				if (hasNext && timer >= nextTime)
				{
					currentIndex++;
					bone = spine.skeleton.FindBone(boneTimeLine[currentIndex].BoneName);
					nextTime = boneTimeLine[currentIndex].Time;
					hasNext = currentIndex < boneTimeLine.Count - 1;
				}
				if (boneTimeLineStartTime > 0f)
				{
					boneTimeLineStartTime -= battleManager.DeltaTime;
				}
				else
				{
					yield return null;
				}
			}
		}

		[global::OpenPCR.Migration("7414a0a3-4f54-4a86-b837-9f936df2f0f0")]
		public IEnumerator TrackTarget(BattleUnitBaseAnimKitController _spine, Vector3 _absolutePosition, bool _followX = true, bool _followY = true, Bone _bone = null, bool _trackRotation = false, Transform _traskScale = null, float _coefficient = 1f, bool _callFromBossDialog = false)
		{
			while (!(this == null) && !(_spine == null) && !timeToDie)
			{
				yield return null;
			}
		}

		[global::OpenPCR.Migration("6aa3d089-abd4-4da8-a6f5-3f8093d74233")]
		public IEnumerator TrackTargetSort(UnitCtrl _unit, bool _allowSortOrderFront = true)
		{
			bool startDepthBack = _unit.IsDepthBack;
			while (!(this == null) && !(_unit == null) && !timeToDie)
			{
				if (startDepthBack && !_unit.IsDepthBack)
				{
					if (_allowSortOrderFront) SetSortOrderFront();
					startDepthBack = false;
				}
				if (_unit.ComponentSortOrder.IsFront)
				{
					if (!IsFront && _allowSortOrderFront) SetSortOrderFront();
				}
				else if (IsFront) SetSortOrderBack();
				yield return null;
			}
		}

		[global::OpenPCR.Migration("2943e253-edd4-4576-bd73-3e69633ec903")]
		public void SetSortOrderForSummon(int _sortOrder)
		{
			particleRendererDictionaryInitialize();
			foreach (KeyValuePair<Renderer, SortOrderAndOffset> item in particleRendererDictionary)
			{
				Renderer key = item.Key;
				if (item.Value.SortOrder < -1000)
				{
					key.sortingOrder = 690;
				}
				else if (item.Value.SortOrder <= 0)
				{
					key.sortingOrder = _sortOrder + -300;
				}
				else if (item.Value.SortOrder < 1000)
				{
					key.sortingOrder = _sortOrder + 300;
				}
				else
				{
					key.sortingOrder = 11000;
				}
			}
		}

		[global::OpenPCR.Migration("2a409b35-7441-4a24-ad74-6e33a0385581")]
		private void setBaseScale()
		{
			BaseScale = transform.localScale;
		}

		[global::OpenPCR.Migration("099c1715-32f7-48ed-8098-21aa4b7586b2")]
		public IEnumerator TrackTargetSortForSummon(UnitCtrl _unitCtrl)
		{
			int currentDepth = 0;
			while (!(_unitCtrl == null) && !(_unitCtrl.GetCurrentSpineCtrl() == null))
			{
				if (currentDepth != _unitCtrl.GetCurrentSpineCtrl().Depth)
				{
					currentDepth = _unitCtrl.GetCurrentSpineCtrl().Depth;
					SetSortOrderForSummon(currentDepth);
				}
				yield return null;
				if (!IsPlaying)
				{
					break;
				}
			}
		}

		[global::OpenPCR.Migration("343327e0-3b90-44f5-8a81-a4fa377d7e14")]
		public virtual void OnAwakeWhenSkipCutIn()
		{
		}
	}
}
