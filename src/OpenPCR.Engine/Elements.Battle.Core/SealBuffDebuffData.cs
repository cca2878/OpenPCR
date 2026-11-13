using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class SealBuffDebuffData
	{
		public global::Elements.UnitCtrl Source { get; set; }
		public global::Elements.Battle.Core.SealData ParentSealData { get; set; }
		public global::Elements.Battle.Core.BuffParamKind TargetParameter { get; set; } = (BuffParamKind)201;
		public global::System.Collections.Generic.Dictionary<Elements.BasePartsData, System.Int64> ValuePerSealDictionary { get; set; } = new Dictionary<BasePartsData, long>();

		private bool isEnable = true;
		public global::Elements.Battle.Core.UnitComponentAbnormal TargetComponent { get; set; }
		private double timer { get; set; }
		public double Duration { get; set; }
		private int currentSealCount { get; set; }

		[global::OpenPCR.Migration("7742fd79-c937-4073-a2ef-7db8752d1cb0")]
		public SealBuffDebuffData()
		{
		}

		[global::OpenPCR.Migration("b6ecca10-4400-424b-8b5c-907aa9a94595")]
		public void UpdateValue(int _count)
		{
			if (currentSealCount == _count)
				return;
			bool enable = currentSealCount < _count;
			int difference = enable ? _count - currentSealCount : currentSealCount - _count;
			TargetComponent.SetBuffDebuffWithMultiplyValue((BuffParamKind)TargetParameter, ValuePerSealDictionary, enable, Source, difference);
			currentSealCount = _count;
		}

		[global::OpenPCR.Migration("029612d8-c285-4cf7-a9c6-1a95f47b622f")]
		public void StartTimer()
		{
			global::Elements.UnitCtrl nativeValue1 = (this).Target;
			global::System.Collections.IEnumerator nativeResult1 = (this).updateTimer();
			if (global::System.Object.ReferenceEquals(nativeValue1, null))
				throw new global::System.NullReferenceException();
			(nativeValue1).AppendCoroutine((global::System.Collections.IEnumerator)(nativeResult1), unchecked((global::Elements.ePauseType)(1u)), null);
		}

		[global::OpenPCR.Migration("56314fc7-fceb-470e-9918-cf7cdaa24950")]
		private IEnumerator updateTimer()
		{
			while (timer < Duration && !Target.ComponentState.IdleOnly && !Target.ComponentState.IsDead)
			{
				if (!isEnable)
					yield break;
				timer += Target.DeltaTimeForPause;
				yield return null;
			}

			if (isEnable)
			{
				DeleteValue();
				ParentSealData.RemoveSealBuffDebuff(this);
			}
		}

		[global::OpenPCR.Migration("40ab0fb0-0f4e-4f88-88fb-b19ebd264fcc")]
		public void DeleteValue()
		{
			isEnable = false;
			TargetComponent.SetBuffDebuffWithMultiplyValue(TargetParameter, ValuePerSealDictionary, false, Source, currentSealCount);
		}

		public global::Elements.UnitCtrl Target { get; set; }
	}
}
