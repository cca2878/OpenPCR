namespace OpenPCR.Host;

/// <summary>A complete frame boundary immediately before a validated player UB opportunity.</summary>
public sealed record UbDecision(long Id, int TriggerEngineFrame, int[] UnitIds);

/// <summary>Search diagnostics are session metadata and do not rewind with battle state.</summary>
public sealed record UbExplorationStatus(bool Enabled, UbDecision? Decision, BattleProgress Progress,
	bool Completed, bool Exhausted, long AutomaticCaptures, long ProbedEngineFrames, long ReplayedEngineFrames);
