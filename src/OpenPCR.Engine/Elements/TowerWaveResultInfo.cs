namespace Elements
{
	public class TowerWaveResultInfo
	{
		public int wave_num { get; private set; }
		public TowerWaveResultUnitInfo[] unit_info_list { get; private set; }
		public int remain_time { get; private set; }

		[global::OpenPCR.Migration("a5aaf6a9-6c3b-4277-bb38-74f848ded5ff")]
		public TowerWaveResultInfo(int _waveNum, TowerWaveResultUnitInfo[] _unitInfoList, int _remainTime)
		{
			wave_num = _waveNum;
			unit_info_list = _unitInfoList;
			remain_time = _remainTime;
		}
	}
}
