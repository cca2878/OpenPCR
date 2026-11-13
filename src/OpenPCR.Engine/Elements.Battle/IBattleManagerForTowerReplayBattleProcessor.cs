namespace Elements.Battle
{
	public interface IBattleManagerForTowerReplayBattleProcessor : IBattleManagerForGeneralReplayBattleProcessor, IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }
	}
}
