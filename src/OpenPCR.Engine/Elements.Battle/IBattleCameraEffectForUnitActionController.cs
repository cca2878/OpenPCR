namespace Elements.Battle
{
	public interface IBattleCameraEffectForUnitActionController
	{
		void StartZoomEffect(BattleDefine.ZoomEffect _zoomEffect, UnitCtrl _unit, float _startTime, bool _ignorePause, bool _useStartCoroutine, bool _isNotUb = true);

		void StartBlurEffect(BlurEffect.BlurEffectData blurData);

		void StartShake(ShakeEffect shake, Skill skill, UnitCtrl unit);

		void UpdateShake();

		void ClearShake();
	}
}
