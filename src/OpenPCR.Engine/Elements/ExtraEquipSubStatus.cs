
namespace Elements
{
	public class ExtraEquipSubStatus
	{
		public global::System.Int32 SlotNumber { get; private set; }
		public global::System.Int32 Status { get; private set; }
		public global::System.Int32 Step { get; private set; }
		public global::System.Boolean IsLock { get; private set; }

		public ExtraEquipSubStatus() { initializeExtraEquipSubStatus(); }
		public ExtraEquipSubStatus(int _slotNumber, int _status, int _step, bool _isLock)
		{
			SlotNumber = _slotNumber;
			Status = _status;
			Step = _step;
			IsLock = _isLock;
		}
		private void initializeExtraEquipSubStatus()
		{
			SlotNumber = 0;
			Status = 0;
			Step = 0;
			IsLock = false;
		}
	}
}
