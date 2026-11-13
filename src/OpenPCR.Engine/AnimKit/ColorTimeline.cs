using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class ColorTimeline : CurveTimeline
	{

		private int slotIndex;

		internal float[] frames;

		private int propertyId;

		[global::OpenPCR.ImplicitMigration("92eaa6bb-e78c-44a5-a614-e4021218dc2a")]
		public sealed override int PropertyId => propertyId;

		[global::OpenPCR.Migration("ad3aa56f-c584-4c31-93ce-2f1339020519")]
		public ColorTimeline(int frameCount, int _slotIndex)
			: base(frameCount)
		{
			frames = new float[frameCount * 5];
			slotIndex = _slotIndex;
			propertyId = 83886080 + slotIndex;
		}

		[global::OpenPCR.Migration("42ef0e00-cc4f-4146-b900-8ecde32dab5d")]
		public void SetFrame(int frameIndex, float time, float r, float g, float b, float a)
		{
			frameIndex *= 5;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = r;
			frames[frameIndex + 2] = g;
			frames[frameIndex + 3] = b;
			frames[frameIndex + 4] = a;
		}

		[global::OpenPCR.Migration("c281e85e-9209-4829-800b-a3c5173e7269")]
		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			Slot slot = skeleton.slots.Items[slotIndex];
			float[] array = frames;
			if (time < array[0])
			{
				SlotData data = slot.data;
				switch (pose)
				{
					case MixPose.Setup:
						slot.r = data.r;
						slot.g = data.g;
						slot.b = data.b;
						slot.a = data.a;
						break;
					case MixPose.Current:
						slot.r += (slot.r - data.r) * alpha;
						slot.g += (slot.g - data.g) * alpha;
						slot.b += (slot.b - data.b) * alpha;
						slot.a += (slot.a - data.a) * alpha;
						break;
				}
				return;
			}
			float num2;
			float num3;
			float num4;
			float num5;
			if (time >= array[array.Length - 5])
			{
				int num = array.Length;
				num2 = array[num + -4];
				num3 = array[num + -3];
				num4 = array[num + -2];
				num5 = array[num + -1];
			}
			else
			{
				int num6 = Animation.BinarySearch(array, time, 5);
				num2 = array[num6 + -4];
				num3 = array[num6 + -3];
				num4 = array[num6 + -2];
				num5 = array[num6 + -1];
				float num7 = array[num6];
				float curvePercent = GetCurvePercent(num6 / 5 - 1, 1f - (time - num7) / (array[num6 + -5] - num7));
				num2 += (array[num6 + 1] - num2) * curvePercent;
				num3 += (array[num6 + 2] - num3) * curvePercent;
				num4 += (array[num6 + 3] - num4) * curvePercent;
				num5 += (array[num6 + 4] - num5) * curvePercent;
			}
			if (alpha == 1f)
			{
				slot.r = num2;
				slot.g = num3;
				slot.b = num4;
				slot.a = num5;
				return;
			}
			float r;
			float g;
			float b;
			float a;
			if (pose == MixPose.Setup)
			{
				r = slot.data.r;
				g = slot.data.g;
				b = slot.data.b;
				a = slot.data.a;
			}
			else
			{
				r = slot.r;
				g = slot.g;
				b = slot.b;
				a = slot.a;
			}
			slot.r = r + (num2 - r) * alpha;
			slot.g = g + (num3 - g) * alpha;
			slot.b = b + (num4 - b) * alpha;
			slot.a = a + (num5 - a) * alpha;
		}
	}
}
