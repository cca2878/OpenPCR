using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenPCR.Host;

namespace OpenPCR.BattleEditor;

/// <summary>An openpcr-battle/v2 request being edited.</summary>
internal sealed class SceneDocument
{
	private readonly JsonObject document;
	private readonly JsonObject original;
	internal bool IsLabyrinth => document["Labyrinth"] is JsonObject;
	internal JsonObject Root => document;
	private SceneDocument(string json)
	{
		document = JsonNode.Parse(BattleRequest.Parse(json).ToJson())!.AsObject();
		original = (JsonObject)document.DeepClone();
	}
	internal static SceneDocument Read(string path) => new(File.ReadAllText(path));
	internal static SceneDocument FromRequest(BattleRequest request) => new(request.ToJson());
	internal static SceneDocument Preset(string name)
	{
		if (name is not ("trial-basic" or "clan-rehearsal-basic" or "labyrinth-basic")) throw new ArgumentException("Preset: trial-basic, clan-rehearsal-basic or labyrinth-basic.");
		using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name + ".json") ?? throw new InvalidDataException("Missing embedded preset.");
		using var reader = new StreamReader(stream); return new(reader.ReadToEnd());
	}
	internal JsonObject Player => document["Player"]!.AsObject();
	internal JsonArray Units => Player["unit_list"]!.AsArray();
	internal string Version => document["MasterVersion"]!.GetValue<string>();
	private JsonArray Battles => document["Battles"]!.AsArray();
	private JsonObject Battle(int? index)
	{
		int value = index ?? 0;
		if (value < 0 || value >= Battles.Count) throw new ArgumentException("Battle index " + value + " is not described; Battles has " + Battles.Count + ".");
		return Battles[value]!.AsObject();
	}
	private JsonObject? Unit(int id) => Units.Select(node => node!.AsObject()).FirstOrDefault(unit => unit["id"]!.GetValue<int>() == id);
	internal bool RemoveUnit(int id)
	{
		var unit = Unit(id);
		if (unit is null) return false;
		Units.Remove(unit); return true;
	}
	internal static int[] Integers(string text) => text == "none" ? Array.Empty<int>() : text.Split(',').Select(value => int.Parse(value, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
	private static JsonArray Slots(int[] ids)
	{
		if (ids.Length > 5) throw new ArgumentException("A party has at most five slots.");
		return new JsonArray(ids.Concat(Enumerable.Repeat(0, 5 - ids.Length)).Select(id => (JsonNode)id).ToArray());
	}
	internal void Party(string text, int? battle = null)
	{
		int[] ids = Integers(text);
		if (ids.Length is < 1 or > 5) throw new ArgumentException("Party requires 1..5 comma-separated slots; 0 is empty, 1 selects the training-ground support.");
		if (IsLabyrinth && ids.Contains(1)) throw new ArgumentException("Labyrinth parties use declared USER/NPC IDs; the support marker 1 is unavailable.");
		Battle(battle)["Party"] = Slots(ids);
		if (!IsLabyrinth && !ids.Contains(1)) document["ClanBattle"]!.AsObject().Remove("support");
	}
	private static readonly string[] Removed = { "enemies", "background", "seconds", "battle-log-id", "now", "skill-sync-at", "enter-id", "block-id", "owned-units", "party", "ex", "ex-log-ids" };
	private static void RejectRemoved(Arguments args)
	{
		foreach (string option in Removed)
			if (args.Take(option) is not null)
				throw new ArgumentException("--" + option + " is not part of openpcr-battle/v2: scenes come from the clan battle or quest IDs, release switches are booleans, and pass-through IDs are not inputs.");
	}
	internal void EditScene(Arguments args)
	{
		RejectRemoved(args);
		int? battle = args.Integer("battle");
		if (args.Integer("seed") is { } seed) Battle(battle)["Seed"] = seed;
		if (args.Boolean("auto") is { } auto) Battle(battle)["Opening"]!["Auto"] = auto;
		if (args.Integer("speed") is { } speed) Battle(battle)["Opening"]!["Speed"] = speed;
		if (args.Take("set-units") is { } set) Battle(battle)["Opening"]!["SetUnits"] = new JsonArray(Integers(set).Select(id => (JsonNode)id).ToArray());
		if (args.Integer("team-level") is { } team) Player["user_info"]!["team_level"] = Arguments.Positive(team, "team-level");
		foreach (var (option, field) in new[] { ("rarity-change", "ChangeRarity"), ("skill-sync", "SkillLevelSync"), ("slip-damage-barrier", "SlipDamageLogBarrier"), ("knight-enhance", "KnightEnhance") })
			if (args.Boolean(option) is { } value) Player["Releases"]![field] = value;
		if (args.Integer("fps") is { } fps) document["Settings"]!["FrameRate"] = fps;
		if (args.Boolean("infinite-hp") is { } infinite) Clan()["infinity_hp"] = infinite;
		if (args.Boolean("quad-speed") is { } quad) Clan()["quad_speed"] = quad;
		long? hp = args.Long("boss-hp"), current = args.Long("boss-current-hp");
		if (hp is not null || current is not null)
		{
			var boss = Clan()["boss_hp"] as JsonObject ?? new JsonObject();
			if (hp is { } max) boss["max_hp"] = max;
			if (current is { } now) boss["current_hp"] = now;
			Clan()["boss_hp"] = boss;
		}
		if (args.Integer("lap") is { } lap) Clan()["lap_num"] = lap;
		if (args.Integer("battle-id") is { } battleId) Clan()["clan_battle_id"] = battleId;
		if (args.Integer("phase") is { } phase) Clan()["phase"] = phase;
		if (args.Integer("order") is { } order) Clan()["order_num"] = order;
	}
	private JsonObject Clan() => document["ClanBattle"] as JsonObject ?? throw new ArgumentException("Clan battle options require a ClanBattle request.");
	internal void EditUnit(Catalog catalog, Arguments args, bool adding)
	{
		int id = int.Parse(args.Require("id"), System.Globalization.CultureInfo.InvariantCulture); catalog.Unit(id);
		var raw = Unit(id);
		if ((raw is not null) == adding) throw new InvalidDataException(adding ? "Unit already exists; use unit-edit." : "Unit is not owned; use unit-add.");
		if (adding) { raw = NewUnit(id); Units.Add(raw); }
		ApplyUnit(catalog, args, raw!, adding);
		if (args.Integer("slot") is { } slot)
		{
			if (slot is < 1 or > 5) throw new ArgumentException("Slot must be 1..5.");
			var party = Battle(args.Integer("battle") ?? args.Integer("ex-party"))["Party"]!.AsArray();
			for (int i = 0; i < party.Count; i++) if (party[i]!.GetValue<int>() == id) party[i] = 0;
			party[slot - 1] = id;
			if (!IsLabyrinth && !party.Any(p => p!.GetValue<int>() == 1)) Clan().Remove("support");
		}
	}
	internal void EditLabyrinth(Arguments args)
	{
		if (!IsLabyrinth) throw new ArgumentException("Labyrinth options require a Labyrinth request.");
		RejectRemoved(args);
		var mode = document["Labyrinth"]!.AsObject();
		foreach (var (option, field) in new[] { ("quest-id", "quest_id"), ("difficulty", "difficulty"), ("alpha-rupee", "alpha_rupee") })
			if (args.Integer(option) is { } value) mode[field] = value;
		if (args.Take("reinforcement-nodes") is { } nodes) mode["labyrinth_enhance_node_list"] = JsonSerializer.SerializeToNode(Integers(nodes));
		foreach (var (option, field) in new[] { ("unit-sources", "unit_list"), ("relics", "relic_list"), ("ex-state", "start_state") })
			if (args.Take(option) is { } file) mode[field] = ParseValue(File.ReadAllText(file));
		if (args.Take("ex-seeds") is { } seeds)
		{
			int[] values = Integers(seeds);
			if (values.Length is < 1 or > 3) throw new ArgumentException("--ex-seeds takes one to three seeds, one per EX party.");
			while (Battles.Count < values.Length) Battles.Add(new JsonObject { ["Party"] = Slots(Array.Empty<int>()), ["Opening"] = new JsonObject { ["Auto"] = false, ["Speed"] = 1, ["SetUnits"] = new JsonArray() }, ["Seed"] = 0 });
			for (int i = 0; i < values.Length; i++) Battles[i]!["Seed"] = values[i];
		}
		int? exParty = args.Integer("ex-party");
		string? units = args.Take("party-units"), set = args.Take("party-set");
		bool? auto = args.Boolean("party-auto"); int? speed = args.Integer("party-speed");
		if (exParty is not null && units is null && set is null && auto is null && speed is null) throw new ArgumentException("--ex-party selects the party that --party-units/--party-auto/--party-speed/--party-set edit.");
		if (units is null && set is null && auto is null && speed is null) return;
		var battle = Battle(exParty);
		if (units is not null)
		{
			int[] ids = Integers(units);
			if (ids.Length > 5 || ids.Contains(1)) throw new ArgumentException("--party-units takes up to five declared USER/NPC IDs (0 is empty).");
			battle["Party"] = Slots(ids);
		}
		if (auto is not null) battle["Opening"]!["Auto"] = auto.Value;
		if (speed is not null) battle["Opening"]!["Speed"] = speed.Value;
		if (set is not null) battle["Opening"]!["SetUnits"] = JsonSerializer.SerializeToNode(Integers(set));
	}
	/// <summary>Declare created units as owned USER sources; NPC entries are supplied explicitly.</summary>
	internal void DeclareUserUnits(int[] ids)
		=> document["Labyrinth"]!["unit_list"] = new JsonArray(ids.Select(id => (JsonNode)new JsonObject { ["unit_id"] = id, ["unit_type"] = 1 }).ToArray());
	internal void InitializeUnits(Catalog catalog, int[] ids, UnitOptions options)
	{
		Units.Clear();
		foreach (int id in ids)
		{
			catalog.Unit(id); var raw = NewUnit(id); ApplyUnit(catalog, raw, options, true);
			Units.Add(raw);
		}
		Party(string.Join(',', ids));
	}
	private static JsonObject NewUnit(int id) => new()
	{
		["id"] = id,
		["unit_rarity"] = 3,
		["battle_rarity"] = 0,
		["unit_level"] = 100,
		["promotion_level"] = 1,
		["exceed_stage"] = 0,
		["union_burst"] = new JsonArray(),
		["main_skill"] = new JsonArray(),
		["ex_skill"] = new JsonArray(),
		["free_skill"] = new JsonArray(),
		["equip_slot"] = new JsonArray(),
		["unique_equip_slot"] = new JsonArray(),
		["ex_equip_slot"] = new JsonArray(),
		["cb_ex_equip_slot"] = new JsonArray()
	};
	internal sealed record UnitOptions(int? Level, int? Rarity, int? BattleRarity, int? Rank, int? Skills, string? Equipment, int? Enhance);
	internal static UnitOptions ReadUnitOptions(Arguments args) => new(args.Integer("level"), args.Integer("rarity"), args.Integer("battle-rarity"),
		args.Integer("rank"), args.Integer("skills"), args.Take("equipment"), args.Integer("enhance"));
	private static void ApplyUnit(Catalog catalog, Arguments args, JsonObject raw, bool adding) => ApplyUnit(catalog, raw, ReadUnitOptions(args), adding);
	private static bool IsSlotted(JsonNode slot) => slot["is_slot"]!.GetValueKind() == JsonValueKind.True || slot["is_slot"]!.GetValueKind() == JsonValueKind.Number && slot["is_slot"]!.GetValue<int>() == 1;
	private static void ApplyUnit(Catalog catalog, JsonObject raw, UnitOptions options, bool adding)
	{
		int id = raw["id"]!.GetValue<int>();
		if (options.Level is { } level) raw["unit_level"] = Arguments.Positive(level, "level");
		if (options.Rarity is { } rarity) raw["unit_rarity"] = rarity;
		if (options.BattleRarity is { } battleRarity) raw["battle_rarity"] = battleRarity;
		int ownedRarity = raw["unit_rarity"]!.GetValue<int>();
		catalog.Require("unit_rarity", "unit_id", id, "rarity", ownedRarity);
		int battle = raw["battle_rarity"]?.GetValue<int>() ?? 0;
		if (battle != 0 && (ownedRarity != 5 || battle is < 3 or > 5)) throw new InvalidDataException("Battle rarity is 0, or 3..5 for an owned five-star unit.");
		int previousRank = raw["promotion_level"]!.GetValue<int>();
		if (options.Rank is { } rank) raw["promotion_level"] = Arguments.Positive(rank, "rank");
		var promotion = catalog.Require("unit_promotion", "unit_id", id, "promotion_level", raw["promotion_level"]!.GetValue<int>());
		if (adding) BuildSkills(catalog, raw, options.Skills ?? raw["unit_level"]!.GetValue<int>());
		else if (options.Skills is { } skills)
		{
			Arguments.Positive(skills, "skills");
			foreach (string field in new[] { "union_burst", "main_skill", "ex_skill", "free_skill" })
				foreach (var skill in raw[field]!.AsArray()) if (skill!["skill_level"]!.GetValue<int>() > 0) skill["skill_level"] = skills;
		}
		if (previousRank != raw["promotion_level"]!.GetValue<int>() && !adding && raw["equip_slot"]!.AsArray().Count > 0 && options.Equipment is null)
			throw new InvalidDataException("Changing rank with equipment requires --equipment none|all to explicitly rebuild the slots.");
		if (options.Equipment is not null || adding)
		{
			string equipment = options.Equipment ?? "none";
			if (equipment is not ("all" or "none")) throw new ArgumentException("Equipment: none or all.");
			if (equipment == "none" && options.Enhance is not (null or 0)) throw new ArgumentException("Enhancement requires equipped slots.");
			var slots = new JsonArray();
			if (equipment == "all") for (int i = 1; i <= 6; i++) slots.Add(Equipment(catalog, Catalog.Number(promotion, "equip_slot_" + i), options.Enhance ?? 0));
			raw["equip_slot"] = slots;
		}
		else if (options.Enhance is { } enhancement)
		{
			if (raw["equip_slot"]!.AsArray().Count == 0) throw new InvalidDataException("No equipment; use --equipment all.");
			foreach (var slot in raw["equip_slot"]!.AsArray())
				if (IsSlotted(slot!))
				{
					var rebuilt = Equipment(catalog, slot!["id"]!.GetValue<int>(), enhancement);
					slot["enhancement_level"] = rebuilt["enhancement_level"]!.DeepClone(); slot["enhancement_pt"] = rebuilt["enhancement_pt"]!.DeepClone();
				}
		}
	}
	private static JsonObject Equipment(Catalog catalog, int id, int level)
	{
		if (level < 0) throw new ArgumentException("Enhance must be nonnegative.");
		// Unreleased equipment still occupies its original slot in the raw account list.
		if (id == 999999) return new() { ["id"] = id, ["is_slot"] = false, ["enhancement_level"] = 0, ["enhancement_pt"] = 0, ["rank"] = 0 };
		var data = catalog.Require("equipment_data", "equipment_id", id);
		int points = level == 0 ? 0 : Catalog.Number(catalog.Require("equipment_enhance_data", "promotion_level", Catalog.Number(data, "promotion_level"), "equipment_enhance_level", level), "total_point");
		return new() { ["id"] = id, ["is_slot"] = true, ["enhancement_level"] = level, ["enhancement_pt"] = points, ["rank"] = 0 };
	}
	private static void BuildSkills(Catalog catalog, JsonObject raw, int level)
	{
		Arguments.Positive(level, "skills"); var definitions = catalog.Require("unit_skill_data", "unit_id", raw["id"]!.GetValue<int>());
		JsonArray Skills(params string[] columns)
		{
			var result = new JsonArray();
			for (int i = 0; i < columns.Length; i++)
			{
				int id = Catalog.Number(definitions, columns[i]);
				if (id > 0) result.Add(new JsonObject { ["skill_id"] = id, ["skill_level"] = level, ["slot_number"] = i });
			}
			return result;
		}
		raw["union_burst"] = Skills("union_burst"); raw["main_skill"] = Skills("main_skill_1", "main_skill_2"); raw["ex_skill"] = Skills("ex_skill_1");
	}
	internal void Stories(string text)
	{
		var ids = Integers(text);
		if (ids.Any(id => id <= 0) || ids.Distinct().Count() != ids.Length) throw new ArgumentException("Stories require distinct positive IDs or none.");
		Player["read_story_ids"] = JsonSerializer.SerializeToNode(ids);
	}
	internal void Set(string pointer, string json)
	{
		if (!pointer.StartsWith('/') || pointer == "/") throw new ArgumentException("Path must be a nonempty JSON Pointer, e.g. /Battles/0/Seed.");
		string[] parts = pointer[1..].Split('/').Select(Unescape).ToArray(); JsonNode parent = document;
		foreach (string part in parts[..^1]) parent = Child(parent, part) ?? throw new ArgumentException("Missing parent in path: " + pointer);
		string key = parts[^1]; JsonNode? value = ParseValue(json);
		if (parent is JsonObject obj) obj[key] = value;
		else if (parent is JsonArray array) array[Index(key, array.Count)] = value;
		else throw new ArgumentException("Path parent must be an object or array.");
	}
	internal void Patch(string path)
	{
		var changes = ParseValue(File.ReadAllText(path)) as JsonArray ?? throw new ArgumentException("Patch must be an array of {Path, Value} entries.");
		if (changes.Count == 0) throw new ArgumentException("Patch cannot be empty.");
		foreach (var node in changes)
		{
			var change = node as JsonObject ?? throw new ArgumentException("Each patch entry must be an object.");
			if (change.Count != 2 || !change.ContainsKey("Path") || !change.ContainsKey("Value")) throw new ArgumentException("Each patch entry requires only Path and Value.");
			Set(change["Path"]!.GetValue<string>(), change["Value"]?.ToJsonString() ?? "null");
		}
	}
	private static JsonNode? ParseValue(string json)
	{
		using var parsed = JsonDocument.Parse(json);
		JsonInputValidation.RejectDuplicateProperties(parsed.RootElement);
		return JsonNode.Parse(json);
	}
	private static string Unescape(string value)
	{
		for (int i = 0; i < value.Length; i++) if (value[i] == '~' && (++i == value.Length || value[i] is not ('0' or '1'))) throw new ArgumentException("Invalid JSON Pointer escape.");
		return value.Replace("~1", "/", StringComparison.Ordinal).Replace("~0", "~", StringComparison.Ordinal);
	}
	private static int Index(string key, int count) => int.TryParse(key, out int i) && i >= 0 && i < count && i.ToString(System.Globalization.CultureInfo.InvariantCulture) == key ? i : throw new ArgumentException("Array index is outside the existing array: " + key);
	private static JsonNode? Child(JsonNode node, string key) => node is JsonObject obj ? obj[key] : node is JsonArray array ? array[Index(key, array.Count)] : null;
	internal object Summary() => new
	{
		Schema = BattleRequest.CurrentSchema,
		MasterVersion = Version,
		Battles = document["Battles"],
		Settings = document["Settings"],
		Player = new
		{
			TeamLevel = Player["user_info"]!["team_level"],
			Releases = Player["Releases"],
			Stories = Player["read_story_ids"],
			Units = Units.Select(raw => new
			{
				Id = raw!["id"],
				Level = raw["unit_level"],
				Rarity = raw["unit_rarity"],
				BattleRarity = raw["battle_rarity"],
				Rank = raw["promotion_level"],
				Equipment = raw["equip_slot"],
				UniqueEquipment = raw["unique_equip_slot"],
				ExEquipment = raw["ex_equip_slot"],
				ClanExEquipment = raw["cb_ex_equip_slot"],
				Ub = raw["union_burst"],
				Main = raw["main_skill"],
				Ex = raw["ex_skill"],
				Free = raw["free_skill"]
			}).ToArray()
		},
		ClanBattle = document["ClanBattle"],
		Labyrinth = document["Labyrinth"]
	};
	internal object Validate(string resources)
	{
		var request = BattleRequest.Parse(document.ToJsonString());
		if (request.Labyrinth is not null) return PreparedSummary(LabyrinthBattlePreparer.Prepare(request, resources).Inputs.Products);
		return PreparedSummary(BattlePreparer.Prepare(request, resources).Inputs.Products);
	}
	private static object PreparedSummary(PreparedBattleProducts products) => new { Stage = "prepared", Owned = products.Owned.Count, Enemies = products.Enemies.Count, Limited = products.Limited.Count, Support = products.Support is not null };
	internal object Save(string resources, string? output, bool newFileOnly = false)
	{
		var prepared = Validate(resources);
		string json = BattleRequest.Parse(document.ToJsonString()).ToJson();
		if (output is not null)
		{
			string target = Path.GetFullPath(output); string directory = Path.GetDirectoryName(target)!;
			Directory.CreateDirectory(directory); string temporary = Path.Combine(directory, "." + Path.GetFileName(target) + "." + Guid.NewGuid().ToString("N") + ".tmp");
			try { File.WriteAllText(temporary, json + "\n"); File.Move(temporary, target, !newFileOnly); }
			finally { if (File.Exists(temporary)) File.Delete(temporary); }
		}
		return new
		{
			Output = output is null ? null : Path.GetFullPath(output),
			DryRun = output is null,
			Prepared = prepared,
			ChangedPaths = Changes(original, document, "").ToArray(),
			Summary = Summary()
		};
	}
	private static IEnumerable<string> Changes(JsonNode? before, JsonNode? after, string path)
	{
		if (JsonNode.DeepEquals(before, after)) yield break;
		if (before is JsonObject a && after is JsonObject b)
		{
			foreach (string key in a.Select(p => p.Key).Union(b.Select(p => p.Key)))
				foreach (string change in Changes(a[key], b[key], path + "/" + key.Replace("~", "~0", StringComparison.Ordinal).Replace("/", "~1", StringComparison.Ordinal))) yield return change;
		}
		else if (before is JsonArray x && after is JsonArray y && x.Count == y.Count)
		{
			for (int i = 0; i < x.Count; i++)
				foreach (string change in Changes(x[i], y[i], path + "/" + i.ToString(System.Globalization.CultureInfo.InvariantCulture))) yield return change;
		}
		else yield return path;
	}
}
