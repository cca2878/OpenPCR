using System.Text.Json.Serialization;

namespace OpenPCR.Host;

/// <summary>Exact-match frame counters for player operations, read before an enabled Update.</summary>
public enum PlayerOperationAnchor { BattleFrame, BattleClock }

/// <summary>Ordered external AUTO, SET or manual UB input. UnitId is the converted battle identity.</summary>
public sealed record PlayerOperation([property: JsonRequired] PlayerOperationAnchor Anchor,
	[property: JsonRequired] int At, [property: JsonRequired] string Kind, int UnitId = 0, bool Value = false);

/// <summary>A detached view of editable future operations; opening AUTO/SET controls remain in effect.</summary>
public sealed record PlayerInputState(float StartTime, int LastAppliedBattleFrame, int LastAppliedBattleClock,
	int MinimumBattleFrame, int MinimumBattleClock, int[] PlayerUnitIds, PlayerOperation[] Operations);
