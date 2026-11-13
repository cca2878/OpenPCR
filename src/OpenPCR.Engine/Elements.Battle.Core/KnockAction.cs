using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class KnockAction : ActionParameter
	{
		private List<BasePartsData> unitListForSort = new List<BasePartsData>();
		public global::System.Collections.Generic.Dictionary<int, global::System.Collections.Generic.List<global::Elements.UnitCtrl>> DisableKnockBackUnitList { get; set; } = new Dictionary<int, List<UnitCtrl>>();

		[global::OpenPCR.Migration("62de1caf-8502-4145-b94f-7dda12fd1bab")]
		public KnockAction() : base()
		{
		}

		[global::OpenPCR.Migration("54f750c5-df37-4f6e-8037-39a130e7d913")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			DisableKnockBackUnitList.Clear();
		}

		[global::OpenPCR.Migration("e1e7f00b-a46b-4229-9af5-c0547dfa1723")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			if (DisableKnockBackUnitList.ContainsKey(_num))
			{
				if (DisableKnockBackUnitList[_num].Contains(_target.Owner))
					return;
				DisableKnockBackUnitList[_num].Add(_target.Owner);
			}
			else
			{
				var units = new List<UnitCtrl>();
				units.Add(_target.Owner);
				DisableKnockBackUnitList[_num] = units;
			}

			if (_source.IsOther != _target.Owner.IsOther && _target.Owner.ComponentAbnormal.OnSetCrowndControlForLoopTriggerListCall())
				return;
			if (_target.Owner.ComponentState.ModeChangeUnableStateBarrier || _target.Owner.ComponentState.TriggerStealthUnableStateBarrier || _target.Owner.ComponentAbnormal.IsUnbeatableForAutoRevival)
			{
				return;
			}

			AppendIsAlreadyExeced(_target.Owner, _num);
			switch (base.ActionDetail1)
			{
				case 1:
					_sourceActionController.AppendCoroutine(updateKnockUpDown(_target.Owner), ePauseType.IGNORE_BLACK_OUT);
					setTargetDamageNoOverrap(_target.Owner);
					break;
				case 3:
					startKnockBack(_sourceActionController, _target.Owner, _source, getKnockValue);
					setTargetDamageNoOverrap(_target.Owner);
					break;
				case 4:
				case 5:
					{
						List<UnitCtrl> list2 = (_source.IsOther ? base.battleManager.UnitList : base.battleManager.EnemyList);
						unitListForSort.Clear();
						for (int num = 0; num < list2.Count; num++)
						{
							UnitCtrl unitCtrl2 = list2[num];
							var multipart = unitCtrl2.ComponentMultiPartsBoss;
							if (unitCtrl2.IsPartsBoss)
							{
								var bossParts = multipart.BossPartsListForBattle;
								for (int num2 = 0; num2 < bossParts.Count; num2++)
								{
									unitListForSort.Add(bossParts[num2]);
								}
							}
							else
							{
								unitListForSort.Add(multipart.GetFirstParts());
							}
						}

						_source.BaseX = _source.transform.position.x;
						unitListForSort.Sort(_source.ComponentCompare.CompareDistanceAsc);
						BasePartsData basePartsData2 = (((double)unitListForSort.Count > _valueDictionary[eValueNumber.VALUE_2]) ? unitListForSort[BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_2])] : unitListForSort[unitListForSort.Count - 1]);
						Vector3 vector = new Vector3((float)((_source.transform.position.x - basePartsData2.GetPosition().x > 0f) ? 1 : (-1)) * (float)_valueDictionary[eValueNumber.VALUE_1] + basePartsData2.GetLocalPosition().x, _target.GetLocalPosition().y, _target.GetLocalPosition().z);
						if (base.ActionDetail1 == 5)
						{
							_sourceActionController.AppendCoroutine(updateParaboricKnock(vector, _target.Owner), ePauseType.SYSTEM, _source);
						}
						else
						{
							_target.Owner.transform.localPosition = vector;
						}

						break;
					}

				case 6:
					startKnockBack(_sourceActionController, _target.Owner, _source, getKnockValueLimited);
					setTargetDamageNoOverrap(_target.Owner);
					break;
				case 8:
					{
						Vector3 pos = default(Vector3);
						if (BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_2]) == -1)
						{
							BasePartsData firstParts = _source.ComponentMultiPartsBoss.GetFirstParts();
							pos = new Vector3((float)((!_source.IsLeftDir && !_source.IsForceLeftDir) ? 1 : (-1)) * (float)_valueDictionary[eValueNumber.VALUE_1] + firstParts.GetLocalPosition().x, _target.GetLocalPosition().y, _target.GetLocalPosition().z);
						}
						else
						{
							List<UnitCtrl> list = (_source.IsOther ? base.battleManager.UnitList : base.battleManager.EnemyList);
							unitListForSort.Clear();
							for (int i = 0; i < list.Count; i++)
							{
								UnitCtrl unitCtrl = list[i];
								var multipart = unitCtrl.ComponentMultiPartsBoss;
								if (unitCtrl.IsPartsBoss)
								{
									var bossParts = multipart.BossPartsListForBattle;
									for (int j = 0; j < bossParts.Count; j++)
									{
										unitListForSort.Add(bossParts[j]);
									}
								}
								else
								{
									unitListForSort.Add(multipart.GetFirstParts());
								}
							}

							_source.BaseX = _source.transform.position.x;
							unitListForSort.Sort(_source.ComponentCompare.CompareDistanceAsc);
							BasePartsData basePartsData = (((double)unitListForSort.Count > _valueDictionary[eValueNumber.VALUE_2]) ? unitListForSort[BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_2])] : unitListForSort[unitListForSort.Count - 1]);
							pos = new Vector3((float)((_source.transform.position.x - basePartsData.GetPosition().x > 0f) ? 1 : (-1)) * (float)_valueDictionary[eValueNumber.VALUE_1] + basePartsData.GetLocalPosition().x, _target.GetLocalPosition().y, _target.GetLocalPosition().z);
						}

						Func<UnitCtrl, UnitCtrl, float> func = delegate
						{
							if (_target.Owner.IsBoss)
							{
								return 0f;
							}

							float x = _target.Owner.ComponentMultiPartsBoss.GetFirstParts().GetLocalPosition().x;
							if (_source.IsLeftDir || _source.IsForceLeftDir)
							{
								if (x < pos.x)
								{
									return pos.x - x;
								}
							}
							else if (x > pos.x)
							{
								return pos.x - x;
							}

							return 0f;
						};
						startKnockBack(_sourceActionController, _target.Owner, _source, func);
						setTargetDamageNoOverrap(_target.Owner);
						break;
					}

				case 9:
					startKnockbackForGiveValue(_sourceActionController, _target.Owner, _source, getKnockValueForGiveValue, _valueDictionary);
					setTargetDamageNoOverrap(_target.Owner);
					break;
				case 2:
				case 7:
					break;
			}
		}

		[global::OpenPCR.Migration("0573a432-6d86-4e83-9561-c54f133ac965")]
		private void setTargetDamageNoOverrap(UnitCtrl _target)
		{
			if (_target.ComponentState.CurrentState != ActionState.DAMAGE)
			{
				if (base.battleManager.ChargeSkillTurn != eChargeSkillTurn.NONE && base.battleManager.BlackOutUnitList.Count != 0)
				{
					base.battleManager.BlackOutUnitList.Add(_target);
				}

				_target.ComponentState.SetState(ActionState.DAMAGE);
			}
		}

		[global::OpenPCR.Migration("ff2c500c-8aec-47a3-a33c-1767ba5fd64f")]
		private float getKnockValue(UnitCtrl _target, UnitCtrl _source)
		{
			float num = ((_target.transform.localPosition.x > _source.transform.localPosition.x) ? 1f : (-1f)) * (_target.IsBoss ? 0f : (float)base.Value[eValueNumber.VALUE_1]);
			if (num > 0f)
			{
				return Mathf.Min(num, BattleDefine.BATTLE_FIELD_SIZE - _target.transform.localPosition.x);
			}

			return Mathf.Max(num, -BattleDefine.BATTLE_FIELD_SIZE - _target.transform.localPosition.x);
		}

		[global::OpenPCR.Migration("cd5d05ef-27da-41f2-9662-18ca69f1d122")]
		private float getKnockValueForGiveValue(UnitCtrl _target, UnitCtrl _source, Dictionary<eValueNumber, double> _valueDictionary)
		{
			float num = ((_target.transform.localPosition.x > _source.transform.localPosition.x) ? 1f : (-1f)) * (_target.IsBoss ? 0f : (float)_valueDictionary[eValueNumber.VALUE_1]);
			if (num > 0f)
			{
				return Mathf.Min(num, BattleDefine.BATTLE_FIELD_SIZE - _target.transform.localPosition.x);
			}

			return Mathf.Max(num, -BattleDefine.BATTLE_FIELD_SIZE - _target.transform.localPosition.x);
		}

		[global::OpenPCR.Migration("1c963872-61ee-41cf-a671-dc5910421dc3")]
		private float getKnockValueLimited(UnitCtrl _target, UnitCtrl _source)
		{
			bool flag = _target.transform.localPosition.x > _source.transform.localPosition.x;
			List<UnitCtrl> list = new List<UnitCtrl>(_target.IsOther ? base.battleManager.EnemyList : base.battleManager.UnitList);
			UnitCtrl unitCtrl = _target;
			if (list.Count != 0)
			{
				list.Sort((UnitCtrl a, UnitCtrl unitCtrl2) => a.transform.position.x.CompareTo(unitCtrl2.transform.position.x));
				unitCtrl = ((base.Value[eValueNumber.VALUE_1] > 0.0 != flag) ? list[0] : list[list.Count - 1]);
			}

			float num = (flag ? 1f : (-1f)) * (_target.IsBoss ? 0f : (float)base.Value[eValueNumber.VALUE_1]);
			float num2 = (flag ? 1f : (-1f)) * (_target.IsBoss ? 0f : (float)base.Value[eValueNumber.VALUE_5]);
			if (num > 0f)
			{
				float b = Mathf.Max(0f, unitCtrl.transform.localPosition.x + num2 - _target.transform.localPosition.x);
				return Mathf.Min(num, Mathf.Min(BattleDefine.BATTLE_FIELD_SIZE - _target.transform.localPosition.x, b));
			}

			float b2 = Mathf.Min(0f, unitCtrl.transform.localPosition.x + num2 - _target.transform.localPosition.x);
			return Mathf.Max(num, Mathf.Max(-BattleDefine.BATTLE_FIELD_SIZE - _target.transform.localPosition.x, b2));
		}

		[global::OpenPCR.Migration("88b2b51b-a681-4223-9b25-101eebc2197a")]
		private void startKnockBack(UnitActionController _sourceActionController, UnitCtrl _target, UnitCtrl _source, Func<UnitCtrl, UnitCtrl, float> _func)
		{
			float num = _func(_target, _source);
			float duration = Mathf.Max(Mathf.Abs(num) / (float)base.Value[eValueNumber.VALUE_3], base.battleManager.DeltaTime);
			_sourceActionController.AppendCoroutine(updateKnockBackCoroutine(_target, _source, num, duration), ePauseType.IGNORE_BLACK_OUT);
		}

		[global::OpenPCR.Migration("fb113da7-7711-4942-b32a-23f141afd2f2")]
		private void startKnockbackForGiveValue(UnitActionController _sourceActionController, UnitCtrl _target, UnitCtrl _source, Func<UnitCtrl, UnitCtrl, Dictionary<eValueNumber, double>, float> _func, Dictionary<eValueNumber, double> _valueDictionary)
		{
			float num = _func(_target, _source, _valueDictionary);
			float num2 = (float)_valueDictionary[eValueNumber.VALUE_3];
			float duration = 90f;
			if (!BattleUtil.Approximately(num2, 0f))
			{
				duration = Mathf.Max(Mathf.Abs(num) / num2, base.battleManager.DeltaTime);
			}

			_sourceActionController.AppendCoroutine(updateKnockBackCoroutine(_target, _source, num, duration), ePauseType.IGNORE_BLACK_OUT);
		}

		[global::OpenPCR.Migration("662f5641-be63-4904-829b-b6b07830a6f5")]
		private IEnumerator updateKnockBackCoroutine(UnitCtrl _target, UnitCtrl _source, float _knockValue, float _duration)
		{
			var abnormal = _target.ComponentAbnormal;
			if (abnormal.IsUnbeatableForAutoRevival)
				yield break;
			float start = _target.transform.localPosition.x;
			float time = 0f;
			abnormal.KnockBackEnableCount++;
			Vector3 localPosition;
			while (true)
			{
				localPosition = _target.transform.localPosition;
				localPosition.x = start + _knockValue * base.KnockAnimationCurve.Evaluate(time / _duration);
				_target.transform.localPosition = localPosition;
				time += base.battleManager.DeltaTime;
				if (time > _duration)
				{
					break;
				}

				yield return null;
			}

			localPosition.x = start + _knockValue;
			_target.transform.localPosition = localPosition;
			abnormal.KnockBackEnableCount--;
			if (!abnormal.IsUnableActionState())
			{
				_target.GetCurrentSpineCtrl().Resume();
			}
		}

		[global::OpenPCR.Migration("24488712-f0af-4793-86f1-0747fbb527fd")]
		private IEnumerator updateKnockUpDown(UnitCtrl _target)
		{
			var abnormal = _target.ComponentAbnormal;
			if (abnormal.IsUnbeatableForAutoRevival)
				yield break;
			float start = _target.transform.localPosition.y;
			float knockValue = (_target.IsBoss ? 0f : (float)base.Value[eValueNumber.VALUE_1]);
			float time = 0f;
			float duration = (float)(base.Value[eValueNumber.VALUE_1] / base.Value[eValueNumber.VALUE_3]);
			abnormal.KnockBackEnableCount++;
			while (duration > time)
			{
				Vector3 localPosition = _target.transform.localPosition;
				time += base.battleManager.DeltaTime;
				if (time > duration)
				{
					time = duration;
				}

				localPosition.y = start + knockValue * base.KnockAnimationCurve.Evaluate(time / duration);
				_target.transform.localPosition = localPosition;
				yield return null;
			}

			duration = (float)(base.Value[eValueNumber.VALUE_1] / base.Value[eValueNumber.VALUE_4]);
			start = _target.transform.localPosition.y;
			time = 0f;
			while (duration > time)
			{
				Vector3 localPosition2 = _target.transform.localPosition;
				time += base.battleManager.DeltaTime;
				if (time > duration)
				{
					time = duration;
				}

				localPosition2.y = start - knockValue * base.KnockDownAnimationCurve.Evaluate(time / duration);
				_target.transform.localPosition = localPosition2;
				yield return null;
			}

			abnormal.KnockBackEnableCount--;
			if (!abnormal.IsUnableActionState())
			{
				_target.GetCurrentSpineCtrl().Resume();
			}
		}

		[global::OpenPCR.Migration("649d200d-1d1b-48cc-bcb1-366805448639")]
		private IEnumerator updateParaboricKnock(Vector3 _pos, UnitCtrl _target)
		{
			float halfDuration = 0.25f;
			CustomEasing easingUpY = new CustomEasing(CustomEasing.eType.outCubic, _target.transform.localPosition.y, _target.transform.localPosition.y + 300f, halfDuration);
			CustomEasing easingDownY = new CustomEasing(CustomEasing.eType.inQuad, _target.transform.localPosition.y + 300f, _target.transform.localPosition.y, halfDuration);
			CustomEasing easingX = new CustomEasing(CustomEasing.eType.linear, _target.transform.localPosition.x, _pos.x, 0.5f);
			float time = 0f;
			do
			{
				yield return null;
				time += base.battleManager.DeltaTime;
				float y = ((!(time < halfDuration)) ? easingDownY.GetCurVal(base.battleManager.DeltaTime) : easingUpY.GetCurVal(base.battleManager.DeltaTime));
				_target.transform.localPosition = new Vector3(easingX.GetCurVal(base.battleManager.DeltaTime), y);
			}
			while (!(time > 0.5f));
		}
	}
}
