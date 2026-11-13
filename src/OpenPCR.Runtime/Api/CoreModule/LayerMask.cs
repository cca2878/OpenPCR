namespace OpenPCR.Runtime.Api
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 4)]
	public struct LayerMask
	{
		private int m_Mask;
		[global::OpenPCR.Migration("61070a2a-240d-4502-81bf-3d17ece2181c")]
		public static int NameToLayer(string layerName)
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Runtime.Api.LayerMask::NameToLayer(System.String):System.Int32", "Unity infrastructure not implemented");
		}
	}
}
