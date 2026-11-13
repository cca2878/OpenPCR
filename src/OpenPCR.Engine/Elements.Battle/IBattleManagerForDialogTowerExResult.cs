namespace Elements.Battle
{
	public interface IBattleManagerForDialogTowerExResult
	{
		eBattleCategory BattleCategory { get; }

		void RunOutBattleResult();

		eBattleClearType GetAutoClearType();
	}
}
