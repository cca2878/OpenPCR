using global::OpenPCR.AnimKit.Scene.Modules;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace OpenPCR.AnimKit.Scene.Modules
{

	public class SkeletonRenderSeparator : MonoBehaviour
	{
		public bool copyPropertyBlock = true;
		public bool copyMeshRendererFlags = true;
		public List<SkeletonPartsRenderer> partsRenderers = new List<SkeletonPartsRenderer>();

		public SkeletonRenderSeparator()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.AnimKit.Scene.Modules.SkeletonRenderSeparator::.ctor()", "body migration pending");
		}
	}
}
