using global::Elements;
using global::OpenPCR.AnimKit;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using global::OpenPCR.Foundation;
using Elements.Battle;
using Elements.Battle.Core;
using Elements.Data;

namespace Elements
{
	public class UnitActionController : MonoBehaviour, ISingletonField
	{
		private enum eRefrexiveType
		{
			REFLEXIVE_NORMAL = 1,
			REFLEXIVE_ABSOLUTE_SEARCH,
			REFLEXIVE_ABSOLUTE_POSITION,
			REFLEXIVE_NORMAL_NOT_CONSIDER_BODY_WIDTH
		}

		[global::OpenPCR.Migration("c1f71186-9b8e-4d01-bf12-215e0b6b587a")] public new FixedTransformMonoBehavior.FixedTransform transform;
		public ActionParameterOnPrefabDetail AttackDetail;
		public bool UseDefaultDelay = true;
		public Skill Attack;
		public List<Skill> UnionBurstList;
		public List<Skill> MainSkillList;
		public List<Skill> SpecialSkillList;
		public List<Skill> SpecialSkillEvolutionList;
		public List<Skill> UnionBurstEvolutionList;
		public List<Skill> MainSkillEvolutionList;
		public List<Skill> SubUnionBurstList;
		public Skill Annihilation;
		private static Yggdrasil<UnitActionController> staticSingletonTree;
		private static IBattleManagerForUnitActionController staticBattleManager;
		private static IBattleCameraEffectForUnitActionController staticBattleCameraEffect;
		private static IBattleEffectPool staticBattleEffectPool;
		private static IBattleTimeScaleForUnitActionController staticBattleTimeScale;
		public UnitCtrl Owner { get; set; }
		public bool UnionBurstIsChargeTime { get; set; }
		public bool UnionBurstCharging { get; set; }
		public bool DisableUBByModeChange { get; set; }
		public bool ModeChanging { get; set; }
		public bool StopModeChangeEndEffectCalled { get; set; }
		public bool IsCancelModeChangeByTrigger { get; set; }
		public bool MoveEnd { get; set; }
		private Dictionary<int, Skill> skillDictionary { get; set; }
		public bool ContinuousActionEndDone { get; set; }
		private bool isUnionBurstOnlyOwner { get; set; }
		private bool updateBranchMotionRunning { get; set; }
		[global::OpenPCR.ImplicitMigration("3d0836f0-1380-4ec4-85c3-5c0d0f01f03a")][global::OpenPCR.Migration("c50ee2f3-7ad8-436d-87f6-6037987e0fc4")] private IBattleManagerForUnitActionController battleManager => staticBattleManager;
		[global::OpenPCR.ImplicitMigration("4d5b24a7-9115-4d45-a1b7-bd6777dd59f5")][global::OpenPCR.Migration("1cd47414-110b-4bee-91e3-9f01f4cdafc2")] private IBattleCameraEffectForUnitActionController battleCameraEffect => staticBattleCameraEffect;
		[global::OpenPCR.ImplicitMigration("bb80bca6-b56a-4cd2-868f-511a253833cc")][global::OpenPCR.Migration("7dc03bc2-2f40-4dd3-9865-07ccdc590909")] private IBattleEffectPool battleEffectPool => staticBattleEffectPool;
		[global::OpenPCR.ImplicitMigration("9fcf3172-80a1-4b66-99d2-daca0d988e2c")][global::OpenPCR.Migration("fec9fb3e-b9e5-4b8b-93d8-020d7f4db427")] private IBattleTimeScaleForUnitActionController battleTimeScale => staticBattleTimeScale;

		[global::OpenPCR.Migration("59383308-c56e-44ce-81df-32f8e509d2a1")]
		public UnitActionController()
		{
		}

		[global::OpenPCR.Migration("7fda75e5-ac7c-4ef9-a3a3-b49addf7742f")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
			staticBattleCameraEffect = null;
			staticBattleEffectPool = null;
			staticBattleTimeScale = null;
		}

		[global::OpenPCR.Migration("632dc365-a3c4-47ad-ab8a-308640abf0f8")]
		private void OnDestroy()
		{
			AttackDetail = null;
			Attack = null;
			UnionBurstList = null;
			MainSkillList = null;
			SpecialSkillList = null;
			SpecialSkillEvolutionList = null;
			Owner = null;
			skillDictionary = null;
		}

		[global::OpenPCR.Migration("5d69e3ea-ada5-42f7-9ed2-50727d502def")]
		public void Initialize(UnitCtrl _owner, UnitParameter _unitParameter, bool _initializeAttackOnly = false, UnitCtrl _seOwner = null)
		{
			if (staticSingletonTree == null)
			{
				staticSingletonTree = this.CreateSingletonTree();
				staticBattleManager = staticSingletonTree.Get<BattleManager>();
				staticBattleCameraEffect = staticSingletonTree.Get<BattleCameraEffect>();
				staticBattleEffectPool = staticSingletonTree.Get<BattleEffectPool>();
				staticBattleTimeScale = staticSingletonTree.Get<BattleTimeScale>();
			}

			Owner = _owner;
			transform = _owner.transform;
			skillDictionary = new Dictionary<int, Skill>();
			Attack.AnimId = eAnimKitCharacterAnimeId.ATTACK;
			Attack.WeaponType = (_initializeAttackOnly ? _seOwner.WeaponSeType : Owner.WeaponSeType);
			Attack.skillAreaWidth = Owner.ComponentSearchArea.SearchAreaSize;
			Attack.SkillId = 1;
			AttackAction attackAction = new AttackAction();
			attackAction.Initialize();
			attackAction.TargetAssignment = eTargetAssignment.OTHER_SIDE;
			attackAction.TargetSort = PriorityPattern.NEAR;
			attackAction.TargetNth = 0;
			attackAction.TargetNum = 1;
			attackAction.TargetWidth = Owner.ComponentSearchArea.SearchAreaSize;
			attackAction.Direction = DirectionType.FRONT;
			attackAction.ActionType = eActionType.ATTACK;
			attackAction.Value = new Dictionary<eValueNumber, double>(new eValueNumber_DictComparer())
			{
				{
					eValueNumber.VALUE_1,
					0f
				},
				{
					eValueNumber.VALUE_3,
					1f
				}
			};
			attackAction.ActionDetail1 = _owner.AtkType;
			float value = 0f;
			if (UseDefaultDelay && BattleDefine.WEAPON_HIT_DELAY_DIC.TryGetValue(_initializeAttackOnly ? _seOwner.WeaponMotionType : Owner.WeaponMotionType, out value))
			{
				attackAction.ActionExecTimeList = new List<ActionExecTime>
				{
					new ActionExecTime
					{
						Weight = 1f,
						DamageNumType = eDamageEffectType.NORMAL
					}
				};
				attackAction.ExecTime = new float[1]
				{
					value
				};
				attackAction.ActionWeightSum = 1f;
			}
			else
			{
				ActionParameterOnPrefabDetail attackDetail = AttackDetail;
				attackDetail.ExecTime = new List<ActionExecTime>(attackDetail.ExecTimeForPrefab);
				int i = 0;
				for (int count = attackDetail.ExecTimeCombo.Count; i < count; i++)
				{
					ActionExecTimeCombo actionExecTimeCombo = attackDetail.ExecTimeCombo[i];
					for (int j = 0; j < actionExecTimeCombo.Count; j++)
					{
						switch (actionExecTimeCombo.InterporationType)
						{
							case eComboInterporationType.LINEAR:
								attackDetail.ExecTime.Add(new ActionExecTime { Time = actionExecTimeCombo.StartTime + (float)j * actionExecTimeCombo.OffsetTime, DamageNumType = eDamageEffectType.COMBO });
								break;
							case eComboInterporationType.CURVE:
								attackDetail.ExecTime.Add(new ActionExecTime { Time = actionExecTimeCombo.StartTime + actionExecTimeCombo.Curve.Evaluate((float)j / (float)actionExecTimeCombo.Count) * (float)actionExecTimeCombo.Count * actionExecTimeCombo.OffsetTime, DamageNumType = eDamageEffectType.COMBO });
								break;
						}
					}
				}

				attackDetail.ExecTime.Sort((ActionExecTime a, ActionExecTime b) => a.Time.CompareTo(b.Time));
				attackAction.ActionExecTimeList = attackDetail.ExecTime;
				attackAction.ExecTime = new float[attackDetail.ExecTime.Count];
				for (int num = 0; num < attackDetail.ExecTime.Count; num++)
				{
					attackAction.ActionWeightSum += attackDetail.ExecTime[num].Weight;
					attackAction.ExecTime[num] = attackDetail.ExecTime[num].Time;
				}
			}

			attackAction.ActionChildrenIndexes = new List<int>();
			attackAction.ActionId = UnitUtility.GetCharaId(_owner.UnitId) * 10000;
			attackAction.TargetList = new List<BasePartsData>();
			Attack.ActionParameters = new List<ActionParameter>();
			Attack.ActionParameters.Add(attackAction);
			skillDictionary.Add(1, Attack);
			DependActionSolve(Attack);
			ExecActionOnStart(Attack);
			if (_initializeAttackOnly)
			{
				return;
			}

			MasterUnitSkillData.UnitSkillData skillData = _unitParameter.SkillData;
			if (UnionBurstList.Count != 0)
			{
				List<int> unionBurstIds = skillData.UnionBurstIds;
				skillDictionary.Add(unionBurstIds[0], UnionBurstList[0]);
				isUnionBurstOnlyOwner = unionBurstIds[0] != 0;
				UnionBurstList[0].SkillNum = 0;
			}

			if (UnionBurstEvolutionList.Count != 0)
			{
				int key = skillData.UnionBurstEvolutionIds[0];
				skillDictionary.Add(key, UnionBurstEvolutionList[0]);
				UnionBurstEvolutionList[0].SkillNum = 0;
			}

			List<int> mainSkillIds = skillData.MainSkillIds;
			int num2 = 0;
			for (int count2 = mainSkillIds.Count; num2 < count2; num2++)
			{
				if (mainSkillIds[num2] != 0)
				{
					MainSkillList[num2].SkillNum = num2 + 1;
					skillDictionary.Add(mainSkillIds[num2], MainSkillList[num2]);
				}
			}

			List<int> mainSkillEvolutionIds = skillData.MainSkillEvolutionIds;
			for (int num3 = 0; num3 < mainSkillEvolutionIds.Count; num3++)
			{
				if (mainSkillEvolutionIds[num3] != 0)
				{
					MainSkillEvolutionList[num3].SkillNum = num3 + 1;
					skillDictionary.Add(mainSkillEvolutionIds[num3], MainSkillEvolutionList[num3]);
				}
			}

			List<int> spSkillIds = skillData.SpSkillIds;
			int num4 = 0;
			for (int count3 = spSkillIds.Count; num4 < count3 && spSkillIds[num4] != 0; num4++)
			{
				SpecialSkillList[num4].SkillNum = num4 + 1;
				skillDictionary.Add(spSkillIds[num4], SpecialSkillList[num4]);
			}

			List<int> spSkillEvolutionIds = skillData.SpSkillEvolutionIds;
			for (int num5 = 0; num5 < spSkillEvolutionIds.Count; num5++)
			{
				int num6 = spSkillEvolutionIds[num5];
				if (num6 == 0)
				{
					break;
				}

				SpecialSkillEvolutionList[num5].SkillNum = num5 + 1;
				skillDictionary.Add(num6, SpecialSkillEvolutionList[num5]);
			}

			List<int> subUnionBurstIds = skillData.SubUnionBurstIds;
			for (int num7 = 0; num7 < subUnionBurstIds.Count; num7++)
			{
				int num8 = subUnionBurstIds[num7];
				if (num8 == 0)
				{
					break;
				}

				SubUnionBurstList[num7].SkillNum = num7;
				skillDictionary.Add(num8, SubUnionBurstList[num7]);
			}

			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (KeyValuePair<int, Skill> item in skillDictionary)
			{
				Skill value2 = item.Value;
				int key2 = item.Key;
				if (key2 == 1 || key2 == 0)
				{
					continue;
				}

				int num9 = Owner.SkillLevels[key2];
				if (num9 == 0)
				{
					continue;
				}

				MasterSkillData.SkillData skillParameter = ManagerSingleton<MasterDataManager>.Instance.masterSkillData[key2];
				setSkillParameter(value2, skillParameter);
				value2.SetLevel(Owner.SkillLevels[key2]);
				List<MasterUnitSkillDataRf.UnitSkillDataRf> list = ((IEnumerable<MasterUnitSkillDataRf.UnitSkillDataRf>)ManagerSingleton<MasterDataManager>.Instance.masterUnitSkillDataRf.GetListWithSkillId(key2)).OrderBy((Func<MasterUnitSkillDataRf.UnitSkillDataRf, int>)((MasterUnitSkillDataRf.UnitSkillDataRf e) => e.min_lv)).ToList();
				for (int num10 = 0; num10 < list.Count; num10++)
				{
					MasterUnitSkillDataRf.UnitSkillDataRf unitSkillDataRf = list[num10];
					if (num9 < (int)unitSkillDataRf.min_lv)
					{
						if (num10 != 0)
						{
							dictionary.Add(value2.SkillId, num10 - 1);
						}

						break;
					}

					if (list.Count == num10 + 1)
					{
						dictionary.Add(value2.SkillId, num10);
					}
				}
			}

			if (ManagerSingleton<MasterDataManager>.Instance.masterEnemyIgnoreSkillRf.Get(Owner.UnitId) == null)
			{
				foreach (KeyValuePair<int, int> item2 in dictionary)
				{
					skillDictionary[item2.Key] = skillDictionary[item2.Key].OverrideSkillList[item2.Value].CloneAsSkill();
				}
			}

			if (UnionBurstList.Count != 0)
			{
				setCutInSkipTimeForPrincessForm(skillData.UnionBurstIds[0]);
			}

			if (UnionBurstEvolutionList.Count != 0)
			{
				setCutInSkipTimeForPrincessForm(skillData.UnionBurstEvolutionIds[0]);
			}
		}

		[global::OpenPCR.Migration("11762c44-f8cb-452c-9f3a-e244649160f4")]
		private void setSkillParameter(SkillBase _skillBase, MasterSkillData.SkillData _skillParameter, int _parentSkillId = -1)
		{
			MasterDataManager masterDataManager = ManagerSingleton<MasterDataManager>.Instance;
			Dictionary<eActionType, int> actionCounter = new Dictionary<eActionType, int>(new eActionType_DictComparer());
			_skillBase.ActionParameters = new List<ActionParameter>();
			if (_skillParameter.SkillId == Owner.UnionBurstSkillId)
			{
				eUbResponceVoiceType value = eUbResponceVoiceType.APPLOUD;
				if (!SkillDefine.UbResponceVoiceDictionary.TryGetValue(_skillParameter.icon_type, out value))
				{
					value = eUbResponceVoiceType.APPLOUD;
				}

				Owner.ComponentSound.SetUnionBurstResponceVoiceType(value);
			}

			if (_parentSkillId != -1)
			{
				_skillBase.SkillId = _parentSkillId;
			}
			else
			{
				_skillBase.SkillId = _skillParameter.SkillId;
			}

			_skillBase.skillAreaWidth = (((int)_skillParameter.skill_area_width == 0) ? Owner.ComponentSearchArea.SearchAreaSize : ((float)(int)_skillParameter.skill_area_width));
			var labyrinthSkillActionReplace = masterDataManager.masterLabyrinthSkillActionReplace;
			for (int i = 0; i < _skillParameter.ActionDataList.Count; i++)
			{
				MasterSkillAction.SkillAction actionParam = _skillParameter.ActionDataList[i];
				eActionType baseActionType = (eActionType)(ushort)actionParam.action_type;
				if (battleManager.IsLabyrinth && !Owner.IsOther)
				{
					var replacement = labyrinthSkillActionReplace.Get(actionParam.action_id);
					if (replacement != null)
					{
						actionParam = replacement;
					}
				}

				createActionValue(_skillParameter, actionParam, baseActionType, _skillBase, actionCounter);
			}

			_skillBase.CastTime = (float)(double)_skillParameter.skill_cast_time;
			_skillBase.UnionBurstCoolDownTime = (float)(double)_skillParameter.boss_ub_cool_time;
			_skillBase.SkillName = _skillParameter.Name;
			switch (_skillBase.SkillMotionType)
			{
				case SkillMotionType.DEFAULT:
					_skillBase.AnimId = Owner.AnimeIdDictionary[_skillBase.SkillId];
					break;
				case SkillMotionType.AWAKE:
					_skillBase.AnimId = eAnimKitCharacterAnimeId.AWAKE;
					break;
				case SkillMotionType.ATTACK:
					_skillBase.AnimId = eAnimKitCharacterAnimeId.ATTACK;
					break;
				case SkillMotionType.EVOLUTION:
					_skillBase.AnimId = eAnimKitCharacterAnimeId.SKILL_EVOLUTION;
					break;
				case SkillMotionType.SP_EVOLUTION:
					_skillBase.AnimId = eAnimKitCharacterAnimeId.SPECIAL_SKILL_EVOLUTION;
					break;
			}

			DependActionSolve(_skillBase);
			InitializeAction(_skillBase);
			// First-phase projection omits the SkinnedMeshRenderer-only mesh prewarm.

			List<MasterUnitSkillDataRf.UnitSkillDataRf> listWithSkillId = masterDataManager.masterUnitSkillDataRf.GetListWithSkillId(_skillParameter.SkillId);
			if (_skillBase is Skill skill)
			{
				MasterSkillData masterSkillData = masterDataManager.masterSkillData;
				for (int num = 0; num < skill.OverrideSkillList.Count; num++)
				{
					if (listWithSkillId.Count > num)
					{
						skill.OverrideSkillList[num].SkillNum = _skillBase.SkillNum;
						setSkillParameter(skill.OverrideSkillList[num], masterSkillData[listWithSkillId[num].rf_skill_id], _skillParameter.SkillId);
						skill.OverrideSkillList[num].SetLevel(Owner.SkillLevels[_skillParameter.SkillId]);
					}
				}
			}
		}

