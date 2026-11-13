using global::OpenPCR.DbStubs;
using System.Collections.Generic;
using System.Linq;
namespace Elements.Data
{
	public class MasterTalentFormationBonus : AbstractMasterData
	{
		public class TalentFormationBonus
		{

			private global::System.Collections.Generic.List<System.Int32> bonusValueList;

			protected global::System.Int32 _formation_bonus_id;

			protected global::System.Int32 _talent_bonus_1;

			protected global::System.Int32 _talent_bonus_2;

			protected global::System.Int32 _talent_bonus_3;

			protected global::System.Int32 _talent_bonus_4;

			protected global::System.Int32 _talent_bonus_5;

			public global::System.Int32 talent_bonus_1 => _talent_bonus_1;

			public global::System.Int32 talent_bonus_2 => _talent_bonus_2;

			public global::System.Int32 talent_bonus_3 => _talent_bonus_3;

			public global::System.Int32 talent_bonus_4 => _talent_bonus_4;

			public global::System.Int32 talent_bonus_5 => _talent_bonus_5;

			public TalentFormationBonus(int formation_bonus_id = 0, int talent_bonus_1 = 0, int talent_bonus_2 = 0, int talent_bonus_3 = 0, int talent_bonus_4 = 0, int talent_bonus_5 = 0)
			{
				_formation_bonus_id = formation_bonus_id;
				_talent_bonus_1 = talent_bonus_1;
				_talent_bonus_2 = talent_bonus_2;
				_talent_bonus_3 = talent_bonus_3;
				_talent_bonus_4 = talent_bonus_4;
				_talent_bonus_5 = talent_bonus_5;
				SetUp();
			}

			[global::OpenPCR.Migration("3d3c4ad9-7208-43c7-93ca-498600ce5488")]
			public long GetBonusValue(int _count)
			{
				return bonusValueList[(_count >= 5 ? 5 : _count) - 1];
			}

			public void SetUp()
			{
				var values = new List<int>();
				values.Add(talent_bonus_1);
				values.Add(talent_bonus_2);
				values.Add(talent_bonus_3);
				values.Add(talent_bonus_4);
				values.Add(talent_bonus_5);
				bonusValueList = values;
			}
		}

		private global::Elements.Data.MasterTalentDatabase _db;

		private global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterTalentFormationBonus.TalentFormationBonus> _lazyPrimaryKeyDictionary;

		public MasterTalentFormationBonus(global::Elements.Data.MasterTalentDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new global::System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterTalentFormationBonus.TalentFormationBonus>();
			_db = db;
		}

		[global::OpenPCR.Migration("9102032d-1ea3-4a56-ac83-0a0a05310dec")]
		public global::Elements.Data.MasterTalentFormationBonus.TalentFormationBonus GetDefaultBonusData()
		{
			return Get(1);
		}

		[global::OpenPCR.Migration("512f8191-cdbf-4fcf-881b-2e3b84d5387a")]
		public global::Elements.Data.MasterTalentFormationBonus.TalentFormationBonus Get(int formation_bonus_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(formation_bonus_id, out TalentFormationBonus value))
			{
				value = _db != null ? _SelectOne(formation_bonus_id) : null;
				_lazyPrimaryKeyDictionary[formation_bonus_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("65b4e1ae-f58b-4b5c-9a9e-e43286832081")]
		private global::Elements.Data.MasterTalentFormationBonus.TalentFormationBonus _SelectOne(int formation_bonus_id)
		{
			PreparedQuery query = _db.GetSelectQuery_TalentFormationBonus();
			if (query == null) return null;
			TalentFormationBonus value = null;
			if (query.BindInt(1, formation_bonus_id))
			{
				if (query.Step()) value = _CreateCachedOrmByQueryResult(query);
				query.Reset();
			}
			return value;
		}

		private global::Elements.Data.MasterTalentFormationBonus.TalentFormationBonus _CreateCachedOrmByQueryResult(global::OpenPCR.DbStubs.Query query)
		{
			int id = query.GetInt(0);
			if (!_lazyPrimaryKeyDictionary.TryGetValue(id, out TalentFormationBonus value))
			{
				int argument1 = query.GetInt(1);
				int argument2 = query.GetInt(2);
				int argument3 = query.GetInt(3);
				int argument4 = query.GetInt(4);
				int argument5 = query.GetInt(5);
				value = new TalentFormationBonus(id, argument1, argument2, argument3, argument4, argument5);
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
