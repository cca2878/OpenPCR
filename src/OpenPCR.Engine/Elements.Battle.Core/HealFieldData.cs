using global::Elements;
using global::OpenPCR.Runtime.Api;
using Elements.Battle;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public partial class HealFieldData : FieldDataBase
	{
		public enum eEffectType
		{
			NONE = 0,
			HEAL = 1
		}

		public enum eValueType
		{
			FIXED = 1,
			PERCENTAGE = 2
		}

		public global::Elements.Battle.Core.HealFieldData.eValueType ValueType { get; set; }
		public global::Elements.Battle.Core.HealFieldData.eEffectType EffectType { get; set; }
		public double Value { get; set; }
		public bool IsMagic { get; set; }

		[global::OpenPCR.Migration("19899195-7828-47bd-b885-804a79cfe8e1")]
		public HealFieldData() : base()
		{
		}

		[global::OpenPCR.Migration("b8404500-61a6-4f9c-b669-6b61e69adf02")]
		public override void StartField()
		{
			if (base.UniqueEffectPrefab != null)
			{
				skillEffect = base.battleEffectPool.GetEffect(base.Source.IsLeftDir ? base.UniqueEffectPrefabLeft : base.UniqueEffectPrefab);
				initializeSkillEffect();
			}
			else
			{
				eEffectType effectType = EffectType;
				if (effectType == eEffectType.HEAL)
				{
					skillEffect = base.battleEffectPool.GetEffect(Singleton<BattleResourceManager>.Instance.HealFieldEffectPrefab);
					skillEffect.transform.parent = ExceptUIRoot.Transform;
					skillEffect.transform.localPosition = new Vector3(base.CenterX / 540f, (5000f + base.battleManager.GetRespawnPos(eUnitRespawnPos.MAIN_POS_3)) / 540f, 0f);
					skillEffect.transform.localScale = Vector3.one * base.Size / 350f;
					skillEffect.InitializeSort();
					if (base.battleManager.GetBlackOutUnitLength() > 0)
					{
						skillEffect.SetSortOrderFront();
					}
					else
					{
						skillEffect.SetSortOrderBack();
					}
				}
			}

			base.StartField();
		}

		[global::OpenPCR.Migration("b33beaed-37ca-4b6d-95d7-c09b061bcf0a")]
		public override void OnRepeat()
		{
			int count = TargetList.Count;
			for (int i = 0; i < count; i++)
			{
				BasePartsData parts = TargetList[i];
				UnitCtrl owner = parts.Owner;
				owner.ComponentDamageControl.SetRecovery(BattleUtil.DoubleToLong(Value), 0, IsMagic ? eInhibitHealType.MAGIC : eInhibitHealType.PHYSICS, owner, false, null, Source.ComponentAbnormal.GetHealDownValue(), true, false, false, false, true, parts, true);
			}
		}
	}
}
