using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Elements;
using Elements.Data;

namespace OpenPCR.Host;

public sealed partial class BattleSession
{
	/// <summary>Common battle output plus its distinct ordinary or EX settlement.</summary>
	public LabyrinthBattleResult ReadLabyrinthBattleResult()
	{
		RequireAvailable();
		if (labyrinthOwner is null) throw new InvalidOperationException("This is not a Labyrinth session.");
		var battle = ReadBattleResult();
		return new("openpcr-labyrinth-result/v1", battle,
			battle.Completed && !PreparedBattleResultOutput.IsLabyrinthExComplete(outputOwner) ? ReadLabyrinthSettlement() : null,
			battle.Completed && PreparedBattleResultOutput.IsLabyrinthExComplete(outputOwner) ? ReadLabyrinthExSettlement() : null);
	}
	/// <summary>Read one completed EX party, including optional final request and independently reusable enemy continuation.</summary>
	public LabyrinthExBattleSettlement ReadLabyrinthExSettlement()
	{
		RequireAvailable();
		if (!PreparedBattleResultOutput.IsLabyrinthExComplete(outputOwner) || labyrinthOwner is null)
			throw new InvalidOperationException("No completed Labyrinth EX party output.");
		var request = PreparedBattleResultOutput.ReadLabyrinthEx(outputOwner);
		LabyrinthExFinishRequest? finish = request is null ? null : new(request.QuestId,
			((TowerWaveResultInfo[])request.Waves).Select(PreparedLabyrinthExContext.Clone).ToArray(),
			((DungeonQueryUnit[])request.Enemies).Select(PreparedLabyrinthExContext.Clone).ToArray(), request.AutoClear);
		var data = labyrinthOwner;
		return new("openpcr-labyrinth-ex-settlement/v2", data.CurrentExPartyIndex, request is not null, finish,
			data.ExEnemyUnitList.Select(PreparedLabyrinthExContext.Clone).ToArray(), data.AlreadyDeadExEnemyList.Select(PreparedLabyrinthExContext.Clone).ToArray(),
			data.ExWaveResultInfoList.Select(PreparedLabyrinthExContext.Clone).ToArray(),
			data.UnitResultInfoDictionaryList.Select(row => (IReadOnlyDictionary<int, LabyrinthExUnitResult>)new ReadOnlyDictionary<int, LabyrinthExUnitResult>(row.ToDictionary(pair => pair.Key, pair => new LabyrinthExUnitResult(pair.Value.Hp, pair.Value.MaxHp, pair.Value.Energy)))).ToArray(),
			data.ExWaveClearTypeList.ToArray(), Singleton<TempData>.Instance.BattleStageData.CurrentContinuousUnit?.Select(row => row.Select(unit => new LabyrinthExContinuousUnit(unit.UnitId, unit.IdentifyNum, unit.Hp, unit.Energy, PreparedLabyrinthExContext.Clone(unit.SkillLimitCounter).ToArray())).ToArray()).ToArray());
	}
}

public sealed record LabyrinthBattleResult(string Schema, BattleResult Battle, LabyrinthBattleSettlement? Ordinary, LabyrinthExBattleSettlement? Ex);
