namespace OpenPCR.Host;

/// <summary>Native result-producer observations. UpdateIndex counts session updates, not Unity frameCount.</summary>
public sealed record BattleTerminalPoint(int UpdateIndex, int BattleFrame, int Wave, int GameState, int Result, int MiliLimitTime);
public sealed record BattleTerminalEvent(string Kind, BattleTerminalPoint Point);
public sealed record BattleTerminalTrace(string Schema, bool Completed, BattleTerminalEvent[] Events);
