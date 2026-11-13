using global::OpenPCR.Runtime.Api;

namespace Elements.Battle
{
	public interface IBattleCameraEffectForUnitCtrl
	{
		float CurrentZoom { get; }

		Vector3 ShakePosition { get; set; }

		void StartShake(ShakeEffect shake, Skill skill, UnitCtrl unit);

		void StopZoomEffect(UnitCtrl _unitCtrl);
	}
}
