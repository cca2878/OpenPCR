
namespace Elements.Battle
{
	public interface IBattleManagerForBattleLog
	{
		int CurrentWave { get; }

		global::System.Int32 FrameCount { get; }

		int FrameRate { get; }

		bool IsSpecialBattle { get; }

		int GetSystemId();

		int GetCurrentWaveOffset();
	}
}
