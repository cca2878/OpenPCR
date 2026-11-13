using global::OpenPCR.AnimKit;
namespace OpenPCR.AnimKit
{
	public class ClippingAttachment : VertexAttachment
	{
		internal SlotData endSlot;

		public ClippingAttachment(string name) : base(name)
		{
		}

		public SlotData EndSlot { get => endSlot; set => endSlot = value; }
	}
}
