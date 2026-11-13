using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class StrikeBackDataSet
	{
		public List<StrikeBackData> DataList { get; set; }
		public SkillEffectCtrl SkillEffect { get; set; }

		[global::OpenPCR.Migration("81b79a41-ac0d-4e12-a9f0-5df7a15563c4")]
		public StrikeBackDataSet()
		{
		}

		[global::OpenPCR.Migration("fa096b79-6aeb-4dba-b84c-70d41260cc74")]
		public void SetTimeToDie()
		{
			SkillEffect.SetTimeToDie(value: true);
		}
	}
}
