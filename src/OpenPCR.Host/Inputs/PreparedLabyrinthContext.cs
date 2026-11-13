using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Elements;
using Elements.Data;

namespace OpenPCR.Host;

/// <summary>Explicit pre-battle Labyrinth products. One invocation, no challenge history.</summary>
public sealed class PreparedLabyrinthContext
{
	public int QuestId { get; }
	public int EnterId { get; }
	public int BlockId { get; }
	public PreparedLabyrinthInputs Mode { get; }
	public IReadOnlyList<PreparedUnit> Units { get; }
	public IReadOnlyDictionary<eParamType, int> Reinforcement { get; }
	public IReadOnlyDictionary<eParamType, int> RelicPassive { get; }
	public PreparedLabyrinthExContext? Ex { get; }
	public IReadOnlyList<PreparedUnit>? AccountUnits { get; }
	public PreparedLabyrinthContext(int questId, int enterId, int blockId, PreparedLabyrinthInputs mode, IEnumerable<PreparedUnit> units,
		IReadOnlyDictionary<eParamType, int> reinforcement, IReadOnlyDictionary<eParamType, int> relicPassive, PreparedLabyrinthExContext? ex = null,
		IEnumerable<PreparedUnit>? accountUnits = null)
	{
		ArgumentNullException.ThrowIfNull(mode); ArgumentNullException.ThrowIfNull(units);
		ArgumentNullException.ThrowIfNull(reinforcement); ArgumentNullException.ThrowIfNull(relicPassive);
		// Entry and block IDs only reach the settlement request; v2 requests leave them 0.
		if (questId <= 0 || enterId < 0 || blockId < 0) throw new ArgumentException("A Labyrinth quest identity is required; entry and block IDs cannot be negative.");
		QuestId = questId; EnterId = enterId; BlockId = blockId; Mode = mode; Ex = ex;
		Units = Array.AsReadOnly(units.ToArray());
		AccountUnits = accountUnits is null ? null : Array.AsReadOnly(accountUnits.ToArray());
		if (AccountUnits is not null && (AccountUnits.Any(unit => unit is null) || AccountUnits.Select(unit => (int)unit.Unique.Id).Distinct().Count() != AccountUnits.Count))
			throw new InvalidDataException("Explicit Labyrinth account units must have unique identities.");
		if (Units.Count == 0 || Units.Any(value => value is null) || Units.Select(value => (int)value.Unique.Id).Distinct().Count() != Units.Count)
			throw new InvalidDataException("The Labyrinth unit context must be explicit and unique.");
		Reinforcement = new ReadOnlyDictionary<eParamType, int>(new Dictionary<eParamType, int>(reinforcement));
		RelicPassive = new ReadOnlyDictionary<eParamType, int>(new Dictionary<eParamType, int>(relicPassive));
	}
	internal LabyrinthTempData Bind(RuntimeMasterData master, TempData temp)
	{
		var data = (LabyrinthTempData)RuntimeHelpers.GetUninitializedObject(typeof(LabyrinthTempData));
		data.EnterId = EnterId; data.BlockId = BlockId; data.Difficulty = Mode.Difficulty;
		var account = Singleton<UserData>.Instance;
		var accountProducts = AccountUnits?.ToDictionary(value => (int)value.Unique.Id);
		var accountParameters = AccountUnits?.ToDictionary(value => (int)value.Unique.Id, value => value.CreateParameter(master));
		if (accountParameters is not null) RuntimeBinding.Set(account, "UnitParameterDictionary", accountParameters);
		data.UnitParameterDictionary = Units.ToDictionary(value => (int)value.Unique.Id, value =>
			accountProducts is not null && accountProducts.TryGetValue(value.Unique.Id, out var source) && ReferenceEquals(source, value)
				? accountParameters![(int)value.Unique.Id] : value.CreateParameter(master));
		PreparedLabyrinthInputs.Bind(data, Mode); PreparedLabyrinthInputs.Bind(master.Manager, Mode);
		temp.SelectedQuestId = QuestId;
		var enhance = (LabyrinthEnhanceParameter)RuntimeHelpers.GetUninitializedObject(typeof(LabyrinthEnhanceParameter));
		RuntimeBinding.Set(enhance, "labyrinthBuffParameterDictionary", new Dictionary<eParamType, int>(Reinforcement));
		RuntimeBinding.Set(enhance, "labyrinthRelicPassiveParameter", new Dictionary<eParamType, int>(RelicPassive));
		temp.BattleStageData.LabyrinthEnhanceParameter = enhance;
		Ex?.Bind(data, temp, account);
		return data;
	}
}

public sealed record LabyrinthBattleSettlement(string Schema, int QuestId, int RemainTime, int AutoClear, BattleSettlementHp[] Hp);
