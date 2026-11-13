using System.Collections.Generic;

namespace Elements
{
	public class TrialBattleTempData
	{
		public long SupportOwnerId;
		public int SupportUnitId;
		public long BossRealDamage;

		public List<TrialBattleSupportUnit> SupportUnitList { get; set; }
		public List<TrialBattleDefine.SupportBattleRarity> SupportBattleRarities { get; set; }
		public TrialBattleDefine.InitClanBattleSelectDifficultyParam InitClanBattleSelectDifficultyParam;
		public TrialBattleDefine.DuringClanBattleModeParam DuringClanBattleModeParam;
		public Dictionary<int, TrialBattleDefine.TalentCustomSettingInfo> TalentCustomSettingInfoDic;

		public TrialBattleTempData()
		{
			SupportBattleRarities = new List<TrialBattleDefine.SupportBattleRarity>();
			TalentCustomSettingInfoDic = new Dictionary<int, TrialBattleDefine.TalentCustomSettingInfo>();
		}
	}
}
