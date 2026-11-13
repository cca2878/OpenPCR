using global::OpenPCR.AnimKit;
using System;
namespace OpenPCR.AnimKit
{
	public abstract class CurveTimeline : Timeline
	{
		protected const int BEZIER_SIZE = 19;
		private float[] curves;
		public int FrameCount
		{
			[global::OpenPCR.Migration("335fde61-34d1-43c2-bd16-7abb0a6fc419")]
			get => curves.Length / BEZIER_SIZE + 1;
		}

		public abstract int PropertyId { get; }

		[global::OpenPCR.Migration("04ae36dd-b964-45fe-a2e7-2647a44f2269")]
		public CurveTimeline(int frameCount)
		{
			if (frameCount <= 0)
			{
				throw new ArgumentException("frameCount must be > 0: " + frameCount, "frameCount");
			}

			curves = new float[(frameCount - 1) * 19];
		}

		public abstract void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction);

		[global::OpenPCR.Migration("d04de195-26ad-40c1-8d46-3853d8e0121e")]
		public float GetCurvePercent(int frameIndex, float percent)
		{
			percent = MathUtils.Clamp(percent, 0f, 1f);
			float[] array = curves;
			int num = frameIndex * 19;
			float num2 = array[num];
			if (num2 == 0f)
			{
				return percent;
			}

			if (num2 == 1f)
			{
				return 0f;
			}

			num++;
			float num3 = 0f;
			int num4 = num;
			for (int num5 = num + 19 - 1; num < num5; num += 2)
			{
				num3 = array[num];
				if (num3 >= percent)
				{
					float num6;
					float num7;
					if (num == num4)
					{
						num6 = 0f;
						num7 = 0f;
					}
					else
					{
						num6 = array[num - 2];
						num7 = array[num - 1];
					}

					return num7 + (array[num + 1] - num7) * (percent - num6) / (num3 - num6);
				}
			}

			float num8 = array[num - 1];
			return num8 + (1f - num8) * (percent - num3) / (1f - num3);
		}

		[global::OpenPCR.Migration("eb627a97-f821-4e0b-8119-4181f8e4ccde")]
		public void SetStepped(int frameIndex)
		{
			curves[frameIndex * 19] = 1f;
		}

		[global::OpenPCR.Migration("799f2f12-c7f2-4ab4-9ac4-e5a436fec00c")]
		public void SetCurve(int frameIndex, float cx1, float cy1, float cx2, float cy2)
		{
			float num = ((-cx1) * 2f + cx2) * 0.03f;
			float num2 = ((-cy1) * 2f + cy2) * 0.03f;
			float num3 = ((cx1 - cx2) * 3f + 1f) * 0.006f;
			float num4 = ((cy1 - cy2) * 3f + 1f) * 0.006f;
			float num5 = num * 2f + num3;
			float num6 = num2 * 2f + num4;
			float num7 = cx1 * 0.3f + num + num3 * (1f / 6f);
			float num8 = cy1 * 0.3f + num2 + num4 * (1f / 6f);
			int i = frameIndex * 19;
			float[] array = curves;
			array[i++] = 2f;
			float num9 = num7;
			float num10 = num8;
			for (int num11 = i + 19 - 1; i < num11; i += 2)
			{
				array[i] = num9;
				array[i + 1] = num10;
				num7 += num5;
				num8 += num6;
				num5 += num3;
				num6 += num4;
				num9 += num7;
				num10 += num8;
			}
		}
	}
}
