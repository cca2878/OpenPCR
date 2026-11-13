using global::OpenPCR.Runtime.Api;

namespace Elements.Battle
{
	public interface IBattleManagerForBattleHeaderController
	{
		bool IsClanBattle { get; }

		bool IsTowerContent { get; }

		bool IsTowerReplayContent { get; }

		bool IsDispTotalDamageBattle { get; }

		eBattleCategory BattleCategory { get; }

		bool IsStartSkillExec { get; }

		UnitUiCtrl UnitUiCtrl { get; }

		CoroutineManager CoroutineManager { get; }

		bool IsSpecialBattle { get; }

		bool IsEndlessBattle { get; }

		bool IsDefenseBattle { get; }

		bool IsDefenseBattleInMode { get; }

		bool IsShowBossGauge { get; }

		long CurrentEnemyPoint { get; }

		float StartLimitTimeForDefense { get; }

		bool IsPlayingPauseEffect { get; }

		bool IsPlayingNoCutInMotion { get; set; }

		bool IsNoCutInMotionRightAfter { get; set; }

		bool CanReserveSkill { get; }

		long StartBattleUnixTime { get; }

		eBattleGameState GameState { get; }

		void GamePause(bool _pause, bool _isBlackoutEnd);

		void TutorialOpenFocusDialog();

		void ResumeAbnormalEffect();

		void RetireBattle();

		void PauseAbnormalEffect();

		int GetCurrentWaveOffset();

		eSpecialBattlePurpose GetPurpose();

		long GetPurposeCount();

		void InstantiatePointCounter(Transform _transform, ref PartsHastunePointCounter _counter);

		void InstantiateDefenseTimeGauge(Transform _transform, ref PartsHatsuneDefenseTimeGauge _gauge);

		void InstantiateSpeicalBattleModeWithoutGauge(Transform _parent, ref PartsSpecialBattleMode _specialBattleMode);

		bool GetIsPlayCutin();

		UnitCtrl GetBossUnit();

		bool IsQuestAutoProgressing();

		bool IsLocalReplay();

		int GetEnvironmentId(bool _isEnemy);

		bool GetNeedsCommonRehearsalIcon();
	}
}
