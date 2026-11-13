using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterTalentWeakness : AbstractMasterData
	{
		public class TalentWeakness
		{
			protected global::System.Int32 _resist_id;
			protected global::System.Int32 _talent_1;
			protected global::System.Int32 _talent_2;
			protected global::System.Int32 _talent_3;
			protected global::System.Int32 _talent_4;
			protected global::System.Int32 _talent_5;
			public List<int> TalentWeaknessPercentList { get; private set; }
			public global::System.Int32 talent_1 { get { return _talent_1; } }
			public global::System.Int32 talent_2 { get { return _talent_2; } }
			public global::System.Int32 talent_3 { get { return _talent_3; } }
			public global::System.Int32 talent_4 { get { return _talent_4; } }
			public global::System.Int32 talent_5 { get { return _talent_5; } }

			public TalentWeakness(int resist_id = 0, int talent_1 = 0, int talent_2 = 0, int talent_3 = 0, int talent_4 = 0, int talent_5 = 0)
			{
				_resist_id = resist_id;
				_talent_1 = talent_1;
				_talent_2 = talent_2;
				_talent_3 = talent_3;
				_talent_4 = talent_4;
				_talent_5 = talent_5;
				SetUp();
			}

			public void SetUp()
			{
				var values = new List<int>();
				values.Add(talent_1);
				values.Add(talent_2);
				values.Add(talent_3);
				values.Add(talent_4);
				values.Add(talent_5);
				TalentWeaknessPercentList = values;
			}
		}

		private MasterTalentDatabase _db;
		private Dictionary<int, TalentWeakness> _lazyPrimaryKeyDictionary;

		public MasterTalentWeakness(MasterTalentDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, TalentWeakness>();
			_db = db;
		}

		[global::OpenPCR.Migration("14fe974a-7302-4b78-87e1-072f911a090e")]
		public TalentWeakness Get(int resist_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(resist_id, out TalentWeakness value))
			{
				value = _db != null ? _SelectOne(resist_id) : null;
				_lazyPrimaryKeyDictionary[resist_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("220b62f9-d2d5-4055-8d2d-886d542e4478")]
		private TalentWeakness _SelectOne(int resist_id)
		{
			if (_db == null)
				throw new System.NullReferenceException();
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterTalentWeakness::_SelectOne(Int32): Native MasterTalentDatabase.GetSelectQuery_TalentWeakness and BindInt(1)/Step/ORM caching require the SQLite input producer.");
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
