using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::OpenPCR.ImplicitMigration("161b1a3c-4487-4e47-9eb0-e1ad071369d2")]
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 8)]
	public struct Vector2 : global::System.IEquatable<global::OpenPCR.Runtime.Api.Vector2>, global::System.IFormattable
	{
		public float x;
		public float y;

		[global::OpenPCR.Migration("3922bb41-cb9b-4914-81cc-afed9c40131a")]
		public Vector2(float x, float y) { this.x = x; this.y = y; }

		[global::OpenPCR.Migration("4bd4323b-259d-4eca-b3e8-d39ac89ebe1f")]
		public void Normalize()
		{
			float mag = magnitude;
			if (mag > kEpsilon)
				this = this / mag;
			else
				this = zero;
		}

		public Vector2 normalized
		{
			[global::OpenPCR.Migration("8f4f3dbf-8e36-4d91-8968-24bae31ded93")]
			get
			{
				Vector2 v = new Vector2(x, y);
				v.Normalize();
				return v;
			}
		}

		[global::OpenPCR.Migration("08852a66-0e8a-48e3-8770-1de96f051042")]
		public string ToString(string format, global::System.IFormatProvider formatProvider)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Vector2::ToString(System.String,System.IFormatProvider):System.String", "Unity infrastructure not implemented");
		}

		[global::OpenPCR.Migration("d24d0b80-4b23-4dfd-a94d-81a3764e44c3")]
		public bool Equals(global::OpenPCR.Runtime.Api.Vector2 other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Vector2::Equals(OpenPCR.Runtime.Api.Vector2):System.Boolean", "Unity infrastructure not implemented");
		}

		public float magnitude { [global::OpenPCR.Migration("58491d11-5587-45e7-ae5a-bb711e2a954e")] get { return (float)global::System.Math.Sqrt(x * x + y * y); } }

		[global::OpenPCR.Migration("5f76308c-aeb8-4da4-acf1-aa9a5328676e")]
		public static Vector2 operator +(Vector2 a, Vector2 b) { return new Vector2(a.x + b.x, a.y + b.y); }
		[global::OpenPCR.Migration("68e17ae2-88d3-445f-a5ee-1f85ab81c7ed")]
		public static Vector2 operator -(Vector2 a, Vector2 b) { return new Vector2(a.x - b.x, a.y - b.y); }
		[global::OpenPCR.Migration("a111bcff-7357-46bd-837e-58fb00433c5c")]
		public static Vector2 operator *(Vector2 a, float d) { return new Vector2(a.x * d, a.y * d); }
		[global::OpenPCR.Migration("140a0a11-ce71-4df4-bbae-0d850b608f4a")]
		public static Vector2 operator /(Vector2 a, float d) { return new Vector2(a.x / d, a.y / d); }

		[global::OpenPCR.Migration("aba08bce-858c-471b-9aaf-afb67f62566e")]
		public static implicit operator Vector2(Vector3 v)
		{
			return new Vector2(v.x, v.y);
		}

		[global::OpenPCR.Migration("cc3e80b3-9c16-465e-a559-ed01f52eaa91")]
		public static implicit operator Vector3(Vector2 v)
		{
			return new Vector3(v.x, v.y, 0);
		}

		static readonly Vector2 zeroVector = new Vector2(0F, 0F);
		static readonly Vector2 oneVector = new Vector2(1F, 1F);
		static readonly Vector2 upVector = new Vector2(0F, 1F);
		static readonly Vector2 downVector = new Vector2(0F, -1F);
		static readonly Vector2 leftVector = new Vector2(-1F, 0F);
		static readonly Vector2 rightVector = new Vector2(1F, 0F);
		static readonly Vector2 positiveInfinityVector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		static readonly Vector2 negativeInfinityVector = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

		public static Vector2 zero { [global::OpenPCR.Migration("4c354158-e5c5-4141-bf1e-323f3f195157")] get { return zeroVector; } }

		public const float kEpsilon = 0.00001F;
	}
}
