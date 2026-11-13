using global::OpenPCR.UICompat;
namespace Elements.Battle
{
	public interface IBattleManagerForPartsBossGauge
	{
		bool IsDispTotalDamageBattle { get; }

		bool IsHatsuneBoss { get; }

		bool IsSpecialBattle { get; }

		bool IsAcnUnknownBattle { get; }

		bool IsTowerContent { get; }

		bool IsTowerReplayContent { get; }

		int GetCurrentWaveOffset();

		bool GetBossGaugeColorChange();

		float GetBossGaugeValue(long _currentHp);

		void SetSpecialBattleGaugeColor(UIProgressBar _gauge);

		bool IsDisplayBossHpUi();
	}
}
