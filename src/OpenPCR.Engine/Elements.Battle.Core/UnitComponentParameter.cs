using System;
using global::OpenPCR.Foundation;
using Elements.Battle;

namespace Elements.Battle.Core
{
	public class UnitComponentParameter
	{
		public global::System.Int64 StartHpWithoutPassive { get; set; } = 0L;
		public global::System.Int64 StartMaxHP { get; set; } = 0L;
		public global::System.Int64 StartAtk { get; set; } = 0L;
		public global::System.Int64 StartMagicStr { get; set; } = 0L;
		public global::System.Int64 StartDef { get; set; } = 0L;
		public global::System.Int64 StartMagicDef { get; set; } = 0L;
		public global::System.Int64 StartDodge { get; set; } = 0L;
		public global::System.Int64 StartAccuracy { get; set; } = 0L;
		public global::System.Int64 StartPhysicalCritical { get; set; } = 0L;
		public global::System.Int64 StartMagicCritical { get; set; } = 0L;
		public global::System.Int64 StartEnergyRecoveryRate { get; set; } = 0L;
		public global::System.Int64 StartLifeSteal { get; set; } = 0L;
		public global::System.Single StartMoveSpeed { get; set; } = 0f;
		public global::System.Int64 StartWaveHpRecovery { get; set; } = 0L;
		public global::System.Int64 StartWaveEnergyRecovery { get; set; } = 0L;
		public global::System.Int64 StartPhysicalPenetrate { get; set; } = 0L;
		public global::System.Int64 StartMagicPenetrate { get; set; } = 0L;
		public global::System.Int64 StartHpRecoveryRate { get; set; } = 0L;
		public global::System.Int64 StartEnergyReduceRate { get; set; } = 0L;
		public global::System.Int64 StartPhysicalCriticalDamageRate { get; set; } = 0L;
		public global::System.Int64 StartMagicCriticalDamageRate { get; set; } = 0L;
		public global::System.Int64 StartPhysicalCriticalDamagePermyriadByRole { get; set; } = 0L;
		public global::System.Int64 StartMagicCriticalDamagePermyriadByRole { get; set; } = 0L;
		public global::System.Int64 StartReceiveCriticalDamageRate { get; set; } = 0L;
		public global::System.Int64 StartPhysicalAndMagicReceiveDamagePercent { get; set; } = 0L;
		public global::System.Int64 StartPhysicalReceiveDamagePercent { get; set; } = 0L;
		public global::System.Int64 StartMagicReceiveDamagePercent { get; set; } = 0L;
		public global::System.Int64 StartPhysicalDamageUpPercent { get; set; } = 0L;
		public global::System.Int64 StartMagicDamageUpPercent { get; set; } = 0L;
		public global::System.Int64 StartAdditionalPhysicalDamageUpPermyriadByRole { get; set; } = 0L;
		public global::System.Int64 StartAdditionalMagicDamageUpPermyriadByRole { get; set; } = 0L;
		public global::System.Int64 StartAbnormalStateDodgeHit { get; set; } = 0L;
		public global::System.Int64 StartAbnormalStateDodgeResist { get; set; } = 0L;
		public global::System.Int64 StartTalentFormationBonus { get; set; } = 0L;
		public global::System.Int64 StartTalentBonusEnhancePercent { get; set; } = 0L;
		public global::System.Int64 StartAdditionalAttackEnhance { get; set; } = 0L;
		public global::System.Int64 StartBuffEnhance { get; set; } = 0L;
		public global::System.Int64 StartDebuffEnhance { get; set; } = 0L;
		public global::System.Int64 StartHealEnhance { get; set; } = 0L;
		public global::System.Double StartEnergyCharge { get; set; } = 0d;
		[global::OpenPCR.Migration("d2d0092e-f279-4f7e-95cc-049d49acfa6d")] public global::System.Int64 StartMaxHpForDamagedEnergy { [global::OpenPCR.Migration("91efe46b-2905-4b0c-9207-0f09fbe41301")] get; [global::OpenPCR.Migration("94333dec-027a-4aae-a273-109aa485fe58")] set; } = 0L;
		public global::System.Int64 DefForDamagedEnergy { get; set; } = 0L;
		public global::System.Int64 MagicDefForDamagedEnergy { get; set; } = 0L;
		public global::System.Int32 Level { get; set; } = 0;
		public global::System.Int64 Hp { get; set; } = 0L;
		public global::System.Int64 MaxHp { get; set; } = 0L;
		public global::System.Int64 Atk { get; set; } = 0L;
		public global::System.Int64 MagicStr { get; set; } = 0L;
		public global::System.Int64 Def { get; set; } = 0L;
		public global::System.Int64 MagicDef { get; set; } = 0L;
		public global::System.Int64 Dodge { get; set; } = 0L;
		public global::System.Int64 Accuracy { get; set; } = 0L;
		public global::System.Int64 PhysicalCritical { get; set; } = 0L;
		public global::System.Int64 MagicCritical { get; set; } = 0L;
		public global::System.Int64 WaveHpRecovery { get; set; } = 0L;
		public global::System.Int64 WaveEnergyRecovery { get; set; } = 0L;
		public global::System.Int64 EnergyRecoveryRate { get; set; } = 0L;
		public global::System.Int64 LifeSteal { get; set; } = 0L;
		public global::System.Int64 PhysicalPenetrate { get; set; } = 0L;
		public global::System.Int64 MagicPenetrate { get; set; } = 0L;
		public global::System.Int64 HpRecoveryRate { get; set; } = 0L;
		public global::System.Int64 EnergyReduceRate { get; set; } = 0L;
		public global::System.Int64 PhysicalCriticalDamageRate { get; set; } = 0L;
		public global::System.Int64 PhysicalCriticalDamagePermyriad { get; set; } = 0L;
		public global::System.Int64 MagicCriticalDamageRate { get; set; } = 0L;
		public global::System.Int64 MagicCriticalDamagePermyriad { get; set; } = 0L;
		public global::System.Int64 ReceiveCriticalDamageRate { get; set; } = 0L;
		public global::System.Int64 AdditionalPhysicalAndMagicReceiveDamagePercent { get; set; } = 0L;
		public global::System.Int64 AdditionalPhysicalReceiveDamagePercent { get; set; } = 0L;
		public global::System.Int64 AdditionalMagicReceiveDamagePercent { get; set; } = 0L;
		public global::System.Int64 AdditionalPhysicalDamageUpPercent { get; set; } = 0L;
		public global::System.Int64 AdditionalPhysicalDamageUpPermyriad { get; set; } = 0L;
		public global::System.Int64 AdditionalMagicDamageUpPercent { get; set; } = 0L;
		public global::System.Int64 AdditionalMagicDamageUpPermyriad { get; set; } = 0L;
		public global::System.Int64 AbnormalStateDodgeHit { get; set; } = 0L;
		public global::System.Int64 AbnormalStateDodgeResist { get; set; } = 0L;
		public global::System.Int64 AdditionalTalentBonusPermyriad { get; set; } = 0L;
		public global::System.Int64 TalentBonusEnhancePercent { get; set; } = 0L;
		public global::System.Int64 AdditionalAttackEnhance { get; set; } = 0L;
		public global::System.Int64 BuffEnhance { get; set; } = 0L;
		public global::System.Int64 DebuffEnhance { get; set; } = 0L;
		public global::System.Int64 HealEnhance { get; set; } = 0L;
		public global::System.Double EnergyCharge { get; set; } = 0d;
		public global::System.Single MoveSpeed { get; set; } = 0f;
		public global::System.Int64 MaxHpAfterPassive { get; set; } = 0L;
		[global::OpenPCR.Migration("e02d9788-766a-490f-b665-e4c451e00778")] public double Energy { [global::OpenPCR.Migration("328a34b7-97d6-412c-9fee-1ecd03e98154")] get; [global::OpenPCR.Migration("c7aa8c18-72d4-4183-a600-71872b59b89e")] private set; }
		public Action<float> OnEnergyChange { get; set; }
		private Action<long> onHpChanged { get; set; }
		private UnitComponentLog componentLog { get; set; }

