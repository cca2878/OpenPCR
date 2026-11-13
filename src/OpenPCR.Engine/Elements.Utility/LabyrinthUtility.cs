using System.Collections.Generic;
using Elements.Data;
namespace Elements.Utility
{
	public static class LabyrinthUtility
	{
		[global::OpenPCR.Migration("04ada9a8-e8d0-40c2-a755-6a0b243873b8")]
		public static void UpdateExEnemy(List<UnitCtrl> _enemyList, List<TowerUnit> _targetEnemyInfoList)
		{
			for (int i = 0; i < _targetEnemyInfoList.Count; i++)
			{
				UnitCtrl unit = _enemyList.Find(value => value.UnitId == _targetEnemyInfoList[i].UnitId);
				if (unit == null || unit.IsSummonOrPhantom) continue;
				var parameter = unit.ComponentParameter;
				long hp = parameter.Hp;
				long restored = BattleUtil.DoubleToLong((double)parameter.StartMaxHP / (double)parameter.MaxHp * (double)parameter.Hp);
				if (restored < 1 && hp > 0) restored = 1;
				_targetEnemyInfoList[i].SetHp(restored);
				_targetEnemyInfoList[i].SetEnergy(0);
			}
		}

		[global::OpenPCR.Migration("538fc0c0-e8ae-4cfb-a12c-f2b1304ef4ba")]
		public static List<UnitParameter> CreateDifficultyEnemeyParameter(int _difficulty)
		{
			var inputs = global::OpenPCR.PreparedLabyrinthInputs.Require(Elements.ManagerSingleton<MasterDataManager>.Instance);
			if (inputs.Difficulty != _difficulty) throw new global::System.InvalidOperationException("Prepared difficulty differs.");
			var result = new List<UnitParameter>();
			foreach (int id in inputs.DifficultyEnemies) result.Add(UnitUtility.CreateUnitParameterFromEnemyParameter(id));
			return result;
		}
		[global::OpenPCR.Migration("01820aec-bfb4-45be-a47e-857f8671f31d")]
		public static List<Elements.Battle.BattleManager.BonusEnemyData> CreateDifficultyBonusEnemyData(int _difficulty, BattleDefine.eExtraEffectExecTiming _timing, List<UnitCtrl> _enemyList)
		{
			var inputs = global::OpenPCR.PreparedLabyrinthInputs.Require(Elements.ManagerSingleton<MasterDataManager>.Instance);
			if (inputs.Difficulty != _difficulty) throw new global::System.InvalidOperationException("Prepared difficulty differs.");
			var result = new List<Elements.Battle.BattleManager.BonusEnemyData>();
			for (int i = 0; i < inputs.DifficultyEnemies.Count; i++)
			{
				if (inputs.DifficultyTimings[i] != (int)_timing) continue;
				int id = inputs.DifficultyEnemies[i];
				var unit = _enemyList.Find(value => value.UnitId == id);
				var skills = new List<(int, int)>();
				foreach (int skill in unit.MainSkillIdList) if (skill != 0) skills.Add((skill, 0));
				result.Add(new Elements.Battle.BattleManager.BonusEnemyData { Id = id, SkillIdAndBonusIdList = skills });
			}
			return result;
		}
		public static Dictionary<eParamType, int> CalcLabyrinthEnhancedBuffParam(List<int> _enhancedNodeIds)
		{ throw new OpenPCR.OutOfBoundaryException("LabyrinthUtility.CalcLabyrinthEnhancedBuffParam Master reinforced node growth production"); }
		public static Dictionary<eParamType, int> CalcLabyrinthRelicPassiveParam(LabyrinthTempData _labyrinthTempData)
		{ throw new OpenPCR.OutOfBoundaryException("LabyrinthUtility.CalcLabyrinthRelicPassiveParam Master relic growth production"); }
	}
}
