using global::OpenPCR.DbStubs;
using System.Collections.Generic;
using System.Linq;

namespace Elements.Data
{
	public class MasterUnitRoleData : AbstractMasterData
	{
		public class UnitRoleData
		{
			protected global::System.Int32 _unit_id;
			protected global::System.Byte _unit_role_id;
			public global::System.Int32 unit_id => _unit_id;
			[global::OpenPCR.ImplicitMigration("0461c59e-b716-4023-9a21-b751757b1639")]
			public global::System.Byte unit_role_id => _unit_role_id;

			public UnitRoleData(int unit_id = 0, byte unit_role_id = 0)
			{
				_unit_id = unit_id;
				_unit_role_id = unit_role_id;
			}
		}

		private MasterUnitRoleDatabase _db;
		private Dictionary<int, UnitRoleData> _lazyPrimaryKeyDictionary;
		public MasterUnitRoleData(MasterUnitRoleDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, UnitRoleData>();
			_db = db;
		}

		[global::OpenPCR.Migration("2a486472-995c-4734-b386-6b7004af91a8")]
		public UnitRoleData Get(int unit_id)
		{
			UnitRoleData value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(unit_id, out value))
			{
				value = _db != null ? _SelectOne(unit_id) : null;
				_lazyPrimaryKeyDictionary[unit_id] = value;
			}

			return value;
		}

		[global::OpenPCR.Migration("2e71f11c-5e47-422a-94d1-8c9a3ee9817b")]
		private UnitRoleData _SelectOne(int unit_id)
		{
			PreparedQuery query = _db.GetSelectQuery_UnitRoleData();
			if (query == null)
				return null;
			if (!query.BindInt(1, unit_id))
				return null;
			UnitRoleData result = null;
			if (query.Step())
				result = _CreateCachedOrmByQueryResult(query);
			query.Reset();
			return result;
		}

		private UnitRoleData _CreateCachedOrmByQueryResult(Query query)
		{
			int key = query.GetInt(0);
			UnitRoleData value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out value))
			{
				byte unit_role_id = unchecked((byte)query.GetInt(1));
				value = new UnitRoleData(key, unit_role_id);
				_lazyPrimaryKeyDictionary.Add(key, value);
			}

			return value;
		}

		[global::OpenPCR.Migration("a95a50e8-10f6-4deb-b3db-ba649763aa34")]
		public int GetOrInvalidRoleId(int _unitId)
		{
			var data = Get(_unitId);
			return data != null ? (byte)data.unit_role_id : -1;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
