using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Foundation;
using System;
using System.Runtime.CompilerServices;
using Elements.Data;

namespace Elements
{
	public abstract class ViewBase : MonoBehaviour
	{
		[SerializeField]
		private bool isAutoSlide = true;
		[SerializeField]
		private bool isShowFooter = true;
		protected TempData tempData { get; private set; }
		protected DialogManager dialogManager { get; private set; }



		public abstract void StartView();
		public abstract void FinishView();
	}
}
