using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elements;
using Elements.Data;
using OpenPCR.Growth;

namespace OpenPCR.Host;

/// <summary>
/// EX challenge state at the start of one party: the party index and the client-local records carried between parties.
/// Party 0 starts from <see cref="Initial(string, string, int)"/>; each later party starts from <see cref="FromSettlement"/> of the previous one.
/// Wire-safe values only; no Native private setters are deserialized.
/// </summary>
public sealed record LabyrinthExState(
	[property: JsonRequired] int PartyIndex,
	[property: JsonRequired] LabyrinthExEnemyInput[] Enemies, [property: JsonRequired] LabyrinthExDeadInput[] AlreadyDeadEnemies,
	[property: JsonRequired] LabyrinthExWaveInput[] Waves, [property: JsonRequired] Dictionary<int, LabyrinthExUnitResultInput>[] UnitResults,
	[property: JsonRequired] eBattleClearType[] ClearTypes, [property: JsonRequired, JsonIgnore(Condition = JsonIgnoreCondition.Never)] LabyrinthExContinuousInput[][]? EnemyContinuation)
{
	/// <summary>
	/// Initial party state: one record per quest wave enemy
	/// in order, IdentifyNum from 1, Hp and MaxHp from labyrinth_enemy_parameter.hp, Energy 0, level/rarity/promotion from the
	/// same row; other fields keep the TowerUnit constructor defaults. No previous results.
	/// </summary>
	public static LabyrinthExState Initial(string resources, string masterVersion, int questId)
	{
		ArgumentNullException.ThrowIfNull(resources); ArgumentNullException.ThrowIfNull(masterVersion);
		using var connection = ResourceDatabases.Open(Path.Combine(Path.GetFullPath(resources), "masterdata", masterVersion + "_unhashed.db"));
		return Initial(new GrowthMasterData(connection), questId);
	}
	internal static LabyrinthExState Initial(GrowthMasterData master, int questId)
	{
		var quest = master.Require("labyrinth_quest_data", ("quest_id", questId));
		var wave = master.Require("labyrinth_wave_group_data", ("wave_group_id", quest.Int("wave_group_id")));
		var enemies = Enumerable.Range(1, 5).Select(i => wave.Int("enemy_id_" + i)).Where(id => id != 0).Select((id, index) =>
		{
			var row = master.Require("labyrinth_enemy_parameter", ("enemy_id", id));
			long hp = row.Long("hp");
			return new LabyrinthExEnemyInput(id, index + 1, hp, 0, Array.Empty<LabyrinthExCounterInput>(), hp, 0,
				row.Int("level"), row.Int("rarity"), row.Int("promotion_level"), 0, null);
		}).ToArray();
		return new(0, enemies, Array.Empty<LabyrinthExDeadInput>(), Array.Empty<LabyrinthExWaveInput>(),
			Array.Empty<Dictionary<int, LabyrinthExUnitResultInput>>(), Array.Empty<eBattleClearType>(), null);
	}
	/// <summary>State for the next party from a completed nonfinal party's settlement.</summary>
	private static readonly JsonSerializerOptions options = new() { IncludeFields = true };
	public static LabyrinthExState FromSettlement(LabyrinthExBattleSettlement settlement)
	{
		ArgumentNullException.ThrowIfNull(settlement);
		if (settlement.IsFinalTeam) throw new InvalidOperationException("The settled EX party is final; there is no next party.");
		// The settlement's public members serialize to the same wire records this state reads.
		T Copy<T>(object value) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, options), options)!;
		return new(settlement.PartyIndex + 1, Copy<LabyrinthExEnemyInput[]>(settlement.Enemies), Copy<LabyrinthExDeadInput[]>(settlement.AlreadyDeadEnemies),
			Copy<LabyrinthExWaveInput[]>(settlement.Waves), Copy<Dictionary<int, LabyrinthExUnitResultInput>[]>(settlement.UnitResults),
			settlement.ClearTypes.ToArray(), settlement.EnemyContinuation is null ? null : Copy<LabyrinthExContinuousInput[][]>(settlement.EnemyContinuation));
	}
	internal PreparedLabyrinthExContext Prepare(IEnumerable<IEnumerable<int>> parties)
	{
		T[] Rows<T>(T[]? rows) => rows is null || rows.Any(row => row is null) ? throw new InvalidDataException("EX arrays and entries must be explicit.") : rows;
		List<SkillLimitCounter> Counters(LabyrinthExCounterInput[] rows) => Rows(rows).Select(row => new SkillLimitCounter(row.skill_id, row.counter)).ToList();
		TowerUnit Enemy(LabyrinthExEnemyInput row)
		{
			var unit = new TowerUnit();
			unit.SetUnitId(row.UnitId); unit.SetIdentifyNum(row.IdentifyNum); unit.SetHp(row.Hp); unit.SetEnergy(row.Energy);
			unit.SetSkillLimitCounter(Counters(row.SkillLimitCounter)); unit.SetMaxHp(row.MaxHp); unit.SetSavedHp(row.SavedHp);
			unit.SetLevel(row.Level); unit.SetRarity(row.Rarity); unit.SetPromotionLevel(row.PromotionLevel); unit.SetEnemyColor(row.EnemyColor);
			if (row.SkinData is { } skin)
			{
				var data = new SkinData();
				RuntimeBinding.Set(data, "IconSkinId", skin.IconSkinId); RuntimeBinding.Set(data, "SdSkinId", skin.SdSkinId);
				RuntimeBinding.Set(data, "StillSkinId", skin.StillSkinId); RuntimeBinding.Set(data, "MotionId", skin.MotionId); RuntimeBinding.Set(unit, "SkinData", data);
			}
			return unit;
		}
		DungeonQueryUnit Dead(LabyrinthExDeadInput row) => new(row.owner_viewer_id, row.unit_id, row.retired, row.hp, row.energy,
			Counters(row.skill_limit_counter).ToArray(), row.damage, Rows(row.parts_list).Select(part =>
			{
				var data = new PartsInfo(); RuntimeBinding.Set(data, "parts_id", part.parts_id); RuntimeBinding.Set(data, "hp", part.hp); return data;
			}).ToArray());
		return new(PartyIndex, parties, Rows(Enemies).Select(Enemy), Rows(AlreadyDeadEnemies).Select(Dead),
			Rows(Waves).Select(row => new TowerWaveResultInfo(row.wave_num, Rows(row.unit_info_list).Select(unit => new TowerWaveResultUnitInfo(unit.unit_id, unit.owner_viewer_id, unit.damage, unit.is_alive)).ToArray(), row.remain_time)),
			Rows(UnitResults).Select(row => row.ToDictionary(pair => pair.Key, pair => pair.Value is { } value ? new LabyrinthUnitResultInfo(value.Hp, value.MaxHp, value.Energy) : throw new InvalidDataException("Null EX prior unit result."))),
			Rows(ClearTypes), EnemyContinuation is null ? null : Rows(EnemyContinuation).Select(row => Rows(row).Select(unit => new ContinuousUnit(unit.UnitId, unit.IdentifyNum, unit.Hp, unit.Energy, Counters(unit.SkillLimitCounter)))));
	}
}

