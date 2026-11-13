using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace Elements.Data
{
	public class LabyrinthTempData
	{
		public List<Dictionary<int, LabyrinthUnitResultInfo>> UnitResultInfoDictionaryList;
		public List<TowerUnit> ExEnemyUnitList;
		public List<DungeonQueryUnit> AlreadyDeadExEnemyList;
		public List<TowerWaveResultInfo> ExWaveResultInfoList;
		public int CurrentExPartyIndex { get; set; }
		public List<eBattleClearType> ExWaveClearTypeList { get; private set; }
		public Dictionary<int, UnitParameter> UnitParameterDictionary { get; set; }
		public int EnterId { get; set; }
		public int BlockId { get; set; }
		public int Difficulty { get; set; }
		public List<int> ReinforcementNodeList { get; set; }
		public LabyrinthTempData()
		{ throw new global::OpenPCR.MigrationPendingException("Elements.Data.LabyrinthTempData::.ctor()", "Behavior implementation is not available."); }
	}

}
