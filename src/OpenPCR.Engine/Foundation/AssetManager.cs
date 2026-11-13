using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System.IO;
using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Elements;

namespace OpenPCR.Foundation
{
	public class AssetManager : MonoBehaviour, IManager
	{
		private Dictionary<string, AssetHandle> handleDictionary = new Dictionary<string, AssetHandle>();
		private Dictionary<string, AssetBundleObject> objectDictionary = new Dictionary<string, AssetBundleObject>();
		private Dictionary<string, AssetBundleEditTag.eDlCategory> stringToAssetBundleDownloadCategoryDictionary;
		public AssetBundleEditTag.eDlCategory GetDownloadCategoryEnum(string _downloadCategoryString)
		{
			return stringToAssetBundleDownloadCategoryDictionary.TryGetValue(_downloadCategoryString, out var value) ? value : AssetBundleEditTag.eDlCategory.NONE;
		}

		public Dictionary<string, eDlStorage> DirectoryToDlStorageDictionary;
		public string[] StorageToPathArray = new string[2];
		private object _lockObjectForManifestHashStore = new object();


		public void SetAssetBundle(string filename, AssetBundle assetbundle)
		{
			if (objectDictionary.TryGetValue(filename, out AssetBundleObject value))
			{
				for (int i = 0; i < value.objectArray.Count; i++)
				{
					global::OpenPCR.Runtime.Api.Object.DestroyImmediate(value.objectArray[i].baseObject, allowDestroyingAssets: true);
				}

				value.objectArray.Clear();
				value.assetBundle.Unload(unloadAllLoadedObjects: true);
				value.assetBundle = assetbundle;
			}
			else
			{
				value = new AssetBundleObject();
				value.assetBundle = assetbundle;
				objectDictionary.Add(filename, value);
			}
		}

		public void SetObjectList(string filename, List<AssetObject> objectList)
		{
			if (objectDictionary.TryGetValue(filename, out AssetBundleObject value))
			{
				value.objectArray = objectList;
			}
		}

		public bool HasObjectList(string _assetName)
		{
			AssetBundleObject value;
			return objectDictionary.TryGetValue(_assetName, out value);
		}

		public global::OpenPCR.Runtime.Api.Object LoadObject(string objectName, Type type, bool isIfFindLoad = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(objectName.ToLower() + ".");
			foreach (AssetBundleObject value in objectDictionary.Values)
			{
				int count = value.objectArray.Count;
				for (int i = 0; i < count; i++)
				{
					if (value.objectArray[i].baseObject != null && value.objectArray[i].basePath.IndexOf(stringBuilder.ToString()) != -1 && (value.objectArray[i].baseObject.GetType() == type || value.objectArray[i].baseObject.GetType().IsSubclassOf(type)))
					{
						return value.objectArray[i].baseObject;
					}
				}
			}

			return null;
		}

		public void RegistCommonAsset(string assetName)
		{
			AssetHandle value = null;
			if (handleDictionary.TryGetValue(assetName, out value))
			{
				value.unloadFlag = AssetHandle.unloadType.Common;
			}
		}

		public AssetHandle GetAssetHandle(string assetName, bool isWarning = true)
		{
			AssetHandle value = null;
			if (!handleDictionary.TryGetValue(assetName, out value))
			{
			}

			return value;
		}

		public AssetBundleObject GetAssetBundleObject(string assetName)
		{
			return objectDictionary.TryGetValue(assetName, out var value) ? value : null;
		}

		public bool FileExists(string _path) => System.IO.File.Exists(_path);
		public string BuildAssetLocalCachePathPreinSupported(string _assetName)
		{
			string path = BuildAssetLocalCachePath(_assetName, false, false, false);
			if (FileExists(path))
				return path;
			return BuildAssetLocalCachePath(_assetName, true, false, false);
		}

		public static string BuildAssetLocalCachePath(string _assetName, bool _isPreinAssetBundle = false, bool _isAssetNameOnly = false, bool _isSaveForDownload = false)
		{
			string filename = System.IO.Path.GetFileName(_assetName);
			string directory = _assetName.Substring(0, _assetName.Length - filename.Length);
			return BuildAssetLocalCachePath(directory, filename, _isPreinAssetBundle, _isAssetNameOnly, _isSaveForDownload);
		}

		public static string BuildAssetLocalCachePath(string _directory, string _filename, bool _isPreinAssetBundle, bool _isAssetNameOnly = false, bool _isSaveForDownload = false)
		{
			StringBuilder builder = buildRootAssetLocalCachePath(_directory, _isPreinAssetBundle, _isAssetNameOnly, _isSaveForDownload);
			builder.Append(_directory);
			builder.Append(_filename);
			return builder.ToString();
		}

		private static StringBuilder buildRootAssetLocalCachePath(string _directory, bool _isPreinAssetBundle, bool _isAssetNameOnly, bool _isSaveForDownload)
		{
			StringBuilder builder = new StringBuilder();
			if (_isAssetNameOnly)
				return builder;
			if (_isPreinAssetBundle)
				throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.Foundation.AssetManager::buildRootAssetLocalCachePath(System.String,System.Boolean,System.Boolean,System.Boolean):System.Text.StringBuilder: runtime capability is unavailable.");
			string directory = _directory.Substring(0, _directory.Length - 1);
			AssetManager manager = Toolbox.AssetManager;
			builder.Append(manager.StorageToPathArray[(int)manager.DirectoryToDlStorageDictionary[directory]]);
			builder.Append("/");
			return builder;
		}

		public void ClearAssetCacheAssetBundle()
		{
			foreach (KeyValuePair<string, AssetBundleObject> entry in objectDictionary)
			{
				AssetBundleObject value = entry.Value;
				List<AssetObject> objectArray = value.objectArray;
				if (objectArray != null)
					for (int i = 0; i < objectArray.Count; i++)
						global::OpenPCR.Runtime.Api.Object.DestroyImmediate(objectArray[i].baseObject, true);
				if (value.assetBundle != null)
					value.assetBundle.Unload(true);
			}

			objectDictionary.Clear();
			handleDictionary.Clear();
		}

		public void UnloadAssetAll()
		{
			foreach (KeyValuePair<string, AssetHandle> entry in handleDictionary)
				entry.Value.Unload();
			List<string> unloadList = new List<string>();
			foreach (KeyValuePair<string, AssetBundleObject> entry in objectDictionary)
				if (!handleDictionary.ContainsKey(entry.Key) || !handleDictionary[entry.Key].reference)
					unloadList.Add(entry.Key);
			unloadList.ForEach(UnloadAssetBundle);
		}

		public void UnloadAssetBundle(string assetName)
		{
			if (objectDictionary.TryGetValue(assetName, out var value))
			{
				int count = value.objectArray.Count;
				for (int i = 0; i < count; i++)
					global::OpenPCR.Runtime.Api.Object.DestroyImmediate(value.objectArray[i].baseObject, true);
				value.objectArray.Clear();
				if (value.assetBundle != null)
				{
					value.assetBundle.Unload(true);
					value.assetBundle = null;
				}

				objectDictionary.Remove(assetName);
			}
		}
	}
}
