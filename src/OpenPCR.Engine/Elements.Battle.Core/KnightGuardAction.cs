using global::Elements;
using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class KnightGuardAction : ActionParameter
	{
		public enum eKnightGuardType
		{
			HEAL = 1
		}

		public enum eAddParamKind
		{
			PHYSICS = 1,
			MAGIC
		}

		private BasePartsData parts;
		[global::OpenPCR.Migration("524d68cc-7b03-4e3c-81ab-4d21d0a58676")]
		public KnightGuardAction() : base()
		{
		}

		[global::OpenPCR.Migration("2c5147b0-a762-4e53-aa56-afc7cd632717")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			parts = _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget);
		}

		[global::OpenPCR.Migration("3edca7cc-4b83-4ef7-b615-104999feb59c")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			if ((long)_target.Owner.ComponentParameter.Hp > 0)
			{
				eAddParamKind eAddParamKind2 = (eAddParamKind)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]);
				long num = 0L;
				num = (_source.IsPartsBoss ? (((eAddParamKind2 == eAddParamKind.PHYSICS) ? parts.GetAtkZero() : parts.GetMagicStrZero())) : ((long)((eAddParamKind2 == eAddParamKind.PHYSICS) ? _source.ComponentParameterClamped.AtkZero : _source.ComponentParameterClamped.MagicStrZero)));
				KnightGuardData knightGuardData = new KnightGuardData
				{
					Value = BattleUtil.DoubleToLong(_valueDictionary[eValueNumber.VALUE_2] + _valueDictionary[eValueNumber.VALUE_4] * (double)num),
					LifeTime = (float)_valueDictionary[eValueNumber.VALUE_6],
					KnightGuardType = (eKnightGuardType)base.ActionDetail1,
					StateIconType = ((base.ActionDetail2 == 0) ? eStateIconType.INVALID_VALUE : ((eStateIconType)base.ActionDetail2)),
					InhibitHealType = ((eAddParamKind2 != eAddParamKind.PHYSICS) ? eInhibitHealType.MAGIC : eInhibitHealType.PHYSICS),
					Skill = _skill,
					Source = _source
				};
				if (base.ActionEffectList.Count != 0)
				{
					NormalSkillEffect normalSkillEffect = base.ActionEffectList[0];
					GameObject gameObject = null;
					gameObject = (_target.Owner.IsLeftDir ? normalSkillEffect.PrefabLeft : normalSkillEffect.Prefab);
					SkillEffectCtrl skillEffectCtrl = (knightGuardData.Effect = base.battleEffectPool.GetEffect(gameObject));
					skillEffectCtrl.transform.parent = ExceptUIRoot.Transform;
					skillEffectCtrl.SortTarget = _target.Owner;
					skillEffectCtrl.InitializeSort();
					skillEffectCtrl.SetPossitionAppearanceType(normalSkillEffect, _target, _target.Owner, _skill);
					skillEffectCtrl.ExecAppendCoroutine();
				}

				if (base.ActionEffectList.Count > 1)
				{
					knightGuardData.ExecEffectData = base.ActionEffectList[1];
				}

				AppendIsAlreadyExeced(_target.Owner, _num);
				_target.Owner.ComponentAbnormal.AddKnightGuard(knightGuardData);
			}
		}

		[global::OpenPCR.Migration("ad5bfbf2-eace-41f1-a170-ef392e8ebb16")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
			Value[eValueNumber.VALUE_4] = MasterData.action_value_4 + MasterData.action_value_5 * _level;
			Value[eValueNumber.VALUE_6] = MasterData.action_value_6 + MasterData.action_value_7 * _level;
		}
	}
}
