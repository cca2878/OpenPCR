using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForAcnBossBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		List<UnitCtrl> EnemyList { get; }

		void StartCoroutineBossLose();
	}
}
