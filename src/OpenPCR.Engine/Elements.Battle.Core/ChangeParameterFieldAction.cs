using global::OpenPCR.Runtime.Api;
using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class ChangeParameterFieldAction : ActionParameter
	{
		[global::OpenPCR.Migration("0f8978a5-22d9-4458-9a90-b9d5300dbf3d")]
		public ChangeParameterFieldAction() : base()
		{
		}

		[global::OpenPCR.Migration("e1dfabd0-9c4a-46e8-b6b6-3a17e1bc6d9c")]
		public override void Initialize()
		{
			base.Initialize();
			Elements.Battle.BattleResourceManager.Instance.LoadDebuffFieldEffect();
		}

		[global::OpenPCR.Migration("93e7e546-18b1-448a-878a-93189f2a6702")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("5e67c7bb-9bf4-452d-848d-4c5f5efe3b21")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			BuffParamKind kind = BuffDebuffAction.GetChangeParamKind(ActionDetail1);
			int value = BattleUtil.DoubleToIntReverseTruncate(_valueDictionary[eValueNumber.VALUE_1]);
			global::OpenPCR.Runtime.Api.GameObject prefab = null;
			global::OpenPCR.Runtime.Api.GameObject prefabLeft = null;
			if (ActionEffectList.Count > 0)
			{
				prefab = ActionEffectList[0].Prefab;
				prefabLeft = ActionEffectList[0].PrefabLeft;
			}

			eFieldTargetType targetType = _source.IsOther != (ActionDetail1 % 10 == 1) ? eFieldTargetType.ENEMY : eFieldTargetType.PLAYER;
			if (_source.IsOther && _source.IsBonusEnemy)
				targetType = ActionDetail1 % 10 == 1 ? eFieldTargetType.ENEMY : eFieldTargetType.PLAYER;
			var field = new ChangeParameterFieldData
			{
				FildType = eFieldType.HEAL,
				ExecType = eFieldExecType.NORMAL,
				LimitTime = (float)_valueDictionary[eValueNumber.VALUE_3],
				CenterX = _target.GetLocalPosition().x + Position,
				Size = (float)_valueDictionary[eValueNumber.VALUE_5],
				FieldTargetType = targetType,
				PauseTarget = _skill.BlackOutTime > 0f ? _source : null,
				EffectType = (ChangeParameterFieldData.eFieldEffectType)ActionDetail3,
				TargetList = new List<BasePartsData>(),
				Value = (double)value,
				BuffParamKind = kind,
				ValueType = (BuffDebuffAction.eChangeParameterType)ActionDetail2,
				Source = _source,
				UniqueEffectPrefab = prefab,
				UniqueEffectPrefabLeft = prefabLeft,
				IsBuff = ActionDetail1 % 10 != 1,
				AlreadyExecedTargetCount = new Dictionary<UnitCtrl, int>(),
				ShowsIcon = _valueDictionary[eValueNumber.VALUE_6] == 0.0,
				TriggerCall = TriggerCall,
				Skill = _skill,
				TargetTalentId = OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_7])
			};
			battleManager.ExecField(field, ActionId);
		}

		[global::OpenPCR.Migration("4bec6914-efa4-4d77-8e06-75f9f6232295")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
		}

		[global::OpenPCR.Migration("97c73f98-d259-462d-ba00-095d624c5a5d")]
		public override double GetActionBaseValue(UnitCtrl _source, eValueNumber _type)
		{
			Value.TryGetValue(_type, out double value);
			if (_type != eValueNumber.VALUE_1)
				return value;
			BuffParamKind kind = BuffDebuffAction.GetChangeParamKind(ActionDetail1);
			if (!BuffDebuffAction.BUFF_DEBUFF_ENHANCE_TARGET_TYPE.Contains(kind) || IsExEquipSkillAction)
				return value;
			long enhance = ActionDetail1 % 10 == 1
				? (long)_source.ComponentParameterClamped.DebuffEnhanceZero
				: (long)_source.ComponentParameterClamped.BuffEnhanceZero;
			return value * ((double)enhance / 10000.0 + 1.0);
		}
	}
}
