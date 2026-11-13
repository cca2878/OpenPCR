using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using global::OpenPCR.Foundation;
using Elements.Battle;

namespace Elements.Battle.Core
{
	public class EnvironmentData : ISingletonField
	{
		private static Yggdrasil<EnvironmentData> staticSingletonTree;
		private static IBattleEffectPool staticBattleEffectPool;
		private static IBattleManagerForFieldDataBase staticBattleManager;
		private int environmentId;
		public Skill Skill { get; set; }
		public UnitCtrl SourceUnit { get; set; }
		public int EnvironmentType { get; set; }
		public List<ActionParameter> TargetActions { get; set; } = new List<ActionParameter>();
		private bool stopCalled { get; set; }
		private SkillEffectCtrl skillEffect { get; set; }
		public bool Enable { get; set; }
		public float LimitTime { get; set; }
		public UnitActionController SourceActionController { get; set; }
		public List<UnitCtrl> EnemyUnitList { get; set; }
		public List<UnitCtrl> UnitList { get; set; }
		public GameObject UniqueEffectPrefab { get; set; }
		public bool WaitingAnotherEnvironmentEnd { get; set; }
		public List<BasePartsData> TargetList { get; set; }
		private float timer { get; set; }
		[global::OpenPCR.ImplicitMigration("3197258f-ed94-43d7-bad9-3462122e95e9")][global::OpenPCR.Migration("4a6b4f1a-5fd2-4eb5-87cd-d6162a796ca6")] protected IBattleEffectPool battleEffectPool => staticBattleEffectPool;
		[global::OpenPCR.ImplicitMigration("32f75014-12eb-45e4-b1c9-ef02563c72a6")][global::OpenPCR.Migration("48bdfd6d-dc41-4357-ad7d-11d34a720efe")] protected IBattleManagerForFieldDataBase battleManager => staticBattleManager;

		[global::OpenPCR.Migration("12b2e075-b4e0-4e39-ab5d-a0ab4c925962")]
		public EnvironmentData()
		{
			if (staticSingletonTree == null)
			{
				staticSingletonTree = this.CreateSingletonTree();
				staticBattleManager = staticSingletonTree.Get<BattleManager>();
				staticBattleEffectPool = staticSingletonTree.Get<BattleEffectPool>();
			}
		}

		[global::OpenPCR.Migration("772e1ddf-b890-4a15-93b9-914b301dcc61")]
		public void ResetTimer()
		{
			timer = 0f;
		}

		[global::OpenPCR.Migration("1af37046-87cc-46bd-8736-337a7550988c")]
		public static void StaticRelease()
		{
			staticSingletonTree = null;
			staticBattleManager = null;
			staticBattleEffectPool = null;
		}

		[global::OpenPCR.Migration("70dbc647-9c27-4192-b97f-9dbb0410fb11")]
		public void StartEnvironment()
		{
			battleManager.EnvironmentCounterDic.AddIfNoContains(EnvironmentType, 0);
			if (battleManager.EnvironmentCounterDic[EnvironmentType] != 0)
				WaitingAnotherEnvironmentEnd = true;
			battleManager.EnvironmentCounterDic[EnvironmentType]++;
			if (!WaitingAnotherEnvironmentEnd)
				InitializeSkillEffect();
			battleManager.AppendCoroutine(updateEnvironment(), ePauseType.SYSTEM, null);
			Enable = true;
			int id = battleManager.CurrentEnvironmentId;
			battleManager.CurrentEnvironmentId = unchecked(id + 1);
			environmentId = id;
		}

		[global::OpenPCR.Migration("0004d73e-9fda-4e3a-9f4e-6236181e57a5")]
		public void StopEnvironmentImmediately()
		{
			stopCalled = true;
			stop();
		}

		[global::OpenPCR.Migration("e7f16e99-c34b-4144-a764-e5d531418baf")]
		private void stop()
		{
			battleManager.EnvironmentCounterDic[EnvironmentType]--;
			if (battleManager.EnvironmentCounterDic[EnvironmentType] == 1)
				battleManager.SetEnvironmentResume(SourceUnit.IsPlayerUnit);
			if (Skill != null && Skill.BonusId != 0)
				battleManager.DeleteBonusIcon(Skill.BonusId);
			for (int i = TargetList.Count - 1; i >= 0; i--)
				if (i < TargetList.Count)
					OnExit(TargetList[i]);
			Enable = false;
			if (skillEffect != null)
				skillEffect.SetTimeToDie(true);
		}

