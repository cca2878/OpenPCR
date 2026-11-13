using global::OpenPCR.Runtime.Api;
using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForLifeGaugeController
	{
		bool IsDefenseReplayMode { get; }

		float DeltaTime { get; }

		List<UnitCtrl> BlackOutUnitList { get; }

		bool IsColosseumBattle { get; }

		bool IsDomeBattle { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		Coroutine StartCoroutine(IEnumerator _routine);

		void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit);
	}
}
