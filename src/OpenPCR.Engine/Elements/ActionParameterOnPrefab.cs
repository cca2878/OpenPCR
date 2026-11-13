using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using Elements.Battle.Core;

namespace Elements
{
	[Serializable]
	public class ActionParameterOnPrefab
	{
		public bool Visible;
		public eActionType ActionType = (eActionType)1;
		public List<ActionParameterOnPrefabDetail> Details = new List<ActionParameterOnPrefabDetail>();
		public AnimationCurve KnockAnimationCurve = new AnimationCurve();
		public AnimationCurve KnockDownAnimationCurve = new AnimationCurve();
		public eEffectType EffectType;
		public bool IsExcludedFromBlackOutTarget;
		[global::OpenPCR.Migration("663d0947-6c18-4804-9095-14dc1ed6a291")]
		public ActionParameterOnPrefab()
		{
		}
	}

}
