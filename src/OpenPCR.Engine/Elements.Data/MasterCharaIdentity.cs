using global::OpenPCR.DbStubs;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterCharaIdentity : AbstractMasterData
	{
		public class CharaIdentity
		{
			protected global::System.Int32 _unit_id;
			protected global::System.Int32 _chara_type;
			protected global::System.Int32 _chara_type_2;
			protected global::System.Int32 _chara_type_3;
			public global::System.Int32 unit_id => _unit_id;

			public CharaIdentity(int unit_id = 0, int chara_type = 0, int chara_type_2 = 0, int chara_type_3 = 0)
			{
				_unit_id = unit_id;
				_chara_type = chara_type;
				_chara_type_2 = chara_type_2;
				_chara_type_3 = chara_type_3;
			}
		}
		private MasterUnitDatabase _db;
		private Dictionary<int, CharaIdentity> _strictPrimaryKeyDictionary;
		private Dictionary<int, CharaIdentity> _lazyPrimaryKeyDictionary;
		public MasterCharaIdentity(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, CharaIdentity>();
			_db = db;
		}

		[global::OpenPCR.Migration("a27a1a0b-8f8b-4900-ab11-097c6b2f30a7")]
		public CharaIdentity Get(int unit_id)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterCharaIdentity::Get(System.Int32):Elements.Data.MasterCharaIdentity\u002BCharaIdentity (outside first-phase chara-groups boundary)"); }

		[global::OpenPCR.Migration("3e45650b-5221-439c-b093-9361584d77cd")]
		private CharaIdentity _SelectOne(int unit_id)
		{
			PreparedQuery query = _db.GetSelectQuery_CharaIdentity();
			if (query == null)
				return null;
			CharaIdentity value = null;
			if (query.BindInt(1, unit_id))
			{
				if (query.Step())
					value = _CreateCachedOrmByQueryResult(query);
				query.Reset();
			}

			return value;
		}

		private CharaIdentity _CreateCachedOrmByQueryResult(Query query)
		{
			int id = query.GetInt(0);
			CharaIdentity value = null;
			if (_strictPrimaryKeyDictionary != null)
				_strictPrimaryKeyDictionary.TryGetValue(id, out value);
			else if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out value))
			{
				int a = query.GetInt(1);
				int b = query.GetInt(2);
				int c = query.GetInt(3);
				value = new CharaIdentity(id, a, b, c);
				_lazyPrimaryKeyDictionary.Add(id, value);
			}

			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_strictPrimaryKeyDictionary = null;
		}
	}
}
