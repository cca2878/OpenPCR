using System.Collections.Generic;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterSpaceBattleData : AbstractMasterData
	{
		public class SpaceBattleData
		{

			public SpaceBattleData(int space_battle_id = 0, int space_enemy_id = 0, int limit_time = 0, int clear_reward_group = 0, int background = 0, string sheet_id = "", string que_id = "", int result_boss_position_y = 0, int quest_detail_bg_id = 0, int quest_detail_bg_position = 0, string quest_name = "")
			{
				throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterSpaceBattleData+SpaceBattleData::.ctor(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.String,System.String,System.Int32,System.Int32,System.Int32,System.String)", "constructor body has no exact 7.0.1 match");
			}
		}
		public MasterSpaceBattleData(MasterSpaceDatabase db) : base(db)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MasterSpaceBattleData::MasterSpaceBattleData(Elements.MasterSpaceDatabase)");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MasterSpaceBattleData::Unload()");
		}
	}
}
