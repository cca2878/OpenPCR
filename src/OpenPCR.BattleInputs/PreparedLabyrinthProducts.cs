using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Elements;

namespace OpenPCR.BattleInputs;

/// <summary>Detached Labyrinth mode products, produced before building a battle.</summary>
public sealed class PreparedLabyrinthProducts
{
	public PreparedLabyrinthInputs Runtime { get; }
	public IReadOnlyDictionary<eParamType, int> Reinforcement { get; }
	public IReadOnlyDictionary<eParamType, int> RelicPassive { get; }
	public PreparedLabyrinthProducts(PreparedLabyrinthInputs runtime, IReadOnlyDictionary<eParamType, int> reinforcement, IReadOnlyDictionary<eParamType, int> relicPassive)
	{
		ArgumentNullException.ThrowIfNull(runtime); ArgumentNullException.ThrowIfNull(reinforcement); ArgumentNullException.ThrowIfNull(relicPassive);
		Runtime = runtime;
		Reinforcement = new ReadOnlyDictionary<eParamType, int>(new Dictionary<eParamType, int>(reinforcement));
		RelicPassive = new ReadOnlyDictionary<eParamType, int>(new Dictionary<eParamType, int>(relicPassive));
	}
}
