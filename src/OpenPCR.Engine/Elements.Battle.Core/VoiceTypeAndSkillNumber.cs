using System.Runtime.InteropServices;

namespace Elements.Battle.Core
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
	public struct VoiceTypeAndSkillNumber
	{
		public SoundManager.eVoiceType VoiceType;
		public int SkillNumber;
	}
}
