using global::OpenPCR.DbStubs;
using System.Collections.Generic;
using System.Linq;

namespace Elements.Data
{
	public class MasterExEquipmentData : AbstractMasterData
	{
		public class ExEquipmentData
		{
			protected global::System.Int32 _ex_equipment_id;
			protected global::System.String _name;
			protected global::System.String _description;
			protected global::System.Int32 _rarity;
			protected global::System.Int32 _category;
			protected global::System.Int32 _restriction_id;
			protected global::System.Int32 _clan_battle_equip_flag;
			protected global::System.Boolean _is_force_protected;
			protected global::System.Boolean _max_rank_flag;
			protected global::System.Int32 _talent_id;
			protected global::System.Int32 _DefaultHp;
			protected global::System.Int32 _MaxHp;
			protected global::System.Int32 _DefaultAtk;
			protected global::System.Int32 _MaxAtk;
			protected global::System.Int32 _DefaultMagicStr;
			protected global::System.Int32 _MaxMagicStr;
			protected global::System.Int32 _DefaultDef;
			protected global::System.Int32 _MaxDef;
			protected global::System.Int32 _DefaultMagicDef;
			protected global::System.Int32 _MaxMagicDef;
			protected global::System.Int32 _DefaultCritical;
			protected global::System.Int32 _MaxCritical;
			protected global::System.Int32 _DefaultMagicCritical;
			protected global::System.Int32 _MaxMagicCritical;
			protected global::System.Int32 _DefaultWaveHpRecovery;
			protected global::System.Int32 _MaxWaveHpRecovery;
			protected global::System.Int32 _DefaultWaveEnergyRecovery;
			protected global::System.Int32 _MaxWaveEnergyRecovery;
			protected global::System.Int32 _DefaultDodge;
			protected global::System.Int32 _MaxDodge;
			protected global::System.Int32 _DefaultPenetrate;
			protected global::System.Int32 _MaxPenetrate;
			protected global::System.Int32 _DefaultMagicPenetrate;
			protected global::System.Int32 _MaxMagicPenetrate;
			protected global::System.Int32 _DefaultLifeSteal;
			protected global::System.Int32 _MaxLifeSteal;
			protected global::System.Int32 _DefaultHpRecoveryRate;
			protected global::System.Int32 _MaxHpRecoveryRate;
			protected global::System.Int32 _DefaultEnergyRecoveryRate;
			protected global::System.Int32 _MaxEnergyRecoveryRate;
			protected global::System.Int32 _DefaultEnergyReduceRate;
			protected global::System.Int32 _MaxEnergyReduceRate;
			protected global::System.Int32 _DefaultAccuracy;
			protected global::System.Int32 _MaxAccuracy;
			protected global::System.Int32 _Skill1;
			protected global::System.Int32 _Skill2;
			protected global::System.Int32 _SkillPower;
			[global::OpenPCR.ImplicitMigration("da5acfbe-4f5c-4e82-b7ad-77d8eaa40d1f")]
			public global::System.Int32 Skill1 => _Skill1;
			[global::OpenPCR.ImplicitMigration("feac0375-7116-40c5-9c73-fb08a783ab8d")]
			public global::System.Int32 Skill2 => _Skill2;

