using System.Runtime.CompilerServices;

namespace Elements
{
	public class IniSetting
	{
		public EquipStrSetting EquipmentEnhance { get; private set; }
		public QuestSetting Quest { get; private set; }
		public DungeonSetting Dungeon { get; private set; }
		public UnitSetting Unit { get; private set; }
		public ArenaSetting Arena { get; private set; }
		public GrandArenaSetting GrandArena { get; private set; }
		public LimitSetting Limit { get; private set; }
		public ClanSetting Clan { get; private set; }
		public ClanBattleSetting ClanBattle { get; private set; }
		public FriendSetting Friend { get; private set; }
		public RoomSetting Room { get; private set; }
		public TowerSetting Tower { get; private set; }
		public RecoverChallengeCountSetting RecoverChallengeCount { get; private set; }
		public RecoverChallengeCountSetting VeryHardRecoverChallengeCount { get; private set; }
		public SendApi SendApi { get; private set; }
		public BattleLogType BattleLogType { get; private set; }
		public NormalGachaSetting NormalGacha { get; private set; }
		public global::System.Int32 RequireExchangePoint { get; private set; }
		public RecoverChallengeCountSetting HatsuneRecoverChallengeCount { get; private set; }
		public CartoonSetting Cartoon { get; private set; }
		public UniqueEquipSetting UniqueEquip { get; private set; }
		public RecoverChallengeCountSetting EquipRecoverChallengeCount { get; private set; }
		public RecoverChallengeCountSetting HighRarityEquipRecoverChallengeCount { get; private set; }
		public UnreadStoryNoticeSetting UnreadStoryNotice { get; private set; }
		public global::System.Int32 MaxBossBattleSkipNum { get; private set; }
		public global::System.Int32 MaxGachaCount { get; private set; }
		public BulkSkipSetting MultipleSkip { get; private set; }
		public FriendSupportUnitIniSetting FriendSupportUnit { get; private set; }
		public KaiserBattleIniSetting KaiserBattle { get; private set; }
		public StoryRaidEvenBattletIniSetting LegionBattle { get; private set; }
		public StoryRaidEvenBattletIniSetting Sre { get; private set; }
		public SerialCodeIniSetting SerialCode { get; private set; }
		public global::System.Int32 ArenaSkipUpperRank { get; private set; }
		public global::System.Int32 LoopBoxMultiGachaCount { get; private set; }
		public TravelIniSetting Travel { get; private set; }
		public ExEquipIniSetting ExEquip { get; private set; }
		public MaxOnceConsumeGoldSetting MaxOnceConsumeGold { get; private set; }
		public SpecialFesDiscountIniSetting Sfd { get; private set; }

		public IniSetting()
		{
			initializeIniSetting();
		}

		private void initializeIniSetting()
		{
			EquipmentEnhance = null;
			Quest = null;
			Dungeon = null;
			Unit = null;
			Arena = null;
			GrandArena = null;
			Limit = null;
			Clan = null;
			ClanBattle = null;
			Friend = null;
			Room = null;
			Tower = null;
			RecoverChallengeCount = null;
			VeryHardRecoverChallengeCount = null;
			SendApi = null;
			BattleLogType = null;
			NormalGacha = null;
			RequireExchangePoint = 300;
			HatsuneRecoverChallengeCount = null;
			Cartoon = null;
			UniqueEquip = null;
			EquipRecoverChallengeCount = null;
			HighRarityEquipRecoverChallengeCount = null;
			UnreadStoryNotice = null;
			MaxBossBattleSkipNum = 0;
			MaxGachaCount = 0;
			MultipleSkip = null;
			FriendSupportUnit = null;
			KaiserBattle = null;
			LegionBattle = null;
			Sre = null;
			SerialCode = null;
			ArenaSkipUpperRank = 0;
			LoopBoxMultiGachaCount = 0;
			Travel = null;
			ExEquip = null;
			MaxOnceConsumeGold = null;
			Sfd = null;
		}
	}
}
