using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class ExtraEquipInfo
	{
		public global::System.Int32 SerialId { get; private set; }
		public global::System.Int32 ExEquipmentId { get; private set; }
		public global::System.Int32 EnhancementPt { get; private set; }
		public global::System.Int32 Rank { get; private set; }
		public global::System.Int32 ProtectionFlag { get; private set; }

		public ExtraEquipInfo()
		{
			initializeExtraEquipInfo();
		}

		private void initializeExtraEquipInfo()
		{
			SerialId = 0;
			ExEquipmentId = 0;
			EnhancementPt = 0;
			Rank = 0;
			ProtectionFlag = 0;
		}
	}
}