			public ExEquipmentData(int ex_equipment_id = 0, string name = "", string description = "", int rarity = 0, int category = 0, int restriction_id = 0, int clan_battle_equip_flag = 0, bool is_force_protected = false, bool max_rank_flag = false, int talent_id = 0, int DefaultHp = 0, int MaxHp = 0, int DefaultAtk = 0, int MaxAtk = 0, int DefaultMagicStr = 0, int MaxMagicStr = 0, int DefaultDef = 0, int MaxDef = 0, int DefaultMagicDef = 0, int MaxMagicDef = 0, int DefaultCritical = 0, int MaxCritical = 0, int DefaultMagicCritical = 0, int MaxMagicCritical = 0, int DefaultWaveHpRecovery = 0, int MaxWaveHpRecovery = 0, int DefaultWaveEnergyRecovery = 0, int MaxWaveEnergyRecovery = 0, int DefaultDodge = 0, int MaxDodge = 0, int DefaultPenetrate = 0, int MaxPenetrate = 0, int DefaultMagicPenetrate = 0, int MaxMagicPenetrate = 0, int DefaultLifeSteal = 0, int MaxLifeSteal = 0, int DefaultHpRecoveryRate = 0, int MaxHpRecoveryRate = 0, int DefaultEnergyRecoveryRate = 0, int MaxEnergyRecoveryRate = 0, int DefaultEnergyReduceRate = 0, int MaxEnergyReduceRate = 0, int DefaultAccuracy = 0, int MaxAccuracy = 0, int Skill1 = 0, int Skill2 = 0, int SkillPower = 0)
			{
				_ex_equipment_id = ex_equipment_id;
				_name = name;
				_description = description;
				_rarity = rarity;
				_category = category;
				_restriction_id = restriction_id;
				_clan_battle_equip_flag = clan_battle_equip_flag;
				_is_force_protected = is_force_protected;
				_max_rank_flag = max_rank_flag;
				_talent_id = talent_id;
				_DefaultHp = DefaultHp;
				_MaxHp = MaxHp;
				_DefaultAtk = DefaultAtk;
				_MaxAtk = MaxAtk;
				_DefaultMagicStr = DefaultMagicStr;
				_MaxMagicStr = MaxMagicStr;
				_DefaultDef = DefaultDef;
				_MaxDef = MaxDef;
				_DefaultMagicDef = DefaultMagicDef;
				_MaxMagicDef = MaxMagicDef;
				_DefaultCritical = DefaultCritical;
				_MaxCritical = MaxCritical;
				_DefaultMagicCritical = DefaultMagicCritical;
				_MaxMagicCritical = MaxMagicCritical;
				_DefaultWaveHpRecovery = DefaultWaveHpRecovery;
				_MaxWaveHpRecovery = MaxWaveHpRecovery;
				_DefaultWaveEnergyRecovery = DefaultWaveEnergyRecovery;
				_MaxWaveEnergyRecovery = MaxWaveEnergyRecovery;
				_DefaultDodge = DefaultDodge;
				_MaxDodge = MaxDodge;
				_DefaultPenetrate = DefaultPenetrate;
				_MaxPenetrate = MaxPenetrate;
				_DefaultMagicPenetrate = DefaultMagicPenetrate;
				_MaxMagicPenetrate = MaxMagicPenetrate;
				_DefaultLifeSteal = DefaultLifeSteal;
				_MaxLifeSteal = MaxLifeSteal;
				_DefaultHpRecoveryRate = DefaultHpRecoveryRate;
				_MaxHpRecoveryRate = MaxHpRecoveryRate;
				_DefaultEnergyRecoveryRate = DefaultEnergyRecoveryRate;
				_MaxEnergyRecoveryRate = MaxEnergyRecoveryRate;
				_DefaultEnergyReduceRate = DefaultEnergyReduceRate;
				_MaxEnergyReduceRate = MaxEnergyReduceRate;
				_DefaultAccuracy = DefaultAccuracy;
				_MaxAccuracy = MaxAccuracy;
				_Skill1 = Skill1;
				_Skill2 = Skill2;
				_SkillPower = SkillPower;
			}
		}

		private MasterInventoryDatabase _db;
		private Dictionary<int, ExEquipmentData> _lazyPrimaryKeyDictionary;
		private Dictionary<int, ExEquipmentData> _strictPrimaryKeyDictionary;
		public MasterExEquipmentData(MasterInventoryDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<int, ExEquipmentData>();
			_db = db;
		}

		[global::OpenPCR.Migration("d788ef9c-1577-4343-8d0f-98990ef1a842")]
		public ExEquipmentData Get(int ex_equipment_id)
		{
			ExEquipmentData value = null;
			if (_strictPrimaryKeyDictionary != null)
			{
				_strictPrimaryKeyDictionary.TryGetValue(ex_equipment_id, out value);
				return value;
			}

			if (!_lazyPrimaryKeyDictionary.TryGetValue(ex_equipment_id, out value))
			{
				value = _db != null ? _SelectOne(ex_equipment_id) : null;
				_lazyPrimaryKeyDictionary[ex_equipment_id] = value;
			}

			return value;
		}

