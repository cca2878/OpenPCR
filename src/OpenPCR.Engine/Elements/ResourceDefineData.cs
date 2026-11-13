using global::OpenPCR.AnimKit.Scene;
using global::OpenPCR.Runtime.Api;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Elements
{
	public class ResourceDefineData : Singleton<ResourceDefineData>
	{
		public class TypeData
		{
			public Type SystemType { get; private set; }
			public string Extention { get; private set; }

			public TypeData(Type _systemType, string _extention)
			{
				SystemType = _systemType;
				Extention = _extention;
			}
		}

		private static readonly List<string> withoutStringForBundleNameList = new List<string>
		{
			".atlas",
			".skel",
			"_Alpha",
			"_Atlas",
			"_Material",
			"_SkeletonData",
			".asset",
			"_Asset",
			"_Additive",
			"_TypeA",
			"_TypeB",
			"_TypeC",
			"_TypeD",
			"_TypeE",
			"_TypeF",
			"_TypeG",
			"_TypeH",
			"_TypeI",
			"_TypeJ",
			"_SpriteGrayScaleAlpha"
		};
		private static readonly string[] withoutStringForBundleNameIgnoreArray = new string[1]
		{
			"_Mask_Alpha"
		};
		public static readonly Dictionary<eResourceDataType, TypeData> TypeDataDictionary = new Dictionary<eResourceDataType, TypeData>
		{
			{
				eResourceDataType.PREFAB,
				new TypeData(typeof(GameObject), ".prefab")
			},
			{
				eResourceDataType.TEXTURE,
				new TypeData(typeof(Texture2D), ".png")
			},
			{
				eResourceDataType.TEXT,
				new TypeData(typeof(object), ".txt")
			},
			{
				eResourceDataType.CSV,
				new TypeData(typeof(object), ".csv")
			},
			{
				eResourceDataType.SPINE,
				new TypeData(typeof(SkeletonDataAsset), ".asset")
			},
			{
				eResourceDataType.MATERIAL,
				new TypeData(typeof(Material), ".mat")
			},
			{
				eResourceDataType.FONT,
				new TypeData(typeof(object), ".otf")
			},
			{
				eResourceDataType.BYTES,
				new TypeData(typeof(object), ".bytes")
			},
			{
				eResourceDataType.CDB,
				new TypeData(typeof(object), ".cdb")
			}
		};
		private Dictionary<eResourceId, ResourceDefineRecord> resourceDefineAllDictionary;
		private Dictionary<eBundleId, BundleDefineRecord> bundleDefineAllDictionary;
		private Dictionary<eBundleId, Dictionary<long, BundleRelationRecord>> bundleRelationDictionary;


		public static ResourceDefineRecord GetResourceDefineRecord(eResourceId _resourceId)
		{
			if (Singleton<ResourceDefineData>.Instance != null)
				_ = Singleton<ResourceDefineData>.Instance.resourceDefineAllDictionary.Count;
			if (Singleton<ResourceDefineData>.Instance.resourceDefineAllDictionary.TryGetValue(_resourceId, out var value))
				return value;
			return null;
		}

		public static BundleRelationRecord GetBundleRelationRecord(BundleRelationBundleIdAndIndex _bundle)
		{
			return GetBundleRelationRecord(_bundle.BundleId, _bundle.Index);
		}

		public static BundleRelationRecord GetBundleRelationRecord(eBundleId _bundleId, long _bundleIndex = 0L)
		{
			if (Singleton<ResourceDefineData>.Instance.bundleRelationDictionary.TryGetValue(_bundleId, out var indexDictionary) && indexDictionary.TryGetValue(_bundleIndex, out var value))
				return value;
			return null;
		}

		public static string GetBundleFilenameFormat(eBundleId _bundleId, bool _isNothingAble = false)
		{
			Dictionary<eBundleId, BundleDefineRecord> dictionary = Singleton<ResourceDefineData>.Instance.bundleDefineAllDictionary;
			BundleDefineRecord value = null;
			if (dictionary.TryGetValue(_bundleId, out value))
			{
				return value.PathName;
			}

			return null;
		}

		public static string GetBundleFilename(eBundleId _bundleId, long _resourceIndex = 0L, bool _isNothingAble = false)
		{
			string bundleFilenameFormat = GetBundleFilenameFormat(_bundleId, _isNothingAble);
			if (bundleFilenameFormat == null)
			{
				return null;
			}

			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("a/");
			stringBuilder.Append(ResourceManager.MakePathNameByIndex(bundleFilenameFormat, _resourceIndex));
			stringBuilder.Append(".unity3d");
			return stringBuilder.ToString();
		}

		public static string GetPath(eResourceId _resourceId, long _resourceIndex = 0L, bool _isNothingAble = false)
		{
			ResourceDefineRecord resourceDefineRecord = GetResourceDefineRecord(_resourceId);
			if (resourceDefineRecord != null)
			{
				return ResourceManager.MakePathNameByIndex(resourceDefineRecord.PathName, _resourceIndex);
			}

			return null;
		}

		public static eResourceDataType GetType(eResourceId _resourceId)
		{
			return GetResourceDefineRecord(_resourceId).Type;
		}

		public static eResourceLoadType GetLoadType(eResourceId _resourceId)
		{
			return GetResourceDefineRecord(_resourceId).LoadType;
		}

		public static string GetBundleFilename(eResourceId _resourceId, long _resourceIndex = 0L, bool _isNothingAble = false)
		{
			return ResourceDefinePathToAssetName(GetPath(_resourceId, _resourceIndex, _isNothingAble));
		}

		public static long ModificationResourceIndex(eResourceId _resourceId, long _resourceIndex)
		{
			if (!isIncludeNumberInPathFormat(GetResourceDefineRecord(_resourceId).PathName))
			{
				return 0L;
			}

			return _resourceIndex;
		}

		public static long ModificationBundleIndex(eBundleId _bundleId, long _resourceIndex)
		{
			if (!isIncludeNumberInPathFormat(GetBundleFilenameFormat(_bundleId)))
			{
				return 0L;
			}

			return _resourceIndex;
		}

		public static Type GetSystemType(eResourceId _resourceId)
		{
			return GetSystemTypeFromResourceDataType(GetType(_resourceId));
		}

		public static Type GetSystemTypeFromResourceDataType(eResourceDataType _type)
		{
			TypeData value = null;
			if (TypeDataDictionary.TryGetValue(_type, out value))
			{
				return value.SystemType;
			}

			return typeof(object);
		}

		public static string GetFileNameWithoutExtensionSpecial(string _assetPath, int _folder_path_index_category = 3)
		{
			string[] array = _assetPath.Split(new char[1] { '/' });
			for (int i = _folder_path_index_category + 1; i < array.Length - 1; i++)
			{
				string text = array[i];
				if (Regex.IsMatch(text, ".*_BUNDLE$") && i <= array.Length - 2)
				{
					return text.Replace("_BUNDLE", "").ToLower();
				}

				if (Regex.IsMatch(text, ".*_BUNDLEROOT$") && i <= array.Length - 3)
				{
					return array[i + 1].ToLower();
				}
			}

			return GetAssetBundleFilenameFromPath(_assetPath);
		}

		public static string GetAssetBundleFilenameFromPath(string _assetPath)
		{
			string text = Path.GetFileNameWithoutExtension(_assetPath);
			bool flag = false;
			string[] array = _assetPath.Split(new char[1] { '/' });
			if (Regex.IsMatch(array[array.Length - 2], ".*_BUNDLENAMECUT$"))
			{
				flag = true;
			}

			string[] array2 = withoutStringForBundleNameIgnoreArray;
			foreach (string text2 in array2)
			{
				int num = text.IndexOf(text2);
				if (num != -1 && num + text2.Length == text.Length)
				{
					return text.ToLower();
				}
			}

			foreach (string withoutStringForBundleName in withoutStringForBundleNameList)
			{
				int num2 = text.IndexOf(withoutStringForBundleName);
				if (num2 != -1 && num2 + withoutStringForBundleName.Length == text.Length)
				{
					text = text.Substring(0, num2);
				}
			}

			text = text.ToLower();
			if (flag)
			{
				string value = "_cut_";
				int num3 = text.IndexOf(value);
				if (num3 != -1)
				{
					text = text.Substring(0, num3);
				}
			}

			return text;
		}

		private static bool isIncludeNumberInPathFormat(string _path)
		{
			return _path.IndexOf('{') != -1;
		}

		public static string ResourceDefinePathToAssetName(string _path)
		{
			if (_path.IsNullOrEmpty())
				return null;
			string[] array = _path.Split(new char[] { '/' });
			if (array.Length <= 1)
				return null;
			string value = array[0].ToLower();
			string extension = Path.GetExtension(_path);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("a/");
			stringBuilder.Append(value);
			stringBuilder.Append("_");
			stringBuilder.Append(GetFileNameWithoutExtensionSpecial(_path, 0));
			if (!_path.Contains("_BUNDLE/") && !_path.Contains("_BUNDLEROOT/"))
				stringBuilder.Append(extension);
			stringBuilder.Append(".unity3d");
			return stringBuilder.ToString();
		}
	}
}
