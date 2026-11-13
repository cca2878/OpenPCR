using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class AttachmentTimeline : Timeline
	{
		private int slotIndex;

		internal float[] frames;

		private string[] attachmentNames;

		protected int propertyId;

		public int PropertyId => propertyId;

		[global::OpenPCR.Migration("872485cd-ba33-470d-b108-5c1bc00c0c86")]
		public AttachmentTimeline(int frameCount, int _slotIndex)
		{
			frames = new float[frameCount];
			attachmentNames = new string[frameCount];
			slotIndex = _slotIndex;
			propertyId = 67108864 + slotIndex;
		}

		[global::OpenPCR.Migration("bed9cc84-55c9-4985-8280-3f626353f264")]
		public void SetFrame(int frameIndex, float time, string attachmentName)
		{
			frames[frameIndex] = time;
			attachmentNames[frameIndex] = attachmentName;
		}

		[global::OpenPCR.Migration("81d9bfc0-09ea-4abc-ae0f-84e08b9a9f2c")]
		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			if (direction == MixDirection.Out && pose == MixPose.Setup)
			{
				string attachmentName = slot.data.attachmentName;
				slot.Attachment = ((attachmentName == null) ? null : skeleton.GetAttachment(slotIndex, attachmentName));
				return;
			}
			float[] array = frames;
			if (time < array[0])
			{
				if (pose == MixPose.Setup)
				{
					string attachmentName = slot.data.attachmentName;
					slot.Attachment = ((attachmentName == null) ? null : skeleton.GetAttachment(slotIndex, attachmentName));
				}
			}
			else
			{
				int num = ((!(time >= array[array.Length - 1])) ? (Animation.BinarySearch(array, time, 1) - 1) : (array.Length - 1));
				string attachmentName = attachmentNames[num];
				slot.Attachment = ((attachmentName == null) ? null : skeleton.GetAttachment(slotIndex, attachmentName));
			}
		}
	}
}
