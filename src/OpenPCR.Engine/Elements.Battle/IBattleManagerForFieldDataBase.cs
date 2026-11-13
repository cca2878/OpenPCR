using System.Collections;
using System.Collections.Generic;

namespace Elements.Battle
{
	public interface IBattleManagerForFieldDataBase
	{
		int FieldIndex { get; set; }

		float DeltaTime { get; }

		eBattleGameState GameState { get; }

		Dictionary<int, int> EnvironmentCounterDic { get; set; }

		int CurrentEnvironmentId { get; set; }

		void AppendEffect(SkillEffectCtrl _skillEffectCtrl, UnitCtrl _unit, bool _isAbnormal);

		void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit);

		int GetBlackOutUnitLength();

		float GetRespawnPos(eUnitRespawnPos _unitRespawnPos);

		void SubstructEnemyPoint(long _point);

		void DeleteBonusIcon(int _bonusId);

		void SetEnvironmentResume(bool _isEnemy);
	}
}
