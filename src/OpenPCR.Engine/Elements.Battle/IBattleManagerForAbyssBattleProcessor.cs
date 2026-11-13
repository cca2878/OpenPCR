namespace Elements.Battle
{
	public interface IBattleManagerForAbyssBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		void DisplayAliveUnitFront();

		eBattleClearType GetAutoClearType();
	}
}
