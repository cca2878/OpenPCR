using global::OpenPCR.Runtime.Api;
using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForUnitActionController
	{
		UnitCtrl DecoyUnit { get; }

		UnitCtrl DecoyOther { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		UnitCtrl RelicEnemyUnit { get; }

		CoroutineManager CoroutineManager { get; }

		List<UnitCtrl> BlackoutUnitTargetList { get; }

		float DeltaTime { get; }

		List<long> ExecutedActionList { get; }

		Dictionary<string, GameObject> ReActionEffectDictionary { get; }

		bool IsDefenseReplayMode { get; }

		bool IsNoCutInMotionRightAfter { set; }

		long AbsorberValue { get; }

		eChargeSkillTurn ChargeSkillTurn { get; }

		bool IsLabyrinth { get; }

		void SetSkillExeScreen(UnitCtrl _cc, float _exeTime, Color _color, bool _endWithMotion);

		void StartBackgroundSpineAnimation(int _skillId, float _skipTime);

		void AddBlackOutTarget(UnitCtrl _source, UnitCtrl _target, BasePartsData _parts);

		void SetSkillExeScreenActive(UnitCtrl _unit, Color _color);

		void CallbackActionEnd(long actionId);

		void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit);

		Coroutine StartCoroutine(IEnumerator routine);

		void StartChangeScale(Skill _skill, float _startTime);

		void SetForegroundEnable(bool _enable);

		bool IsSkipping();

		bool CanCreateBattleEffect();
	}
}
