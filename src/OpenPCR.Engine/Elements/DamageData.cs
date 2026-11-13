using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;

namespace Elements
{
	[Serializable]
	public partial class DamageData
	{
		public global::System.Func<double, double> LimitDamageFunc { get; set; }


		public float CriticalRate { get; set; }

		public bool IgnoreDef { get; set; }

		public long LifeSteal { get; set; }

		public enum eDamageType
		{
			ATK = 1,
			MGC,
			NONE
		}

		public enum eDamageSoundType
		{
			HIT,
			SLIP
		}
		public long Damage { get; set; }
		public eDamageType DamageType { get; set; }
		public UnitCtrl Source { get; set; }
		public UnitCtrl SourceForDamageNumEffect { get; set; }
		public eActionType ActionType { get; set; }
		public BasePartsData Target { get; set; }

		public long DefPenetrate { get; set; }

		[global::OpenPCR.Migration("25593123-e1db-4cf6-b8eb-b9bf8c958337")]
		public DamageData()
		{
			DamegeNumScale = 1f;
			OverrideDamageLimit = -1L;
		}
		public eDamageSoundType DamageSoundType { get; set; }
		public long TotalDamageForLogBarrier { get; set; }
		public bool IsLogBarrierCritical { get; set; }
		public long LogBarrierExpectedDamage { get; set; }
		public long LogBarrierExpectedDamageForEnergyCalc { get; set; }
		public bool LogBarrierExpectedIsNoDamageByResist { get; set; }
		public UnitCtrl StrikeBackSource { get; set; }
		public float CriticalRateForLogBarrier { get; set; }
		public eDamageEffectType DamegeEffectType { get; set; }
		public bool TargetIncludeMyTeam { get; set; }
		public bool IsDivisionDamage { get; set; }
		public bool IsSlipDamage { get; set; }
		public double CriticalDamageRate { get; set; }
		public bool DamageNumMagic { get; set; }
		public float DamegeNumScale { get; set; }
		public Func<long, long> ExecAbsorber { get; set; }
		public bool IsAlwaysChargeEnegry { get; set; }
		public long OverrideDamageLimit { get; set; }
		public bool IgnoreStrikeBackAndAllUnitDamageCutTrigger { get; set; }
		public bool IgnoreLogBarrier { get; set; }
		public bool IgnoreTalentBonusForRatioSlip { get; set; }
	}
}
