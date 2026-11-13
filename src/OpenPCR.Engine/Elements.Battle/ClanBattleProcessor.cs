using global::Elements;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using Elements.Data;

namespace Elements.Battle
{
	public class ClanBattleProcessor : BaseBattleProcessor
	{
		protected IBattleManagerForClanBattleProcessor battleManager;
		[global::OpenPCR.Migration("3608062a-0dee-4586-8ce6-0b266d1b72ab")]
		public ClanBattleProcessor(IBattleManagerForClanBattleProcessor _battleManager) : base(_battleManager, eBattleCategory.CLAN_BATTLE)
		{
			battleManager = _battleManager;
		}

		[global::OpenPCR.Migration("213ee36a-44d6-4b04-ade1-e9b4bffd7a73")]
		public override bool IsDisplayBossHpUi()
		{ return global::OpenPCR.Presentation.Noop<bool>(); }

		[global::OpenPCR.Migration("5ce79bce-9389-42ad-a22e-c5bd0ccd6a46")]
		public override void GetResultAnimeId(BattleAnimKitController _spine, out eAnimKitCharacterAnimeId _animeId, out eAnimKitCharacterAnimeId _loopAnimeId)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.ClanBattleProcessor::GetResultAnimeId(Elements.BattleAnimKitController,out Elements.eAnimKitCharacterAnimeId,out Elements.eAnimKitCharacterAnimeId):System.Void: runtime capability is unavailable."); }

		[global::OpenPCR.Migration("11fe9034-1719-47a6-95fc-c7264ae7f716")]
		public override UnitParameter GetUnitParameter(int _unitId)
		{
			if (Singleton<ClanBattleTempData>.Instance.SupportUnitId == _unitId)
				return ClanBattleUtil.GetSupportUnitParam();
			return userData.UnitParameterDictionary[_unitId];
		}

		[global::OpenPCR.Migration("e0af5eff-ade4-4944-acb6-f25aec0429d3")]
		public override long GetMainPlayerViewerId(long _defaultViewerId, int _unitId)
		{
			long result = _defaultViewerId;
			if (_unitId == Singleton<ClanBattleTempData>.Instance.SupportUnitId)
				result = Singleton<ClanBattleTempData>.Instance.SupportOwnerId;
			return result;
		}

		[global::OpenPCR.Migration("cf18fd1b-9a33-4adf-9ea5-d3e97f25b335")]
		public override void BattleRetire(ref eViewId _returnViewId)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.ClanBattleProcessor::BattleRetire(ref Elements.eViewId):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("c47ac029-286d-4395-a8a7-480b316c597c")]
		public override void SetupHpAndEnergy()
		{
			BattleHeaderController header = SingletonMonoBehaviour<BattleHeaderController>.Instance;
			int count = battleManager.GetEnemyCtrlLength();
			bool rehearsal = Singleton<ClanBattleTempData>.Instance.IsRehearsal;
			for (int i = 0; i < count; i++)
			{
				UnitCtrl unit = battleManager.GetEnemyCtrl(i);
				if (unit.IsBonusEnemy)
					continue;
				unit.ComponentParameter.MaxHp = (tempData.BattleStageData.CurrentClanBattleBossInfo.MaxHp);
				long hp = (tempData.BattleStageData.CurrentClanBattleBossInfo.CurrentHp);
				if (rehearsal && hp == 0L)
					hp = unit.ComponentParameter.MaxHp;
				unit.ComponentParameter.SetCurrentHp(hp);
				unit.ComponentDamageControl.SetBossGaugeValue();
			}

			header.BossTotalDamage.Init(0L, rehearsal, Singleton<ClanBattleTempData>.Instance.IsTrainingBattle, false);
		}

		[global::OpenPCR.Migration("e7b990eb-a654-4e07-8c24-e10d5d07ac07")]
		public override void PrepareBattle()
		{
			setupEnemyDamageInfo();
		}

