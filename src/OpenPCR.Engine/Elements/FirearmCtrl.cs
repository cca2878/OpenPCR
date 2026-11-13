using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Elements.Battle.Core;

namespace Elements
{
	public class FirearmCtrl : SkillEffectCtrl
	{
		[SerializeField]
		protected float MoveRate = 4f;
		[SerializeField] private float HitDelay = 0.1f;
		[SerializeField]
		private float duration = 0.8f;
		public eMoveTypes MoveType;
		[SerializeField]
		private float startRotate = 60f;
		[SerializeField]
		private float endRotate = -60f;
		public Bounds ColliderBox;
		private CustomEasing easingX;
		private CustomEasing easingUpY;
		private CustomEasing easingDownY;
		private CustomEasing easingUpRotate;
		private CustomEasing easingDownRotate;
		private bool activeSelf = true;
		public bool IsAbsolute { get; set; }
		public BasePartsData FireTarget { get; protected set; }
		public List<SkillEffectBase> SkillHitEffects { get; set; }
		public List<ActionParameter> EndActions { get; set; }
		public Skill Skill { get; set; }
		public Action<FirearmCtrl> OnHitAction { get; set; }
		public Vector3 TargetPos { get; set; }
		public List<ShakeEffect> ShakeEffects { get; set; }
		private bool stopFlag { get; set; }
		protected Vector3 initialPosistion { get; set; }
		protected Vector3 speed { get; set; }
		protected UnitCtrl owner { get; set; }
		protected Action onCowHit { get; set; }

		[global::OpenPCR.Migration("959b3ace-fdd6-4a78-8725-7e3b3a09fcf5")]
		public FirearmCtrl()
		{
		}

		[global::OpenPCR.Migration("23d5ff0d-d78c-463e-9c31-5624bc113874")]
		protected virtual Vector3 getHeadBonePos(BasePartsData _target)
		{
			return _target.GetBottomTransformPosition() + _target.GetFixedCenterPos();
		}

		[global::OpenPCR.Migration("398a0e4f-653d-4f15-933f-d125f2b13b0f")]
		public virtual void Initialize(BasePartsData _target, List<ActionParameter> _actions, Skill _skill, List<SkillEffectBase> _skillEffect, UnitCtrl _owner, float _height, bool _hasBlackOutTime, bool _isAbsolute, Vector3 _targetPosition, List<ShakeEffect> _shakes, eTargetBone _targetBone)
		{
			ShakeEffects = _shakes;
			IsAbsolute = _isAbsolute;
			Skill = _skill;
			if (_isAbsolute)
			{
				TargetPos = _targetPosition;
			}
			else
			{
				switch (_targetBone)
				{
					case eTargetBone.CENTER:
					case eTargetBone.FIXED_CENETER:
						TargetPos = _target.GetBottomTransformPosition() + _target.GetFixedCenterPos();
						break;
					case eTargetBone.HEAD:
						TargetPos = getHeadBonePos(_target);
						break;
					case eTargetBone.BOTTOM:
						TargetPos = _target.GetBottomTransformPosition();
						break;
				}
			}

			FireTarget = _target;
			_target.Owner.FirearmCtrlsOnMe.Add(this);
			EndActions = _actions;
			SkillHitEffects = _skillEffect;
			owner = _owner;
			setInitialPosition();
			initMoveType(_height, _owner);
			float lifeDistance = Vector3.Distance(TargetPos, initialPosistion) + 1f;
			base.battleManager.AppendCoroutine(updatePosition(lifeDistance), ePauseType.SYSTEM, _hasBlackOutTime ? _owner : null);
			base.battleManager.AppendEffect(this, _hasBlackOutTime ? _owner : null, _isAbnormal: false);
		}

		[global::OpenPCR.Migration("5dc35a23-f9e4-4d22-8228-517425da333e")]
		protected virtual void setInitialPosition()
		{
			initialPosistion = base.transform.position;
		}

		[global::OpenPCR.Migration("eb74b5ad-f4c0-4e7e-809c-ddc4d04f1c7a")]
		protected virtual void Awake()
		{
			isRepeat = true;
		}

