using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class UnitComponentLog : IUnitComponent
	{
		private UnitCtrl owner { get; set; }
		private Elements.Battle.IBattleLog battleLog { get; set; }


		[global::OpenPCR.Migration("706907d9-c5e8-4e75-8eeb-f31023fd9494")]
		public UnitComponentLog(UnitCtrl _owner, IBattleLog _battleLog)
		{
			owner = _owner;
			battleLog = _battleLog;
		}

		[global::OpenPCR.Migration("0ebc2821-d200-43fa-a992-4352a7a4ebe9")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
		}

		[global::OpenPCR.Migration("2fd5e36d-7f9d-4a40-b5fb-85eae3fd3955")]
		public void AppendEnergyLog(double _energy, eSetEnergyType _type, UnitCtrl _source = null)
		{
			battleLog.AppendBattleLog(eBattleLogType.SET_ENERGY, (int)_type, 0, BattleUtil.DoubleToInt(_energy), 0, 0, 1, 1, _source, owner);
		}
	}
}
