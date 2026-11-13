using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class FieldDataBase : ISingletonField
	{
		public global::Elements.eFieldType FildType { get; set; }
		public global::Elements.eFieldExecType ExecType { get; set; }
		public float LimitTime { get; set; }
		public float CenterX { get; set; }
		public float Size { get; set; }
		public int TargetTalentId { get; set; }

		private static IBattleManagerForFieldDataBase staticBattleManager;
		protected SkillEffectCtrl skillEffect;
		private static Yggdrasil<FieldDataBase> staticSingletonTree;
		private static IBattleEffectPool staticBattleEffectPool;
		public bool Enable { get; private set; }
		public eFieldTargetType FieldTargetType { get; set; }
		public List<BasePartsData> TargetList { get; set; }
		public List<UnitCtrl> AllTargetList { get; set; }
		[global::OpenPCR.ImplicitMigration("aade98c8-aef6-49c0-9a37-cb2a1f409896")][global::OpenPCR.Migration("72364d79-5a67-4b23-b4b0-e6082638b600")] protected IBattleManagerForFieldDataBase battleManager => staticBattleManager;
		public UnitCtrl PauseTarget { get; set; }
		public Skill Skill { get; set; }
		public UnitCtrl Source { get; set; }
		protected int fieldIndex { get; private set; }
		[global::OpenPCR.ImplicitMigration("f3011440-33e0-48ab-9080-3ce70f65e0db")][global::OpenPCR.Migration("b5476143-fbb3-4a2b-a163-ba7f1d6810d1")] protected IBattleEffectPool battleEffectPool => staticBattleEffectPool;
		public GameObject UniqueEffectPrefab { get; set; }
		public GameObject UniqueEffectPrefabLeft { get; set; }
		private bool stopFlag { get; set; }

		[global::OpenPCR.Migration("c464dca6-7058-4677-be34-47d1f33de154")]
		public FieldDataBase()
		{
			if (staticSingletonTree == null)
			{
				staticSingletonTree = this.CreateSingletonTree();
				staticBattleManager = staticSingletonTree.Get<Elements.Battle.BattleManager>();
				staticBattleEffectPool = staticSingletonTree.Get<Elements.Battle.BattleEffectPool>();
			}
		}

		[global::OpenPCR.Migration("edb129d8-2fcb-4ab5-8c0e-34bb924e86b5")]
		public static void StaticRelease()
		{
			staticBattleManager = null;
			staticSingletonTree = null;
			staticBattleEffectPool = null;
		}

		[global::OpenPCR.Migration("670ccca4-b921-4c26-93f0-c29bed8d5511")]
		public virtual void OnEnter(BasePartsData unit)
		{
			TargetList.Add(unit);
		}

		[global::OpenPCR.Migration("6e104925-49a9-4848-adea-cba302965990")]
		public virtual void OnExit(BasePartsData unit)
		{
			TargetList.Remove(unit);
		}

		[global::OpenPCR.Migration("83ded530-ed76-42f0-8acb-d6f557a7cd72")]
		public virtual void OnRepeat()
		{
		}

		[global::OpenPCR.Migration("d8f37e08-b8ee-4c22-b75d-9c238b1a04f2")]
		public virtual void StartField()
		{
			if (skillEffect != null)
			{
				battleManager.AppendEffect(skillEffect, PauseTarget, _isAbnormal: false);
			}

			battleManager.AppendCoroutine(Update(), ePauseType.SYSTEM, PauseTarget);
			fieldIndex = ++battleManager.FieldIndex;
		}

		[global::OpenPCR.Migration("a5f7c873-b3c2-4093-81de-342512a149cd")]
		protected void initializeSkillEffect()
		{
			skillEffect.transform.parent = ExceptUIRoot.Transform;
			FieldEffectController fieldEffectController = skillEffect as FieldEffectController;
			skillEffect.transform.localPosition = new Vector3(fieldEffectController.DisplayCenter ? 0f : (CenterX / 540f), (5000f + battleManager.GetRespawnPos(eUnitRespawnPos.MAIN_POS_3)) / 540f, 0f);
			skillEffect.transform.localScale = Vector3.one * Size / 350f;
			skillEffect.InitializeSort();
			if (battleManager.GetBlackOutUnitLength() > 0)
			{
				skillEffect.SetSortOrderFront();
			}
			else
			{
				skillEffect.SetSortOrderBack();
			}
		}

		[global::OpenPCR.Migration("d3b7fdfb-a12b-4369-8d6d-3681c54f2e62")]
		public virtual void ResetTarget(UnitCtrl _unit, eAbnormalState _abnormalState, bool _strict = false)
		{
		}

		[global::OpenPCR.Migration("a1d4e327-ad98-452c-906f-d67127743332")]
		protected virtual int getClearedIndex(UnitCtrl _unit)
		{
			return 0;
		}

		[global::OpenPCR.Migration("d05fe3d2-7ae2-4da7-b1d1-1ba824574827")]
		public IEnumerator Update()
		{
			float time = 0f;
			float intervalCount = 0f;
			Enable = true;
			while (true)
			{
				if (battleManager.GetBlackOutUnitLength() == 0 && skillEffect != null)
				{
					skillEffect.SetSortOrderBack();
				}

				updateTarget();
				if (ExecType == eFieldExecType.REPEAT)
				{
					if (battleManager.GetBlackOutUnitLength() == 0)
					{
						intervalCount += battleManager.DeltaTime;
					}

					if (intervalCount > 1f)
					{
						intervalCount = 0f;
						OnRepeat();
					}
				}

				if (battleManager.GetBlackOutUnitLength() == 0)
				{
					time += battleManager.DeltaTime;
				}

				eBattleGameState gameState = battleManager.GameState;
				if (time > LimitTime || gameState == eBattleGameState.NEXT_WAVE_PROCESS || gameState == eBattleGameState.WAIT_WAVE_END || stopFlag)
				{
					break;
				}

				yield return null;
			}

			if (Skill != null && Skill.BonusId != 0)
			{
				battleManager.DeleteBonusIcon(Skill.BonusId);
			}

			Enable = false;
			for (int num2 = TargetList.Count - 1; num2 >= 0; num2--)
			{
				if (num2 < TargetList.Count)
				{
					OnExit(TargetList[num2]);
				}
			}

			if (!(skillEffect != null))
			{
				yield break;
			}

			FieldEffectController fieldEffect = skillEffect as FieldEffectController;
			fieldEffect.StopEmitter();
			if (fieldEffect.EndDuration == 0f)
			{
				skillEffect.SetTimeToDie(value: true);
			}
			else
			{
				float endDuration = fieldEffect.EndDuration;
				while (endDuration > 0f)
				{
					if (battleManager.GetBlackOutUnitLength() == 0)
					{
						endDuration -= battleManager.DeltaTime;
					}

					yield return null;
				}

				skillEffect.SetTimeToDie(value: true);
			}

			battleManager.AppendCoroutine(createPrefabWithTime(fieldEffect.EndEffectPrefab, skillEffect.transform.position), ePauseType.SYSTEM, PauseTarget);
		}

		[global::OpenPCR.Migration("2acb1113-cd51-406a-8c14-ff1b3f57311e")]
		protected void updateTarget()
		{
			List<UnitCtrl> all = AllTargetList;
			for (int i = 0; i < all.Count; i++)
			{
				Action<BasePartsData> action = parts =>
				{
					if (parts.Owner.ComponentAbnormal.IsStealth || getClearedIndex(parts.Owner) >= fieldIndex || parts.Owner.ComponentState.IsDead)
					{
						if (TargetList.Contains(parts))
							OnExit(parts);
						return;
					}

					bool inside = (parts.GetLocalPosition().x <= CenterX + Size && parts.GetLocalPosition().x >= CenterX - Size) || BattleUtil.Approximately(parts.GetLocalPosition().x, CenterX + Size) || BattleUtil.Approximately(parts.GetLocalPosition().x, CenterX - Size);
					if (parts.Owner.IsSummonOrPhantom && parts.Owner.ComponentState.IdleOnly)
						inside = false;
					if (!parts.GetTargetable())
						inside = false;
					if (TargetTalentId != 0)
						inside &= parts.Owner.TalentIds.Contains(TargetTalentId);
					bool working = judgeWorking(parts.Owner);
					if (!working || !inside)
					{
						if (TargetList.Contains(parts))
							OnExit(parts);
					}
					else if (!TargetList.Contains(parts))
						OnEnter(parts);
				};
				UnitCtrl unit = AllTargetList[i];
				UnitComponentMultiPartsBoss component = unit.ComponentMultiPartsBoss;
				if (unit.IsPartsBoss)
				{
					List<PartsData> list = component.BossPartsListForBattle;
					for (int p = 0; p < list.Count; p++)
						action(list[p]);
				}
				else
					action(component.GetFirstParts());
				all = AllTargetList;
			}
		}

		[global::OpenPCR.Migration("fcb20824-77f2-4c45-8f75-19955bf65029")]
		protected virtual bool judgeWorking(UnitCtrl _unit)
		{
			return true;
		}

		[global::OpenPCR.Migration("024739d4-618d-45cf-adad-323d68827fd5")]
		public void StopField(eTargetAssignment _targetAssignment, bool _isOther)
		{
			if (judgeStopTarget(_targetAssignment, _isOther))
			{
				stopFlag = true;
			}
		}

		[global::OpenPCR.Migration("6e3bfcc1-e62c-443a-b735-357759ff8f1a")]
		private bool judgeStopTarget(eTargetAssignment _targetAssignment, bool _isOther)
		{
			return _targetAssignment switch
			{
				eTargetAssignment.ALL => true,
				eTargetAssignment.OTHER_SIDE => FieldTargetType == eFieldTargetType.PLAYER == _isOther,
				eTargetAssignment.OWNER_SITE => FieldTargetType == eFieldTargetType.PLAYER == !_isOther,
				_ => false,
			};
		}

		[global::OpenPCR.Migration("bf5bb592-b568-43bf-af50-2b2ee53b5640")]
		private IEnumerator createPrefabWithTime(List<PrefabWithTime> _prefabWithTimeList, Vector3 _position)
		{
			float time = 0f;
			bool[] isEffectCreated = new bool[_prefabWithTimeList.Count];
			int numCreatedEffect = 0;
			while (true)
			{
				for (int i = 0; i < _prefabWithTimeList.Count; i++)
				{
					if (!isEffectCreated[i] && !(time <= _prefabWithTimeList[i].Time))
					{
						battleEffectPool.GetEffect(_prefabWithTimeList[i].Prefab);
						SkillEffectCtrl effect = battleEffectPool.GetEffect(Singleton<BattleResourceManager>.Instance.HealFieldEffectPrefab);
						effect.transform.parent = ExceptUIRoot.Transform;
						effect.transform.position = _position;
						effect.InitializeSort();
						effect.SetSortOrderBack();
						isEffectCreated[i] = true;
						numCreatedEffect++;
						if (numCreatedEffect >= _prefabWithTimeList.Count)
						{
							yield break;
						}
					}
				}

				yield return null;
				time += Time.deltaTime;
			}
		}

		public bool IsTargetAbnormalStateBuff { get; set; }
	}
}
