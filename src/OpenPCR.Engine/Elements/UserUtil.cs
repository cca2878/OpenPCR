using Elements.Data;

namespace Elements
{
	public static class UserUtil
	{
		[global::OpenPCR.Migration("3e9e3eae-6e5c-47d9-ada0-4d74444b6181")]
		public static bool IsContentReleased(MasterContentReleaseData.ContentReleaseData _contentReleaseData)
		{
			if (_contentReleaseData == null)
				return false;
			UserData instance = Singleton<UserData>.Instance;
			if ((int)_contentReleaseData.team_level > 0 && (int)instance.UserInfo.TeamLevel < (int)_contentReleaseData.team_level)
				return false;
			if ((int)_contentReleaseData.quest_id > 0)
				return IsUnlockQuestClear(_contentReleaseData.quest_id);
			return true;
		}

		[global::OpenPCR.Migration("160e5c8d-fc35-4910-b588-02235ded3c51")]
		public static bool IsUnlockQuestClear(int _questId)
		{
			if (_questId == 0)
				return true;
			UserData instance = Singleton<UserData>.Instance;
			if (instance.QuestMapParameterDictionary.TryGetValue(_questId, out var value))
				return (int)value.ClearFlg > 0;
			return instance.MainStoryArrivalNormalQuestId != -1 && instance.MainStoryArrivalNormalQuestId >= _questId;
		}
	}
}
