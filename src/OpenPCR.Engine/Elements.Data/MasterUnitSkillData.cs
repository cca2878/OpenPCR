using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterUnitSkillData : AbstractMasterData
	{
		[global::OpenPCR.ImplicitMigration("0ab2c99c-d237-4517-a449-42228e085772")]
		public UnitSkillData this[int unit_id] => Get(unit_id);

		[global::OpenPCR.Migration("5a7570a3-cff8-4d9d-b3e2-3a965b92257a")]
		public UnitSkillData Get(int unit_id)
		{
			UnitSkillData value = null;
			if (!_lazyPrimaryKeyDictionary.TryGetValue(unit_id, out value))
			{
				value = _db != null ? _SelectOne(unit_id) : null;
				_lazyPrimaryKeyDictionary[unit_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("f141161c-a20f-418d-9c7b-22abebcf9980")]
		private UnitSkillData _SelectOne(int unit_id)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterUnitSkillData._SelectOne: persistent Master database resource row production");
		}

		public class UnitSkillData
		{
			public List<int> ExSkillIds { get; set; }
			public List<int> ExSkillEvolutionIds { get; set; }
			public global::System.Collections.Generic.List<System.Int32> UnionBurstIds { get; set; }
			public global::System.Collections.Generic.List<System.Int32> MainSkillIds { get; set; }
			public global::System.Collections.Generic.List<System.Int32> SpSkillIds { get; set; }
			public global::System.Collections.Generic.List<System.Int32> SpSkillEvolutionIds { get; set; }
			public global::System.Collections.Generic.List<System.Int32> UnionBurstEvolutionIds { get; set; }
			public global::System.Collections.Generic.List<System.Int32> MainSkillEvolutionIds { get; set; }
			public global::System.Collections.Generic.List<System.Int32> SubUnionBurstIds { get; set; }
			[global::OpenPCR.ImplicitMigration("d01c7745-444f-49c4-a28d-3be61bb61845")]
			public global::System.Int32 ex_skill_evolution_1 => _ex_skill_evolution_1;

			protected global::System.Int32 _unit_id;
			protected global::System.Int32 _union_burst;
			protected global::System.Int32 _main_skill_1;
			protected global::System.Int32 _main_skill_2;
			protected global::System.Int32 _main_skill_3;
			protected global::System.Int32 _main_skill_4;
			protected global::System.Int32 _main_skill_5;
			protected global::System.Int32 _main_skill_6;
			protected global::System.Int32 _main_skill_7;
			protected global::System.Int32 _main_skill_8;
			protected global::System.Int32 _main_skill_9;
			protected global::System.Int32 _main_skill_10;
			protected global::System.Int32 _ex_skill_1;
			protected global::System.Int32 _ex_skill_evolution_1;
			protected global::System.Int32 _ex_skill_2;
			protected global::System.Int32 _ex_skill_evolution_2;
			protected global::System.Int32 _ex_skill_3;
			protected global::System.Int32 _ex_skill_evolution_3;
			protected global::System.Int32 _ex_skill_4;
			protected global::System.Int32 _ex_skill_evolution_4;
			protected global::System.Int32 _ex_skill_5;
			protected global::System.Int32 _ex_skill_evolution_5;
			protected global::System.Int32 _sp_union_burst;
			protected global::System.Int32 _sp_skill_1;
			protected global::System.Int32 _sp_skill_2;
			protected global::System.Int32 _sp_skill_3;
			protected global::System.Int32 _sp_skill_4;
			protected global::System.Int32 _sp_skill_5;
			protected global::System.Int32 _union_burst_evolution;
			protected global::System.Int32 _main_skill_evolution_1;
			protected global::System.Int32 _main_skill_evolution_2;
			protected global::System.Int32 _sp_skill_evolution_1;
			protected global::System.Int32 _sp_skill_evolution_2;
			public UnitSkillData(int unit_id = 0, int union_burst = 0, int main_skill_1 = 0, int main_skill_2 = 0, int main_skill_3 = 0, int main_skill_4 = 0, int main_skill_5 = 0, int main_skill_6 = 0, int main_skill_7 = 0, int main_skill_8 = 0, int main_skill_9 = 0, int main_skill_10 = 0, int ex_skill_1 = 0, int ex_skill_evolution_1 = 0, int ex_skill_2 = 0, int ex_skill_evolution_2 = 0, int ex_skill_3 = 0, int ex_skill_evolution_3 = 0, int ex_skill_4 = 0, int ex_skill_evolution_4 = 0, int ex_skill_5 = 0, int ex_skill_evolution_5 = 0, int sp_union_burst = 0, int sp_skill_1 = 0, int sp_skill_2 = 0, int sp_skill_3 = 0, int sp_skill_4 = 0, int sp_skill_5 = 0, int union_burst_evolution = 0, int main_skill_evolution_1 = 0, int main_skill_evolution_2 = 0, int sp_skill_evolution_1 = 0, int sp_skill_evolution_2 = 0)
			{
				_unit_id = unit_id;
				_union_burst = union_burst;
				_main_skill_1 = main_skill_1;
				_main_skill_2 = main_skill_2;
				_main_skill_3 = main_skill_3;
				_main_skill_4 = main_skill_4;
				_main_skill_5 = main_skill_5;
				_main_skill_6 = main_skill_6;
				_main_skill_7 = main_skill_7;
				_main_skill_8 = main_skill_8;
				_main_skill_9 = main_skill_9;
				_main_skill_10 = main_skill_10;
				_ex_skill_1 = ex_skill_1;
				_ex_skill_evolution_1 = ex_skill_evolution_1;
				_ex_skill_2 = ex_skill_2;
				_ex_skill_evolution_2 = ex_skill_evolution_2;
				_ex_skill_3 = ex_skill_3;
				_ex_skill_evolution_3 = ex_skill_evolution_3;
				_ex_skill_4 = ex_skill_4;
				_ex_skill_evolution_4 = ex_skill_evolution_4;
				_ex_skill_5 = ex_skill_5;
				_ex_skill_evolution_5 = ex_skill_evolution_5;
				_sp_union_burst = sp_union_burst;
				_sp_skill_1 = sp_skill_1;
				_sp_skill_2 = sp_skill_2;
				_sp_skill_3 = sp_skill_3;
				_sp_skill_4 = sp_skill_4;
				_sp_skill_5 = sp_skill_5;
				_union_burst_evolution = union_burst_evolution;
				_main_skill_evolution_1 = main_skill_evolution_1;
				_main_skill_evolution_2 = main_skill_evolution_2;
				_sp_skill_evolution_1 = sp_skill_evolution_1;
				_sp_skill_evolution_2 = sp_skill_evolution_2;
				SetUp();
			}

			public void SetUp()
			{
				UnionBurstIds = new List<int>();
				MainSkillIds = new List<int>();
				ExSkillIds = new List<int>();
				ExSkillEvolutionIds = new List<int>();
				SpSkillIds = new List<int>();
				UnionBurstEvolutionIds = new List<int>();
				MainSkillEvolutionIds = new List<int>();
				SpSkillEvolutionIds = new List<int>();
				UnionBurstIds.Add(_union_burst);
				SubUnionBurstIds = new List<int>();
				MainSkillIds.Add(_main_skill_1);
				MainSkillIds.Add(_main_skill_2);
				MainSkillIds.Add(_main_skill_3);
				MainSkillIds.Add(_main_skill_4);
				MainSkillIds.Add(_main_skill_5);
				MainSkillIds.Add(_main_skill_6);
				MainSkillIds.Add(_main_skill_7);
				MainSkillIds.Add(_main_skill_8);
				MainSkillIds.Add(_main_skill_9);
				MainSkillIds.Add(_main_skill_10);
				ExSkillIds.Add(_ex_skill_1);
				ExSkillIds.Add(_ex_skill_2);
				ExSkillIds.Add(_ex_skill_3);
				ExSkillIds.Add(_ex_skill_4);
				ExSkillIds.Add(_ex_skill_5);
				ExSkillEvolutionIds.Add(_ex_skill_evolution_1);
				ExSkillEvolutionIds.Add(_ex_skill_evolution_2);
				ExSkillEvolutionIds.Add(_ex_skill_evolution_3);
				ExSkillEvolutionIds.Add(_ex_skill_evolution_4);
				ExSkillEvolutionIds.Add(_ex_skill_evolution_5);
				SpSkillIds.Add(_sp_skill_1);
				SpSkillIds.Add(_sp_skill_2);
				SpSkillIds.Add(_sp_skill_3);
				SpSkillIds.Add(_sp_skill_4);
				SpSkillIds.Add(_sp_skill_5);
				UnionBurstEvolutionIds.Add(_union_burst_evolution);
				MainSkillEvolutionIds.Add(_main_skill_evolution_1);
				MainSkillEvolutionIds.Add(_main_skill_evolution_2);
				SpSkillEvolutionIds.Add(_sp_skill_evolution_1);
				SpSkillEvolutionIds.Add(_sp_skill_evolution_2);
				SubUnionBurstIds.Add(_sp_union_burst);
			}
		}

		private MasterUnitDatabase _db;
		private Dictionary<int, UnitSkillData> _lazyPrimaryKeyDictionary;
		public MasterUnitSkillData(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, UnitSkillData>();
			_db = db;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
