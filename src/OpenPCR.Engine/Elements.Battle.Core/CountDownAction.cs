using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class CountDownAction : ActionParameter
	{
		private ActionParameter actionParameter { get; set; }

		[global::OpenPCR.Migration("f9ea7c7b-ad9a-4249-8294-f8c388762a37")]
		public CountDownAction() : base()
		{
		}

		[global::OpenPCR.Migration("d95c1d7c-7e50-454d-aa89-f384139de457")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			actionParameter = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail1);
			actionParameter.IsAlwaysChargeEnegry = true;
			_skill.UseAccumulatedDamage = false;
		}

		[global::OpenPCR.Migration("57ca5ce2-9818-46c7-8879-2a044e719f20")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			actionParameter.CancelByIfForAll = true;
		}

		[global::OpenPCR.Migration("1324e909-a68a-4b63-a942-d04182aeaf15")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			for (int i = 0; i < base.ActionEffectList.Count; i++)
			{
				NormalSkillEffect normalSkillEffect = base.ActionEffectList[i];
				GameObject gameObject = null;
				gameObject = (_target.Owner.IsLeftDir ? normalSkillEffect.PrefabLeft : normalSkillEffect.Prefab);
				SkillEffectCtrl effect = base.battleEffectPool.GetEffect(gameObject);
				effect.transform.parent = ExceptUIRoot.Transform;
				effect.SortTarget = _target.Owner;
				effect.InitializeSort();
				effect.SetPossitionAppearanceType(normalSkillEffect, _target, _target.Owner, _skill);
				effect.ExecAppendCoroutine();
				if (effect is CountDownEffectCtrl)
				{
					CountDownEffectCtrl countDownEffectCtrl = effect as CountDownEffectCtrl;
					countDownEffectCtrl.SetDamageText(global::OpenPCR.NativeNumeric.ToInt32(_valueDictionary[eValueNumber.VALUE_1]));
					_target.Owner.AppendCoroutine(updateCountDown((float)_valueDictionary[eValueNumber.VALUE_1], countDownEffectCtrl, _target.Owner, _skill, _sourceActionController, _source), ePauseType.SYSTEM);
				}
			}
		}

		[global::OpenPCR.Migration("e392d85e-a545-4b3a-a223-714464f225cc")]
		private IEnumerator updateCountDown(float _duration, CountDownEffectCtrl _countDownEffect, UnitCtrl _target, Skill _skill, UnitActionController _sourceUnitActionController, UnitCtrl _source)
		{
			float timer = _duration;
			UnitComponentState state = _target.ComponentState;
			while (timer > 0f)
			{
				if (state.IsDead)
				{
					_countDownEffect.SetTimeToDie(value: true);
					yield break;
				}

				timer -= base.battleManager.DeltaTime;
				_countDownEffect.SetDamageText(OpenPCR.NativeNumeric.ToInt32(timer));
				yield return null;
			}

			_countDownEffect.SetTimeToDie(value: true);
			actionParameter.CancelByIfForAll = false;
			for (int num = actionParameter.TargetList.Count - 1; num >= 0; num--)
			{
				UnitCtrl owner = actionParameter.TargetList[num].Owner;
				if (owner.ComponentAbnormal.IsStealth || ((state.IsDead || (long)owner.ComponentParameter.Hp <= 0) && !owner.ComponentState.HasUnDeadTime))
				{
					actionParameter.TargetList.RemoveAt(num);
				}
			}

			_sourceUnitActionController.ExecUnitActionWithDelay(actionParameter, _skill, _first: false, _boneCount: false, _ignoreCancel: true);
			for (int i = 0; i < _skill.ShakeEffects.Count; i++)
			{
				if (_skill.ShakeEffects[i].TargetMotion == 2)
				{
					_sourceUnitActionController.AppendCoroutine(_sourceUnitActionController.StartShakeWithDelay(_skill.ShakeEffects[i], _skill), ePauseType.VISUAL, (_skill.BlackOutTime > 0f) ? _source : null);
				}
			}

			for (int j = 0; j < _countDownEffect.CountEndEffectData.Count; j++)
			{
				_target.AppendCoroutine(createEndEffect(_countDownEffect.CountEndEffectData[j], _target, _source), ePauseType.SYSTEM);
			}
		}

		[global::OpenPCR.Migration("868f64d3-d647-47fc-b30c-1c3e2bb7ed1d")]
		private IEnumerator createEndEffect(CountEndEffectData _endEffect, UnitCtrl _target, UnitCtrl _source)
		{
			float time = _endEffect.Time;
			while (time > 0f)
			{
				time -= battleManager.DeltaTime;
				yield return null;
			}

			GameObject prefab = _target.IsLeftDir ? _endEffect.PrefabLeft : _endEffect.Prefab;
			SkillEffectCtrl effect = battleEffectPool.GetEffect(prefab, null);
			effect.transform.parent = ExceptUIRoot.Transform;
			effect.transform.position = _target.BottomTransform.position;
			effect.SortTarget = _target;
			effect.InitializeSort();
			effect.PlaySe(_source.ComponentSound.SoundUnitId, _source.IsLeftDir);
			effect.ExecAppendCoroutine(null, false);
		}

		[global::OpenPCR.Migration("e0576678-007c-44d7-939a-8ec65b04b33c")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
		}
	}
}
