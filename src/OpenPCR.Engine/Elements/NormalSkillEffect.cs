using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;

namespace Elements
{
	[Serializable]
	public class NormalSkillEffect : SkillEffectBase
	{
		public List<SkillEffectBase> FireArmEndEffects = new List<SkillEffectBase>();
		[global::OpenPCR.Migration("b0f5e61a-d7b6-4504-9e2e-93430c23a40b")]
		public NormalSkillEffect()
		{
		}
	}
}
