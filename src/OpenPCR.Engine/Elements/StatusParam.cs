using System.Runtime.CompilerServices;

namespace Elements
{
	public class StatusParam
	{
		public global::System.Int64 Hp { get; private set; }
		public global::System.Int32 Atk { get; private set; }
		public global::System.Int32 Def { get; private set; }
		public global::System.Int32 MagicStr { get; private set; }
		public global::System.Int32 MagicDef { get; private set; }
		public global::System.Int32 PhysicalCritical { get; private set; }
		public global::System.Int32 MagicCritical { get; private set; }
		public global::System.Int32 WaveHpRecovery { get; private set; }
		public global::System.Int32 WaveEnergyRecovery { get; private set; }
		public global::System.Int32 HpRecoveryRate { get; private set; }
		public global::System.Int32 PhysicalPenetrate { get; private set; }
		public global::System.Int32 MagicPenetrate { get; private set; }
		public global::System.Int32 LifeSteal { get; private set; }
		public global::System.Int32 Dodge { get; private set; }
		public global::System.Int32 EnergyReduceRate { get; private set; }
		public global::System.Int32 EnergyRecoveryRate { get; private set; }
		public global::System.Int32 Accuracy { get; private set; }

		[global::OpenPCR.Migration("1866df77-54d4-4554-84a9-bf6222921020")]
		public StatusParam()
		{
			initializeStatusParam();
		}

		[global::OpenPCR.Migration("e059f3e5-344c-4801-9c34-19d5aa4f18d9")]
		public void SetHp(long _hp)
		{
			Hp = _hp;
		}

		[global::OpenPCR.Migration("5106afd4-b6ee-4066-8278-81a703d31392")]
		public void SetAtk(int _atk)
		{
			Atk = _atk;
		}

		[global::OpenPCR.Migration("a56fb0e9-7665-49df-b148-b2ca1b1ca8a5")]
		public void SetDef(int _def)
		{
			Def = _def;
		}

		[global::OpenPCR.Migration("bbeff5cb-3b63-491e-9565-c41813d590b5")]
		public void SetMagicStr(int _magicStr)
		{
			MagicStr = _magicStr;
		}

		[global::OpenPCR.Migration("01e6b3d8-778f-4d0e-9b35-494cf26c9fb2")]
		public void SetMagicDef(int _magicDef)
		{
			MagicDef = _magicDef;
		}

		[global::OpenPCR.Migration("f6b55103-16fa-4e8c-b0ea-9bfbabb941b3")]
		public void SetPhysicalCritical(int _physicalCritical)
		{
			PhysicalCritical = _physicalCritical;
		}

		[global::OpenPCR.Migration("3e4bffbe-d6bd-43e9-ac66-758336a5a97c")]
		public void SetMagicCritical(int _magicCritical)
		{
			MagicCritical = _magicCritical;
		}

		[global::OpenPCR.Migration("4692fba9-3b2a-4b2d-8d1e-8dd2e595fdae")]
		public void SetWaveHpRecovery(int _waveHpRecovery)
		{
			WaveHpRecovery = _waveHpRecovery;
		}

		[global::OpenPCR.Migration("0d373e74-bde1-4434-8154-33f4d2b0c1ec")]
		public void SetWaveEnergyRecovery(int _waveEnergyRecovery)
		{
			WaveEnergyRecovery = _waveEnergyRecovery;
		}

		[global::OpenPCR.Migration("09f61e49-502f-41a8-aa47-b054aea2fdd4")]
		public void SetHpRecoveryRate(int _hpRecoveryRate)
		{
			HpRecoveryRate = _hpRecoveryRate;
		}

		[global::OpenPCR.Migration("65b15cbf-98aa-4aa7-8a25-5e4370c86ed1")]
		public void SetPhysicalPenetrate(int _physicalPenetrate)
		{
			PhysicalPenetrate = _physicalPenetrate;
		}

		[global::OpenPCR.Migration("53267d57-73a0-44e4-9f75-dcb4a2dc54a4")]
		public void SetMagicPenetrate(int _magicPenetrate)
		{
			MagicPenetrate = _magicPenetrate;
		}

		[global::OpenPCR.Migration("d5f8aa9c-854f-4ff3-997a-216588c200de")]
		public void SetLifeSteal(int _lifeSteal)
		{
			LifeSteal = _lifeSteal;
		}

		[global::OpenPCR.Migration("9a81d08b-624b-4814-b948-52b39ab8c7c3")]
		public void SetDodge(int _dodge)
		{
			Dodge = _dodge;
		}

		[global::OpenPCR.Migration("c5415435-f669-4110-b38e-94c988dba3d2")]
		public void SetEnergyReduceRate(int _energyReduceRate)
		{
			EnergyReduceRate = _energyReduceRate;
		}

		[global::OpenPCR.Migration("71b072af-bef9-42a0-8cd6-1f276bb4ccc9")]
		public void SetEnergyRecoveryRate(int _energyRecoveryRate)
		{
			EnergyRecoveryRate = _energyRecoveryRate;
		}

		[global::OpenPCR.Migration("8f4b651a-2564-4ccd-9652-826822c6ca8b")]
		public void SetAccuracy(int _accuracy)
		{
			Accuracy = _accuracy;
		}

		[global::OpenPCR.Migration("c83a4a06-36b3-4267-8cd1-999e84016132")]
		private void initializeStatusParam()
		{
			Hp = 0L;
			Atk = 0;
			Def = 0;
			MagicStr = 0;
			MagicDef = 0;
			PhysicalCritical = 0;
			MagicCritical = 0;
			WaveHpRecovery = 0;
			WaveEnergyRecovery = 0;
			HpRecoveryRate = 0;
			PhysicalPenetrate = 0;
			MagicPenetrate = 0;
			LifeSteal = 0;
			Dodge = 0;
			EnergyReduceRate = 0;
			EnergyRecoveryRate = 0;
			Accuracy = 0;
		}

		[global::OpenPCR.Migration("c60ffb81-5458-4916-9381-b0ed249a9cf1")]
		public long GetParam(eParamType _paramType)
		{
			return _paramType switch
			{
				eParamType.HP => Hp,
				eParamType.ATK => (int)Atk,
				eParamType.DEF => (int)Def,
				eParamType.MAGIC_ATK => (int)MagicStr,
				eParamType.MAGIC_DEF => (int)MagicDef,
				eParamType.PHYSICAL_CRITICAL => (int)PhysicalCritical,
				eParamType.MAGIC_CRITICAL => (int)MagicCritical,
				eParamType.WAVE_HP_RECOVERY => (int)WaveHpRecovery,
				eParamType.WAVE_ENERGY_RECOVERY => (int)WaveEnergyRecovery,
				eParamType.HP_RECOVERY_RATE => (int)HpRecoveryRate,
				eParamType.PHYSICAL_PENETRATE => (int)PhysicalPenetrate,
				eParamType.MAGIC_PENETRATE => (int)MagicPenetrate,
				eParamType.LIFE_STEAL => (int)LifeSteal,
				eParamType.DODGE => (int)Dodge,
				eParamType.ENERGY_RECOVERY_RATE => (int)EnergyRecoveryRate,
				eParamType.ENERGY_REDUCE_RATE => (int)EnergyReduceRate,
				eParamType.ACCURACY => (int)Accuracy,
				_ => 0L,
			};
		}
	}
}
