using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 8)]
	public sealed class Coroutine : global::OpenPCR.Runtime.Api.YieldInstruction
	{
		[global::OpenPCR.Migration("5e9488c5-2f62-440e-97e0-282a112a3fd8")]
		private Coroutine()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Coroutine::.ctor()", "Unity infrastructure not implemented");
		}
	}
}
