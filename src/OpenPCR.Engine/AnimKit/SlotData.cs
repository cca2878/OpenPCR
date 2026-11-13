using global::OpenPCR.AnimKit;
using System;
namespace OpenPCR.AnimKit
{
	public class SlotData
	{
		public string Name
		{
			[global::OpenPCR.Migration("bf999e51-2e61-4a84-af4d-042438fe97ab")]
			get { return name; }
		}

		internal int index;
		internal string name;
		internal BoneData boneData;
		internal float r = 1f;
		internal float g = 1f;
		internal float b = 1f;
		internal float a = 1f;
		internal float r2;
		internal float g2;
		internal float b2;
		internal string attachmentName;

		[global::OpenPCR.Migration("33ff9620-518c-49c9-8841-10cdd0cc1d40")]
		public SlotData(int index, string name, BoneData boneData)
		{
			if (index < 0)
			{
				throw new ArgumentException("index must be >= 0.", "index");
			}

			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}

			if (boneData == null)
			{
				throw new ArgumentNullException("boneData", "boneData cannot be null.");
			}

			this.index = index;
			this.name = name;
			this.boneData = boneData;
		}
	}
}
