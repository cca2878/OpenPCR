using global::OpenPCR.AnimKit;
using global::OpenPCR.AnimKit.Scene;
using global::OpenPCR.Runtime.Api;
using System.Runtime.CompilerServices;

namespace OpenPCR.AnimKit.Scene
{

	public class SkeletonAnimation : SkeletonRenderer, ISkeletonAnimation, IAnimationStateComponent
	{
		public AnimationState state;
		[SerializeField]
		private string _animationName;
		public bool loop;
		public float timeScale = 1f;
#if OPENPCR_OPTIMIZED
		[global::OpenPCR.Optimization("a6bde2f5-fab6-40cc-89e7-9f60f66eaa7f")]
		private bool pausedStable;
		[global::OpenPCR.Optimization("9cae91bb-5a92-40a0-b08b-de00e6f508b1")]
		private System.Collections.Generic.List<int> pausedValues;
		[global::OpenPCR.Optimization("40cfd93b-93d6-478a-9b2b-711e72b0034f")]
		private System.Collections.Generic.List<object> pausedRefs;
		[global::OpenPCR.Optimization("8c74d0fd-22dc-4ffa-a75c-7d409fcd8217")]
		private System.Collections.Generic.List<int> scratchValues;
		[global::OpenPCR.Optimization("0ea39723-d2de-4fa4-a450-2c859385ae8f")]
		private System.Collections.Generic.List<object> scratchRefs;
		[global::OpenPCR.Optimization("0ece230f-29f9-4574-b684-8d5fd7011b92")]
		private System.Collections.Generic.List<int> beforeValues;
		[global::OpenPCR.Optimization("45e39a8d-0afa-4d02-9f69-d9989200b8de")]
		private System.Collections.Generic.List<object> beforeRefs;
#endif
		public AnimationState AnimationState
		{
			get
			{ throw new global::OpenPCR.MigrationPendingException("OpenPCR.AnimKit.Scene.SkeletonAnimation::get_AnimationState():OpenPCR.AnimKit.AnimationState", "Behavior implementation is not available."); }
		}

		[global::OpenPCR.Migration("53922b4e-4286-49e1-99de-7e7402595090")]
		public string AnimationName
		{
			[global::OpenPCR.Migration("73adfe5b-d419-46fd-8a1f-ffff9c52fe5d")]
			get
			{
				if (!valid)
				{
					return null;
				}

				return state.GetCurrent(0)?.Animation.Name;
			}

			[global::OpenPCR.Migration("3d1cf173-43d0-4a6e-ad53-7dff3cf87a0c")]
			set
			{
				_animationName = value;
				if (valid)
				{
					if (string.IsNullOrEmpty(value))
					{
						state.ClearTrack(0);
						return;
					}

					state.SetAnimation(0, value, loop);
					Update();
				}
			}
		}

		public event UpdateBonesDelegate UpdateLocal
;

		public event UpdateBonesDelegate UpdateWorld
;

		public event UpdateBonesDelegate UpdateComplete
;

		[global::OpenPCR.Migration("cb5fb4a4-ba00-4a69-a434-2052d71504b4")]
		public SkeletonAnimation()
		{
		}

		[global::OpenPCR.Migration("ac9b1192-9542-42d6-af9f-b43f186a11a5")]
		public virtual void Update()
		{
			Update(Time.deltaTime);
		}

