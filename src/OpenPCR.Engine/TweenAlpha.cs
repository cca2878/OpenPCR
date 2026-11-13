using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;

namespace OpenPCR.UICompat
{

	public class TweenAlpha : UITweener
	{
		public float from = 1f;
		public float to = 1f;
		protected override void OnUpdate(float factor, bool isFinished)
		{
			// Intentional projection: Native only applies from/to interpolation to UI alpha.
			// UITweener retains the actual clock, enabled state and completion dispatch.
		}



		public float value
		{
			get
			{
				throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.UICompat.TweenAlpha::get_value():System.Single: runtime capability is unavailable.");
			}

			set
			{
				throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.UICompat.TweenAlpha::set_value(System.Single):System.Void: runtime capability is unavailable.");
			}
		}
	}

}
