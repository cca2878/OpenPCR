
namespace Elements
{
	public class UserQuestInfo
	{
		public global::System.Int32 QuestId { get; private set; }
		public global::System.Int32 ClearFlg { get; private set; }
		public global::System.Int32 ResultType { get; private set; }
		public global::System.Int32 DailyClearCount { get; private set; }
		public global::System.Int32 DailyRecoveryCount { get; private set; }

		private void initializeUserQuestInfo()
		{
			QuestId = 0;
			ClearFlg = 0;
			ResultType = 0;
			DailyClearCount = 0;
			DailyRecoveryCount = 0;
		}

		public UserQuestInfo()
		{
			initializeUserQuestInfo();
		}

		public UserQuestInfo(int _questId, int _clearFlg, int _resultType, int _dailyClearCount, int _dailyRecoveryCount)
		{
			QuestId = _questId;
			ClearFlg = _clearFlg;
			ResultType = _resultType;
			DailyClearCount = _dailyClearCount;
			DailyRecoveryCount = _dailyRecoveryCount;
		}
	}
}
