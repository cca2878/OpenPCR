using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;

namespace Elements
{
	public class TweenInitEffectCtrl : SkillEffectCtrl
	{
		[SerializeField]
		private float toLocalX = 1.4f;
		[SerializeField]
		private float toWorldOffsetY = 1.4f;


		[global::OpenPCR.Migration("702f0c7d-1a25-4f71-ae6a-5ad69f3fe50e")]
		protected override void initTweener(UITweener _tweener)
		{
			if (!global::OpenPCR.PreparedEffectInputs.IsRendererOnlyTweener(_tweener) && _tweener is TweenPosition)
			{
				TweenPosition obj = _tweener as TweenPosition;
				obj.from = base.transform.localPosition;
				obj.to.x = base.transform.localPosition.x + toLocalX;
				obj.to.y = (5000f + base.battleManager.GetRespawnPos(eUnitRespawnPos.MAIN_POS_3)) / 540f + toWorldOffsetY;
			}

			base.initTweener(_tweener);
		}
	}
}
