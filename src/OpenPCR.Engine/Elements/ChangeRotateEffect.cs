using global::OpenPCR.Runtime.Api;
using System.Collections;

namespace Elements
{
	public class ChangeRotateEffect : SkillEffectCtrl
	{
		[SerializeField]
		private float startAngle;

		[SerializeField]
		private float startDuration;

		[SerializeField]
		private CustomEasing.eType startEasingType = CustomEasing.eType.outQuad;

		[SerializeField]
		private float duration;

		[SerializeField]
		private float endDuration;

		[SerializeField]
		private CustomEasing.eType endEasingType = CustomEasing.eType.outQuad;

		[SerializeField]
		private float angle;

		[SerializeField]
		private Vector2 centerPos = Vector2.zero;

		[SerializeField]
		private bool reverseMode;

		private float startTime { get; set; }

		[global::OpenPCR.Migration("781714b4-bd13-42ca-b8d9-2bc3e053bee0")] public ChangeRotateEffect() { }

		[global::OpenPCR.Migration("de9eacdd-1bed-4def-af1a-e50f82ff37dd")]
		public override void SetPossitionAppearanceType(SkillEffectBase _skillEffect, BasePartsData _target, UnitCtrl _owner, Skill _skill, eToadTrackType _toadTrackType)
		{
			base.SetPossitionAppearanceType(_skillEffect, _target, _owner, _skill, _toadTrackType);
			bool flag = _skill.SkillId == _owner.UnionBurstSkillId;
			AppendCoroutine(updateRotate(_target.Owner), flag ? ePauseType.NO_DIALOG : ePauseType.VISUAL, flag ? null : _target.Owner);
		}

		[global::OpenPCR.Migration("1c2b3f91-2897-4aa5-9168-43a410a43a5b")]
		public override void SetStartTime(float _starttime)
		{
			startTime = _starttime;
		}

		[global::OpenPCR.Migration("30e40731-8553-49a0-85ef-00187d07fae6")]
		private IEnumerator updateRotate(UnitCtrl _target)
		{
			_target.CurrentRotateCoroutineId++;
			int id = _target.CurrentRotateCoroutineId;
			float localStartTime = startTime;
			float direction = (_target.IsLeftDir ? (-1f) : 1f);
			_target.RotateCenter.SetLocalPosY(centerPos.y);
			_target.RotateCenter.SetLocalPosX(direction * centerPos.x);
			_target.GetCurrentSpineCtrl().transform.SetLocalPosY(-centerPos.y);
			_target.GetCurrentSpineCtrl().transform.SetLocalPosX(-(direction * centerPos.x));
			float time = startDuration;
			CustomEasing customEasing = new CustomEasing(startEasingType, 0f, 1f, 1f);
			do
			{
				if (id != _target.CurrentRotateCoroutineId)
				{
					_target = null;
					timeToDie = true;
					yield break;
				}
				if (localStartTime > 0f)
				{
					localStartTime -= base.battleManager.DeltaTime;
				}
				else
				{
					yield return null;
				}
				time -= base.battleManager.DeltaTime;
				float curVal = customEasing.GetCurVal(base.battleManager.DeltaTime / startDuration);
				_target.RotateCenter.eulerAngles = new Vector3(0f, 0f, direction * LerpAngle(startAngle, angle, curVal));
			}
			while (!(time < 0f));
			time = duration;
			do
			{
				if (id != _target.CurrentRotateCoroutineId)
				{
					_target = null;
					timeToDie = true;
					yield break;
				}
				_target.RotateCenter.eulerAngles = new Vector3(0f, 0f, direction * angle);
				if (localStartTime > 0f)
				{
					localStartTime -= base.battleManager.DeltaTime;
				}
				else
				{
					yield return null;
				}
				time -= base.battleManager.DeltaTime;
			}
			while (!(time < 0f));
			customEasing = new CustomEasing(endEasingType, 0f, 1f, 1f);
			time = endDuration;
			do
			{
				if (id != _target.CurrentRotateCoroutineId)
				{
					_target = null;
					timeToDie = true;
					yield break;
				}
				if (localStartTime > 0f)
				{
					localStartTime -= base.battleManager.DeltaTime;
				}
				else
				{
					yield return null;
				}
				time -= base.battleManager.DeltaTime;
				float curVal2 = customEasing.GetCurVal(base.battleManager.DeltaTime / endDuration);
				_target.RotateCenter.eulerAngles = new Vector3(0f, 0f, direction * LerpAngle(angle, 0f, curVal2));
			}
			while (!(time < 0f));
			_target.RotateCenter.eulerAngles = Vector3.zero;
			_target = null;
			timeToDie = true;
		}

		[global::OpenPCR.Migration("ec0f184d-0bdb-43b8-9144-fe660c4327b0")]
		public float LerpAngle(float _a, float _b, float _t)
		{
			float num = Mathf.Repeat(_b - _a, 360f);
			if (reverseMode)
			{
				if (num <= 180f)
				{
					return _a + 360f - (360f - num) * Mathf.Clamp01(_t);
				}
			}
			else if (num > 180f)
			{
				num -= 360f;
			}
			return _a + num * Mathf.Clamp01(_t);
		}
	}
}
