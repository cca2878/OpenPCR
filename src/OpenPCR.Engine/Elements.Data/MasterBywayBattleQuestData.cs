using global::OpenPCR.DbStubs;
using System.Collections.Generic;
using System.Linq;

namespace Elements.Data
{
	public class MasterBywayBattleQuestData : AbstractMasterData
	{
		public class BywayBattleQuestData
		{
			public List<int> WaveStartStoryIds { get; private set; }
			public List<int> WaveEndStoryIds { get; private set; }
			public List<int> WaveGroupIds { get; private set; }

			public void SetUp()
			{
				WaveStartStoryIds = new List<int>
				{
					story_id_wavestart_1,
					story_id_wavestart_2,
					story_id_wavestart_3
				};
				WaveEndStoryIds = new List<int>
				{
					story_id_waveend_1,
					story_id_waveend_2,
					story_id_waveend_3
				};
				WaveGroupIds = new List<int>
				{
					wave_group_id_1,
					wave_group_id_2,
					wave_group_id_3
				};
			}

			protected global::System.Int32 _quest_id;
			protected global::System.Int32 _auto_restrict_type;
			protected global::System.Int32 _limit_time;
			protected global::System.Int32 _background_1;
			protected global::System.Int32 _wave_group_id_1;
			protected global::System.String _wave_bgm_sheet_id_1;
			protected global::System.String _wave_bgm_que_id_1;
			protected global::System.Int32 _story_id_wavestart_1;
			protected global::System.Int32 _story_id_waveend_1;
			protected global::System.Int32 _background_2;
			protected global::System.Int32 _wave_group_id_2;
			protected global::System.String _wave_bgm_sheet_id_2;
			protected global::System.String _wave_bgm_que_id_2;
			protected global::System.Int32 _story_id_wavestart_2;
			protected global::System.Int32 _story_id_waveend_2;
			protected global::System.Int32 _background_3;
			protected global::System.Int32 _wave_group_id_3;
			protected global::System.String _wave_bgm_sheet_id_3;
			protected global::System.String _wave_bgm_que_id_3;
			protected global::System.Int32 _story_id_wavestart_3;
			protected global::System.Int32 _story_id_waveend_3;
			protected global::System.Int32 _enemy_image_1;
			protected global::System.Int32 _enemy_image_2;
			protected global::System.Int32 _enemy_image_3;
			protected global::System.Int32 _enemy_image_4;
			protected global::System.Int32 _enemy_image_5;
			public global::System.Int32 auto_restrict_type => _auto_restrict_type;
			public global::System.Int32 story_id_wavestart_1 => _story_id_wavestart_1;
			public global::System.Int32 story_id_wavestart_2 => _story_id_wavestart_2;
			public global::System.Int32 story_id_wavestart_3 => _story_id_wavestart_3;
			public global::System.Int32 story_id_waveend_1 => _story_id_waveend_1;
			public global::System.Int32 story_id_waveend_2 => _story_id_waveend_2;
			public global::System.Int32 story_id_waveend_3 => _story_id_waveend_3;
			public global::System.Int32 wave_group_id_1 => _wave_group_id_1;
			public global::System.Int32 wave_group_id_2 => _wave_group_id_2;
			public global::System.Int32 wave_group_id_3 => _wave_group_id_3;

			public BywayBattleQuestData(int quest_id = 0, int auto_restrict_type = 0, int limit_time = 0, int background_1 = 0, int wave_group_id_1 = 0, string wave_bgm_sheet_id_1 = "", string wave_bgm_que_id_1 = "", int story_id_wavestart_1 = 0, int story_id_waveend_1 = 0, int background_2 = 0, int wave_group_id_2 = 0, string wave_bgm_sheet_id_2 = "", string wave_bgm_que_id_2 = "", int story_id_wavestart_2 = 0, int story_id_waveend_2 = 0, int background_3 = 0, int wave_group_id_3 = 0, string wave_bgm_sheet_id_3 = "", string wave_bgm_que_id_3 = "", int story_id_wavestart_3 = 0, int story_id_waveend_3 = 0, int enemy_image_1 = 0, int enemy_image_2 = 0, int enemy_image_3 = 0, int enemy_image_4 = 0, int enemy_image_5 = 0)
			{
				_quest_id = quest_id;
				_auto_restrict_type = auto_restrict_type;
				_limit_time = limit_time;
				_background_1 = background_1;
				_wave_group_id_1 = wave_group_id_1;
				_wave_bgm_sheet_id_1 = wave_bgm_sheet_id_1;
				_wave_bgm_que_id_1 = wave_bgm_que_id_1;
				_story_id_wavestart_1 = story_id_wavestart_1;
				_story_id_waveend_1 = story_id_waveend_1;
				_background_2 = background_2;
				_wave_group_id_2 = wave_group_id_2;
				_wave_bgm_sheet_id_2 = wave_bgm_sheet_id_2;
				_wave_bgm_que_id_2 = wave_bgm_que_id_2;
				_story_id_wavestart_2 = story_id_wavestart_2;
				_story_id_waveend_2 = story_id_waveend_2;
				_background_3 = background_3;
				_wave_group_id_3 = wave_group_id_3;
				_wave_bgm_sheet_id_3 = wave_bgm_sheet_id_3;
				_wave_bgm_que_id_3 = wave_bgm_que_id_3;
				_story_id_wavestart_3 = story_id_wavestart_3;
				_story_id_waveend_3 = story_id_waveend_3;
				_enemy_image_1 = enemy_image_1;
				_enemy_image_2 = enemy_image_2;
				_enemy_image_3 = enemy_image_3;
				_enemy_image_4 = enemy_image_4;
				_enemy_image_5 = enemy_image_5;
				SetUp();
			}
		}

