using System;
using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForGrandArenaBattleProcessor : IBattleManagerForBaseBattleProcessor
	{
		int CurrentWave { get; }

		List<UnitCtrl> UnitList { get; }

		int PlayVoiceUnitId { get; set; }

		bool IsSuspendLose { get; }

		void PlayPartyUnitJoyMotion();

		IEnumerator CallTimer(float _time, Action _onComplete);

		List<UnitCtrl> GetAliveUnitList();

		void ShowResultDialog();

		void RemoveAllCoroutines();
	}
}
