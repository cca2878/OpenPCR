using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForDamageEffectCtrlBase
	{
		bool IsPauseFrameCount { get; }

		List<UnitCtrl> BlackoutUnitTargetList { get; }

		bool IsDefenseReplayMode { get; }

		bool HideDamageNumFlag { get; }

		eBattleGameState GameState { get; }

		bool IsPrincessSkillMovieRunning { get; set; }

		bool IsPrincessSkillMovieFirstHalfRunning { get; }

		bool CanPlaySe();
	}
}
