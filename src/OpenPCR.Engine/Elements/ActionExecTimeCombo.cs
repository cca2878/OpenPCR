using global::OpenPCR.Runtime.Api;
using System;
using Elements.Battle.Core;

namespace Elements
{
	[Serializable]
	public class ActionExecTimeCombo
	{
		public float StartTime;
		public float OffsetTime;
		public float Weight = 1.0f;
		public int Count;
		public eComboInterporationType InterporationType;
		public AnimationCurve Curve = new AnimationCurve();
		[global::OpenPCR.Migration("0f879cf2-63ac-4c81-85b0-b4e2ba794602")]
		public ActionExecTimeCombo()
		{
		}
	}

}
