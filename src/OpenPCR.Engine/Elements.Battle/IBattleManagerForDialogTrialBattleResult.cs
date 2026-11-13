using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForDialogTrialBattleResult
	{
		eBattleCategory BattleCategory { get; }

		eBattleResult BattleResult { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		eBattleClearType GetAutoClearType();

		UnitCtrl GetBossUnit();

		bool IsBossInfinityHpMode();
	}
}
