using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class PathConstraintSpacingTimeline : PathConstraintPositionTimeline
	{

		public PathConstraintSpacingTimeline(int frameCount, int _pathConstraintIndex)
			: base(frameCount, _pathConstraintIndex)
		{
			pathConstraintIndex = _pathConstraintIndex;
			propertyId = 201326592 + pathConstraintIndex;
		}

		[global::OpenPCR.Migration("c7722b63-19dc-48ae-8b23-ade936d9988f")]
		public override void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			PathConstraint pathConstraint = skeleton.pathConstraints.Items[pathConstraintIndex];
			float[] array = frames;
			if (time < array[0])
			{
				switch (pose)
				{
					case MixPose.Setup:
						pathConstraint.spacing = pathConstraint.data.spacing;
						break;
					case MixPose.Current:
						pathConstraint.spacing += (pathConstraint.data.spacing - pathConstraint.spacing) * alpha;
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
				pathConstraint.spacing = pathConstraint.data.spacing + (num - pathConstraint.data.spacing) * alpha;
			}
			else
			{
				pathConstraint.spacing += (num - pathConstraint.spacing) * alpha;
			}
		}
	}
}
