using global::OpenPCR.Runtime.Api;
using System;
using System.Collections.Generic;
using global::OpenPCR.Foundation;
using Elements.Battle;

namespace Elements
{
	public class PartsMultiBossGauge : MonoBehaviour, ISingletonField
	{
		[SerializeField]
		private PartsHeaderHpGauge hpGauge;

		private bool isBossGaugeColorChange;
		private IBattleManagerForPartsBossGauge battleManager;


		[global::OpenPCR.Migration("ac9dffee-df40-4329-97eb-2d07966b3113")]
		public void SetNormalizedLifeAmount(long _maxHp, long _currentHp)
		{
			float value = (float)_currentHp / (float)_maxHp;
			if (isBossGaugeColorChange && battleManager.GetBossGaugeColorChange())
				value = battleManager.GetBossGaugeValue(_currentHp);
			hpGauge.PlaySliderAnimation(value);
			// Native HP labels/percentage text are pure output and intentionally excluded.
		}
	}
}
