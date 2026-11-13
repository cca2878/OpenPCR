using System.Collections.Generic;

namespace Elements.Data
{
	public class MasterPromotionBonus : AbstractMasterData
	{
		private MasterUnitDatabase _db;
		private Dictionary<ulong, MasterPromotionBonus.PromotionBonus> _lazyPrimaryKeyDictionary;
		private Dictionary<int, List<MasterPromotionBonus.PromotionBonus>> _dictionaryOfListWithUnitId;
		public class PromotionBonus
		{
			protected global::System.Int32 _unit_id;
			protected global::System.Int32 _promotion_level;
			protected global::System.Double _Hp;
			protected global::System.Double _Attack;
			protected global::System.Double _MagicAttack;
			protected global::System.Double _Defense;
			protected global::System.Double _MagicDefense;
			protected global::System.Double _Critical;
			protected global::System.Double _MagicCritical;
			protected global::System.Double _WaveHpRecovery;
			protected global::System.Double _WaveEnergyRecovery;
			protected global::System.Double _Dodge;
			protected global::System.Double _Penetration;
			protected global::System.Double _MagicPenetration;
			protected global::System.Double _LifeSteal;
			protected global::System.Double _HpRecoveryRate;
			protected global::System.Double _EnergyRecoveryRate;
			protected global::System.Double _EnergyReduceRate;
			protected global::System.Double _Accuracy;
			public global::System.Int32 unit_id => _unit_id;
			public global::System.Int32 promotion_level => _promotion_level;
			public global::System.Double MagicCritical => _MagicCritical;
			public global::System.Double WaveHpRecovery => _WaveHpRecovery;
			public global::System.Double WaveEnergyRecovery => _WaveEnergyRecovery;
			public global::System.Double HpRecoveryRate => _HpRecoveryRate;
			public global::System.Double EnergyRecoveryRate => _EnergyRecoveryRate;
			public global::System.Double EnergyReduceRate => _EnergyReduceRate;
			public PromotionBonus(int unit_id = 0, int promotion_level = 0, double Hp = 0, double Attack = 0, double MagicAttack = 0, double Defense = 0, double MagicDefense = 0, double Critical = 0, double MagicCritical = 0, double WaveHpRecovery = 0, double WaveEnergyRecovery = 0, double Dodge = 0, double Penetration = 0, double MagicPenetration = 0, double LifeSteal = 0, double HpRecoveryRate = 0, double EnergyRecoveryRate = 0, double EnergyReduceRate = 0, double Accuracy = 0)
			{
				_unit_id = unit_id;
				_promotion_level = promotion_level;
				_Hp = Hp;
				_Attack = Attack;
				_MagicAttack = MagicAttack;
				_Defense = Defense;
				_MagicDefense = MagicDefense;
				_Critical = Critical;
				_MagicCritical = MagicCritical;
				_WaveHpRecovery = WaveHpRecovery;
				_WaveEnergyRecovery = WaveEnergyRecovery;
				_Dodge = Dodge;
				_Penetration = Penetration;
				_MagicPenetration = MagicPenetration;
				_LifeSteal = LifeSteal;
				_HpRecoveryRate = HpRecoveryRate;
				_EnergyRecoveryRate = EnergyRecoveryRate;
				_EnergyReduceRate = EnergyReduceRate;
				_Accuracy = Accuracy;
			}
		}

		public MasterPromotionBonus(MasterUnitDatabase db) : base(db)
		{
			_lazyPrimaryKeyDictionary = new Dictionary<ulong, PromotionBonus>();
			_dictionaryOfListWithUnitId = new Dictionary<int, List<PromotionBonus>>();
			_db = db;
		}
		public PromotionBonus Get(ulong key) => Get(unchecked((int)key), unchecked((int)(key >> 32)));
		[global::OpenPCR.Migration("cf4a1218-b3dd-43b4-8d3b-cd743ced24c8")]
		public PromotionBonus Get(int unit_id, int promotion_level)
		{
			ulong key = (uint)unit_id | ((ulong)(uint)promotion_level << 32);
			if (!_lazyPrimaryKeyDictionary.TryGetValue(key, out var value))
			{
				value = _db != null ? _SelectOne(unit_id, promotion_level) : null;
				_lazyPrimaryKeyDictionary[key] = value;
			}
			return value;
		}
		[global::OpenPCR.Migration("330ff6d0-87db-4cf5-84b1-4cf902d60224")]
		public bool HasKey(int unit_id, int promotion_level) => Get(unit_id, promotion_level) != null;
		[global::OpenPCR.Migration("e1f35c14-f365-4b7d-b575-5df840978a4c")]
		private PromotionBonus _SelectOne(int unit_id, int promotion_level)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.Data.MasterPromotionBonus._SelectOne: persistent MasterUnitDatabase growth-row production");
		}
		public override void Unload()
		{
			_lazyPrimaryKeyDictionary = null;
			_dictionaryOfListWithUnitId = null;
		}
	}
}
