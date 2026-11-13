using System;
using System.Collections.Generic;

namespace Elements
{
	[Serializable]
	public class SubUbVoiceTimingSet
	{
		public List<VoiceDelayAndEnable> Voice1List = new List<VoiceDelayAndEnable>();
		public List<VoiceDelayAndEnable> Voice2List = new List<VoiceDelayAndEnable>();
		[global::OpenPCR.Migration("9b2055fa-8b59-43e4-afcf-e16da2632072")]
		public SubUbVoiceTimingSet()
		{
		}
	}
}
