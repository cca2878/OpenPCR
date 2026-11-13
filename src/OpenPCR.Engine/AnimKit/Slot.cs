using global::OpenPCR.AnimKit;
using System;
namespace OpenPCR.AnimKit
{
	public class Slot
	{
		internal SlotData data;
		internal Bone bone;
		internal float r;
		internal float g;
		internal float b;
		internal float a;
		internal float r2;
		internal float g2;
		internal float b2;
		internal Attachment attachment;
		internal float attachmentTime;
		internal ExposedList<float> attachmentVertices = new ExposedList<float>();

		[global::OpenPCR.Migration("5696a00a-a3b1-4f5f-810f-7c91ba2a9a34")]
		public Attachment Attachment
		{
			[global::OpenPCR.Migration("5a75ca17-195d-4b7f-ac01-c1af84202ef1")]
			get
			{
				return attachment;
			}

			[global::OpenPCR.Migration("5dd6d8f7-80f6-464b-a7d7-09de82c9883d")]
			set
			{
				if (attachment != value)
				{
					attachment = value;
					attachmentTime = bone.skeleton.time;
					attachmentVertices.Clear(clearArray: false);
				}
			}
		}

		[global::OpenPCR.Migration("4de6411c-1958-47a4-844a-3fc3876f18f5")]
		public Slot(SlotData data, Bone bone)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data", "data cannot be null.");
			}

			if (bone == null)
			{
				throw new ArgumentNullException("bone", "bone cannot be null.");
			}

			this.data = data;
			this.bone = bone;
			SetToSetupPose();
		}

		[global::OpenPCR.Migration("a8e82084-3dae-4b9f-8c92-e63785eb8fc4")]
		public void SetToSetupPose()
		{
			r = data.r;
			g = data.g;
			b = data.b;
			a = data.a;
			if (data.attachmentName == null)
			{
				Attachment = null;
				return;
			}

			attachment = null;
			Attachment = bone.skeleton.GetAttachment(data.index, data.attachmentName);
		}

		[global::OpenPCR.Migration("5adfaaee-c443-4409-813a-003b9ca6752c")]
		public float AttachmentTime
		{
			[global::OpenPCR.Migration("46371835-315e-446b-92ba-666d51d092a1")]
			get { return bone.skeleton.time - attachmentTime; }
			[global::OpenPCR.Migration("aca1922f-a567-4338-b2dd-4082abe324cf")]
			set { attachmentTime = bone.skeleton.time - value; }
		}
	}
}