		private MasterBywayQuestDatabase _db;
		private Dictionary<int, BywayBattleQuestData> _lazyPrimaryKeyDictionary;
		public MasterBywayBattleQuestData(MasterBywayQuestDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, BywayBattleQuestData>();
			_db = db;
		}

		public BywayBattleQuestData Get(int quest_id)
		{
			BywayBattleQuestData value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(quest_id, out value))
			{
				value = _db != null ? _SelectOne(quest_id) : null;
				_lazyPrimaryKeyDictionary[quest_id] = value;
			}

			return value;
		}

		private BywayBattleQuestData _SelectOne(int quest_id)
		{
			PreparedQuery query = _db.GetSelectQuery_BywayBattleQuestData();
			if (query == null)
				return null;
			if (!query.BindInt(1, quest_id))
				return null;
			BywayBattleQuestData result = null;
			if (query.Step())
				result = _CreateCachedOrmByQueryResult(query);
			query.Reset();
			return result;
		}

		private BywayBattleQuestData _CreateCachedOrmByQueryResult(Query query)
		{
			int key = query.GetInt(0);
			BywayBattleQuestData value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out value))
			{
				int auto_restrict_type = query.GetInt(1);
				int limit_time = query.GetInt(2);
				int background_1 = query.GetInt(3);
				int wave_group_id_1 = query.GetInt(4);
				string wave_bgm_sheet_id_1 = query.GetText(5);
				string wave_bgm_que_id_1 = query.GetText(6);
				int story_id_wavestart_1 = query.GetInt(7);
				int story_id_waveend_1 = query.GetInt(8);
				int background_2 = query.GetInt(9);
				int wave_group_id_2 = query.GetInt(10);
				string wave_bgm_sheet_id_2 = query.GetText(11);
				string wave_bgm_que_id_2 = query.GetText(12);
				int story_id_wavestart_2 = query.GetInt(13);
				int story_id_waveend_2 = query.GetInt(14);
				int background_3 = query.GetInt(15);
				int wave_group_id_3 = query.GetInt(16);
				string wave_bgm_sheet_id_3 = query.GetText(17);
				string wave_bgm_que_id_3 = query.GetText(18);
				int story_id_wavestart_3 = query.GetInt(19);
				int story_id_waveend_3 = query.GetInt(20);
				int enemy_image_1 = query.GetInt(21);
				int enemy_image_2 = query.GetInt(22);
				int enemy_image_3 = query.GetInt(23);
				int enemy_image_4 = query.GetInt(24);
				int enemy_image_5 = query.GetInt(25);
				value = new BywayBattleQuestData(key, auto_restrict_type, limit_time, background_1, wave_group_id_1, wave_bgm_sheet_id_1, wave_bgm_que_id_1, story_id_wavestart_1, story_id_waveend_1, background_2, wave_group_id_2, wave_bgm_sheet_id_2, wave_bgm_que_id_2, story_id_wavestart_2, story_id_waveend_2, background_3, wave_group_id_3, wave_bgm_sheet_id_3, wave_bgm_que_id_3, story_id_wavestart_3, story_id_waveend_3, enemy_image_1, enemy_image_2, enemy_image_3, enemy_image_4, enemy_image_5);
				_lazyPrimaryKeyDictionary.Add(key, value);
			}

			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
