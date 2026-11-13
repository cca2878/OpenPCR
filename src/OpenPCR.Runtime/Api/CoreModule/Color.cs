using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct Color : global::System.IEquatable<global::OpenPCR.Runtime.Api.Color>, global::System.IFormattable
	{
		public float r;
		public float g;
		public float b;
		public float a;

		[global::OpenPCR.Migration("7ecc11f5-b17d-425d-914e-c578c4decd83")]
		public Color(float r, float g, float b, float a)
		{
			this.r = r; this.g = g; this.b = b; this.a = a;
		}

		[global::OpenPCR.Migration("43b672d9-6cdf-4410-bd07-c1e09e25b24d")]
		public Color(float r, float g, float b)
		{
			this.r = r; this.g = g; this.b = b; this.a = 1.0F;
		}

		[global::OpenPCR.Migration("d4b7717f-85a2-42b2-8fcd-0ec601b9f5ea")]
		public string ToString(string format, global::System.IFormatProvider formatProvider)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Color::ToString(System.String,System.IFormatProvider):System.String", "Unity infrastructure not implemented");
		}

		[global::OpenPCR.Migration("53856ce7-71d3-44f8-aaaa-ae660cecee79")]
		public bool Equals(global::OpenPCR.Runtime.Api.Color other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Color::Equals(OpenPCR.Runtime.Api.Color):System.Boolean", "Unity infrastructure not implemented");
		}

		[global::OpenPCR.Migration("85d5514c-0e05-49f8-8c00-aeb200fefa8c")]
		public static Color operator +(Color a, Color b) { return new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a); }

		[global::OpenPCR.Migration("7f2fc188-36d9-44ea-953a-0b0b5c7bc57c")]
		public static Color operator *(Color a, Color b) { return new Color(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a); }

		[global::OpenPCR.Migration("944cb034-9e79-409d-99f0-4b614ee783c1")]
		public static Color Lerp(Color a, Color b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Color(
				a.r + (b.r - a.r) * t,
				a.g + (b.g - a.g) * t,
				a.b + (b.b - a.b) * t,
				a.a + (b.a - a.a) * t
			);
		}

		public static Color white { [global::OpenPCR.Migration("44270575-924b-48bd-b5b1-cc7bc8db8a9f")] get { return new Color(1F, 1F, 1F, 1F); } }
		public static Color black { [global::OpenPCR.Migration("4c027b6d-271a-4124-966a-5b1b117cf042")] get { return new Color(0F, 0F, 0F, 1F); } }
		public static Color clear { [global::OpenPCR.Migration("e0ccbb17-9c6e-4a43-ad37-87fce1a1f3db")] get { return new Color(0F, 0F, 0F, 0F); } }
	}
}
