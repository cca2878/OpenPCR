using global::OpenPCR.AnimKit;
using System;
using System.Collections.Generic;

namespace OpenPCR.AnimKit
{
	public class IkConstraintData
	{
		internal string name;
		internal int order;
		internal List<BoneData> bones = new List<BoneData>();
		internal BoneData target;
		internal int bendDirection = 1;
		internal float mix = 1f;

		public IkConstraintData(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}

			this.name = name;
		}
	}
}
