namespace Elements.Battle
{
	public interface IBattleManagerForSevenQuestBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		int CurrentWave { get; }

		void SetBattleResult(eBattleResult result);

		eBattleClearType GetAutoClearType();
	}
}