		[global::OpenPCR.Migration("90ec993e-e5ba-4069-94e0-1f4337867c08")]
		public void DependActionSolve(SkillBase _skillBase)
		{
			for (int i = 0; i < _skillBase.ActionParameters.Count; i++)
			{
				if (_skillBase.ActionParameters[i].DepenedActionId == 0)
				{
					continue;
				}

				int j = 0;
				for (int count = _skillBase.ActionParameters.Count; j < count; j++)
				{
					ActionParameter actionParameter = _skillBase.ActionParameters[j];
					if (_skillBase.ActionParameters[i].DepenedActionId == actionParameter.ActionId)
					{
						actionParameter.ActionChildrenIndexes.Add(i);
					}
				}
			}

			_skillBase.HasParentIndexes = new List<int>();
			int k = 0;
			for (int count2 = _skillBase.ActionParameters.Count; k < count2; k++)
			{
				ActionParameter actionParameter2 = _skillBase.ActionParameters[k];
				_skillBase.HasParentIndexes = _skillBase.HasParentIndexes.Union(actionParameter2.ActionChildrenIndexes).ToList();
				if (actionParameter2.ActionType == eActionType.REFLEXIVE)
				{
					int l = 0;
					for (int count3 = actionParameter2.ActionChildrenIndexes.Count; l < count3; l++)
					{
						_skillBase.ActionParameters[actionParameter2.ActionChildrenIndexes[l]].ReferencedByReflection = true;
					}
				}
			}

			UpdateEffectRunTimeData(_skillBase, _skillBase.SkillEffects.Select(effect => (SkillEffectBase)effect).ToList());
		}

		[global::OpenPCR.Migration("280902ed-ec52-4884-8fa6-6c4657ac5678")]
		public void UpdateEffectRunTimeData(SkillBase _skillBase, List<SkillEffectBase> _skillList, bool _resetflag = true)
		{
			if (_resetflag)
			{
				int i = 0;
				for (int count = _skillBase.ActionParameters.Count; i < count; i++)
				{
					_skillBase.ActionParameters[i].ReferencedByEffect = false;
				}
			}

			int j = 0;
			for (int count2 = _skillList.Count; j < count2; j++)
			{
				SkillEffectBase effect = _skillList[j];
				effect.TargetAction = _skillBase.ActionParameters.Find((ActionParameter e) => e.ActionId == effect.TargetActionId);
				if (effect.TargetAction == null)
				{
					effect.TargetAction = _skillBase.ActionParameters[0];
				}

				if (effect is NormalSkillEffect normalEffect)
				{
					UpdateEffectRunTimeData(_skillBase, normalEffect.FireArmEndEffects, _resetflag: false);
				}

				eEffectBehavior effectBehavior = effect.EffectBehavior;
				if (effectBehavior != eEffectBehavior.FIREARM && (uint)(effectBehavior - 5) > 1u)
				{
					continue;
				}

				if (_skillBase.SkillId == 1)
				{
					effect.FireAction = _skillBase.ActionParameters[0];
					effect.FireAction.ReferencedByEffect = true;
				}
				else if (effect.FireActionId != -1)
				{
					effect.FireAction = _skillBase.ActionParameters.Find((ActionParameter e) => e.ActionId == effect.FireActionId);
					effect.FireAction.ReferencedByEffect = true;
				}
			}
		}

		[global::OpenPCR.Migration("682b9b0b-3e58-4ca4-a89c-3ddb71efecd6")]
		public void ExecActionOnStart(Skill _skill)
		{
			for (int i = 0; i < _skill.ActionParameters.Count; i++)
			{
				ActionParameter actionParameter = _skill.ActionParameters[i];
				if (actionParameter is AttackActionBase)
				{
					_skill.HasAttack = true;
				}

				actionParameter.ExecActionOnStart(_skill, Owner, this);
			}

			for (int j = 0; j < _skill.SkillEffects.Count; j++)
			{
				_skill.SkillEffects[j].AlreadyFireArmExecedData = new Dictionary<UnitCtrl, bool>();
				_skill.SkillEffects[j].AlreadyFireArmExecedKeys = new List<UnitCtrl>();
			}

			_skill.DamagedPartsList = new List<BasePartsData>();
		}

		[global::OpenPCR.Migration("16c3e08b-d488-4561-b784-36ea75a45fef")]
		private void execActionOnWaveStart(Skill _skill)
		{
			for (int i = 0; i < _skill.ActionParameters.Count; i++)
			{
				_skill.ActionParameters[i].ExecActionOnWaveStart(_skill, Owner, this);
			}
		}

		[global::OpenPCR.Migration("8e202d56-6eb6-4a38-9646-36c5d9bfb063")]
		public void InitializeAction(SkillBase _skillBase)
		{
			for (int i = 0; i < _skillBase.ActionParameters.Count; i++)
			{
				_skillBase.ActionParameters[i].Initialize(Owner);
			}
		}

		[global::OpenPCR.Migration("7b571649-4d24-412c-abfa-f0e80929d284")]
		private void createActionValue(MasterSkillData.SkillData skillParameter, MasterSkillAction.SkillAction actionParam, eActionType _baseActionType, SkillBase _skillBase, Dictionary<eActionType, int> actionCounter)
		{
			ActionParameter actionParameter = Activator.CreateInstance(SkillDefine.SkillActionTypeDictionary[(eActionType)(ushort)actionParam.action_type]) as ActionParameter;
			actionParameter.TargetSort = (PriorityPattern)(int)actionParam.target_type;
			actionParameter.TargetNth = actionParam.target_number;
			actionParameter.TargetNum = actionParam.target_count;
			actionParameter.TargetWidth = (((int)actionParam.target_range <= 0) ? Owner.ComponentSearchArea.SearchAreaSize : ((float)(int)actionParam.target_range));
			actionParameter.ActionType = (eActionType)(ushort)actionParam.action_type;
			actionParameter.ActionDetail1 = actionParam.action_detail_1;
			actionParameter.ActionDetail2 = actionParam.action_detail_2;
			actionParameter.ActionDetail3 = actionParam.action_detail_3;
			actionParameter.Value = new Dictionary<eValueNumber, double>(new eValueNumber_DictComparer());
			actionParameter.Value.Add(eValueNumber.VALUE_1, (double)actionParam.action_value_1);
			actionParameter.Value.Add(eValueNumber.VALUE_2, (double)actionParam.action_value_2);
			actionParameter.Value.Add(eValueNumber.VALUE_3, (double)actionParam.action_value_3);
			actionParameter.Value.Add(eValueNumber.VALUE_4, (double)actionParam.action_value_4);
			actionParameter.Value.Add(eValueNumber.VALUE_5, (double)actionParam.action_value_5);
			actionParameter.Value.Add(eValueNumber.VALUE_6, (double)actionParam.action_value_6);
			actionParameter.Value.Add(eValueNumber.VALUE_7, (double)actionParam.action_value_7);
			actionParameter.MasterData = actionParam;
			int num = 0;
			if (actionCounter.ContainsKey(_baseActionType))
			{
				num = actionCounter[_baseActionType];
				actionCounter[_baseActionType]++;
			}
			else
			{
				actionCounter.Add(_baseActionType, 1);
			}

			ActionParameterOnPrefab actionParameterOnPrefab = _skillBase.ActionParametersOnPrefab.Find((ActionParameterOnPrefab e) => e.ActionType == _baseActionType);
			if (actionParameterOnPrefab == null)
			{
				actionParameterOnPrefab = new ActionParameterOnPrefab
				{
					ActionType = actionParameter.ActionType
				};
				_skillBase.ActionParametersOnPrefab.Add(actionParameterOnPrefab);
			}

			ActionParameterOnPrefabDetail actionParameterOnPrefabDetail = null;
			if (num >= actionParameterOnPrefab.Details.Count)
			{
				actionParameterOnPrefabDetail = new ActionParameterOnPrefabDetail
				{
					ExecTime = new List<ActionExecTime>()
				};
				actionParameterOnPrefab.Details.Add(actionParameterOnPrefabDetail);
			}
			else
			{
				actionParameterOnPrefabDetail = actionParameterOnPrefab.Details[num];
				actionParameterOnPrefabDetail.ExecTime = new List<ActionExecTime>(actionParameterOnPrefabDetail.ExecTimeForPrefab);
				int num2 = 0;
				for (int count = actionParameterOnPrefabDetail.ExecTimeCombo.Count; num2 < count; num2++)
				{
					ActionExecTimeCombo actionExecTimeCombo = actionParameterOnPrefabDetail.ExecTimeCombo[num2];
					for (int num3 = 0; num3 < actionExecTimeCombo.Count; num3++)
					{
						switch (actionExecTimeCombo.InterporationType)
						{
							case eComboInterporationType.LINEAR:
								actionParameterOnPrefabDetail.ExecTime.Add(new ActionExecTime { Time = actionExecTimeCombo.StartTime + (float)num3 * actionExecTimeCombo.OffsetTime, DamageNumType = eDamageEffectType.COMBO });
								break;
							case eComboInterporationType.CURVE:
								actionParameterOnPrefabDetail.ExecTime.Add(new ActionExecTime { Time = actionExecTimeCombo.StartTime + actionExecTimeCombo.Curve.Evaluate((float)num3 / (float)actionExecTimeCombo.Count) * (float)actionExecTimeCombo.Count * actionExecTimeCombo.OffsetTime, DamageNumType = eDamageEffectType.COMBO });
								break;
						}
					}
				}

				actionParameterOnPrefabDetail.ExecTime.Sort((ActionExecTime a, ActionExecTime b) => a.Time.CompareTo(b.Time));
			}

			actionParameter.ActionExecTimeList = actionParameterOnPrefabDetail.ExecTime;
			actionParameter.ExecTime = new float[actionParameterOnPrefabDetail.ExecTime.Count];
			actionParameter.KnockAnimationCurve = actionParameterOnPrefab.KnockAnimationCurve;
			actionParameter.KnockDownAnimationCurve = actionParameterOnPrefab.KnockDownAnimationCurve;
			actionParameter.EffectType = actionParameterOnPrefab.EffectType;
			actionParameter.ActionEffectList = actionParameterOnPrefabDetail.ActionEffectList;
			actionParameter.ActionSubEffectList = actionParameterOnPrefabDetail.ActionSubEffectList;
			int num4 = 0;
			for (int count2 = actionParameterOnPrefabDetail.ExecTime.Count; num4 < count2; num4++)
			{
				actionParameter.ActionWeightSum += actionParameterOnPrefabDetail.ExecTime[num4].Weight;
				actionParameter.ExecTime[num4] = actionParameterOnPrefabDetail.ExecTime[num4].Time;
			}

			actionParameter.DepenedActionId = actionParam.DependActionId;
			actionParameter.ActionId = actionParam.action_id;
			actionParameter.TargetList = new List<BasePartsData>();
			actionParameter.ActionChildrenIndexes = new List<int>();
			actionParameter.TargetAssignment = (eTargetAssignment)(int)actionParam.target_assignment;
			actionParameter.Direction = (DirectionType)(int)actionParam.target_area;
			actionParameter.IsExcludedFromBlackOutTarget = actionParameterOnPrefab.IsExcludedFromBlackOutTarget;
			_skillBase.ActionParameters.Add(actionParameter);
		}

		[global::OpenPCR.Migration("d7679c63-7866-4203-9455-1cfdee2dae96")]
		public bool StartAction(int skillId)
		{
			return StartAction(skillId, skillDictionary[skillId]);
		}

		[global::OpenPCR.Migration("2d03ccc2-42d8-4002-9f21-9de9aec5991a")]
		public bool StartAction(int _skillId, Skill _skill)
		{
			Skill skill = _skill;
			actionStartFirstProcess(skill, out var _hasNoTarget, false);
			bool flag = false;
			List<int> hasParentIndexes = skill.HasParentIndexes;
			if (skill.BlackOutTime > 0f)
			{
				for (int i = 0; i < skill.ActionParameters.Count; i++)
				{
					ActionParameter actionParameter = skill.ActionParameters[i];
					if ((hasParentIndexes.Contains(i) && !actionParameter.ReferencedByReflection) || actionParameter.ReferencedByEffect)
					{
						continue;
					}

					int j = 0;
					for (int count = actionParameter.TargetList.Count; j < count && (j < actionParameter.TargetNum || actionParameter.TargetNum == 0 || j == 0); j++)
					{
						UnitCtrl owner = actionParameter.TargetList[j].Owner;
						if (actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE)
						{
							owner.ComponentScaleChange.SetScaleChangeTarget(true);
						}

						battleManager.AddBlackOutTarget(Owner, owner, actionParameter.TargetList[j]);
						if (Owner != owner && skill.DisableOtherCharaOnStart)
						{
							owner.GetCurrentSpineCtrl().CurColor = new Color(1f, 1f, 1f, 0f);
						}
					}
				}

				battleManager.StartChangeScale(skill, skill.CutInSkipTime);
			}

			if (_skillId == Owner.UnionBurstSkillId)
			{
				battleManager.SetSkillExeScreen(Owner, skill.BlackOutTime, skill.BlackoutColor, skill.BlackoutEndWithMotion || skill.IsPrincessForm);
				battleManager.StartBackgroundSpineAnimation(skill.SkillId, skill.CutInSkipTime);
				battleTimeScale.StopSlowEffect();
				if (skill.SlowEffect.Enable)
				{
					battleTimeScale.StartSlowEffect(skill.SlowEffect, Owner, skill.CutInSkipTime, _useCoroutine: false);
				}

				if (Owner.IsPlayerUnit)
				{
					flag = true;
				}
			}

			bool flag2 = flag || Owner.IsBoss;
			if (flag2)
			{
				SingletonMonoBehaviour<BattleHeaderController>.Instance.SkillWindow.Indicate(skill.SkillName, Owner.IsBoss);
			}

			BattleUnitBaseAnimKitController currentSpineCtrl = Owner.GetCurrentSpineCtrl();
			if (skill.ForcePlayNoTarget)
			{
				_hasNoTarget = false;
			}

			UnitComponentAbnormal componentAbnormal = Owner.ComponentAbnormal;
			if (_hasNoTarget || (componentAbnormal.IsAbnormalState(eAbnormalState.SILENCE) && !componentAbnormal.AttackWhenSilence))
			{
				if (Owner.IsBoss && !componentAbnormal.IsConfusionOrConvert() && !skill.IsPrincessForm && currentSpineCtrl.IsAnimation(skill.AnimId, skill.SkillNum, -1, Owner.MotionPrefix))
				{
					Owner.ComponentAnimation.PlayAnime(skill.AnimId, skill.SkillNum, -1, Owner.MotionPrefix, _isLoop: false, _targetCtr: null, _quiet: false, _startTime: 0f, _ignoreBlackout: false);
					CreateNormalPrefabWithTargetMotion(skill, 0, _first: true);
				}

				return false;
			}

			if (!flag2)
			{
				Owner.IndicateSkillName(skill.SkillName);
			}

			for (int k = 0; k < skill.ShakeEffects.Count; k++)
			{
				if (skill.ShakeEffects[k].TargetMotion == 0)
				{
					AppendCoroutine(StartShakeWithDelay(skill.ShakeEffects[k], skill, _first: true), ePauseType.VISUAL, (skill.BlackOutTime > 0f) ? Owner : null);
				}
			}

			startBlurEffects(skill, _first: true);
			if (skill.AnimId != eAnimKitCharacterAnimeId.NONE && !skill.IsPrincessForm && currentSpineCtrl.IsAnimation(skill.AnimId, skill.SkillNum, -1, Owner.MotionPrefix))
			{
				if (Owner.ComponentUnionBurst.IsCutInSkip)
				{
					Owner.ComponentUnionBurst.SetIsCutInSkip(false);
					Owner.RestartPlayAnimeCoroutine(skill.CutInSkipTime, skill.AnimId, skill.SkillNum, Owner.MotionPrefix);
				}
				else
				{
					Owner.ComponentAnimation.PlayAnime(skill.AnimId, skill.SkillNum, -1, Owner.MotionPrefix, _isLoop: false, _targetCtr: null, _quiet: false, _startTime: skill.CutInSkipTime, _ignoreBlackout: false);
				}
			}

			skill.ReadySkill();
			skill.AweValue = componentAbnormal.CalcAweValue(_skillId == Owner.UnionBurstSkillId, _skillId == 1);
			for (int l = 0; l < skill.ActionParameters.Count; l++)
			{
				ActionParameter actionParameter2 = skill.ActionParameters[l];
				actionParameter2.ReadyAction(Owner, this, skill);
				if ((!hasParentIndexes.Contains(l) || actionParameter2.ReferencedByReflection) && !actionParameter2.ReferencedByEffect)
				{
					ExecUnitActionWithDelay(actionParameter2, skill, _first: true, _boneCount: true);
				}
			}

			for (int m = 0; m < battleManager.BlackoutUnitTargetList.Count; m++)
			{
				battleManager.BlackoutUnitTargetList[m].DisableSortOrderFrontOnBlackoutTarget = false;
			}

			CreateNormalPrefabWithTargetMotion(skill, 0, _first: true);
			if (skill.ZoomEffect.Enable)
			{
				battleCameraEffect.StartZoomEffect(skill.ZoomEffect, Owner, skill.CutInSkipTime, _ignorePause: false, _useStartCoroutine: false, skill.SkillId != Owner.UnionBurstSkillId);
			}

			Owner.ComponentSortOrder.StartChangeSortOrder(skill.ChangeSortDatas, skill.CutInSkipTime);
			return true;
		}

