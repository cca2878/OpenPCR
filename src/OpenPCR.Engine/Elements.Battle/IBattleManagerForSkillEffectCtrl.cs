using global::OpenPCR.Runtime.Api;
using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForSkillEffectCtrl
	{
		bool IsValid { get; }

		float DeltaTime { get; }

		bool IsHatsuneBoss { get; }

		bool IsSpecialBattle { get; }

		bool IsTowerContent { get; }

		bool IsHighRarityQuest { get; }

		List<UnitCtrl> BlackOutUnitList { get; }

		List<UnitCtrl> BlackoutUnitTargetList { get; }

		bool IsDefenseReplayMode { get; }

		bool IsSortBackBossBattle { get; }

		eChargeSkillTurn ChargeSkillTurn { get; set; }

		long AbsorberValue { get; set; }

		void AppendEffect(SkillEffectCtrl _skillEffectCtrl, UnitCtrl _unit, bool _isAbnormal);

		void AddEffectToUpdateList(SkillEffectCtrl ef);

		void RemoveEffectToUpdateList(SkillEffectCtrl ef);

		Coroutine StartCoroutine(IEnumerator _routine);

		void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit);

		UnitCtrl GetBossUnit();

		void GamePause(bool _pause, bool _isBlackoutEnd = false);

		void SetSkillExeScreen(UnitCtrl _source, float blackoutTime, Color blackoutColor, bool _endWithMotion);

		float GetRespawnPos(eUnitRespawnPos _unitRespawnPos);

		bool CanPlaySe();
	}
}
