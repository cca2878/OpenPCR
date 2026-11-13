using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 64)]
	public struct Matrix4x4 : global::System.IEquatable<global::OpenPCR.Runtime.Api.Matrix4x4>, global::System.IFormattable
	{
		public float m00;
		public float m10;
		public float m20;
		public float m30;
		public float m01;
		public float m11;
		public float m21;
		public float m31;
		public float m02;
		public float m12;
		public float m22;
		public float m32;
		public float m03;
		public float m13;
		public float m23;
		public float m33;
		[global::OpenPCR.Migration("d111e696-02e6-4f9c-879b-1544d5e714b8")]
		public bool Equals(global::OpenPCR.Runtime.Api.Matrix4x4 other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Matrix4x4::Equals(OpenPCR.Runtime.Api.Matrix4x4):System.Boolean", "Unity infrastructure not implemented");
		}
		[global::OpenPCR.Migration("39b2b96d-e3db-4fb0-b9f4-d532b3be1ea6")]
		public string ToString(string format, global::System.IFormatProvider formatProvider)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Matrix4x4::ToString(System.String,System.IFormatProvider):System.String", "Unity infrastructure not implemented");
		}
	}
}
