using global::Elements;
using global::Elements.Battle;
using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;

namespace Elements
{
	public class AnimKitCowFireArm : FirearmCtrl
	{
		public GameObject WalkEffectPrefab;
		public GameObject ReturnWalkEffectPrefab;
		public GameObject AttackEffectPrefab;
		[SerializeField]
		private int skillId;
		[SerializeField]
		private int index;
		[SerializeField]
		private ShakeEffect shake;
		[SerializeField]
		private bool isLeftDir;
		private static IBattleEffectPool staticBattleEffectPool;
		private static IBattleCameraEffectForAnimKitCowFireArm staticBattleCameraEffect;
		private static bool isStaticInit;
		private BattleAnimKitController unitSpineController { get; set; }
		private SkillEffectCtrl effectSmoke { get; set; }
		private bool turn { get; set; }
		private bool ownerIsLeftDir { get; set; }
		[global::OpenPCR.ImplicitMigration("46d790fb-f1f7-4869-919c-a47e4189ea13")]
		protected IBattleEffectPool battleEffectPool => staticBattleEffectPool;
		[global::OpenPCR.ImplicitMigration("27984b22-bc49-46eb-b8ad-ac6ba438dc68")]
		protected IBattleCameraEffectForAnimKitCowFireArm battleCameraEffect => staticBattleCameraEffect;

		[global::OpenPCR.Migration("6fd123e6-55e7-4869-82e6-91abb6cd1e7a")]
		public AnimKitCowFireArm() : base()
		{
		}

		[global::OpenPCR.Migration("194356ea-8dea-41a2-aee2-a391f1b721b2")]
		protected override bool getStopFlag()
		{
			return false;
		}

		[global::OpenPCR.Migration("c17d4d20-3f31-49e8-9859-d04be6112f78")]
		public new static void StaticRelease()
		{
			isStaticInit = false;
			staticBattleCameraEffect = null;
			staticBattleEffectPool = null;
		}

