using System.Runtime.InteropServices;
namespace Elements
{
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
	public struct VoiceGroupIdSet
	{
		public int GroupId { readonly get; private set; }
		public int GroupUnitId { readonly get; private set; }
		public int UnitId { readonly get; private set; }
		[global::OpenPCR.Migration("1f86ab35-e84a-49b9-a1ff-39284454195b")]
		public VoiceGroupIdSet(int _groupId, int _groupUnitId, int _unitId)
		{
			GroupId = _groupId;
			GroupUnitId = _groupUnitId;
			UnitId = _unitId;
		}
	}
}
