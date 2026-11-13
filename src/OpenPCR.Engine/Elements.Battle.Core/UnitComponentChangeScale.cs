using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UnitComponentChangeScale : UnitComponentBase, IUnitComponent
	{
		private int changeScaleId;
		[global::OpenPCR.Migration("88480713-a3ac-4902-b52f-5a668ad9c6dd")]
		public UnitComponentChangeScale(UnitCtrl _unitController) : base(_unitController)
		{
		}

		[global::OpenPCR.Migration("14bb1289-0391-4fe8-8dc1-add4bc41387a")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
		}

		[global::OpenPCR.Migration("7988c768-e18a-452c-a6e8-2e3232d74e55")]
		public void StartScaleChange(List<ScaleChanger> _scaleChangers, float _startTime, float _blackoutTime)
		{
			if (_scaleChangers.Count != 0)
			{
				Vector3 defaultScale = new Vector3((float)((!unitController.IsLeftDir && !unitController.IsForceLeftDir) ? 1 : (-1)) * unitController.Scale, Mathf.Abs(unitController.Scale));
				unitController.AppendCoroutine(updateScaleChange(++changeScaleId, _scaleChangers, _startTime, defaultScale, _blackoutTime), ePauseType.NO_DIALOG);
			}
		}

		[global::OpenPCR.Migration("7b47e86b-47c4-4db2-ba5e-5a8bc03dca46")]
		public void StopScaleChange()
		{
			Vector3 defaultScale = new Vector3((!unitController.IsLeftDir && !unitController.IsForceLeftDir ? 1f : -1f) * unitController.Scale, Mathf.Abs(unitController.Scale));
			ScaleChangeValue = 1f;
			setScale(defaultScale, 1f);
			changeScaleId++;
		}

		[global::OpenPCR.Migration("6b700052-6cbe-49a5-a812-2590c68685a0")]
		private IEnumerator updateScaleChange(int _changeScaleId, List<ScaleChanger> _scaleChangers, float _startTime, Vector3 _defaultScale, float _blackoutTime)
		{
			float timer = 0f;
			int currentIndex = 0;
			ScaleChanger currentData = _scaleChangers[currentIndex];
			CustomEasing currentEasing = new CustomEasing(currentData.Easing, 1f, currentData.Scale, currentData.Duration);
			while (true)
			{
				if (unitController.GetIsPrincessFormUnionBurstRunning())
				{
					yield return null;
					continue;
				}
				if (_changeScaleId != changeScaleId)
				{
					yield break;
				}
				timer += unitController.GetBattleDeltaTime();
				if (timer < currentData.Time - currentData.Duration)
				{
					if (timer >= _startTime)
					{
						yield return null;
					}
					continue;
				}
				if (timer < currentData.Time)
				{
					ScaleChangeValue = currentEasing.GetCurVal(unitController.GetBattleDeltaTime());
					setScale(_defaultScale, ScaleChangeValue);
					if (timer >= _startTime)
					{
						yield return null;
					}
					continue;
				}
				ScaleChangeValue = currentData.Scale;
				setScale(_defaultScale, ScaleChangeValue);
				currentIndex++;
				if (_scaleChangers.Count == currentIndex || timer > _blackoutTime + _startTime)
				{
					break;
				}
				float scale = currentData.Scale;
				currentData = _scaleChangers[currentIndex];
				currentEasing = new CustomEasing(currentData.Easing, scale, currentData.Scale, currentData.Duration);
			}
			while (true)
			{
				if (_changeScaleId != changeScaleId)
				{
					yield break;
				}
				if (timer > _blackoutTime + _startTime)
				{
					break;
				}
				yield return null;
				timer += unitController.GetBattleDeltaTime();
			}
			ScaleChangeValue = 1f;
			setScale(_defaultScale, 1f);
		}

		public bool IsScaleChangeTarget { get; private set; }
		public float ScaleChangeValue { get; private set; } = 1f;

		[global::OpenPCR.Migration("5e8638e3-6dab-4a4f-bd68-6c72676d019c")]
		public void SetScaleChangeTarget(bool _isTarget)
		{
			IsScaleChangeTarget = _isTarget;
		}

		[global::OpenPCR.Migration("74144857-c5dc-4597-8862-df98f593d0eb")]
		private void setScale(Vector3 _defaultScale, float _scale = 1f)
		{
			unitController.GetCurrentSpineCtrl().transform.localScale = _defaultScale * _scale;
			foreach (var effect in unitController.AuraEffectList) effect.transform.localScale = effect.BaseScale * _scale;
		}
	}
}
