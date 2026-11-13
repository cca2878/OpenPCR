using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForPartsQuestResultFormer
	{
		eBattleCategory BattleCategory { get; }

		eBattleResult BattleResult { get; }

		UnitUiCtrl UnitUiCtrl { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		bool IsTowerContent { get; }

		bool IsTowerReplayContent { get; }

		int CurrentDefeatCount { get; }

		void RunOutBattleResult();

		eBattleClearType GetAutoClearType();

		bool IsEnableQuestAutoProgress();

		bool IsQuestAutoProgressing();

		bool IsLocalReplay();
	}
}
