using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenPCR.Host;

/// <summary>
/// Battle request <c>openpcr-battle/v2</c>: the player side, device settings, the battle sequence and exactly one mode
/// section. Server-shaped objects ignore undeclared fields; OpenPCR-defined objects reject unknown fields.
/// </summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record BattleRequest(
	[property: JsonRequired] string Schema,
	[property: JsonRequired] string MasterVersion,
	[property: JsonRequired] BattlePlayerInput Player,
	[property: JsonRequired] BattleDeviceSettings Settings,
	[property: JsonRequired] BattleEntryInput[] Battles,
	ClanBattleInput? ClanBattle = null,
	LabyrinthInput? Labyrinth = null)
{
	public const string CurrentSchema = "openpcr-battle/v2";
	public static BattleRequest Read(string path) => Parse(File.ReadAllText(path));
	public static BattleRequest Parse(string json)
	{
		using var document = JsonDocument.Parse(json);
		JsonInputValidation.RejectDuplicateProperties(document.RootElement);
		var result = document.RootElement.Deserialize<BattleRequest>(JsonOptions()) ?? throw new InvalidDataException("Battle request is null.");
		result.Validate();
		return result;
	}
	public string ToJson() { Validate(); return JsonSerializer.Serialize(this, JsonOptions()); }
	/// <summary>One shared instance: the serializer builds its converters per options object, and locks it on first use.</summary>
	internal static JsonSerializerOptions JsonOptions() => jsonOptions;
	private static readonly JsonSerializerOptions jsonOptions = new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Converters = { new JsonStringEnumConverter() }
	};
	/// <summary>Structural checks that need no resources; masterdata-dependent checks run while planning.</summary>
	internal void Validate()
	{
		if (Schema != CurrentSchema) throw new InvalidDataException("Unsupported battle request schema: " + Schema);
		if (string.IsNullOrEmpty(MasterVersion) || MasterVersion.Length != 12 || MasterVersion.Any(c => c is < '0' or > '9')) throw new InvalidDataException("MasterVersion must be twelve digits.");
		if (Player is null || Settings is null || Battles is null || Battles.Any(battle => battle is null)) throw new InvalidDataException("Player, Settings and Battles are required.");
		if ((ClanBattle is null) == (Labyrinth is null)) throw new InvalidDataException("Exactly one mode section (ClanBattle or Labyrinth) is required.");
		Player.Validate();
		Settings.Validate();
		foreach (var battle in Battles) battle.Validate();
		if (ClanBattle is { } clan)
		{
			clan.Validate();
			if (Battles.Length != 1) throw new InvalidDataException("ClanBattle describes exactly one battle.");
			ValidateParty(Battles[0], "Battles[0]", allowEmpty: false, allowSupport: clan.Support is not null, Player.UnitList.Select(unit => unit.Id));
			if (Battles[0].Party.Contains(1) != (clan.Support is not null)) throw new InvalidDataException("A support slot (1) and ClanBattle.support are supplied together.");
		}
		if (Labyrinth is { } labyrinth)
		{
			labyrinth.Validate(Player);
			if (Battles.Length is < 1 or > 3) throw new InvalidDataException("Labyrinth describes one battle, or up to three EX parties.");
			for (int i = 0; i < Battles.Length; i++)
				ValidateParty(Battles[i], "Battles[" + i + "]", allowEmpty: i > 0, allowSupport: false, labyrinth.UnitList.Select(unit => unit.UnitId));
		}
		foreach (var battle in Battles.Where(battle => battle.OperationList.Length != 0))
			if (Labyrinth is not null) throw new NotSupportedException("Labyrinth battles do not support in-battle operations.");
	}
	/// <summary>Party slots use <paramref name="available"/> units: Player units for ClanBattle, Labyrinth.unit_list for Labyrinth.</summary>
	private static void ValidateParty(BattleEntryInput battle, string name, bool allowEmpty, bool allowSupport, IEnumerable<int> available)
	{
		var party = battle.Party;
		if (party.Length != 5 || party.Any(id => id < 0) || !allowSupport && party.Contains(1))
			throw new InvalidDataException(name + ".Party requires five slots; 0 is empty" + (allowSupport ? " and 1 selects the support." : "."));
		var units = party.Where(id => id != 0).ToArray();
		if (units.Length == 0 && !allowEmpty) throw new InvalidDataException(name + ".Party requires at least one unit.");
		if (units.Distinct().Count() != units.Length) throw new InvalidDataException(name + ".Party repeats a unit.");
		var supplied = available.ToHashSet();
		foreach (int id in units.Where(id => id > 1))
			if (!supplied.Contains(id)) throw new InvalidDataException(name + ".Party unit is not supplied: " + id);
		var opening = battle.Opening;
		if (units.Length == 0 && opening.SetUnits.Length != 0) throw new InvalidDataException(name + " is empty and cannot set units.");
		if (opening.SetUnits.Distinct().Count() != opening.SetUnits.Length || opening.SetUnits.Except(units).Any())
			throw new InvalidDataException(name + ".Opening.SetUnits must be distinct units of the same party.");
	}
}