		[global::OpenPCR.Migration("2ba9d092-c4d2-45a4-b279-cbe0c3698b4c")]
		private IEnumerator updateEnvironment()
		{
			while (true)
			{
				if (stopCalled)
					yield break;
				if (battleManager.GetBlackOutUnitLength() == 0 && skillEffect != null)
					skillEffect.SetSortOrderBack();
				Action<BasePartsData> action = _unit =>
				{
					if (_unit.Owner.ComponentAbnormal.IsStealth || _unit.Owner.ComponentState.IsDead)
					{
						if (TargetList.Contains(_unit))
							OnExit(_unit);
						return;
					}

					bool flag = true;
					if (_unit.Owner.IsSummonOrPhantom)
						flag = !_unit.Owner.ComponentState.IdleOnly;
					if (_unit.Owner.ComponentState.RevivalFlagForEnvironment)
					{
						_unit.Owner.ComponentState.RevivalFlagForEnvironment = false;
						foreach (ActionParameter parameter in TargetActions)
							parameter.ResetHitData();
					}
					else if (flag)
					{
						if (!TargetList.Contains(_unit))
							OnEnter(_unit);
						return;
					}

					if (TargetList.Contains(_unit))
						OnExit(_unit);
				};
				if (!WaitingAnotherEnvironmentEnd)
				{
					foreach (UnitCtrl unit in EnemyUnitList)
					{
						UnitComponentMultiPartsBoss parts = unit.ComponentMultiPartsBoss;
						if (unit.IsPartsBoss)
						{
							List<PartsData> list = parts.BossPartsListForBattle;
							for (int i = 0; i < list.Count; i++)
								action(list[i]);
						}
						else
							action(parts.GetFirstParts(false, 0f));
					}

					foreach (UnitCtrl unit in UnitList)
					{
						UnitComponentMultiPartsBoss parts = unit.ComponentMultiPartsBoss;
						if (unit.IsPartsBoss)
						{
							List<PartsData> list = parts.BossPartsListForBattle;
							for (int i = 0; i < list.Count; i++)
								action(list[i]);
						}
						else
							action(parts.GetFirstParts(false, 0f));
					}
				}

				if (battleManager.GetBlackOutUnitLength() == 0)
					timer += battleManager.DeltaTime;
				eBattleGameState gameState = battleManager.GameState;
				if (SourceUnit.ComponentState.IsDead)
					break;
				if (!((long)SourceUnit.ComponentParameter.Hp > 0L && ((int)gameState & ~1) != 2 && timer <= LimitTime))
					break;
				yield return null;
			}

			stop();
			BattleHeaderController.Instance.DisappearEnvironmentIcon(!SourceUnit.IsPlayerUnit);
		}

		[global::OpenPCR.Migration("f582ab02-6afd-4a95-b1a8-914b499418b7")]
		public virtual void OnEnter(BasePartsData _parts)
		{
			UnitCtrl owner = _parts.Owner;
			bool added = false;
			foreach (ActionParameter parameter in TargetActions)
			{
				if (parameter.TargetSort == PriorityPattern.ATK_PHYSICS)
				{
					if (_parts.Owner.AtkType != 1)
						continue;
				}
				else if (parameter.TargetSort == PriorityPattern.ATK_MAGIC && _parts.Owner.AtkType != 2)
					continue;
				if (parameter.IsChangeSpeedAction() && owner.ComponentAbnormal.OverwriteSpeedDataCount > 0)
					continue;
				if (!added)
				{
					TargetList.Add(_parts);
					added = true;
				}

				if (_parts.Owner.IsSummonOrPhantom)
					parameter.AppendTargetNum(_parts.Owner, 0);
				if (!parameter.IsAppendTargetNum(_parts.Owner))
					parameter.AppendTargetNum(_parts.Owner, 0);
				if (parameter.ActionType == eActionType.BUFF_DEBUFF)
					((BuffDebuffAction)parameter).EnvironmentId = environmentId;
				else if (parameter.ActionType == eActionType.EXTEND_SLIP_DAMAGE)
					((ExtendSlipDamageAction)parameter).EnvironmentId = environmentId;
				SourceActionController.ExecAction(parameter, Skill, _parts, 0, 0f, false, null, false);
			}
		}

		[global::OpenPCR.Migration("c3165d45-5430-4e99-888d-0fd8cda2da5f")]
		public virtual void OnExit(BasePartsData _parts)
		{
			TargetList.Remove(_parts);
			UnitComponentAbnormal abnormal = _parts.Owner.ComponentAbnormal;
			foreach (ActionParameter parameter in TargetActions)
			{
				if (parameter.TargetSort == PriorityPattern.ATK_PHYSICS)
				{
					if (_parts.Owner.AtkType != 1)
						continue;
				}
				else if (parameter.TargetSort == PriorityPattern.ATK_MAGIC && _parts.Owner.AtkType != 2)
					continue;
				abnormal.DisableAbnormalStateById(parameter.EnvironmentTargetAbnormalState, parameter.ActionId, false, -1);
			}

			abnormal.DisableEnvironmentCoroutine(environmentId);
		}

		[global::OpenPCR.Migration("e0058816-92b0-48ab-b879-165b09210b8b")]
		public void InitializeSkillEffect()
		{
			skillEffect = battleEffectPool.GetEffect(UniqueEffectPrefab, null);
			skillEffect.transform.parent = ExceptUIRoot.Transform;
			skillEffect.transform.localPosition = new Vector3(0f, (battleManager.GetRespawnPos((eUnitRespawnPos)4) + 5000f) / 540f, 0f);
			skillEffect.InitializeSort();
			skillEffect.SetSortOrderBack();
		}
	}
}
