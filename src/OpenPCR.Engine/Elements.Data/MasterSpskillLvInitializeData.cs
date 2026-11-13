using global::OpenPCR.DbStubs;
using System.Collections.Generic;
using System.Linq;

namespace Elements.Data
{
	public class MasterSpskillLvInitializeData : AbstractMasterData
	{
		public class SpskillLvInitializeData
		{
			protected global::System.Int32 _initialize_skill_id;
			protected global::System.Int32 _base_skill_id;
			[global::OpenPCR.ImplicitMigration("aafa26dc-5a19-4918-a96e-c8370565691f")]
			public global::System.Int32 base_skill_id => _base_skill_id;

			public SpskillLvInitializeData(int initialize_skill_id = 0, int base_skill_id = 0)
			{
				_initialize_skill_id = initialize_skill_id;
				_base_skill_id = base_skill_id;
			}
		}

		private MasterSkillDatabase _db;
		private Dictionary<int, SpskillLvInitializeData> _lazyPrimaryKeyDictionary;
		public MasterSpskillLvInitializeData(MasterSkillDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, SpskillLvInitializeData>();
			_db = db;
		}

		[global::OpenPCR.Migration("85d4705f-0b9b-483d-b5db-bcfb6dfe4dec")]
		public SpskillLvInitializeData Get(int initialize_skill_id)
		{
			SpskillLvInitializeData value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(initialize_skill_id, out value))
			{
				value = _db != null ? _SelectOne(initialize_skill_id) : null;
				_lazyPrimaryKeyDictionary[initialize_skill_id] = value;
			}

			return value;
		}

		[global::OpenPCR.Migration("57a5c616-7fce-4489-8170-1111c26721d8")]
		private SpskillLvInitializeData _SelectOne(int initialize_skill_id)
		{
			PreparedQuery query = _db.GetSelectQuery_SpskillLvInitializeData();
			if (query == null)
				return null;
			if (!query.BindInt(1, initialize_skill_id))
				return null;
			SpskillLvInitializeData result = null;
			if (query.Step())
				result = _CreateCachedOrmByQueryResult(query);
			query.Reset();
			return result;
		}

		private SpskillLvInitializeData _CreateCachedOrmByQueryResult(Query query)
		{
			int key = query.GetInt(0);
			SpskillLvInitializeData value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out value))
			{
				int base_skill_id = query.GetInt(1);
				value = new SpskillLvInitializeData(key, base_skill_id);
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
