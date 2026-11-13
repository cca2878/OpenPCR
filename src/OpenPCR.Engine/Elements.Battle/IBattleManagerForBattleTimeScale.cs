using System.Collections;

namespace Elements.Battle
{
	public interface IBattleManagerForBattleTimeScale
	{
		float DeltaTime { get; }

		void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit);

		void StartCoroutineIgnoreFps(IEnumerator cr);
	}
}
