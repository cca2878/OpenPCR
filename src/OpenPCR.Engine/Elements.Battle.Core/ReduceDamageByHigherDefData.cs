using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class ReduceDamageByHigherDefData
	{
		public bool Enable { get; set; }
		public double Timer { get; set; }

		[global::OpenPCR.Migration("7bfd3804-249f-49e3-b41b-d592d92d8853")]
		public ReduceDamageByHigherDefData()
		{
		}

		[global::OpenPCR.Migration("32de84df-182a-4ba4-8d5a-bcc9f2099752")]
		public void StartTimer()
		{
			global::Elements.UnitCtrl nativeValue1 = (this).Target;
			global::System.Collections.IEnumerator nativeResult1 = (this).updateTimer();
			if (global::System.Object.ReferenceEquals(nativeValue1, null))
				throw new global::System.NullReferenceException();
			(nativeValue1).AppendCoroutine((global::System.Collections.IEnumerator)(nativeResult1), unchecked((global::Elements.ePauseType)(1u)), null);
		}

		[global::OpenPCR.Migration("2418a37e-2430-44c8-9369-14b25599c797")]
		private IEnumerator updateTimer()
		{
			while (Enable)
			{
				var target = Target;
				var state = target.ComponentState;
				if (state.IdleOnly || state.IsDead || Timer <= 0.0)
				{
					Disable();
					yield break;
				}

				Timer -= (double)target.DeltaTimeForPause;
				yield return null;
			}
		}

		[global::OpenPCR.Migration("50fa6908-b831-49b4-a15e-21205b8407b1")]
		public void Disable()
		{
			var target = Target;
			Timer = 0.0;
			Enable = false;
			target.ComponentAbnormal.OnChangeState.Call((eStateIconType)203, false);
		}

		public global::Elements.UnitCtrl Target { get; set; }
	}
}
