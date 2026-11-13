namespace Elements
{
	public static class QuestAutoProgressUtility
	{
		[global::OpenPCR.Migration("e711c1c3-55e5-4936-b9c6-b0a636942f35")]
		public static bool IsSupportEnableBattelCategory(eBattleCategory _battleCategory)
		{
			return _battleCategory == eBattleCategory.QUEST || _battleCategory == eBattleCategory.BYWAY_QUEST;
		}

		[global::OpenPCR.Migration("abeb6904-b500-4b2e-a49f-20577ecbde7e")]
		public static int GetUbSettingIndex(eBattleCategory _battleCategory)
		{
			SaveDataManager manager = ManagerSingleton<SaveDataManager>.Instance;
			switch (_battleCategory)
			{
				case eBattleCategory.QUEST:
				case eBattleCategory.BYWAY_QUEST:
					return manager.MainQuestAutoProgressUbSettingIndex;
				case eBattleCategory.HATSUNE_BATTLE:
				case eBattleCategory.SEVEN_QUEST:
					return manager.EventQuestAutoProgressUbSettingIndex;
				default:
					return 0;
			}
		}

		[global::OpenPCR.Migration("25fb74c7-d4db-466f-94c9-e2414e1336bd")]
		public static bool IsEnableBattleCategory(global::Elements.eBattleCategory _battleCategory)
		{
			return _battleCategory == eBattleCategory.QUEST || _battleCategory == eBattleCategory.HATSUNE_BATTLE
				|| _battleCategory == eBattleCategory.SEVEN_QUEST || _battleCategory == eBattleCategory.BYWAY_QUEST;
		}
	}
}
