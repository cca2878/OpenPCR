using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class TransformConstraintTimeline : CurveTimeline
	{

		private int transformConstraintIndex;

		internal float[] frames;

		private int propertyId;

		public override int PropertyId => propertyId;

		[global::OpenPCR.Migration("821fdad5-5b7e-400a-a04c-f93586955986")]
		public TransformConstraintTimeline(int frameCount, int _transformConstraintIndex)
			: base(frameCount)
		{
			frames = new float[frameCount * 5];
			transformConstraintIndex = _transformConstraintIndex;
			propertyId = 167772160 + transformConstraintIndex;
		}

		[global::OpenPCR.Migration("5aa9f03b-853f-4019-8836-36c8e6015456")]
		public void SetFrame(int frameIndex, float time, float rotateMix, float translateMix, float scaleMix, float shearMix)
		{
			frameIndex *= 5;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = rotateMix;
			frames[frameIndex + 2] = translateMix;
			frames[frameIndex + 3] = scaleMix;
			frames[frameIndex + 4] = shearMix;
		}

		[global::OpenPCR.Migration("c0556494-44b9-4148-927d-6e8e2b3772e5")]
		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			TransformConstraint transformConstraint = skeleton.transformConstraints.Items[transformConstraintIndex];
			float[] array = frames;
			if (time < array[0])
			{
				TransformConstraintData data = transformConstraint.data;
				switch (pose)
				{
					case MixPose.Setup:
						transformConstraint.rotateMix = data.rotateMix;
						transformConstraint.translateMix = data.translateMix;
						transformConstraint.scaleMix = data.scaleMix;
						transformConstraint.shearMix = data.shearMix;
						break;
					case MixPose.Current:
						transformConstraint.rotateMix += (data.rotateMix - transformConstraint.rotateMix) * alpha;
						transformConstraint.translateMix += (data.translateMix - transformConstraint.translateMix) * alpha;
						transformConstraint.scaleMix += (data.scaleMix - transformConstraint.scaleMix) * alpha;
						transformConstraint.shearMix += (data.shearMix - transformConstraint.shearMix) * alpha;
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
			if (pose == MixPose.Setup)
			{
				TransformConstraintData data2 = transformConstraint.data;
				transformConstraint.rotateMix = data2.rotateMix + (num2 - data2.rotateMix) * alpha;
				transformConstraint.translateMix = data2.translateMix + (num3 - data2.translateMix) * alpha;
				transformConstraint.scaleMix = data2.scaleMix + (num4 - data2.scaleMix) * alpha;
				transformConstraint.shearMix = data2.shearMix + (num5 - data2.shearMix) * alpha;
			}
			else
			{
				transformConstraint.rotateMix += (num2 - transformConstraint.rotateMix) * alpha;
				transformConstraint.translateMix += (num3 - transformConstraint.translateMix) * alpha;
				transformConstraint.scaleMix += (num4 - transformConstraint.scaleMix) * alpha;
				transformConstraint.shearMix += (num5 - transformConstraint.shearMix) * alpha;
			}
		}
	}
}
