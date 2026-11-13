using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Data;

namespace Elements.Battle.Core
{
	public class UnitComponentPassiveSkill : UnitComponentBase
	{
		public List<ExSkillData> StartExSkillList { get; private set; } = new List<ExSkillData>();
		public Dictionary<eExSkillCondition, List<ExSkillData>> ConditionExSkillDictionary { get; private set; } = new Dictionary<eExSkillCondition, List<ExSkillData>>();

		private UnitComponentParameter componentParameter;
		private Dictionary<eParamType, long> knightEnhanceBuffParameter = new Dictionary<eParamType, long>();
		private Dictionary<eParamType, long> roleBuffParameter = new Dictionary<eParamType, long>();
		private Dictionary<eParamType, long> labyrinthBuffParameter = new Dictionary<eParamType, long>();
		public Dictionary<eParamType, PassiveActionValue> OwnerPassiveAction { get; private set; }

		[global::OpenPCR.Migration("64d7ac78-1b93-404a-8603-df9799826874")]
		public UnitComponentPassiveSkill(UnitCtrl _unitController) : base(_unitController)
		{
			OwnerPassiveAction = new Dictionary<eParamType, PassiveActionValue>(new eParamType_DictComparer());
		}

		[global::OpenPCR.Migration("3ae5ad6a-f8d8-4fae-b6f8-4845ece64bc7")]
		private Dictionary<eParamType, PassiveActionValue> addPassiveValue(Dictionary<eParamType, PassiveActionValue> _dic1, Dictionary<eParamType, PassiveActionValue> _dic2)
		{
			var result = new Dictionary<eParamType, PassiveActionValue>(_dic1, new eParamType_DictComparer());
			foreach (var item in _dic2)
			{
				bool exists = result.TryGetValue(item.Key, out var value);
				if (!exists) value = default(PassiveActionValue);
				value.Value = item.Value.Value + value.Value;
				value.Percentage = item.Value.Percentage + value.Percentage;
				if (exists) result[item.Key] = value;
				else result.Add(item.Key, value);
			}
			return result;
		}

		[global::OpenPCR.Migration("cf864f59-56f3-4af2-bc46-605c81ec18e7")]
		private long calculatePassiveSkillValue(double _value, KeyValuePair<eParamType, PassiveActionValue> _passiveActionKV)
		{
			return BattleUtil.DoubleToLong(System.Math.Ceiling(_passiveActionKV.Value.Value + (_passiveActionKV.Value.Percentage + 100.0) * _value / 100.0));
		}

		[global::OpenPCR.Migration("53dcebfa-ae7a-4d28-b318-5d86bf10063f")]
		public void ApplyPassiveSkill(bool _isFirst, bool _considerEquipmentAndBonus)
		{
			if (_considerEquipmentAndBonus) applyPassiveSkillValue(_isFirst);
			applyTalentLevel();
			applyKnightEnhance(_isFirst);
			applyRoleEnhance(_isFirst);
			applyLabyrinthEnchnce(_isFirst);
			unitController.ComponentRelicPassiveSkill.ApplyRelicPassiveSkill(_isFirst, _considerEquipmentAndBonus);
			componentParameter.MaxHpAfterPassive = componentParameter.MaxHp;
			unitController.UnitActionController.SetAttackValue(unitController.ComponentParameterClamped.AdditionalAttackEnhanceZero);
		}


		[global::OpenPCR.Migration("aad949a9-e5ae-4e5d-94bf-3f9e5b0ac24e")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentParameter = _container.ComponentParameter;
		}
		private long additiveAtkForTalentLevel;

