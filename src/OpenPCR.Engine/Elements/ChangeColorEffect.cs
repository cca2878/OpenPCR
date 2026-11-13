using global::OpenPCR.Runtime.Api;
using System.Collections;
using Elements.Battle.Core;

namespace Elements
{
	public class ChangeColorEffect : SkillEffectCtrl
	{
		[SerializeField]
		private float startDuration;

		[SerializeField]
		private float duration;

		[SerializeField]
		private float endDuration;

		[SerializeField]
		private Color additiveColor = Color.black;

		[SerializeField]
		private Color multiplyColor = Color.white;

		[SerializeField]
		private bool setSortBack;

		[SerializeField]
		private bool exceptOwner;

		private Color startAdditiveColor { get; set; }

		private Color startMutiplyColor { get; set; }

		protected float startTime { get; set; }

		[global::OpenPCR.Migration("16f43f5f-1c7e-4d38-adb0-df0228a837e2")] public ChangeColorEffect() { }

		[global::OpenPCR.Migration("06a0f5d0-5e38-43c6-8811-e1cc8f8ec1bd")]
		public override void SetPossitionAppearanceType(SkillEffectBase _skillEffect, BasePartsData _target, UnitCtrl _owner, Skill _skill, eToadTrackType _toadTrackType)
		{
			base.SetPossitionAppearanceType(_skillEffect, _target, _owner, _skill, _toadTrackType);
			bool isUnionBurst = _skill.SkillId == _owner.UnionBurstSkillId;
			StartUpdateDisappear(isUnionBurst, _target);
		}

		[global::OpenPCR.Migration("2067a22f-4ee5-4349-abcf-2f7d20344d84")]
		public override void StartUpdateDisappear(bool _isUnionBurst, BasePartsData _target)
		{
			AppendCoroutine(updateDisappear(), _isUnionBurst ? GetPauseType() : ePauseType.VISUAL, _isUnionBurst ? null : _target.Owner);
		}

		[global::OpenPCR.Migration("387e770e-3d29-4121-8f5b-d215c330e06e")]
		public virtual ePauseType GetPauseType()
		{
			return ePauseType.IGNORE_BLACK_OUT;
		}

		[global::OpenPCR.Migration("3c1b621d-702a-40ff-a0e1-edd5a821c934")]
		public override void OnAwakeWhenSkipCutIn()
		{
			if ((!exceptOwner || !(base.SortTarget == source)) && setSortBack)
			{
				base.SortTarget.ComponentSortOrder.SetSortOrderBack();
				base.SortTarget.DisableSortOrderFrontOnBlackoutTarget = true;
			}
		}

		[global::OpenPCR.Migration("776219b3-26c5-4735-b45a-a0c60618a31f")]
		private IEnumerator updateDisappear()
		{
			yield return new WaitWhile(() => base.SortTarget == null);
			if (exceptOwner && base.SortTarget == source)
			{
				yield break;
			}
			float localStartTime = startTime;
			startAdditiveColor = new Color(0f, 0f, 0f, 0f);
			startMutiplyColor = Color.white;
			if (setSortBack)
			{
				base.SortTarget.ComponentSortOrder.SetSortOrderBack();
			}
			float time = startDuration;
			UnitComponentColor colorComponent = base.SortTarget.ComponentColor;
			do
			{
				if (localStartTime > 0f)
				{
					localStartTime -= base.battleManager.DeltaTime;
				}
				else
				{
					yield return null;
				}
				time -= base.battleManager.DeltaTime;
				colorComponent.SetCurColor(this, Color.Lerp(startMutiplyColor, multiplyColor, 1f - time / startDuration));
				colorComponent.SetCurColorOffset(this, Color.Lerp(startAdditiveColor, additiveColor, 1f - time / startDuration));
			}
			while (!(time < 0f));
			time = duration;
			if (multiplyColor.a == 0f)
			{
				base.SortTarget.HideOtherAuraEffect.Add(this);
				base.SortTarget.ComponentAbnormal.EnableAbnormalEffect(_active: false);
				base.SortTarget.ComponentAbnormal.EnableAuraEffect(_active: false);
			}
			do
			{
				colorComponent.SetCurColor(this, multiplyColor);
				colorComponent.SetCurColorOffset(this, additiveColor);
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
			time = endDuration;
			do
			{
				if (localStartTime > 0f)
				{
					localStartTime -= base.battleManager.DeltaTime;
				}
				else
				{
					yield return null;
				}
				time -= base.battleManager.DeltaTime;
				colorComponent.SetCurColor(this, Color.Lerp(multiplyColor, startMutiplyColor, 1f - time / endDuration));
				colorComponent.SetCurColorOffset(this, Color.Lerp(additiveColor, startAdditiveColor, 1f - time / endDuration));
			}
			while (!(time < 0f));
			colorComponent.ResetColor(this);
			if (setSortBack && base.battleManager.BlackOutUnitList.Contains(source))
			{
				base.SortTarget.ComponentSortOrder.SetSortOrderFront();
			}
			if (multiplyColor.a == 0f)
			{
				base.SortTarget.HideOtherAuraEffect.Remove(this);
				base.SortTarget.ComponentAbnormal.EnableAbnormalEffect(_active: true);
				base.SortTarget.ComponentAbnormal.EnableAuraEffect(_active: true);
			}
			base.SortTarget = null;
			timeToDie = true;
		}
	}
}
