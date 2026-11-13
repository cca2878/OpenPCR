using global::OpenPCR.AnimKit;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenPCR.AnimKit
{
	public class TrackEntry : Pool<TrackEntry>.IPoolable
	{
		internal Animation animation;
		internal TrackEntry next;
		internal TrackEntry mixingFrom;
		internal int trackIndex;
		internal bool loop;
		internal float eventThreshold;
		internal float attachmentThreshold;
		internal float drawOrderThreshold;
		internal float animationStart;
		internal float animationEnd;
		internal float animationLast;
		internal float nextAnimationLast;
		internal float delay;
		internal float trackTime;
		internal float trackLast;
		internal float nextTrackLast;
		internal float trackEnd;
		internal float timeScale = 1f;
		internal float alpha;
		internal float mixTime;
		internal float mixDuration;
		internal float interruptAlpha;
		internal float totalAlpha;
		internal readonly ExposedList<int> timelineData = new ExposedList<int>();
		internal readonly ExposedList<TrackEntry> timelineDipMix = new ExposedList<TrackEntry>();
		internal readonly ExposedList<float> timelinesRotation = new ExposedList<float>();
		[global::OpenPCR.ImplicitMigration("1d14461c-e5d5-40dd-a7a2-56f9c4a79c14")]
		public Animation Animation => animation;

		[global::OpenPCR.Migration("ea898d9d-030a-43d1-868e-0b0032c8753f")]
		public float time
		{
			[global::OpenPCR.Migration("d9dcd049-acd3-433a-9bab-e9fdb160dbb1")]
			get
			{
				return trackTime;
			}

			[global::OpenPCR.Migration("c0c631f9-e137-46ab-8972-52606f0ae8bb")]
			set
			{
				trackTime = value;
			}
		}

		[global::OpenPCR.Migration("2c1c40d2-f301-4cb6-95d5-6a836914490f")]
		public float lastTime
		{
			[global::OpenPCR.Migration("f40f7427-dad0-455f-8be7-e4778f300d4e")]
			get
			{
				return animationLast;
			}

			[global::OpenPCR.Migration("62248ef5-8824-4267-be07-771a0facd2e0")]
			set
			{
				animationLast = value;
			}
		}

		[global::OpenPCR.Migration("9492ca3b-a104-4ca7-bdcb-e91ec0d12b74")]
		public float AnimationTime
		{
			[global::OpenPCR.Migration("d56451a9-a3ed-4312-a3ad-373bb94ed438")]
			get
			{
				if (loop)
				{
					float num = animationEnd - animationStart;
					if (num == 0f)
					{
						return animationStart;
					}

					return trackTime % num + animationStart;
				}

				return Math.Min(trackTime + animationStart, animationEnd);
			}
		}
		[global::OpenPCR.ImplicitMigration("f943188a-bda8-465c-8aee-7386ae5c712e")][global::OpenPCR.Migration("e4624e65-53ba-4b91-8541-40e36a9c1ff4")] public bool IsComplete => trackTime >= animationEnd - animationStart;

		[global::OpenPCR.Migration("3f25bb03-aecd-43dc-acad-de4eb241d82e")]
		public TrackEntry()
		{
		}

		[global::OpenPCR.Migration("adaabcb5-4640-4b91-a980-b9aba26fe947")]
		public void Reset()
		{
			next = null;
			mixingFrom = null;
			animation = null;
			timelineData.Clear();
			timelineDipMix.Clear();
			timelinesRotation.Clear();
			Start = null;
			Interrupt = null;
			End = null;
			Dispose = null;
			Complete = null;
			Event = null;
		}

		[global::OpenPCR.Migration("b1532244-338d-4e99-8959-180a900d30ea")]
		internal TrackEntry SetTimelineData(TrackEntry to, ExposedList<TrackEntry> mixingToArray, HashSet<int> propertyIDs)
		{
			if (to != null)
			{
				mixingToArray.Add(to);
			}
			TrackEntry result = ((mixingFrom != null) ? mixingFrom.SetTimelineData(this, mixingToArray, propertyIDs) : this);
			if (to != null)
			{
				mixingToArray.Pop();
			}
			TrackEntry[] items = mixingToArray.Items;
			int num = mixingToArray.Count - 1;
			Timeline[] items2 = animation.timelines.Items;
			int count = animation.timelines.Count;
			int[] items3 = timelineData.Resize(count).Items;
			timelineDipMix.Clear();
			TrackEntry[] items4 = timelineDipMix.Resize(count).Items;
			for (int i = 0; i < count; i++)
			{
				int propertyId = items2[i].PropertyId;
				if (!propertyIDs.Add(propertyId))
				{
					items3[i] = 0;
					continue;
				}
				if (to == null || !to.HasTimeline(propertyId))
				{
					items3[i] = 1;
					continue;
				}
				int num2 = num;
				while (true)
				{
					if (num2 >= 0)
					{
						TrackEntry trackEntry = items[num2];
						if (trackEntry.HasTimeline(propertyId))
						{
							num2--;
							continue;
						}
						if (trackEntry.mixDuration > 0f)
						{
							items3[i] = 3;
							items4[i] = trackEntry;
							break;
						}
					}
					items3[i] = 2;
					break;
				}
			}
			return result;
		}

		[global::OpenPCR.Migration("c51fb615-a61c-4e79-aace-02d173c9a164")]
		[global::OpenPCR.Optimization("cb3ccbc4-a1e1-45fc-b4d5-53c8c417414f")]
		private bool HasTimeline(int id)
		{
#if OPENPCR_OPTIMIZED
			HashSet<int> ids = animation.timelineIds;
			if (ids == null)
			{
				ids = new HashSet<int>();
				Timeline[] timelines = animation.timelines.Items;
				for (int index = 0, total = animation.timelines.Count; index < total; index++) ids.Add(timelines[index].PropertyId);
				animation.timelineIds = ids;
			}
			return ids.Contains(id);
#else
			Timeline[] items = animation.timelines.Items;
			int i = 0;
			for (int count = animation.timelines.Count; i < count; i++)
			{
				if (items[i].PropertyId == id)
				{
					return true;
				}
			}

			return false;
#endif
		}

		[global::OpenPCR.Migration("174a523a-06ae-4308-9926-fd173b33f4a0")]
		internal void OnStart()
		{
			if (Start != null)
			{
				Start(this);
			}
		}

		[global::OpenPCR.Migration("93d59087-34e8-4045-b02a-b81b6442c2b5")]
		internal void OnInterrupt()
		{
			if (Interrupt != null)
			{
				Interrupt(this);
			}
		}

		[global::OpenPCR.Migration("ff2a5660-2c6f-44a5-9d35-9fc7e467568d")]
		internal void OnEnd()
		{
			if (End != null)
			{
				End(this);
			}
		}

		[global::OpenPCR.Migration("9ec35551-92ca-4334-9a55-6f464670f477")]
		internal void OnDispose()
		{
			if (Dispose != null)
			{
				Dispose(this);
			}
		}

		[global::OpenPCR.Migration("4a8fccad-ea16-44ef-a188-70f58f857a5a")]
		internal void OnComplete()
		{
			if (Complete != null)
			{
				Complete(this);
			}
		}

		[global::OpenPCR.Migration("ba79eb1b-2a34-4f99-ad6c-d834771c2ce9")]
		internal void OnEvent(Event e)
		{
			if (Event != null)
			{
				Event(this, e);
			}
		}
		public event global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate Start; public event global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate Interrupt; public event global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate End; public event global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate Dispose; public event global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate Complete; public event global::OpenPCR.AnimKit.AnimationState.TrackEntryEventDelegate Event;
	}
}
