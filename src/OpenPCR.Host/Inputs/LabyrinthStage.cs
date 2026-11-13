using System;
using System.IO;
using System.Linq;
using OpenPCR.Growth;

namespace OpenPCR.Host;

/// <summary>
/// Derives enemies, time limit and background from the Labyrinth quest and difficulty tables.
/// </summary>
/// <param name="Category">59 for quest types 1 and 2, 60 for quest type 3 (EX).</param>
/// <param name="EnemyIds">Wave group enemy_id_1..5 in order with zero entries removed.</param>
/// <param name="TimeLimit">(float)(quest.limit_time − difficulty.battle_decrease_time).</param>
/// <param name="Backgrounds">The display row's battle_bg_id (the EX display table for EX quests).</param>
internal sealed record LabyrinthStage(int Category, int[] EnemyIds, float TimeLimit, int[] Backgrounds)
{
	/// <summary>Read the stage for one quest and difficulty from an explicitly deployed masterdata version.</summary>
	public static LabyrinthStage Read(string resources, string masterVersion, int questId, int difficulty)
	{
		ArgumentNullException.ThrowIfNull(resources); ArgumentNullException.ThrowIfNull(masterVersion);
		if (masterVersion.Length != 12 || !masterVersion.All(char.IsAsciiDigit))
			throw new ArgumentException("Specify the twelve-digit masterdata deployment version.", nameof(masterVersion));
		using var connection = ResourceDatabases.Open(Path.Combine(Path.GetFullPath(resources), "masterdata", masterVersion + "_unhashed.db"));
		return Read(new GrowthMasterData(connection), questId, difficulty);
	}

	internal static LabyrinthStage Read(GrowthMasterData master, int questId, int difficulty)
	{
		var quest = master.Require("labyrinth_quest_data", ("quest_id", questId));
		int type = quest.Int("quest_type");
		if (type is not (1 or 2 or 3)) throw new InvalidDataException("Unsupported Labyrinth quest type: " + type);
		bool ex = type == 3;
		var display = master.Require(ex ? "labyrinth_ex_quest_data_display" : "labyrinth_quest_data_display", ("quest_id", questId));
		var difficultyRow = master.Require("labyrinth_difficulty_data", ("difficulty", difficulty));
		var wave = master.Require("labyrinth_wave_group_data", ("wave_group_id", quest.Int("wave_group_id")));
		int[] enemies = Enumerable.Range(1, 5).Select(i => wave.Int("enemy_id_" + i)).Where(id => id != 0).ToArray();
		return new(ex ? 60 : 59, enemies, quest.Int("limit_time") - difficultyRow.Int("battle_decrease_time"), new[] { display.Int("battle_bg_id") });
	}

	/// <summary>Stand-in stage used only to validate a request's structure before resources are opened.</summary>
	internal static LabyrinthStage Placeholder(int category) => new(category, new[] { 1 }, 1f, new[] { 1 });
}
