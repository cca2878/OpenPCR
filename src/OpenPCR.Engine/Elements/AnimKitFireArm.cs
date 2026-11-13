using global::Elements;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle;
using Elements.Battle.Core;

namespace Elements
{
	public class AnimKitFireArm : FirearmCtrl
	{
		private enum eSortOrder
		{
			DEFAULT,
			FOREMOST,
			FRONT,
			BACK,
			TOP_BACK
		}

		private enum eSortOrderType
		{
			FRONT_OF_FX,
			BACK_OF_FX
		}

		[SerializeField]
		private eSortOrder sortOrder;
		[SerializeField]
		private eSortOrderType sortOrderType = eSortOrderType.BACK_OF_FX;
		public List<GameObject> WalkEffectPrefab;
		public List<GameObject> WalkEffectPrefabLeft;
		[SerializeField]
		private float moveDuration;
		[SerializeField]
		private float sortChangeTime;
		[SerializeField]
		private int skillId;
		[SerializeField]
		private int index;
		[SerializeField]
		private bool isLeftDir;
		[SerializeField]
		private Vector3 targetPosition;
		private static IBattleEffectPool staticBattleEffectPool;
		private static bool isStaticInit;
		private BattleAnimKitController unitSpineController { get; set; }
		private List<SkillEffectCtrl> effectSmokeList { get; set; }
		[global::OpenPCR.ImplicitMigration("c6cfed8b-eb90-4cea-9da3-13b700c988e1")]
		protected IBattleEffectPool battleEffectPool => staticBattleEffectPool;

		[global::OpenPCR.Migration("e9045abb-4480-45d6-b7f5-99ed75fc25a3")]
		public AnimKitFireArm()
		{
		}

		[global::OpenPCR.Migration("e093f8d4-5620-40a1-ab70-0a4e68f08d77")]
		protected override bool getStopFlag()
		{
			return false;
		}

		[global::OpenPCR.Migration("ac0e1ddd-92e8-4544-a647-afbcbdf54013")]
		public new static void StaticRelease()
		{
			isStaticInit = false;
			staticBattleEffectPool = null;
		}

		[global::OpenPCR.Migration("f9a21a3a-ec7d-47f9-aa42-b2a1566e29c5")]
		protected override void Awake()
		{
			base.Awake();
			BattleAnimKitController.LoadCreateImmidiateBySkinId(eAnimKitType.SKILL_EFFECT, skillId, skillId, index, base.transform, delegate (BattleAnimKitController unit)
			{
				unitSpineController = unit;
				unitSpineController.Depth = 200;
			});
			if (!isStaticInit)
			{
				staticBattleEffectPool = base.singletonTree.Get<BattleEffectPool>();
				isStaticInit = true;
			}
		}

		[global::OpenPCR.Migration("93cce222-dec1-4536-8686-b24da3797ec0")]
		public override void Initialize(BasePartsData _target, List<ActionParameter> _actions, Skill _skill, List<SkillEffectBase> _skillEffect, UnitCtrl _owner, float _height, bool _hasBlackOutTime, bool _isAbsolute, Vector3 _targetPosition, List<ShakeEffect> _shakes, eTargetBone _targetBone)
		{
			base.FireTarget = _target;
			base.owner = _owner;
			base.Skill = _skill;
			unitSpineController.Owner = _owner;
			unitSpineController.PlayAnimeNoOverlap(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT, skillId, _playLoop: false);
			int num = ((sortOrderType == eSortOrderType.FRONT_OF_FX) ? 1 : (-1));
			switch (sortOrder)
			{
				case eSortOrder.TOP_BACK:
					unitSpineController.Depth = 690 + num;
					break;
				case eSortOrder.FOREMOST:
					unitSpineController.Depth = 11000 + num;
					break;
				case eSortOrder.BACK:
					unitSpineController.Depth = BattleDefine.GetUnitSortOrder(base.owner) + -300 + num;
					break;
				case eSortOrder.FRONT:
					unitSpineController.Depth = BattleDefine.GetUnitSortOrder(base.owner) + 300 + num;
					break;
			}

			if (base.owner.ComponentSortOrder.IsFront)
			{
				unitSpineController.Depth += 11500;
			}

			base.transform.position = base.owner.BottomTransform.transform.position;
			base.transform.localScale = new Vector3((float)((!isLeftDir) ? 1 : (-1)) * Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y, base.transform.localScale.z);
			base.battleManager.AppendCoroutine(updateAppear(), ePauseType.SYSTEM, (base.Skill.BlackOutTime > 0f) ? base.owner : null);
		}

		[global::OpenPCR.Migration("c588ea96-d740-45b2-9035-f8ab78beb592")]
		private IEnumerator updateAppear()
		{
			while (unitSpineController.IsPlayAnimeBattle)
			{
				yield return null;
			}

			startMove();
		}

