using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AbnormalStateFieldAction : ActionParameter
	{
		private ActionParameter targetAction;
		public bool IsTargetAbnormalStateBuff { get; set; }
		[global::OpenPCR.Migration("7a5802e7-1f6a-40c7-8df5-62aab181680a")]
		public AbnormalStateFieldAction() : base()
		{
		}

		[global::OpenPCR.Migration("c3ef93ea-b657-4a4c-8031-d4663cc80961")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			targetAction = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail1);
			targetAction.AbnormalStateFieldAction = this;
		}

		[global::OpenPCR.Migration("a5fe6898-d478-497a-abb8-81ba9164a283")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			targetAction.CancelByIfForAll = true;
		}

		[global::OpenPCR.Migration("63138fe2-9045-4665-9f8f-ce6bef31c7ac")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			global::OpenPCR.Runtime.Api.GameObject prefab = null;
			global::OpenPCR.Runtime.Api.GameObject prefabLeft = null;
			if (ActionEffectList.Count > 0)
			{
				prefab = ActionEffectList[0].Prefab;
				prefabLeft = ActionEffectList[0].PrefabLeft;
			}

			var field = new AbnormalStateFieldData
			{
				ExecType = eFieldExecType.NORMAL,
				FildType = eFieldType.HEAL,
				LimitTime = (float)_valueDictionary[eValueNumber.VALUE_1],
				CenterX = _target.GetLocalPosition().x + Position,
				Size = (float)_valueDictionary[eValueNumber.VALUE_3],
				PauseTarget = _skill.BlackOutTime > 0f ? _source : null,
				TargetList = new List<BasePartsData>(),
				Source = _source,
				UniqueEffectPrefab = prefab,
				UniqueEffectPrefabLeft = prefabLeft,
				TargetAction = targetAction,
				SourceActionController = _sourceActionController,
				FieldTargetType = _source.IsOther != !_source.IsBonusEnemy ? eFieldTargetType.ENEMY : eFieldTargetType.PLAYER,
				Skill = _skill,
				IsTargetAbnormalStateBuff = IsTargetAbnormalStateBuff
			};
			battleManager.ExecField(field, ActionId);
		}

		[global::OpenPCR.Migration("c0ee08ed-93db-4553-a8ec-3677859f3972")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}

		public global::Elements.Battle.Core.eAbnormalState TargetAbnormalState { get; set; }
	}
}
