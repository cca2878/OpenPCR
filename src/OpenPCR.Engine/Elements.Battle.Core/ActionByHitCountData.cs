using System.Runtime.CompilerServices;

namespace Elements.Battle.Core
{
	public class ActionByHitCountData
	{
		public float LimitTime { get; set; }
		public int ExecCounter { get; set; }
		public int ExecLimit { get; set; }
		public int ConditionCount { get; set; }
		public int HitCounter { get; set; }
		public global::Elements.eStateIconType StateIconType { get; set; }
		public bool Enable { get; set; }

		[global::OpenPCR.Migration("1c722eeb-4461-4de0-bf40-87de55aa09a0")]
		public ActionByHitCountData()
		{
		}
	}
}
