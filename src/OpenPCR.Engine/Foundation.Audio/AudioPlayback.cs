using global::OpenPCR.Foundation.Audio;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using global::OpenPCR.AudioStubs;

namespace OpenPCR.Foundation.Audio
{
	[StructLayout((LayoutKind)0, Pack = 1, Size = 48)]
	public struct AudioPlayback
	{

		[global::OpenPCR.Migration("4c9160d9-692c-4eb9-96bf-c867a6ae8938")]
		public static AudioPlayback Error(SoundGroup soundGroup)
		{
			return global::OpenPCR.Presentation.Noop<AudioPlayback>();
		}
	}
}
