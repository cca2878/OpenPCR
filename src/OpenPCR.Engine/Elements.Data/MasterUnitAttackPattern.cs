using System;
using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterUnitAttackPattern : AbstractMasterData
	{
		private Elements.Data.MasterUnitDatabase _db;
		private System.Collections.Generic.Dictionary<System.Int32, Elements.Data.MasterUnitAttackPattern.UnitAttackPattern> _lazyPrimaryKeyDictionary;
		public class UnitAttackPattern
		{
			protected global::System.Int32 _pattern_id;
			protected global::System.Int32 _unit_id;
			protected global::System.Int32 _loop_start;
			protected global::System.Int32 _loop_end;
			protected global::System.Int32 _atk_pattern_1;
			protected global::System.Int32 _atk_pattern_2;
			protected global::System.Int32 _atk_pattern_3;
			protected global::System.Int32 _atk_pattern_4;
			protected global::System.Int32 _atk_pattern_5;
			protected global::System.Int32 _atk_pattern_6;
			protected global::System.Int32 _atk_pattern_7;
			protected global::System.Int32 _atk_pattern_8;
			protected global::System.Int32 _atk_pattern_9;
			protected global::System.Int32 _atk_pattern_10;
			protected global::System.Int32 _atk_pattern_11;
			protected global::System.Int32 _atk_pattern_12;
			protected global::System.Int32 _atk_pattern_13;
			protected global::System.Int32 _atk_pattern_14;
			protected global::System.Int32 _atk_pattern_15;
			protected global::System.Int32 _atk_pattern_16;
			protected global::System.Int32 _atk_pattern_17;
			protected global::System.Int32 _atk_pattern_18;
			protected global::System.Int32 _atk_pattern_19;
			protected global::System.Int32 _atk_pattern_20;
			public global::System.Int32 loop_start { [global::OpenPCR.Migration("25d953a2-b466-4ea0-b3df-ff0d94ac4f98")] get => _loop_start; }
			public global::System.Int32 loop_end { [global::OpenPCR.Migration("3136157e-e03b-4f10-801e-a1188d5d18df")] get => _loop_end; }
			public List<int> PatternList { get; set; }
			[global::OpenPCR.Migration("6359accc-44a1-4670-93df-a1f87dc17e9b")]
			public UnitAttackPattern(System.Int32 pattern_id = 0, System.Int32 unit_id = 0, System.Int32 loop_start = 0, System.Int32 loop_end = 0, System.Int32 atk_pattern_1 = 0, System.Int32 atk_pattern_2 = 0, System.Int32 atk_pattern_3 = 0, System.Int32 atk_pattern_4 = 0, System.Int32 atk_pattern_5 = 0, System.Int32 atk_pattern_6 = 0, System.Int32 atk_pattern_7 = 0, System.Int32 atk_pattern_8 = 0, System.Int32 atk_pattern_9 = 0, System.Int32 atk_pattern_10 = 0, System.Int32 atk_pattern_11 = 0, System.Int32 atk_pattern_12 = 0, System.Int32 atk_pattern_13 = 0, System.Int32 atk_pattern_14 = 0, System.Int32 atk_pattern_15 = 0, System.Int32 atk_pattern_16 = 0, System.Int32 atk_pattern_17 = 0, System.Int32 atk_pattern_18 = 0, System.Int32 atk_pattern_19 = 0, System.Int32 atk_pattern_20 = 0)
			{
				_pattern_id = pattern_id;
				_unit_id = unit_id;
				_loop_start = loop_start;
				_loop_end = loop_end;
				_atk_pattern_1 = atk_pattern_1;
				_atk_pattern_2 = atk_pattern_2;
				_atk_pattern_3 = atk_pattern_3;
				_atk_pattern_4 = atk_pattern_4;
				_atk_pattern_5 = atk_pattern_5;
				_atk_pattern_6 = atk_pattern_6;
				_atk_pattern_7 = atk_pattern_7;
				_atk_pattern_8 = atk_pattern_8;
				_atk_pattern_9 = atk_pattern_9;
				_atk_pattern_10 = atk_pattern_10;
				_atk_pattern_11 = atk_pattern_11;
				_atk_pattern_12 = atk_pattern_12;
				_atk_pattern_13 = atk_pattern_13;
				_atk_pattern_14 = atk_pattern_14;
				_atk_pattern_15 = atk_pattern_15;
				_atk_pattern_16 = atk_pattern_16;
				_atk_pattern_17 = atk_pattern_17;
				_atk_pattern_18 = atk_pattern_18;
				_atk_pattern_19 = atk_pattern_19;
				_atk_pattern_20 = atk_pattern_20;
				SetUp();
			}
			[global::OpenPCR.Migration("7937a225-65b8-4c48-8c7f-c60a668ef719")]
			public void SetUp()
			{
				PatternList = new List<int>();
				PatternList.Add(_atk_pattern_1);
				PatternList.Add(_atk_pattern_2);
				PatternList.Add(_atk_pattern_3);
				PatternList.Add(_atk_pattern_4);
				PatternList.Add(_atk_pattern_5);
				PatternList.Add(_atk_pattern_6);
				PatternList.Add(_atk_pattern_7);
				PatternList.Add(_atk_pattern_8);
				PatternList.Add(_atk_pattern_9);
				PatternList.Add(_atk_pattern_10);
				PatternList.Add(_atk_pattern_11);
				PatternList.Add(_atk_pattern_12);
				PatternList.Add(_atk_pattern_13);
				PatternList.Add(_atk_pattern_15);
				PatternList.Add(_atk_pattern_16);
				PatternList.Add(_atk_pattern_17);
				PatternList.Add(_atk_pattern_18);
				PatternList.Add(_atk_pattern_19);
				PatternList.Add(_atk_pattern_20);
			}
		}
		public MasterUnitAttackPattern(Elements.Data.MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, UnitAttackPattern>();
			_db = db;
		}
		public UnitAttackPattern this[int id] { [global::OpenPCR.Migration("9601a25c-04bb-4eaa-9aec-ee730add358c")] get => Get(id); }
		[global::OpenPCR.Migration("1bc7aa21-b58d-4223-9ffb-d2231e2e680f")]
		public UnitAttackPattern Get(int pattern_id)
		{
			UnitAttackPattern value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(pattern_id, out value))
			{
				value = _db != null ? _SelectOne(pattern_id) : null;
				_lazyPrimaryKeyDictionary[pattern_id] = value;
			}
			return value;
		}
		[global::OpenPCR.Migration("2faff4dd-e385-45f0-ace9-d72655148d57")]
		private UnitAttackPattern _SelectOne(int pattern_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitAttackPattern._SelectOne SQLite query production and ORM mapping");
		}
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
