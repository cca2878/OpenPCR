using global::OpenPCR.Foundation;
using Elements.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class TriggerAction : ActionParameter
	{
		private enum eTriggerType
		{
			DODGE = 1,
			DAMAGED,
			HP,
			DEAD,
			CRITICAL_DAMAGED,
			CRITICAL_DAMAGED_WITH_SUMMON,
			LIMIT_TIME,
			STEALTH_FREE,
			BREAK,
			SLIP_DAMAGE,
			ALL_BREAK,
			DAMAGED_2,
			DAMAGED_HP,
			DAMAGED_FOR_EXEC_TO_SOURCE
		}
		private int actionCountWaveReset;
		[global::OpenPCR.Migration("af964339-e1dc-4f93-a1f2-56e532380cd3")]
		public TriggerAction() : base()
		{
		}

		[global::OpenPCR.Migration("10b29efd-8256-4c11-86c2-6198a4fdd5ff")]
		public override void ExecActionOnWaveStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			actionCountWaveReset = 0;
			base.ExecActionOnWaveStart(_skill, _source, _sourceActionController);
			var component = _source.ComponentMultiPartsBoss;
			PartsData part = component.BossPartsListForBattle.Find(e => e.Index == _skill.ParameterTarget);
			if (ActionDetail1 == 9)
				component.AddPartsBreakTime(part.Index, (float)Value[eValueNumber.VALUE_3]);
		}

		[global::OpenPCR.Migration("d5638a21-7982-49ed-a3d2-6afb0bb19516")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			var sourceParts = _source.ComponentMultiPartsBoss;
			PartsData parts = sourceParts.BossPartsListForBattle.Find((PartsData e) => e.Index == _skill.ParameterTarget);
			if (sourceParts.MultiBossPartsData != null)
			{
				parts = sourceParts.MultiBossPartsData;
			}

			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			UnitCtrl target = _source;
			if (base.ActionDetail2 != 0)
			{
				List<UnitCtrl> list = ((!_source.IsOther) ? base.battleManager.UnitList : base.battleManager.EnemyList);
				target = list.Find((UnitCtrl e) => e.UnitId == base.ActionDetail2);
				if (target == null)
				{
					return;
				}
			}

			bool actionDone = false;
			int actionCount = 0;
			switch ((eTriggerType)base.ActionDetail1)
			{
				case eTriggerType.DODGE:
					{
						UnitCtrl unitCtrl6 = target;
						unitCtrl6.ComponentDamageControl.OnDodge = (Action)Delegate.Combine(unitCtrl6.ComponentDamageControl.OnDodge, (Action)delegate
						{
							float num2 = BattleManager.Random(0f, 1f);
							if (_source.ComponentSearchArea.TargetEnemyList.Count != 0 && !judgeSilenceOrToad(_source) && (_source.ComponentState.CurrentState != ActionState.SKILL || _source.CurrentSkillId != _skill.SkillId) && num2 <= base.Value[eValueNumber.VALUE_1] / 100.0)
							{
								if (_source.GetCurrentSpineCtrl().IsAnimation(_skill.AnimId, _skill.SkillNum))
								{
									_source.ComponentState.SetState(ActionState.SKILL, 0, _skill.SkillId);
								}
								else
								{
									_sourceActionController.StartAction(_skill.SkillId);
								}
							}
						});
						break;
					}

				case eTriggerType.DAMAGED:
					{
						UnitCtrl unitCtrl5 = target;
						unitCtrl5.ComponentDamageControl.OnDamage = (UnitComponentDamageControl.OnDamageDelegate)Delegate.Combine(unitCtrl5.ComponentDamageControl.OnDamage, (UnitComponentDamageControl.OnDamageDelegate)delegate (bool byAtack, long damage, bool critical)
						{
							if (byAtack)
							{
								float num2 = BattleManager.Random(0f, 1f);
								if (_source.ComponentSearchArea.TargetEnemyList.Count != 0 && !judgeSilenceOrToad(_source) && (_source.ComponentState.CurrentState != ActionState.SKILL || _source.CurrentSkillId != _skill.SkillId) && num2 <= base.Value[eValueNumber.VALUE_1] / 100.0)
								{
									if (_source.GetCurrentSpineCtrl().IsAnimation(_skill.AnimId, _skill.SkillNum))
									{
										_source.ComponentState.SetState(ActionState.SKILL, 0, _skill.SkillId);
									}
									else
									{
										_sourceActionController.StartAction(_skill.SkillId);
									}
								}
							}
						});
						break;
					}

				case eTriggerType.HP:
					{
						if (base.Value[eValueNumber.VALUE_4] == 99f)
						{
							target.ComponentAbnormal.IsTough = true;
						}

						if (base.Value[eValueNumber.VALUE_3] == 0f)
						{
							target.ComponentState.HasUnDeadTime = true;
						}

						Action<bool> hpAction = null;
						UnitCtrl unitCtrl8 = target;
						unitCtrl8.ComponentDamageControl.OnHpChange = (UnitComponentDamageControl.OnDamageDelegate)Delegate.Combine(unitCtrl8.ComponentDamageControl.OnHpChange, (UnitComponentDamageControl.OnDamageDelegate)delegate
						{
							var initialState = _source.ComponentState;
							float num2 = BattleManager.Random(0f, 1f);
							if (initialState.HasUnDeadTime)
							{
								if (initialState.UnDeadTimeHitCount > 1)
								{
									initialState.UnDeadTimeHitCount--;
									if (initialState.UnDeadTimeHitCount == 1)
									{
										initialState.SetState(ActionState.DIE);
									}
								}
								else
								{
									actionDone = false;
								}
							}

							if (!judgeSilenceOrToad(_source) && !_source.ComponentAbnormal.IsUnableActionState() && !_source.ComponentState.IsDead && num2 <= base.Value[eValueNumber.VALUE_1] / 100.0 && !actionDone)
							{
								var hpParameter = _source.ComponentParameter;
								long hp = hpParameter.Hp;
								double hpRate = base.Value[eValueNumber.VALUE_3] / 100.0;
								long maxHp = hpParameter.MaxHp;
								if (!BattleUtil.LessThanOrApproximately((double)hp, hpRate * (double)maxHp))
									return;
								actionDone = true;
								hpAction ??= delegate (bool _waitUbTurn)
								{
									if (judgeSilenceOrToad(_source) || _source.ComponentAbnormal.IsUnableActionState())
									{
										actionDone = false;
									}
									else
									{
										if ((_skill.BlackOutTime > 0f) & _waitUbTurn)
										{
											base.battleManager.CancelByAwake = true;
										}

										_source.SkillUseCount[_skill.SkillId]++;
										_source.ComponentState.CancelByAwake = true;
										_source.ComponentState.CurrentTriggerSkillId = _skill.SkillId;
										if (_skill.BlackOutTime > 0f)
										{
											_sourceActionController.StopSlowEffect();
											base.battleManager.SetSkillEffectSortOrderBack();
											base.battleManager.GamePause(pause: true, _isBlackoutEnd: false);
											base.battleManager.SetSkillExeScreen(_source, _skill.BlackOutTime, _skill.BlackoutColor, _skill.BlackoutEndWithMotion);
											_source.ComponentSortOrder.SetSortOrderFront();
										}

										_source.ComponentState.SetState(ActionState.SKILL, 0, _skill.SkillId);
									}
								};
								Action<bool> action = hpAction;
								if (_source.ComponentState.CurrentState == ActionState.UNION_BURST)
								{
									_source.AppendCoroutine(waitStateIdle(action, _source), ePauseType.SYSTEM, _source);
								}
								else if (base.battleManager.ChargeSkillTurn == eChargeSkillTurn.NONE)
								{
									action.Call(arg1: false);
								}
								else
								{
									_source.AppendCoroutine(waitChargeSkillTurnNone(action, _source), ePauseType.SYSTEM, _source);
								}
							}
						});
						break;
					}

				case eTriggerType.DEAD:
					{
						if (base.ActionDetail3 != 0 && target.SkillUseCount[_skill.SkillId] >= base.ActionDetail3)
						{
							break;
						}

						UnitCtrl unitCtrl4 = target;
						unitCtrl4.ComponentState.OnDeadForRevival = (UnitComponentState.OnDeadDelegate)Delegate.Combine(unitCtrl4.ComponentState.OnDeadForRevival, (UnitComponentState.OnDeadDelegate)delegate (UnitCtrl owner)
						{
							if ((target.ComponentState.EnemyPoint == 0 || base.battleManager.CurrentEnemyPoint < base.battleManager.GetSpecialBattleData().PurposeCount) && !judgeSilenceOrToad(_source))
							{
								_source.SkillUseCount[_skill.SkillId]++;
								if (BattleManager.Random(0f, 1f) <= base.Value[eValueNumber.VALUE_1] / 100.0)
								{
									if (base.ActionDetail3 == 0 || actionCount < base.ActionDetail3)
									{
										int skillId = _skill.SkillId;
										actionCount++;
										if (base.ActionDetail3 != 0 && actionCount == base.ActionDetail3)
										{
											target.ComponentState.OnDeadForRevival = null;
										}

										if (skillId == owner.UnionBurstSkillId)
										{
											_sourceActionController.AppendCoroutine(owner.ComponentState.SetStateWithDelayForRevival(base.ExecTime[0], ActionState.UNION_BURST, 0, skillId), ePauseType.SYSTEM, owner);
										}
										else
										{
											_sourceActionController.AppendCoroutine(owner.ComponentState.SetStateWithDelayForRevival(base.ExecTime[0], ActionState.SKILL, 0, skillId), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? owner : null);
										}
									}
								}
								else
								{
									_source.ComponentState.IsDead = true;
									base.battleManager.CheckAcnLosingBattleFinish();
									base.battleManager.CallbackDead(_source);
									_source.gameObject.SetActive(value: false);
									base.battleManager.CallbackFadeOutDone(_source);
								}
							}
						});
						break;
					}

				case eTriggerType.CRITICAL_DAMAGED:
					{
						UnitCtrl unitCtrl7 = target;
						unitCtrl7.ComponentDamageControl.OnDamage = (UnitComponentDamageControl.OnDamageDelegate)Delegate.Combine(unitCtrl7.ComponentDamageControl.OnDamage, (UnitComponentDamageControl.OnDamageDelegate)delegate (bool byAtack, long damage, bool critical)
						{
							if (!_source.ComponentState.IsDead && !_source.ComponentState.IdleOnly && !judgeSilenceOrToad(_source) && critical && BattleManager.Random(0f, 1f) <= base.Value[eValueNumber.VALUE_1] / 100.0)
							{
								if (base.Value[eValueNumber.VALUE_4] != 1f)
								{
									_source.ComponentState.CancelByAwake = true;
									_source.ComponentState.CurrentTriggerSkillId = _skill.SkillId;
									_source.ComponentState.SetState(ActionState.SKILL, 0, _skill.SkillId);
								}
								else
								{
									_sourceActionController.StartAction(_skill.SkillId);
								}
							}
						});
						break;
					}

				case eTriggerType.CRITICAL_DAMAGED_WITH_SUMMON:
					{
						UnitCtrl unitCtrl2 = target;
						unitCtrl2.ComponentDamageControl.OnDamage = (UnitComponentDamageControl.OnDamageDelegate)Delegate.Combine(unitCtrl2.ComponentDamageControl.OnDamage, (UnitComponentDamageControl.OnDamageDelegate)delegate (bool byAtack, long damage, bool critical)
						{
							bool flag = false;
							foreach (KeyValuePair<int, UnitCtrl> item2 in target.SummonUnitDictionary)
							{
								if (!item2.Value.ComponentState.IsDead)
								{
									flag = true;
								}
							}

							if (flag && !_source.ComponentState.IsDead && !_source.ComponentState.IdleOnly && !judgeSilenceOrToad(_source) && critical && BattleManager.Random(0f, 1f) <= base.Value[eValueNumber.VALUE_1] / 100.0)
							{
								_source.ComponentState.CancelByAwake = true;
								_source.ComponentState.CurrentTriggerSkillId = _skill.SkillId;
								_source.ComponentState.SetState(ActionState.SKILL, 0, _skill.SkillId);
							}
						});
						break;
					}

				case eTriggerType.LIMIT_TIME:
					{
						bool used2 = false;
						UnitCtrl unitCtrl3 = target;
						unitCtrl3.ComponentState.OnUpdateWhenIdle = (Action<float>)Delegate.Combine(unitCtrl3.ComponentState.OnUpdateWhenIdle, (Action<float>)delegate (float _limitTime)
						{
							if (!judgeSilenceOrToad(_source) && !used2 && _limitTime <= base.Value[eValueNumber.VALUE_3])
							{
								used2 = true;
								if (BattleManager.Random(0f, 1f) <= base.Value[eValueNumber.VALUE_1] / 100.0)
								{
									var state = _source.ComponentState;
									state.CancelByAwake = true;
									state.CurrentTriggerSkillId = _skill.SkillId;
									if (_skill.BlackOutTime > 0f)
									{
										_sourceActionController.StopSlowEffect();
										base.battleManager.SetSkillEffectSortOrderBack();
										base.battleManager.GamePause(pause: true, _isBlackoutEnd: false);
										base.battleManager.SetSkillExeScreen(_source, _skill.BlackOutTime, _skill.BlackoutColor, _skill.BlackoutEndWithMotion);
										_source.ComponentSortOrder.SetSortOrderFront();
									}

									state.SetState(ActionState.SKILL, 0, _skill.SkillId);
								}
							}
						});
						break;
					}

				case eTriggerType.STEALTH_FREE:
					{
						bool used = false;
						_source.ComponentAbnormal.IsTough = base.Value[eValueNumber.VALUE_3] == 0f;
						UnitCtrl unitCtrl = _source;
						unitCtrl.ComponentState.OnUpdateWhenIdle = (Action<float>)Delegate.Combine(unitCtrl.ComponentState.OnUpdateWhenIdle, (Action<float>)delegate
						{
							if (!judgeSilenceOrToad(_source) && !used && _source.ComponentAbnormal.IsStealth && _source.ActionsTargetOnMe.Count == 0)
							{
								used = true;
								if (BattleManager.Random(0f, 1f) <= base.Value[eValueNumber.VALUE_1] / 100.0)
								{
									_source.ComponentState.CancelByAwake = true;
									_source.ComponentState.TriggerStealthUnableStateBarrier = true;
									_source.ComponentState.CurrentTriggerSkillId = _skill.SkillId;
									_source.ComponentState.SetState(ActionState.SKILL, 0, _skill.SkillId);
								}
							}
						});
						break;
					}

				case eTriggerType.BREAK:
					{
						parts.BreakEffectList = _skill.SkillEffects;
						UnitCtrl unitCtrl12 = _source;
						unitCtrl12.ComponentState.OnUpdateWhenIdle = (Action<float>)Delegate.Combine(unitCtrl12.ComponentState.OnUpdateWhenIdle, (Action<float>)delegate
						{
							if ((int)parts.BreakPoint == 0 && !parts.IsBreak && !judgeSilenceOrToad(_source) && base.battleManager.ChargeSkillTurn == eChargeSkillTurn.NONE && BattleManager.Random(0f, 1f) <= base.Value[eValueNumber.VALUE_1] / 100.0)
							{
								_source.AppendBreakLog(parts.BreakSource);
								parts.IsBreak = true;
								parts.OnBreak.Call();
								parts.SetBreak(_enable: true, base.battleManager.UnitUiCtrl.transform, _displayEffect: true);
								_source.AppendCoroutine(_source.ComponentMultiPartsBoss.UpdateBreak((float)base.Value[eValueNumber.VALUE_3], parts), ePauseType.SYSTEM);
							}
						});
						break;
					}

				case eTriggerType.SLIP_DAMAGE:
					{
						UnitCtrl unitCtrl11 = target;
						unitCtrl11.ComponentAbnormal.OnSlipDamage = (Action)Delegate.Combine(unitCtrl11.ComponentAbnormal.OnSlipDamage, (Action)delegate
						{
							if (_source.ComponentSearchArea.TargetEnemyList.Count != 0 && !judgeSilenceOrToad(_source) && (_source.ComponentState.CurrentState != ActionState.SKILL || _source.CurrentSkillId != _skill.SkillId) && BattleManager.Random(0f, 1f) <= base.Value[eValueNumber.VALUE_1] / 100.0)
							{
								if (base.Value[eValueNumber.VALUE_4] != 1f && _source.GetCurrentSpineCtrl().IsAnimation(_skill.AnimId, _skill.SkillNum))
								{
									_source.ComponentState.SetState(ActionState.SKILL, 0, _skill.SkillId);
								}
								else
								{
									_sourceActionController.StartAction(_skill.SkillId);
								}
							}
						});
						break;
					}

				case eTriggerType.ALL_BREAK:
					{
						UnitCtrl unitCtrl10 = target;
						unitCtrl10.ComponentState.OnBreakAll = (Action)Delegate.Combine(unitCtrl10.ComponentState.OnBreakAll, (Action)delegate
						{
							if (!judgeSilenceOrToad(_source) && BattleManager.Random(0f, 1f) <= base.Value[eValueNumber.VALUE_1] / 100.0)
							{
								if (base.Value[eValueNumber.VALUE_4] != 1f && _source.GetCurrentSpineCtrl().IsAnimation(_skill.AnimId, _skill.SkillNum))
								{
									_source.ComponentState.SetState(ActionState.SKILL, 0, _skill.SkillId);
								}
								else
								{
									_sourceActionController.StartAction(_skill.SkillId);
								}
							}
						});
						break;
					}

				case eTriggerType.DAMAGED_2:
					{
						UnitCtrl unitCtrl9 = target;
						unitCtrl9.ComponentDamageControl.OnDamage = (UnitComponentDamageControl.OnDamageDelegate)Delegate.Combine(unitCtrl9.ComponentDamageControl.OnDamage, (UnitComponentDamageControl.OnDamageDelegate)delegate
						{
							if (!_source.ComponentState.IsDead && !_source.ComponentState.IdleOnly && !judgeSilenceOrToad(_source) && BattleManager.Random(0f, 1f) <= base.Value[eValueNumber.VALUE_1] / 100.0)
							{
								if (base.Value[eValueNumber.VALUE_4] != 1f)
								{
									_source.ComponentState.CancelByAwake = true;
									_source.ComponentState.CurrentTriggerSkillId = _skill.SkillId;
									_source.ComponentState.SetState(ActionState.SKILL, 0, _skill.SkillId);
								}
								else
								{
									_sourceActionController.StartAction(_skill.SkillId);
								}
							}
						});
						break;
					}

				case eTriggerType.DAMAGED_HP:
					{
						UnitComponentParameter sourceParameter = _source.ComponentParameter;
						long num = unchecked((long)sourceParameter.Hp - BattleUtil.DoubleToLong(base.Value[eValueNumber.VALUE_3]));
						List<PrefabWithTime> list2 = new List<PrefabWithTime>();
						foreach (NormalSkillEffect actionEffect in base.ActionEffectList)
						{
							PrefabWithTime item = new PrefabWithTime
							{
								Prefab = actionEffect.Prefab,
								PrefabLeft = actionEffect.PrefabLeft,
								Time = actionEffect.ExecTime[0],
								TargetBoneName = actionEffect.TargetBoneName,
								IsTrackRotation = actionEffect.TrackRotation
							};
							list2.Add(item);
						}

						if (list2.Count > 0)
						{
							_source.AppendCoroutine(_source.ComponentState.CreatePrefabWithTime(list2, _isIdle: false, -1, _isAura: true, _isShieldEffect: false, _ignorePause: false, base.ActionId), ePauseType.NO_DIALOG);
						}

						UnitComponentDamageControl sourceDamage = _source.ComponentDamageControl;
						if (num > 0)
						{
							sourceDamage.DamagedTriggerHpAndActionDoneList.Add((num, false, base.ActionId, _skill));
							sourceDamage.DamagedTriggerHpAndActionDoneList.Sort(((long triggerHp, bool isActionDone, int actionId, Skill skill) a, (long triggerHp, bool isActionDone, int actionId, Skill skill) b) => b.triggerHp.CompareTo(a.triggerHp));
						}

						if (sourceDamage.DamagedTriggerHpAndActionDoneList.Count != 1)
						{
							break;
						}

						bool actionDoing = false;
						target.ComponentDamageControl.OnHpChangeForDamagedHP = delegate
						{
							float num2 = BattleManager.Random(0f, 1f);
							if (!judgeSilenceOrToad(_source) && !_source.ComponentAbnormal.IsUnableActionState() && !_source.ComponentState.IsDead && !(num2 > base.Value[eValueNumber.VALUE_1] / 100.0))
							{
								for (int i = 0; i < sourceDamage.DamagedTriggerHpAndActionDoneList.Count; i++)
								{
									int iForAction = i;
									var (triggerHp, flag, actionId, skill) = sourceDamage.DamagedTriggerHpAndActionDoneList[i];
									if (!flag)
									{
										if ((long)sourceParameter.Hp <= triggerHp && !actionDoing)
										{
											actionDoing = true;
											sourceDamage.DamagedTriggerHpAndActionDoneList[iForAction] = (triggerHp, true, actionId, skill);
											Action<bool> action = delegate (bool _waitUbTurn)
											{
												actionDoing = false;
												if (judgeSilenceOrToad(_source) || _source.ComponentAbnormal.IsUnableActionState())
												{
													sourceDamage.DamagedTriggerHpAndActionDoneList[iForAction] = (triggerHp, false, actionId, skill);
												}
												else
												{
													_source.ComponentState.DeleteDamagedHpAuraEffect(actionId);
													if ((skill.BlackOutTime > 0f) & _waitUbTurn)
													{
														base.battleManager.CancelByAwake = true;
													}

													_source.SkillUseCount[skill.SkillId]++;
													_source.ComponentState.CancelByAwake = true;
													_source.ComponentState.CurrentTriggerSkillId = skill.SkillId;
													if (skill.BlackOutTime > 0f)
													{
														_sourceActionController.StopSlowEffect();
														base.battleManager.SetSkillEffectSortOrderBack();
														base.battleManager.GamePause(pause: true, _isBlackoutEnd: false);
														base.battleManager.SetSkillExeScreen(_source, skill.BlackOutTime, skill.BlackoutColor, skill.BlackoutEndWithMotion);
														_source.ComponentSortOrder.SetSortOrderFront();
													}

													_source.ComponentState.SetState(ActionState.SKILL, 0, skill.SkillId);
												}
											};
											if (_source.ComponentState.CurrentState == ActionState.UNION_BURST)
											{
												_source.AppendCoroutine(waitStateIdle(action, _source), ePauseType.SYSTEM, _source);
											}
											else if (base.battleManager.ChargeSkillTurn == eChargeSkillTurn.NONE)
											{
												action.Call(arg1: false);
											}
											else
											{
												_source.AppendCoroutine(waitChargeSkillTurnNone(action, _source), ePauseType.SYSTEM, _source);
											}
										}

										break;
									}
								}
							}
						};
						break;
					}

				case eTriggerType.DAMAGED_FOR_EXEC_TO_SOURCE:
					{
						foreach (ActionParameter action in _skill.ActionParameters)
							action.IgnoreTriggerCausedByTrigger = true;
						target.ComponentDamageControl.OnDamageForExecToSource += (damageData, action) =>
						{
							if (ActionDetail3 != 0 && actionCountWaveReset == ActionDetail3)
								return;
							if (damageData.Source == null)
								return;
							if (action.IgnoreTriggerCausedByTrigger || action.AlreadyTriggeredActions.Contains(this))
								return;
							float random = BattleManager.Random(0f, 1f);
							if (!(Value[eValueNumber.VALUE_1] / 100.0 >= (double)random))
								return;
							action.AlreadyTriggeredActions.Add(this);
							actionCountWaveReset++;
							Action<bool> execute = _waitUbTurn =>
							{
								if (damageData.ActionType == eActionType.ATTACK || damageData.ActionType == eActionType.RATIO_DAMAGE || damageData.ActionType == eActionType.UPPER_LIMIT_ATTACK)
									_source.UnitActionController.ExecSkillWithTargetParts(_skill, damageData.Source.ComponentMultiPartsBoss.GetIndexSmallestParts());
							};
							if (_source.ComponentState.CurrentState == ActionState.UNION_BURST)
								_source.AppendCoroutine(waitStateIdle(execute, _source), ePauseType.SYSTEM, _source);
							else if (battleManager.ChargeSkillTurn == eChargeSkillTurn.NONE)
								execute.Call(false);
							else
								_source.AppendCoroutine(waitChargeSkillTurnNone(execute, _source), ePauseType.SYSTEM, _source);
						};
						break;
					}
			}
		}

		[global::OpenPCR.Migration("c101d807-d3d5-47bb-bebd-f8bd2e4b9ef8")]
		private bool judgeSilenceOrToad(UnitCtrl _unitCtrl)
		{
			var abnormal = _unitCtrl.ComponentAbnormal;
			return abnormal.IsAbnormalState(eAbnormalState.SILENCE) || abnormal.ToadDatas.Count > 0;
		}

		[global::OpenPCR.Migration("bd4e042f-860d-47df-8def-efd5aaafd6bb")]
		private IEnumerator waitStateIdle(Action<bool> _callback, UnitCtrl _source)
		{
			var state = _source.ComponentState;
			while (state.CurrentState != ActionState.IDLE)
				yield return null;
			_callback.Call(false);
		}

		[global::OpenPCR.Migration("6a123067-0d22-4461-9853-8471c993f03a")]
		private IEnumerator waitChargeSkillTurnNone(Action<bool> _callback, UnitCtrl _source)
		{
			while (true)
			{
				yield return null;
				if (battleManager.ChargeSkillTurn != eChargeSkillTurn.NONE)
					continue;
				if (Value[eValueNumber.VALUE_3] != 0.0 && _source.ComponentParameter.Hp == 0)
					yield break;
				_callback.Call(true);
				yield break;
			}
		}

		[global::OpenPCR.Migration("553c7c99-29bd-4a61-bcd9-eec09fa69101")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			eTriggerType actionDetail = (eTriggerType)base.ActionDetail1;
			if (actionDetail != eTriggerType.STEALTH_FREE)
			{
				return;
			}

			var state = _source.ComponentState;
			state.IdleOnly = true;
			state.CurrentState = ActionState.IDLE;
			base.battleManager.CallbackIdleOnlyDone(_source);
			List<UnitCtrl> list = (_source.IsOther ? base.battleManager.EnemyList : base.battleManager.UnitList);
			if (list.Contains(_source))
			{
				list.Remove(_source);
			}

			state.IsDead = true;
			base.battleManager.CheckAcnLosingBattleFinish();
			_source.ComponentAbnormal.CureAllAbnormalState();
			foreach (SkillEffectCtrl repeatEffect in _source.RepeatEffectList)
			{
				if (!(repeatEffect is ChangeColorEffect))
				{
					repeatEffect.SetTimeToDie(value: true);
				}
			}

			base.battleManager.CallbackFadeOutDone(_source);
			base.battleManager.CallbackDead(_source);
			_source.gameObject.SetActive(value: false);
			state = _source.ComponentState;
			if (state.EnemyPoint != 0)
			{
				base.battleManager.SubstructEnemyPoint(state.EnemyPoint);
			}
		}

		[global::OpenPCR.Migration("63090309-12a5-4a4e-80c4-734fb70df459")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}
	}
}