/// <summary>The player side. Server-shaped: undeclared fields are ignored.</summary>
public sealed record BattlePlayerInput(
	[property: JsonPropertyName("user_info"), JsonRequired] WireUserInfo UserInfo,
	[property: JsonPropertyName("unit_list"), JsonRequired] WireUnit[] UnitList,
	[property: JsonPropertyName("read_story_ids"), JsonRequired] int[] ReadStoryIds,
	[property: JsonPropertyName("unit_role_list"), JsonRequired] WireUnitRole[] UnitRoleList,
	[property: JsonPropertyName("ini_setting"), JsonRequired] WireIniSetting IniSetting,
	[property: JsonPropertyName("errm"), JsonRequired] int Errm,
	[property: JsonRequired] BattleReleases Releases,
	[property: JsonPropertyName("princess_knight_info")] WirePrincessKnight? PrincessKnightInfo = null,
	[property: JsonPropertyName("user_ex_equip")] WireExEquipment[]? UserExEquip = null)
{
	internal void Validate()
	{
		if (UserInfo is null || UnitList is null || ReadStoryIds is null || UnitRoleList is null || IniSetting is null || Releases is null)
			throw new InvalidDataException("Player requires user_info, unit_list, read_story_ids, unit_role_list, ini_setting and Releases.");
		if (UserInfo.TeamLevel <= 0) throw new InvalidDataException("user_info.team_level must be positive.");
		if (UnitList.Any(unit => unit is null) || UnitList.Select(unit => unit.Id).Distinct().Count() != UnitList.Length) throw new InvalidDataException("unit_list entries must be unique units.");
		var serials = (UserExEquip ?? Array.Empty<WireExEquipment>()).Select(item => item.SerialId).ToArray();
		if (serials.Distinct().Count() != serials.Length) throw new InvalidDataException("user_ex_equip serials must be unique.");
		if (UnitRoleList.Select(role => role.UnitRoleId).Distinct().Count() != UnitRoleList.Length) throw new InvalidDataException("unit_role_list roles must be unique.");
		if (IniSetting.BattleLogType is null || IniSetting.LogBarrier is null) throw new InvalidDataException("ini_setting requires battle_log_type and log_barrier.");
	}
}

/// <summary>
/// Release switches that the game derives from server values. Callers derive them from the server responses:
/// see the Labyrinth and battle-input documentation for each rule.
/// </summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record BattleReleases(
	[property: JsonRequired] bool ChangeRarity,
	[property: JsonRequired] bool SkillLevelSync,
	[property: JsonRequired] bool SlipDamageLogBarrier,
	[property: JsonRequired] bool KnightEnhance);

/// <summary>Device-local settings shared by every battle of the request.</summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record BattleDeviceSettings(
	[property: JsonRequired] int FrameRate,
	[property: JsonRequired] bool ApplyOtherUserSkins,
	[property: JsonRequired] bool ResetSkillReserveEveryTime,
	[property: JsonRequired] bool AdvantageAndBreakdownBonusShows,
	[property: JsonRequired] bool MultiTargetFirst,
	[property: JsonRequired] bool TalentFormationBonusFirst,
	bool PlayMultiTargetAppearance = false)
{
	internal void Validate()
	{
		if (FrameRate is not (30 or 60)) throw new NotSupportedException("Settings.FrameRate must be 30 or 60.");
	}
}

/// <summary>One battle of the sequence.</summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record BattleEntryInput(
	[property: JsonRequired] int[] Party,
	[property: JsonRequired] BattleOpening Opening,
	[property: JsonRequired] int Seed,
	PlayerOperation[]? Operations = null,
	JsonElement? Opponent = null)
{
	internal PlayerOperation[] OperationList => Operations ?? Array.Empty<PlayerOperation>();
	internal void Validate()
	{
		if (Party is null || Opening is null || OperationList.Any(operation => operation is null)) throw new InvalidDataException("A battle requires Party and Opening.");
		if (Opponent is { ValueKind: not JsonValueKind.Null }) throw new NotSupportedException("Opponent is reserved for arena modes.");
		Opening.Validate();
	}
}

/// <summary>Starting AUTO, speed (1, 2 or 4) and SET units of one battle. The UB SET switch (a UI convenience) is always on.</summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record BattleOpening([property: JsonRequired] bool Auto, [property: JsonRequired] int Speed, [property: JsonRequired] int[] SetUnits)
{
	internal void Validate()
	{
		if (SetUnits is null) throw new InvalidDataException("Opening.SetUnits is required.");
		if (Speed is not (1 or 2 or 4)) throw new InvalidDataException("Opening.Speed must be 1, 2 or 4.");
	}
}

