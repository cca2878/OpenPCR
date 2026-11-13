using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenPCR.Growth;

public sealed record LabyrinthOwnedUnit(int UnitId, int UnitType);
public sealed record LabyrinthOwnedRelic(int RelicId, int EffectedCount, int Enable);

/// <summary>Complete explicit gameplay ownership and inventory context for one battle.</summary>
public sealed class LabyrinthGrowthContext
{
	public int Difficulty { get; }
	public int AlphaRupee { get; }
	public IReadOnlyList<LabyrinthOwnedUnit> OwnedUnits { get; }
	public IReadOnlyList<LabyrinthOwnedRelic> Relics { get; }
	public IReadOnlyList<int> ReinforcementNodes { get; }
	public LabyrinthGrowthContext(int difficulty, int alphaRupee, IEnumerable<LabyrinthOwnedUnit> ownedUnits, IEnumerable<LabyrinthOwnedRelic> relics, IEnumerable<int> reinforcementNodes)
	{
		ArgumentNullException.ThrowIfNull(ownedUnits); ArgumentNullException.ThrowIfNull(relics); ArgumentNullException.ThrowIfNull(reinforcementNodes);
		Difficulty = difficulty; AlphaRupee = alphaRupee;
		OwnedUnits = Array.AsReadOnly(ownedUnits.ToArray()); Relics = Array.AsReadOnly(relics.ToArray()); ReinforcementNodes = Array.AsReadOnly(reinforcementNodes.ToArray());
		if (difficulty <= 0 || OwnedUnits.Any(unit => unit is null || unit.UnitId <= 0) || Relics.Any(relic => relic is null || relic.RelicId <= 0) || ReinforcementNodes.Any(id => id <= 0))
			throw new InvalidDataException("Invalid Labyrinth difficulty or explicit ownership context.");
	}
}
