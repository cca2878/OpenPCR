namespace Elements.Battle
{
	public interface IBattleManagerForDialogLabyrinthExResult
	{
		eBattleResult BattleResult { get; }

		void RunOutBattleResult();
	}
}
