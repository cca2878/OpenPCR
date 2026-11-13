using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;
using System.Collections.Generic;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>Prepared pose boundary; authored identity rotations and the proved live planar rotation consumers.</summary>
	public readonly struct EngineVector : IEquatable<EngineVector>
	{
		public float X { get; init; }
		public float Y { get; init; }
		public float Z { get; init; }
		public EngineVector(float X, float Y, float Z) { this.X = X; this.Y = Y; this.Z = Z; }
		public bool Equals(EngineVector other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
		public override bool Equals(object? value) => value is EngineVector other && Equals(other);
		public override int GetHashCode()
		{
			unchecked
			{
				var hash = EqualityComparer<float>.Default.GetHashCode(X);
				hash = hash * -1521134295 + EqualityComparer<float>.Default.GetHashCode(Y);
				hash = hash * -1521134295 + EqualityComparer<float>.Default.GetHashCode(Z);
				return hash;
			}
		}
		public override string ToString() => new System.Text.StringBuilder("EngineVector { ").Append("X = ").Append(X).Append(", ").Append("Y = ").Append(Y).Append(", ").Append("Z = ").Append(Z).Append(" }").ToString();
		public static bool operator ==(EngineVector left, EngineVector right) => left.Equals(right);
		public static bool operator !=(EngineVector left, EngineVector right) => !left.Equals(right);
		public void Deconstruct(out float X, out float Y, out float Z) { X = this.X; Y = this.Y; Z = this.Z; }
	}
	public readonly struct EngineQuaternion : IEquatable<EngineQuaternion>
	{
		public float X { get; init; }
		public float Y { get; init; }
		public float Z { get; init; }
		public float W { get; init; }
		public EngineQuaternion(float X, float Y, float Z, float W) { this.X = X; this.Y = Y; this.Z = Z; this.W = W; }
		public bool Equals(EngineQuaternion other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z) && W.Equals(other.W);
		public override bool Equals(object? value) => value is EngineQuaternion other && Equals(other);
		public override int GetHashCode()
		{
			unchecked
			{
				var hash = EqualityComparer<float>.Default.GetHashCode(X);
				hash = hash * -1521134295 + EqualityComparer<float>.Default.GetHashCode(Y);
				hash = hash * -1521134295 + EqualityComparer<float>.Default.GetHashCode(Z);
				hash = hash * -1521134295 + EqualityComparer<float>.Default.GetHashCode(W);
				return hash;
			}
		}
		public override string ToString() => new System.Text.StringBuilder("EngineQuaternion { ").Append("X = ").Append(X).Append(", ").Append("Y = ").Append(Y).Append(", ").Append("Z = ").Append(Z).Append(", ").Append("W = ").Append(W).Append(" }").ToString();
		public static bool operator ==(EngineQuaternion left, EngineQuaternion right) => left.Equals(right);
		public static bool operator !=(EngineQuaternion left, EngineQuaternion right) => !left.Equals(right);
		public void Deconstruct(out float X, out float Y, out float Z, out float W) { X = this.X; Y = this.Y; Z = this.Z; W = this.W; }
	}
	public sealed record EngineTransformPose(object Transform, EngineVector LocalPosition, EngineVector LocalScale, EngineQuaternion LocalRotation);

	public static class PreparedTransformInputs
	{
		private static Dictionary<object, EngineTransformPose>? poses;
		private static Func<EngineQuaternion, EngineQuaternion, EngineQuaternion>? multiplyRotation;
		private static Func<EngineQuaternion, EngineVector, EngineVector>? rotateVector;
		private static readonly EngineQuaternion identity = new(0, 0, 0, 1);
		// Axis-aligned world reads (TransformPoint and Scale) are pure functions of the scene, object identity and poses.
		// Their per-transform position/scale terms and checks hold until any of those changes, which advances the epoch.
		private static long worldEpoch;
		private static Dictionary<object, AxisAlignedWorld>? world;
		private readonly struct AxisAlignedWorld
		{
			public readonly long Epoch; public readonly EngineVector Position, Scale;
			public AxisAlignedWorld(long epoch, EngineVector position, EngineVector scale) { Epoch = epoch; Position = position; Scale = scale; }
		}
		internal static void InvalidateWorld() => worldEpoch++;
		private static bool CachedWorld(object transform, out AxisAlignedWorld cached)
		{
			if (world is not null && world.TryGetValue(transform, out cached) && cached.Epoch == worldEpoch) return true;
			cached = default; return false;
		}
		public static bool IsInitialized => poses is not null;
		public static void Initialize(IReadOnlyList<EngineTransformPose> inputs)
		{
			if (poses is not null) throw new InvalidOperationException("Transform inputs are already initialized.");
			var next = new Dictionary<object, EngineTransformPose>(ReferenceEqualityComparer.Instance);
			foreach (var input in inputs)
			{
				if (!ReferenceEquals(SceneScripting.Transform(input.Transform), input.Transform)) throw new ArgumentException("Pose requires an actual Transform.");
				if (input.LocalRotation != new EngineQuaternion(0f, 0f, 0f, 1f)) throw new NotSupportedException("Prepared axis-aligned transforms require an authored identity quaternion.");
				if (!next.TryAdd(input.Transform, input)) throw new ArgumentException("Duplicate transform input.");
			}
			InvalidateWorld(); world = new Dictionary<object, AxisAlignedWorld>(ReferenceEqualityComparer.Instance);
			poses = next;
		}
		public static void BindPlanarRotationMath(Func<EngineQuaternion, EngineQuaternion, EngineQuaternion> multiply, Func<EngineQuaternion, EngineVector, EngineVector> rotate)
		{
			if (poses is null || multiplyRotation is not null) throw new InvalidOperationException("Rotation math requires an unbound prepared pose session.");
			ArgumentNullException.ThrowIfNull(multiply); ArgumentNullException.ThrowIfNull(rotate);
			InvalidateWorld(); multiplyRotation = multiply; rotateVector = rotate;
		}
		private static EngineTransformPose Require(object transform)
		{
			if (!ReferenceEquals(SceneScripting.Transform(transform), transform) || poses is null || !poses.TryGetValue(transform, out var pose))
				throw new InvalidOperationException("Transform has no explicit prepared pose.");
			return pose;
		}
		// Require followed by Parent on the same wrapper, read in one scene access with the same failure order.
		private static EngineTransformPose Require(object transform, out object? parent)
		{
			if (!SceneScripting.TransformParent(transform, out parent) || poses is null || !poses.TryGetValue(transform, out var pose))
				throw new InvalidOperationException("Transform has no explicit prepared pose.");
			return pose;
		}
		public static EngineVector LocalPosition(object transform) => Require(transform).LocalPosition;
		private static void RequirePlanar(EngineQuaternion rotation)
		{
			if (rotation.X != 0 || rotation.Y != 0 || !float.IsFinite(rotation.Z) || !float.IsFinite(rotation.W) || rotation.Z == 0 && rotation.W == 0)
				throw new NotSupportedException("Prepared rotation requires a finite planar quaternion.");
		}
		private static EngineQuaternion Compose(EngineQuaternion a, EngineQuaternion b) =>
			a == identity ? b : b == identity ? a : (multiplyRotation ?? throw new InvalidOperationException("Planar rotation math is not bound."))(a, b);
		// Quaternion inversion negates X, Y and Z and retains W without normalization.
		private static EngineQuaternion Inverse(EngineQuaternion value) => new(-value.X, -value.Y, -value.Z, value.W);
		public static EngineQuaternion Rotation(object transform)
		{
			var pose = Require(transform, out var parent); RequirePlanar(pose.LocalRotation);
			return parent is null ? pose.LocalRotation : Compose(Rotation(parent), pose.LocalRotation);
		}
		public static void Rotation(object transform, EngineQuaternion value)
		{
			var pose = Require(transform); RequirePlanar(value);
			if (multiplyRotation is null) throw new InvalidOperationException("Planar rotation writes require the explicit math producer.");
			var parent = SceneScripting.Parent(transform);
			poses![transform] = pose with { LocalRotation = parent is null ? value : Compose(Inverse(Rotation(parent)), value) }; InvalidateWorld();
		}
		// Local rotation normalization:
		// pairwise sum of squares, float sqrt and per-component divide when 1e-30 < sum, otherwise identity.
		public static void LocalRotation(object transform, EngineQuaternion value)
		{
			var pose = Require(transform);
			float sum = (value.X * value.X + value.Y * value.Y) + (value.Z * value.Z + value.W * value.W);
			float norm = MathF.Sqrt(sum);
			var normalized = 1e-30f < sum ? new EngineQuaternion(value.X / norm, value.Y / norm, value.Z / norm, value.W / norm) : identity;
			RequirePlanar(normalized);
			poses![transform] = pose with { LocalRotation = normalized }; InvalidateWorld();
		}
		private static bool Unrotated(object transform)
		{
			for (object? current = transform; current is not null;)
			{
				var rotation = Require(current, out var parent).LocalRotation; RequirePlanar(rotation);
				if (rotation != identity) return false;
				current = parent;
			}
			return true;
		}
		private static EngineVector Rotate(EngineQuaternion rotation, EngineVector value) => rotation == identity ? value :
			(rotateVector ?? throw new InvalidOperationException("Planar rotation math is not bound."))(rotation, value);
		public static void Register(EngineTransformPose input)
		{
			if (poses is null) throw new InvalidOperationException("Transform inputs are not initialized.");
			if (!ReferenceEquals(SceneScripting.Transform(input.Transform), input.Transform) || input.LocalRotation != new EngineQuaternion(0, 0, 0, 1)) throw new NotSupportedException("A live axis-aligned Transform is required.");
			if (!poses.TryAdd(input.Transform, input)) throw new ArgumentException("Transform pose is already bound.");
			InvalidateWorld();
		}
		public static void SetParent(object transform, object? parent, bool worldPositionStays)
		{
			Require(transform);
			if (parent is not null) Require(parent);
			if (!worldPositionStays) { SceneScripting.Reparent(transform, parent); return; }
			var position = Position(transform); var scale = Scale(transform); var rotation = Rotation(transform);
			if (worldPositionStays && parent is not null)
			{
				var parentScale = Scale(parent);
				if (parentScale.X == 0 || parentScale.Y == 0 || parentScale.Z == 0) throw new NotSupportedException("Singular parent cannot preserve the world pose.");
			}
			SceneScripting.Reparent(transform, parent);
			if (worldPositionStays)
			{
				var localRotation = parent is null ? rotation : Compose(Inverse(Rotation(parent)), rotation);
				poses![transform] = Require(transform) with { LocalRotation = localRotation }; InvalidateWorld();
				Position(transform, position);
				var parentScale = parent is null ? new EngineVector(1, 1, 1) : Scale(parent);
				LocalScale(transform, new(scale.X / parentScale.X, scale.Y / parentScale.Y, scale.Z / parentScale.Z));
			}
		}
		public static EngineVector LocalScale(object transform) => Require(transform).LocalScale;
		public static void LocalPosition(object transform, EngineVector value) { var pose = Require(transform); poses![transform] = pose with { LocalPosition = value }; InvalidateWorld(); }
		public static void LocalScale(object transform, EngineVector value) { var pose = Require(transform); poses![transform] = pose with { LocalScale = value }; InvalidateWorld(); }
		public static EngineVector Scale(object transform)
		{
			if (CachedWorld(transform, out var cached)) return cached.Scale;
			var result = new EngineVector(1f, 1f, 1f);
			for (object? current = transform; current is not null;)
			{
				var pose = Require(current, out var next); RequirePlanar(pose.LocalRotation);
				if (pose.LocalRotation != identity)
					for (object? parent = next; parent is not null;)
					{
						var scale = Require(parent, out var above).LocalScale;
						if (scale.X != scale.Y) throw new NotSupportedException("Rotated nonuniform ancestor scale requires the Native matrix scale producer.");
						parent = above;
					}
				result = Multiply(result, pose.LocalScale);
				current = next;
			}
			return result;
		}
		public static EngineVector Position(object transform)
		{
			var pose = Require(transform, out var parent);
			return parent is null ? pose.LocalPosition : TransformPoint(parent, pose.LocalPosition);
		}
		public static void Position(object transform, EngineVector value)
		{
			var parent = SceneScripting.Parent(transform); LocalPosition(transform, parent is null ? value : InverseTransformPoint(parent, value));
		}
		private const int CachedDepth = 32;
		public static EngineVector TransformPoint(object transform, EngineVector value)
		{
			if (CachedWorld(transform, out var cached))
			{
				var cachedScaled = Multiply(cached.Scale, value);
				return new(cached.Position.X + cachedScaled.X, cached.Position.Y + cachedScaled.Y, cached.Position.Z + cachedScaled.Z);
			}
			// Unrotated(transform)'s readouts and checks in its order. Each level's local position and scale are kept so
			// the axis-aligned recursion below repeats the original arithmetic without reading the scene again.
			Span<EngineVector> positions = stackalloc EngineVector[CachedDepth];
			Span<EngineVector> scales = stackalloc EngineVector[CachedDepth];
			int depth = 0;
			for (object? current = transform; current is not null;)
			{
				var pose = Require(current, out var parent); RequirePlanar(pose.LocalRotation);
				if (pose.LocalRotation != identity) return TransformPlanarPoint(transform, value);
				if (depth == CachedDepth) return TransformPointUncached(transform, value);
				positions[depth] = pose.LocalPosition; scales[depth] = pose.LocalScale; depth++;
				current = parent;
			}
			// AxisAlignedPoint at level 0, split into its position and scale terms so they can be reused.
			var position = depth == 1 ? positions[0] : AxisAlignedPoint(positions.Slice(0, depth), scales.Slice(0, depth), 1, positions[0]);
			var scale = new EngineVector(1f, 1f, 1f);
			for (int index = 0; index < depth; index++) scale = Multiply(scale, scales[index]);
			world![transform] = new AxisAlignedWorld(worldEpoch, position, scale);
			var scaled = Multiply(scale, value);
			return new(position.X + scaled.X, position.Y + scaled.Y, position.Z + scaled.Z);
		}
		// TransformPoint at chain level: Position(level) + Multiply(Scale(level), value), where Position recurses into the
		// parent and Scale folds the local scales upward from (1, 1, 1), exactly as the uncached members compute them.
		private static EngineVector AxisAlignedPoint(ReadOnlySpan<EngineVector> positions, ReadOnlySpan<EngineVector> scales, int level, EngineVector value)
		{
			var position = level + 1 == positions.Length ? positions[level] : AxisAlignedPoint(positions, scales, level + 1, positions[level]);
			var scale = new EngineVector(1f, 1f, 1f);
			for (int index = level; index < scales.Length; index++) scale = Multiply(scale, scales[index]);
			var scaled = Multiply(scale, value);
			return new(position.X + scaled.X, position.Y + scaled.Y, position.Z + scaled.Z);
		}
		private static EngineVector TransformPointUncached(object transform, EngineVector value)
		{
			if (!Unrotated(transform)) return TransformPlanarPoint(transform, value);
			var position = Position(transform); var scaled = Multiply(Scale(transform), value);
			return new(position.X + scaled.X, position.Y + scaled.Y, position.Z + scaled.Z);
		}
		private static EngineVector TransformPlanarPoint(object transform, EngineVector value)
		{
			var pose = Require(transform, out var parent); var scaled = Rotate(pose.LocalRotation, Multiply(pose.LocalScale, value));
			var local = new EngineVector(pose.LocalPosition.X + scaled.X, pose.LocalPosition.Y + scaled.Y, pose.LocalPosition.Z + scaled.Z);
			return parent is null ? local : TransformPlanarPoint(parent, local);
		}
		public static EngineVector InverseTransformPoint(object transform, EngineVector value)
		{
			if (!Unrotated(transform)) return InversePlanarPoint(transform, value);
			var position = Position(transform); var scale = Scale(transform);
			if (scale.X == 0 || scale.Y == 0 || scale.Z == 0) throw new NotSupportedException("Singular transform inversion requires the Native matrix mechanism.");
			return new((value.X - position.X) / scale.X, (value.Y - position.Y) / scale.Y, (value.Z - position.Z) / scale.Z);
		}
		private static EngineVector InversePlanarPoint(object transform, EngineVector value)
		{
			var pose = Require(transform, out var parent);
			var local = parent is null ? value : InversePlanarPoint(parent, value);
			local = Rotate(Inverse(pose.LocalRotation), new(local.X - pose.LocalPosition.X, local.Y - pose.LocalPosition.Y, local.Z - pose.LocalPosition.Z));
			var scale = pose.LocalScale;
			if (scale.X == 0 || scale.Y == 0 || scale.Z == 0) throw new NotSupportedException("Singular transform inversion requires the Native matrix mechanism.");
			return new(local.X / scale.X, local.Y / scale.Y, local.Z / scale.Z);
		}
		private static EngineVector Multiply(EngineVector a, EngineVector b) => new(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		public static void EndSession() { InvalidateWorld(); poses = null; world = null; multiplyRotation = null; rotateVector = null; }
	}

}
