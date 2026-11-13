using System.Collections.Generic;
using global::OpenPCR.DbStubs;

namespace Elements.Data
{
	public class MasterEnemyParameter : AbstractMasterData
	{

		[global::OpenPCR.Migration("c0e5267e-66e9-4be7-a167-c71e98b16bf6")]
		public EnemyParameter GetFromAllKind(int unit_id)
		{
			EnemyParameter result = Get(unit_id);
			if (result != null)
				return result;
			return (EnemyParameter)global::OpenPCR.PreparedEnemyLookupInputs.RequireResult(this, unit_id);
		}

		public class EnemyParameter
		{

			public global::System.Int32 level { [global::OpenPCR.Migration("d178e50c-33a2-4834-acae-b5dc294ab947")] get { return _level; } }

			public global::System.Int64 hp { [global::OpenPCR.Migration("4075fede-565e-454e-ad99-dbfacea2667d")] get { return _hp; } }

			public global::System.Int32 atk { [global::OpenPCR.Migration("cc265bb8-8103-4afd-984f-1e9de2696b6b")] get { return _atk; } }

			public global::System.Int32 magic_str { [global::OpenPCR.Migration("a50fab52-3086-4ea3-9373-cda51c742279")] get { return _magic_str; } }

			public global::System.Int32 def { [global::OpenPCR.Migration("5a5a24ac-18de-4f17-a565-c38536850390")] get { return _def; } }

			public global::System.Int32 magic_def { [global::OpenPCR.Migration("a8ffcb6a-516d-475e-9eca-c705ff22bec6")] get { return _magic_def; } }

			public global::System.Int32 physical_critical { [global::OpenPCR.Migration("c1a76dbf-465b-418e-8027-a8c6c44ba9c9")] get { return _physical_critical; } }

			public global::System.Int32 magic_critical { [global::OpenPCR.Migration("1d583d04-7184-4fc7-b214-add077a7286b")] get { return _magic_critical; } }

			public global::System.Int32 dodge { [global::OpenPCR.Migration("e28fb2c2-0e72-47a2-8a54-124a03b4555d")] get { return _dodge; } }

			public global::System.Int32 life_steal { [global::OpenPCR.Migration("caf39d1c-1b4f-4dab-93d6-d50a51e5e2b7")] get { return _life_steal; } }

			public global::System.Int32 hp_recovery_rate { [global::OpenPCR.Migration("c5a3b22c-ba0e-4795-b9f7-6ed362d9f6cf")] get { return _hp_recovery_rate; } }

			public global::System.Int32 resist_status_id { [global::OpenPCR.Migration("cb022af3-bc93-4d42-b9aa-9b9e398cce72")] get { return _resist_status_id; } }

			public global::System.Int32 resist_variation_id { [global::OpenPCR.Migration("500d4532-4db1-4ed7-b86f-5fa314c1c575")] get { return _resist_variation_id; } }

			public global::System.Int32 accuracy { [global::OpenPCR.Migration("c918e4df-ee94-491e-99d1-06653c004eed")] get { return _accuracy; } }

			[global::OpenPCR.ImplicitMigration("90f39470-fbfb-459c-afdb-b876adf247d3")]
			public global::System.Int32 break_durability => _break_durability;
			[global::OpenPCR.ImplicitMigration("4acc9b23-7da3-4be5-a8fd-e36075608409")]
			public global::System.Int64 virtual_hp => _virtual_hp;

