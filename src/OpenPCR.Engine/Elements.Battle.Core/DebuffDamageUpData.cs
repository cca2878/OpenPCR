using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class DebuffDamageUpData
	{

		public enum eEffectType
		{
			ADD = 1,
			SUBTRACT = 2
		}


		public double DebuffDamageUpValue { get; set; }

		public double DebuffDamageUpLimitValue { get; set; }

		public float DebuffDamageUpTimer { get; set; }

		public global::Elements.Battle.Core.DebuffDamageUpData.eEffectType EffectType { get; set; }


		[global::OpenPCR.Migration("68644aaf-1939-4e29-8228-79c40d101591")]
		public DebuffDamageUpData()
		{

		}
	}
}
