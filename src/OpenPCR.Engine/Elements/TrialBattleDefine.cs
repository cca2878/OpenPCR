using System.Collections.Generic;

namespace Elements
{
	public class TrialBattleDefine
	{
		public enum eTalentFormationBonusState { OFF = 0, ON = 1 }
		public class InitClanBattleSelectDifficultyParam
		{
			public int ClanBattleId;
			public int OrderNum;

		}
		public class DuringClanBattleModeParam
		{
			public int InfinityHpModeKey;
			public int BossResultPosY;
			public int Phase;

		}
		public class SupportBattleRarity
		{
			public int UnitId;

		}
		public class TalentCustomSettingInfo
		{
			public eTalentFormationBonusState TalentFormationBonusState;
			public List<int> BossTalentWeaknessIdList = new List<int>();

		}

	}
}
