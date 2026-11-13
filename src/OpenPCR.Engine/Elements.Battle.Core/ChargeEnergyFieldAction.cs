using global::OpenPCR.Runtime.Api;
using System;
using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChargeEnergyFieldAction : ActionParameter
	{
		[global::OpenPCR.Migration("62890a99-06f8-4e3e-a3c9-f8863ad367c1")]
		public ChargeEnergyFieldAction() : base()
		{
		}

		[global::OpenPCR.Migration("3d12b1bb-2391-4822-8e8b-d235c7567698")]
		public override void Initialize()
		{
			base.Initialize();
			Elements.Battle.BattleResourceManager.Instance.LoadChargeEnergyFieldEffect();
		}

		[global::OpenPCR.Migration("463095b4-345e-4338-8787-7a01d561017f")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			var chargeType = (ChargeEnergyFieldData.eChargeType)ActionDetail1;
			double value = _valueDictionary[eValueNumber.VALUE_1];
			if (chargeType == ChargeEnergyFieldData.eChargeType.DECREASE)
				value = -value;
			global::OpenPCR.Runtime.Api.GameObject prefab = null;
			global::OpenPCR.Runtime.Api.GameObject prefabLeft = null;
			if (ActionEffectList.Count > 0)
			{
				prefab = ActionEffectList[0].Prefab;
				prefabLeft = ActionEffectList[0].PrefabLeft;
			}

			var field = new ChargeEnergyFieldData
			{
				FildType = eFieldType.HEAL,
				ChargeType = chargeType,
				ExecType = eFieldExecType.REPEAT,
				LimitTime = (float)_valueDictionary[eValueNumber.VALUE_3],
				CenterX = _target.GetLocalPosition().x + Position,
				Size = (float)_valueDictionary[eValueNumber.VALUE_5],
				FieldTargetType = _source.IsOther == (chargeType == ChargeEnergyFieldData.eChargeType.INCREASE) ? eFieldTargetType.ENEMY : eFieldTargetType.PLAYER,
				PauseTarget = _skill.BlackOutTime > 0f ? _source : null,
				FieldEffectType = (ChargeEnergyFieldData.eFieldEffectType)ActionDetail3,
				TargetList = new List<BasePartsData>(),
				Value = value,
				Source = _source,
				Skill = _skill,
				UniqueEffectPrefab = prefab,
				UniqueEffectPrefabLeft = prefabLeft
			};
			battleManager.ExecField(field, ActionId);
		}

		[global::OpenPCR.Migration("24897848-39ad-4f3f-a4f1-758b9bb0eebf")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + System.Math.Ceiling(MasterData.action_value_2 * _level);
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}
	}
}
