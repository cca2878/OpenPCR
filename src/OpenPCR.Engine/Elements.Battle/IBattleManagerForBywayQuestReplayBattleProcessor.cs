namespace Elements.Battle
{
	public interface IBattleManagerForBywayQuestReplayBattleProcessor : IBattleManagerForGeneralReplayBattleProcessor, IBattleManagerForBaseBattleProcessor
	{
		bool IsBossBattle { get; }

		int GetLatestClearStarNum();

		void GamePause(bool pause, bool _isBlackoutEnd);
	}
}
