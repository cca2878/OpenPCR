using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class WaveEnemyInfo
	{
		public global::System.Int32 EnemyId { get; private set; }
		public global::System.Int32 DropGold { get; private set; }
		public List<InventoryInfo> DropReward { get; private set; }

		public WaveEnemyInfo()
		{
			initializeWaveEnemyInfo();
		}

		private void initializeWaveEnemyInfo()
		{
			EnemyId = 0;
			DropGold = 0;
			DropReward = new List<InventoryInfo>();
		}
	}
}
