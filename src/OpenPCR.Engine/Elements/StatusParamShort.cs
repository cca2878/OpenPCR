using System.Runtime.CompilerServices;

namespace Elements
{
	public class StatusParamShort
	{
		public global::System.Int64 Hp { get; private set; }
		public global::System.Int32 Atk { get; private set; }
		public global::System.Int32 Def { get; private set; }
		public global::System.Int32 Matk { get; private set; }
		public global::System.Int32 Mdef { get; private set; }
		public global::System.Int32 Crt { get; private set; }
		public global::System.Int32 Mcrt { get; private set; }
		public global::System.Int32 Hrec { get; private set; }
		public global::System.Int32 Erec { get; private set; }
		public global::System.Int32 HrecRate { get; private set; }
		public global::System.Int32 Pnt { get; private set; }
		public global::System.Int32 Mpnt { get; private set; }
		public global::System.Int32 LifeSteal { get; private set; }
		public global::System.Int32 Dodge { get; private set; }
		public global::System.Int32 ErecRate { get; private set; }
		public global::System.Int32 EredRate { get; private set; }
		public global::System.Int32 Accuracy { get; private set; }

		public StatusParamShort()
		{
			initializeStatusParamShort();
		}

		private void initializeStatusParamShort()
		{
			Hp = 0L;
			Atk = 0;
			Def = 0;
			Matk = 0;
			Mdef = 0;
			Crt = 0;
			Mcrt = 0;
			Hrec = 0;
			Erec = 0;
			HrecRate = 0;
			Pnt = 0;
			Mpnt = 0;
			LifeSteal = 0;
			Dodge = 0;
			ErecRate = 0;
			EredRate = 0;
			Accuracy = 0;
		}
	}
}
