using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForDialogNormalArenaResult
	{
		eBattleResult BattleResult { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		Dictionary<int, int> PlayerHpDic { get; }

		Dictionary<int, int> EnemyHpDic { get; }
	}
}
