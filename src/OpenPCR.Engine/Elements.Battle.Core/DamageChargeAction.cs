using global::Elements;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class DamageChargeAction : ActionParameter
	{
		private enum eDamageChargeType
		{
			PASSIVE_DAMAGE = 1,
			ACTIVE_DAMAGE,
			PASSIVE_DAMAGE_LOOP
		}
		private double defaultValue;
		private ActionParameter targetAction;
		[global::OpenPCR.Migration("35c28c6b-b238-47dd-9212-dca8450bc4a2")]
		public DamageChargeAction() : base()
		{
		}

		[global::OpenPCR.Migration("1f08003f-686d-43d6-aa4e-dfa9f7df3158")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
			switch ((eDamageChargeType)base.ActionDetail1)
			{
				case eDamageChargeType.PASSIVE_DAMAGE:
				case eDamageChargeType.PASSIVE_DAMAGE_LOOP:
					{
						UnitCtrl unitCtrl = _source;
						unitCtrl.ComponentDamageControl.OnDamage = (UnitComponentDamageControl.OnDamageDelegate)Delegate.Combine(unitCtrl.ComponentDamageControl.OnDamage, (UnitComponentDamageControl.OnDamageDelegate)delegate (bool byAttack, long damage, bool critical)
						{
							if (byAttack && _source.ComponentDamageControl.IsOnDamageCharge && base.ActionDetail2 != 0)
							{
								targetAction.Value[eValueNumber.VALUE_1] += damage * base.Value[eValueNumber.VALUE_1];
							}
						});
						if (base.ActionDetail2 != 0)
						{
							targetAction = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2);
							this.defaultValue = targetAction.Value[eValueNumber.VALUE_1];
						}

						break;
					}

				case eDamageChargeType.ACTIVE_DAMAGE:
					{
						ActionParameter actionParameter = _skill.ActionParameters.Find((ActionParameter x) => x.ActionId == base.ActionDetail3);
						actionParameter.OnDamageHit = (OnDamageHitDelegate)Delegate.Combine(actionParameter.OnDamageHit, (OnDamageHitDelegate)delegate (long damage)
						{
							for (int i = 0; i < base.ActionChildrenIndexes.Count; i++)
							{
								_skill.ActionParameters[base.ActionChildrenIndexes[i]].Value[eValueNumber.VALUE_1] += damage * base.Value[eValueNumber.VALUE_1];
							}
						});
						actionParameter.OnActionEnd = (OnActionEndDelegate)Delegate.Combine(actionParameter.OnActionEnd, (OnActionEndDelegate)delegate
						{
							for (int i = 0; i < base.ActionChildrenIndexes.Count; i++)
							{
								_sourceActionController.ExecUnitActionWithDelay(_skill.ActionParameters[base.ActionChildrenIndexes[i]], _skill, _first: false, _boneCount: false);
							}
						});
						if (base.ActionDetail2 != 0)
						{
							ActionParameter toAction = _skill.ActionParameters.Find((ActionParameter e) => e.ActionId == base.ActionDetail2);
							double defaultValue = toAction.Value[eValueNumber.VALUE_1];
							ActionParameter actionParameter2 = toAction;
							actionParameter2.OnActionEnd = (OnActionEndDelegate)Delegate.Combine(actionParameter2.OnActionEnd, (OnActionEndDelegate)delegate
							{
								toAction.Value[eValueNumber.VALUE_1] = defaultValue;
							});
						}

						break;
					}
			}
		}

		[global::OpenPCR.Migration("789aacf9-2657-4eb8-96cf-585cdc8b5777")]
		public override void ReadyAction(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			base.ReadyAction(_source, _sourceActionController, _skill);
			if (targetAction != null)
			{
				targetAction.Value[eValueNumber.VALUE_1] = defaultValue;
			}
		}

		[global::OpenPCR.Migration("90c3bf98-1030-47c8-a352-2fd75a408439")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictionary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictionary);
			int actionDetail = base.ActionDetail1;
			if (actionDetail == 1 || actionDetail == 3)
			{
				_source.ComponentDamageControl.IsOnDamageCharge = true;
				_sourceActionController.CreateNormalPrefabWithTargetMotion(_skill, 1, _first: false);
				_sourceActionController.AppendCoroutine(PlayMotionLoopForDamageCharge(_skill.AnimId, _skill, _source, _sourceActionController, _skill), ePauseType.SYSTEM, _source);
			}
		}

		[global::OpenPCR.Migration("1b501588-0c8c-44fa-ad56-7b356675ceaa")]
		private IEnumerator PlayMotionLoopForDamageCharge(eAnimKitCharacterAnimeId animeId, Skill skill, UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			float timer = 0f;
			while (true)
			{
				if (isActionCancel(_source, _sourceActionController, _skill))
				{
					yield break;
				}

				if (!_source.GetCurrentSpineCtrl().IsPlayAnimeBattle)
				{
					break;
				}

				yield return null;
			}

			_source.ComponentAnimation.PlayAnime(skill.AnimId, skill.SkillNum, 1);
			List<SkillEffectCtrl> skillEffectList = new List<SkillEffectCtrl>();
			for (int i = 0; i < base.ActionEffectList.Count; i++)
			{
				GameObject mDOJNMEMHLN = (_source.IsLeftDir ? base.ActionEffectList[i].PrefabLeft : base.ActionEffectList[i].Prefab);
				SkillEffectCtrl effect = base.battleEffectPool.GetEffect(mDOJNMEMHLN);
				effect.transform.parent = ExceptUIRoot.Transform;
				effect.InitializeSort();
				effect.PlaySe(_source.ComponentSound.SoundUnitId, _source.IsLeftDir);
				effect.SetPossitionAppearanceType(base.ActionEffectList[i], _source.ComponentMultiPartsBoss.GetFirstParts(_owner: true), _source, _skill);
				effect.ExecAppendCoroutine(_source);
				skillEffectList.Add(effect);
			}

			Action stopEffect = delegate
			{
				for (int j = 0; j < skillEffectList.Count; j++)
				{
					skillEffectList[j].SetTimeToDie(value: true);
				}

				for (int k = 0; k < _skill.LoopEffectObjs.Count; k++)
				{
					_skill.LoopEffectObjs[k].SetTimeToDie(value: true);
				}

				_skill.LoopEffectObjs.Clear();
			};
			while (true)
			{
				timer += base.battleManager.DeltaTime;
				if (isActionCancel(_source, _sourceActionController, _skill))
				{
					_source.ComponentDamageControl.IsOnDamageCharge = false;
					stopEffect.Call();
					yield break;
				}

				if ((long)_source.ComponentParameter.Hp <= 0)
				{
					_source.ComponentState.SetState(ActionState.IDLE);
					_sourceActionController.CancelAction(_skill.SkillId);
					stopEffect.Call();
					yield break;
				}

				if (timer > base.Value[eValueNumber.VALUE_3])
				{
					break;
				}

				yield return null;
			}

			stopEffect.Call();
			_source.ComponentAnimation.PlayAnime(animeId, skill.SkillNum, 2, -1, _isLoop: false);
			_sourceActionController.AppendCoroutine(waitMotinEnd(_source, _skill, _sourceActionController), ePauseType.SYSTEM, _source);
			_sourceActionController.CreateNormalPrefabWithTargetMotion(skill, 2, _first: false);
			for (int num = 0; num < skill.ShakeEffects.Count; num++)
			{
				if (skill.ShakeEffects[num].TargetMotion == 2)
				{
					_sourceActionController.AppendCoroutine(_sourceActionController.StartShakeWithDelay(skill.ShakeEffects[num], skill, _first: true), ePauseType.VISUAL, _source);
				}
			}

			_source.ComponentDamageControl.IsOnDamageCharge = false;
		}

		[global::OpenPCR.Migration("ecb586e6-8f63-479b-ac48-287e090a8562")]
		private IEnumerator waitMotinEnd(UnitCtrl _source, Skill _skill, UnitActionController _sourceActionController)
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

			if (_skill.BlackoutEndWithMotion)
			{
				base.battleManager.SetBlackoutTimeZero();
			}

			_source.ComponentState.SkillEndProcess();
		}

		[global::OpenPCR.Migration("37fbd97e-4027-4739-a730-696f2f396f94")]
		public override void SetLevel(int _level)
		{
			base.SetLevel(_level);
			Value[eValueNumber.VALUE_1] = MasterData.action_value_1 + MasterData.action_value_2 * _level;
		}

		[global::OpenPCR.Migration("65d94e92-f386-4a14-80e4-8fe49a92a5a4")]
		private bool isActionCancel(UnitCtrl _source, UnitActionController _sourceActionController, Skill _skill)
		{
			if (_source.ComponentAbnormal.IsUnableActionState() || _source.ComponentState.IsCancelActionState(_skill.SkillId == _source.UnionBurstSkillId, _skill.SkillId))
			{
				_source.ComponentState.CancelByAwake = false;
				_source.ComponentState.CancelByConvert = false;
				_source.ComponentState.CancelByToad = false;
				_sourceActionController.CancelAction(_skill.SkillId);
				return true;
			}

			return false;
		}
	}
}
