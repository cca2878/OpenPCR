using System.Runtime.CompilerServices;

namespace Elements
{
	public class SkinData
	{
		public global::System.Int32 IconSkinId { get; private set; }
		public global::System.Int32 SdSkinId { get; private set; }
		public global::System.Int32 StillSkinId { get; private set; }
		public global::System.Int32 MotionId { get; private set; }

		public SkinData()
		{
			initializeSkinData();
		}

		private void initializeSkinData()
		{
			IconSkinId = 0;
			SdSkinId = 0;
			StillSkinId = 0;
			MotionId = 0;
		}
	}
}
