using System.Runtime.InteropServices;
namespace Elements
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct CutinVoiceStatus
	{
		public bool IsUseSpecialVoice { get; private set; }
		public VoiceGroupIdSet GroupIdSet { get; private set; }
		public VoiceTimingSet TimingSet { get; private set; }
		[global::OpenPCR.Migration("7522bb60-05b2-4c7c-a213-867d43c756d4")]
		public CutinVoiceStatus(bool _IsUseSpecialVoice, VoiceGroupIdSet _GroupIdSet, VoiceTimingSet _TimingSet)
		{
			IsUseSpecialVoice = _IsUseSpecialVoice;
			GroupIdSet = _GroupIdSet;
			TimingSet = _TimingSet;
		}
	}
}
