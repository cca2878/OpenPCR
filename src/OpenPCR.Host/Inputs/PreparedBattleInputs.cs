using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Elements;
using Elements.Data;
using OpenPCR.BattleInputs;

namespace OpenPCR.Host;

/// <summary>Complete pre-Awake scene and products. Mutable products transfer to exactly one session.</summary>
public sealed class PreparedBattleInputs
{
	private readonly BattleDefinition definition;
	public BattleDefinition Definition => definition.Copy();
	public PreparedBattleProducts Products { get; }
	public PreparedLabyrinthContext? Labyrinth { get; }
	public PreparedBattleInputs(BattleDefinition definition, PreparedBattleProducts products)
		: this(definition, products, null) { }
	public PreparedBattleInputs(BattleDefinition definition, PreparedBattleProducts products, PreparedLabyrinthContext? labyrinth)
	{
		ArgumentNullException.ThrowIfNull(definition); ArgumentNullException.ThrowIfNull(products);
		definition.Validate(labyrinth is not null); products.Validate(definition);
		if ((definition.Scene.Category is 59 or 60) != (labyrinth is not null) || (definition.Scene.Category == 60) != (labyrinth?.Ex is not null)) throw new InvalidDataException("Labyrinth context/category mismatch.");
		if (labyrinth?.Ex is { } ex && !definition.Party.SequenceEqual(ex.Parties[ex.PartyIndex])) throw new InvalidDataException("EX selected party differs from its explicit party index.");
		if (labyrinth?.Ex is { } enemyContext && !enemyContext.MatchesEnemyWave(definition.Scene.EnemyIds)) throw new InvalidDataException("EX enemy records differ from the explicit enemy-wave order.");
		if (labyrinth is not null && definition.Party.Where(id => id > 1).Except(labyrinth.Units.Select(value => (int)value.Unique.Id)).Any()) throw new InvalidDataException("Labyrinth context lacks a selected unit.");
		this.definition = definition.Copy(); Products = products; Labyrinth = labyrinth;
	}
	internal PreparedPartyInputs Consume(RuntimeMasterData master)
	{
		Products.Validate(definition, master);
		Products.Claim();
		return new(definition.Scene.Category == 60 ? (ePartyType)(132 + Labyrinth!.Ex!.PartyIndex) : definition.Scene.Category == 59 ? ePartyType.LABYRINTH : definition.Scene.Category == 9 ? ePartyType.CLAN_BATTLE : ePartyType.TRIAL_BATTLE, definition.Party.ToArray(),
			Products.Owned, Products.Support, definition.Support?.OwnerViewerId ?? 0,
			Products.Limited.ToDictionary(p => (p.UniqueId, p.Support), p => (p.Data, p.Inputs)), Products.Enemies, ResourceRequest());
	}
	private BattleResourceRequest ResourceRequest()
	{
		var players = Products.Support is null ? Products.Owned : Products.Owned.Append(Products.Support);
		int playerCount = Products.Owned.Count + (Products.Support is null ? 0 : 1);
		var requests = players.Concat(Products.Enemies).Select((unit, index) => new UnitResourceRequest(index < playerCount ? "player" : "enemy", index,
			unit.Unique.Id, unit.PrefabId, unit.Unique.UnionBurst.Concat(unit.Unique.MainSkill).Concat(unit.Unique.ExSkill).Concat(unit.Unique.FreeSkill)
				.Where(skill => (int)skill.SkillLevel > 0).Select(skill => (int)skill.SkillId).ToArray())).ToArray();
		return new(definition.MasterVersion, requests);
	}
}

