using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 4)]
	public sealed class WaitForSeconds : global::OpenPCR.Runtime.Api.YieldInstruction
	{
		internal float m_Seconds;

		[global::OpenPCR.Migration("8fcac93b-f6c0-4b0b-93e9-07a8ad50f6c5")]
		public WaitForSeconds(float seconds)
		{
			m_Seconds = seconds;
		}
	}
}
