namespace Elements.Battle.Core
{
	public class AccumulativeDamageSourceData
	{
		private global::Elements.Battle.Core.eAccumulativeDamageType AccumulativeDamageType = (eAccumulativeDamageType)1;
		private double effectValue;
		private int countLimit;
		[global::OpenPCR.Migration("224d310a-3da0-4a50-b5c3-573d9fbe62be")]
		public AccumulativeDamageSourceData(eAccumulativeDamageType _damageType, double _effectValue, int _countLimit)
		{
			AccumulativeDamageType = _damageType;
			effectValue = _effectValue;
			countLimit = _countLimit;
		}

		[global::OpenPCR.Migration("c7da609f-30e6-43d2-b410-4f2c6c367199")]
		public AccumulativeDamageData CreateAccumulativeDamageData()
		{
			var result = new AccumulativeDamageData();
			result.AccumulativeDamageType = AccumulativeDamageType;
			result.FixedValue = AccumulativeDamageType == eAccumulativeDamageType.FIXED ? effectValue : 0.0;
			result.PercentageValue = AccumulativeDamageType == eAccumulativeDamageType.PERCENTAGE ? effectValue : 0.0;
			result.CountLimit = countLimit;
			return result;
		}
	}
}
