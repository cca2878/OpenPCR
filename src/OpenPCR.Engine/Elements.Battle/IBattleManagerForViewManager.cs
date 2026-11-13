namespace Elements.Battle
{
	public interface IBattleManagerForViewManager
	{
		eBattleCategory BattleCategory { get; }

		void GamePause(bool pause, bool _isBlackoutEnd);
	}
}
