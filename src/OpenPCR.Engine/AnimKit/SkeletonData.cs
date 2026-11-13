using global::OpenPCR.AnimKit;
using System;
using System.Collections.Generic;

namespace OpenPCR.AnimKit
{
	public class SkeletonData
	{
		internal string name;
		internal ExposedList<BoneData> bones = new ExposedList<BoneData>();
		internal ExposedList<SlotData> slots = new ExposedList<SlotData>();
		internal ExposedList<Skin> skins = new ExposedList<Skin>();
		internal Skin defaultSkin;
		internal ExposedList<EventData> events = new ExposedList<EventData>();
		internal Dictionary<string, Animation> animations = new Dictionary<string, Animation>();
		internal ExposedList<IkConstraintData> ikConstraints = new ExposedList<IkConstraintData>();
		internal ExposedList<TransformConstraintData> transformConstraints = new ExposedList<TransformConstraintData>();
		internal ExposedList<PathConstraintData> pathConstraints = new ExposedList<PathConstraintData>();

		public ExposedList<Skin> Skins
		{
			[global::OpenPCR.Migration("8562569d-f9fe-4d94-806c-ee3109835389")]
			get { return skins; }
			set { skins = value; }
		}

		public Skin DefaultSkin
		{
			get
			{
				return defaultSkin;
			}

			set
			{
				defaultSkin = value;
			}
		}

		[global::OpenPCR.Migration("8d435731-a421-4120-a9ab-d717e8ba24ca")]
		public SkeletonData()
		{
		}

		[global::OpenPCR.Migration("c16e33b1-5f6f-4774-b0cd-78a001992896")]
		public Animation FindAnimation(string animationName)
		{
			if (animationName == null)
			{
				throw new ArgumentNullException("animationName cannot be null.");
			}

			if (animations.ContainsKey(animationName))
			{
				return animations[animationName];
			}

			return null;
		}

		[global::OpenPCR.Migration("37033cf2-a466-4c3a-a0f4-4603a92da731")]
		public bool IsAnimation(string animationName)
		{
			if (animationName == null)
			{
				throw new ArgumentNullException("animationName cannot be null.");
			}

			return animations.ContainsKey(animationName);
		}
	}
}
