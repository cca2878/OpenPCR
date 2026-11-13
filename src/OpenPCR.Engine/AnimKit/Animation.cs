using global::OpenPCR.AnimKit;
using System;
namespace OpenPCR.AnimKit
{
	public class Animation
	{
		internal ExposedList<Timeline> timelines;
#if OPENPCR_OPTIMIZED
		[global::OpenPCR.Optimization("56144106-c329-44bc-a878-e12c44c5e574")]
		internal System.Collections.Generic.HashSet<int> timelineIds;
#endif
		internal float duration;
		internal string name;
		[global::OpenPCR.ImplicitMigration("af42e30d-d8d1-44eb-a625-a0bf1b243c7f")]
		public string Name => name;

		[global::OpenPCR.Migration("2db12596-2a3b-445d-abe3-3555629d3577")]
		public float Duration
		{
			[global::OpenPCR.Migration("b7c33a07-4828-4b98-bf3f-086190060b0f")]
			get
			{
				return duration;
			}

			[global::OpenPCR.Migration("17f5f6fd-6f5d-4abf-a1a2-9a9d49557387")]
			set
			{
				duration = value;
			}
		}

		public Animation(string name, ExposedList<Timeline> timelines, float duration)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}

			if (timelines == null)
			{
				throw new ArgumentNullException("timelines", "timelines cannot be null.");
			}

			this.name = name;
			this.timelines = timelines;
			this.duration = duration;
		}

		[global::OpenPCR.Migration("68702ca2-53c8-47f7-96f3-05cc47978a57")]
		internal static int BinarySearch(float[] values, float target, int step)
		{
			int num = 0;
			int num2 = values.Length / step - 2;
			if (num2 == 0)
			{
				return step;
			}

			int num3 = (int)((uint)num2 >> 1);
			while (true)
			{
				if (values[(num3 + 1) * step] <= target)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3;
				}

				if (num == num2)
				{
					break;
				}

				num3 = (int)((uint)(num + num2) >> 1);
			}

			return (num + 1) * step;
		}

		[global::OpenPCR.Migration("6c560d3f-5508-4d98-a9ff-4b6032b5986c")]
		internal static int BinarySearch(float[] values, float target)
		{
			int num = 0;
			int num2 = values.Length - 2;
			if (num2 == 0)
			{
				return 1;
			}
			int num3 = (int)((uint)num2 >> 1);
			while (true)
			{
				if (values[num3 + 1] <= target)
				{
					num = num3 + 1;
				}
				else
				{
					num2 = num3;
				}
				if (num == num2)
				{
					break;
				}
				num3 = (int)((uint)(num + num2) >> 1);
			}
			return num + 1;
		}
	}
}
