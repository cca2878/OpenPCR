using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class UnitDataForView
	{
		public global::System.Int32 Id { get; private set; }
		public global::System.Int32 UnitLevel { get; private set; }
		public global::System.Int32 UnitRarity { get; private set; }
		public global::System.Int32 BattleRarity { get; private set; }
		public ePromotionLevel PromotionLevel { get; private set; }
		public global::System.Int32 Power { get; private set; }
		public SkinData SkinData { get; private set; }
		public List<EquipSlot> UniqueEquipSlot { get; private set; }
		public List<ExtraEquipSlot> ExEquipSlot { get; private set; }
		public List<ExtraEquipSlot> CbExEquipSlot { get; private set; }

		public UnitDataForView()
		{
			initializeUnitDataForView();
		}

		private void initializeUnitDataForView()
		{
			Id = 0;
			UnitLevel = 0;
			UnitRarity = 0;
			BattleRarity = 0;
			PromotionLevel = ePromotionLevel.INVALID_VALUE;
			Power = 0;
			SkinData = null;
			UniqueEquipSlot = new List<EquipSlot>();
			ExEquipSlot = new List<ExtraEquipSlot>();
			CbExEquipSlot = new List<ExtraEquipSlot>();
		}
	}
}
