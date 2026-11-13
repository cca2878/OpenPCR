using System.Runtime.CompilerServices;

namespace Elements
{
	public class TowerSetting
	{
		public global::System.Int32 InitialEnergy { get; private set; }
		public global::System.Int32 TimeupHpPenalty { get; private set; }
		public global::System.Int32 TimeupEnergyPenalty { get; private set; }
		public global::System.Int32 SupportRentalCostCoefficient { get; private set; }
		public global::System.Int32 ReduceEnemyEnergyValue { get; private set; }
		public global::System.Int32 ReduceEnemyEnergyLowerLimit { get; private set; }
		public global::System.Int32 GetClearedExQuestInterval { get; private set; }
		public global::System.Int32 SupportLvBand { get; private set; }
		public global::System.Int32 UsableUnitCount { get; private set; }

		public TowerSetting()
		{
			initializeTowerSetting();
		}

		private void initializeTowerSetting()
		{
			InitialEnergy = 0;
			TimeupHpPenalty = 0;
			TimeupEnergyPenalty = 0;
			SupportRentalCostCoefficient = 0;
			ReduceEnemyEnergyValue = 0;
			ReduceEnemyEnergyLowerLimit = 0;
			GetClearedExQuestInterval = 0;
			SupportLvBand = 0;
			UsableUnitCount = 0;
		}
	}
}
