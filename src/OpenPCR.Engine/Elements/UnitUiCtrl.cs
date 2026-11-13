using global::Elements;
using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System.Linq;
using Elements.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle;
using Elements.Battle.Core;

namespace Elements
{
	public class UnitUiCtrl : SingletonMonoBehaviour<UnitUiCtrl>, ISingletonField
	{
		private static readonly string BTN_ON_SPRITE_NAME = "battle_btn_main_auto";
		private static readonly string BTN_OFF_SPRITE_NAME = "battle_btn_sub_auto";
		private static readonly string BTN_ON_SPRITE_NAME_SPEED = "battle_btn_main_speedup";
		private static readonly string BTN_OFF_SPRITE_NAME_SPEED = "battle_btn_sub_speedup";
		private static readonly string BTN_ON_SPRITE_NAME_SPEED_QUADRUPLE = "battle_btn_main_speedup4x";
		[SerializeField]
		private CustomUIButton gameSpeedButton;
		[SerializeField]
		private CustomUIButton skillModeButton;
		[SerializeField]
		private GameObject skillModeButtonEffect;
		[SerializeField]
		private CustomUIButton skipBattleButton;
		private long myViewerId;
		private SaveDataManager saveDataManager = ManagerSingleton<SaveDataManager>.Instance;
		[SerializeField]
		private UITexture[] UnitIconPnls;
		[SerializeField]
		private UISlider[] lifeGaugeSliderArray;
		[SerializeField]
		private UISlider[] skillGaugeSliderArray;
		[SerializeField]
		private AbromalStateIconController[] AbnormalStateIconControllers;
		[SerializeField]
		private GameObject[] skillEnableEffects;
		[SerializeField]
		private GameObject[] skillReserveEffects;
		[SerializeField]
		private UISprite[] FrameSprites;
		[SerializeField]
		private CustomUIButton[] buttonArray;
		[SerializeField]
		private CustomUIButton guestButton;
		[SerializeField]
		private PartsUnitIconStarControl[] starIconCtrls;
		[SerializeField]
		private PartsEquipStatusIcon[] equipStatusIconCtrls;
		[SerializeField]
		private GameObject[] effectiveIcons;
		[SerializeField]
		private Animator[] skillReserveAnimations;
		[SerializeField]
		private Animator guestSkillReserveAnimation;
		[SerializeField]
		private FullScreenFade fullScreenFade;
		[SerializeField]
		private GameObject skillReserveExecuteEffectBaseObj;
		private static readonly Color ENABLE_COLOR = Color.white;
		private static readonly Color DESABLE_COLOR = new Color(0.3921f, 0.3921f, 0.3921f);
		private static readonly Color LIGHT_DESABLE_COLOR = new Color(0.7843f, 0.7843f, 0.7843f);
		private static readonly Vector3 POSITION_STATE_ICON_CONTROLLER_TRANSFORM_LOCAL = new Vector3(-45f, 96f, 0f);
		private static readonly Vector3 SCALE_ABNOMAL_STATE_ICON_CONTROLLER_TRANSFORM_LOCAL = new Vector3(0.5f, 0.5f, 1f);
		private IBattleManagerForUnitUiCtrl battleManager;
		private bool isDisableUpdate;
		private ViewManager viewManager = ManagerSingleton<ViewManager>.Instance;
		private float battleDeltaTime = 1f / 30f;
		private Yggdrasil<UnitUiCtrl> singletonTree;
		private IBattleTimeScaleForUnitUiCtrl battleTimeScale;
		private bool guestIsUnit;
		private bool guestUnitDead;
		private bool guestSkillReady;
		private float guestLifeAmount = 1f;
		private float guestMaxLifeAmount = 1f;
		[SerializeField]
		private UITexture guestUnitIconPnl;
		[SerializeField]
		private UISlider guestLifeGaugeSlider;
		[SerializeField]
		private UISlider guestSkillGaugeSlider;
		private AbromalStateIconController guestAbnormalStateIconController;
		[SerializeField]
		private GameObject guestSkillEnableEffect;
		[SerializeField]
		private GameObject guestSkillReserveEffect;
		[SerializeField]
		private UISprite guestFrameSprite;
		[SerializeField]
		private Transform unitBaseTransform;
		[SerializeField]
		private UIGrid buttonLayoutGrid;
		private bool autoModeFlagBackup;
		private bool speedModeFlagBackup;
		private bool speedQuadrupleFlagBackup;
		private AnimKitNormalEffect skillOnObj { get; set; }
		private GameObject skillOnPrefab { get; set; }
		private ParticleSystem skillReserveExecParticle { get; set; }
		private GameObject skillReserveExecParticlePrefab { get; set; }
		public UnitCtrl[] UnitCtrls { get; private set; }
		public bool IsAutoMode { get; private set; }
		public bool IsEnabledChangeAutoMode { get; private set; }
		private bool isSpeedMode { get; set; }
		private bool isBattleSpeedQuadruple { get; set; }
		public bool IsUnitCreated { get; private set; }
		private bool[] isUnits { get; set; }
		private bool[] unitDeads { get; set; }
		private Dictionary<int, UnitParameter> m_pUnitParameters { get; set; }
		public bool EnableTap { get; set; }
		private UnitCtrl skillExeUnit { get; set; }
		private bool[] skillReadys { get; set; }
		public UnitSkillReserve SkillReserves { get; set; }
		private eBattleCategory battleCategory { get; set; }

		public bool IsDisableUpdate
		{
			[global::OpenPCR.Migration("19209432-fb74-48cc-9554-a56ef43bee64")]
			get
			{
				return isDisableUpdate;
			}

			[global::OpenPCR.Migration("3ef47b0d-0f5d-4072-9be4-4ff787156703")]
			set
			{
				isDisableUpdate = value;
			}
		}

		public bool AllEnemyHp0 { get; private set; }
		private float[] lifeAmounts { get; set; }
		private float[] maxLifeAmounts { get; set; }
		public UnitCtrl GuestUnit { get; set; }

		[global::OpenPCR.Migration("63cc1aae-6935-4673-af01-61f9c222466e")]
		public UnitUiCtrl()
		{
		}

		[global::OpenPCR.Migration("8d3c9386-4457-4ea9-b7b9-0e842897c164")]
		protected override void OnAwake()
		{
			m_pUnitParameters = Singleton<UserData>.Instance.UnitParameterDictionary;
			IsUnitCreated = false;
			EnableTap = true;
			UnitCtrls = new UnitCtrl[5];
			isUnits = new bool[5];
			unitDeads = new bool[5];
			skillReadys = new bool[5];
			SkillReserves = new UnitSkillReserve();
			lifeAmounts = new float[5]
			{
				1f,
				1f,
				1f,
				1f,
				1f
			};
			maxLifeAmounts = new float[5]
			{
				1f,
				1f,
				1f,
				1f,
				1f
			};
		}

