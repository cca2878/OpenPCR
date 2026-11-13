using global::OpenPCR.Runtime.Api;
using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class AttackFieldAction : ActionParameter
	{
		private BasePartsData parts;
		[global::OpenPCR.Migration("92fbc71b-dd17-42d1-8a34-d8ad57f76ac8")]
		public AttackFieldAction() : base()
		{
		}

		[global::OpenPCR.Migration("3fb185c9-a84d-422f-8374-21c90a876c72")]
		public override void Initialize()
		{
			base.Initialize();
			BattleResourceManager.Instance.LoadAttackFieldEffect(ActionDetail2);
		}

		[global::OpenPCR.Migration("90ad241b-1c92-4d56-84a0-1ab5e9ffadb2")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget);
		}

		[global::OpenPCR.Migration("f4346d35-073c-4450-bc25-ba11cf548310")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (ActionDetail3 != 0)
				EnergyChargeMultiple = (double)ActionDetail3 / -100.0 + 1.0;
			var clamped = _source.ComponentParameterClamped;
			double physicalFixed = _valueDictionary[eValueNumber.VALUE_1];
			double physicalRate = _valueDictionary[eValueNumber.VALUE_3];
			long atk = _source.IsPartsBoss ? parts.GetAtkZero() : (long)clamped.AtkZero;
			double magicFixed = _valueDictionary[eValueNumber.VALUE_1];
			double magicRate = _valueDictionary[eValueNumber.VALUE_3];
			long magic = _source.IsPartsBoss ? parts.GetMagicStrZero() : (long)clamped.MagicStrZero;
			bool isWeakTypeDamage = ActionDetail1 == 5;
			bool useMagic = isWeakTypeDamage ? _source.AtkType == 2 : ActionDetail1 % 2 == 0;
			double num = useMagic ? magicFixed + magicRate * (double)magic : physicalFixed + physicalRate * (double)atk;
			DamageData.eDamageType damageType = useMagic ? DamageData.eDamageType.MGC : DamageData.eDamageType.ATK;
			GameObject uniqueEffectPrefab = null;
			GameObject uniqueEffectPrefabLeft = null;
			if (base.ActionEffectList.Count > 0)
			{
				uniqueEffectPrefab = base.ActionEffectList[0].Prefab;
				uniqueEffectPrefabLeft = base.ActionEffectList[0].PrefabLeft;
			}

			AttackFieldData pFDAEFDOBIP = new AttackFieldData
			{
				FildType = eFieldType.HEAL,
				ExecType = ((base.ActionDetail1 > 2) ? eFieldExecType.REPEAT : eFieldExecType.NORMAL),
				LimitTime = (float)_valueDictionary[eValueNumber.VALUE_5],
				CenterX = _target.GetLocalPosition().x + base.Position,
				Size = (float)_valueDictionary[eValueNumber.VALUE_7],
				FieldTargetType = ((!_source.IsOther) ? eFieldTargetType.ENEMY : eFieldTargetType.PLAYER),
				PauseTarget = ((_skill.BlackOutTime > 0f) ? _source : null),
				EffectId = base.ActionDetail2,
				TargetList = new List<BasePartsData>(),
				Value = num,
				DamageType = damageType,
				ActionId = base.ActionId,
				Skill = _skill,
				Source = _source,
				UniqueEffectPrefab = uniqueEffectPrefab,
				UniqueEffectPrefabLeft = uniqueEffectPrefabLeft,
				EnergyChargeMultiple = EnergyChargeMultiple,
				AbsorberValue = base.battleManager.AbsorberValue,
				IsEnvironmentAbnormalState = IsEnvironmentAbnormalState,
				IsWeakTypeDamage = isWeakTypeDamage
			};
			base.battleManager.ExecField(pFDAEFDOBIP, base.ActionId);
		}

		[global::OpenPCR.Migration("5f03640a-a6a7-4612-97eb-1d7ad27b6b4b")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
			Value[eValueNumber.VALUE_3] = MasterData.action_value_3 + MasterData.action_value_4 * _level;
			Value[eValueNumber.VALUE_5] = MasterData.action_value_5 + MasterData.action_value_6 * _level;
		}
	}
}
