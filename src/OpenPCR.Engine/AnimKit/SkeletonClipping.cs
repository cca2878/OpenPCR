using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class SkeletonClipping
	{
		internal readonly Triangulator triangulator = new Triangulator();
		internal readonly ExposedList<float> clippingPolygon = new ExposedList<float>();
		internal readonly ExposedList<float> clipOutput = new ExposedList<float>(128);
		internal readonly ExposedList<float> clippedVertices = new ExposedList<float>(128);
		internal readonly ExposedList<int> clippedTriangles = new ExposedList<int>(128);
		internal readonly ExposedList<float> clippedUVs = new ExposedList<float>(128);
		internal readonly ExposedList<float> scratch = new ExposedList<float>();

		public SkeletonClipping()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.AnimKit.SkeletonClipping::.ctor()", "body migration pending");
		}
	}
}
