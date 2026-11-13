using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class PassiveSealData
	{
		public float LifeTime { get; set; }
		public UnitCtrl Target { get; set; }

		public UnitCtrl Source { get; set; }
		public float SealDuration { get; set; }
		public eStateIconType TargetStateIcon { get; set; }
		public bool DisplayCount { get; set; } = true;
		public PassiveSealAction.eSealTarget SealTarget { get; set; }
		public int SealNumLimit { get; set; }
		public int SealNum { get; set; }
		[global::OpenPCR.Migration("496c4fe3-53e1-4fe6-af89-82a539153d87")]
		public PassiveSealData()
		{

		}

		[global::OpenPCR.Migration("ab77742e-ca73-4fc9-b163-20f97187f2e0")]
		public IEnumerator Update()
		{
			while (true)
			{
				LifeTime -= Target.DeltaTimeForPause;
				if (LifeTime < 0f)
				{
					break;
				}

				yield return null;
			}
		}
	}
}
