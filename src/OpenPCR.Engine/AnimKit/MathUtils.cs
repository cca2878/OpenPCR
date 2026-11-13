using global::OpenPCR.Runtime.Api;
using System;

namespace OpenPCR.AnimKit
{
	public static class MathUtils
	{

		[global::OpenPCR.Migration("cb082b75-127d-48fb-9dec-8754abbc813d")]
		public static float Sin(float radians)
		{
			return Mathf.Sin(radians);
		}

		[global::OpenPCR.Migration("63b98aec-4a89-43b1-8d8b-36f4640ea9b3")]
		public static float Cos(float radians)
		{
			return Mathf.Cos(radians);
		}

		[global::OpenPCR.Migration("b7e58492-e2db-42fe-a04a-0bff3b0733e3")]
		public static float SinDeg(float degrees)
		{
			return Mathf.Sin(degrees * ((float)Math.PI / 180f));
		}

		[global::OpenPCR.Migration("8cdfd52f-4dcc-4020-91ce-193a1435d985")]
		public static float CosDeg(float degrees)
		{
			return Mathf.Cos(degrees * ((float)Math.PI / 180f));
		}

		[global::OpenPCR.Migration("f9d287bf-ec30-4574-9ce4-508e74284a03")]
		public static float Atan2(float y, float x)
		{
			if (x == 0f)
			{
				if (y > 0f)
				{
					return (float)Math.PI / 2f;
				}
				if (y == 0f)
				{
					return 0f;
				}
				return -(float)Math.PI / 2f;
			}
			return Mathf.Atan2(y, x);
		}

		[global::OpenPCR.Migration("ca8eb50f-acd3-4c8a-8fcd-f028ef8f0e72")]
		public static float Clamp(float value, float min, float max)
		{
			if (value < min) return min;
			if (value > max) return max;
			return value;
		}
	}
}
