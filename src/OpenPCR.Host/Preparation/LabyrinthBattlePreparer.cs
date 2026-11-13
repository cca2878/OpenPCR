using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenPCR.Growth;
using OpenPCR.Runtime.Impl;

namespace OpenPCR.Host;

/// <summary>Products and starting execution inputs for one Labyrinth battle (category 59, or one EX party).</summary>
/// <param name="State">For EX, the challenge state this party starts from; null for category 59.</param>
public sealed record PreparedLabyrinthBattle(PreparedBattleInputs Inputs, BattleExecutionInputs Execution, LabyrinthExState? State);

/// <summary>Raw account and received NPC sources become detached products before Native battle ownership.</summary>
public static class LabyrinthBattlePreparer
{
	/// <summary>Prepare category 59, or the first EX party to run (Labyrinth.start_state, or party 0 from the quest).</summary>
	public static PreparedLabyrinthBattle Prepare(BattleRequest request, string resources)
	{
		ArgumentNullException.ThrowIfNull(request);
		return Prepare(request, resources, request.Labyrinth?.StartState);
	}

	/// <summary>Prepare the v2 Labyrinth battle that <paramref name="state"/> starts (null for category 59 or EX party 0).</summary>
	internal static PreparedLabyrinthBattle Prepare(BattleRequest request, string resources, LabyrinthExState? state)
	{
		ArgumentNullException.ThrowIfNull(request); request.Validate();
		var context = request.Labyrinth ?? throw new InvalidDataException("Not a Labyrinth request.");
		var random = ScriptingRandom.Current;
		try
		{
			using var database = new ResourceDatabases(resources, request.MasterVersion);
			var masterRows = new GrowthMasterData(database.Master);
			var stage = LabyrinthStage.Read(masterRows, context.QuestId, context.Difficulty);
			bool isEx = stage.Category == 60;
			if (!isEx && request.Battles.Length != 1) throw new InvalidDataException("Quest " + context.QuestId + " is not an EX quest; describe exactly one battle.");
			int index = 0; PreparedLabyrinthExContext? ex = null;
			if (isEx)
			{
				state ??= LabyrinthExState.Initial(masterRows, context.QuestId);
				index = state.PartyIndex;
				if (index >= request.Battles.Length || request.Battles[index].Party.All(id => id == 0)) throw new InvalidDataException("EX party " + index + " is empty or not described.");
				var parties = Enumerable.Range(0, 3).Select(i => i < request.Battles.Length ? request.Battles[i].Party.ToArray() : new int[5]);
				ex = state.Prepare(parties);
				if (!ex.MatchesEnemyWave(stage.EnemyIds)) throw new InvalidDataException("EX state enemy records differ from the quest wave.");
			}
			else if (state is not null) throw new InvalidDataException("A start state applies only to EX quests.");
			var battle = request.Battles[index];
			var scene = new BattleScene(stage.Category, stage.EnemyIds.ToArray(), stage.TimeLimit, 0, battle.Seed, stage.Backgrounds.ToArray(), null, null);
			var definition = new BattleDefinition(request.MasterVersion, battle.Party.ToArray(), null, scene, BattlePlanner.Account(request.Player, masterRows),
				BattlePlanner.Settings(request.Settings, battle.Opening, bossInfinityHp: false, clanRehearsalMaxSpeed: null), BattlePlanner.Logging(request.Player));
			definition.Validate(preparedLabyrinth: true);
			var growth = BattlePlanner.Growth(request.MasterVersion, request.Player, null);
			using var calculator = new GrowthCalculator(growth, database.Master, request.MasterVersion);
			var account = growth.Units.Keys.ToDictionary(id => id, id => calculator.CalculateUnit(id));
			var units = context.UnitList.ToDictionary(source => source.UnitId,
				source => source.UnitType == 1 ? account[source.UnitId] : calculator.CalculateLabyrinthNpc(BattlePlanner.Unit(source.UnitData!), BattlePlanner.Bonus(source.UnitData!.BonusParam)));
			var owned = battle.Party.Where(id => id > 1).Select(id => units[id]).ToArray();
			var npcs = context.UnitList.Where(source => source.UnitType != 1).Select(source => source.UnitId).ToHashSet();
			var limited = new List<PreparedLimitedUnit>();
			foreach (var unit in owned)
			{
				limited.Add(new(unit.Unique.Id, false, unit.Limited, unit.LimitedInputs));
				var converted = npcs.Contains(unit.Unique.Id) ? calculator.CalculateConvertedLabyrinthNpc(unit.Unique) : calculator.CalculateConvertedLimited(unit.Unique, false);
				if (converted is { } product) limited.Add(new(product.Limited.Id, false, product.Limited, product.Inputs));
			}
			var mode = calculator.PrepareLabyrinthMode(new LabyrinthGrowthContext(context.Difficulty, context.AlphaRupee,
				context.UnitList.Select(unit => new LabyrinthOwnedUnit(unit.UnitId, unit.UnitType)),
				context.RelicList.Select(relic => new LabyrinthOwnedRelic(relic.RelicId, relic.EffectedCount, relic.Enable)), context.LabyrinthEnhanceNodeList));
			var products = new PreparedBattleProducts(request.MasterVersion, owned, null, stage.EnemyIds.Select(calculator.CalculateLabyrinthEnemy), limited,
				new(calculator.CalculateTeam(), calculator.CalculateTeam(true)), calculator.PrepareAccountGrowth(),
				calculator.PrepareLabyrinthParameters(stage.Category, context.QuestId, context.Difficulty));
			var labyrinth = new PreparedLabyrinthContext(context.QuestId, 0, 0, mode.Runtime, units.Values, mode.Reinforcement, mode.RelicPassive, ex, account.Values);
			return new(new(definition, products, labyrinth), BattlePlanner.Execution(battle), isEx ? state : null);
		}
		finally { ScriptingRandom.Current = random; }
	}
}
