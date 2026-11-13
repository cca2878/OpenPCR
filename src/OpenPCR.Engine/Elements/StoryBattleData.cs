using global::OpenPCR.Runtime.Api;
using System.Collections.Generic;

namespace Elements
{
	public class StoryBattleData : MonoBehaviour
	{
		public float StartPosition;
		public List<StoryBattleTimeLineData> TimeLineData = new List<StoryBattleTimeLineData>();
		public bool IsUseVoiceTimeLine;
		public List<StoryBattleVoiceTimeLineData> VoiceTimeLineData = new List<StoryBattleVoiceTimeLineData>();
		public StoryBattleData()
		{
			throw new global::OpenPCR.MigrationPendingException("Elements.StoryBattleData::.ctor()", "body migration pending");
		}
	}
}
