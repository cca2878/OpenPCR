using System.Collections.Generic;

namespace Elements
{
	public class ContinuousUnit
	{
		public readonly int UnitId;
		public readonly int IdentifyNum;
		public readonly long Hp;
		public readonly int Energy;
		public readonly List<SkillLimitCounter> SkillLimitCounter;

		[global::OpenPCR.Migration("cc02085f-d481-4842-866e-ea6c37fb8310")]
		public ContinuousUnit(int _unitId, int _unitIndex, long _hp, int _energy, List<SkillLimitCounter> _skillLimitCounter)
		{
			UnitId = _unitId;
			IdentifyNum = _unitIndex;
			Hp = _hp;
			Energy = _energy;
			SkillLimitCounter = _skillLimitCounter;
		}

		[global::OpenPCR.Migration("a40d4ab1-9c08-4149-b840-b52d94332e98")]
		public ContinuousUnit(DungeonUnit _dungeonUnit)
		{
			UnitId = _dungeonUnit.UnitId;
			IdentifyNum = 0;
			Hp = _dungeonUnit.Hp;
			Energy = _dungeonUnit.Energy;
			SkillLimitCounter = _dungeonUnit.SkillLimitCounter;
		}

		[global::OpenPCR.Migration("a9536c07-782a-429c-b6f1-5f013fcda303")]
		public ContinuousUnit(TowerUnit _towerUnit)
		{
			UnitId = _towerUnit.UnitId;
			IdentifyNum = _towerUnit.IdentifyNum;
			Hp = _towerUnit.Hp;
			Energy = _towerUnit.Energy;
			SkillLimitCounter = _towerUnit.SkillLimitCounter;
		}
	}
}
