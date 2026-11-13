namespace Elements
{
	public abstract class ManagerSingleton<T> : ManagerBase
	{
		protected abstract void onInit();



		public sealed override void DestroyInstance()
		{
			Instance = default(T);
		}

		public static T Instance { get; private set; }
	}
}
