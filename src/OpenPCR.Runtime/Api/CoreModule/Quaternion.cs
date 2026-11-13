using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
namespace OpenPCR.Runtime.Api
{
	[global::OpenPCR.ImplicitMigration("ef2429e2-d9db-425f-b49d-c8ecc951d093")]
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct Quaternion : global::System.IEquatable<global::OpenPCR.Runtime.Api.Quaternion>, global::System.IFormattable
	{
		public float x;
		public float y;
		public float z;
		public float w;

		[global::OpenPCR.Migration("f8821e61-8ab5-43f1-8555-1b677c1f4390")]
		public Quaternion(float x, float y, float z, float w) { this.x = x; this.y = y; this.z = z; this.w = w; }
		[global::OpenPCR.Migration("a3c16cf2-1d3a-41d3-ac3f-848e4dd12c40")]
		public static global::OpenPCR.Runtime.Api.Quaternion FromToRotation(global::OpenPCR.Runtime.Api.Vector3 fromDirection, global::OpenPCR.Runtime.Api.Vector3 toDirection)
		{
			return global::OpenPCR.Presentation.Noop<global::OpenPCR.Runtime.Api.Quaternion>();
		}
		[global::OpenPCR.Migration("0ad824bb-6c01-4f7d-86ea-e491b2cb45a6")]
		public static Quaternion AngleAxis(float angle, Vector3 axis) { Quaternion ret; AngleAxis_Injected(angle, ref axis, out ret); return ret; }

		[global::OpenPCR.Migration("a3255a70-6772-41c0-aeb6-0dab5c5bf991")]
		private static void AngleAxis_Injected(float angle, ref Vector3 axis, out Quaternion ret)
		{
			var q = global::OpenPCR.Runtime.Impl.QuaternionScripting.AngleAxis(angle, axis.x, axis.y, axis.z);
			ret = new Quaternion(q.X, q.Y, q.Z, q.W);
		}
		[global::OpenPCR.Migration("a58258b0-9ae7-43dd-9b9b-2f0def8b02c6")]
		public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
		{
			return new Quaternion(
				lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
				lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z,
				lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x,
				lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
		}
		[global::OpenPCR.Migration("1d78a6bf-b8c9-414e-bf2a-95693ecc1ddb")]
		public static Vector3 operator *(Quaternion rotation, Vector3 point)
		{
			float x = rotation.x * 2F;
			float y = rotation.y * 2F;
			float z = rotation.z * 2F;
			float xx = rotation.x * x;
			float yy = rotation.y * y;
			float zz = rotation.z * z;
			float xy = rotation.x * y;
			float xz = rotation.x * z;
			float yz = rotation.y * z;
			float wx = rotation.w * x;
			float wy = rotation.w * y;
			float wz = rotation.w * z;

			Vector3 res;
			res.x = (1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z;
			res.y = (xy + wz) * point.x + (1F - (xx + zz)) * point.y + (yz - wx) * point.z;
			res.z = (xz - wy) * point.x + (yz + wx) * point.y + (1F - (xx + yy)) * point.z;
			return res;
		}
		// Single-precision degrees-to-radians factor.
		[global::OpenPCR.Migration("b785c9d0-dc65-47d6-87d8-4df5e95adeb8")]
		public static Quaternion Euler(float x, float y, float z) { const float Deg2Rad = 0.0174532924F; return Internal_FromEulerRad(new Vector3(x, y, z) * Deg2Rad); }
		[global::OpenPCR.Migration("9a7f7999-4775-4717-8c7f-daff0a85b9a4")]
		public static Quaternion Euler(Vector3 euler) { const float Deg2Rad = 0.0174532924F; return Internal_FromEulerRad(euler * Deg2Rad); }

		[global::OpenPCR.Migration("8142b0bf-0a29-4f28-b0f0-1c81f0fe4633")]
		private static Quaternion Internal_FromEulerRad(Vector3 euler) { Quaternion ret; Internal_FromEulerRad_Injected(ref euler, out ret); return ret; }

		[global::OpenPCR.Migration("f856ccdd-37f1-4701-b386-98b66115ceae")]
		private static void Internal_FromEulerRad_Injected(ref Vector3 euler, out Quaternion ret)
		{
			var q = global::OpenPCR.Runtime.Impl.QuaternionScripting.EulerToQuaternionZXY(euler.x, euler.y, euler.z);
			ret = new Quaternion(q.X, q.Y, q.Z, q.W);
		}
		[global::OpenPCR.Migration("07266fc3-0fb8-4448-869d-ccef8c51e7de")]
		public bool Equals(global::OpenPCR.Runtime.Api.Quaternion other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Quaternion::Equals(OpenPCR.Runtime.Api.Quaternion):System.Boolean", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("dae1a5e7-e9c1-443b-b893-e3f43c259134")]
		public string ToString(string format, global::System.IFormatProvider formatProvider)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Quaternion::ToString(System.String,System.IFormatProvider):System.String", "Unity infrastructure not implemented");
		}
		static readonly Quaternion identityQuaternion = new Quaternion(0F, 0F, 0F, 1F);

		public static Quaternion identity
		{
			[global::OpenPCR.Migration("631827c6-9849-457f-9709-2637ebe2451b")]
			get
			{
				return identityQuaternion;
			}
		}
		// Single-precision radians-to-degrees factor.
		public Vector3 eulerAngles
		{
			[global::OpenPCR.Migration("05c57a53-7aa2-4399-9e6b-985a8aae40ea")]
			get { const float Rad2Deg = 57.2957802F; return Internal_MakePositive(Internal_ToEulerRad(this) * Rad2Deg); }
		}

		// negativeFlip = -0.0001f * Rad2Deg (0xBBBBBF2F) and positiveFlip = 360 + negativeFlip (0x43B3FF44), as folded in the target.
		[global::OpenPCR.Migration("3e63123b-961c-45e0-bf4c-b4cd93112733")]
		private static Vector3 Internal_MakePositive(Vector3 euler)
		{
			const float negativeFlip = -0.00572957797F;
			const float positiveFlip = 359.994263F;

			if (euler.x < negativeFlip)
				euler.x += 360.0f;
			else if (euler.x > positiveFlip)
				euler.x -= 360.0f;

			if (euler.y < negativeFlip)
				euler.y += 360.0f;
			else if (euler.y > positiveFlip)
				euler.y -= 360.0f;

			if (euler.z < negativeFlip)
				euler.z += 360.0f;
			else if (euler.z > positiveFlip)
				euler.z -= 360.0f;

			return euler;
		}

		[global::OpenPCR.Migration("e4dfd16b-ce17-4a9c-9e7a-c8dbe2b4f647")]
		private static Vector3 Internal_ToEulerRad(Quaternion rotation) { Vector3 ret; Internal_ToEulerRad_Injected(ref rotation, out ret); return ret; }

		[global::OpenPCR.Migration("2b8566bb-6eac-45f8-abec-aabbe1d80a9d")]
		private static void Internal_ToEulerRad_Injected(ref Quaternion rotation, out Vector3 ret)
		{
			var e = global::OpenPCR.Runtime.Impl.QuaternionScripting.ToEulerRadZXY(rotation.x, rotation.y, rotation.z, rotation.w);
			ret = new Vector3(e.X, e.Y, e.Z);
		}
	}
}
