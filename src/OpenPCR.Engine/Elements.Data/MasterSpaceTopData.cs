using System.Collections.Generic;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterSpaceTopData : AbstractMasterData
	{
		public class SpaceTopData
		{
			protected global::System.Int32 _id;
			protected global::System.Int32 _space_id;
			protected global::System.Int32 _space_battle_id;
			protected global::System.Int32 _part_flag;
			protected global::System.Int32 _story_id;
			protected global::System.String _time_from;
			protected global::System.String _time_to;
			protected global::System.String _skip_battle_time;
			protected global::System.String _name;

			public SpaceTopData(int id = 0, int space_id = 0, int space_battle_id = 0, int part_flag = 0, int story_id = 0, string time_from = "", string time_to = "", string skip_battle_time = "", string name = "")
			{
				_id = id;
				_space_id = space_id;
				_space_battle_id = space_battle_id;
				_part_flag = part_flag;
				_story_id = story_id;
				_time_from = time_from;
				_time_to = time_to;
				_skip_battle_time = skip_battle_time;
				_name = name;
			}
		}
		private MasterSpaceDatabase _db;
		private Dictionary<int, SpaceTopData> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<SpaceTopData>> _dictionaryOfListWithSpaceId;
		private Dictionary<int, List<SpaceTopData>> _dictionaryOfListWithStoryId;
		public MasterSpaceTopData(MasterSpaceDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, SpaceTopData>();
			_dictionaryOfListWithSpaceId = new Dictionary<int, List<SpaceTopData>>();
			_dictionaryOfListWithStoryId = new Dictionary<int, List<SpaceTopData>>();
			_db = db;
		}

		public override void Unload()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.MasterSpaceTopData::Unload()");
		}
	}
}
