using Elements.Battle;
using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class IfForChildlenAction : ActionParameter
	{
		[global::OpenPCR.Migration("01bac8bd-83fb-4ab3-8a9b-2cf8b10d3a0c")]
		public IfForChildlenAction() : base()
		{
		}

		[global::OpenPCR.Migration("f7a03e90-2a07-4d31-91fd-dc8e0af80f75")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			if (base.ActionDetail1 == 1001)
			{
				_skill.CriticalPartsList.Clear();
			}
		}

		[global::OpenPCR.Migration("d99b6135-50e8-4415-a880-7613c84870ea")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			if (ActionDetail1 == 1001)
				_skill.CriticalPartsList = new List<BasePartsData>();
		}

		[global::OpenPCR.Migration("d157bfe0-5124-42e2-907b-94c7ae9cf37e")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictinary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictinary);
			if (!_enabledChildAction.ContainsKey(base.ActionDetail2))
			{
				_enabledChildAction.Add(base.ActionDetail2, value: false);
			}

			if (!_enabledChildAction.ContainsKey(base.ActionDetail3))
			{
				_enabledChildAction.Add(base.ActionDetail3, value: false);
			}

			var targetAbnormal = _target.Owner.ComponentAbnormal;
			bool flag = false;
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
				case eIfType.DECOY:
					flag = targetAbnormal.IsAbnormalState(eAbnormalState.DECOY);
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

				case eIfType.BREAK:
					{
						if (!_target.Owner.IsPartsBoss)
						{
							break;
						}

						var parts = _target.Owner.ComponentMultiPartsBoss.BossPartsListForBattle;
						for (int num = 0; num < parts.Count; num++)
						{
							if (parts[num].IsBreak)
							{
								flag = true;
								break;
							}
						}

						break;
					}

				case eIfType.UNIT_ID:
					flag = base.TargetList.FindAll((BasePartsData e) => IfForAllAction.JudgeCountableUnit(e.Owner, _source, TargetAssignment)).Exists((BasePartsData e) => (double)e.Owner.UnitId == _valueDictinary[eValueNumber.VALUE_3]);
					break;
				case eIfType.CRITICAL:
					flag = _skill.CriticalPartsList.Contains(_target);
					break;
				case eIfType.ATK_TYPE:
					flag = _target.Owner.AtkType == 1;
					break;
				case eIfType.TOAD:
					flag = targetAbnormal.ToadDatas.Count > 0;
					break;
				case eIfType.BUFF_DEBUFF:
					{
						BuffParamKind changeParamKind = BuffDebuffAction.GetChangeParamKind(global::OpenPCR.NativeNumeric.ToInt32(_valueDictinary[eValueNumber.VALUE_3]));
						flag = targetAbnormal.IsBuffDebuffEnable(changeParamKind, global::OpenPCR.NativeNumeric.ToInt32(_valueDictinary[eValueNumber.VALUE_3]) % 10 != 1);
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
						flag = targetAbnormal.SealDictionary.ContainsKey(eStateIconType2) && ((_valueDictinary[eValueNumber.VALUE_3] != 0f) ? ((double)targetAbnormal.SealDictionary[eStateIconType2].GetCurrentCount() >= _valueDictinary[eValueNumber.VALUE_3]) : (targetAbnormal.SealDictionary[eStateIconType2].GetCurrentCount() > 0));
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
					else if (base.ActionDetail1 > 900)
					{
						flag = (double)(long)_target.Owner.ComponentParameter.Hp / (double)(long)_target.Owner.ComponentParameter.MaxHp < (double)(base.ActionDetail1 % 100) / 100.0;
					}
					else if (base.ActionDetail1 > 700)
					{
						flag = base.TargetList.FindAll((BasePartsData e) => IfForAllAction.JudgeCountableUnit(e.Owner, _source, TargetAssignment)).Count == base.ActionDetail1 - 700;
					}

					break;
			}

			if (flag)
			{
				_enabledChildAction[base.ActionDetail2] = true;
				_skill.EffectBranchId = global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_1]);
			}
			else
			{
				_enabledChildAction[base.ActionDetail3] = true;
				_skill.EffectBranchId = global::OpenPCR.NativeNumeric.ToInt32(base.Value[eValueNumber.VALUE_2]);
			}

			_sourceActionController.AppendCoroutine(_sourceActionController.UpdateBranchMotion(this, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
		}

		[global::OpenPCR.Migration("c3a66d7d-eab5-4141-8ce0-477c87e59a9e")]
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
	}
}