		[global::OpenPCR.Migration("5280c911-4610-4d35-903f-17ba87e7796f")]
		private void initMoveType(float _height, UnitCtrl _owner)
		{
			switch (MoveType)
			{
				case eMoveTypes.LINEAR:
					{
						Vector3 vector = TargetPos - base.transform.position;
						vector.Normalize();
						speed = MoveRate * vector;
						base.transform.rotation = Quaternion.FromToRotation((_owner == null || !_owner.IsLeftDir) ? Vector3.right : Vector3.left, vector);
						break;
					}

				case eMoveTypes.NONE:
				case eMoveTypes.HORIZONTAL:
					speed = new Vector3(MoveRate, 0f, 0f);
					break;
				case eMoveTypes.PARABORIC:
				case eMoveTypes.PARABORIC_ROTATE:
					{
						float durationTime = duration / 2f;
						easingUpY = new CustomEasing(CustomEasing.eType.outCubic, base.transform.position.y, base.transform.position.y + _height, durationTime);
						easingDownY = new CustomEasing(CustomEasing.eType.inQuad, base.transform.position.y + _height, TargetPos.y, durationTime);
						easingX = new CustomEasing(CustomEasing.eType.linear, base.transform.position.x, TargetPos.x, duration);
						break;
					}
			}

			if (MoveType == eMoveTypes.PARABORIC_ROTATE)
			{
				easingUpRotate = new CustomEasing(CustomEasing.eType.inQuad, startRotate, 0f, duration / 2f);
				easingDownRotate = new CustomEasing(CustomEasing.eType.linear, 0f, endRotate, duration / 2f);
			}
		}

		[global::OpenPCR.Migration("bd2e117a-3e13-4c0b-8f5d-9933953ebf73")]
		private IEnumerator updatePosition(float _lifeDistance)
		{
			float currentTime = 0f;
			bool hitFlag = false;
			float hitTimer = 0f;
			while (base.IsPlaying)
			{
				if (!activeSelf)
				{
					yield return null;
					continue;
				}
				float deltaTime = base.battleManager.DeltaTime;
				if (hitFlag)
				{
					hitTimer += deltaTime;
					float num = ((MoveType == eMoveTypes.LINEAR) ? (0.2f / MoveRate) : HitDelay);
					if (hitTimer > num || BattleUtil.Approximately(hitTimer, num))
					{
						hitTimer = 0f;
						hitFlag = false;
						stopFlag = true;
						if (OnHitAction != null)
						{
							FireTarget.Owner.FirearmCtrlsOnMe.Remove(this);
							OnHitAction(this);
						}
						onCowHit.Call();
						onCowHit = null;
					}
				}
				if (getStopFlag())
				{
					activeSelf = false;
					FireTarget.Owner.FirearmCtrlsOnMe.Remove(this);
					timeToDie = true;
				}
				else
				{
					if (this == null)
					{
						break;
					}
					Vector3 vector = base.transform.position;
					switch (MoveType)
					{
						case eMoveTypes.LINEAR:
						case eMoveTypes.HORIZONTAL:
							vector += speed * deltaTime;
							base.transform.position = vector;
							break;
						case eMoveTypes.PARABORIC:
						case eMoveTypes.PARABORIC_ROTATE:
							currentTime += deltaTime;
							vector = GetParaboricPosition(currentTime, deltaTime);
							if (currentTime > duration)
							{
								hitFlag = true;
							}
							break;
					}
					if (MoveType == eMoveTypes.PARABORIC_ROTATE)
					{
						// Only the particle's rendered orientation consumes this angle.
						// Preserve easing progression; numerical trajectory and bounds stay below.
						_ = (currentTime >= duration / 2f) ? easingDownRotate.GetCurVal(deltaTime, canOver: true) : easingUpRotate.GetCurVal(deltaTime, canOver: true);
					}
					base.transform.position = vector;
					eMoveTypes moveType = MoveType;
					if (moveType == eMoveTypes.LINEAR || moveType == eMoveTypes.HORIZONTAL)
					{
						if (IsAbsolute && Vector3.Distance(initialPosistion, vector) > _lifeDistance - 1f)
						{
							hitFlag = true;
						}
						if (!(Vector3.Distance(initialPosistion, vector) <= _lifeDistance))
						{
							FireTarget.Owner.FirearmCtrlsOnMe.Remove(this);
							if (!IsAbsolute) timeToDie = true;
						}
					}
				}
				if (!IsAbsolute)
				{
					hitFlag = collisionDetection(hitFlag, currentTime);
				}
				yield return null;
			}
		}

