using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class PathAttachment : VertexAttachment
	{
		internal float[] lengths;
		internal bool closed;
		internal bool constantSpeed;

		[global::OpenPCR.Migration("41ad23c6-9feb-4eb5-ac26-795b58cd61eb")]
		public PathAttachment(string name) : base(name)
		{
		}
	}
}
