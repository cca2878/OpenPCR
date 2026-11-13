using global::OpenPCR.AnimKit;
using System;
namespace OpenPCR.AnimKit
{
	public class PointAttachment : Attachment
	{
		internal float x;
		internal float y;
		internal float rotation;

		public PointAttachment(string name) : base(name)
		{
		}
	}
}
