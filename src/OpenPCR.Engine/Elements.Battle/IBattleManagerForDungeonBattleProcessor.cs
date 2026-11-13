namespace Elements.Battle
{
	public interface IBattleManagerForDungeonBattleProcessor : IBattleManagerForBaseBattleProcessor, IBattleManagerForTimeUpDeadEffect
	{
		void DisplayAliveUnitFront();

		void UpdateSkillCounter(ContinuousUnit _continuousUnit, UnitCtrl _unitCtrl);

		eBattleClearType GetAutoClearType();
	}
}
