using System.Collections.Generic;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterHatsuneSpecialBattle : AbstractMasterData
	{
		public class HatsuneSpecialBattle
		{
			protected global::System.Int32 _event_id;
			protected global::System.Int32 _mode;
			protected global::System.Int32 _recommended_level;
			protected global::System.Int32 _purpose_type;
			protected global::System.Int32 _purpose_count;
			protected global::System.Int32 _trigger_hp;
			protected global::System.Int32 _story_id_mode_start;
			protected global::System.Int32 _story_id_mode_end;
			protected global::System.Int32 _wave_group_id;
			protected global::System.Int32 _unnecessary_defeat_chara;
			protected global::System.Double _story_start_second;
			protected global::System.Double _action_start_second;
			protected global::System.Int32 _hp_gauge_color_flag;
			protected global::System.Int32 _start_idle_trigger;
			protected global::System.Double _appear_time;
			protected global::System.Double _detail_boss_bg_size;
			protected global::System.Double _detail_boss_bg_height;
			protected global::System.String _detail_boss_motion;
			protected global::System.Boolean _is_hide_boss;
			public global::System.Int32 purpose_count => _purpose_count;
			public global::System.Double action_start_second => _action_start_second;
			public global::System.Double appear_time => _appear_time;
			public global::System.Int32 start_idle_trigger => _start_idle_trigger;

			public HatsuneSpecialBattle(int event_id = 0, int mode = 0, int recommended_level = 0, int purpose_type = 0, int purpose_count = 0, int trigger_hp = 0, int story_id_mode_start = 0, int story_id_mode_end = 0, int wave_group_id = 0, int unnecessary_defeat_chara = 0, double story_start_second = 0.0, double action_start_second = 0.0, int hp_gauge_color_flag = 0, int start_idle_trigger = 0, double appear_time = 0.0, double detail_boss_bg_size = 0.0, double detail_boss_bg_height = 0.0, string detail_boss_motion = "", bool is_hide_boss = false)
			{
				_event_id = event_id;
				_mode = mode;
				_recommended_level = recommended_level;
				_purpose_type = purpose_type;
				_purpose_count = purpose_count;
				_trigger_hp = trigger_hp;
				_story_id_mode_start = story_id_mode_start;
				_story_id_mode_end = story_id_mode_end;
				_wave_group_id = wave_group_id;
				_unnecessary_defeat_chara = unnecessary_defeat_chara;
				_story_start_second = story_start_second;
				_action_start_second = action_start_second;
				_hp_gauge_color_flag = hp_gauge_color_flag;
				_start_idle_trigger = start_idle_trigger;
				_appear_time = appear_time;
				_detail_boss_bg_size = detail_boss_bg_size;
				_detail_boss_bg_height = detail_boss_bg_height;
				_detail_boss_motion = detail_boss_motion;
				_is_hide_boss = is_hide_boss;
			}
		}

		private MasterHatsuneDatabase _db;
		private Dictionary<ulong, HatsuneSpecialBattle> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<HatsuneSpecialBattle>> _dictionaryOfListWithEventId;
		private Dictionary<int, List<HatsuneSpecialBattle>> _dictionaryOfListWithWaveGroupId;
		public MasterHatsuneSpecialBattle(MasterHatsuneDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<ulong, HatsuneSpecialBattle>();
			_dictionaryOfListWithEventId = new Dictionary<int, List<HatsuneSpecialBattle>>();
			_dictionaryOfListWithWaveGroupId = new Dictionary<int, List<HatsuneSpecialBattle>>();
			_db = db;
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MasterHatsuneSpecialBattle::Unload()");
		}
	}
}
