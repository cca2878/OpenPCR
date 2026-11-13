using global::OpenPCR.Runtime.Api;
using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class AccumulativeDamageData
	{
		public int DamagedCount { get; set; }
		public eAccumulativeDamageType AccumulativeDamageType { get; set; }
		public double FixedValue { get; set; }
		public double PercentageValue { get; set; }
		public int CountLimit { get; set; }

		[global::OpenPCR.Migration("c9754d8c-66cd-45d6-b63d-11a18ab0980b")]
		public AccumulativeDamageData()
		{
		}

		[global::OpenPCR.Migration("7f332318-eab7-4a36-aacf-0b4cc53bf907")]
		public long CalcAdditionalDamage(int _count, long _baseDamage)
		{
			int count = _count < CountLimit ? _count : CountLimit;
			switch (AccumulativeDamageType)
			{
				case eAccumulativeDamageType.FIXED:
					return BattleUtil.DoubleToLong(FixedValue * (double)count);
				case eAccumulativeDamageType.PERCENTAGE:
					return BattleUtil.DoubleToLong(PercentageValue * (double)unchecked((long)count * _baseDamage) / 100.0);
				default:
					return 0;
			}
		}
	}
}
