using System;

namespace OpenPCR.Host;

public static class BattleRunner
{
	/// <summary>Run every battle of a v2 request: one battle, or each Labyrinth EX party in turn.</summary>
	public static BattleRunResult Run(BattleRequest request, BattleRunOptions options, Action<BattleProgress>? progress = null, BattleUpdateClock? clock = null)
		=> BattleSequence.Run(request, options, progress, clock);
	/// <summary>Run one battle from explicit prepared products.</summary>
	public static BattleResult Run(PreparedBattleInputs input, BattleRunOptions options, BattleExecutionInputs? execution = null, Action<BattleProgress>? progress = null, BattleUpdateClock? clock = null)
	{
		using var session = BattleSession.Create(input, options, execution, progress, clock: clock);
		session.RunToEnd(); return session.ReadBattleResult();
	}
}
