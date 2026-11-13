namespace Elements.Battle
{
	public interface IBattleManagerForUekBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }

		void StartCoroutineBossLose();

		void UekOnDeadLastUnit(UnitCtrl _unit, int _area);
	}
}
