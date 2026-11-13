using System.Runtime.CompilerServices;

namespace Elements
{
	public class SkillLevelInfo
	{
		public global::System.Int32 SkillId { get; private set; }
		public global::System.Int32 SkillLevel { get; private set; }
		public global::System.Int32 SlotNumber { get; private set; }

		[global::OpenPCR.Migration("9338db36-e027-41d6-82af-6dabae2e5d25")]
		public SkillLevelInfo()
		{
			initializeSkillLevelInfo();
		}

		[global::OpenPCR.Migration("ed31e31b-40a5-41e4-96ea-943b581326d0")]
		public void SetSkillId(int _skillId)
		{
			SkillId = _skillId;
		}

		[global::OpenPCR.Migration("a0e1e1af-c487-410a-9640-3da30241b21f")]
		public void SetSkillLevel(int _skillLevel)
		{
			SkillLevel = _skillLevel;
		}

		[global::OpenPCR.Migration("3829154f-e45c-4899-800f-8a2dcd5503ab")]
		public void SetSlotNumber(int _slotNumber)
		{
			SlotNumber = _slotNumber;
		}

		[global::OpenPCR.Migration("dfdc7fdc-6a5c-46d9-8cd3-43209ee35212")]
		private void initializeSkillLevelInfo()
		{
			SkillId = 0;
			SkillLevel = 0;
			SlotNumber = 0;
		}
	}
}