		[global::OpenPCR.Migration("e853f036-6210-4c8a-bcb7-2f9c8642b07b")]
		private void applyPassiveSkillValue(bool _first)
		{
			var byEnemy = unitController.GetPassiveDictionaryByEnemy();
			var byUnit = unitController.GetPassiveDictionaryByUnit();
			foreach (var item in addPassiveValue(addPassiveValue(byEnemy, byUnit), OwnerPassiveAction))
			{
				switch ((int)item.Key)
				{
					case 1:
						if (!_first || unitController.IsClanBattleOrSekaiEnemy) continue;
						componentParameter.MaxHp = calculatePassiveSkillValue((double)(long)componentParameter.StartMaxHP, item);
						componentParameter.SetCurrentHp(BattleUtil.DoubleToLong((double)(long)componentParameter.StartHpWithoutPassive / (double)(long)componentParameter.StartMaxHP * (double)(long)componentParameter.MaxHp));
						break;
					case 2: componentParameter.Atk = calculatePassiveSkillValue((double)(long)componentParameter.StartAtk, item); break;
					case 3: componentParameter.Def = calculatePassiveSkillValue((double)(long)componentParameter.StartDef, item); break;
					case 4: componentParameter.MagicStr = calculatePassiveSkillValue((double)(long)componentParameter.StartMagicStr, item); break;
					case 5: componentParameter.MagicDef = calculatePassiveSkillValue((double)(long)componentParameter.StartMagicDef, item); break;
					case 6: componentParameter.PhysicalCritical = calculatePassiveSkillValue((double)(long)componentParameter.StartPhysicalCritical, item); break;
					case 7: componentParameter.MagicCritical = calculatePassiveSkillValue((double)(long)componentParameter.StartMagicCritical, item); break;
					case 8: componentParameter.Dodge = calculatePassiveSkillValue((double)(long)componentParameter.StartDodge, item); break;
					case 9: componentParameter.LifeSteal = calculatePassiveSkillValue((double)(long)componentParameter.StartLifeSteal, item); break;
					case 10: componentParameter.WaveHpRecovery = calculatePassiveSkillValue((double)(long)componentParameter.StartWaveHpRecovery, item); break;
					case 11: componentParameter.WaveEnergyRecovery = calculatePassiveSkillValue((double)(long)componentParameter.StartWaveEnergyRecovery, item); break;
					case 12: componentParameter.PhysicalPenetrate = calculatePassiveSkillValue((double)(long)componentParameter.StartPhysicalPenetrate, item); break;
					case 13: componentParameter.MagicPenetrate = calculatePassiveSkillValue((double)(long)componentParameter.StartMagicPenetrate, item); break;
					case 14: componentParameter.EnergyRecoveryRate = calculatePassiveSkillValue((double)(long)componentParameter.StartEnergyRecoveryRate, item); break;
					case 15: componentParameter.HpRecoveryRate = calculatePassiveSkillValue((double)(long)componentParameter.StartHpRecoveryRate, item); break;
					case 16: componentParameter.EnergyReduceRate = calculatePassiveSkillValue((double)(long)componentParameter.StartEnergyReduceRate, item); break;
					case 17: componentParameter.Accuracy = calculatePassiveSkillValue((double)(long)componentParameter.StartAccuracy, item); break;
				}
			}
		}

		[global::OpenPCR.Migration("906b1175-0ed4-43cf-a9f9-8b09ff4cd605")]
		private void applyTalentLevel()
		{
			if ((int)unitController.AtkType == 1) componentParameter.Atk = unchecked((long)componentParameter.Atk + additiveAtkForTalentLevel);
			else componentParameter.MagicStr = unchecked((long)componentParameter.MagicStr + additiveAtkForTalentLevel);
		}

		[global::OpenPCR.Migration("b0cf5884-984f-4f6f-bac0-1fa492cd4d92")]
		private void applyKnightEnhance(bool _first)
		{
			bool immutableMaxHp = unitController.IsClanBattleOrSekaiEnemy;
			foreach (var item in knightEnhanceBuffParameter)
				componentParameter.ApplyEnhanceValue(item.Key, item.Value, _first, immutableMaxHp, UnitDefine.eParameterEnhanceType.KNIGHT_ENHANCE);
		}

		[global::OpenPCR.Migration("a08e959f-6d69-41bf-8939-508bf742a3e0")]
		private void applyRoleEnhance(bool _first)
		{
			bool immutableMaxHp = unitController.IsClanBattleOrSekaiEnemy;
			foreach (var item in roleBuffParameter)
				componentParameter.ApplyEnhanceValue(item.Key, item.Value, _first, immutableMaxHp, UnitDefine.eParameterEnhanceType.ROLE);
		}

		[global::OpenPCR.Migration("b6588bc9-8962-4f08-9fb8-bcd2840aecea")]
		private void applyLabyrinthEnchnce(bool _first)
		{
			bool immutableMaxHp = unitController.IsClanBattleOrSekaiEnemy;
			foreach (var item in labyrinthBuffParameter)
				componentParameter.ApplyEnhanceValue(item.Key, item.Value, _first, immutableMaxHp, UnitDefine.eParameterEnhanceType.LABYRINTH);
		}

