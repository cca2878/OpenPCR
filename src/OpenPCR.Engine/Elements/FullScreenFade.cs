using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;

namespace Elements
{
	public class FullScreenFade : MonoBehaviour
	{
		[SerializeField]
		private TweenAlpha fadeTween;
		public float Duration => fadeTween.duration;

		public FullScreenFade()
		{
		}



		public void FadeOut(bool _resetsInitialAlpha = true, Action _onFinished = null)
		{
			fadeTween.SetOnFinished(() => _onFinished.Call());
			if (_resetsInitialAlpha)
				fadeTween.ResetToBeginning();
			fadeTween.PlayForward();
		}
	}
}
