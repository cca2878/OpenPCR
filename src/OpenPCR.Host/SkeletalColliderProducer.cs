using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
using System;
using Elements;

namespace OpenPCR.Host;

/// <summary>Unit-root collision input whose temporary components are consumed and removed during initialization.</summary>
internal static class SkeletalColliderProducer
{
	public static void Bind(GameObject owner)
	{
		PreparedColliderInputs.Bind(owner, (attachment, sourceSlot, trigger, kinematic, gravity) =>
		{
			var box = (BoundingBoxAttachment)attachment; var slot = (Slot)sourceSlot;
			if (trigger || gravity != 0f) throw new NotSupportedException("Only the first-phase initialization collider consumer is projected.");
			if (RuntimeBinding.Get(box, "bones") is not null) throw new NotSupportedException("Weighted collision attachments require their Native world/local mechanism.");
			var bone = (Bone)RuntimeBinding.Get(slot, "bone")!; var skeleton = (Skeleton)RuntimeBinding.Get(bone, "skeleton")!;
			if (!ReferenceEquals(bone, ((ExposedList<Bone>)RuntimeBinding.Get(skeleton, "bones")!).Items[0]) && owner.GetComponent<Rigidbody2D>() is null) owner.AddComponent<Rigidbody2D>();
			var collider = owner.AddComponent<PolygonCollider2D>();
			PreparedColliderBoundsX.Bind(collider, owner.transform, (float[])RuntimeBinding.Get(box, "vertices")!);
			return collider;
		});
	}
}
