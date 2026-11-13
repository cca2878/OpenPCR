using global::OpenPCR.Foundation;
using System;

namespace Elements.Battle.Core
{
	public class SealDamageCountCancelData : SealCancelData
	{
		[global::OpenPCR.Migration("560dda06-eb59-4c75-9c9c-82e1e37a861d")]
		public SealDamageCountCancelData(int _releaseDamageCount, Action _cancelSealAction) : base(_cancelSealAction)
		{
			releaseDamageCount = _releaseDamageCount;
		}

		[global::OpenPCR.Migration("9524ed68-0d56-4f44-8442-effc88f9b03b")]
		public override void OnActivate()
		{
			(this).damageCount = unchecked((int)(0u));
		}

		[global::OpenPCR.Migration("933d0784-8f42-4eb2-bebd-ba0a8dc786fe")]
		public void OnDamage()
		{
			int divisor = releaseDamageCount;
			int count = unchecked(damageCount + 1);
			int quotient = divisor == 0 ? 0 : unchecked((int)((long)count / divisor));
			damageCount = count;
			int remainder = unchecked(count - quotient * divisor);
			if (remainder == 0) global::OpenPCR.Foundation.EventExtension.Call(CancelSealAction);
		}

		private int releaseDamageCount;
		private int damageCount;
	}
}
