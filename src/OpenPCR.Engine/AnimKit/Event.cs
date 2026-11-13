using global::OpenPCR.AnimKit;
using System;
namespace OpenPCR.AnimKit
{
	public class Event
	{
		internal readonly EventData data;
		internal readonly float time;
		internal int intValue;
		internal float floatValue;
		internal string stringValue;

		[global::OpenPCR.ImplicitMigration("29f46b4f-8448-4cfe-823b-d7d5a49a1933")]
		public EventData Data => throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.AnimKit.Event::get_Data():OpenPCR.AnimKit.EventData: runtime capability is unavailable.");
		public float Time => time;
		public int Int { [global::OpenPCR.Migration("3e35c6b9-3de0-4df5-b279-ce924693900a")] get => intValue; set => intValue = value; }
		public float Float { [global::OpenPCR.Migration("96b5c47e-c248-4998-a329-b6582fdc6839")] get => floatValue; set => floatValue = value; }
		public string String { get => stringValue; set => stringValue = value; }

		public Event(float time, EventData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}

			this.time = time;
			this.data = data;
		}
	}
}
