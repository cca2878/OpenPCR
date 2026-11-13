using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Foundation
{
	public class OverrideApplication
	{
		public static int targetFrameRate
		{
			get => global::OpenPCR.Runtime.Api.Application.targetFrameRate;
			[global::OpenPCR.Migration("45efb4b1-365f-43a5-8506-fe718be0d7e9")]
			set => global::OpenPCR.Runtime.Api.Application.targetFrameRate = value;
		}
	}
}
