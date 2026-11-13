namespace Elements.Battle
{
	public interface IBattleManagerForDimensionFaultRehearsalBattleProcessor : IBattleManagerForBaseDimensionFaultBattleProcessor, IBattleManagerForBaseBattleProcessor, IBattleManagerForTimeUpDeadEffect
	{
		void UpdateSkillCounter(ContinuousUnit _continuousUnit, UnitCtrl _unitCtrl);
	}
}
