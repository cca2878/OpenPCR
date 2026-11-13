using global::OpenPCR.AnimKit;
using System;

namespace OpenPCR.AnimKit
{
	internal class EventQueue
	{
		private class EventQueueEntry
		{
			public EventType type;

			public TrackEntry entry;

			public Event e;

			[global::OpenPCR.Migration("0ad3cc03-8b75-44db-8efe-5fa4b9ce4c8f")]
			public EventQueueEntry(EventType eventType, TrackEntry trackEntry, Event e = null)
			{
				type = eventType;
				entry = trackEntry;
				this.e = e;
			}

			[global::OpenPCR.Migration("379fc4b0-d935-4c23-a3e1-1ff880a95412")]
			public void SetData(EventType _eventType, TrackEntry _trackEntry, Event _event = null)
			{
				type = _eventType;
				entry = _trackEntry;
				e = _event;
			}
		}

		private enum EventType
		{
			NULL,
			Start,
			Interrupt,
			End,
			Dispose,
			Complete,
			Event
		}

		private readonly ExposedList<EventQueueEntry> eventQueueEntries = new ExposedList<EventQueueEntry>();

		internal bool drainDisabled;
#if OPENPCR_OPTIMIZED
		[global::OpenPCR.Optimization("1b018c81-2e6d-44b4-99c9-40b3d69490ee")]
		internal int PendingCount()
		{
			return eventQueueEntries.Count;
		}
#endif

		private readonly AnimationState state;

		private readonly Pool<TrackEntry> trackEntryPool;

		private const int EVENT_QUEUE_ENTRY_POOL = 32;

		private EventQueueEntry[] eventQueueEntryPool = new EventQueueEntry[EVENT_QUEUE_ENTRY_POOL];

		internal event Action AnimationsChanged;

		internal EventQueue(AnimationState state, Action HandleAnimationsChanged, Pool<TrackEntry> trackEntryPool)
		{
			this.state = state;
			AnimationsChanged += HandleAnimationsChanged;
			this.trackEntryPool = trackEntryPool;
			initEventQueueExtryPool();
		}

		[global::OpenPCR.Migration("5363f863-a424-4109-85a9-614bf6e3d568")]
		internal void Start(TrackEntry entry)
		{
			eventQueueEntries.Add(eventQueueExtryGet(EventType.Start, entry));
			if (AnimationsChanged != null)
			{
				AnimationsChanged();
			}
		}

		[global::OpenPCR.Migration("2d573aa6-3530-4b4f-bd0f-e1bf3b85b5c7")]
		internal void Interrupt(TrackEntry entry)
		{
			eventQueueEntries.Add(eventQueueExtryGet(EventType.Interrupt, entry));
		}

		[global::OpenPCR.Migration("5ba9deb5-2ee4-4ece-a9bc-a0e59bde0422")]
		internal void End(TrackEntry entry)
		{
			eventQueueEntries.Add(eventQueueExtryGet(EventType.End, entry));
			if (AnimationsChanged != null)
			{
				AnimationsChanged();
			}
		}

		[global::OpenPCR.Migration("f098d5d7-0c67-45d0-8e1d-4ff8fbced42d")]
		internal void Dispose(TrackEntry entry)
		{
			eventQueueEntries.Add(eventQueueExtryGet(EventType.Dispose, entry));
		}

		[global::OpenPCR.Migration("678c7741-2710-443f-be12-4da2ebb92e35")]
		internal void Complete(TrackEntry entry)
		{
			eventQueueEntries.Add(eventQueueExtryGet(EventType.Complete, entry));
		}

		[global::OpenPCR.Migration("c37d8059-8f5d-4029-bd80-5bb3ee6f4099")]
		internal void Event(TrackEntry entry, Event e)
		{
			eventQueueEntries.Add(eventQueueExtryGet(EventType.Event, entry, e));
		}

		[global::OpenPCR.Migration("18e89ecc-ae70-418b-83d1-6c5098305ccb")]
		internal void Drain()
		{
			if (drainDisabled)
			{
				return;
			}
			drainDisabled = true;
			ExposedList<EventQueueEntry> exposedList = eventQueueEntries;
			AnimationState animationState = state;
			for (int i = 0; i < exposedList.Count; i++)
			{
				EventQueueEntry eventQueueEntry = exposedList.Items[i];
				TrackEntry entry = eventQueueEntry.entry;
				switch (eventQueueEntry.type)
				{
					case EventType.Start:
						entry.OnStart();
						animationState.OnStart(entry);
						break;
					case EventType.Interrupt:
						entry.OnInterrupt();
						animationState.OnInterrupt(entry);
						break;
					case EventType.End:
						entry.OnEnd();
						animationState.OnEnd(entry);
						goto case EventType.Dispose;
					case EventType.Dispose:
						entry.OnDispose();
						animationState.OnDispose(entry);
						trackEntryPool.Free(entry);
						break;
					case EventType.Complete:
						entry.OnComplete();
						animationState.OnComplete(entry);
						break;
					case EventType.Event:
						entry.OnEvent(eventQueueEntry.e);
						animationState.OnEvent(entry, eventQueueEntry.e);
						break;
				}
			}
			eventQueueEntriesClear();
			drainDisabled = false;
		}

		private void initEventQueueExtryPool()
		{
			for (int i = 0; i < eventQueueEntryPool.Length; i++)
			{
				eventQueueEntryPool[i] = new EventQueueEntry(EventType.NULL, null);
			}
		}

		[global::OpenPCR.Migration("5c9e4112-1c01-44c2-a15d-cc8ef270fa41")]
		private EventQueueEntry eventQueueExtryGet(EventType _eventType, TrackEntry _trackEntry, Event _event = null)
		{
			for (int i = 0; i < eventQueueEntryPool.Length; i++)
			{
				EventQueueEntry eventQueueEntry = eventQueueEntryPool[i];
				if (eventQueueEntry.type == EventType.NULL)
				{
					eventQueueEntry.SetData(_eventType, _trackEntry, _event);
					return eventQueueEntry;
				}
			}
			return new EventQueueEntry(_eventType, _trackEntry, _event);
		}

		[global::OpenPCR.Migration("63063b1e-1504-44ce-8050-891abe9d4452")]
		private void eventQueueExtryRelease(EventQueueEntry _enentQueueEntry)
		{
			_enentQueueEntry.type = EventType.NULL;
		}

		[global::OpenPCR.Migration("93b91cf4-952d-4cd8-b7ff-18cc04fcb69e")]
		private void eventQueueEntriesClear()
		{
			for (int i = 0; i < eventQueueEntries.Count; i++)
			{
				EventQueueEntry enentQueueEntry = eventQueueEntries.Items[i];
				eventQueueExtryRelease(enentQueueEntry);
			}
			eventQueueEntries.Clear();
		}
	}
}
