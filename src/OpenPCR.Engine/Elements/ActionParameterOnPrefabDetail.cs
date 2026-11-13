using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements
{
	[Serializable]
	public class ActionParameterOnPrefabDetail
	{
		public bool Visible;
		public List<ActionExecTime> ExecTimeForPrefab = new List<ActionExecTime>();
		public List<ActionExecTimeCombo> ExecTimeCombo = new List<ActionExecTimeCombo>();
		public int ActionId;
		public List<NormalSkillEffect> ActionEffectList = new List<NormalSkillEffect>();
		public List<NormalSkillEffect> ActionSubEffectList = new List<NormalSkillEffect>();
		public List<ActionExecTime> ExecTime { get; set; }

		[global::OpenPCR.Migration("bb6a1e1a-2267-400c-987b-dc6982a7fc89")]
		public ActionParameterOnPrefabDetail()
		{
		}
	}
}
