using global::OpenPCR.Foundation;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Data;

namespace Elements
{
	public class UnitData
	{

		[global::OpenPCR.Migration("a156fe56-2ee4-496b-9e50-b1f61ba2d61c")]
		public long TotalHp
		{
			[global::OpenPCR.Migration("2949780f-1910-48d4-bc00-cdb66347c86b")]
			get
			{
				return global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(1);
			}
		}

		[global::OpenPCR.Migration("affd8bfd-6677-4c0a-a40d-381d057c7a44")]
		public int TotalAtk
		{
			[global::OpenPCR.Migration("6418f784-4fa7-40f2-8585-23e510bf36f0")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(2);
			}
		}

		[global::OpenPCR.Migration("f072fd8a-4921-4aca-a3db-309920d40f70")]
		public int TotalDef
		{
			[global::OpenPCR.Migration("97b30c16-8115-4700-92e3-f7b50d44c45b")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(3);
			}
		}

		[global::OpenPCR.Migration("0fcca9df-faf9-40de-a453-88c5c91adaf9")]
		public int TotalMagicAtk
		{
			[global::OpenPCR.Migration("1f22089a-88e3-44eb-83e6-64471f1a25de")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(4);
			}
		}

		[global::OpenPCR.Migration("0b13d181-eaa5-466e-918e-d28b0a5f2af5")]
		public int TotalMagicDef
		{
			[global::OpenPCR.Migration("59b6e90a-e7d9-406b-8546-48f045b15c4b")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(5);
			}
		}

		[global::OpenPCR.Migration("0aa854c1-d8f1-43e7-8346-e8648e31a1b1")]
		public int TotalCritical
		{
			[global::OpenPCR.Migration("9620c620-d912-4bc0-8d71-213beb39f41f")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(6);
			}
		}

		[global::OpenPCR.Migration("1e050bb0-a881-4e61-914d-8ef37f6b20d5")]
		public int TotalMagicCritical
		{
			[global::OpenPCR.Migration("9b8d5a50-4ca7-4e46-ad00-20d379c55c39")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(7);
			}
		}

		[global::OpenPCR.Migration("2d24c17c-0da1-428b-845e-a129d2dac0a8")]
		public int TotalWaveHpRecovery
		{
			[global::OpenPCR.Migration("8c4e29c5-435a-412c-b6d4-90ce0b9effb1")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(10);
			}
		}

		[global::OpenPCR.Migration("b5fd24b0-f319-4f20-b328-300565332371")]
		public int TotalWaveEnergyRecovery
		{
			[global::OpenPCR.Migration("fcae0ca6-25cf-4d5f-a781-062c8e106781")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(11);
			}
		}

		[global::OpenPCR.Migration("bcfb2958-a310-4783-b7eb-cc3f77d0ba81")]
		public int TotalHpRecoveryRate
		{
			[global::OpenPCR.Migration("aa5c86d3-0688-4b48-9e14-a6ff4f77ba53")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(15);
			}
		}

		[global::OpenPCR.Migration("aded98a9-52d2-4bb1-bcb5-f10168360047")]
		public int TotalPhysicalPenetrate
		{
			[global::OpenPCR.Migration("98a21d02-339e-49c4-9cac-d0e84c7c6a6f")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(12);
			}
		}

		[global::OpenPCR.Migration("f31b9c5d-b9eb-40c0-abd8-2b56fa3d2feb")]
		public int TotalMagicPenetrate
		{
			[global::OpenPCR.Migration("1b67227b-f4da-402f-a0f6-988c4ef6a7a0")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(13);
			}
		}

		[global::OpenPCR.Migration("6395a839-f681-467d-b620-a9a0e19ff82f")]
		public int TotalLifeSteal
		{
			[global::OpenPCR.Migration("5fb736bf-0f05-48e6-bd22-b9df68c56e66")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(9);
			}
		}

