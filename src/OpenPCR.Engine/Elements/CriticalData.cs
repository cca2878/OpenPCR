using System.Runtime.CompilerServices;

namespace Elements
{
	public class CriticalData
	{

		[global::OpenPCR.Migration("2fc593fb-a5f6-49b8-977b-6744d62bed41")]
		public CriticalData()
		{
		}

		public bool IsCritical { get; set; }
		public double CriticalRate { get; set; }
		public long ExpectedDamage { get; set; }
		public long ExpectedDamageForEnergyCalc { get; set; }
		public long ExpectedDamageNotCritical { get; set; }
		public bool ExpectedIsNoDamageByResist { get; set; }
	}
}
