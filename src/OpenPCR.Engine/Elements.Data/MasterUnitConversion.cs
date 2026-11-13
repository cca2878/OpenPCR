using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterUnitConversion : AbstractMasterData
	{
		private MasterUnitDatabase _db;
		private Dictionary<int, UnitConversion> _lazyPrimaryKeyDictionary;
		private Dictionary<int, UnitConversion> _dictionaryOfOrmWithUnitId;

		public class UnitConversion
		{
			protected global::System.Int32 _original_unit_id;
			protected global::System.Int32 _unit_id;
			[global::OpenPCR.ImplicitMigration("06481ef4-19fe-4b12-b11f-988008d4625b")]
			public global::System.Int32 original_unit_id => _original_unit_id;
			[global::OpenPCR.ImplicitMigration("67b53477-360a-4482-a8f9-11de08681626")]
			public global::System.Int32 unit_id => _unit_id;

			public UnitConversion(int original_unit_id = 0, int unit_id = 0)
			{
				_original_unit_id = original_unit_id;
				_unit_id = unit_id;
			}
		}

		public MasterUnitConversion(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, UnitConversion>();
			_dictionaryOfOrmWithUnitId = new Dictionary<int, UnitConversion>();
			_db = db;
		}

		[global::OpenPCR.Migration("94626a90-bf0a-422e-bbd9-70d8812c9a0d")]
		public UnitConversion Get(int original_unit_id)
		{
			UnitConversion value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(original_unit_id, out value))
			{
				value = _db != null ? _SelectOne(original_unit_id) : null;
				_lazyPrimaryKeyDictionary[original_unit_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("aaaa550e-c405-4371-ba32-73716bd9ad38")]
		private UnitConversion _SelectOne(int original_unit_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitConversion::_SelectOne(System.Int32): Native persistent MasterUnitDatabase query/ORM producer; prepared conversion cache read remains implemented");
		}

		[global::OpenPCR.Migration("08054fa6-c0f2-45c1-b848-74ff3a74b0be")]
		public UnitConversion GetWithUnitId(int unit_id)
		{
			UnitConversion value = null;
			if (!_dictionaryOfOrmWithUnitId.TryGetValue(unit_id, out value))
			{
				value = _db != null ? _SelectWithUnitId(unit_id) : null;
				_dictionaryOfOrmWithUnitId[unit_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("223521b2-d8bd-4baf-9deb-5dc9cace7edb")]
		private UnitConversion _SelectWithUnitId(int unit_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitConversion::_SelectWithUnitId(System.Int32): Native persistent MasterUnitDatabase query/ORM producer; prepared conversion read cache remains implemented");
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfOrmWithUnitId = null;
		}
	}
}
