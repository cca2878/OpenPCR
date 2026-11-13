namespace Elements
{
	public class HatsuneUtility
	{
		[global::OpenPCR.Migration("acd8bfc8-75a3-4e38-bcbe-731266f33031")]
		public static global::Elements.Data.MasterHatsuneBossEnemySetting.HatsuneBossEnemySetting GetNowEventBossEnemySettingForEnemyId(int _enemyId)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.HatsuneUtility::GetNowEventBossEnemySettingForEnemyId(System.Int32):Elements.Data.MasterHatsuneBossEnemySetting+HatsuneBossEnemySetting: periphery input member; behavior is supplied by the wiring phase");
		}

		[global::OpenPCR.Migration("14c867d9-9416-4f2b-94b1-15b1a3d9fdeb")]
		public static bool IsMultiBossDestructionTarget(int _enemyId)
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.HatsuneUtility::IsMultiBossDestructionTarget(System.Int32):System.Boolean: periphery input member; behavior is supplied by the wiring phase");
		}

		[global::OpenPCR.Migration("33667528-e27e-4885-aa6f-7a9244c433c3")]
		public static bool IsNowEventMultiBoss()
		{
			if (!IsMultiTargetBossRelease()) return false;
			var settings = GetNowBossBattleEnemySettingDatas();
			return settings != null && settings.Count > 1;
		}

		public static bool IsMultiTargetBossRelease()
		{
			UserData user = Singleton<UserData>.Instance;
			var eventType = Singleton<Elements.Data.TempData>.Instance.HatsuneTemp.SelectedEventType;
			if (eventType == HatsuneDefine.eHatsuneEventType.Hatsune ||
				eventType == HatsuneDefine.eHatsuneEventType.Revival)
				return true;
			if (eventType == HatsuneDefine.eHatsuneEventType.Shiori &&
				user.ShioriMultiTargetBossReleaseUnixTime >= 1L)
				return user.NowUnixTime >= user.ShioriMultiTargetBossReleaseUnixTime;
			return false;
		}

		[global::OpenPCR.Migration("5dccd530-d6f2-414a-98c0-98dfebbcf788")]
		public static global::System.Collections.Generic.List<Elements.Data.MasterHatsuneBossEnemySetting.HatsuneBossEnemySetting> GetNowBossBattleEnemySettingDatas()
		{
			throw new global::OpenPCR.OutOfBoundaryException("HatsuneUtility.GetNowBossBattleEnemySettingDatas: outside guild/training data production");
		}
	}
}
