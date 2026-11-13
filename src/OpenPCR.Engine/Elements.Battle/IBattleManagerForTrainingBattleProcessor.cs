namespace Elements.Battle
{
	public interface IBattleManagerForTrainingBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		int FrameRate { get; }

		int CurrentWave { get; }

		void SetBattleResult(eBattleResult result);
	}
}
