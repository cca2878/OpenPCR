using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class ExtendSlipDamageData
	{
		private double timer;
		public bool Enable { get; set; }
		public float ExtendValue { get; set; } = 1f;
		public global::Elements.UnitCtrl Target { get; set; }
		public int EnvironmentId { get; set; } = -1;
		public double Duration { get; set; }

		[global::OpenPCR.Migration("5838a0bc-9b2c-4f0e-a11b-ed22b69e4953")]
		public ExtendSlipDamageData()
		{
		}

		[global::OpenPCR.Migration("62627c0d-9ecb-46b1-a83a-50485ccc2fc9")]
		public void Initialize()
		{
			global::Elements.UnitCtrl nativeValue1 = (this).Target;
			(this).Enable = true;
			global::System.Collections.IEnumerator nativeResult1 = (this).updateTimer();
			if (global::System.Object.ReferenceEquals(nativeValue1, null))
				throw new global::System.NullReferenceException();
			(nativeValue1).AppendCoroutine((global::System.Collections.IEnumerator)(nativeResult1), unchecked((global::Elements.ePauseType)(1u)), null);
		}

		[global::OpenPCR.Migration("15fdf266-f3f1-447f-bab8-144084981a14")]
		private IEnumerator updateTimer()
		{
			while (timer < Duration)
			{
				var target = Target;
				var state = target.ComponentState;
				if (state.IdleOnly || state.IsDead || !Enable)
					yield break;
				timer += (double)target.DeltaTimeForPause;
				yield return null;
			}
		}
	}
}
