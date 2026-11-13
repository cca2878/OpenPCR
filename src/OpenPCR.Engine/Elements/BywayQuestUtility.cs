namespace Elements
{
	public static class BywayQuestUtility
	{
		[global::OpenPCR.Migration("dff30d10-1665-4563-b78c-dc54e603ce48")]
		public static bool IsFixAutoBattle(int _questId)
		{
			var record = ManagerSingleton<global::Elements.Data.MasterDataManager>.Instance.masterBywayBattleQuestData.Get(_questId);
			return record != null && (int)record.auto_restrict_type == 2;
		}
	}
}
