using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public class WaitForSecondsRealtime : global::OpenPCR.Runtime.Api.CustomYieldInstruction
	{
		[global::OpenPCR.Migration("99bc7211-8858-40d0-820f-47c6fc294f29")]
		public WaitForSecondsRealtime(float time)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.WaitForSecondsRealtime::.ctor(System.Single)", "Unity infrastructure not implemented");
		}
		public override bool keepWaiting
		{
			[global::OpenPCR.Migration("f99281fe-af3d-4c9e-b837-86529e089c08")]
			get
			{
				throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.WaitForSecondsRealtime::get_keepWaiting():System.Boolean", "Unity infrastructure not implemented");
			}
		}
	}
}
