using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForTalentQuestBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		void DisplayAliveUnitFront();

		void SetBattleResult(eBattleResult result);

		eBattleClearType GetAutoClearType();
	}
}
