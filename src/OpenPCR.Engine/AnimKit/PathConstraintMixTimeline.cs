using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class PathConstraintMixTimeline : CurveTimeline
	{

		private int pathConstraintIndex;

		internal float[] frames;

		private int propertyId;

		public override int PropertyId => propertyId;

		[global::OpenPCR.Migration("45e1649e-656a-423d-a54d-8fee5b862886")]
		public PathConstraintMixTimeline(int frameCount, int _pathConstraintIndex)
			: base(frameCount)
		{
			frames = new float[frameCount * 3];
			pathConstraintIndex = _pathConstraintIndex;
			propertyId = 218103808 + pathConstraintIndex;
		}

		[global::OpenPCR.Migration("95c01d06-d88b-4ebc-857f-08856b35a001")]
		public void SetFrame(int frameIndex, float time, float rotateMix, float translateMix)
		{
			frameIndex *= 3;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = rotateMix;
			frames[frameIndex + 2] = translateMix;
		}

		[global::OpenPCR.Migration("1cd81456-1e6f-4b62-8978-16b531f95211")]
		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			PathConstraint pathConstraint = skeleton.pathConstraints.Items[pathConstraintIndex];
			float[] array = frames;
			if (time < array[0])
			{
				switch (pose)
				{
					case MixPose.Setup:
						pathConstraint.rotateMix = pathConstraint.data.rotateMix;
						pathConstraint.translateMix = pathConstraint.data.translateMix;
						break;
					case MixPose.Current:
						pathConstraint.rotateMix += (pathConstraint.data.rotateMix - pathConstraint.rotateMix) * alpha;
						pathConstraint.translateMix += (pathConstraint.data.translateMix - pathConstraint.translateMix) * alpha;
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
				pathConstraint.rotateMix = pathConstraint.data.rotateMix + (num - pathConstraint.data.rotateMix) * alpha;
				pathConstraint.translateMix = pathConstraint.data.translateMix + (num2 - pathConstraint.data.translateMix) * alpha;
			}
			else
			{
				pathConstraint.rotateMix += (num - pathConstraint.rotateMix) * alpha;
				pathConstraint.translateMix += (num2 - pathConstraint.translateMix) * alpha;
			}
		}
	}
}
