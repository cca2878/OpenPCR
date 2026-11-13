using global::OpenPCR.Foundation;
using System.IO;
using System.Runtime.CompilerServices;

namespace OpenPCR.Foundation
{
	public class AssetHandle
	{
		public enum AssetType : byte
		{
			Manifests,
			AppResBundle,
			AssetBundle,
			Sound,
			Movie
		}

		public enum unloadType : byte
		{
			None,
			Common,
			Temp
		}

		public string name { get; }
		public ulong XXHash64Number { get; }
		public string Md5 { get; private set; }
		public AssetBundleEditTag.eDlCategory DlCategory { get; }
		public int datasize { get; }

		private bool isRedownloadAsset;
		private bool checkedRedownload;
		public bool isReDownloadAsset
		{
			get
			{
				if (checkedRedownload)
					return isRedownloadAsset;
				throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.AssetHandle::get_isReDownloadAsset():System.Boolean: runtime capability is unavailable.");
			}

			set
			{
				isRedownloadAsset = value;
				checkedRedownload = true;
			}
		}

		public AssetType assetType { get; }
		public bool reference { get; set; }
		public unloadType unloadFlag { get; set; }

		public AssetHandle(string _name, string _md5, string _xxhash64, string _stringCategory, string _size, AssetType _assetType)
		{
			AssetManager manager = Toolbox.AssetManager;
			name = _name;
			assetType = _assetType;
			DlCategory = manager.GetDownloadCategoryEnum(_stringCategory);
			Md5 = _md5;
			XXHash64Number = XXHash64Utility.XXHash64StringToU64(_xxhash64);
			unloadFlag = unloadType.None;
			if (ReferenceEquals(Toolbox.AssetManager, null))
				throw new System.NullReferenceException();
			datasize = int.TryParse(_size, out var result) ? result : 0;
			if (string.IsNullOrEmpty(_xxhash64))
				isReDownloadAsset = true;
		}

		public bool Unload()
		{
			if (unloadFlag == unloadType.None && reference && IsAssetBundle())
			{
				reference = false;
				Toolbox.AssetManager.UnloadAssetBundle(name);
			}

			return true;
		}

		public bool UnloadTemporary()
		{
			if (unloadFlag == unloadType.Temp && reference && IsAssetBundle())
			{
				reference = false;
				Toolbox.AssetManager.UnloadAssetBundle(name);
			}

			return true;
		}

		public bool UnloadCommon()
		{
			if (unloadFlag == unloadType.Common && reference && IsAssetBundle())
			{
				reference = false;
				Toolbox.AssetManager.UnloadAssetBundle(name);
			}

			return true;
		}

		public bool IsAssetBundle()
		{
			if (assetType != AssetType.AssetBundle)
			{
				return assetType == AssetType.AppResBundle;
			}

			return true;
		}
	}
}
