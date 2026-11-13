using System.Collections;
using System.Collections.Generic;
using global::OpenPCR.Foundation;
using Elements.Data;

namespace Elements.Battle.Core
{
	public class UnitComponentActionPattern : UnitComponentBase, IUnitComponent
	{
		public int ActionPatternIndex { get; private set; }
		public bool ActionPatternIsLoop { get; private set; }
		public int CurrentActionPatternId { get; private set; }
		public Dictionary<int, List<int>> ActionPatternDictionary { get; private set; }
		public Dictionary<int, List<int>> ActionPatternLoopDictionary { get; private set; }
		private int unitId { get; set; }
		private Dictionary<int, int> skillLevelDictionary { get; set; }
		private UnitComponentAbnormal componentAbnormal { get; set; }

		[global::OpenPCR.Migration("f8573b24-2dd9-4180-a798-1daaebe61a17")]
		public UnitComponentActionPattern(UnitCtrl _unitController, int _unitId, Dictionary<int, int> _skillLevelDictionary) : base(_unitController)
		{
			ActionPatternDictionary = new Dictionary<int, List<int>>();
			ActionPatternLoopDictionary = new Dictionary<int, List<int>>();
			skillLevelDictionary = _skillLevelDictionary;
			unitId = _unitId;
		}

		[global::OpenPCR.Migration("522b4424-49b0-4c38-91cf-adb6aa4c8fd3")]
		public void RegisterComponentSet(IUnitComponentContainer _container)
		{
			componentAbnormal = _container.ComponentAbnormal;
		}

		[global::OpenPCR.Migration("55bf33e9-169e-48d4-817f-a5083446aa87")]
		public void Setup(MasterUnitSkillData.UnitSkillData _skillData)
		{
			ActionPatternDictionary.Clear();
			ActionPatternLoopDictionary.Clear();
			int id = UnitUtility.GetDefaultActionPatternId(unitId);
			CreateAttackPattern(_skillData, id);
			CurrentActionPatternId = id;
		}

		[global::OpenPCR.Migration("2b655220-41f6-4835-acae-9008a7814b90")]
		public void CreateAttackPattern(MasterUnitSkillData.UnitSkillData _skillData, int _attackPatternId)
		{
			if (ActionPatternDictionary.ContainsKey(_attackPatternId))
			{
				return;
			}

			MasterUnitAttackPattern.UnitAttackPattern unitAttackPattern = ManagerSingleton<MasterDataManager>.Instance.masterUnitAttackPattern[_attackPatternId];
			List<int> list = new List<int>();
			for (int i = 0; i < (int)unitAttackPattern.loop_start - 1; i++)
			{
				int num = unitAttackPattern.PatternList[i];
				switch (num)
				{
					case 1:
						list.Add(1);
						componentAbnormal.AttackWhenSilence = true;
						continue;
					case 0:
						continue;
				}

				switch (num / 1000)
				{
					case 1:
						{
							int num5 = num % 1000 - 1;
							int num6 = _skillData.MainSkillIds[num5];
							int num7 = 0;
							if (_skillData.MainSkillEvolutionIds.Count > num5)
							{
								num7 = _skillData.MainSkillEvolutionIds[num5];
							}

							if (skillLevelDictionary[num6] == 0 && (num7 == 0 || skillLevelDictionary[num7] == 0))
							{
								list.Add(1);
							}
							else if (num7 != 0 && skillLevelDictionary[num7] != 0)
							{
								list.Add(num7);
							}
							else
							{
								list.Add(num6);
							}

							break;
						}

					case 2:
						{
							int num2 = num % 1000 - 1;
							int num3 = 0;
							if (_skillData.SpSkillEvolutionIds.Count > num2)
							{
								num3 = _skillData.SpSkillEvolutionIds[num2];
							}

							int num4 = 0;
							if (_skillData.MainSkillEvolutionIds.Count > num2)
							{
								num4 = _skillData.MainSkillEvolutionIds[num2];
							}

							if (num3 != 0 && num4 != 0 && skillLevelDictionary[num4] != 0)
							{
								list.Add(num3);
							}
							else
							{
								list.Add(_skillData.SpSkillIds[num2]);
							}

							break;
						}
				}
			}

			ActionPatternDictionary.Add(_attackPatternId, list);
			List<int> list2 = new List<int>();
			if ((int)unitAttackPattern.loop_start > 0)
			{
				for (int j = (int)unitAttackPattern.loop_start - 1; j < (int)unitAttackPattern.loop_end; j++)
				{
					int num8 = unitAttackPattern.PatternList[j];
					switch (num8)
					{
						case 1:
							list2.Add(1);
							componentAbnormal.AttackWhenSilence = true;
							continue;
						default:
							switch (num8 / 1000)
							{
								case 1:
									{
										int num12 = num8 % 1000 - 1;
										int num13 = _skillData.MainSkillIds[num12];
										int num14 = 0;
										if (_skillData.MainSkillEvolutionIds.Count > num12)
										{
											num14 = _skillData.MainSkillEvolutionIds[num12];
										}

										if (skillLevelDictionary[num13] == 0 && (num14 == 0 || skillLevelDictionary[num14] == 0))
										{
											list2.Add(1);
										}
										else if (num14 != 0 && skillLevelDictionary[num14] != 0)
										{
											if (num12 == 0)
											{
												unitController.MainSkill1Evolved = true;
											}

											list2.Add(num14);
										}
										else
										{
											list2.Add(num13);
										}

										break;
									}

								case 2:
									{
										int num9 = num8 % 1000 - 1;
										int num10 = 0;
										if (_skillData.SpSkillEvolutionIds.Count > num9)
										{
											num10 = _skillData.SpSkillEvolutionIds[num9];
										}

										int num11 = 0;
										if (_skillData.MainSkillEvolutionIds.Count > num9)
										{
											num11 = _skillData.MainSkillEvolutionIds[num9];
										}

										if (num10 != 0 && num11 != 0 && skillLevelDictionary[num11] != 0)
										{
											list2.Add(num10);
										}
										else
										{
											list2.Add(_skillData.SpSkillIds[num9]);
										}

										break;
									}
							}

							continue;
						case 0:
							break;
					}

					break;
				}
			}

			ActionPatternLoopDictionary.Add(_attackPatternId, list2);
		}