		[global::OpenPCR.Migration("a4992b09-5b34-4483-ac76-20eb8463c1b8")]
		public override void LoseProcess()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.ClanBattleProcessor::LoseProcess():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("9ac9dd23-1466-48eb-99b3-156fa3265d18")]
		public override void FinishBattle()
		{
			ClanBattleTempData clan = Singleton<ClanBattleTempData>.Instance;
			bool isInvalid = false;
			int battleId = clan.BattleId;
			int lapNum = clan.BattleLapNum;
			int phase = clan.BattlePhase;
			int clanId = Singleton<UserData>.Instance.UserClan.ClanId;
			int orderNum = clan.BossOrderNum;
			int count = battleManager.GetEnemyCtrlLength();
			long bossHp = 0L;
			long bossMaxHp = 0L;
			for (int i = 0; i < count; i++)
			{
				UnitCtrl unit = battleManager.GetEnemyCtrl(i);
				if (unit.IsSummonOrPhantom || unit.IsBonusEnemy)
					continue;
				bossHp = unchecked(bossHp + unit.ComponentParameter.Hp);
				bossMaxHp = unchecked(bossMaxHp + unit.ComponentParameter.MaxHp);
			}

			long bossDamage = tempData.EnemyDamageInfoList[0].damage;
			ClanBattleFinishUnit userUnit = createClanBattleFinishUnit();
			long startHp = (tempData.BattleStageData.CurrentClanBattleBossInfo.CurrentHp);
			if (clan.IsRehearsal && startHp == 0L)
				startHp = bossMaxHp;
			clan.ClanBattleBossRealDamage = unchecked(startHp - bossHp);
			int remainTime = battleManager.GetMiliLimitTime();
			var finishTimeline = new UnitUnionBurstTimeline();
			finishTimeline.SetUnitId(0);
			finishTimeline.SetRemainTime(battleManager.GetRoundUpTimeLimit());
			finishTimeline.SetIsBattleFinish(1);
			tempData.UnitUnionBurstTimelineList.Add(finishTimeline);
			ApiManager api = ManagerSingleton<ApiManager>.Instance;
			if (clan.IsTrainingBattle)
			{
				api.AddClanBattleTrainingFinishPostParam(clanId, clan.BattleTrainingId, clan.BattleClanBattleMode,
					phase, orderNum, userUnit, bossHp, bossDamage, remainTime, clan.ClanBattleBossRealDamage,
					tempData.BattleStageData.BattleLogId, (int)battleManager.GetAutoClearType(),
					tempData.UnitUnionBurstTimelineList, battleManager.GetBattleElapsedTime(),
					battleManager.GetStartRemainTime(), param => clan.BattleTrainingFinishReceiveParam = param);
			}
			else if (clan.IsRehearsal)
			{
				api.AddClanBattleRehearsalFinishPostParam(clanId, battleId, lapNum, phase, orderNum,
					userUnit, bossHp, bossDamage, remainTime, clan.ClanBattleBossRealDamage,
					tempData.BattleStageData.BattleLogId, Convert.ToInt32(clan.IsActualBossStatus),
					tempData.UnitUnionBurstTimelineList, battleManager.GetBattleElapsedTime(),
					battleManager.GetStartRemainTime(), (int)battleManager.GetAutoClearType(), clan.SelectedStockIndex,
					param => clan.BattleRehearsalFinishReceiveParam = param);
			}
			else
			{
				api.AddClanBattleFinishPostParam(clanId, battleId, lapNum, phase, orderNum,
					userUnit, bossHp, bossDamage, remainTime, clan.ClanBattleBossRealDamage,
					tempData.BattleStageData.BattleLogId, battleManager.BattleLog.GetBattleLogListCsv(),
					(int)battleManager.GetAutoClearType(), tempData.UnitUnionBurstTimelineList,
					battleManager.GetBattleElapsedTime(), battleManager.GetStartRemainTime(), param =>
					{
						clan.BattleFinishReceiveParam = param;
						isInvalid = param.Dead.Equals((global::System.Int32)1);
					});
			}

			battleManager.ResultApiSendExec(() =>
			{
				if (isInvalid)
				{
					// Native confirmation gate schedules ResetData and ChangeView only after the user confirms.
					throw new global::OpenPCR.OutOfBoundaryException("Clan invalid-finish submit dialog and confirmation callback backend");
				}
				userData.SetClanBattleRestrictionExtraEquip();
				battleManager.RevivalAtFinishBattle();
			}, ClanBattleUtil.OnErrorClanBattleApiRequest);
		}

		[global::OpenPCR.Migration("ed15f892-6d3e-4aa4-a1b5-96b9b24bd5c9")]
		private ClanBattleFinishUnit createClanBattleFinishUnit()
		{
			long viewerId = Singleton<UserData>.Instance.UserInfo.ViewerId;
			var result = new ClanBattleFinishUnit();
			var hpList = new List<UnitHpInfo>();
			int count = battleManager.GetUnitCtrlLength();
			for (int i = 0; i < count; i++)
			{
				UnitCtrl unit = battleManager.GetUnitCtrl(i);
				if (unit.IsSummonOrPhantom)
					continue;
				var hp = new UnitHpInfo();
				hp.SetUnitId(unit.OriginalUnitId);
				hp.SetViewerId(viewerId);
				hp.SetHp(unit.ComponentParameter.Hp);
				hpList.Add(hp);
				if (Singleton<ClanBattleTempData>.Instance == null)
					throw new NullReferenceException();
			}

			result.SetUnitHpList(hpList.ToArray());
			result.SetUnitDamageList(tempData.UnitDamageInfoList.ToArray());
			return result;
		}

		[global::OpenPCR.Migration("c43f0421-eb75-4282-a5ec-0a83d3a48914")]
		public override void Release()
		{
			base.Release();
			battleManager = null;
		}

		[global::OpenPCR.Migration("6d9c66df-dba9-41db-8db4-8990009f0f78")]
		public override int GetSystemId()
		{
			return 107;
		}

		[global::OpenPCR.Migration("cf17dbb8-1bad-4fbc-8d4c-c7686af64c8f")]
		public override long GetTalentBonusPermyriad(int _talentId, int _count)
		{
			UnitParameter first = tempData.BattleStageData.CurrentBattleWaves.First().First();
			return QuestUtility.SearchTalentFormationBonus(battleCategory, first.UniqueData.Id, 0, _talentId).GetBonusValue(_count);
		}

		[global::OpenPCR.Migration("6bcc06b5-b9ac-4d4b-beb2-26af843a97f5")]
		public override List<UnitParameter> GetGenerateExtraEffectEnemy(int _waveIndex)
		{
			UnitParameter first = tempData.BattleStageData.CurrentBattleWaves[_waveIndex].First();
			MasterExtraEffectData.ExtraEffectData effect = ManagerSingleton<MasterDataManager>.Instance.masterExtraEffectData.SearchExtraEffectList(battleCategory, first.UniqueData.Id, 0);
			return CreateExtraEffectEnemy(effect);
		}

		[global::OpenPCR.Migration("e6cd84bc-d663-4c7d-a4c7-728fc2a258d5")]
		public override List<BattleManager.BonusEnemyData> GetExtraEffectEnemyData(BattleDefine.eExtraEffectExecTiming _timing)
		{
			UnitCtrl boss = battleManager.GetBossUnit();
			MasterExtraEffectData.ExtraEffectData effect = ManagerSingleton<MasterDataManager>.Instance.masterExtraEffectData.SearchExtraEffectList(battleCategory, boss.OriginalUnitId, 0);
			return CreateBonusEnemyDatas(effect, _timing);
		}

		[global::OpenPCR.Migration("1ea71b19-1922-4838-a462-194938d0c19f")]
		public override bool IsShowEffectiveIcon(int _unitId)
		{ return global::OpenPCR.Presentation.Noop<bool>(); }
	}
}
