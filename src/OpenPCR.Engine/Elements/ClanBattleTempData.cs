using System.Collections.Generic;

namespace Elements
{
	public class ClanBattleTempData : Singleton<ClanBattleTempData>
	{
		public int SupportUnitId { get; set; }
		public long SupportOwnerId { get; set; }
		public bool IsRehearsal { get; set; }
		public bool IsTrainingBattle { get; set; }
		public long ClanBattleBossRealDamage { get; set; }

		public List<ClanBattleSupportUnit> SupportUnitList { get; set; }
		public ClanBattleTopInfo ClanBattleTopInfo { get; set; }
		public int BattleId { get; set; }
		public int BattleLapNum { get; set; }
		public int BattlePhase { get; set; }
		public int BossOrderNum { get; set; }
		public int BattleClanBattleMode { get; set; }
		public int BattleTrainingId { get; set; }
		public ClanBattleFinishReceiveParam BattleFinishReceiveParam { get; set; }
		public ClanBattleRehearsalFinishReceiveParam BattleRehearsalFinishReceiveParam { get; set; }
		public ClanBattleTrainingFinishReceiveParam BattleTrainingFinishReceiveParam { get; set; }
		public bool IsActualBossStatus { get; set; }
		public int SelectedStockIndex { get; set; }

		public ClanBattleTempData()
		{
			ResetData();
		}

		public void ResetData()
		{
			// Account lobby production creates TopInfo, reward, ranking and result records; combat consumes the native support, lap, phase and mode properties directly.
			throw new global::OpenPCR.OutOfBoundaryException("Elements.ClanBattleTempData::ResetData");
		}
	}
}