		[global::OpenPCR.ImplicitMigration("fb8d05f1-e10e-409c-a619-b1a45101df49")][global::OpenPCR.Migration("4d83c92e-8b2c-419b-ace4-0d812441ad8e")] public float EnegryAmount => (float)Energy / 1000f;
		[global::OpenPCR.ImplicitMigration("cdf81501-20a0-4cf5-97c8-a7edb13e0342")][global::OpenPCR.Migration("0c0a4fdc-394d-4257-91c2-4608447dc77f")] public float LifeAmount => global::OpenPCR.Presentation.Noop<float>();
		[global::OpenPCR.ImplicitMigration("64837f6a-c17b-4df2-af2b-1cea23a83983")][global::OpenPCR.Migration("90f62942-35d0-4232-960d-6295ad1d3052")] public float MaxLifeAmount => (float)(long)MaxHp / (float)(long)MaxHpAfterPassive;

		[global::OpenPCR.Migration("f674745c-5cdc-4984-ad60-ac000167bf83")]
		public UnitComponentParameter(Action<long> _onHpChanged)
		{
			onHpChanged = _onHpChanged;
		}

		[global::OpenPCR.Migration("cc877871-5c31-4e41-ad22-fc9be2229846")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentLog = _container.ComponentLog;
		}

		[global::OpenPCR.Migration("09edec91-3ace-4bb0-ad5f-a79ec6c6d873")]
		public void InitializeParameter(global::System.Int64 _hp, global::System.Int64 _atk, global::System.Int64 _def, global::System.Int64 _magicStr, global::System.Int64 _magicDef, global::System.Int64 _waveHpRecovery, global::System.Int64 _waveEnergyRecovery, global::System.Int64 _physicalCritical, global::System.Int64 _magicCritical, global::System.Int64 _dodge, global::System.Int64 _accuracy, global::System.Int64 _lifeSteal, global::System.Int64 _physicalPenetrate, global::System.Int64 _magicPenetrate, global::System.Int64 _hpRecoveryRate, global::System.Int64 _energyRecoveryRate, global::System.Int64 _energyReduceRate, global::System.Int64 _startMaxHpForDamagedEnergy, global::System.Int64 _defForDamagedEnergy, global::System.Int64 _magicDefForDamagedEnergy, global::System.Int64 _physicalCriticalDamageRate, global::System.Int64 _magicCriticalDamageRate, global::System.Int64 _physicalCriticalDamagePermyriadByRole, global::System.Int64 _magicCriticalDamagePermyriadByRole, global::System.Int64 _receiveCriticalDamageRate, global::System.Int64 _additionalPhysicalAndMagicReceiveDamagePercent, global::System.Int64 _additionalPhysicalReceiveDamagePercent, global::System.Int64 _additionalMagicReceiveDamagePercent, global::System.Int64 _additionalPhysicalDamageUpPercent, global::System.Int64 _additionalMagicDamageUpPercent, global::System.Int64 _additionalPhysicalDamageUpPermyriadByRole, global::System.Int64 _additionalMagicDamageUpPermyriadByRole, global::System.Int64 _additionalTalentBonusPermyriad, global::System.Int64 _talentBonusEnhancePercent, global::System.Int64 _abnormalStateDodgeHit, global::System.Int64 _abnormalStateDodgeResist, global::System.Int64 _additionalAttackEnhance, global::System.Int64 _buffEnhance, global::System.Int64 _debuffEnhance, global::System.Int64 _healEnhance, global::System.Double _energyCharge, global::System.Single _moveSpeed)
		{
			StartMaxHP = _hp;
			MaxHp = _hp;
			Hp = _hp;
			Atk = _atk;
			StartAtk = _atk;
			Def = _def;
			StartDef = _def;
			MagicStr = _magicStr;
			StartMagicStr = _magicStr;
			MagicDef = _magicDef;
			StartMagicDef = _magicDef;
			WaveHpRecovery = _waveHpRecovery;
			StartWaveHpRecovery = _waveHpRecovery;
			WaveEnergyRecovery = _waveEnergyRecovery;
			StartWaveEnergyRecovery = _waveEnergyRecovery;
			PhysicalCritical = _physicalCritical;
			StartPhysicalCritical = _physicalCritical;
			MagicCritical = _magicCritical;
			StartMagicCritical = _magicCritical;
			Dodge = _dodge;
			StartDodge = _dodge;
			Accuracy = _accuracy;
			StartAccuracy = _accuracy;
			LifeSteal = _lifeSteal;
			StartLifeSteal = _lifeSteal;
			PhysicalPenetrate = _physicalPenetrate;
			StartPhysicalPenetrate = _physicalPenetrate;
			MagicPenetrate = _magicPenetrate;
			StartMagicPenetrate = _magicPenetrate;
			HpRecoveryRate = _hpRecoveryRate;
			StartHpRecoveryRate = _hpRecoveryRate;
			EnergyRecoveryRate = _energyRecoveryRate;
			StartEnergyRecoveryRate = _energyRecoveryRate;
			EnergyReduceRate = _energyReduceRate;
			StartEnergyReduceRate = _energyReduceRate;
			StartMaxHpForDamagedEnergy = _startMaxHpForDamagedEnergy;
			DefForDamagedEnergy = _defForDamagedEnergy;
			MagicDefForDamagedEnergy = _magicDefForDamagedEnergy;
			PhysicalCriticalDamageRate = _physicalCriticalDamageRate;
			StartPhysicalCriticalDamageRate = _physicalCriticalDamageRate;
			MagicCriticalDamageRate = _magicCriticalDamageRate;
			StartMagicCriticalDamageRate = _magicCriticalDamageRate;
			PhysicalCriticalDamagePermyriad = _physicalCriticalDamagePermyriadByRole;
			StartPhysicalCriticalDamagePermyriadByRole = _physicalCriticalDamagePermyriadByRole;
			MagicCriticalDamagePermyriad = _magicCriticalDamagePermyriadByRole;
			StartMagicCriticalDamagePermyriadByRole = _magicCriticalDamagePermyriadByRole;
			ReceiveCriticalDamageRate = _receiveCriticalDamageRate;
			StartReceiveCriticalDamageRate = _receiveCriticalDamageRate;
			AdditionalPhysicalAndMagicReceiveDamagePercent = _additionalPhysicalAndMagicReceiveDamagePercent;
			StartPhysicalAndMagicReceiveDamagePercent = _additionalPhysicalAndMagicReceiveDamagePercent;
			AdditionalPhysicalReceiveDamagePercent = _additionalPhysicalReceiveDamagePercent;
			StartPhysicalReceiveDamagePercent = _additionalPhysicalReceiveDamagePercent;
			AdditionalMagicReceiveDamagePercent = _additionalMagicReceiveDamagePercent;
			StartMagicReceiveDamagePercent = _additionalMagicReceiveDamagePercent;
			AdditionalPhysicalDamageUpPercent = _additionalPhysicalDamageUpPercent;
			StartPhysicalDamageUpPercent = _additionalPhysicalDamageUpPercent;
			AdditionalMagicDamageUpPercent = _additionalMagicDamageUpPercent;
			StartMagicDamageUpPercent = _additionalMagicDamageUpPercent;
			AdditionalPhysicalDamageUpPermyriad = _additionalPhysicalDamageUpPermyriadByRole;
			StartAdditionalPhysicalDamageUpPermyriadByRole = _additionalPhysicalDamageUpPermyriadByRole;
			AdditionalMagicDamageUpPermyriad = _additionalMagicDamageUpPermyriadByRole;
			StartAdditionalMagicDamageUpPermyriadByRole = _additionalMagicDamageUpPermyriadByRole;
			AdditionalTalentBonusPermyriad = _additionalTalentBonusPermyriad;
			StartTalentFormationBonus = _additionalTalentBonusPermyriad;
			TalentBonusEnhancePercent = _talentBonusEnhancePercent;
			StartTalentBonusEnhancePercent = _talentBonusEnhancePercent;
			AbnormalStateDodgeHit = _abnormalStateDodgeHit;
			StartAbnormalStateDodgeHit = _abnormalStateDodgeHit;
			AbnormalStateDodgeResist = _abnormalStateDodgeResist;
			StartAbnormalStateDodgeResist = _abnormalStateDodgeResist;
			AdditionalAttackEnhance = _additionalAttackEnhance;
			StartAdditionalAttackEnhance = _additionalAttackEnhance;
			BuffEnhance = _buffEnhance;
			StartBuffEnhance = _buffEnhance;
			DebuffEnhance = _debuffEnhance;
			StartDebuffEnhance = _debuffEnhance;
			HealEnhance = _healEnhance;
			StartHealEnhance = _healEnhance;
			EnergyCharge = _energyCharge;
			StartEnergyCharge = _energyCharge;
			MoveSpeed = _moveSpeed;
			StartMoveSpeed = _moveSpeed;
		}