		[global::OpenPCR.Migration("bd6dc835-fa69-457e-be46-328504a99c25")]
		public int TotalDodge
		{
			[global::OpenPCR.Migration("5a6116a2-b481-494f-87db-9cea524e9188")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(8);
			}
		}

		[global::OpenPCR.Migration("9b6d863c-d337-4a1c-9db8-51beed540882")]
		public int TotalAccuracy
		{
			[global::OpenPCR.Migration("a520a7dc-1540-484f-9b64-bb2d93742102")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(17);
			}
		}

		[global::OpenPCR.Migration("115b5013-7170-404f-802b-93e3a18680ba")]
		public int TotalEnergyRecoveryRate
		{
			[global::OpenPCR.Migration("f9589773-c780-4149-9933-cc54fe5a7668")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(14);
			}
		}

		[global::OpenPCR.Migration("cfd60a2c-61fe-4a3a-9c7e-844ce93e386e")]
		public int TotalEnergyReduceRate
		{
			[global::OpenPCR.Migration("373e734b-21e5-4dac-9d63-f86cf2bf8792")]
			get
			{
				return (int)global::OpenPCR.PreparedUnitInputBinding.Require(this).Attribute(16);
			}
		}

		[global::OpenPCR.ImplicitMigration("b4397e45-2211-4e02-8cb3-4bf5d3518116")][global::OpenPCR.Migration("ae800838-5c03-4c45-9ec2-42935bc353b0")] public EquipSlot UniqueEquipSlot2 => GetUniqueEquipSlot(1);

		[global::OpenPCR.Migration("59be31f8-a71f-457a-890f-d6e192febe82")]
		public EquipSlot GetUniqueEquipSlot(int _slotIndex)
		{
			return UniqueEquipSlot != null && UniqueEquipSlot.Count > _slotIndex ? UniqueEquipSlot[_slotIndex] : null;
		}

		public global::System.Int32 Id { get; private set; }
		public DateTime GetTime { get; private set; }
		public global::System.Int32 StartRarety { get; private set; }
		public global::System.Int32 UnitRarity { get; private set; }
		public global::System.Int32 BattleRarity { get; private set; }
		public global::System.Int32 UnitLevel { get; private set; }
		public global::System.Int32 UnitExp { get; private set; }
		public ePromotionLevel PromotionLevel { get; private set; }
		public List<SkillLevelInfo> UnionBurst { get; private set; }
		public List<SkillLevelInfo> MainSkill { get; private set; }
		public List<SkillLevelInfo> ExSkill { get; private set; }
		public List<SkillLevelInfo> FreeSkill { get; private set; }
		public List<EquipSlot> EquipSlot { get; private set; }
		public List<EquipSlot> UniqueEquipSlot { get; private set; }
		public global::System.Int32 ExceedStage { get; private set; }
		public UnitParam UnitParam { get; private set; }
		public StatusParamShort BonusParam { get; private set; }
		public global::System.Int32 ResistStatusId { get; private set; }
		public global::System.Int32 ResistVariationId { get; private set; }
		public global::System.Int32 Power { get; private set; }
		public SkinData SkinData { get; private set; }
		public global::System.Int32 IdentifyNum { get; private set; }
		public global::System.Int32 FavoriteFlag { get; private set; }
		public UnlockRarity6Slot UnlockRarity6Item { get; private set; }
		public List<ExtraEquipSlot> ExEquipSlot { get; private set; }
		public List<ExtraEquipSlot> CbExEquipSlot { get; private set; }

		private UserData userData
		{
			get { return Singleton<UserData>.Instance; }
		}

		[global::OpenPCR.Migration("595d51fb-dbd7-4b4d-b767-bf92003a5a0c")]
		public UnitData()
		{
			initializeUnitData();
		}

		[global::OpenPCR.Migration("26b973d5-2f85-44de-9dd5-c4a2920322a4")]
		public void SetId(int _id)
		{
			Id = _id;
		}

