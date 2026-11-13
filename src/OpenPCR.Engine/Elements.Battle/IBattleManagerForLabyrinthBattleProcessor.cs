using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForLabyrinthBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }

		List<UnitCtrl> EnemyList { get; }

		void DisplayAliveUnitFront();

		eBattleClearType GetAutoClearType();
	}
}
