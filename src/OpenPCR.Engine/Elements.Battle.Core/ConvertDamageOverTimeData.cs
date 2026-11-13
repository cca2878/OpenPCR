using System.Collections;
using System.Collections.Generic;
using global::OpenPCR.Foundation;

namespace Elements.Battle.Core
{
	public class ConvertDamageOverTimeData
	{
		public bool Enable { get; set; }
		public double Timer { get; set; }
		public UnitCtrl Target { get; set; }
		public UnitCtrl Source { get; set; }
		public double DamageCutValue { get; set; }
		public int NumberOfDamage { get; set; }
		public List<long> DamageList { get; set; } = new List<long>();
		public ConvertDamageOverTimeAction OwnerAction { get; set; }

		[global::OpenPCR.Migration("804acf3c-158c-427e-b5fd-6c2b382b2213")]
		public ConvertDamageOverTimeData()
		{
		}

		[global::OpenPCR.Migration("09f8b11c-4848-4974-80c1-a2f6daba98b5")]
		public void StartTimer()
		{
			global::Elements.UnitCtrl nativeValue1 = (this).Target;
			global::System.Collections.IEnumerator nativeResult1 = (this).updateTimer();
			if (global::System.Object.ReferenceEquals(nativeValue1, null))
				throw new global::System.NullReferenceException();
			(nativeValue1).AppendCoroutine((global::System.Collections.IEnumerator)(nativeResult1), unchecked((global::Elements.ePauseType)(1u)), null);
		}

		[global::OpenPCR.Migration("4b80d42e-7ebe-4ffc-9775-32bf06818414")]
		private IEnumerator updateTimer()
		{
			while (Enable)
			{
				if (Target.ComponentState.IdleOnly || Target.ComponentState.IsDead || !(Timer > 0.0))
				{
					Disable(false);
					yield break;
				}

				Timer -= (double)Target.DeltaTimeForPause;
				yield return null;
			}
		}

		[global::OpenPCR.Migration("60ed2afb-6284-4d39-b485-d02e54c50b5c")]
		public void Disable(bool _resetDamage)
		{
			Timer = 0.0;
			Enable = false;
			Target.ComponentAbnormal.OnChangeState.Call(eStateIconType.CONVERT_DAMAGE_OVER_TIME, false);
			if (_resetDamage)
				DamageList.Clear();
		}

		[global::OpenPCR.Migration("cd20f014-1b04-43ce-a6e4-31991fd68b40")]
		public void AddDamage(long _value)
		{
			if (_value == 0)
				return;
			int count = DamageList.Count;
			for (int i = 0; i < NumberOfDamage; i++)
			{
				if (i < DamageList.Count)
					DamageList[i] = unchecked(DamageList[i] + _value);
				else
					DamageList.Add(_value);
			}

			if (count == 0)
				Target.AppendCoroutine(updateDamage(), ePauseType.SYSTEM, null);
		}

		[global::OpenPCR.Migration("1d20ea55-ce6e-46dc-b92b-5c303d70eabf")]
		private IEnumerator updateDamage()
		{
			double timer = 0.0;
			float interval = Target.ComponentAbnormal.GetSlipDamageExtendValue();
			while (DamageList.Count > 0)
			{
				if (Target.ComponentState.IdleOnly || Target.ComponentState.IsDead)
					yield break;
				timer -= (double)Target.DeltaTimeForPause;
				if (BattleUtil.LessThanOrApproximately(timer, 0.0))
				{
					DamageData damage = new DamageData
					{
						Damage = DamageList[0],
						DamageType = DamageData.eDamageType.NONE,
						Source = Target,
						SourceForDamageNumEffect = Source,
						ActionType = eActionType.CONVERT_DAMAGE_OVER_TIME,
						Target = Target.ComponentMultiPartsBoss.GetFirstParts(false, 0f)
					};
					Target.ComponentDamageControl.SetDamage(damage, false, OwnerAction.ActionId, null, true, null, false, OwnerAction.OnDefeatEnemy, false, 1.0, 1.0, null, OwnerAction.EnergyChargeMultiple, OwnerAction.UsedChargeEnergyByReceiveDamage);
					DamageList.RemoveAt(0);
					timer = (double)interval;
				}

				yield return null;
			}
		}
	}
}
