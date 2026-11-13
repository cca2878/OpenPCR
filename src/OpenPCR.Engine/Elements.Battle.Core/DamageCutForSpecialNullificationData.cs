using global::Elements;
using global::OpenPCR.Runtime.Api;
using System.Collections;
using System.Collections.Generic;
using global::OpenPCR.Foundation;
namespace Elements.Battle.Core
{
	public class DamageCutForSpecialNullificationData
	{
		public enum eDamageCutType { ATK = 1, MGC, ALL }
		public enum eReleaseType { TIMER, NO_FRONT }
		private eDamageCutType damageCutType = eDamageCutType.ATK;
		private double damageCutValue;
		private UnitCtrl owner;
		private eStateIconType iconType;
		private bool enable = true;
		private double timer;
		private IBattleManagerForActionParameter battleManager;
		private IBattleEffectPool battleEffectPool;
		private List<SkillEffectCtrl> skillEffectList = new List<SkillEffectCtrl>();
		private Skill skill;
		[global::OpenPCR.Migration("50b51c5c-d99d-4dc5-b31d-98bdc3c198a5")]
		public DamageCutForSpecialNullificationData(eDamageCutType _damageCutType, double _value,
			double _limitTime, UnitCtrl _owner, eStateIconType _iconType, eReleaseType _releaseType,
			IBattleManagerForActionParameter _battleManager, IBattleEffectPool _battleEffectPool,
			List<NormalSkillEffect> _actionEffectList, Skill _skill)
		{
			damageCutType = _damageCutType;
			damageCutValue = _value;
			timer = _limitTime;
			owner = _owner;
			iconType = _iconType;
			battleManager = _battleManager;
			battleEffectPool = _battleEffectPool;
			skill = _skill;
			if (iconType != eStateIconType.NONE) owner.ComponentAbnormal.OnChangeState.Call(iconType, true);
			if (_releaseType == eReleaseType.NO_FRONT)
			{
				if (judgeNoFront(_owner))
				{
					_owner.ComponentDamageControl.SetMissAtk(_owner, eMissLogType.MISS_DAMAGE_CUT_NO_FRONT,
						eDamageEffectType.NORMAL, null, 1f);
					Disable();
				}
				else
				{
					startEffect(_actionEffectList);
					_owner.AppendCoroutine(updateFrontNum(_owner), ePauseType.SYSTEM, null);
				}
			}
			else if (_releaseType == eReleaseType.TIMER)
			{
				_owner.AppendCoroutine(updateTimer(_owner), ePauseType.SYSTEM, null);
				startEffect(_actionEffectList);
			}
		}
		[global::OpenPCR.Migration("b0b2b7fd-c8f5-46a9-a00f-11cd198c0b67")]
		private void startEffect(List<NormalSkillEffect> _normalSkillEffects)
		{
			foreach (NormalSkillEffect effect in _normalSkillEffects) playActionEffect(owner, skill, effect);
		}
		[global::OpenPCR.Migration("e24c975c-20cd-49ac-ac40-9ec239dddd1b")]
		private IEnumerator updateTimer(UnitCtrl _target)
		{
			while (enable)
			{
				if (timer <= 0.0 || _target.ComponentState.IdleOnly || _target.ComponentState.IsDead)
				{
					Disable();
					timer = 0.0;
					yield break;
				}
				timer -= (double)_target.DeltaTimeForPause;
				yield return null;
			}
		}
		[global::OpenPCR.Migration("703905b0-831a-4fec-9da6-8a4a8bbc0515")]
		private IEnumerator updateFrontNum(UnitCtrl _target)
		{
			while (enable)
			{
				if (judgeNoFront(_target) || _target.ComponentState.IdleOnly || _target.ComponentState.IsDead)
				{
					Disable();
					timer = 0.0;
					yield break;
				}
				yield return null;
			}
		}
		[global::OpenPCR.Migration("f7050a22-62b7-4414-8e85-3988c0ec3fb8")]
		private bool judgeNoFront(UnitCtrl _target)
		{
			var list = _target.IsOther ? battleManager.EnemyList : battleManager.UnitList;
			foreach (UnitCtrl unit in list)
			{
				if (_target == unit || !judgeIsCountTarget(unit)) continue;
				if (_target.IsLeftDir)
				{
					if (_target.transform.localPosition.x > unit.transform.localPosition.x) return false;
				}
				else if (_target.transform.localPosition.x < unit.transform.localPosition.x) return false;
			}
			return true;
		}
		[global::OpenPCR.Migration("c422771e-8f9a-4469-90cf-4a705f2e9090")]
		private bool judgeIsCountTarget(UnitCtrl _unit)
		{
			return _unit.ComponentParameter.Hp != 0 && !_unit.ComponentAbnormal.IsStealth;
		}
		[global::OpenPCR.Migration("618cb5de-070c-48bf-8378-8a85bedb6711")]
		public double GetDamageCutValue(DamageData.eDamageType _damageType)
		{
			if (!enable) return 0.0;
			if (_damageType == DamageData.eDamageType.MGC)
			{
				if (damageCutType == eDamageCutType.ATK) return 0.0;
			}
			else if (_damageType == DamageData.eDamageType.ATK && damageCutType == eDamageCutType.MGC) return 0.0;
			return damageCutValue;
		}
		[global::OpenPCR.Migration("4a012b48-e6e7-4c11-b837-fa12933cd112")]
		public void Disable()
		{
			enable = false;
			if (iconType != eStateIconType.NONE) owner.ComponentAbnormal.OnChangeState.Call(iconType, false);
			foreach (SkillEffectCtrl effect in skillEffectList)
			{
				effect.SetTimeToDie(true);
				effect.OnEffectEnd.Call(effect);
			}
			skillEffectList.Clear();
		}
		[global::OpenPCR.Migration("788582a4-2134-42ba-8e84-dc59b3a2ac69")]
		private void playActionEffect(UnitCtrl _source, Skill _skill, NormalSkillEffect _actionEffect)
		{
			var prefab = _source.IsLeftDir ? _actionEffect.PrefabLeft : _actionEffect.Prefab;
			var effect = battleEffectPool.GetEffect(prefab, null);
			effect.IsRepeat = true;
			effect.transform.parent = ExceptUIRoot.Transform;
			effect.InitializeSort();
			effect.PlaySe(_source.ComponentSound.SoundUnitId, _source.IsLeftDir);
			effect.SetPossitionAppearanceType(_actionEffect,
				_source.ComponentMultiPartsBoss.GetFirstParts(true, 0f), _source, _skill,
				SkillEffectCtrl.eToadTrackType.NONE);
			effect.ExecAppendCoroutine(_source, false);
			skillEffectList.Add(effect);
		}
	}
}
