using global::OpenPCR.AnimKit;
using System;
using System.Collections.Generic;
namespace OpenPCR.AnimKit
{
	public class Skeleton
	{
		internal SkeletonData data;
		internal ExposedList<Bone> bones;
		internal ExposedList<Slot> slots;
		internal ExposedList<Slot> drawOrder;
		internal ExposedList<IkConstraint> ikConstraints;
		internal ExposedList<TransformConstraint> transformConstraints;
		internal ExposedList<PathConstraint> pathConstraints;
		internal ExposedList<IUpdatable> updateCache = new ExposedList<IUpdatable>();
		internal ExposedList<Bone> updateCacheReset = new ExposedList<Bone>();
		internal Skin skin;
		internal float r = 1f;
		internal float g = 1f;
		internal float b = 1f;
		internal float a = 1f;
		internal float time;
		internal bool flipX;
		internal bool flipY;
		internal float x;
		internal float y;

		[global::OpenPCR.ImplicitMigration("83eac386-029d-411c-8cb7-f6b36491c7db")]
		public SkeletonData Data => data;
		[global::OpenPCR.ImplicitMigration("b8c262d2-bdc3-49eb-bd5f-bb1912311bdf")]
		public ExposedList<Slot> Slots => slots;

		[global::OpenPCR.Migration("6aeaad11-572b-4609-bf84-57cf8d034fde")]
		public int FindSlotIndex(string slotName)
		{
			if (slotName == null)
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			ExposedList<Slot> list = slots;
			Slot[] items = list.Items;
			for (int i = 0, count = list.Count; i < count; i++)
				if (items[i].data.name.Equals(slotName))
					return i;
			return -1;
		}

		[global::OpenPCR.Migration("fcc15aa0-a432-48e8-9be8-12a166636639")]
		public Skeleton(SkeletonData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}
			this.data = data;
			bones = new ExposedList<Bone>(data.bones.Count);
			foreach (BoneData bone3 in data.bones)
			{
				Bone item;
				if (bone3.parent == null)
				{
					item = new Bone(bone3, this, null);
				}
				else
				{
					Bone bone = bones.Items[bone3.parent.index];
					item = new Bone(bone3, this, bone);
					bone.children.Add(item);
				}
				bones.Add(item);
			}
			slots = new ExposedList<Slot>(data.slots.Count);
			drawOrder = new ExposedList<Slot>(data.slots.Count);
			foreach (SlotData slot in data.slots)
			{
				Bone bone2 = bones.Items[slot.boneData.index];
				Slot item2 = new Slot(slot, bone2);
				slots.Add(item2);
				drawOrder.Add(item2);
			}
			ikConstraints = new ExposedList<IkConstraint>(data.ikConstraints.Count);
			foreach (IkConstraintData ikConstraint in data.ikConstraints)
			{
				ikConstraints.Add(new IkConstraint(ikConstraint, this));
			}
			transformConstraints = new ExposedList<TransformConstraint>(data.transformConstraints.Count);
			foreach (TransformConstraintData transformConstraint in data.transformConstraints)
			{
				transformConstraints.Add(new TransformConstraint(transformConstraint, this));
			}
			pathConstraints = new ExposedList<PathConstraint>(data.pathConstraints.Count);
			foreach (PathConstraintData pathConstraint in data.pathConstraints)
			{
				pathConstraints.Add(new PathConstraint(pathConstraint, this));
			}
			UpdateCache();
			UpdateWorldTransform();
		}

		[global::OpenPCR.Migration("106fd036-c514-49f1-bc94-86f08f0088b7")]
		public void UpdateWorldTransform()
		{
			ExposedList<Bone> exposedList = updateCacheReset;
			Bone[] items = exposedList.Items;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Bone obj = items[i];
				obj.ax = obj.x;
				obj.ay = obj.y;
				obj.arotation = obj.rotation;
				obj.ascaleX = obj.scaleX;
				obj.ascaleY = obj.scaleY;
				obj.ashearX = obj.shearX;
				obj.ashearY = obj.shearY;
				obj.appliedValid = true;
			}

