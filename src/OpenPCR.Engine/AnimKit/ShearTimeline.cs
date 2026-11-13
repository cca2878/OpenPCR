using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class ShearTimeline : TranslateTimeline
	{
		public ShearTimeline(int frameCount, int _boneIndex)
			: base(frameCount, _boneIndex)
		{
			propertyId = 50331648 + _boneIndex;
		}

		[global::OpenPCR.Migration("9152718f-58a1-4db8-9875-3555e102e141")]
		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			Bone bone = skeleton.bones.Items[base.BoneIndex];
			float[] array = frames;
			if (time < array[0])
			{
				switch (pose)
				{
					case MixPose.Setup:
						bone.shearX = bone.data.shearX;
						bone.shearY = bone.data.shearY;
						break;
					case MixPose.Current:
						bone.shearX += (bone.data.shearX - bone.shearX) * alpha;
						bone.shearY += (bone.data.shearY - bone.shearY) * alpha;
						break;
				}
				return;
			}
			float num;
			float num2;
			if (time >= array[array.Length - 3])
			{
				num = array[array.Length + -2];
				num2 = array[array.Length + -1];
			}
			else
			{
				int num3 = Animation.BinarySearch(array, time, 3);
				num = array[num3 + -2];
				num2 = array[num3 + -1];
				float num4 = array[num3];
				float curvePercent = GetCurvePercent(num3 / 3 - 1, 1f - (time - num4) / (array[num3 + -3] - num4));
				num += (array[num3 + 1] - num) * curvePercent;
				num2 += (array[num3 + 2] - num2) * curvePercent;
			}
			if (pose == MixPose.Setup)
			{
				bone.shearX = bone.data.shearX + num * alpha;
				bone.shearY = bone.data.shearY + num2 * alpha;
			}
			else
			{
				bone.shearX += (bone.data.shearX + num - bone.shearX) * alpha;
				bone.shearY += (bone.data.shearY + num2 - bone.shearY) * alpha;
			}
		}
	}
}
