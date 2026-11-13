using System;
using System.Collections.Generic;
using global::OpenPCR.Runtime.Api;
namespace Elements
{
	[global::System.Serializable]
	public class ScaleChanger
	{
		public float Time;
		public float Scale;
		public float Duration;
		public global::Elements.CustomEasing.eType Easing;
		[global::OpenPCR.Migration("17bdd603-e1fa-45a8-8dbc-b231f8ec97c4")]
		public ScaleChanger()
		{
			Scale = 1f;
			Easing = (CustomEasing.eType)2;
		}
	}
}