		[global::OpenPCR.Migration("dc4f0ea2-c4f7-42a2-9fa9-1f52f0c4ae3d")]
		private ExEquipmentData _SelectOne(int ex_equipment_id)
		{
			PreparedQuery query = _db.GetSelectQuery_ExEquipmentData();
			if (query == null)
				return null;
			if (!query.BindInt(1, ex_equipment_id))
				return null;
			ExEquipmentData result = null;
			if (query.Step())
				result = _CreateCachedOrmByQueryResult(query);
			query.Reset();
			return result;
		}

		private ExEquipmentData _CreateCachedOrmByQueryResult(Query query)
		{
			int key = query.GetInt(0);
			ExEquipmentData value = null;
			if (_strictPrimaryKeyDictionary != null)
			{
				_strictPrimaryKeyDictionary.TryGetValue(key, out value);
				return value;
			}

			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out value))
			{
				string name = query.GetText(1);
				string description = query.GetText(2);
				int rarity = query.GetInt(3);
				int category = query.GetInt(4);
				int restriction_id = query.GetInt(5);
				int clan_battle_equip_flag = query.GetInt(6);
				bool is_force_protected = query.GetInt(7) == 1;
				bool max_rank_flag = query.GetInt(8) == 1;
				int talent_id = query.GetInt(9);
				int DefaultHp = query.GetInt(10);
				int MaxHp = query.GetInt(11);
				int DefaultAtk = query.GetInt(12);
				int MaxAtk = query.GetInt(13);
				int DefaultMagicStr = query.GetInt(14);
				int MaxMagicStr = query.GetInt(15);
				int DefaultDef = query.GetInt(16);
				int MaxDef = query.GetInt(17);
				int DefaultMagicDef = query.GetInt(18);
				int MaxMagicDef = query.GetInt(19);
				int DefaultCritical = query.GetInt(20);
				int MaxCritical = query.GetInt(21);
				int DefaultMagicCritical = query.GetInt(22);
				int MaxMagicCritical = query.GetInt(23);
				int DefaultWaveHpRecovery = query.GetInt(24);
				int MaxWaveHpRecovery = query.GetInt(25);
				int DefaultWaveEnergyRecovery = query.GetInt(26);
				int MaxWaveEnergyRecovery = query.GetInt(27);
				int DefaultDodge = query.GetInt(28);
				int MaxDodge = query.GetInt(29);
				int DefaultPenetrate = query.GetInt(30);
				int MaxPenetrate = query.GetInt(31);
				int DefaultMagicPenetrate = query.GetInt(32);
				int MaxMagicPenetrate = query.GetInt(33);
				int DefaultLifeSteal = query.GetInt(34);
				int MaxLifeSteal = query.GetInt(35);
				int DefaultHpRecoveryRate = query.GetInt(36);
				int MaxHpRecoveryRate = query.GetInt(37);
				int DefaultEnergyRecoveryRate = query.GetInt(38);
				int MaxEnergyRecoveryRate = query.GetInt(39);
				int DefaultEnergyReduceRate = query.GetInt(40);
				int MaxEnergyReduceRate = query.GetInt(41);
				int DefaultAccuracy = query.GetInt(42);
				int MaxAccuracy = query.GetInt(43);
				int Skill1 = query.GetInt(44);
				int Skill2 = query.GetInt(45);
				int SkillPower = query.GetInt(46);
				value = new ExEquipmentData(key, name, description, rarity, category, restriction_id, clan_battle_equip_flag, is_force_protected, max_rank_flag, talent_id, DefaultHp, MaxHp, DefaultAtk, MaxAtk, DefaultMagicStr, MaxMagicStr, DefaultDef, MaxDef, DefaultMagicDef, MaxMagicDef, DefaultCritical, MaxCritical, DefaultMagicCritical, MaxMagicCritical, DefaultWaveHpRecovery, MaxWaveHpRecovery, DefaultWaveEnergyRecovery, MaxWaveEnergyRecovery, DefaultDodge, MaxDodge, DefaultPenetrate, MaxPenetrate, DefaultMagicPenetrate, MaxMagicPenetrate, DefaultLifeSteal, MaxLifeSteal, DefaultHpRecoveryRate, MaxHpRecoveryRate, DefaultEnergyRecoveryRate, MaxEnergyRecoveryRate, DefaultEnergyReduceRate, MaxEnergyReduceRate, DefaultAccuracy, MaxAccuracy, Skill1, Skill2, SkillPower);
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
