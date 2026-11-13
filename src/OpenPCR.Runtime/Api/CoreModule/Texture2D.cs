using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public sealed class Texture2D : global::OpenPCR.Runtime.Api.Texture
	{
		[global::OpenPCR.Migration("4876a1f1-0484-463e-9d84-dd75f4a368d9")]
		public Texture2D(int width, int height)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Texture2D::.ctor(System.Int32,System.Int32)", "Unity infrastructure not implemented");
		}
	}
}
