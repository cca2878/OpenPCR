using global::OpenPCR.Runtime.Api;
using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChangeSpeedOverwriteFieldAction : ActionParameter
	{
		private static readonly Dictionary<ChangeSpeedAction.eChangeSpeedType, eOverwriteAbnormalState> abnormalStateDic = new Dictionary<ChangeSpeedAction.eChangeSpeedType, eOverwriteAbnormalState>
		{
			{
				ChangeSpeedAction.eChangeSpeedType.SLOW,
				eOverwriteAbnormalState.SLOW
			},
			{
				ChangeSpeedAction.eChangeSpeedType.HASTE,
				eOverwriteAbnormalState.HASTE
			}
		};
		private const int UNIQUE_EFFECT_SHOW_VALUE = 1;
		[global::OpenPCR.Migration("18f9c499-58d3-43c3-abf2-7ea234bbc026")]
		public ChangeSpeedOverwriteFieldAction() : base()
		{
		}

		[global::OpenPCR.Migration("7e81a372-09c7-444a-80d2-bb9f9d85ef14")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			GameObject uniqueEffectPrefab = null;
			GameObject uniqueEffectPrefabLeft = null;
			if (base.ActionSubEffectList.Count > 0)
			{
				uniqueEffectPrefab = base.ActionSubEffectList[0].Prefab;
				uniqueEffectPrefabLeft = base.ActionSubEffectList[0].PrefabLeft;
			}

			eOverwriteAbnormalState eOverwriteAbnormalState = abnormalStateDic[(ChangeSpeedAction.eChangeSpeedType)base.ActionDetail1];
			base.battleManager.AddChangeSpeedOverwriteFieldCounter();
			ChangeSpeedOverwriteFieldData pFDAEFDOBIP = new ChangeSpeedOverwriteFieldData
			{
				FildType = eFieldType.HEAL,
				ExecType = eFieldExecType.NORMAL,
				LimitTime = (float)_valueDictionary[eValueNumber.VALUE_3],
				CenterX = _target.GetLocalPosition().x + base.Position,
				Size = (float)_valueDictionary[eValueNumber.VALUE_5],
				FieldTargetType = ((_source.IsOther == (eOverwriteAbnormalState == eOverwriteAbnormalState.HASTE)) ? eFieldTargetType.ENEMY : eFieldTargetType.PLAYER),
				PauseTarget = ((_skill.BlackOutTime > 0f) ? _source : null),
				AbnormalState = eOverwriteAbnormalState,
				TargetList = new List<BasePartsData>(),
				EffectValue = base.Value[eValueNumber.VALUE_1],
				Source = _source,
				Skill = _skill,
				ActionParameter = this,
				ActionCounter = base.battleManager.ChangeSpeedOverwriteFieldCounter,
				UniqueEffectPrefab = uniqueEffectPrefab,
				UniqueEffectPrefabLeft = uniqueEffectPrefabLeft,
				IsShowUniqueEffect = (base.ActionDetail2 == 1)
			};
			base.battleManager.ExecField(pFDAEFDOBIP, base.ActionId);
		}

		[global::OpenPCR.Migration("6d533d36-f729-43ab-9839-aeaeca4e09cb")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
