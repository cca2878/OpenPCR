using System.Collections.Generic;
using Elements.Data;

namespace Elements.Battle
{
	public class LabyrinthBattleProcessor : BaseBattleProcessor
	{
		private IBattleManagerForLabyrinthBattleProcessor battleManager;
		private LabyrinthTempData labyrinthTempData;
		[global::OpenPCR.Migration("82b315d6-7ecf-43e2-a8e8-b93244bf6bba")]
		public LabyrinthBattleProcessor(IBattleManagerForLabyrinthBattleProcessor _battleManager, LabyrinthTempData _labyrinthTempData)
			: base(_battleManager, eBattleCategory.LABYRINTH)
		{ battleManager = _battleManager; labyrinthTempData = _labyrinthTempData; }
		[global::OpenPCR.Migration("92541d7b-f389-402e-bc50-02b1fac94956")]
		protected override eResourceId getWinTextResourceId() => eResourceId.FLATOUT_COMBINEANIMATION_TOWER_BATTLEWIN;
		[global::OpenPCR.Migration("9223fda4-e834-4ded-a831-efb7889ec99e")]
		public override void DisplayUnitFront(bool _guestWithIphoneX) { ComplementZoom(); battleManager.DisplayAliveUnitFront(); }
		[global::OpenPCR.Migration("9e5ad30c-fc59-4431-8c6f-7a6ceef3762c")]
		public override void BattleRetire(ref eViewId _returnViewId)
		{ throw new global::OpenPCR.OutOfBoundaryException("Labyrinth retirement navigation is outside a completed battle invocation."); }
		[global::OpenPCR.Migration("424ed245-752e-499c-8a27-724ce45349b0")]
		public override void FinishBattle()
		{
			var hp = battleManager.CreateUnitHpInfoList(0L, Singleton<UserData>.Instance.UserInfo.ViewerId);
			int autoClear = (int)battleManager.GetAutoClearType();
			ManagerSingleton<ApiManager>.Instance.AddLabyrinthBattleFinishPostParam(labyrinthTempData.EnterId, labyrinthTempData.BlockId,
				tempData.SelectedQuestId, battleManager.GetMiliLimitTime(), hp, autoClear, null);
			battleManager.ResultApiSendExec(battleManager.RevivalAtFinishBattle, null);
		}
		[global::OpenPCR.Migration("7ba3100e-5185-4136-97d0-980ff56c85a9")]
		public override void Release() { battleManager = null; labyrinthTempData = null; base.Release(); }
		[global::OpenPCR.Migration("6ff8d0bd-8e67-4fb8-b4c9-c04c604c5e5d")]
		public override int GetSystemId() => 136;
		[global::OpenPCR.Migration("a19df19f-3649-41cf-ad8c-f286f778abe2")]
		public override float GetWaitTimeWhenFinishBattle() => battleManager.BattleResult == eBattleResult.WIN ? 1f : 0f;
		[global::OpenPCR.Migration("c738793b-cfd7-410a-9e07-834331f35cf1")]
		public override void SetupTreasureBoxEffect(UnitCtrl _unitCtrl) { }
		[global::OpenPCR.Migration("4a69cc5a-d92e-4669-bb3e-ad63694398b3")]
		public override long GetTalentBonusPermyriad(int _talentId, int _count)
			=> QuestUtility.SearchTalentFormationBonus(battleCategory, tempData.SelectedQuestId, 0, _talentId).GetBonusValue(_count);
		[global::OpenPCR.Migration("6268216d-9d5b-41fa-ab32-fa01186b79c5")]
		public override List<UnitParameter> GetGenerateExtraEffectEnemy(int _waveIndex)
		{
			var effect = ManagerSingleton<MasterDataManager>.Instance.masterExtraEffectData.SearchExtraEffectList(eBattleCategory.LABYRINTH, tempData.SelectedQuestId, 0);
			var result = CreateExtraEffectEnemy(effect);
			result.AddRange(Elements.Utility.LabyrinthUtility.CreateDifficultyEnemeyParameter(labyrinthTempData.Difficulty));
			return result;
		}
		[global::OpenPCR.Migration("f11a4dc9-302b-455a-af80-0e7da0515c40")]
		public override List<BattleManager.BonusEnemyData> GetExtraEffectEnemyData(BattleDefine.eExtraEffectExecTiming _timing)
		{
			var effect = ManagerSingleton<MasterDataManager>.Instance.masterExtraEffectData.SearchExtraEffectList(eBattleCategory.LABYRINTH, tempData.SelectedQuestId, 0);
			var result = CreateBonusEnemyDatas(effect, _timing);
			result.AddRange(Elements.Utility.LabyrinthUtility.CreateDifficultyBonusEnemyData(labyrinthTempData.Difficulty, _timing, battleManager.EnemyList));
			return result;
		}
		[global::OpenPCR.Migration("908709e4-da81-48de-a77f-0ddc68484f73")]
		public override void SetupHpAndEnergyAfterApplyPassiveSkill()
		{
			var boss = battleManager.GetBossUnit();
			if (boss != null && boss.IsBoss) boss.ComponentDamageControl.SetBossGaugeValue();
			base.SetupHpAndEnergyAfterApplyPassiveSkill();
		}
		[global::OpenPCR.Migration("8c02fdc1-20bf-4ac9-8a21-ac6b178987a6")]
		public override bool IsShowEffectiveIcon(int _unitId) => false;
		[global::OpenPCR.Migration("4d89d3ba-4a51-4250-afe5-47dc9e906e9e")]
		public override UnitParameter GetUnitParameter(int _unitId) => labyrinthTempData.UnitParameterDictionary[_unitId];
		[global::OpenPCR.Migration("29bbd16d-f12a-4c34-a54d-44857adc4d40")]
		public override bool IsDisplayBossHpUi() => true;
	}
}
