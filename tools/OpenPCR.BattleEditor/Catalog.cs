using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace OpenPCR.BattleEditor;

/// <summary>Editor lookups only; no parameter formulas or runtime state.</summary>
internal sealed class Catalog : IDisposable
{
	private readonly SqliteConnection database;
	internal Catalog(string resources, string version)
	{
		if (version.Length != 12 || version.Any(c => c is < '0' or > '9')) throw new ArgumentException("Master version must have twelve digits.");
		string path = Path.GetFullPath(Path.Combine(resources, "masterdata", version + "_unhashed.db"));
		if (!File.Exists(path)) throw new FileNotFoundException("Masterdata deployment is missing.", path);
		database = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path, Mode = SqliteOpenMode.ReadOnly, Pooling = false }.ToString());
		try { database.Open(); } catch { database.Dispose(); throw; }
	}
	internal List<Dictionary<string, object>> Query(string sql, params (string Name, object Value)[] parameters)
	{
		using var command = database.CreateCommand(); command.CommandText = sql;
		foreach (var (name, value) in parameters) command.Parameters.AddWithValue(name, value);
		using var reader = command.ExecuteReader(); var rows = new List<Dictionary<string, object>>();
		while (reader.Read())
		{
			var row = new Dictionary<string, object>(StringComparer.Ordinal);
			for (int i = 0; i < reader.FieldCount; i++) row.Add(reader.GetName(i), reader.GetValue(i));
			rows.Add(row);
		}
		return rows;
	}
	internal Dictionary<string, object> Unit(int id)
	{
		var row = Query("SELECT unit_id,unit_name,rarity FROM unit_data WHERE unit_id=$id AND EXISTS (SELECT 1 FROM unit_promotion p WHERE p.unit_id=unit_data.unit_id) AND NOT EXISTS (SELECT 1 FROM unit_conversion c WHERE c.unit_id=unit_data.unit_id)", ("$id", id)).SingleOrDefault();
		return row ?? throw new InvalidDataException("No original owned unit definition: " + id);
	}
	internal static int Number(Dictionary<string, object> row, string name) => Convert.ToInt32(row[name], CultureInfo.InvariantCulture);
	internal Dictionary<string, object> Require(string table, string key, int id, string? secondKey = null, int second = 0)
	{
		// Table/column names here are literals at editor call sites, never user-supplied SQL.
		var rows = Query("SELECT * FROM " + table + " WHERE " + key + "=$id" + (secondKey is null ? "" : " AND " + secondKey + "=$second"), ("$id", id), ("$second", second));
		return rows.SingleOrDefault() ?? throw new InvalidDataException($"Missing {table}: {key}={id}" + (secondKey is null ? "" : $", {secondKey}={second}"));
	}
	internal object Search(string kind, string? query, int? id, int limit)
	{
		if (limit is < 1 or > 1000) throw new ArgumentException("Limit must be 1..1000.");
		if (kind == "backgrounds") return Query("SELECT DISTINCT map_bg AS Id,'clan_battle_2_map_data' AS Source FROM clan_battle_2_map_data WHERE ($id=0 OR map_bg=$id) AND CAST(map_bg AS TEXT) LIKE $query ORDER BY map_bg LIMIT $limit",
			("$id", id ?? 0), ("$query", "%" + (query ?? "") + "%"), ("$limit", limit));
		if (kind == "clan-maps") return Query("SELECT clan_battle_id AS Id,id AS MapId,phase AS Phase,lap_num_from AS LapFrom,lap_num_to AS LapTo,0 AS Mode FROM clan_battle_2_map_data WHERE ($id=0 OR clan_battle_id=$id) AND CAST(clan_battle_id AS TEXT) LIKE $query UNION ALL SELECT clan_battle_id,id,phase,lap_num_from,lap_num_to,1 FROM clan_battle_s_map_data WHERE ($id=0 OR clan_battle_id=$id) AND CAST(clan_battle_id AS TEXT) LIKE $query ORDER BY Id,Mode,Phase,LapFrom LIMIT $limit",
			("$id", id ?? 0), ("$query", "%" + (query ?? "") + "%"), ("$limit", limit));
		if (kind == "labyrinth-quests") return Query("SELECT quest_id AS Id,quest_type AS QuestType,limit_time AS TimeLimit,wave_group_id AS WaveGroupId FROM labyrinth_quest_data WHERE ($id=0 OR quest_id=$id) AND CAST(quest_id AS TEXT) LIKE $query ORDER BY quest_id LIMIT $limit",
			("$id", id ?? 0), ("$query", "%" + (query ?? "") + "%"), ("$limit", limit));
		if (kind == "labyrinth-relics") return Query("SELECT relic_id AS Id,name AS Name,relic_mark_id AS MarkId,relic_mark_count AS MarkCount,effect_group_id AS EffectGroupId FROM labyrinth_relic WHERE ($id=0 OR relic_id=$id) AND (name LIKE $query OR CAST(relic_id AS TEXT) LIKE $query) ORDER BY relic_id LIMIT $limit",
			("$id", id ?? 0), ("$query", "%" + (query ?? "") + "%"), ("$limit", limit));
		var (table, key, name, extra) = kind switch
		{
			"units" => ("unit_data", "unit_id", "unit_name", " AND EXISTS (SELECT 1 FROM unit_promotion p WHERE p.unit_id=unit_data.unit_id) AND NOT EXISTS (SELECT 1 FROM unit_conversion c WHERE c.unit_id=unit_data.unit_id)"),
			"enemies" => ("enemy_parameter", "enemy_id", "name", ""),
			"labyrinth-enemies" => ("labyrinth_enemy_parameter", "enemy_id", "name", ""),
			"equipment" => ("equipment_data", "equipment_id", "equipment_name", ""),
			"stories" => ("chara_story_status", "story_id", "unlock_story_name", ""),
			_ => throw new ArgumentException("Catalog kind: units, enemies, backgrounds, clan-maps, equipment, stories, labyrinth-enemies, labyrinth-quests, labyrinth-relics.")
		};
		return Query($"SELECT {key} AS Id,{name} AS Name FROM {table} WHERE ($id=0 OR {key}=$id) AND ({name} LIKE $query OR CAST({key} AS TEXT) LIKE $query){extra} ORDER BY {key} LIMIT $limit",
			("$id", id ?? 0), ("$query", "%" + (query ?? "") + "%"), ("$limit", limit));
	}
	internal object DescribeUnit(int id) => new
	{
		Unit = Unit(id),
		Rarities = Query("SELECT rarity FROM unit_rarity WHERE unit_id=$id ORDER BY rarity", ("$id", id)),
		Promotions = Query("SELECT * FROM unit_promotion WHERE unit_id=$id ORDER BY promotion_level", ("$id", id)),
		Skills = Require("unit_skill_data", "unit_id", id),
		UniqueEquipment = Query("SELECT * FROM unit_unique_equipment WHERE unit_id=$id ORDER BY equip_slot", ("$id", id))
	};
	public void Dispose() => database.Dispose();
}
