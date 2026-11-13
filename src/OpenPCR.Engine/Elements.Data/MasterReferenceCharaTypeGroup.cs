using global::OpenPCR.DbStubs;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterReferenceCharaTypeGroup : AbstractMasterData
	{
		public class ReferenceCharaTypeGroup
		{
			protected global::System.Int32 _target_id;
			protected global::System.Int32 _group_id;
			protected global::System.Int32 _chara_type;

			public ReferenceCharaTypeGroup(int target_id = 0, int group_id = 0, int chara_type = 0)
			{
				_target_id = target_id;
				_group_id = group_id;
				_chara_type = chara_type;
			}
		}
		private MasterUnitDatabase _db;
		private Dictionary<int, ReferenceCharaTypeGroup> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<ReferenceCharaTypeGroup>> _dictionaryOfListWithGroupId;
		public MasterReferenceCharaTypeGroup(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ReferenceCharaTypeGroup>();
			_dictionaryOfListWithGroupId = new Dictionary<int, List<ReferenceCharaTypeGroup>>();
			_db = db;
		}

		public ReferenceCharaTypeGroup Get(int target_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(target_id, out ReferenceCharaTypeGroup value))
			{
				value = _db != null ? _SelectOne(target_id) : null;
				_lazyPrimaryKeyDictionary[target_id] = value;
			}

			return value;
		}

		private ReferenceCharaTypeGroup _SelectOne(int target_id)
		{
			PreparedQuery query = _db.GetSelectQuery_ReferenceCharaTypeGroup();
			if (query == null)
				return null;
			ReferenceCharaTypeGroup value = null;
			if (query.BindInt(1, target_id))
			{
				if (query.Step())
					value = _CreateCachedOrmByQueryResult(query);
				query.Reset();
			}

			return value;
		}

		[global::OpenPCR.Migration("0528f8e1-91b7-4bba-9c06-3c2a2f9dd412")]
		private List<ReferenceCharaTypeGroup> _ListSelectWithGroupId(int group_id)
		{
			var values = new List<ReferenceCharaTypeGroup>();
			PreparedQuery query = _db.GetSelectQueryWithIndex_ReferenceCharaTypeGroup_GroupId();
			if (query != null)
			{
				if (!query.BindInt(1, group_id))
					return null;
				while (query.Step())
					values.Add(_CreateCachedOrmByQueryResult(query));
				query.Reset();
			}

			return values;
		}

		private ReferenceCharaTypeGroup _CreateCachedOrmByQueryResult(Query query)
		{
			int id = query.GetInt(0);
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out ReferenceCharaTypeGroup value))
			{
				int group = query.GetInt(1);
				int target = query.GetInt(2);
				value = new ReferenceCharaTypeGroup(id, group, target);
				_lazyPrimaryKeyDictionary.Add(id, value);
			}

			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithGroupId = null;
		}
	}
}
