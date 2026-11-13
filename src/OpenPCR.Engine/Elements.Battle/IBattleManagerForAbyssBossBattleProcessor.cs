namespace Elements.Battle
{
	public interface IBattleManagerForAbyssBossBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		void DisplayAliveUnitFront();

		eBattleClearType GetAutoClearType();

		void StartCoroutineBossLose();
	}
}
