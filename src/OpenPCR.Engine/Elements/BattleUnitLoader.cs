using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Elements
{
	public class BattleUnitLoader : Singleton<BattleUnitLoader>
	{
		public List<BundleRelationBundleIdAndIndex> loadedBundleList = new List<BundleRelationBundleIdAndIndex>();
		public List<BundleRelationBundleIdAndIndex> keepBundleList = new List<BundleRelationBundleIdAndIndex>();
		private List<int> enemyPrefabIdList = new List<int>();
		public BattleUnitLoader()
		{
		}


		[global::OpenPCR.Migration("591a0973-fc01-4540-b58d-f6cde0c5b1f6")]
		public static void AddLoadAcnEffectAndPrefab()
		{
			ResourceManager instance = ManagerSingleton<ResourceManager>.Instance;
			if (ResourceManager.IsUseAssetBundle())
			{
				if (ResourceManager.IsExistResource(eBundleId.BATTLE_EFFECT_ACN_MATERIAL, 0L))
					instance.AddLoadBundleId(eBundleId.BATTLE_EFFECT_ACN_MATERIAL, 0L);
				if (ResourceManager.IsExistResource(eBundleId.BATTLE_EFFECT_ACN_PREFAB, 0L))
					instance.AddLoadBundleId(eBundleId.BATTLE_EFFECT_ACN_PREFAB, 0L);
			}
		}

		[global::OpenPCR.Migration("a58a4475-4f4b-4cd3-a157-bf4804bbfcc4")]
		public void AddLoadResource(int _unitPrefabId, Action<GameObject> _action = null, bool _isWaveEnemy = false, bool _isInstantiate = false)
		{
			global::OpenPCR.PreparedBattleResourceLoads.Add(this, _unitPrefabId, _action == null ? null : value => _action((GameObject)value), _isWaveEnemy, _isInstantiate);
		}

		[global::OpenPCR.Migration("af76933f-160b-4f7f-93b7-beb9efc5c5ec")]
		public void AddLoadEffect(eResourceId _resourceId, Action<object> _action = null)
		{ ManagerSingleton<ResourceManager>.Instance.AddLoadResourceId(_resourceId, _action); }

		[global::OpenPCR.Migration("1213089f-7c29-4347-9761-ec790b086e19")]
		public void AddLoadEffect(eResourceId _resourceId, long _resourceIndex, Action<object> _action = null)
		{ ManagerSingleton<ResourceManager>.Instance.AddLoadResourceId(_resourceId, _resourceIndex, _action); }

		[global::OpenPCR.Migration("8bb0cf56-3681-4cf2-bc71-8475f7f0e1a6")]
		public object LoadEffectImmidatly(eResourceId _resourceId, long _resourceIndex)
		{
			List<BundleRelationBundleIdAndIndex> list = collectBundleList(eBundleId.ALL_FXSK, _resourceIndex);
			list.ForEach(loadBundleImmediately);
			keepBundleList.AddRangeIfNoContains(list);
			return ManagerSingleton<ResourceManager>.Instance.LoadResourceImmediately(_resourceId, _resourceIndex);
		}

		private List<BundleRelationBundleIdAndIndex> collectBundleList(eBundleId _rootBundleId, long _rootBundleIndex)
		{
			List<BundleRelationBundleIdAndIndex> list = new List<BundleRelationBundleIdAndIndex>();
			list.Add(new BundleRelationBundleIdAndIndex(_rootBundleId, _rootBundleIndex));
			List<BundleRelationBundleIdAndIndex> list2 = new List<BundleRelationBundleIdAndIndex>();
			list2.AddRangeIfNoContains(list);
			List<BundleRelationBundleIdAndIndex> list3 = list;
			while (true)
			{
				List<BundleRelationBundleIdAndIndex> list4 = new List<BundleRelationBundleIdAndIndex>();
				for (int i = 0; i < list3.Count; i++)
				{
					BundleRelationRecord bundleRelationRecord = ResourceDefineData.GetBundleRelationRecord(list3[i]);
					if (bundleRelationRecord != null)
					{
						for (int j = 0; j < bundleRelationRecord.ChildBundleIdAndIndexArray.Length; j++)
						{
							list4.AddIfNoContains(bundleRelationRecord.ChildBundleIdAndIndexArray[j]);
						}
					}
				}

				if (list4.Count == 0)
				{
					break;
				}

				list2.AddRangeIfNoContains(list4);
				list3 = list4;
			}

			list2.Reverse();
			List<BundleRelationBundleIdAndIndex> list5 = new List<BundleRelationBundleIdAndIndex>();
			while (list2.Count != 0)
			{
				int count = list5.Count;
				for (int num = list2.Count - 1; num >= 0; num--)
				{
					BundleRelationBundleIdAndIndex bundleRelationBundleIdAndIndex = list2[num];
					BundleRelationRecord bundleRelationRecord2 = ResourceDefineData.GetBundleRelationRecord(bundleRelationBundleIdAndIndex);
					if (bundleRelationRecord2 == null || !ContainsBundleListArray(list2, bundleRelationRecord2.ChildBundleIdAndIndexArray))
					{
						list5.Add(bundleRelationBundleIdAndIndex);
						list2.RemoveAt(num);
						break;
					}
				}

				if (count == list5.Count)
				{
					break;
				}
			}

			return list5;
		}

		private bool ContainsBundleListArray(List<BundleRelationBundleIdAndIndex> _fromList, BundleRelationBundleIdAndIndex[] _checkArray)
		{
			for (int i = 0; i < _fromList.Count; i++)
			{
				BundleRelationBundleIdAndIndex bundleRelationBundleIdAndIndex = _fromList[i];
				for (int j = 0; j < _checkArray.Length; j++)
				{
					if (bundleRelationBundleIdAndIndex.Equals(_checkArray[j]))
					{
						return true;
					}
				}
			}

			return false;
		}

		private void loadBundleImmediately(BundleRelationBundleIdAndIndex _bundleIdAndIndex)
		{
			ManagerSingleton<ResourceManager>.Instance.LoadAssetBundleImmediately(_bundleIdAndIndex.BundleId, _bundleIdAndIndex.Index);
			loadedBundleList.AddIfNoContains(_bundleIdAndIndex);
		}
	}
}
