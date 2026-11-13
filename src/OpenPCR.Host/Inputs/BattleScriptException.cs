namespace OpenPCR.Host;

/// <summary>A game-script exception that ended one Unity Update callback while the battle continued.</summary>
public sealed record BattleScriptException(int EngineFrame, int BattleFrame, float TimeRemaining,
	string Callback, string ExceptionType, string Message, string StackTrace);
