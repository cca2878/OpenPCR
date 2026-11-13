using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForArenaBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		bool CanQuadruple { get; }

		float DeltaTime { get; }

		List<UnitCtrl> EnemyList { get; }

		List<UnitCtrl> UnitList { get; }

		bool IsSuspendLose { get; }

		void OpenResultWinDialog();

		void ResetFieldTimeUp();

		void RemoveAllCoroutines();
	}
}
