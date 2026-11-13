using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class BuffDebuffAction : ActionParameter
	{
		[global::OpenPCR.Migration("5f3b1ad8-7930-4614-8beb-a49a20e6167a")]
		static BuffDebuffAction()
		{
			BUFF_DEBUFF_ENHANCE_TARGET_TYPE = new List<BuffParamKind>
			{
				(BuffParamKind)1,
				(BuffParamKind)2,
				(BuffParamKind)3,
				(BuffParamKind)4,
				(BuffParamKind)6,
				(BuffParamKind)7,
				(BuffParamKind)11,
				(BuffParamKind)12,
				(BuffParamKind)14,
				(BuffParamKind)15,
				(BuffParamKind)16,
				(BuffParamKind)17,
				(BuffParamKind)18,
				(BuffParamKind)19
			};
		}

		public static readonly global::System.Collections.Generic.List<Elements.Battle.Core.BuffParamKind> BUFF_DEBUFF_ENHANCE_TARGET_TYPE;
		public enum eBuffDebuffStartReleaseType
		{
			NORMAL = 1,
			BREAK
		}

		public enum eChangeParameterType
		{
			FIXED = 1,
			PERCENTAGE,
			CURRENT_VALUE_PERCENTAGE
		}

		public int EnvironmentId { get; set; } = -1;

		[global::OpenPCR.Migration("81fb2725-aa80-4ab4-8a4d-090e6f4f9b5f")]
		public BuffDebuffAction() : base()
		{
		}

		[global::OpenPCR.Migration("d19d5209-a7b7-4d4f-abc6-38ecade8183b")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			List<PartsData> bossPartsListForBattle = _source.ComponentMultiPartsBoss.BossPartsListForBattle;
			PartsData partsData = bossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget);
			eBuffDebuffStartReleaseType actionDetail = (eBuffDebuffStartReleaseType)base.ActionDetail2;
			if (actionDetail != eBuffDebuffStartReleaseType.BREAK)
			{
				return;
			}

			Dictionary<BasePartsData, long> buffParam = new Dictionary<BasePartsData, long>();
			bool isDebuff = base.ActionDetail1 % 10 == 1;
			BuffParamKind buffParamKind = GetChangeParamKind(base.ActionDetail1);
			partsData.OnBreak = (Action)Delegate.Combine(partsData.OnBreak, (Action)delegate
			{
				if (_source.IsPartsBoss)
				{
					for (int i = 0; i < bossPartsListForBattle.Count; i++)
					{
						buffParam.Add(bossPartsListForBattle[i], CalculateBuffDebuffParam(bossPartsListForBattle[i], base.Value[eValueNumber.VALUE_2], (eChangeParameterType)global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_1]), buffParamKind, isDebuff));
					}
				}

				_source.ComponentAbnormal.SetBuffParam(BuffParamKind.NUM, buffParamKind, buffParam, 0f, 0, null, _despelable: true, eEffectType.COMMON, !isDebuff, getIsAdditional(), isShowIcon());
				_source.ComponentAbnormal.EnableBuffParam(buffParamKind, buffParam, _enable: true, _source, !isDebuff, getIsAdditional(), true, TriggerCall);
			});
			partsData.OnBreakEnd = (Action)Delegate.Combine(partsData.OnBreakEnd, (Action)delegate
			{
				_source.ComponentAbnormal.EnableBuffParam(buffParamKind, buffParam, _enable: false, _source, !isDebuff, getIsAdditional(), true, true);
				buffParam.Clear();
			});
		}

		[global::OpenPCR.Migration("09de0f99-e7d5-418a-a1d8-ecde03e88aa5")]
		private bool getIsAdditional()
		{
			return base.ActionDetail1 / 1000 == 1;
		}

		[global::OpenPCR.Migration("846ef5cd-ad1f-4364-aa25-e07625c7c311")]
		private bool isShowIcon()
		{
			return BattleUtil.Approximately(Value[eValueNumber.VALUE_6], 0.0);
		}

		[global::OpenPCR.Migration("6c526d57-f083-4673-9fd4-6fa724fed19b")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			eBuffDebuffStartReleaseType actionDetail = (eBuffDebuffStartReleaseType)base.ActionDetail2;
			if (actionDetail == eBuffDebuffStartReleaseType.BREAK)
			{
				return;
			}

			BuffParamKind changeParamKind = GetChangeParamKind(base.ActionDetail1);
			Dictionary<BasePartsData, long> dictionary = new Dictionary<BasePartsData, long>();
			if (_target.Owner.IsPartsBoss && !AbnormalStateDefine.IS_EFFECT_BOSS_BODY_ONLY[changeParamKind])
			{
				for (int i = 0; i < _target.Owner.ComponentMultiPartsBoss.BossPartsListForBattle.Count; i++)
				{
					dictionary.Add(_target.Owner.ComponentMultiPartsBoss.BossPartsListForBattle[i], CalculateBuffDebuffParam(_target.Owner.ComponentMultiPartsBoss.BossPartsListForBattle[i], _valueDictionary[eValueNumber.VALUE_2], (eChangeParameterType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]), changeParamKind, base.ActionDetail1 % 10 == 1));
				}
			}
			else
			{
				dictionary.Add(_target.Owner.ComponentMultiPartsBoss.DummyPartsData, CalculateBuffDebuffParam(_target, _valueDictionary[eValueNumber.VALUE_2], (eChangeParameterType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]), changeParamKind, base.ActionDetail1 % 10 == 1));
			}

			bool despelable = _valueDictionary[eValueNumber.VALUE_7] != 2f;
			foreach (NormalSkillEffect actionEffect in base.ActionEffectList)
			{
				GameObject gameObject = null;
				gameObject = (_target.Owner.IsLeftDir ? actionEffect.PrefabLeft : actionEffect.Prefab);
				SkillEffectCtrl effect = base.battleEffectPool.GetEffect(gameObject);
				effect.transform.parent = ExceptUIRoot.Transform;
				effect.SortTarget = _target.Owner;
				effect.InitializeSort();
				effect.SetPossitionAppearanceType(actionEffect, _target, _target.Owner, _skill);
				effect.ExecAppendCoroutine();
				effect.PlaySe(_source.ComponentSound.SoundUnitId, _source.IsLeftDir);
			}

			_target.Owner.ComponentAbnormal.SetBuffParam(changeParamKind, changeParamKind, dictionary, (float)_valueDictionary[eValueNumber.VALUE_4], _skill.SkillId, _source, despelable, base.EffectType, base.ActionDetail1 % 10 != 1, getIsAdditional(), isShowIcon(), _skill.BonusId, EnvironmentId, TriggerCall);
		}

		[global::OpenPCR.Migration("30dc083c-2b51-49a3-82ba-2866155289bb")]
		public static long CalculateBuffDebuffParam(BasePartsData _target, double _value, eChangeParameterType _changeParamType, BuffParamKind _targetChangeParamKind, bool _isDebuff)
		{
			double result = 0.0;
			if (_changeParamType == eChangeParameterType.CURRENT_VALUE_PERCENTAGE)
				result = getBuffDebuffValueForCurrentValuePercentage(_target, _value, _targetChangeParamKind);
			else if (_changeParamType == eChangeParameterType.PERCENTAGE)
			{
				UnitComponentParameter parameter = _target.Owner.ComponentParameter;
				double scale = _value / 100.0;
				switch (_targetChangeParamKind)
				{
					case BuffParamKind.ATK:
						result = scale * (double)(_target.GetStartAtk());
						break;
					case BuffParamKind.DEF:
						result = scale * (double)(_target.GetStartDef());
						break;
					case BuffParamKind.MAGIC_STR:
						result = scale * (double)(_target.GetStartMagicStr());
						break;
					case BuffParamKind.MAGIC_DEF:
						result = scale * (double)(_target.GetStartMagicDef());
						break;
					case BuffParamKind.DODGE:
						result = scale * (double)(_target.GetStartDodge());
						break;
					case BuffParamKind.PHYSICAL_CRITICAL:
						result = scale * (double)(_target.GetStartPhysicalCritical());
						break;
					case BuffParamKind.MAGIC_CRITICAL:
						result = scale * (double)(_target.GetStartMagicCritical());
						break;
					case BuffParamKind.ENERGY_RECOVER_RATE:
						result = scale * (double)((long)parameter.StartEnergyRecoveryRate);
						break;
					case BuffParamKind.LIFE_STEAL:
						result = scale * (double)(_target.GetStartLifeSteal());
						break;
					case BuffParamKind.MOVE_SPEED:
						result = scale * (double)((float)parameter.StartMoveSpeed);
						break;
					case BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE:
						result = scale * (double)((long)parameter.StartPhysicalCriticalDamageRate);
						break;
					case BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE:
						result = scale * (double)((long)parameter.StartMagicCriticalDamageRate);
						break;
					case BuffParamKind.ACCURACY:
						result = scale * (double)((long)parameter.StartAccuracy);
						break;
					case BuffParamKind.RECEIVE_CRITICAL_DAMAGE_RATE:
						result = scale * (double)((long)parameter.StartReceiveCriticalDamageRate);
						break;
					case BuffParamKind.RECEIVE_PHYSICAL_AND_MAGIC_DAMAGE_PERCENT:
						result = scale * (double)((long)parameter.StartPhysicalAndMagicReceiveDamagePercent);
						break;
					case BuffParamKind.RECEIVE_PHYSICAL_DAMAGE_PERCENT:
						result = scale * (double)((long)parameter.StartPhysicalReceiveDamagePercent);
						break;
					case BuffParamKind.RECEIVE_MAGIC_DAMAGE_PERCENT:
						result = scale * (double)((long)parameter.StartMagicReceiveDamagePercent);
						break;
					case BuffParamKind.PHYSICAL_DAMAGE_UP_PERCENT:
						result = scale * (double)((long)parameter.StartPhysicalDamageUpPercent);
						break;
					case BuffParamKind.MAGIC_DAMAGE_UP_PERCENT:
						result = scale * (double)((long)parameter.StartMagicDamageUpPercent);
						break;
					case BuffParamKind.MAX_HP:
						result = scale * (double)((long)parameter.StartMaxHP);
						break;
					case BuffParamKind.BUFF_ENHANCE:
						result = scale * (double)((long)parameter.StartBuffEnhance);
						break;
				}
			}
			else if (_changeParamType == eChangeParameterType.FIXED)
				result = BattleUtil.DoubleToIntReverseTruncate(_value);
			long amount = BattleUtil.DoubleToLong(System.Math.Ceiling(System.Math.Abs(result)));
			if (_targetChangeParamKind != BuffParamKind.BUFF_ENHANCE && (_targetChangeParamKind == BuffParamKind.MAX_HP || _isDebuff))
				return -BattleUtil.DoubleToLong(((double)_target.GetDebuffResistPercent(_targetChangeParamKind) / -100.0 + 1.0) * (double)amount);
			return amount;
		}

		[global::OpenPCR.Migration("024e5ec5-17fd-46d3-89a3-f0edf9a7a882")]
		private static double getBuffDebuffValueForCurrentValuePercentage(BasePartsData _target, double _value, BuffParamKind _targetChangeParamKind)
		{
			UnitComponentParameter parameter = _target.Owner.ComponentParameter;
			UnitComponentParameterClamped clamped = _target.Owner.ComponentParameterClamped;
			double scale = _value / 100.0;
			switch (_targetChangeParamKind)
			{
				case BuffParamKind.ATK:
					return scale * (double)(_target.GetAtkZero());
				case BuffParamKind.DEF:
					return scale * (double)(_target.GetDefZero());
				case BuffParamKind.MAGIC_STR:
					return scale * (double)(_target.GetMagicStrZero());
				case BuffParamKind.MAGIC_DEF:
					return scale * (double)(_target.GetMagicDefZero());
				case BuffParamKind.DODGE:
					return scale * (double)((long)clamped.DodgeZero);
				case BuffParamKind.PHYSICAL_CRITICAL:
					return scale * (double)(_target.GetPhysicalCriticalZero());
				case BuffParamKind.MAGIC_CRITICAL:
					return scale * (double)(_target.GetMagicCriticalZero());
				case BuffParamKind.ENERGY_RECOVER_RATE:
					return scale * (double)((long)clamped.EnergyRecoveryRateZero);
				case BuffParamKind.LIFE_STEAL:
					return scale * (double)(_target.GetLifeStealZero());
				case BuffParamKind.MOVE_SPEED:
					return scale * (double)((float)clamped.MoveSpeedOrMin);
				case BuffParamKind.PHYSICAL_CRITICAL_DAMAGE_RATE:
					return scale * (double)((double)clamped.PhysicalCriticalDamageRateOrMin);
				case BuffParamKind.MAGIC_CRITICAL_DAMAGE_RATE:
					return scale * (double)((double)clamped.MagicCriticalDamageRateOrMin);
				case BuffParamKind.ACCURACY:
					return scale * (double)((long)clamped.AccuracyZero);
				case BuffParamKind.RECEIVE_CRITICAL_DAMAGE_RATE:
					return scale * (double)((long)clamped.ReceiveCriticalDamageRateOrMin);
				case BuffParamKind.PHYSICAL_DAMAGE_UP_PERCENT:
					return scale * (double)((double)clamped.PhysicalDamageUpPercentOrMin);
				case BuffParamKind.MAGIC_DAMAGE_UP_PERCENT:
					return scale * (double)((double)clamped.MagicDamageUpPercentOrMin);
				case BuffParamKind.MAX_HP:
					return scale * (double)((long)parameter.MaxHp);
				case BuffParamKind.BUFF_ENHANCE:
					return scale * (double)((long)parameter.BuffEnhance);
				default:
					return 0.0;
			}
		}

		[global::OpenPCR.Migration("65ded3ce-a06c-4ffb-a906-1c2967d218e3")]
		public static BuffParamKind GetChangeParamKind(int value)
		{
			if (value == 1)
				return BuffParamKind.MAX_HP;
			if (value == 200)
				return BuffParamKind.BUFF_ENHANCE;
			return (BuffParamKind)(value % 1000 / 10);
		}

		[global::OpenPCR.Migration("50150572-a44f-48bf-8d2f-65d1636781ad")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
			Value[eValueNumber.VALUE_4] = MasterData.action_value_4 + MasterData.action_value_5 * _level;
		}

		[global::OpenPCR.Migration("03549a86-4def-4481-8e0e-279c48e2d892")]
		public override double GetActionBaseValue(UnitCtrl _source, eValueNumber _type)
		{
			Value.TryGetValue(_type, out double value);
			if (_type != eValueNumber.VALUE_2)
				return value;
			BuffParamKind kind = GetChangeParamKind(ActionDetail1);
			if (!BUFF_DEBUFF_ENHANCE_TARGET_TYPE.Contains(kind) || IsExEquipSkillAction)
				return value;
			long enhance = ActionDetail1 % 10 == 1 ? (long)_source.ComponentParameterClamped.DebuffEnhanceZero : (long)_source.ComponentParameterClamped.BuffEnhanceZero;
			return value * ((double)enhance / 10000.0 + 1.0);
		}
	}
}
