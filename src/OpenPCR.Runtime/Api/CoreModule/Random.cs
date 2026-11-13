using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	// Managed surface from Random.bindings.cs; engine-native members forward to OpenPCR.Runtime.Impl.ScriptingRandom (libunity).
	public static class Random
	{
		[global::OpenPCR.Migration("949cce1e-b35b-49f7-86ba-18fba0baf577")]
		public static void InitState(int seed) { global::OpenPCR.Runtime.Impl.ScriptingRandom.SetSeed(seed); }

		public static State state
		{
			[global::OpenPCR.Migration("0bcf928c-85e4-40c3-ae73-1578aa4a5daa")]
			get { State ret; get_state_Injected(out ret); return ret; }
			[global::OpenPCR.Migration("74e79052-1d97-4587-bc9c-cabf5c3cc712")]
			set { set_state_Injected(ref value); }
		}

		[global::OpenPCR.Migration("f99d63e2-4133-4ce5-8603-6c3357c618f8")]
		public static float Range(float minInclusive, float maxInclusive) { return global::OpenPCR.Runtime.Impl.ScriptingRandom.RangeFloat(minInclusive, maxInclusive); }

		[global::OpenPCR.Migration("72c6150b-80ee-4e8f-8c80-0a467b927f56")]
		public static int Range(int minInclusive, int maxExclusive) { return RandomRangeInt(minInclusive, maxExclusive); }

		[global::OpenPCR.Migration("7a6e1603-1f95-4741-a54c-f9905b36e16c")]
		private static int RandomRangeInt(int minInclusive, int maxExclusive) { return global::OpenPCR.Runtime.Impl.ScriptingRandom.RandomRangeInt(minInclusive, maxExclusive); }

		// The icalls copy the 16-byte state as a whole; State's fields are private, so the copy reinterprets the same layout.
		[global::OpenPCR.Migration("6180ebe2-91f5-45ac-9da7-69c5715b79b6")]
		private static void get_state_Injected(out State ret) { ret = global::System.Runtime.CompilerServices.Unsafe.As<global::OpenPCR.Runtime.Impl.ScriptingRandom.State, State>(ref global::OpenPCR.Runtime.Impl.ScriptingRandom.Current); }

		[global::OpenPCR.Migration("e8e79028-aad1-4ee8-991b-271de590042b")]
		private static void set_state_Injected(ref State value) { global::OpenPCR.Runtime.Impl.ScriptingRandom.Current = global::System.Runtime.CompilerServices.Unsafe.As<State, global::OpenPCR.Runtime.Impl.ScriptingRandom.State>(ref value); }

		[global::System.Serializable]
		[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 16)]
		public struct State
		{
			private int s0;
			private int s1;
			private int s2;
			private int s3;
		}
	}
}
