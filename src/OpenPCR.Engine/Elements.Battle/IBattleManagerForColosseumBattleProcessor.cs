using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForColosseumBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		void SetBattleResult(eBattleResult result);
	}
}
