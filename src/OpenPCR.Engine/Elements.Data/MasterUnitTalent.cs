using global::OpenPCR.DbStubs;
using System.Collections.Generic;
using System.Linq;

namespace Elements.Data
{
	public class MasterUnitTalent : AbstractMasterData
	{
		public class UnitTalent
		{
			protected global::System.Int32 _setting_id;
			protected global::System.Int32 _unit_id;
			protected global::System.Int32 _talent_id;
			public global::System.Int32 unit_id => _unit_id;
			[global::OpenPCR.ImplicitMigration("efe12298-e5ee-4283-a43f-8ede184945f8")]
			public global::System.Int32 talent_id => _talent_id;

			public UnitTalent(int setting_id = 0, int unit_id = 0, int talent_id = 0)
			{
				_setting_id = setting_id;
				_unit_id = unit_id;
				_talent_id = talent_id;
			}
		}

		private MasterTalentDatabase _db;
		private Dictionary<int, UnitTalent> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<UnitTalent>> _dictionaryOfListWithUnitId;
		public MasterUnitTalent(MasterTalentDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, UnitTalent>();
			_dictionaryOfListWithUnitId = new Dictionary<int, List<UnitTalent>>();
			_db = db;
		}

		public UnitTalent Get(int setting_id)
		{
			UnitTalent value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(setting_id, out value))
			{
				value = _db != null ? _SelectOne(setting_id) : null;
				_lazyPrimaryKeyDictionary[setting_id] = value;
			}

			return value;
		}

		private UnitTalent _SelectOne(int setting_id)
		{
			PreparedQuery query = _db.GetSelectQuery_UnitTalent();
			if (query == null)
				return null;
			if (!query.BindInt(1, setting_id))
				return null;
			UnitTalent result = null;
			if (query.Step())
				result = _CreateCachedOrmByQueryResult(query);
			query.Reset();
			return result;
		}

		private UnitTalent _CreateCachedOrmByQueryResult(Query query)
		{
			int key = query.GetInt(0);
			UnitTalent value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out value))
			{
				int unit_id = query.GetInt(1);
				int talent_id = query.GetInt(2);
				value = new UnitTalent(key, unit_id, talent_id);
				_lazyPrimaryKeyDictionary.Add(key, value);
			}

			return value;
		}

		[global::OpenPCR.Migration("6e90b9bc-f5c5-471f-b3da-e3781694391a")]
		public List<int> GetTalentIdsAsc(int _unitId)
		{
			return GetListWithUnitIdOrderByTalentIdAsc(_unitId).Select(value => (int)value.talent_id).ToList();
		}

		[global::OpenPCR.Migration("c72689a7-9662-46c6-94c1-23112b45017b")]
		public List<UnitTalent> GetListWithUnitIdOrderByTalentIdAsc(int unit_id)
		{
			if (!_dictionaryOfListWithUnitId.TryGetValue(unit_id, out var value))
			{
				value = _ListSelectWithUnitIdOrderByTalentIdAsc(unit_id);
				_dictionaryOfListWithUnitId.Add(unit_id, value);
			}

			return value;
		}

		[global::OpenPCR.Migration("79b9ce26-793a-4d97-b81c-f9adebcd1e66")]
		private List<UnitTalent> _ListSelectWithUnitIdOrderByTalentIdAsc(int unit_id)
		{
			List<UnitTalent> result = new List<UnitTalent>();
			PreparedQuery query = _db.GetSelectQueryWithIndex_UnitTalent_UnitId();
			if (query != null)
			{
				if (!query.BindInt(1, unit_id))
					return null;
				while (query.Step())
					result.Add(_CreateCachedOrmByQueryResult(query));
				query.Reset();
			}

			return result;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithUnitId = null;
		}
	}
}
