using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
namespace OpenPCR.AnimKit.Scene
{
	public class SkeletonRendererInstruction
	{
		public readonly ExposedList<SubmeshInstruction> submeshInstructions = new ExposedList<SubmeshInstruction>();
		public int rawVertexCount = -1;
		public readonly ExposedList<Attachment> attachments = new ExposedList<Attachment>();
		[global::OpenPCR.Migration("f43dd035-cb03-44c7-a175-8717713e9d6c")]
		public SkeletonRendererInstruction()
		{
			throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.AnimKit.Scene.SkeletonRendererInstruction::.ctor(): runtime capability is unavailable.");
		}

		public void Set(SkeletonRendererInstruction other)
		{
			throw new OpenPCR.MigrationPendingException("OpenPCR.AnimKit.Scene.SkeletonRendererInstruction::Set(OpenPCR.AnimKit.Scene.SkeletonRendererInstruction):System.Void", "structure diagnostic remains after declaration migration");
		}
	}
}
