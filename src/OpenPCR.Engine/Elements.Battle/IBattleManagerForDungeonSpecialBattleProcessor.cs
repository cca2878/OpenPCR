namespace Elements.Battle
{
	public interface IBattleManagerForDungeonSpecialBattleProcessor : IBattleManagerForDungeonBattleProcessor, IBattleManagerForBaseBattleProcessor, IBattleManagerForTimeUpDeadEffect
	{
		void AddTempDataBattleLog();

		void GamePause(bool _pause, bool _isBlackoutEnd = false);
	}
}
