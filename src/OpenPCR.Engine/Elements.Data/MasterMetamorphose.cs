using global::OpenPCR.DbStubs;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterMetamorphose : AbstractMasterData
	{
		public class Metamorphose
		{
			protected global::System.Int32 _type_id;
			protected global::System.Int32 _condition_value;
			protected global::System.Int32 _prefab_id;
			[global::OpenPCR.ImplicitMigration("c14db8c7-40cc-4960-a6b9-3316b1d6fc37")]
			public global::System.Int32 prefab_id => throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterMetamorphose\u002BMetamorphose::get_prefab_id():System.Int32 (outside first-phase toad-table boundary)");

			public Metamorphose(int type_id = 0, int condition_value = 0, int prefab_id = 0)
			{
				_type_id = type_id;
				_condition_value = condition_value;
				_prefab_id = prefab_id;
			}
		}
		private MasterSkillDatabase _db;
		private Dictionary<ulong, Metamorphose> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<Metamorphose>> _dictionaryOfListWithTypeId;
		public MasterMetamorphose(MasterSkillDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<ulong, Metamorphose>();
			_dictionaryOfListWithTypeId = new Dictionary<int, List<Metamorphose>>();
			_db = db;
		}
		public Metamorphose Get(ulong key) => Get(unchecked((int)key), unchecked((int)(key >> 32)));
		[global::OpenPCR.Migration("7864569b-46fc-4370-86e0-ea6cd00726fa")]
		public Metamorphose Get(int type_id, int condition_value)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterMetamorphose::Get(System.Int32,System.Int32):Elements.Data.MasterMetamorphose\u002BMetamorphose (outside first-phase toad-table boundary)"); }
		[global::OpenPCR.Migration("955d31aa-8302-4abd-8755-050f311e4046")]
		private Metamorphose _SelectOne(int type_id, int condition_value)
		{
			PreparedQuery query = _db.GetSelectQuery_Metamorphose();
			if (query == null)
				return null;
			Metamorphose value = null;
			if (query.BindInt(1, type_id) && query.BindInt(2, condition_value))
			{
				if (query.Step())
					value = _CreateCachedOrmByQueryResult(query);
				query.Reset();
			}

			return value;
		}

		[global::OpenPCR.Migration("d20edd62-b66a-427a-a49d-5ef616b1ad81")]
		private List<Metamorphose> _ListSelectWithTypeId(int type_id)
		{
			var values = new List<Metamorphose>();
			PreparedQuery query = _db.GetSelectQueryWithIndex_Metamorphose_TypeId();
			if (query != null)
			{
				if (!query.BindInt(1, type_id))
					return null;
				while (query.Step())
					values.Add(_CreateCachedOrmByQueryResult(query));
				query.Reset();
			}

			return values;
		}

		private Metamorphose _CreateCachedOrmByQueryResult(Query query)
		{
			int type = query.GetInt(0);
			int condition = query.GetInt(1);
			ulong key = unchecked((uint)type | ((ulong)(uint)condition << 32));
			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out Metamorphose value))
			{
				int prefab = query.GetInt(2);
				value = new Metamorphose(type, condition, prefab);
				_lazyPrimaryKeyDictionary.Add(key, value);
			}

			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithTypeId = null;
		}
	}
}