		[global::OpenPCR.Migration("71f82589-646c-492a-9f06-b353788fa51e")]
		private void actionStartFirstProcess(Skill _skill, out bool _hasNoTarget, bool _resetActionTriggerOnly)
		{
			if (_skill.UnionBurstCoolDownTime > 0f)
			{
				Owner.ComponentUnionBurst.SetCoolDownTime(_skill.UnionBurstCoolDownTime);
			}

			_skill.LifeSteal = 0;
			_skill.DefeatEnemyCount = 0;
			_skill.DefeatByThisSkill = false;
			_skill.AlreadyAddAttackSelfSeal = false;
			_skill.AlreadyExexActionByHit = false;
			_skill.AlreadyAddAttackSealForAllEnemy = false;
			_skill.Cancel = false;
			_skill.EffectBranchId = 0;
			_skill.LoopEffectAlreadyDone = false;
			ContinuousActionEndDone = false;
			_skill.OwnerReturnPosition = transform.localPosition;
			_skill.CountBlind = false;
			_skill.AbsorberValue = battleManager.AbsorberValue;
			if (_skill.HasAttack && _skill.IsLifeStealEnabled)
			{
				Owner.ComponentAbnormal.ApplyLifeSteal(_skill);
			}

			_hasNoTarget = true;
			for (int i = 0; i < _skill.ActionParameters.Count; i++)
			{
				ActionParameter actionParameter = _skill.ActionParameters[i];
				if (_resetActionTriggerOnly && !actionParameter.NoDelayExecByTriggerOnly)
				{
					continue;
				}

				actionParameter.IdOffsetDictionary = new Dictionary<BasePartsData, long>();
				actionParameter.CancelByIfForAll = false;
				actionParameter.AdditionalValue = null;
				actionParameter.MultipleValue = null;
				actionParameter.DivideValue = null;
				actionParameter.UsedChargeEnergyByReceiveDamage.Clear();
				if (actionParameter.ReferencedByReflection)
				{
					continue;
				}

				if (!actionParameter.IsSearchAndSorted)
				{
					searchAndSortTarget(_skill, actionParameter, transform.position);
				}

				actionParameter.IsSearchAndSorted = false;
				if (actionParameter.ActionType == eActionType.REFLEXIVE)
				{
					Vector3 basePosition = default(Vector3);
					bool considerBodyWidth = true;
					float position = 0f;
					bool isFlightStateTargetByReflection = false;
					switch ((eRefrexiveType)actionParameter.ActionDetail1)
					{
						case eRefrexiveType.REFLEXIVE_ABSOLUTE_SEARCH:
							{
								Vector3 ownerPosition = transform.position;
								bool isLeftDir = Owner.IsLeftDir;
								double value = actionParameter.Value[eValueNumber.VALUE_1];
								if (isLeftDir)
								{
									value = -value;
								}

								basePosition = ownerPosition + new Vector3((float)(value / 540.0), 0f, 0f);
								break;
							}

						case eRefrexiveType.REFLEXIVE_NORMAL:
							{
								if (actionParameter.TargetList.Count == 0)
								{
									clearChildrenTarget(actionParameter, _skill);
									continue;
								}

								BasePartsData target = actionParameter.TargetList[0];
								basePosition = target.GetPosition();
								isFlightStateTargetByReflection = target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT);
								break;
							}

						case eRefrexiveType.REFLEXIVE_ABSOLUTE_POSITION:
							{
								basePosition = transform.position;
								bool isLeftDir = Owner.IsLeftDir;
								float value = (float)actionParameter.Value[eValueNumber.VALUE_1];
								position = isLeftDir ? -value : value;
								break;
							}

						case eRefrexiveType.REFLEXIVE_NORMAL_NOT_CONSIDER_BODY_WIDTH:
							{
								if (actionParameter.TargetList.Count == 0)
								{
									clearChildrenTarget(actionParameter, _skill);
									continue;
								}

								BasePartsData target = actionParameter.TargetList[0];
								basePosition = target.GetPosition();
								isFlightStateTargetByReflection = target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT);
								considerBodyWidth = false;
								break;
							}
					}

					int j = 0;
					for (int count = actionParameter.ActionChildrenIndexes.Count; j < count; j++)
					{
						ActionParameter actionParameter2 = _skill.ActionParameters[actionParameter.ActionChildrenIndexes[j]];
						actionParameter2.Position = position;
						actionParameter2.IsFlightStateTargetByReflection = isFlightStateTargetByReflection;
						searchAndSortTarget(_skill, actionParameter2, basePosition, _quiet: false, considerBodyWidth);
					}
				}