		[global::OpenPCR.Migration("9d8ef01d-82d6-4583-becb-433da925d22f")]
		public override bool _Update()
		{
			if (!activeSelf)
			{
				base.gameObject.SetActive(value: false);
			}

			if (timeToDie)
			{
				// Stopping the rendered audio source has no battle consumer.
				base.IsPlaying = false;
				base.gameObject.SetActive(value: false);
				return false;
			}

			return true;
		}

		[global::OpenPCR.Migration("c06b3b5e-8cfd-42e5-a5e5-410ae60103bd")]
		protected virtual bool getStopFlag()
		{
			return stopFlag;
		}
		public bool InFlag { get; set; }

		[global::OpenPCR.Migration("c749ff18-f7ba-4f5e-b87d-7e0ec52b94d0")]
		private Vector3 GetParaboricPosition(float _currentTime, float _deltaTime)
		{
			Vector3 result = base.transform.position;
			if (easingX.IsMoving)
			{
				result = new Vector3(easingX.GetCurVal(_deltaTime, canOver: true), (_currentTime >= duration / 2f) ? easingDownY.GetCurVal(_deltaTime, canOver: true) : easingUpY.GetCurVal(_deltaTime, canOver: true), base.transform.position.z);
			}
			return result;
		}

		[global::OpenPCR.Migration("fc4da991-0e25-4e35-9677-e5e91f90fa45")]
		private bool collisionDetection(bool _hitFlag, float _currentTime)
		{
			if ((MoveType == eMoveTypes.PARABORIC || MoveType == eMoveTypes.PARABORIC_ROTATE) && _currentTime < duration * 0.5f)
			{
				return _hitFlag;
			}
			if (_hitFlag || InFlag)
			{
				return _hitFlag;
			}
			if (MoveType == eMoveTypes.LINEAR)
			{
				return collisionDetectionForLinear();
			}
			float num = base.transform.position.x + ColliderBox.center.x;
			float num2 = ColliderBox.extents.x * 0.5f;
			float num3 = FireTarget.GetColliderCenter().x + FireTarget.GetPosition().x;
			float num4 = FireTarget.GetColliderSize().x * 0.5f;
			float num5 = num - num2;
			float num6 = num + num2;
			float num7 = num3 - num4;
			float num8 = num3 + num4;
			if ((num7 < num6 || BattleUtil.Approximately(num7, num6)) && (num8 > num5 || BattleUtil.Approximately(num8, num5)))
			{
				if (MoveType == eMoveTypes.PARABORIC || MoveType == eMoveTypes.PARABORIC_ROTATE)
				{
					InFlag = true;
					return _hitFlag;
				}
				return true;
			}
			return _hitFlag;
		}

		[global::OpenPCR.Migration("4690f196-6512-44a4-b88a-6c5d50eab1ca")]
		private bool collisionDetectionForLinear()
		{
			float speedX = speed.x;
			float positionX = base.transform.position.x;
			float centerX = ColliderBox.center.x;
			float extentsX = ColliderBox.extents.x;
			float targetCenterX = FireTarget.GetColliderCenter().x;
			float targetPositionX = FireTarget.GetPosition().x;
			float projectileCenter = positionX + centerX;
			float targetCenter = targetCenterX + targetPositionX;
			float projectileHalf = extentsX * 0.5f;
			float targetHalf = FireTarget.GetColliderSize().x * 0.5f;
			if (speedX > 0f)
			{
				float projectileEdge = projectileCenter + projectileHalf;
				float targetEdge = targetCenter - targetHalf;
				return projectileEdge >= targetEdge || BattleUtil.Approximately(projectileEdge, targetEdge);
			}
			else
			{
				float projectileEdge = projectileCenter - projectileHalf;
				float targetEdge = targetCenter + targetHalf;
				return projectileEdge <= targetEdge || BattleUtil.Approximately(projectileEdge, targetEdge);
			}
		}

		[global::OpenPCR.Migration("a5f3ad34-b785-432d-8d8a-51a3c585ac0b")]
		public override void ResetParameter(GameObject _prefab, int _skinId, bool _isShadow)
		{
			base.ResetParameter(_prefab, _skinId, false);
			stopFlag = false;
			activeSelf = true;
		}
	}
}