		[global::OpenPCR.Migration("c20aa1e4-8f19-47ec-9762-aee7e8a728c1")]
		public void SetUnitRarity(int _unitRarity)
		{
			UnitRarity = _unitRarity;
		}

		[global::OpenPCR.Migration("454e27e2-6268-4429-8756-49ad80370e04")]
		public void SetBattleRarity(int _battleRarity)
		{
			BattleRarity = _battleRarity;
		}

		[global::OpenPCR.Migration("cdf0b06b-e133-4038-b661-e61a4da6aac9")]
		public void SetUnitLevel(int _unitLevel)
		{
			UnitLevel = _unitLevel;
		}

		[global::OpenPCR.Migration("4dfa0218-e390-438e-939a-69428fd02e4b")]
		public void SetPromotionLevel(ePromotionLevel _promotionLevel)
		{
			PromotionLevel = _promotionLevel;
		}

		[global::OpenPCR.Migration("eff0d853-2996-4d5d-8564-d46db24e1493")]
		public void SetUnionBurst(List<SkillLevelInfo> _unionBurst)
		{
			UnionBurst = _unionBurst;
		}

		[global::OpenPCR.Migration("d1460c32-36e6-48b0-bbfa-493f974e944f")]
		public void SetMainSkill(List<SkillLevelInfo> _mainSkill)
		{
			MainSkill = _mainSkill;
		}

		[global::OpenPCR.Migration("1d33b723-393e-4599-b633-fa3c56f58977")]
		public void SetExSkill(List<SkillLevelInfo> _exSkill)
		{
			ExSkill = _exSkill;
		}

		[global::OpenPCR.Migration("2e5c1c53-c98e-482b-9e71-1d78d9f65e8a")]
		public void SetFreeSkill(List<SkillLevelInfo> _freeSkill)
		{
			FreeSkill = _freeSkill;
		}

		[global::OpenPCR.Migration("5656e5c1-11f3-4f30-970c-9c862c5fa4bd")]
		public void SetEquipSlot(List<EquipSlot> _equipSlot)
		{
			EquipSlot = _equipSlot;
		}

		[global::OpenPCR.Migration("cfa41486-5b2a-434d-9af4-9731e6ba3329")]
		public void SetUniqueEquipSlot(List<EquipSlot> _uniqueEquipSlot)
		{
			UniqueEquipSlot = _uniqueEquipSlot;
		}

		[global::OpenPCR.Migration("4e6c0d6f-4d6c-402f-a49a-0660e77d1ffb")]
		public void SetExceedStage(int _exceedStage)
		{
			ExceedStage = _exceedStage;
		}

		[global::OpenPCR.Migration("f8703e2e-b067-4b56-8911-00d0773cbd81")]
		public void SetUnitParam(UnitParam _unitParam)
		{
			UnitParam = _unitParam;
		}

		[global::OpenPCR.Migration("baeb3876-9741-4922-82c7-bafaacc61f59")]
		public void SetBonusParam(StatusParamShort _bonusParam)
		{
			BonusParam = _bonusParam;
		}

		[global::OpenPCR.Migration("10b5257d-0ba2-43fd-8739-927c9e1dfddb")]
		public void SetResistStatusId(int _resistStatusId)
		{
			ResistStatusId = _resistStatusId;
		}

		[global::OpenPCR.Migration("5809f9e2-27ce-4e5d-a974-a3fc7d5447ca")]
		public void SetResistVariationId(int _resistVariationId)
		{
			ResistVariationId = _resistVariationId;
		}

		[global::OpenPCR.Migration("de1a9d02-48ae-48ae-98ec-e27d6ec5197a")]
		public void SetPower(int _power)
		{
			Power = _power;
		}

		[global::OpenPCR.Migration("14056a67-8b5d-4ae8-962c-8a30e24b344c")]
		public void SetSkinData(SkinData _skinData)
		{
			SkinData = _skinData;
		}

