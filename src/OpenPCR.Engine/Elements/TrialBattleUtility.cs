using System.Collections.Generic;
namespace Elements
{
	public class TrialBattleUtility
	{

		[global::OpenPCR.Migration("9733ae43-74c1-439e-8a05-81a8bcff88ab")]
		public static bool IsBossInfinityHpMode(int _questId)
		{
			return ManagerSingleton<SaveDataManager>.Instance.TrialBattleInfinityHpActiveQuestIds.Contains(_questId);
		}
	}
}
