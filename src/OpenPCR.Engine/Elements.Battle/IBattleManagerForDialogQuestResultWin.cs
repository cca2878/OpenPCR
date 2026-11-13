using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForDialogQuestResultWin
	{
		eBattleCategory BattleCategory { get; }

		eBattleResult BattleResult { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		Dictionary<int, int> PlayerHpDic { get; }

		Dictionary<int, int> EnemyHpDic { get; }

		bool IsTowerReplayContent { get; }

		eBattleClearType GetAutoClearType();

		bool IsQuestAutoProgressing();

		bool IsLocalReplay();
	}
}
