using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 16)]
	public class AsyncOperation : global::OpenPCR.Runtime.Api.YieldInstruction
	{
		[global::OpenPCR.Migration("b41329bd-8fa2-4548-872d-881bac9d721d")]
		protected AsyncOperation(nint ptr)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.AsyncOperation::.ctor(System.IntPtr)", "Unity infrastructure not implemented");
		}
	}
}
