using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class EventTimeline : Timeline
	{
		internal float[] frames;

		private Event[] events;

		public int PropertyId => 117440512;

		public EventTimeline(int frameCount)
		{
			frames = new float[frameCount];
			events = new Event[frameCount];
		}

		[global::OpenPCR.Migration("c5043cb3-7365-429a-aa41-516bf3915676")]
		public void SetFrame(int frameIndex, Event e)
		{
			frames[frameIndex] = e.time;
			events[frameIndex] = e;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, ExposedList<Event> firedEvents, float alpha, MixPose pose, MixDirection direction)
		{
			if (firedEvents == null)
			{
				return;
			}
			float[] array = frames;
			int num = array.Length;
			if (lastTime > time)
			{
				Apply(skeleton, lastTime, 2.1474836E+09f, firedEvents, alpha, pose, direction);
				lastTime = -1f;
			}
			else if (lastTime >= array[num - 1])
			{
				return;
			}
			if (time < array[0])
			{
				return;
			}
			int i;
			if (lastTime < array[0])
			{
				i = 0;
			}
			else
			{
				i = Animation.BinarySearch(array, lastTime);
				float num2 = array[i];
				while (i > 0 && array[i - 1] == num2)
				{
					i--;
				}
			}
			for (; i < num && time >= array[i]; i++)
			{
				firedEvents.Add(events[i]);
			}
		}
	}
}
