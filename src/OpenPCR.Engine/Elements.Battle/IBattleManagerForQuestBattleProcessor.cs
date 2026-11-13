namespace Elements.Battle
{
	public interface IBattleManagerForQuestBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		int CurrentWave { get; }

		int FrameRate { get; }

		bool IsBossBattle { get; }

		eBattleClearType GetAutoClearType();

		void SetBattleResult(eBattleResult result);
	}
}
