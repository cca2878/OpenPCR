namespace Elements
{
	[global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1, Size = 8)]
	public struct VoiceGroupAndGroupUnit
	{
		public int GroupId { readonly get; private set; }
		public int GroupUnitId { readonly get; private set; }

		public VoiceGroupAndGroupUnit(int _groupId, int _groupUnitId)
		{
			GroupId = _groupId;
			GroupUnitId = _groupUnitId;
		}
	}
}
