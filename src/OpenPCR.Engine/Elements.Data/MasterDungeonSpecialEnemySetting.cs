namespace Elements.Data
{
	public class MasterDungeonSpecialEnemySetting : AbstractMasterData
	{
		public MasterDungeonSpecialEnemySetting(MasterDungeonDatabase db) : base(db)
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterDungeonSpecialEnemySetting.ctor: outside guild/training data production");
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("MasterDungeonSpecialEnemySetting.Unload: outside guild/training data production");
		}

		public class DungeonSpecialEnemySetting
		{
			protected global::System.Int32 _id;
			protected global::System.Int32 _special_battle_id;
			protected global::System.Int32 _enemy_identify;
			protected global::System.Int32 _disp_order;
			protected global::System.Boolean _must_kill_flag;
			protected global::System.Double _detail_offset_x;
			protected global::System.Double _detail_offset_y;
			protected global::System.Double _detail_scale;

			public DungeonSpecialEnemySetting(int id = 0, int special_battle_id = 0, int enemy_identify = 0, int disp_order = 0, bool must_kill_flag = false, double detail_offset_x = 0, double detail_offset_y = 0, double detail_scale = 0)
			{
				_id = id;
				_special_battle_id = special_battle_id;
				_enemy_identify = enemy_identify;
				_disp_order = disp_order;
				_must_kill_flag = must_kill_flag;
				_detail_offset_x = detail_offset_x;
				_detail_offset_y = detail_offset_y;
				_detail_scale = detail_scale;
			}
		}
	}
}
