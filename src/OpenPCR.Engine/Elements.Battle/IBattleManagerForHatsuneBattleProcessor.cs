namespace Elements.Battle
{
	public interface IBattleManagerForHatsuneBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		int CurrentWave { get; }

		void SetBattleResult(eBattleResult result);
	}
}
