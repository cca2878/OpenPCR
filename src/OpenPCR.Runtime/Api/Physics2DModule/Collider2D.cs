using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	public class Collider2D : Behaviour
	{
		[global::OpenPCR.Migration("42fa810e-f80b-4339-afd7-7b9e0941ea9c")]
		public Collider2D() { }
		public Bounds bounds
		{
			[global::OpenPCR.Migration("e9f8b0c7-e189-4a09-944d-c4fd91ce8dde")]
			get { var input = global::OpenPCR.Runtime.Impl.PreparedColliderBoundsX.Read(this); return new Bounds { center = new Vector3(input.Center.X, input.Center.Y, input.Center.Z), extents = new Vector3(input.Extents.X, input.Extents.Y, input.Extents.Z) }; }
		}
	}
	public sealed class PolygonCollider2D : Collider2D
	{
		[global::OpenPCR.Migration("c29c2d27-015a-4c84-93b3-a38763c64c42")]
		public PolygonCollider2D() { }
	}
	public sealed class Rigidbody2D : Component
	{
		[global::OpenPCR.Migration("b11a116f-67ce-413e-ad37-6d84ba5be174")]
		public Rigidbody2D() { }
	}
}
