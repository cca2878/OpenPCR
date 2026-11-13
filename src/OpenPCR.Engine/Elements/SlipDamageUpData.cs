using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;
using global::OpenPCR.Foundation;

namespace Elements
{
	public class SlipDamageUpData
	{
		public float Timer { get; set; }
		public bool DisplayIcon { get; set; }

		[global::OpenPCR.Migration("35e45449-9b97-4619-8975-6bd39e49dc73")]
		public void StartTimer(UnitCtrl _owner)
		{
			if (DisplayIcon)
			{
				UnitComponentAbnormal abnormal = _owner.ComponentAbnormal;
				if (abnormal.SlipDamageUpIconCounter == 0)
					abnormal.OnChangeState.Call(eStateIconType.SLIP_DAMAGE_UP, true);
				abnormal.SlipDamageUpIconCounter = unchecked(abnormal.SlipDamageUpIconCounter + 1);
			}
			_owner.AppendCoroutine(updateTimer(_owner), ePauseType.SYSTEM, null);
		}

		[global::OpenPCR.Migration("a5704977-a270-4ef5-9ede-0bc8a81546e6")]
		private IEnumerator updateTimer(UnitCtrl _owner)
		{
			UnitComponentAbnormal abnormal = _owner.ComponentAbnormal;
			UnitComponentState state = _owner.ComponentState;
			while (Enable && !state.IdleOnly && !state.IsDead && !(Timer <= 0f))
			{
				Timer -= _owner.DeltaTimeForPause;
				yield return null;
			}
			if (Enable)
			{
				Timer = 0f;
				Enable = false;
				if (DisplayIcon)
				{
					abnormal.SlipDamageUpIconCounter = unchecked(abnormal.SlipDamageUpIconCounter - 1);
					if (abnormal.SlipDamageUpIconCounter == 0)
						abnormal.OnChangeState.Call(eStateIconType.SLIP_DAMAGE_UP, false);
				}
			}
		}

		public double Value { get; set; }
		public long LimitDamage { get; set; }
		public bool Enable { get; set; }

		[global::OpenPCR.Migration("14e00d1d-093e-4b53-aef7-1f355f5468c7")]
		public SlipDamageUpData()
		{
		}
	}
}
