using global::OpenPCR.AnimKit;
using System;
namespace OpenPCR.AnimKit
{
	public class BoneData
	{
		internal int index;
		internal string name;
		internal BoneData parent;
		internal float length;
		internal float x;
		internal float y;
		internal float rotation;
		internal float scaleX = 1f;
		internal float scaleY = 1f;
		internal float shearX;
		internal float shearY;
		internal TransformMode transformMode;

		[global::OpenPCR.Migration("a7152c5f-e97e-4c96-9d05-2a3ebfd1bf47")]
		public BoneData(int index, string name, BoneData parent)
		{
			if (index < 0)
			{
				throw new ArgumentException("index must be >= 0", "index");
			}

			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}

			this.index = index;
			this.name = name;
			this.parent = parent;
		}
	}
}
