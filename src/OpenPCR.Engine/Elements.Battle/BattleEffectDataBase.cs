using global::OpenPCR.UICompat;
using System;
using System.Collections.Generic;

namespace Elements.Battle
{
	[Serializable]
	public class BattleEffectDataBase
	{
		public List<SkillEffectCtrl> SkillEffectList = new List<SkillEffectCtrl>();
		public List<UITweener> UiTweenerList = new List<UITweener>();
		public List<UITweener> UiTweenerListIgnoreBlackout = new List<UITweener>();
		[global::OpenPCR.Migration("4e2bf785-8af2-4f5e-acad-7231303f60e2")]
		public BattleEffectDataBase()
		{
		}

		[global::OpenPCR.Migration("8e6c9b7e-8105-41f4-aa29-2456f91f56c6")]
		public virtual void SetSkillEffectSortBack()
		{
			for (int num = SkillEffectList.Count - 1; num >= 0; num--)
			{
				SkillEffectCtrl skillEffectCtrl = SkillEffectList[num];
				if (skillEffectCtrl == null || !skillEffectCtrl.IsPlaying)
				{
					SkillEffectList.RemoveAt(num);
				}
				else if (skillEffectCtrl.gameObject.activeSelf && skillEffectCtrl.IsFront)
				{
					skillEffectCtrl.SetSortOrderBack();
				}
			}
		}

		[global::OpenPCR.Migration("087967a8-78b8-43bd-bdc4-966798516ae0")]
		public virtual void Pause()
		{
			for (int num = SkillEffectList.Count - 1; num >= 0; num--)
			{
				if (SkillEffectList[num] == null || !SkillEffectList[num].IsPlaying)
				{
					SkillEffectList.RemoveAt(num);
				}
				else if (SkillEffectList[num].gameObject.activeSelf)
				{
					SkillEffectList[num].Pause();
				}
			}

			for (int num2 = UiTweenerList.Count - 1; num2 >= 0; num2--)
			{
				if (UiTweenerList[num2] == null)
				{
					UiTweenerList.RemoveAt(num2);
				}
				else
				{
					UiTweenerList[num2].enabled = false;
				}
			}

			for (int num3 = UiTweenerListIgnoreBlackout.Count - 1; num3 >= 0; num3--)
			{
				if (UiTweenerListIgnoreBlackout[num3] == null)
				{
					UiTweenerListIgnoreBlackout.RemoveAt(num3);
				}
				else
				{
					UiTweenerListIgnoreBlackout[num3].enabled = false;
				}
			}
		}

		[global::OpenPCR.Migration("0e499a4f-5bb8-4bd0-b3c8-4d3ee02e5a63")]
		public virtual void Resume()
		{
			for (int num = SkillEffectList.Count - 1; num >= 0; num--)
			{
				if (SkillEffectList[num] == null || !SkillEffectList[num].IsPlaying)
				{
					SkillEffectList.RemoveAt(num);
				}
				else
				{
					SkillEffectList[num].Resume();
				}
			}

			for (int num2 = UiTweenerList.Count - 1; num2 >= 0; num2--)
			{
				if (UiTweenerList[num2] == null)
				{
					UiTweenerList.RemoveAt(num2);
				}
				else
				{
					UiTweenerList[num2].enabled = true;
				}
			}
		}

		[global::OpenPCR.Migration("df38bdcf-3e30-468f-a945-917e46989a5c")]
		public virtual void SetSortOrderBack()
		{
			for (int num = SkillEffectList.Count - 1; num >= 0; num--)
			{
				if (SkillEffectList[num] == null || !SkillEffectList[num].IsPlaying)
				{
					SkillEffectList.RemoveAt(num);
				}
				else
				{
					SkillEffectList[num].SetSortOrderBack();
				}
			}
		}

	}
}
