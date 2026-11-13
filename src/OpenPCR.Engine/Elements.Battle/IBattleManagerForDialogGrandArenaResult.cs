using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForDialogGrandArenaResult
	{
		eBattleResult BattleResult { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }
	}
}
