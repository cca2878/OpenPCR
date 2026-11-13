using global::OpenPCR.Runtime.Api;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Elements.Battle;

namespace Elements
{
	public class PartsBossGauge : MonoBehaviour, ISingletonField
	{
		private IBattleManagerForPartsBossGauge battleManager;

		[global::OpenPCR.Migration("de96732d-26d4-435c-9303-d25a548cb80b")]
		public void SetNormalizedLifeAmount(long _maxHp, long _currentHp)
		{
			global::OpenPCR.Presentation.Noop();
		}

		[global::OpenPCR.Migration("467fdfa0-6793-46ef-a9a0-60f57d97c2e2")]
		public void UpdateInvalidLifeAmount(long _hp)
		{
			global::OpenPCR.Presentation.Noop();
		}
		public AbromalStateIconController AbnormalStateIcon { get; set; }


	}
}
