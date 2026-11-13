using System.Runtime.CompilerServices;

namespace Elements
{
	public class EquipSlot
	{
		public global::System.Int32 Id { get; private set; }
		public global::System.Boolean IsSlot { get; private set; }
		public global::System.Int32 EnhancementLevel { get; private set; }
		public global::System.Int32 EnhancementPt { get; private set; }
		public global::System.Int32 Rank { get; private set; }
		public global::System.Int32 Status { get; private set; }

		public EquipSlot()
		{
			initializeEquipSlot();
		}

		private void initializeEquipSlot()
		{
			Id = 0;
			IsSlot = false;
			EnhancementLevel = 0;
			EnhancementPt = 0;
			Rank = 0;
			Status = 0;
		}
	}
}
