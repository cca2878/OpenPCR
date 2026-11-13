using System.Runtime.CompilerServices;

namespace Elements
{
	public class UnitHpInfo
	{
		public long viewer_id { get; private set; }
		public int unit_id { get; private set; }
		public long hp { get; private set; }

		[global::OpenPCR.Migration("97b07eb8-3085-4832-9ab4-e42f7bc96f97")]
		public UnitHpInfo()
		{
			initializeUnitHpInfo();
		}

		[global::OpenPCR.Migration("d891e802-d9e8-4f4d-ada1-fa309f241cc0")]
		public void SetViewerId(long _viewerId)
		{
			viewer_id = _viewerId;
		}

		[global::OpenPCR.Migration("a60a09f5-919e-4a23-bbb2-a070f1fd9e2d")]
		public void SetUnitId(int _unitId)
		{
			unit_id = _unitId;
		}

		[global::OpenPCR.Migration("5092212f-66d4-452e-8bd4-1f4ffdd9eda9")]
		public void SetHp(long _hp)
		{
			hp = _hp;
		}

		[global::OpenPCR.Migration("a426fde2-cfe0-41d3-9427-67cb2f77dc3c")]
		private void initializeUnitHpInfo()
		{
			viewer_id = 0;
			unit_id = 0;
			hp = 0;
		}
	}
}
