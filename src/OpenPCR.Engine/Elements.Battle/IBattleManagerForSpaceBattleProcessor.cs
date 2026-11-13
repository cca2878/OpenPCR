using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForSpaceBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		float DeltaTime { get; }

		void StartCoroutineBossLose();
	}
}
