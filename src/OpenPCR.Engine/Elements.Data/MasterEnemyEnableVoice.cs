using global::OpenPCR.DbStubs;
using System.Collections.Generic;
using System.Linq;

namespace Elements.Data
{
	public class MasterEnemyEnableVoice : AbstractMasterData
	{
		public class EnemyEnableVoice
		{
			protected global::System.Int32 _unit_id;
			protected global::System.Int32 _voice_id;
			public global::System.Int32 unit_id => _unit_id;
			[global::OpenPCR.ImplicitMigration("6cc2c06e-3a01-41fe-9f78-7ea0488d8d56")]
			public global::System.Int32 voice_id => _voice_id;

			public EnemyEnableVoice(int unit_id = 0, int voice_id = 0)
			{
				_unit_id = unit_id;
				_voice_id = voice_id;
			}
		}

		private MasterEnemyDatabase _db;
		private Dictionary<int, EnemyEnableVoice> _lazyPrimaryKeyDictionary;
		private Dictionary<int, EnemyEnableVoice> _strictPrimaryKeyDictionary;
		public MasterEnemyEnableVoice(MasterEnemyDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, EnemyEnableVoice>();
			_db = db;
		}

		[global::OpenPCR.Migration("d9fa6830-5891-43a1-a1a7-4158af02dcc7")]
		public EnemyEnableVoice Get(int unit_id)
		{
			EnemyEnableVoice value = null;
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

		[global::OpenPCR.Migration("cef3d7b4-f60a-4036-9da2-58d8f0a75e47")]
		private EnemyEnableVoice _SelectOne(int unit_id)
		{
			PreparedQuery query = _db.GetSelectQuery_EnemyEnableVoice();
			if (query == null)
				return null;
			if (!query.BindInt(1, unit_id))
				return null;
			EnemyEnableVoice result = null;
			if (query.Step())
				result = _CreateCachedOrmByQueryResult(query);
			query.Reset();
			return result;
		}

		private EnemyEnableVoice _CreateCachedOrmByQueryResult(Query query)
		{
			int key = query.GetInt(0);
			EnemyEnableVoice value = null;
			if (_strictPrimaryKeyDictionary != null)
			{
				_strictPrimaryKeyDictionary.TryGetValue(key, out value);
				return value;
			}

			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out value))
			{
				int voice_id = query.GetInt(1);
				value = new EnemyEnableVoice(key, voice_id);
				_lazyPrimaryKeyDictionary.Add(key, value);
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