		[global::OpenPCR.Migration("5f4549c4-58e7-4b0d-8588-b30f3a0c7985")]
		[global::OpenPCR.Optimization("5037589a-a148-4a56-ba2e-1b7673919821")]
		public void Update(float deltaTime)
		{
#if OPENPCR_OPTIMIZED
			if (valid)
			{
				deltaTime *= timeScale;
				if (state.TimeScale == 0f && UpdateLocal == null && UpdateWorld == null && UpdateComplete == null)
				{
					scratchValues ??= new System.Collections.Generic.List<int>(); scratchRefs ??= new System.Collections.Generic.List<object>();
					if (pausedStable)
					{
						scratchValues.Clear(); scratchRefs.Clear();
						state.AppendPausedSignature(scratchValues, scratchRefs, false);
						if (MatchesPausedSignature(scratchValues, scratchRefs))
						{
							skeleton.Update(deltaTime);
							return;
						}
						pausedStable = false;
					}
					beforeValues ??= new System.Collections.Generic.List<int>(); beforeRefs ??= new System.Collections.Generic.List<object>();
					beforeValues.Clear(); beforeRefs.Clear();
					AppendPausedSignature(beforeValues, beforeRefs, true);
					skeleton.Update(deltaTime);
					state.Update(deltaTime);
					state.Apply(skeleton);
					skeleton.UpdateWorldTransform();
					scratchValues.Clear(); scratchRefs.Clear();
					AppendPausedSignature(scratchValues, scratchRefs, true);
					if (SameSignature(scratchValues, scratchRefs, beforeValues, beforeRefs))
					{
						pausedValues ??= new System.Collections.Generic.List<int>(); pausedRefs ??= new System.Collections.Generic.List<object>();
						pausedValues.Clear(); pausedRefs.Clear();
						AppendPausedSignature(pausedValues, pausedRefs, false);
						pausedStable = true;
					}
					return;
				}
				pausedStable = false;
				skeleton.Update(deltaTime);
				state.Update(deltaTime);
				state.Apply(skeleton);
				if (UpdateLocal != null)
				{
					UpdateLocal(this);
				}

				skeleton.UpdateWorldTransform();
				if (UpdateWorld != null)
				{
					UpdateWorld(this);
					skeleton.UpdateWorldTransform();
				}

				if (UpdateComplete != null)
				{
					UpdateComplete(this);
				}
			}
#else
			if (valid)
			{
				deltaTime *= timeScale;
				skeleton.Update(deltaTime);
				state.Update(deltaTime);
				state.Apply(skeleton);
				if (UpdateLocal != null)
				{
					UpdateLocal(this);
				}

				skeleton.UpdateWorldTransform();
				if (UpdateWorld != null)
				{
					UpdateWorld(this);
					skeleton.UpdateWorldTransform();
				}

				if (UpdateComplete != null)
				{
					UpdateComplete(this);
				}
			}
#endif
		}

#if OPENPCR_OPTIMIZED
		[global::OpenPCR.Optimization("6549bd10-54c6-4542-a726-724f9ab44470")]
		private void AppendPausedSignature(System.Collections.Generic.List<int> values, System.Collections.Generic.List<object> refs, bool deep)
		{
			state.AppendPausedSignature(values, refs, deep);
			Skeleton s = skeleton;
			refs.Add(s.skin);
			values.Add(System.BitConverter.SingleToInt32Bits(s.r)); values.Add(System.BitConverter.SingleToInt32Bits(s.g));
			values.Add(System.BitConverter.SingleToInt32Bits(s.b)); values.Add(System.BitConverter.SingleToInt32Bits(s.a));
			values.Add(s.flipX ? 1 : 0); values.Add(s.flipY ? 1 : 0);
			values.Add(System.BitConverter.SingleToInt32Bits(s.x)); values.Add(System.BitConverter.SingleToInt32Bits(s.y));
			Slot[] slots = s.slots.Items;
			values.Add(s.slots.Count);
			for (int i = 0, n = s.slots.Count; i < n; i++)
			{
				Slot slot = slots[i];
				refs.Add(slot.attachment);
				values.Add(System.BitConverter.SingleToInt32Bits(slot.r)); values.Add(System.BitConverter.SingleToInt32Bits(slot.g));
				values.Add(System.BitConverter.SingleToInt32Bits(slot.b)); values.Add(System.BitConverter.SingleToInt32Bits(slot.a));
				values.Add(slot.attachmentVertices.Count);
				if (deep)
				{
					values.Add(System.BitConverter.SingleToInt32Bits(slot.attachmentTime));
					for (int j = 0, m = slot.attachmentVertices.Count; j < m; j++) values.Add(System.BitConverter.SingleToInt32Bits(slot.attachmentVertices.Items[j]));
				}
			}
			if (!deep) return;
			Slot[] order = s.drawOrder.Items;
			for (int i = 0, n = s.drawOrder.Count; i < n; i++) refs.Add(order[i]);
			Bone[] bones = s.bones.Items;
			for (int i = 0, n = s.bones.Count; i < n; i++)
			{
				Bone b = bones[i];
				values.Add(System.BitConverter.SingleToInt32Bits(b.x)); values.Add(System.BitConverter.SingleToInt32Bits(b.y));
				values.Add(System.BitConverter.SingleToInt32Bits(b.rotation)); values.Add(System.BitConverter.SingleToInt32Bits(b.scaleX));
				values.Add(System.BitConverter.SingleToInt32Bits(b.scaleY)); values.Add(System.BitConverter.SingleToInt32Bits(b.shearX));
				values.Add(System.BitConverter.SingleToInt32Bits(b.shearY)); values.Add(System.BitConverter.SingleToInt32Bits(b.ax));
				values.Add(System.BitConverter.SingleToInt32Bits(b.ay)); values.Add(System.BitConverter.SingleToInt32Bits(b.arotation));
				values.Add(System.BitConverter.SingleToInt32Bits(b.ascaleX)); values.Add(System.BitConverter.SingleToInt32Bits(b.ascaleY));
				values.Add(System.BitConverter.SingleToInt32Bits(b.ashearX)); values.Add(System.BitConverter.SingleToInt32Bits(b.ashearY));
				values.Add(b.appliedValid ? 1 : 0); values.Add(b.sorted ? 1 : 0);
				values.Add(System.BitConverter.SingleToInt32Bits(b.a)); values.Add(System.BitConverter.SingleToInt32Bits(b.b));
				values.Add(System.BitConverter.SingleToInt32Bits(b.c)); values.Add(System.BitConverter.SingleToInt32Bits(b.d));
				values.Add(System.BitConverter.SingleToInt32Bits(b.worldX)); values.Add(System.BitConverter.SingleToInt32Bits(b.worldY));
			}
			for (int i = 0, n = s.ikConstraints.Count; i < n; i++)
			{
				IkConstraint c = s.ikConstraints.Items[i];
				values.Add(System.BitConverter.SingleToInt32Bits(c.mix)); values.Add(c.bendDirection);
			}
			for (int i = 0, n = s.transformConstraints.Count; i < n; i++)
			{
				TransformConstraint c = s.transformConstraints.Items[i];
				values.Add(System.BitConverter.SingleToInt32Bits(c.rotateMix)); values.Add(System.BitConverter.SingleToInt32Bits(c.translateMix));
				values.Add(System.BitConverter.SingleToInt32Bits(c.scaleMix)); values.Add(System.BitConverter.SingleToInt32Bits(c.shearMix));
			}
			for (int i = 0, n = s.pathConstraints.Count; i < n; i++)
			{
				PathConstraint c = s.pathConstraints.Items[i];
				values.Add(System.BitConverter.SingleToInt32Bits(c.position)); values.Add(System.BitConverter.SingleToInt32Bits(c.spacing));
				values.Add(System.BitConverter.SingleToInt32Bits(c.rotateMix)); values.Add(System.BitConverter.SingleToInt32Bits(c.translateMix));
			}
		}

