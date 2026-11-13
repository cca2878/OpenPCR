using System.Collections.Generic;

namespace Elements.Battle.Core
{
	public class DestroyAction : ActionParameter
	{
		[global::OpenPCR.Migration("77b25156-1fed-49cb-9ffd-090ebb529ba1")]
		public DestroyAction() : base()
		{
		}

		[global::OpenPCR.Migration("863c7cf1-675a-4094-8bb8-be16c45e46b7")]
		public override void ExecActionOnStart(Skill _skill, UnitCtrl _source, UnitActionController _sourceActionController)
		{
			base.ExecActionOnStart(_skill, _source, _sourceActionController);
		}

		[global::OpenPCR.Migration("b7579670-f226-45cb-842b-39335ab8c555")]
		public override void ExecAction(UnitCtrl _source, BasePartsData _target, int _num, UnitActionController _sourceActionController, Skill _skill, float _starttime, Dictionary<int, bool> _enabledChildAction, Dictionary<eValueNumber, double> _valueDictinary)
		{
			base.ExecAction(_source, _target, _num, _sourceActionController, _skill, _starttime, _enabledChildAction, _valueDictinary);
			AppendIsAlreadyExeced(_target.Owner, _num);
			switch (base.ActionDetail1)
			{
				case 0:
					_target.Owner.ComponentDamageControl.SetDamage(new DamageData { Damage = _target.Owner.ComponentParameter.Hp, DamageType = DamageData.eDamageType.NONE, Source = _source, ActionType = eActionType.DESTROY, IsAlwaysChargeEnegry = IsAlwaysChargeEnegry }, _byAttack: false, base.ActionId, null, _hasEffect: false, null, _energyAdd: true, base.OnDefeatEnemy, _noMotion: false, 1f, 1f, null, EnergyChargeMultiple, base.UsedChargeEnergyByReceiveDamage);
					break;
				case 1:
					{
						var state = _target.Owner.ComponentState;
						state.IdleOnly = true;
						state.CurrentState = ActionState.IDLE;
						base.battleManager.CallbackIdleOnlyDone(_target.Owner);
						List<UnitCtrl> list = (_target.Owner.IsOther ? base.battleManager.EnemyList : base.battleManager.UnitList);
						if (list.Contains(_target.Owner))
						{
							list.Remove(_target.Owner);
						}

						state.IsDead = true;
						base.battleManager.CheckAcnLosingBattleFinish();
						_target.Owner.ComponentAbnormal.CureAllAbnormalState();
						base.battleManager.CallbackFadeOutDone(_target.Owner);
						base.battleManager.CallbackDead(_target.Owner);
						_target.Owner.gameObject.SetActive(value: false);
						_target.Owner.ComponentParameter.Hp = 0L;
						{
							foreach (SkillEffectCtrl repeatEffect in _target.Owner.RepeatEffectList)
							{
								repeatEffect.SetTimeToDie(value: true);
							}

							break;
						}
					}
			}
		}
	}
}