		[global::OpenPCR.Migration("e3ba16ac-8474-4b58-80c5-d4a3c150f551")]
		public void Initialize(ePartyType _usePartyType)
		{
			TempData tempData = Singleton<TempData>.Instance;
			TempData.BattleStage battleStageData = tempData.BattleStageData;
			battleCategory = battleStageData.CurrentBattleCategory;
			bool flag = false;
			myViewerId = Singleton<UserData>.Instance.UserInfo.ViewerId;
			singletonTree = this.CreateSingletonTree();
			battleManager = singletonTree.Get<BattleManager>();
			battleTimeScale = singletonTree.Get<BattleTimeScale>();
			Dictionary<int, long> dispatchOwnerId = new Dictionary<int, long>();
			Dictionary<int, int> dispatchUnitId = new Dictionary<int, int>();
			uint categoryValue = (uint)battleCategory;
			flag = categoryValue < 46 && ((0x3706114621B8UL >> (int)categoryValue) & 1UL) != 0;
			if (battleCategory == eBattleCategory.BYWAY_QUEST)
				flag |= BywayQuestUtility.IsFixAutoBattle(tempData.SelectedQuestId);
			if (battleCategory == eBattleCategory.TUTORIAL || battleCategory == eBattleCategory.STORY)
			{
				skillModeButton.isEnabled = false;
				allSetModeButton.isEnabled = false;
				skillModeButton.SetOnDisabledClickDelegate(this, onClickPauseButtonDisable);
				allSetModeButton.SetOnDisabledClickDelegate(this, onClickPauseButtonDisable);
				setSpeedMode(_isSpeedMode: false, _isBattleSpeedQuadruple: false);
				gameSpeedButton.isEnabled = false;
				gameSpeedButton.SetOnDisabledClickDelegate(this, onClickPauseButtonDisable);
			}

			UnitParameter unitParameter = null;
			Dictionary<long, List<UnitParameter>> pUnitListDictionary = new Dictionary<long, List<UnitParameter>>
			{
				{
					myViewerId,
					new List<UnitParameter>()
				}
			};
			int num = 0;
			switch (_usePartyType)
			{
				case ePartyType.LABYRINTH:
				case ePartyType.LABYRINTH_EX_1:
				case ePartyType.LABYRINTH_EX_2:
				case ePartyType.LABYRINTH_EX_3:
					var labyrinthUnits = singletonTree.Get<LabyrinthTempData>().UnitParameterDictionary;
					var labyrinthParty = Singleton<UserData>.Instance.FindPartyUnitIdArray(_usePartyType);
					for (int i = 0; i < 5; i++)
						if (labyrinthParty[i] != 0) pUnitListDictionary[myViewerId].Add(labyrinthUnits[labyrinthParty[i]]);
					break;
				case ePartyType.REPLAY:
					pUnitListDictionary[myViewerId] = battleStageData.ReplayPartyUnits;
					if (!BattleUtil.IsClanBattleCategory(battleCategory))
					{
						break;
					}

					foreach (UnitParameter item in pUnitListDictionary[myViewerId])
					{
						item.UpdateLimitedUnitData(_calcLocalPower: true, _isSupportOrReplay: true);
					}

					break;
				case ePartyType.STORY:
					pUnitListDictionary[myViewerId] = new List<UnitParameter>(tempData.StoryQuestPartyParameter.Values);
					UnitUtility.SortPartyByPosition(pUnitListDictionary[myViewerId]);
					break;
				default:
					{
						int[] array = Singleton<UserData>.Instance.FindPartyUnitIdArray(_usePartyType);
						UnitParameter unitParameter2 = null;
						for (int i = 0; i < 5; i++)
						{
							bool flag2 = false;
							num = array[i];
							if (num == 0)
							{
								continue;
							}

							if (IsDispatchUnit(num))
							{
								switch (battleCategory)
								{
									case eBattleCategory.QUEST:
									case eBattleCategory.TRAINING:
									case eBattleCategory.HATSUNE_BATTLE:
										long supportOwnerId = tempData.SupportOwnerId;
										int supportUnitId = tempData.SupportUnitId;
										unitParameter2 = new UnitParameter(SupportUnitUtility.GetSupportUnit(supportOwnerId, supportUnitId).UnitParamData);
										dispatchOwnerId.Add(1, supportOwnerId);
										dispatchUnitId.Add(1, supportUnitId);
										break;
									case eBattleCategory.CLAN_BATTLE:
										unitParameter2 = ClanBattleUtil.GetSupportUnitParam();
										if (unitParameter2 != null)
										{
											dispatchOwnerId.Add(1, Singleton<ClanBattleTempData>.Instance.SupportOwnerId);
											dispatchUnitId.Add(1, unitParameter2.UniqueData.Id);
										}

										break;
									case eBattleCategory.TRIAL_BATTLE:
									case eBattleCategory.TRIAL_CLAN_BATTLE:
										TrialBattleTempData trialTemp = this.CreateSingletonTree().Get<TrialBattleTempData>();
										unitParameter2 = new UnitParameter(trialTemp.SupportUnitList.Find(e => (long)e.OwnerViewerId == trialTemp.SupportOwnerId && (int)e.UnitParamData.Id == trialTemp.SupportUnitId).UnitParamData);
										if (unitParameter2 != null)
										{
											unitParameter2.UpdateLimitedUnitData(true, BattleUtil.IsClanBattleCategory(battleCategory));
											dispatchOwnerId.Add(1, trialTemp.SupportOwnerId);
											dispatchUnitId.Add(1, unitParameter2.UniqueData.Id);
										}

										break;
									default:
										throw new OpenPCR.OutOfBoundaryException("UnitUiCtrl.Initialize: outside guild/training support input production");
								}
							}
							else
							{
								unitParameter2 = m_pUnitParameters[num];
								if ((battleCategory == eBattleCategory.CLAN_BATTLE || battleCategory == eBattleCategory.TRIAL_CLAN_BATTLE))
								{
									flag2 = true;
								}
							}

							if (battleCategory == eBattleCategory.LEGION_BATTLE_SUB)
								throw new OpenPCR.OutOfBoundaryException("UnitUiCtrl.Initialize: SRE effect parameter input production");
							if (flag2)
							{
								unitParameter2 = UnitUtility.AlterUnitDataLevelLimit(unitParameter2);
							}

							pUnitListDictionary[myViewerId].Add(unitParameter2);
						}

						break;
					}
			}

			foreach (List<UnitParameter> value2 in pUnitListDictionary.Values)
			{
				for (int num8 = 0; num8 < value2.Count; num8++)
				{
					bool isClanSupportOrReplay = value2[num8].IsClanSupportOrReplay;
					UnitParameter target = value2[num8];
					value2[num8] = UnitUtility.MightConvertRarity6Unit(in target);
					if (isClanSupportOrReplay)
					{
						value2[num8].UpdateLimitedUnitData(_calcLocalPower: true, _isSupportOrReplay: true);
					}
				}
			}

			ResourceManager instance2 = ManagerSingleton<ResourceManager>.Instance;
			instance2.AddLoadResourceId(eResourceId.ABNORMAL_STATEICON);
			instance2.AddLoadResourceId(eResourceId.FX_SKILLON_1);
			instance2.AddLoadResourceId(eResourceId.FX_UB_TAP, delegate (object obj)
			{
				skillOnPrefab = obj as GameObject;
				skillOnObj = global::OpenPCR.Runtime.Api.Object.Instantiate(skillOnPrefab).GetComponent<AnimKitNormalEffect>();
				skillOnObj.transform.SetParent(ExceptUIRoot.Transform);
				skillOnObj.SetDepth(4901);
				skillOnObj.SetActive(_isActive: false);
				skillOnObj.SetIsIndependentBattleSync();
			});
			instance2.AddLoadResourceId(eResourceId.FX_SKILL_EXECUTE, delegate (object obj)
			{
				skillReserveExecParticlePrefab = global::OpenPCR.Runtime.Api.Object.Instantiate(obj as GameObject);
				skillReserveExecParticle = skillReserveExecParticlePrefab.GetComponentInChildren<ParticleSystem>();
				skillReserveExecParticlePrefab.transform.SetParent(skillReserveExecuteEffectBaseObj.transform);
				skillReserveExecParticlePrefab.SetActive(value: false);
			});
			if (tempData.BattleSkipState != eBattleSkipState.SKIPPING)
			{
				instance2.AddLoadResourceId(eResourceId.FX_BUFF_STATE);
				instance2.AddLoadResourceId(eResourceId.FX_DEBUFF_STATE);
				instance2.AddLoadResourceId(eResourceId.FX_BUFF_DEBUFF_STATE);
			}

			int count = pUnitListDictionary[myViewerId].Count;
			battleManager.InitializePassiveSkillUnitList(pUnitListDictionary[myViewerId], ePassiveInitType.ALL_BY_UNIT, 0);
			AbnormalStateIconControllers = new AbromalStateIconController[5];
			UnitUtility.SortPartyByPosition(pUnitListDictionary[myViewerId]);
			int num9 = 0;
			UnitCtrl heroUnit = null;
			for (int num10 = 0; num10 < 5; num10++)
			{
				setVisibleSkillFrame(num10, _isVisible: false);
				setVisibleSkillReserveFrame(num10, _isVisible: false);
				controllingSkillReserveAnimation(num10, _isPlay: false);
				// Native pure icon color/energy-slider output reset excluded.
				effectiveIcons[num10].SetActive(value: false);
				// Native pure icon color/energy-slider output reset excluded.
				if (num10 >= count)
				{
					setVisibleObject(num10, _enable: false);
					continue;
				}

				unitParameter = pUnitListDictionary[myViewerId][num10];
				bool isHero = (int)unitParameter.MasterData.PrefabId == 900001;
				if (isHero)
				{
					num9++;
				}

				bool otherUser = BattleUtil.IsCheckSupportChara(battleCategory) && isOtherUnit(dispatchOwnerId, dispatchUnitId, unitParameter.UniqueData.Id);
				bool flag3 = UnitUtility.GetSetSkinNo(unitParameter.UniqueData, UnitDefine.eSkinType.Sd, otherUser, null) >= 6;
				MasterUnitData.UnitData unitData = ManagerSingleton<MasterDataManager>.Instance.masterUnitData.Get(UnitUtility.GetUnitResourceId(unitParameter.UniqueData.Id));
				int cutinId = (flag3 ? unitData.cutin1_star6 : unitData.CutIn1);
				if (saveDataManager.CutInPlaybackType != ePlaybackType.NONE && !battleManager.IsDefenseReplayMode && tempData.BattleSkipState != eBattleSkipState.SKIPPING && cutinId != 0)
				{
					ManagerSingleton<MovieManager>.Instance.Load(eMovieType.CUT_IN, cutinId, false, null, true, 0L, false, false, false, 0f, 1f, 0);
				}

				int forCallbackKeepI = num10 - num9;
				battleManager.CreateUnit(UnitUtility.GetUnitResourceId(unitParameter.MasterData.PrefabId), delegate (UnitCtrl _unitCtrl)
				{
					if (!isHero)
					{
						UnitCtrls[forCallbackKeepI] = _unitCtrl;
						if (!battleManager.IsDefenseReplayMode)
							setOnInitCallback(forCallbackKeepI, _unitCtrl, cutinId);
					}
					else
					{
						heroUnit = _unitCtrl;
					}
				}, _addUnitList: true);
				if (isHero)
				{
					continue;
				}

				isUnits[forCallbackKeepI] = true;
				unitDeads[forCallbackKeepI] = false;
				bool otherUser2 = false;
				UnitData battleUnitData = BattleUtil.GetBattleUnitData(unitParameter, battleCategory);
				int unitId = battleUnitData.Id;
				SkillReserves[unitId] = false;
				if (BattleUtil.IsCheckSupportChara(battleCategory))
				{
					otherUser2 = isOtherUnit(dispatchOwnerId, dispatchUnitId, unitId);
				}

				// Native icon texture, star/equipment badge, effective and trait sprite outputs are excluded.
				if (!flag)
				{
					buttonArray[forCallbackKeepI].SetOnClickDelegate(this, onTapPanel, num10, _arg1: true, _arg2: false);
				}

				if (battleManager.CanReserveSkill)
				{
					buttonArray[forCallbackKeepI].ChangeEnabledSE();
					buttonArray[forCallbackKeepI].ChangeDisabledSE();
				}

				if (num10 >= count - num9)
				{
					setVisibleObject(num10, _enable: false);
				}

				int indexForCallback = forCallbackKeepI;
				instance2.AddInstantiateGameObject(eResourceId.ABNORMAL_STATEICON, 0L, delegate (GameObject createdGameObject)
				{
					AbromalStateIconController component = createdGameObject.GetComponent<AbromalStateIconController>();
					component.Initialize(4, false, false, 1, false);
					component.transform.parent = buttonArray[indexForCallback].transform;
					component.transform.localPosition = POSITION_STATE_ICON_CONTROLLER_TRANSFORM_LOCAL;
					component.transform.localScale = SCALE_ABNOMAL_STATE_ICON_CONTROLLER_TRANSFORM_LOCAL;
					AbnormalStateIconControllers[indexForCallback] = component;
				});
				if (tempData.BattleSkipState != eBattleSkipState.SKIPPING)
				{
					float y = -150f * ManagerSingleton<ViewManager>.Instance.CurUIRoot.transform.lossyScale.x + 0.054f;
					instance2.AddInstantiateGameObject(eResourceId.FX_BUFF_DEBUFF_STATE, 0L, delegate (GameObject createdGameObject)
					{
						createdGameObject.transform.SetParent(ExceptUIRoot.Transform);
						createdGameObject.transform.position = buttonArray[indexForCallback].transform.position;
						createdGameObject.transform.position += new Vector3(0f, y, 0f);
						createdGameObject.SetActive(value: false);
					});
					instance2.AddInstantiateGameObject(eResourceId.FX_DEBUFF_STATE, 0L, delegate (GameObject createdGameObject)
					{
						createdGameObject.transform.SetParent(ExceptUIRoot.Transform);
						createdGameObject.transform.position = buttonArray[indexForCallback].transform.position;
						createdGameObject.transform.position += new Vector3(0f, y, 0f);
						createdGameObject.SetActive(value: false);
					});
					instance2.AddInstantiateGameObject(eResourceId.FX_BUFF_STATE, 0L, delegate (GameObject createdGameObject)
					{
						createdGameObject.transform.SetParent(ExceptUIRoot.Transform);
						createdGameObject.transform.position = buttonArray[indexForCallback].transform.position;
						createdGameObject.transform.position += new Vector3(0f, y, 0f);
						createdGameObject.SetActive(value: false);
					});
				}
			}

			if (battleStageData.GuestUnitParameter != null)
			{
				unitBaseTransform.localPosition += new Vector3(85f, 0f, 0f);
				UnitParameter guestParam = battleStageData.GuestUnitParameter;
				setGuestVisibleSkillFrame(_isVisible: false);
				setGuestVisibleSkillReserveFrame(_isVisible: false);
				controllingGuestSkillReserveAnimation(_isPlay: false);
				battleManager.CreateUnit(UnitUtility.GetUnitResourceId(guestParam.MasterData.PrefabId), delegate (UnitCtrl _unitCtrl)
				{
					bool num11 = UnitUtility.GetSetSkinNo(guestParam.UniqueData, UnitDefine.eSkinType.Sd, false, null) >= 6;
					MasterUnitData.UnitData unitData2 = ManagerSingleton<MasterDataManager>.Instance.masterUnitData.Get(UnitUtility.GetUnitResourceId(guestParam.UniqueData.Id));
					int num12 = (num11 ? unitData2.cutin1_star6 : unitData2.CutIn1);
					if (!battleManager.IsDefenseReplayMode && tempData.BattleSkipState != eBattleSkipState.SKIPPING)
					{
						ManagerSingleton<MovieManager>.Instance.Load(eMovieType.CUT_IN, num12, false, null, true, 0L, false, false, false, 0f, 1f, 0);
					}

					GuestUnit = _unitCtrl;
					_unitCtrl.IsGuest = true;
					setupGuestUiCallback(_unitCtrl, num12);
				}, _addUnitList: true);
				guestIsUnit = true;
				guestUnitDead = false;
				bool otherUser3 = false;
				if (BattleUtil.IsCheckSupportChara(battleCategory))
				{
					otherUser3 = isOtherUnit(dispatchOwnerId, dispatchUnitId, guestParam.UniqueData.Id);
				}

				// Guest icon texture and promotion frame are pure render outputs.
				if (!flag)
				{
					guestButton.SetOnClickDelegate(this, onTapGuestPanel);
				}

				// Native onLongPressGuest/onLongPressNowGuest delegates only open the skill detail popup; UI input excluded.

				if (battleManager.CanReserveSkill)
				{
					guestButton.ChangeEnabledSE();
					guestButton.ChangeDisabledSE();
				}

				instance2.AddInstantiateGameObject(eResourceId.ABNORMAL_STATEICON, 0L, delegate (GameObject createdGameObject)
				{
					AbromalStateIconController component = createdGameObject.GetComponent<AbromalStateIconController>();
					component.Initialize(4, false, false, 1, false);
					component.transform.parent = guestButton.transform;
					component.transform.localPosition = POSITION_STATE_ICON_CONTROLLER_TRANSFORM_LOCAL;
					component.transform.localScale = SCALE_ABNOMAL_STATE_ICON_CONTROLLER_TRANSFORM_LOCAL;
					guestAbnormalStateIconController = component;
				});
			}
			else
			{
				setVisibleGuest(_enable: false);
			}

			AutoSpeedSetting autoSpeedSetting = saveDataManager.AutoSpeedSetting;
			if (!tempData.CanSkipBattle)
			{
				skipBattleButton.SetActive(_isActive: false);
				buttonLayoutGrid.Reposition();
			}

			if (!battleManager.CanAllSetMode)
				hideAllSetButton();
			if (flag)
			{
				bool replayAuto = true;
				bool replayCategory = categoryValue <= 40 && ((0x10610462000UL >> (int)categoryValue) & 1UL) != 0;
				if (replayCategory)
					replayAuto = battleStageData.ReplayClearType == eBattleClearType.AUTO_ONLY;
				SetAutoMode(replayAuto);
				Vector3 modePosition = Vector3.zero;
				if (!replayAuto)
					modePosition.y += 20000f;
				skillModeButtonEffect.transform.localPosition = modePosition;
				bool fixAuto = (battleCategory == eBattleCategory.BYWAY_QUEST || battleCategory == eBattleCategory.BYWAY_QUEST_REPLAY) && BywayQuestUtility.IsFixAutoBattle(tempData.SelectedQuestId);
				if (battleManager.CanAllSetMode)
				{
					Vector3 allSetPosition = Vector3.zero;
					if (replayCategory && !replayAuto)
					{
						bool allSet = battleStageData.ReplayClearType == eBattleClearType.ALL_SET_ONLY;
						SetAllSetMode(allSet);
						if (!allSet)
							allSetPosition.y += 20000f;
					}
					else
						allSetPosition.y += 20000f;
					allSetModeButtonEffect.transform.localPosition = allSetPosition;
					if (fixAuto)
						hideAllSetButton();
				}
				else
				{
					hideAllSetButton();
					if (fixAuto)
						battleManager.InitOnlyAutoClearFlag(false);
				}
			}
			else
			{
				SetAutoMode(false);
				SetAllSetMode(false);
				if (battleManager.IsQuestAutoProgressing())
				{
					if (allSetModeButton.gameObject.activeSelf && QuestAutoProgressUtility.GetUbSettingIndex(battleCategory) == 0)
						allSetModeOn();
					else if (skillModeButton.gameObject.activeSelf && QuestAutoProgressUtility.GetUbSettingIndex(battleCategory) == 1)
						skillModeOn();
				}
				else if (autoSpeedSetting.ButtonState == SystemSettingDefine.eTypeButtonState.ON)
				{
					if (skillModeButton.gameObject.activeSelf && autoSpeedSetting.IsAuto)
						skillModeOn();
					if (allSetModeButton.gameObject.activeSelf && autoSpeedSetting.IsAllSet)
					{
						if (saveDataManager.UseUbSetMode)
							allSetModeOn();
						else
						{
							SetAllSetMode(false);
							saveAutoSetting();
						}
					}
				}
			}

			if (gameSpeedButton.isEnabled)
			{
				setSpeedMode(_isSpeedMode: false, _isBattleSpeedQuadruple: false);
				if (battleManager.IsQuestAutoProgressing())
				{
					if (battleManager.CanQuadruple)
					{
						setSpeedMode(_isSpeedMode: true, _isBattleSpeedQuadruple: false);
					}

					onClickSpeedBtn();
				}
				else if (autoSpeedSetting.ButtonState == SystemSettingDefine.eTypeButtonState.ON && autoSpeedSetting.IsSpeedUp)
				{
					if (saveDataManager.IsBattleSpeedQuadruple && battleManager.CanQuadruple)
					{
						setSpeedMode(_isSpeedMode: true, _isBattleSpeedQuadruple: false);
					}

					onClickSpeedBtn();
				}
			}

			fullScreenFade.SetActive(_isActive: false);
			if (tempData.CanSkipBattle)
			{
				// Native FullScreenFade.Initialize only sizes/repositions its UI texture; excluded.
				viewManager.PartsBattleSkipAnime.Load();
				if (tempData.BattleSkipState == eBattleSkipState.SKIPPING)
				{
					fullScreenFade.SetActive(_isActive: true);
					ManagerSingleton<BlockLayerManager>.Instance.SetBlockButton(_isEnabled: true);
					if (battleManager.IsTowerContent)
					{
						prepareForNotFullAutoSkip();
						// Native ShowAnimation(0.2f, 0.25f, null) is a pure rendering output; omitted.
					}
				}
			}

			instance2.StartLoad(delegate
			{
				int num11 = 0;
				int neededCount = 0;
				int loadedCount = 0;
				List<UnitParameter> value = null;
				if (pUnitListDictionary.TryGetValue(myViewerId, out value))
				{
					bool mOBKHPNMEDM = false;
					int j = 0;
					for (int count2 = value.Count; j < count2; j++)
					{
						UnitParameter unitParameter3 = value[j];
						UnitCtrl unitCtrl = UnitCtrls[j - num11];
						if ((int)unitParameter3.MasterData.PrefabId == 900001)
						{
							num11++;
							unitCtrl = heroUnit;
						}

						if (unitCtrl != null)
						{
							if (BattleUtil.IsCheckSupportChara(battleCategory))
							{
								mOBKHPNMEDM = isOtherUnit(dispatchOwnerId, dispatchUnitId, unitParameter3.MasterData.UnitId);
							}

							neededCount++;
							battleManager.CreateUnitSpine(unitCtrl, BattleDefine.GetUnitRespawnPos(j), unitParameter3, delegate (UnitCtrl loadedUnit)
							{
								loadedCount++;
								if (loadedCount == neededCount)
								{
									IsUnitCreated = true;
								}
							}, mOBKHPNMEDM);
						}
					}
				}

				if (GuestUnit != null)
				{
					UnitParameter guestUnitParameter = battleStageData.GuestUnitParameter;
					neededCount++;
					battleManager.CreateUnitSpine(GuestUnit, battleStageData.GuestRespawnPos, guestUnitParameter, delegate (UnitCtrl loadedUnit)
					{
						loadedCount++;
						if (loadedCount == neededCount)
						{
							IsUnitCreated = true;
						}

						battleStageData.GuestUnitParameter = null;
					}, _isOther: true);
				}
			});
			gameSpeedButton.SetOnClickDelegate(this, onClickSpeedBtn);
			Action action = null;
			Action action2 = null;
			if (flag)
			{
				action = onClickDisableSkillModeButton;
				action2 = onClickDisableSkillModeButton;
			}
			else
			{
				action = onClickSkillModeBtn;
				action2 = onClickAllSetModeBtn;
			}

			skillModeButton.SetOnClickDelegate(this, action);
			if (allSetModeButton != null)
			{
				allSetModeButton.SetOnClickDelegate(this, action2);
			}

			// Native onBattleControllerButtonLongPressed delegates (speed/auto/all-set/skip) only open button description popups; UI input excluded.
			IsEnabledChangeAutoMode = !flag;
			if (skipBattleButton != null)
			{
				skipBattleButton.SetOnClickDelegate(this, onClickSkipBattleButton);
			}

			if (battleManager.IsLocalReplay())
			{
				// Native FullScreenFade.Initialize only sizes/repositions its UI texture; excluded.
			}

			battleDeltaTime = battleManager.DeltaTime;
			void skillModeOn()
			{
				onClickSkillModeBtn();
				battleManager.InitOnlyAutoClearFlag(false);
			}

			void allSetModeOn()
			{
				onClickAllSetModeBtn();
				battleManager.InitOnlyAutoClearFlag(true);
			}
		}

