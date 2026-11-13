using global::OpenPCR.Runtime.Api;
using System.Collections;
using System.Collections.Generic;
using Elements.Battle.Core;

namespace Elements.Battle
{
	public interface IBattleManagerForActionParameter
	{
		float TimeLimit { get; }

		int ChangeSpeedOverwriteFieldCounter { get; }

		float DeltaTime { get; }

		eBattleCategory BattleCategory { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		eChargeSkillTurn ChargeSkillTurn { get; set; }

		List<UnitCtrl> BlackOutUnitList { get; }

		eBattleGameState GameState { get; }

		bool CancelByAwake { get; set; }

		bool IsBossBattle { get; }

		long CurrentEnemyPoint { get; }

		UnitUiCtrl UnitUiCtrl { get; }

		long AbsorberValue { get; }

		double DodgeTPRecoverRatio { get; }

		void ExecField(FieldDataBase _fieldData, int _actionId);

		void StartAllUnitEtherealizationCheck(UnitCtrl _source);

		void ExecEnvironment(EnvironmentData _environmentData, bool _isEnemySide);

		void ExecAllUnitSealDamageCut(AllUnitSealDamageCutData _data, bool _enemySide);

		void StopField(int _actionId, eTargetAssignment _targetAssignment, bool _isOther);

		bool ExistsField(int _actionId, bool _isOther);

		void AddChangeSpeedOverwriteFieldCounter();

		void CallbackActionEnd(long actionId);

		void SetBlackoutTimeZero();

		UnitCtrl Summon(SummonData _summonData);

		void GamePause(bool pause, bool _isBlackoutEnd);

		void SetSkillExeScreen(UnitCtrl _cc, float _exeTime, Color _color, bool _endWithMotion);

		void CallbackDead(UnitCtrl _unit);

		void CheckAcnLosingBattleFinish();

		void CallbackFadeOutDone(UnitCtrl unit);

		void CallbackIdleOnlyDone(UnitCtrl unit);

		Coroutine StartCoroutine(IEnumerator routine);

		UnitCtrl GetBossUnit();

		void SubstructEnemyPoint(long _enemyPoint);

		void SetSkillEffectSortOrderBack();

		int GetBlackOutUnitLength();

		void DeleteBonusIcon(int _bonusId);

		bool ExistsEnvironment(int _id);

		SpecialBattleData GetSpecialBattleData();

		void UpdateAbnormalStateField();
	}
}
