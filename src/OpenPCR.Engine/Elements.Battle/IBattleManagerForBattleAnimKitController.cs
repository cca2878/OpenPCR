using global::Elements;
using System.Collections;

namespace Elements.Battle
{
	public interface IBattleManagerForBattleAnimKitController
	{
		float DeltaTime { get; }

		bool IsPausingEffectSkippedInThisFrame { get; }

		void AddUnitSpineControllerList(BattleAnimKitController _unitSpineController);

		void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit);

		void RemoveUnitSpineControllerList(BattleAnimKitController _unitSpineController);

		bool IsSkipping();
	}
}
