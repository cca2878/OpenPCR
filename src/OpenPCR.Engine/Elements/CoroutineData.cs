using System.Collections.Generic;

namespace Elements
{
	public class CoroutineData : CoroutineDataBase
	{
		public Dictionary<UnitCtrl, CoroutineDataBase> CoroutineDataDictionary = new Dictionary<UnitCtrl, CoroutineDataBase>();

		private List<UnitCtrl> blackOutUnitList;

		private List<UnitCtrl> coroutineDataRemoveKeyList = new List<UnitCtrl>();

		private List<UnitCtrl> keyList { get; set; }

		[global::OpenPCR.Migration("deea459f-5ed3-43ff-b076-ccb779c98144")]
		public CoroutineData()
		{
		}

		[global::OpenPCR.Migration("4007086a-9da8-4f2e-bc7e-df9545a1e5c1")]
		public void Init(List<UnitCtrl> _blackOutUnitList)
		{
			blackOutUnitList = _blackOutUnitList;
		}

		[global::OpenPCR.Migration("90ae64fa-5f52-4e1c-abed-1e7c35285be7")]
		public override void Update()
		{
			if (blackOutUnitList.Count != 0)
			{
				for (int num = blackOutUnitList.Count - 1; num >= 0; num--)
				{
					CoroutineDataBase value = null;
					if (CoroutineDataDictionary.TryGetValue(blackOutUnitList[num], out value))
					{
						value.Update();
					}
					while (blackOutUnitList.Count <= num - 1)
					{
						num--;
					}
				}
				return;
			}
			base.Update();
			if (keyList == null)
			{
				keyList = new List<UnitCtrl>(CoroutineDataDictionary.Keys);
			}
			else
			{
				foreach (UnitCtrl key2 in CoroutineDataDictionary.Keys)
				{
					if (!keyList.Contains(key2))
					{
						keyList.Add(key2);
					}
				}
				for (int num2 = keyList.Count - 1; num2 >= 0; num2--)
				{
					UnitCtrl key = keyList[num2];
					if (!CoroutineDataDictionary.ContainsKey(key))
					{
						keyList.RemoveAt(num2);
					}
				}
			}
			for (int i = 0; i < keyList.Count; i++)
			{
				UnitCtrl unitCtrl = keyList[i];
				if (unitCtrl == null)
				{
					coroutineDataRemoveKeyList.Add(unitCtrl);
				}
				else
				{
					CoroutineDataDictionary[unitCtrl].Update();
				}
			}
			for (int num3 = coroutineDataRemoveKeyList.Count - 1; num3 >= 0; num3--)
			{
				CoroutineDataDictionary.Remove(coroutineDataRemoveKeyList[num3]);
				coroutineDataRemoveKeyList.RemoveAt(num3);
			}
		}
	}
}
