using System;
using System.Runtime.CompilerServices;

namespace OpenPCR.AnimKit
{
	public abstract class Attachment
	{
		public string Name { get; private set; }

		[global::OpenPCR.Migration("d2bd1fec-c3dc-4676-9de9-bd2e9fadffa7")]
		public Attachment(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null");
			}

			Name = name;
		}
	}
}
