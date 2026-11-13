using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using Elements.Data;

namespace Elements
{
	public class BattleUtil
	{
		public BattleUtil()
		{
		}


		[global::OpenPCR.Migration("fb7ea895-d064-41e3-bb57-e2f3d9afd193")]
		public static float GetCriticalRate(float critical, int level, int targetLevel)
		{
			return critical * 0.05f * ((float)level / targetLevel) * 0.01f;
		}

		[global::OpenPCR.Migration("adaf6053-8ff9-4d2a-bb4e-6c9695ba5d5d")]
		public static float GetDodgeByLevelDiff(int _level, BasePartsData _target)
		{
			if (_target.Owner.IsBoss)
				return 1f;
			return 1f - Mathf.Max(_target.GetLevel() - _level, 0f) / 100f;
		}

		[global::OpenPCR.Migration("17a4b9f8-b0bf-4b31-bc4d-3157278e3a15")]
		public static float CalcDodgeByLevelDiffUseParam(int _level, UnitCtrl _source, BasePartsData _target, bool _isExEquipSkillAction, float _coefficient = 1f)
		{
			var owner = _target.Owner;
			float rate = 1f;
			if (!owner.IsBoss)
			{
				float value = (1f - Mathf.Max(unchecked(_target.GetLevel() - _level), 0f) / 100f) * _coefficient;
				rate = value > 1f ? 1f : value;
				long resist = owner.ComponentParameterClamped.AbnormalStateDodgeResistZero;
				long hit = _source.ComponentParameterClamped.AbnormalStateDodgeHitZero;
				if (!_source.IsBonusEnemy && !_source.IsBoss && !_isExEquipSkillAction)
					return rate / ((float)resist / 10000f + 1f) * ((float)hit / 10000f + 1f);
			}

			return rate;
		}

		[global::OpenPCR.Migration("caf9347b-8b38-4395-84f6-c22f9268fe16")]
		public static bool IsCheckSupportChara(eBattleCategory _battleCategory)
		{
			uint category = (uint)_battleCategory;
			if (category > (uint)eBattleCategory.SEVEN_QUEST)
				return false;
			if (((1L << (int)category) & 0x1FFC8F9EE38C607L) != 0)
				return true;
			if (_battleCategory == eBattleCategory.HATSUNE_SPECIAL_BATTLE)
				throw new global::OpenPCR.OutOfBoundaryException("Elements.BattleUtil.IsCheckSupportChara: HatsuneSpecial difficulty/support selection outside clan battle and training");
			return false;
		}

		[global::OpenPCR.Migration("120b739e-311e-4714-973d-6c8438d559f8")]
		public static bool IsClanBattleCategory(eBattleCategory _battleCategory)
		{
			return _battleCategory == eBattleCategory.CLAN_BATTLE || _battleCategory == eBattleCategory.REPLAY_CLAN_BATTLE || _battleCategory == eBattleCategory.TRIAL_CLAN_BATTLE;
		}

		[global::OpenPCR.Migration("30cdde9d-008e-4f35-b1cb-76fca26f2bd9")]
		public static UnitData GetBattleUnitData(UnitParameter _unitParameter, eBattleCategory _battleCategory)
		{
			if (_unitParameter == null)
				return null;
			if (IsClanBattleCategory(_battleCategory))
				return _unitParameter.LimitedUnitData;
			return _unitParameter.UniqueData;
		}

		[global::OpenPCR.Migration("81cf9269-87fa-4fee-a474-15601f80d52c")]
		public static bool IsQuestSupportUnit(eBattleCategory _battleCategory, int _unitId)
		{
			TempData tempData = Singleton<TempData>.Instance;
			uint category = (uint)_battleCategory;
			bool eligible = category <= (uint)eBattleCategory.HATSUNE_BATTLE ? ((1U << (int)category) & 0x403U) != 0 : unchecked(category - 46U) < 11U || (category <= (uint)eBattleCategory.SEVEN_SPECIAL_BOSS && ((1L << (int)category) & 0x400088000010000L) != 0);
			if (!eligible)
				return false;
			SupportUnitDefine.QuestStartParam support = tempData.SupportQuestStartParam;
			return support != null && support.ViewerId != 0 && support.UnitId != 0 && support.UnitId == _unitId;
		}

