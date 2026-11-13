
namespace Elements.Battle
{
	public interface IBattleManagerForBattleVoiceUtility
	{
		BattleVoiceFlagManager BattleVoiceFlagMgr { get; }

		int PlayVoiceUnitId { get; set; }

		int PlayVoiceIndex { get; set; }

		bool PlayVoiceUnitIsRarity6 { get; set; }

		int PlayVoiceGroupId { get; set; }

		int PlayVoiceGroupUnitId { get; set; }

		bool IsDefenseReplayMode { get; set; }

		global::System.Int32 FrameCount { get; }
	}
}
