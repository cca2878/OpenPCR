using global::Elements;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;
using Elements.Data;
using global::OpenPCR.Runtime.Api;

namespace Elements.Battle
{
	public abstract class BaseBattleProcessor
	{
		[global::OpenPCR.Migration("880400c1-519b-4f0c-b073-ee2570a972aa")]
		public void ComplementZoom()
		{
			float positionY = getZoomPosY();
			battleManager.StartCoroutine(battleManager.UpdateZoom(1.45f, positionY));
		}
		[global::OpenPCR.Migration("5849b05e-e964-434d-a1f0-31639249c9f4")]
		public virtual float GetResultCameraScale()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::GetResultCameraScale():System.Single (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("c97105f6-7661-4020-a9e0-3be303c55adf")]
		public virtual bool IsInitialMode()
		{
			return true;
		}

		[global::OpenPCR.Migration("7793aa20-dede-48ff-8842-06019ef677b8")]
		public virtual global::Elements.Battle.Core.SpecialBattleData GetSpecialBattleData()
		{
			return null;
		}

		[global::OpenPCR.Migration("7013c360-f0b1-4684-b9be-0abcc071d96e")]
		public virtual bool GetNeedsCommonRehearsalIcon()
		{
			return false;
		}

		[global::OpenPCR.Migration("366d76b8-8869-4241-8a8a-9f7a0bee08a3")]
		public virtual global::System.Collections.Generic.List<Elements.ContinuousUnit> ListContinousUnit()
		{
			return null;
		}

		[global::OpenPCR.Migration("c9a24bdf-64e1-409e-8f0a-180d8c163298")] public abstract long GetTalentBonusPermyriad(int _talentId, int _count);
		protected UserData userData;
		protected TempData tempData;
		protected SoundManager soundManager;
		protected readonly eBattleCategory battleCategory = eBattleCategory.TUTORIAL;
		private IBattleManagerForOnlyBaseBattleProcessor battleManager;
		protected IBattleCameraEffectForBattleProcessor battleCameraEffect;
		protected IBattleTimeScaleForBattleProcessor battleTimeScale;
		[global::OpenPCR.Migration("bb37e845-eda6-4cb7-8e62-8913ce790b6b")]
		public BaseBattleProcessor(IBattleManagerForBaseBattleProcessor _battleManager, eBattleCategory _battleCategory)
		{
			battleManager = (IBattleManagerForOnlyBaseBattleProcessor)_battleManager;
			battleCameraEffect = battleManager.GetBattleCameraEffectForBattleProcessor();
			battleTimeScale = battleManager.GetIBattleTimeScaleForBattleProcessor();
			userData = Singleton<UserData>.Instance;
			tempData = Singleton<TempData>.Instance;
			soundManager = ManagerSingleton<SoundManager>.Instance;
			battleCategory = _battleCategory;
		}

		[global::OpenPCR.Migration("ba120429-0fe9-4d29-af7a-ccc0df09b92a")]
		public virtual void AddUnitSkillUseCount(ref Dictionary<int, int> _skillUseCounts, int _unitId)
		{
		}