			[global::OpenPCR.ImplicitMigration("7c11f1fd-8506-42ff-9af0-64aac8bf10bc")]
			public global::System.Int32 unit_id => _unit_id;
			[global::OpenPCR.ImplicitMigration("e026d74d-b19d-4c29-82f3-e09786c73246")]
			public global::System.Int32 energy_recovery_rate => _energy_recovery_rate;
			[global::OpenPCR.ImplicitMigration("1e570c88-6e55-4d16-bb95-8aac90d33f97")]
			public global::System.Int32 energy_reduce_rate => _energy_reduce_rate;
			[global::OpenPCR.ImplicitMigration("c7b86122-08d0-426b-a912-318a1e6a7b6e")]
			public global::System.Int32 magic_penetrate => _magic_penetrate;
			[global::OpenPCR.ImplicitMigration("41828c19-4a73-4894-8056-e997b9d98cf2")]
			public global::System.Int32 physical_penetrate => _physical_penetrate;
			public global::System.Int32 promotion_level => _promotion_level;
			[global::OpenPCR.ImplicitMigration("216cf8bd-89b9-42b1-8f08-22ff2cf17818")]
			public global::System.Int32 wave_energy_recovery => _wave_energy_recovery;
			[global::OpenPCR.ImplicitMigration("d67b645a-403e-4259-8bcf-cd835d4fe46b")]
			public global::System.Int32 wave_hp_recovery => _wave_hp_recovery;


			protected global::System.Int32 _enemy_id;
			protected global::System.Int32 _unit_id;
			public global::System.String name => _name;
			protected global::System.String _name;
			protected global::System.Int32 _level;
			protected global::System.Int32 _rarity;
			protected global::System.Int32 _promotion_level;
			protected global::System.Int64 _hp;
			protected global::System.Int32 _atk;
			protected global::System.Int32 _magic_str;
			protected global::System.Int32 _def;
			protected global::System.Int32 _magic_def;
			protected global::System.Int32 _physical_critical;
			protected global::System.Int32 _magic_critical;
			protected global::System.Int32 _wave_hp_recovery;
			protected global::System.Int32 _wave_energy_recovery;
			protected global::System.Int32 _dodge;
			protected global::System.Int32 _physical_penetrate;
			protected global::System.Int32 _magic_penetrate;
			protected global::System.Int32 _life_steal;
			protected global::System.Int32 _hp_recovery_rate;
			protected global::System.Int32 _energy_recovery_rate;
			protected global::System.Int32 _energy_reduce_rate;
			protected global::System.Int32 _union_burst_level;
			protected global::System.Int32 _main_skill_lv_1;
			protected global::System.Int32 _main_skill_lv_2;
			protected global::System.Int32 _main_skill_lv_3;
			protected global::System.Int32 _main_skill_lv_4;
			protected global::System.Int32 _main_skill_lv_5;
			protected global::System.Int32 _main_skill_lv_6;
			protected global::System.Int32 _main_skill_lv_7;
			protected global::System.Int32 _main_skill_lv_8;
			protected global::System.Int32 _main_skill_lv_9;
			protected global::System.Int32 _main_skill_lv_10;
			protected global::System.Int32 _ex_skill_lv_1;
			protected global::System.Int32 _ex_skill_lv_2;
			protected global::System.Int32 _ex_skill_lv_3;
			protected global::System.Int32 _ex_skill_lv_4;
			protected global::System.Int32 _ex_skill_lv_5;
			protected global::System.Int32 _resist_status_id;
			protected global::System.Int32 _resist_variation_id;
			protected global::System.Int32 _accuracy;
			protected global::System.Int32 _unique_equipment_flag_1;
			protected global::System.Int32 _break_durability;
			protected global::System.Int64 _virtual_hp;

