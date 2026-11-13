using global::Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Data;

namespace Elements.Battle
{
	public class TrialBattleProcessor : BaseBattleProcessor, ISingletonField
	{
		protected IBattleManagerForTrialBattleProcessor battleManager;
		private BattleHeaderController battleHeaderController = SingletonMonoBehaviour<BattleHeaderController>.Instance;
		protected TrialBattleTempData trialBattleTemp;
		private TrialBattleDefine.TalentCustomSettingInfo customSettingInfo;
		private ApiManager apiManager = ManagerSingleton<ApiManager>.Instance;

		[global::OpenPCR.Migration("1943b115-6a31-4a84-bc79-5c51acd6fc3d")]
		public TrialBattleProcessor(IBattleManagerForTrialBattleProcessor _battleManager, eBattleCategory _battleCategory)
			: base(_battleManager, _battleCategory)
		{
			battleManager = _battleManager;
			trialBattleTemp = this.CreateSingletonTree().Get<TrialBattleTempData>();
			if (battleCategory == eBattleCategory.TRIAL_BATTLE)
				customSettingInfo = trialBattleTemp.TalentCustomSettingInfoDic[tempData.SelectedQuestId];
		}

		[global::OpenPCR.Migration("fb6e3257-4609-4060-8c7e-0b0491979244")]
		public override bool IsDisplayBossHpUi() { return global::OpenPCR.Presentation.Noop<bool>(); }
		[global::OpenPCR.Migration("1d3080f0-ea6a-4f08-95ec-a9a2540ab187")]
		public override bool IsBossInfinityHpBattle() { return true; }
		[global::OpenPCR.Migration("b207eaef-a902-4a9c-96de-aa47ef162158")]
		public override bool IsBossInfinityHpMode()
		{
			int key = battleCategory == eBattleCategory.TRIAL_CLAN_BATTLE
				? trialBattleTemp.DuringClanBattleModeParam.InfinityHpModeKey : tempData.SelectedQuestId;
			return TrialBattleUtility.IsBossInfinityHpMode(key);
		}
		[global::OpenPCR.Migration("3230c972-1977-423e-b81d-4821ab40e5e2")]
		public override void GetResultAnimeId(BattleAnimKitController _spine, out eAnimKitCharacterAnimeId _animeId, out eAnimKitCharacterAnimeId _loopAnimeId)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.TrialBattleProcessor::GetResultAnimeId(Elements.BattleAnimKitController,out Elements.eAnimKitCharacterAnimeId,out Elements.eAnimKitCharacterAnimeId):System.Void: runtime capability is unavailable."); }
		[global::OpenPCR.Migration("b24a0024-7155-41ca-9770-d84b33e15da3")]
		public override void SetupHpAndEnergy()
		{
			base.SetupHpAndEnergy();
			UnitCtrl boss = battleManager.GetBossUnit();
			boss.ComponentDamageControl.SetBossGaugeValue();
			if (IsBossInfinityHpMode())
			{
				long maxHp = boss.ComponentParameter.MaxHp;
				// Native writes only the rendered infinity-life label; the parameter read and mode query remain ordered.
			}
			battleHeaderController.BossTotalDamage.Init(0L, false, false, IsBossInfinityHpMode());
		}
		[global::OpenPCR.Migration("84203d7c-0fd3-4af5-a1ab-0f8fe36326de")]
		public override void LoseProcess()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.TrialBattleProcessor::LoseProcess():System.Void (outside supported first-phase lifecycle/input scope)"); }
		[global::OpenPCR.Migration("4d78e0b0-cc4f-400b-a8d1-30d8cf4d1f61")]
		public override void FinishBattle()
		{
			var parameter = battleManager.GetBossUnit().ComponentParameter;
			long totalDamage = IsBossInfinityHpMode() ? battleManager.TotalBossDamage
				: unchecked(parameter.MaxHp - parameter.Hp);
			if (battleCategory == eBattleCategory.TRIAL_CLAN_BATTLE)
			{
				apiManager.AddTrialClanBattleFinishPostParam(trialBattleTemp.InitClanBattleSelectDifficultyParam.ClanBattleId,
					trialBattleTemp.DuringClanBattleModeParam.Phase, trialBattleTemp.InitClanBattleSelectDifficultyParam.OrderNum,
					tempData.BattleStageData.BattleLogId, createTrialBattleFinishUnit(), totalDamage, null);
			}
			else if (battleCategory == eBattleCategory.TRIAL_BATTLE)
			{
				throw new global::OpenPCR.OutOfBoundaryException("Ordinary trial quest finish request producer outside clan/trial-clan scope");
			}
			trialBattleTemp.BossRealDamage = totalDamage;
			apiManager.AddTrialBattleTopPostParam(null);
			battleManager.ResultApiSendExec(battleManager.RevivalAtFinishBattle, null);
		}
		[global::OpenPCR.Migration("320a912e-d3fe-4d59-aa74-5bf59206390f")]
		private TrialBattleFinishUnit createTrialBattleFinishUnit()
		{
			long viewerId = Singleton<UserData>.Instance.UserInfo.ViewerId;
			var result = new TrialBattleFinishUnit();
			var hpList = new List<UnitHpInfo>();
			int count = battleManager.GetUnitCtrlLength();
			for (int i = 0; i < count; i++)
			{
				UnitCtrl unit = battleManager.GetUnitCtrl(i);
				if (unit.IsSummonOrPhantom) continue;
				var hp = new UnitHpInfo();
				hp.SetUnitId(unit.OriginalUnitId);
				hp.SetViewerId(viewerId);
				hp.SetHp(unit.ComponentParameter.Hp);
				hpList.Add(hp);
			}
			result.SetUnitHpList(hpList.ToArray());
			result.SetUnitDamageList(tempData.UnitDamageInfoList.ToArray());
			return result;
		}
		[global::OpenPCR.Migration("122dd48a-533b-497d-b583-0f5522f16229")]
		public override void BattleRetire(ref eViewId _returnViewId)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.TrialBattleProcessor::BattleRetire(ref Elements.eViewId):System.Void (outside supported first-phase lifecycle/input scope)"); }
		[global::OpenPCR.Migration("4c42f284-7932-4e15-88ea-177b80a1c131")]
		public override void Release()
		{
			base.Release();
			battleManager = null;
			trialBattleTemp = null;
		}
		[global::OpenPCR.Migration("f174b6c6-7570-46f2-96b1-0565f450dc40")]
		public override int GetSystemId() { return 909; }
		[global::OpenPCR.Migration("9babb99b-458a-46a6-8852-29033ff768ed")]
		public override long GetMainPlayerViewerId(long _defaultViewerId, int _unitId)
		{
			return trialBattleTemp.SupportUnitId == _unitId ? trialBattleTemp.SupportOwnerId : _defaultViewerId;
		}
		[global::OpenPCR.Migration("18a6f310-5cd2-45d2-ab12-c7abf5e178e9")]
		public override long GetTalentBonusPermyriad(int _talentId, int _count)
		{
			if (battleCategory == eBattleCategory.TRIAL_CLAN_BATTLE)
			{
				UnitParameter first = tempData.BattleStageData.CurrentBattleWaves.First().First();
				return QuestUtility.SearchTalentFormationBonus(eBattleCategory.CLAN_BATTLE, first.UniqueData.Id, 0, _talentId).GetBonusValue(_count);
			}
			if (customSettingInfo.TalentFormationBonusState == TrialBattleDefine.eTalentFormationBonusState.OFF) return 0L;
			throw new global::OpenPCR.OutOfBoundaryException("Ordinary trial quest talent formation master producer outside clan/trial-clan scope");
		}
		[global::OpenPCR.Migration("50ad13e8-3527-4d1e-b973-24b28a38e525")]
		public override void SetPartsTalentWeakness(int _unitId, BasePartsData _partsData)
		{
			if (battleCategory == eBattleCategory.TRIAL_CLAN_BATTLE) { base.SetPartsTalentWeakness(_unitId, _partsData); return; }
			throw new global::OpenPCR.OutOfBoundaryException("Ordinary trial quest talent weakness master producer outside clan/trial-clan scope");
		}
		[global::OpenPCR.Migration("06e03e71-c32b-49b2-87c6-2561e9f483e0")]
		public override List<UnitParameter> GetGenerateExtraEffectEnemy(int _waveIndex)
		{
			MasterDataManager mdm = ManagerSingleton<MasterDataManager>.Instance;
			MasterExtraEffectData.ExtraEffectData effect = null;
			if (battleCategory == eBattleCategory.TRIAL_CLAN_BATTLE)
			{
				UnitParameter first = tempData.BattleStageData.CurrentBattleWaves[_waveIndex].First();
				effect = mdm.masterExtraEffectData.SearchExtraEffectList(eBattleCategory.CLAN_BATTLE, first.UniqueData.Id, 0);
			}
			else if (battleCategory == eBattleCategory.TRIAL_BATTLE)
				throw new global::OpenPCR.OutOfBoundaryException("Ordinary trial quest extra effect master producer outside clan/trial-clan scope");
			return CreateExtraEffectEnemy(effect);
		}
		[global::OpenPCR.Migration("79412b8d-135b-4e7c-a198-7ba0348853c2")]
		public override List<BattleManager.BonusEnemyData> GetExtraEffectEnemyData(BattleDefine.eExtraEffectExecTiming _timing)
		{
			MasterDataManager mdm = ManagerSingleton<MasterDataManager>.Instance;
			MasterExtraEffectData.ExtraEffectData effect = null;
			if (battleCategory == eBattleCategory.TRIAL_CLAN_BATTLE)
				effect = mdm.masterExtraEffectData.SearchExtraEffectList(eBattleCategory.CLAN_BATTLE, battleManager.GetBossUnit().OriginalUnitId, 0);
			else if (battleCategory == eBattleCategory.TRIAL_BATTLE)
				throw new global::OpenPCR.OutOfBoundaryException("Ordinary trial quest extra effect master producer outside clan/trial-clan scope");
			return CreateBonusEnemyDatas(effect, _timing);
		}
		[global::OpenPCR.Migration("dfcb459a-542c-4f43-84cb-412523a27f3d")]
		public override bool IsShowEffectiveIcon(int _unitId) { return global::OpenPCR.Presentation.Noop<bool>(); }
	}
}
