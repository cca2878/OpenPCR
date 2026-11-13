using global::OpenPCR.Runtime.Api;
using Common;

namespace Elements
{
	public abstract class ManagerBase : MonoBehaviour, IDestroyInstance
	{


		public abstract void DestroyInstance()
		;
	}
}
