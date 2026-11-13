using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForLabyrinthExBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		void ShowResultDialog();

		void DisplayAliveUnitFront();

		void OpenLabyrinthExResultDialog(bool _isVictory, int _finalWaveIndex);

		void RunOutBattleResult();

		eBattleClearType GetAutoClearType();
	}
}
