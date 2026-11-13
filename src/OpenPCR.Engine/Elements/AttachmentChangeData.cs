using global::OpenPCR.AnimKit;
using System;
using System.Runtime.CompilerServices;

namespace Elements
{
	[Serializable]
	public class AttachmentChangeData
	{

		public int TargetIndex { get; set; }

		public global::OpenPCR.AnimKit.Attachment TargetAttachment { get; set; }

		public global::OpenPCR.AnimKit.Attachment AppliedAttachment { get; set; }

		public string TargetAttachmentName;
		public string AppliedAttachmentName;
		public AttachmentChangeData()
		{ throw new global::OpenPCR.MigrationPendingException("Elements.AttachmentChangeData::.ctor()", "Behavior implementation is not available."); }
	}

}
