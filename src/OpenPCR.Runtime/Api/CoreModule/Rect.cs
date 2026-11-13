using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 16)]
	public struct Rect : global::System.IEquatable<global::OpenPCR.Runtime.Api.Rect>, global::System.IFormattable
	{
		private float m_XMin;
		private float m_YMin;
		private float m_Width;
		private float m_Height;

		[global::OpenPCR.Migration("1a126668-9308-4eea-be42-26e86ab2d915")]
		public Rect(float x, float y, float width, float height)
		{
			m_XMin = x;
			m_YMin = y;
			m_Width = width;
			m_Height = height;
		}

		[global::OpenPCR.Migration("1a007788-6d7d-458e-9b5f-9b3f39fe68e9")]
		public bool Equals(global::OpenPCR.Runtime.Api.Rect other)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Rect::Equals(OpenPCR.Runtime.Api.Rect):System.Boolean", "Unity infrastructure not implemented");
		}

		[global::OpenPCR.Migration("3a2848e2-7ffd-4f01-ace3-d0a2a29ae2e9")]
		public string ToString(string format, global::System.IFormatProvider formatProvider)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.Rect::ToString(System.String,System.IFormatProvider):System.String", "Unity infrastructure not implemented");
		}

		public float y
		{
			[global::OpenPCR.Migration("110d82c8-0a2c-4f31-a4fb-8bb48b9a2954")]
			get { return m_YMin; }
			[global::OpenPCR.Migration("66191453-6c6b-4c76-b4e8-9c58df00b60b")]
			set { m_YMin = value; }
		}

		public float width
		{
			[global::OpenPCR.Migration("1d957451-f05e-4d05-9a32-01dee2b36bfd")]
			get { return m_Width; }
			[global::OpenPCR.Migration("de6e00c9-5336-415e-8afe-f203c36d4ffe")]
			set { m_Width = value; }
		}

		public float height
		{
			[global::OpenPCR.Migration("65a5a8aa-b0f8-411b-9cf4-5557b8b25cf0")]
			get { return m_Height; }
			[global::OpenPCR.Migration("fc1059e8-0f8b-4a52-bea6-da44da949efe")]
			set { m_Height = value; }
		}

		public float xMin
		{
			[global::OpenPCR.Migration("2c7be84c-a169-4d61-9622-202996835f62")]
			get { return m_XMin; }
			[global::OpenPCR.Migration("eaaf295f-883f-4592-8e6c-9d6e762df014")]
			set { float oldxmax = xMax; m_XMin = value; m_Width = oldxmax - m_XMin; }
		}
		public float yMin
		{
			[global::OpenPCR.Migration("aaf213e9-4227-426d-97bb-44152a4d513a")]
			get { return m_YMin; }
			[global::OpenPCR.Migration("d215d2c2-4d74-47dd-b035-55f3b33dd32a")]
			set { float oldymax = yMax; m_YMin = value; m_Height = oldymax - m_YMin; }
		}
		public float xMax
		{
			[global::OpenPCR.Migration("49429487-4b34-4fc5-ae31-d3773d6abd0e")]
			get { return m_Width + m_XMin; }
			[global::OpenPCR.Migration("9843eb67-2e81-40b1-9cb2-23ef77831fc6")]
			set { m_Width = value - m_XMin; }
		}
		public float yMax
		{
			[global::OpenPCR.Migration("4a8f7e49-1252-481f-8b22-73441467f067")]
			get { return m_Height + m_YMin; }
			[global::OpenPCR.Migration("84250332-2d49-465b-8030-938608743ec9")]
			set { m_Height = value - m_YMin; }
		}
	}
}
