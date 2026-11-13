namespace Elements.Battle
{
	public interface IBattleManagerForReplayClanBattleProcessor : IBattleManagerForGeneralReplayBattleProcessor, IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }

		void StartCoroutineBossLose();
	}
}
