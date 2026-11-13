using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class TowerTempData
	{
		public int UseSkipTicketNum = 1;
		public bool IsRehearsal;
		public int LastChallengedExQuestId = -1;
		public int ExFrameRate = 30;
		public List<string> battleLogList;
		public eBattleResult ReplayBackupTowerBattleResult = eBattleResult.INVALID_VALUE;

		public TowerTempData()
		{
			throw new OpenPCR.MigrationPendingException("Elements.TowerTempData::.ctor()", "structure diagnostic remains after declaration migration");
		}
	}
}
