using Elements.Battle;
using System;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class GiveValueAction : ActionParameter, ISingletonField
	{
		protected enum eAdditiveValueType
		{
			HP = 0,
			DAMAGE = 1,
			DEFEAT_NUMBER = 2,
			SEAL = 3,
			TARGET = 4,
			DAMAGED_TARGET = 5,
			SKILL_DAMAGE = 6,
			TARGET_ATK = 7,
			TARGET_MAGIC_ATK = 8,
			TARGET_DEF = 9,
			TARGET_MAGIC_DEF = 10,
			AHEAD_UNIT_NUM = 11,
			BEHIND_UNIT_NUM = 12,
			DAMAGE_RATE = 13,
			MAX_HP = 14,
			HP_SUM = 15,
			TP = 16,
			SKILL_COUNT = 20,
			STRIKE_BACK_COUNT = 100,
			SEAL_COUNT = 200,
			SEAL_COUNT_THREE_DIGIT = 2000
		}

		protected BasePartsData parts;
		protected BasePartsData parameterParts;
		[global::OpenPCR.Migration("4abf2c82-7da2-4bb7-a357-9ebe72232427")]
		public GiveValueAction() : base()
		{
		}

		[global::OpenPCR.Migration("586d367a-535f-4e99-9b4c-fee49ee89e22")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			switch (global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_1]))
			{
				case 2:
					{
						ActionParameter actionParameter = _skill.ActionParameters[0];
						int num = 0;
						for (int count = _skill.ActionParameters.Count; num < count; num++)
						{
							ActionParameter actionParameter2 = _skill.ActionParameters[num];
							actionParameter2.OnDefeatEnemy = (Action)Delegate.Combine(actionParameter2.OnDefeatEnemy, (Action)delegate
							{
								_skill.DefeatEnemyCount++;
							});
							if (actionParameter2.ActionId != base.ActionDetail2 && actionParameter2.ActionId != base.ActionDetail3)
							{
								actionParameter = ((actionParameter.ExecTime[actionParameter.ExecTime.Length - 1] > actionParameter2.ExecTime[actionParameter2.ExecTime.Length - 1]) ? actionParameter : actionParameter2);
							}
						}

						break;
					}

				case 11:
				case 12:
					parts = (_source.IsPartsBoss ? _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget) : _source.ComponentMultiPartsBoss.GetFirstParts());
					break;
			}

			if (base.Value[eValueNumber.VALUE_6] != 0f)
			{
				parameterParts = (_source.IsPartsBoss ? _source.ComponentMultiPartsBoss.BossPartsListForBattle.Find((PartsData e) => e.Index == global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_6])) : _source.ComponentMultiPartsBoss.GetFirstParts());
			}
		}

		[global::OpenPCR.Migration("30a64f4e-ac46-417c-8985-7225c7e966cd")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			Dictionary<eValueNumber, double> dictionary = new Dictionary<eValueNumber, double>();
			eValueNumber eValueNumber2 = (eValueNumber)(base.ActionDetail2 - 1);
			dictionary.Add(eValueNumber2, 0f);
			createValue(_source, _skill, _valueDictionary, dictionary, eValueNumber2, _target);
			base.battleLog.AppendBattleLog(eBattleLogType.GIVE_VALUE_ADDITIONAL, 0, 0L, 0L, 0, _source: _source, _target: _target.Owner, _actionId: base.ActionId);
			setValue(dictionary, _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail1));
		}

		[global::OpenPCR.Migration("e26e53b8-30c1-4d3c-b26c-e20de9d12569")]
		protected virtual void createValue(UnitCtrl _source, Skill _skill, Dictionary<eValueNumber, double> _valueDictionary, Dictionary<eValueNumber, double> _addValue, eValueNumber _evalue, BasePartsData _target)
		{
			switch ((eAdditiveValueType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]))
			{
				case eAdditiveValueType.HP:
					_addValue[_evalue] = calcHpValue(_source, _valueDictionary);
					return;
				case eAdditiveValueType.DAMAGE:
					_addValue[_evalue] = calcDamageValue(_source, _valueDictionary);
					return;
				case eAdditiveValueType.DEFEAT_NUMBER:
					_addValue[_evalue] = (double)_skill.DefeatEnemyCount * _valueDictionary[eValueNumber.VALUE_2];
					return;
				case eAdditiveValueType.TARGET:
					_addValue[_evalue] = (double)base.TargetList.Count * _valueDictionary[eValueNumber.VALUE_2];
					return;
				case eAdditiveValueType.DAMAGED_TARGET:
					_addValue[_evalue] = (double)_skill.DamagedPartsList.Count * _valueDictionary[eValueNumber.VALUE_2];
					return;
				case eAdditiveValueType.SKILL_DAMAGE:
					_addValue[_evalue] = (double)_skill.TotalDamage * _valueDictionary[eValueNumber.VALUE_2];
					return;
				case eAdditiveValueType.TARGET_ATK:
					if (_target.Owner == _source && parameterParts != null)
					{
						_addValue[_evalue] = (double)parameterParts.GetAtkZero() * _valueDictionary[eValueNumber.VALUE_2];
					}
					else
					{
						_addValue[_evalue] = (double)(long)_target.Owner.ComponentParameterClamped.AtkZero * _valueDictionary[eValueNumber.VALUE_2];
					}

					return;
				case eAdditiveValueType.TARGET_MAGIC_ATK:
					if (_target.Owner == _source && parameterParts != null)
					{
						_addValue[_evalue] = (double)parameterParts.GetMagicStrZero() * _valueDictionary[eValueNumber.VALUE_2];
					}
					else
					{
						_addValue[_evalue] = (double)(long)_target.Owner.ComponentParameterClamped.MagicStrZero * _valueDictionary[eValueNumber.VALUE_2];
					}

					return;
				case eAdditiveValueType.TARGET_DEF:
					if (_target.Owner == _source && parameterParts != null)
					{
						_addValue[_evalue] = (double)parameterParts.GetDefZero() * _valueDictionary[eValueNumber.VALUE_2];
					}
					else
					{
						_addValue[_evalue] = (double)_target.GetDefZero() * _valueDictionary[eValueNumber.VALUE_2];
					}

					return;
				case eAdditiveValueType.TARGET_MAGIC_DEF:
					if (_target.Owner == _source && parameterParts != null)
					{
						_addValue[_evalue] = (double)parameterParts.GetMagicDefZero() * _valueDictionary[eValueNumber.VALUE_2];
					}
					else
					{
						_addValue[_evalue] = (double)_target.GetMagicDefZero() * _valueDictionary[eValueNumber.VALUE_2];
					}

					return;
				case eAdditiveValueType.AHEAD_UNIT_NUM:
					{
						List<UnitCtrl> targetList = ((!_source.IsOther) ? base.battleManager.UnitList : base.battleManager.EnemyList);
						_addValue[_evalue] = (double)countUnitNumCompareX(targetList, parts, _ahead: true) * _valueDictionary[eValueNumber.VALUE_2];
						return;
					}

				case eAdditiveValueType.BEHIND_UNIT_NUM:
					{
						List<UnitCtrl> targetList2 = ((!_source.IsOther) ? base.battleManager.UnitList : base.battleManager.EnemyList);
						_addValue[_evalue] = (double)countUnitNumCompareX(targetList2, parts, _ahead: false) * _valueDictionary[eValueNumber.VALUE_2];
						return;
					}

				case eAdditiveValueType.DAMAGE_RATE:
					{
						long num = _target.Owner.ComponentParameter.MaxHp;
						double value = (double)(num - (long)_target.Owner.ComponentParameter.Hp) / (double)num * _valueDictionary[eValueNumber.VALUE_2];
						_addValue[_evalue] = BattleUtil.DoubleToPerMile(value);
						return;
					}

				case eAdditiveValueType.MAX_HP:
					_addValue[_evalue] = calcMaxHpValue(_source, _valueDictionary);
					return;
				case eAdditiveValueType.HP_SUM:
					{
						double sum = 0.0;
						foreach (BasePartsData part in TargetList)
							if (!part.Owner.ComponentMultiPartsBoss.IsAlreadyCounted)
							{
								part.Owner.ComponentMultiPartsBoss.SetIsAlreadyCountedParts(true);
								sum += _valueDictionary[eValueNumber.VALUE_2] * (double)(long)part.Owner.ComponentParameter.Hp;
							}

						foreach (BasePartsData part in TargetList)
							part.Owner.ComponentMultiPartsBoss.SetIsAlreadyCountedParts(false);
						_addValue[_evalue] = sum;
						return;
					}

				case eAdditiveValueType.TP:
					_addValue[_evalue] = _target.Owner.ComponentParameter.Energy * _valueDictionary[eValueNumber.VALUE_2];
					return;
				case eAdditiveValueType.SEAL:
					return;
			}

			if (_valueDictionary[eValueNumber.VALUE_1] > 200f)
			{
				eStateIconType eStateIconType2 = eStateIconType.INVALID_VALUE;
				eStateIconType2 = ((!(_valueDictionary[eValueNumber.VALUE_1] > 2000f)) ? ((eStateIconType)OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1] % 200f)) : ((eStateIconType)OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1] % 2000f)));
				SealData value2 = null;
				if (_target.Owner.ComponentAbnormal.SealDictionary.TryGetValue(eStateIconType2, out value2))
				{
					_addValue[_evalue] = (double)value2.GetCurrentCount() * _valueDictionary[eValueNumber.VALUE_2];
				}
			}
			else if (_valueDictionary[eValueNumber.VALUE_1] > 100f)
			{
				StrikeBackDataSet value3 = null;
				if (_target.Owner.ComponentAbnormal.StrikeBackDictionary.TryGetValue((EnchantStrikeBackAction.eStrikeBackEffectType)(OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]) - 100), out value3))
				{
					_addValue[_evalue] = (double)value3.DataList.Count * _valueDictionary[eValueNumber.VALUE_2];
				}
			}
			else if (_valueDictionary[eValueNumber.VALUE_1] >= 20f)
			{
				int key = OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]) % 10;
				if (_source.ComponentAbnormal.SkillExecCountDictionary.TryGetValue(key, out var value4))
				{
					_addValue[_evalue] = (double)value4 * _valueDictionary[eValueNumber.VALUE_2];
				}
			}
		}

		[global::OpenPCR.Migration("37f02657-a805-4f43-9373-501c83cb75f5")]
		protected virtual void setValue(Dictionary<eValueNumber, double> _value, ActionParameter _action)
		{
		}

		[global::OpenPCR.Migration("fc3b8b9b-91a6-43a4-8b63-1d86293c6c9b")]
		protected virtual double calcHpValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return 0f;
		}

		[global::OpenPCR.Migration("77ae1535-d302-4bfd-ad26-b30ade156d2c")]
		protected virtual double calcMaxHpValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return 0f;
		}

		[global::OpenPCR.Migration("8fa9ea77-5d00-4383-bb5c-a4c715a13c53")]
		protected virtual double calcDamageValue(UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			return 0f;
		}

		[global::OpenPCR.Migration("e88fbe58-fc5d-4de0-a894-ad979895e67f")]
		protected int countUnitNumCompareX(List<UnitCtrl> _targetList, BasePartsData _source, bool _ahead)
		{
			int num = 0;
			foreach (UnitCtrl _target in _targetList)
			{
				var partsComponent = _target.ComponentMultiPartsBoss;
				if (_target.IsPartsBoss)
				{
					foreach (PartsData item in partsComponent.BossPartsListForBattle)
					{
						if (comparePosition(_source, item, _ahead))
						{
							num++;
						}
					}
				}
				else if (comparePosition(_source, partsComponent.GetFirstParts(), _ahead))
				{
					num++;
				}
			}

			return num;
		}

		[global::OpenPCR.Migration("548fd080-5c74-4a39-916c-8b5df5ce6d0e")]
		private bool comparePosition(BasePartsData _source, BasePartsData _target, bool _ahead)
		{
			if (_source == _target)
			{
				return false;
			}

			if ((long)_target.Owner.ComponentParameter.Hp == 0L || _target.Owner.ComponentAbnormal.IsStealth)
			{
				return false;
			}

			if (_target.Owner.IsSummonOrPhantom && _target.Owner.ComponentState.IdleOnly)
			{
				return false;
			}

			float x = _source.GetBottomTransformPosition().x;
			float x2 = _target.GetBottomTransformPosition().x;
			if (BattleUtil.Approximately(x, x2))
			{
				return true;
			}

			if (!_source.Owner.IsLeftDir)
			{
				return (x < x2) == _ahead;
			}

			return (x > x2) == _ahead;
		}
	}
}
