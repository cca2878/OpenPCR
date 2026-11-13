using System.Runtime.CompilerServices;

namespace Elements
{
	public class ClanBattleFinishUnit
	{
		public UnitDamageInfo[] unit_damage_list { get; private set; }
		public UnitHpInfo[] unit_hp_list { get; private set; }

		[global::OpenPCR.Migration("69430a61-610e-4700-9d5c-223066f550c1")]
		public ClanBattleFinishUnit()
		{
			initializeClanBattleFinishUnit();
		}

		[global::OpenPCR.Migration("aa4915c4-07b9-4bd1-b7f4-cdce3d187462")]
		public void SetUnitDamageList(UnitDamageInfo[] _unitDamageList)
		{
			unit_damage_list = _unitDamageList;
		}

		[global::OpenPCR.Migration("8c27800a-1978-49e2-963e-e9797f0da043")]
		public void SetUnitHpList(UnitHpInfo[] _unitHpList)
		{
			unit_hp_list = _unitHpList;
		}

		[global::OpenPCR.Migration("92d58efb-b456-42a1-9ff2-615c75dc7ac0")]
		private void initializeClanBattleFinishUnit()
		{
			unit_damage_list = null;
			unit_hp_list = null;
		}
	}
}
