using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Runtime.Api
{
	public sealed class MaterialPropertyBlock
	{
		[global::OpenPCR.Migration("01340c35-ae46-4974-bb5b-70f38979ec98")]
		public MaterialPropertyBlock()
		{
			global::OpenPCR.Presentation.Noop();
		}
		[global::OpenPCR.Migration("4720fc02-439a-4c81-873e-74f1cb86df6b")]
		public void SetColor(int nameID, global::OpenPCR.Runtime.Api.Color value)
		{
			global::OpenPCR.Presentation.Noop();
		}
	}
}
