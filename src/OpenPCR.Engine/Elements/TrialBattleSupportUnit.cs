using System.Runtime.CompilerServices;

namespace Elements
{
	public class TrialBattleSupportUnit
	{
		public UnitDataLight UnitData { get; private set; }
		public global::System.Int64 OwnerViewerId { get; private set; }
		public global::System.String OwnerName { get; private set; }
		public UnitData UnitParamData { get; private set; }

		public TrialBattleSupportUnit()
		{
			initializeTrialBattleSupportUnit();
		}

		private void initializeTrialBattleSupportUnit()
		{
			UnitData = null;
			OwnerViewerId = 0;
			OwnerName = null;
			UnitParamData = null;
		}
	}
}
