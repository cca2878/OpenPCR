using global::OpenPCR.Runtime.Api;

namespace Elements
{
	[global::System.Serializable]
	public class PartsHeaderRewardTween : MonoBehaviour
	{
		private int currentNum;
		private eSE updateNumSe = (eSE)225;



		public void SetNum(int _num)
		{
			if (currentNum != _num)
			{
				currentNum = _num;
				setActiveTweenAlpha(true);
				setNumLabel(_num);
			}
		}

		private void setActiveTweenAlpha(bool _isActive)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.PartsHeaderRewardTween::setActiveTweenAlpha: reward UI widget activation backend");
		}

		private void setNumLabel(int _num)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.PartsHeaderRewardTween::setNumLabel: reward UI text, sound and alpha/scale tween backend");
		}
	}
}
