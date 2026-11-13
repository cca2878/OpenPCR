using System.Collections;
using System;

namespace Elements.Battle
{
	public interface IBattleManagerForQuestReplayBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		int CurrentWave { get; }

		bool IsBossBattle { get; }

		void DisplayAliveUnitFront();

		int GetLatestClearStarNum();

		void GamePause(bool pause, bool _isBlackoutEnd);

		void StartCameraZoomReset(Action _callback);
	}
}
