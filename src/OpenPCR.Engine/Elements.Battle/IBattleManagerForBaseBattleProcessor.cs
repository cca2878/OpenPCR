using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForBaseBattleProcessor
	{
		eBattleResult BattleResult { get; }

		BattleLog BattleLog { get; }

		List<UnitCtrl> BossList { get; set; }

		bool IsCurtain { get; set; }

		int GetMiliLimitTime();

		void CallbackRequestFinishBattle();

		void ResultApiSendExec(Action _execCallback = null, Action<int> _errorCallback = null);

		Coroutine StartCoroutine(IEnumerator routine);

		int GetUnitCtrlLength();

		UnitCtrl GetUnitCtrl(int _idx);

		int GetEnemyCtrlLength();

		UnitCtrl GetEnemyCtrl(int _idx);

		UnitCtrl FindEnemy(int _id);

		UnitCtrl GetBossUnit();

		IEnumerator RecreateDeadUnits(Action _callback, eBattleResult _battleResult);

		void RevivalAtFinishBattle();

		List<UnitHpInfo> CreateUnitHpInfoList(long _enemyViewerId, long _myViewerId);

		List<UnitHpInfoForFriendBattle> CreateUnitHpInfoListForFriendBattle(long _enemyViewerId, long _myViewerId);

		void SetupLosePlayVoiceUnitId();

		void StartResult();

		IEnumerator UpdateBossIdleMotion(BattleAnimKitController _unitSpineController, int _motionPrefix);

		void TurnOffAllEffects();

		void OffSkillExeScreen();

		void ChangeAllUnitDefaultShader();

		void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit = null);
	}
}
