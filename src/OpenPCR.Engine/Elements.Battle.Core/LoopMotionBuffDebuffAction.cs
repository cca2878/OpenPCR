using Elements.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class LoopMotionBuffDebuffAction : ActionParameter
	{
		private class BuffDebuffData
		{
			public UnitCtrl Target { get; set; }
			public Dictionary<BasePartsData, long> Value { get; set; }
			public BuffParamKind BuffParamKind { get; set; }

			[global::OpenPCR.Migration("0ca459ce-7adc-4f54-8be2-91735567abe1")]
			public BuffDebuffData() : base()
			{
			}
		}

		private bool triggered { get; set; }
		private int hitCount { get; set; }

		private List<LoopMotionBuffDebuffAction> childActions = new List<LoopMotionBuffDebuffAction>();
		private List<BuffDebuffData> targetAndValueList = new List<BuffDebuffData>();
		[global::OpenPCR.Migration("02a547bd-5b04-46fa-912b-6efdf5e55fe2")]
		public LoopMotionBuffDebuffAction() : base()
		{
		}

		private enum eReleaseType
		{
			DAMAGED = 1
		}

		[global::OpenPCR.Migration("6603c084-33a1-49ad-887f-2f083f58705c")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			if (base.ActionDetail2 > 10)
			{
				ActionParameter actionParameter = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2 && e is LoopMotionBuffDebuffAction);
				if (actionParameter != null)
				{
					(actionParameter as LoopMotionBuffDebuffAction).childActions.Add(this);
				}
			}
		}

		[global::OpenPCR.Migration("0091dddf-5cdc-4480-99fd-a360db9c59f4")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			hitCount = 0;
			targetAndValueList.Clear();
			if (base.ActionDetail2 <= 10)
			{
				triggered = false;
				_source.AppendCoroutine(updateStartMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, _source);
			}
		}

		[global::OpenPCR.Migration("6eb6f4ed-862d-49f4-a2cb-8c8dc2997390")]
		private IEnumerator updateStartMotion(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill))
				{
					ReleaseBuffDebuff(_source);
					for (int i = 0; i < childActions.Count; i++)
					{
						childActions[i].ReleaseBuffDebuff(_source);
					}

					yield break;
				}

				if (!_source.UnitSpineCtrl.IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			_sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, 1, _first: false);
			_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, 1, -1, _isLoop: true);
			_sourceActionController.AppendCoroutine(updateLoopMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, _source);
		}

		[global::OpenPCR.Migration("24dd3d07-42af-4c91-aa93-c9893e0debb3")]
		private IEnumerator updateLoopMotion(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			float timer = (float)base.Value[eValueNumber.VALUE_4];
			bool enableTimer = timer > 0f;
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill))
				{
					ReleaseBuffDebuff(_source);
					for (int i = 0; i < childActions.Count; i++)
					{
						childActions[i].ReleaseBuffDebuff(_source);
					}

					for (int num = _skill.LoopEffectObjs.Count - 1; num >= 0; num--)
					{
						SkillEffectCtrl skillEffectCtrl = _skill.LoopEffectObjs[num];
						if (!(skillEffectCtrl == null) && skillEffectCtrl.GetComponent<SkillEffectCtrl>().SortTarget == _source)
						{
							skillEffectCtrl.SetTimeToDie(value: true);
							_skill.LoopEffectObjs.RemoveAt(num);
						}
					}

					yield break;
				}

				if (enableTimer)
				{
					timer -= _source.DeltaTimeForPause;
					if (timer < 0f || base.battleManager.GameState != eBattleGameState.PLAY)
					{
						enableTimer = false;
						startMotionEnd(_byTimer: true, _source, _skill, _sourceActionController);
						yield break;
					}
				}

				if (triggered)
				{
					break;
				}

				yield return null;
			}

			startMotionEnd(_byTimer: false, _source, _skill, _sourceActionController);
		}

		[global::OpenPCR.Migration("f93bfed7-8a4f-41f7-aca5-8e712bc2d727")]
		private void startMotionEnd(bool _byTimer, UnitCtrl _source, Skill _skill, UnitActionController _sourceActionController)
		{
			_skill.LoopEffectAlreadyDone = true;
			for (int num = _skill.LoopEffectObjs.Count - 1; num >= 0; num--)
			{
				if (!(_skill.LoopEffectObjs[num] == null) && _skill.LoopEffectObjs[num].GetComponent<SkillEffectCtrl>().SortTarget == _source)
				{
					_skill.LoopEffectObjs[num].SetTimeToDie(value: true);
					_skill.LoopEffectObjs.RemoveAt(num);
				}
			}

			ReleaseBuffDebuff(_source);
			for (int i = 0; i < childActions.Count; i++)
			{
				childActions[i].ReleaseBuffDebuff(_source);
			}

			int num2 = (_byTimer ? 3 : 2);
			_sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, num2, _first: false);
			_source.ComponentAnimation.PlayAnimeNoOverlap(_skill.AnimId, _skill.SkillNum, num2);
			_sourceActionController.AppendCoroutine(updateEndMotion(_source, _sourceActionController, _skill), ePauseType.SYSTEM, (_skill.BlackOutTime > 0f) ? _source : null);
		}

		[global::OpenPCR.Migration("68fbd331-9bf8-4cdb-85b4-b8bb3d5c6a84")]
		private IEnumerator updateEndMotion(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill))
				{
					yield break;
				}

				if (!_source.UnitSpineCtrl.IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			_source.ComponentState.SetState(ActionState.IDLE);
		}

		[global::OpenPCR.Migration("3fc2add5-bd69-405f-b19c-6db30b7a580a")]
		public void ReleaseBuffDebuff(UnitCtrl _source)
		{
			triggered = true;
			for (int i = 0; i < targetAndValueList.Count; i++)
			{
				BuffDebuffData buffDebuffData = targetAndValueList[i];
				buffDebuffData.Target.ComponentAbnormal.EnableBuffParam(buffDebuffData.BuffParamKind, buffDebuffData.Value, _enable: false, _source, base.ActionDetail1 % 10 != 1, _additional: false, _showsIcon: true, _triggerCall: true);
			}
		}

		[global::OpenPCR.Migration("aba4deb1-b09e-4005-805f-9d3c120b979e")]
		private bool isActionCancel(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			if (_source.ComponentAbnormal.IsUnableActionState() || _source.ComponentState.IsCancelActionState(_skill.SkillId == _source.UnionBurstSkillId))
			{
				_source.ComponentState.CancelByAwake = false;
				_source.ComponentState.CancelByConvert = false;
				_source.ComponentState.CancelByToad = false;
				_sourceActionController.CancelAction(_skill.SkillId);
				return true;
			}

			return false;
		}

		[global::OpenPCR.Migration("c9bdd320-0d99-46d2-aa73-640492a5dac5")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			if (triggered)
			{
				return;
			}

			BuffParamKind changeParamKind = BuffDebuffAction.GetChangeParamKind(base.ActionDetail1);
			Dictionary<BasePartsData, long> dictionary = new Dictionary<BasePartsData, long>();
			bool flag = base.ActionDetail1 % 10 == 1;
			var parts = _target.Owner.ComponentMultiPartsBoss;
			if (_target.Owner.IsPartsBoss)
			{
				var bossParts = parts.BossPartsListForBattle;
				for (int i = 0; i < bossParts.Count; i++)
				{
					dictionary.Add(bossParts[i], BuffDebuffAction.CalculateBuffDebuffParam(bossParts[i], _valueDictionary[eValueNumber.VALUE_2], (BuffDebuffAction.eChangeParameterType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]), changeParamKind, flag));
				}
			}
			else
			{
				dictionary.Add(parts.DummyPartsData, BuffDebuffAction.CalculateBuffDebuffParam(_target, _valueDictionary[eValueNumber.VALUE_2], (BuffDebuffAction.eChangeParameterType)BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]), changeParamKind, flag));
			}

			var abnormal = _target.Owner.ComponentAbnormal;
			abnormal.SetBuffParam(BuffParamKind.NUM, changeParamKind, dictionary, 0f, 0, null, _despelable: true, eEffectType.COMMON, !flag, _additional: false, _isShowIcon: false, _bonusId: 0, _environmentId: -1, _triggerCall: true);
			abnormal.EnableBuffParam(changeParamKind, dictionary, _enable: true, _source, !flag, _additional: false, _showsIcon: true, _triggerCall: TriggerCall);
			eReleaseType actionDetail = (eReleaseType)base.ActionDetail2;
			if (actionDetail == eReleaseType.DAMAGED)
			{
				_source.ComponentDamageControl.OnDamageForLoopTrigger = delegate (bool _byAttack, long _damage, bool _critical)
				{
					if (_byAttack)
					{
						hitCount++;
						if (hitCount == base.ActionDetail3)
						{
							triggered = true;
						}
					}
				};
			}

			targetAndValueList.Add(new BuffDebuffData { Target = _target.Owner, BuffParamKind = changeParamKind, Value = dictionary });
		}

		[global::OpenPCR.Migration("4c63f017-975a-48c1-bbee-a50c96c4279d")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_2] = MasterData.action_value_2 + MasterData.action_value_3 * _level;
			Value[eValueNumber.VALUE_4] = MasterData.action_value_4 + MasterData.action_value_5 * _level;
		}

		[global::OpenPCR.Migration("b28ff15f-a3de-419c-98e9-8ce5ae63e69e")]
		public override double GetActionBaseValue(UnitCtrl _source, eValueNumber _type)
		{
			double value = 0.0;
			Value.TryGetValue(_type, out value);
			if (_type != eValueNumber.VALUE_2)
				return value;
			BuffParamKind kind = BuffDebuffAction.GetChangeParamKind(ActionDetail1);
			if (!BuffDebuffAction.BUFF_DEBUFF_ENHANCE_TARGET_TYPE.Contains(kind) || IsExEquipSkillAction)
				return value;
			long enhancement = ActionDetail1 % 10 == 1 ? (long)_source.ComponentParameterClamped.DebuffEnhanceZero : (long)_source.ComponentParameterClamped.BuffEnhanceZero;
			return value * ((double)enhancement / 10000.0 + 1.0);
		}
	}
}
