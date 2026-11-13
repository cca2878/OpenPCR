namespace Elements.Battle
{
	public interface IBattleCameraEffectForBattleProcessor
	{
		float BattleCameraWaveScale { get; }
		float BossBattleScale { get; }

		void SetBgBlur(bool _isBlur);
	}
}
