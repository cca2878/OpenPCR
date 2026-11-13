using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct Vector4 : global::System.IEquatable<global::OpenPCR.Runtime.Api.Vector4>, global::System.IFormattable
	{
		public float x;
		public float y;
		public float z;
		public float w;
		[global::OpenPCR.Migration("a8c993d8-a1cc-4cce-9d68-f23003bfb0e7")]
		public bool Equals(global::OpenPCR.Runtime.Api.Vector4 other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Vector4::Equals(OpenPCR.Runtime.Api.Vector4):System.Boolean", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("3334d09d-8f4a-4e0e-850d-ccf8e6be726e")]
		public string ToString(string format, global::System.IFormatProvider formatProvider)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Vector4::ToString(System.String,System.IFormatProvider):System.String", "Unity infrastructure not implemented");
		}
	}
}
