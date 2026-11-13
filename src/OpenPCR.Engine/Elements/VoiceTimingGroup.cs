using System;

namespace Elements
{
	[Serializable]
	public class VoiceTimingGroup
	{
		public int VoiceGroupId;
		public VoiceTimingSet ConstantVelocity;
		public VoiceTimingSet DoubleSpeed;
		public VoiceTimingGroup(int _VoiceGroupId, VoiceTimingSet _ConstantVelocity, VoiceTimingSet _DoubleSpeed)
		{
			VoiceGroupId = _VoiceGroupId;
			ConstantVelocity = _ConstantVelocity;
			DoubleSpeed = _DoubleSpeed;
		}
	}
}
