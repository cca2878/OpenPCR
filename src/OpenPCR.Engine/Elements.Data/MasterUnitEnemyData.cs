using System.Collections.Generic;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterUnitEnemyData : AbstractMasterData
	{
		private MasterUnitDatabase _db;
		private Dictionary<int, UnitEnemyData> _strictPrimaryKeyDictionary;
		private Dictionary<int, UnitEnemyData> _lazyPrimaryKeyDictionary;
		[global::OpenPCR.Migration("3a92b511-3eb1-4c06-852a-ce72c9bbb906")]
		public UnitEnemyData Get(int unit_id)
		{
			UnitEnemyData value = null;
			if (_strictPrimaryKeyDictionary != null)
			{
				_strictPrimaryKeyDictionary.TryGetValue(unit_id, out value);
				return value;
			}
			if (!_lazyPrimaryKeyDictionary.TryGetValue(unit_id, out value))
			{
				value = _db != null ? _SelectOne(unit_id) : null;
				_lazyPrimaryKeyDictionary[unit_id] = value;
			}
			return value;
		}
		[global::OpenPCR.Migration("05246c72-2bc7-48f2-9fbe-65d6a02ca50c")]
		private UnitEnemyData _SelectOne(int unit_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitEnemyData._SelectOne SQLite query production and ORM mapping");
		}
		public class UnitEnemyData
		{
			protected global::System.Int32 _unit_id;
			protected global::System.String _unit_name;
			protected global::System.Int32 _prefab_id;
			protected global::System.Int32 _motion_type;
			protected global::System.Int32 _se_type;
			protected global::System.Int32 _move_speed;
			protected global::System.Int32 _search_area_width;
			protected global::System.Int32 _atk_type;
			protected global::System.Double _normal_atk_cast_time;
			protected global::System.Int32 _cutin;
			protected global::System.Int32 _cutin_star6;
			protected global::System.Int32 _visual_change_flag;
			protected global::System.String _comment;

			public global::System.Int32 unit_id
			{
				get { return _unit_id; }
			}
			public global::System.String unit_name
			{
				get { return _unit_name; }
			}
			public global::System.Int32 prefab_id
			{
				get { return _prefab_id; }
			}
			public global::System.Int32 motion_type
			{
				get { return _motion_type; }
			}
			public global::System.Int32 se_type
			{
				get { return _se_type; }
			}
			public global::System.Int32 move_speed
			{
				get { return _move_speed; }
			}
			public global::System.Int32 search_area_width
			{
				get { return _search_area_width; }
			}
			public global::System.Int32 atk_type
			{
				get { return _atk_type; }
			}
			public global::System.Double normal_atk_cast_time
			{
				get { return _normal_atk_cast_time; }
			}
			public global::System.Int32 cutin
			{
				get { return _cutin; }
			}
			public global::System.Int32 cutin_star6
			{
				get { return _cutin_star6; }
			}
			public global::System.Int32 visual_change_flag
			{
				get { return _visual_change_flag; }
			}
			public global::System.String comment
			{
				get { return _comment; }
			}
			public UnitEnemyData(int unit_id = 0, string unit_name = "", int prefab_id = 0, int motion_type = 0, int se_type = 0, int move_speed = 0, int search_area_width = 0, int atk_type = 0, double normal_atk_cast_time = 0.0, int cutin = 0, int cutin_star6 = 0, int visual_change_flag = 0, string comment = "")
			{
				_unit_id = unit_id;
				_unit_name = unit_name;
				_prefab_id = prefab_id;
				_motion_type = motion_type;
				_se_type = se_type;
				_move_speed = move_speed;
				_search_area_width = search_area_width;
				_atk_type = atk_type;
				_normal_atk_cast_time = normal_atk_cast_time;
				_cutin = cutin;
				_cutin_star6 = cutin_star6;
				_visual_change_flag = visual_change_flag;
				_comment = comment;
			}
		}
		public MasterUnitEnemyData(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, UnitEnemyData>();
			_db = db;
		}
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}
	}

}
