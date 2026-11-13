using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System.Collections;

namespace Elements
{
	public class PartsHeaderHpGauge : MonoBehaviour
	{
		[SerializeField]
		private UISlider lateSlider;
		[SerializeField]
		private float gaugeDuration = 0.2f;
		[SerializeField]
		private float redGaugeDuration = 0.5f;
		private float toSliderValue;
		private Transform gaugeParticleParent;
		private ParticleSystem gaugeParticle;
		public UISlider TargetSlider { get; private set; }

		private UISlider targetLateSlider;
		private Coroutine gaugeUpdateCoroutine;
		private Coroutine redGaugeUpdateCoroutine;
		public PartsHeaderHpGauge()
		{
		}


		public void PlaySliderAnimation(float _toValue)
		{
			if (toSliderValue != _toValue)
			{
				toSliderValue = _toValue;
				stopGaugeUpdate();
				gaugeUpdateCoroutine = StartCoroutine(updateGauge(TargetSlider.value, toSliderValue));
			}
		}

		private IEnumerator updateGauge(float _fromValue, float _toValue)
		{
			float time = 0f;
			setGaugeParticleEnable(true);
			while (!(time > gaugeDuration))
			{
				yield return null;
				time += Time.deltaTime;
				float fraction = time / gaugeDuration;
				float factor = fraction <= 1f ? fraction : 1f;
				if (fraction < 0f)
					factor = 0f;
				float value = _fromValue + (_toValue - _fromValue) * factor;
				TargetSlider.value = value;
				setGaugeParticlePos(value);
			}

			setGaugeParticleEnable(false);
			stopRedGaugeUpdate();
			redGaugeUpdateCoroutine = StartCoroutine(updateGaugeRed(lateSlider.value, _toValue));
			stopGaugeUpdate();
		}

		private void stopGaugeUpdate()
		{
			if (gaugeUpdateCoroutine != null)
			{
				StopCoroutine(gaugeUpdateCoroutine);
				gaugeUpdateCoroutine = null;
			}
		}

		private IEnumerator updateGaugeRed(float _fromValue, float _toValue)
		{
			while (gaugeUpdateCoroutine != null)
				yield return null;
			float time = 0f;
			while (!(time > redGaugeDuration))
			{
				yield return null;
				time += Time.deltaTime;
				float fraction = time / redGaugeDuration;
				float factor = fraction <= 1f ? fraction : 1f;
				if (fraction < 0f)
					factor = 0f;
				targetLateSlider.value = _fromValue + (_toValue - _fromValue) * factor;
			}

			stopRedGaugeUpdate();
		}

		private void stopRedGaugeUpdate()
		{
			if (redGaugeUpdateCoroutine != null)
			{
				StopCoroutine(redGaugeUpdateCoroutine);
				redGaugeUpdateCoroutine = null;
			}
		}

		private void setGaugeParticlePos(float _rate)
		{
			float sliderX = TargetSlider.transform.localPosition.x;
			int width = TargetSlider.foregroundWidget.width;
			Vector3 position = gaugeParticleParent.localPosition;
			float halfWidth = (float)width * 0.5f;
			float startX = sliderX - halfWidth + 8f;
			float endX = sliderX + halfWidth;
			float factor = _rate <= 1f ? _rate : 1f;
			if (_rate < 0f)
				factor = 0f;
			gaugeParticleParent.localPosition = new Vector3(startX + factor * (endX - startX), position.y, position.z);
		}

		private void setGaugeParticleEnable(bool _enable)
		{
			if (_enable)
				gaugeParticle.Play(true);
			else
			{
				gaugeParticle.Clear(true);
				gaugeParticle.Stop(true);
			}
		}
	}
}
