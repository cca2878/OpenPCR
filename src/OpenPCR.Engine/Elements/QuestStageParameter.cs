using System.Collections.Generic;
namespace Elements
{
	public class QuestStageParameter
	{
		public int QuestId { get; private set; } = -1;
		public List<WaveEnemyInfoList> WaveEnemy { get; private set; } = new List<WaveEnemyInfoList>();
		public List<UnitParameter> EnemyParameter { get; private set; } = new List<UnitParameter>();
		private List<InventoryInfo> subDrops = new List<InventoryInfo>();
		public QuestStageParameter(int _questId = -1) { QuestId = _questId; }
		[global::OpenPCR.Migration("a049d853-727e-4b98-ad23-150e898a6473")]
		public List<WaveEnemyInfo> GetWaveEnemyInfo(int _index)
		{
			if (_index < 0) return null;
			if (WaveEnemy.Count <= _index) return null;
			return WaveEnemy[_index].EnemyInfoList;
		}
	}
}
