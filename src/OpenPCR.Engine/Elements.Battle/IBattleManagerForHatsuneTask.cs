using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForHatsuneTask
	{
		eBattleResult BattleResult { get; }

		List<UnitCtrl> EnemyList { get; }

		UnitCtrl FindEnemy(int _id);

		UnitCtrl GetBossUnit();
	}
}
