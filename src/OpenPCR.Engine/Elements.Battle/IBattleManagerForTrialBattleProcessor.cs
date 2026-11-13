namespace Elements.Battle
{
	public interface IBattleManagerForTrialBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		long TotalBossDamage { get; }

		void StartCoroutineBossLose();
	}
}
