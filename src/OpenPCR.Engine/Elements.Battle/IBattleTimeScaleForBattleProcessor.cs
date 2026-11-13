namespace Elements.Battle
{
	public interface IBattleTimeScaleForBattleProcessor
	{
		bool SpeedupFlag { get; set; }

		void SetBaseTimeScale(float _timeScale);
	}
}