		[global::OpenPCR.Migration("316f09b4-6d78-43ef-aa56-269e07a232a7")]
		private void startMove()
		{
			unitSpineController.PlayAnimeNoOverlap(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_WALK, skillId);
			List<GameObject> list = (isLeftDir ? WalkEffectPrefabLeft : WalkEffectPrefab);
			effectSmokeList = new List<SkillEffectCtrl>();
			for (int i = 0; i < list.Count; i++)
			{
				SkillEffectCtrl effect = battleEffectPool.GetEffect(list[i]);
				effect.SortTarget = base.owner;
				effect.InitializeSort();
				effect.transform.position = base.transform.position;
				effect.transform.parent = ExceptUIRoot.Transform;
				// Smoke tracking feeds only particle rendering; retain the actual effect lifecycle.
				effect.GetComponent<SkillEffectCtrl>().PlaySe(base.owner.ComponentSound.SoundUnitId, base.owner.IsLeftDir);
				effect.ExecAppendCoroutine(base.owner);
				effectSmokeList.Add(effect);
			}

			base.battleManager.AppendCoroutine(updatePosition(), ePauseType.SYSTEM, (base.Skill.BlackOutTime > 0f) ? base.owner : null);
		}

		[global::OpenPCR.Migration("ade62963-a5e8-420c-94f7-5674c502e603")]
		private IEnumerator updatePosition()
		{
			float time = moveDuration;
			bool sortChanged = false;
			while (true)
			{
				base.transform.position = Vector3.Lerp(base.FireTarget.GetBottomTransformPosition() + targetPosition, base.owner.BottomTransform.position, time / moveDuration);
				time -= base.battleManager.DeltaTime;
				if (moveDuration - time > sortChangeTime && !sortChanged)
				{
					sortChanged = true;
					unitSpineController.Depth = base.FireTarget.Owner.ComponentSortOrder.SortOrder + 200;
					for (int i = 0; i < effectSmokeList.Count; i++)
					{
						SkillEffectCtrl skillEffectCtrl = effectSmokeList[i];
						skillEffectCtrl.SortTarget = base.FireTarget.Owner;
						if (base.FireTarget.Owner.ComponentSortOrder.IsFront)
						{
							skillEffectCtrl.SetSortOrderFront();
						}
						else
						{
							skillEffectCtrl.SetSortOrderBack();
						}
					}
				}

				if (time < 0f || BattleUtil.Approximately(time, 0f))
				{
					break;
				}

				yield return null;
			}

			startAttack();
		}

		[global::OpenPCR.Migration("f42d4146-b50a-487a-9643-8b714a40c09f")]
		private void startAttack()
		{
			base.OnHitAction = null;
			effectSmokeSetTimeToDie();
			base.speed = Vector3.zero;
			unitSpineController.PlayAnimeNoOverlap(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_ATTACK, skillId, _playLoop: false);
			if (base.battleManager.CanPlaySe())
			{
				// Attack audio is a presentation output; the original gate and following lifecycle remain.
			}

			AppendCoroutine(updateAttack(), ePauseType.SYSTEM, base.owner);
		}

		[global::OpenPCR.Migration("2a960742-f5d8-461f-84e7-f56201bd5087")]
		private void effectSmokeSetTimeToDie()
		{
			for (int i = 0; i < effectSmokeList.Count; i++)
			{
				if (effectSmokeList[i].IsRepeat)
				{
					effectSmokeList[i].SetTimeToDie(value: true);
				}
			}
		}

		[global::OpenPCR.Migration("502c1968-4209-4b1b-9b24-4c472a6377ba")]
		private IEnumerator updateAttack()
		{
			while (unitSpineController.IsPlayAnimeBattle)
			{
				yield return null;
			}

			SetTimeToDie(value: true);
		}

		[global::OpenPCR.Migration("f5eb2189-ba25-49a1-a866-9368779c15d1")]
		public override bool _Update()
		{
			if (isLeftDir ? (base.transform.position.x > 4.6296296f) : (base.transform.position.x < -4.6296296f))
			{
				timeToDie = true;
				effectSmokeSetTimeToDie();
			}

			bool result = base._Update();
			if (base.FireTarget.Owner.ComponentState.IsDead)
			{
				effectSmokeSetTimeToDie();
			}

			return result;
		}

		[global::OpenPCR.Migration("990a5f0b-e72c-4ac4-bba2-1feb8992a6b9")]
		public override void Pause()
		{
			unitSpineController.Pause();
		}

		[global::OpenPCR.Migration("aa8178c1-d9d6-4ba4-9c0d-0df19c6dc89a")]
		public override void Resume()
		{
			unitSpineController.Resume();
		}

		[global::OpenPCR.Migration("44c1cd14-2e83-44c9-a600-11d9b0f024f0")]
		public override void SetSortOrderBack()
		{
			base.SetSortOrderBack();
			if (unitSpineController != null)
			{
				setSortOrderBackImpl();
			}
		}

		[global::OpenPCR.Migration("59d90a48-f3e0-4654-a89b-aa9e51f7f002")]
		private void setSortOrderBackImpl()
		{
			int num = ((sortOrderType == eSortOrderType.FRONT_OF_FX) ? 1 : (-1));
			switch (sortOrder)
			{
				case eSortOrder.TOP_BACK:
					unitSpineController.Depth = 690 + num;
					break;
				case eSortOrder.FOREMOST:
					unitSpineController.Depth = 11000 + num;
					break;
				case eSortOrder.BACK:
					unitSpineController.Depth = BattleDefine.GetUnitSortOrder(base.SortTarget) + -300 + num;
					break;
				case eSortOrder.FRONT:
					unitSpineController.Depth = BattleDefine.GetUnitSortOrder(base.SortTarget) + 300 + num;
					break;
			}
		}
	}
}
