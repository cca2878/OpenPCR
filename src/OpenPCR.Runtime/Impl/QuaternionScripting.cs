using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>A vector as libunity returns it: x, y, z.</summary>
	public readonly struct Vec3
	{
		public readonly float X, Y, Z;
		public Vec3(float x, float y, float z) { X = x; Y = y; Z = z; }
	}

	/// <summary>A quaternion as libunity returns it: x, y, z, w.</summary>
	public readonly struct Quat
	{
		public readonly float X, Y, Z, W;
		public Quat(float x, float y, float z, float w) { X = x; Y = y; Z = z; W = w; }
	}

	/// <summary>
	/// Quaternion operations preserve expression order, including multiplication by and addition of zero,
	/// to retain signed-zero and NaN behavior. Trigonometric results depend on the platform math library.
	/// </summary>
	public static class QuaternionScripting
	{
		/// <summary>
		/// Convert Euler angles using ZXY order.
		/// Half angles are euler * 0.5; each goes through sincosf; the X and Y axis quaternions are multiplied first, then Z.
		/// </summary>
		public static Quat EulerToQuaternionZXY(float ex, float ey, float ez)
		{
			const float Z = 0f; // MOVI d0, #0 kept as an operand
			var (sx, cx) = MathF.SinCos(ex * 0.5f);
			var (sy, cy) = MathF.SinCos(ey * 0.5f);
			var (sz, cz) = MathF.SinCos(ez * 0.5f);

			// X then Y (registers s1, s2, s3, s6 hold the partial quaternion)
			float r3 = cx * cy;
			float r7 = sx * cy;
			float r16 = cx * sy;
			float r1 = cx * Z;
			float r2 = cy * Z;
			float r6 = sx * Z;
			float r18 = sy * Z;
			float r17 = r1 + r2;
			r1 = r1 + r7;
			r2 = r16 + r2;
			r3 = r3 - r6;
			r7 = sx * sy;
			r16 = r17 + Z;
			r1 = r18 + r1;
			r2 = r6 + r2;
			r3 = r3 - r18;
			r6 = r16 - r7;

			// then Z
			float q16 = cz * r1;
			float q17 = cz * r2;
			float q7 = r3 * Z;
			float q18 = sz * r3;
			float q20 = r1 * Z;
			float q3 = cz * r3;
			float q1 = sz * r1;
			float q19 = cz * r6;
			float q5 = q16 + q7;
			q16 = sz * r2;
			q7 = q17 + q7;
			q17 = r6 * Z;
			float q2 = r2 * Z;
			float q0 = q3 - q20;
			float q4 = sz * r6;
			q18 = q18 + q19;
			q3 = q16 + q5;
			q5 = q17 + q7;
			q16 = q0 - q2;
			q7 = q20 + q18;
			q0 = q3 - q17;
			q1 = q5 - q1;
			q3 = q16 - q4;
			q2 = q7 - q2;
			return new Quat(q0, q1, q2, q3);
		}

		/// <summary>
		/// Normalize by sqrt(((x*x + y*y) + z*z) + w*w) unless that is below 1e-5
		/// (0x3727C5AC; NaN normalizes), in which case the identity is used. Convert in ZXY order:
		/// X = -1 * asin(clamp(v, -1, 1)), Y and Z use atan2. When |yz - xw| reaches 0x3EFFFFDE
		/// (or is NaN), Y uses the singular products and Z is zero.
		/// </summary>
		public static Vec3 ToEulerRadZXY(float qx, float qy, float qz, float qw)
		{
			float dot = qx * qx + qy * qy;
			dot = dot + qz * qz;
			dot = dot + qw * qw;
			float magnitude = MathF.Sqrt(dot);
			float x, y, z, w;
			if (magnitude < 1e-5f) { x = 0f; y = 0f; z = 0f; w = 1f; }
			else { x = qx / magnitude; y = qy / magnitude; z = qz / magnitude; w = qw / magnitude; }

			float xx = x * x, xy = x * y, xz = x * z, xw = x * w;
			float yy = y * y, yz = y * z, yw = y * w;
			float zz = z * z, zw = z * w, ww = w * w;

			float r1 = yz - xw;
			float r20 = yy - zz;
			float r0 = xy + zw;
			r20 = r20 - xx;
			float r10 = r0 + r0;
			float r21 = r1 < 0f ? -r1 : r1;
			float r8 = ww + r20;
			r1 = r1 + r1;
			float r11, r9;
			bool singular = !(r21 < BitConverter.Int32BitsToSingle(0x3EFFFFDE));
			if (!singular)
			{
				float t = zz - xx;
				float r3 = xz + yw;
				t = t - yy;
				r11 = r3 + r3;
				r9 = ww + t;
			}
			else
			{
				float r2 = xw - yz;
				float r3 = xy - zw;
				float r4 = yz + xw;
				float r6 = r2 * r3;
				float r5 = r0 * r4;
				r2 = r2 * r4;
				r0 = r0 * r3;
				r11 = r5 + r6;
				r9 = r2 - r0;
			}
			float ex = AsinScaled(-1f, r1);
			float ey = MathF.Atan2(r11, r9);
			float ez = singular ? 0f : MathF.Atan2(r10, r8);
			return new Vec3(ex, ey, ez);
		}

		// Clamp b to [-1, 1] (GT then MI on the original b; NaN passes through), asinf, multiply by a.
		private static float AsinScaled(float a, float b)
		{
			float clamped = b > 1f ? 1f : b;
			clamped = b < -1f ? -1f : clamped;
			return MathF.Asin(clamped) * a;
		}

		/// <summary>
		/// Angle-axis conversion: |axis| = sqrt((x*x + y*y) + z*z); if it is not greater than 1e-6 (0x358637BD, NaN included)
		/// the identity is returned. Otherwise the half angle is ((angle / 360) + (angle / 360)) * PI (0x40490FDB) * 0.5,
		/// and x, y, z = axis * (sin / |axis|), w = cos.
		/// </summary>
		public static Quat AngleAxis(float angle, float ax, float ay, float az)
		{
			float magnitude = MathF.Sqrt(ax * ax + ay * ay + az * az);
			if (!(magnitude > 1e-6f)) return new Quat(0f, 0f, 0f, 1f);
			float half = angle / 360f;
			half = half + half;
			half = half * 3.14159274f;
			half = half * 0.5f;
			var (s, c) = MathF.SinCos(half);
			float k = s / magnitude;
			return new Quat(ax * k, ay * k, az * k, c);
		}
	}

}
