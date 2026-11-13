using System.Collections.Generic;
using Elements.Battle;

namespace Elements.Battle.Core
{
	public partial class ChargeEnergyFieldData : FieldDataBase
	{
		public enum eFieldEffectType
		{
			NONE = 0,
			HEAL = 1,
			DAMAGE = 2
		}

		public enum eChargeType
		{
			INCREASE = 1,
			DECREASE = 2
		}

		public global::Elements.Battle.Core.ChargeEnergyFieldData.eChargeType ChargeType = (eChargeType)1;
		public global::Elements.Battle.Core.ChargeEnergyFieldData.eFieldEffectType FieldEffectType;
		public double Value;
		[global::OpenPCR.Migration("ae919696-3cf2-4a19-af7f-2cd51306be0e")]
		public ChargeEnergyFieldData()
		{
		}

		[global::OpenPCR.Migration("fa29b4ff-60ae-4b62-b5ce-bff6b8cbb421")]
		public override void StartField()
		{
			if (base.UniqueEffectPrefab != null)
			{
				skillEffect = base.battleEffectPool.GetEffect(base.Source.IsLeftDir ? base.UniqueEffectPrefabLeft : base.UniqueEffectPrefab);
				initializeSkillEffect();
			}
			else
			{
				switch (FieldEffectType)
				{
					case eFieldEffectType.HEAL:
						skillEffect = base.battleEffectPool.GetEffect(Singleton<BattleResourceManager>.Instance.ChargeEnergyFieldHealEffectPrefab);
						initializeSkillEffect();
						break;
					case eFieldEffectType.DAMAGE:
						skillEffect = base.battleEffectPool.GetEffect(Singleton<BattleResourceManager>.Instance.ChargeEnergyFieldDamageEffectPrefab);
						initializeSkillEffect();
						break;
				}
			}

			base.StartField();
		}

		[global::OpenPCR.Migration("02d63784-ed8c-4019-8da6-2d421b4f8b53")]
		public override void OnRepeat()
		{
			var consumed = new List<UnitCtrl>();
			int count = TargetList.Count;
			for (int i = 0; i < count; i++)
			{
				UnitCtrl owner = TargetList[i].Owner;
				if (owner.ComponentState.IsDead || (long)owner.ComponentParameter.Hp < 1 || consumed.Contains(owner))
					continue;
				double energy = Value;
				if (owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.ENERGY_DAMAGE_REDUCE) && energy < 0.0)
					energy *= owner.ComponentAbnormal.GetAbnormalStateMainValue(eAbnormalStateCategory.ENERGY_DAMAGE_REDUCE);
				owner.ComponentDamageControl.ChargeEnergy(eSetEnergyType.BY_CHANGE_ENERGY, energy, false, null, ChargeType == eChargeType.INCREASE, Source);
				consumed.Add(owner);
			}
		}
	}
}