		[global::OpenPCR.Migration("0d71fa59-3c69-4ec5-96d9-86046d7b85ce")]
		public void InitParamValue(bool _isFirst)
		{
			if (_isFirst)
				StartHpWithoutPassive = Hp;
			Atk = StartAtk;
			Def = StartDef;
			MagicStr = StartMagicStr;
			MagicDef = StartMagicDef;
			PhysicalCritical = StartPhysicalCritical;
			MagicCritical = StartMagicCritical;
			Dodge = StartDodge;
			LifeSteal = StartLifeSteal;
			WaveHpRecovery = StartWaveHpRecovery;
			WaveEnergyRecovery = StartWaveEnergyRecovery;
			PhysicalPenetrate = StartPhysicalPenetrate;
			MagicPenetrate = StartMagicPenetrate;
			HpRecoveryRate = StartHpRecoveryRate;
			EnergyRecoveryRate = StartEnergyRecoveryRate;
			EnergyReduceRate = StartEnergyReduceRate;
			Accuracy = StartAccuracy;
			PhysicalCriticalDamageRate = StartPhysicalCriticalDamageRate;
			MagicCriticalDamageRate = StartMagicCriticalDamageRate;
			PhysicalCriticalDamagePermyriad = StartPhysicalCriticalDamagePermyriadByRole;
			MagicCriticalDamagePermyriad = StartMagicCriticalDamagePermyriadByRole;
			AdditionalPhysicalDamageUpPercent = StartPhysicalDamageUpPercent;
			AdditionalMagicDamageUpPercent = StartMagicDamageUpPercent;
			AdditionalPhysicalDamageUpPermyriad = StartAdditionalPhysicalDamageUpPermyriadByRole;
			AdditionalMagicDamageUpPermyriad = StartAdditionalMagicDamageUpPermyriadByRole;
			AdditionalTalentBonusPermyriad = StartTalentFormationBonus;
			TalentBonusEnhancePercent = StartTalentBonusEnhancePercent;
			AbnormalStateDodgeResist = StartAbnormalStateDodgeResist;
			AbnormalStateDodgeHit = StartAbnormalStateDodgeHit;
			AdditionalAttackEnhance = StartAdditionalAttackEnhance;
			BuffEnhance = StartBuffEnhance;
			DebuffEnhance = StartDebuffEnhance;
			HealEnhance = StartHealEnhance;
			EnergyCharge = StartEnergyCharge;
		}

