using global::OpenPCR.AudioStubs;
using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public class AwakeUbNoneStopEffect : SkillEffectCtrl
	{
		public enum eDisplayType { ALWAYS, DISPLAY_HIGH, DISPLAY_LOW }
		[SerializeField] private eDisplayType displayType;
		[SerializeField] private float startHpPercentage;

		[global::OpenPCR.Migration("7448b870-1b9f-4cb1-87df-ea2b93e59d29")]
		public override void SetTimeToDieByStartHp(float _hpPercent)
		{
			if (!(startHpPercentage < _hpPercent))
			{
				if (displayType != eDisplayType.DISPLAY_HIGH) return;
			}
			else if (displayType != eDisplayType.DISPLAY_LOW) return;
			base.SetTimeToDie(true);
		}

		[global::OpenPCR.Migration("cbf4590c-f265-4ad4-9ad0-bf04a27a76cd")]
		public AwakeUbNoneStopEffect()
		{
		}
	}
}
