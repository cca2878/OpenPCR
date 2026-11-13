using global::Elements;
using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using Elements.Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public partial class StrikeBackData : ISingletonField
	{
		public enum eStrikeBackType
		{
			PHYSICAL_GUARD = 1,
			MAGIC_GUARD = 2,
			PHYSICAL_DRAIN = 3,
			MAGIC_DRAIN = 4,
			BOTH_GUARD = 5,
			BOTH_DRAIN = 6
		}

		private static global::Elements.Yggdrasil<Elements.Battle.Core.StrikeBackData> staticSingletonTree;
		private static global::Elements.Battle.IBattleEffectPool staticBattleEffectPool;
		public global::Elements.Battle.Core.StrikeBackData.eStrikeBackType StrikeBackType { get; set; }
		public long Damage { get; set; }
		public global::Elements.Battle.Core.EnchantStrikeBackAction.eStrikeBackEffectType EffectType { get; set; }
		public global::Elements.Battle.Core.ActionParameter TargetAction { get; set; }
		public int ActionId { get; set; }
		public global::Elements.Battle.Core.StrikeBackDataSet DataSet { get; set; }
		public bool IsDieing { get; set; }
		public bool Execed { get; set; }
		public global::System.Collections.Generic.List<Elements.NormalSkillEffect> ActionEffectList { get; set; }
		public global::Elements.Skill Skill { get; set; }
		public global::Elements.UnitCtrl Source { get; set; }
		public bool IsEnvironmentAbnormalState { get; set; }

		[global::OpenPCR.Migration("89ce0e44-a691-4692-a4bb-95e3aae87c70")]
		protected global::Elements.Battle.IBattleEffectPool battleEffectPool
		{
			[global::OpenPCR.Migration("b2225ba3-101e-4311-9f1e-1f42be7343fd")]
			get
			{
				return staticBattleEffectPool;
			}
		}

		[global::OpenPCR.Migration("423ec2f7-377b-4535-96d9-ba5dc9d55d7d")]
		public StrikeBackData()
		{
			if (staticSingletonTree == null)
			{
				staticSingletonTree = this.CreateSingletonTree();
				staticBattleEffectPool = staticSingletonTree.Get<Elements.Battle.BattleEffectPool>();
			}
		}

		[global::OpenPCR.Migration("48c6373f-dc56-4db2-a734-d72b95f800e6")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleEffectPool = null;
		}

		[global::OpenPCR.Migration("2718fe93-085e-4772-a1e1-aa88115f7f36")]
		public void Exec(UnitCtrl _target, UnitCtrl _strickBackGivenUnit, long _recovery, Action _callback)
		{
			Execed = true;
			if (_target.ComponentState.CurrentState == ActionState.UNION_BURST)
				_target.AppendCoroutine(waitStateIdle(_target, () => execImpl(_target, _strickBackGivenUnit, _recovery, _callback)), ePauseType.SYSTEM, _strickBackGivenUnit);
			else
				execImpl(_target, _strickBackGivenUnit, _recovery, _callback);
		}

		[global::OpenPCR.Migration("0005515b-f160-4772-bbd8-83831581433f")]
		private IEnumerator waitStateIdle(UnitCtrl _target, Action _callback)
		{
			UnitComponentState state = _target.ComponentState;
			while (state.CurrentState == ActionState.UNION_BURST)
				yield return null;
			_callback.Call();
		}

		[global::OpenPCR.Migration("1e717692-ed4c-46d5-b717-473a605de165")]
		private void execImpl(UnitCtrl _target, UnitCtrl _strikeBackGivenUnit, long _recovery, Action _callback)
		{
			IsDieing = true;
			bool sourceUnavailable = false;
			bool opposing = _target.IsOther != _strikeBackGivenUnit.IsOther;
			if (opposing && !_target.ComponentAbnormal.IsStealth)
			{
				if (Damage != 0)
				{
					DamageData data = new DamageData
					{
						Target = _target.ComponentMultiPartsBoss.GetFirstParts(true),
						Damage = Damage,
						DamageType = DamageData.eDamageType.NONE,
						ActionType = eActionType.ENCHANT_STRIKE_BACK,
						TargetIncludeMyTeam = _target.IsOther == _strikeBackGivenUnit.IsOther || IsEnvironmentAbnormalState,
						SourceForDamageNumEffect = _strikeBackGivenUnit,
						StrikeBackSource = _strikeBackGivenUnit,
						CriticalRate = 0f
					};
					_target.ComponentDamageControl.SetDamage(data, false, ActionId);
				}

				if (TargetAction != null)
				{
					UnitComponentState state = Source.ComponentState;
					if (state.IsDead || state.IdleOnly)
						sourceUnavailable = true;
					else
					{
						BasePartsData parts = _target.ComponentMultiPartsBoss.GetIndexSmallestParts();
						TargetAction.ReadyAction(Source, Source.UnitActionController, Skill);
						if (parts.Owner.IsSummonOrPhantom)
							TargetAction.AppendTargetNum(parts.Owner, 0);
						if (!TargetAction.IsAppendTargetNum(parts.Owner))
							TargetAction.AppendTargetNum(parts.Owner, 0);
						Source.UnitActionController.ExecAction(TargetAction, Skill, parts, 0, 0f);
					}
				}
			}

			CircleEffectController circle = DataSet.SkillEffect as CircleEffectController;
			if (StrikeBackType == eStrikeBackType.PHYSICAL_DRAIN || StrikeBackType == eStrikeBackType.MAGIC_DRAIN || StrikeBackType == eStrikeBackType.BOTH_DRAIN)
				_strikeBackGivenUnit.ComponentDamageControl.SetRecovery(_recovery, 0, eInhibitHealType.PHYSICS, _strikeBackGivenUnit, false, null);
			circle.DisableTop(_strikeBackGivenUnit.IsLeftDir, _callback);
			if (opposing && !_target.ComponentAbnormal.IsStealth && !sourceUnavailable)
			{
				List<NormalSkillEffect> list = ActionEffectList;
				for (int i = 0; i < list.Count; i++)
				{
					GameObject prefab = _strikeBackGivenUnit.IsLeftDir ? list[i].PrefabLeft : list[i].Prefab;
					SkillEffectCtrl effect = battleEffectPool.GetEffect(prefab);
					effect.transform.parent = ExceptUIRoot.Transform;
					effect.InitializeSort();
					effect.PlaySe(_strikeBackGivenUnit.ComponentSound.SoundUnitId, _strikeBackGivenUnit.IsLeftDir);
					effect.SetPossitionAppearanceType(ActionEffectList[i], _target.ComponentMultiPartsBoss.GetFirstParts(true), _strikeBackGivenUnit, Skill);
					effect.ExecAppendCoroutine(Skill.BlackOutTime > 0f ? _strikeBackGivenUnit : null);
					list = ActionEffectList;
				}
			}
		}
	}
}
