namespace Elements
{
	public class ClanBattleTopInfo
	{
		public int ClanBattleMode { get; set; }
		public ClanBattleTopInfo()
		{
			ResetData();
		}
		[global::OpenPCR.Migration("09af7013-af45-4158-9311-7a4d6ad572d2")]
		public bool IsSingleBattleMode()
		{
			return ClanBattleMode == 1;
		}
		public void ResetData()
		{
			// Lobby response production creates reward, ranking, carry-over and clan API records; the battle mode reader preserves the actual stored ClanBattleMode.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.ClanBattleTopInfo::ResetData");
		}
	}
}
