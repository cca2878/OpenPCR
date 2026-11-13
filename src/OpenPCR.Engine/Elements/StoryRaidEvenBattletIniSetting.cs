using System.Runtime.CompilerServices;

namespace Elements
{
	public class StoryRaidEvenBattletIniSetting
	{
		public global::System.Int32 SupportLimit { get; private set; }
		public global::System.Int32 SupportChangeInterval { get; private set; }
		public global::System.Int32 RemainingCountMax { get; private set; }
		public global::System.Int32 LimitUnitLevel { get; private set; }

		public StoryRaidEvenBattletIniSetting()
		{
			initializeStoryRaidEvenBattletIniSetting();
		}

		private void initializeStoryRaidEvenBattletIniSetting()
		{
			SupportLimit = 0;
			SupportChangeInterval = 0;
			RemainingCountMax = 0;
			LimitUnitLevel = 10;
		}
	}
}