		[global::OpenPCR.Migration("be44f53e-899d-447a-a556-5103996c0585")]
		public static bool Approximately(float _a, float _b)
		{
			return Mathf.Abs(_b - _a) < Mathf.Max(0.00001f * Mathf.Max(Mathf.Abs(_a), Mathf.Abs(_b)), Mathf.Epsilon * 8f);
		}

		[global::OpenPCR.Migration("53673c06-dad7-4106-bada-3756b3dd3b0d")]
		public static bool Approximately(double _a, double _b)
		{
			return Math.Abs(_b - _a) < Math.Max(1e-14 * Math.Max(Math.Abs(_a), Math.Abs(_b)), 2.220446049250313e-16);
		}

		[global::OpenPCR.Migration("72b7e9a9-51f3-47b7-aafc-089ee02a2088")]
		public static bool LessThanOrApproximately(float _lhs, float _rhs)
		{
			return _lhs < _rhs || Approximately(_lhs, _rhs);
		}

		[global::OpenPCR.Migration("9c30766e-39b0-43d6-b0b0-8b15e7d45e77")]
		public static bool LessThanOrApproximately(double _lhs, double _rhs)
		{
			return _lhs < _rhs || Approximately(_lhs, _rhs);
		}

		[global::OpenPCR.Migration("acd79a9f-7fdd-40ea-a2ab-eb188ca93a79")]
		public static int FloatToInt(float _num)
		{
			float rounded = _num >= 0f ? (float)Math.Ceiling(_num) : (float)Math.Floor(_num);
			int result = global::OpenPCR.NativeNumeric.ToInt32(rounded);
			return Approximately(_num, result) ? result : global::OpenPCR.NativeNumeric.ToInt32(_num);
		}

		[global::OpenPCR.Migration("db503db4-c128-46db-a55d-5c283ca3d10a")]
		public static int DoubleToInt(double _num)
		{
			double rounded = _num >= 0.0 ? Math.Ceiling(_num) : Math.Floor(_num);
			int result = global::OpenPCR.NativeNumeric.ToInt32(rounded);
			return Approximately(_num, result) ? result : global::OpenPCR.NativeNumeric.ToInt32(_num);
		}

		[global::OpenPCR.Migration("2baf4527-1465-42de-8219-ef8a3270b350")]
		public static long DoubleToLong(double _num)
		{
			double rounded = _num >= 0.0 ? Math.Ceiling(_num) : Math.Floor(_num);
			long result = global::OpenPCR.NativeNumeric.ToInt64(rounded);
			return Approximately(_num, result) ? result : global::OpenPCR.NativeNumeric.ToInt64(_num);
		}

		[global::OpenPCR.Migration("5154170d-ab54-4dfb-8cc5-f123a1994e74")]
		public static int DoubleToIntReverseTruncate(double _num)
		{
			int result = global::OpenPCR.NativeNumeric.ToInt32(_num);
			if (Approximately(result, _num))
				return result;
			return DoubleToInt(_num > 0.0 ? Math.Ceiling(_num) : Math.Floor(_num));
		}

		[global::OpenPCR.Migration("c2debac9-d501-44c0-9f5c-afeb8551149d")]
		public static long DoubleToLongReverseTruncate(double _num)
		{
			long result = global::OpenPCR.NativeNumeric.ToInt64(_num);
			if (Approximately(result, _num))
				return result;
			return DoubleToLong(_num > 0.0 ? Math.Ceiling(_num) : Math.Floor(_num));
		}

		[global::OpenPCR.Migration("41651cd0-3f2b-436f-940f-9f191380e4d4")]
		public static double DoubleToPerMile(double _value)
		{
			return DoubleToLong(_value * 1000.0) / 1000.0;
		}

