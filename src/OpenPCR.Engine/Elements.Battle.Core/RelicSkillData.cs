using System;
using System.Collections.Generic;
using System.Linq;
using Elements.Data;

namespace Elements.Battle.Core
{
	public class RelicSkillData
	{
		private Queue<(float execTime, Skill skill)> sortedSkillQueue;
		private Queue<(float execTime, Skill skill)> execSkillQueue = new Queue<(float execTime, Skill skill)>();
		private UnitCtrl unitController;
		private MasterDataManager masterDataManager;
		private MasterRelicActionExecTime masterRelicActionExecTime;
		private MasterSkillData masterSkillData;
		public int ExecSkillReaminCount => execSkillQueue.Count;

		[global::OpenPCR.Migration("db4ff7c8-73f3-4698-8cc2-a4722c45e1d4")]
		public RelicSkillData(UnitCtrl _unitController, int _effectGroupId)
		{
			unitController = _unitController;
			masterDataManager = ManagerSingleton<MasterDataManager>.Instance;
			masterRelicActionExecTime = masterDataManager.masterRelicActionExecTime;
			masterSkillData = masterDataManager.masterSkillData;
			sortedSkillQueue = createSortedSkillList(_effectGroupId, unitController.UnitActionController);
		}

		[global::OpenPCR.Migration("74620068-aafc-456a-b695-e622dc487425")]
		private Queue<(float, Skill)> createSortedSkillList(int _effctGroupId, UnitActionController _actionController)
		{
			var effectIds = masterDataManager.masterLabyrinthRelicEffectGroup.Get(_effctGroupId).EffectIdList;
			var timings = masterDataManager.masterRelicSkillExecTime;
			var skills = new List<(float execTime, Skill skill)>();
			foreach (int effectId in effectIds)
			{
				var effect = masterDataManager.masterLabyrinthRelicEffect.Get(effectId);
				if (effect.effect_type != 1) continue;
				var skill = new Skill();
				var data = masterSkillData.Get(effect.effect_value_1);
				skill.SkillId = data.SkillId;
				foreach (var action in data.ActionDataList)
					skill.ActionParameters.Add(createActionParameter(action));
				_actionController.DependActionSolve(skill);
				_actionController.InitializeAction(skill);
				_actionController.ExecActionOnStart(skill);
				skill.SetLevel(effect.effect_value_2);
				foreach (var timing in timings.GetListWithSkillIdOrderByIdAsc(skill.SkillId))
					skills.Add(((float)timing.exec_time, skill));
			}
			return new Queue<(float, Skill)>(skills.OrderBy(value => value.execTime));
		}

		[global::OpenPCR.Migration("45816515-4974-453c-be55-1bb9da4247c3")]
		private ActionParameter createActionParameter(MasterSkillAction.SkillAction _action)
		{
			var action = (ActionParameter)Activator.CreateInstance(SkillDefine.SkillActionTypeDictionary[(eActionType)_action.action_type]);
			action.TargetSort = (PriorityPattern)_action.target_type;
			action.TargetNth = _action.target_number;
			action.TargetNum = _action.target_count;
			action.TargetWidth = _action.target_range > 0 ? (float)_action.target_range : unitController.ComponentSearchArea.SearchAreaSize;
			action.ActionType = (eActionType)_action.action_type;
			action.ActionDetail1 = _action.action_detail_1;
			action.ActionDetail2 = _action.action_detail_2;
			action.ActionDetail3 = _action.action_detail_3;
			action.Value = new Dictionary<eValueNumber, double>(new eValueNumber_DictComparer())
			{
				{ eValueNumber.VALUE_1, _action.action_value_1 },
				{ eValueNumber.VALUE_2, _action.action_value_2 },
				{ eValueNumber.VALUE_3, _action.action_value_3 },
				{ eValueNumber.VALUE_4, _action.action_value_4 },
				{ eValueNumber.VALUE_5, _action.action_value_5 },
				{ eValueNumber.VALUE_6, _action.action_value_6 },
				{ eValueNumber.VALUE_7, _action.action_value_7 }
			};
			action.MasterData = _action;
			action.ActionEffectList = new List<NormalSkillEffect>();
			action.ExecTime = masterRelicActionExecTime.GetListWithActionIdOrderByIdAsc(_action.action_id).Select(value => (float)value.exec_time).ToArray();
			action.ActionWeightSum = action.ExecTime.Length;
			action.ActionExecTimeList = Enumerable.Repeat(new ActionExecTime(), action.ExecTime.Length).ToList();
			action.EffectType = eEffectType.NONE;
			action.ActionSubEffectList = new List<NormalSkillEffect>();
			action.DepenedActionId = _action.DependActionId;
			action.ActionId = _action.action_id;
			action.TargetList = new List<BasePartsData>();
			action.ActionChildrenIndexes = new List<int>();
			action.TargetAssignment = (eTargetAssignment)_action.target_assignment;
			action.Direction = (DirectionType)_action.target_area;
			return action;
		}

		[global::OpenPCR.Migration("9658056c-b40d-426c-9312-02d846be2d97")]
		public void TryExec(float _passedTime)
		{
			while (execSkillQueue.Count > 0 && BattleUtil.LessThanOrApproximately(execSkillQueue.Peek().execTime, _passedTime))
			{
				var skill = execSkillQueue.Dequeue().skill;
				unitController.UnitActionController.StartAction(skill.SkillId, skill);
			}
		}

		[global::OpenPCR.Migration("4a39b284-9814-43ce-9a8b-6f8f1b7dd48e")]
		public void ResetExecStatus()
		{
			execSkillQueue.Clear();
			execSkillQueue = new Queue<(float execTime, Skill skill)>(sortedSkillQueue);
		}
	}
}
