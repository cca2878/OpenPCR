namespace Elements
{
	public class UnitUnionBurstTimeline
	{
		public int unit_id { get; private set; }
		public int remain_time { get; private set; }
		public int is_battle_finish { get; private set; }

		[global::OpenPCR.Migration("bbfa9cf8-dee4-489d-96df-3968cbde1cf9")]
		public UnitUnionBurstTimeline()
		{
			initializeUnitUnionBurstTimeline();
		}

		[global::OpenPCR.Migration("cc24d3f9-6aee-4a4c-bd1c-54ffdee861d8")]
		public void SetUnitId(int _unitId)
		{
			unit_id = _unitId;
		}

		[global::OpenPCR.Migration("5a031707-a811-42e0-8247-f15c04505d7a")]
		public void SetRemainTime(int _remainTime)
		{
			remain_time = _remainTime;
		}

		[global::OpenPCR.Migration("6fb82cfc-c505-4c8c-b343-4288de98e22a")]
		public void SetIsBattleFinish(int _isBattleFinish)
		{
			is_battle_finish = _isBattleFinish;
		}

		[global::OpenPCR.Migration("6efbefaf-1e37-4872-8739-71564aeb81e6")]
		private void initializeUnitUnionBurstTimeline()
		{
			unit_id = 0;
			remain_time = 0;
			is_battle_finish = 0;
		}
	}
}
