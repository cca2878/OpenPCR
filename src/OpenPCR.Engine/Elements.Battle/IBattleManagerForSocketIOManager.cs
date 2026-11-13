namespace Elements.Battle
{
	public interface IBattleManagerForSocketIOManager
	{
		eBattleGameState GameState { get; }

		void GamePause(bool pause, bool _isBlackoutEnd);
	}
}
