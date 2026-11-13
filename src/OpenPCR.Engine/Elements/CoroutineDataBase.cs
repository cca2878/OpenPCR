using System.Collections;
using System.Collections.Generic;

namespace Elements
{
	public class CoroutineDataBase
	{
		public List<IEnumerator> CoroutineList = new List<IEnumerator>();

		[global::OpenPCR.Migration("83562abb-fedd-433a-becb-909ebd169ee3")]
		public CoroutineDataBase()
		{
		}

		[global::OpenPCR.Migration("7af4fcff-e99b-4c24-9adb-a2053a325b09")]
		public void Destroy()
		{
			for (int num = CoroutineList.Count - 1; num > 0; num--)
			{
				CoroutineList[num] = null;
				CoroutineList.RemoveAt(num);
			}
			CoroutineList.Clear();
		}

		[global::OpenPCR.Migration("90c2d44f-a50e-43f1-bb1c-9adef288a321")]
		public virtual void Update()
		{
			if (CoroutineList.Count <= 0)
			{
				return;
			}
			for (int num = CoroutineList.Count - 1; num >= 0; num--)
			{
				if (!CoroutineList[num].MoveNext())
				{
					CoroutineList[num] = null;
					CoroutineList.RemoveAt(num);
				}
			}
		}
	}
}