		private long talentFormationBonusPermyriad;
		[global::OpenPCR.ImplicitMigration("f203365a-257e-4682-8984-9e6941430c90")][global::OpenPCR.Migration("9e1f6416-ca01-4a5e-8ec5-52bbdf89198f")] public long TalentFormationBonusPermyriad => talentFormationBonusPermyriad;

		[global::OpenPCR.Migration("8fe001a7-26b2-4193-be3e-79ec16b815a3")]
		public void SetTalentFormationBonus(Dictionary<int, long> _talentFormationBonusDictionary)
		{
			long bonus = 0L;
			if (_talentFormationBonusDictionary != null)
			{
				for (int i = 0; i < unitController.TalentIds.Count; i++)
				{
					_talentFormationBonusDictionary.TryGetValue(unitController.TalentIds[i], out long value);
					bonus = unchecked(bonus + value);
				}
			}
			talentFormationBonusPermyriad = bonus;
		}

		[global::OpenPCR.Migration("022e61eb-f7f1-459b-acdb-d50e903c034c")]
		public void RegisterExSkill(ExSkillData _exSkillData)
		{
			if (_exSkillData.ExSkillConditionData.ContainsKey((eExSkillCondition)0)) StartExSkillList.Add(_exSkillData);
			else
			{
				foreach (var item in _exSkillData.ExSkillConditionData)
				{
					if (ConditionExSkillDictionary.TryGetValue(item.Key, out var list)) list.Add(_exSkillData);
					else ConditionExSkillDictionary.Add(item.Key, new List<ExSkillData> { _exSkillData });
				}
			}
		}

		[global::OpenPCR.Migration("9c9fe28b-1495-46e7-8b75-7f6c815f4b45")]
		public void SetEnhanceData(TeamEnhanceParameter _enhanceParameter)
		{
			setAdditiveAtkForTalentLevel(_enhanceParameter);
			setKnightEnhance(_enhanceParameter);
			setRoleEnhance(_enhanceParameter);
		}

		[global::OpenPCR.Migration("324cdd9e-f241-44f2-81e5-2fa3045bc2b9")]
		public void SetContentsEnhanceData(LabyrinthEnhanceParameter _enhanceParameter)
		{
			setLabyrinthEnhance(_enhanceParameter);
		}

		[global::OpenPCR.Migration("2c7fa943-5c5d-4667-a0a9-427d364b2d5a")]
		private void setAdditiveAtkForTalentLevel(TeamEnhanceParameter _teamEnhanceParameter)
		{
			if (_teamEnhanceParameter == null) return;
			List<int> talents = unitController.TalentIds;
			for (int i = 0; i < talents.Count; i++) additiveAtkForTalentLevel = unchecked(additiveAtkForTalentLevel + _teamEnhanceParameter.GetAdditiveAtkForTalentLevel(talents[i]));
		}

		[global::OpenPCR.Migration("e1161638-d4da-43fc-ad33-f54b3bf953b7")]
		private void setKnightEnhance(TeamEnhanceParameter _teamEnhanceParameter)
		{
			if (_teamEnhanceParameter == null) return;
			List<int> talents = unitController.TalentIds;
			for (int i = 0; i < talents.Count; i++)
			{
				foreach (var item in _teamEnhanceParameter.GetKnightEnhance(talents[i]))
				{ knightEnhanceBuffParameter.TryGetValue(item.Key, out long prior); knightEnhanceBuffParameter[item.Key] = unchecked(prior + (long)item.Value); }
			}
		}

		[global::OpenPCR.Migration("bc592adb-c8ab-4d2d-bfd3-ada05de4d4d9")]
		private void setRoleEnhance(TeamEnhanceParameter _teamEnhanceParameter)
		{
			if (_teamEnhanceParameter == null) return;
			foreach (var item in _teamEnhanceParameter.GetRoleEnhance(unitController.RoleId, unitController))
			{ roleBuffParameter.TryGetValue(item.Key, out long prior); roleBuffParameter[item.Key] = unchecked(prior + (long)item.Value); }
		}

		[global::OpenPCR.Migration("bd57530e-72b1-46b9-908f-436883ca98a9")]
		private void setLabyrinthEnhance(LabyrinthEnhanceParameter _enhanceParameter)
		{
			if (_enhanceParameter == null) return;
			foreach (var item in _enhanceParameter.LabyrinthBuffParameterDictionary)
			{ labyrinthBuffParameter.TryGetValue(item.Key, out long prior); labyrinthBuffParameter[item.Key] = unchecked(prior + (long)item.Value); }
		}

	}
}
