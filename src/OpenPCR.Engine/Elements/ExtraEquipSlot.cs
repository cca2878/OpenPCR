using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class ExtraEquipSlot
	{
		public global::System.Int32 SerialId { get; private set; }
		public global::System.Int32 ExEquipmentId { get; private set; }
		public global::System.Int32 EnhancementPt { get; private set; }

		public List<ExtraEquipSubStatus> SubStatus { get; private set; }
		public global::System.Int32 AlcesFlag { get; private set; }

		public ExtraEquipSlot()
		{
			initializeExtraEquipSlot();
		}

		public ExtraEquipSlot(ExtraEquipSlot _slot)
		{
			SerialId = _slot.SerialId;
			ExEquipmentId = _slot.ExEquipmentId;
			EnhancementPt = _slot.EnhancementPt;
			var newSubStatus = new List<ExtraEquipSubStatus>();
			if (_slot.SubStatus != null && _slot.SubStatus.Count > 0)
				_slot.SubStatus.ForEach(ss => newSubStatus.Add(new ExtraEquipSubStatus(ss.SlotNumber, ss.Status, ss.Step, ss.IsLock)));
			SubStatus = newSubStatus;
		}

		private void initializeExtraEquipSlot()
		{
			SerialId = 0;
			ExEquipmentId = 0;
			EnhancementPt = 0;
			SubStatus = new List<ExtraEquipSubStatus>();
			AlcesFlag = 0;
		}
	}
}