				if (actionParameter.TargetList.Count != 0)
				{
					_hasNoTarget = false;
				}
			}
		}

		[global::OpenPCR.Migration("f45d913d-149b-463f-b948-231cfa8f3935")]
		private void clearChildrenTarget(ActionParameter _actionParameter, Skill _skill)
		{
			foreach (int index in _actionParameter.ActionChildrenIndexes)
			{
				_skill.ActionParameters[index].TargetList.Clear();
			}
		}

		[global::OpenPCR.Migration("8149978b-f89f-4521-8f86-f0c8e92bd187")]
		private void searchAndSortTarget(Skill _skill, ActionParameter _action, Vector3 _basePosition, bool _quiet = false, bool _considerBodyWidth = true)
		{
			searchTargetUnit(_action, _basePosition, _skill, _considerBodyWidth);
			sortTargetListByTargetPattern(_action, Owner.BottomTransform, _quiet);
			bool flag = false;
			for (int i = 0; i < _action.TargetList.Count - _action.TargetNth; i++)
			{
				flag = true;
				_action.TargetList[i] = _action.TargetList[_action.TargetNth + i];
			}

			if (flag)
			{
				int count = _action.TargetList.Count;
				for (int num = count - 1; num >= count - _action.TargetNth; num--)
				{
					_action.TargetList.RemoveAt(num);
				}
			}

			if (!flag && _action.TargetNth != 0 && _action.TargetList.Count != 0)
			{
				BasePartsData item = _action.TargetList[_action.TargetList.Count - 1];
				_action.TargetList.Clear();
				_action.TargetList.Add(item);
			}

			BasePartsData basePartsData = null;
			if (_action.TargetAssignment == eTargetAssignment.OTHER_SIDE && _action.TargetNum == 1 && !_action.IgnoreDecoy)
			{
				if (Owner.IsOther)
				{
					if (Owner.ComponentAbnormal.IsConfusionOrConvert())
					{
						if (battleManager.DecoyOther != null)
						{
							basePartsData = battleManager.DecoyOther.ComponentMultiPartsBoss.GetFirstParts(_owner: false, Owner.transform.position.x);
						}
					}
					else if (battleManager.DecoyUnit != null)
					{
						basePartsData = battleManager.DecoyUnit.ComponentMultiPartsBoss.GetFirstParts(_owner: false, Owner.transform.position.x);
					}
				}
				else if (Owner.ComponentAbnormal.IsConfusionOrConvert())
				{
					if (battleManager.DecoyUnit != null)
					{
						basePartsData = battleManager.DecoyUnit.ComponentMultiPartsBoss.GetFirstParts(_owner: false, Owner.transform.position.x);
					}
				}
				else if (battleManager.DecoyOther != null)
				{
					basePartsData = battleManager.DecoyOther.ComponentMultiPartsBoss.GetFirstParts(_owner: false, Owner.transform.position.x);
				}

				if (Owner.ComponentAbnormal.IsTauntState() && !Owner.ComponentAbnormal.IsConfusionOrConvert())
				{
					basePartsData = Owner.ComponentAbnormal.TauntSource;
				}

				if (basePartsData != null && !basePartsData.Owner.ComponentState.IsDead && !basePartsData.Owner.ComponentAbnormal.IsSpyOrEtherialization(Owner) && isInActionTargetArea(_skill, _action, _basePosition, _considerBodyWidth, basePartsData))
				{
					_action.TargetList.Clear();
					_action.TargetList.Add(basePartsData);
				}
			}

			_action.IsSearchAndSorted = true;
		}

		[global::OpenPCR.Migration("760e6f84-e31e-42a6-ab0a-2bf0bda7381a")]
		private void setCutInSkipTimeForPrincessForm(int _skillId)
		{
			Skill skill = skillDictionary[_skillId];
			if (skill.IsPrincessForm)
			{
				skill.CutInSkipTime = 0f;
			}
		}

		[global::OpenPCR.Migration("5df2d747-2f0f-410f-a6b0-f2c65c6ebca5")]
		public IEnumerator StartActionWithOutCutIn(UnitCtrl _unit, int _skillId, Action _callback)
		{
			if (!battleManager.CanCreateBattleEffect())
			{
				_callback.Call();
				yield break;
			}

			battleManager.StartBackgroundSpineAnimation(_skillId, 0f);
			battleTimeScale.StopSlowEffect();
			Skill skill = skillDictionary[_skillId];
			BattleAnimKitController unitSpineController = Owner.GetCurrentSpineCtrl();
			TrackEntry entry = unitSpineController.state.GetCurrent(0);
			float deltaTimeAccumulated = 0f;
			BattleHeaderController battleHeaderController = SingletonMonoBehaviour<BattleHeaderController>.Instance;
			battleManager.SetSkillExeScreenActive(_unit, Color.black);
			Owner.ComponentSortOrder.SetSortOrderFront();
			AddBlackoutTarget(_skillId);
			if (skill.IsPrincessForm)
			{
				yield return null;
				_callback.Call();
				yield break;
			}

			Owner.ComponentAnimation.PlayAnime(skill.AnimId, skill.SkillNum, -1, Owner.MotionPrefix, _isLoop: false);
			for (int i = 0; i < skill.SkillEffects.Count; i++)
			{
				SkillEffectBase skillEffect = skill.SkillEffects[i];
				if (skillEffect.TargetMotionIndex == 0)
				{
					skill.Cancel = false;
					StartCoroutine(updateCoroutineWithOutCutIn(createNormalPrefabWithDelay(skillEffect, skill, _first: false, _skipCutIn: true)));
				}
			}

			if (skill.ZoomEffect.Enable)
			{
				battleCameraEffect.StartZoomEffect(skill.ZoomEffect, Owner, 0f, _ignorePause: true, _useStartCoroutine: false, _isNotUb: false);
			}

			if (skill.SkillId == Owner.UnionBurstSkillId)
			{
				Owner.ComponentSortOrder.StartChangeSortOrder(skill.ChangeSortDatas, 0f);
			}

			while (!battleManager.CoroutineManager.VisualPause && !battleManager.IsSkipping())
			{
				yield return null;
			}

			this.Timer(delegate
			{
				battleManager.SetSkillExeScreenActive(_unit, skill.BlackoutColor);
			});
			Owner.GetCurrentSpineCtrl().SetTimeScale(1f);
			Owner.ComponentUnionBurst.SetIsCutInSkip(true);
			entry = unitSpineController.state.GetCurrent(0);
			while (true)
			{
				if (!battleHeaderController.IsPaused)
				{
					deltaTimeAccumulated += Time.deltaTime;
				}

				while (deltaTimeAccumulated > 0f)
				{
					if (!battleManager.BlackoutUnitTargetList.Contains(Owner))
					{
						unitSpineController.RealUpdate();
						unitSpineController.RealLateUpdate();
					}

					battleManager.BlackoutUnitTargetList.ForEach(delegate (UnitCtrl _it)
					{
						_it.GetCurrentSpineCtrl().RealUpdate();
					});
					battleManager.BlackoutUnitTargetList.ForEach(delegate (UnitCtrl _it)
					{
						_it.GetCurrentSpineCtrl().RealLateUpdate();
					});
					Owner.EffectSpineControllerList.ForEach(delegate (BattleAnimKitController _fx)
					{
						_fx.RealUpdate();
					});
					Owner.EffectSpineControllerList.ForEach(delegate (BattleAnimKitController _fx)
					{
						_fx.RealLateUpdate();
					});
					deltaTimeAccumulated -= battleManager.DeltaTime;
				}

				if (entry.time >= skill.CutInSkipTime)
				{
					break;
				}

				if (!battleManager.IsSkipping())
				{
					yield return null;
				}

				battleManager.IsNoCutInMotionRightAfter = false;
			}

			Owner.EffectSpineControllerList.Clear();
			_callback.Call();
		}

		[global::OpenPCR.Migration("06a7e852-0d18-4a1c-b465-099552efda39")]
		public void StopSlowEffect()
		{
			battleTimeScale.StopSlowEffect();
		}

		[global::OpenPCR.Migration("8c2ad3a3-20e4-4c1c-8fcb-a2dc737dc665")]
		public void AddBlackoutTarget(int _skillId)
		{
			Skill skill = skillDictionary[_skillId];
			int i = 0;
			for (int count = skill.ActionParameters.Count; i < count; i++)
			{
				ActionParameter actionParameter = skill.ActionParameters[i];
				if (actionParameter.ReferencedByReflection || actionParameter.IsExcludedFromBlackOutTarget)
				{
					continue;
				}

				searchAndSortTarget(skill, actionParameter, transform.position, _quiet: true);
				for (int j = 0; j < actionParameter.TargetNum; j++)
				{
					if (actionParameter.TargetList.Count <= j)
					{
						continue;
					}

					UnitCtrl owner = actionParameter.TargetList[j].Owner;
					if (actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE)
					{
						owner.ComponentScaleChange.SetScaleChangeTarget(true);
					}

					battleManager.AddBlackOutTarget(Owner, owner, actionParameter.TargetList[j]);
					if (actionParameter.ActionType != eActionType.REFLEXIVE)
					{
						continue;
					}

					Vector3 basePosition = default(Vector3);
					bool considerBodyWidth = true;
					bool isFlightStateTargetByReflection = false;
					switch ((eRefrexiveType)actionParameter.ActionDetail1)
					{
						case eRefrexiveType.REFLEXIVE_ABSOLUTE_SEARCH:
							{
								Vector3 position = transform.position;
								bool isLeftDir = Owner.IsLeftDir;
								float value = (float)actionParameter.Value[eValueNumber.VALUE_1];
								if (isLeftDir)
								{
									value = -value;
								}

								basePosition = position + new Vector3(value / 540f, 0f, 0f);
								break;
							}

						case eRefrexiveType.REFLEXIVE_NORMAL:
							{
								BasePartsData target = actionParameter.TargetList[0];
								basePosition = target.GetPosition();
								isFlightStateTargetByReflection = target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT);
								break;
							}

						case eRefrexiveType.REFLEXIVE_ABSOLUTE_POSITION:
							basePosition = transform.position;
							break;
						case eRefrexiveType.REFLEXIVE_NORMAL_NOT_CONSIDER_BODY_WIDTH:
							{
								BasePartsData target = actionParameter.TargetList[0];
								basePosition = target.GetPosition();
								isFlightStateTargetByReflection = target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT);
								considerBodyWidth = false;
								break;
							}
					}

					int k = 0;
					for (int count2 = actionParameter.ActionChildrenIndexes.Count; k < count2; k++)
					{
						ActionParameter actionParameter2 = skill.ActionParameters[actionParameter.ActionChildrenIndexes[k]];
						actionParameter2.IsFlightStateTargetByReflection = isFlightStateTargetByReflection;
						searchAndSortTarget(skill, actionParameter2, basePosition, _quiet: false, considerBodyWidth);
						for (int l = 0; l < actionParameter2.TargetNum && l != actionParameter2.TargetList.Count; l++)
						{
							owner = actionParameter2.TargetList[l].Owner;
							if (actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE)
							{
								owner.ComponentScaleChange.SetScaleChangeTarget(true);
							}

							battleManager.AddBlackOutTarget(Owner, owner, actionParameter2.TargetList[l]);
						}
					}
				}

				battleManager.StartChangeScale(skill, 0f);
			}
		}

		[global::OpenPCR.Migration("8cbd125a-487c-461f-9de4-9f78ba25bb8b")]
		public void SearchTargetByAction(int _skillId)
		{
			Skill skill = skillDictionary[_skillId];
			int i = 0;
			for (int count = skill.ActionParameters.Count; i < count; i++)
			{
				ActionParameter actionParameter = skill.ActionParameters[i];
				if (actionParameter.ReferencedByReflection)
				{
					continue;
				}

				searchAndSortTarget(skill, actionParameter, transform.position, _quiet: true);
				actionParameter.IsSearchAndSorted = false;
				for (int j = 0; j < actionParameter.TargetNum; j++)
				{
					if (actionParameter.TargetList.Count > j && actionParameter.ActionType == eActionType.REFLEXIVE)
					{
						Vector3 basePosition = default(Vector3);
						bool considerBodyWidth = true;
						bool isFlightStateTargetByReflection = false;
						switch ((eRefrexiveType)actionParameter.ActionDetail1)
						{
							case eRefrexiveType.REFLEXIVE_ABSOLUTE_SEARCH:
								{
									Vector3 position = transform.position;
									bool isLeftDir = Owner.IsLeftDir;
									float value = (float)actionParameter.Value[eValueNumber.VALUE_1];
									if (isLeftDir)
									{
										value = -value;
									}

									basePosition = position + new Vector3(value / 540f, 0f, 0f);
									break;
								}

							case eRefrexiveType.REFLEXIVE_NORMAL:
								{
									BasePartsData target = actionParameter.TargetList[0];
									basePosition = target.GetPosition();
									isFlightStateTargetByReflection = target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT);
									break;
								}

							case eRefrexiveType.REFLEXIVE_ABSOLUTE_POSITION:
								basePosition = transform.position;
								break;
							case eRefrexiveType.REFLEXIVE_NORMAL_NOT_CONSIDER_BODY_WIDTH:
								{
									BasePartsData target = actionParameter.TargetList[0];
									basePosition = target.GetPosition();
									isFlightStateTargetByReflection = target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT);
									considerBodyWidth = false;
									break;
								}
						}

						int k = 0;
						for (int count2 = actionParameter.ActionChildrenIndexes.Count; k < count2; k++)
						{
							ActionParameter actionParameter2 = skill.ActionParameters[actionParameter.ActionChildrenIndexes[k]];
							actionParameter2.IsFlightStateTargetByReflection = isFlightStateTargetByReflection;
							searchAndSortTarget(skill, actionParameter2, basePosition, _quiet: false, considerBodyWidth);
							actionParameter2.IsSearchAndSorted = false;
						}
					}
				}
			}
		}

		[global::OpenPCR.Migration("89ed4f22-a4f3-49e7-baab-9edc91232831")]
		private IEnumerator updateCoroutineWithOutCutIn(IEnumerator _coroutine)
		{
			float deltaTimeAccumulated = 0f;
			while (true)
			{
				yield return null;
				if (!SingletonMonoBehaviour<BattleHeaderController>.Instance.IsPaused)
				{
					deltaTimeAccumulated += Time.deltaTime;
				}

				while (deltaTimeAccumulated > 0f)
				{
					if (!_coroutine.MoveNext())
					{
						yield break;
					}

					deltaTimeAccumulated -= battleManager.DeltaTime;
				}
			}
		}

		[global::OpenPCR.Migration("60c11935-cad9-4bd3-a88a-2dbf64376fa6")]
		public void CreateNormalPrefabWithTargetMotion(Skill _skill, int _targetmotion, bool _first, bool _useStartCoroutine = false, bool _modechangeEndEffect = false)
		{
			int i = 0;
			for (int count = _skill.SkillEffects.Count; i < count; i++)
			{
				NormalSkillEffect normalSkillEffect = _skill.SkillEffects[i];
				if (normalSkillEffect.TargetMotionIndex != _targetmotion)
				{
					continue;
				}

				bool flag = normalSkillEffect.IsVisualEffect();
				if (!(battleManager.IsSkipping() & flag))
				{
					if (_useStartCoroutine)
					{
						battleManager.StartCoroutine(createNormalPrefabWithDelay(normalSkillEffect, _skill, _first));
						continue;
					}

					ePauseType pauseType = ((!flag) ? ePauseType.SYSTEM : ePauseType.VISUAL);
					UnitCtrl unit = ((_skill.BlackOutTime > 0f) ? Owner : null);
					AppendCoroutine(createNormalPrefabWithDelay(normalSkillEffect, _skill, _first, _skipCutIn: false, _modechangeEndEffect, _modechangeEndEffect), pauseType, unit);
				}
			}
		}

		[global::OpenPCR.Migration("058bcad0-642a-47d5-abcd-4b836adda50c")]
		public void ExecUnitActionWithDelay(ActionParameter _action, Skill _skill, bool _first, bool _boneCount, bool _ignoreCancel = false)
		{
			if (_action.TargetList.Count == 0)
			{
				initWhenNoTarget(_action, _skill);
			}

			eActionType actionType = _action.ActionType;
			if (actionType == eActionType.CONTINUOUS_ATTACK)
			{
				if (Owner.GetCurrentSpineCtrl().IsAnimation(_skill.AnimId, _skill.SkillNum, 1))
				{
					AppendCoroutine((_action as ContinuousAttackAction).UpdateMotionRoopForContinuousDamage(_skill, Owner, this), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? Owner : null);
				}

				_action.ContinuousTargetCount = 0;
			}

			for (int i = 0; i < _action.TargetList.Count && (i < _action.TargetNum || _action.TargetNum == 0 || i == 0); i++)
			{
				BasePartsData target = _action.TargetList[i];
				AppendCoroutine(ExecActionWithDelayAndTarget(_action, _skill, target, 0f, _first, _boneCount, _ignoreCancel), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? Owner : null);
			}
		}

		[global::OpenPCR.Migration("bd58a346-c7c1-4246-9d99-f1b415c29b92")]
		public void ExecUnitActionNoDelay(ActionParameter _action, Skill _skill)
		{
			if (_action.TargetList.Count == 0)
			{
				initWhenNoTarget(_action, _skill);
			}

			for (int i = 0; i < _action.TargetList.Count && (i < _action.TargetNum || _action.TargetNum == 0 || i == 0); i++)
			{
				BasePartsData target = _action.TargetList[i];
				if (!_action.AlreadyExecedData.ContainsKey(target.Owner))
				{
					_action.AppendTargetNum(target.Owner, 0);
				}

				ExecAction(_action, _skill, target, 0, 0f);
			}
		}

		[global::OpenPCR.Migration("1b596325-3e99-453a-9643-190a91e712c6")]
		public IEnumerator ExecActionWithDelayAndTarget(ActionParameter _action, Skill _skill, BasePartsData _target, float _starttime, bool _first = false, bool _boneCount = true, bool _ignoreCancel = false, BasePartsData _protectedTarget = null)
		{
			if (_action.NoDelayExecByTriggerOnly)
			{
				yield break;
			}

			ProtectionData protectionTargetData = _target.Owner.ComponentAbnormal.ProtectionTargetData;
			if (protectionTargetData != null)
			{
				protectionTargetData.DisableIfOwnerDead();
			}

			for (int i = 0; i < _action.ExecTime.Length; i++)
			{
				if (_boneCount)
				{
					_action.AppendTargetNum(_target.Owner, i);
				}
			}

			float time = _first ? _starttime + _skill.CutInSkipTime : _starttime;
			long[] ids = new long[_action.ExecTime.Length];
			for (int j = 0; j < _action.ExecTime.Length; j++)
			{
				ids[j] = 0L;
				long num = Convert.ToInt64(_action.ActionId);
				while (battleManager.ExecutedActionList.Contains(unchecked(num * 100L + ids[j])))
				{
					ids[j] = unchecked(ids[j] + 1L);
				}

				long actionIndivisualId = unchecked(num * 100L + ids[j]);
				_target.Owner.ActionsTargetOnMe.Add(actionIndivisualId);
				battleManager.ExecutedActionList.Add(actionIndivisualId);
				float waitTime = _action.ExecTime[j];
				while (true)
				{
					time += battleManager.DeltaTime;
					if (!_ignoreCancel && (_skill.Cancel || _action.CancelByIfForAll))
					{
						if (_action.OnActionEnd != null)
						{
							_action.OnActionEnd();
						}

						_target.Owner.ActionsTargetOnMe.Remove(actionIndivisualId);
						battleManager.CallbackActionEnd(actionIndivisualId);
						yield break;
					}

					if (waitTime <= time || (_action.ActionType == eActionType.TRIGER && _action.ActionDetail1 == 4))
					{
						break;
					}

					yield return null;
				}

				ExecAction(_action, _skill, _target, j, time, false, _protectedTarget, false);
				if ((_action.ActionType != eActionType.CONTINUOUS_ATTACK || _action.ActionDetail1 != 3) && (_action.ActionType != eActionType.MODE_CHANGE || _action.ActionDetail1 == 3))
				{
					_target.Owner.ActionsTargetOnMe.Remove(actionIndivisualId);
					battleManager.CallbackActionEnd(actionIndivisualId);
				}
				else if (_action.IdOffsetDictionary.ContainsKey(_target))
				{
					_action.IdOffsetDictionary[_target] = ids[j];
				}
				else
				{
					_action.IdOffsetDictionary.Add(_target, ids[j]);
				}

				if (j == _action.ExecTime.Length - 1 && _action.OnActionEnd != null && _action.ActionType != eActionType.CONTINUOUS_ATTACK && _action.ActionType != eActionType.MOVE)
				{
					_action.OnActionEnd();
				}
			}
		}

		[global::OpenPCR.Migration("e262e0b0-fb0a-4ce2-8758-d26efedddce0")]
		public void ExecAction(ActionParameter _action, Skill _skill, BasePartsData _target, int _num, float _starttime, bool _execChildNoFrame = false, BasePartsData _protectedTarget = null, bool _ignoreProtect = false)
		{
			Dictionary<eValueNumber, double> additionalValue = _action.AdditionalValue;
			Dictionary<eValueNumber, double> multipleValue = _action.MultipleValue;
			Dictionary<eValueNumber, double> divideValue = _action.DivideValue;
			Dictionary<int, bool> enabledChildAction = new Dictionary<int, bool>();
			Func<ActionParameter, eValueNumber, double> func = delegate (ActionParameter _targetAction, eValueNumber _type)
			{
				double additional = 0.0;
				if (additionalValue != null && additionalValue.ContainsKey(_type))
				{
					additional = additionalValue[_type];
				}

				double multiple = 1.0;
				if (multipleValue != null && multipleValue.ContainsKey(_type))
				{
					multiple = multipleValue[_type];
				}

				double divide = 1.0;
				if (divideValue != null && divideValue.ContainsKey(_type) && divideValue[_type] != 0.0)
				{
					divide = divideValue[_type];
				}

				double value = _targetAction.GetActionBaseValue(Owner, _type);
				return multiple * (additional + value) / divide;
			};
			Dictionary<eValueNumber, double> valueDictionary = new Dictionary<eValueNumber, double>(new eValueNumber_DictComparer())
			{
				{
					eValueNumber.VALUE_1,
					func(_action, eValueNumber.VALUE_1)
				},
				{
					eValueNumber.VALUE_2,
					func(_action, eValueNumber.VALUE_2)
				},
				{
					eValueNumber.VALUE_3,
					func(_action, eValueNumber.VALUE_3)
				},
				{
					eValueNumber.VALUE_4,
					func(_action, eValueNumber.VALUE_4)
				},
				{
					eValueNumber.VALUE_5,
					func(_action, eValueNumber.VALUE_5)
				},
				{
					eValueNumber.VALUE_6,
					func(_action, eValueNumber.VALUE_6)
				},
				{
					eValueNumber.VALUE_7,
					func(_action, eValueNumber.VALUE_7)
				}
			};
			bool last = _action.JudgeLastAndNotExeced(_target.Owner, _num);
			bool targetOneParts = _action.AlreadyExecedData[_target.Owner][_num].TargetPartsNumber == 1;
			int detail = _action.ActionType == eActionType.BLIND ? _action.ActionDetail2 : _action.ActionDetail1;
			BasePartsData protectedTargetForChildren = null;
			if (!_target.ResistStatus(_action.ActionType, detail, Owner, last, targetOneParts, _target))
			{
				bool isAlreadyExeced = _action.JudgeIsAlreadyExeced(_target.Owner, _num);
				if (_protectedTarget != null || isAlreadyExeced)
				{
					bool startSourceEffect = _protectedTarget != null;
					if (startSourceEffect)
					{
						_action.SkipAppendIsAlreadyExeced = true;
					}

					UnitComponentAbnormal componentAbnormal = _target.Owner.ComponentAbnormal;
					bool redirected = false;
					if (SkillDefine.ProtectEnableDic[_action.ActionType] && _action.TargetAssignment == eTargetAssignment.OTHER_SIDE && !_ignoreProtect)
					{
						ProtectionData protectionTargetData = componentAbnormal.ProtectionTargetData;
						BasePartsData protectionParts;
						if (_skill.ProtectParts.ContainsKey(_target))
						{
							protectionTargetData.StartEffect(_target.Owner, battleEffectPool, battleManager, _action.ActionType);
							protectionParts = _skill.ProtectParts[_target];
							redirected = true;
						}
						else if (protectionTargetData != null && protectionTargetData.TryExec())
						{
							protectionTargetData.StartEffect(_target.Owner, battleEffectPool, battleManager, _action.ActionType);
							protectionParts = protectionTargetData.ProtectionParts;
							_skill.ProtectParts[_target] = protectionParts;
							redirected = true;
						}
						else
						{
							protectionParts = _target;
							_target = null;
						}

						if (redirected)
						{
							_action.ProtectedParts = _target;
							_action.SkipAppendIsAlreadyExeced = true;
							startSourceEffect = false;
						}

						if (!_action.AlreadyExecedData.ContainsKey(protectionParts.Owner))
						{
							_action.AppendTargetNum(protectionParts.Owner, _num);
						}

						protectedTargetForChildren = _target;
						_target = protectionParts;
					}

					if (startSourceEffect)
					{
						componentAbnormal.ProtectionSourceData.StartEffect(_protectedTarget.Owner, battleEffectPool, battleManager, _action.ActionType);
						protectedTargetForChildren = _protectedTarget;
					}

					if (!redirected || !_target.ResistStatus(_action.ActionType, detail, _target.Owner, last, true, _target))
					{
						_action.ExecAction(Owner, _target, _num, this, _skill, _starttime, enabledChildAction, valueDictionary);
						_action.SkipAppendIsAlreadyExeced = false;
					}
				}
			}

			if (_action.ActionType == eActionType.REFLEXIVE)
			{
				return;
			}

			if (!_action.HitOnceDic.ContainsKey(_target))
			{
				ExecChildrenAction(_action, _skill, _target, _num, _starttime, enabledChildAction, _execChildNoFrame, protectedTargetForChildren);
			}
			else
			{
				if (_num != _action.ExecTime.Length - 1)
				{
					return;
				}

				if (_action.HitOnceDic[_target])
				{
					ExecChildrenAction(_action, _skill, _target, _num, _starttime, enabledChildAction, _execChildNoFrame, protectedTargetForChildren);
					return;
				}

				for (int i = 0; i < _action.ActionChildrenIndexes.Count; i++)
				{
					ActionParameter child = _skill.ActionParameters[_action.ActionChildrenIndexes[i]];
					if (child.OnActionEnd != null)
					{
						child.OnActionEnd();
					}
				}
			}
		}

		[global::OpenPCR.Migration("43b90e98-162b-48fb-a186-dd5289e9e4c9")]
		public void ExecChildrenAction(ActionParameter _action, Skill _skill, BasePartsData _target, int _num, float _starttime, Dictionary<int, bool> _enabledChildAction, bool _execNoFrame, BasePartsData _protectedTarget)
		{
			int count = _action.ActionChildrenIndexes.Count;
			for (int i = 0; i < count; i++)
			{
				ActionParameter actionParameter = _skill.ActionParameters[_action.ActionChildrenIndexes[i]];
				BasePartsData childProtectedTarget;
				if (_protectedTarget != null)
				{
					if (SkillDefine.ProtectEnableDic[actionParameter.ActionType])
					{
						childProtectedTarget = _protectedTarget;
					}
					else
					{
						_target = _protectedTarget;
						childProtectedTarget = null;
					}
				}
				else
				{
					childProtectedTarget = null;
				}

				if ((actionParameter.ActionType == eActionType.MODE_CHANGE && actionParameter.ActionDetail1 == 3) || (_enabledChildAction.ContainsKey(actionParameter.ActionId) && !_enabledChildAction[actionParameter.ActionId]) || (_action.ActionType == eActionType.ATTACK && _num != _action.ExecTime.Length - 1))
				{
					continue;
				}

				if (actionParameter.TargetSort != PriorityPattern.PARENT_TARGET_PARTS)
				{
					if (!_execNoFrame)
					{
						AppendCoroutine(ExecActionWithDelayAndTarget(actionParameter, _skill, _target, _starttime, false, true, false, childProtectedTarget), ePauseType.SYSTEM, _skill.BlackOutTime > 0f ? Owner : null);
						continue;
					}

					actionParameter.AppendTargetNum(_target.Owner, 0);
					ExecAction(actionParameter, _skill, _target, 0, _starttime, true, childProtectedTarget, false);
				}
				else
				{
					if (!_target.Owner.IsPartsBoss || actionParameter.ExecedMultiBossList.Contains(_target.Owner))
					{
						continue;
					}

					foreach (PartsData item in _target.Owner.ComponentMultiPartsBoss.BossPartsListForBattle)
					{
						if (item.GetTargetable())
						{
							if (!_execNoFrame)
							{
								AppendCoroutine(ExecActionWithDelayAndTarget(actionParameter, _skill, item, _starttime, false, true, false, childProtectedTarget), ePauseType.SYSTEM, _skill.BlackOutTime > 0f ? Owner : null);
								continue;
							}

							actionParameter.AppendTargetNum(_target.Owner, 0);
							ExecAction(actionParameter, _skill, _target, 0, _starttime, true, childProtectedTarget, false);
						}
					}

					actionParameter.ExecedMultiBossList.Add(_target.Owner);
				}
			}
		}

		[global::OpenPCR.Migration("aefd116f-1434-464f-8f27-d25b24ef0280")]
		private void initWhenNoTarget(ActionParameter _action, Skill _skill)
		{
			_action.OnInitWhenNoTarget.Call();
			Queue<int> val = new Queue<int>((IEnumerable<int>)_action.ActionChildrenIndexes);
			while (val.Count > 0)
			{
				ActionParameter actionParameter = _skill.ActionParameters[val.Dequeue()];
				actionParameter.OnInitWhenNoTarget.Call();
				for (int i = 0; i < actionParameter.ActionChildrenIndexes.Count; i++)
				{
					val.Enqueue(actionParameter.ActionChildrenIndexes[i]);
				}
			}
		}

		[global::OpenPCR.Migration("7d42c97b-3b2d-4c92-9b53-362b759c169d")]
		private IEnumerator createNormalPrefabWithDelay(SkillEffectBase _skilleffect, Skill _skill, bool _first = false, bool _skipCutIn = false, bool _isFirearmEndEffect = false, bool _modeChangeEndEffect = false)
		{
			float time = (_first ? _skill.CutInSkipTime : 0f);
			string key = ((Owner.IsLeftDir || Owner.IsForceLeftDirOrPartsBoss) ? _skilleffect.PrefabLeft.name : _skilleffect.Prefab.name);
			if (battleManager.ReActionEffectDictionary.ContainsKey(key))
			{
				GameObject gameObject = battleManager.ReActionEffectDictionary[key];
				if (gameObject == null)
				{
					yield break;
				}

				battleManager.ReActionEffectDictionary.Remove(key);
				global::OpenPCR.Runtime.Api.Object.Destroy(gameObject);
			}
			else if (_skilleffect.IsReaction)
			{
				battleManager.ReActionEffectDictionary.Add(key, null);
			}

			int generateEffectCount = 0;
			float[] execTime = _skilleffect.ExecTime;
			switch (_skilleffect.EffectBehavior)
			{
				case eEffectBehavior.NORMAL:
				case eEffectBehavior.FIREARM:
				case eEffectBehavior.WORLD_POS_CENTER:
				case eEffectBehavior.TARGET_CENTER:
				case eEffectBehavior.SKILL_AREA_RANDOM:
					generateEffectCount = execTime.Length;
					break;
				case eEffectBehavior.SERIES:
				case eEffectBehavior.SERIES_FIREARM:
					generateEffectCount = _skilleffect.TargetAction.TargetList.Count;
					break;
			}

			bool[] execed = new bool[generateEffectCount];
			for (int execTimeIndex = 0; execTimeIndex < generateEffectCount; execTimeIndex++)
			{
				float waitTime = 0f;
				if (_skill.AnimId != eAnimKitCharacterAnimeId.ATTACK || !UseDefaultDelay || _skilleffect.EffectBehavior != eEffectBehavior.FIREARM || !BattleDefine.WEAPON_EFFECT_DELAY_DIC.TryGetValue(Owner.WeaponMotionType, out waitTime))
				{
					waitTime = ((execTime.Length - 1 >= execTimeIndex) ? execTime[execTimeIndex] : ((execTime.Length < 2) ? execTime[0] : ((execTime[execTime.Length - 1] - execTime[execTime.Length - 2]) * (float)(execTimeIndex - execTime.Length + 1) + execTime[execTime.Length - 1])));
				}

				if (execed[execTimeIndex])
				{
					continue;
				}

				float startTime = 0f;
				if (_skipCutIn && (waitTime > _skill.CutInSkipTime || BattleUtil.Approximately(waitTime, _skill.CutInSkipTime)))
				{
					continue;
				}

				if (!_skipCutIn && waitTime < _skill.CutInSkipTime && !_isFirearmEndEffect)
				{
					if (!_skilleffect.PlayWithCutIn || !Owner.ComponentUnionBurst.PlayCutInFlag || Owner.ComponentUnionBurst.MovieId == 0)
					{
						continue;
					}

					startTime = _skill.CutInSkipTime - waitTime;
				}

				do
				{
					yield return null;
					time += battleManager.DeltaTime;
					if (_skill.Cancel || !Owner.gameObject.activeSelf || (_modeChangeEndEffect && Owner.ComponentAbnormal.IsUnableActionState()))
					{
						yield break;
					}
				}
				while (!(waitTime <= time));
				execed[execTimeIndex] = true;
				BasePartsData firearmEndTarget = null;
				if (_skilleffect.EffectBehavior == eEffectBehavior.FIREARM || _skilleffect.EffectBehavior == eEffectBehavior.SERIES_FIREARM)
				{
					switch (_skilleffect.FireArmEndTarget)
					{
						case eEffectTarget.FORWARD_TARGET:
						case eEffectTarget.BACK_TARGET:
							{
								bool flag = _skilleffect.FireArmEndTarget == eEffectTarget.FORWARD_TARGET == !Owner.IsOther;
								List<BasePartsData> list = new List<BasePartsData>(_skilleffect.TargetAction.TargetList.GetRange(0, Mathf.Min(_skilleffect.TargetAction.TargetNum, _skilleffect.TargetAction.TargetList.Count)));
								list.Sort((BasePartsData a, BasePartsData b) => a.GetPosition().x.CompareTo(b.GetPosition().x));
								if (list.Count == 0)
								{
									yield break;
								}

								firearmEndTarget = list[(!flag) ? (list.Count - 1) : 0];
								break;
							}

						case eEffectTarget.OWNER:
						case eEffectTarget.TARGET_CENTER_FOR_FIREARM:
							firearmEndTarget = Owner.ComponentMultiPartsBoss.GetFirstParts(_owner: true);
							break;
						case eEffectTarget.ALL_TARGET:
							firearmEndTarget = _skilleffect.TargetAction.TargetList[execTimeIndex];
							break;
					}
				}

				switch (_skilleffect.EffectTarget)
				{
					case eEffectTarget.OWNER:
					case eEffectTarget.TARGET_CENTER_FOR_FIREARM:
						createNormalEffectPrefab(_skilleffect, _skill, Owner.ComponentMultiPartsBoss.GetFirstParts(_owner: true), firearmEndTarget, actionStart: true, startTime, _skipCutIn, execTimeIndex, _modeChangeEndEffect);
						break;
					case eEffectTarget.FORWARD_TARGET:
					case eEffectTarget.BACK_TARGET:
						{
							bool flag2 = _skilleffect.EffectTarget == eEffectTarget.FORWARD_TARGET == !Owner.IsOther;
							List<BasePartsData> list4 = new List<BasePartsData>(_skilleffect.TargetAction.TargetList.GetRange(0, Mathf.Min(_skilleffect.TargetAction.TargetNum, _skilleffect.TargetAction.TargetList.Count)));
							list4.Sort((BasePartsData a, BasePartsData b) => a.GetPosition().x.CompareTo(b.GetPosition().x));
							if (list4.Count != 0)
							{
								createNormalEffectPrefab(_skilleffect, _skill, list4[(!flag2) ? (list4.Count - 1) : 0], firearmEndTarget, actionStart: true, startTime, _skipCutIn, execTimeIndex);
							}

							break;
						}

					case eEffectTarget.ALL_TARGET:
						{
							eEffectBehavior effectBehavior = _skilleffect.EffectBehavior;
							if ((uint)(effectBehavior - 5) <= 1u)
							{
								createNormalEffectPrefab(_skilleffect, _skill, _skilleffect.TargetAction.TargetList[execTimeIndex], firearmEndTarget, actionStart: true, startTime, _skipCutIn, execTimeIndex);
								break;
							}

							int num2 = 0;
							for (int count = _skilleffect.TargetAction.TargetList.Count; num2 < count && num2 < _skilleffect.TargetAction.TargetNum; num2++)
							{
								createNormalEffectPrefab(_skilleffect, _skill, _skilleffect.TargetAction.TargetList[num2], firearmEndTarget, num2 == 0, startTime, _skipCutIn, execTimeIndex);
							}

							break;
						}

					case eEffectTarget.ALL_OTHER:
						{
							List<UnitCtrl> list3 = (Owner.IsOther ? battleManager.UnitList : battleManager.EnemyList);
							for (int num3 = 0; num3 < list3.Count; num3++)
							{
								if ((long)list3[num3].ComponentParameter.Hp != 0L && !list3[num3].ComponentAbnormal.IsStealth && !list3[num3].ComponentAbnormal.JudgeIsEtherialization(Owner))
								{
									createNormalEffectPrefab(_skilleffect, _skill, list3[num3].ComponentMultiPartsBoss.GetFirstParts(_owner: true), firearmEndTarget, actionStart: true, startTime, _skipCutIn, execTimeIndex);
								}
							}

							break;
						}

					case eEffectTarget.ALL_UNIT_EXCEPT_OWNER:
						{
							List<UnitCtrl> list2 = (Owner.IsOther ? battleManager.EnemyList : battleManager.UnitList);
							for (int num = 0; num < list2.Count; num++)
							{
								if (list2[num] != Owner && !list2[num].ComponentState.IsDead && !list2[num].ComponentAbnormal.IsStealth && !list2[num].ComponentAbnormal.JudgeIsEtherialization(Owner))
								{
									createNormalEffectPrefab(_skilleffect, _skill, list2[num].ComponentMultiPartsBoss.GetFirstParts(_owner: true), firearmEndTarget, actionStart: true, startTime, _skipCutIn, execTimeIndex);
								}
							}

							break;
						}
				}
			}
		}

		[global::OpenPCR.Migration("2908f2cd-06a0-49d2-bcb2-ce4ad65379bb")]
		private IEnumerator createNormalPrefabWithDelayAndTarget(SkillEffectBase _skilleffect, Skill _skill, float _delay, BasePartsData _target, bool _first)
		{
			float time = (_first ? _skill.CutInSkipTime : 0f);
			if (_delay < time)
			{
				yield break;
			}

			while (true)
			{
				time += battleManager.DeltaTime;
				if (_skill.Cancel)
				{
					yield break;
				}

				if (_delay <= time)
				{
					break;
				}

				yield return null;
			}

			if (_skilleffect.EffectTarget == eEffectTarget.OWNER)
			{
				createNormalEffectPrefab(_skilleffect, _skill, Owner.ComponentMultiPartsBoss.GetFirstParts(_owner: true), null, actionStart: false, 0f, _skipCutIn: false);
			}
			else
			{
				createNormalEffectPrefab(_skilleffect, _skill, _target, null, actionStart: false, 0f, _skipCutIn: false);
			}
		}

		[global::OpenPCR.Migration("dd098c01-a3c6-43a4-a6f7-296d71a46262")]
		private void createNormalEffectPrefab(SkillEffectBase _skillEffect, Skill _skill, BasePartsData _target, BasePartsData _firearmEndTarget, bool actionStart, float _starttime, bool _skipCutIn, int execTimeIndex = 0, bool _modeChangeEndEffect = false)
		{
			if ((_skillEffect.EffectBehavior == eEffectBehavior.SERIES_FIREARM && !(_skillEffect.FireAction is AttackAction) && !(_skillEffect.FireAction is RatioDamageAction) && !(_skillEffect.FireAction is UpperLimitAttackAction) && !(_skillEffect.FireAction is HealAction) && _skillEffect.AppendAndJudgeAlreadyExeced(_firearmEndTarget.Owner)) || (_skillEffect.TargetBranchId != 0 && _skillEffect.TargetBranchId != _skill.EffectBranchId) || (_skillEffect.TargetMotionIndex == 1 && _skill.LoopEffectAlreadyDone) || (_modeChangeEndEffect && StopModeChangeEndEffectCalled))
			{
				return;
			}

			GameObject prefab = null;
			SkillEffectCtrl skillEffectCtrl = createPrefab(_skillEffect, _skill, _target, ref prefab);
			skillEffectCtrl.InitializeSort();
			skillEffectCtrl.SetStartTime(_starttime);
			UnitComponentAbnormal componentAbnormal = Owner.ComponentAbnormal;
			if (componentAbnormal.ToadDatas.Count > 0)
			{
				skillEffectCtrl.PlaySe(componentAbnormal.ToadDatas[0].BattleSpineController.UnitId, Owner.IsLeftDir);
			}
			else
			{
				skillEffectCtrl.PlaySe(Owner.ComponentSound.SoundUnitId, Owner.IsLeftDir);
			}

			skillEffectCtrl.SetPossitionAppearanceType(_skillEffect, _target, Owner, _skill);
			skillEffectCtrl.ExecAppendCoroutine((_skill.BlackOutTime > 0f) ? Owner : null, skillEffectCtrl is AwakeUbNoneStopEffect);
			skillEffectCtrl.SetTimeToDieByStartHp(Owner.StartHpPercent);
			if (_modeChangeEndEffect)
			{
				componentAbnormal.ModeChangeEndEffectList.Add(skillEffectCtrl);
			}

			if (_skipCutIn)
			{
				skillEffectCtrl.OnAwakeWhenSkipCutIn();
			}

			switch (_skillEffect.EffectBehavior)
			{
				case eEffectBehavior.SKILL_AREA_RANDOM:
					{
						Vector3 position2 = default(Vector3);
						int attackSide = GetAttackSide(_skillEffect.TargetAction.Direction, Owner);
						float num2 = ((attackSide < 1) ? (-_skillEffect.TargetAction.TargetWidth) : 0f);
						float num3 = ((attackSide > -1) ? _skillEffect.TargetAction.TargetWidth : 0f);
						float num4 = ((_skillEffect.TargetAction.ActionType == eActionType.REFLEXIVE) ? _skillEffect.TargetAction.TargetList[0].GetPosition().x : transform.position.x);
						num2 += num4;
						num3 += num4;
						if (num2 < -BattleDefine.BATTLE_FIELD_SIZE)
						{
							num2 = -BattleDefine.BATTLE_FIELD_SIZE;
						}

						if (num3 > BattleDefine.BATTLE_FIELD_SIZE)
						{
							num3 = BattleDefine.BATTLE_FIELD_SIZE;
						}

						position2.x = (num2 + (num3 - num2) * BattleManager.Random(0f, 1f)) / 540f;
						position2.y = _skillEffect.CenterY + _skillEffect.DeltaY * BattleManager.Random(-1f, 1f) + 9.259259f;
						skillEffectCtrl.transform.position = position2;
						break;
					}

				case eEffectBehavior.TARGET_CENTER:
					{
						Vector3 position = default(Vector3);
						int num = 0;
						for (int count = _skillEffect.TargetAction.TargetList.Count; num < count; num++)
						{
							position += _skillEffect.TargetAction.TargetList[num].GetPosition() / count;
						}

						skillEffectCtrl.transform.position = position;
						break;
					}

				case eEffectBehavior.WORLD_POS_CENTER:
					switch (_skillEffect.TrackDimension)
					{
						case eTrackDimension.NONE:
							skillEffectCtrl.transform.position = new Vector3(0f, 9.259259f, 0f);
							break;
						case eTrackDimension.X:
							skillEffectCtrl.SetWorldPosY(9.259259f);
							break;
						case eTrackDimension.Y:
							skillEffectCtrl.SetWorldPosX(0f);
							break;
					}

					break;
				case eEffectBehavior.FIREARM:
				case eEffectBehavior.SERIES_FIREARM:
					{
						FirearmCtrl firearmCtrl = skillEffectCtrl as FirearmCtrl;
						List<ActionParameter> list = new List<ActionParameter>();
						if (_skillEffect.FireActionId != -1 && actionStart)
						{
							list.Add(_skillEffect.FireAction);
						}

						bool isAbsolute;
						Vector3 targetPosition;
						if (_skillEffect.FireArmEndTarget == eEffectTarget.TARGET_CENTER_FOR_FIREARM)
						{
							float minimum = float.MaxValue;
							float maximum = float.MinValue;
							for (int targetIndex = 0; targetIndex < _skillEffect.TargetAction.TargetList.Count; targetIndex++)
							{
								int targetNum = _skillEffect.TargetAction.TargetNum;
								if (targetIndex >= targetNum && targetIndex != 0 && targetNum != 0)
								{
									break;
								}

								BasePartsData parts = _skillEffect.TargetAction.TargetList[targetIndex];
								float maximumCandidate = parts.GetPosition().x;
								maximum = maximum > maximumCandidate ? maximum : maximumCandidate;
								float minimumCandidate = parts.GetPosition().x;
								minimum = minimum < minimumCandidate ? minimum : minimumCandidate;
							}

							float ownerPositionY = transform.position.y;
							float centerX = (minimum + maximum) * 0.5f;
							isAbsolute = true;
							targetPosition = new Vector3(centerX, ownerPositionY, 0f);
						}
						else
						{
							isAbsolute = _skillEffect.IsAbsoluteFireArm;
							Vector3 ownerPosition = transform.position;
							float direction = Owner.IsLeftDir ? -1f : 1f;
							targetPosition = ownerPosition + direction * new Vector3(_skillEffect.AbsoluteFireDistance, 0f, 0f);
						}

						List<SkillEffectBase> endEffects = actionStart && _skillEffect is NormalSkillEffect normalEffect ? normalEffect.FireArmEndEffects : new List<SkillEffectBase>();
						firearmCtrl.Initialize(_firearmEndTarget, list, _skill, endEffects, Owner, _skillEffect.Height, _skill.BlackOutTime > 0f, isAbsolute, targetPosition, _skillEffect.ShakeEffects, _skillEffect.FireArmEndTargetBone);
						if (_skillEffect.EffectBehavior == eEffectBehavior.FIREARM)
						{
							firearmCtrl.OnHitAction = onFirearmHit;
							break;
						}

						firearmCtrl.OnHitAction = delegate (FirearmCtrl fctrl)
						{
							for (int i = 0; i < fctrl.ShakeEffects.Count; i++)
							{
								if (fctrl.ShakeEffects[i].TargetMotion == 0)
								{
									AppendCoroutine(StartShakeWithDelay(fctrl.ShakeEffects[i], fctrl.Skill), ePauseType.VISUAL, (fctrl.Skill.BlackOutTime > 0f) ? Owner : null);
								}
							}

							if (_skillEffect.FireActionId != -1)
							{
								AppendCoroutine(ExecActionWithDelayAndTarget(_skillEffect.FireAction, _skill, _firearmEndTarget, 0f), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? Owner : null);
							}

							if (_skillEffect is NormalSkillEffect hitNormalEffect)
							{
								int j = 0;
								for (int count2 = hitNormalEffect.FireArmEndEffects.Count; j < count2; j++)
								{
									AppendCoroutine(createNormalPrefabWithDelayAndTarget(hitNormalEffect.FireArmEndEffects[j], _skill, 0f, _firearmEndTarget, _first: false), ePauseType.VISUAL, (_skill.BlackOutTime > 0f) ? Owner : null);
								}
							}
						};
						break;
					}

				case eEffectBehavior.SERIES:
					if (_skillEffect.FireActionId != -1)
					{
						AppendCoroutine(ExecActionWithDelayAndTarget(_skillEffect.FireAction, _skill, _target, 0f), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? Owner : null);
					}

					break;
			}

			skillEffectCtrl.RestartTween();
		}

		[global::OpenPCR.Migration("76cb0155-dff6-408a-abf7-84816964f02b")]
		private SkillEffectCtrl createPrefab(SkillEffectBase skillEffect, Skill skill, BasePartsData target, ref GameObject prefab)
		{
			SkillEffectCtrl skillEffectCtrl = null;
			prefab = ((Owner.IsLeftDir || Owner.IsForceLeftDirOrPartsBoss) ? skillEffect.PrefabLeft : skillEffect.Prefab);
			string key = prefab.name;
			skillEffectCtrl = battleEffectPool.GetEffect(prefab, Owner);
			skillEffectCtrl.transform.parent = ExceptUIRoot.Transform;
			skill.EffectObjs.Add(skillEffectCtrl);
			if (skillEffect.TargetMotionIndex == 1)
			{
				skill.LoopEffectObjs.Add(skillEffectCtrl);
				SkillEffectCtrl skillEffectCtrl2 = skillEffectCtrl;
				skillEffectCtrl2.OnEffectEnd = (Action<SkillEffectCtrl>)Delegate.Combine(skillEffectCtrl2.OnEffectEnd, (Action<SkillEffectCtrl>)delegate (SkillEffectCtrl obj)
				{
					skill.LoopEffectObjs.Remove(obj);
				});
			}

			if (skillEffectCtrl.IsRepeat && !(skillEffectCtrl is FirearmCtrl) && !(skillEffectCtrl is AnimKitNormalEffect))
			{
				target.Owner.RepeatEffectList.Add(skillEffectCtrl);
				SkillEffectCtrl skillEffectCtrl3 = skillEffectCtrl;
				skillEffectCtrl3.OnEffectEnd = (Action<SkillEffectCtrl>)Delegate.Combine(skillEffectCtrl3.OnEffectEnd, (Action<SkillEffectCtrl>)delegate (SkillEffectCtrl obj)
				{
					target.Owner.RepeatEffectList.Remove(obj);
				});
			}

			SkillEffectCtrl skillEffectCtrl4 = skillEffectCtrl;
			skillEffectCtrl4.OnEffectEnd = (Action<SkillEffectCtrl>)Delegate.Combine(skillEffectCtrl4.OnEffectEnd, (Action<SkillEffectCtrl>)delegate (SkillEffectCtrl destroyedEffect)
			{
				skill.EffectObjs.Remove(destroyedEffect);
			});
			if (skillEffect.IsReaction)
			{
				battleManager.ReActionEffectDictionary[key] = skillEffectCtrl.gameObject;
			}

			return skillEffectCtrl;
		}

		[global::OpenPCR.Migration("eb41e1f9-b231-4c5a-beef-c7da5a9e8de2")]
		private void onFirearmHit(FirearmCtrl firearmCtrl)
		{
			if (firearmCtrl == null || firearmCtrl.FireTarget == null || firearmCtrl.Skill == null)
			{
				return;
			}

			for (int i = 0; i < firearmCtrl.ShakeEffects.Count; i++)
			{
				if (firearmCtrl.ShakeEffects[i].TargetMotion == 0)
				{
					AppendCoroutine(StartShakeWithDelay(firearmCtrl.ShakeEffects[i], firearmCtrl.Skill), ePauseType.VISUAL, (firearmCtrl.Skill.BlackOutTime > 0f) ? Owner : null);
				}
			}

			int j = 0;
			for (int count = firearmCtrl.EndActions.Count; j < count; j++)
			{
				ActionParameter action = firearmCtrl.EndActions[j];
				ExecUnitActionWithDelay(action, firearmCtrl.Skill, _first: false, _boneCount: true);
			}

			int k = 0;
			for (int count2 = firearmCtrl.SkillHitEffects.Count; k < count2; k++)
			{
				SkillEffectBase normalSkillEffect = firearmCtrl.SkillHitEffects[k];
				if (!battleManager.IsSkipping() || !normalSkillEffect.IsVisualEffect())
				{
					AppendCoroutine(createNormalPrefabWithDelay(normalSkillEffect, firearmCtrl.Skill, _first: false, _skipCutIn: false, _isFirearmEndEffect: true), ePauseType.VISUAL, (firearmCtrl.Skill.BlackOutTime > 0f) ? Owner : null);
				}
			}
		}

		[global::OpenPCR.Migration("204eac16-d4f6-4dd9-b62e-ca0f20e3f239")]
		private void searchTargetUnit(ActionParameter actionParameter, Vector3 basePosition, Skill skill, bool _considerBodyWidth)
		{
			float x = transform.parent.lossyScale.x;
			actionParameter.TargetList.Clear();
			UnitComponentAbnormal componentAbnormal = Owner.ComponentAbnormal;
			bool isOther = Owner.IsOther;
			List<UnitCtrl> list = (componentAbnormal.IsConfusionOrConvert() ? ((!isOther) ? battleManager.UnitList : battleManager.EnemyList) : (isOther ? battleManager.UnitList : battleManager.EnemyList));
			List<UnitCtrl> list2 = (isOther ? battleManager.EnemyList : battleManager.UnitList);
			List<UnitCtrl> list3 = null;
			switch (actionParameter.TargetAssignment)
			{
				case eTargetAssignment.OTHER_SIDE:
					list3 = list;
					break;
				case eTargetAssignment.OLD_OWNER_SIDE:
				case eTargetAssignment.OWNER_SITE:
					list3 = list2;
					break;
				case eTargetAssignment.ALL:
					list3 = new List<UnitCtrl>(battleManager.UnitList);
					list3.AddRange(battleManager.EnemyList);
					break;
			}

			switch (convertPriorityPattern(actionParameter.TargetSort))
			{
				case PriorityPattern.SUMMON:
				case PriorityPattern.ALL_SUMMON_RANDOM:
					list3 = list3.FindAll((UnitCtrl e) => e.IsSummonOrPhantom);
					break;
				case PriorityPattern.OWN_SUMMON_RANDOM:
					list3 = Owner.SummonUnitDictionary.Values.ToList().FindAll((UnitCtrl e) =>
					{
						UnitComponentState componentState = e.ComponentState;
						return !componentState.IdleOnly && !componentState.IsDead;
					});
					break;
				case PriorityPattern.ATK_MAGIC:
					list3 = list3.FindAll((UnitCtrl e) => e.AtkType == 2);
					break;
				case PriorityPattern.ATK_PHYSICS:
					list3 = list3.FindAll((UnitCtrl e) => e.AtkType == 1);
					break;
				case PriorityPattern.NEAR_MY_TEAM_WITHOUT_OWNER:
				case PriorityPattern.ENERGY_ASC_BACK_WITHOUT_OWNER:
				case PriorityPattern.ATK_DEC_FORWARD_WITHOUT_OWNER:
				case PriorityPattern.HP_ASC_WITHOUT_OWNER:
				case PriorityPattern.MAGIC_STR_DEC_WITHOUT_OWNER_FORWARD:
					list3 = list3.FindAll((UnitCtrl e) => e != Owner);
					break;
				case PriorityPattern.ATK_DEC_FORWARD_WITHOUT_OWNER_AND_MAGIC_CHARA:
					list3 = list3.FindAll((UnitCtrl e) => e != Owner && e.AtkType == 1);
					break;
				case PriorityPattern.FLIGHT_ONLY:
					list3 = list3.FindAll((UnitCtrl e) => e.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT));
					break;
				case PriorityPattern.NEAR_WITHOUT_TARGET_ABNORMAL_STATE:
				case PriorityPattern.FAR_WITHOUT_TARGET_ABNORMAL_STATE:
					{
						eAbnormalState targetAbnormalState = (eAbnormalState)((int)actionParameter.TargetSort % 1000);
						list3 = list3.FindAll((UnitCtrl e) => !e.ComponentAbnormal.IsAbnormalState(targetAbnormalState));
						break;
					}

				case PriorityPattern.NEAR_SEAL_ENCHANTED:
					{
						eStateIconType iconType = (eStateIconType)((int)actionParameter.TargetSort % 1000);
						list3 = list3.FindAll((UnitCtrl e) =>
						{
							Dictionary<eStateIconType, SealData> sealDictionary = e.ComponentAbnormal.SealDictionary;
							return sealDictionary.ContainsKey(iconType) && sealDictionary[iconType].GetCurrentCount() > 0;
						});
						break;
					}

				case PriorityPattern.NEAR_TALENT:
					{
						int talentId = (int)actionParameter.TargetSort % 1000;
						list3 = list3.FindAll((UnitCtrl e) => e.TalentIds.Contains(talentId));
						break;
					}

				case PriorityPattern.NEAR_TALENT_WITHOUT_OWNER:
					{
						int talentId = (int)actionParameter.TargetSort % 1000;
						list3 = list3.FindAll((UnitCtrl e) => e.TalentIds.Contains(talentId) && e != Owner);
						break;
					}
			}

			if (actionParameter.TargetSort == PriorityPattern.BACK || actionParameter.TargetSort == PriorityPattern.FORWARD)
			{
				int num2 = 0;
				for (int count = list3.Count; num2 < count; num2++)
				{
					UnitCtrl unitCtrl = list3[num2];
					if (!judgeIsTarget(unitCtrl, actionParameter))
					{
						continue;
					}

					UnitComponentMultiPartsBoss componentMultiPartsBoss = unitCtrl.ComponentMultiPartsBoss;
					if (unitCtrl.IsPartsBoss)
					{
						List<PartsData> bossPartsList = componentMultiPartsBoss.BossPartsListForBattle;
						for (int num3 = 0; num3 < bossPartsList.Count; num3++)
						{
							PartsData partsData = bossPartsList[num3];
							if (partsData.GetTargetable())
							{
								actionParameter.TargetList.Add(partsData);
							}
						}
					}
					else
					{
						actionParameter.TargetList.Add(componentMultiPartsBoss.GetFirstParts());
					}
				}

				filterTargetByFlight(actionParameter);
				return;
			}

			float num = actionParameter.TargetWidth;
			int attackSide = GetAttackSide(actionParameter.Direction, Owner);
			if (num <= 0f)
			{
				num = ((skill.SkillId == 1) ? Owner.ComponentSearchArea.SearchAreaSize : skill.skillAreaWidth);
			}

			float start = ((attackSide < 1) ? (-num) : 0f);
			float end = ((attackSide > -1) ? num : 0f);
			BasePartsData basePartsData = null;
			int num4 = 0;
			for (int count2 = list3.Count; num4 < count2; num4++)
			{
				UnitCtrl unitCtrl2 = list3[num4];
				UnitComponentMultiPartsBoss componentMultiPartsBoss = unitCtrl2.ComponentMultiPartsBoss;
				if (!unitCtrl2.IsPartsBoss)
				{
					if (judgeIsInTargetArea(actionParameter, basePosition, _considerBodyWidth, x, start, end, componentMultiPartsBoss.GetFirstParts()))
					{
						if (judgeIsTarget(unitCtrl2, actionParameter))
						{
							actionParameter.TargetList.Add(componentMultiPartsBoss.GetFirstParts());
						}
						else if ((long)unitCtrl2.ComponentParameter.Hp == 0L)
						{
							basePartsData = componentMultiPartsBoss.GetFirstParts();
						}
					}

					continue;
				}

				bool flag = actionParameter.TargetSort == PriorityPattern.HP_ASC || actionParameter.TargetSort == PriorityPattern.HP_DEC || actionParameter.TargetSort == PriorityPattern.ENERGY_ASC || actionParameter.TargetSort == PriorityPattern.ENERGY_DEC || actionParameter.TargetSort == PriorityPattern.ENERGY_REDUCING;
				BasePartsData basePartsData2 = null;
				List<PartsData> bossPartsList = componentMultiPartsBoss.BossPartsListForBattle;
				for (int num5 = 0; num5 < bossPartsList.Count; num5++)
				{
					PartsData partsData2 = bossPartsList[num5];
					if (!judgeIsInTargetArea(actionParameter, basePosition, _considerBodyWidth, x, start, end, partsData2) || !judgeIsTarget(unitCtrl2, actionParameter))
					{
						continue;
					}

					if (flag)
					{
						if (basePartsData2 == null)
						{
							basePartsData2 = partsData2;
							continue;
						}

						float num6 = Mathf.Abs(basePartsData2.GetPosition().x - Owner.transform.position.x);
						float num7 = Mathf.Abs(partsData2.GetPosition().x - Owner.transform.position.x);
						if (num6 > num7 || BattleUtil.Approximately(num6, num7))
						{
							basePartsData2 = partsData2;
						}
					}
					else
					{
						actionParameter.TargetList.Add(partsData2);
					}
				}

				if (basePartsData2 != null)
				{
					actionParameter.TargetList.Add(basePartsData2);
				}
			}

			filterTargetByFlight(actionParameter);
			if (actionParameter.TargetSort == PriorityPattern.NEAR && actionParameter.TargetList.Count == 0 && basePartsData != null && !componentAbnormal.IsConfusionOrConvert())
			{
				actionParameter.TargetList.Add(basePartsData);
			}
		}

		[global::OpenPCR.Migration("0961c050-bf67-440b-8eab-4d7e6010c23d")]
		private bool isInActionTargetArea(Skill _skill, ActionParameter _action, Vector3 _basePosition, bool _considerBodyWidth, BasePartsData _checkUnitCtrl)
		{
			int attackSide = GetAttackSide(_action.Direction, Owner);
			float num = _action.TargetWidth;
			if (num <= 0f)
			{
				num = ((_skill.SkillId == 1) ? Owner.ComponentSearchArea.SearchAreaSize : _skill.skillAreaWidth);
			}

			float start = ((attackSide != 1) ? (-num) : 0f);
			float end = ((attackSide != -1) ? num : 0f);
			return judgeIsInTargetArea(_action, _basePosition, _considerBodyWidth, transform.parent.lossyScale.x, start, end, _checkUnitCtrl);
		}

		[global::OpenPCR.Migration("c4d7a95d-9fc1-4d4f-aade-834f1b53b4fe")]
		private bool judgeIsInTargetArea(ActionParameter _actionParameter, Vector3 _basePosition, bool _considerBodyWidth, float _parentLossyScale, float _start, float _end, BasePartsData _unitCtrl)
		{
			if ((uint)((int)_actionParameter.Direction - 7) <= 2u && _unitCtrl.Owner.IsSummonOrPhantom)
			{
				return false;
			}

			if (!_unitCtrl.GetTargetable())
			{
				return false;
			}

			float num = _unitCtrl.GetPosition().x / _parentLossyScale - _basePosition.x / _parentLossyScale;
			float num2 = _unitCtrl.GetBodyWidth();
			if (_considerBodyWidth)
			{
				num2 += Owner.BodyWidth;
			}

			float num3 = _start - num2 * 0.5f;
			float num4 = _end + num2 * 0.5f;
			if ((!(num >= num3) || !(num <= num4)) && !BattleUtil.Approximately(num, num3) && !BattleUtil.Approximately(num, num4))
			{
				return _actionParameter.Direction == DirectionType.ALL || _actionParameter.Direction == DirectionType.ALL_INCLUDE_FLIGHT || _actionParameter.Direction == DirectionType.ALL_WITHOUT_SUMMON;
			}

			return true;
		}

		[global::OpenPCR.Migration("94de3b7d-699f-4c85-9563-1fbec3527832")]
		private bool judgeIsTarget(UnitCtrl _unitCtrl, ActionParameter _actionParameter)
		{
			UnitComponentAbnormal componentAbnormal = _unitCtrl.ComponentAbnormal;
			UnitComponentState componentState = _unitCtrl.ComponentState;
			if (componentAbnormal.JudgeIsEtherializationConsiderDebuffImmune(Owner, _actionParameter) || componentAbnormal.IsStealth)
			{
				return false;
			}

			if (_unitCtrl.IsPhantom && _actionParameter.TargetSort != PriorityPattern.OWN_SUMMON_RANDOM)
			{
				return false;
			}

			if ((componentState.IsDead || (long)_unitCtrl.ComponentParameter.Hp <= 0L) && !componentState.HasUnDeadTime)
			{
				return false;
			}

			if (_actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE && Owner == _unitCtrl)
			{
				return false;
			}

			if (_actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE && _actionParameter.TargetNum == 1)
			{
				return !componentAbnormal.IsAbnormalState(eAbnormalState.SPY);
			}

			return true;
		}

		[global::OpenPCR.Migration("4def2a32-2eb7-4e18-b9cc-46cb7bec77df")]
		public static int GetAttackSide(DirectionType _direction, UnitCtrl _owner)
		{
			if ((uint)_direction <= 7u && ((1 << (int)_direction) & 0x92) != 0)
			{
				return _owner.IsLeftDir ? -1 : 1;
			}

			return 0;
		}

		[global::OpenPCR.Migration("64aea194-9b7a-4068-969e-dc245a49512b")]
		private PriorityPattern convertPriorityPattern(PriorityPattern _pattern)
		{
			if ((int)_pattern >= 1000)
			{
				return (PriorityPattern)((int)_pattern / 1000 * 1000);
			}

			return _pattern;
		}

		[global::OpenPCR.Migration("7a84fd9f-d445-453a-bf38-ace7b5ea6a6f")]
		private void sortTargetListByTargetPattern(ActionParameter _actionParameter, Transform _baseTransform, bool _quiet)
		{
			List<BasePartsData> targetList = _actionParameter.TargetList;
			switch (convertPriorityPattern(_actionParameter.TargetSort))
			{
				case PriorityPattern.RANDOM:
				case PriorityPattern.RANDOM_ONCE:
				case PriorityPattern.ALL_SUMMON_RANDOM:
				case PriorityPattern.OWN_SUMMON_RANDOM:
					{
						int num = targetList.Count;
						while (num > 1)
						{
							int index = BattleManager.Random(0, num);
							num--;
							BasePartsData value = targetList[index];
							targetList[index] = targetList[num];
							targetList[num] = value;
						}

						break;
					}

				case PriorityPattern.HP_ASC:
					BattleUnitComparator.QuickSort(targetList, BattleUnitComparator.CompareLifeAsc);
					break;
				case PriorityPattern.HP_DEC:
					BattleUnitComparator.QuickSort(targetList, BattleUnitComparator.CompareLifeDec);
					break;
				case PriorityPattern.FAR:
				case PriorityPattern.FAR_WITHOUT_TARGET_ABNORMAL_STATE:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareDistanceDec);
					break;
				case PriorityPattern.NEAR:
				case PriorityPattern.NEAR_MY_TEAM_WITHOUT_OWNER:
				case PriorityPattern.NEAR_WITHOUT_TARGET_ABNORMAL_STATE:
				case PriorityPattern.NEAR_SEAL_ENCHANTED:
				case PriorityPattern.NEAR_TALENT:
				case PriorityPattern.NEAR_TALENT_WITHOUT_OWNER:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareDistanceAsc);
					break;
				case PriorityPattern.BOSS:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareBoss);
					break;
				case PriorityPattern.OWNER:
					targetList.Clear();
					targetList.Add(Owner.ComponentMultiPartsBoss.GetFirstParts(_owner: false, -BattleDefine.BATTLE_FIELD_SIZE));
					break;
				case PriorityPattern.ALL_ENEMY_INCLUDE_STEALTH:
					targetList.Clear();
					foreach (UnitCtrl item in Owner.IsOther ? battleManager.UnitList : battleManager.EnemyList)
					{
						if (!item.IsPartsBoss)
						{
							targetList.Add(item.ComponentMultiPartsBoss.GetFirstParts());
							continue;
						}

						foreach (PartsData item2 in item.ComponentMultiPartsBoss.BossPartsListForBattle)
						{
							if (item2.GetTargetable())
							{
								targetList.Add(item2);
							}
						}
					}

					break;
				case PriorityPattern.FORWARD:
				case PriorityPattern.BACK:
					{
						bool isForwardPattern = _actionParameter.TargetSort == PriorityPattern.FORWARD;
						sortTargetPosition(targetList, isForwardPattern, BattleUnitComparator.CompareLeft, BattleUnitComparator.CompareRight);
						break;
					}

				case PriorityPattern.ABSOLUTE_POSITION:
					targetList.Clear();
					targetList.Add(Owner.ComponentMultiPartsBoss.GetFirstParts(_owner: true));
					break;
				case PriorityPattern.ENERGY_ASC:
				case PriorityPattern.ENERGY_REDUCING:
					BattleUnitComparator.QuickSort(targetList, BattleUnitComparator.CompareEnergyAsc);
					break;
				case PriorityPattern.ENERGY_DEC:
					BattleUnitComparator.QuickSort(targetList, BattleUnitComparator.CompareEnergyDec);
					break;
				case PriorityPattern.ATK_ASC:
					BattleUnitComparator.QuickSort(targetList, BattleUnitComparator.CompareAtkAsc);
					break;
				case PriorityPattern.ATK_DEC:
					BattleUnitComparator.QuickSort(targetList, BattleUnitComparator.CompareAtkDec);
					break;
				case PriorityPattern.MAGIC_STR_ASC:
					BattleUnitComparator.QuickSort(targetList, BattleUnitComparator.CompareMagicStrAsc);
					break;
				case PriorityPattern.MAGIC_STR_DEC:
					BattleUnitComparator.QuickSort(targetList, BattleUnitComparator.CompareMagicStrDec);
					break;
				case PriorityPattern.HP_ASC_NEAR:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareLifeAscNear);
					break;
				case PriorityPattern.HP_DEC_NEAR:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareLifeDecNear);
					break;
				case PriorityPattern.ENERGY_ASC_NEAR:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareEnergyAscNear);
					break;
				case PriorityPattern.ENERGY_DEC_NEAR:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareEnergyDecNear);
					break;
				case PriorityPattern.ENERGY_DEC_NEAR_MAX_FORWARD:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareEnergyDecNearForward);
					break;
				case PriorityPattern.ATK_DEC_NEAR:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareAtkDecNear);
					break;
				case PriorityPattern.ATK_ASC_NEAR:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareAtkAscNear);
					break;
				case PriorityPattern.MAGIC_STR_DEC_NEAR:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareMagicStrDecNear);
					break;
				case PriorityPattern.MAGIC_STR_ASC_NEAR:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareMagicStrAscNear);
					break;
				case PriorityPattern.SHADOW:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareShadow);
					break;
				case PriorityPattern.HP_VALUE_DEC_NEAR_FORWARD:
					Owner.BaseX = _baseTransform.position.x;
					sortTargetPosition(targetList, _isForwardPattern: true, BattleUnitComparator.CompareLifeValueDecSameLeft, BattleUnitComparator.CompareLifeValueDecSameRight);
					break;
				case PriorityPattern.HP_VALUE_ASC_NEAR_FORWARD:
					Owner.BaseX = _baseTransform.position.x;
					sortTargetPosition(targetList, _isForwardPattern: true, BattleUnitComparator.CompareLifeValueAscSameLeft, BattleUnitComparator.CompareLifeValueAscSameRight);
					break;
				case PriorityPattern.PHYSICS_OR_MAGIC_HIGH_ATTACK_DEC:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareHigherAtkOrMagicStrDecSameNear);
					break;
				case PriorityPattern.PHYSICS_OR_MAGIC_HIGH_ATTACK_ASC:
					Owner.BaseX = _baseTransform.position.x;
					targetList.Sort(Owner.ComponentCompare.CompareHigherAtkOrMagicStrAscSameNear);
					break;
				case PriorityPattern.ENERGY_ASC_BACK_WITHOUT_OWNER:
					sortTargetPosition(targetList, _isForwardPattern: false, BattleUnitComparator.CompareEnergyAscSameLeft, BattleUnitComparator.CompareEnergyAscSameRight);
					break;
				case PriorityPattern.ATK_DEC_FORWARD_WITHOUT_OWNER:
				case PriorityPattern.ATK_DEC_FORWARD_WITHOUT_OWNER_AND_MAGIC_CHARA:
					sortTargetPosition(targetList, _actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE, BattleUnitComparator.CompareAtkDecSameLeft, BattleUnitComparator.CompareAtkDecSameRight);
					break;
				case PriorityPattern.HP_ASC_WITHOUT_OWNER:
					sortTargetPosition(targetList, _actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE, BattleUnitComparator.CompareHpAscSameLeft, BattleUnitComparator.CompareHpAscSameRight);
					break;
				case PriorityPattern.ATK_DEF_ASC_FORWARD:
					sortTargetPosition(targetList, _actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE, BattleUnitComparator.CompareAtkDefAscSameLeft, BattleUnitComparator.CompareAtkDefAscSameRight);
					break;
				case PriorityPattern.MAGIC_DEF_ASC_FORWARD:
					sortTargetPosition(targetList, _actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE, BattleUnitComparator.CompareMagicDefAscSameLeft, BattleUnitComparator.CompareMagicDefAscSameRight);
					break;
				case PriorityPattern.FLIGHT_ONLY:
					sortTargetPosition(targetList, _actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE, BattleUnitComparator.CompareLeft, BattleUnitComparator.CompareRight);
					break;
				case PriorityPattern.MAGIC_STR_DEC_WITHOUT_OWNER_FORWARD:
					sortTargetPosition(targetList, _actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE, BattleUnitComparator.CompareMagicStrDecSameLeft, BattleUnitComparator.CompareMagicStrDecSameRight);
					break;
				default:
					targetList.Sort(Owner.ComponentCompare.CompareDistanceAsc);
					break;
			}

			switch (_actionParameter.TargetSort)
			{
				case PriorityPattern.ENERGY_REDUCING:
					{
						List<BasePartsData> list3 = new List<BasePartsData>();
						List<BasePartsData> list4 = new List<BasePartsData>();
						for (int j = 0; j < targetList.Count; j++)
						{
							if (targetList[j].Owner.ComponentUnionBurst.ExistsReduceEnergyAction())
							{
								list3.Add(targetList[j]);
							}
							else
							{
								list4.Add(targetList[j]);
							}
						}

						list3.AddRange(list4);
						_actionParameter.TargetList = list3;
						break;
					}

				case PriorityPattern.ENERGY_DEC:
				case PriorityPattern.ENERGY_DEC_NEAR:
					{
						List<BasePartsData> list = new List<BasePartsData>();
						List<BasePartsData> list2 = new List<BasePartsData>();
						for (int i = 0; i < targetList.Count; i++)
						{
							if (BattleUtil.Approximately(targetList[i].Owner.ComponentParameter.Energy, 1000f))
							{
								list.Add(targetList[i]);
							}
							else
							{
								list2.Add(targetList[i]);
							}
						}

						list2.AddRange(list);
						_actionParameter.TargetList = list2;
						break;
					}
			}
		}

		[global::OpenPCR.Migration("3dbd08f9-6cde-4229-a80d-19e4882caa04")]
		private void filterTargetByFlight(ActionParameter _actionParameter)
		{
			if (_actionParameter.TargetSort == PriorityPattern.FLIGHT_ONLY)
			{
				return;
			}

			if ((uint)((int)_actionParameter.Direction - 4) < 3u)
			{
				return;
			}

			if (_actionParameter.TargetAssignment != eTargetAssignment.OTHER_SIDE)
			{
				return;
			}

			if (_actionParameter.TargetNum == 1)
			{
				return;
			}

			if (_actionParameter.ReferencedByReflection)
			{
				if (_actionParameter.TargetNth < 1)
				{
					bool isFlightTarget = _actionParameter.IsFlightStateTargetByReflection;
					_actionParameter.TargetList.RemoveAll((BasePartsData target) => isFlightTarget != target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT));
				}
				else
				{
					_actionParameter.TargetList.RemoveAll((BasePartsData target) => target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT));
				}

				return;
			}

			if (!_actionParameter.TargetList.Any((BasePartsData target) => !target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT)))
			{
				return;
			}

			_actionParameter.TargetList.RemoveAll((BasePartsData target) => target.Owner.ComponentAbnormal.IsAbnormalState(eAbnormalState.FLIGHT));
		}

		[global::OpenPCR.Migration("54bf23df-afd2-4091-a1c5-8b138b4375c4")]
		private void sortTargetPosition(List<BasePartsData> _targetList, bool _isForwardPattern, Comparison<BasePartsData> _forwardComparison, Comparison<BasePartsData> _backComparison)
		{
			UnitCtrl owner = Owner;
			UnitComponentAbnormal componentAbnormal = owner.ComponentAbnormal;
			bool isOther = owner.IsOther;
			if ((isOther ^ componentAbnormal.IsConfusionOrConvert()) == _isForwardPattern)
			{
				_targetList.Sort(_backComparison);
			}
			else
			{
				_targetList.Sort(_forwardComparison);
			}
		}

		[global::OpenPCR.Migration("598ed06d-d88a-4758-b366-3de196ec00c9")]
		public void CancelAction(int skillId)
		{
			Skill skill = skillDictionary[skillId];
			for (int num = skill.EffectObjs.Count - 1; num >= 0; num--)
			{
				if (skill.EffectObjs[num] != null && !(skill.EffectObjs[num] is FirearmCtrl) && !skill.EffectObjs[num].IsRepeat)
				{
					skill.EffectObjs[num].SetTimeToDie(value: true);
					skill.EffectObjs[num].OnEffectEnd(skill.EffectObjs[num]);
				}
			}

			skill.Cancel = true;
		}

		[global::OpenPCR.Migration("8483cbfc-bd73-4371-8abf-b98692fbdf50")]
		public IEnumerator StartShakeWithDelay(ShakeEffect _shake, Skill _skill, bool _first = false)
		{
			if (_first && _shake.StartTime < _skill.CutInSkipTime)
			{
				yield break;
			}

			float time = (_first ? _skill.CutInSkipTime : 0f);
			while (true)
			{
				time += Owner.DeltaTimeForPause;
				if (_skill.Cancel)
				{
					yield break;
				}

				if (time > _shake.StartTime)
				{
					break;
				}

				yield return null;
			}

			battleCameraEffect.StartShake(_shake, _skill, Owner);
		}

		[global::OpenPCR.Migration("c8abf4dd-d2de-4154-9f53-9168d98a7860")]
		private void startBlurEffects(Skill _skill, bool _first = false)
		{
			int i = 0;
			for (int count = _skill.BlurEffects.Count; i < count; i++)
			{
				AppendCoroutine(startBlurWithDelay(_skill.BlurEffects[i], _skill, _first), ePauseType.VISUAL, (_skill.BlackOutTime > 0f) ? Owner : null);
			}
		}

		[global::OpenPCR.Migration("3a35e581-f2b7-4602-8cb5-21213fbbf197")]
		private IEnumerator startBlurWithDelay(BlurEffect.BlurEffectData _blurData, Skill _skill, bool _first)
		{
			if (_first && _blurData.StartTime < _skill.CutInSkipTime)
			{
				yield break;
			}

			float time = (_first ? _skill.CutInSkipTime : 0f);
			while (true)
			{
				time += Owner.DeltaTimeForPause;
				if (_skill.Cancel)
				{
					yield break;
				}

				if (time > _blurData.StartTime)
				{
					break;
				}

				yield return null;
			}

			battleCameraEffect.StartBlurEffect(_blurData);
		}

		[global::OpenPCR.Migration("d6d87eb4-80bd-4ac8-bc7f-103c6d740b73")]
		public void AppendCoroutine(IEnumerator cr, ePauseType pauseType, UnitCtrl unit = null)
		{
			battleManager.AppendCoroutine(cr, pauseType, unit);
		}

		[global::OpenPCR.Migration("bd425cfc-f996-4b39-a665-34de4c526e00")]
		public bool HasNextAnime(int skillId)
		{
			Skill skill = skillDictionary[skillId];
			if (skill.IsPrincessForm)
			{
				return true;
			}

			if (skill.AnimId == eAnimKitCharacterAnimeId.SKILL || skill.AnimId == eAnimKitCharacterAnimeId.SPECIAL_SKILL_EVOLUTION || skill.AnimId == eAnimKitCharacterAnimeId.SKILL_EVOLUTION || skill.AnimId == eAnimKitCharacterAnimeId.SUB_UNION_BURST)
			{
				return Owner.GetCurrentSpineCtrl().IsAnimation(skill.AnimId, skill.SkillNum, 1, -1);
			}

			return false;
		}

		[global::OpenPCR.Migration("d5d05d44-1a65-42d0-8c58-d105fc064271")]
		public bool IsLoopMotionPlaying(int _skillId)
		{
			Skill skill = skillDictionary[_skillId];
			return Owner.GetCurrentSpineCtrl().AnimationName == Owner.GetCurrentSpineCtrl().ConvertAnimeIdToAnimeName(skill.AnimId, skill.SkillNum, 1, -1);
		}

		[global::OpenPCR.Migration("8f34689e-86d2-45cf-8cc8-2dc1220e9c8f")]
		public SkillMotionType GetSkillMotionType(int _skillId)
		{
			return skillDictionary[_skillId].SkillMotionType;
		}

		[global::OpenPCR.Migration("491bb8fc-2e82-4c32-a8a0-eb8e4cbcb0c2")]
		public int GetSkillNum(int _skillId)
		{
			return skillDictionary[_skillId].SkillNum;
		}

		[global::OpenPCR.Migration("651441a1-b536-4e35-8f95-40208d3f5fb4")]
		public eAnimKitCharacterAnimeId GetAnimeId(int _skillId)
		{
			return skillDictionary[_skillId].AnimId;
		}

		[global::OpenPCR.Migration("aa3ea379-1cf6-4037-86e9-76907d50886f")]
		public bool IsModeChange(int _skillId)
		{
			return skillDictionary[_skillId].IsModeChange;
		}

		[global::OpenPCR.Migration("0229a379-c40d-4ece-9aa6-25cbe5af400a")]
		public void SetBonusId(int _skillId, int _bonusId)
		{
			skillDictionary[_skillId].BonusId = _bonusId;
		}

		[global::OpenPCR.Migration("3a8e9718-81ef-418d-983e-32e30fe18d3d")]
		public void ExecActionOnStart()
		{
			foreach (KeyValuePair<int, Skill> item in skillDictionary)
			{
				ExecActionOnStart(item.Value);
			}
		}

		[global::OpenPCR.Migration("614338f4-b074-40fb-83f3-00d0ff5a5fe6")]
		public void ExecActionOnWaveStart()
		{
			foreach (KeyValuePair<int, Skill> item in skillDictionary)
			{
				execActionOnWaveStart(item.Value);
			}
		}

		[global::OpenPCR.Migration("71fa2097-beda-4efa-ba01-e46938b015b5")]
		public IEnumerator UpdateBranchMotion(ActionParameter _action, Skill _skill)
		{
			if (_skill.AnimId == eAnimKitCharacterAnimeId.NONE || updateBranchMotionRunning)
			{
				yield break;
			}

			updateBranchMotionRunning = true;
			BattleAnimKitController unitSpineController = Owner.GetCurrentSpineCtrl();
			if (!unitSpineController.IsAnimation(_skill.AnimId, _skill.SkillNum, 1, -1))
			{
				updateBranchMotionRunning = false;
				yield break;
			}

			while (true)
			{
				if (actionCancelAndGetCancel(_skill.SkillId))
				{
					updateBranchMotionRunning = false;
					yield break;
				}

				if (!unitSpineController.IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			if (_skill.EffectBranchId == 0)
			{
				updateBranchMotionRunning = false;
				yield break;
			}

			Owner.ComponentAnimation.PlayAnime(_skill.AnimId, _skill.SkillNum, _skill.EffectBranchId, -1, _isLoop: false);
			if (_skill.SkillId == Owner.UnionBurstSkillId)
			{
				Owner.ComponentState.HasEndUnionBurstAnimeForIfAction = true;
			}

			while (true)
			{
				if (actionCancelAndGetCancel(_skill.SkillId))
				{
					updateBranchMotionRunning = false;
					yield break;
				}

				if (!unitSpineController.IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			Owner.ComponentState.SetState(global::Elements.Battle.Core.ActionState.IDLE, 0, 0, false);
			updateBranchMotionRunning = false;
		}

		[global::OpenPCR.Migration("6654a509-5c4a-408d-9821-1e705558d564")]
		private bool actionCancelAndGetCancel(int _skillId)
		{
			UnitComponentAbnormal componentAbnormal = Owner.ComponentAbnormal;
			UnitComponentState componentState = Owner.ComponentState;
			if (componentAbnormal.IsUnableActionState() || componentState.IsCancelActionState(_skillId == Owner.UnionBurstSkillId))
			{
				componentState.CancelByAwake = false;
				componentState.CancelByConvert = false;
				CancelAction(_skillId);
				return true;
			}

			return false;
		}

		[global::OpenPCR.Migration("34be7fbb-a65a-4824-9e6a-833e20f95349")]
		public bool GetForceCutinOff(int _skillId)
		{
			return skillDictionary[_skillId].ForceCutinOff;
		}

		[global::OpenPCR.Migration("1f2f4643-a449-4e1e-8987-030914421329")]
		public bool GetIsSkillPrincessForm(int _skillId)
		{
			return skillDictionary[_skillId].IsPrincessForm;
		}

		[global::OpenPCR.Migration("91718a8f-b526-44d4-b00a-24e4e2ca65fe")]
		public List<PrincessSkillMovieData> GetPrincessFormMovieData(int _skillId)
		{
			return skillDictionary[_skillId].PrincessSkillMovieDataList;
		}

		[global::OpenPCR.Migration("65f63a34-8bf5-4f1a-9136-993be6b48f7e")]
		public Skill CreateExSkill(int _id, ExSkillScriptable _exSkillScriptable, ExSkillData _exSkillData)
		{
			Skill skill = new Skill();
			skill.ActionParameters = new List<ActionParameter>();
			MasterDataManager masterDataManager = ManagerSingleton<MasterDataManager>.Instance;
			MasterSkillData.SkillData skillData = masterDataManager.masterSkillData[_id];
			MasterLabyrinthSkillActionReplace labyrinthSkillActionReplace = masterDataManager.masterLabyrinthSkillActionReplace;
			for (int i = 0; i < skillData.ActionDataList.Count; i++)
			{
				MasterSkillAction.SkillAction action = skillData.ActionDataList[i];
				if (battleManager.IsLabyrinth && !Owner.IsOther)
				{
					MasterLabyrinthSkillActionReplace.LabyrinthSkillActionReplace replacement = labyrinthSkillActionReplace.Get(action.action_id);
					if (replacement != null)
					{
						action = replacement;
					}
				}

				skill.ActionParameters.Add(createExEquipActionParameter(action, _exSkillScriptable, _exSkillData));
			}

			foreach (KeyValuePair<eExSkillCondition, ExConditionPassiveData> kv in _exSkillData.ExSkillConditionData)
			{
				_exSkillData.LimitNumMax = Math.Max(_exSkillData.LimitNumMax, kv.Value.LimitNum);
				Owner.ComponentAbnormal.OnChangeState.Call(kv.Value.IconType, arg2: true);
				switch (kv.Key)
				{
					case eExSkillCondition.DODGE:
						{
							UnitComponentDamageControl damageControl = Owner.ComponentDamageControl;
							damageControl.OnDodge = (Action)Delegate.Combine(damageControl.OnDodge, (Action)delegate
							{
								_exSkillData.TryExSkill(eExSkillCondition.DODGE, this, _all: true);
							});
							break;
						}

					case eExSkillCondition.DAMAGED:
						{
							UnitComponentDamageControl damageControl = Owner.ComponentDamageControl;
							damageControl.OnDamage = (UnitComponentDamageControl.OnDamageDelegate)Delegate.Combine(damageControl.OnDamage, (UnitComponentDamageControl.OnDamageDelegate)delegate (bool byAttack, long damage, bool critical)
							{
								if (byAttack)
								{
									_exSkillData.TryExSkill(eExSkillCondition.DAMAGED, this, _all: true);
								}
							});
							break;
						}

					case eExSkillCondition.CRITICAL_DAMAGE:
						{
							UnitComponentDamageControl damageControl = Owner.ComponentDamageControl;
							damageControl.OnDamage = (UnitComponentDamageControl.OnDamageDelegate)Delegate.Combine(damageControl.OnDamage, (UnitComponentDamageControl.OnDamageDelegate)delegate (bool byAttack, long damage, bool critical)
							{
								if (byAttack && critical)
								{
									_exSkillData.TryExSkill(eExSkillCondition.CRITICAL_DAMAGE, this, _all: true);
								}
							});
							break;
						}

					case eExSkillCondition.TRIGGER_HP:
						{
							UnitComponentDamageControl damageControl = Owner.ComponentDamageControl;
							damageControl.OnHpChange = (UnitComponentDamageControl.OnDamageDelegate)Delegate.Combine(damageControl.OnHpChange, (UnitComponentDamageControl.OnDamageDelegate)delegate
							{
								UnitComponentParameter parameter = Owner.ComponentParameter;
								if (BattleUtil.LessThanOrApproximately((double)(long)parameter.Hp, kv.Value.Value / 100.0 * (double)(long)parameter.MaxHp))
								{
									_exSkillData.TryExSkill(eExSkillCondition.TRIGGER_HP, this, _all: true);
								}
							});
							break;
						}

					case eExSkillCondition.LIMIT_TIME:
						{
							UnitComponentState state = Owner.ComponentState;
							state.OnUpdateWhenIdle = (Action<float>)Delegate.Combine(state.OnUpdateWhenIdle, (Action<float>)delegate (float _limitTime)
							{
								if (BattleUtil.LessThanOrApproximately(_limitTime, kv.Value.Value))
								{
									_exSkillData.TryExSkill(eExSkillCondition.LIMIT_TIME, this, _all: true);
								}
							});
							break;
						}
				}
			}

			_exSkillData.LimitNum = _exSkillData.LimitNumMax;
			DependActionSolve(skill);
			InitializeAction(skill);
			ExecActionOnStart(skill);
			return skill;
		}

		[global::OpenPCR.Migration("8db3a943-a8cb-4d1f-9dda-30d6f6ce5ad3")]
		public void ExecExSkill(Skill _skill)
		{
			actionStartFirstProcess(_skill, out bool hasNoTarget, false);
			_skill.ReadySkill();
			_skill.AweValue = Owner.ComponentAbnormal.CalcAweValue(false, false);
			for (int i = 0; i < _skill.ActionParameters.Count; i++)
			{
				ActionParameter actionParameter = _skill.ActionParameters[i];
				actionParameter.ReadyAction(Owner, this, _skill);
				if ((_skill.HasParentIndexes.Contains(i) && !actionParameter.ReferencedByReflection) || actionParameter.ReferencedByEffect)
				{
					continue;
				}

				foreach (BasePartsData target in actionParameter.TargetList)
				{
					actionParameter.AppendTargetNum(target.Owner, 0);
					ExecAction(actionParameter, _skill, target, 0, 0f, true, null, false);
				}
			}
		}

		[global::OpenPCR.Migration("dd22a99e-9004-46d1-83a3-871cf0a8210d")]
		public void ExecSkillWithTargetId(Skill _skill, int _actionId)
		{
			actionStartFirstProcess(_skill, out bool hasNoTarget, true);
			_skill.ReadySkill();
			_skill.AweValue = Owner.ComponentAbnormal.CalcAweValue(false, false);
			for (int i = 0; i < _skill.ActionParameters.Count; i++)
			{
				ActionParameter actionParameter = _skill.ActionParameters[i];
				actionParameter.ReadyAction(Owner, this, _skill);
				if ((_skill.HasParentIndexes.Contains(i) && !actionParameter.ReferencedByReflection) || actionParameter.ReferencedByEffect || actionParameter.ActionId != _actionId)
				{
					continue;
				}

				foreach (BasePartsData target in actionParameter.TargetList)
				{
					actionParameter.AppendTargetNum(target.Owner, 0);
					ExecAction(actionParameter, _skill, target, 0, 0f, true, null, false);
				}
			}
		}

		[global::OpenPCR.Migration("eff35ae2-e790-47c2-afd5-c17f8b80bcba")]
		public void ExecSkillWithTargetPartsAndActionList(Skill _skill, BasePartsData _targetParts, List<ActionParameter> _actionList, bool _considerTargetAssignment)
		{
			actionStartFirstProcess(_skill, out bool hasNoTarget, true);
			_skill.ReadySkill();
			_skill.AweValue = Owner.ComponentAbnormal.CalcAweValue(false, false);
			for (int i = 0; i < _actionList.Count; i++)
			{
				ActionParameter actionParameter = _actionList[i];
				actionParameter.ReadyAction(Owner, this, _skill);
				if ((_skill.HasParentIndexes.Contains(i) && !actionParameter.ReferencedByReflection) || actionParameter.ReferencedByEffect)
				{
					continue;
				}

				if (actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE || !_considerTargetAssignment)
				{
					bool isSpyOrEtherialization = actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE && _targetParts.Owner.ComponentAbnormal.IsSpyOrEtherialization(Owner);
					bool isStealth = _targetParts.Owner.ComponentAbnormal.IsStealth;
					if (!isSpyOrEtherialization && !isStealth)
					{
						actionParameter.AppendTargetNum(_targetParts.Owner, 0);
						ExecAction(actionParameter, _skill, _targetParts, 0, 0f, true, null, false);
					}
				}
				else
				{
					foreach (BasePartsData target in actionParameter.TargetList)
					{
						actionParameter.AppendTargetNum(target.Owner, 0);
						ExecAction(actionParameter, _skill, target, 0, 0f, true, null, false);
					}
				}
			}
		}

		[global::OpenPCR.Migration("908afc3b-c86e-4c99-ac91-ee3853d4441d")]
		public void ExecSkillWithTargetParts(Skill _skill, BasePartsData _parts)
		{
			actionStartFirstProcess(_skill, out bool hasNoTarget, false);
			_skill.ReadySkill();
			_skill.AweValue = Owner.ComponentAbnormal.CalcAweValue(false, false);
			foreach (ActionParameter actionParameter in _skill.ActionParameters)
			{
				actionParameter.ReadyAction(Owner, this, _skill);
				if (actionParameter.ActionType == eActionType.IF_FOR_ALL)
				{
					if (actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE)
					{
						actionParameter.AppendTargetNum(_parts.Owner, 0);
						ExecAction(actionParameter, _skill, _parts, 0, 0f, true, null, false);
					}
					else
					{
						foreach (BasePartsData target in actionParameter.TargetList)
						{
							actionParameter.AppendTargetNum(target.Owner, 0);
							ExecAction(actionParameter, _skill, target, 0, 0f, true, null, false);
						}
					}
				}
			}

			for (int i = 0; i < _skill.ActionParameters.Count; i++)
			{
				ActionParameter actionParameter = _skill.ActionParameters[i];
				if (actionParameter.ActionType == eActionType.IF_FOR_ALL || actionParameter.CancelByIfForAll)
				{
					continue;
				}

				if ((_skill.HasParentIndexes.Contains(i) && !actionParameter.ReferencedByReflection) || actionParameter.ReferencedByEffect)
				{
					continue;
				}

				if (actionParameter.TargetAssignment == eTargetAssignment.OTHER_SIDE)
				{
					if (!_parts.Owner.ComponentAbnormal.IsStealth && !_parts.Owner.ComponentAbnormal.IsSpyOrEtherialization(Owner))
					{
						actionParameter.AppendTargetNum(_parts.Owner, 0);
						ExecAction(actionParameter, _skill, _parts, 0, 0f, true, null, false);
					}
				}
				else
				{
					foreach (BasePartsData target in actionParameter.TargetList)
					{
						actionParameter.AppendTargetNum(target.Owner, 0);
						ExecAction(actionParameter, _skill, target, 0, 0f, true, null, false);
					}
				}
			}
		}

		[global::OpenPCR.Migration("f86ed58d-6139-4438-836d-b8e3c4c5c389")]
		private ActionParameter createExEquipActionParameter(MasterSkillAction.SkillAction _action, ExSkillScriptable _exSkillScriptable, ExSkillData _exSkillData)
		{
			ActionParameter actionParameter = Activator.CreateInstance(SkillDefine.SkillActionTypeDictionary[(eActionType)(ushort)_action.action_type]) as ActionParameter;
			actionParameter.IsExEquipSkillAction = true;
			actionParameter.TargetSort = (PriorityPattern)(int)_action.target_type;
			actionParameter.TargetNth = _action.target_number;
			actionParameter.TargetNum = _action.target_count;
			actionParameter.TargetWidth = (((int)_action.target_range <= 0) ? Owner.ComponentSearchArea.SearchAreaSize : ((float)(int)_action.target_range));
			actionParameter.ActionType = (eActionType)(ushort)_action.action_type;
			actionParameter.ActionDetail1 = _action.action_detail_1;
			actionParameter.ActionDetail2 = _action.action_detail_2;
			actionParameter.ActionDetail3 = _action.action_detail_3;
			actionParameter.Value = new Dictionary<eValueNumber, double>(new eValueNumber_DictComparer());
			actionParameter.Value.Add(eValueNumber.VALUE_1, (double)_action.action_value_1);
			actionParameter.Value.Add(eValueNumber.VALUE_2, (double)_action.action_value_2);
			actionParameter.Value.Add(eValueNumber.VALUE_3, (double)_action.action_value_3);
			actionParameter.Value.Add(eValueNumber.VALUE_4, (double)_action.action_value_4);
			actionParameter.Value.Add(eValueNumber.VALUE_5, (double)_action.action_value_5);
			actionParameter.Value.Add(eValueNumber.VALUE_6, (double)_action.action_value_6);
			actionParameter.Value.Add(eValueNumber.VALUE_7, (double)_action.action_value_7);
			actionParameter.MasterData = _action;
			actionParameter.ActionEffectList = new List<NormalSkillEffect>();
			actionParameter.ActionExecTimeList = new List<ActionExecTime>
			{
				new ActionExecTime
				{
					Weight = 1f,
					DamageNumType = eDamageEffectType.NORMAL
				}
			};
			actionParameter.ActionWeightSum = 1f;
			actionParameter.ExecTime = new float[1];
			actionParameter.KnockAnimationCurve = _exSkillScriptable.KnockAnimationCurve;
			actionParameter.KnockDownAnimationCurve = _exSkillScriptable.KnockDownAnimationCurve;
			actionParameter.EffectType = _exSkillScriptable.EffectType;
			actionParameter.DepenedActionId = _action.DependActionId;
			actionParameter.ActionId = _action.action_id;
			actionParameter.TargetList = new List<BasePartsData>();
			actionParameter.ActionChildrenIndexes = new List<int>();
			actionParameter.TargetAssignment = (eTargetAssignment)(int)_action.target_assignment;
			actionParameter.Direction = (DirectionType)(int)_action.target_area;
			if (actionParameter.ActionType == eActionType.EX_START_PASSIVE)
			{
				_exSkillData.ExSkillConditionData.Add(eExSkillCondition.START, new ExConditionPassiveData());
			}
			else if (actionParameter.ActionType == eActionType.EX_CONDITION_PASSIVE)
			{
				double coolTime = actionParameter.Value[eValueNumber.VALUE_4];
				if (coolTime > 0.0)
				{
					_exSkillData.UseCoolTime = true;
				}

				_exSkillData.ExSkillConditionData.Add((eExSkillCondition)actionParameter.ActionDetail1, new ExConditionPassiveData { EffectType = (ExConditionPassiveData.eEffectType)actionParameter.ActionDetail2, LimitNum = actionParameter.ActionDetail3, HitRate = (float)(actionParameter.Value[eValueNumber.VALUE_1] + actionParameter.Value[eValueNumber.VALUE_2] * (double)(int)Owner.ComponentParameter.Level), Value = actionParameter.Value[eValueNumber.VALUE_3], CoolTime = (float)coolTime, IconType = (eStateIconType)BattleUtil.DoubleToInt(actionParameter.Value[eValueNumber.VALUE_5]) });
			}

			return actionParameter;
		}

		[global::OpenPCR.Migration("7999454f-970f-4ee5-abcd-e1296064d3da")]
		public void SetAttackValue(long _additionalAttackEnhance)
		{
			List<ActionParameter>.Enumerator enumerator = Attack.ActionParameters.GetEnumerator();
			try
			{
				double value = (double)_additionalAttackEnhance / 10000.0 + 1.0;
				while (enumerator.MoveNext())
				{
					enumerator.Current.Value[eValueNumber.VALUE_3] = value;
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}
	}
}
