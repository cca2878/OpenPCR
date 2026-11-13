using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForAcnSpecialBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		long CurrentEnemyPoint { get; }

		List<int> KillOrder { get; }

		void AddTempDataBattleLog();

		void GamePause(bool _pause, bool _isBlackoutEnd = false);

		void StartCoroutineBossLose();
	}
}
