namespace Elements.Battle.Core
{
	public class ExConditionPassiveData
	{
		public eEffectType EffectType { get; set; }
		public int LimitNum { get; set; }
		public float HitRate { get; set; }
		public double Value { get; set; }
		public float CoolTime { get; set; }
		public Elements.eStateIconType IconType { get; set; }

		public enum eEffectType
		{
			NORMAL,
			CANCEL,
			SHORTEN
		}

		[global::OpenPCR.Migration("0f7a6c93-e767-420a-b290-730e9a734fe0")]
		public ExConditionPassiveData()
		{ }
	}
}
