using System.Collections.Generic;

namespace Common
{
	public static class SingletonManager
	{
		private static List<IDestroyInstance> singletonList = new List<IDestroyInstance>();

		public static void OnCreateinstance(IDestroyInstance _instance)
		{
			singletonList.Add(_instance);
		}
	}
}
