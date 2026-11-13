using global::Elements;
using System.Linq;
using System.Text;
using global::OpenPCR.Foundation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using global::OpenPCR.Foundation.Audio;
using Elements.Data;
using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public static class UnitUtility
	{
		[global::OpenPCR.Migration("84460e45-7b42-4713-8429-b8bfe811097d")]
		public static List<ContinuousUnit> ConvertTowerUnitListToContinuous(List<TowerUnit> _towerUnitList)
		{
			var result = new List<ContinuousUnit>();
			for (int i = 0; i < _towerUnitList.Count; i++)
				result.Add(new ContinuousUnit(_towerUnitList[i]));
			return result;
		}

		[global::OpenPCR.Migration("30ac47a1-8eef-4e61-927a-8c8cf4a587e3")]
		public static int GetSkillIdConsiderOverride(int _skillId, int _level)
		{
			MasterUnitSkillDataRf skillDataRf = ManagerSingleton<MasterDataManager>.Instance.masterUnitSkillDataRf;
			List<MasterUnitSkillDataRf.UnitSkillDataRf> reverseMatches = skillDataRf.GetListWithRfSkillId(_skillId);
			if (reverseMatches.Count > 0)
				_skillId = reverseMatches[0].skill_id;
			List<MasterUnitSkillDataRf.UnitSkillDataRf> ordered = skillDataRf.GetListWithSkillId(_skillId)
				.OrderBy(value => (int)value.min_lv).ToList();
			for (int i = 0; i < ordered.Count; i++)
			{
				if ((int)ordered[i].min_lv > _level)
					return i == 0 ? _skillId : (int)ordered[i - 1].rf_skill_id;
				if (i + 1 == ordered.Count)
					_skillId = ordered[i].rf_skill_id;
			}
			return _skillId;
		}

		[global::OpenPCR.Migration("3f2e43e7-7a35-4ebd-b359-d689af0d3f06")]
		public static int GetSpineCharaBaseAnimationNumber(int _unitId)
		{
			if (JudgeIsGuest(_unitId))
				return _unitId;
			MasterUnitData.UnitData unitData = ManagerSingleton<MasterDataManager>.Instance.masterUnitData.Get(_unitId);
			if (unitData != null)
				return (int)unitData.MotionType == 0 ? _unitId : 0;
			MasterUnitEnemyData.UnitEnemyData enemyData = ManagerSingleton<MasterDataManager>.Instance.masterUnitEnemyData.Get(_unitId);
			return enemyData == null || (int)enemyData.motion_type == 0 ? _unitId : 0;
		}

		[global::OpenPCR.Migration("ca834110-4591-41fe-803d-a826050f5ec2")]
		public static int GetSkinId(global::Elements.eAnimKitType _spineType, int _unitId, int _rarity, int _unitIdForParameter = 0, global::Elements.UnitDefine.eSkinType _skin = (UnitDefine.eSkinType)1, bool _isOther = false)
		{
			if (_rarity == -2)
			{
				int id = _unitIdForParameter != 0 ? _unitIdForParameter : _unitId;
				if (IsConversionUnit(id))
					id = GetOriginalUnitId(id);
				_rarity = GetSetSkinNo(Singleton<UserData>.Instance.UnitParameterDictionary[id].UniqueData, _skin, _isOther);
			}

			if (ResourceDefineAnimKit.IsSkinIdSpineType(_spineType))
				return GetSkinId(_unitId, _rarity);
			if (_spineType == eAnimKitType.SD_SHADOW && _rarity == 6)
				return GetSkinIdFromRarity(6);
			return _unitId;
		}

		public class SummonSkillData
		{
			public int SkillId { get; private set; }
			public int SkillLevel { get; private set; }
			public int Rarity { get; private set; }
			public int PromotionLevel { get; private set; }
			public int SummonTargetUnitId { get; private set; }

			[global::OpenPCR.Migration("6fae62f7-a81e-4aff-8921-9b6290b829e2")]
			public SummonSkillData(int _skillId, int _skillLevel, int _rarity, int _promotionLevel, int _summonTargetUnitId)
			{
				SkillId = _skillId;
				SkillLevel = _skillLevel;
				Rarity = _rarity;
				PromotionLevel = _promotionLevel;
				SummonTargetUnitId = _summonTargetUnitId;
			}
		}

		[global::OpenPCR.Migration("20e5ab8c-c7d8-4b2d-beed-350f2d23cd74")]
		public static int GetDefaultActionPatternId(int _id)
		{
			return unchecked(GetUnitResourceId(_id) * 100) | 1;
		}

		[global::OpenPCR.Migration("61f58396-4703-4b45-8286-603b6d42de8e")]
		public static int GetUnitResourceId(int _unitId)
		{
			if (_unitId < 1000000)
				return _unitId;
			MasterEnemyParameter.EnemyParameter data = ManagerSingleton<MasterDataManager>.Instance.masterEnemyParameter.GetFromAllKind(_unitId);
			return data != null ? (int)data.unit_id : _unitId;
		}

		[global::OpenPCR.Migration("2c5afd2a-34ae-4987-af59-d75ab3f61a2c")]
		public static int GetCharaId(int _unitId)
		{
			_unitId = GetUnitResourceId(_unitId);
			return _unitId / 100;
		}

		[global::OpenPCR.Migration("f7fe53bd-ebfd-4e1c-acf7-7e1a61be059b")]
		public static int GetSummonResourceId(int _unitId)
		{
			int resourceId = GetUnitResourceId(_unitId);
			return ManagerSingleton<MasterDataManager>.Instance.masterUnitData[resourceId].PrefabId;
		}

		[global::OpenPCR.Migration("0f05ca6a-9f1b-4beb-ad99-97b0e76994e1")]
		public static bool IsPersonUnit(int _unitId)
		{
			_unitId = GetUnitResourceId(_unitId);
			UnitDefine.UnitType unitType = getUnitType(_unitId);
			if (unitType != UnitDefine.UnitType.PERSON)
			{
				return unitType == UnitDefine.UnitType.GUEST;
			}

			return true;
		}

		[global::OpenPCR.Migration("f341c932-41dd-4db7-9b33-e125cfaa53e7")]
		public static bool IsUnitTypePersonOrSummonPerson(int _unitId)
		{
			UnitDefine.UnitType unitType = getUnitType(_unitId);
			if (unitType != UnitDefine.UnitType.PERSON)
			{
				return unitType == UnitDefine.UnitType.SUMMON_PERSON;
			}

			return true;
		}

		[global::OpenPCR.Migration("a4ef36bb-0a9d-4df8-8dc2-20638a07dd9b")]
		public static bool IsUnitTypeSummonPersonOnly(int _unitId)
		{
			return getUnitType(_unitId) == UnitDefine.UnitType.SUMMON_PERSON;
		}

		[global::OpenPCR.Migration("75a6a04a-65d6-471f-8406-b31dfe077732")]
		public static bool JudgeIsBoss(int _unitId)
		{
			_unitId = GetUnitResourceId(_unitId);
			return getUnitType(_unitId) == UnitDefine.UnitType.BOSS;
		}

		[global::OpenPCR.Migration("1b10f2eb-b661-423c-b57a-b0a09593ac96")]
		public static bool JudgeIsSummon(int _unitId)
		{
			_unitId = GetUnitResourceId(_unitId);
			UnitDefine.UnitType unitType = getUnitType(_unitId);
			if (unitType != UnitDefine.UnitType.SUMMON_PERSON)
			{
				return unitType == UnitDefine.UnitType.SUMMON_MONSTER;
			}

			return true;
		}

		[global::OpenPCR.Migration("f4d1a9e8-930d-43eb-aa85-41b9d4f4d8b5")]
		public static bool IsConversionUnit(int _unitId)
		{
			return (unchecked((uint)(_unitId - 170300)) >> 2) < 0x979U;
		}

		[global::OpenPCR.Migration("41a1f0ab-b7ae-47e6-8ee4-dd4546526552")]
		public static eUnitBattlePos GetUnitPosType(float _searchAreaWidth)
		{
			MasterPositionSetting.PositionSetting setting = ManagerSingleton<MasterDataManager>.Instance.masterPositionSetting.Get(1);
			if ((float)(int)setting.front >= _searchAreaWidth)
				return eUnitBattlePos.FRONT;
			return (float)(int)setting.middle < _searchAreaWidth
				? eUnitBattlePos.BACK
				: eUnitBattlePos.MIDDLE;
		}

		[global::OpenPCR.Migration("405c7647-af72-4fd0-972e-1c7044327463")]
		public static bool IsPlayableUnit(int _unitId)
		{
			return IsPlayableUnit(ManagerSingleton<MasterDataManager>.Instance.masterUnitData.Get(_unitId));
		}

		[global::OpenPCR.Migration("68d83eef-e911-4a7c-b08d-2682cf9bae67")]
		public static bool IsPlayableUnit(MasterUnitData.UnitData _unitData)
		{
			if (_unitData == null || !IsPersonUnit(_unitData.UnitId) || JudgeIsGuest(_unitData.UnitId) || (int)_unitData.Rarity < 1)
				return false;
			if ((int)_unitData.CutIn1 == 0)
				return false;
			if (JudgeIsBattleGuest(_unitData.UnitId) || IsConversionUnit(_unitData.UnitId))
				return false;
			return (int)_unitData.OriginalUnitId == 0;
		}

		[global::OpenPCR.Migration("5c440159-14aa-4f6b-93d7-b2acb1fa69ee")]
		public static bool JudgeIsBattleGuest(int _unitId)
		{
			return (unchecked((uint)(_unitId - 190000)) >> 4) < 0x271;
		}

		[global::OpenPCR.Migration("10c0c5b9-0e2a-42f7-8269-5c24ab399844")]
		public static long CalcUnitParameterForDamagedEnergy(eParamType _paramType, UnitCtrl _unitCtrl)
		{
			return global::OpenPCR.PreparedUnitInputBinding.Require(_unitCtrl.UnitParameter.UniqueData).DamagedEnergy((int)_paramType);
		}

		[global::OpenPCR.Migration("69e844ef-0e42-49ff-96ae-23d1363677bf")]
		public static bool JudgeIsGuest(int _unitId)
		{
			return getUnitType(_unitId) == UnitDefine.UnitType.GUEST;
		}

		[global::OpenPCR.Migration("10398c5f-48e1-4d87-a3e3-d4673a93f2c9")]
		public static bool IsEnemyUnit(int _unitId)
		{
			UnitDefine.UnitType unitType = getUnitType(_unitId);
			if (_unitId < 1000000 && unitType != UnitDefine.UnitType.MONSTER)
			{
				return unitType == UnitDefine.UnitType.BOSS;
			}

			return true;
		}

		[global::OpenPCR.Migration("10f6dd76-0d30-4707-ba82-60a3efbc9104")]
		public static bool IsQuestMonsterUnit(int _unitId)
		{
			return _unitId >= 1000000;
		}

		[global::OpenPCR.Migration("09ed8722-ea57-4270-a996-e3e72dd939af")]
		private static int round(double _x)
		{
			int value = OpenPCR.NativeNumeric.ToInt32(_x);
			if (_x - value >= 0.5 || isDoubleApproximately(value + 0.5, _x))
				value = unchecked(value + 1);
			return value;
		}

		private static bool isDoubleApproximately(double _a, double _b)
		{
			return Math.Abs(_b - _a) < Math.Max(1E-14 * Math.Max(Math.Abs(_a), Math.Abs(_b)), 2.220446049250313E-16);
		}

		[global::OpenPCR.Migration("1ae5ab0d-a55c-47a8-a284-d4e68e8562e0")]
		private static UnitDefine.UnitType getUnitType(int _unitId)
		{
			return (UnitDefine.UnitType)(_unitId / 100000);
		}

		[global::OpenPCR.Migration("7398af7e-e802-4f6d-bb61-9f2063c7bab7")]
		public static int CalcOverall(int _unitId, int _level, int _rarity, int _rank, UnitData _unitUniqueData)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.UnitUtility::CalcOverall(System.Int32,System.Int32,System.Int32,System.Int32,Elements.UnitData):System.Int32") /* Native Master/account growth parameter or display-power producer; complete Native capture retained; prepared battle parameters required, full growth calculator outside current scope */;
		}

		public static int CalcUnitBaseParameter(eParamType _paramType, int _unitId, int _level, int _rarity, int _rank)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.UnitUtility::CalcUnitBaseParameter(Elements.eParamType,System.Int32,System.Int32,System.Int32,System.Int32):System.Int32") /* Native Master/account growth parameter or display-power producer; complete Native capture retained; prepared battle parameters required, full growth calculator outside current scope */;
		}

		public static int CalcEquipParameter(eParamType _paramType, List<EquipSlot> _equipSlotList, List<EquipSlot> _uniqueEquipSlotList)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.UnitUtility::CalcEquipParameter(Elements.eParamType,System.Collections.Generic.List`1<Elements.EquipSlot>,System.Collections.Generic.List`1<Elements.EquipSlot>):System.Int32") /* Native Master/account growth parameter or display-power producer; complete Native capture retained; prepared battle parameters required, full growth calculator outside current scope */;
		}

		[global::OpenPCR.Migration("66e2adb1-99f5-4afd-b713-23ac3b863c00")]
		public static UnitParameter CreateUnitParameterFromEnemyParameter(int _enemyParameterId)
		{
			return (UnitParameter)global::OpenPCR.PreparedParameters.Enemy(ManagerSingleton<MasterDataManager>.Instance, _enemyParameterId);
		}

		[global::OpenPCR.Migration("9a5638a2-1356-4830-b594-985de2a582c4")]
		private static List<SkillLevelInfo> createSkillLevelFromEnemyParameter(List<int> _mainSkills, List<int> _levels, List<int> _plusSkills, List<bool> _plusSkillFlags)
		{
			List<SkillLevelInfo> list = new List<SkillLevelInfo>();
			int i = 0;
			for (int count = _mainSkills.Count; i < count; i++)
			{
				int num = _mainSkills[i];
				if (num != 0)
				{
					if (_plusSkillFlags != null && _plusSkillFlags.Count > i && _plusSkillFlags[i])
					{
						num = _plusSkills[i];
					}

					SkillLevelInfo skillLevelInfo = new SkillLevelInfo();
					skillLevelInfo.SetSkillId(num);
					skillLevelInfo.SetSkillLevel(_levels[i]);
					list.Add(skillLevelInfo);
				}
			}

			return list;
		}

		[global::OpenPCR.Migration("d4d83d9f-c837-416d-ace6-dac42d36a388")]
		public static void CalcParamAndSkill(UnitData _unitData, SummonSkillData _summonSkillData = null, bool _isPowerLocal = false, bool _useBattleRarity = true)
		{
			if (ManagerSingleton<MasterDataManager>.Instance == null)
			{
				return;
			}

			MasterDataManager instance = ManagerSingleton<MasterDataManager>.Instance;
			if (Singleton<UserData>.Instance == null || Singleton<UserData>.Instance.UnitParameterDictionary == null)
			{
				return;
			}

			int unitId = _unitData.Id;
			int level = _unitData.UnitLevel;
			int rarity = _unitData.UnitRarity;
			if (_useBattleRarity)
			{
				rarity = _unitData.GetCurrentRarity();
			}

			int promotionLevel = (int)_unitData.PromotionLevel;
			if (IsQuestMonsterUnit(_unitData.Id))
			{
				unitId = GetUnitResourceId(_unitData.Id);
				MasterEnemyParameter.EnemyParameter fromAllKind = instance.masterEnemyParameter.GetFromAllKind(_unitData.Id);
				StatusParam statusParam = new StatusParam();
				statusParam.SetHp((long)fromAllKind.hp);
				statusParam.SetAtk(fromAllKind.atk);
				statusParam.SetDef(fromAllKind.def);
				statusParam.SetMagicStr(fromAllKind.magic_str);
				statusParam.SetMagicDef(fromAllKind.magic_def);
				statusParam.SetPhysicalCritical(fromAllKind.physical_critical);
				statusParam.SetMagicCritical(fromAllKind.magic_critical);
				statusParam.SetDodge(fromAllKind.dodge);
				statusParam.SetLifeSteal(fromAllKind.life_steal);
				statusParam.SetWaveHpRecovery(fromAllKind.wave_hp_recovery);
				statusParam.SetWaveEnergyRecovery(fromAllKind.wave_energy_recovery);
				statusParam.SetPhysicalPenetrate(fromAllKind.physical_penetrate);
				statusParam.SetMagicPenetrate(fromAllKind.magic_penetrate);
				statusParam.SetEnergyRecoveryRate(fromAllKind.energy_recovery_rate);
				statusParam.SetHpRecoveryRate(fromAllKind.hp_recovery_rate);
				statusParam.SetEnergyReduceRate(fromAllKind.energy_reduce_rate);
				statusParam.SetAccuracy(fromAllKind.accuracy);
				_unitData.SetResistStatusId(fromAllKind.resist_status_id);
				_unitData.SetResistVariationId(fromAllKind.resist_variation_id);
				StatusParam equipParam = new StatusParam();
				UnitParam unitParam = new UnitParam();
				unitParam.SetBaseParam(statusParam);
				unitParam.SetEquipParam(equipParam);
				_unitData.SetUnitParam(unitParam);
				if (_isPowerLocal)
				{
					int power = calcOverallEnemy(statusParam, _unitData);
					_unitData.SetPower(power);
				}
			}
			else
			{
				if (_summonSkillData != null)
				{
					level = _summonSkillData.SkillLevel;
					rarity = _summonSkillData.Rarity;
					promotionLevel = _summonSkillData.PromotionLevel;
				}

				StatusParam baseParam = CreateBaseParam(unitId, level, rarity, promotionLevel);
				StatusParam equipParam2 = CreateEquipParam(_unitData);
				if (_unitData.ExSkill.Count > 0 && (int)_unitData.ExSkill[0].SkillLevel > 0)
				{
					MasterUnitSkillData.UnitSkillData unitSkillData = instance.masterUnitSkillData.Get(_unitData.Id);
					int skillId = ((_unitData.GetCurrentRarity() >= 5) ? unitSkillData.ExSkillEvolutionIds[0] : unitSkillData.ExSkillIds[0]);
					_unitData.ExSkill[0].SetSkillId(skillId);
				}

				int conversionUnit = GetConversionUnit(_unitData.Id, _unitData.UnitRarity);
				if ((int)_unitData.Id != conversionUnit)
				{
					SetConversionSkill(ref _unitData, conversionUnit);
				}

				UnitParam unitParam2 = new UnitParam();
				unitParam2.SetBaseParam(baseParam);
				unitParam2.SetEquipParam(equipParam2);
				_unitData.SetUnitParam(unitParam2);
				if (_isPowerLocal)
				{
					int power2 = CalcOverall(unitId, level, rarity, promotionLevel, _unitData);
					_unitData.SetPower(power2);
				}
			}
		}

		[global::OpenPCR.Migration("6bc7d96e-496e-49ed-a343-46b04533a355")]
		public static void SetConversionSkill(ref UnitData _unitData, int _conversionId)
		{
			MasterDataManager instance = ManagerSingleton<MasterDataManager>.Instance;
			_ = Singleton<UserData>.Instance;
			MasterUnitSkillData.UnitSkillData unitSkillData = instance.masterUnitSkillData.Get(_conversionId);
			_unitData.UnionBurst[0].SetSkillId(unitSkillData.UnionBurstEvolutionIds[0]);
			_unitData.UnionBurst[0].SetSkillLevel(_unitData.UnionBurst[0].SkillLevel);
			_unitData.UnionBurst[0].SetSlotNumber(_unitData.UnionBurst[0].SlotNumber);
			if (_unitData.MainSkill.Count < 1)
			{
				SkillLevelInfo item = new SkillLevelInfo();
				_unitData.MainSkill.Add(item);
			}

			_unitData.MainSkill[0].SetSkillId(unitSkillData.MainSkillEvolutionIds[0]);
			_unitData.MainSkill[0].SetSlotNumber(0);
			if (_unitData.MainSkill.Count < 2)
			{
				SkillLevelInfo item2 = new SkillLevelInfo();
				_unitData.MainSkill.Add(item2);
			}

			var secondSlot = _unitData.UniqueEquipSlot2;
			bool isSecondEquipped = secondSlot != null && secondSlot.IsSlot;
			_unitData.MainSkill[1].SetSkillId(isSecondEquipped ? unitSkillData.MainSkillEvolutionIds[1] : unitSkillData.MainSkillIds[1]);
			_unitData.MainSkill[1].SetSlotNumber(1);
			if (_unitData.ExSkill.Count < 1)
			{
				SkillLevelInfo item3 = new SkillLevelInfo();
				_unitData.ExSkill.Add(item3);
			}

			_unitData.ExSkill[0].SetSkillId(unitSkillData.ExSkillEvolutionIds[0]);
			_unitData.ExSkill[0].SetSlotNumber(0);
		}

		[global::OpenPCR.Migration("dbfcc278-411f-4450-8f75-3321e6cc5b28")]
		public static StatusParam CreateBaseParam(int _unitId, int _level, int _rarity, int _rank)
		{
			StatusParam statusParam = new StatusParam();
			statusParam.SetHp(CalcUnitBaseParameter(eParamType.HP, _unitId, _level, _rarity, _rank));
			statusParam.SetAtk(CalcUnitBaseParameter(eParamType.ATK, _unitId, _level, _rarity, _rank));
			statusParam.SetDef(CalcUnitBaseParameter(eParamType.DEF, _unitId, _level, _rarity, _rank));
			statusParam.SetMagicStr(CalcUnitBaseParameter(eParamType.MAGIC_ATK, _unitId, _level, _rarity, _rank));
			statusParam.SetMagicDef(CalcUnitBaseParameter(eParamType.MAGIC_DEF, _unitId, _level, _rarity, _rank));
			statusParam.SetPhysicalCritical(CalcUnitBaseParameter(eParamType.PHYSICAL_CRITICAL, _unitId, _level, _rarity, _rank));
			statusParam.SetMagicCritical(CalcUnitBaseParameter(eParamType.MAGIC_CRITICAL, _unitId, _level, _rarity, _rank));
			statusParam.SetDodge(CalcUnitBaseParameter(eParamType.DODGE, _unitId, _level, _rarity, _rank));
			statusParam.SetLifeSteal(CalcUnitBaseParameter(eParamType.LIFE_STEAL, _unitId, _level, _rarity, _rank));
			statusParam.SetWaveHpRecovery(CalcUnitBaseParameter(eParamType.WAVE_HP_RECOVERY, _unitId, _level, _rarity, _rank));
			statusParam.SetWaveEnergyRecovery(CalcUnitBaseParameter(eParamType.WAVE_ENERGY_RECOVERY, _unitId, _level, _rarity, _rank));
			statusParam.SetPhysicalPenetrate(CalcUnitBaseParameter(eParamType.PHYSICAL_PENETRATE, _unitId, _level, _rarity, _rank));
			statusParam.SetMagicPenetrate(CalcUnitBaseParameter(eParamType.MAGIC_PENETRATE, _unitId, _level, _rarity, _rank));
			statusParam.SetEnergyRecoveryRate(CalcUnitBaseParameter(eParamType.ENERGY_RECOVERY_RATE, _unitId, _level, _rarity, _rank));
			statusParam.SetHpRecoveryRate(CalcUnitBaseParameter(eParamType.HP_RECOVERY_RATE, _unitId, _level, _rarity, _rank));
			statusParam.SetEnergyReduceRate(CalcUnitBaseParameter(eParamType.ENERGY_REDUCE_RATE, _unitId, _level, _rarity, _rank));
			statusParam.SetAccuracy(CalcUnitBaseParameter(eParamType.ACCURACY, _unitId, _level, _rarity, _rank));
			return statusParam;
		}

		[global::OpenPCR.Migration("ba3bc659-dfee-462c-8ac1-235dcf8279f2")]
		public static StatusParam CreateEquipParam(UnitData _unitData)
		{
			StatusParam statusParam = new StatusParam();
			List<EquipSlot> equipSlot = _unitData.EquipSlot;
			List<EquipSlot> uniqueEquipSlot = _unitData.UniqueEquipSlot;
			statusParam.SetHp(CalcEquipParameter(eParamType.HP, equipSlot, uniqueEquipSlot));
			statusParam.SetAtk(CalcEquipParameter(eParamType.ATK, equipSlot, uniqueEquipSlot));
			statusParam.SetDef(CalcEquipParameter(eParamType.DEF, equipSlot, uniqueEquipSlot));
			statusParam.SetMagicStr(CalcEquipParameter(eParamType.MAGIC_ATK, equipSlot, uniqueEquipSlot));
			statusParam.SetMagicDef(CalcEquipParameter(eParamType.MAGIC_DEF, equipSlot, uniqueEquipSlot));
			statusParam.SetPhysicalCritical(CalcEquipParameter(eParamType.PHYSICAL_CRITICAL, equipSlot, uniqueEquipSlot));
			statusParam.SetMagicCritical(CalcEquipParameter(eParamType.MAGIC_CRITICAL, equipSlot, uniqueEquipSlot));
			statusParam.SetDodge(CalcEquipParameter(eParamType.DODGE, equipSlot, uniqueEquipSlot));
			statusParam.SetLifeSteal(CalcEquipParameter(eParamType.LIFE_STEAL, equipSlot, uniqueEquipSlot));
			statusParam.SetWaveHpRecovery(CalcEquipParameter(eParamType.WAVE_HP_RECOVERY, equipSlot, uniqueEquipSlot));
			statusParam.SetWaveEnergyRecovery(CalcEquipParameter(eParamType.WAVE_ENERGY_RECOVERY, equipSlot, uniqueEquipSlot));
			statusParam.SetPhysicalPenetrate(CalcEquipParameter(eParamType.PHYSICAL_PENETRATE, equipSlot, uniqueEquipSlot));
			statusParam.SetMagicPenetrate(CalcEquipParameter(eParamType.MAGIC_PENETRATE, equipSlot, uniqueEquipSlot));
			statusParam.SetEnergyRecoveryRate(CalcEquipParameter(eParamType.ENERGY_RECOVERY_RATE, equipSlot, uniqueEquipSlot));
			statusParam.SetHpRecoveryRate(CalcEquipParameter(eParamType.HP_RECOVERY_RATE, equipSlot, uniqueEquipSlot));
			statusParam.SetEnergyReduceRate(CalcEquipParameter(eParamType.ENERGY_REDUCE_RATE, equipSlot, uniqueEquipSlot));
			statusParam.SetAccuracy(CalcEquipParameter(eParamType.ACCURACY, equipSlot, uniqueEquipSlot));
			return statusParam;
		}

		[global::OpenPCR.Migration("d0a0fdfc-8bf7-4e29-a524-31d69ae789a0")]
		public static UnitParameter CreateSummonUnitParameter(UnitParameter _owner, int _id, int _level, bool _considerEquipment, bool _mainSkill1Evolved)
		{
			return (UnitParameter)global::OpenPCR.PreparedParameters.Summon(ManagerSingleton<MasterDataManager>.Instance, _owner, _id, _level, _considerEquipment, _mainSkill1Evolved);
		}

		[global::OpenPCR.Migration("722ca6a4-b4dc-4dc9-bb7c-227eea2cede2")]
		public static void calcurateSkillLevelInfo(UnitParameter _target, int _promotionLevel, int _level, bool _considerExSkill, bool _mainSkill1Evolved, bool _exSkillEvolved)
		{
			List<SkillLevelInfo> mainSkill = createSkillLevel(_target.SkillData.MainSkillIds, _promotionLevel, _level, _isMainSkill: true, _mainSkill1Evolved, _target.SkillData.MainSkillEvolutionIds);
			_target.UniqueData.SetMainSkill(mainSkill);
			List<SkillLevelInfo> unionBurst = createSkillLevel(_target.SkillData.UnionBurstIds, _promotionLevel, _level);
			_target.UniqueData.SetUnionBurst(unionBurst);
			if (_considerExSkill)
			{
				int key = 301;
				MasterUnlockSkillData.UnlockSkillData value = null;
				if (ManagerSingleton<MasterDataManager>.Instance.masterUnlockSkillData.dictionary.TryGetValue(key, out value) && (int)value.promotion_level <= _promotionLevel)
				{
					List<SkillLevelInfo> exSkill = createSkillLevel(_exSkillEvolved ? new List<int> { _target.SkillData.ex_skill_evolution_1 } : _target.SkillData.ExSkillIds, _promotionLevel, _level);
					_target.UniqueData.SetExSkill(exSkill);
				}
			}
		}

		[global::OpenPCR.Migration("3ed1c442-6591-4b5b-a702-bbe09d33fd84")]
		private static List<SkillLevelInfo> createSkillLevel(List<int> _mainSkills, int _promotionLevel, int _level, bool _isMainSkill = false, bool _mainSkill1Evolved = false, List<int> _mainSkillEvolvedIds = null)
		{
			List<SkillLevelInfo> list = new List<SkillLevelInfo>();
			int i = 0;
			for (int count = _mainSkills.Count; i < count; i++)
			{
				int num = _mainSkills[i];
				if ((_isMainSkill & _mainSkill1Evolved) && i == 0)
				{
					num = ((_mainSkillEvolvedIds[i] == 0) ? num : _mainSkillEvolvedIds[0]);
				}

				if (num == 0)
				{
					continue;
				}

				SkillLevelInfo skillLevelInfo = new SkillLevelInfo();
				skillLevelInfo.SetSkillId(num);
				if (_isMainSkill)
				{
					int key = 201 + i;
					MasterUnlockSkillData.UnlockSkillData value = null;
					if (ManagerSingleton<MasterDataManager>.Instance.masterUnlockSkillData.dictionary.TryGetValue(key, out value))
					{
						int num2 = value.promotion_level;
						skillLevelInfo.SetSkillLevel((_promotionLevel >= num2) ? _level : 0);
					}
				}
				else
				{
					skillLevelInfo.SetSkillLevel(_level);
				}

				list.Add(skillLevelInfo);
			}

			return list;
		}

		[global::OpenPCR.Migration("300ec191-b401-44bf-b40c-b3adcc7b1795")]
		private static List<SummonSkillData> searchSummonSkill(UnitParameter _unitParameter)
		{
			UnitData unitData = _unitParameter.UniqueData;
			MasterDataManager masterDatamanager = ManagerSingleton<MasterDataManager>.Instance;
			List<SummonSkillData> summonSkillList = new List<SummonSkillData>();
			Action<List<SkillLevelInfo>> action = delegate (List<SkillLevelInfo> _skillLevelList)
			{
				for (int i = 0; i < _skillLevelList.Count; i++)
				{
					SkillLevelInfo skillLevelInfo2 = _skillLevelList[i];
					if ((int)skillLevelInfo2.SkillId != 0)
					{
						Elements.Utility.SkillData skillData = Elements.Utility.SkillUtility.GetSkillData(skillLevelInfo2.SkillId, skillLevelInfo2.SkillLevel);
						for (int j = 0; j < skillData.ActionInfoList.Count; j++)
						{
							Elements.Utility.ActionData actionData = skillData.ActionInfoList[j];
							MasterSkillAction.SkillAction skillAction = masterDatamanager.masterSkillAction.Get(actionData.ActionId);
							if ((ushort)skillAction.action_type == 15)
							{
								int num2 = OpenPCR.NativeNumeric.ToInt32(actionData.ActionValue.Value4) / 100;
								if (num2 == 0)
								{
									num2 = unitData.GetCurrentRarity();
								}

								int num3 = OpenPCR.NativeNumeric.ToInt32(actionData.ActionValue.Value4) % 100;
								if (num3 == 0)
								{
									num3 = (int)unitData.PromotionLevel;
								}

								global::System.Int32 action_detail_ = skillAction.action_detail_2;
								SummonSkillData item = new SummonSkillData(skillLevelInfo2.SkillId, skillLevelInfo2.SkillLevel, num2, num3, action_detail_);
								summonSkillList.Add(item);
							}
						}
					}
				}
			};
			action(unitData.UnionBurst);
			action(unitData.MainSkill);
			action(unitData.ExSkill);
			List<SkillLevelInfo> list = new List<SkillLevelInfo>();
			for (int num = 0; num < _unitParameter.SkillData.SpSkillIds.Count; num++)
			{
				SkillLevelInfo skillLevelInfo = new SkillLevelInfo();
				skillLevelInfo.SetSkillId(_unitParameter.SkillData.SpSkillIds[num]);
				list.Add(skillLevelInfo);
			}

			action(list);
			action(unitData.FreeSkill);
			return summonSkillList;
		}

		[global::OpenPCR.Migration("2638369f-3c01-49a3-83f1-14f7db3f1c97")]
		public static void SortPartyByPosition(List<UnitParameter> _partyList)
		{
			_partyList?.Sort((UnitParameter _a, UnitParameter _b) => compareUnitOrder(_a.MasterData, _b.MasterData));
		}

		[global::OpenPCR.Migration("73f482bb-54bd-4ae9-9f3e-099c58aea546")]
		private static int calcOverallEnemy(StatusParam _baseParam, UnitData _unitData)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.UnitUtility::calcOverallEnemy(Elements.StatusParam,Elements.UnitData):System.Int32") /* Native Master/account growth parameter or display-power producer; complete Native capture retained; prepared battle parameters required, full growth calculator outside current scope */;
		}

		[global::OpenPCR.Migration("ebc1bb3a-1d90-45b1-b73d-e053352d5149")]
		private static int compareUnitOrder(MasterUnitData.UnitData _aUnitData, MasterUnitData.UnitData _bUnitData)
		{
			int num = _aUnitData.SearchAreaWidth;
			int num2 = _bUnitData.SearchAreaWidth;
			if (num == num2)
			{
				return (int)_aUnitData.UnitId - (int)_bUnitData.UnitId;
			}

			return num - num2;
		}

		[global::OpenPCR.Migration("6b854c51-e5cf-48c1-86fd-dcdd6f56a412")]
		public static int GetSkinId(int _unitId, int _rarity)
		{
			if (IsUnitTypePersonOrSummonPerson(_unitId))
			{
				bool summon = IsUnitTypeSummonPersonOnly(_unitId);
				if (_rarity != 0 || !summon)
				{
					if ((uint)_rarity < 3)
						_unitId = unchecked(_unitId + 10);
					else if ((uint)unchecked(_rarity - 3) < 3)
						_unitId = unchecked(_unitId + 30);
					else if (_rarity == 6)
						_unitId = unchecked(_unitId + 60);
				}
			}

			return _unitId;
		}

		[global::OpenPCR.Migration("6ea98c5a-fd81-419e-af1a-1339add81956")]
		public static int SkinIdToUnitId(int _skinId)
		{
			int num = _skinId / 100;
			int num2 = SkinIdToClassId(_skinId);
			return num * 100 + num2;
		}

		[global::OpenPCR.Migration("b7a5dfa7-ec33-4d37-9473-9efd07d8ab27")]
		public static int SkinIdToClassId(int _skinId)
		{
			return _skinId % 10;
		}

		[global::OpenPCR.Migration("475fe108-d7bc-4a67-8a15-0e82d2dced35")]
		public static int GetUnitSetCount(int _unitId)
		{
			var data = ManagerSingleton<MasterDataManager>.Instance.masterUnitPosAdjustment.Get(_unitId);
			return data != null ? data.UnitIdList.Count : 1;
		}

		[global::OpenPCR.Migration("896264a1-9fea-4da3-b93a-48615ce18cdb")]
		public static double CalcDamageTpReduceRate(int _teamLevel, int _promotionLevel)
		{
			if (_teamLevel == 0)
				return 1.0;
			MasterEReduction.EReduction damageTpReduce = ManagerSingleton<MasterDataManager>.Instance.masterEReduction.dictionary.Values
				.OrderByDescending(data => (int)data.Border)
				.First(data => _teamLevel >= (int)data.Border);
			return (damageTpReduce.DamageTpReduceRateArray ?? getDamageTpReduceArrayAndCache())[_promotionLevel - 1];

			double[] getDamageTpReduceArrayAndCache()
			{
				int maximumRank = Enum.GetValues(typeof(ePromotionLevel)).Cast<int>().Max();
				damageTpReduce.DamageTpReduceRateArray = new double[maximumRank];
				for (int rank = maximumRank; rank > 0; --rank)
					setRate(rank);
				return damageTpReduce.DamageTpReduceRateArray;
			}

			void setRate(int rank)
			{
				int initialRankDiff = Math.Max(0, (int)damageTpReduce.Threshold1 + 1 - rank);
				double initialTpReduceRate = 1.0;
				reduceRateValue(damageTpReduce.Threshold1, damageTpReduce.Threshold2, damageTpReduce.Value1);
				reduceRateValue(damageTpReduce.Threshold2, damageTpReduce.Threshold3, damageTpReduce.Value2);
				reduceRateValue(damageTpReduce.Threshold3, damageTpReduce.Threshold4, damageTpReduce.Value3);
				reduceRateValue(damageTpReduce.Threshold4, damageTpReduce.Threshold5, damageTpReduce.Value4);
				reduceRateValue(damageTpReduce.Threshold5, 0, damageTpReduce.Value5);
				damageTpReduce.DamageTpReduceRateArray[rank - 1] = initialTpReduceRate;

				void reduceRateValue(int upperLevel, int lowerLevel, double reduceValue)
				{
					if (initialRankDiff > 0)
					{
						int count = Math.Min(initialRankDiff, upperLevel - lowerLevel);
						initialRankDiff -= count;
						initialTpReduceRate -= (double)count * reduceValue;
					}
				}
			}
		}

		[global::OpenPCR.Migration("06d77121-e23f-4e70-bcb6-215738fde36d")]
		public static int GetConversionUnit(int _unitId, int _rarity)
		{
			var user = Singleton<UserData>.Instance;
			if (_rarity == -2)
			{
				if (user == null || user.UnitParameterDictionary == null || !user.UnitParameterDictionary.ContainsKey(_unitId))
					return _unitId;
				_rarity = user.UnitParameterDictionary[_unitId].UniqueData.UnitRarity;
			}

			if (_rarity < 6)
				return _unitId;
			var manager = ManagerSingleton<MasterDataManager>.Instance;
			if (ReferenceEquals(manager, null) || manager.masterUnitConversion == null)
				return _unitId;
			var conversion = manager.masterUnitConversion.Get(_unitId);
			return conversion != null ? (int)conversion.unit_id : _unitId;
		}

		[global::OpenPCR.Migration("024b3a93-228c-4f02-993d-80cef38c5ea6")]
		public static int GetOriginalUnitId(int _unitId)
		{
			if (!IsConversionUnit(_unitId))
				return _unitId;
			var data = ManagerSingleton<MasterDataManager>.Instance.masterUnitConversion.GetWithUnitId(_unitId);
			return data != null ? (int)data.original_unit_id : _unitId;
		}

		[global::OpenPCR.Migration("d81b5329-9422-4dfb-b8d5-f9002290f79e")]
		public static UnitParameter MightConvertRarity6Unit(in UnitParameter _targetUnit)
		{
			int conversionUnit = GetConversionUnit(_targetUnit.UniqueData.Id, _targetUnit.UniqueData.UnitRarity);
			if (conversionUnit == (int)_targetUnit.UniqueData.Id)
			{
				return _targetUnit;
			}

			UnitData unitData = _targetUnit.UniqueData.CreateCopyData();
			unitData.SetId(conversionUnit);
			return new UnitParameter(unitData);
		}

		[global::OpenPCR.Migration("0361b7fb-c3ca-46c1-9986-8d0ed83fdf62")]
		public static UnitParameter AlterUnitDataLevelLimit(UnitParameter _param)
		{
			UnitParameter obj = new UnitParameter
			{
				UniqueDataForView = _param.UniqueDataForView,
				MasterData = _param.MasterData,
				SkillData = _param.SkillData
			};
			_param.UpdateLimitedUnitData();
			obj.UniqueData = _param.LimitedUnitData;
			return obj;
		}

		[global::OpenPCR.Migration("4af84a47-a97b-46fa-b688-711adc5f2f1f")]
		public static int GetSetSkinNo(UnitData _unitData, UnitDefine.eSkinType _type, bool _otherUser = false, UnitCtrl _unit = null)
		{
			return GetSetSkinNo(_unitData.UnitRarity, _unitData.SkinData, _type, _otherUser, _unit);
		}

		[global::OpenPCR.Migration("26682312-1363-486f-b709-5c6bfae1a7bd")]
		public static int GetSetSkinNo(int _rarity, SkinData _skinData, UnitDefine.eSkinType _type, bool _otherUser = false, UnitCtrl _unit = null)
		{
			int result = getSetSkinNoImpl(_rarity, _skinData, _type, _otherUser);
			if (_unit != null)
				_unit.SkinRarity = result;
			return result;
		}

		[global::OpenPCR.Migration("0bb87894-1e95-485f-b47b-63541cc79151")]
		private static int getSetSkinNoImpl(int _rarity, SkinData _skinData, UnitDefine.eSkinType _type, bool _otherUser = false)
		{
			if (_type == UnitDefine.eSkinType.Motion)
				return _skinData == null ? 0 : GetSetSkinData(_skinData, _type);
			int result = GetSkinIdFromRarity(_rarity);
			if (IsChangeSkin(_rarity, _otherUser))
			{
				int skin = GetSetSkinData(_skinData, _type);
				if (skin != 0)
					return skin;
			}

			return result;
		}

		[global::OpenPCR.Migration("7703d6de-282e-4e06-a28b-8004384d9ee4")]
		public static int GetSetSkinData(SkinData _skin, UnitDefine.eSkinType _type)
		{
			if (_skin == null)
				return 0;
			switch (_type)
			{
				case UnitDefine.eSkinType.Icon:
					return _skin.IconSkinId;
				case UnitDefine.eSkinType.Sd:
					return _skin.SdSkinId;
				case UnitDefine.eSkinType.Still:
					return _skin.StillSkinId;
				case UnitDefine.eSkinType.Motion:
					return _skin.MotionId;
				default:
					return 0;
			}
		}

		[global::OpenPCR.Migration("6ca996bc-51e1-48ec-b27c-fc456e62ef5a")]
		public static bool IsChangeSkin(int _rarity, bool _otherUser = false)
		{
			if (!_otherUser)
				return _rarity >= 3;
			bool enabled = ManagerSingleton<SaveDataManager>.Instance.ApplySkinSettingsFromOtherUsers;
			return _rarity >= 3 && enabled;
		}

		[global::OpenPCR.Migration("62a2313e-df0f-472e-9069-9cb3ffe62493")]
		public static int GetSkinIdFromRarity(int _rarity)
		{
			if ((uint)unchecked(_rarity - 3) > 3)
				return 1;
			return _rarity == 6 ? 6 : 3;
		}


	}
}
