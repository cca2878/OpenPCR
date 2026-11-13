namespace Elements.Battle
{
	public interface IBattleManagerForAutoLogic
	{
		float TimeLimit { get; }

		int GetUnitCtrlLength();

		UnitCtrl GetUnitCtrl(int _idx);

		int GetEnemyCtrlLength();

		UnitCtrl GetEnemyCtrl(int _idx);
	}
}
