using System;
using System.Collections.Generic;

namespace Elements
{
	[Serializable]
	public class VoiceTimingSet
	{
		public VoiceDelayAndEnable Cutin;
		public VoiceDelayAndEnable Voice1;
		public VoiceDelayAndEnable Voice2;
		public VoiceDelayAndEnable Voice3;
		public VoiceDelayAndEnable Voice4;
		public VoiceTimingSet(VoiceDelayAndEnable _Cutin, VoiceDelayAndEnable _Voice1, VoiceDelayAndEnable _Voice2, VoiceDelayAndEnable _Voice3, VoiceDelayAndEnable _Voice4)
		{
			Cutin = new VoiceDelayAndEnable
			{
				Delay = _Cutin.Delay,
				Enable = _Cutin.Enable
			};
			Voice1 = new VoiceDelayAndEnable
			{
				Delay = _Voice1.Delay,
				Enable = _Voice1.Enable
			};
			Voice2 = new VoiceDelayAndEnable
			{
				Delay = _Voice2.Delay,
				Enable = _Voice2.Enable
			};
			Voice3 = new VoiceDelayAndEnable
			{
				Delay = _Voice3.Delay,
				Enable = _Voice3.Enable
			};
			Voice4 = new VoiceDelayAndEnable
			{
				Delay = _Voice4.Delay,
				Enable = _Voice4.Enable
			};
		}
	}
}
