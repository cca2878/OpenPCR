using System.Runtime.CompilerServices;

namespace Elements
{
	public class InitSettingParameter
	{
		public InitSettingEquipment EquipStr;
		public QuestSetting QuestSetting;
		public InitSettingUnit Unit;
		public ArenaSetting Arena;
		public GrandArenaSetting GrandArena;
		public LimitSetting Limit;
		public DungeonSetting DungeonSetting;
		public ClanSetting ClanSetting;
		public ClanBattleSetting ClanBattleSetting;
		public RoomSetting RoomSetting;
		public FriendSetting FriendSetting;
		public RecoverChallengeCountSetting RecoverChallengeCountSetting;
		public RecoverChallengeCountSetting VeryHardRecoverChallengeCountSetting;
		public TowerSetting TowerSetting;
		public SendApi SendApi;
		public BattleLogType BattleLogType;
		public NormalGachaSetting NormalGachaSetting;
		public RecoverChallengeCountSetting HatsuneRecoverChallengeCountSetting;
		public CartoonSetting CartoonSetting;
		public UniqueEquipSetting UniqueEquipSetting;
		public RecoverChallengeCountSetting EquipRecoverChallengeCount;
		public RecoverChallengeCountSetting HighRarityEquipRecoverChallengeCount;
		public UnreadStoryNoticeSetting UnreadStoryNoticeSetting;
		public FriendSupportUnitIniSetting FriendSupportUnitSetting;
		public BulkSkipSetting BulkSkipSetting;
		public KaiserBattleIniSetting KaiserBattleIniSetting;
		public StoryRaidEvenBattletIniSetting LegionBattleIniSetting;
		public StoryRaidEvenBattletIniSetting StoryRaidEventBattleIniSetting;
		public SerialCodeIniSetting SerialCodeSetting;
		public TravelIniSetting TravelSetting;
		public ExEquipIniSetting ExEquipSetting;
		public SpecialFesDiscountIniSetting SpecialFesDiscountIniSetting;
		public LogBarrierIniSetting LogBarrierIniSetting;
		public int ArenaSkipUpperRank { get; private set; }
		public MaxOnceConsumeGoldSetting MaxOnceConsumeGoldSetting { get; private set; }

		public InitSettingParameter(IniSetting setting)
		{
			EquipStr = new InitSettingEquipment(setting);
			QuestSetting = setting.Quest;
			Unit = new InitSettingUnit(setting.Unit);
			Arena = setting.Arena;
			GrandArena = setting.GrandArena;
			Limit = setting.Limit;
			DungeonSetting = setting.Dungeon;
			ClanSetting = setting.Clan;
			ClanBattleSetting = setting.ClanBattle;
			RoomSetting = setting.Room;
			FriendSetting = setting.Friend;
			RecoverChallengeCountSetting = setting.RecoverChallengeCount;
			VeryHardRecoverChallengeCountSetting = setting.VeryHardRecoverChallengeCount;
			TowerSetting = setting.Tower;
			SendApi = setting.SendApi;
			BattleLogType = setting.BattleLogType;
			NormalGachaSetting = setting.NormalGacha;
			HatsuneRecoverChallengeCountSetting = setting.HatsuneRecoverChallengeCount;
			CartoonSetting = setting.Cartoon;
			UniqueEquipSetting = setting.UniqueEquip;
			EquipRecoverChallengeCount = setting.EquipRecoverChallengeCount;
			HighRarityEquipRecoverChallengeCount = setting.HighRarityEquipRecoverChallengeCount;
			UnreadStoryNoticeSetting = setting.UnreadStoryNotice;
			FriendSupportUnitSetting = setting.FriendSupportUnit;
			BulkSkipSetting = setting.MultipleSkip;
			KaiserBattleIniSetting = setting.KaiserBattle;
			LegionBattleIniSetting = setting.LegionBattle;
			StoryRaidEventBattleIniSetting = setting.Sre;
			SerialCodeSetting = setting.SerialCode;
			ArenaSkipUpperRank = setting.ArenaSkipUpperRank;
			TravelSetting = setting.Travel;
			ExEquipSetting = setting.ExEquip;
			MaxOnceConsumeGoldSetting = setting.MaxOnceConsumeGold;
			SpecialFesDiscountIniSetting = setting.Sfd;
		}
	}
}
