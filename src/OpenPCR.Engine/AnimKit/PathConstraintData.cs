using global::OpenPCR.AnimKit;
using System;
namespace OpenPCR.AnimKit
{
	public class PathConstraintData
	{
		internal string name;
		internal int order;
		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();
		internal SlotData target;
		internal PositionMode positionMode;
		internal SpacingMode spacingMode;
		internal RotateMode rotateMode;
		internal float offsetRotation;
		internal float position;
		internal float spacing;
		internal float rotateMix;
		internal float translateMix;

		[global::OpenPCR.Migration("58018f60-e39c-4813-b79d-1038e9a5a5ce")]
		public PathConstraintData(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}

			this.name = name;
		}
	}
}
