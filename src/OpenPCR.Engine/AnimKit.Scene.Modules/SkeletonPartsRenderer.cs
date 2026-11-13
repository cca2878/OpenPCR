using global::OpenPCR.Runtime.Api;

namespace OpenPCR.AnimKit.Scene.Modules
{
	public class SkeletonPartsRenderer : MonoBehaviour
	{

		public MeshRenderer MeshRenderer
		{
			[global::OpenPCR.Migration("39676a05-481d-4f26-ac23-f8d917f7ff15")]
			get
			{ throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.AnimKit.Scene.Modules.SkeletonPartsRenderer::get_MeshRenderer():OpenPCR.Runtime.Api.MeshRenderer: runtime capability is unavailable."); }
		}

		public SkeletonPartsRenderer()
		{ throw new global::OpenPCR.MigrationPendingException("OpenPCR.AnimKit.Scene.Modules.SkeletonPartsRenderer::.ctor()", "Behavior implementation is not available."); }
	}
}
