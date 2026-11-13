using Elements;

namespace OpenPCR.BattleInputs;

/// <summary>Battle-ready products; no raw account or calculator dependencies.</summary>
public sealed record PreparedUnit(UnitData Unique, UnitData Limited, PreparedUnitInputs UniqueInputs, PreparedUnitInputs LimitedInputs, int PrefabId, bool IsClanSupportOrReplay = false);
