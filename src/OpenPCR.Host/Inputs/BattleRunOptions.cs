namespace OpenPCR.Host;

/// <summary>Deployment and host execution budget; separate from a scene definition and player operations.</summary>
public sealed record BattleRunOptions(string Resources, int MaxEngineFrames);
