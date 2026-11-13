using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;

namespace Elements.Battle
{
	public interface IBattleManagerForCameraEffect
	{
		float DeltaTime { get; }

		Vector3 PlayCameraPos0 { get; }

		void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit);

		IEnumerator CallFade(float _from, float _to, float _duration, Action<float> _setFade, Action _onFinish);

		int GetBlackoutUnitTargetLength();

		UnitCtrl GetBlackoutUnitTarget(int _idx);

		void StartCoroutineIgnoreFps(IEnumerator _enumerator);
	}
}
