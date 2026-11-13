namespace Elements.Battle
{
	public interface IBattleLog
	{
		void AppendBattleLog(eBattleLogType _battleLogType, int _type, long _value, long _currentValue, int _duration, int _actionId, int _value2 = 1, int _value3 = 1, UnitCtrl _source = null, UnitCtrl _target = null);
	}
}
