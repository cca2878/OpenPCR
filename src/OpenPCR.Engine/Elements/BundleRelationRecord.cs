using global::OpenPCR.Runtime.Api;
namespace Elements
{
	[global::System.Serializable]
	public class BundleRelationRecord
	{
		[global::OpenPCR.Runtime.Api.SerializeField]
		public eBundleId ParentBundleId;
		[global::OpenPCR.Runtime.Api.SerializeField]
		public long ParentBundleIndex;
		[global::OpenPCR.Runtime.Api.SerializeField]
		public BundleRelationBundleIdAndIndex[] ChildBundleIdAndIndexArray;
		public BundleRelationRecord(eBundleId _parentBundleId, long _parentBundleIndex, BundleRelationBundleIdAndIndex[] _childBundleIdAndIndexArray)
		{
			ParentBundleId = _parentBundleId;
			ParentBundleIndex = _parentBundleIndex;
			ChildBundleIdAndIndexArray = _childBundleIdAndIndexArray;
		}
	}
}
