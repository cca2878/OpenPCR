using global::OpenPCR.Runtime.Api;
using System;

namespace Elements
{
	[Serializable]
	public class BundleRelationBundleIdAndIndex : IEquatable<BundleRelationBundleIdAndIndex>
	{
		[global::OpenPCR.Runtime.Api.SerializeField]
		public eBundleId BundleId;
		[global::OpenPCR.Runtime.Api.SerializeField]
		public long Index;
		public BundleRelationBundleIdAndIndex(eBundleId _bundleId, long _index = 0L)
		{
			BundleId = _bundleId;
			Index = _index;
		}

		public bool Equals(BundleRelationBundleIdAndIndex _it)
		{
			if (_it == null)
			{
				return false;
			}

			if (BundleId == _it.BundleId)
			{
				return Index == _it.Index;
			}

			return false;
		}
	}
}
