namespace OpenPCR.Runtime.Api.Helpers
{
	[global::OpenPCR.ImplicitMigration("8642fea6-98c5-4019-90ea-29e89b192de3")]
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 1)]
	public struct MathfInternal
	{
		// Mutable static values are not constant-folded.
		public static float FloatMinNormal = 1.17549435E-38f;
		public static float FloatMinDenormal = global::System.Single.Epsilon;

		public static bool IsFlushToZeroEnabled = (FloatMinDenormal == 0);
	}
}
