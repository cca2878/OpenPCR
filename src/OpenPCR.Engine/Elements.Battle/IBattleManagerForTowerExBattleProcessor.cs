using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForTowerExBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		int FrameRate { get; }

		bool CanQuadruple { get; }

		UnitUiCtrl UnitUiCtrl { get; }

		List<TowerQueryUnit> CreateTowerQueryUnitList(bool _isEx, bool _isPlayer);

		void DisplayAliveUnitFront();

		void OpenTowerExResultDialog(bool _isVictory, int _finalWaveIndex, bool _isFirst);

		void TowerOnDeadLastUnit(UnitCtrl _unit);

		void ShowResultDialog();

		eBattleClearType GetAutoClearType();

		void PlayFlatoutAfterMove(bool _isPlayUnitJoyMotion);
	}
}
