using global::OpenPCR.Runtime.Api;
using System;

namespace Elements
{
	[Serializable]
	public class ResourceDefineRecord
	{
		[global::OpenPCR.Runtime.Api.SerializeField]
		public eResourceId ResourceId;
		[global::OpenPCR.Runtime.Api.SerializeField]
		public string PathName;
		[global::OpenPCR.Runtime.Api.SerializeField]
		public eResourceDataType Type;
		[global::OpenPCR.Runtime.Api.SerializeField]
		public eResourceLoadType LoadType;
		[global::OpenPCR.Runtime.Api.SerializeField]
		public eResourceId[] PreloadResource;
		[global::OpenPCR.Runtime.Api.SerializeField]
		public eBundleId[] PreloadBundle;
		public ResourceDefineRecord(eResourceId _resourceId, string _pathName, eResourceDataType _dataType, eResourceLoadType _loadType, eResourceId[] _preloadResource, eBundleId[] _preloadBundle)
		{
			ResourceId = _resourceId;
			PathName = _pathName;
			Type = _dataType;
			LoadType = _loadType;
			PreloadResource = _preloadResource;
			PreloadBundle = _preloadBundle;
		}
	}
}
