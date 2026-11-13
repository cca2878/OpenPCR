using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForBaseDimensionFaultBattleProcessor : IBattleManagerForBaseBattleProcessor, IBattleManagerForTimeUpDeadEffect
	{
		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		void DisplayAliveUnitFront();

		eBattleClearType GetAutoClearType();

		void StartCoroutineBossLose();
	}
}