/// <summary>Clan battle boss scene: category 9 (<c>clan</c>) or the training-ground clan mode, category 32 (<c>trial</c>).</summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ClanBattleInput(
	[property: JsonPropertyName("clan_battle_id"), JsonRequired] int ClanBattleId,
	[property: JsonPropertyName("phase"), JsonRequired] int Phase,
	[property: JsonPropertyName("order_num"), JsonRequired] int OrderNum,
	[property: JsonPropertyName("kind"), JsonRequired] string Kind,
	[property: JsonPropertyName("lap_num")] int? LapNum = null,
	[property: JsonPropertyName("boss_hp")] ClanBossHpInput? BossHp = null,
	[property: JsonPropertyName("quad_speed")] bool QuadSpeed = false,
	[property: JsonPropertyName("infinity_hp")] bool InfinityHp = false,
	[property: JsonPropertyName("support")] ClanSupportInput? Support = null)
{
	internal bool IsTrial => Kind == "trial";
	internal void Validate()
	{
		if (Kind is not ("clan" or "trial")) throw new InvalidDataException("ClanBattle.kind must be clan or trial.");
		if (ClanBattleId <= 0 || Phase <= 0 || OrderNum is < 1 or > 5 || LapNum is <= 0) throw new InvalidDataException("ClanBattle requires a positive clan_battle_id, phase, lap_num and order_num 1-5.");
		if (IsTrial && (BossHp is not null || QuadSpeed || LapNum is not null)) throw new InvalidDataException("ClanBattle trial takes neither lap_num, boss_hp nor quad_speed.");
		if (!IsTrial && (InfinityHp || Support is not null)) throw new NotSupportedException("ClanBattle clan takes neither infinity_hp nor support (the clan support producer is unavailable).");
		if (BossHp is { } hp && (hp.MaxHp <= 0 || hp.CurrentHp <= 0 || hp.CurrentHp > hp.MaxHp)) throw new InvalidDataException("ClanBattle boss_hp requires 0 < current_hp <= max_hp.");
		if (Support is { } support && (support.UnitData is null || support.UnitData.Id != support.UnitId || support.UnitId <= 1)) throw new InvalidDataException("ClanBattle.support unit_data must match unit_id.");
	}
}

/// <summary>Current boss HP state of a clan battle.</summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record ClanBossHpInput([property: JsonPropertyName("max_hp"), JsonRequired] long MaxHp, [property: JsonPropertyName("current_hp"), JsonRequired] long CurrentHp);

/// <summary>Selected training-ground support.</summary>
public sealed record ClanSupportInput([property: JsonPropertyName("unit_id"), JsonRequired] int UnitId, [property: JsonPropertyName("unit_data"), JsonRequired] WireSupportUnit UnitData);

/// <summary>Labyrinth context, as in <c>labyrinth/top</c> and <c>labyrinth/resume</c>. Undeclared fields are ignored.</summary>
public sealed record LabyrinthInput(
	[property: JsonPropertyName("quest_id"), JsonRequired] int QuestId,
	[property: JsonPropertyName("difficulty"), JsonRequired] int Difficulty,
	[property: JsonPropertyName("alpha_rupee"), JsonRequired] int AlphaRupee,
	[property: JsonPropertyName("unit_list"), JsonRequired] LabyrinthUnitInput[] UnitList,
	[property: JsonPropertyName("relic_list"), JsonRequired] LabyrinthRelicInput[] RelicList,
	[property: JsonPropertyName("labyrinth_enhance_node_list"), JsonRequired] int[] LabyrinthEnhanceNodeList,
	[property: JsonPropertyName("start_state")] LabyrinthExState? StartState = null)
{
	internal void Validate(BattlePlayerInput player)
	{
		if (UnitList is null || RelicList is null || LabyrinthEnhanceNodeList is null || UnitList.Any(unit => unit is null) || RelicList.Any(relic => relic is null))
			throw new InvalidDataException("Labyrinth requires unit_list, relic_list and labyrinth_enhance_node_list.");
		if (UnitList.Select(unit => unit.UnitId).Distinct().Count() != UnitList.Length) throw new InvalidDataException("Labyrinth unit_list entries must be unique.");
		foreach (var unit in UnitList)
		{
			if (unit.UnitType == 1 && !player.UnitList.Any(owned => owned.Id == unit.UnitId)) throw new InvalidDataException("Labyrinth USER unit is not in Player.unit_list: " + unit.UnitId);
			if (unit.UnitType == 2 && (unit.UnitData is null || unit.UnitData.Id != unit.UnitId)) throw new InvalidDataException("Labyrinth NPC requires unit_data with the same id: " + unit.UnitId);
			if (unit.UnitType is not (1 or 2)) throw new InvalidDataException("Labyrinth unit_type must be 1 (USER) or 2 (NPC).");
		}
	}
}

/// <summary>Labyrinth owned unit; NPC entries (unit_type 2) carry their server unit_data.</summary>
public sealed record LabyrinthUnitInput(
	[property: JsonPropertyName("unit_id"), JsonRequired] int UnitId,
	[property: JsonPropertyName("unit_type"), JsonRequired] int UnitType,
	[property: JsonPropertyName("unit_data")] WireUnit? UnitData = null);

/// <summary>Labyrinth relic, in server order.</summary>
public sealed record LabyrinthRelicInput(
	[property: JsonPropertyName("relic_id"), JsonRequired] int RelicId,
	[property: JsonPropertyName("effected_count"), JsonRequired] int EffectedCount,
	[property: JsonPropertyName("enable"), JsonRequired] int Enable);
