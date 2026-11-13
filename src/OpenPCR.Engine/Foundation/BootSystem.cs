using global::OpenPCR.Runtime.Api;
namespace OpenPCR.Foundation
{
	public class BootSystem : global::OpenPCR.Runtime.Api.MonoBehaviour
	{
		[global::OpenPCR.Runtime.Api.SerializeField]
		private static bool _skipBoot;


		[global::OpenPCR.Migration("8409cdf0-e0f6-4594-9d0c-8313dff53b8b")]
		public static bool GetSkipBoot() => _skipBoot;
	}
}