		[global::OpenPCR.Migration("8f1a1fc4-a786-4330-901f-0caac062ef02")]
		public void SetExEquipSlot(List<ExtraEquipSlot> _exEquipSlot)
		{
			ExEquipSlot = _exEquipSlot;
		}

		[global::OpenPCR.Migration("5795f890-adb1-40d3-b6f0-ee9f1b6e801b")]
		public void SetCbExEquipSlot(List<ExtraEquipSlot> _cbExEquipSlot)
		{
			CbExEquipSlot = _cbExEquipSlot;
		}

		[global::OpenPCR.Migration("bc8a7f14-f86b-4086-928c-371f9aca6f6d")]
		private void initializeUnitData()
		{
			Id = 0;
			GetTime = default(DateTime);
			StartRarety = 0;
			UnitRarity = 0;
			BattleRarity = 0;
			UnitLevel = 0;
			UnitExp = 0;
			PromotionLevel = ePromotionLevel.INVALID_VALUE;
			UnionBurst = new List<SkillLevelInfo>();
			MainSkill = new List<SkillLevelInfo>();
			ExSkill = new List<SkillLevelInfo>();
			FreeSkill = new List<SkillLevelInfo>();
			EquipSlot = new List<EquipSlot>();
			UniqueEquipSlot = new List<EquipSlot>();
			ExceedStage = 0;
			UnitParam = null;
			BonusParam = null;
			ResistStatusId = 0;
			ResistVariationId = 0;
			Power = -1;
			SkinData = null;
			IdentifyNum = 0;
			FavoriteFlag = 0;
			UnlockRarity6Item = null;
			ExEquipSlot = new List<ExtraEquipSlot>();
			CbExEquipSlot = new List<ExtraEquipSlot>();
		}

		[global::OpenPCR.Migration("69a23f70-4870-49d4-9d87-41059290f250")]
		public UnitData CreateCopyData()
		{
			UnitData result = (UnitData)MemberwiseClone();
			result.SetUnionBurst(createSkillInstance(result.UnionBurst));
			if (result.MainSkill != null)
				result.SetMainSkill(createSkillInstance(result.MainSkill));
			if (result.ExSkill != null)
				result.SetExSkill(createSkillInstance(result.ExSkill));
			if (result.ExEquipSlot != null)
				result.SetExEquipSlot(createExtraEquipSlotInstance(result.ExEquipSlot));
			global::OpenPCR.PreparedUnitInputBinding.Copy(this, result);
			return result;

			static List<SkillLevelInfo> createSkillInstance(List<SkillLevelInfo> originalData)
			{
				List<SkillLevelInfo> copied = new List<SkillLevelInfo>();
				for (int i = 0; i < originalData.Count; i++)
				{
					copied.Add(new SkillLevelInfo());
					copied[i].SetSkillId(originalData[i].SkillId);
					copied[i].SetSkillLevel(originalData[i].SkillLevel);
					copied[i].SetSlotNumber(originalData[i].SlotNumber);
				}
				return copied;
			}

			static List<ExtraEquipSlot> createExtraEquipSlotInstance(List<ExtraEquipSlot> originalData)
			{
				List<ExtraEquipSlot> copied = new List<ExtraEquipSlot>();
				for (int i = 0; i < originalData.Count; i++)
					copied.Add(new ExtraEquipSlot(originalData[i]));
				return copied;
			}
		}

		[global::OpenPCR.Migration("2e5ff379-379e-4cae-924c-8969d76e1d39")]
		public int GetCurrentRarity()
		{
			if ((int)UnitRarity != 5)
			{
				return UnitRarity;
			}

			if (!userData.IsReleaseChangeRarityFlag)
			{
				return UnitRarity;
			}

			return ((int)BattleRarity == 0) ? UnitRarity : BattleRarity;
		}
	}
}
