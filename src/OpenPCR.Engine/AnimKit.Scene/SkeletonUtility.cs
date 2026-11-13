using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
using global::OpenPCR.Runtime.Api;
namespace OpenPCR.AnimKit.Scene
{
	public class SkeletonUtility : global::OpenPCR.Runtime.Api.MonoBehaviour
	{
		public SkeletonUtility()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Unity skeletal collider/render component construction requires the resource backend.");
		}
		[global::OpenPCR.Migration("46752adb-6ceb-46f3-b31d-16d32ab6f9d7")]
		public static global::OpenPCR.Runtime.Api.PolygonCollider2D AddBoundingBoxAsComponent(global::OpenPCR.AnimKit.BoundingBoxAttachment box, global::OpenPCR.AnimKit.Slot slot, global::OpenPCR.Runtime.Api.GameObject gameObject, bool isTrigger = true, bool isKinematic = true, float gravityScale = 0f)
		{
			if (box == null) return null;
			return (global::OpenPCR.Runtime.Api.PolygonCollider2D)global::OpenPCR.PreparedColliderInputs.Create(box, slot, gameObject, isTrigger, isKinematic, gravityScale);
		}
	}
}
