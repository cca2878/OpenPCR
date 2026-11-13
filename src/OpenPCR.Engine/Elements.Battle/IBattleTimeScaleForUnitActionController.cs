namespace Elements.Battle
{
	public interface IBattleTimeScaleForUnitActionController
	{
		void StartSlowEffect(BattleDefine.SlowEffect _slowEffect, UnitCtrl _unit, float _startTime, bool _useCoroutine);

		void StopSlowEffect();
	}
}
