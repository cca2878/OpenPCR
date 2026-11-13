using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle;

namespace Elements
{
	public class DamageEffectCtrlBase : MonoBehaviour, ISingletonField
	{
		[SerializeField]
		private UIPanel uiPanel;
		[SerializeField]
		private Animation serializedAnimation;
		private float defaultScale = -1f;
		private static Yggdrasil<DamageEffectCtrlBase> staticSingletonTree = null;
		private static IBattleManagerForDamageEffectCtrlBase staticBattleManager = null;
		private static IBattleCameraEffectForDamageEffectCtrl staticBattleCameraEffect = null;
		private static IBattleTimeScaleForDamageEffectCtrlBase staticBattleTimeScale = null;
		private Vector3 offset { get; set; }
		public bool IsPlaying { get; private set; }
		private bool isPause { get; set; }
		private float timescale { get; set; }
		private int dummySortingOrder { get; set; }
		protected IBattleManagerForDamageEffectCtrlBase battleManager => staticBattleManager;
		protected IBattleCameraEffectForDamageEffectCtrl battleCameraEffect => staticBattleCameraEffect;
		protected IBattleTimeScaleForDamageEffectCtrlBase battleTimeScale => staticBattleTimeScale;

		public UnitCtrl Source { get; private set; }
		public DamageEffectCtrlBase()
		{

		}


		[global::OpenPCR.Migration("cb181d74-3fef-47cd-87d9-2f3dffb5d93c")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
			staticBattleCameraEffect = null;
			staticBattleTimeScale = null;
		}

		[global::OpenPCR.Migration("deca607d-7aa5-4045-8aa3-d760851e3886")]
		public void SetPosition(BasePartsData _unitctrl, Vector3 _offset, float _scale)
		{
			offset = _offset;
			StartCoroutine(updateStability(_unitctrl, _scale));
		}

		private IEnumerator updateStability(BasePartsData _unitctrl, float _scale)
		{
			BattleHeaderController battleHeaderController = SingletonMonoBehaviour<BattleHeaderController>.Instance;
			while (IsPlaying)
			{
				if (_unitctrl == null)
				{
					IsPlaying = false;
					break;
				}
				if (battleManager.IsPrincessSkillMovieFirstHalfRunning)
				{
					dummySortingOrder = 11499;
					uiPanel.sortingOrder = 11499;
				}
				if (!battleHeaderController.IsPaused && battleManager.IsPauseFrameCount && dummySortingOrder > 11500)
				{
					dummySortingOrder = 11499;
					uiPanel.sortingOrder = 11499;
				}
				bool flag = (battleManager.BlackoutUnitTargetList.Count != 0 && dummySortingOrder < 11500) || battleManager.IsPauseFrameCount || battleManager.HideDamageNumFlag || battleManager.IsPrincessSkillMovieRunning;
				if (flag)
				{
					base.transform.localScale = Vector3.one * defaultScale * _scale;
				}
				else
				{
					base.transform.localScale = Vector3.one * defaultScale * _scale / battleCameraEffect.CurrentZoom;
				}
				base.transform.position = _unitctrl.GetBottomTransformPosition() + _unitctrl.GetFixedCenterPos() + offset + (flag ? battleCameraEffect.ShakePosition : (-battleCameraEffect.ZoomPos + new Vector3(0f, -9.259259f, 0f)));
				if (!flag)
				{
					base.transform.position *= 1f / battleCameraEffect.CurrentZoom;
					if (battleManager.IsDefenseReplayMode)
					{
						base.transform.SetWorldPosX(-base.transform.position.x);
					}
				}
				else if (battleManager.IsDefenseReplayMode)
				{
					Vector3 localScale = base.transform.localScale;
					localScale.x = -localScale.x;
					base.transform.localScale = localScale;
				}
				if (flag && !isPause && battleManager.GameState == eBattleGameState.PLAY)
				{
					isPause = true;
					foreach (AnimationState item in serializedAnimation)
					{
						item.speed = 0f;
					}
				}
				else if (timescale != battleTimeScale.BaseTimeScale || (isPause && !flag) || battleManager.GameState != eBattleGameState.PLAY)
				{
					isPause = false;
					timescale = battleTimeScale.BaseTimeScale;
					foreach (AnimationState item2 in serializedAnimation)
					{
						if (timescale != 0f)
						{
							item2.speed = 1f / timescale;
						}
					}
				}
				yield return null;
			}
		}

		[global::OpenPCR.Migration("b912b782-a68a-43fa-b3c9-a3d18409b99b")]
		public void SetSortOrder(int order)
		{
			dummySortingOrder = order;
			if (dummySortingOrder > 11500)
			{
				order -= 11500;
			}

			uiPanel.sortingOrder = order;
		}

		public void SetSource(UnitCtrl _source)
		{
			Source = _source;
		}

		public void DestroySelf()
		{
			IsPlaying = false;
			base.gameObject.SetActive(value: false);
		}

		public virtual void ResetParameter(GameObject prefab)
		{
			if (defaultScale < 0f)
			{
				defaultScale = base.transform.localScale.x;
			}
			base.transform.localPosition = prefab.transform.localPosition;
			offset = Vector3.zero;
			IsPlaying = true;
			if (serializedAnimation != null)
			{
				serializedAnimation.Play();
				StartCoroutine(updateAnimation());
			}
		}

		private IEnumerator updateAnimation()
		{
			yield return new WaitWhile(() => serializedAnimation.isPlaying);
			DestroySelf();
		}

		private void Awake()
		{
			if (staticSingletonTree == null)
			{
				staticSingletonTree = this.CreateSingletonTree();
				staticBattleManager = staticSingletonTree.Get<BattleManager>();
				staticBattleCameraEffect = staticSingletonTree.Get<BattleCameraEffect>();
				staticBattleTimeScale = staticSingletonTree.Get<BattleTimeScale>();
			}
		}
	}
}
