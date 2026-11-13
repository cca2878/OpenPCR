using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System.Diagnostics;
using global::OpenPCR.Foundation;

namespace OpenPCR.UICompat
{

	public static class UIMath
	{

		public static void MoveRect(UIRect rect, float x, float y)
		{
			int num = Mathf.FloorToInt(x + 0.5f);
			int num2 = Mathf.FloorToInt(y + 0.5f);
			rect.cachedTransform.localPosition += new Vector3(num, num2);
			int num3 = 0;
			if ((bool)rect.leftAnchor.target)
			{
				num3++;
				rect.leftAnchor.absolute += num;
			}
			if ((bool)rect.rightAnchor.target)
			{
				num3++;
				rect.rightAnchor.absolute += num;
			}
			if ((bool)rect.bottomAnchor.target)
			{
				num3++;
				rect.bottomAnchor.absolute += num2;
			}
			if ((bool)rect.topAnchor.target)
			{
				num3++;
				rect.topAnchor.absolute += num2;
			}
			if (num3 != 0)
			{
				rect.UpdateAnchors();
			}
		}
	}

}
