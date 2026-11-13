namespace Elements.Battle
{
	public interface IBattleManagerForDimensionFaultBattleProcessor : IBattleManagerForBaseDimensionFaultBattleProcessor, IBattleManagerForBaseBattleProcessor, IBattleManagerForTimeUpDeadEffect
	{
		void UpdateSkillCounter(ContinuousUnit _continuousUnit, UnitCtrl _unitCtrl);
	}
}