		[global::OpenPCR.Migration("38e7daa4-0a5f-423b-97c7-91571547a742")]
		private bool IsDispatchUnit(int _unitId)
		{
			switch (battleCategory)
			{
				case eBattleCategory.KAISER_BATTLE_MAIN:
					if (1 <= _unitId)
					{
						return _unitId <= (int)Singleton<UserData>.Instance.InitSettingParameter.KaiserBattleIniSetting.SupportLimit;
					}

					return false;
				case eBattleCategory.LEGION_BATTLE_MAIN:
					if (1 <= _unitId)
					{
						return _unitId <= (int)Singleton<UserData>.Instance.InitSettingParameter.LegionBattleIniSetting.SupportLimit;
					}

					return false;
				case eBattleCategory.LEGION_BATTLE_SUB:
				case eBattleCategory.MIROKU_BATTLE:
					if (1 <= _unitId)
					{
						return _unitId <= (int)Singleton<UserData>.Instance.InitSettingParameter.StoryRaidEventBattleIniSetting.SupportLimit;
					}

					return false;
				case eBattleCategory.SEVEN_SCENARIO_BOSS:
				case eBattleCategory.MRE_BOSS:
					return unchecked((uint)(_unitId - 1)) < 5U;
				default:
					return 1 == _unitId;
			}
		}

