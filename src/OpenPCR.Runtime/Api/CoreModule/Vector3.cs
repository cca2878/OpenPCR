using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::OpenPCR.ImplicitMigration("f56937a9-7f59-4ea3-926a-73e3888bc49a")]
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 12)]
	public struct Vector3 : global::System.IEquatable<global::OpenPCR.Runtime.Api.Vector3>, global::System.IFormattable
	{
		public const float kEpsilon = 0.00001F;

		public float x;
		public float y;
		public float z;

		[global::OpenPCR.Migration("64f0f41d-8970-479c-851f-41043f97c04a")]
		public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Vector3(
				a.x + (b.x - a.x) * t,
				a.y + (b.y - a.y) * t,
				a.z + (b.z - a.z) * t
			);
		}

		[global::OpenPCR.Migration("02dcab57-15f8-4464-87ea-67576b2de0e7")]
		public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
		[global::OpenPCR.Migration("7ed97fdd-ba07-4ca2-a03e-3bf887adf277")]
		public Vector3(float x, float y) { this.x = x; this.y = y; z = 0F; }

		[global::OpenPCR.Migration("bb310237-aacb-486e-88b7-42124100f1e5")]
		public static Vector3 Scale(Vector3 a, Vector3 b) { return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z); }

		[global::OpenPCR.Migration("e2d2b0e4-02ad-4e32-92e6-24faac2f91ce")]
		public bool Equals(global::OpenPCR.Runtime.Api.Vector3 other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Vector3::Equals(OpenPCR.Runtime.Api.Vector3):System.Boolean", "Unity infrastructure not implemented");
		}

		[global::OpenPCR.Migration("9e2030fb-10d4-422e-a132-ed8063dc4f4d")]
		public static Vector3 Normalize(Vector3 value)
		{
			float mag = Magnitude(value);
			if (mag > kEpsilon)
				return value / mag;
			else
				return zero;
		}

		[global::OpenPCR.Migration("f6dabc38-bf2b-49d0-b1e6-3cc31fde2d8b")]
		public void Normalize()
		{
			float mag = Magnitude(this);
			if (mag > kEpsilon)
				this = this / mag;
			else
				this = zero;
		}

		public Vector3 normalized
		{
			[global::OpenPCR.Migration("267d9361-de06-416d-bb8a-09ac7c0484e4")]
			get { return Vector3.Normalize(this); }
		}

		[global::OpenPCR.Migration("cc9b059c-8722-4609-9907-3aec7c130508")]
		public static float Distance(Vector3 a, Vector3 b)
		{
			float diff_x = a.x - b.x;
			float diff_y = a.y - b.y;
			float diff_z = a.z - b.z;
			return (float)global::System.Math.Sqrt(diff_x * diff_x + diff_y * diff_y + diff_z * diff_z);
		}

		[global::OpenPCR.Migration("361be08c-efdf-4e2f-8d66-212790f27b94")]
		public static float Magnitude(Vector3 vector) { return (float)global::System.Math.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z); }

		static readonly Vector3 zeroVector = new Vector3(0F, 0F, 0F);
		static readonly Vector3 oneVector = new Vector3(1F, 1F, 1F);
		static readonly Vector3 upVector = new Vector3(0F, 1F, 0F);
		static readonly Vector3 downVector = new Vector3(0F, -1F, 0F);
		static readonly Vector3 leftVector = new Vector3(-1F, 0F, 0F);
		static readonly Vector3 rightVector = new Vector3(1F, 0F, 0F);
		static readonly Vector3 forwardVector = new Vector3(0F, 0F, 1F);
		static readonly Vector3 backVector = new Vector3(0F, 0F, -1F);
		static readonly Vector3 positiveInfinityVector = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		static readonly Vector3 negativeInfinityVector = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

		public static Vector3 zero { [global::OpenPCR.Migration("b68650fa-2045-4b49-9b89-b7de857d169a")] get { return zeroVector; } }
		public static Vector3 one { [global::OpenPCR.Migration("853479ef-46ce-4e98-85c8-e98069bb8a88")] get { return oneVector; } }
		public static Vector3 forward { [global::OpenPCR.Migration("f36d9cca-aadf-4f34-a959-f6497dd31159")] get { return forwardVector; } }
		public static Vector3 back { [global::OpenPCR.Migration("98db1bc1-46b8-415d-842e-63d67bdd4eb6")] get { return backVector; } }
		public static Vector3 up { [global::OpenPCR.Migration("9f9d1460-2db0-420e-9605-6cb1c6606559")] get { return upVector; } }
		public static Vector3 left { [global::OpenPCR.Migration("c6e824ce-fe6d-4a94-a0a4-ca7437a3a304")] get { return leftVector; } }
		public static Vector3 right { [global::OpenPCR.Migration("d5867ef0-a7c2-4857-bc0a-1424a3cb155b")] get { return rightVector; } }

		[global::OpenPCR.Migration("8d02fa7a-64eb-40ad-ab47-607e9b892ab9")]
		public static Vector3 operator +(Vector3 a, Vector3 b) { return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z); }
		[global::OpenPCR.Migration("70098daf-1f6a-48d4-a765-f2368e0d7858")]
		public static Vector3 operator -(Vector3 a, Vector3 b) { return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z); }
		[global::OpenPCR.Migration("4fa8ecd8-2aa8-4f96-a92a-e343734d8f72")]
		public static Vector3 operator -(Vector3 a) { return new Vector3(-a.x, -a.y, -a.z); }
		[global::OpenPCR.Migration("4f6675a9-c2a9-4ad5-84e3-219c75e718c3")]
		public static Vector3 operator *(Vector3 a, float d) { return new Vector3(a.x * d, a.y * d, a.z * d); }
		[global::OpenPCR.Migration("4014c2b0-e00f-4b58-9497-2d26b0230155")]
		public static Vector3 operator *(float d, Vector3 a) { return new Vector3(a.x * d, a.y * d, a.z * d); }
		[global::OpenPCR.Migration("68349fcf-e9a1-4dec-a5ab-c9fe24fec32d")]
		public static Vector3 operator /(Vector3 a, float d) { return new Vector3(a.x / d, a.y / d, a.z / d); }

		[global::OpenPCR.Migration("1ec51884-55aa-41bd-94e0-46d5a4739242")]
		public string ToString(string format, global::System.IFormatProvider formatProvider)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Vector3::ToString(System.String,System.IFormatProvider):System.String", "Unity infrastructure not implemented");
		}
	}
}
