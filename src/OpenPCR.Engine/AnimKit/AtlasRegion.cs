namespace OpenPCR.AnimKit
{
	public class AtlasRegion
	{
		public string name;
		public int x;
		public int y;
		public int width;
		public int height;
		public float u;
		public float v;
		public float u2;
		public float v2;
		public float offsetX;
		public float offsetY;
		public int originalWidth;
		public int originalHeight;
		public int index;
		public bool rotate;
		public AtlasRegion()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.AnimKit.AtlasRegion::.ctor()", "body migration pending");
		}
	}
}