		[global::OpenPCR.Migration("d73590a1-f186-4edd-8457-17a3942025aa")]
		private void setVisibleObject(int _index, bool _enable)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("c889e686-7f04-40ab-afdb-894419106837")]
		private void setVisibleGuest(bool _enable)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("77925554-cfe7-4c09-b46c-122daedb29e8")]
		public void _Update(bool _canUpdateTime)
		{
			if (!IsUnitCreated || IsDisableUpdate || battleManager == null)
			{
				return;
			}

			updatePlayerSkill();
			updateEnemySkill();
			if (skillExeUnit != null)
			{
				UnitCtrl unitCtrl = skillExeUnit;
				if (unitCtrl.IsOther)
				{
					unitCtrl.ComponentSortOrder.SetSortOrderFront();
				}

				unitCtrl.CutInFrameSet.CutInFrame = battleManager.FrameCount + 1;
				unitCtrl.CutInFrameSet.ServerCutInFrame = battleManager.FrameCount + 1;
				unitCtrl.SkillEnableFrame = 0;
				skillExeUnit = null;
			}
		}

		[global::OpenPCR.Migration("50089311-6b1a-4c17-b58a-00144f836f00")]
		public void ReallocateUnitCtrl(int _index, int _unitId, int _skinRarity, UnitCtrl _unitCtrl, bool _isDefenceMode = false)
		{
			var unitData = ManagerSingleton<MasterDataManager>.Instance.masterUnitData.Get(UnitUtility.GetUnitResourceId(_unitId));
			int cutinId = _skinRarity >= 6 ? (int)unitData.cutin1_star6 : (int)unitData.CutIn1;
			if (_isDefenceMode)
				setUpUiCallbacks(_index, _unitCtrl, cutinId);
			else
			{
				UnitCtrls[_index] = null;
				setOnInitCallback(_index, _unitCtrl, cutinId);
			}
		}

		[global::OpenPCR.Migration("3ea259af-f17e-42c3-b9e7-aa002f3d029b")]
		public void SortUnitCtrlsByAliveness()
		{
			List<UnitCtrl> list = new List<UnitCtrl>();
			List<UnitCtrl> list2 = new List<UnitCtrl>();
			for (int i = 0; i < UnitCtrls.Length; i++)
			{
				UnitCtrl unitCtrl = UnitCtrls[i];
				if (!(unitCtrl == null))
				{
					if ((long)unitCtrl.ComponentParameter.Hp > 0)
					{
						list2.Add(unitCtrl);
					}
					else
					{
						list.Add(unitCtrl);
					}
				}
			}

			list2.AddRange(list);
			for (int j = 0; j < list2.Count; j++)
			{
				UnitCtrl unitCtrl2 = list2[j];
				UnitCtrls[j] = unitCtrl2;
			}
		}

		[global::OpenPCR.Migration("9dfd2fb1-6fb5-438f-93d4-46852561ac8d")]
		private void saveAutoSetting()
		{
			AutoSpeedSetting setting = saveDataManager.AutoSpeedSetting;
			setting.IsAuto = IsAutoMode;
			if (battleManager.CanAllSetMode)
				setting.IsAllSet = IsAllSetMode && saveDataManager.UseUbSetMode;
			saveDataManager.AutoSpeedSetting = setting;
			saveDataManager.Save();
			if (setting.ButtonState == SystemSettingDefine.eTypeButtonState.OFF)
				battleManager.DisableAutoClear();
		}

		[global::OpenPCR.Migration("af03b259-9ca8-4494-86d2-5c5f4e4131a9")]
		public void SetAutoMode(bool _isAutoMode)
		{
			setAutoModeFlag(_isAutoMode);
			// Native UI normal/background sprite and effect position outputs are intentionally excluded.
		}

		[global::OpenPCR.Migration("419b3007-96a2-49de-a032-86695c790cef")]
		private void setAutoModeFlag(bool _isAutoMode)
		{
			IsAutoMode = _isAutoMode;
		}

		private void setAutoModeFlagBackup()
		{
			autoModeFlagBackup = IsAutoMode;
			allSetModeFlagBackup = IsAllSetMode;
		}

		private void setSpeedModeSettingsBackup()
		{
			speedModeFlagBackup = isSpeedMode;
			speedQuadrupleFlagBackup = isBattleSpeedQuadruple;
		}

		[global::OpenPCR.Migration("7a751b3c-ffc5-4b58-acc2-23933dcebeda")]
		public void HideSettingButtons()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("8313a25e-2328-4ba6-9ba6-f10f8119d802")]
		public void HidePopUp()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("cdf64e95-f90c-42fd-91d5-6cb91f2b7d98")]
		public void UpdateAllEnemyHp0()
		{
			AllEnemyHp0 = true;
			if (battleManager == null)
			{
				return;
			}

			List<UnitCtrl> enemyList = battleManager.EnemyList;
			if (enemyList == null)
			{
				return;
			}

			for (int i = 0; i < enemyList.Count; i++)
			{
				UnitCtrl unitCtrl = enemyList[i];
				if (!(unitCtrl == null) && (long)unitCtrl.ComponentParameter.Hp > 0)
				{
					AllEnemyHp0 = false;
					break;
				}
			}
		}

		private void setGuestIcon(bool _isEnable)
		{
			guestUnitIconPnl.color = (_isEnable ? ENABLE_COLOR : DESABLE_COLOR);
			if (!_isEnable)
			{
				guestFrameSprite.color = LIGHT_DESABLE_COLOR;
			}
		}

		[global::OpenPCR.Migration("c8210af8-2b13-4d78-beae-32a428892eff")]
		private void updatePlayerSkill()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("a81d4239-3fd2-4a81-b2ab-34bcea9a52e9")]
		private void updateEnemySkill()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("b425ebe1-3d7e-4ffc-bd85-2df3ceb6e96f")]
		private void performTapPanelAction(int _unitNo, bool _isPlayer, bool _isAuto)
		{
			if (battleManager.IsGuestCutinPlaying || battleManager.IsTalentBonusEffectPlaying)
			{
				return;
			}

			UnitCtrl unitCtrl = (_isPlayer ? UnitCtrls[_unitNo] : battleManager.EnemyList[_unitNo]);
			if (battleCategory == eBattleCategory.TUTORIAL)
			{
				if (!skillReadys[_unitNo])
				{
					return;
				}

				if (ManagerSingleton<DialogManager>.Instance.ForceCloseOne(eDialogId.FOCUS_OBJECT))
				{
					battleManager.GamePause(pause: false, _isBlackoutEnd: false);
				}
			}

			unitCtrl.ComponentUnionBurst.SetIsUbExecTrying(true);
		}

		[global::OpenPCR.Migration("36551f24-c8e0-4fd2-a35d-a42c6b2019e1")]
		private void performTapGuestPanelAction()
		{
			if (!battleManager.IsGuestCutinPlaying && !battleManager.IsTalentBonusEffectPlaying)
			{
				GuestUnit.ComponentUnionBurst.SetIsUbExecTrying(true);
			}
		}

		[global::OpenPCR.Migration("271d0e67-e9ca-4828-a17b-c420f31d6b87")]
		private void onDie(int _index)
		{
			unitDeads[_index] = true;
			ResetSkillReserve(UnitCtrls[_index], _isDead: true);
			// Gauge, icon and abnormal-icon visibility only render the death; reservation
			// and slot-dead state above have real consumers and must remain live.
		}

		private void onGuestDie()
		{
			guestUnitDead = true;
			ResetSkillReserve(GuestUnit, _isDead: true);
			guestSkillGaugeSlider.value = 0f;
			setGuestIcon(_isEnable: false);
		}

		private void onGuestChangeState(eStateIconType _iconType, bool _enable)
		{
			if (guestIsUnit && !(guestUnitDead & _enable))
			{
				if (_enable)
				{
					guestAbnormalStateIconController.AddIcon(_iconType);
				}
				else
				{
					guestAbnormalStateIconController.DeleteIcon(_iconType);
				}
			}
		}

		private void onGuestChangeStateNum(eStateIconType _iconType, int _num)
		{
			if (guestIsUnit && !guestUnitDead)
			{
				guestAbnormalStateIconController.SetStateNum(_iconType, _num);
			}
		}

		private void onGuestChangeStatePassiveEnable(eStateIconType _iconType, bool _enable)
		{
			if (guestIsUnit && !guestUnitDead)
			{
				guestAbnormalStateIconController.SetStatePassiveEnable(_iconType, _enable);
			}
		}

		[global::OpenPCR.Migration("fcdbe012-5809-4cfa-a7c1-48c5b17c7fc7")]
		private void onClickSkillModeBtn()
		{
			if (EnableTap)
			{
				battleManager.DisableAutoClear();
				SetAutoMode(!IsAutoMode);
				saveAutoSetting();
			}
		}

		[global::OpenPCR.Migration("0519727f-667d-4a2b-abff-aa8b480889f7")]
		private void onClickSpeedBtn()
		{
			if (!EnableTap)
			{
				return;
			}

			if (battleManager.IsPrincessSkillRunning)
			{
				viewManager.OpenNoticeWindow(eTextId.DISABLE_SPEED_BUTTON_P_SKILL);
				return;
			}

			if (battleManager.CanQuadruple && isSpeedMode && !isBattleSpeedQuadruple)
			{
				setSpeedMode(_isSpeedMode: true, _isBattleSpeedQuadruple: true);
			}
			else
			{
				setSpeedMode(!isSpeedMode, _isBattleSpeedQuadruple: false);
			}

			saveSpeedSetting();
		}

		private void saveSpeedSetting()
		{
			AutoSpeedSetting autoSpeedSetting = saveDataManager.AutoSpeedSetting;
			autoSpeedSetting.IsSpeedUp = isSpeedMode;
			saveDataManager.AutoSpeedSetting = autoSpeedSetting;
			if (battleManager.CanQuadruple || !isSpeedMode)
			{
				saveDataManager.IsBattleSpeedQuadruple = isBattleSpeedQuadruple;
			}
		}

		[global::OpenPCR.Migration("0b9e9cdc-c165-40b6-a2d8-cefc8afcd819")]
		private void setSpeedMode(bool _isSpeedMode, bool _isBattleSpeedQuadruple)
		{
			setSpeedModeSettings(_isSpeedMode, _isBattleSpeedQuadruple);
			// Native UI speed sprites and effect position outputs are intentionally excluded.
		}

		private void setSpeedModeSettings(bool _isSpeedMode, bool _isBattleSpeedQuadruple)
		{
			isSpeedMode = _isSpeedMode;
			isBattleSpeedQuadruple = _isBattleSpeedQuadruple;
			battleTimeScale.IsSpeedQuadruple = _isBattleSpeedQuadruple;
			battleTimeScale.SpeedupFlag = isSpeedMode;
		}

		[global::OpenPCR.Migration("d6f65b98-c9c8-45ad-a8bc-5d65f5ca366d")]
		public void prepareForNotFullAutoSkip()
		{
			if (battleCategory == eBattleCategory.TOWER_CLOISTER)
			{
				setSpeedModeSettingsBackup();
				setSpeedModeSettings(_isSpeedMode: false, _isBattleSpeedQuadruple: false);
			}

			if (IsAllSetMode)
				battleManager.DisableAutoClear();
			setAutoModeFlagBackup();
			setAllSetModeFlag(false);
			setAutoModeFlag(_isAutoMode: true);
			SkillReserves.Reset();
		}

		[global::OpenPCR.Migration("55a3b116-0e53-4999-bcac-cba296460f61")]
		private void setVisibleSkillFrame(int _index, bool _isVisible)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("96658b4b-bfe1-4b42-a15e-5c953d584d1b")]
		private void setGuestVisibleSkillFrame(bool _isVisible)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("f37ba4bb-d2a6-4ca4-beab-4510a8407e5a")]
		private void setVisibleSkillReserveFrame(int _index, bool _isVisible)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("4ef4c986-da2a-4764-9d8a-ffb86cabe493")]
		private void setGuestVisibleSkillReserveFrame(bool _isVisible)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("9eaee72e-7a7a-42e6-932e-51b471348c3a")]
		private void turnSkillReady(int _index, bool _isActive)
		{
			if (_isActive != skillReadys[_index])
			{
				setVisibleSkillFrame(_index, _isActive);
				skillReadys[_index] = _isActive;
			}
		}

		[global::OpenPCR.Migration("6273d8dc-f086-4893-a6c2-0492470fd81c")]
		private void turnSkillReadyGuest(bool _isActive)
		{
			if (_isActive != guestSkillReady)
			{
				setGuestVisibleSkillFrame(_isActive);
				guestSkillReady = _isActive;
			}
		}

		[global::OpenPCR.Migration("f655f165-de0f-43b6-91cf-15941a8e4e3e")]
		public void UpdateUi()
		{
			updatePlayerSkill();
			updateEnemySkill();
		}

		[global::OpenPCR.Migration("2f297ea6-fb8a-4bf6-807a-0d235c580a58")]
		public void ResetSkillReserve(UnitCtrl _unit, bool _isDead = false)
		{
			if (battleManager.CanAllSetMode && saveDataManager.UseUbSetMode && ((saveDataManager.ResetSkillReserveEveryTime && !battleManager.IsReplayContents) || _isDead))
			{
				if (_unit.IsGuest)
					SetDispGuestSkillReserve(false);
				else
					SetDispSkillReserve(_unit, false);
				if (_isDead)
					return;
				if (IsAllSetMode)
					battleManager.DisableAutoClear();
				UpdateAllSetButton();
			}
		}

		[global::OpenPCR.Migration("cc51661f-873a-44e1-923d-183cdf8d85af")]
		public void PlaySkillOn(UnitCtrl _unit)
		{ global::OpenPCR.Presentation.Noop(); }

		[global::OpenPCR.Migration("3497bdaf-c890-453c-aeec-bee98394dc92")]
		private void controllingSkillReserveAnimation(int _iconIndex, bool _isPlay)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("00228979-900b-42bc-86fa-fbe66a7566b1")]
		private void controllingGuestSkillReserveAnimation(bool _isPlay)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("66cbdb8a-e1a1-406d-9aa0-7d01ff2085de")]
		public void PlaySkillReserveExecuteEffect(UnitCtrl _unit)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("fbe5d562-c06c-40d1-ab76-868740ff74b0")]
		public void ExecReserveSKill(UnitCtrl _unit)
		{
			if (!_unit.IsGuest)
			{
				if (SkillReserves[_unit.UnitId])
				{
					int unitNo = Array.IndexOf(UnitCtrls, _unit);
					performTapPanelAction(unitNo, _isPlayer: true, _isAuto: false);
				}
			}
			else if (ReservedGuestSkill)
			{
				performTapGuestPanelAction();
			}
		}

		public bool IsAllSetMode { get; private set; }

		public Transform TalentBonusIconParentTransform
		{
			[global::OpenPCR.Migration("1c054fe6-9395-48e7-8f0b-3e30c0307f76")]
			get
			{
				return talentBonusIconParentTransform;
			}
		}

		[global::OpenPCR.Migration("806b9691-9c08-4d9d-87ff-21cf97c3a642")]
		public void SetTalentFormationBonusActive(bool _isActive)
		{
			talentBonusObject.SetActiveWithCheck(_isActive);
		}

		private GameObject talentBonusObject;
		[SerializeField]
		private Transform talentBonusIconParentTransform;
		private bool allSetModeFlagBackup;
		private Coroutine syncSkillReserveAnimationTimeCoroutine;
		public bool ReservedGuestSkill { get; private set; }

		[global::OpenPCR.Migration("d36defd2-4bcd-46ae-8291-38900b230fee")]
		public void SetDispSkillReserve(UnitCtrl _unit, bool _isActive)
		{
			if (SkillReserves[_unit.UnitId] != _isActive)
			{
				SkillReserves[_unit.UnitId] = _isActive;
				// Frame visibility, Animator playback and the real-time synchronization
				// coroutine only render reservation state; no battle clock/RNG/callback consumes them.
			}
		}

		[global::OpenPCR.Migration("ab535e43-5d67-46c0-802c-8809568eb747")]
		public void SetDispGuestSkillReserve(bool _isActive)
		{
			if (ReservedGuestSkill != _isActive)
			{
				setGuestVisibleSkillReserveFrame(_isActive);
				controllingGuestSkillReserveAnimation(_isActive);
				ReservedGuestSkill = _isActive;
				if (_isActive && syncSkillReserveAnimationTimeCoroutine == null)
					syncSkillReserveAnimationTimeCoroutine = StartCoroutine(syncSkillReserveAnimationTime());
			}
		}

		private float getSkillReserveAnimationNormalizedTime()
		{
			if (!battleManager.CanReserveSkill || !saveDataManager.UseUbSetMode)
				return 0f;
			for (int i = 0; i < 5; i++)
			{
				var unit = UnitCtrls[i];
				if (unit != null && SkillReserves[unit.UnitId])
					return skillReserveAnimations[i].GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
			}

			return ReservedGuestSkill ? guestSkillReserveAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f : 0f;
		}

		private IEnumerator syncSkillReserveAnimationTime()
		{
			yield return new WaitForSecondsRealtime(2f);
			if (battleManager.CanReserveSkill && saveDataManager.UseUbSetMode)
			{
				float baseTime = 0f;
				for (int i = 0; i < 5; i++)
				{
					var unit = UnitCtrls[i];
					if (unit == null || !SkillReserves[unit.UnitId])
						continue;
					float time = skillReserveAnimations[i].GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
					if (time != 0f)
					{
						float quantized = Mathf.Floor(time * 100f) / 100f;
						if (baseTime == 0f)
							baseTime = quantized;
						else if (Mathf.Abs(baseTime - quantized) > 0.05f)
							controllingSkillReserveAnimation(i, true);
					}
				}

				if (ReservedGuestSkill)
				{
					float time = Mathf.Floor((guestSkillReserveAnimation.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f) * 100f) / 100f;
					if (time != 0f && baseTime != 0f && Mathf.Abs(baseTime - time) > 0.05f)
						controllingGuestSkillReserveAnimation(true);
				}
			}

			syncSkillReserveAnimationTimeCoroutine = null;
		}

		private static readonly string SMART_AUTO_BTN_ON_SPRITE_NAME = "battle_btn_main_allset";
		private static readonly string SMART_AUTO_BTN_OFF_SPRITE_NAME = "battle_btn_sub_allset";
		[SerializeField]
		private CustomUIButton allSetModeButton;
		[SerializeField]
		private UISprite allSetModeButtonBg;
		[SerializeField]
		private GameObject allSetModeButtonEffect;
		[global::OpenPCR.Migration("c3bad7cb-ca19-4015-a76f-595f526d3986")]
		private void setAllSetModeFlag(bool _isAllSetMode)
		{
			IsAllSetMode = _isAllSetMode;
		}

		[global::OpenPCR.Migration("39f52688-8f10-4c34-b32b-72009ec0555d")]
		public void SetAllSetMode(bool _isAutoMode)
		{
			setAllSetModeFlag(_isAutoMode);
			// Native UI normal/background sprite and effect position outputs are intentionally excluded.
		}

		[global::OpenPCR.Migration("17da69a3-dad1-46a1-a872-621e9cf88a7e")]
		private bool canChangeReserveSkillStatus(int _index, bool _isGuest, bool _isAllSet = false)
		{
			if (!(_isAllSet ? battleManager.CanAllSetMode : battleManager.CanReserveSkill) || !saveDataManager.UseUbSetMode)
				return false;
			if (battleManager.TimeLimit <= 0f && battleManager.GameState == eBattleGameState.WAIT_WAVE_END)
				return false;
			if (_isGuest)
				return !guestUnitDead && (_isAllSet || !guestSkillReady);
			return !unitDeads[_index] && (_isAllSet || !skillReadys[_index]);
		}

		[global::OpenPCR.Migration("2e30dfd5-c35d-46b8-8ac0-93710d98f5a8")]
		public void AllSetSkillReserve()
		{
			if (!battleManager.CanAllSetMode)
				return;
			if (!saveDataManager.UseUbSetMode)
			{
				saveDataManager.UseUbSetMode = true;
				saveDataManager.Save();
				SingletonMonoBehaviour<BattleHeaderController>.Instance.SetSkillReserveLabel(true);
			}

			for (int i = 0; i < 5; i++)
			{
				var unit = UnitCtrls[i];
				if (unit == null || unit.UnitId == 0)
					break;
				if (canChangeReserveSkillStatus(i, false, true))
				{
					if (skillReadys[i])
						turnSkillReady(i, false);
					SetDispSkillReserve(unit, true);
				}
			}

			if (guestIsUnit && canChangeReserveSkillStatus(0, true, true))
			{
				if (guestSkillReady)
					turnSkillReadyGuest(false);
				SetDispGuestSkillReserve(true);
			}
		}

		[global::OpenPCR.Migration("5b66e697-9a8c-4163-bbfa-efa8d4d2d48d")]
		public void UpdateAllSetButton()
		{
			if (!battleManager.CanReserveSkill)
				return;
			bool hasUnreserved = false;
			for (int i = 0; i < 5; i++)
			{
				var unit = UnitCtrls[i];
				if (unit == null || unit.UnitId == 0)
					break;
				if (!unitDeads[i] && !SkillReserves[unit.UnitId])
				{
					if (!IsAllSetMode)
						return;
					hasUnreserved = true;
					break;
				}
			}

			if (guestIsUnit && !ReservedGuestSkill)
			{
				if (!IsAllSetMode)
					return;
			}
			else if (IsAllSetMode && !hasUnreserved)
				return;
			battleManager.DisableAutoClear();
			bool previous = IsAllSetMode;
			if (!previous)
				AllSetSkillReserve();
			SetAllSetMode(!previous);
			saveAutoSetting();
		}

		[global::OpenPCR.Migration("b02bd1cb-1807-4c67-9c81-bf87bec5cc85")]
		private void setOnInitCallback(int _index, UnitCtrl _unitController, int _cutinId)
		{
			_unitController.OnInitCallbackForUi = unit => setUpUiCallbacks(_index, unit, _cutinId);
		}

		[global::OpenPCR.Migration("be1aec93-18f3-4dac-8d78-84626841e648")]
		private void setUpUiCallbacks(int _index, UnitCtrl _unit, int _cutinId)
		{
			// Retain icon subscription presence: AutoRevivalData invokes these delegates.
			// Their callbacks only render icon state/count; no battle state is written.
			_unit.ComponentAbnormal.OnChangeState += (_, _) => { };
			_unit.ComponentAbnormal.OnChangeStatePassiveEnable += (_, _) => { };
			_unit.ComponentAbnormal.OnChangeStateNum += (_, _) => { };
			_unit.ComponentState.OnDieForZeroHp += unit => onDie(_index);
			// Energy slider and icon-shake callbacks are rendering only.
			_unit.ComponentUnionBurst.MovieId = _cutinId;
		}

		[global::OpenPCR.Migration("e7ce24c1-4713-4835-bdeb-e0ed3f7732c5")]
		private bool isOtherUnit(Dictionary<int, long> _dicViewId, Dictionary<int, int> _dicUnitId, int _unitId)
		{
			int originalId = UnitUtility.GetOriginalUnitId(_unitId);
			uint category = (uint)battleCategory;
			if (category <= 35 && ((0x826000000UL >> (int)category) & 1UL) != 0)
				return _dicViewId.Count >= 1 && _dicUnitId.ContainsValue(originalId);
			if (_dicViewId.Count == 0 || _dicUnitId.Count == 0)
				return false;
			return _dicViewId[1] != 0L && _dicUnitId[1] == originalId;
		}

		[global::OpenPCR.Migration("2164cf5b-cfa3-425b-add4-e1ccb02c8b45")]
		private void hideAllSetButton()
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("a51be825-31cb-46e5-be41-4d8fbc202f37")]
		private void setupGuestUiCallback(UnitCtrl _unitController, int _cutinId)
		{
			Action<float> energyCallback = null;
			Action<eStateIconType, bool> stateCallback = null;
			Action<eStateIconType, int> numCallback = null;
			Action<eStateIconType, bool> passiveCallback = null;
			Action<UnitCtrl> dieCallback = null;
			_unitController.OnInitCallbackForUi = unit =>
			{
				unit.ComponentParameter.OnEnergyChange += energyCallback ??= amount => onGuestEnergyChange(amount);
				unit.ComponentAbnormal.OnChangeState += stateCallback ??= (type, enable) => onGuestChangeState(type, enable);
				unit.ComponentAbnormal.OnChangeStateNum += numCallback ??= (type, num) => onGuestChangeStateNum(type, num);
				unit.ComponentAbnormal.OnChangeStatePassiveEnable += passiveCallback ??= (type, enable) => onGuestChangeStatePassiveEnable(type, enable);
				if (!battleManager.IsDefenseReplayMode)
					unit.ComponentState.OnDieForZeroHp += dieCallback ??= dead => onGuestDie();
				// Native damage shake callback is a pure Animator.Play output and is intentionally excluded.
				unit.ComponentUnionBurst.MovieId = _cutinId;
			};
		}

		private void onGuestEnergyChange(float _energyAmount)
		{
			if (guestIsUnit && !guestUnitDead)
				guestSkillGaugeSlider.value = _energyAmount;
		}

		[global::OpenPCR.Migration("2c82daca-9890-45fc-8f0d-c600856fdad5")]
		private void onClickAllSetModeBtn()
		{
			if (!EnableTap)
				return;
			battleManager.DisableAutoClear();
			if (IsAllSetMode)
				AllResetSkillReserve();
			else
				AllSetSkillReserve();
			SetAllSetMode(!IsAllSetMode);
			saveAutoSetting();
		}

		[global::OpenPCR.Migration("30b88422-1d01-4272-b0bc-06ec9b28f27c")]
		public void AllResetSkillReserve()
		{
			for (int i = 0; i < 5; i++)
			{
				UnitCtrl unit = UnitCtrls[i];
				if (unit == null || unit.UnitId == 0)
					break;
				SetDispSkillReserve(unit, false);
			}

			if (guestIsUnit)
				SetDispGuestSkillReserve(false);
		}

		[global::OpenPCR.Migration("586db84c-a4b3-4ad9-9eb1-030ba43ecda3")]
		private void onTapPanel(int _unitNo, bool _isPlayer, bool _isAuto)
		{
			if (battleManager.CanReserveSkill)
			{
				if (canChangeReserveSkillStatus(_unitNo, false, false))
				{
					UnitCtrl unit = UnitCtrls[_unitNo];
					SetDispSkillReserve(unit, !SkillReserves[unit.UnitId]);
					// Native audio-only PlaySe output is intentionally excluded.
					UpdateAllSetButton();
				}
				// Native unsuccessful-reservation PlaySe output is intentionally excluded.
			}

			performTapPanelAction(_unitNo, _isPlayer, _isAuto);
		}

		[global::OpenPCR.Migration("02ed96fe-1bc2-4a37-bb58-58a87d5b396a")]
		private void onTapGuestPanel()
		{
			if (battleManager.CanReserveSkill)
			{
				if (!battleManager.IsDefenseBattle && canChangeReserveSkillStatus(0, true, false))
				{
					SetDispGuestSkillReserve(!ReservedGuestSkill);
					// Native audio-only PlaySe output is intentionally excluded.
					UpdateAllSetButton();
				}
			}

			performTapGuestPanelAction();
		}

		[global::OpenPCR.Migration("a94d7d56-6801-47bc-be43-5ddc12fbb6e4")]
		private void onClickSkipBattleButton()
		{
			if (!EnableTap)
				return;
			battleManager.ExecuteSkipProcess(() =>
			{
				int category = (int)battleCategory;
				if (category > 8 || ((1 << category) & 0x198) == 0)
					prepareForNotFullAutoSkip();
				// Native audio-only PlaySe output is intentionally excluded.
				executeSkip();
			});
		}

		private void executeSkip()
		{
			Singleton<TempData>.Instance.BattleSkipState = eBattleSkipState.PREPARING_TO_SKIP;
			fullScreenFade.SetActive(true);
			float duration = fullScreenFade.Duration + 0.2f;
			battleManager.FadeOutAllVoices(duration);
			battleManager.FadeOutAllSe(duration);
			Singleton<BlockLayerManager>.Instance.SetBlockButton(true);
			// Native ShowAnimation(0.2f, 0f, null) has no completion callback; pure rendering is excluded.
			fullScreenFade.FadeOut(true, () => StartCoroutine(startSkipping()));
		}

		private IEnumerator startSkipping()
		{
			while (battleManager.IsPrincessSkillMovieRunning || battleManager.GetIsPlayCutin() || battleManager.IsPlayingNoCutInMotionForPSkill || battleManager.IsPlayingNoCutInMotion)
				yield return null;
			Singleton<TempData>.Instance.BattleSkipState = eBattleSkipState.SKIPPING;
		}

		[global::OpenPCR.Migration("b225f87b-55ac-452c-aa97-b064ffeb9641")]
		private void onClickPauseButtonDisable()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.UnitUiCtrl::onClickPauseButtonDisable():System.Void");
		}

		[global::OpenPCR.Migration("0938dfee-3df3-4508-b9a0-5b5a618537f1")]
		private void onClickDisableSkillModeButton()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.UnitUiCtrl::onClickDisableSkillModeButton():System.Void");
		}

		[global::OpenPCR.Migration("75e537d1-30ff-4fab-8eed-b692f28f68a5")]
		public void SetTalentFormationBonus(Dictionary<int, long> _talentFormationBonusDic)
		{
			global::OpenPCR.Presentation.Noop();
		}
	}
}
