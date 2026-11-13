using System;

namespace Elements
{
	[Serializable]
	public class ActionExecTime
	{
		public float Time;
		public eDamageEffectType DamageNumType;
		public double Weight = 1.0;
		public float DamageNumScale = 1.0f;
		[global::OpenPCR.Migration("1b059b63-90ac-48cb-9194-fb29a6dbdb70")]
		public ActionExecTime()
		{
		}
	}
}
