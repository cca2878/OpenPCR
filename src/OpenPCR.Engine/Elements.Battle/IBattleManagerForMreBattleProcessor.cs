using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForMreBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }

		List<int> KillOrder { get; }

		eBattleClearType GetAutoClearType();

		void StartCoroutineBossLose();
	}
}
