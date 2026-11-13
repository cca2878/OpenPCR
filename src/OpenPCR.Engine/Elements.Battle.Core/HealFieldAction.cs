using global::OpenPCR.Runtime.Api;
using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class HealFieldAction : ActionParameter
	{
		private BasePartsData parts;
		[global::OpenPCR.Migration("b0942517-8b40-443a-be0a-2df8d69b8abb")]
		public HealFieldAction() : base()
		{
		}

		[global::OpenPCR.Migration("e90e4281-ecd7-46f5-9f82-8891b9f3889c")]
		public override void Initialize()
		{
			base.Initialize();
			Elements.Battle.BattleResourceManager.Instance.LoadHealFieldEffect();
		}

		[global::OpenPCR.Migration("9563a3a3-6c1f-4424-bf75-d32c736a2fc0")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget);
		}

		[global::OpenPCR.Migration("650e547b-ff76-4370-9aef-81b070683f21")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			double value = _valueDictionary[eValueNumber.VALUE_1];
			double rate = _valueDictionary[eValueNumber.VALUE_3];
			long status;
			if ((ActionDetail1 & 1) != 0)
				status = _source.IsPartsBoss ? parts.GetAtkZero() : (long)_source.ComponentParameterClamped.AtkZero;
			else
				status = _source.IsPartsBoss ? parts.GetMagicStrZero() : (long)_source.ComponentParameterClamped.MagicStrZero;
			global::OpenPCR.Runtime.Api.GameObject prefab = null;
			global::OpenPCR.Runtime.Api.GameObject prefabLeft = null;
			if (ActionEffectList.Count > 0)
			{
				prefab = ActionEffectList[0].Prefab;
				prefabLeft = ActionEffectList[0].PrefabLeft;
			}

			long enhance = IsExEquipSkillAction ? 0L : (long)_source.ComponentParameterClamped.HealEnhanceZero;
			var field = new HealFieldData
			{
				FildType = eFieldType.HEAL,
				ExecType = ActionDetail1 > 2 ? eFieldExecType.REPEAT : eFieldExecType.NORMAL,
				LimitTime = (float)_valueDictionary[eValueNumber.VALUE_5],
				CenterX = _target.GetLocalPosition().x + Position,
				Size = (float)_valueDictionary[eValueNumber.VALUE_7],
				FieldTargetType = _source.IsOther ? eFieldTargetType.ENEMY : eFieldTargetType.PLAYER,
				PauseTarget = _skill.BlackOutTime > 0f ? _source : null,
				ValueType = (HealFieldData.eValueType)ActionDetail2,
				EffectType = (HealFieldData.eEffectType)ActionDetail3,
				TargetList = new List<BasePartsData>()
			};
			field.Source = _source;
			field.Value = (value + rate * (double)status) * ((double)enhance + 10000.0) / 10000.0;
			field.IsMagic = (ActionDetail1 & 1) == 0;
			field.UniqueEffectPrefab = prefab;
			field.UniqueEffectPrefabLeft = prefabLeft;
			battleManager.ExecField(field, ActionId);
		}

		[global::OpenPCR.Migration("5c4e4e60-b9d2-42db-a4a4-daba9694f263")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
			Value[eValueNumber.VALUE_5] = MasterData.action_value_5 + MasterData.action_value_6 * _level;
		}
	}
}
