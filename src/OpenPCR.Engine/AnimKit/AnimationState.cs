using global::OpenPCR.AnimKit;
using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace OpenPCR.AnimKit
{
	public class AnimationState
	{
		public delegate void TrackEntryDelegate(TrackEntry trackEntry);
		public delegate void TrackEntryEventDelegate(TrackEntry trackEntry, Event e);
		private AnimationStateData data;
		private Pool<TrackEntry> trackEntryPool = new Pool<TrackEntry>();
		private readonly ExposedList<TrackEntry> tracks = new ExposedList<TrackEntry>();
		private readonly ExposedList<Event> events = new ExposedList<Event>();
		private readonly EventQueue queue;
		private readonly HashSet<int> propertyIDs = new HashSet<int>();
		private readonly ExposedList<TrackEntry> mixingTo = new ExposedList<TrackEntry>();
		private bool animationsChanged;
		private float timeScale = 1f;
		[global::OpenPCR.ImplicitMigration("ad7303d4-490e-421e-bbc2-ff28af152aa6")]
		public AnimationStateData Data => data;
#if OPENPCR_OPTIMIZED
		[global::OpenPCR.Optimization("2e756cfb-fa39-43b2-a261-6376ad004dbc")]
		internal void AppendPausedSignature(List<int> values, List<object> refs, bool deep)
		{
			values.Add(BitConverter.SingleToInt32Bits(timeScale));
			values.Add(animationsChanged ? 1 : 0);
			values.Add(queue.PendingCount());
			values.Add(queue.drainDisabled ? 1 : 0);
			values.Add(tracks.Count);
			TrackEntry[] items = tracks.Items;
			for (int i = 0, n = tracks.Count; i < n; i++) AppendPausedEntry(items[i], values, refs, deep);
			if (deep)
			{
				values.Add(events.Count);
				values.Add(propertyIDs.Count);
				values.Add(mixingTo.Count);
				for (int i = 0, n = mixingTo.Count; i < n; i++) refs.Add(mixingTo.Items[i]);
			}
		}

		[global::OpenPCR.Optimization("e723f69f-1207-441e-b6b8-3d74c0a3366f")]
		internal void AppendPausedEntry(TrackEntry entry, List<int> values, List<object> refs, bool deep)
		{
			refs.Add(entry);
			if (entry == null) return;
			refs.Add(entry.animation);
			values.Add(entry.trackIndex);
			values.Add(entry.loop ? 1 : 0);
			values.Add(BitConverter.SingleToInt32Bits(entry.eventThreshold));
			values.Add(BitConverter.SingleToInt32Bits(entry.attachmentThreshold));
			values.Add(BitConverter.SingleToInt32Bits(entry.drawOrderThreshold));
			values.Add(BitConverter.SingleToInt32Bits(entry.animationStart));
			values.Add(BitConverter.SingleToInt32Bits(entry.animationEnd));
			values.Add(BitConverter.SingleToInt32Bits(entry.animationLast));
			values.Add(BitConverter.SingleToInt32Bits(entry.nextAnimationLast));
			values.Add(BitConverter.SingleToInt32Bits(entry.delay));
			values.Add(BitConverter.SingleToInt32Bits(entry.trackTime));
			values.Add(BitConverter.SingleToInt32Bits(entry.trackLast));
			values.Add(BitConverter.SingleToInt32Bits(entry.nextTrackLast));
			values.Add(BitConverter.SingleToInt32Bits(entry.trackEnd));
			values.Add(BitConverter.SingleToInt32Bits(entry.timeScale));
			values.Add(BitConverter.SingleToInt32Bits(entry.alpha));
			values.Add(BitConverter.SingleToInt32Bits(entry.mixTime));
			values.Add(BitConverter.SingleToInt32Bits(entry.mixDuration));
			values.Add(BitConverter.SingleToInt32Bits(entry.interruptAlpha));
			values.Add(BitConverter.SingleToInt32Bits(entry.totalAlpha));
			if (deep)
			{
				values.Add(entry.timelineData.Count);
				for (int i = 0, n = entry.timelineData.Count; i < n; i++) values.Add(entry.timelineData.Items[i]);
				values.Add(entry.timelineDipMix.Count);
				for (int i = 0, n = entry.timelineDipMix.Count; i < n; i++) refs.Add(entry.timelineDipMix.Items[i]);
				values.Add(entry.timelinesRotation.Count);
				for (int i = 0, n = entry.timelinesRotation.Count; i < n; i++) values.Add(BitConverter.SingleToInt32Bits(entry.timelinesRotation.Items[i]));
			}
			AppendPausedEntry(entry.mixingFrom, values, refs, deep);
			AppendPausedEntry(entry.next, values, refs, deep);
		}
#endif

		public float TimeScale
		{
			[global::OpenPCR.Migration("d9c82054-b700-4872-a687-3cae8fcf6046")]
			get
			{
				return timeScale;
			}

			[global::OpenPCR.Migration("3a6809db-be89-4ba5-a362-2843229a315a")]
			set
			{
				timeScale = value;
			}
		}

		public event TrackEntryDelegate Start
;

		public event TrackEntryDelegate Complete
;

		public event TrackEntryEventDelegate Event
;

		[global::OpenPCR.Migration("4fb1d3a9-a35e-4a95-87df-6ad82a7acd1f")]
		public AnimationState(AnimationStateData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}

			this.data = data;
			queue = new EventQueue(this, delegate
			{
				animationsChanged = true;
			}, trackEntryPool);
		}

		[global::OpenPCR.Migration("167d2913-4f45-4545-b1cc-2010d07caef6")]
		public void Update(float delta)
		{
			delta *= timeScale;
			TrackEntry[] items = tracks.Items;
			int i = 0;
			for (int count = tracks.Count; i < count; i++)
			{
				TrackEntry trackEntry = items[i];
				if (trackEntry == null)
				{
					continue;
				}

				trackEntry.animationLast = trackEntry.nextAnimationLast;
				trackEntry.trackLast = trackEntry.nextTrackLast;
				float num = delta * trackEntry.timeScale;
				if (trackEntry.delay > 0f)
				{
					trackEntry.delay -= num;
					if (trackEntry.delay > 0f)
					{
						continue;
					}

					num = -trackEntry.delay;
					trackEntry.delay = 0f;
				}

				TrackEntry trackEntry2 = trackEntry.next;
				if (trackEntry2 != null)
				{
					float num2 = trackEntry.trackLast - trackEntry2.delay;
					if (num2 >= 0f)
					{
						trackEntry2.delay = 0f;
						trackEntry2.trackTime = num2 + delta * trackEntry2.timeScale;
						trackEntry.trackTime += num;
						SetCurrent(i, trackEntry2, interrupt: true);
						while (trackEntry2.mixingFrom != null)
						{
							trackEntry2.mixTime += num;
							trackEntry2 = trackEntry2.mixingFrom;
						}

						continue;
					}
				}
				else if (trackEntry.trackLast >= trackEntry.trackEnd && trackEntry.mixingFrom == null)
				{
					items[i] = null;
					queue.End(trackEntry);
					DisposeNext(trackEntry);
					continue;
				}

				if (trackEntry.mixingFrom != null && UpdateMixingFrom(trackEntry, delta))
				{
					TrackEntry mixingFrom = trackEntry.mixingFrom;
					trackEntry.mixingFrom = null;
					while (mixingFrom != null)
					{
						queue.End(mixingFrom);
						mixingFrom = mixingFrom.mixingFrom;
					}
				}

				trackEntry.trackTime += num;
			}

			queue.Drain();
		}

		[global::OpenPCR.Migration("204f1924-91d9-445e-93e7-99ce7bf103e6")]
		private bool UpdateMixingFrom(TrackEntry to, float delta)
		{
			TrackEntry mixingFrom = to.mixingFrom;
			if (mixingFrom == null)
			{
				return true;
			}

			bool result = UpdateMixingFrom(mixingFrom, delta);
			if (to.mixTime > 0f && (to.mixTime >= to.mixDuration || to.timeScale == 0f))
			{
				if (mixingFrom.totalAlpha == 0f || to.mixDuration == 0f)
				{
					to.mixingFrom = mixingFrom.mixingFrom;
					to.interruptAlpha = mixingFrom.interruptAlpha;
					queue.End(mixingFrom);
				}

				return result;
			}

			mixingFrom.animationLast = mixingFrom.nextAnimationLast;
			mixingFrom.trackLast = mixingFrom.nextTrackLast;
			mixingFrom.trackTime += delta * mixingFrom.timeScale;
			to.mixTime += delta * to.timeScale;
			return false;
		}

		[global::OpenPCR.Migration("83286c64-e1af-49de-8dd2-6f3d8933619e")]
		public bool Apply(Skeleton skeleton)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}

			if (animationsChanged)
			{
				AnimationsChanged();
			}

			ExposedList<Event> exposedList = events;
			bool result = false;
			TrackEntry[] items = tracks.Items;
			int i = 0;
			for (int count = tracks.Count; i < count; i++)
			{
				TrackEntry trackEntry = items[i];
				if (trackEntry == null || trackEntry.delay > 0f)
				{
					continue;
				}

				result = true;
				MixPose mixPose = ((i == 0) ? MixPose.Current : MixPose.CurrentLayered);
				float num = trackEntry.alpha;
				if (trackEntry.mixingFrom != null)
				{
					num *= ApplyMixingFrom(trackEntry, skeleton, mixPose);
				}
				else if (trackEntry.trackTime >= trackEntry.trackEnd && trackEntry.next == null)
				{
					num = 0f;
				}

				float animationLast = trackEntry.animationLast;
				float animationTime = trackEntry.AnimationTime;
				int count2 = trackEntry.animation.timelines.Count;
				ExposedList<Timeline> timelines = trackEntry.animation.timelines;
				Timeline[] items2 = timelines.Items;
				if (num == 1f)
				{
					for (int j = 0; j < count2; j++)
					{
						items2[j].Apply(skeleton, animationLast, animationTime, exposedList, 1f, MixPose.Setup, MixDirection.In);
					}
				}
				else
				{
					int[] items3 = trackEntry.timelineData.Items;
					bool flag = trackEntry.timelinesRotation.Count == 0;
					if (flag)
					{
						trackEntry.timelinesRotation.EnsureCapacity(timelines.Count << 1);
					}

					float[] items4 = trackEntry.timelinesRotation.Items;
					for (int k = 0; k < count2; k++)
					{
						Timeline timeline = items2[k];
						MixPose pose = ((items3[k] < 1) ? mixPose : MixPose.Setup);
						if (timeline is RotateTimeline rotateTimeline)
						{
							ApplyRotateTimeline(rotateTimeline, skeleton, animationTime, num, pose, items4, k << 1, flag);
						}
						else
						{
							timeline.Apply(skeleton, animationLast, animationTime, exposedList, num, pose, MixDirection.In);
						}
					}
				}

				QueueEvents(trackEntry, animationTime);
				exposedList.Clear(clearArray: false);
				trackEntry.nextAnimationLast = animationTime;
				trackEntry.nextTrackLast = trackEntry.trackTime;
			}

			queue.Drain();
			return result;
		}

		[global::OpenPCR.Migration("88b72aed-086c-408f-a7f1-55fd9f0e5f2d")]
		public bool LightApply(Skeleton skeleton)
		{
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton", "skeleton cannot be null.");
			}

			if (animationsChanged)
			{
				AnimationsChanged();
			}

			ExposedList<Event> exposedList = events;
			bool result = false;
			TrackEntry[] items = tracks.Items;
			int i = 0;
			for (int count = tracks.Count; i < count; i++)
			{
				TrackEntry trackEntry = items[i];
				if (trackEntry != null && !(trackEntry.delay > 0f))
				{
					result = true;
					float animationTime = trackEntry.AnimationTime;
					QueueEvents(trackEntry, animationTime);
					exposedList.Clear(clearArray: false);
					trackEntry.nextAnimationLast = animationTime;
					trackEntry.nextTrackLast = trackEntry.trackTime;
				}
			}

			queue.Drain();
			return result;
		}

		[global::OpenPCR.Migration("7786f63f-b800-4946-bf21-22b5743838a7")]
		private float ApplyMixingFrom(TrackEntry to, Skeleton skeleton, MixPose currentPose)
		{
			TrackEntry mixingFrom = to.mixingFrom;
			if (mixingFrom.mixingFrom != null)
			{
				ApplyMixingFrom(mixingFrom, skeleton, currentPose);
			}
			float num;
			if (to.mixDuration == 0f)
			{
				num = 1f;
			}
			else
			{
				num = to.mixTime / to.mixDuration;
				if (num > 1f)
				{
					num = 1f;
				}
			}
			ExposedList<Event> exposedList = ((num >= mixingFrom.eventThreshold) ? null : events);
			bool flag = num < mixingFrom.attachmentThreshold;
			bool flag2 = num < mixingFrom.drawOrderThreshold;
			float animationLast = mixingFrom.animationLast;
			float animationTime = mixingFrom.AnimationTime;
			ExposedList<Timeline> timelines = mixingFrom.animation.timelines;
			int count = timelines.Count;
			Timeline[] items = timelines.Items;
			int[] items2 = mixingFrom.timelineData.Items;
			TrackEntry[] items3 = mixingFrom.timelineDipMix.Items;
			bool flag3 = mixingFrom.timelinesRotation.Count == 0;
			if (flag3)
			{
				mixingFrom.timelinesRotation.Resize(timelines.Count << 1);
			}
			float[] items4 = mixingFrom.timelinesRotation.Items;
			float num2 = mixingFrom.alpha * to.interruptAlpha;
			float num3 = num2 * (1f - num);
			mixingFrom.totalAlpha = 0f;
			for (int i = 0; i < count; i++)
			{
				Timeline timeline = items[i];
				MixPose pose;
				float num4;
				switch (items2[i])
				{
					case 0:
						if ((!flag && timeline is AttachmentTimeline) || (!flag2 && timeline is DrawOrderTimeline))
						{
							continue;
						}
						pose = currentPose;
						num4 = num3;
						break;
					case 1:
						pose = MixPose.Setup;
						num4 = num3;
						break;
					case 2:
						pose = MixPose.Setup;
						num4 = num2;
						break;
					default:
						{
							pose = MixPose.Setup;
							num4 = num2;
							TrackEntry trackEntry = items3[i];
							num4 *= Math.Max(0f, 1f - trackEntry.mixTime / trackEntry.mixDuration);
							break;
						}
				}
				mixingFrom.totalAlpha += num4;
				if (timeline is RotateTimeline rotateTimeline)
				{
					ApplyRotateTimeline(rotateTimeline, skeleton, animationTime, num4, pose, items4, i << 1, flag3);
				}
				else
				{
					timeline.Apply(skeleton, animationLast, animationTime, exposedList, num4, pose, MixDirection.Out);
				}
			}
			if (to.mixDuration > 0f)
			{
				QueueEvents(mixingFrom, animationTime);
			}
			events.Clear(clearArray: false);
			mixingFrom.nextAnimationLast = animationTime;
			mixingFrom.nextTrackLast = mixingFrom.trackTime;
			return num;
		}

		[global::OpenPCR.Migration("ea02bfd5-ab5d-437b-b4e2-667cdf7173d0")]
		private static void ApplyRotateTimeline(RotateTimeline rotateTimeline, Skeleton skeleton, float time, float alpha, MixPose pose, float[] timelinesRotation, int i, bool firstFrame)
		{
			if (firstFrame)
			{
				timelinesRotation[i] = 0f;
			}

			if (alpha == 1f)
			{
				rotateTimeline.Apply(skeleton, 0f, time, null, 1f, pose, MixDirection.In);
				return;
			}

			Bone bone = skeleton.bones.Items[rotateTimeline.BoneIndex];
			float[] frames = rotateTimeline.frames;
			if (time < frames[0])
			{
				if (pose == MixPose.Setup)
				{
					bone.rotation = bone.data.rotation;
				}

				return;
			}

			float num;
			if (time >= frames[frames.Length - 2])
			{
				num = bone.data.rotation + frames[frames.Length + -1];
			}
			else
			{
				int num2 = Animation.BinarySearch(frames, time, 2);
				float num3 = frames[num2 + -1];
				float num4 = frames[num2];
				float curvePercent = rotateTimeline.GetCurvePercent((num2 >> 1) - 1, 1f - (time - num4) / (frames[num2 + -2] - num4));
				num = frames[num2 + 1] - num3;
				num -= (float)((16384 - (int)(16384.499999999996 - (double)(num / 360f))) * 360);
				num = num3 + num * curvePercent + bone.data.rotation;
				num -= (float)((16384 - (int)(16384.499999999996 - (double)(num / 360f))) * 360);
			}

			float num5 = ((pose == MixPose.Setup) ? bone.data.rotation : bone.rotation);
			float num6 = num - num5;
			float num7;
			if (num6 == 0f)
			{
				num7 = timelinesRotation[i];
			}
			else
			{
				num6 -= (float)((16384 - (int)(16384.499999999996 - (double)(num6 / 360f))) * 360);
				float num8;
				float value;
				if (firstFrame)
				{
					num8 = 0f;
					value = num6;
				}
				else
				{
					num8 = timelinesRotation[i];
					value = timelinesRotation[i + 1];
				}

				bool flag = num6 > 0f;
				bool flag2 = num8 >= 0f;
				if (Math.Sign(value) != Math.Sign(num6) && Math.Abs(value) <= 90f)
				{
					if (Math.Abs(num8) > 180f)
					{
						num8 += (float)(360 * Math.Sign(num8));
					}

					flag2 = flag;
				}

				num7 = num6 + num8 - num8 % 360f;
				if (flag2 != flag)
				{
					num7 += (float)(360 * Math.Sign(num8));
				}

				timelinesRotation[i] = num7;
			}

			timelinesRotation[i + 1] = num6;
			num5 += num7 * alpha;
			bone.rotation = num5 - (float)((16384 - (int)(16384.499999999996 - (double)(num5 / 360f))) * 360);
		}

		[global::OpenPCR.Migration("9808a595-977e-4688-96c7-cd29ef2d647c")]
		private void QueueEvents(TrackEntry entry, float animationTime)
		{
			float animationStart = entry.animationStart;
			float animationEnd = entry.animationEnd;
			float num = animationEnd - animationStart;
			float num2 = entry.trackLast % num;
			ExposedList<Event> exposedList = events;
			Event[] items = exposedList.Items;
			int i = 0;
			int count;
			for (count = exposedList.Count; i < count; i++)
			{
				Event obj = items[i];
				if (obj.time < num2)
				{
					break;
				}

				if (!(obj.time > animationEnd))
				{
					queue.Event(entry, obj);
				}
			}

			bool num3;
			if (!entry.loop)
			{
				if (!(animationTime >= animationEnd))
				{
					goto IL_00c3;
				}

				num3 = entry.animationLast < animationEnd;
			}
			else
			{
				num3 = num2 > entry.trackTime % num;
			}

			if (num3)
			{
				queue.Complete(entry);
			}

			goto IL_00c3;
		IL_00c3:
			for (; i < count; i++)
			{
				if (!(items[i].time < animationStart))
				{
					queue.Event(entry, items[i]);
				}
			}
		}

		[global::OpenPCR.Migration("3280cf21-5325-4f13-a4f3-6179c2fd58e6")]
		public void ClearTrack(int trackIndex)
		{
			if (trackIndex >= tracks.Count)
			{
				return;
			}

			TrackEntry trackEntry = tracks.Items[trackIndex];
			if (trackEntry == null)
			{
				return;
			}

			queue.End(trackEntry);
			DisposeNext(trackEntry);
			TrackEntry trackEntry2 = trackEntry;
			while (true)
			{
				TrackEntry mixingFrom = trackEntry2.mixingFrom;
				if (mixingFrom == null)
				{
					break;
				}

				queue.End(mixingFrom);
				trackEntry2.mixingFrom = null;
				trackEntry2 = mixingFrom;
			}

			tracks.Items[trackEntry.trackIndex] = null;
			queue.Drain();
		}

		[global::OpenPCR.Migration("470dcfd5-60ee-406f-aaaf-f1379ba79ddb")]
		private void SetCurrent(int index, TrackEntry current, bool interrupt)
		{
			TrackEntry trackEntry = ExpandToIndex(index);
			tracks.Items[index] = current;
			if (trackEntry != null)
			{
				if (interrupt)
				{
					queue.Interrupt(trackEntry);
				}

				current.mixingFrom = trackEntry;
				current.mixTime = 0f;
				if (trackEntry.mixingFrom != null && trackEntry.mixDuration > 0f)
				{
					current.interruptAlpha *= Math.Min(1f, trackEntry.mixTime / trackEntry.mixDuration);
				}

				trackEntry.timelinesRotation.Clear();
			}

			queue.Start(current);
		}

		[global::OpenPCR.Migration("7869a4a2-5246-4007-ba7b-e024a163c6d6")]
		public TrackEntry SetAnimation(int trackIndex, string animationName, bool loop)
		{
			Animation animation = data.skeletonData.FindAnimation(animationName);
			if (animation == null)
			{
				throw new ArgumentException("Animation not found: " + animationName, "animationName");
			}

			return SetAnimation(trackIndex, animation, loop);
		}

		[global::OpenPCR.Migration("65b64f70-9f80-4cb6-9adf-49644cc997f4")]
		public TrackEntry SetAnimation(int trackIndex, Animation animation, bool loop)
		{
			if (animation == null)
			{
				throw new ArgumentNullException("animation", "animation cannot be null.");
			}

			bool interrupt = true;
			TrackEntry trackEntry = ExpandToIndex(trackIndex);
			if (trackEntry != null)
			{
				if (trackEntry.nextTrackLast == -1f)
				{
					tracks.Items[trackIndex] = trackEntry.mixingFrom;
					queue.Interrupt(trackEntry);
					queue.End(trackEntry);
					DisposeNext(trackEntry);
					trackEntry = trackEntry.mixingFrom;
					interrupt = false;
				}
				else
				{
					DisposeNext(trackEntry);
				}
			}

			TrackEntry trackEntry2 = NewTrackEntry(trackIndex, animation, loop, trackEntry);
			SetCurrent(trackIndex, trackEntry2, interrupt);
			queue.Drain();
			return trackEntry2;
		}

		[global::OpenPCR.Migration("bfa8af36-b8a8-4376-9277-8a87de8d70ac")]
		private TrackEntry ExpandToIndex(int index)
		{
			if (index < tracks.Count)
			{
				return tracks.Items[index];
			}
			while (index >= tracks.Count)
			{
				tracks.Add(null);
			}
			return null;
		}

		[global::OpenPCR.Migration("3cfb3f70-25ed-4ea1-8875-00088d14ffe5")]
		private TrackEntry NewTrackEntry(int trackIndex, Animation animation, bool loop, TrackEntry last)
		{
			TrackEntry trackEntry = trackEntryPool.Obtain();
			trackEntry.trackIndex = trackIndex;
			trackEntry.animation = animation;
			trackEntry.loop = loop;
			trackEntry.eventThreshold = 0f;
			trackEntry.attachmentThreshold = 0f;
			trackEntry.drawOrderThreshold = 0f;
			trackEntry.animationStart = 0f;
			trackEntry.animationEnd = animation.Duration;
			trackEntry.animationLast = -1f;
			trackEntry.nextAnimationLast = -1f;
			trackEntry.delay = 0f;
			trackEntry.trackTime = 0f;
			trackEntry.trackLast = -1f;
			trackEntry.nextTrackLast = -1f;
			trackEntry.trackEnd = float.MaxValue;
			trackEntry.timeScale = 1f;
			trackEntry.alpha = 1f;
			trackEntry.interruptAlpha = 1f;
			trackEntry.mixTime = 0f;
			trackEntry.mixDuration = ((last == null) ? 0f : data.GetMix(last.animation, animation));
			return trackEntry;
		}

		[global::OpenPCR.Migration("cf89dcf3-ba58-495f-a8c9-dcb6cfb89cbd")]
		private void DisposeNext(TrackEntry entry)
		{
			for (TrackEntry next = entry.next; next != null; next = next.next)
			{
				queue.Dispose(next);
			}

			entry.next = null;
		}

		[global::OpenPCR.Migration("85fabd17-0b34-428f-a928-c32b7e9f5b3a")]
		private void AnimationsChanged()
		{
			animationsChanged = false;
			HashSet<int> hashSet = propertyIDs;
			hashSet.Clear();
			ExposedList<TrackEntry> mixingToArray = mixingTo;
			TrackEntry[] items = tracks.Items;
			int i = 0;
			for (int count = tracks.Count; i < count; i++)
			{
				items[i]?.SetTimelineData(null, mixingToArray, hashSet);
			}
		}

		[global::OpenPCR.Migration("33268730-a66e-40b8-a0bf-a960d80639de")]
		public TrackEntry GetCurrent(int trackIndex)
		{
			if (trackIndex < tracks.Count)
			{
				return tracks.Items[trackIndex];
			}

			return null;
		}

		[global::OpenPCR.Migration("72b8ede8-b983-4a6b-bcb7-09a3085c0c69")]
		internal void OnStart(TrackEntry entry)
		{
			if (Start != null)
			{
				Start(entry);
			}
		}

		[global::OpenPCR.Migration("1dbe76c6-80fc-4331-a1f0-74f39e43ef89")]
		internal void OnInterrupt(TrackEntry entry)
		{
			if (Interrupt != null)
			{
				Interrupt(entry);
			}
		}

		[global::OpenPCR.Migration("21b293fa-deef-4dc0-8d04-2cdd7e202c65")]
		internal void OnEnd(TrackEntry entry)
		{
			if (End != null)
			{
				End(entry);
			}
		}

		[global::OpenPCR.Migration("338e14cb-4994-41f6-b4f4-334b69a26149")]
		internal void OnDispose(TrackEntry entry)
		{
			if (Dispose != null)
			{
				Dispose(entry);
			}
		}

		[global::OpenPCR.Migration("70edf7e5-6be3-4688-bb55-6204371b0d7f")]
		internal void OnComplete(TrackEntry entry)
		{
			if (Complete != null)
			{
				Complete(entry);
			}
		}

		[global::OpenPCR.Migration("f8a293f5-ca6c-4557-acd3-3acf399426e1")]
		internal void OnEvent(TrackEntry entry, Event e)
		{
			if (Event != null)
			{
				Event(entry, e);
			}
		}
		public event global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate Interrupt; public event global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate End; public event global::OpenPCR.AnimKit.AnimationState.TrackEntryDelegate Dispose;
	}
}
