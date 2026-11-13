using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForPartsQuestResultLater
	{
		eBattleCategory BattleCategory { get; }

		bool IsTowerContent { get; }

		bool IsTowerReplayContent { get; }

		bool IsHighRarityQuest { get; }

		List<RewardData> RewardList { get; }

		bool IsHatsuneBoss { get; }

		bool IsHatsuneBattle { get; }

		bool IsSpecialBattle { get; }

		bool IsQuestAutoProgressing();
	}
}
