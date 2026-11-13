namespace Elements.Battle
{
	public interface IBattleManagerForMirageBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		eBattleClearType GetAutoClearType();

		void DisplayAliveUnitFront();
	}
}