/// <summary>A detached product bundle. After session creation callers must not mutate or reuse its Native objects.</summary>
public sealed class PreparedBattleProducts
{
	private int claimed;
	public string MasterVersion { get; }
	public IReadOnlyList<PreparedUnit> Owned { get; }
	public PreparedUnit? Support { get; }
	public IReadOnlyList<PreparedUnit> Enemies { get; }
	public IReadOnlyList<PreparedLimitedUnit> Limited { get; }
	public PreparedTeamGrowth Team { get; }
	public PreparedAccountGrowth Account { get; }
	public PreparedParameterProducts Parameters { get; }
	public PreparedBattleProducts(string masterVersion, IEnumerable<PreparedUnit> owned, PreparedUnit? support, IEnumerable<PreparedUnit> enemies,
		IEnumerable<PreparedLimitedUnit> limited, PreparedTeamGrowth team, PreparedAccountGrowth account, PreparedParameterProducts parameters)
	{
		ArgumentNullException.ThrowIfNull(owned); ArgumentNullException.ThrowIfNull(enemies); ArgumentNullException.ThrowIfNull(limited);
		ArgumentNullException.ThrowIfNull(team); ArgumentNullException.ThrowIfNull(account); ArgumentNullException.ThrowIfNull(parameters);
		MasterVersion = masterVersion; Owned = Array.AsReadOnly(owned.ToArray()); Support = support;
		Enemies = Array.AsReadOnly(enemies.ToArray()); Limited = Array.AsReadOnly(limited.ToArray()); Team = team; Account = account; Parameters = parameters;
	}
	internal void Claim()
	{
		if (Interlocked.CompareExchange(ref claimed, 1, 0) != 0) throw new InvalidOperationException("Prepared products have already transferred to a session; prepare fresh products for another session.");
	}
	internal void Validate(BattleDefinition definition, RuntimeMasterData? master = null)
	{
		if (Volatile.Read(ref claimed) != 0) throw new InvalidOperationException("Prepared products have already transferred to a session.");
		if (MasterVersion != definition.MasterVersion) throw new InvalidDataException("Prepared product/scene masterdata versions differ.");
		if (Owned.Any(p => p is null) || Enemies.Any(p => p is null) || Limited.Any(p => p is null)) throw new InvalidDataException("Prepared product entries cannot be null.");
		foreach (var product in Owned.Concat(Support is null ? Array.Empty<PreparedUnit>() : new[] { Support }).Concat(Enemies))
		{
			if (product.Unique is null || product.Limited is null || product.UniqueInputs is null || product.LimitedInputs is null || product.PrefabId <= 0 || (int)product.Unique.Id != (int)product.Limited.Id)
				throw new InvalidDataException("A prepared unit requires matching Unique/Limited identities, both independent input products and a prefab identity.");
		}
		if (!Owned.Select(p => (int)p.Unique.Id).SequenceEqual(definition.Party.Where(id => id > 1))) throw new InvalidDataException("Prepared owned units differ from explicit party order.");
		if (!Enemies.Select(p => (int)p.Unique.Id).SequenceEqual(definition.Scene.EnemyIds)) throw new InvalidDataException("Prepared enemy order differs from the scene.");
		if ((Support is null) != (definition.Support is null) || Support is not null && ((int)Support.Unique.Id != definition.Support!.UnitId || !Support.IsClanSupportOrReplay)) throw new InvalidDataException("Prepared support identity/flag differs from the scene.");
		var players = Support is null ? Owned : Owned.Append(Support);

		foreach (var team in new[] { Team.Unit, Team.Enemy })
		{
			if (team is null) continue;
			if (RuntimeBinding.Get(team, "talentLevelParameter") is not Dictionary<int, int> ||
				RuntimeBinding.Get(team, "knightEnhanceBuffParameter") is not Dictionary<int, Dictionary<eParamType, int>> knight || knight.Values.Any(value => value is null) ||
				RuntimeBinding.Get(team, "roleBuffParameter") is not Dictionary<int, Dictionary<UnitRoleDefine.eRoleParamType, int>> role || role.Values.Any(value => value is null))
				throw new InvalidDataException("A non-null prepared team requires all three complete enhancement dictionaries.");
		}
		var refreshes = new Dictionary<(int, bool), PreparedLimitedUnit>();
		foreach (var product in Limited)
		{
			if (product.Data is null || product.Inputs is null || product.UniqueId != (int)product.Data.Id || !refreshes.TryAdd((product.UniqueId, product.Support), product)) throw new InvalidDataException("Invalid/duplicate prepared Limited refresh product.");
		}
		var expected = new HashSet<(int, bool)>();
		foreach (var player in players)
		{
			bool support = ReferenceEquals(player, Support);
			if (!support && player.IsClanSupportOrReplay) throw new InvalidDataException("Owned scene units require the non-support Limited production flag.");
			var key = ((int)player.Unique.Id, support); expected.Add(key);
			if (!refreshes.ContainsKey(key)) throw new InvalidDataException("Missing prepared Limited refresh: " + key);
			if (master is null) continue;
			int converted = UnitUtility.GetConversionUnit(player.Unique.Id, player.Unique.UnitRarity);
			if (converted != (int)player.Unique.Id)
			{
				var convertedKey = (converted, support); expected.Add(convertedKey);
				if (!refreshes.ContainsKey(convertedKey)) throw new InvalidDataException("Missing converted Limited refresh: " + convertedKey);
			}
		}
		if (master is not null && refreshes.Keys.Except(expected).Any()) throw new InvalidDataException("Prepared Limited refresh has an unknown owner identity/flag.");
	}
}

public sealed record PreparedLimitedUnit(int UniqueId, bool Support, UnitData Data, PreparedUnitInputs Inputs);
internal sealed record PreparedPartyInputs(ePartyType PartyType, int[] Deck, IReadOnlyList<PreparedUnit> Owned, PreparedUnit? Support, long SupportOwner,
	IReadOnlyDictionary<(int Id, bool Support), (UnitData Limited, PreparedUnitInputs Inputs)> Limited, IReadOnlyList<PreparedUnit> Enemies, BattleResourceRequest Resources);
