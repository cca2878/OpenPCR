using global::OpenPCR.Runtime.Impl;
using System.Text.Json.Serialization;

namespace OpenPCR.Host;

public sealed record BattleProgress(int EngineFrame, int BattleFrame, float TimeRemaining, string Result);
public sealed record BattleUnitResult(int UnitId, bool Enemy, long Hp, long MaxHp, long Energy);
public sealed record BattleSettlementHp(int UnitId, long ViewerId, long Hp);
public sealed record BattleSettlementDamage(int UnitId, long ViewerId, long Damage, int Rarity);
/// <summary>Clan or trial settlement read from its finish request. Boss HP and remaining time are in BattleResult.</summary>
/// <param name="ClanBattleId">Null for the clan-training finish protocol.</param>
public sealed record ClanBattleSettlement(int? ClanBattleId, int Phase, int OrderNum, long TotalDamage,
	BattleSettlementHp[] Hp, BattleSettlementDamage[] Damage)
{
	/// <summary>The original clan finish request (null for a trial), for replay comparisons.</summary>
	internal object? ClanRequest { get; init; }
	/// <summary>The finish request's battle log ID, for replay comparisons.</summary>
	internal int BattleLogId { get; init; }
}
/// <param name="FrameRate">Battle frame rate set when the stage is built; the client uploads it with the battle log.</param>
/// <param name="SystemId">The battle processor's system ID; the client uploads it with the battle log.</param>
public sealed record BattleResult(bool Completed, int EngineFrame, int BattleFrame, float TimeRemaining, string Result,
	BattleUnitResult[] Units, int FrameRate, int SystemId, string Csv, ScriptingRandom.State RandomState)
{
	/// <summary>Ordered, detached script diagnostics; null (and omitted from JSON) when no callback threw.</summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public BattleScriptException[]? ScriptExceptions { get; init; }
}
