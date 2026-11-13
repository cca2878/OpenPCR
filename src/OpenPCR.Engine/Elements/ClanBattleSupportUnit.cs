using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class ClanBattleSupportUnit
	{
		public UnitData UnitData { get; private set; }
		public global::System.Int32 CurrentSupportUnit { get; private set; }
		public global::System.Int32 StockIndex { get; private set; }
		public global::System.Int64 OwnerViewerId { get; private set; }
		public global::System.String OwnerName { get; private set; }
		public global::System.Int32 RemainingCount { get; private set; }

		public ClanBattleSupportUnit()
		{
			initializeClanBattleSupportUnit();
		}

		private void initializeClanBattleSupportUnit()
		{
			UnitData = null;
			CurrentSupportUnit = 0;
			StockIndex = 0;
			OwnerViewerId = 0;
			OwnerName = null;
			RemainingCount = 0;
		}
	}
}
