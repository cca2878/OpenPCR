using System;
using System.Collections.Generic;

namespace Elements
{
	public class UnitDataLight
	{
		public global::System.Int32 Id { get; private set; }
		public DateTime GetTime { get; private set; }
		public global::System.Int32 StartRarety { get; private set; }
		public global::System.Int32 UnitRarity { get; private set; }
		public global::System.Int32 BattleRarity { get; private set; }
		public global::System.Int32 UnitLevel { get; private set; }
		public global::System.Int32 UnitExp { get; private set; }
		public ePromotionLevel PromotionLevel { get; private set; }
		public List<SkillLevelInfoLight> UnionBurst { get; private set; }
		public List<SkillLevelInfoLight> MainSkill { get; private set; }
		public List<SkillLevelInfoLight> ExSkill { get; private set; }
		public List<EquipSlotLight> EquipSlot { get; private set; }
		public List<EquipSlotLight> UniqueEquipSlot { get; private set; }
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

		public UnitDataLight()
		{
			initializeUnitDataLight();
		}

		private void initializeUnitDataLight()
		{
			Id = 0;
			GetTime = default(DateTime);
			StartRarety = 0;
			UnitRarity = 0;
			BattleRarity = 0;
			UnitLevel = 0;
			UnitExp = 0;
			PromotionLevel = ePromotionLevel.INVALID_VALUE;
			UnionBurst = new List<SkillLevelInfoLight>();
			MainSkill = new List<SkillLevelInfoLight>();
			ExSkill = new List<SkillLevelInfoLight>();
			EquipSlot = new List<EquipSlotLight>();
			UniqueEquipSlot = new List<EquipSlotLight>();
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
	}
}
