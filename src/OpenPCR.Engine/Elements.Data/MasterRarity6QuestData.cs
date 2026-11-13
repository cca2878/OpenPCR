using global::OpenPCR.DbStubs;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;
namespace Elements.Data
{
	public class MasterRarity6QuestData : AbstractMasterData
	{
		public class Rarity6QuestData
		{

			private global::OpenPCR.Runtime.Api.Vector2[] enemyPositions = new global::OpenPCR.Runtime.Api.Vector2[5];

			private float[] enemySizes = new float[5];

			private int[] rewardImages = new int[5];
			private int[] rewardNums = new int[5];
			protected global::System.Int32 _rarity_6_quest_id;

			protected global::System.Int32 _unit_id;

			protected global::System.String _quest_name;

			protected global::System.Int32 _limit_time;

			protected global::System.Int32 _recommended_level;

			protected global::System.Int32 _reward_group_id;

			protected global::System.Int32 _treasure_type;

			protected global::System.Int32 _reward_image_1;

			protected global::System.Int32 _reward_count_1;

			protected global::System.Int32 _reward_image_2;

			protected global::System.Int32 _reward_count_2;

			protected global::System.Int32 _reward_image_3;

			protected global::System.Int32 _reward_count_3;

			protected global::System.Int32 _reward_image_4;

			protected global::System.Int32 _reward_count_4;

			protected global::System.Int32 _reward_image_5;

			protected global::System.Int32 _reward_count_5;

			protected global::System.Int32 _background;

			protected global::System.Int32 _bg_position;

			protected global::System.Int32 _wave_group_id;

			protected global::System.Int32 _enemy_position_x_1;

			protected global::System.Int32 _enemy_local_position_y_1;

			protected global::System.Double _enemy_size_1;

			protected global::System.Int32 _enemy_position_x_2;

			protected global::System.Int32 _enemy_local_position_y_2;

			protected global::System.Double _enemy_size_2;

			protected global::System.Int32 _enemy_position_x_3;

			protected global::System.Int32 _enemy_local_position_y_3;

			protected global::System.Double _enemy_size_3;

			protected global::System.Int32 _enemy_position_x_4;

			protected global::System.Int32 _enemy_local_position_y_4;

			protected global::System.Double _enemy_size_4;

			protected global::System.Int32 _enemy_position_x_5;

			protected global::System.Int32 _enemy_local_position_y_5;

			protected global::System.Double _enemy_size_5;

			protected global::System.String _wave_bgm;

			[global::OpenPCR.ImplicitMigration("b1a0e1e3-c6d1-4dcc-a2a7-3ee83c77a755")]
			public global::System.Int32 unit_id => throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterRarity6QuestData\u002BRarity6QuestData::get_unit_id():System.Int32 (outside first-phase rarity6-quest boundary)");

			public global::System.Int32 reward_image_1 => _reward_image_1;

			public global::System.Int32 reward_count_1 => _reward_count_1;

			public global::System.Int32 reward_image_2 => _reward_image_2;

			public global::System.Int32 reward_count_2 => _reward_count_2;

			public global::System.Int32 reward_image_3 => _reward_image_3;

			public global::System.Int32 reward_count_3 => _reward_count_3;

			public global::System.Int32 reward_image_4 => _reward_image_4;

			public global::System.Int32 reward_count_4 => _reward_count_4;

			public global::System.Int32 reward_image_5 => _reward_image_5;

			public global::System.Int32 reward_count_5 => _reward_count_5;

			public global::System.Int32 enemy_position_x_1 => _enemy_position_x_1;

			public global::System.Int32 enemy_local_position_y_1 => _enemy_local_position_y_1;

			public global::System.Double enemy_size_1 => _enemy_size_1;

			public global::System.Int32 enemy_position_x_2 => _enemy_position_x_2;

			public global::System.Int32 enemy_local_position_y_2 => _enemy_local_position_y_2;

			public global::System.Double enemy_size_2 => _enemy_size_2;

			public global::System.Int32 enemy_position_x_3 => _enemy_position_x_3;

			public global::System.Int32 enemy_local_position_y_3 => _enemy_local_position_y_3;

			public global::System.Double enemy_size_3 => _enemy_size_3;

			public global::System.Int32 enemy_position_x_4 => _enemy_position_x_4;

			public global::System.Int32 enemy_local_position_y_4 => _enemy_local_position_y_4;

			public global::System.Double enemy_size_4 => _enemy_size_4;

			public global::System.Int32 enemy_position_x_5 => _enemy_position_x_5;

			public global::System.Int32 enemy_local_position_y_5 => _enemy_local_position_y_5;

			public global::System.Double enemy_size_5 => _enemy_size_5;

