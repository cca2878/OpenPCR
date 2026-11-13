namespace Elements.Battle
{
	public interface IBattleTimeScaleForUnitCtrl
	{
		bool SpeedupFlag { get; set; }

		bool IsSpeedQuadruple { get; set; }

		float SpeedUpRate { get; }
	}
}
