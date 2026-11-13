using global::Elements;
using global::Elements.Battle;
using global::OpenPCR.AnimKit;
using global::OpenPCR.ImageEffects;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle
{
	public class BattleCameraEffect : IBattleCameraEffectForDamageEffectCtrl, IBattleCameraEffectForBlurEffect, IBattleCameraEffectForAnimKitCowFireArm, IBattleCameraEffectForUnitActionController, IBattleCameraEffectForUnitCtrl, IBattleCameraEffectForPartsBossTotalDamage, IBattleCameraEffectForBattleProcessor
	{
		private int currentZoomId;
		public float BattleCameraUbScale { get; set; }
		private IBattleManagerForCameraEffect battleManager;
		private MotionBlur motionBlur;
		private BlurOptimized playCameraBlurOptimized;
		private Camera playCamera;
		private List<ShakeEffect> currentShake = new List<ShakeEffect>();
		private Dictionary<UnitCtrl, bool> zoomStopFlagDic = new Dictionary<UnitCtrl, bool>();
		private UnitCtrl currentZoomUnit;
		public Vector3 ShakePosition { get; set; }
		public Vector3 ZoomPos { get; set; }
		public float CurrentZoom { get; set; }

		public float BattleCameraWaveScale { get; set; }
		public float BossBattleScale { get; set; }

		public bool CameraEffectStopFlag { get; set; }

		[global::OpenPCR.Migration("019d3627-26d1-412b-9433-04a5bdfc399b")]
		public BattleCameraEffect(IBattleManagerForCameraEffect _battleManager, Camera _playCamera, MotionBlur _motionBlur, BlurOptimized _playCameraBlurOptimized)
		{
			battleManager = _battleManager;
			playCamera = _playCamera;
			motionBlur = _motionBlur;
			playCameraBlurOptimized = _playCameraBlurOptimized;
		}

		[global::OpenPCR.Migration("e1ac440f-f44a-49f1-859c-d43b45a3eb34")]
		public void StartShake(ShakeEffect shake, Skill skill, UnitCtrl unit = null)
		{
			currentShake.Add(shake);
			shake.ResetStart(skill, unit);
		}

		[global::OpenPCR.Migration("e4219529-8bff-43b8-83fd-67dc77563301")]
		public void ClearShake()
		{
			currentShake.Clear();
		}

		[global::OpenPCR.Migration("b877e58b-49df-4e72-9d00-d650955ddf73")]
		public void UpdateShake()
		{
			if (currentShake.Count == 0)
			{
				return;
			}

			float num = 0f;
			for (int num2 = currentShake.Count - 1; num2 >= 0; num2--)
			{
				ShakeEffect shakeEffect = currentShake[num2];
				bool num3 = shakeEffect.Simulate(battleManager.DeltaTime);
				if (num <= shakeEffect.CurrentAmplutude)
				{
					num = shakeEffect.CurrentAmplutude;
					ShakePosition = shakeEffect.CurrentShakePos;
				}

				if (num3)
				{
					currentShake.RemoveAt(num2);
				}

				if (shakeEffect.GetOwnerPause())
				{
					ShakePosition = Vector3.zero;
				}
			}

			setPlayCameraPosition();
		}

		[global::OpenPCR.Migration("1df125cb-ca77-4a81-ae32-b19d48efdcec")]
		private void setPlayCameraPosition()
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("5198b0bc-5d6b-47c4-8410-dc4bcb78afc7")]
		public void StartZoomEffect(BattleDefine.ZoomEffect _zoomEffect, UnitCtrl _unit, float _startTime, bool _ignorePause = false, bool _useStartCoroutine = false, bool _isNotUb = true)
		{
			if (currentZoomUnit == _unit && currentZoomId == _unit.ComponentUnionBurst.Counter)
			{
				return;
			}
			currentZoomId = _unit.ComponentUnionBurst.Counter;
			currentZoomUnit = _unit;
			TurnOffZoomStopFlag(_unit);
			float num = (float)Screen.height / (float)Screen.width;
			bool isOtherNotShadow = _unit.IsOther && !_unit.IsShadow;
			float bossBattleScale = BattleCameraWaveScale;
			if (bossBattleScale != 1f && !isOtherNotShadow)
			{
				CurrentZoom = 2.0444443f * (float)Screen.height / (float)Screen.width;
			}
			if ((bossBattleScale != 1f && isOtherNotShadow) & _isNotUb)
			{
				CurrentZoom = 2.0444443f * num * bossBattleScale;
			}
			float num2 = 2.0444443f * num * (isOtherNotShadow ? (_isNotUb ? bossBattleScale : BattleCameraUbScale) : 1f);
			float lJLDNIECKDO = ((_unit.IsLeftDir || _unit.IsForceLeftDirOrPartsBoss) ? (-_zoomEffect.OffsetX) : _zoomEffect.OffsetX);
			float cameraCenterPosXFromTargetZoomAndUnit = getCameraCenterPosXFromTargetZoomAndUnit(_zoomEffect.To, lJLDNIECKDO, _unit, _zoomEffect.ZoomTarget, _zoomEffect.ConsiderScreenSize, _zoomEffect.BoneName);
			float cameraCenterPosYFromTargetZoomAndUnit = getCameraCenterPosYFromTargetZoomAndUnit(_zoomEffect.To, _zoomEffect.OffsetY, _unit, _zoomEffect.ZoomTarget, _zoomEffect.ConsiderScreenSize, _zoomEffect.UseYBottom, _zoomEffect.BoneName);
			_zoomEffect.StartPosYEasing = new CustomEasing(_zoomEffect.StartEasingType, ZoomPos.y, cameraCenterPosYFromTargetZoomAndUnit, _zoomEffect.StartDuration);
			_zoomEffect.StartPosXEasing = new CustomEasing(_zoomEffect.StartEasingType, ZoomPos.x, cameraCenterPosXFromTargetZoomAndUnit, _zoomEffect.StartDuration);
			_zoomEffect.ToPosX = cameraCenterPosXFromTargetZoomAndUnit;
			_zoomEffect.ToPosY = cameraCenterPosYFromTargetZoomAndUnit;
			_zoomEffect.StartEasing = new CustomEasing(_zoomEffect.StartEasingType, CurrentZoom, _zoomEffect.To * num2, _zoomEffect.StartDuration);
			List<BattleDefine.ZoomEffectPlual> zoomEffectList = _zoomEffect.ZoomEffectList;
			if (zoomEffectList.Count == 0)
			{
				_zoomEffect.EndPosYEasing = new CustomEasing(_zoomEffect.EndEasingType, cameraCenterPosYFromTargetZoomAndUnit, 0f, _zoomEffect.EndDuration);
				_zoomEffect.EndEasing = new CustomEasing(_zoomEffect.EndEasingType, num2 * _zoomEffect.To, num2, _zoomEffect.EndDuration);
				_zoomEffect.EndPosXEasing = new CustomEasing(_zoomEffect.EndEasingType, cameraCenterPosXFromTargetZoomAndUnit, 0f, _zoomEffect.EndDuration);
			}
			float to = _zoomEffect.To;
			int i = 0;
			for (int count = zoomEffectList.Count; i < count; i++)
			{
				BattleDefine.ZoomEffectPlual zoomEffectPlual = zoomEffectList[i];
				float beginValue = cameraCenterPosXFromTargetZoomAndUnit;
				float beginValue2 = cameraCenterPosYFromTargetZoomAndUnit;
				float lJLDNIECKDO2 = ((_unit.IsLeftDir || _unit.IsForceLeftDirOrPartsBoss) ? (-zoomEffectPlual.OffsetX) : zoomEffectPlual.OffsetX);
				cameraCenterPosXFromTargetZoomAndUnit = getCameraCenterPosXFromTargetZoomAndUnit(zoomEffectPlual.To, lJLDNIECKDO2, _unit, zoomEffectPlual.ZoomTarget, _zoomEffect.ConsiderScreenSize, zoomEffectPlual.BoneName);
				cameraCenterPosYFromTargetZoomAndUnit = getCameraCenterPosYFromTargetZoomAndUnit(zoomEffectPlual.To, zoomEffectPlual.OffsetY, _unit, zoomEffectPlual.ZoomTarget, _zoomEffect.ConsiderScreenSize, zoomEffectPlual.UseYBottom, zoomEffectPlual.BoneName);
				zoomEffectPlual.StartPosXEasing = new CustomEasing(zoomEffectPlual.StartEasingType, beginValue, cameraCenterPosXFromTargetZoomAndUnit, zoomEffectPlual.StartDuration);
				zoomEffectPlual.StartPosYEasing = new CustomEasing(zoomEffectPlual.StartEasingType, beginValue2, cameraCenterPosYFromTargetZoomAndUnit, zoomEffectPlual.StartDuration);
				zoomEffectPlual.ToPosX = cameraCenterPosXFromTargetZoomAndUnit;
				zoomEffectPlual.ToPosY = cameraCenterPosYFromTargetZoomAndUnit;
				zoomEffectPlual.StartEasing = new CustomEasing(zoomEffectPlual.StartEasingType, num2 * to, num2 * zoomEffectPlual.To, zoomEffectPlual.StartDuration);
				to = zoomEffectPlual.To;
				if (i == count - 1)
				{
					_zoomEffect.EndPosYEasing = new CustomEasing(_zoomEffect.EndEasingType, cameraCenterPosYFromTargetZoomAndUnit, 0f, _zoomEffect.EndDuration);
					_zoomEffect.EndEasing = new CustomEasing(_zoomEffect.EndEasingType, num2 * _zoomEffect.To, num2, _zoomEffect.EndDuration);
					_zoomEffect.EndPosXEasing = new CustomEasing(_zoomEffect.EndEasingType, cameraCenterPosXFromTargetZoomAndUnit, 0f, _zoomEffect.EndDuration);
				}
			}
			if (_useStartCoroutine)
			{
				battleManager.StartCoroutineIgnoreFps(updateZoomEffect(_zoomEffect, _unit, _startTime, currentZoomId));
			}
			else if (_ignorePause)
			{
				battleManager.AppendCoroutine(updateZoomEffect(_zoomEffect, _unit, _startTime, currentZoomId), ePauseType.NO_DIALOG, null);
			}
			else
			{
				battleManager.AppendCoroutine(updateZoomEffect(_zoomEffect, _unit, _startTime, currentZoomId), ePauseType.VISUAL, _unit);
			}
		}

		[global::OpenPCR.Migration("b41caf74-95a2-4530-9707-45a1cd4cf6b3")]
		public void TurnOffZoomStopFlag(UnitCtrl _zoomUnit)
		{
			List<UnitCtrl> list = new List<UnitCtrl>(zoomStopFlagDic.Keys);
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				zoomStopFlagDic[list[i]] = true;
			}

			if (zoomStopFlagDic.ContainsKey(_zoomUnit))
			{
				zoomStopFlagDic[_zoomUnit] = false;
			}
			else
			{
				zoomStopFlagDic.Add(_zoomUnit, value: false);
			}
		}

		[global::OpenPCR.Migration("86b357d9-ed45-4515-83c2-b0bb419f7f53")]
		public bool JudgeStopEndZoom(UnitCtrl _unit)
		{
			bool value;
			return zoomStopFlagDic.TryGetValue(_unit, out value) & value;
		}

		[global::OpenPCR.Migration("fa36b9da-d1c5-48f3-b965-fcb0d0900b65")]
		public void StopZoomEffect(UnitCtrl _unitCtrl)
		{
			if (currentZoomUnit == _unitCtrl)
			{
				_unitCtrl.ComponentUnionBurst.IncrementCounter();
				currentZoomId = _unitCtrl.ComponentUnionBurst.Counter;
			}
			CurrentZoom = 2.0444443f * (float)Screen.height / (float)Screen.width * (_unitCtrl.IsOther ? BattleCameraWaveScale : 1f);
			playCamera.orthographicSize = CurrentZoom;
			ZoomPos = Vector3.zero;
			setPlayCameraPosition();
		}

		[global::OpenPCR.Migration("1b9e4c61-e6b3-46e3-b568-8b2055851de2")]
		public void StartBlurEffect(BlurEffect.BlurEffectData blurData)
		{
			battleManager.AppendCoroutine(updateBlurEffect(blurData), ePauseType.IGNORE_BLACK_OUT, null);
		}

		[global::OpenPCR.Migration("4bb6ad3a-e9ca-4696-8b0a-9a3bcf838480")]
		private IEnumerator updateBlurEffect(BlurEffect.BlurEffectData blurData)
		{
			CustomEasing startEase = new CustomEasing(CustomEasing.eType.outQuad, 0f, blurData.Value, blurData.StartDuration);
			CustomEasing endEase = new CustomEasing(CustomEasing.eType.outQuad, blurData.Value, 0f, blurData.EndDuration);
			float time = 0f;
			while (true)
			{
				time += battleManager.DeltaTime;
				_ = startEase.GetCurVal(battleManager.DeltaTime);
				if (time > blurData.StartDuration)
				{
					break;
				}

				yield return null;
			}

			time = 0f;
			while (true)
			{
				time += battleManager.DeltaTime;
				if (time > blurData.Duration)
				{
					break;
				}

				yield return null;
			}

			time = 0f;
			while (true)
			{
				time += battleManager.DeltaTime;
				_ = endEase.GetCurVal(battleManager.DeltaTime);
				if (time > blurData.StartDuration)
				{
					break;
				}

				yield return null;
			}

		}

		[global::OpenPCR.Migration("718d13dc-262f-45db-8252-28e99832ca98")]
		public void SetBgBlur(bool _isBlur)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BattleCameraEffect::SetBgBlur(System.Boolean):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("4e407dfd-a26d-4cb1-816f-c56e0cae1a95")]
		public void Release()
		{
			battleManager = null;
			playCamera = null;
			motionBlur = null;
			playCameraBlurOptimized = null;
			currentShake = null;
			zoomStopFlagDic = null;
			currentZoomUnit = null;
		}

		[global::OpenPCR.Migration("8570725b-04c7-4d15-8540-9bd690ed3e06")]
		private bool judgeStopZoom(UnitCtrl _unit, int _currentZoomId)
		{
			if (!zoomStopFlagDic[_unit])
			{
				return _currentZoomId != currentZoomId;
			}
			return true;
		}

		[global::OpenPCR.Migration("b5418894-1e27-4cf1-b0c6-f39dc8b231bf")]
		private IEnumerator updateZoomEffect(BattleDefine.ZoomEffect _zoomEffect, UnitCtrl _unit, float _startTime, int _currentZoomId)
		{
			float time = 0f;
			while (true)
			{
				if (judgeStopZoom(_unit, _currentZoomId))
				{
					yield break;
				}
				float deltaTimeForPause = _unit.DeltaTimeForPause;
				time += deltaTimeForPause;
				_startTime -= deltaTimeForPause;
				if (time > _zoomEffect.StartDelay)
				{
					break;
				}
				if (_startTime <= 0f)
				{
					yield return null;
				}
			}
			time = 0f;
			while (true)
			{
				float deltaTimeForPause2 = _unit.DeltaTimeForPause;
				if (judgeStopZoom(_unit, _currentZoomId))
				{
					yield break;
				}
				time += deltaTimeForPause2;
				_startTime -= deltaTimeForPause2;
				CurrentZoom = _zoomEffect.StartEasing.GetCurVal(deltaTimeForPause2);
				playCamera.orthographicSize = CurrentZoom;
				if (time > _zoomEffect.StartDuration)
				{
					break;
				}
				ZoomPos = new Vector3(_zoomEffect.StartPosXEasing.GetCurVal(deltaTimeForPause2), _zoomEffect.StartPosYEasing.GetCurVal(deltaTimeForPause2), 0f);
				setPlayCameraPosition();
				if (_startTime <= 0f)
				{
					yield return null;
				}
			}
			ZoomPos = new Vector3(_zoomEffect.ToPosX, _zoomEffect.ToPosY, 0f);
			setPlayCameraPosition();
			time = 0f;
			while (true)
			{
				float deltaTimeForPause3 = _unit.DeltaTimeForPause;
				if (judgeStopZoom(_unit, _currentZoomId))
				{
					yield break;
				}
				time += deltaTimeForPause3;
				_startTime -= deltaTimeForPause3;
				if (time > _zoomEffect.Duration)
				{
					break;
				}
				setPlayCameraPosition();
				if (_startTime <= 0f)
				{
					yield return null;
				}
			}
			int i = 0;
			for (int count = _zoomEffect.ZoomEffectList.Count; i < count; i++)
			{
				time = 0f;
				BattleDefine.ZoomEffectPlual zoom = _zoomEffect.ZoomEffectList[i];
				while (true)
				{
					float deltaTimeForPause4 = _unit.DeltaTimeForPause;
					if (judgeStopZoom(_unit, _currentZoomId))
					{
						yield break;
					}
					time += deltaTimeForPause4;
					_startTime -= deltaTimeForPause4;
					CurrentZoom = zoom.StartEasing.GetCurVal(deltaTimeForPause4);
					playCamera.orthographicSize = CurrentZoom;
					if (time > zoom.StartDuration)
					{
						break;
					}
					ZoomPos = new Vector3(zoom.StartPosXEasing.GetCurVal(deltaTimeForPause4), zoom.StartPosYEasing.GetCurVal(deltaTimeForPause4), 0f);
					setPlayCameraPosition();
					if (_startTime <= 0f)
					{
						yield return null;
					}
				}
				ZoomPos = new Vector3(zoom.ToPosX, zoom.ToPosY, 0f);
				setPlayCameraPosition();
				time = 0f;
				while (true)
				{
					float deltaTimeForPause5 = _unit.DeltaTimeForPause;
					if (judgeStopZoom(_unit, _currentZoomId))
					{
						yield break;
					}
					time += deltaTimeForPause5;
					_startTime -= deltaTimeForPause5;
					if (time > zoom.Duration)
					{
						break;
					}
					setPlayCameraPosition();
					if (_startTime <= 0f)
					{
						yield return null;
					}
				}
			}
			time = 0f;
			while (true)
			{
				float deltaTimeForPause6 = _unit.DeltaTimeForPause;
				if (judgeStopZoom(_unit, _currentZoomId))
				{
					yield break;
				}
				time += deltaTimeForPause6;
				_startTime -= deltaTimeForPause6;
				CurrentZoom = _zoomEffect.EndEasing.GetCurVal(deltaTimeForPause6);
				playCamera.orthographicSize = CurrentZoom;
				ZoomPos = new Vector3(_zoomEffect.EndPosXEasing.GetCurVal(deltaTimeForPause6), _zoomEffect.EndPosYEasing.GetCurVal(deltaTimeForPause6), 0f);
				setPlayCameraPosition();
				if (time > _zoomEffect.EndDuration)
				{
					break;
				}
				if (_startTime <= 0f)
				{
					yield return null;
				}
			}
			currentZoomUnit = null;
			CurrentZoom = 2.0444443f * (float)Screen.height / (float)Screen.width * (_unit.IsOther && !_unit.IsShadow ? BattleCameraWaveScale : 1f);
			ZoomPos = Vector3.zero;
			setPlayCameraPosition();
		}

		[global::OpenPCR.Migration("9c19953a-ffa6-4dcf-b4e7-ce7a733b7c8d")]
		private float getCameraCenterPosXFromTargetZoomAndUnit(float _targetZoom, float _offsetX, UnitCtrl _unit, eZoomTarget _zoomTarget, bool _considerScreenSize, string _boneName)
		{ return global::OpenPCR.Presentation.Noop<float>(); }

		private float getCameraCenterPosXFromTargetZoomAndUnitImpl(float _targetZoom, float _offsetX, UnitCtrl _unit, eZoomTarget _zoomTarget, bool _considerScreenSize, string _boneName)
		{
			switch (_zoomTarget)
			{
				case eZoomTarget.ENEMY_TARGET:
				case eZoomTarget.O_AND_ET:
				case eZoomTarget.PLAYER_TARGET:
				case eZoomTarget.O_AND_PT:
					{
						float num = 0f;
						int num2 = 0;
						int blackoutUnitTargetLength = battleManager.GetBlackoutUnitTargetLength();
						for (int i = 0; i < blackoutUnitTargetLength; i++)
						{
							UnitCtrl blackoutUnitTarget = battleManager.GetBlackoutUnitTarget(i);
							if (!getUnitIsZoomTarget(_unit, _zoomTarget, blackoutUnitTarget))
							{
								continue;
							}
							if (!blackoutUnitTarget.IsPartsBoss)
							{
								num += blackoutUnitTarget.transform.position.x;
								num2++;
								continue;
							}
							for (int j = 0; j < blackoutUnitTarget.ComponentMultiPartsBoss.BossPartsListForBattle.Count; j++)
							{
								if (blackoutUnitTarget.ComponentMultiPartsBoss.BossPartsListForBattle[j].IsBlackoutTarget)
								{
									num += blackoutUnitTarget.ComponentMultiPartsBoss.BossPartsListForBattle[j].GetPosition().x;
									num2++;
								}
							}
						}
						if (num2 != 0)
						{
							num /= (float)num2;
						}
						if (_zoomTarget == eZoomTarget.ENEMY_TARGET || _zoomTarget == eZoomTarget.PLAYER_TARGET)
						{
							return num;
						}
						return (num + _unit.transform.position.x) / 2f;
					}
				case eZoomTarget.O_BONE:
					{
						Bone bone = _unit.GetCurrentSpineCtrl().skeleton.FindBone(_boneName);
						if (bone != null)
						{
							return BattleUnitBaseAnimKitController.BoneWorldToGlobalPosConsiderRotate(bone, _unit.RotateCenter, _unit.BottomTransform.lossyScale).x;
						}
						return _unit.transform.position.x;
					}
				default:
					return _unit.transform.position.x;
			}
		}

		private static bool getUnitIsZoomTarget(UnitCtrl _unit, eZoomTarget _zoomTarget, UnitCtrl unit)
		{
			return unit.IsOther != _unit.IsOther == (_zoomTarget == eZoomTarget.O_AND_ET || _zoomTarget == eZoomTarget.ENEMY_TARGET);
		}

		[global::OpenPCR.Migration("a77a4a08-cc83-4f03-b80e-31d38e08fc58")]
		private float getCameraCenterPosYFromTargetZoomAndUnit(float _targetZoom, float _offsetY, UnitCtrl _unit, eZoomTarget _zoomTarget, bool _considerScreenSize, bool _useBottom, string _boneName)
		{ return global::OpenPCR.Presentation.Noop<float>(); }
	}
}
