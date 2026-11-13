namespace Elements
{
	public class DungeonUtility
	{
		[global::OpenPCR.Migration("2da51c5e-3790-473e-b91c-0cb150b5e340")]
		public static bool IsNowSpecialMultiBoss()
		{
			throw new global::OpenPCR.OutOfBoundaryException("Elements.DungeonUtility::IsNowSpecialMultiBoss():System.Boolean: periphery input member; behavior is supplied by the wiring phase");
		}

		[global::OpenPCR.Migration("4d438e99-1a93-41b1-bf31-462eef97e93d")]
		public static global::System.Collections.Generic.List<Elements.Data.MasterDungeonSpecialEnemySetting.DungeonSpecialEnemySetting> GetNowSpecialBossBattleEnemySettingDatas()
		{
			throw new global::OpenPCR.OutOfBoundaryException("DungeonUtility.GetNowSpecialBossBattleEnemySettingDatas: outside guild/training data production");
		}

		[global::OpenPCR.Migration("19e1dd3b-6a9d-4f5a-a329-a8e1b29a1d3f")]
		public static Elements.Data.MasterWaveGroupData.WaveGroupData GetNowSpecialBossWaveGroupData()
		{
			throw new global::OpenPCR.OutOfBoundaryException("DungeonUtility.GetNowSpecialBossWaveGroupData: outside guild/training data production");
		}

		[global::OpenPCR.Migration("06d9dd6a-0cb8-436d-83ab-693717f045ec")]
		public static bool IsMustKillSpecialMultiBoss(int _enemyId)
		{
			throw new global::OpenPCR.OutOfBoundaryException("DungeonUtility.IsMustKillSpecialMultiBoss: outside guild/training data production");
		}
	}
}