		[global::OpenPCR.Migration("0f1736b3-3421-4d64-a30d-1150dafe9cfc")]
		public virtual void BattleRetire(ref eViewId _returnViewId)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::BattleRetire(ref Elements.eViewId):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("b1fb6e07-a7f7-48c8-8091-ebec59a53857")]
		public virtual void DisplayUnitFront(bool _guestWithIphoneX)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::DisplayUnitFront(System.Boolean):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("031aeaa9-7d4e-4480-abda-c5477006e0b2")]
		public virtual void SetupHpAndEnergy()
		{
		}

		[global::OpenPCR.Migration("24d5c1f1-7251-4726-ac95-5c4d046a5af8")]
		public virtual float GetMultiTargetEffectTimer()
		{
			return 0f;
		}

		[global::OpenPCR.Migration("a557fb43-f67a-4f09-b64c-72096ce9d912")]
		public virtual void SetupHpAndEnergyAfterApplyPassiveSkill()
		{
		}

		[global::OpenPCR.Migration("8ec076f0-255f-4f95-a703-a580a3731b72")]
		public virtual void FinishWave()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::FinishWave():System.Void (outside first-phase wave boundary)"); }

		[global::OpenPCR.Migration("574b2a1f-5247-4aad-b515-0fe55cd5ff3b")] public abstract void FinishBattle();

		[global::OpenPCR.Migration("e8154e94-fd13-4987-a7b6-8fa4bf9f3048")]
		public virtual bool IsDisplayBossHpUi()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::IsDisplayBossHpUi():System.Boolean (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("7066cdd3-1633-498a-9e59-47658cd80a29")]
		public virtual bool IsBossInfinityHpBattle()
		{
			return false;
		}

		[global::OpenPCR.Migration("e7e69937-2e1b-4882-ac94-7c21b003fb0d")]
		public virtual bool IsBossInfinityHpMode()
		{
			return false;
		}

		[global::OpenPCR.Migration("1a3288f9-e21f-4083-9eba-007bd83b1eba")]
		public virtual long GetInvalidLifeAmount(long _maxHp)
		{
			return _maxHp;
		}

		[global::OpenPCR.Migration("13c7adf0-0d35-48b3-9b01-8fa220e55738")]
		public virtual int GetSeed()
		{
			return tempData.BattleStageData.Seed;
		}

		[global::OpenPCR.Migration("4404f0fa-edd2-41ef-9b89-193538c9aabe")]
		public virtual float GetActionStartTime()
		{
			return 0f;
		}

		[global::OpenPCR.Migration("00177153-89dc-443f-ae8b-934b5cb14e37")]
		public virtual float GetStoryStartTime()
		{
			return 0f;
		}

		[global::OpenPCR.Migration("e991ea05-c7e7-49fc-aea4-f63f4cfd9437")]
		public virtual int GetFrameRate()
		{
			SaveDataManager save = ManagerSingleton<SaveDataManager>.Instance;
			if (save.IsBattleSpeedQuadruple && save.AutoSpeedSetting.IsSpeedUp && battleManager.CanQuadruple)
				return 30;
			return ManagerSingleton<SaveDataManager>.Instance.BattleFrameRate;
		}

		[global::OpenPCR.Migration("0deb1128-0d1e-4513-9655-6ee3bef8fbaf")]
		public virtual float GetWaitTimeWhenFinishBattle()
		{
			return 0f;
		}

		[global::OpenPCR.Migration("dd697283-e01b-4bb0-9ea4-c6db46fa091e")]
		public virtual long GetMainPlayerViewerId(long _defaultViewerId, int _unitId)
		{
			return userData.UserInfo.ViewerId;
		}

		[global::OpenPCR.Migration("52092225-99a3-4eb0-95e3-71a99e19994a")]
		public virtual void GetResultAnimeId(BattleAnimKitController _spine, out eAnimKitCharacterAnimeId _animeId, out eAnimKitCharacterAnimeId _loopAnimeId)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::GetResultAnimeId(Elements.BattleAnimKitController,out Elements.eAnimKitCharacterAnimeId,out Elements.eAnimKitCharacterAnimeId):System.Void: runtime capability is unavailable."); }

		[global::OpenPCR.Migration("1954a0de-19b9-4d07-af1a-2856b4645bdd")]
		public virtual UnitParameter GetUnitParameter(int _unitId)
		{
			if (tempData.SupportUnitId == _unitId)
				return new UnitParameter(SupportUnitUtility.GetSupportUnit(tempData.SupportOwnerId, _unitId).UnitParamData);
			return userData.UnitParameterDictionary[_unitId];
		}

		[global::OpenPCR.Migration("7592a160-ea21-42bc-afe6-1c982ee3914a")]
		protected virtual long getEnemyUserViewerId()
		{
			if (userData.ArenaParameters == null)
			{
				return 0;
			}

			return userData.ArenaParameters.BattleViewerId;
		}

		[global::OpenPCR.Migration("56ce3b8e-e045-448a-a95e-98b6caab70d8")]
		public virtual bool GetBattleLogEnable(bool _defaultFlag)
		{
			return _defaultFlag;
		}

		[global::OpenPCR.Migration("867abb07-7c83-47e1-a52a-74a7425ea544")]
		public bool IsNecessaryOtherUserInfo()
		{
			bool result = false;
			eBattleCategory eBattleCategory2 = battleCategory;
			if ((uint)(eBattleCategory2 - 2) <= 2u || (uint)(eBattleCategory2 - 7) <= 1u || eBattleCategory2 == eBattleCategory.FRIEND)
			{
				result = true;
			}

			return result;
		}

		[global::OpenPCR.Migration("363ecd40-d234-45f1-92c5-d542384dc765")]
		public virtual bool IsFirstWave()
		{
			return true;
		}

		[global::OpenPCR.Migration("44cd3924-090b-4fe8-878c-d1eaca6940bc")]
		public virtual void NextWaveStartStoryFinishCallback()
		{
		}

		[global::OpenPCR.Migration("cd47ba7a-33aa-448f-95c4-13624130711f")]
		public virtual int GetWaveEndStoryId()
		{
			return 0;
		}

		[global::OpenPCR.Migration("d8ab8393-5843-45ff-b88c-96c0ecc45e31")]
		public virtual bool CheckContinuousUnitDead(List<ContinuousUnit> _continuousUnitList, UnitData _uniqueData)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::CheckContinuousUnitDead(System.Collections.Generic.List\u00601\u003CElements.ContinuousUnit\u003E,Elements.UnitData):System.Boolean (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("2b865efe-a06f-48f3-a1a9-b6642db3d35e")]
		public virtual void UpdateContinuousUnitSkillCounter(List<ContinuousUnit> _continuousUnitList, UnitData _uniqueData, UnitCtrl _unitCtrl)
		{
		}

		[global::OpenPCR.Migration("a2c103f1-d866-424a-abba-645a8062e0cf")]
		protected virtual float getZoomPosY()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::getZoomPosY():System.Single (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("c5174fd6-bc5a-49ab-88a5-21fc29ad34b9")]
		protected virtual eUnitRespawnPos getUnitRespawnPos(int _index, bool _isEnemySide)
		{
			return BattleDefine.GetUnitRespawnPos(_index, _isEnemySide);
		}

		[global::OpenPCR.Migration("7f7ab2c2-8f7a-48f5-b547-8088fb431e17")]
		public virtual float GetDropTreasureBoxDeltaTime()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::GetDropTreasureBoxDeltaTime():System.Single (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("f9de1aac-a1a0-43e1-bdfc-1147d38306e0")]
		public virtual void ModifyUnitCtrl(ref UnitCtrl _unitCtrl, UnitData _unitData)
		{
		}

		[global::OpenPCR.Migration("11092192-65c3-4a24-948d-2c36a32f0947")]
		public virtual void DrawTheCurtains()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::DrawTheCurtains():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("b75c08e4-bd86-4023-9905-cc417f7816fb")]
		public virtual void LoseProcess()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::LoseProcess():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("0f45b037-ff11-4c6a-950e-cf2a16ae5fa5")]
		public virtual void TimeUpWinProcess()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::TimeUpWinProcess():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("7cf6b921-637a-4e75-baa3-199ecaaf67ca")]
		public virtual void WinProcess()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::WinProcess():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("c7a4b47d-1b08-4616-a364-e38d6a2633e4")]
		public virtual void OnDeadLastUnit(UnitCtrl _unit)
		{
		}

		[global::OpenPCR.Migration("e1f7de45-a500-46f6-b4f1-328f91770b8e")]
		public virtual void GetEnemyAtWaveIdx(int _waveIdx, ref List<UnitParameter> _enemyList)
		{
			_enemyList = tempData.BattleStageData.CurrentBattleWaves[_waveIdx];
		}

		[global::OpenPCR.Migration("d0eb311b-75b6-4274-bf08-7db987ccff77")] public abstract List<UnitParameter> GetGenerateExtraEffectEnemy(int _waveIndex);
		[global::OpenPCR.Migration("66178f00-6cfd-4eae-a61a-3729593ce048")]
		protected List<UnitParameter> CreateExtraEffectEnemy(MasterExtraEffectData.ExtraEffectData _extraEffect)
		{
			var result = new List<UnitParameter>();
			if (_extraEffect != null)
				for (int i = 0; i < _extraEffect.EnemyIds.Count; i++)
					result.Add(UnitUtility.CreateUnitParameterFromEnemyParameter(_extraEffect.EnemyIds[i]));
			return result;
		}

		[global::OpenPCR.Migration("b8274812-174c-41cc-866d-c71d65a92c2c")]
		public virtual void PlayWinPerformance()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::PlayWinPerformance():System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("5055a296-fdac-46cf-b254-1b3e0f7425ac")]
		public virtual void PlayViewBGM()
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("0cf9ebc6-5ba1-42f4-ae2b-efb06192300a")]
		public virtual void PlayStory(Action _callback, int _storyId, bool _end, ViewBattle _viewBattle)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::PlayStory(System.Action,System.Int32,System.Boolean,Elements.ViewBattle):System.Void (outside first-phase story boundary)"); }

		[global::OpenPCR.Migration("4e0d5635-2a69-459f-8fd4-b6058a74bffe")]
		public virtual void PlayStoryAfter(Action _callback, int _storyId, bool _isEnd, ViewBattle _viewBattle)
		{
			_callback.Call();
		}

		[global::OpenPCR.Migration("8c6a30df-5c97-473b-82e8-d5f8e93a4b5b")]
		public virtual void PrepareBattle()
		{
			setupEnemyDamageInfo();
			setupEnemySkinData();
		}

		[global::OpenPCR.Migration("b1a3cdf1-46c4-4a37-8270-78005fe3b6bc")]
		public virtual void SetupEnemyStartProcess(UnitCtrl _enemyCtrl, int _enemyIdx)
		{
			eUnitRespawnPos pos = _enemyCtrl.IsBonusEnemy ? eUnitRespawnPos.SUB_POS_1 : getUnitRespawnPos(_enemyIdx, true);
			if ((pos == eUnitRespawnPos.MAIN_POS_1) & battleManager.IsSortBackBossBattle)
				pos = eUnitRespawnPos.SUB_POS_1;
			if (!_enemyCtrl.IsBoss)
				_enemyCtrl.ComponentState.StartStateIsWalk = true;
			else
			{
				eUnitRespawnPos? slot = null;
				int count = getBossCount();
				if (count == 1)
					slot = eUnitRespawnPos.MAIN_POS_5;
				else if (count == 2)
				{
					int index = getBossIndex(_enemyCtrl);
					if (index == 0)
						slot = eUnitRespawnPos.MAIN_POS_5;
					else if (index == 1)
						slot = eUnitRespawnPos.SUB_POS_5;
				}
				else if (count == 3)
				{
					int index = getBossIndex(_enemyCtrl);
					if ((uint)index <= 2u)
						slot = index == 0 ? eUnitRespawnPos.MAIN_POS_5 : index == 1 ? eUnitRespawnPos.SUB_POS_5 : eUnitRespawnPos.MAIN_POS_4;
				}

				if (slot.HasValue)
					pos = _enemyCtrl.BossSortIsBack ? eUnitRespawnPos.MAIN_POS_1 : slot.Value;
				_enemyCtrl.ComponentState.SetState(ActionState.GAME_START, 0, 0, false);
				_enemyCtrl.ComponentParameter.MoveSpeed = 0f;
			}

			_enemyCtrl.BattleStartProcess(pos);
		}

		[global::OpenPCR.Migration("a6f974e9-5c6d-4ee7-be75-bd739f8757d7")]
		protected void setupEnemyDamageInfo()
		{
			long viewerId = getEnemyUserViewerId();
			int count = battleManager.GetEnemyCtrlLength();
			for (int i = 0; i < count; i++)
			{
				UnitCtrl unit = battleManager.GetEnemyCtrl(i);
				unit.ComponentDamageControl.UnitDamageInfo = tempData.CreateDamageData(unit.OriginalUnitId, viewerId, unit.UnitParameter.UniqueData.GetCurrentRarity(), true, unit.UnitParameter.EnemyColor);
			}
		}

		[global::OpenPCR.Migration("4696481b-9edb-4edd-9207-42c45d6be021")]
		protected void setupEnemySkinData()
		{
			int enemyCtrlLength = battleManager.GetEnemyCtrlLength();
			int i = 0;
			for (int num = enemyCtrlLength; i < num; i++)
			{
				UnitCtrl enemy = battleManager.GetEnemyCtrl(i);
				UnitParameter unitParameter = tempData.BattleStageData.CurrentBattleWaves[0].Find((UnitParameter GEDLBPMPOKB) => (int)GEDLBPMPOKB.UniqueData.Id == enemy.OriginalUnitId);
				if (unitParameter != null)
					enemy.ComponentDamageControl.UnitDamageInfo.SetSkinData(unitParameter.UniqueData.SkinData);
			}
		}

		[global::OpenPCR.Migration("419b6420-1023-4faa-bd81-56e5215d531d")]
		public virtual void SetupTreasureBoxEffect(UnitCtrl _unitCtrl)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("c71b72fc-5b60-4583-92ef-e8a882f45787")]
		public virtual void SetupDropItem(UnitCtrl unitCtrl, WaveEnemyInfo info)
		{
			unitCtrl.Rupee = 0;
			unitCtrl.RewardCount = 0;
		}

		[global::OpenPCR.Migration("97ae2d66-ddca-4e8b-a01c-206515e89a76")]
		public virtual void SortPosition(List<UnitCtrl> _unitList, float[] _searchAreaList, int[] _posList)
		{
			BattleUtil.SortPosition(_unitList, _searchAreaList, _posList, false);
		}

		[global::OpenPCR.Migration("c74d5140-240b-408a-b364-99dde449bf9f")]
		public virtual void CleanUpData()
		{
		}

		[global::OpenPCR.Migration("8f5b9b6d-f974-4bf4-9794-284e302d6561")]
		public virtual void Release()
		{
			battleManager = null;
			battleCameraEffect = null;
			battleTimeScale = null;
			userData = null;
			tempData = null;
			soundManager = null;
		}

		[global::OpenPCR.Migration("26822cf5-c340-46e6-8cbf-06f1ea38562f")] public abstract int GetSystemId();
		[global::OpenPCR.Migration("c6973788-9ebd-49b8-8965-5fe0ca2ecc6e")]
		public virtual void PlayTimeup(Action _callback)
		{
			ManagerSingleton<DialogManager>.Instance.OpenFlatoutPlayer(DialogFlatoutPlayer.FlatoutType.TIME_UP, _callback, 0, 0, null);
		}

		[global::OpenPCR.Migration("89aa7c39-ea11-4e32-bd1f-666de52f74aa")]
		public virtual int GetCurrentWaveOffset()
		{
			return 0;
		}

		[global::OpenPCR.Migration("8128590b-0567-4556-9f45-4b2de15172c5")]
		public virtual bool GetEnableShadowEffect()
		{
			return false;
		}

		[global::OpenPCR.Migration("75f7fc0b-7b7c-4877-a72f-0f83aacadce4")]
		public virtual void CreateVeryHardBackgroundEffect()
		{
		}

		[global::OpenPCR.Migration("ea48abf9-dffc-41c4-9cad-f2ef717be896")]
		public virtual bool JudgeWinWhenTimeUp()
		{
			return false;
		}

		[global::OpenPCR.Migration("390f5a21-9882-4cbf-8127-c8575f7f0881")]
		public virtual bool GetForceNormalSD()
		{
			return false;
		}

		[global::OpenPCR.Migration("1219d169-d503-4ee6-b100-ce11561f653a")]
		public virtual bool DisplayRetireFail()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::DisplayRetireFail():System.Boolean (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("e1114674-3ac6-4c6a-8c6d-33bf399c3310")]
		public float GetDodgeTPRecoveryRatio()
		{
			return (float)(double)ManagerSingleton<MasterDataManager>.Instance.masterDodgeTpRecovery.Get(GetSystemId()).recovery_ratio;
		}

		[global::OpenPCR.Migration("3a8bbf3b-f759-4ff9-b9a5-b7ea315e0e4a")]
		public virtual double CalcPlayerDamageTpReduceRate(int _promotionLevel)
		{
			return UnitUtility.CalcDamageTpReduceRate(userData.UserInfo.TeamLevel, _promotionLevel);
		}

		[global::OpenPCR.Migration("feaa99be-9e26-4b71-b0d9-ea2570a53ae0")]
		public virtual double CalcEnemyDamageTpReduceRate(int _promotionLevel)
		{
			return 1.0;
		}

		[global::OpenPCR.Migration("3a61fbdb-cacb-42ff-aa11-4f16796cf179")]
		public virtual bool IsDefenseBattle()
		{
			return false;
		}

		[global::OpenPCR.Migration("b928e242-1291-4bf6-92f3-41c4a593e731")]
		public virtual bool IsDefenseBattleInMode()
		{
			return false;
		}

		[global::OpenPCR.Migration("83a74a29-793c-4055-99ca-99f5fba62303")]
		public virtual float GetStartTimeLimitForDefense()
		{
			return 90f;
		}

		[global::OpenPCR.Migration("c160f7c2-9186-48c1-9629-368f6cf841be")]
		public virtual float GetTakeOverTimeForDefense()
		{
			return 90f;
		}

		[global::OpenPCR.Migration("c6fc6160-5ec0-4761-8fe5-fb40c3262060")]
		public virtual bool IsShowBossGauge()
		{
			return true;
		}

		[global::OpenPCR.Migration("3dfd4f4f-8871-45e9-a5ec-d97bddb45913")]
		public virtual (long currentHp, long maxHp) GetTotalBossHp()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::GetTotalBossHp():System.ValueTuple\u00602\u003CSystem.Int64,System.Int64\u003E (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("8a36d22e-5222-4372-a46f-0f4488e9f124")]
		public virtual void OpenBonusDialog(Action _callback, bool _isDefeat)
		{
		}

		[global::OpenPCR.Migration("150eba5e-69d3-4783-9b39-c1203c1f3d15")]
		public virtual bool GetIsMultiBossBattle()
		{
			return false;
		}

		[global::OpenPCR.Migration("693cc384-b231-498d-86b0-4ec25ea808e7")]
		public virtual void DisplayMultiBossFrontOfDialog(List<UnitCtrl> _bossList)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::DisplayMultiBossFrontOfDialog(System.Collections.Generic.List\u00601\u003CElements.UnitCtrl\u003E):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("db2131b1-3e5a-49a6-a8df-d2eba3b7c772")]
		public virtual bool IsPlayMiddleGroundLoseEffect()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::IsPlayMiddleGroundLoseEffect():System.Boolean (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("5ab50c17-32bd-43f6-a459-f634d6d592fd")]
		public virtual void DoPlayerMoveTaskAfterWin(Action _finishCallBack)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::DoPlayerMoveTaskAfterWin(System.Action):System.Void (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("b24291bb-2db4-45d9-9f5c-6e5830f58fea")]
		public virtual bool IsReplay()
		{
			return false;
		}

		[global::OpenPCR.Migration("2a7a483f-a518-42e2-8894-3f7fddc0c762")]
		public virtual bool JudgeIsIgnoreUnit(UnitCtrl _unit)
		{
			return _unit.IsBonusEnemy;
		}

		[global::OpenPCR.Migration("cee40c52-9d88-424d-b7c7-d1866cad1fbf")]
		public virtual long GetEnemyPoint()
		{
			return 0;
		}

		[global::OpenPCR.Migration("f29fe6eb-c110-4881-bb78-c27a346b8cb4")]
		public virtual long GetPurposeCount()
		{
			return 0;
		}

		[global::OpenPCR.Migration("fda55a01-4eee-446b-8b0f-a5816bf1922d")]
		public virtual eSpecialBattlePurpose GetPurpose()
		{
			return eSpecialBattlePurpose.INVALID_VALUE;
		}

		[global::OpenPCR.Migration("cf12f848-e79e-4083-a320-00785d53406a")]
		public virtual int GetEnemyPoint(int _unitId)
		{
			return 0;
		}

		[global::OpenPCR.Migration("c9a76b07-2ca9-41d3-99de-2dbaba043577")]
		public virtual int GetInitialPosition(int _unitId)
		{
			return 0;
		}

		[global::OpenPCR.Migration("7b42f0b1-aa92-46c9-806a-02e4550a664e")]
		public virtual bool IsSpecialBattle()
		{
			return false;
		}

		[global::OpenPCR.Migration("244d249f-981f-44e1-9b19-f5d4eb647de1")]
		public virtual List<UnitDamageInfo> GetUnitDamageInfos()
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::GetUnitDamageInfos():System.Collections.Generic.List\u00601\u003CElements.UnitDamageInfo\u003E (outside supported first-phase lifecycle/input scope)"); }

		[global::OpenPCR.Migration("ad218c4d-4cc4-4a02-8ac2-898e2ee194d6")]
		public virtual bool GetBossGaugeColorChange()
		{
			return false;
		}

		[global::OpenPCR.Migration("aab41dde-14e3-4b7a-9d80-9889bc852c22")]
		protected virtual int getBossCount()
		{
			switch (Singleton<TempData>.Instance.BattleStageData.CurrentBattleCategory)
			{
				case eBattleCategory.HATSUNE_BOSS_BATTLE:
				case eBattleCategory.HATSUNE_SPECIAL_BATTLE:
					if (HatsuneUtility.IsNowEventMultiBoss())
					{
						return HatsuneUtility.GetNowBossBattleEnemySettingDatas().Count;
					}

					break;
				case eBattleCategory.DUNGEON_SPECIAL:
					if (DungeonUtility.IsNowSpecialMultiBoss())
					{
						return DungeonUtility.GetNowSpecialBossBattleEnemySettingDatas().Count;
					}

					break;
			}

			return battleManager.BossList.Count;
		}

		[global::OpenPCR.Migration("4eea0f49-8762-4df9-912c-f6c4b5f21452")]
		protected virtual int getBossIndex(UnitCtrl _enemyCtrl)
		{
			switch (Singleton<TempData>.Instance.BattleStageData.CurrentBattleCategory)
			{
				case eBattleCategory.HATSUNE_BOSS_BATTLE:
				case eBattleCategory.HATSUNE_SPECIAL_BATTLE:
					if (HatsuneUtility.IsNowEventMultiBoss())
					{
						MasterHatsuneBossEnemySetting.HatsuneBossEnemySetting nowEventBossEnemySettingForEnemyId = HatsuneUtility.GetNowEventBossEnemySettingForEnemyId(_enemyCtrl.UnitId);
						return getBossCount() - (int)nowEventBossEnemySettingForEnemyId.enemy_identify;
					}

					break;
				case eBattleCategory.DUNGEON_SPECIAL:
					{
						if (!DungeonUtility.IsNowSpecialMultiBoss())
						{
							break;
						}

						MasterWaveGroupData.WaveGroupData nowSpecialBossWaveGroupData = DungeonUtility.GetNowSpecialBossWaveGroupData();
						for (int i = 0; i < nowSpecialBossWaveGroupData.EnemyInfos.Count; i++)
						{
							if (nowSpecialBossWaveGroupData.EnemyInfos[i].EnemyId == _enemyCtrl.UnitId)
							{
								return i;
							}
						}

						break;
					}
			}

			return battleManager.BossList.IndexOf(_enemyCtrl);
		}

		[global::OpenPCR.Migration("c7887a22-2f4e-4ef7-b46a-cecca5972e41")]
		public virtual void SetPartsTalentWeakness(int _unitId, BasePartsData _partsData)
		{
			_partsData.SetTalentWeakness(ManagerSingleton<MasterDataManager>.Instance.masterEnemyTalentWeakness.GetWeaknessDictionary(_unitId));
		}

		[global::OpenPCR.Migration("602ce7a8-1ec9-47bb-91a9-7a3278a0bb51")]
		protected List<BattleManager.BonusEnemyData> CreateBonusEnemyDatas(MasterExtraEffectData.ExtraEffectData _extraEffect, BattleDefine.eExtraEffectExecTiming _timing)
		{
			var result = new List<BattleManager.BonusEnemyData>();
			if (_extraEffect != null)
			{
				for (int i = 0; i < _extraEffect.EnemyIds.Count; i++)
				{
					if (_extraEffect.ExecTimings[i] != _timing)
						continue;
					int enemyId = _extraEffect.EnemyIds[i];
					UnitCtrl unit = battleManager.FindEnemy(enemyId);
					var skills = new List<(int, int)>();
					for (int j = 0; j < unit.MainSkillIdList.Count; j++)
						if (unit.MainSkillIdList[j] != 0)
							skills.Add((unit.MainSkillIdList[j], 0));
					result.Add(new BattleManager.BonusEnemyData { SkillIdAndBonusIdList = skills, Id = enemyId });
				}
			}

			return result;
		}

		[global::OpenPCR.Migration("e8f73d12-b43b-4553-a1dc-6d41b642b859")] public abstract List<BattleManager.BonusEnemyData> GetExtraEffectEnemyData(BattleDefine.eExtraEffectExecTiming _timing);
		[global::OpenPCR.Migration("c143c44c-a1d3-4cf6-b410-2b02c3778a86")]
		public virtual int GetTimeStartStoryId()
		{
			return 0;
		}

		[global::OpenPCR.Migration("3886fa07-4cbf-4189-8751-b01fc5b21256")]
		public virtual int GetWaveStartStoryId()
		{
			return 0;
		}

		[global::OpenPCR.Migration("da04e916-3db9-4485-a479-7b13cd447e18")]
		public virtual bool HasWaveStartStory()
		{
			return GetWaveStartStoryId() != 0;
		}

		[global::OpenPCR.Migration("fdcc45ea-fda6-40eb-ba7d-6db16a97468c")]
		public virtual bool IsPlayTalentFormationBonusEffect()
		{
			int confirm = ManagerSingleton<SaveDataManager>.Instance.TalentFormationBonusConfirm;
			bool first = ManagerSingleton<SaveDataManager>.Instance.TalentFormationBonusFirst;
			if (confirm == 1 || (confirm == 2 && first))
				return false;
			return battleManager.IsActiveUnitTalentFormationBonus && battleManager.CurrentWave < 1;
		}

		[global::OpenPCR.Migration("b8319f27-798f-49f9-b6aa-77a5537e1115")] public abstract bool IsShowEffectiveIcon(int _unitId);
		[global::OpenPCR.Migration("e2e6819b-a5ee-46e6-ab41-b49c1de3a5c7")]
		public virtual IEnumerator LoadWinTextResource()
		{
			bool isLoaded = false;
			ResourceManager resourceManager = ManagerSingleton<ResourceManager>.Instance;
			resourceManager.AddLoadResourceId(getWinTextResourceId(), 0L, null, false, true);
			resourceManager.StartLoad(delegate
			{
				isLoaded = true;
			});
			while (!isLoaded)
				yield return null;
		}

		[global::OpenPCR.Migration("48405cb6-fc43-4c1f-9622-134b4ccb194e")]
		protected virtual eResourceId getWinTextResourceId()
		{
			return eResourceId.FLATOUT_COMBINEANIMATION_BATTLEWIN_LOOP;
		}

		[global::OpenPCR.Migration("d7cb5af7-a1d5-47ca-a006-7e5939ab9ab6")]
		public virtual ContinuousUnit IdentifyContinuous(List<ContinuousUnit> _continuousUnitList, UnitData _unitData)
		{ throw new global::OpenPCR.OutOfBoundaryException("Elements.Battle.BaseBattleProcessor::IdentifyContinuous(System.Collections.Generic.List\u00601\u003CElements.ContinuousUnit\u003E,Elements.UnitData):Elements.ContinuousUnit (outside first-phase mode-leaf boundary)"); }

		[global::OpenPCR.Migration("eab45482-30c6-44e5-878b-7057f39e44c9")]
		public virtual bool HasTimeStartStoryId()
		{
			return false;
		}

		[global::OpenPCR.Migration("935eb317-7914-483f-be08-baf36988a2e7")]
		public virtual bool GetIsDefenseReplayMode()
		{
			return false;
		}

		[global::OpenPCR.Migration("f7c50ece-f696-43cf-9c9a-4d4ece0209c7")]
		public virtual Dictionary<BattleManager.eBonusType, List<BattleManager.BonusData>> GetExtraEffectDirectionDictionary()
		{
			return null;
		}

		[global::OpenPCR.Migration("6a29e1d1-137f-4ce9-b380-c3916ff6090e")]
		public virtual int GetTriggerHpPercent()
		{
			return 0;
		}

		[global::OpenPCR.Migration("6f947120-f847-4e47-bba9-185cba92c859")]
		public virtual float GetCameraWaveScale(int _waveIdx)
		{
			if (battleCameraEffect.BossBattleScale == 0f)
				return 1f;
			return battleCameraEffect.BossBattleScale;
		}

		[global::OpenPCR.Migration("4d769853-fe39-41b2-9499-f4aa08a52075")]
		public virtual bool IsReleasedKnightEnhance()
		{
			return ManagerSingleton<MasterDataManager>.Instance.masterContentReleaseData.CheckReleaseContents(eSystemId.KNIGHT_ENHANCE);
		}
	}
}