			public EnemyParameter(int enemy_id = 0, int unit_id = 0, string name = "", int level = 0, int rarity = 0, int promotion_level = 0, long hp = 0L, int atk = 0, int magic_str = 0, int def = 0, int magic_def = 0, int physical_critical = 0, int magic_critical = 0, int wave_hp_recovery = 0, int wave_energy_recovery = 0, int dodge = 0, int physical_penetrate = 0, int magic_penetrate = 0, int life_steal = 0, int hp_recovery_rate = 0, int energy_recovery_rate = 0, int energy_reduce_rate = 0, int union_burst_level = 0, int main_skill_lv_1 = 0, int main_skill_lv_2 = 0, int main_skill_lv_3 = 0, int main_skill_lv_4 = 0, int main_skill_lv_5 = 0, int main_skill_lv_6 = 0, int main_skill_lv_7 = 0, int main_skill_lv_8 = 0, int main_skill_lv_9 = 0, int main_skill_lv_10 = 0, int ex_skill_lv_1 = 0, int ex_skill_lv_2 = 0, int ex_skill_lv_3 = 0, int ex_skill_lv_4 = 0, int ex_skill_lv_5 = 0, int resist_status_id = 0, int resist_variation_id = 0, int accuracy = 0, int unique_equipment_flag_1 = 0, int break_durability = 0, long virtual_hp = 0L)
			{
				_enemy_id = enemy_id;
				_unit_id = unit_id;
				_name = name;
				_level = level;
				_rarity = rarity;
				_promotion_level = promotion_level;
				_hp = hp;
				_atk = atk;
				_magic_str = magic_str;
				_def = def;
				_magic_def = magic_def;
				_physical_critical = physical_critical;
				_magic_critical = magic_critical;
				_wave_hp_recovery = wave_hp_recovery;
				_wave_energy_recovery = wave_energy_recovery;
				_dodge = dodge;
				_physical_penetrate = physical_penetrate;
				_magic_penetrate = magic_penetrate;
				_life_steal = life_steal;
				_hp_recovery_rate = hp_recovery_rate;
				_energy_recovery_rate = energy_recovery_rate;
				_energy_reduce_rate = energy_reduce_rate;
				_union_burst_level = union_burst_level;
				_main_skill_lv_1 = main_skill_lv_1;
				_main_skill_lv_2 = main_skill_lv_2;
				_main_skill_lv_3 = main_skill_lv_3;
				_main_skill_lv_4 = main_skill_lv_4;
				_main_skill_lv_5 = main_skill_lv_5;
				_main_skill_lv_6 = main_skill_lv_6;
				_main_skill_lv_7 = main_skill_lv_7;
				_main_skill_lv_8 = main_skill_lv_8;
				_main_skill_lv_9 = main_skill_lv_9;
				_main_skill_lv_10 = main_skill_lv_10;
				_ex_skill_lv_1 = ex_skill_lv_1;
				_ex_skill_lv_2 = ex_skill_lv_2;
				_ex_skill_lv_3 = ex_skill_lv_3;
				_ex_skill_lv_4 = ex_skill_lv_4;
				_ex_skill_lv_5 = ex_skill_lv_5;
				_resist_status_id = resist_status_id;
				_resist_variation_id = resist_variation_id;
				_accuracy = accuracy;
				_unique_equipment_flag_1 = unique_equipment_flag_1;
				_break_durability = break_durability;
				_virtual_hp = virtual_hp;
			}
		}
		[global::OpenPCR.Migration("e300e20e-9174-47ce-92e8-0b3193562178")]
		protected EnemyParameter Get(int enemy_id)
		{
			if (!_lazyPrimaryKeyDictionary.TryGetValue(enemy_id, out EnemyParameter value))
			{
				value = _db != null ? _SelectOne(enemy_id) : null;
				_lazyPrimaryKeyDictionary[enemy_id] = value;
			}
			return value;
		}

		[global::OpenPCR.Migration("dd90e6a7-a077-41aa-a0ff-97f765ab0f6b")]
		private EnemyParameter _SelectOne(int enemy_id)
		{
			if (_db == null)
				throw new System.NullReferenceException();
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterEnemyParameter::_SelectOne(Int32): Native MasterEnemyDatabase.GetSelectQuery_EnemyParameter, BindInt/Step/record production requires the SQLite master provider outside prepared battle input construction.");
		}

		private MasterEnemyDatabase _db;
		private Dictionary<int, EnemyParameter> _lazyPrimaryKeyDictionary;
		public MasterEnemyParameter(MasterEnemyDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, EnemyParameter>();
			_db = db;
		}

		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
		}
	}
}
