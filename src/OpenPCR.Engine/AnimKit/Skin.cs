using global::OpenPCR.AnimKit;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenPCR.AnimKit
{
	public class Skin
	{
		[StructLayout((LayoutKind)0, Pack = 1, Size = 24)]
		public struct AttachmentKeyTuple
		{
			public readonly int slotIndex;
			public readonly string name;
			internal readonly int nameHashCode;
			[global::OpenPCR.Migration("bbc70416-d120-4478-bd39-65d45f8f65a4")]
			public AttachmentKeyTuple(int slotIndex, string name)
			{
				this.slotIndex = slotIndex;
				this.name = name;
				nameHashCode = name.GetHashCode();
			}
		}

		internal string name;
		private Dictionary<AttachmentKeyTuple, Attachment> attachments = new Dictionary<AttachmentKeyTuple, Attachment>(AttachmentKeyTupleComparer.Instance);


		private class AttachmentKeyTupleComparer : IEqualityComparer<AttachmentKeyTuple>
		{
			internal static readonly AttachmentKeyTupleComparer Instance = new AttachmentKeyTupleComparer();
			bool IEqualityComparer<AttachmentKeyTuple>.Equals(AttachmentKeyTuple o1, AttachmentKeyTuple o2)
			{
				return o1.slotIndex == o2.slotIndex && o1.nameHashCode == o2.nameHashCode && o1.name == o2.name;
			}
			int IEqualityComparer<AttachmentKeyTuple>.GetHashCode(AttachmentKeyTuple o) { return o.slotIndex; }
		}
		public Dictionary<AttachmentKeyTuple, Attachment> Attachments => attachments;
		[global::OpenPCR.Migration("6e210da8-35cb-4ac5-afb6-bad2f1d11021")]
		public Skin(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name", "name cannot be null.");
			}

			this.name = name;
		}

		[global::OpenPCR.Migration("7f3b555c-cbd6-492f-80ca-4c0151d46b4b")]
		public void AddAttachment(int slotIndex, string name, Attachment attachment)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment", "attachment cannot be null.");
			}

			attachments[new AttachmentKeyTuple(slotIndex, name)] = attachment;
		}

		public Dictionary<AttachmentKeyTuple, Attachment> CopyAttachment()
		{
			Dictionary<AttachmentKeyTuple, Attachment> dictionary = new Dictionary<AttachmentKeyTuple, Attachment>();
			foreach (KeyValuePair<AttachmentKeyTuple, Attachment> attachment in attachments)
			{
				dictionary.Add(attachment.Key, attachment.Value);
			}

			return dictionary;
		}

		[global::OpenPCR.Migration("612ec5b1-b5c0-4e4b-89e9-77b96e5640a8")]
		public void FindNamesForSlot(int slotIndex, List<string> names)
		{
			if (names == null)
				throw new ArgumentNullException("names", "names cannot be null.");
			foreach (AttachmentKeyTuple key in attachments.Keys)
				if (key.slotIndex == slotIndex)
					names.Add(key.name);
		}

		[global::OpenPCR.Migration("3e6dccb6-bd89-447d-85f0-ddd63958cf09")]
		public Attachment GetAttachment(int slotIndex, string name)
		{
			attachments.TryGetValue(new AttachmentKeyTuple(slotIndex, name), out var value);
			return value;
		}
	}
}
