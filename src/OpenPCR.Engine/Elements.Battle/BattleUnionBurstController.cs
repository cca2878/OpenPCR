namespace Elements.Battle
{
	public class BattleUnionBurstController
	{
		private IBattleManagerForBattleUnionBurstController battleManager;
		[global::OpenPCR.Migration("295d805a-f6de-46d7-b6db-b6e47fd972dd")]
		public BattleUnionBurstController(IBattleManagerForBattleUnionBurstController _battleManager)
		{
			battleManager = _battleManager;
		}

		[global::OpenPCR.Migration("99e6d540-fe22-4caa-a347-788ed41d98c9")]
		public void TryExecUnionBurst()
		{
			if (battleManager.GetIsPlayCutin() || battleManager.GameState != eBattleGameState.PLAY)
				return;
			int index = 0;
			int count;
			while (index < (count = battleManager.GetUnitCtrlLength()))
			{
				UnitCtrl unit = battleManager.GetUnitCtrl(index);
				battleManager.UnitUiCtrl.ExecReserveSKill(unit);
				if (unit.UnionBurstSkillId != 0)
					unit.ComponentSearchArea.UpdateSkillTarget();
				var unionBurst = unit.ComponentUnionBurst;
				if (unionBurst.IsAutoOrUbExecTrying() && unionBurst.JudgeSkillReadyAndIsMyTurn())
				{
					unit.SetDirectionAuto();
					unit.ComponentSearchArea.UpdateSkillTarget();
					if (unionBurst.IsAutoOrUbExecTrying() && unionBurst.JudgeSkillReadyAndIsMyTurn())
					{
						battleManager.ChargeSkillTurn = eChargeSkillTurn.PLAYER;
						battleManager.UnionBurstUseUnit = unit;
						battleManager.UnitUiCtrl.PlaySkillReserveExecuteEffect(unit);
						battleManager.UnitUiCtrl.UpdateUi();
						battleManager.UnitUiCtrl.ResetSkillReserve(unit, false);
						if (!battleManager.IsDefenseReplayMode)
							battleManager.UnitUiCtrl.PlaySkillOn(unit);
						unit.ComponentUnionBurst.StartCutIn();
						break;
					}
				}
				index++;
			}
			for (int i = 0; i < battleManager.GetUnitCtrlLength(); i++)
				battleManager.GetUnitCtrl(i).ComponentUnionBurst.SetIsUbExecTrying(false);
			if (index >= count)
				TryExecEnemyUnionBurst();
		}

		[global::OpenPCR.Migration("a0115210-3991-4519-8e76-d67d00a7c77e")]
		public void TryExecEnemyUnionBurst()
		{
			for (int i = 0; i < battleManager.GetEnemyCtrlLength(); i++)
			{
				UnitCtrl unit = battleManager.GetEnemyCtrl(i);
				if (unit.UnionBurstSkillId != 0)
					unit.ComponentSearchArea.UpdateSkillTarget();
				if (unit.IsSummonOrPhantom && (unit.SummonSource == null || unit.SummonSource.ComponentParameter.Hp < 1L))
					continue;
				var unionBurst = unit.ComponentUnionBurst;
				var state = unit.ComponentState;
				if (!unionBurst.JudgeSkillReadyAndIsMyTurn() || state.CurrentState != Core.ActionState.IDLE)
					continue;
				unit.SetDirectionAuto();
				unit.ComponentSearchArea.UpdateSkillTarget();
				if (!unionBurst.JudgeSkillReadyAndIsMyTurn() || state.CurrentState != Core.ActionState.IDLE)
					continue;
				battleManager.ChargeSkillTurn = eChargeSkillTurn.ENEMY;
				battleManager.UnionBurstUseUnit = unit;
				battleManager.UnitUiCtrl.UpdateUi();
				if (battleManager.IsDefenseReplayMode)
					battleManager.UnitUiCtrl.PlaySkillOn(unit);
				unionBurst.StartCutIn();
				break;
			}
		}

		[global::OpenPCR.Migration("55639c30-53eb-4b0e-89c1-dfea3ed1da64")]
		public void Release()
		{
			battleManager = null;
		}
	}
}