		[global::OpenPCR.Migration("33c1cae7-2de1-476d-bc93-c3f9b89c1231")]
		public void SetCurrentHp(long _value)
		{
			Hp = _value;
			onHpChanged.Call((long)Hp);
		}

		[global::OpenPCR.Migration("7d01be1d-15a2-4cd6-805d-6707cf7e0b85")]
		public void ApplyEnhanceValue(eParamType _buffType, long _value, bool _first, bool _isImmutableMaxHp, UnitDefine.eParameterEnhanceType _ehanceType)
		{
			switch (_buffType)
			{
				case eParamType.HP:
					if (_first && !_isImmutableMaxHp)
					{
						MaxHp = unchecked((long)MaxHp + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartMaxHP * _value) / 10000d)));
						Hp = BattleUtil.DoubleToLong((double)(long)StartHpWithoutPassive / (double)(long)StartMaxHP * (double)(long)MaxHp);
					}
					break;
				case eParamType.ATK:
					Atk = unchecked((long)Atk + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartAtk * _value) / 10000d)));
					break;
				case eParamType.DEF:
					Def = unchecked((long)Def + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartDef * _value) / 10000d)));
					break;
				case eParamType.MAGIC_ATK:
					MagicStr = unchecked((long)MagicStr + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartMagicStr * _value) / 10000d)));
					break;
				case eParamType.MAGIC_DEF:
					MagicDef = unchecked((long)MagicDef + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartMagicDef * _value) / 10000d)));
					break;
				case eParamType.PHYSICAL_CRITICAL:
					PhysicalCritical = unchecked((long)PhysicalCritical + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartPhysicalCritical * _value) / 10000d)));
					break;
				case eParamType.MAGIC_CRITICAL:
					MagicCritical = unchecked((long)MagicCritical + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartMagicCritical * _value) / 10000d)));
					break;
				case eParamType.WAVE_HP_RECOVERY:
					WaveHpRecovery = unchecked((long)WaveHpRecovery + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartWaveHpRecovery * _value) / 10000d)));
					break;
				case eParamType.WAVE_ENERGY_RECOVERY:
					WaveEnergyRecovery = unchecked((long)WaveEnergyRecovery + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartWaveEnergyRecovery * _value) / 10000d)));
					break;
				case eParamType.DODGE:
					Dodge = unchecked((long)Dodge + _value);
					break;
				case eParamType.LIFE_STEAL:
					LifeSteal = unchecked((long)LifeSteal + _value);
					break;
				case eParamType.ENERGY_RECOVERY_RATE:
					EnergyRecoveryRate = unchecked((long)EnergyRecoveryRate + _value);
					break;
				case eParamType.HP_RECOVERY_RATE:
					HpRecoveryRate = unchecked((long)HpRecoveryRate + _value);
					break;
				case eParamType.ENERGY_REDUCE_RATE:
					EnergyReduceRate = unchecked((long)EnergyReduceRate + _value);
					break;
				case eParamType.ACCURACY:
					Accuracy = unchecked((long)Accuracy + _value);
					break;
				case eParamType.TALENT_BONUS:
					AdditionalTalentBonusPermyriad = unchecked((long)AdditionalTalentBonusPermyriad + _value);
					break;
				case eParamType.ABNORMAL_STATE_DODGE_HIT:
					AbnormalStateDodgeHit = unchecked((long)AbnormalStateDodgeHit + _value);
					break;
				case eParamType.ABNORMAL_STATE_DODGE_RESIST:
					AbnormalStateDodgeResist = unchecked((long)AbnormalStateDodgeResist + _value);
					break;
				case eParamType.ATTACK_ENHANCE:
					AdditionalAttackEnhance = unchecked((long)AdditionalAttackEnhance + _value);
					break;
				case eParamType.BUFF_ENHANCE:
					BuffEnhance = unchecked((long)BuffEnhance + _value);
					break;
				case eParamType.DEBUFF_ENHANCE:
					DebuffEnhance = unchecked((long)DebuffEnhance + _value);
					break;
				case eParamType.HEAL_ENHANCE:
					HealEnhance = unchecked((long)HealEnhance + _value);
					break;
				case eParamType.PHYSICAL_PENETRATE:
					applyEnhancePhysicalPenetrate(_ehanceType, _value);
					break;
				case eParamType.MAGIC_PENETRATE:
					applyEnhanceMagicPenetrate(_ehanceType, _value);
					break;
				case eParamType.PHYSICAL_CRITICAL_DAMAGE_RATE:
					applyEnhancePhysicalCriticalDamageRate(_ehanceType, _value);
					break;
				case eParamType.MAGIC_CRITICAL_DAMAGE_RATE:
					applyEnhanceMagicCriticalDamageRate(_ehanceType, _value);
					break;
				case eParamType.PHYSICAL_DAMAGE_UP_PERCENT:
					applyEnhancePhysicalDamageUpPercent(_ehanceType, _value);
					break;
				case eParamType.MAGIC_DAMAGE_UP_PERCENT:
					applyEnhanceMagicDamageUpPercent(_ehanceType, _value);
					break;
				case eParamType.ENERGY_CHARGE:
					EnergyCharge = (double)_value / 10d + (double)EnergyCharge;
					break;
			}
		}

		[global::OpenPCR.Migration("1542cc3d-f53b-428a-93b3-c402f574640c")]
		private void applyEnhancePhysicalPenetrate(UnitDefine.eParameterEnhanceType _ehanceType, long _value)
		{
			if (((int)_ehanceType & ~1) == 2)
				PhysicalPenetrate = unchecked((long)PhysicalPenetrate + _value);
			else
				PhysicalPenetrate = unchecked((long)PhysicalPenetrate + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartPhysicalPenetrate * _value) / 10000d)));
		}

		[global::OpenPCR.Migration("d16087f5-ddc4-4f77-8a62-03265f0b7c38")]
		private void applyEnhanceMagicPenetrate(UnitDefine.eParameterEnhanceType _ehanceType, long _value)
		{
			if (((int)_ehanceType & ~1) == 2)
				MagicPenetrate = unchecked((long)MagicPenetrate + _value);
			else
				MagicPenetrate = unchecked((long)MagicPenetrate + BattleUtil.DoubleToLong(Math.Ceiling((double)unchecked((long)StartMagicPenetrate * _value) / 10000d)));
		}

		[global::OpenPCR.Migration("df12b47c-7b31-433a-b4e7-b186c745e2c6")]
		private void applyEnhancePhysicalCriticalDamageRate(UnitDefine.eParameterEnhanceType _ehanceType, long _value)
		{
			if (((int)_ehanceType | 2) == 3)
				PhysicalCriticalDamagePermyriad = unchecked((long)PhysicalCriticalDamagePermyriad + _value);
			else
				PhysicalCriticalDamageRate = unchecked((long)PhysicalCriticalDamageRate + _value);
		}

		[global::OpenPCR.Migration("879e2790-4404-4dd2-8da3-e942e95cb3a1")]
		private void applyEnhanceMagicCriticalDamageRate(UnitDefine.eParameterEnhanceType _ehanceType, long _value)
		{
			if (((int)_ehanceType | 2) == 3)
				MagicCriticalDamagePermyriad = unchecked((long)MagicCriticalDamagePermyriad + _value);
			else
				MagicCriticalDamageRate = unchecked((long)MagicCriticalDamageRate + _value);
		}

		[global::OpenPCR.Migration("963ec384-4dda-4d1f-9c90-69846632484a")]
		private void applyEnhancePhysicalDamageUpPercent(UnitDefine.eParameterEnhanceType _ehanceType, long _value)
		{
			if (((int)_ehanceType | 2) == 3)
				AdditionalPhysicalDamageUpPermyriad = unchecked((long)AdditionalPhysicalDamageUpPermyriad + _value);
			else
				AdditionalPhysicalDamageUpPercent = unchecked((long)AdditionalPhysicalDamageUpPercent + _value);
		}

		[global::OpenPCR.Migration("74f1296d-336b-4a84-9e16-efcf09bc62e7")]
		private void applyEnhanceMagicDamageUpPercent(UnitDefine.eParameterEnhanceType _ehanceType, long _value)
		{
			if (((int)_ehanceType | 2) == 3)
				AdditionalMagicDamageUpPermyriad = unchecked((long)AdditionalMagicDamageUpPermyriad + _value);
			else
				AdditionalMagicDamageUpPercent = unchecked((long)AdditionalMagicDamageUpPercent + _value);
		}

		[global::OpenPCR.Migration("e51c9c2c-d8d7-4828-8e8e-64e633a4b221")]
		public void SetEnergy(double _energy, eSetEnergyType _type, UnitCtrl _source = null)
		{
			componentLog.AppendEnergyLog(_energy, _type, _source);
			Energy = Math.Min((double)UnitDefine.MAX_ENERGY, Math.Max(0d, _energy));
			Energy = BattleUtil.DoubleToPerMile(Energy);
			if (OnEnergyChange != null)
				OnEnergyChange.Call(EnegryAmount);
		}
	}
}
