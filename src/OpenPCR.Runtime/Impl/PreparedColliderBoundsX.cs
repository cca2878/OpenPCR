using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;
using System.Runtime.CompilerServices;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>X-only AABB consumer projection with explicit narrowing and operation order.</summary>
	public static class PreparedColliderBoundsX
	{
		private sealed record Shape(object Transform, float Center, float HalfWidth);
		private static readonly ConditionalWeakTable<object, Shape> shapes = new();
		public static void Bind(object collider, object transform, ReadOnlySpan<float> scaledAttachmentVertices)
		{
			if (scaledAttachmentVertices.Length < 6 || scaledAttachmentVertices.Length % 2 != 0) throw new ArgumentException("Unweighted polygon requires complete x/y vertices.");
			if (!ReferenceEquals(SceneScripting.Transform(collider), transform)) throw new ArgumentException("Collider geometry must use its actual owning Transform.");
			float lo = scaledAttachmentVertices[0], hi = lo;
			for (int i = 2; i < scaledAttachmentVertices.Length; i += 2) { float x = scaledAttachmentVertices[i]; if (x < lo) lo = x; if (x > hi) hi = x; }
			shapes.Add(collider, new(transform, (lo + hi) * 0.5f, (hi - lo) * 0.5f));
		}
		public static (EngineVector Center, EngineVector Extents) Read(object collider)
		{
			if (!shapes.TryGetValue(collider, out var shape)) throw new InvalidOperationException("Collider has no prepared X-bound resource.");
			_ = SceneScripting.Transform(collider);
			var position = PreparedTransformInputs.Position(shape.Transform); var scale = PreparedTransformInputs.Scale(shape.Transform);
			return (new(position.X + shape.Center * scale.X, position.Y, position.Z), new(shape.HalfWidth * Math.Abs(scale.X), 0f, 0f));
		}
		public static void Release(object collider) => shapes.Remove(collider);
	}

}
