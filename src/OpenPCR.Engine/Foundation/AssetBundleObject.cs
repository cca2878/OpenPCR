using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenPCR.Foundation
{
	public class AssetBundleObject
	{
		public AssetBundle assetBundle { get; set; }
		public List<AssetObject> objectArray { get; set; }

		public AssetBundleObject()
		{
			assetBundle = null;
			objectArray = new List<AssetObject>();
		}
	}
}
