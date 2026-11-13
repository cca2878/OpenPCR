using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForStoryBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		void SetStoryBattleDataPrefab(StoryBattleData _storyBattleDataPrefab);

		IEnumerator StoryBattleAnimationUpdate();

		List<int> GetStoryIdListToStoryBattleData();
	}
}
