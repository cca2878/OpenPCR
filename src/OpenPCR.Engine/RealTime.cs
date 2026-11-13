using global::OpenPCR.Runtime.Api;

namespace OpenPCR.UICompat
{

	public class RealTime : MonoBehaviour
	{
		public static float time => Time.unscaledTime;
		public static float deltaTime => Time.unscaledDeltaTime;


	}

}
