using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Explicit, Pack = 1, Size = 4)]
	public struct Color32 : global::System.IEquatable<global::OpenPCR.Runtime.Api.Color32>, global::System.IFormattable
	{
		[global::System.Runtime.InteropServices.FieldOffset(0)]
		private int rgba;
		[global::System.Runtime.InteropServices.FieldOffset(0)]
		public byte r;
		[global::System.Runtime.InteropServices.FieldOffset(1)]
		public byte g;
		[global::System.Runtime.InteropServices.FieldOffset(2)]
		public byte b;
		[global::System.Runtime.InteropServices.FieldOffset(3)]
		public byte a;

		[global::OpenPCR.Migration("1a521a62-25a2-406e-8cb2-09ef8f87dc45")]
		public Color32(byte r, byte g, byte b, byte a)
		{
			rgba = 0; this.r = r; this.g = g; this.b = b; this.a = a;
		}

		[global::OpenPCR.Migration("000e9813-d708-4ee9-921c-dd4c26dfda06")]
		public static implicit operator Color(Color32 c)
		{
			return new Color(c.r / 255f, c.g / 255f, c.b / 255f, c.a / 255f);
		}

		[global::OpenPCR.Migration("1b0fc41b-077c-463d-a1f8-e4ab499152b5")]
		public bool Equals(global::OpenPCR.Runtime.Api.Color32 other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Color32::Equals(OpenPCR.Runtime.Api.Color32):System.Boolean", "Unity infrastructure not implemented");
		}

		[global::OpenPCR.Migration("34865bb4-14b8-4137-a8bb-8c08569e5300")]
		public string ToString(string format, global::System.IFormatProvider formatProvider)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Color32::ToString(System.String,System.IFormatProvider):System.String", "Unity infrastructure not implemented");
		}
	}
}
