using System.Collections.Generic;

namespace Elements
{
	public class BattleVoiceFlagManager
	{
		public bool IsPlayed(int _unitId, int _groupId, SoundManager.eVoiceType _voiceType, bool _isEnemy = false)
		{
			if (_isEnemy)
			{
				return individualEnemyGroupVoiceUsedFlagDic.TryGetValue(_unitId, out var groups) && groups[_groupId][_voiceType];
			}

			return individualGroupVoiceUsedFlagDic[_unitId][_groupId][_voiceType];
		}

		[global::OpenPCR.Migration("019b2bf7-c255-4702-80db-c88239f26605")]
		public void SetPlayedFlag(int _unitId, int _groupId, SoundManager.eVoiceType _voiceType, bool _isPlayed = true, bool _isEnemy = false)
		{
			if (_isEnemy)
			{
				if (individualEnemyGroupVoiceUsedFlagDic.TryGetValue(_unitId, out var groups))
					groups[_groupId][_voiceType] = _isPlayed;
			}
			else
			{
				individualGroupVoiceUsedFlagDic[_unitId][_groupId][_voiceType] = _isPlayed;
			}
		}
		private Dictionary<int, Dictionary<int, Dictionary<SoundManager.eVoiceType, bool>>> individualGroupVoiceUsedFlagDic;
		private Dictionary<int, Dictionary<int, Dictionary<SoundManager.eVoiceType, bool>>> individualEnemyGroupVoiceUsedFlagDic;

		[global::OpenPCR.Migration("6dfa6d90-724e-409f-ade9-4bb1593ba72d")]
		public BattleVoiceFlagManager(List<UnitCtrl> _unitList, List<UnitCtrl> _enemyList)
		{
			global::OpenPCR.Presentation.Noop();
		}
	}
}
