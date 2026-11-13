namespace Elements
{
	public class TowerWaveResultUnitInfo
	{
		public int unit_id { get; private set; }
		public long owner_viewer_id { get; private set; }
		public long damage { get; private set; }
		public int is_alive { get; private set; }

		[global::OpenPCR.Migration("1fde7c83-1cb9-45a9-b352-21120c855831")]
		public TowerWaveResultUnitInfo(int _unitId, long _ownerViewerId, long _damage, int _isAlive)
		{
			unit_id = _unitId;
			owner_viewer_id = _ownerViewerId;
			damage = _damage;
			is_alive = _isAlive;
		}
	}
}
