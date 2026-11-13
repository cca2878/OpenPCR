using System.Collections;

namespace Elements.Battle
{
	public interface IBattleManagerForUbAbnormalData
	{
		float DeltaTime { get; }

		void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit);
	}
}
