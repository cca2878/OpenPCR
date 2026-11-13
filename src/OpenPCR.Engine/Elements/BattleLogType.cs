using System.Runtime.CompilerServices;

namespace Elements
{
	public class BattleLogType
	{
		public global::System.Int32 Miss { get; private set; }
		public global::System.Int64 SetDamage { get; private set; }
		public global::System.Int32 SetAbnormal { get; private set; }
		public global::System.Int32 SetRecovery { get; private set; }
		public global::System.Int32 SetBuffDebuff { get; private set; }
		public global::System.Int32 SetState { get; private set; }
		public global::System.Int32 ButtonTap { get; private set; }
		public global::System.Int32 SetEnergy { get; private set; }
		public global::System.Int64 DamageCharge { get; private set; }
		public global::System.Int32 GiveValueAdditional { get; private set; }
		public global::System.Int32 GiveValueMultiply { get; private set; }
		public global::System.Int32 WaveEndHp { get; private set; }
		public global::System.Int64 WaveEndDamageAmount { get; private set; }
		public global::System.Int32 Break { get; private set; }
		public global::System.Int32 SuspendCount { get; private set; }
		public global::System.Int32 Suspend { get; private set; }

		public BattleLogType()
		{
			initializeBattleLogType();
		}

		private void initializeBattleLogType()
		{
			Miss = 0;
			SetDamage = 0;
			SetAbnormal = 0;
			SetRecovery = 0;
			SetBuffDebuff = 0;
			SetState = 0;
			ButtonTap = 0;
			SetEnergy = 0;
			DamageCharge = 0;
			GiveValueAdditional = 0;
			GiveValueMultiply = 0;
			WaveEndHp = 0;
			WaveEndDamageAmount = 0;
			Break = 0;
			SuspendCount = 0;
			Suspend = 0;
		}
	}
}
