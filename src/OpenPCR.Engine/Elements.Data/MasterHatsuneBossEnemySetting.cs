namespace Elements.Data
{
	public class MasterHatsuneBossEnemySetting : global::Elements.Data.AbstractMasterData
	{

		public override void Unload()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterHatsuneBossEnemySetting::Unload():System.Void", "Native consumer declaration; behavior evidence pending");
		}

		public class HatsuneBossEnemySetting
		{
			public global::System.Int32 enemy_identify { [global::OpenPCR.Migration("14811822-91dc-4182-8608-11c1d4b11cab")] get { throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterHatsuneBossEnemySetting\u002BHatsuneBossEnemySetting::get_enemy_identify():System.Int32 (outside first-phase hatsune-boss boundary)"); } }
			public HatsuneBossEnemySetting(int boss_id = 0, int enemy_identify = 0, int event_id = 0, int must_kill_flag = 0, int event_boss_treasure_box_id = 0, double reward_gold_coefficient = 0.0, int reward_gold_limit = 0, int detail_offset_x = 0, int detail_offset_y = 0, double detail_scale = 0.0, int map_offset_x = 0, int map_offset_y = 0, double map_scale = 0.0, int map_depth = 0)
			{
				throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterHatsuneBossEnemySetting+HatsuneBossEnemySetting::.ctor(System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Double,System.Int32,System.Int32,System.Int32,System.Double,System.Int32,System.Int32,System.Double,System.Int32):System.Void", "Native consumer declaration; behavior evidence pending");
			}
		}
		public MasterHatsuneBossEnemySetting(global::Elements.Data.MasterHatsuneDatabase db) : base(db)
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.Data.MasterHatsuneBossEnemySetting::.ctor(Elements.Data.MasterHatsuneDatabase):System.Void", "Native consumer declaration; behavior evidence pending");
		}
	}
}
