using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Api.Helpers;
namespace OpenPCR.Runtime.Api
{
	[global::OpenPCR.ImplicitMigration("c9fcd154-6df2-496e-a696-409a3625f241")]
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 1)]
	public struct Mathf
	{
		// Trigonometric functions use single-precision arithmetic.
		[global::OpenPCR.Migration("310506bc-0727-4bba-9bf9-a813a37ef235")]
		public static float Sin(float f) { return global::System.MathF.Sin(f); }

		[global::OpenPCR.Migration("0514ffd8-1930-4d51-9285-2a565349509a")]
		public static float Cos(float f) { return global::System.MathF.Cos(f); }

		[global::OpenPCR.Migration("b7c2b0a6-cd25-44b2-9177-141f65da03b4")]
		public static float Atan2(float y, float x) { return global::System.MathF.Atan2(y, x); }

		[global::OpenPCR.Migration("ae554438-79b9-4a64-8f3d-edf7a68cdc56")]
		public static float Sqrt(float f) { return (float)global::System.Math.Sqrt(f); }

		[global::OpenPCR.Migration("a310106f-39d0-4c2e-8933-fa960a98a032")]
		public static float Abs(float f) { return global::System.Math.Abs(f); }

		[global::OpenPCR.Migration("448c1bac-bb45-4659-a962-a7055091eeaf")]
		public static float Min(float a, float b) { return a < b ? a : b; }

		[global::OpenPCR.Migration("42a0b638-80eb-4911-9383-18922a4765b5")]
		public static int Min(int a, int b) { return a < b ? a : b; }

		[global::OpenPCR.Migration("72e0c872-3372-482b-b267-61b2b19d91f5")]
		public static float Max(float a, float b) { return a > b ? a : b; }

		[global::OpenPCR.Migration("cb91af8c-403c-4cf5-aa8a-45ce6b4de81f")]
		public static int Max(int a, int b) { return a > b ? a : b; }

		[global::OpenPCR.Migration("27f62a44-e50a-4627-8128-f19e6b0b3ff4")]
		public static float Ceil(float f) { return (float)global::System.Math.Ceiling(f); }

		[global::OpenPCR.Migration("09314ade-0ec7-48af-9504-59856c24c994")]
		public static float Floor(float f) { return (float)global::System.Math.Floor(f); }

		// Preserve the defined overflow and infinity handling after rounding down.
		[global::OpenPCR.Migration("ddb81e52-77db-465f-8f9c-274f9dfa53e2")]
		public static int FloorToInt(float f) { return global::OpenPCR.NativeNumeric.ToInt32(global::System.Math.Floor(f)); }

		[global::OpenPCR.Migration("40648231-d518-42af-9427-aa06d7f3a4c8")]
		public static float Sign(float f) { return f >= 0F ? 1F : -1F; }

		public static readonly float Epsilon =
			global::OpenPCR.Runtime.Api.Helpers.MathfInternal.IsFlushToZeroEnabled ? global::OpenPCR.Runtime.Api.Helpers.MathfInternal.FloatMinNormal
			: global::OpenPCR.Runtime.Api.Helpers.MathfInternal.FloatMinDenormal;

		[global::OpenPCR.Migration("d837b5ee-b812-4672-8361-b2fcc5ecc6e9")]
		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
				value = min;
			else if (value > max)
				value = max;
			return value;
		}

		[global::OpenPCR.Migration("2f79ec65-c2bb-402c-975b-165a334a9417")]
		public static float Clamp01(float value)
		{
			if (value < 0F)
				return 0F;
			else if (value > 1F)
				return 1F;
			else
				return value;
		}

		[global::OpenPCR.Migration("8619707e-1862-4d60-8698-b4f017ce6f7c")]
		public static float Lerp(float a, float b, float t)
		{
			return a + (b - a) * Clamp01(t);
		}

		[global::OpenPCR.Migration("5c8fbe8a-300c-4a26-991e-b3068c409cbb")]
		public static float Repeat(float t, float length)
		{
			return Clamp(t - Mathf.Floor(t / length) * length, 0.0f, length);
		}
	}
}
