using System.Runtime.CompilerServices;

namespace Elements
{
	public class UnitParam
	{
		public StatusParam BaseParam { get; private set; }
		public StatusParam EquipParam { get; private set; }

		[global::OpenPCR.Migration("03c409d7-84c7-4c3b-bacf-fade4d1aea7e")]
		public UnitParam()
		{
			initializeUnitParam();
		}

		[global::OpenPCR.Migration("2277ad43-871b-4c42-95ab-b4f0ceba99cd")]
		public void SetBaseParam(StatusParam _baseParam)
		{
			BaseParam = _baseParam;
		}

		[global::OpenPCR.Migration("4a38c15f-92ed-4e30-8605-f069bad39db9")]
		public void SetEquipParam(StatusParam _equipParam)
		{
			EquipParam = _equipParam;
		}

		[global::OpenPCR.Migration("33dd7b3f-f7d5-459d-865a-997304cbb510")]
		private void initializeUnitParam()
		{
			BaseParam = null;
			EquipParam = null;
		}
	}
}
