using global::OpenPCR.Runtime.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using Elements;
using OpenPCR.Growth;

namespace OpenPCR.Host;

/// <summary>Raw growth preparation, completed before Native Awake/Init. Every invocation returns fresh products.</summary>
public static class BattlePreparer
{
	/// <summary>Prepare a v2 clan or training-ground battle; returns the products and the starting execution inputs.</summary>
	public static (PreparedBattleInputs Inputs, BattleExecutionInputs Execution) Prepare(BattleRequest request, string resources)
	{
		ArgumentNullException.ThrowIfNull(request); request.Validate();
		if (request.ClanBattle is null) throw new System.IO.InvalidDataException("Not a clan battle request.");
		var random = ScriptingRandom.Current;
		try
		{
			using var database = new ResourceDatabases(resources, request.MasterVersion);
			var plan = BattlePlanner.PlanClan(request, new GrowthMasterData(database.Master));
			return (Prepare(plan, database), plan.Execution);
		}
		finally { ScriptingRandom.Current = random; }
	}
	internal static PreparedBattleInputs Prepare(PlannedBattle plan, ResourceDatabases resources)
	{
		var definition = plan.Definition;
		definition.Validate();
		using var calculator = new GrowthCalculator(plan.Growth, resources.Master, resources.MasterVersion);
		var owned = new List<OpenPCR.BattleInputs.PreparedUnit>(); OpenPCR.BattleInputs.PreparedUnit? support = null;
		var limited = new List<PreparedLimitedUnit>();
		void AddLimited(OpenPCR.BattleInputs.PreparedUnit product, bool isSupport)
		{
			limited.Add(new(product.Unique.Id, isSupport, product.Limited, product.LimitedInputs));
			if (calculator.CalculateConvertedLimited(isSupport ? product.Unique : product.Limited, isSupport) is { } converted)
				limited.Add(new(converted.Limited.Id, isSupport, converted.Limited, converted.Inputs));
		}
		foreach (int id in definition.Party)
		{
			if (id == 0) continue;
			if (id == 1) { support = calculator.CalculateSupport(definition.Support!.UnitId); AddLimited(support, true); }
			else { var product = calculator.CalculateUnit(id); owned.Add(product); AddLimited(product, false); }
		}
		var enemies = definition.Scene.EnemyIds.Select(calculator.CalculateEnemy).ToArray();
		var parameters = calculator.PrepareParameters(definition.Scene.EnemyIds);
		var account = calculator.PrepareAccountGrowth();
		var team = new OpenPCR.BattleInputs.PreparedTeamGrowth(calculator.CalculateTeam(), calculator.CalculateTeam(true));
		var products = new PreparedBattleProducts(definition.MasterVersion, owned, support, enemies, limited, team, account, parameters);
		return new(definition, products);
	}
}
