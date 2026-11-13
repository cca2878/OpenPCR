using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 8)]
	public class ScriptableObject : global::OpenPCR.Runtime.Api.Object
	{
		[global::OpenPCR.Migration("6ed2ba72-4a41-4cc2-98be-96f0db453815")]
		public ScriptableObject()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.ScriptableObject::.ctor()", "Unity infrastructure not implemented");
		}
	}
}
