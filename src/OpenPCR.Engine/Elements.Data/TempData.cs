using global::OpenPCR.Foundation;
using System;
using System.Collections.Generic;
using global::OpenPCR.Runtime.Api;

namespace Elements.Data
{
	public class TempData : global::Elements.Singleton<global::Elements.Data.TempData>
	{

		public System.Collections.Generic.List<SupportUnitSetting> SupportUnitSettingList { get; set; }
		public global::System.Collections.Generic.List<Elements.SupportUnitStatus> SupportFriendList { get; set; }
		public global::System.Collections.Generic.List<Elements.SupportUnitStatus> SupportGeneralList { get; set; }
		public global::System.Collections.Generic.List<Elements.SupportUnitStatus> SupportNpcList { get; set; }
		public int SelectedQuestId { get; set; }
		public global::Elements.ePartyType SelectedPartyType { get; set; }
		public global::OpenPCR.Foundation.XorShift128Random XorShiftRandom { get; private set; }
		private HatsuneTempData hatsuneTempData;
		public HatsuneTempData HatsuneTemp => hatsuneTempData;

		[global::OpenPCR.Migration("232065d2-5b65-4f26-a098-d6ff6ddb9cdd")]
		public void InitXorShiftRandom(int _seed)
		{
			XorShiftRandom = new global::OpenPCR.Foundation.XorShift128Random(
				unchecked((uint)_seed), 362436069u, 521288629u, 88675123u);
		}
		public global::Elements.SupportUnitDefine.QuestStartParam SupportQuestStartParam { get; set; }

		private BattleStage battleStageData;
		private List<global::Elements.UnitDamageInfo> unitDamageInfoList;
		private List<global::Elements.UnitDamageInfo> enemyDamageInfoList;
		private List<int> enemyColorIdList;
		private List<List<UnitDamageInfo>> unitDamageHistoryList;
		private List<List<UnitDamageInfo>> enemyDamageHistoryList;
		private List<List<int>> enemyColorHistoryList;
		public List<List<UnitDamageInfo>> UnitDamageHistoryList
		{
			[global::OpenPCR.Migration("61a2c000-e220-4971-ba7f-389b79bc871a")]
			get => unitDamageHistoryList; [global::OpenPCR.Migration("6cd6114f-4a85-4c54-8165-0e2b2a74a925")]
			set => unitDamageHistoryList = value;
		}
		public List<List<UnitDamageInfo>> EnemyDamageHistoryList
		{
			[global::OpenPCR.Migration("dba0f37d-0f63-4045-8a9a-ad558f8dbe83")]
			get => enemyDamageHistoryList; [global::OpenPCR.Migration("afd4a404-01ad-4b06-a2be-596e4aafc64b")]
			set => enemyDamageHistoryList = value;
		}
		public List<List<int>> EnemyColorHistoryList
		{
			[global::OpenPCR.Migration("1e245b5f-eb11-46d6-8e20-0a7f092da26e")]
			get => enemyColorHistoryList; [global::OpenPCR.Migration("6fbe5d39-3c00-40e4-8304-afc530b9e943")]
			set => enemyColorHistoryList = value;
		}
		private List<global::Elements.UnitUnionBurstTimeline> unitUnionBurstTimelineList;
		public global::System.Collections.Generic.Dictionary<int, global::Elements.UnitParameter> StoryQuestPartyParameter;
		public global::Elements.eBattleSkipState BattleSkipState;
		public int SupportUnitId { get; set; }
		public long SupportOwnerId { get; set; }

		public bool CanSkipBattle;
		public global::Elements.eViewId ReturnViewOnRematch;
		public global::Elements.Data.TempData.BattleStage BattleStageData { [global::OpenPCR.Migration("558983a6-bf1e-47d2-82b2-2f77968835de")] get => battleStageData; set => battleStageData = value; }
		public List<global::Elements.UnitDamageInfo> UnitDamageInfoList { [global::OpenPCR.Migration("217d9ead-13a3-490a-9768-9c340818c6e5")] get => unitDamageInfoList; set => unitDamageInfoList = value; }
		public List<global::Elements.UnitDamageInfo> EnemyDamageInfoList { [global::OpenPCR.Migration("1e9035ac-9dc1-4005-9c00-1d52d553c850")] get => enemyDamageInfoList; set => enemyDamageInfoList = value; }
		public List<int> EnemyColorIdList { [global::OpenPCR.Migration("e9d3ae12-dbf3-4533-8bfa-4456f1c5b50c")] get => enemyColorIdList; set => enemyColorIdList = value; }
		public List<global::Elements.UnitUnionBurstTimeline> UnitUnionBurstTimelineList { [global::OpenPCR.Migration("f83bd15b-60a8-4acc-82fa-7459a82305c5")] get => unitUnionBurstTimelineList; set => unitUnionBurstTimelineList = value; }

