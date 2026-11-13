using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class ProtectionData
	{
		public float Timer;
		private readonly List<eActionType> NoneEffectActionList;
		[global::OpenPCR.Migration("96415072-0027-413c-a769-15139a06a153")]
		public ProtectionData()
		{
			NoneEffectActionList = new List<eActionType>
			{
				(eActionType)28,
				(eActionType)23
			};
			Counter = -1;
			EffectPrefabForProtectionPartsList = new List<AbnormalStateEffectPrefabData>();
		}

		[global::OpenPCR.Migration("6493109b-14fc-4aa5-b594-75aba2e0eef9")]
		public void StartTimer()
		{
			Target.AppendCoroutine(updateTimer(), ePauseType.SYSTEM, null);
		}

		[global::OpenPCR.Migration("17a370de-df42-41e5-a8ac-0853bf859753")]
		private IEnumerator updateTimer()
		{
			while (Enable)
			{
				UnitCtrl protector = ProtectionParts.Owner;
				if (Target.ComponentState.IdleOnly || Target.ComponentState.IsDead || Timer <= 0f || protector.ComponentState.IdleOnly || protector.ComponentState.IsDead)
				{
					Disable();
					yield break;
				}

				Timer -= Target.DeltaTimeForPause;
				yield return null;
			}
		}

		[global::OpenPCR.Migration("4682216a-f000-43a1-b9b1-56ea2ffe04dd")]
		public void Disable()
		{
			Timer = 0f;
			Enable = false;
			DisableIcon();
		}

		[global::OpenPCR.Migration("7fc11fee-63f6-46ac-b5c2-0140fc670cbc")]
		public void StartEffect(UnitCtrl _target, IBattleEffectPool _pool, IBattleManagerForUnitActionController _battleManager, eActionType _actionType)
		{
			if (_battleManager.ChargeSkillTurn != eChargeSkillTurn.NONE || NoneEffectActionList.Contains(_actionType) || effectCounter >= EffectMaxCount)
			{
				return;
			}

			effectCounter++;
			ProtectionData data = _target.ComponentAbnormal.ProtectionTargetData;
			Action<GameObject, BasePartsData, eTargetBone, eTrackType> start = (_effectPrefab, _effectTarget, _targetBone, _trackType) =>
			{
				if (_effectPrefab == null)
				{
					return;
				}

				bool shadow = !_pool.ForceNormalSD && Source.IsShadow;
				SkillEffectCtrl effect = _pool.GetEffect(_effectPrefab, null);
				effect.ResetParameter(_effectPrefab, UnitUtility.GetSkinId(Source.ComponentSound.SoundUnitId, Source.SDSkin), shadow);
				effect.InitializeSort();
				effect.PlaySe(Source.ComponentSound.SoundUnitId, _effectTarget.Owner.IsLeftDir);
				global::OpenPCR.AnimKit.Bone bone = null;
				switch (_targetBone)
				{
					case eTargetBone.BOTTOM:
						effect.transform.position = _effectTarget.Owner.transform.position;
						break;
					case eTargetBone.HEAD:
						bone = _effectTarget.GetStateBone();
						effect.transform.position = BattleUnitBaseAnimKitController.BoneWorldToGlobalPosConsiderRotate(_effectTarget.GetStateBone(), _effectTarget.Owner.RotateCenter, _effectTarget.GetBottomLossyScale(), false);
						break;
					case eTargetBone.CENTER:
						bone = _effectTarget.GetCenterBone();
						effect.transform.position = BattleUnitBaseAnimKitController.BoneWorldToGlobalPosConsiderRotate(_effectTarget.GetCenterBone(), _effectTarget.Owner.RotateCenter, _effectTarget.GetBottomLossyScale(), false);
						break;
					case eTargetBone.FIXED_CENETER:
						effect.transform.position = _effectTarget.GetBottomTransformPosition() + _effectTarget.GetFixedCenterPos();
						break;
				}

				Vector3 offset = effect.transform.position - _effectTarget.GetBottomTransformPosition();
				if (_trackType == eTrackType.BONE)
				{
					_target.StartCoroutine(effect.TrackTarget(_effectTarget, offset, true, true, bone, false, SkillEffectCtrl.eToadTrackType.NONE));
				}
				else if (_trackType == eTrackType.BOTTOM)
				{
					_target.StartCoroutine(effect.TrackTarget(_effectTarget, offset, true, true, null, false, SkillEffectCtrl.eToadTrackType.NONE));
				}

				effect.transform.parent = ExceptUIRoot.Transform;
				effect.SortTarget = _effectTarget.Owner;
				effect.ExecAppendCoroutine(_effectTarget.Owner, true);
				_battleManager.StartCoroutine(effect.TrackTargetSort(_effectTarget.Owner, true));
			};
			start(_target.IsLeftDir ? data.EffectPrefabLeft : data.EffectPrefab, _target.ComponentMultiPartsBoss.GetFirstParts(false, 0f), eTargetBone.BOTTOM, eTrackType.NONE);
			foreach (AbnormalStateEffectPrefabData prefab in data.EffectPrefabForProtectionPartsList)
			{
				start(data.ProtectionParts.Owner.IsLeftDir ? prefab.LeftEffectPrefab : prefab.RightEffectPrefab, data.ProtectionParts, prefab.TargetBone, prefab.TrackType);
			}

			_battleManager.AppendCoroutine(UpdateEffect(_battleManager.DeltaTime), ePauseType.IGNORE_BLACK_OUT, null);
		}

		[global::OpenPCR.Migration("343301b2-4f5c-4b47-8340-44a2e9c5d039")]
		public IEnumerator UpdateEffect(float _deltaTime)
		{
			float timer = EffectDuration;
			while (true)
			{
				timer -= _deltaTime;
				if (timer < 0f)
				{
					break;
				}

				yield return null;
			}

			effectCounter--;
		}

		public UnitCtrl Source { get; set; }
		public eStateIconType IconType { get; set; }
		public bool Enable { get; set; }
		public int Counter { get; set; }
		public BasePartsData ProtectionParts { get; set; }
		public UnitCtrl Target { get; set; }
		public GameObject EffectPrefab { get; set; }
		public GameObject EffectPrefabLeft { get; set; }
		public List<AbnormalStateEffectPrefabData> EffectPrefabForProtectionPartsList { get; set; }
		public int EffectMaxCount { get; set; }
		public float EffectDuration { get; set; }
		private int effectCounter { get; set; }
		public bool IsEnchantProtect { get; set; }

		[global::OpenPCR.Migration("809dfe4b-559b-460d-88da-dedb2d90b186")]
		public bool HasCountLimit
		{
			[global::OpenPCR.Migration("3d2acdf6-255f-443d-9c65-5f81fb3b588b")]
			get
			{
				return Counter != -1;
			}
		}

		[global::OpenPCR.Migration("ed958e6f-5542-49c7-8537-84cbbb26bf8d")]
		public void DisableIcon()
		{
			UnitCtrl owner = IsEnchantProtect ? ProtectionParts.Owner : Target;
			owner.ComponentAbnormal.OnChangeState?.Invoke(IconType, false);
		}

		[global::OpenPCR.Migration("130a848f-dc04-44d5-8a22-47be0bf4903d")]
		public void EnableIcon()
		{
			UnitCtrl owner = IsEnchantProtect ? ProtectionParts.Owner : Target;
			owner.ComponentAbnormal.OnChangeState?.Invoke(IconType, true);
		}

		[global::OpenPCR.Migration("3d9c2a58-003d-4466-93bf-c0fb610c14a2")]
		public void UpdateIconCount()
		{
			if (HasCountLimit)
			{
				UnitCtrl owner = IsEnchantProtect ? ProtectionParts.Owner : Target;
				owner.ComponentAbnormal.OnChangeStateNum?.Invoke(IconType, Counter);
			}
		}

		[global::OpenPCR.Migration("a43595b6-3fe3-47fe-a1b5-8d639e216882")]
		public void DisableIfOwnerDead()
		{
			if (Enable && (ProtectionParts.Owner.ComponentState.IdleOnly || (long)ProtectionParts.Owner.ComponentParameter.Hp <= 0L))
			{
				Disable();
			}
		}

		[global::OpenPCR.Migration("3563f34e-ea35-4cec-a39f-b89e94a89693")]
		public bool TryExec()
		{
			if (!Enable)
			{
				return false;
			}

			UnitComponentAbnormal abnormal = ProtectionParts.Owner.ComponentAbnormal;
			if (abnormal.IsUnableActionState() || abnormal.IsConfusionOrConvert() || abnormal.ToadDatas.Count > 0)
			{
				return false;
			}

			if (HasCountLimit)
			{
				Counter--;
				if (Counter == 0)
				{
					Disable();
				}
				else
				{
					UpdateIconCount();
				}
			}

			return true;
		}
	}
}
