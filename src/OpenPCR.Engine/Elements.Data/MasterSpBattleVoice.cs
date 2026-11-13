using global::OpenPCR.DbStubs;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterSpBattleVoice : AbstractMasterData
	{
		public class SpBattleVoice
		{
			protected global::System.Int32 _id;
			protected global::System.Int32 _unit_id;
			protected global::System.Int32 _voice_type;
			protected global::System.Int32 _value;
			public global::System.Int32 id => _id;
			public global::System.Int32 unit_id => _unit_id;
			[global::OpenPCR.ImplicitMigration("471bf150-f936-4226-903d-f40ef1043275")]
			public global::System.Int32 voice_type => _voice_type;
			[global::OpenPCR.ImplicitMigration("4da3fb72-964d-4798-8b58-19faa07aa673")]
			public global::System.Int32 value => _value;

			public SpBattleVoice(int id = 0, int unit_id = 0, int voice_type = 0, int value = 0)
			{
				_id = id;
				_unit_id = unit_id;
				_voice_type = voice_type;
				_value = value;
			}
		}
		private MasterUnitDatabase _db;
		private Dictionary<ulong, SpBattleVoice> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<SpBattleVoice>> _dictionaryOfListWithUnitId;
		public MasterSpBattleVoice(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<ulong, SpBattleVoice>();
			_dictionaryOfListWithUnitId = new Dictionary<int, List<SpBattleVoice>>();
			_db = db;
		}

		[global::OpenPCR.Migration("1a727aea-97e1-4750-9718-9285639a0879")]
		public List<SpBattleVoice> GetListWithUnitId(int unit_id)
		{
			if (!_dictionaryOfListWithUnitId.TryGetValue(unit_id, out List<SpBattleVoice> value))
			{
				value = _ListSelectWithUnitId(unit_id);
				_dictionaryOfListWithUnitId.Add(unit_id, value);
			}

			return value;
		}

		[global::OpenPCR.Migration("086c80c8-5897-4ab4-9c4a-e69736571292")]
		private List<SpBattleVoice> _ListSelectWithUnitId(int unit_id)
		{
			var values = new List<SpBattleVoice>();
			PreparedQuery query = _db.GetSelectQueryWithIndex_SpBattleVoice_UnitId();
			if (query != null)
			{
				if (!query.BindInt(1, unit_id))
					return null;
				while (query.Step())
					values.Add(_CreateCachedOrmByQueryResult(query));
				query.Reset();
			}

			return values;
		}

		private SpBattleVoice _CreateCachedOrmByQueryResult(Query query)
		{
			int id = query.GetInt(0);
			int unit = query.GetInt(1);
			ulong key = unchecked((uint)id | ((ulong)(uint)unit << 32));
			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out SpBattleVoice value))
			{
				int type = query.GetInt(2);
				int voiceValue = query.GetInt(3);
				value = new SpBattleVoice(id, unit, type, voiceValue);
				_lazyPrimaryKeyDictionary.Add(key, value);
			}

			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithUnitId = null;
		}
	}
}