		[global::OpenPCR.Optimization("c4abc92c-b1b6-471e-897d-794c9def0f3b")]
		private bool MatchesPausedSignature(System.Collections.Generic.List<int> stateValues, System.Collections.Generic.List<object> stateRefs)
		{
			int vi = stateValues.Count, ri = stateRefs.Count;
			if (pausedValues.Count < vi || pausedRefs.Count < ri) return false;
			for (int i = 0; i < vi; i++) if (stateValues[i] != pausedValues[i]) return false;
			for (int i = 0; i < ri; i++) if (!ReferenceEquals(stateRefs[i], pausedRefs[i])) return false;
			Skeleton s = skeleton;
			if (!ReferenceEquals(pausedRefs[ri++], s.skin)) return false;
			if (pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(s.r) || pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(s.g)
				|| pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(s.b) || pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(s.a)
				|| pausedValues[vi++] != (s.flipX ? 1 : 0) || pausedValues[vi++] != (s.flipY ? 1 : 0)
				|| pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(s.x) || pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(s.y)
				|| pausedValues[vi++] != s.slots.Count) return false;
			Slot[] slots = s.slots.Items;
			for (int i = 0, n = s.slots.Count; i < n; i++)
			{
				Slot slot = slots[i];
				if (!ReferenceEquals(pausedRefs[ri++], slot.attachment)
					|| pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(slot.r) || pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(slot.g)
					|| pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(slot.b) || pausedValues[vi++] != System.BitConverter.SingleToInt32Bits(slot.a)
					|| pausedValues[vi++] != slot.attachmentVertices.Count) return false;
			}
			return vi == pausedValues.Count && ri == pausedRefs.Count;
		}

		[global::OpenPCR.Optimization("5d015b9a-2543-46f4-8d73-98f2b4e47be2")]
		private bool SameSignature(System.Collections.Generic.List<int> values, System.Collections.Generic.List<object> refs, System.Collections.Generic.List<int> otherValues, System.Collections.Generic.List<object> otherRefs)
		{
			if (values.Count != otherValues.Count || refs.Count != otherRefs.Count) return false;
			for (int i = 0, n = values.Count; i < n; i++) if (values[i] != otherValues[i]) return false;
			for (int i = 0, n = refs.Count; i < n; i++) if (!ReferenceEquals(refs[i], otherRefs[i])) return false;
			return true;
		}
#endif
	}
}
