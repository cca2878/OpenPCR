using global::Elements;
using global::OpenPCR.Runtime.Api;
using Elements.Battle;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class EnchantTriggerAction : ActionParameter
	{
		private int effectCounter { get; set; }
		private int effectMaxCount { get; set; }
		private float effectDuration { get; set; }
		private ActionParameter actionParameter { get; set; }

		[global::OpenPCR.Migration("ce5c0c9e-2743-4776-a7bf-accfedf88a75")]
		public EnchantTriggerAction() : base()
		{
		}

		[global::OpenPCR.Migration("2fae8308-acdd-42e6-a0db-cfd007b39d3a")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			if (ActionDetail2 != 0)
				actionParameter = _skill.ActionParameters.Find(e => e.ActionId == ActionDetail2);
			foreach (var parameter in _skill.ActionParameters)
				parameter.TriggerCall = false;
		}

		[global::OpenPCR.Migration("91330f50-54d8-479d-88a1-3f520883e0b7")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			if (actionParameter != null)
			{
				actionParameter.NoDelayExecByTriggerOnly = true;
				foreach (int index in actionParameter.ActionChildrenIndexes)
					_skill.ActionParameters[index].NoDelayExecByTriggerOnly = true;
			}
		}

		[global::OpenPCR.Migration("30d1d5d8-d766-40f3-992a-fec061349f3a")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			effectMaxCount = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_1]);
			int counter = 0;
			effectDuration = (float)_valueDictionary[eValueNumber.VALUE_2];
			int limit = BattleUtil.DoubleToInt(_valueDictionary[eValueNumber.VALUE_3]);
			Action cached = null;
			Action callback = () =>
			{
				if (judgeSilenceOrToad(_source))
					return;
				if (limit != 0 && counter == limit)
					return;
				counter++;
				if (cached == null)
					cached = () =>
					{
						startEffect(_source);
						_sourceActionController.ExecSkillWithTargetId(_skill, ActionDetail2);
					};
				if (battleManager.ChargeSkillTurn == eChargeSkillTurn.NONE)
					cached.Call();
				else
					_target.Owner.AppendCoroutine(waitChargeSkillTurnNone(cached, _target.Owner), ePauseType.SYSTEM, null);
			};
			if (ActionDetail1 == 2)
				_target.Owner.ComponentAbnormal.OnGiveCriticalDamage += callback;
			else if (ActionDetail1 == 1)
				_target.Owner.ComponentAbnormal.OnSetBuffForEnchantTrigger += callback;
		}

		[global::OpenPCR.Migration("9c4415ce-9601-401f-a6e9-dd72e242bb37")]
		public void startEffect(UnitCtrl _target)
		{
			if (ActionEffectList.Count == 0 || battleManager.ChargeSkillTurn != eChargeSkillTurn.NONE || effectCounter >= effectMaxCount)
				return;
			effectCounter++;
			for (int i = 0; i < ActionEffectList.Count; i++)
			{
				global::OpenPCR.Runtime.Api.GameObject prefab = _target.IsLeftDir ? ActionEffectList[i].PrefabLeft : ActionEffectList[i].Prefab;
				SkillEffectCtrl effect = battleEffectPool.GetEffect(prefab, _target);
				effect.ResetParameter(prefab);
				effect.InitializeSort();
				effect.PlaySe(_target.ComponentSound.SoundUnitId, _target.IsLeftDir);
				effect.transform.position = _target.transform.position;
				effect.transform.parent = ExceptUIRoot.Transform;
				effect.SortTarget = _target;
				effect.ExecAppendCoroutine(_target, true);
				battleManager.StartCoroutine(effect.TrackTargetSort(_target, false));
			}

			_target.AppendCoroutine(UpdateEffect(battleManager.DeltaTime), ePauseType.IGNORE_BLACK_OUT, null);
		}

		[global::OpenPCR.Migration("42df4ce1-6b77-4685-bee1-d17c597d495c")]
		public IEnumerator UpdateEffect(float _deltaTime)
		{
			float timer = effectDuration;
			while (timer > 0f)
			{
				yield return null;
				timer -= _deltaTime;
			}

			effectCounter--;
		}

		[global::OpenPCR.Migration("6fb5dc32-7014-4fe1-b7bd-b84484e851c8")]
		private IEnumerator waitChargeSkillTurnNone(Action _callback, UnitCtrl _source)
		{
			while (battleManager.ChargeSkillTurn != eChargeSkillTurn.NONE)
				yield return null;
			if (_source.ComponentParameter.Hp != 0)
				_callback.Call();
		}

		[global::OpenPCR.Migration("a5d2d232-52e0-472d-abf2-28d531e73797")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}

		[global::OpenPCR.Migration("665edc51-f920-4712-a4cf-3faccc525818")]
		private bool judgeSilenceOrToad(UnitCtrl _unitCtrl)
		{
			var abnormal = _unitCtrl.ComponentAbnormal;
			return abnormal.IsAbnormalState(eAbnormalState.SILENCE) || abnormal.ToadDatas.Count > 0;
		}
	}
}
