using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Common;
using Elements.Data;
using Elements.Utility;

namespace Elements
{
	public class UserData : Singleton<UserData>
	{
		public long ShioriMultiTargetBossReleaseUnixTime { get; private set; }


		private long slipDamageLogBarrierReleaseUnixTime;

		[global::OpenPCR.Migration("4b254b0d-5470-4a90-b730-ddbfed6e0f38")] public bool IsReleaseSlipDamageLogBarrier() { return slipDamageLogBarrierReleaseUnixTime >= 1L && nowUnixTime >= slipDamageLogBarrierReleaseUnixTime; }


		private int currentFrame = -1;

		private long nowUnixTime;

		public DateTime NowDateTime => TimeData.GetNowTime();

		private long skillLevelSyncReleaseUnixTime;

		[global::OpenPCR.Migration("e743bf99-590e-4a75-8688-0ea0cb547d81")] public bool IsReleaseSkillLevelSync() => skillLevelSyncReleaseUnixTime >= 1L && nowUnixTime >= skillLevelSyncReleaseUnixTime;


		public global::System.Collections.Generic.Dictionary<System.Int32, Elements.UserQuestInfo> QuestMapParameterDictionary { get; private set; }

		public int MainStoryArrivalNormalQuestId { get; private set; }


		public global::Elements.QuestStageParameter QuestStageParameter { get; private set; }
		public TimeData TimeData = new TimeData();
		private InventoryController inventoryController = new InventoryController();

		public UserInfo UserInfo { get; private set; }

		public UserClan UserClan { get; private set; }
		public int CurrentClanBattleMode { get; private set; }

		public List<RestrictionExtraEquip> RestrictionExtraEquips { get; set; }
		private List<RestrictionExtraEquip> temporaryRestrictionExtraEquips { get; set; }

		[global::OpenPCR.Migration("9509ea0b-7873-4713-ac45-83c5d545bd06")]
		public void SetClanBattleRestrictionExtraEquip()
		{
			foreach (RestrictionExtraEquip temp in temporaryRestrictionExtraEquips)
			{
				if (!RestrictionExtraEquips.Any(data => (int)data.UnitId == (int)temp.UnitId && (int)data.SerialId == (int)temp.SerialId))
					RestrictionExtraEquips.Add(new RestrictionExtraEquip(temp.SerialId, temp.UnitId));
			}
		}


		public int EnergyRecoveryRateMax { get; set; }

		public long NowUnixTime
		{
			[global::OpenPCR.Migration("c5574f97-c28c-4e40-8094-87bc5805644e")]
			get
			{
				int frameCount = global::OpenPCR.Runtime.Api.Time.frameCount; if (currentFrame != frameCount) nowUnixTime = global::OpenPCR.Foundation.TimeUtil.ToUnixTime(NowDateTime); currentFrame = frameCount; return nowUnixTime;
			}
		}

		public InitSettingParameter InitSettingParameter { get; private set; }

		public Dictionary<int, UnitParameter> UnitParameterDictionary { get; private set; }

		private Dictionary<ePartyType, int[]> partyDictionary { get; set; }

		public bool IsReleaseChangeRarityFlag { get; set; }
		public ArenaParameters ArenaParameters { get; private set; }

		[global::OpenPCR.Migration("033b5ae8-c5ed-4753-ac60-36dff59e0ac4")]
		public UserData()
		{
			throw new global::OpenPCR.OutOfBoundaryException("UserData account construction belongs to the external input provider. Supply account identity/team level, current time and release timestamps/flags, prepared unit/party dictionaries and resolved inventory before battle; the native login/account manager is outside first-phase scope.");
		}

		[global::OpenPCR.Migration("b70a8500-6859-46a8-a67d-c3e6c24b83bb")]
		public ExtraEquipPair SearchExtraEquip(int _serialId)
		{
			return inventoryController.SearchExtraEquip(_serialId);
		}

		[global::OpenPCR.Migration("37c789a2-7d1d-4869-9d58-b0aaceedd5e5")]
		public bool IsEmptyParty(ePartyType _partyType)
		{
			return System.Linq.Enumerable.All(partyDictionary[_partyType], id => id == 0);
		}

		[global::OpenPCR.Migration("db5bc3b0-e864-4e58-aa39-6f85d232250b")]
		public int[] FindPartyUnitIdArray(ePartyType _partyType)
		{
			if (_partyType == ePartyType.CLAN_BATTLE || _partyType == ePartyType.TRIAL_BATTLE || unchecked((uint)((int)_partyType - 131)) <= 3u)
				return partyDictionary[_partyType];

			throw new global::OpenPCR.OutOfBoundaryException("Elements.UserData::FindPartyUnitIdArray(Elements.ePartyType):System.Int32[]");
		}
	}
}
