using global::OpenPCR.Runtime.Api;
using System.Collections;
using System.Collections.Generic;

namespace Elements
{
	public class CoroutineManager : MonoBehaviour
	{
		private Dictionary<ePauseType, CoroutineData> coroutineDataDictionary = new Dictionary<ePauseType, CoroutineData>(new ePauseType_DictComparer())
		{
			{
				ePauseType.VISUAL,
				new CoroutineData()
			},
			{
				ePauseType.SYSTEM,
				new CoroutineData()
			},
			{
				ePauseType.IGNORE_BLACK_OUT,
				new CoroutineData()
			},
			{
				ePauseType.NO_DIALOG,
				new CoroutineData()
			}
		};

		public bool SystemPause { get; set; }

		public bool VisualPause { get; set; }

		[global::OpenPCR.Migration("873e4382-5e5b-4987-9929-dbca81ec9ca3")]
		public CoroutineManager()
		{
		}

		[global::OpenPCR.Migration("b6e35419-26e9-40bd-8dae-dead4d58a7e3")]
		public void Init(List<UnitCtrl> _blackOutUnitList)
		{
			Dictionary<ePauseType, CoroutineData>.Enumerator enumerator = coroutineDataDictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.Value.Init(_blackOutUnitList);
			}
		}

		[global::OpenPCR.Migration("9cb22601-7aa0-41bb-89f9-3efe6c624e48")]
		public void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit = null)
		{
			if (unit == null)
			{
				coroutineDataDictionary[pauseType].CoroutineList.Add(cr);
				return;
			}
			if (!coroutineDataDictionary[pauseType].CoroutineDataDictionary.ContainsKey(unit))
			{
				coroutineDataDictionary[pauseType].CoroutineDataDictionary.Add(unit, new CoroutineDataBase());
			}
			coroutineDataDictionary[pauseType].CoroutineDataDictionary[unit].CoroutineList.Add(cr);
		}

		[global::OpenPCR.Migration("23da211d-6278-4edc-b4de-00350ad3a4b3")]
		public void RemoveAll()
		{
			foreach (CoroutineData value in coroutineDataDictionary.Values)
			{
				value.Destroy();
			}
		}

		[global::OpenPCR.Migration("662cd2a2-9359-482f-8572-1fd80ab63c2a")]
		private void OnDestroy()
		{
			foreach (KeyValuePair<ePauseType, CoroutineData> item in coroutineDataDictionary)
			{
				item.Value.Destroy();
				item.Value.CoroutineDataDictionary.Clear();
			}
		}

		[global::OpenPCR.Migration("fac9d8cc-1f48-4a90-a4c3-1e299ecc92ca")]
		public void _Update()
		{
			if (!SystemPause)
			{
				coroutineDataDictionary[ePauseType.SYSTEM].Update();
				List<IEnumerator> coroutineList = coroutineDataDictionary[ePauseType.IGNORE_BLACK_OUT].CoroutineList;
				for (int num = coroutineList.Count - 1; num >= 0; num--)
				{
					if (!coroutineList[num].MoveNext())
					{
						coroutineList.RemoveAt(num);
					}
				}
			}
			if (!VisualPause)
			{
				coroutineDataDictionary[ePauseType.VISUAL].Update();
			}
		}

		[global::OpenPCR.Migration("119f831f-caf8-4a6f-b4e8-aab5c9b05c78")]
		public void NoDialogUpdate()
		{
			List<IEnumerator> coroutineList = coroutineDataDictionary[ePauseType.NO_DIALOG].CoroutineList;
			for (int num = coroutineList.Count - 1; num >= 0; num--)
			{
				if (!coroutineList[num].MoveNext())
				{
					coroutineList.RemoveAt(num);
				}
			}
		}
		[global::OpenPCR.Migration("fcabe6c6-f584-467f-b363-f9f5693932e5")]
		public void ReorderByEnemyList(ePauseType _pauseType, List<UnitCtrl> _enemyList)
		{
			Dictionary<UnitCtrl, CoroutineDataBase> old = coroutineDataDictionary[_pauseType].CoroutineDataDictionary;
			Dictionary<UnitCtrl, CoroutineDataBase> reordered = new Dictionary<UnitCtrl, CoroutineDataBase>();
			foreach (KeyValuePair<UnitCtrl, CoroutineDataBase> item in old)
			{
				if (item.Key.IsOther)
				{
					if (!_enemyList.Contains(item.Key))
					{
						reordered.Add(item.Key, item.Value);
					}
				}
				else
				{
					reordered.Add(item.Key, item.Value);
				}
			}
			foreach (UnitCtrl enemy in _enemyList)
			{
				if (old.TryGetValue(enemy, out CoroutineDataBase data))
				{
					reordered.Add(enemy, data);
				}
			}
			coroutineDataDictionary[_pauseType].CoroutineDataDictionary = reordered;
		}

	}
}