		[global::OpenPCR.Migration("eca7521e-2d10-43d2-9e63-51349b5a593b")]
		public global::Elements.UnitDamageInfo CreateDamageData(int unitId, long viewerId, int _rarity, bool isEnemy = false, int _enemyColorId = 0)
		{
			var damageInfo = new global::Elements.UnitDamageInfo();
			damageInfo.SetUnitId(unitId);
			damageInfo.SetViewerId(viewerId);
			damageInfo.SetRarity(_rarity);
			if (isEnemy)
			{
				EnemyDamageInfoList.Add(damageInfo);
				enemyColorIdList.Add(_enemyColorId);
			}
			else
			{
				UnitDamageInfoList.Add(damageInfo);
			}

			return damageInfo;
		}

		public global::Elements.QuestAutoProgressData QuestAutoProgressData { get; set; }

		public TempData()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.TempData::.ctor(): Native constructor first initializes event, friend, gacha, Hatsune, login and minigame account producers before its battle-input storage; complete account initialization is outside the offline battle-input boundary.");
		}
		public class HatsuneTempData
		{
			public global::Elements.HatsuneDefine.eHatsuneEventType SelectedEventType { get; private set; }

			public HatsuneTempData()
			{
				throw new global::OpenPCR.OutOfBoundaryException("TempData.HatsuneTempData: complete event account/master/gacha initialization and AllClear production are outside the prepared battle input boundary.");
			}
		}

		public class BattleStage
		{

			public int BattleLogId { get; set; }

			private global::System.Collections.Generic.List<Elements.UnitParameter> replayPartyUnits;
			public global::System.Collections.Generic.List<Elements.UnitParameter> ReplayPartyUnits { [global::OpenPCR.Migration("85ac2ce7-8975-4176-b24d-1f74af2af555")] get => replayPartyUnits; set => replayPartyUnits = value; }
			public float BattleTimeLimit { get; set; }
			public global::Elements.eBattleClearType ReplayClearType { get; set; }
			public global::Elements.UnitParameter GuestUnitParameter { get; set; }

			public int[] CurrentBattleBackgrounds;
			public global::System.Collections.Generic.List<TeamEnhanceParameter> UnitTeamEnhanceInfoForEachParty;
			public int CurrentBattleId { get; set; }
			public string BattleName { get; set; }
			public string BattleLevel { get; set; }
			public bool IsDefenseMode { get; set; }

			private global::System.Collections.Generic.List<System.Collections.Generic.List<Elements.ContinuousUnit>> currentContinuousUnit;
			private global::System.Collections.Generic.List<Elements.InventoryInfo> currentBattleReward;
			public bool IsResultWin { get; set; }
			public global::Elements.eUnitRespawnPos GuestRespawnPos { get; set; }
			public global::System.Collections.Generic.List<System.Collections.Generic.List<Elements.ContinuousUnit>> CurrentContinuousUnit { [global::OpenPCR.Migration("08e7a3f6-f135-42dc-95be-067d0e31ca76")] get => currentContinuousUnit; set => currentContinuousUnit = value; }
			public global::System.Collections.Generic.List<Elements.InventoryInfo> CurrentBattleReward { [global::OpenPCR.Migration("a2aeda63-6164-4b32-a25f-dab64084061c")] get => currentBattleReward; set => currentBattleReward = value; }

			private global::Elements.BossInfo currentClanBattleBossInfo;
			public int Seed { get; set; }
			public global::Elements.eBattleCategory CurrentBattleCategory { get; set; }

			private global::System.Collections.Generic.List<global::System.Collections.Generic.List<global::Elements.UnitParameter>> currentBattleWaves;
			[global::OpenPCR.Migration("3cffc2f7-af16-4fee-a1f4-68d13b3cba05")] public global::System.Collections.Generic.List<global::System.Collections.Generic.List<global::Elements.UnitParameter>> CurrentBattleWaves { [global::OpenPCR.Migration("1ed623cd-cfd3-4ae3-ac6d-151fa8d18c68")] get => currentBattleWaves; [global::OpenPCR.Migration("f7bbdb77-5557-4fd1-9338-04ff91d77fac")] set => currentBattleWaves = value; }
			public global::Elements.BossInfo CurrentClanBattleBossInfo { [global::OpenPCR.Migration("636f64ff-3838-45cf-b0d3-6f326270f110")] get => currentClanBattleBossInfo; set => currentClanBattleBossInfo = value; }
			public TeamEnhanceParameter UnitTeamEnhanceInfo { get; set; }
			public TeamEnhanceParameter EnemyTeamEnhanceInfo { get; set; }
			public LabyrinthEnhanceParameter LabyrinthEnhanceParameter { get; set; }

			public BattleStage()
			{
				UnitTeamEnhanceInfoForEachParty = new global::System.Collections.Generic.List<TeamEnhanceParameter>();
				CurrentBattleCategory = (global::Elements.eBattleCategory)0;
				CurrentBattleId = 0;
				CurrentBattleWaves = new global::System.Collections.Generic.List<global::System.Collections.Generic.List<global::Elements.UnitParameter>>();
				currentContinuousUnit = null;
				CurrentClanBattleBossInfo = null;
				currentBattleReward = null;
				ReplayPartyUnits = null;
				BattleName = string.Empty;
				BattleLevel = string.Empty;
				IsDefenseMode = false;
				UnitTeamEnhanceInfo = null;
				EnemyTeamEnhanceInfo = null;
			}
		}
	}
}