public sealed record LabyrinthExCounterInput([property: JsonRequired] int skill_id, [property: JsonRequired] int counter);
public sealed record LabyrinthExUnitResultInput([property: JsonRequired] long Hp, [property: JsonRequired] long MaxHp, [property: JsonRequired] double Energy);
public sealed record LabyrinthExSkinInput([property: JsonRequired] int IconSkinId, [property: JsonRequired] int SdSkinId,
	[property: JsonRequired] int StillSkinId, [property: JsonRequired] int MotionId);
public sealed record LabyrinthExEnemyInput([property: JsonRequired] int UnitId, [property: JsonRequired] int IdentifyNum,
	[property: JsonRequired] long Hp, [property: JsonRequired] int Energy, [property: JsonRequired] LabyrinthExCounterInput[] SkillLimitCounter,
	[property: JsonRequired] long MaxHp, [property: JsonRequired] long SavedHp, [property: JsonRequired] int Level,
	[property: JsonRequired] int Rarity, [property: JsonRequired] int PromotionLevel, [property: JsonRequired] int EnemyColor,
	[property: JsonRequired, JsonIgnore(Condition = JsonIgnoreCondition.Never)] LabyrinthExSkinInput? SkinData);
public sealed record LabyrinthExPartInput([property: JsonRequired] int parts_id, [property: JsonRequired] long hp);
public sealed record LabyrinthExDeadInput([property: JsonRequired] long owner_viewer_id, [property: JsonRequired] int unit_id,
	[property: JsonRequired] int retired, [property: JsonRequired] long hp, [property: JsonRequired] int energy,
	[property: JsonRequired] LabyrinthExCounterInput[] skill_limit_counter, [property: JsonRequired] long damage,
	[property: JsonRequired] LabyrinthExPartInput[] parts_list);
public sealed record LabyrinthExWaveUnitInput([property: JsonRequired] int unit_id, [property: JsonRequired] long owner_viewer_id,
	[property: JsonRequired] long damage, [property: JsonRequired] int is_alive);
public sealed record LabyrinthExWaveInput([property: JsonRequired] int wave_num, [property: JsonRequired] LabyrinthExWaveUnitInput[] unit_info_list,
	[property: JsonRequired] int remain_time);
public sealed record LabyrinthExContinuousInput([property: JsonRequired] int UnitId, [property: JsonRequired] int IdentifyNum,
	[property: JsonRequired] long Hp, [property: JsonRequired] int Energy, [property: JsonRequired] LabyrinthExCounterInput[] SkillLimitCounter);
