using System.Runtime.CompilerServices;

namespace Elements
{
	public class DuplicateUnitInfo
	{
		public global::System.Int32 UnitId { get; private set; }
		public global::System.Int32 Rarity { get; private set; }
		public global::System.Int32 Count { get; private set; }

		public DuplicateUnitInfo()
		{
			initializeDuplicateUnitInfo();
		}

		private void initializeDuplicateUnitInfo()
		{
			UnitId = 0;
			Rarity = 0;
			Count = 0;
		}
	}
}
