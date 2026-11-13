using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>Player Keyframe layout (28 bytes; no editor tangent-mode field).</summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct EngineCurveKey
	{
		public float Time, Value, InTangent, OutTangent;
		public int WeightedMode;
		public float InWeight, OutWeight;
	}

	/// <summary>
	/// ClampForever, unweighted curve evaluation with a 1e-4 minimum interval width, signed step tangents
	/// and a cached interval. Weighted curves, other wrap modes, duplicate times and nonfinite evaluations fail.
	/// Resource loaders must preserve and validate these properties before creating curves.
	/// </summary>
	public static class CurveScripting
	{
		private sealed class Curve
		{
			internal readonly EngineCurveKey[] Keys;
			internal float Start = float.PositiveInfinity, End, A, B, C, D;
			internal Curve(EngineCurveKey[] keys) { Keys = keys; }
		}
		private static readonly ConcurrentDictionary<nint, Curve> curves = new();
		private static long nextHandle;

		public static IntPtr Create(ReadOnlySpan<byte> playerKeyBytes, int preInfinity, int postInfinity)
		{
			if (preInfinity != 2 || postInfinity != 2)
				throw new NotSupportedException("AnimationCurve requires verified ClampForever resource modes (native mode 2). Other wrap modes are outside the current engine subset.");
			if (playerKeyBytes.Length % 28 != 0) throw new ArgumentException("Expected whole 28-byte player Keyframes.", nameof(playerKeyBytes));
			var input = MemoryMarshal.Cast<byte, EngineCurveKey>(playerKeyBytes);
			var valid = new System.Collections.Generic.List<EngineCurveKey>(input.Length);
			foreach (var key in input)
			{
				// Internal_CopyFrom removes keys with nonfinite time/value before sorting.
				if (!float.IsFinite(key.Time) || !float.IsFinite(key.Value)) continue;
				if (key.WeightedMode != 0 || float.IsNaN(key.InTangent) || float.IsNaN(key.OutTangent))
					throw new NotSupportedException("AnimationCurve weighted or NaN-tangent keys are outside the verified engine subset.");
				valid.Add(key);
			}
			var keys = valid.ToArray();
			Array.Sort(keys, (left, right) => left.Time.CompareTo(right.Time));
			for (int i = 1; i < keys.Length; i++)
				if (keys[i - 1].Time == keys[i].Time)
					throw new NotSupportedException("AnimationCurve duplicate-time key ordering is outside the verified engine subset.");
			var handle = (nint)Interlocked.Increment(ref nextHandle);
			if (!curves.TryAdd(handle, new Curve(keys))) throw new InvalidOperationException("Curve engine handle collision.");
			return handle;
		}
		public static void Destroy(IntPtr handle) { if (handle != IntPtr.Zero) curves.TryRemove(handle, out _); }
		private static Curve Get(IntPtr handle) => curves.TryGetValue(handle, out var curve) ? curve :
			throw new InvalidOperationException("AnimationCurve has no live engine storage.");

		public static float Evaluate(IntPtr handle, float time)
		{
			var curve = Get(handle);
			lock (curve)
			{
				var keys = curve.Keys;
				if (keys.Length == 1) return keys[0].Value;
				if (keys.Length == 0) return 0f;
				if (!float.IsFinite(time)) throw new NotSupportedException("Nonfinite curve evaluation is outside the verified engine subset.");
				if (!(time >= curve.Start && time < curve.End))
				{
					if (time < keys[0].Time)
					{
						curve.Start = time + -1000f; curve.End = keys[0].Time;
						curve.A = 0f; curve.B = 0f; curve.C = 0f; curve.D = keys[0].Value;
					}
					else if (time >= keys[^1].Time)
					{
						curve.Start = keys[^1].Time; curve.End = float.PositiveInfinity;
						curve.A = 0f; curve.B = 0f; curve.C = 0f; curve.D = keys[^1].Value;
					}
					else
					{
						int low = 0, count = keys.Length;
						while (count > 0)
						{
							int half = count / 2, middle = low + half;
							if (keys[middle].Time > time) count = half;
							else { low = middle + 1; count -= half + 1; }
						}
						SetInterval(curve, keys[low - 1], keys[low]);
					}
				}
				float t = time - curve.Start;
				// Separate f32 operations: no FMA or normalized Hermite reassociation.
				float value = curve.A * t;
				value = value + curve.B; value = value * t;
				value = value + curve.C; value = value * t;
				return value + curve.D;
			}
		}
		private static void SetInterval(Curve curve, EngineCurveKey left, EngineCurveKey right)
		{
			curve.Start = left.Time + 0f; curve.End = right.Time + 0f;
			float width = right.Time - left.Time;
			width = width < 1e-4f ? 1e-4f : width;
			float delta = right.Value - left.Value;
			float inverse = 1f / width, inverseSquared = inverse * inverse;
			float outScaled = width * left.OutTangent, inScaled = width * right.InTangent;
			float b = delta + delta; b = b + delta;
			b = b - outScaled; b = b - outScaled;
			float a = outScaled + inScaled; a = a - delta; a = a - delta;
			a = a * inverseSquared; a = a * inverse;
			b = b - inScaled; b = b * inverseSquared;
			curve.A = a; curve.B = b; curve.C = left.OutTangent; curve.D = left.Value;
			if (float.IsPositiveInfinity(left.OutTangent) || float.IsPositiveInfinity(right.InTangent))
			{
				curve.A = 0f; curve.B = 0f; curve.C = 0f; curve.D = left.Value;
			}
			else if (float.IsNegativeInfinity(left.OutTangent) || float.IsNegativeInfinity(right.InTangent))
			{
				curve.A = 0f; curve.B = 0f; curve.C = 0f; curve.D = right.Value;
			}
		}
	}

}