			IUpdatable[] items2 = updateCache.Items;
			int j = 0;
			for (int count2 = updateCache.Count; j < count2; j++)
			{
				items2[j].Update();
			}
		}

		[global::OpenPCR.Migration("c77a6834-52c5-4c6f-8200-ae2a1280987b")]
		public Bone FindBone(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName", "boneName cannot be null.");
			}

			ExposedList<Bone> exposedList = bones;
			Bone[] items = exposedList.Items;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Bone bone = items[i];
				if (bone.data.name == boneName)
				{
					return bone;
				}
			}

			return null;
		}

		[global::OpenPCR.Migration("1af75550-d3b0-4653-91bf-d76c491990a5")]
		public Attachment GetAttachment(int slotIndex, string attachmentName)
		{
			if (attachmentName == null)
			{
				throw new ArgumentNullException("attachmentName", "attachmentName cannot be null.");
			}

			if (skin != null)
			{
				Attachment attachment = skin.GetAttachment(slotIndex, attachmentName);
				if (attachment != null)
				{
					return attachment;
				}
			}

			if (data.defaultSkin == null)
			{
				return null;
			}

			return data.defaultSkin.GetAttachment(slotIndex, attachmentName);
		}

		[global::OpenPCR.Migration("512e64c6-9e20-44ea-8974-1c75222b4128")]
		public void Update(float delta)
		{
			time += delta;
		}

		[global::OpenPCR.Migration("3aeea015-4c9e-41e6-acb3-387ebcef4652")]
		public void UpdateCache()
		{
			updateCache.Clear();
			updateCacheReset.Clear();
			ExposedList<Bone> exposedList = bones;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				exposedList.Items[i].sorted = false;
			}
			ExposedList<IkConstraint> exposedList2 = ikConstraints;
			ExposedList<TransformConstraint> exposedList3 = transformConstraints;
			ExposedList<PathConstraint> exposedList4 = pathConstraints;
			int count2 = ikConstraints.Count;
			int count3 = exposedList3.Count;
			int count4 = exposedList4.Count;
			int num = count2 + count3 + count4;
			for (int j = 0; j < num; j++)
			{
				int num2 = 0;
				while (true)
				{
					if (num2 < count2)
					{
						IkConstraint ikConstraint = exposedList2.Items[num2];
						if (ikConstraint.data.order == j)
						{
							SortIkConstraint(ikConstraint);
							break;
						}
						num2++;
						continue;
					}
					int num3 = 0;
					while (true)
					{
						if (num3 < count3)
						{
							TransformConstraint transformConstraint = exposedList3.Items[num3];
							if (transformConstraint.data.order == j)
							{
								SortTransformConstraint(transformConstraint);
								break;
							}
							num3++;
							continue;
						}
						for (int k = 0; k < count4; k++)
						{
							PathConstraint pathConstraint = exposedList4.Items[k];
							if (pathConstraint.data.order == j)
							{
								SortPathConstraint(pathConstraint);
								break;
							}
						}
						break;
					}
					break;
				}
			}
			int l = 0;
			for (int count5 = exposedList.Count; l < count5; l++)
			{
				SortBone(exposedList.Items[l]);
			}
		}

		[global::OpenPCR.Migration("bbf2f298-91d5-4d73-a6f1-c5e1fa39860c")]
		private void SortIkConstraint(IkConstraint constraint)
		{
			Bone target = constraint.target;
			SortBone(target);
			ExposedList<Bone> exposedList = constraint.bones;
			Bone bone = exposedList.Items[0];
			SortBone(bone);
			if (exposedList.Count > 1)
			{
				Bone item = exposedList.Items[exposedList.Count - 1];
				if (!updateCache.Contains(item))
				{
					updateCacheReset.Add(item);
				}
			}
			updateCache.Add(constraint);
			SortReset(bone.children);
			exposedList.Items[exposedList.Count - 1].sorted = true;
		}

		[global::OpenPCR.Migration("f8f73e5e-55d1-44e3-b0a3-21aa49e074bc")]
		private void SortPathConstraint(PathConstraint constraint)
		{
			Slot target = constraint.target;
			int index = target.data.index;
			Bone bone = target.bone;
			if (skin != null)
			{
				SortPathConstraintAttachment(skin, index, bone);
			}
			if (data.defaultSkin != null && data.defaultSkin != skin)
			{
				SortPathConstraintAttachment(data.defaultSkin, index, bone);
			}
			int i = 0;
			for (int count = data.skins.Count; i < count; i++)
			{
				SortPathConstraintAttachment(data.skins.Items[i], index, bone);
			}
			Attachment attachment = target.attachment;
			if (attachment is PathAttachment)
			{
				SortPathConstraintAttachment(attachment, bone);
			}
			ExposedList<Bone> exposedList = constraint.bones;
			int count2 = exposedList.Count;
			for (int j = 0; j < count2; j++)
			{
				SortBone(exposedList.Items[j]);
			}
			updateCache.Add(constraint);
			for (int k = 0; k < count2; k++)
			{
				SortReset(exposedList.Items[k].children);
			}
			for (int l = 0; l < count2; l++)
			{
				exposedList.Items[l].sorted = true;
			}
		}

		[global::OpenPCR.Migration("2c0fc3fe-f929-4ee9-a027-2f68e95bef05")]
		private void SortTransformConstraint(TransformConstraint constraint)
		{
			SortBone(constraint.target);
			ExposedList<Bone> exposedList = constraint.bones;
			int count = exposedList.Count;
			if (constraint.data.local)
			{
				for (int i = 0; i < count; i++)
				{
					Bone bone = exposedList.Items[i];
					SortBone(bone.parent);
					if (!updateCache.Contains(bone))
					{
						updateCacheReset.Add(bone);
					}
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					SortBone(exposedList.Items[j]);
				}
			}
			updateCache.Add(constraint);
			for (int k = 0; k < count; k++)
			{
				SortReset(exposedList.Items[k].children);
			}
			for (int l = 0; l < count; l++)
			{
				exposedList.Items[l].sorted = true;
			}
		}

		[global::OpenPCR.Migration("b6f13f75-0a35-49f6-aa90-0db432eaefc8")]
		private void SortPathConstraintAttachment(Skin skin, int slotIndex, Bone slotBone)
		{
			foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> attachment in skin.Attachments)
			{
				if (attachment.Key.slotIndex == slotIndex)
				{
					SortPathConstraintAttachment(attachment.Value, slotBone);
				}
			}
		}

		[global::OpenPCR.Migration("1042fb71-1afa-42e1-8772-c332b0ec6d3b")]
		private void SortPathConstraintAttachment(Attachment attachment, Bone slotBone)
		{
			if (!(attachment is PathAttachment))
			{
				return;
			}
			int[] array = ((PathAttachment)attachment).bones;
			if (array == null)
			{
				SortBone(slotBone);
				return;
			}
			ExposedList<Bone> exposedList = bones;
			int num = 0;
			int num2 = array.Length;
			while (num < num2)
			{
				int num3 = array[num++];
				num3 += num;
				while (num < num3)
				{
					SortBone(exposedList.Items[array[num++]]);
				}
			}
		}

		[global::OpenPCR.Migration("629f2fb2-bd7b-48eb-ba8f-81ebad5f6ae7")]
		private void SortBone(Bone bone)
		{
			if (!bone.sorted)
			{
				Bone parent = bone.parent;
				if (parent != null)
				{
					SortBone(parent);
				}
				bone.sorted = true;
				updateCache.Add(bone);
			}
		}

		[global::OpenPCR.Migration("9899450c-7c0f-43ea-90ff-274e5ad9bc8a")]
		private static void SortReset(ExposedList<Bone> bones)
		{
			Bone[] items = bones.Items;
			int i = 0;
			for (int count = bones.Count; i < count; i++)
			{
				Bone bone = items[i];
				if (bone.sorted)
				{
					SortReset(bone.children);
				}
				bone.sorted = false;
			}
		}

		[global::OpenPCR.Migration("c716bc9d-2f09-47dd-9957-fa9c89105e37")]
		public Slot FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName", "slotName cannot be null.");
			}
			ExposedList<Slot> exposedList = slots;
			Slot[] items = exposedList.Items;
			int i = 0;
			for (int count = exposedList.Count; i < count; i++)
			{
				Slot slot = items[i];
				if (slot.data.name == slotName)
				{
					return slot;
				}
			}
			return null;
		}
	}
}
