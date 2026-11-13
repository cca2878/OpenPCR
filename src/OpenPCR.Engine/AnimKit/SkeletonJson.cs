using global::OpenPCR.AnimKit;
using System;
using System.Collections.Generic;

namespace OpenPCR.AnimKit
{
	public class SkeletonJson
	{
		private AttachmentLoader attachmentLoader;

		private List<LinkedMesh> linkedMeshes = new List<LinkedMesh>();

		public float Scale { get; set; }

		[global::OpenPCR.Migration("ba9f9993-6e16-4881-8ae2-b477aba376da")]
		public SkeletonJson(AttachmentLoader attachmentLoader)
		{
			if (attachmentLoader == null)
			{
				throw new ArgumentNullException("attachmentLoader", "attachmentLoader cannot be null.");
			}
			this.attachmentLoader = attachmentLoader;
			Scale = 1f;
		}

		internal class LinkedMesh
		{
			internal string parent;

			internal string skin;

			internal int slotIndex;

			internal MeshAttachment mesh;

			[global::OpenPCR.Migration("ae465cd8-d3d8-49a4-bd3b-bf65514dce1b")]
			public LinkedMesh(MeshAttachment mesh, string skin, int slotIndex, string parent)
			{
				this.mesh = mesh;
				this.skin = skin;
				this.slotIndex = slotIndex;
				this.parent = parent;
			}
		}

	}
}
