using System.Runtime.CompilerServices;

namespace Elements
{
	public class KaiserBattleIniSetting
	{
		public global::System.Int32 SupportLvBand { get; private set; }
		public global::System.Int32 SupportLimit { get; private set; }
		public global::System.Single AllowableDamageCoefficient { get; private set; }
		public global::System.Int32 AllowableEnemyPoint { get; private set; }
		public global::System.Int32 AllowableBarrierPoint { get; private set; }
		public global::System.Int32 RemainingCountMax { get; private set; }

		public KaiserBattleIniSetting()
		{
			initializeKaiserBattleIniSetting();
		}

		private void initializeKaiserBattleIniSetting()
		{
			SupportLvBand = 0;
			SupportLimit = 0;
			AllowableDamageCoefficient = 0f;
			AllowableEnemyPoint = 0;
			AllowableBarrierPoint = 0;
			RemainingCountMax = 0;
		}
	}
}
