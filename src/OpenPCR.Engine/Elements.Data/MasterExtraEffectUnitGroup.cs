using global::OpenPCR.DbStubs;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterExtraEffectUnitGroup : AbstractMasterData
	{
		public class ExtraEffectUnitGroup
		{
			protected global::System.Int32 _target_id;
			protected global::System.Int32 _group_id;
			protected global::System.Int32 _unit_id;
			[global::OpenPCR.ImplicitMigration("44f387ba-6e98-4993-b0d9-4bfd9aa5764f")]
			public global::System.Int32 unit_id => _unit_id;

			public ExtraEffectUnitGroup(int target_id = 0, int group_id = 0, int unit_id = 0)
			{
				_target_id = target_id;
				_group_id = group_id;
				_unit_id = unit_id;
			}
		}
		private MasterUnitDatabase _db;
		private Dictionary<int, ExtraEffectUnitGroup> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<ExtraEffectUnitGroup>> _dictionaryOfListWithGroupId;
		public MasterExtraEffectUnitGroup(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ExtraEffectUnitGroup>();
			_dictionaryOfListWithGroupId = new Dictionary<int, List<ExtraEffectUnitGroup>>();
			_db = db;
		}

		public ExtraEffectUnitGroup Get(int target_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(target_id, out ExtraEffectUnitGroup value))
			{
				value = _db != null ? _SelectOne(target_id) : null;
				_lazyPrimaryKeyDictionary[target_id] = value;
			}

			return value;
		}

		private ExtraEffectUnitGroup _SelectOne(int target_id)
		{
			PreparedQuery query = _db.GetSelectQuery_ExtraEffectUnitGroup();
			if (query == null)
				return null;
			ExtraEffectUnitGroup value = null;
			if (query.BindInt(1, target_id))
			{
				if (query.Step())
					value = _CreateCachedOrmByQueryResult(query);
				query.Reset();
			}

			return value;
		}

		[global::OpenPCR.Migration("43f09597-ef55-44bc-8e8c-8b6afe2aea8f")]
		private List<ExtraEffectUnitGroup> _ListSelectWithGroupId(int group_id)
		{
			var values = new List<ExtraEffectUnitGroup>();
			PreparedQuery query = _db.GetSelectQueryWithIndex_ExtraEffectUnitGroup_GroupId();
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

		private ExtraEffectUnitGroup _CreateCachedOrmByQueryResult(Query query)
		{
			int id = query.GetInt(0);
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out ExtraEffectUnitGroup value))
			{
				int group = query.GetInt(1);
				int target = query.GetInt(2);
				value = new ExtraEffectUnitGroup(id, group, target);
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
