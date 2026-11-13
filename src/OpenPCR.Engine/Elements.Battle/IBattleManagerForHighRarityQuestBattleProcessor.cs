namespace Elements.Battle
{
	public interface IBattleManagerForHighRarityQuestBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }

		void HighRarityOnDeadLastUnit(UnitCtrl _unit, eBattleTreasureBoxType _treasureType);

		eBattleClearType GetAutoClearType();
	}
}