		[global::OpenPCR.Migration("dde9feae-a2c8-4703-91fb-7bb563e0e588")]
		public void Reset()
		{
			int id = UnitUtility.GetDefaultActionPatternId(unitId);
			ActionPatternIndex = 0;
			CurrentActionPatternId = id;
			ActionPatternIsLoop = ActionPatternDictionary[id].Count == 0;
		}

		[global::OpenPCR.Migration("247c43f6-ba39-41bb-8e2c-9388fa25ee61")]
		public void ChangeAttackPattern(int _attackPatternId, int _spSkillLevel, float _limitTime = -1f)
		{
			int oldIndex = CurrentActionPatternId;
			CurrentActionPatternId = _attackPatternId;
			ActionPatternIndex = 0;
			ActionPatternIsLoop = ActionPatternDictionary[_attackPatternId].Count == 0;
			UnitActionController action = unitController.UnitActionController;
			List<Skill> specials = action.SpecialSkillList;
			List<int> fixedIds = unitController.LevelFixedSpSkillIdList;
			foreach (Skill skill in specials)
				if (!fixedIds.Contains(skill.SkillId))
					skill.SetLevel(_spSkillLevel);
			foreach (Skill skill in action.SpecialSkillEvolutionList)
				if (!fixedIds.Contains(skill.SkillId))
					skill.SetLevel(_spSkillLevel);
			if (_limitTime > 0f)
				unitController.AppendCoroutine(updateChangeActionPattern(oldIndex, _limitTime), ePauseType.SYSTEM, unitController);
		}

		[global::OpenPCR.Migration("190544fd-e58d-4440-af58-b9878c0f997f")]
		private IEnumerator updateChangeActionPattern(int oldIndex, float limitTime)
		{
			float time = 0f;
			while (true)
			{
				time += unitController.DeltaTimeForPause;
				if (time > limitTime)
				{
					ActionPatternIndex = 0;
					CurrentActionPatternId = oldIndex;
					yield break;
				}

				yield return null;
			}
		}

		[global::OpenPCR.Migration("1c1b2d5d-1c2b-4bbf-a39d-513f1fc8f2a0")]
		public bool HasLoopPattern()
		{
			return ActionPatternLoopDictionary[CurrentActionPatternId].Count != 0;
		}

		[global::OpenPCR.Migration("29d158ff-ce4b-42b4-aa79-8d40b921c032")]
		public int GetSkillId()
		{
			return (ActionPatternIsLoop ? ActionPatternLoopDictionary : ActionPatternDictionary)[CurrentActionPatternId][ActionPatternIndex];
		}

		[global::OpenPCR.Migration("eb5e3cda-073f-4fd2-a4b9-88456edba88c")]
		public int MoveToNext()
		{
			int index = ActionPatternIndex;
			int result;
			if (ActionPatternIsLoop)
			{
				if (index + 1 < ActionPatternLoopDictionary[CurrentActionPatternId].Count)
				{
					result = ActionPatternLoopDictionary[CurrentActionPatternId][ActionPatternIndex + 1];
					ActionPatternIndex++;
					return result;
				}

				result = ActionPatternLoopDictionary[CurrentActionPatternId][0];
				ActionPatternIndex = 0;
				return result;
			}

			if (index + 1 >= ActionPatternDictionary[CurrentActionPatternId].Count)
			{
				result = ActionPatternLoopDictionary[CurrentActionPatternId][0];
				ActionPatternIsLoop = true;
				ActionPatternIndex = 0;
				return result;
			}

			result = ActionPatternDictionary[CurrentActionPatternId][ActionPatternIndex + 1];
			ActionPatternIndex++;
			return result;
		}

		[global::OpenPCR.Migration("c28266f6-7f69-428b-9471-eb2962ecd8d1")]
		public int GetFirstActionSkillId()
		{
			if (ActionPatternIsLoop || ActionPatternIndex != 0 || ActionPatternDictionary == null)
				return 0;
			if (ActionPatternDictionary[CurrentActionPatternId].Count == 0)
				return 0;
			return GetSkillId();
		}
	}
}
