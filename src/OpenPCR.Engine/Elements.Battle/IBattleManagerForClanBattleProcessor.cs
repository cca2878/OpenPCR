namespace Elements.Battle
{
	public interface IBattleManagerForClanBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		long TotalBossDamage { get; }

		long SekaiAccumulateDamage { get; }

		float TimeLimit { get; }

		void StartCoroutineBossLose();

		eBattleClearType GetAutoClearType();

		int GetStartRemainTime();

		int GetBattleElapsedTime();

		int GetRoundUpTimeLimit();
	}
}
