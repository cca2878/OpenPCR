using System;
using System.Runtime.CompilerServices;

namespace OpenPCR.AnimKit
{
	public class EventData
	{
		internal string name;

		[global::OpenPCR.ImplicitMigration("e785beae-7d23-447d-8330-1f2eb5bf2f29")]
		public string Name => throw new global::OpenPCR.OutOfBoundaryException("OpenPCR.AnimKit.EventData::get_Name():System.String: runtime capability is unavailable.");

		public EventData(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}

			this.name = name;
		}

		public string String { get; set; }
	}
}