			public Rarity6QuestData(int rarity_6_quest_id = 0, int unit_id = 0, string quest_name = "", int limit_time = 0, int recommended_level = 0, int reward_group_id = 0, int treasure_type = 0, int reward_image_1 = 0, int reward_count_1 = 0, int reward_image_2 = 0, int reward_count_2 = 0, int reward_image_3 = 0, int reward_count_3 = 0, int reward_image_4 = 0, int reward_count_4 = 0, int reward_image_5 = 0, int reward_count_5 = 0, int background = 0, int bg_position = 0, int wave_group_id = 0, int enemy_position_x_1 = 0, int enemy_local_position_y_1 = 0, double enemy_size_1 = 0.0, int enemy_position_x_2 = 0, int enemy_local_position_y_2 = 0, double enemy_size_2 = 0.0, int enemy_position_x_3 = 0, int enemy_local_position_y_3 = 0, double enemy_size_3 = 0.0, int enemy_position_x_4 = 0, int enemy_local_position_y_4 = 0, double enemy_size_4 = 0.0, int enemy_position_x_5 = 0, int enemy_local_position_y_5 = 0, double enemy_size_5 = 0.0, string wave_bgm = "")
			{
				_rarity_6_quest_id = rarity_6_quest_id;
				_unit_id = unit_id;
				_quest_name = quest_name;
				_limit_time = limit_time;
				_recommended_level = recommended_level;
				_reward_group_id = reward_group_id;
				_treasure_type = treasure_type;
				_reward_image_1 = reward_image_1;
				_reward_count_1 = reward_count_1;
				_reward_image_2 = reward_image_2;
				_reward_count_2 = reward_count_2;
				_reward_image_3 = reward_image_3;
				_reward_count_3 = reward_count_3;
				_reward_image_4 = reward_image_4;
				_reward_count_4 = reward_count_4;
				_reward_image_5 = reward_image_5;
				_reward_count_5 = reward_count_5;
				_background = background;
				_bg_position = bg_position;
				_wave_group_id = wave_group_id;
				_enemy_position_x_1 = enemy_position_x_1;
				_enemy_local_position_y_1 = enemy_local_position_y_1;
				_enemy_size_1 = enemy_size_1;
				_enemy_position_x_2 = enemy_position_x_2;
				_enemy_local_position_y_2 = enemy_local_position_y_2;
				_enemy_size_2 = enemy_size_2;
				_enemy_position_x_3 = enemy_position_x_3;
				_enemy_local_position_y_3 = enemy_local_position_y_3;
				_enemy_size_3 = enemy_size_3;
				_enemy_position_x_4 = enemy_position_x_4;
				_enemy_local_position_y_4 = enemy_local_position_y_4;
				_enemy_size_4 = enemy_size_4;
				_enemy_position_x_5 = enemy_position_x_5;
				_enemy_local_position_y_5 = enemy_local_position_y_5;
				_enemy_size_5 = enemy_size_5;
				_wave_bgm = wave_bgm;
				SetUp();
			}

			public void SetUp()
			{
				enemyPositions[0] = new global::OpenPCR.Runtime.Api.Vector2((int)enemy_position_x_1, (int)enemy_local_position_y_1);
				enemyPositions[1] = new global::OpenPCR.Runtime.Api.Vector2((int)enemy_position_x_2, (int)enemy_local_position_y_2);
				enemyPositions[2] = new global::OpenPCR.Runtime.Api.Vector2((int)enemy_position_x_3, (int)enemy_local_position_y_3);
				enemyPositions[3] = new global::OpenPCR.Runtime.Api.Vector2((int)enemy_position_x_4, (int)enemy_local_position_y_4);
				enemyPositions[4] = new global::OpenPCR.Runtime.Api.Vector2((int)enemy_position_x_5, (int)enemy_local_position_y_5);
				enemySizes[0] = (float)(double)enemy_size_1;
				enemySizes[1] = (float)(double)enemy_size_2;
				enemySizes[2] = (float)(double)enemy_size_3;
				enemySizes[3] = (float)(double)enemy_size_4;
				enemySizes[4] = (float)(double)enemy_size_5;
				rewardImages[0] = reward_image_1;
				rewardImages[1] = reward_image_2;
				rewardImages[2] = reward_image_3;
				rewardImages[3] = reward_image_4;
				rewardImages[4] = reward_image_5;
				rewardNums[0] = reward_count_1;
				rewardNums[1] = reward_count_2;
				rewardNums[2] = reward_count_3;
				rewardNums[3] = reward_count_4;
				rewardNums[4] = reward_count_5;
			}
		}

		private global::Elements.Data.MasterQuestDatabase _db;

