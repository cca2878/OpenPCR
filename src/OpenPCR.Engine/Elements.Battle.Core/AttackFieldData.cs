using Elements.Battle;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AttackFieldData : FieldDataBase
	{
		public global::Elements.DamageData.eDamageType DamageType { get; set; }
		public bool IsWeakTypeDamage { get; set; }
		public int EffectId { get; set; }
		public double Value { get; set; }
		public int ActionId { get; set; }
		public double EnergyChargeMultiple { get; set; } = 1.0;
		public long AbsorberValue { get; set; }
		public bool IsEnvironmentAbnormalState { get; set; }

		[global::OpenPCR.Migration("871930f2-0bfa-4cb1-ab57-809f0b85eb9a")]
		public AttackFieldData()
		{
		}

		[global::OpenPCR.Migration("f9edbfa6-24b7-4edf-9ac1-1f4984d08e94")]
		public override void StartField()
		{
			if (base.UniqueEffectPrefab != null)
			{
				skillEffect = base.battleEffectPool.GetEffect(base.Source.IsLeftDir ? base.UniqueEffectPrefabLeft : base.UniqueEffectPrefab);
				initializeSkillEffect();
			}
			else if (EffectId > 0)
			{
				skillEffect = base.battleEffectPool.GetEffect(Singleton<BattleResourceManager>.Instance.GetAttaclFieldEffect(EffectId));
				initializeSkillEffect();
			}

			base.StartField();
		}

		[global::OpenPCR.Migration("87fe72f0-3879-484e-b283-e9d3dc6ee8cb")]
		public override void OnRepeat()
		{
			int count = TargetList.Count;
			for (int i = 0; i < count; i++)
			{
				UnitComponentParameterClamped parameter = TargetList[i].Owner.ComponentParameterClamped;
				if (TargetList[i].Owner.ComponentState.CurrentState == ActionState.DIE)
					continue;
				DamageData.eDamageType damageType = DamageType;
				if (IsWeakTypeDamage)
				{
					long defense = TargetList[i].GetDefZero();
					long magicDefense = TargetList[i].GetMagicDefZero();
					if (defense != magicDefense)
						damageType = defense < magicDefense ? DamageData.eDamageType.ATK : DamageData.eDamageType.MGC;
				}

				double receive = 1.0;
				double damageUp = 1.0;
				if (damageType == DamageData.eDamageType.ATK)
				{
					receive = (double)parameter.PhysicalReceiveDamagePercentOrMin / 100.0;
					damageUp = (double)Source.ComponentParameterClamped.PhysicalDamageUpPercentOrMin / 100.0;
				}
				else if (damageType == DamageData.eDamageType.MGC)
				{
					receive = (double)parameter.MagicReceiveDamagePercentOrMin / 100.0;
					damageUp = (double)Source.ComponentParameterClamped.MagicDamageUpPercentOrMin / 100.0;
				}

				BasePartsData target = TargetList[i];
				DamageData data = new DamageData
				{
					Target = target,
					Damage = BattleUtil.DoubleToLong(damageUp * (receive * Value)),
					Source = Source,
					DamageType = damageType,
					ActionType = eActionType.ATTACK_FIELD,
					IsSlipDamage = target.Owner.IsOther == Source.IsOther || IsEnvironmentAbnormalState,
					ExecAbsorber = damage =>
					{
						if (damage > AbsorberValue)
						{
							long result = damage - AbsorberValue;
							battleManager.SubstructEnemyPoint(unchecked((int)AbsorberValue));
							AbsorberValue = 0;
							return result;
						}

						AbsorberValue -= damage;
						battleManager.SubstructEnemyPoint(unchecked((int)damage));
						return 0;
					}
				};
				TargetList[i].Owner.ComponentDamageControl.SetDamage(data, false, ActionId, null, true, Skill, true, null, true, 1.0, 1.0, null, EnergyChargeMultiple);
			}
		}
	}
}
