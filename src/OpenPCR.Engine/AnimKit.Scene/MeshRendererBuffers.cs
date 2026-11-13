using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using System;

namespace OpenPCR.AnimKit.Scene
{
	public class MeshRendererBuffers : IDisposable
	{
		internal readonly ExposedList<Material> submeshMaterials = new ExposedList<Material>();
		internal Material[] sharedMaterials = new Material[0];
		[global::OpenPCR.Migration("d7f69702-23ee-467e-b6eb-7112e65fb3fb")]
		public MeshRendererBuffers()
		{
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.AnimKit.Scene.MeshRendererBuffers::.ctor(): runtime capability is unavailable.");
		}

		public void Dispose()
		{
			throw new OpenPCR.MigrationPendingException("OpenPCR.AnimKit.Scene.MeshRendererBuffers::Dispose():System.Void", "structure diagnostic remains after declaration migration");
		}
	}
}