		[global::OpenPCR.Migration("76c7b6c8-5456-4b6d-ac91-a7afe24ce3dc")]
		public static eUnitRespawnPos SearchRespawnPos(eUnitRespawnPos _basePos, List<UnitCtrl> _unitList)
		{
			if (_unitList.FindIndex((UnitCtrl e) => e.RespawnPos == _basePos) == -1)
			{
				return _basePos;
			}

			int basePos = (int)_basePos;
			int index1 = 0;
			bool flag = _basePos == eUnitRespawnPos.MAIN_POS_1;
			int index2 = 0;
			bool flag2 = _basePos == eUnitRespawnPos.SUB_POS_5;
			for (index1 = 1; index1 <= basePos && _unitList.FindIndex((UnitCtrl e) => e.RespawnPos == (eUnitRespawnPos)(basePos - index1)) != -1; index1++)
			{
				if (index1 == basePos)
				{
					flag = true;
				}
			}

			for (index2 = 1; index2 <= 9 - basePos && _unitList.FindIndex((UnitCtrl e) => e.RespawnPos == (eUnitRespawnPos)(basePos + index2)) != -1; index2++)
			{
				if (index2 == 9 - basePos)
				{
					flag2 = true;
				}
			}

			if (!flag2 && !flag)
			{
				if (index2 < index1)
				{
					return (eUnitRespawnPos)(basePos + index2);
				}

				return (eUnitRespawnPos)(basePos - index1);
			}

			if (flag2 && !flag)
			{
				return (eUnitRespawnPos)(basePos - index1);
			}

			if (flag && !flag2)
			{
				return (eUnitRespawnPos)(basePos + index2);
			}

			return _basePos;
		}

		[global::OpenPCR.Migration("0775b1a2-eec4-482a-96a3-f447d29f7a72")]
		public static void SortPosition(List<UnitCtrl> _unitList, float[] _searchAreaList, int[] _posList, bool _isWithId)
		{
			int last = _unitList.Count - 1;
			for (int first = 0; first < last; first++)
			{
				for (int right = last; right > first; right--)
				{
					float leftArea = _searchAreaList[right - 1];
					float rightArea = _searchAreaList[right];
					bool swap;
					if (_isWithId && leftArea == rightArea)
						swap = leftArea > rightArea || _unitList[_posList[right - 1]].UnitId > _unitList[_posList[right]].UnitId;
					else
						swap = leftArea > rightArea;
					if (swap)
					{
						int pos = _posList[right];
						_posList[right] = _posList[right - 1];
						_posList[right - 1] = pos;
						float area = _searchAreaList[right];
						_searchAreaList[right] = _searchAreaList[right - 1];
						_searchAreaList[right - 1] = area;
					}
				}
			}
		}

		[global::OpenPCR.Migration("29d46b26-e8d7-4015-ad7e-e5cd926a9344")]
		public static double CalcAttenuationForEnergy(double _start, double _additional)
		{
			var attenuations = ManagerSingleton<MasterDataManager>.Instance.masterTpRecoveryAt.Attenuations;
			double total = 0.0;
			for (int index = 1; index < attenuations.Count; index++)
			{
				if ((int)attenuations[index].limit_value > _start)
				{
					if (_start + _additional < (int)attenuations[index].limit_value)
						return total + _additional * (double)attenuations[index].correction_value;
					total += ((int)attenuations[index].limit_value - _start) * (double)attenuations[index].correction_value;
					_additional -= (int)attenuations[index].limit_value - _start;
					_start = (int)attenuations[index].limit_value;
				}
			}

			return total + _additional * (double)attenuations[0].correction_value;
		}

		[global::OpenPCR.Migration("b9684781-7c2e-4f86-8dc1-0c28f4207b4f")]
		public static long CalcAttenuationForLifeSteal(long _start, long _additional)
		{
			var attenuations = ManagerSingleton<MasterDataManager>.Instance.masterHpDrainAt.Attenuations;
			double total = 0.0;
			for (int index = 1; index < attenuations.Count; index++)
			{
				if ((int)attenuations[index].limit_value > _start)
				{
					if (_start + _additional < (int)attenuations[index].limit_value)
						return DoubleToLongReverseTruncate(total + (double)_additional * (double)attenuations[index].correction_value);
					total += ((long)(int)attenuations[index].limit_value - _start) * (double)attenuations[index].correction_value;
					_additional -= (int)attenuations[index].limit_value - _start;
					_start = (int)attenuations[index].limit_value;
				}
			}

			return DoubleToLongReverseTruncate(total + (double)_additional * (double)attenuations[0].correction_value);
		}
	}
}
