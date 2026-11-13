using System.Collections.Generic;
using Elements.Data;

namespace Elements
{
	public class SpaceBattleTempData
	{
		public int SpaceBattleProgress;
		public List<ClanBattleSupportUnit> SupportUnitList = new List<ClanBattleSupportUnit>();
		public int SupportUnitId;
		public long SupportOwnerId;
		public MasterSpaceSchedule.SpaceSchedule SpaceSchedule;
		public MasterSpaceTopData.SpaceTopData SpaceTopData;
		public MasterSpaceBattleData.SpaceBattleData SpaceBattleData;
		public MasterEnemyParameter.EnemyParameter EnemyParameter;
		public SpaceFinishReceiveParam FinishReceiveParam;
		public long CurrentBossHp;
		public bool SingleBattleWinFlag;
		public SpaceBattleTempData()
		{
			ResetData();
		}

		public void ResetData()
		{
			SpaceBattleProgress = 0;
			SupportUnitList = new List<ClanBattleSupportUnit>();
			SupportUnitId = 0;
			SupportOwnerId = 0;
			SpaceSchedule = null;
			SpaceTopData = null;
			SpaceBattleData = null;
			EnemyParameter = null;
			FinishReceiveParam = null;
			CurrentBossHp = 0;
			SingleBattleWinFlag = false;
		}
	}
}
