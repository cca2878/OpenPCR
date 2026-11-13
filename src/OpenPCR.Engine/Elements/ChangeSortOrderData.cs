using System;
using System.Collections.Generic;
using global::OpenPCR.Runtime.Api;
namespace Elements
{
	[global::System.Serializable]
	public class ChangeSortOrderData
	{
		public global::Elements.ChangeSortOrderData.eSortType SortType;
		public float Time;
		[global::OpenPCR.Migration("0a9913e8-8d87-496c-adc3-eddeb16fc1d5")]
		public ChangeSortOrderData()
		{
		}
		public enum eSortType
		{
			DEFAULT = 0,
			FRONT = 1,
			BACK = 2,
		}
	}
}
