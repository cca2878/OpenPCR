using System;
using System.Collections;

namespace Elements.Battle
{
	public interface IBattleManagerForKaiserSubBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }

		void StartCoroutineBossLose();

		IEnumerator CallTimer(float time, Action onComplete);

		void AddTempDataBattleLog();

		void DeterminePositionOrder();
	}
}
