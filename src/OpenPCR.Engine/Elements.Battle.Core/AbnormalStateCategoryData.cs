using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AbnormalStateCategoryData
	{
		public bool DisableBySwitch { get; set; }
		public global::Elements.Battle.Core.eAbnormalState SwitchingAbnormalState { get; set; }
		public double SubValue { get; set; }
		public double EnergyReduceRate { get; set; }
		public bool IsEnergyReduceMode { get; set; }
		public bool IsDamageRelease { get; set; }
		public bool IsReleasedByDamage { get; set; }
		public global::Elements.Battle.Core.eActionType ActionType { get; set; }
		public long AbsorberValue { get; set; }
		public global::Elements.Skill Skill { get; set; }
		public global::Elements.UnitCtrl Source { get; set; }
		public int ActionDetail1 { get; set; }
		public int ActionDetail2 { get; set; }
		public bool IsEnvironmentAbnormalState { get; set; }
		public global::System.Collections.IEnumerator UpdateCoroutine { get; set; }

		public eAbnormalState CurrentAbnormalState;
		public bool enable;
		public double MainValue;
		public float Time;
		public float Duration;
		public List<AbnormalStateEffectGameObject> Effects = new List<AbnormalStateEffectGameObject>();
		public int ActionId;
		public double EnergyChargeMultiple = 1f;
		public bool ShowsIcon = true;
		public int SourceFieldIndex { get; set; } = -1;

		[global::OpenPCR.Migration("4b6c3201-82ce-4f5b-bfe0-426d1aadbdbc")]
		public AbnormalStateCategoryData()
		{
		}

		[global::OpenPCR.Migration("bb4c512f-08f1-44a0-9053-2ff11fa06d9d")]
		public AbnormalStateCategoryData Clone()
		{
			return (AbnormalStateCategoryData)MemberwiseClone();
		}
	}
}
