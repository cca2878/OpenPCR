namespace Elements.Battle
{
	public interface IBattleManagerForBattleUnionBurstController
	{
		eChargeSkillTurn ChargeSkillTurn { get; set; }

		UnitCtrl UnionBurstUseUnit { get; set; }

		UnitUiCtrl UnitUiCtrl { get; }

		bool IsDefenseReplayMode { get; }

		eBattleGameState GameState { get; }

		int GetEnemyCtrlLength();

		UnitCtrl GetEnemyCtrl(int _idx);

		bool GetIsPlayCutin();

		int GetUnitCtrlLength();

		UnitCtrl GetUnitCtrl(int _idx);
	}
}
