namespace Elements
{
	public interface InterfaceClanBattleBossData
	{
		int BossId { get; }
		int ClanBattleId { get; }
		int Difficulty { get; }
		int OrderNum { get; }
		int BossThumbId { get; }
		int PositionX { get; }
		int PositionY { get; }
		float ScaleRatio { get; }
		int MapPositionX { get; }
		int MapPositionY { get; }
		float TapWidthRatio { get; }
		float TapHeightRatio { get; }
		int CursorPosition { get; }
		int ResultBossPositionY { get; }
		int QuestDetailBgId { get; }
		int QuestDetailBgPosition { get; }
		float QuestDetailMonsterSize { get; }
		int QuestDetailMonsterHeight { get; }
		float BattleReportMonsterSize { get; }
		int BattleReportMonsterHeight { get; }
		int Background { get; }
		string WaveBgm { get; }
	}
}
