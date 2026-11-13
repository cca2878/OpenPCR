using System.Runtime.CompilerServices;

namespace Elements
{
	public class UnitHpInfoForFriendBattle
	{

		private void initializeUnitHpInfoForFriendBattle() { viewer_id = 0L; unit_id = 0; hp = 0L; hp_rate = 0; }


		public long viewer_id { get; private set; }

		public int unit_id { get; private set; }

		public long hp { get; private set; }

		public int hp_rate { get; private set; }


		public UnitHpInfoForFriendBattle()
		{ initializeUnitHpInfoForFriendBattle(); }
	}
}
