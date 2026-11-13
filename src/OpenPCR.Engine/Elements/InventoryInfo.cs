using System.Runtime.CompilerServices;

namespace Elements
{
	public class InventoryInfo
	{
		public global::System.Int32 Id { get; private set; }
		public eInventoryType Type { get; private set; }
		public global::System.Int32 Count { get; private set; }
		public global::System.Int32 Received { get; private set; }
		public global::System.Int32 Stock { get; private set; }
		public UnitData UnitData { get; private set; }
		public DuplicateUnitInfo ExchangeData { get; private set; }
		public ExtraEquipInfo ExEquip { get; private set; }

		public InventoryInfo()
		{
			initializeInventoryInfo();
		}

		private void initializeInventoryInfo()
		{
			Id = 0;
			Type = eInventoryType.INVALID_VALUE;
			Count = -1;
			Received = -1;
			Stock = -1;
			UnitData = null;
			ExchangeData = null;
			ExEquip = null;
		}
	}
}
