using global::OpenPCR.AnimKit;
using System;
namespace OpenPCR.AnimKit
{
	public class TransformConstraintData
	{
		internal string name;
		internal int order;
		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();
		internal BoneData target;
		internal float rotateMix;
		internal float translateMix;
		internal float scaleMix;
		internal float shearMix;
		internal float offsetRotation;
		internal float offsetX;
		internal float offsetY;
		internal float offsetScaleX;
		internal float offsetScaleY;
		internal float offsetShearY;
		internal bool relative;
		internal bool local;

		[global::OpenPCR.Migration("ec04d59f-1cab-4c5a-8165-b4da86a76443")]
		public TransformConstraintData(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}

			this.name = name;
		}
	}
}
