using System;
using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForTowerBattleProcessor : IBattleManagerForBaseBattleProcessor, IBattleManagerForTimeUpDeadEffect
	{
		float DeltaTime { get; }

		UnitUiCtrl UnitUiCtrl { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		int FrameRate { get; }

		int CurrentWave { get; }

		eBattleCategory BattleCategory { get; }

		void DisplayAliveUnitFront();

		List<TowerQueryUnit> CreateTowerQueryUnitList(bool _isEx, bool _isPlayer);

		void TowerOnDeadLastUnit(UnitCtrl _unit);

		void UpdateSkillCounter(ContinuousUnit _continuousUnit, UnitCtrl _unitCtrl);

		eBattleClearType GetAutoClearType();

		void PlayFlatoutAfterMove(bool _isPlayUnitJoyMotion);

		void RetireBattle();

		IEnumerator SetupPlayerResultPosition(Action _finishCallBack);
	}
}
