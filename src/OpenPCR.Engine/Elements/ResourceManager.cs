using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System.IO;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class ResourceManager : ManagerSingleton<ResourceManager>
	{
		[global::OpenPCR.Migration("8264f60e-91ef-4054-bfe1-077806bc78dc")]
		public T InstantiateAndGetComponent<T>(eResourceId _resourceId, Transform _parentTransform = null, long _resourceIndex = 0L)
		{
			return LoadImmediately(_resourceId, _parentTransform, _resourceIndex).GetComponent<T>();
		}

		private enum eRequestType
		{
			NONE,
			RESOURCE,
			BUNDLE,
			INSTANTIATE,
			STARTLOAD
		}

		private class RequestBase
		{
			public bool AbleAbandon;
			public bool IsNothingToError = true;
			public virtual eRequestType RequestType => eRequestType.NONE;
		}

		private class RequestLoadResource : RequestBase
		{
			public object LoadedObject;
			public eResourceId Id { get; private set; }
			public long Index { get; private set; }
			public Action<object> FinishCallback { get; private set; }

			public RequestLoadResource(eResourceId _resourceId, long _resourceIndex = 0L, Action<object> _finishCallback = null, bool _ableAbandon = false, bool _isNothingToError = true)
			{
				Id = _resourceId;
				Index = _resourceIndex;
				FinishCallback = _finishCallback;
				AbleAbandon = _ableAbandon;
				IsNothingToError = _isNothingToError;
				LoadedObject = null;
			}
		}

		private class RequestInstantiate : RequestBase
		{
			public eResourceId Id { get; private set; }
			public long Index { get; private set; }
			public Transform ParentTransform { get; private set; }
			public Action<GameObject> FinishCallback { get; private set; }

			public RequestInstantiate(eResourceId _resourceId, long _resourceIndex = 0L, Transform _parentTransform = null, Action<GameObject> _finishCallback = null)
			{
				Id = _resourceId;
				Index = _resourceIndex;
				ParentTransform = _parentTransform;
				FinishCallback = _finishCallback;
			}
		}

		private class RequestLoadBundle : RequestBase
		{
			public string BundleFileName { get; private set; }

			public RequestLoadBundle(eBundleId _bundleId, long _bundleIndex = 0L, bool _ableAbandon = false, bool _isNothingToError = true)
			{
				AbleAbandon = _ableAbandon;
				IsNothingToError = _isNothingToError;
				BundleFileName = ResourceDefineData.GetBundleFilename(_bundleId, _bundleIndex);
			}
		}

		private class StartLoadRequest : RequestBase
		{
			public Action FinishCallback { get; private set; }

			[global::OpenPCR.Migration("cb336366-7aaf-475f-80f3-d60ded8f62a0")]
			public StartLoadRequest(Action _finishCallback)
			{
				FinishCallback = _finishCallback;
			}
		}

		public static Font jpFont;
		private Dictionary<eResourceId, Dictionary<long, object>> dataDic = new Dictionary<eResourceId, Dictionary<long, object>>(new eResourceId_DictComparer());
		private List<RequestBase> requestList = new List<RequestBase>();
		private bool isLoadCoroutineActive;
		private ResourcesManager cuteResourcesMgr;
		private AssetManager assetMgr;
		private List<ResourceIdIndexAndObject> regidentPreinResourceList = new List<ResourceIdIndexAndObject>();
		private List<string> viewPreLoadBundleNameList = new List<string>();
		private Stopwatch stopwatch = new Stopwatch();
		private bool isUnloadUnusedAssetsExecuting;
		public bool IsAllCommonAssetBundleDownloaded { get; set; }

		public class ResourceIdIndexAndObject
		{
			public eResourceId ResourceId { get; private set; }
			public long ResourceIndex { get; private set; }
			public object LoadedObject { get; set; }

			public ResourceIdIndexAndObject(eResourceId _resourceId, long _resourceIndex, object _loadedObject)
			{
				ResourceId = _resourceId;
				ResourceIndex = _resourceIndex;
				LoadedObject = _loadedObject;
			}
		}

		[global::OpenPCR.Migration("dd5e877b-9098-429c-98b9-61d3bc441c39")]
		public ResourceManager()
		{
		}

		[global::OpenPCR.Migration("7d3c489a-8d42-4748-b045-378843257d29")]
		public static bool IsExistResource(eResourceId _resourceId, long _resourceIndex = 0L)
		{
			if (IsUseAssetBundle())
			{
				string bundleFilename = ResourceDefineData.GetBundleFilename(_resourceId, _resourceIndex, _isNothingAble: true);
				if (bundleFilename.IsNullOrEmpty())
				{
					return false;
				}

				return Toolbox.AssetManager.GetAssetHandle(bundleFilename, isWarning: false) != null;
			}

			return false;
		}

		[global::OpenPCR.Migration("7215a582-411d-44a1-b680-efc3380aa1c5")]
		public static bool IsExistResource(eBundleId _bundleId, long _bundleIndex = 0L)
		{
			return global::OpenPCR.PreparedResourceInputBinding.HasBundle(ManagerSingleton<ResourceManager>.Instance, (int)_bundleId, _bundleIndex);
		}

		[global::OpenPCR.Migration("343854bb-1134-4ea6-bc44-6d74d43d37cd")]
		public void UnloadBundleId(eBundleId _bundleId, long _bundleIndex = 0L, bool _isUnloadTypeNoneOnly = false)
		{
			string bundleFilename = ResourceDefineData.GetBundleFilename(_bundleId, _bundleIndex);
			unloadCuteAssetHandle(bundleFilename, _isUnloadTypeNoneOnly);
		}

		private void unloadCuteAssetHandle(string _bundleFilename, bool _isUnloadTypeNoneOnly = false)
		{
			AssetHandle assetHandle = assetMgr.GetAssetHandle(_bundleFilename, isWarning: false);
			if (assetHandle == null)
			{
				return;
			}

			switch (assetHandle.unloadFlag)
			{
				case AssetHandle.unloadType.Common:
					if (!_isUnloadTypeNoneOnly)
					{
						assetHandle.UnloadCommon();
					}

					break;
				case AssetHandle.unloadType.Temp:
					if (!_isUnloadTypeNoneOnly)
					{
						assetHandle.UnloadTemporary();
					}

					break;
				default:
					assetHandle.Unload();
					break;
			}
		}

		[global::OpenPCR.Migration("70df3d0e-b4c5-4dc5-a8de-aa33ad990055")]
		public void AddLoadResourceId(eResourceId _resourceId, long _resourceIndex, Action<object> _finishCallback = null, bool _ableAbandon = false, bool _isNothingToError = true)
		{
			global::OpenPCR.PreparedBattleResourceLoads.AddResource(this, (int)_resourceId, _resourceIndex, _finishCallback, _isNothingToError);
		}

		[global::OpenPCR.Migration("ebac010b-191e-4cb1-ae93-9f5cbf2bc957")]
		public void AddLoadResourceId(eResourceId _resourceId, Action<object> _finishCallback = null, bool _ableAbandon = false, bool _isNothingToError = true)
		{
			AddLoadResourceId(_resourceId, 0L, _finishCallback, _ableAbandon, _isNothingToError);
		}

		[global::OpenPCR.Migration("70c7f25b-4cde-4484-a986-800b5f25936c")]
		public void AddLoadBundleId(eBundleId _bundleId, long _bundleIndex = 0L, bool _ableAbandon = false, bool _isNothingToError = true)
		{
			if (cuteResourcesMgr.IsUseAssetBundle())
			{
				requestList.Add(new RequestLoadBundle(_bundleId, _bundleIndex, _ableAbandon, _isNothingToError));
			}
		}

		[global::OpenPCR.Migration("37e0aa68-3816-4155-9bd4-893e52e29f0c")]
		public void AddInstantiateGameObject(eResourceId _resourceId, long _resourceIndex, Action<GameObject> _finishCallback = null)
		{
			global::OpenPCR.PreparedBattleResourceLoads.AddInstantiate(this, (int)_resourceId, _resourceIndex, null, _finishCallback == null ? null : value => _finishCallback((GameObject)value));
		}

		[global::OpenPCR.Migration("a6f0f286-ead8-45b7-8364-f62af1235db4")]
		public void AddInstantiateGameObject(eResourceId _resourceId, long _resourceIndex, Transform _parentTransform, Action<GameObject> _finishCallback = null)
		{
			global::OpenPCR.PreparedBattleResourceLoads.AddInstantiate(this, (int)_resourceId, _resourceIndex, _parentTransform, _finishCallback == null ? null : value => _finishCallback((GameObject)value));
		}

		[global::OpenPCR.Migration("dc151b90-29e4-4f8c-8ede-2f40896d5005")]
		public void RegistCommonAsset(eBundleId _bundleId, long _resourceIndex = 0L)
		{
			string bundleFilename = ResourceDefineData.GetBundleFilename(_bundleId, _resourceIndex);
			assetMgr.RegistCommonAsset(bundleFilename);
		}

		[global::OpenPCR.Migration("3ef66a81-055f-45b0-a9a9-213ca4d6bfbe")]
		public void StartLoad(Action _finishCallBack = null)
		{
			global::OpenPCR.PreparedBattleResourceLoads.Flush(this, _finishCallBack);
		}

		[global::OpenPCR.Migration("8440c4bc-1ab1-4831-a4cc-37885d5b0de3")]
		public GameObject LoadImmediately(eResourceId _resourceId, Transform _parentTransform = null, long _resourceIndex = 0L)
		{
			return (GameObject)global::OpenPCR.PreparedResourceInputBinding.Instantiate(this, (int)_resourceId, _resourceIndex, _parentTransform);
		}

		[global::OpenPCR.Migration("d5b21164-c002-47ad-bef3-f3ee932b7228")]
		public object LoadResourceImmediately(eResourceId _resourceId, long _resourceIndex = 0L, bool _isNothingToError = true)
		{
			return global::OpenPCR.PreparedBattleResourceLoads.ReadResource(this, (int)_resourceId, _resourceIndex, _isNothingToError);
		}

		[global::OpenPCR.Migration("ea1582b5-7da5-4c36-9e35-cbee0e9a82b0")]
		public object LoadResourceImmediatelyNoPreload(eResourceId _resourceId, long _resourceIndex = 0L, bool _isCacheOnly = false, bool _isNothingToError = true)
		{
			object obj = dataDicGet(_resourceId, _resourceIndex);
			if (obj != null)
			{
				return obj;
			}

			object obj2 = loadResourceImmediatelyNoPreloadPrime(_resourceId, _resourceIndex, _isCacheOnly, _isNothingToError);
			if (obj2 != null)
			{
				dataDicSet(_resourceId, _resourceIndex, obj2);
			}

			return obj2;
		}

		public object loadResourceImmediatelyNoPreloadPrime(eResourceId _resourceId, long _resourceIndex = 0L, bool _isCacheOnly = false, bool _isNothingToError = true)
		{
			Type systemType = ResourceDefineData.GetSystemType(_resourceId);
			string path = ResourceDefineData.GetPath(_resourceId, _resourceIndex);
			if (isLoadFromAssetBundle(_resourceId))
			{
				global::OpenPCR.Runtime.Api.Object obj = assetMgr.LoadObject(path, systemType, isIfFindLoad: true);
				if (obj != null)
				{
					return obj;
				}

				if (_isCacheOnly)
				{
					return null;
				}

				string bundleFilename = ResourceDefineData.GetBundleFilename(_resourceId, _resourceIndex);
				if (!loadAssetBundleImmediatelyByName(bundleFilename, _isNothingToError))
				{
					return null;
				}

				obj = assetMgr.LoadObject(path, systemType, isIfFindLoad: true);
				if (obj == null)
				{
					return null;
				}

				return obj;
			}

			global::OpenPCR.Runtime.Api.Object obj2 = Resources.Load(path, systemType);
			_ = (obj2 == null) & _isNothingToError;
			return obj2;
		}

		[global::OpenPCR.Migration("2860322e-75ab-4ee6-b391-9b43ea2a60f9")]
		private void loadAttachedResources(eResourceId _resourceId, long _resourceIndex = 0L, bool _isImmidate = false, bool _ableAbandon = false, bool _isNothingToError = true)
		{
			ResourceDefineRecord resourceDefineRecord = ResourceDefineData.GetResourceDefineRecord(_resourceId);
			loadAttachedResourcesBundleIdArray(resourceDefineRecord.PreloadBundle, _resourceIndex, _isImmidate, _ableAbandon, _isNothingToError);
			loadAttachedResourcesResourceIdArray(resourceDefineRecord.PreloadResource, _resourceIndex, _isImmidate, _ableAbandon, _isNothingToError);
		}

		[global::OpenPCR.Migration("37f98582-e569-4223-bf58-692c393bc48d")]
		public static long CreateIndex(long _index0, long _index1)
		{
			if (_index1 == -1)
			{
				return _index0;
			}

			return CreateIndexPrime(_index0, _index1);
		}

		public static long CreateIndexPrime(long _index0, long _index1 = 0L)
		{
			return (_index1 << 30) | _index0;
		}

		public static bool IsUseAssetBundle()
		{
			return Toolbox.ResourcesManager.IsUseAssetBundle();
		}

		[global::OpenPCR.Migration("4fee5043-3c52-4947-bae7-128bf4298d33")]
		private IEnumerator loadCoroutine()
		{
			stopwatch.Stop();
			stopwatch.Reset();
			stopwatch.Start();
			while (true)
			{
				int num = requestList.FindIndex((RequestBase _it) => _it.RequestType != eRequestType.STARTLOAD);
				if (num != -1)
				{
					execRequest(requestList[num]);
					requestList.RemoveAt(num);
				}

				int numberOfStartLoad = 0;
				int numberOfOther = 0;
				requestList.ForEach(delegate (RequestBase _it)
				{
					if (_it.RequestType == eRequestType.STARTLOAD)
					{
						numberOfStartLoad++;
					}
					else
					{
						numberOfOther++;
					}
				});
				if (numberOfOther == 0)
				{
					if (numberOfStartLoad == 0)
					{
						break;
					}

					int num2 = requestList.FindIndex((RequestBase _it) => _it.RequestType == eRequestType.STARTLOAD);
					if (num2 != -1)
					{
						execRequest(requestList[num2]);
						requestList.RemoveAt(num2);
					}
				}

				if (stopwatch.Elapsed.TotalSeconds > 0.06666667014360428)
				{
					yield return null;
					stopwatch.Stop();
					stopwatch.Reset();
					stopwatch.Start();
				}
			}

			isLoadCoroutineActive = false;
		}

		private void execRequest(RequestBase _request)
		{
			switch (_request.RequestType)
			{
				case eRequestType.RESOURCE:
					execRequestLoadResource(_request as RequestLoadResource);
					break;
				case eRequestType.BUNDLE:
					execRequestLoadBundle(_request as RequestLoadBundle);
					break;
				case eRequestType.INSTANTIATE:
					execRequestInstantiate(_request as RequestInstantiate);
					break;
				case eRequestType.STARTLOAD:
					(_request as StartLoadRequest).FinishCallback.Call();
					break;
				case eRequestType.NONE:
					break;
			}
		}

		private void execRequestLoadBundle(RequestLoadBundle _request)
		{
			AssetHandle assetHandle = assetMgr.GetAssetHandle(_request.BundleFileName, isWarning: false);
			if (assetHandle != null && !assetHandle.reference)
			{
				loadAssetBundleImmediatelyByName(_request.BundleFileName, _request.IsNothingToError);
				Toolbox.AssetManager.GetAssetBundleObject(_request.BundleFileName);
			}
		}

		private void execRequestLoadResource(RequestLoadResource _request)
		{
			global::OpenPCR.Runtime.Api.Object obj = dataDicGet(_request.Id, _request.Index) as global::OpenPCR.Runtime.Api.Object;
			if (obj == null)
			{
				Type systemType = ResourceDefineData.GetSystemType(_request.Id);
				obj = assetMgr.LoadObject(ResourceDefineData.GetPath(_request.Id, _request.Index), systemType, isIfFindLoad: true);
				if (obj != null)
				{
					dataDicSet(_request.Id, _request.Index, obj);
				}
			}

			if (obj != null)
			{
				_request.FinishCallback.Call(obj);
				return;
			}

			global::OpenPCR.Runtime.Api.Object obj2 = null;
			if (isLoadFromAssetBundle(_request.Id))
			{
				eResourceId id = _request.Id;
				Type systemType2 = ResourceDefineData.GetSystemType(_request.Id);
				long index = _request.Index;
				string bundleFilename = ResourceDefineData.GetBundleFilename(_request.Id, _request.Index);
				loadAssetBundleImmediatelyByName(bundleFilename, _request.IsNothingToError);
				obj2 = assetMgr.LoadObject(ResourceDefineData.GetPath(id, index), systemType2);
			}
			else
			{
				Type systemType3 = ResourceDefineData.GetSystemType(_request.Id);
				obj2 = Resources.Load(ResourceDefineData.GetPath(_request.Id, _request.Index), systemType3);
			}

			dataDicSet(_request.Id, _request.Index, obj2);
			_request.FinishCallback.Call(obj2);
		}

		private void execRequestInstantiate(RequestInstantiate _request)
		{
			object obj = dataDicGet(_request.Id, _request.Index);
			if (obj != null)
			{
				GameObject gameObject = global::OpenPCR.Runtime.Api.Object.Instantiate(obj as GameObject);
				gameObject.transform.SetParent(_request.ParentTransform, worldPositionStays: false);
				_request.FinishCallback.Call(gameObject);
			}
		}

		private bool isLoadFromAssetBundle(eResourceId _resourceId)
		{
			if (ResourceDefineData.GetLoadType(_resourceId) != eResourceLoadType.PREIN)
			{
				return cuteResourcesMgr.IsUseAssetBundle();
			}

			return false;
		}

		private bool loadAssetBundleImmediatelyByName(string _bundleFilename, bool _isNothingToError = true)
		{
			return loadAssetBundleImmediatelyByNameLoadOnly(_bundleFilename, _isNothingToError, _isShader: false);
		}

		private bool loadAssetBundleImmediatelyByNameLoadOnly(string _bundleFileName, bool _isNothingToError, bool _isShader)
		{
			if (assetMgr.HasObjectList(_bundleFileName))
			{
				return true;
			}

			AssetBundle assetBundle = AssetBundle.LoadFromFile(Toolbox.AssetManager.BuildAssetLocalCachePathPreinSupported(_bundleFileName));
			if (assetBundle == null)
			{
				return false;
			}

			assetMgr.SetAssetBundle(_bundleFileName, assetBundle);
			AssetHandle assetHandle = assetMgr.GetAssetHandle(_bundleFileName, isWarning: false);
			if (assetHandle != null)
			{
				assetHandle.reference = true;
			}

			assetMgr.SetObjectList(_bundleFileName, getRegistList(assetBundle, _isShader));
			return true;
		}

		private List<AssetObject> getRegistList(AssetBundle _assetBundle, bool _isShader)
		{
			List<AssetObject> list = new List<AssetObject>();
			if (_isShader)
			{
				global::OpenPCR.Runtime.Api.Object[] array = _assetBundle.LoadAllAssets();
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					if (array[i] is Shader)
					{
						list.Add(new AssetObject("", array[i]));
					}
				}
			}
			else
			{
				string[] allAssetNames = _assetBundle.GetAllAssetNames();
				global::OpenPCR.Runtime.Api.Object[] array2 = _assetBundle.LoadAllAssets();
				int num2 = array2.Length;
				int num3 = allAssetNames.Length;
				for (int j = 0; j < num2; j++)
				{
					for (int k = 0; k < num3; k++)
					{
						string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(allAssetNames[k]);
						if (array2[j].name.ToLower().Equals(fileNameWithoutExtension.ToLower()))
						{
							string kJCOIANBACD = Path.ChangeExtension(allAssetNames[k], ".any");
							list.Add(new AssetObject(kJCOIANBACD, array2[j]));
							break;
						}
					}
				}
			}

			return list;
		}

		private object dataDicGet(eResourceId _resourceId, long _resourceIndex)
		{
			if (dataDic.TryGetValue(_resourceId, out var value) && value.TryGetValue(_resourceIndex, out var value2))
			{
				return value2;
			}

			return null;
		}

		private void dataDicSet(eResourceId _resourceId, long _resourceIndex, object _resourceObject)
		{
			if (!dataDic.TryGetValue(_resourceId, out var value))
			{
				value = new Dictionary<long, object>();
				value[_resourceIndex] = _resourceObject;
				dataDic[_resourceId] = value;
			}
			else if (!dataDic[_resourceId].ContainsKey(_resourceIndex))
			{
				dataDic[_resourceId][_resourceIndex] = _resourceObject;
			}
		}

		private void dataDicSet(eResourceId _resourceId, long _resourceIndex, global::OpenPCR.Runtime.Api.Object _resourceObject)
		{
			dataDicSet(_resourceId, _resourceIndex, (object)_resourceObject);
		}

		protected override void onInit()
		{
			cuteResourcesMgr = Toolbox.ResourcesManager;
			assetMgr = Toolbox.AssetManager;
			regidentPreinResourceList.Clear();
			IsAllCommonAssetBundleDownloaded = false;
			ClearForViewTitle();
			jpFont = Resources.Load<Font>("Fonts/FOT-HummingStd-Eorg");
		}

		public void ClearForViewTitle()
		{
			IsAllCommonAssetBundleDownloaded = false;
			isLoadCoroutineActive = false;
			assetMgr.ClearAssetCacheAssetBundle();
			ClearForViewChange();
		}

		public void ClearForViewChange()
		{
			for (int i = 0; i < regidentPreinResourceList.Count; i++)
			{
				ResourceIdIndexAndObject resource = regidentPreinResourceList[i];
				resource.LoadedObject = dataDicGet(resource.ResourceId, resource.ResourceIndex);
			}

			dataDic.Clear();
			for (int i = 0; i < regidentPreinResourceList.Count; i++)
			{
				ResourceIdIndexAndObject resource = regidentPreinResourceList[i];
				dataDicSet(resource.ResourceId, resource.ResourceIndex, resource.LoadedObject);
			}

			requestList.Clear();
			assetMgr.UnloadAssetAll();
			isUnloadUnusedAssetsExecuting = false;
			KickUnloadUnusedAssets(null);
		}

		private void loadAttachedResourcesBundleIdArray(eBundleId[] _preloadBundloeIdArray, long _resourceIndex, bool _isImmidate, bool _ableAbandon = false, bool _isNothingToError = true)
		{
			if (_preloadBundloeIdArray == null)
			{
				return;
			}

			foreach (eBundleId bundleId in _preloadBundloeIdArray)
			{
				long bundleIndex = ResourceDefineData.ModificationBundleIndex(bundleId, _resourceIndex);
				if (_isImmidate)
				{
					LoadAssetBundleImmediately(bundleId, bundleIndex, _isNothingToError);
				}
				else
				{
					AddLoadBundleId(bundleId, bundleIndex, _ableAbandon, _isNothingToError);
				}
			}
		}

		private void loadAttachedResourcesResourceIdArray(eResourceId[] _preloadResourceIdArray, long _resourceIndex, bool _isImmidate, bool _ableAbandon = false, bool _isNothingToError = true)
		{
			if (_preloadResourceIdArray == null)
			{
				return;
			}

			foreach (eResourceId resourceId in _preloadResourceIdArray)
			{
				long resourceIndex = ResourceDefineData.ModificationResourceIndex(resourceId, _resourceIndex);
				if (_isImmidate)
				{
					LoadResourceImmediatelyNoPreload(resourceId, resourceIndex, _isCacheOnly: false, _isNothingToError);
				}
				else
				{
					requestList.Add(new RequestLoadResource(resourceId, resourceIndex, null, _ableAbandon, _isNothingToError));
				}
			}
		}

		[global::OpenPCR.Migration("2333a489-4d1a-428a-abb0-b5b8866b80e9")]
		public void LoadAssetBundleImmediately(eBundleId _bundleId, long _bundleIndex = 0L, bool _isNothingToError = true)
		{
			if (cuteResourcesMgr.IsUseAssetBundle())
			{
				string bundleFilename = ResourceDefineData.GetBundleFilename(_bundleId, _bundleIndex);
				loadAssetBundleImmediatelyByName(bundleFilename, _isNothingToError);
			}
		}

		public static long[] SeparteIndex(long _mixtureIndex)
		{
			long[] array = new long[2];
			array[1] = (int)((_mixtureIndex >> 30) & 0xFFFFF);
			array[0] = (int)(_mixtureIndex & 0x3FFFFFFF);
			return array;
		}

		public static string MakePathNameByIndex(string _formatString, long _mixtureIndex)
		{
			long[] array = SeparteIndex(_mixtureIndex);
			return string.Format(_formatString, array[0], array[1]);
		}

		public IEnumerator CoroutineUnloadUnusedAssets(Action _finishCallback = null)
		{
			yield return Resources.UnloadUnusedAssets();
			_finishCallback.Call();
		}

		public void KickUnloadUnusedAssets(Action _finishCallback = null)
		{
			if (isUnloadUnusedAssetsExecuting)
			{
				_finishCallback.Call();
				return;
			}

			isUnloadUnusedAssetsExecuting = true;
			StartCoroutine(CoroutineUnloadUnusedAssets(delegate
			{
				_finishCallback.Call();
				isUnloadUnusedAssetsExecuting = false;
			}));
		}
	}
}
