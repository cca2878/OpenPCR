using global::OpenPCR.AnimKit;
using System;
using System.Collections.Generic;

namespace OpenPCR.AnimKit
{
	public class AnimationStateData
	{
		[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
		public struct AnimationPair
		{
			public readonly Animation a1;

			public readonly Animation a2;

			[global::OpenPCR.Migration("221cdbc7-f42d-48f7-ae28-0d1f4408bae7")]
			public AnimationPair(Animation a1, Animation a2)
			{
				this.a1 = a1;
				this.a2 = a2;
			}
		}

		private class AnimationPairComparer : IEqualityComparer<AnimationPair>
		{
			internal static readonly AnimationPairComparer Instance = new AnimationPairComparer();

			bool IEqualityComparer<AnimationPair>.Equals(AnimationPair x, AnimationPair y)
			{
				if (x.a1 == y.a1)
				{
					return x.a2 == y.a2;
				}
				return false;
			}

			int IEqualityComparer<AnimationPair>.GetHashCode(AnimationPair obj)
			{
				int hashCode = obj.a1.GetHashCode();
				return ((hashCode << 5) + hashCode) ^ obj.a2.GetHashCode();
			}
		}

		internal SkeletonData skeletonData;

		private readonly Dictionary<AnimationPair, float> animationToMixTime = new Dictionary<AnimationPair, float>(AnimationPairComparer.Instance);

		internal float defaultMix;

		public AnimationStateData(SkeletonData skeletonData)
		{
			if (skeletonData == null)
			{
				throw new ArgumentException("skeletonData cannot be null.");
			}
			this.skeletonData = skeletonData;
		}

		public void SetMix(string fromName, string toName, float duration)
		{
			Animation animation = skeletonData.FindAnimation(fromName);
			if (animation == null)
			{
				throw new ArgumentException("Animation not found: " + fromName);
			}
			Animation animation2 = skeletonData.FindAnimation(toName);
			if (animation2 == null)
			{
				throw new ArgumentException("Animation not found: " + toName);
			}
			SetMix(animation, animation2, duration);
		}

		public void SetMix(Animation from, Animation to, float duration)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from", "from cannot be null.");
			}
			if (to == null)
			{
				throw new ArgumentNullException("to", "to cannot be null.");
			}
			AnimationPair key = new AnimationPair(from, to);
			animationToMixTime.Remove(key);
			animationToMixTime.Add(key, duration);
		}

		[global::OpenPCR.Migration("5b8ece10-0d1e-40d2-a6de-cd23695ce6ab")]
		public float GetMix(Animation from, Animation to)
		{
			if (from == null)
			{
				throw new ArgumentNullException("from", "from cannot be null.");
			}
			if (to == null)
			{
				throw new ArgumentNullException("to", "to cannot be null.");
			}
			AnimationPair key = new AnimationPair(from, to);
			if (animationToMixTime.TryGetValue(key, out var value))
			{
				return value;
			}
			return defaultMix;
		}
	}
}
