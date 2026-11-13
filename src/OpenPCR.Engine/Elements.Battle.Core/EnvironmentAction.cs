using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class EnvironmentAction : ActionParameter
	{
		private global::System.Collections.Generic.List<global::Elements.Battle.Core.ActionParameter> targetActions { get; set; } = new List<ActionParameter>();

		public List<eAbnormalState> targetAbnormalStateList = new List<eAbnormalState>();
		[global::OpenPCR.Migration("c4830b08-837c-451e-bef2-469a59e5ee14")]
		public EnvironmentAction() : base()
		{
		}

		[global::OpenPCR.Migration("f4f9d693-d641-4e8d-8208-613dba7bec00")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			foreach (int index in ActionChildrenIndexes)
			{
				ActionParameter action = _skill.ActionParameters[index];
				action.IsEnvironmentAbnormalState = true;
				targetActions.Add(action);
			}
		}

		[global::OpenPCR.Migration("0665277a-5d39-4473-8e9c-1b5f304e4c65")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			foreach (var action in targetActions)
				action.CancelByIfForAll = true;
		}

		[global::OpenPCR.Migration("c1587e8c-9ec7-413b-b908-1a9dec62baa1")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			global::OpenPCR.Runtime.Api.GameObject prefab = ActionEffectList.Count > 0 ? ActionEffectList[0].Prefab : null;
			var environment = new EnvironmentData
			{
				LimitTime = (float)_valueDictionary[eValueNumber.VALUE_1],
				UniqueEffectPrefab = prefab,
				SourceActionController = _sourceActionController,
				Skill = _skill,
				TargetList = new List<BasePartsData>(),
				TargetActions = targetActions,
				SourceUnit = _source,
				EnvironmentType = ActionDetail2
			};
			battleManager.ExecEnvironment(environment, !_source.IsPlayerUnit);
		}

		[global::OpenPCR.Migration("644c0101-1b5a-44f0-91bc-e056df7b3d12")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
