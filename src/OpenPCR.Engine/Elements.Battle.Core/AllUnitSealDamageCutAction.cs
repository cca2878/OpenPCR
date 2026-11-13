using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AllUnitSealDamageCutAction : ActionParameter
	{

		private global::System.Collections.Generic.List<global::Elements.Battle.Core.ActionParameter> targetActions { get; set; } = new List<ActionParameter>();
		[global::OpenPCR.Migration("d673253f-7c1b-4ec6-bc9b-70acd3e87d65")]
		public AllUnitSealDamageCutAction() : base()
		{
		}

		[global::OpenPCR.Migration("113fe8ef-2154-40c8-bd5b-0b40855a39b2")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			System.Action<int> add = detail =>
			{
				if (detail == 0) return;
				var action = _skill.ActionParameters.Find(e => e.ActionId == detail);
				action.IgnoreTriggerCausedByTrigger = true;
				targetActions.Add(action);
			};
			add.Call(ActionDetail1);
			add.Call(ActionDetail2);
			add.Call(ActionDetail3);
		}

		[global::OpenPCR.Migration("28b177b4-5f50-4009-8f90-ba3ee1573279")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			foreach (var action in targetActions)
			{
				action.NoDelayExecByTriggerOnly = true;
				foreach (int index in action.ActionChildrenIndexes)
					_skill.ActionParameters[index].NoDelayExecByTriggerOnly = true;
			}
		}

		[global::OpenPCR.Migration("1e8e216f-3739-444d-a980-886b467f7d91")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (_source != _target.Owner)
				return;
			global::OpenPCR.Runtime.Api.GameObject prefab = null;
			global::OpenPCR.Runtime.Api.GameObject prefabLeft = null;
			if (ActionEffectList.Count > 0)
			{
				prefab = ActionEffectList[0].Prefab;
				prefabLeft = ActionEffectList[0].PrefabLeft;
			}
			var data = new AllUnitSealDamageCutData
			{
				Source = _source,
				IconType = (eStateIconType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]),
				DamageCutRate = _valueDictionary[eValueNumber.VALUE_3] / 100.0,
				StartCount = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_2]),
				TargetActions = targetActions,
				EffectPrefab = prefab,
				EffectPrefabLeft = prefabLeft,
				Skill = _skill,
				Duration = (float)_valueDictionary[eValueNumber.VALUE_4]
			};
			battleManager.ExecAllUnitSealDamageCut(data, _source.IsOther);
		}

		[global::OpenPCR.Migration("1d6e75f8-6ab3-4946-aa6a-e2dde712fe11")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
