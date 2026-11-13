using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class PathConstraintPositionTimeline : CurveTimeline
	{

		protected int pathConstraintIndex;

		internal float[] frames;

		protected int propertyId;

		public override int PropertyId => propertyId;

		[global::OpenPCR.Migration("34b6ac89-8472-4992-aeea-80f4cd9ab875")]
		public PathConstraintPositionTimeline(int frameCount, int _pathConstraintIndex)
			: base(frameCount)
		{
			frames = new float[frameCount * 2];
			pathConstraintIndex = _pathConstraintIndex;
			propertyId = 184549376 + pathConstraintIndex;
		}

		[global::OpenPCR.Migration("7bd5687f-9e30-490e-b6aa-6954292935cb")]
		public void SetFrame(int frameIndex, float time, float value)
		{
			frameIndex *= 2;
			frames[frameIndex] = time;
			frames[frameIndex + 1] = value;
		}

		[global::OpenPCR.Migration("29d16c43-6643-44c3-8d1c-c0b5692e8078")]
		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			PathConstraint pathConstraint = skeleton.pathConstraints.Items[pathConstraintIndex];
			float[] array = frames;
			if (time < array[0])
			{
				switch (pose)
				{
					case MixPose.Setup:
						pathConstraint.position = pathConstraint.data.position;
						break;
					case MixPose.Current:
						pathConstraint.position += (pathConstraint.data.position - pathConstraint.position) * alpha;
						break;
				}
				return;
			}
			float num;
			if (time >= array[array.Length - 2])
			{
				num = array[array.Length + -1];
			}
			else
			{
				int num2 = Animation.BinarySearch(array, time, 2);
				num = array[num2 + -1];
				float num3 = array[num2];
				float curvePercent = GetCurvePercent(num2 / 2 - 1, 1f - (time - num3) / (array[num2 + -2] - num3));
				num += (array[num2 + 1] - num) * curvePercent;
			}
			if (pose == MixPose.Setup)
			{
				pathConstraint.position = pathConstraint.data.position + (num - pathConstraint.data.position) * alpha;
			}
			else
			{
				pathConstraint.position += (num - pathConstraint.position) * alpha;
			}
		}
	}
}
