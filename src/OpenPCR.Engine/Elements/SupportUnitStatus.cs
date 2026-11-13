using System;
using System.Collections.Generic;

namespace Elements
{
	public class SupportUnitStatus
	{
		public global::System.String OwnerName { get; private set; }
		public global::System.Int64 OwnerViewerId { get; private set; }
		public UnitDataLight UnitData { get; private set; }
		public global::System.Int32 UserPositionStatus { get; private set; }
		public UnitData UnitParamData { get; private set; }

		public SupportUnitStatus()
		{
			initializeSupportUnitStatus();
		}

		private void initializeSupportUnitStatus()
		{
			OwnerName = null;
			OwnerViewerId = 0L;
			UnitData = null;
			UserPositionStatus = 0;
		}
	}
}
