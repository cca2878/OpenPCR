using System;
using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForAcnEndlessBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		int CurrentDefeatCount { get; }

		List<UnitCtrl> FadeOutUnitList { get; }

		void ResetFieldTimeUp();

		IEnumerator CallTimer(float time, Action onComplete);

		void PlayFlatoutAfterMove(bool _isPlayUnitJoyMotion);

		void GamePause(bool _pause, bool _isBlackoutEnd = false);

		void OpenResultWinDialog();

		IEnumerator InitializeAndUpdateWalkPlayer(Action _onWalkEnd, bool _isOutOfSystemUpdate);

		void ShowResultDialogCoroutine(bool _isWin);
	}
}
