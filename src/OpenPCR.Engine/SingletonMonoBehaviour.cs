using global::OpenPCR.Runtime.Api;
using Common;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour, IDestroyInstance where T : MonoBehaviour, IDestroyInstance
{
	public static T Instance { get; private set; }

	[global::OpenPCR.Migration("a8e8a16f-92e9-435e-84cc-129ea5280dc3")]
	protected SingletonMonoBehaviour()
	{
	}

	protected virtual void Awake()
	{
		if ((Object)(object)Instance == null)
		{
			Instance = GetComponent<T>();
			SingletonManager.OnCreateinstance(Instance);
		}
		OnAwake();
	}

	protected virtual void OnAwake()
	{
	}

	public void DestroyInstance()
	{
		Instance = default(T);
	}
}
