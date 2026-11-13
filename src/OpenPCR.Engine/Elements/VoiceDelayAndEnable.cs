using System;

namespace Elements
{
	[Serializable]
	public class VoiceDelayAndEnable
	{
		public float Delay;
		public bool Enable = true;
		[global::OpenPCR.Migration("e619aa4d-a9a2-4a6e-b569-4f2111ac1107")]
		public VoiceDelayAndEnable()
		{
		}
	}
}
