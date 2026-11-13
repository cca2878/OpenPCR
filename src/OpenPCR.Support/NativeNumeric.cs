namespace OpenPCR
{
	/// <summary>Signed floating-point conversions with explicit rounding and overflow behavior.</summary>
	public static class NativeNumeric
	{
		// FCVT returns zero for NaN and saturates finite overflow. IL2CPP's
		// explicit +Infinity guard selects the minimum value before return.
		public static int ToInt32(double value)
		{
			if (double.IsNaN(value)) return 0;
			if (double.IsPositiveInfinity(value)) return int.MinValue;
			if (value >= int.MaxValue) return int.MaxValue;
			if (value <= int.MinValue) return int.MinValue;
			return (int)value;
		}

		public static long ToInt64(double value)
		{
			if (double.IsNaN(value)) return 0;
			if (double.IsPositiveInfinity(value)) return long.MinValue;
			if (value >= 9223372036854775808.0) return long.MaxValue;
			if (value <= -9223372036854775808.0) return long.MinValue;
			return (long)value;
		}
	}
}
