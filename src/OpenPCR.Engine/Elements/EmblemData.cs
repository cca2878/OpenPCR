using System.Runtime.CompilerServices;

namespace Elements
{
	public class EmblemData
	{
		public global::System.Int32 EmblemId { get; private set; }
		public global::System.Int32 ExValue { get; private set; }

		public EmblemData()
		{
			initializeEmblemData();
		}

		private void initializeEmblemData()
		{
			EmblemId = 0;
			ExValue = 0;
		}
	}
}
