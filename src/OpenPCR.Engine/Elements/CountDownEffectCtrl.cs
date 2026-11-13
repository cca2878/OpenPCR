using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements
{
	public class CountDownEffectCtrl : SkillEffectCtrl
	{
		public List<CountEndEffectData> CountEndEffectData;
		[SerializeField]
		private int warningLimitTime = 5;
		[global::OpenPCR.Migration("1acec36e-95e1-407d-92af-216c2909da52")]
		public CountDownEffectCtrl()
		{
		}

		[global::OpenPCR.Migration("f84af992-c803-40ea-9dda-bd76f11d2388")]
		public override void InitializeSort()
		{
			base.InitializeSort();
			// UIPanel sorting is render-only; the base registration stays live.
		}

		[global::OpenPCR.Migration("a5dd09d9-2038-417c-94a9-92e16bbaee0c")]
		public void SetDamageText(int _number)
		{
			global::OpenPCR.Presentation.Noop();
		}
	}
}
