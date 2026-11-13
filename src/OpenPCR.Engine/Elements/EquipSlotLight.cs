using System;
using System.Collections.Generic;

namespace Elements
{
	public class EquipSlotLight
	{
		public global::System.Boolean IsSlot { get; private set; }
		public global::System.Int32 EnhancementPt { get; private set; }
		public global::System.Int32 Rank { get; private set; }

		public EquipSlotLight()
		{
			initializeEquipSlotLight();
		}

		private void initializeEquipSlotLight()
		{
			IsSlot = false;
			EnhancementPt = 0;
			Rank = 0;
		}
	}
}
