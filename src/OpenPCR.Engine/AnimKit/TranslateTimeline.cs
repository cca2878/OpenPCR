using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class TranslateTimeline : CurveTimeline
	{

		private int boneIndex;

		internal float[] frames;

		protected int propertyId;

		public int BoneIndex
		{
			get
			{
				return boneIndex;
			}
			set
			{
				boneIndex = value;
			}
		}

		public sealed override int PropertyId => propertyId;

		[global::OpenPCR.Migration("d4125c6b-c807-4e6f-86d3-a5c35e1f035b")]
		public TranslateTimeline(int frameCount, int _boneIndex)
			: base(frameCount)
		{
			frames = new float[frameCount * 3];
			boneIndex = _boneIndex;
			propertyId = 16777216 + boneIndex;
		}

		[global::OpenPCR.Migration("19b07630-c3fa-48b4-a9c2-08966fbea53a")]
		public void SetFrame(int frameIndex, float time, float x, float y)
		{
			frameIndex *= 3;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = x;
			frames[frameIndex + 2] = y;
		}

		[global::OpenPCR.Migration("102ddba4-e488-48d2-b9e7-1d6dfac6aa94")]
		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			Bone bone = skeleton.bones.Items[boneIndex];
			float[] array = frames;
			if (time < array[0])
			{
				switch (pose)
				{
					case MixPose.Setup:
						bone.x = bone.data.x;
						bone.y = bone.data.y;
						break;
					case MixPose.Current:
						bone.x += (bone.data.x - bone.x) * alpha;
						bone.y += (bone.data.y - bone.y) * alpha;
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
				bone.x = bone.data.x + num * alpha;
				bone.y = bone.data.y + num2 * alpha;
			}
			else
			{
				bone.x += (bone.data.x + num - bone.x) * alpha;
				bone.y += (bone.data.y + num2 - bone.y) * alpha;
			}
		}
	}
}
