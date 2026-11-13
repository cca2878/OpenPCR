using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public sealed class WaitUntil : global::OpenPCR.Runtime.Api.CustomYieldInstruction
	{
		private readonly global::System.Func<bool> m_Predicate;
		private readonly double m_MaxExecutionTime = -1;

		[global::OpenPCR.Migration("088069c8-f3fd-4b36-bcc9-b672f09d0f24")]
		public WaitUntil(global::System.Func<bool> predicate)
		{
			m_Predicate = predicate;
		}
		public override bool keepWaiting
		{
			[global::OpenPCR.Migration("7bf2128f-4aae-410f-8046-8c4ab935baa9")]
			get
			{
				if (m_MaxExecutionTime == -1)
					return !m_Predicate();
				throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Runtime.Api.WaitUntil::get_keepWaiting():System.Boolean: runtime capability is unavailable.");
			}
		}
	}
}
