using Elements.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class IfForAllAction : ActionParameter
	{
		[global::OpenPCR.Migration("14e7e45b-8f19-41d0-88a6-2d5c8902578c")]
		public IfForAllAction() : base()
		{
		}

		[global::OpenPCR.Migration("a65f112e-e7c2-4e36-8e76-cef4573a729e")]
		public static bool JudgeCountableUnit(UnitCtrl _unit, UnitCtrl _source, eTargetAssignment _targetAssignment)
		{
			UnitComponentAbnormal abnormal = _unit.ComponentAbnormal;
			bool protectedAllowed = _targetAssignment != eTargetAssignment.OTHER_SIDE || !abnormal.GetIsProtected();
			if ((long)_unit.ComponentParameter.Hp == 0L)
				return false;
			return !abnormal.IsStealth && protectedAllowed && !abnormal.JudgeIsEtherialization(_source);
		}

		[global::OpenPCR.Migration("51407bbb-d444-4532-99ac-9ae1778c8757")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			switch ((eIfType)base.ActionDetail1)
			{
				case eIfType.DEFEAT:
					{
						ActionParameter actionParameter = _skill.ActionParameters[0];
						int i = 0;
						for (int count = _skill.ActionParameters.Count; i < count; i++)
						{
							ActionParameter actionParameter2 = _skill.ActionParameters[i];
							actionParameter2.OnDefeatEnemy = (Action)Delegate.Combine(actionParameter2.OnDefeatEnemy, (Action)delegate
							{
								_skill.DefeatByThisSkill = true;
								_skill.EffectBranchId = BattleUtil.DoubleToInt(base.Value[eValueNumber.VALUE_1]);
								if (base.ActionDetail3 != 0)
								{
									_skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail3).CancelByIfForAll = true;
								}
							});
							if (actionParameter2.ActionId != base.ActionDetail2 && actionParameter2.ActionId != base.ActionDetail3 && actionParameter2 != this)
							{
								actionParameter = ((actionParameter.ExecTime[actionParameter.ExecTime.Length - 1] > actionParameter2.ExecTime[actionParameter2.ExecTime.Length - 1]) ? actionParameter : actionParameter2);
							}
						}

						ActionParameter actionParameter3 = actionParameter;
						actionParameter3.OnActionEnd = (OnActionEndDelegate)Delegate.Combine(actionParameter3.OnActionEnd, (OnActionEndDelegate)delegate
						{
							_source.AppendCoroutine(waitFrameEnd(_skill), ePauseType.SYSTEM, _source);
						});
						ActionParameter actionParameter4 = actionParameter;
						actionParameter4.OnInitWhenNoTarget = (Action)Delegate.Combine(actionParameter4.OnInitWhenNoTarget, (Action)delegate
						{
							if (base.ActionDetail2 != 0)
							{
								_skill.EffectBranchId = BattleUtil.DoubleToInt(base.Value[eValueNumber.VALUE_2]);
								_skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2).CancelByIfForAll = true;
							}
						});
						break;
					}

				case eIfType.CRITICAL:
					_skill.CriticalPartsList = new List<BasePartsData>();
					break;
			}
		}

		[global::OpenPCR.Migration("00addb79-04e6-4f2a-920d-9a992919a764")]
		private IEnumerator waitFrameEnd(Skill _skill)
		{
			yield return null;
			if (!_skill.DefeatByThisSkill && base.ActionDetail2 != 0)
			{
				_skill.EffectBranchId = BattleUtil.DoubleToInt(base.Value[eValueNumber.VALUE_2]);
				_skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2).CancelByIfForAll = true;
			}
		}

		[global::OpenPCR.Migration("e827f74b-e4bb-4894-ab6f-6f152c177687")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			if (base.ActionDetail1 == 1001)
			{
				_skill.CriticalPartsList.Clear();
			}
		}

		[global::OpenPCR.Migration("6ac6fe52-1380-4d3b-b68f-c539b011dc75")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictinary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictinary);
			bool flag = false;
			if (base.ActionDetail1 == 1000)
			{
				return;
			}

			var targetAbnormal = _target.Owner.ComponentAbnormal;
			switch ((eIfType)base.ActionDetail1)
			{
				case eIfType.STOP:
					flag = targetAbnormal.IsUnableActionState();
					break;
				case eIfType.HASTE:
					flag = targetAbnormal.IsHasteSpeed();
					break;
				case eIfType.SLOW:
					flag = targetAbnormal.IsSlowSpeed();
					break;
				case eIfType.PHYSICS_BLIND:
					flag = targetAbnormal.IsAbnormalState(eAbnormalState.PHYSICS_DARK);
					break;
				case eIfType.CONVERT:
					flag = targetAbnormal.IsAbnormalState(eAbnormalState.CONVERT);
					break;
				case eIfType.DECOY:
					flag = targetAbnormal.IsAbnormalState(eAbnormalState.DECOY);
					break;
				case eIfType.BURN:
					flag = targetAbnormal.IsAbnormalState(eAbnormalState.BURN);
					break;
				case eIfType.CURSE:
					flag = targetAbnormal.IsCurseState();
					break;
				case eIfType.POISON:
					flag = targetAbnormal.IsPoisonState();
					break;
				case eIfType.VENOM:
					flag = targetAbnormal.IsAbnormalState(eAbnormalState.VENOM);
					break;
				case eIfType.HEX:
					flag = targetAbnormal.IsAbnormalState(eAbnormalState.HEX);
					break;
				case eIfType.CURSE_OR_HEX:
					flag = targetAbnormal.IsCurseState() || targetAbnormal.IsAbnormalState(eAbnormalState.HEX);
					break;
				case eIfType.POISON_OR_VENOM:
					flag = targetAbnormal.IsPoisonState() || targetAbnormal.IsAbnormalState(eAbnormalState.VENOM);
					break;
				case eIfType.SLIP_DAMAGE:
					flag = targetAbnormal.IsSlipDamageState();
					break;
				case eIfType.ALONE:
					{
						List<UnitCtrl> list = (_source.IsOther ? base.battleManager.UnitList : base.battleManager.EnemyList);
						flag = !list.Exists((UnitCtrl e) => e.IsPartsBoss && !e.ComponentState.IsDead) && list.FindAll((UnitCtrl e) =>
						{
							if (!((!e.ComponentState.IsDead && (long)e.ComponentParameter.Hp > 0) || e.ComponentState.HasUnDeadTime))
								return false;
							var abnormal = e.ComponentAbnormal;
							return !abnormal.IsStealth && !abnormal.JudgeIsEtherialization(_source);
						}).Count == 1;
						break;
					}

				case eIfType.ATK_TYPE:
					flag = _target.Owner.AtkType == 1;
					break;
				case eIfType.TOAD:
					flag = targetAbnormal.ToadDatas.Count > 0;
					break;
				case eIfType.BREAK:
					{
						if (!_target.Owner.IsPartsBoss)
						{
							break;
						}

						var parts = _target.Owner.ComponentMultiPartsBoss.BossPartsListForBattle;
						for (int num2 = 0; num2 < parts.Count; num2++)
						{
							if (parts[num2].IsBreak)
							{
								flag = true;
								break;
							}
						}

						break;
					}

				case eIfType.UNIT_ID:
					flag = base.TargetList.FindAll((BasePartsData e) => JudgeCountableUnit(e.Owner, _source, TargetAssignment)).Exists((BasePartsData e) => BattleUtil.Approximately((double)e.Owner.ComponentSound.SoundUnitId, _valueDictinary[eValueNumber.VALUE_3]));
					break;
				case eIfType.SEAL_COUNT:
					{
						eStateIconType key = (eStateIconType)global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_3]);
						HashSet<UnitCtrl> hashSet = new HashSet<UnitCtrl>();
						for (int num = 0; num < base.TargetList.Count; num++)
						{
							UnitCtrl owner = base.TargetList[num].Owner;
							var abnormal = owner.ComponentAbnormal;
							if (!abnormal.GetIsProtected() && abnormal.SealDictionary.TryGetValue(key, out var value2) && value2.GetCurrentCount() != 0)
							{
								hashSet.Add(owner);
							}
						}

						flag = hashSet.Count >= BattleUtil.DoubleToInt(base.Value[eValueNumber.VALUE_4]);
						break;
					}

				case eIfType.CRITICAL:
					flag = _skill.CriticalPartsList.Count > 0;
					break;
				case eIfType.BUFF_DEBUFF:
					{
						int value = BattleUtil.DoubleToInt(_valueDictinary[eValueNumber.VALUE_3]);
						BuffParamKind changeParamKind = BuffDebuffAction.GetChangeParamKind(value);
						flag = targetAbnormal.IsBuffDebuffEnable(changeParamKind, value % 10 != 1);
						break;
					}

				case eIfType.MULTI_BOSS:
					flag = _target.Owner.IsPartsBoss;
					break;
				case eIfType.TARGET_HP:
					{
						var parameter = _target.Owner.ComponentParameter;
						flag = (long)parameter.Hp == (long)parameter.MaxHp;
						break;
					}
				case eIfType.ATK_TYPE_COUNT:
				case eIfType.MAGIC_TYPE_COUNT:
					{
						HashSet<UnitCtrl> owners = new HashSet<UnitCtrl>();
						int attackType = ActionDetail1 == 2000 ? 1 : 2;
						for (int index = 0; index < TargetList.Count && index < TargetNum; index++)
						{
							UnitCtrl owner = TargetList[index].Owner;
							UnitComponentAbnormal abnormal = owner.ComponentAbnormal;
							if (abnormal.GetIsProtected())
							{
								if (abnormal.ProtectionTargetData.ProtectionParts.Owner.AtkType == attackType)
									owners.Add(abnormal.ProtectionTargetData.ProtectionParts.Owner);
							}
							else if (owner.AtkType == attackType)
								owners.Add(owner);
						}

						flag = owners.Count >= BattleUtil.DoubleToInt(Value[eValueNumber.VALUE_3]);
						break;
					}

				case eIfType.BARRIER:
					flag = targetAbnormal.IsBarrierState();
					break;
				default:
					if (base.ActionDetail1 < 100)
					{
						flag = BattleManager.Random(0f, 100f) < (float)base.ActionDetail1;
					}
					else if ((base.ActionDetail1 > 600 && base.ActionDetail1 < 700) || base.ActionDetail1 > 6000)
					{
						eStateIconType eStateIconType2 = eStateIconType.INVALID_VALUE;
						eStateIconType2 = (eStateIconType)((base.ActionDetail1 <= 6000) ? (base.ActionDetail1 - 600) : (base.ActionDetail1 - 6000));
						flag = targetAbnormal.SealDictionary.ContainsKey(eStateIconType2) && ((_valueDictinary[eValueNumber.VALUE_3] != 0f) ? (targetAbnormal.SealDictionary[eStateIconType2].GetCurrentCount() >= BattleUtil.DoubleToInt(_valueDictinary[eValueNumber.VALUE_3])) : (targetAbnormal.SealDictionary[eStateIconType2].GetCurrentCount() > 0));
					}
					else if (base.ActionDetail1 >= 4000)
						flag = _target.Owner.TalentIds.Contains(base.ActionDetail1 - 4000);
					else if (base.ActionDetail1 >= 3000)
						flag = battleManager.ExistsEnvironment(base.ActionDetail1 - 3000);
					else if (base.ActionDetail1 >= 1600)
					{
						flag = isAbnormalState((eIfAbnormalState)(base.ActionDetail1 % 100), _target);
					}
					else if (base.ActionDetail1 > 1500)
					{
						eAbnormalState abnormalState = ChangeSpeedAction.AbnormalStateDic[(ChangeSpeedAction.eChangeSpeedType)(base.ActionDetail1 % 100)];
						flag = targetAbnormal.IsAbnormalState(abnormalState);
					}
					else if (base.ActionDetail1 > 1200)
					{
						int value = 0;
						targetAbnormal.SkillExecCountDictionary.TryGetValue(base.ActionDetail1 % 100 / 10, out value);
						flag = value == base.ActionDetail1 % 10;
					}
					else if (base.ActionDetail1 > 900)
					{
						var parameter = _target.Owner.ComponentParameter;
						flag = (double)(long)parameter.Hp / (double)(long)parameter.MaxHp < (double)(base.ActionDetail1 % 100) / 100.0;
					}
					else if (base.ActionDetail1 > 700)
					{
						flag = base.TargetList.FindAll((BasePartsData e) => JudgeCountableUnit(e.Owner, _source, TargetAssignment)).Count == base.ActionDetail1 - 700;
					}

					break;
			}

			if (!flag)
			{
				_skill.EffectBranchId = BattleUtil.DoubleToInt(base.Value[eValueNumber.VALUE_2]);
				if (base.ActionDetail2 != 0)
				{
					cancelAction(_skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2), _skill);
				}
			}
			else
			{
				_skill.EffectBranchId = BattleUtil.DoubleToInt(base.Value[eValueNumber.VALUE_1]);
				if (base.ActionDetail3 != 0)
				{
					cancelAction(_skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail3), _skill);
				}
			}

			_sourceActionController.AppendCoroutine(_sourceActionController.UpdateBranchMotion(this, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
		}

		[global::OpenPCR.Migration("e4f3159c-3484-4c9c-b3bd-16b40d4a89d3")]
		private bool isAbnormalState(eIfAbnormalState _abnormalState, BasePartsData _target)
		{
			if ((unchecked((uint)(@_abnormalState)) == 2u))
			{
				if (global::System.Object.ReferenceEquals(@_target, null))
					throw new global::System.NullReferenceException();
				global::Elements.UnitCtrl nativeGetter1 = (@_target).Owner;
				if (global::System.Object.ReferenceEquals(nativeGetter1, null))
					throw new global::System.NullReferenceException();
				global::Elements.Battle.Core.UnitComponentAbnormal nativeValue2 = (nativeGetter1).ComponentAbnormal;
				if (global::System.Object.ReferenceEquals(nativeValue2, null))
					throw new global::System.NullReferenceException();
				bool nativeResult1 = (nativeValue2).IsAbnormalState(unchecked((global::Elements.Battle.Core.eAbnormalState)(71u)));
				return ((nativeResult1 ? 1u : 0u) != 0);
			}
			else
			{
				if ((unchecked((uint)(@_abnormalState)) == 1u))
				{
					if (global::System.Object.ReferenceEquals(@_target, null))
						throw new global::System.NullReferenceException();
					global::Elements.UnitCtrl nativeGetter3 = (@_target).Owner;
					if (global::System.Object.ReferenceEquals(nativeGetter3, null))
						throw new global::System.NullReferenceException();
					global::Elements.Battle.Core.UnitComponentAbnormal nativeValue4 = (nativeGetter3).ComponentAbnormal;
					if (global::System.Object.ReferenceEquals(nativeValue4, null))
						throw new global::System.NullReferenceException();
					bool nativeResult2 = (nativeValue4).IsAbnormalState(unchecked((global::Elements.Battle.Core.eAbnormalState)(67u)));
					return ((nativeResult2 ? 1u : 0u) != 0);
				}
				else
				{
					if (!((unchecked((uint)(@_abnormalState)) == 0)))
					{
						return false;
					}
					else
					{
						if (global::System.Object.ReferenceEquals(@_target, null))
							throw new global::System.NullReferenceException();
						global::Elements.UnitCtrl nativeGetter5 = (@_target).Owner;
						if (global::System.Object.ReferenceEquals(nativeGetter5, null))
							throw new global::System.NullReferenceException();
						global::Elements.Battle.Core.UnitComponentAbnormal nativeValue6 = (nativeGetter5).ComponentAbnormal;
						if (global::System.Object.ReferenceEquals(nativeValue6, null))
							throw new global::System.NullReferenceException();
						bool nativeResult3 = (nativeValue6).IsAbnormalState(unchecked((global::Elements.Battle.Core.eAbnormalState)(35u)));
						return ((nativeResult3 ? 1u : 0u) != 0);
					}
				}
			}
		}

		[global::OpenPCR.Migration("fd7afeef-3090-4df9-a549-867a05e5b953")]
		private void cancelAction(ActionParameter _action, Skill _skill)
		{
			_action.CancelByIfForAll = true;
			if (_action is IfForAllAction)
			{
				(_action as IfForAllAction).CancelBoth(_skill);
			}
		}

		[global::OpenPCR.Migration("f462718c-0614-4329-8cf1-bc2fbce86614")]
		public void CancelBoth(Skill _skill)
		{
			if (base.ActionDetail2 != 0)
			{
				cancelAction(_skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2), _skill);
			}

			if (base.ActionDetail3 != 0)
			{
				cancelAction(_skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail3), _skill);
			}
		}
	}
}
