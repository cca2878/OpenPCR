using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public sealed class WaitWhile : global::OpenPCR.Runtime.Api.CustomYieldInstruction
	{
		private readonly global::System.Func<bool> m_Predicate;
		private readonly double m_MaxExecutionTime = -1;

		[global::OpenPCR.Migration("a371c6a5-3892-4708-b0b9-3e41544bbcc0")]
		public WaitWhile(global::System.Func<bool> predicate)
		{
			m_Predicate = predicate;
		}
		public override bool keepWaiting
		{
			[global::OpenPCR.Migration("9de56eab-a71a-4ee8-96b3-06c1c568010a")]
			get
			{
				if (m_MaxExecutionTime == -1)
					return m_Predicate();
				throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Runtime.Api.WaitWhile::get_keepWaiting():System.Boolean: runtime capability is unavailable.");
			}
		}
	}
}
