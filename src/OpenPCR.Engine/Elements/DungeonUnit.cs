using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class DungeonUnit
	{
		public global::System.Int32 UnitId { get; private set; }
		public global::System.Int64 Hp { get; private set; }
		public global::System.Int32 Energy { get; private set; }
		public List<SkillLimitCounter> SkillLimitCounter { get; private set; }
		public global::System.Int64 MaxHp { get; private set; }
		public global::System.Int32 Power { get; private set; }
		public global::System.Int32 Level { get; private set; }
		public global::System.Int32 Rarity { get; private set; }
		public global::System.Int32 PromotionLevel { get; private set; }
		public SkinData SkinData { get; private set; }
		public List<global::System.Int32> UniqueEquippedList { get; private set; }
		public List<ExtraEquipSlot> ExEquipSlot { get; private set; }
		public List<ExtraEquipSlot> CbExEquipSlot { get; private set; }
		public List<PartsInfo> PartsList { get; private set; }

		public DungeonUnit()
		{
			initializeDungeonUnit();
		}

		private void initializeDungeonUnit()
		{
			UnitId = 0;
			Hp = 0;
			Energy = 0;
			SkillLimitCounter = new List<SkillLimitCounter>();
			MaxHp = 0;
			Power = 0;
			Level = 0;
			Rarity = 0;
			PromotionLevel = 0;
			SkinData = null;
			UniqueEquippedList = new List<global::System.Int32>();
			ExEquipSlot = new List<ExtraEquipSlot>();
			CbExEquipSlot = new List<ExtraEquipSlot>();
			PartsList = new List<PartsInfo>();
		}
	}
}
