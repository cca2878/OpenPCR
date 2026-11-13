using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Data
{
	public class TeamEnhanceParameter
	{
		public global::Elements.PrincessKnightInfo PrincessKnightInfo { get; }
		private global::System.Collections.Generic.List<Elements.UnitRoleInfo> unitRoleInfoList { get; }
		private global::System.Collections.Generic.Dictionary<System.Int32, System.Int32> talentLevelParameter { get; set; }
		private global::System.Collections.Generic.Dictionary<System.Int32, System.Collections.Generic.Dictionary<Elements.eParamType, System.Int32>> knightEnhanceBuffParameter { get; set; }
		private global::System.Collections.Generic.Dictionary<System.Int32, System.Collections.Generic.Dictionary<Elements.UnitRoleDefine.eRoleParamType, System.Int32>> roleBuffParameter { get; set; }

		[global::OpenPCR.Migration("89f0c3c8-f50a-4b20-bdd2-856a952e8618")]
		public int GetAdditiveAtkForTalentLevel(int _talentId)
		{
			int value = 0;
			talentLevelParameter.TryGetValue(_talentId, out value);
			return value;
		}

		[global::OpenPCR.Migration("10378fb3-9c7b-493e-8247-3bd89f3dd17d")]
		public global::System.Collections.Generic.Dictionary<Elements.eParamType, System.Int32> GetKnightEnhance(int _talentId)
		{
			knightEnhanceBuffParameter.TryGetValue(_talentId, out var value);
			return value ?? new Dictionary<eParamType, int>();
		}

		[global::OpenPCR.Migration("065f9d25-b04d-4fea-b16c-1ad59b1571db")]
		public global::System.Collections.Generic.Dictionary<Elements.eParamType, System.Int32> GetRoleEnhance(int _roleId, global::Elements.UnitCtrl _owner)
		{
			var result = new Dictionary<eParamType, int>();
			bool isAtk = _owner.AtkType != 2;
			if (roleBuffParameter.TryGetValue(_roleId, out var values))
				foreach (var entry in values)
					result[getParamType(entry.Key, isAtk)] = entry.Value;
			return result;
		}

		[global::OpenPCR.Migration("d6d53379-adcf-42b4-a991-e3b2fc28b2cf")]
		private global::Elements.eParamType getParamType(global::Elements.UnitRoleDefine.eRoleParamType _roleParamType, bool _isAtk)
		{
			switch (_roleParamType)
			{
				case UnitRoleDefine.eRoleParamType.HP:
					return eParamType.HP;
				case UnitRoleDefine.eRoleParamType.ATTACK:
					return _isAtk ? eParamType.ATK : eParamType.MAGIC_ATK;
				case UnitRoleDefine.eRoleParamType.DEF:
					return eParamType.DEF;
				case UnitRoleDefine.eRoleParamType.MAGIC_DEF:
					return eParamType.MAGIC_DEF;
				case UnitRoleDefine.eRoleParamType.CRITICAL:
					return _isAtk ? eParamType.PHYSICAL_CRITICAL : eParamType.MAGIC_CRITICAL;
				case UnitRoleDefine.eRoleParamType.CRITICAL_DAMAGE_RATE:
					return _isAtk ? eParamType.PHYSICAL_CRITICAL_DAMAGE_RATE : eParamType.MAGIC_CRITICAL_DAMAGE_RATE;
				case UnitRoleDefine.eRoleParamType.DAMAGE_UP_PERCENT:
					return _isAtk ? eParamType.PHYSICAL_DAMAGE_UP_PERCENT : eParamType.MAGIC_DAMAGE_UP_PERCENT;
				case UnitRoleDefine.eRoleParamType.ABNORMAL_STATE_DODGE_HIT:
					return eParamType.ABNORMAL_STATE_DODGE_HIT;
				case UnitRoleDefine.eRoleParamType.ABNORMAL_STATE_DODGE_RESIST:
					return eParamType.ABNORMAL_STATE_DODGE_RESIST;
				case UnitRoleDefine.eRoleParamType.ATTACK_ENHANCE:
					return eParamType.ATTACK_ENHANCE;
				case UnitRoleDefine.eRoleParamType.BUFF_ENHANCE:
					return eParamType.BUFF_ENHANCE;
				case UnitRoleDefine.eRoleParamType.DEBUFF_ENHANCE:
					return eParamType.DEBUFF_ENHANCE;
				case UnitRoleDefine.eRoleParamType.HEAL_ENHANCE:
					return eParamType.HEAL_ENHANCE;
				case UnitRoleDefine.eRoleParamType.ENERGY_CHARGE:
					return eParamType.ENERGY_CHARGE;
				default:
					return eParamType.NONE;
			}
		}

		public TeamEnhanceParameter(PrincessKnightInfo _princessKnightInfo, List<UnitRoleInfo> _unitRoleList, bool _isSkipCalculation = false)
		{
			PrincessKnightInfo = _princessKnightInfo;
			unitRoleInfoList = _unitRoleList;
			if (!_isSkipCalculation)
				Calculate();
		}

		public void Calculate()
		{
			talentLevelParameter = calcTalentLevelParamDict(PrincessKnightInfo);
			knightEnhanceBuffParameter = calcTalentBuffParam(PrincessKnightInfo);
			roleBuffParameter = calcRoleBuffParam(unitRoleInfoList);
		}

		private Dictionary<int, int> calcTalentLevelParamDict(PrincessKnightInfo _princessKnightInfo)
		{
			if (_princessKnightInfo == null)
				return new Dictionary<int, int>();
			throw new OpenPCR.OutOfBoundaryException("TeamEnhanceParameter.calcTalentLevelParamDict Master talent growth production");
		}

		private Dictionary<int, Dictionary<eParamType, int>> calcTalentBuffParam(PrincessKnightInfo _princessKnightInfo)
		{
			var result = new Dictionary<int, Dictionary<eParamType, int>>();
			if (_princessKnightInfo == null)
				return result;
			throw new OpenPCR.OutOfBoundaryException("TeamEnhanceParameter.calcTalentBuffParam Master talent and KnightEnhanceUtility growth production");
		}

		private Dictionary<int, Dictionary<UnitRoleDefine.eRoleParamType, int>> calcRoleBuffParam(List<UnitRoleInfo> _unitRoleList)
		{
			var result = new Dictionary<int, Dictionary<UnitRoleDefine.eRoleParamType, int>>();
			if (_unitRoleList == null)
				return result;
			throw new OpenPCR.OutOfBoundaryException("TeamEnhanceParameter.calcRoleBuffParam Master role and UnitRoleInfoExtensionMethod growth production");
		}
	}
}
