namespace Elements
{
	public class TrialBattleFinishUnit
	{
		public UnitDamageInfo[] unit_damage_list { get; private set; }
		public UnitHpInfo[] unit_hp_list { get; private set; }
		[global::OpenPCR.Migration("67cc676c-25d1-4c78-9677-aef90fa08fa0")]
		public TrialBattleFinishUnit() { initializeTrialBattleFinishUnit(); }
		[global::OpenPCR.Migration("401061ce-e900-441e-92a2-110072182e19")]
		public void SetUnitDamageList(UnitDamageInfo[] _unitDamageList) { unit_damage_list = _unitDamageList; }
		[global::OpenPCR.Migration("f0c45c03-c04f-4b64-b68a-15c189bdd571")]
		public void SetUnitHpList(UnitHpInfo[] _unitHpList) { unit_hp_list = _unitHpList; }
		[global::OpenPCR.Migration("384898e5-9dee-4c18-bce7-0cec870ae03b")]
		private void initializeTrialBattleFinishUnit()
		{
			unit_damage_list = null;
			unit_hp_list = null;
		}
	}
}
