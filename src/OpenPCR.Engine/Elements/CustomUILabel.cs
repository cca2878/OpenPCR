using global::OpenPCR.UICompat;
using global::OpenPCR.Runtime.Api;
using System;
using global::OpenPCR.Foundation;

namespace Elements
{
	public class CustomUILabel : UILabel
	{
		[global::OpenPCR.Runtime.Api.SerializeField]
		private string strTextId = eTextId.NONE.ToString();
		private eTextId curTextId = eTextId.NONE;
		private Color originalColor = Color.white;
		private Color originalEffectColor = Color.white;
		private bool isUseColorCode = true;



		public void SetText(eTextId textId, params object[] args)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.CustomUILabel::SetText(Elements.eTextId, params object[])");
		}

		public void SetText(string _str, params object[] _args)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.CustomUILabel::SetText(string, params object[])");
		}

		public void SetTextNum(int _num)
		{
			SetText(_num.ToString(), Array.Empty<object>());
		}
	}
}
