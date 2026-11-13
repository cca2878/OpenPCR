using System;
using Elements.Battle;

namespace Elements.Battle.Core
{
	public class UnitComponentParameterClamped
	{
		[global::OpenPCR.ImplicitMigration("f05b73c4-c66d-475d-ac6b-8d34e25855de")][global::OpenPCR.Migration("5ca40567-9abf-4d99-bace-8594efa0236f")] public global::System.Int64 AtkZero => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.Atk) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.ATK)));
		[global::OpenPCR.ImplicitMigration("5026fa7d-f176-4453-a938-4ffac293cbe8")][global::OpenPCR.Migration("cc83d171-204b-4c5c-8782-dc2a16a3af11")] public global::System.Int64 MagicStrZero => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.MagicStr) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.MAGIC_STR)));
		[global::OpenPCR.ImplicitMigration("c7fa741b-901a-4b13-99ea-62b2fa95d0b3")][global::OpenPCR.Migration("b3aabbfe-8603-439b-a270-c6a14b77c8ba")] public global::System.Int64 DefZero => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.Def) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.DEF)));
		[global::OpenPCR.ImplicitMigration("314116f0-6115-4318-beb1-f6d9babf2597")][global::OpenPCR.Migration("cacfef71-c1aa-4adb-a1c5-11147d409519")] public global::System.Int64 MagicDefZero => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.MagicDef) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.MAGIC_DEF)));
		[global::OpenPCR.ImplicitMigration("91c316a1-8ecd-47a7-91c4-9401d3ea00f5")][global::OpenPCR.Migration("46b34140-2fa3-4c26-a768-4c40683b15dd")] public global::System.Int64 DodgeZero => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.Dodge) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.DODGE)));
		[global::OpenPCR.ImplicitMigration("108ab2e5-c4a1-4dd2-bb88-d4b380888d4c")][global::OpenPCR.Migration("832a3727-c02f-456a-ab9d-56cb1f70b80b")] public global::System.Int64 AccuracyZero => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.Accuracy) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.ACCURACY)));
		[global::OpenPCR.ImplicitMigration("7a0f2825-eb3b-4ca4-a12a-5aa9abf295d3")][global::OpenPCR.Migration("69e17a53-1052-46d1-a982-62e5111894a0")] public global::System.Int64 PhysicalCriticalZero => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.PhysicalCritical) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.PHYSICAL_CRITICAL)));
		[global::OpenPCR.ImplicitMigration("fe69c5b0-db13-4658-a7b7-784637da3e1b")][global::OpenPCR.Migration("3a0a5129-0337-41f1-b103-4d6a156f9027")] public global::System.Int64 MagicCriticalZero => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.MagicCritical) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.MAGIC_CRITICAL)));
		[global::OpenPCR.ImplicitMigration("c62caedc-2c33-4e23-9a7c-ed317779e882")][global::OpenPCR.Migration("b04e97b1-f56b-4a05-960c-06c9b4c2a55e")] public global::System.Int64 LifeStealZero => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.LifeSteal) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.LIFE_STEAL)));
		[global::OpenPCR.ImplicitMigration("86b088a9-0bd8-413a-982a-9ef5e8b62394")][global::OpenPCR.Migration("ac256d62-235c-4f65-a8b4-e9a4c5097b2b")] public global::System.Int64 PhysicalPenetrateZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.PhysicalPenetrate));
		[global::OpenPCR.ImplicitMigration("e92d684b-3234-4865-a7ea-32b5e699ed17")][global::OpenPCR.Migration("4ca8dccf-97c8-4317-8448-2be52925240a")] public global::System.Int64 MagicPenetrateZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.MagicPenetrate));
		[global::OpenPCR.ImplicitMigration("ff157167-1350-4ae2-b2e3-572aa7f93976")][global::OpenPCR.Migration("92ffb65a-738e-4d8a-9dfc-a2abb2f74f0e")] public global::System.Int64 HpRecoveryRateZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.HpRecoveryRate));
		[global::OpenPCR.ImplicitMigration("ec9cea83-aa25-4a0d-98e9-15e392049523")][global::OpenPCR.Migration("00d94ada-4e59-419e-a3b3-39d3ac33d15a")] public global::System.Int64 EnergyReduceRateZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.EnergyReduceRate));
		[global::OpenPCR.ImplicitMigration("0b2d1f2f-621e-4915-9da1-4b71be0fe3db")][global::OpenPCR.Migration("49b4d333-3f7e-4c72-9f68-360222a50ff1")] public global::System.Int64 AbnormalStateDodgeHitZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.AbnormalStateDodgeHit));
		[global::OpenPCR.ImplicitMigration("f7574e9b-2b0a-4e99-9d0e-19fcdb528df6")][global::OpenPCR.Migration("e78e53b4-3308-4acd-b34c-637af3670c13")] public global::System.Int64 AbnormalStateDodgeResistZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.AbnormalStateDodgeResist));
		[global::OpenPCR.ImplicitMigration("f45f9506-3bc7-4735-b210-a0ab152ba585")][global::OpenPCR.Migration("44c50ce4-5616-4ad4-86d7-bb3e987c51c0")] public global::System.Int64 TalentBonusEnhancePercentZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.TalentBonusEnhancePercent));
		[global::OpenPCR.ImplicitMigration("d6121498-463a-4fe2-b263-4954d68c48a5")][global::OpenPCR.Migration("bfb24b7f-55b4-431d-8e63-3bdfd77e4734")] public global::System.Int64 AdditionalAttackEnhanceZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.AdditionalAttackEnhance));
		[global::OpenPCR.ImplicitMigration("6c389fbe-f185-48ac-8b75-c30ed563794a")][global::OpenPCR.Migration("b4c6f85f-9cd6-4d3c-a47f-6ad8f01f943e")] public global::System.Int64 BuffEnhanceZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.BuffEnhance));
		[global::OpenPCR.ImplicitMigration("555192bb-0400-464d-9c45-94a453c44b64")][global::OpenPCR.Migration("273f5882-7642-4046-af20-e915e9cfa4ef")] public global::System.Int64 DebuffEnhanceZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.DebuffEnhance));
		[global::OpenPCR.ImplicitMigration("8f662048-cc6a-4659-a240-ddc48fdb6c76")][global::OpenPCR.Migration("b2e90552-b1fa-4a1e-8059-35185827ef47")] public global::System.Int64 HealEnhanceZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.HealEnhance));
		[global::OpenPCR.ImplicitMigration("67922c8c-168e-43ec-8b38-adc469b25285")][global::OpenPCR.Migration("71182e39-a0e8-4ced-8099-b2993eed0819")] public global::System.Int64 TalentBonusPermyriadZero => Math.Min(2000000000L, Math.Max(0L, (long)componentParameter.AdditionalTalentBonusPermyriad));
		[global::OpenPCR.ImplicitMigration("32224dbc-3c29-45e0-bf9c-4165eaf93a22")][global::OpenPCR.Migration("0740f7d6-610c-4d0b-9a8b-ac1ecd407073")] public global::System.Double EnergyChargeZero => Math.Min(2000000000d, Math.Max(0d, (double)componentParameter.EnergyCharge));

		[global::OpenPCR.Migration("ea2a675c-f955-4bb6-95d4-ee80a0fd3fa6")]
		public global::System.Int64 EnergyRecoveryRateZero
		{
			[global::OpenPCR.Migration("d995f9cd-4f33-4f15-a858-45adbd2284c5")]
			get
			{
				long value = Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.EnergyRecoveryRate) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.ENERGY_RECOVER_RATE)));
				return energyRecoveryRateMax == -1 ? value : Math.Min(value, (long)energyRecoveryRateMax);
			}
		}

		[global::OpenPCR.Migration("127808e2-7363-4dd0-817b-527b73d27645")]
		public long EnergyRecoveryRateZeroWithLimit
		{
			[global::OpenPCR.Migration("4891cf52-3e2d-4126-85df-497c48314a35")]
			get
			{
				var data = componentAbnormal.LimitEnergyRecoveryRateData;
				if (data != null && data.Enable)
					return Math.Min((long)EnergyRecoveryRateZero, BattleUtil.DoubleToLong(data.Value));
				return (long)EnergyRecoveryRateZero;
			}
		}

		[global::OpenPCR.Migration("c973d204-f095-4399-bf58-5124cc7435c2")]
		public global::System.Single MoveSpeedOrMin
		{
			[global::OpenPCR.Migration("f3852134-71e0-49a8-94d2-422536b352c4")]
			get
			{
				float minimum = ignoreMinMoveSpeed ? 0f : 1f;
				float speed = (float)componentParameter.MoveSpeed;
				float value = (speed < 0f ? 0f : speed) + (float)componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.MOVE_SPEED);
				return minimum > value ? minimum : value;
			}
		}

		[global::OpenPCR.ImplicitMigration("86b92d92-fab2-41a2-8e31-005eab36a0ab")][global::OpenPCR.Migration("ee9d841b-81f1-44be-8bdb-39889b00c7a9")] public global::System.Int64 DefZeroForDamagedEnergy => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.DefForDamagedEnergy) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.DEF)));
		[global::OpenPCR.ImplicitMigration("4b1de260-48b8-4724-97ce-f30f5506f902")][global::OpenPCR.Migration("707f59c1-5e6c-4374-b295-ea1b922ed39d")] public global::System.Int64 MagicDefZeroForDamagedEnergy => Math.Min(2000000000L, unchecked(Math.Max(0L, (long)componentParameter.MagicDefForDamagedEnergy) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.MAGIC_DEF)));
		[global::OpenPCR.ImplicitMigration("ddf56254-b2af-44a0-ad42-e72ba89bbec7")][global::OpenPCR.Migration("55e9528f-a052-468c-a45d-4ab24f314399")] public global::System.Int64 ReceiveCriticalDamageRateOrMin => Math.Min(2000000000L, unchecked(Math.Max(50L, (long)componentParameter.ReceiveCriticalDamageRate) + componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.RECEIVE_CRITICAL_DAMAGE_RATE)));
		[global::OpenPCR.ImplicitMigration("88932086-fb3c-4be8-9293-6d73a2a70af7")][global::OpenPCR.Migration("9238699b-b79f-48c6-8506-2c44d8c2a29b")] public global::System.Double PhysicalCriticalDamageRateOrMin => Math.Min(2000000000d, Math.Max(50d, ((double)(long)componentParameter.PhysicalCriticalDamageRate * 100d + (double)(long)componentParameter.PhysicalCriticalDamagePermyriad) / 100d) + (double)componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE));
		[global::OpenPCR.ImplicitMigration("c60d7e4e-ed61-4e95-b4ca-b9f767d8e4ea")][global::OpenPCR.Migration("cfa11564-7608-4225-a4b4-3dd2b25f7523")] public global::System.Double PhysicalReceiveDamagePercentOrMin => Math.Max(0d, (double)unchecked((long)componentParameter.AdditionalPhysicalReceiveDamagePercent + (long)componentParameter.AdditionalPhysicalAndMagicReceiveDamagePercent) + 100d);
		[global::OpenPCR.ImplicitMigration("c48bed0f-77d1-4e04-bf5d-d10484f14c95")][global::OpenPCR.Migration("cf1fc5e8-143e-4d10-b2cc-52f820c54473")] public global::System.Double PhysicalDamageUpPercentOrMin => Math.Max(0d, (double)(long)componentParameter.AdditionalPhysicalDamageUpPercent * 100d + (double)(long)componentParameter.AdditionalPhysicalDamageUpPermyriad + 10000d) / 100d;
		[global::OpenPCR.ImplicitMigration("2c920c0d-38c4-4d2a-878b-9504947c3d62")][global::OpenPCR.Migration("6f2d7850-0157-4c5e-9227-01b532e60604")] public global::System.Double MagicCriticalDamageRateOrMin => Math.Min(2000000000d, Math.Max(50d, ((double)(long)componentParameter.MagicCriticalDamageRate * 100d + (double)(long)componentParameter.MagicCriticalDamagePermyriad) / 100d) + (double)componentAbnormal.GetAdditionalBuffDictionary(BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE));
		[global::OpenPCR.ImplicitMigration("0def287a-3240-482d-951a-2179b50ecddf")][global::OpenPCR.Migration("603b0b64-018f-468c-a90b-5e7f879dbdad")] public global::System.Double MagicReceiveDamagePercentOrMin => Math.Max(0d, (double)unchecked((long)componentParameter.AdditionalMagicReceiveDamagePercent + (long)componentParameter.AdditionalPhysicalAndMagicReceiveDamagePercent) + 100d);
		[global::OpenPCR.ImplicitMigration("79e98c39-2389-481b-92c1-83ab6c83f22e")][global::OpenPCR.Migration("fd17169e-2286-4183-b475-b123816267f3")] public global::System.Double MagicDamageUpPercentOrMin => Math.Max(0d, (double)(long)componentParameter.AdditionalMagicDamageUpPercent * 100d + (double)(long)componentParameter.AdditionalMagicDamageUpPermyriad + 10000d) / 100d;

		private int energyRecoveryRateMax { get; set; }
		private UnitComponentAbnormal componentAbnormal { get; set; }
		private UnitComponentParameter componentParameter { get; set; }
		private bool ignoreMinMoveSpeed { get; set; }

		[global::OpenPCR.Migration("3a961f89-a2a1-4a0c-ac20-b0136d0e1e57")]
		public UnitComponentParameterClamped(int _energyRecoverRateMax, bool _ignoreMinMoveSpeed)
		{
			energyRecoveryRateMax = _energyRecoverRateMax;
			ignoreMinMoveSpeed = _ignoreMinMoveSpeed;
		}

		[global::OpenPCR.Migration("d5907070-283f-405d-bbdd-f7d5b297ed68")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAbnormal = _container.ComponentAbnormal;
			componentParameter = _container.ComponentParameter;
		}

		[global::OpenPCR.Migration("b949d6df-19d0-4a33-906e-2ad92cfd1f07")]
		public float GetDodgeRate(long _accuracy)
		{
			long value = Math.Max(unchecked((long)DodgeZero - _accuracy), 0L);
			return (float)value / ((float)value + 100f);
		}
	}
}
