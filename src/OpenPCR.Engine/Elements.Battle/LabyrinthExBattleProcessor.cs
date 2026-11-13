using System.Collections.Generic;
using Elements.Data;
using Elements.Utility;

namespace Elements.Battle
{
	public class LabyrinthExBattleProcessor : BaseBattleProcessor
	{
		private IBattleManagerForLabyrinthExBattleProcessor battleManager;
		private LabyrinthTempData labyrinthTempData;
		[global::OpenPCR.Migration("95506af1-05eb-4bf6-8582-6d5f027e6e4a")]
		public LabyrinthExBattleProcessor(IBattleManagerForLabyrinthExBattleProcessor _battleManager, LabyrinthTempData _labyrinthTempData)
			: base(_battleManager, eBattleCategory.LABYRINTH_EX)
		{ battleManager = _battleManager; labyrinthTempData = _labyrinthTempData; }
		[global::OpenPCR.Migration("4ed4e834-cd81-4c48-806c-479aa34d6a88")]
		protected override eResourceId getWinTextResourceId() => eResourceId.FLATOUT_COMBINEANIMATION_TOWER_EX_BATTLEWIN;
		[global::OpenPCR.Migration("7f4f6029-e61e-43b2-ac9a-cf2564703992")]
		public override void DrawTheCurtains()
		{ throw new global::OpenPCR.OutOfBoundaryException("EX result navigation and starting another party belong to the caller of a completed single battle."); }
		[global::OpenPCR.Migration("0dabc6e7-8753-42c1-ad39-5a6f27379996")]
		public override void BattleRetire(ref eViewId _returnViewId)
		{ throw new global::OpenPCR.OutOfBoundaryException("EX retirement navigation is outside a completed battle invocation."); }
		[global::OpenPCR.Migration("02d371ad-ff72-44dd-a3fe-f75a0b460ff3")]
		public override ContinuousUnit IdentifyContinuous(List<ContinuousUnit> _continuousUnitList, UnitData _unitData)
			=> _continuousUnitList.Find(value => value.UnitId == (int)_unitData.Id);
		[global::OpenPCR.Migration("fbe9bc89-5e35-44a0-807b-6bc94e79c470")]
		public override bool CheckContinuousUnitDead(List<ContinuousUnit> _continuousUnitList, UnitData _uniqueData)
		{
			var unit = _continuousUnitList.Find(value => value.UnitId == (int)_uniqueData.Id);
			return unit != null && unit.Hp == 0;
		}
		[global::OpenPCR.Migration("1abf39f0-cd29-4258-980d-cf859a23dafa")]
		public override void WinProcess() => battleManager.OpenLabyrinthExResultDialog(true, labyrinthTempData.CurrentExPartyIndex);
		[global::OpenPCR.Migration("95e703b8-0175-4ba1-a09a-97b1eda36964")]
		public override void LoseProcess()
		{
			if (isFinalWave()) battleManager.OpenLabyrinthExResultDialog(false, labyrinthTempData.CurrentExPartyIndex);
			battleManager.SetupLosePlayVoiceUnitId();
			battleManager.StartResult();
		}
		[global::OpenPCR.Migration("5936a320-9d4c-41e6-b910-e729852900dc")]
		public override void DisplayUnitFront(bool _guestWithIphoneX) { ComplementZoom(); battleManager.DisplayAliveUnitFront(); }
		[global::OpenPCR.Migration("f3d532ed-11d5-4486-aa7a-2e7a0861b1b0")]
		private bool isFinalWave()
		{
			int index = labyrinthTempData.CurrentExPartyIndex;
			return battleManager.BattleResult == eBattleResult.WIN || index == 2 || userData.IsEmptyParty((ePartyType)(index + 133));
		}
		[global::OpenPCR.Migration("b32f48a6-244b-48b9-b4c4-a432f33abb96")]
		public override void FinishBattle()
		{
			tempData.UnitDamageHistoryList.Add(new List<UnitDamageInfo>(tempData.UnitDamageInfoList));
			tempData.EnemyDamageHistoryList.Add(new List<UnitDamageInfo>(tempData.EnemyDamageInfoList));
			tempData.EnemyColorHistoryList.Add(new List<int>(tempData.EnemyColorIdList));
			labyrinthTempData.ExWaveResultInfoList.Add(TowerUtility.CreateWaveResultInfo(labyrinthTempData.CurrentExPartyIndex, battleManager.UnitList, tempData.UnitDamageInfoList, battleManager.GetMiliLimitTime()));
			labyrinthTempData.ExWaveClearTypeList.Add(battleManager.GetAutoClearType());
			bool final = isFinalWave();
			if (final)
			{
				var enemies = createEnemyQueryUnitList();
				enemies.AddRange(labyrinthTempData.AlreadyDeadExEnemyList);
				int autoClear = (int)TowerUtility.SearchExtraClearType(labyrinthTempData.ExWaveClearTypeList);
				ManagerSingleton<ApiManager>.Instance.AddLabyrinthBossBattleFinishPostParam(labyrinthTempData.EnterId, labyrinthTempData.BlockId,
					tempData.SelectedQuestId, labyrinthTempData.ExWaveResultInfoList, enemies, autoClear, null);
				battleManager.ResultApiSendExec(battleManager.RevivalAtFinishBattle, null);
			}
			else
			{
				battleManager.ResultApiSendExec(battleManager.RevivalAtFinishBattle, null);
				LabyrinthUtility.UpdateExEnemy(battleManager.EnemyList, labyrinthTempData.ExEnemyUnitList);
				registerAlreadyDeadEnemy();
				tempData.BattleStageData.CurrentContinuousUnit = new List<List<ContinuousUnit>> { UnitUtility.ConvertTowerUnitListToContinuous(labyrinthTempData.ExEnemyUnitList) };
			}
			saveMyUnitParamData(battleManager.UnitList);
			global::OpenPCR.PreparedBattleResultOutput.CompleteLabyrinthEx(ManagerSingleton<ApiManager>.Instance, final);
		}
		[global::OpenPCR.Migration("ca059d1f-61ab-4c64-a2ac-2183dd9bbcee")]
		public void registerAlreadyDeadEnemy()
		{
			var enemies = battleManager.EnemyList;
			for (int i = 0; i < enemies.Count; i++)
			{
				UnitCtrl unit = enemies[i];
				if (unit.IsSummonOrPhantom || unit.IsBonusEnemy) continue;
				var parameter = unit.ComponentParameter;
				if (parameter.Hp > 0) continue;
				double energy = parameter.Energy;
				var counters = new List<SkillLimitCounter>();
				foreach (var counter in unit.SkillUseCount)
					if (counter.Value != 0) counters.Add(new SkillLimitCounter(counter.Key, counter.Value));
				int id = unit.UnitId;
				var damage = tempData.EnemyDamageInfoList.Find(value => value.unit_id == unit.OriginalUnitId);
				var parts = new PartsInfo[0];
				labyrinthTempData.AlreadyDeadExEnemyList.Add(new DungeonQueryUnit(0, id, 1, 0, BattleUtil.DoubleToInt(energy), counters.ToArray(), damage.damage, parts));
			}
		}
		[global::OpenPCR.Migration("d2543656-36b9-4fb4-b42c-44c99be433c3")]
		private void saveMyUnitParamData(List<UnitCtrl> _unitList)
		{
			var result = new Dictionary<int, LabyrinthUnitResultInfo>();
			for (int i = 0; i < _unitList.Count; i++)
			{
				UnitCtrl unit = _unitList[i];
				if (unit.IsSummonOrPhantom) continue;
				var parameter = unit.ComponentParameter;
				result.Add(unit.OriginalUnitId, new LabyrinthUnitResultInfo(parameter.Hp, parameter.MaxHp, parameter.Energy));
			}
			labyrinthTempData.UnitResultInfoDictionaryList.Add(result);
		}
		[global::OpenPCR.Migration("893bd9fd-39b7-40f9-b38c-e0d82bb003e6")]
		private List<DungeonQueryUnit> createEnemyQueryUnitList()
		{
			var enemies = battleManager.EnemyList;
			var result = new List<DungeonQueryUnit>();
			for (int i = 0; i < enemies.Count; i++)
			{
				UnitCtrl unit = enemies[i];
				if (unit.IsSummonOrPhantom || unit.IsBonusEnemy) continue;
				var counters = new List<SkillLimitCounter>();
				foreach (var counter in unit.SkillUseCount)
					if (counter.Value != 0) counters.Add(new SkillLimitCounter(counter.Key, counter.Value));
				var parameter = unit.ComponentParameter;
				int id = unit.OriginalUnitId;
				long hp = parameter.Hp;
				double energy = parameter.Energy;
				long restored = BattleUtil.DoubleToLong((double)parameter.StartMaxHP / (double)parameter.MaxHp * (double)hp);
				if (restored < 1 && hp > 0) restored = 1;
				var damage = tempData.EnemyDamageInfoList.Find(value => value.unit_id == unit.OriginalUnitId);
				var parts = new PartsInfo[0];
				result.Add(new DungeonQueryUnit(0, id, hp < 1 ? 1 : 0, restored, BattleUtil.DoubleToInt(energy), counters.ToArray(), damage.damage, parts));
			}
			return result;
		}
		[global::OpenPCR.Migration("cdf528ab-7466-4adb-a892-a5220d54069f")]
		public override void Release() { battleManager = null; labyrinthTempData = null; base.Release(); }
		[global::OpenPCR.Migration("46cd9ac4-6c46-4659-8227-30e8a089c229")]
		public override int GetSystemId() => 136;
		[global::OpenPCR.Migration("c245eb7b-a2a6-47e0-9aac-4a18d5993ba8")]
		public override bool IsDisplayBossHpUi() => true;
		[global::OpenPCR.Migration("7fe86409-4751-4f54-af5b-4a280e964f10")]
		public override void SetupHpAndEnergyAfterApplyPassiveSkill()
		{
			base.SetupHpAndEnergyAfterApplyPassiveSkill();
			var boss = battleManager.GetBossUnit();
			if (boss != null && boss.IsBoss) boss.ComponentDamageControl.SetBossGaugeValue();
		}
		[global::OpenPCR.Migration("39d58ed5-ec40-4b5b-9b2e-1514923339fd")]
		public override long GetTalentBonusPermyriad(int _talentId, int _count)
			=> QuestUtility.SearchTalentFormationBonus(battleCategory, tempData.SelectedQuestId, 0, _talentId).GetBonusValue(_count);
		[global::OpenPCR.Migration("1e4330dd-444f-43e6-a7f2-c30371fb037a")]
		public override List<UnitParameter> GetGenerateExtraEffectEnemy(int _waveIndex)
		{
			var effect = ManagerSingleton<MasterDataManager>.Instance.masterExtraEffectData.SearchExtraEffectList(eBattleCategory.LABYRINTH_EX, tempData.SelectedQuestId, 0);
			var result = CreateExtraEffectEnemy(effect);
			result.AddRange(LabyrinthUtility.CreateDifficultyEnemeyParameter(labyrinthTempData.Difficulty));
			return result;
		}
		[global::OpenPCR.Migration("28ec74fe-d2fc-44d2-af7c-f84e1bd18de4")]
		public override List<BattleManager.BonusEnemyData> GetExtraEffectEnemyData(BattleDefine.eExtraEffectExecTiming _timing)
		{
			var effect = ManagerSingleton<MasterDataManager>.Instance.masterExtraEffectData.SearchExtraEffectList(eBattleCategory.LABYRINTH_EX, tempData.SelectedQuestId, 0);
			var result = CreateBonusEnemyDatas(effect, _timing);
			result.AddRange(LabyrinthUtility.CreateDifficultyBonusEnemyData(labyrinthTempData.Difficulty, _timing, battleManager.EnemyList));
			return result;
		}
		[global::OpenPCR.Migration("e8e8c346-0f68-413f-b22f-4e76cec1a15b")]
		public override bool IsShowEffectiveIcon(int _unitId) => false;
		[global::OpenPCR.Migration("3c030573-3c68-417e-b441-425f6e851c2b")]
		public override UnitParameter GetUnitParameter(int _unitId) => labyrinthTempData.UnitParameterDictionary[_unitId];
	}
}
