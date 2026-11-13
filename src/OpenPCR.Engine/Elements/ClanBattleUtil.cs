using System.Collections.Generic;
using System.Linq;
using Elements.Data;

namespace Elements
{
	public static class ClanBattleUtil
	{
		[global::OpenPCR.Migration("764080e6-9111-4f6d-8d06-3a427335513d")]
		public static UnitParameter GetSupportUnitParam()
		{
			ClanBattleTempData temp = Singleton<ClanBattleTempData>.Instance;
			if (temp.SupportUnitId == 0)
				return null;
			List<ClanBattleSupportUnit> units = temp.SupportUnitList;
			for (int i = 0; i < units.Count; ++i)
			{
				ClanBattleSupportUnit unit = units[i];
				if ((long)unit.OwnerViewerId == temp.SupportOwnerId && (int)unit.UnitData.Id == temp.SupportUnitId)
				{
					UnitParameter parameter = new UnitParameter(unit.UnitData);
					parameter.UpdateLimitedUnitData(true, true);
					return parameter;
				}
			}
			return null;
		}

		[global::OpenPCR.Migration("f06b0a3e-afc0-4e20-a0e9-9e5158609400")]
		public static IEnumerable<InterfaceClanBattleMapData> GetMapDataList(int _clanBattleId, int _clanBattleMode)
		{
			MasterDataManager master = ManagerSingleton<MasterDataManager>.Instance;
			if (_clanBattleMode == 1)
				return master.masterClanBattleSMapData.GetListWithClanBattleId(_clanBattleId).Cast<InterfaceClanBattleMapData>();
			return master.masterClanBattle2MapData.GetListWithClanBattleId(_clanBattleId).Cast<InterfaceClanBattleMapData>();
		}

		public static bool IsMatchMapData(InterfaceClanBattleMapData _mapData, int _lapNum, int _phase)
		{
			if (_mapData.Phase != _phase)
				return false;
			return (_mapData.LapNumFrom <= _lapNum && _mapData.LapNumTo >= _lapNum)
				|| (_mapData.LapNumFrom <= _lapNum && _mapData.LapNumTo == -1);
		}

		[global::OpenPCR.Migration("b2b13ddf-01e8-4ae1-98b5-e64b652f7c51")]
		public static ClanBattleDefine.eClanAuraEffectType GetCurrentEventBossDifficulity(int _clanBattleId, int _lapNum, int _phase, bool _isSingleModeBoss)
		{
			return (ClanBattleDefine.eClanAuraEffectType)GetMapDataList(_clanBattleId, _isSingleModeBoss ? 1 : 0)
				.FirstOrDefault(map => IsMatchMapData(map, _lapNum, _phase)).AuraEffect;
		}

		[global::OpenPCR.Migration("bad6afc5-cde0-4b8f-a5da-ae9368f5c229")]
		public static PrefabWithTime.eEffectDifficulty GetCurrentBossEffectDifficulty(int _battleId, int _lapNum, int _phase, bool _isSingleModeBoss)
		{
			ClanBattleDefine.eClanAuraEffectType aura = GetCurrentEventBossDifficulity(_battleId, _lapNum, _phase, _isSingleModeBoss);
			return aura == ClanBattleDefine.eClanAuraEffectType.HARD
				? PrefabWithTime.eEffectDifficulty.HARD : PrefabWithTime.eEffectDifficulty.NORMAL;
		}
		[global::OpenPCR.Migration("c96bb2dc-ae87-4ca1-919e-33f46807eb45")]
		public static void OnErrorClanBattleApiRequest(int _resultCode)
		{
			// The unpatched Native method returns directly; IFix is outside this migration.
		}
	}
}
