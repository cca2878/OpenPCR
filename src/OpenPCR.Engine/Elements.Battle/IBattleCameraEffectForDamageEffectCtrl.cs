using global::OpenPCR.Runtime.Api;

namespace Elements.Battle
{
	public interface IBattleCameraEffectForDamageEffectCtrl
	{
		float CurrentZoom { get; }

		Vector3 ShakePosition { get; }

		Vector3 ZoomPos { get; }
	}
}
