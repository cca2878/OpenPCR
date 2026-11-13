using System;
using System.Collections;

namespace Elements.Battle
{
	public interface IBattleManagerForOnlyBaseBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		float DeltaTime { get; }

		int FrameRate { get; }

		bool IsSortBackBossBattle { get; set; }

		bool CanQuadruple { get; }

		UnitUiCtrl UnitUiCtrl { get; }

		int CurrentWave { get; }

		bool IsActiveUnitTalentFormationBonus { get; }

		IEnumerator CallTimer(float time, Action onComplete);

		void PlayPartyUnitJoyMotion();

		void PlayFlatoutAfterMove(bool _isPlayUnitJoyMotion);

		IBattleCameraEffectForBattleProcessor GetBattleCameraEffectForBattleProcessor();

		IBattleTimeScaleForBattleProcessor GetIBattleTimeScaleForBattleProcessor();

		void GamePause(bool pause, bool _isBlackoutEnd);

		IEnumerator UpdateZoom(float _scale, float _posY);

		void OpenResultWinDialog();

		void ShowResultDialog();

		void PlayStory(Action _callback, int _storyId, bool _end);

		void NextWaveProcess();

		IEnumerator InitializeAndUpdateWalkPlayer(Action _onWalkEnd, bool _isOutOfSystemUpdate);
	}
}
