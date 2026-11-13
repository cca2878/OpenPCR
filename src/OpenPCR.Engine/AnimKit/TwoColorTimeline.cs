using global::OpenPCR.AnimKit;
using System;

namespace OpenPCR.AnimKit
{
	public class TwoColorTimeline : CurveTimeline
	{

		internal float[] frames;

		private int slotIndex;

		private int propertyId;

		[global::OpenPCR.ImplicitMigration("a44d5d69-9d72-4087-ad02-03ae8eea0e89")]
		public override int PropertyId => propertyId;

		[global::OpenPCR.Migration("8298a1bb-30d6-464f-b92d-0607f4d42401")]
		public TwoColorTimeline(int frameCount, int _slotIndex)
			: base(frameCount)
		{
			frames = new float[frameCount * 8];
			slotIndex = _slotIndex;
			propertyId = 234881024 + slotIndex;
		}

		[global::OpenPCR.Migration("c37a2c74-c56f-4373-b0fc-3995409c3a58")]
		public void SetFrame(int frameIndex, float time, float r, float g, float b, float a, float r2, float g2, float b2)
		{
			frameIndex *= 8;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = r;
			frames[frameIndex + 2] = g;
			frames[frameIndex + 3] = b;
			frames[frameIndex + 4] = a;
			frames[frameIndex + 5] = r2;
			frames[frameIndex + 6] = g2;
			frames[frameIndex + 7] = b2;
		}

		[global::OpenPCR.Migration("2e3f85e5-0d34-457a-89ce-ffb1c155e6eb")]
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
						slot.r2 = data.r2;
						slot.g2 = data.g2;
						slot.b2 = data.b2;
						break;
					case MixPose.Current:
						slot.r += (slot.r - data.r) * alpha;
						slot.g += (slot.g - data.g) * alpha;
						slot.b += (slot.b - data.b) * alpha;
						slot.a += (slot.a - data.a) * alpha;
						slot.r2 += (slot.r2 - data.r2) * alpha;
						slot.g2 += (slot.g2 - data.g2) * alpha;
						slot.b2 += (slot.b2 - data.b2) * alpha;
						break;
				}
				return;
			}
			float num2;
			float num3;
			float num4;
			float num5;
			float num6;
			float num7;
			float num8;
			if (time >= array[array.Length - 8])
			{
				int num = array.Length;
				num2 = array[num + -7];
				num3 = array[num + -6];
				num4 = array[num + -5];
				num5 = array[num + -4];
				num6 = array[num + -3];
				num7 = array[num + -2];
				num8 = array[num + -1];
			}
			else
			{
				int num9 = Animation.BinarySearch(array, time, 8);
				num2 = array[num9 + -7];
				num3 = array[num9 + -6];
				num4 = array[num9 + -5];
				num5 = array[num9 + -4];
				num6 = array[num9 + -3];
				num7 = array[num9 + -2];
				num8 = array[num9 + -1];
				float num10 = array[num9];
				float curvePercent = GetCurvePercent(num9 / 8 - 1, 1f - (time - num10) / (array[num9 + -8] - num10));
				num2 += (array[num9 + 1] - num2) * curvePercent;
				num3 += (array[num9 + 2] - num3) * curvePercent;
				num4 += (array[num9 + 3] - num4) * curvePercent;
				num5 += (array[num9 + 4] - num5) * curvePercent;
				num6 += (array[num9 + 5] - num6) * curvePercent;
				num7 += (array[num9 + 6] - num7) * curvePercent;
				num8 += (array[num9 + 7] - num8) * curvePercent;
			}
			if (alpha == 1f)
			{
				slot.r = num2;
				slot.g = num3;
				slot.b = num4;
				slot.a = num5;
				slot.r2 = num6;
				slot.g2 = num7;
				slot.b2 = num8;
				return;
			}
			float r;
			float g;
			float b;
			float a;
			float r2;
			float g2;
			float b2;
			if (pose == MixPose.Setup)
			{
				r = slot.data.r;
				g = slot.data.g;
				b = slot.data.b;
				a = slot.data.a;
				r2 = slot.data.r2;
				g2 = slot.data.g2;
				b2 = slot.data.b2;
			}
			else
			{
				r = slot.r;
				g = slot.g;
				b = slot.b;
				a = slot.a;
				r2 = slot.r2;
				g2 = slot.g2;
				b2 = slot.b2;
			}
			slot.r = r + (num2 - r) * alpha;
			slot.g = g + (num3 - g) * alpha;
			slot.b = b + (num4 - b) * alpha;
			slot.a = a + (num5 - a) * alpha;
			slot.r2 = r2 + (num6 - r2) * alpha;
			slot.g2 = g2 + (num7 - g2) * alpha;
			slot.b2 = b2 + (num8 - b2) * alpha;
		}
	}
}
