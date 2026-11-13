using global::OpenPCR.Runtime.Api;
namespace Elements
{
	[global::System.Serializable]
	public class BundleDefineRecord
	{
		[global::OpenPCR.Runtime.Api.SerializeField]
		public eBundleId BundleId;
		[global::OpenPCR.Runtime.Api.SerializeField]
		public string PathName;
		public BundleDefineRecord(eBundleId _bundleId, string _pathName)
		{
			BundleId = _bundleId;
			PathName = _pathName;
		}
	}
}
