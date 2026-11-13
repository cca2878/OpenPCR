using System.Collections.Generic;

namespace Elements.Data
{
	[global::System.Reflection.DefaultMember("Item")]
	public class MasterWaveGroupData : AbstractMasterData
	{
		public MasterWaveGroupData(MasterQuestDatabase db) : base(db)
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterWaveGroupData.ctor: outside guild/training data production");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterWaveGroupData.Unload: outside guild/training data production");
		}

		public class WaveGroupData
		{
			public List<EnemyInfo> EnemyInfos;
			public WaveGroupData(int id = 0, int wave_group_id = 0, int odds = 0, int enemy_id_1 = 0, int drop_gold_1 = 0, int drop_reward_id_1 = 0, int enemy_id_2 = 0, int drop_gold_2 = 0, int drop_reward_id_2 = 0, int enemy_id_3 = 0, int drop_gold_3 = 0, int drop_reward_id_3 = 0, int enemy_id_4 = 0, int drop_gold_4 = 0, int drop_reward_id_4 = 0, int enemy_id_5 = 0, int drop_gold_5 = 0, int drop_reward_id_5 = 0, int guest_enemy_id = 0, int guest_lane = 0)
			{
				throw new global::OpenPCR.OutOfBoundaryException("MasterWaveGroupData.WaveGroupData.ctor: outside guild/training data production");
			}

			public class EnemyInfo
			{
				public int EnemyId;
				public int Gold;
				public int EnemyDropId;
				public EnemyInfo(int _id, int _gold, int _drop)
				{
					EnemyId = _id;
					Gold = _gold;
					EnemyDropId = _drop;
				}
			}
		}
	}
}