		[global::OpenPCR.Migration("4024626c-79b6-4e2b-9ea0-08134ca98c76")]
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
				staticBattleCameraEffect = base.singletonTree.Get<BattleCameraEffect>();
				staticBattleEffectPool = base.singletonTree.Get<BattleEffectPool>();
				isStaticInit = true;
			}
		}

		[global::OpenPCR.Migration("f7f21ebf-f6cf-4c0e-a7ec-679299cb1da7")]
		protected override void setInitialPosition()
		{
			base.initialPosistion = new Vector3((float)((!isLeftDir) ? 1 : (-1)) * -2500f / 540f, base.FireTarget.GetPosition().y);
		}

		[global::OpenPCR.Migration("99bf84b1-1c64-4757-b6b0-07b655b7d3d3")]
		public override void Initialize(BasePartsData _target, List<ActionParameter> _actions, Skill _skill, List<SkillEffectBase> _skillEffect, UnitCtrl _owner, float _height, bool _hasBlackOutTime, bool _isAbsolute, Vector3 _targetPosition, List<ShakeEffect> _shakes, eTargetBone _targetBone)
		{
			base.Initialize(_target, _actions, _skill, _skillEffect, _owner, _height, _hasBlackOutTime, _isAbsolute, _targetPosition, _shakes, _targetBone);
			unitSpineController.Owner = _owner;
			unitSpineController.PlayAnimeNoOverlap(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_WALK, skillId);
			base.onCowHit = stopAndTurn;
			Vector3 position = base.transform.position;
			position.y = base.FireTarget.GetPosition().y;
			UnitCtrl unitCtrl = base.FireTarget.Owner;
			unitCtrl.OnIsFrontFalse = (Action)Delegate.Combine(unitCtrl.OnIsFrontFalse, (Action)delegate
			{
				unitSpineController.Depth = base.FireTarget.Owner.GetSortOrderConsiderBlackout() + 200;
			});
			position.x = (float)((!isLeftDir) ? 1 : (-1)) * -2500f / 540f;
			unitSpineController.Depth = base.FireTarget.Owner.GetSortOrderConsiderBlackout() + 200;
			base.transform.position = position;
			base.transform.localScale = new Vector3((float)((!isLeftDir) ? 1 : (-1)) * Mathf.Abs(base.transform.localScale.x), base.transform.localScale.y, base.transform.localScale.z);
			effectSmoke = battleEffectPool.GetEffect(isLeftDir ? ReturnWalkEffectPrefab : WalkEffectPrefab);
			effectSmoke.SortTarget = base.FireTarget.Owner;
			effectSmoke.transform.position = base.transform.position;
			effectSmoke.transform.parent = ExceptUIRoot.Transform;
			// Smoke tracking feeds only particle rendering; retain the actual effect lifecycle.
			ownerIsLeftDir = _owner.IsLeftDir;
			effectSmoke.GetComponent<SkillEffectCtrl>().PlaySe(_owner.ComponentSound.SoundUnitId, ownerIsLeftDir);
			effectSmoke.ExecAppendCoroutine(base.owner);
			shake.ShakeLoopEnd = false;
			battleCameraEffect.StartShake(shake, base.Skill, _owner);
		}

		[global::OpenPCR.Migration("49e02ba7-a7a4-4ee2-8d11-471054b35c1d")]
		private void stopAndTurn()
		{
			shake.ShakeLoopEnd = true;
			base.OnHitAction = null;
			effectSmoke.SetTimeToDie(value: true);
			base.speed = Vector3.zero;
			unitSpineController.PlayAnimeNoOverlap(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_ATTACK, skillId, _playLoop: false);
			if (base.battleManager.CanPlaySe())
			{
				// Attack audio is a presentation output; the original gate and following lifecycle remain.
			}

			AppendCoroutine(updateAttack(), ePauseType.SYSTEM, base.owner);
			turn = true;
		}

		[global::OpenPCR.Migration("9b69c082-4b14-4f3b-b721-c5127933e8bf")]
		private IEnumerator updateAttack()
		{
			bool effectCreated = false;
			float time = 0f;
			while (true)
			{
				time += base.battleManager.DeltaTime;
				if (!unitSpineController.IsPlayAnimeBattle)
				{
					break;
				}

				if (time > 0.3f && !effectCreated)
				{
					effectCreated = true;
					SkillEffectCtrl effect = battleEffectPool.GetEffect(AttackEffectPrefab);
					effect.transform.parent = ExceptUIRoot.Transform;
					effect.transform.position = base.FireTarget.GetBottomTransformPosition() + base.FireTarget.GetFixedCenterPos();
					effect.InitializeSort();
					effect.PlaySe(base.owner.ComponentSound.SoundUnitId, base.owner.IsLeftDir);
					effect.SortTarget = base.FireTarget.Owner;
					effect.ExecAppendCoroutine(base.owner);
					if (base.FireTarget.Owner.ComponentSortOrder.IsFront)
					{
						effect.SetSortOrderFront();
					}
					else
					{
						effect.SetSortOrderBack();
					}
				}

				yield return null;
			}

			base.transform.localScale = new Vector3(-base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
			unitSpineController.PlayAnimeNoOverlap(eAnimKitCharacterAnimeId.SUMMON_SKILL_EFFECT_WALK, skillId);
			base.speed = new Vector3(-MoveRate, 0f, 0f);
			shake.ShakeLoopEnd = false;
			battleCameraEffect.StartShake(shake, base.Skill, base.owner);
			effectSmoke = battleEffectPool.GetEffect(isLeftDir ? WalkEffectPrefab : ReturnWalkEffectPrefab);
			effectSmoke.SortTarget = base.FireTarget.Owner;
			effectSmoke.transform.position = base.transform.position;
			effectSmoke.transform.parent = ExceptUIRoot.Transform;
			effectSmoke.ExecAppendCoroutine(base.owner);
			// Smoke tracking feeds only particle rendering; retain the actual effect lifecycle.
			effectSmoke.GetComponent<SkillEffectCtrl>().PlaySe(base.owner.ComponentSound.SoundUnitId, ownerIsLeftDir);
		}

		[global::OpenPCR.Migration("7dbf48a6-4b1b-44f4-89a6-42a2c89a24b6")]
		public override bool _Update()
		{
			if (isLeftDir ? (base.transform.position.x > 4.6296296f) : (base.transform.position.x < -4.6296296f))
			{
				shake.ShakeLoopEnd = true;
				timeToDie = true;
				effectSmoke.SetTimeToDie(value: true);
			}

			bool result = base._Update();
			if (base.FireTarget.Owner.ComponentState.IsDead && !turn)
			{
				effectSmoke.SetTimeToDie(value: true);
				turn = true;
				base.transform.localScale = new Vector3((float)(isLeftDir ? 1 : (-1)) * base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
				base.speed = new Vector3(-MoveRate, 0f, 0f);
			}

			return result;
		}

		[global::OpenPCR.Migration("6fbf4f2c-5b27-4b3b-837f-c62125e7f6f3")]
		public override void Pause()
		{
			unitSpineController.Pause();
		}

		[global::OpenPCR.Migration("c75cf1d0-8b82-4ee3-984f-e04c89df2b13")]
		public override void Resume()
		{
			unitSpineController.Resume();
		}
	}
}
