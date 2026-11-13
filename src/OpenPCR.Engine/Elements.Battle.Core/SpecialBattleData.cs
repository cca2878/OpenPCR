using Elements.Data;

namespace Elements.Battle.Core
{
	public class SpecialBattleData
	{
		public int PurposeCount { get; }
		public double ActionStartSecond { get; }
		public double AppearTime { get; }
		public double StartIdleTrigger { get; }

		public SpecialBattleData(MasterHatsuneSpecialBattle.HatsuneSpecialBattle _data)
		{
			PurposeCount = _data.purpose_count;
			ActionStartSecond = _data.action_start_second;
			AppearTime = _data.appear_time;
			StartIdleTrigger = _data.start_idle_trigger;
		}
	}
}