		private global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterRarity6QuestData.Rarity6QuestData> _lazyPrimaryKeyDictionary;

		private global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterRarity6QuestData.Rarity6QuestData> _dictionaryOfOrmWithRarity6QuestId;

		public MasterRarity6QuestData(global::Elements.Data.MasterQuestDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, Rarity6QuestData>();
			_dictionaryOfOrmWithRarity6QuestId = new Dictionary<int, Rarity6QuestData>();
			_db = db;
		}

		public global::Elements.Data.MasterRarity6QuestData.Rarity6QuestData Get(int unit_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(unit_id, out Rarity6QuestData row))
			{
				row = _db != null ? _SelectOne(unit_id) : null;
				_lazyPrimaryKeyDictionary[unit_id] = row;
			}
			return row;
		}

		private global::Elements.Data.MasterRarity6QuestData.Rarity6QuestData _SelectOne(int unit_id)
		{
			PreparedQuery query = _db.GetSelectQuery_Rarity6QuestData();
			if (query == null) return null;
			Rarity6QuestData row = null;
			if (query.BindInt(1, unit_id)) { if (query.Step()) row = _CreateCachedOrmByQueryResult(query); query.Reset(); }
			return row;
		}

		[global::OpenPCR.Migration("8e59eb51-e3c0-471d-9af9-80ddcdd8607b")]
		public global::Elements.Data.MasterRarity6QuestData.Rarity6QuestData GetWithRarity6QuestId(int rarity_6_quest_id)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterRarity6QuestData::GetWithRarity6QuestId(System.Int32):Elements.Data.MasterRarity6QuestData\u002BRarity6QuestData (outside first-phase rarity6-quest boundary)"); }

		[global::OpenPCR.Migration("10811034-0ec1-4f2b-a91a-841f53ac37a2")]
		private global::Elements.Data.MasterRarity6QuestData.Rarity6QuestData _SelectWithRarity6QuestId(int rarity_6_quest_id)
		{
			PreparedQuery query = _db.GetSelectQueryWithIndex_Rarity6QuestData_Rarity6QuestId();
			if (query == null) return null;
			Rarity6QuestData row = null;
			if (query.BindInt(1, rarity_6_quest_id)) { if (query.Step()) row = _CreateCachedOrmByQueryResult(query); query.Reset(); }
			return row;
		}

		private global::Elements.Data.MasterRarity6QuestData.Rarity6QuestData _CreateCachedOrmByQueryResult(global::OpenPCR.DbStubs.Query query)
		{
			int keyValue = query.GetInt(1);
			if (!_lazyPrimaryKeyDictionary.TryGetValue(keyValue, out Rarity6QuestData value))
			{
				int argument0 = query.GetInt(0);
				string argument2 = query.GetText(2);
				int argument3 = query.GetInt(3);
				int argument4 = query.GetInt(4);
				int argument5 = query.GetInt(5);
				int argument6 = query.GetInt(6);
				int argument7 = query.GetInt(7);
				int argument8 = query.GetInt(8);
				int argument9 = query.GetInt(9);
				int argument10 = query.GetInt(10);
				int argument11 = query.GetInt(11);
				int argument12 = query.GetInt(12);
				int argument13 = query.GetInt(13);
				int argument14 = query.GetInt(14);
				int argument15 = query.GetInt(15);
				int argument16 = query.GetInt(16);
				int argument17 = query.GetInt(17);
				int argument18 = query.GetInt(18);
				int argument19 = query.GetInt(19);
				int argument20 = query.GetInt(20);
				int argument21 = query.GetInt(21);
				double argument22 = query.GetDouble(22);
				int argument23 = query.GetInt(23);
				int argument24 = query.GetInt(24);
				double argument25 = query.GetDouble(25);
				int argument26 = query.GetInt(26);
				int argument27 = query.GetInt(27);
				double argument28 = query.GetDouble(28);
				int argument29 = query.GetInt(29);
				int argument30 = query.GetInt(30);
				double argument31 = query.GetDouble(31);
				int argument32 = query.GetInt(32);
				int argument33 = query.GetInt(33);
				double argument34 = query.GetDouble(34);
				string argument35 = query.GetText(35);
				value = new Rarity6QuestData(argument0, keyValue, argument2, argument3, argument4, argument5, argument6, argument7, argument8, argument9, argument10, argument11, argument12, argument13, argument14, argument15, argument16, argument17, argument18, argument19, argument20, argument21, argument22, argument23, argument24, argument25, argument26, argument27, argument28, argument29, argument30, argument31, argument32, argument33, argument34, argument35);
				_lazyPrimaryKeyDictionary.Add(keyValue, value);
			}
			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfOrmWithRarity6QuestId = null;
		}
	}
}
