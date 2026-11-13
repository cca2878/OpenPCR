using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 24)]
	public struct Bounds : global::System.IEquatable<global::OpenPCR.Runtime.Api.Bounds>, global::System.IFormattable
	{
		private Vector3 m_Center;
		private Vector3 m_Extents;

		[global::OpenPCR.Migration("036f2f9c-967e-414b-9021-5a02e1a74ced")]
		public bool Equals(global::OpenPCR.Runtime.Api.Bounds other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Bounds::Equals(OpenPCR.Runtime.Api.Bounds):System.Boolean", "Unity infrastructure not implemented");
		}

		[global::OpenPCR.Migration("99a228ef-4e70-4cfb-b8a4-d7aa1650ac6a")]
		public string ToString(string format, global::System.IFormatProvider formatProvider)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Bounds::ToString(System.String,System.IFormatProvider):System.String", "Unity infrastructure not implemented");
		}

		public Vector3 center
		{
			[global::OpenPCR.Migration("584565c4-0a9f-4608-85e4-6a81d04106af")]
			get { return m_Center; }
			[global::OpenPCR.Migration("58519f42-c5a9-4bc7-8816-6469fba5ae2d")]
			set { m_Center = value; }
		}

		public Vector3 size
		{
			[global::OpenPCR.Migration("811c4cdb-168e-4c00-a942-2d16b9112293")]
			get { return m_Extents * 2.0F; }
			[global::OpenPCR.Migration("21121985-043c-4ec4-b6a3-42d580a20f12")]
			set { m_Extents = value * 0.5F; }
		}

		public Vector3 extents
		{
			[global::OpenPCR.Migration("6bb96026-22d1-431d-8687-927a0a706c87")]
			get { return m_Extents; }
			[global::OpenPCR.Migration("a0579f29-ea47-4db4-ac31-669f5e2b3b60")]
			set { m_Extents = value; }
		}
	}
}
