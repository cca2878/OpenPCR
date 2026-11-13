using System;
using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForHatsuneBossBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }

		long CurrentEnemyPoint { get; }

		List<int> KillOrder { get; }

		UnitUiCtrl UnitUiCtrl { get; }

		void StartCoroutineBossLose();

		IEnumerator CallTimer(float time, Action onComplete);

		void AddTempDataBattleLog();

		void DeterminePositionOrder();

		eBattleClearType GetAutoClearType();

		void GamePause(bool _pause, bool _isBlackoutEnd);
	}
}
