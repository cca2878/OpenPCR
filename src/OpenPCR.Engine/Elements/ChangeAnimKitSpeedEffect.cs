using global::OpenPCR.Runtime.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Elements
{
	public class ChangeAnimKitSpeedEffect : SkillEffectCtrl
	{

		[SerializeField] private float startTimeScale;
		[SerializeField] private float startDuration;
		[SerializeField] private CustomEasing.eType startEasingType = CustomEasing.eType.outQuad;
		[SerializeField] private float duration;
		[SerializeField] private float endDuration;
		[SerializeField] private CustomEasing.eType endEasingType = CustomEasing.eType.outQuad;
		[SerializeField] private float timeScale;

		[global::OpenPCR.Migration("e9806ea9-2e9c-4849-b47f-924128b06a9a")]
		public ChangeAnimKitSpeedEffect()
		{ }

		[global::OpenPCR.Migration("8fc7f40d-a8b3-4fda-a49a-63b4e3ff31c3")]
		public override void SetPossitionAppearanceType(SkillEffectBase _skillEffect, BasePartsData _target, UnitCtrl _owner, Skill _skill, eToadTrackType _toadTrackType)
		{
			base.SetPossitionAppearanceType((global::Elements.SkillEffectBase)(@_skillEffect), (global::Elements.BasePartsData)(@_target), (global::Elements.UnitCtrl)(@_owner), (global::Elements.Skill)(@_skill), unchecked((global::Elements.SkillEffectCtrl.eToadTrackType)(unchecked((uint)(@_toadTrackType)))));
			if (global::System.Object.ReferenceEquals(@_skill, null))
				throw new global::System.NullReferenceException();
			if (global::System.Object.ReferenceEquals(@_owner, null))
				throw new global::System.NullReferenceException();
			if (global::System.Object.ReferenceEquals(@_target, null))
				throw new global::System.NullReferenceException();
			int nativeValue1 = (@_skill).SkillId;
			int nativeValue2 = (@_owner).UnionBurstSkillId;
			global::Elements.UnitCtrl nativeGetter3 = (@_target).Owner;
			global::System.Collections.IEnumerator nativeResult2 = (this).updateSpineSpeed((global::Elements.UnitCtrl)(nativeGetter3));
			if (!((unchecked((uint)(nativeValue1)) == unchecked((uint)(nativeValue2)))))
			{
				global::Elements.UnitCtrl nativeGetter4 = (@_target).Owner;
				if (global::System.Object.ReferenceEquals(this, null))
					throw new global::System.NullReferenceException();
				base.AppendCoroutine((global::System.Collections.IEnumerator)(nativeResult2), unchecked((global::Elements.ePauseType)(0u)), (global::Elements.UnitCtrl)(nativeGetter4));
				return;
			}
			else
			{
				if (global::System.Object.ReferenceEquals(this, null))
					throw new global::System.NullReferenceException();
				base.AppendCoroutine((global::System.Collections.IEnumerator)(nativeResult2), unchecked((global::Elements.ePauseType)(3u)), null);
				return;
			}
		}

		[global::OpenPCR.Migration("44bc9650-d6ae-4d8e-b12b-aea3964abf73")]
		public override void SetStartTime(float _starttime)
		{
			(this).startTime = @_starttime;
		}

		[global::OpenPCR.Migration("036ec3b2-0c10-44fc-b23f-1da13e3869d3")]
		private IEnumerator updateSpineSpeed(UnitCtrl _target)
		{
			int id = ++_target.CurrentSpineSpeedCoroutineId;
			float localStartTime = startTime;
			float time = startDuration;
			CustomEasing customEasing = new CustomEasing(startEasingType, 0f, 1f, 1f);
			while (true)
			{
				if (id != _target.CurrentSpineSpeedCoroutineId)
				{
					_target = null;
					timeToDie = true;
					yield break;
				}
				if (localStartTime <= 0f) yield return null;
				else localStartTime -= battleManager.DeltaTime;
				time -= battleManager.DeltaTime;
				float value = customEasing.GetCurVal(battleManager.DeltaTime / startDuration);
				float clamped = value <= 1f ? value : 1f;
				if (value < 0f) clamped = 0f;
				_target.GetCurrentSpineCtrl().SetTimeScaleForEffect(startTimeScale + clamped * (timeScale - startTimeScale));
				if (time < 0f) break;
			}
			time = duration;
			while (true)
			{
				if (id != _target.CurrentSpineSpeedCoroutineId)
				{
					_target = null;
					timeToDie = true;
					yield break;
				}
				_target.GetCurrentSpineCtrl().SetTimeScaleForEffect(timeScale);
				if (localStartTime <= 0f) yield return null;
				else localStartTime -= battleManager.DeltaTime;
				time -= battleManager.DeltaTime;
				if (time < 0f) break;
			}
			customEasing = new CustomEasing(endEasingType, 0f, 1f, 1f);
			time = endDuration;
			while (true)
			{
				if (id != _target.CurrentSpineSpeedCoroutineId)
				{
					_target = null;
					timeToDie = true;
					yield break;
				}
				if (localStartTime <= 0f) yield return null;
				else localStartTime -= battleManager.DeltaTime;
				time -= battleManager.DeltaTime;
				float value = customEasing.GetCurVal(battleManager.DeltaTime / endDuration);
				float clamped = value <= 1f ? value : 1f;
				if (value < 0f) clamped = 0f;
				_target.GetCurrentSpineCtrl().SetTimeScaleForEffect(timeScale + clamped * (1f - timeScale));
				if (time < 0f) break;
			}
			_target.GetCurrentSpineCtrl().SetTimeScaleForEffect(1f);
			_target = null;
			timeToDie = true;
		}

		private float startTime { get; set; }
	}
}
