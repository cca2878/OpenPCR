using global::OpenPCR.DbStubs;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterTalentFormationOverwrite : AbstractMasterData
	{
		public class TalentFormationOverwrite
		{
			private List<MasterTalentFormationBonus.TalentFormationBonus> overwriteBonusIdList;
			protected global::System.Int32 _id;
			public global::System.Int32 id => _id;

			protected global::System.Int32 _content_type;

			protected global::System.Int32 _target_value_1;

			protected global::System.Int32 _target_value_2;
			[global::OpenPCR.ImplicitMigration("0beabbb9-d0a3-4bec-b86c-f301ba939ae1")]
			public global::System.Int32 target_value_2 => _target_value_2;

			protected global::System.Int32 _formation_bonus_id_1;
			public global::System.Int32 formation_bonus_id_1 => _formation_bonus_id_1;

			protected global::System.Int32 _formation_bonus_id_2;
			public global::System.Int32 formation_bonus_id_2 => _formation_bonus_id_2;

			protected global::System.Int32 _formation_bonus_id_3;
			public global::System.Int32 formation_bonus_id_3 => _formation_bonus_id_3;

			protected global::System.Int32 _formation_bonus_id_4;
			public global::System.Int32 formation_bonus_id_4 => _formation_bonus_id_4;

			protected global::System.Int32 _formation_bonus_id_5;
			public global::System.Int32 formation_bonus_id_5 => _formation_bonus_id_5;

			public TalentFormationOverwrite(int id = 0, int content_type = 0, int target_value_1 = 0, int target_value_2 = 0, int formation_bonus_id_1 = 0, int formation_bonus_id_2 = 0, int formation_bonus_id_3 = 0, int formation_bonus_id_4 = 0, int formation_bonus_id_5 = 0)
			{
				_id = id;
				_content_type = content_type;
				_target_value_1 = target_value_1;
				_target_value_2 = target_value_2;
				_formation_bonus_id_1 = formation_bonus_id_1;
				_formation_bonus_id_2 = formation_bonus_id_2;
				_formation_bonus_id_3 = formation_bonus_id_3;
				_formation_bonus_id_4 = formation_bonus_id_4;
				_formation_bonus_id_5 = formation_bonus_id_5;
				SetUp();
			}

			[global::OpenPCR.Migration("f764e439-3290-4383-9385-d51be341179b")]
			public MasterTalentFormationBonus.TalentFormationBonus GetTalentFormationBonus(int _talentId)
			{
				return overwriteBonusIdList[_talentId - 1];
			}

			public void SetUp()
			{
				MasterTalentFormationBonus master = global::Elements.ManagerSingleton<MasterDataManager>.Instance.masterTalentFormationBonus;
				var result = new List<MasterTalentFormationBonus.TalentFormationBonus>();
				result.Add(master.Get(formation_bonus_id_1));
				result.Add(master.Get(formation_bonus_id_2));
				result.Add(master.Get(formation_bonus_id_3));
				result.Add(master.Get(formation_bonus_id_4));
				result.Add(master.Get(formation_bonus_id_5));
				overwriteBonusIdList = result;
			}
		}
		private MasterTalentDatabase _db;
		private Dictionary<int, TalentFormationOverwrite> _lazyPrimaryKeyDictionary;
		private Dictionary<ulong, List<TalentFormationOverwrite>> _dictionaryOfListWithContentTypeAndTargetValue1;
		public MasterTalentFormationOverwrite(MasterTalentDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, TalentFormationOverwrite>();
			_dictionaryOfListWithContentTypeAndTargetValue1 = new Dictionary<ulong, List<TalentFormationOverwrite>>();
			_db = db;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithContentTypeAndTargetValue1 = null;
		}

		[global::OpenPCR.Migration("df33342a-3e4c-4783-9ff5-817c509dc438")]
		public TalentFormationOverwrite SearchTalentFormationOverwrite(global::Elements.eBattleCategory _battleCategory, int _targetValue1, int _targetValue2)
		{
			var values = GetListWithContentTypeAndTargetValue1((int)_battleCategory, _targetValue1);
			return values == null ? null : values.Find(data => (int)data.target_value_2 == _targetValue2);
		}

		[global::OpenPCR.Migration("57d943c2-39bc-42f0-a253-3a837f67a775")]
		public List<TalentFormationOverwrite> GetListWithContentTypeAndTargetValue1(int content_type, int target_value_1)
		{
			ulong key = (ulong)(uint)content_type | ((ulong)(uint)target_value_1 << 32);
			if (!_dictionaryOfListWithContentTypeAndTargetValue1.TryGetValue(key, out var value))
			{
				value = _ListSelectWithContentTypeAndTargetValue1(content_type, target_value_1);
				_dictionaryOfListWithContentTypeAndTargetValue1.Add(key, value);
			}

			return value;
		}

		[global::OpenPCR.Migration("edce067e-efdb-4d1c-8d96-e5b39a4eed50")]
		private List<TalentFormationOverwrite> _ListSelectWithContentTypeAndTargetValue1(int content_type, int target_value_1)
		{
			var values = new List<TalentFormationOverwrite>();
			PreparedQuery query = _db.GetSelectQueryWithIndex_TalentFormationOverwrite_ContentType_TargetValue1();
			if (query == null)
				return values;
			if (!query.BindInt(1, content_type))
				return null;
			if (!query.BindInt(2, target_value_1))
				return null;
			while (query.Step())
				values.Add(_CreateCachedOrmByQueryResult(query));
			query.Reset();
			return values;
		}

		private TalentFormationOverwrite _CreateCachedOrmByQueryResult(Query query)
		{
			int id = query.GetInt(0);
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out var value))
			{
				int argument1 = query.GetInt(1);
				int argument2 = query.GetInt(2);
				int argument3 = query.GetInt(3);
				int argument4 = query.GetInt(4);
				int argument5 = query.GetInt(5);
				int argument6 = query.GetInt(6);
				int argument7 = query.GetInt(7);
				int argument8 = query.GetInt(8);
				value = new TalentFormationOverwrite(id, argument1, argument2, argument3, argument4, argument5, argument6, argument7, argument8);
				_lazyPrimaryKeyDictionary.Add(id, value);
			}

			return value;
		}
	}
}
