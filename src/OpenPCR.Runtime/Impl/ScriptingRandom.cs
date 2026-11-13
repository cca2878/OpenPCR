using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;
using System.Runtime.InteropServices;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>
	/// A process-wide xorshift128 state shared by every OpenPCR.Runtime.Api.Random caller.
	/// </summary>
	public static class ScriptingRandom
	{
		/// <summary>Same 16-byte layout as OpenPCR.Runtime.Api.Random.State (s0..s3).</summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct State
		{
			public uint X, Y, Z, W;
		}

		private static State current;

		/// <summary>
		/// Verification-only observer, invoked before every state access or change (draw, seed, Random.state read/write).
		/// It must not touch this generator; <see cref="Peek"/> reads the state without notifying it.
		/// </summary>
		public static Action? Observer;

		/// <summary>The global state; Random.state reads and writes it as one 16-byte copy.</summary>
		public static ref State Current
		{
			get
			{
				Observer?.Invoke();
				return ref current;
			}
		}

		/// <summary>Reads the global state without notifying <see cref="Observer"/>.</summary>
		public static State Peek() => current;

		/// <summary>Seed initialization: x = seed, then y, z, w each previous * 0x6C078965 + 1.</summary>
		public static void SetSeed(int seed)
		{
			Observer?.Invoke();
			unchecked
			{
				uint x = (uint)seed;
				uint y = x * 0x6C078965u + 1u;
				uint z = y * 0x6C078965u + 1u;
				uint w = z * 0x6C078965u + 1u;
				current = new State { X = x, Y = y, Z = z, W = w };
			}
		}

		/// <summary>The xorshift128 step inlined in every draw: t = x ^ (x &lt;&lt; 11); w' = t ^ (t &gt;&gt; 8) ^ w ^ (w &gt;&gt; 19).</summary>
		private static uint Next()
		{
			Observer?.Invoke();
			uint x = current.X, w = current.W;
			uint t = x ^ (x << 11);
			uint next = t ^ (t >> 8) ^ w ^ (w >> 19);
			current.X = current.Y;
			current.Y = current.Z;
			current.Z = w;
			current.W = next;
			return next;
		}

		// The scale is exactly 0x34000001, not 2^-23.
		private static readonly float UnitScale = BitConverter.Int32BitsToSingle(0x34000001);

		/// <summary>Floating-point range: f = (next &amp; 0x7FFFFF) * scale; f * min + (1 - f) * max (f = 0 yields max).</summary>
		public static float RangeFloat(float min, float max)
		{
			float f = (float)(Next() & 0x7FFFFFu) * UnitScale;
			return f * min + (1f - f) * max;
		}

		/// <summary>
		/// Integer range: signed compare; min &lt; max draws min + next % (max - min), min &gt; max
		/// draws min - next % (min - max), both with unsigned modulo and 32-bit wrap; min == max returns min without drawing.
		/// </summary>
		public static int RandomRangeInt(int min, int max)
		{
			unchecked
			{
				if (min < max)
				{
					uint range = (uint)(max - min);
					return min + (int)(Next() % range);
				}
				if (min > max)
				{
					uint range = (uint)(min - max);
					return min - (int)(Next() % range);
				}
				return min;
			}
		}
	}

}
