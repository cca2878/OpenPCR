using Elements.Data;

namespace Elements
{
	public class QuestUtility
	{
		[global::OpenPCR.Migration("0b06d3ec-cdb0-4c1c-b94a-84686a8db0d0")]
		public static MasterTalentFormationBonus.TalentFormationBonus SearchTalentFormationBonus(eBattleCategory _category, int _targetValue1, int _targetValue2, int _talentId)
		{
			MasterDataManager manager = ManagerSingleton<MasterDataManager>.Instance;
			var overwrite = manager.masterTalentFormationOverwrite.SearchTalentFormationOverwrite(_category, _targetValue1, _targetValue2);
			if (overwrite != null)
				return overwrite.GetTalentFormationBonus(_talentId);
			return manager.masterTalentFormationBonus.GetDefaultBonusData();
		}
	}
}
