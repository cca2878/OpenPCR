namespace Elements.Battle
{
	public interface IBattleManagerForBywayQuestBattleProcessor : IBattleManagerForQuestBattleProcessor, IBattleManagerForBaseBattleProcessor
	{
		bool IsQuestAutoProgressing();
	}
}
