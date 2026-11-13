using Common;

namespace Elements
{
	public class Singleton<T> : IDestroyInstance where T : class, IDestroyInstance, new()
	{
		protected Singleton()
		{
			Instance = null;
		}

		public void DestroyInstance()
		{
			Instance = null;
		}

		public static T Instance { get; private set; }
	}
}
