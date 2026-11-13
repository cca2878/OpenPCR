using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using Elements.Battle.Core;

namespace Elements.Battle
{
	public interface IBattleManagerForUnitUiCtrl
	{
		bool IsDefenseReplayMode { get; }

		float DeltaTime { get; }

		global::System.Int32 FrameCount { get; }

		List<UnitCtrl> EnemyList { get; }

		int ServerDeadFrame { get; }

		eChargeSkillTurn ChargeSkillTurn { get; set; }

		List<UnitCtrl> BlackOutUnitList { get; }

		bool IsPauseFrameCount { get; }

		eBattleGameState GameState { get; }

		float TimeLimit { get; }

		bool CanQuadruple { get; }

		bool IsPrincessSkillRunning { get; }

		bool IsPrincessSkillMovieRunning { get; }

		bool IsGuestCutinPlaying { get; }

		bool IsPlayingNoCutInMotionForPSkill { get; }

		bool IsPlayingNoCutInMotion { get; }

		bool CanReserveSkill { get; }

		bool IsDefenseBattle { get; }

		bool IsTowerContent { get; }

		bool CanAllSetMode { get; }

		bool IsReplayContents { get; }

		bool IsTalentBonusEffectPlaying { get; }

		bool IsReleasedKnightEnhance { get; }

		bool IsLabyrinth { get; }

		void InitializePassiveSkillUnitList(List<UnitParameter> _unitList, ePassiveInitType _passiveInitType, int _waveNumber);

		void CreateUnit(int _unitId, Action<UnitCtrl> _callback, bool _addUnitList);

		void InitOnlyAutoClearFlag(bool _isUseAllSet);

		void DisableAutoClear();

		void CreateUnitSpine(UnitCtrl _unit, eUnitRespawnPos _respawnPos, UnitParameter _pParam, Action<UnitCtrl> _callback, bool _isOther);

		void GamePause(bool pause, bool _isBlackoutEnd);

		void TutorialSkillExec(GameObject _focus);

		bool GetIsPlayCutin();

		void FadeOutAllVoices(float _fadeSec);

		void FadeOutAllSe(float _fadeSec);

		bool CanSkipBattle();

		bool IsQuestAutoProgressing();

		bool IsLocalReplay();

		void ExecuteSkipProcess(Action _callback);

		bool CanCreateBattleEffect();

		bool IsShowEffectiveIcon(int _unitId);
	}
}
