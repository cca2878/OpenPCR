using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForDialogQuestFailed
	{
		int PlayVoiceUnitId { get; }

		int PlayVoiceIndex { get; }

		int PlayVoiceGroupId { get; }

		int PlayVoiceGroupUnitId { get; set; }

		eBattleCategory BattleCategory { get; }

		eBattleResult BattleResult { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		int CurrentWave { get; set; }

		bool IsTowerContent { get; }

		bool IsTowerReplayContent { get; }

		eBattleClearType GetAutoClearType();

		bool IsQuestAutoProgressing();

		bool IsLocalReplay();
	}
}
