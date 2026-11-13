using global::OpenPCR.DbStubs;
using System.Collections.Generic;
using System.Linq;
namespace Elements.Data
{
	public class MasterUnitSkillDataRf : AbstractMasterData
	{
		public class UnitSkillDataRf
		{
			protected global::System.Int32 _id;

			protected global::System.Int32 _skill_id;

			protected global::System.Int32 _rf_skill_id;

			protected global::System.Int32 _min_lv;

			protected global::System.Int32 _max_lv;

			public global::System.Int32 id => _id;

			public global::System.Int32 skill_id => _skill_id;

			[global::OpenPCR.ImplicitMigration("cc1fcdb6-65ca-442a-8a54-37f9b5e028df")]
			public global::System.Int32 rf_skill_id => _rf_skill_id;

			[global::OpenPCR.ImplicitMigration("873d0d69-a4ea-4564-9dbd-c0bbe48be135")]
			public global::System.Int32 min_lv => _min_lv;

			public UnitSkillDataRf(int id = 0, int skill_id = 0, int rf_skill_id = 0, int min_lv = 0, int max_lv = 0)
			{
				_id = id;
				_skill_id = skill_id;
				_rf_skill_id = rf_skill_id;
				_min_lv = min_lv;
				_max_lv = max_lv;
			}
		}

		private global::Elements.Data.MasterSkillDatabase _db;

		private global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf> _lazyPrimaryKeyDictionary;

		private global::System.Collections.Generic.Dictionary<System.Int32, System.Collections.Generic.List<Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf>> _dictionaryOfListWithSkillId;

		private global::System.Collections.Generic.Dictionary<System.Int32, System.Collections.Generic.List<Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf>> _dictionaryOfListWithRfSkillId;

		public MasterUnitSkillDataRf(global::Elements.Data.MasterSkillDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf>();
			_dictionaryOfListWithSkillId = new global::System.Collections.Generic.Dictionary<System.Int32, System.Collections.Generic.List<Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf>>();
			_dictionaryOfListWithRfSkillId = new global::System.Collections.Generic.Dictionary<System.Int32, System.Collections.Generic.List<Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf>>();
			_db = db;
		}

		public global::Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf Get(int id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out UnitSkillDataRf value))
			{
				value = _db != null ? _SelectOne(id) : null;
				_lazyPrimaryKeyDictionary[id] = value;
			}
			return value;
		}

		private global::Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf _SelectOne(int id)
		{
			PreparedQuery query = _db.GetSelectQuery_UnitSkillDataRf();
			if (query == null) return null;
			UnitSkillDataRf value = null;
			if (query.BindInt(1, id))
			{
				if (query.Step()) value = _CreateCachedOrmByQueryResult(query);
				query.Reset();
			}
			return value;
		}

		[global::OpenPCR.Migration("0c93eeca-44ff-4558-b16a-3fd4a2b452d4")]
		public global::System.Collections.Generic.List<Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf> GetListWithSkillId(int skill_id)
		{
			if (!_dictionaryOfListWithSkillId.TryGetValue(skill_id, out List<UnitSkillDataRf> value))
			{
				value = _ListSelectWithSkillId(skill_id);
				_dictionaryOfListWithSkillId.Add(skill_id, value);
			}
			return value;
		}

		[global::OpenPCR.Migration("49efd04b-a8b6-42ed-8854-2367fff11c50")]
		private global::System.Collections.Generic.List<Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf> _ListSelectWithSkillId(int skill_id)
		{
			var values = new List<UnitSkillDataRf>();
			PreparedQuery query = _db.GetSelectQueryWithIndex_UnitSkillDataRf_SkillId();
			if (query != null)
			{
				if (!query.BindInt(1, skill_id)) return null;
				while (query.Step()) values.Add(_CreateCachedOrmByQueryResult(query));
				query.Reset();
			}
			return values;
		}

		[global::OpenPCR.Migration("dc516fab-63b5-4771-adba-4b3a01336f19")]
		public global::System.Collections.Generic.List<Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf> GetListWithRfSkillId(int rf_skill_id)
		{
			if (!_dictionaryOfListWithRfSkillId.TryGetValue(rf_skill_id, out List<UnitSkillDataRf> value))
			{
				value = _ListSelectWithRfSkillId(rf_skill_id);
				_dictionaryOfListWithRfSkillId.Add(rf_skill_id, value);
			}
			return value;
		}

		[global::OpenPCR.Migration("9cbfdd7c-d6e3-43bf-b04c-5bf0cfcc8248")]
		private global::System.Collections.Generic.List<Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf> _ListSelectWithRfSkillId(int rf_skill_id)
		{
			var values = new List<UnitSkillDataRf>();
			PreparedQuery query = _db.GetSelectQueryWithIndex_UnitSkillDataRf_RfSkillId();
			if (query != null)
			{
				if (!query.BindInt(1, rf_skill_id)) return null;
				while (query.Step()) values.Add(_CreateCachedOrmByQueryResult(query));
				query.Reset();
			}
			return values;
		}

		private global::Elements.Data.MasterUnitSkillDataRf.UnitSkillDataRf _CreateCachedOrmByQueryResult(global::OpenPCR.DbStubs.Query query)
		{
			int id = query.GetInt(0);
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out UnitSkillDataRf value))
			{
				int argument1 = query.GetInt(1);
				int argument2 = query.GetInt(2);
				int argument3 = query.GetInt(3);
				int argument4 = query.GetInt(4);
				value = new UnitSkillDataRf(id, argument1, argument2, argument3, argument4);
				_lazyPrimaryKeyDictionary.Add(id, value);
			}
			return value;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithSkillId = null;
			_dictionaryOfListWithRfSkillId = null;
		}
	}
}
