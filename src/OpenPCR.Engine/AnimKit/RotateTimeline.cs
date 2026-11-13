using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class RotateTimeline : CurveTimeline
	{
		private int boneIndex;
		internal float[] frames;
		private int propertyId;
		[global::OpenPCR.ImplicitMigration("e6bf74e5-6673-4265-a371-ada41d6099b8")]
		public int BoneIndex => boneIndex;

		public override int PropertyId => propertyId;

		[global::OpenPCR.Migration("ad95db70-0d1c-4954-a9a6-b7805bcda13c")]
		public RotateTimeline(int frameCount, int _boneIndex) : base(frameCount)
		{
			frames = new float[frameCount << 1];
			boneIndex = _boneIndex;
			propertyId = boneIndex;
		}

		[global::OpenPCR.Migration("780acbcc-faa7-4e0c-922c-4f4946790b56")]
		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			Bone bone = skeleton.bones.Items[boneIndex];
			float[] array = frames;
			if (time < array[0])
			{
				switch (pose)
				{
					case MixPose.Setup:
						bone.rotation = bone.data.rotation;
						break;
					case MixPose.Current:
						{
							float num = bone.data.rotation - bone.rotation;
							num -= (float)((16384 - (int)(16384.499999999996 - (double)(num / 360f))) * 360);
							bone.rotation += num * alpha;
							break;
						}
				}

				return;
			}

			if (time >= array[array.Length - 2])
			{
				if (pose == MixPose.Setup)
				{
					bone.rotation = bone.data.rotation + array[array.Length + -1] * alpha;
					return;
				}

				float num2 = bone.data.rotation + array[array.Length + -1] - bone.rotation;
				num2 -= (float)((16384 - (int)(16384.499999999996 - (double)(num2 / 360f))) * 360);
				bone.rotation += num2 * alpha;
				return;
			}

			int num3 = Animation.BinarySearch(array, time, 2);
			float num4 = array[num3 + -1];
			float num5 = array[num3];
			float curvePercent = GetCurvePercent((num3 >> 1) - 1, 1f - (time - num5) / (array[num3 + -2] - num5));
			float num6 = array[num3 + 1] - num4;
			num6 -= (float)((16384 - (int)(16384.499999999996 - (double)(num6 / 360f))) * 360);
			num6 = num4 + num6 * curvePercent;
			if (pose == MixPose.Setup)
			{
				num6 -= (float)((16384 - (int)(16384.499999999996 - (double)(num6 / 360f))) * 360);
				bone.rotation = bone.data.rotation + num6 * alpha;
			}
			else
			{
				num6 = bone.data.rotation + num6 - bone.rotation;
				num6 -= (float)((16384 - (int)(16384.499999999996 - (double)(num6 / 360f))) * 360);
				bone.rotation += num6 * alpha;
			}
		}

		[global::OpenPCR.Migration("078236e1-ac89-4f95-9728-7ae693235e76")]
		public void SetFrame(int frameIndex, float time, float degrees)
		{
			frameIndex <<= 1;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = degrees;
		}
	}
}
