using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace OpenPCR.Foundation
{
	public class ResourcesManager : MonoBehaviour, IManager
	{
		private bool useAssetBundle = true;

		public ResourcesManager()
		{
			throw new global::OpenPCR.MigrationPendingException("OpenPCR.Foundation.ResourcesManager::.ctor()", "body migration pending");
		}

		[global::OpenPCR.Migration("88616877-99e7-481a-8255-d6345156f015")]
		public bool IsUseAssetBundle()
		{
			return useAssetBundle;
		}
	}
}
