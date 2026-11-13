using System.Collections.Generic;
namespace Elements
{
	public class WaveEnemyInfoList
	{
		public List<WaveEnemyInfo> EnemyInfoList { get; private set; }
		public WaveEnemyInfoList() { initializeWaveEnemyInfoList(); }
		private void initializeWaveEnemyInfoList() { EnemyInfoList = new List<WaveEnemyInfo>(); }
	}
}
