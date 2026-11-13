using global::OpenPCR.DbStubs;
using System.Collections.Generic;
using System.Linq;
namespace Elements.Data
{
	public class MasterEnemyIgnoreSkillRf : AbstractMasterData
	{
		public class EnemyIgnoreSkillRf
		{
			protected global::System.Int32 _enemy_id;

			public global::System.Int32 enemy_id => _enemy_id;

			public EnemyIgnoreSkillRf(int enemy_id = 0)
			{
				_enemy_id = enemy_id;
			}
		}

		private global::Elements.Data.MasterEnemyDatabase _db;

		private global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterEnemyIgnoreSkillRf.EnemyIgnoreSkillRf> _lazyPrimaryKeyDictionary;

		public MasterEnemyIgnoreSkillRf(global::Elements.Data.MasterEnemyDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterEnemyIgnoreSkillRf.EnemyIgnoreSkillRf>();
			_db = db;
		}

		[global::OpenPCR.Migration("409db4d8-ad2c-4e3f-b0b2-73b2562c97cb")]
		public global::Elements.Data.MasterEnemyIgnoreSkillRf.EnemyIgnoreSkillRf Get(int enemy_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(enemy_id, out EnemyIgnoreSkillRf value))
			{
				value = _db != null ? _SelectOne(enemy_id) : null;
				_lazyPrimaryKeyDictionary[enemy_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("1b0ca238-55d0-43dc-85db-4c0eed19a66a")]
		private global::Elements.Data.MasterEnemyIgnoreSkillRf.EnemyIgnoreSkillRf _SelectOne(int enemy_id)
		{
			PreparedQuery query = _db.GetSelectQuery_EnemyIgnoreSkillRf();
			if (query == null) return null;
			EnemyIgnoreSkillRf value = null;
			if (query.BindInt(1, enemy_id))
			{
				if (query.Step()) value = _CreateCachedOrmByQueryResult(query);
				query.Reset();
			}
			return value;
		}

		private global::Elements.Data.MasterEnemyIgnoreSkillRf.EnemyIgnoreSkillRf _CreateCachedOrmByQueryResult(global::OpenPCR.DbStubs.Query query)
		{
			int id = query.GetInt(0);
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out EnemyIgnoreSkillRf value))
			{

				value = new EnemyIgnoreSkillRf(id);
				_lazyPrimaryKeyDictionary.Add(id, value);
			}
			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
