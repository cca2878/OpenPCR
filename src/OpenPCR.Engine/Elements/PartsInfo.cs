using System;
using System.Collections.Generic;
using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public class PartsInfo
	{
		public int parts_id { get; private set; }
		public long hp { get; private set; }

		public PartsInfo()
		{
			initializePartsInfo();
		}

		private void initializePartsInfo()
		{
			parts_id = 0;
			hp = 0L;
		}
	}
}
