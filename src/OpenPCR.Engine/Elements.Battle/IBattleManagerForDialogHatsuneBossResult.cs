using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForDialogHatsuneBossResult
	{
		eBattleCategory BattleCategory { get; }

		eBattleResult BattleResult { get; }

		List<UnitCtrl> UnitList { get; }

		List<UnitCtrl> EnemyList { get; }

		long CurrentEnemyPoint { get; }

		bool IsSpecialBattle { get; }

		bool IsDefenseBattle { get; }

		float TimeLimit { get; }

		float TakeOverTimeForDefense { get; }

		float StartLimitTimeForDefense { get; }

		bool IsShowBossGauge { get; }

		bool IsHatsuneBattle { get; }

		eBattleClearType GetAutoClearType();

		void RunOutBattleResult();

		eSpecialBattlePurpose GetPurpose();

		long GetPurposeCount();

		void SetDamageGoldModeMaxHpAttackNum(ref long _damage, ref int _gold, ref int _mode, ref long _maxHp, ref int _attackNum);

		int GetCurrentWaveOffset();

		bool GetBossGaugeColorChange();

		void InstantiatePointCounter(Transform _transform, ref PartsHastunePointCounter _counter);

		void SetSpecialBattleGaugeColor(UIProgressBar _gauge);

		float GetBossGaugeValue(long _currentHp);

		void InstantiateDefenseTimeGauge(Transform parent, ref PartsHatsuneDefenseTimeGauge localTimeGauge);
		public
				(long currentHp, long maxHp) GetTotalBossHp()
		;
	}
}
