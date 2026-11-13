using System;
using System.Collections;
using System.Collections.Generic;
using Elements.Uek;

namespace Elements.Battle
{
	public interface IBattleManagerForGeneralReplayBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		UnitUiCtrl UnitUiCtrl { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		int CurrentWave { get; set; }

		void DisplayAliveUnitFront();

		void UpdateSkillCounter(ContinuousUnit _continuousUnit, UnitCtrl _unitCtrl);

		BaseBattleProcessor CreateBattleProcessorImpl(UekTempData _uekTempData);

		void PlayFlatoutAfterMove(bool _isPlayUnitJoyMotion);

		IEnumerator CallTimer(float time, Action onComplete);

		IEnumerator SetupPlayerResultPosition(Action _finishCallBack);
	}
}
