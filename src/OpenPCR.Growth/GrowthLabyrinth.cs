using System;
using System.Collections.Generic;
using System.Linq;
using Elements;

namespace OpenPCR.Growth;

public sealed partial class GrowthCalculator
{
	/// <summary>Native node/relic formulas and selection, using the complete declared ownership set.</summary>
	public PreparedLabyrinthProducts PrepareLabyrinthMode(LabyrinthGrowthContext context)
	{
		ArgumentNullException.ThrowIfNull(context);
		_ = ActiveInput;
		_ = master.Require("labyrinth_difficulty_data", ("difficulty", context.Difficulty));
		var difficulty = master.Require("labyrinth_difficulty_enemy_data", ("difficulty", context.Difficulty));
		var difficultyIds = new List<int>(); var timings = new List<int>();
		for (int i = 1; i <= 5; i++)
			if (difficulty.Int("enemy_id_" + i) != 0) { difficultyIds.Add(difficulty.Int("enemy_id_" + i)); timings.Add(difficulty.Int("exec_timing_" + i)); }
		var targetValues = new Dictionary<int, int> { [1] = context.OwnedUnits.Count, [30] = context.AlphaRupee };
		for (int type = 10; type <= 29; type++)
		{
			int selected = type;
			targetValues.Add(type, context.OwnedUnits.Count(unit => selected < 20
				? master.Require("unit_role_data", ("unit_id", unit.UnitId)).Int("unit_role_id") == selected - 10
				: master.Rows("unit_talent").Any(row => row.Int("unit_id") == unit.UnitId && row.Int("talent_id") == selected % 10)));
		}
		var marks = new Dictionary<int, int>(); var active = new List<int>(); var passiveRows = new List<GrowthMasterData.Row>();
		IEnumerable<GrowthMasterData.Row> Effects(int group)
		{
			var row = master.Require("labyrinth_relic_effect_group", ("effect_group_id", group));
			for (int i = 1; i <= 5; i++)
				if (row.Int("effect_id_" + i) != 0) yield return master.Require("labyrinth_relic_effect", ("effect_id", row.Int("effect_id_" + i)));
		}
		foreach (var owned in context.Relics)
		{
			var row = master.Require("labyrinth_relic", ("relic_id", owned.RelicId));
			int mark = row.Int("relic_mark_id"); marks[mark] = unchecked(marks.GetValueOrDefault(mark) + row.Int("relic_mark_count"));
			int group = row.Int("effect_group_id"); var effects = Effects(group).ToArray();
			if (owned.Enable != 0 && effects.Any(effect => effect.Int("effect_type") == 1)) active.Add(group);
			passiveRows.AddRange(effects.Where(effect => effect.Int("effect_type") == 14));
		}
		// Every mark threshold row is visited in the loaded Native table order; thresholds can stack.
		var markGroups = new List<int>();
		foreach (var row in master.Rows("labyrinth_relic_mark_effect"))
			if (marks.GetValueOrDefault(row.Int("relic_mark_id")) >= row.Int("trigger_count")) markGroups.Add(row.Int("effect_group_id"));
		var requiredMarkIds = master.Rows("labyrinth_relic").Select(row => row.Int("relic_mark_id"))
			.Concat(master.Rows("labyrinth_relic_mark_effect").Select(row => row.Int("relic_mark_id"))).Distinct();
		foreach (int mark in requiredMarkIds) targetValues[unchecked(100 + mark)] = marks.GetValueOrDefault(mark);
		// Resource actions can explicitly request a mark not present on any owned relic.
		foreach (var row in master.Rows("skill_action").Where(row => row.Int("action_type") == 127))
		{
			int type = NativeNumeric.ToInt32(row.Double("action_value_1"));
			if (type >= 100) targetValues[type] = marks.GetValueOrDefault(unchecked(type - 100));
		}
		var passive = new Dictionary<eParamType, int>();
		foreach (var row in passiveRows)
		{
			var type = (eParamType)row.Int("effect_value_1"); int value = row.Int("effect_value_2"); int countType = row.Int("effect_value_3");
			if (countType != 0)
			{
				int count = countType >= 100 ? marks.GetValueOrDefault(unchecked(countType - 100)) : targetValues[countType];
				value = unchecked(value * Math.Min(count, row.Int("effect_value_4")));
			}
			passive[type] = unchecked(passive.GetValueOrDefault(type) + value);
		}
		var reinforcement = new Dictionary<eParamType, int>();
		foreach (int id in context.ReinforcementNodes)
		{
			int effect = master.Require("labyrinth_enhance_node", ("node_id", id)).Int("effect_id");
			var row = master.Require("labyrinth_enhance_data", ("effect_id", effect));
			if (row.Int("effect_type") == 1) Add((eParamType)row.Int("effect_value_1"), row.Int("effect_value_2"));
			else if (row.Int("effect_type") == 2) Add((eParamType)104, row.Int("effect_value_1"));
		}
		void Add(eParamType type, int value) => reinforcement[type] = unchecked(reinforcement.GetValueOrDefault(type) + value);
		return new(new PreparedLabyrinthInputs(context.Difficulty, difficultyIds, timings, active, markGroups, targetValues), reinforcement, passive);
	}
	/// <summary>Prepare the actors selected by the explicit Labyrinth quest and difficulty, including its hidden relic actor.</summary>
	public PreparedParameterProducts PrepareLabyrinthParameters(int category, int questId, int difficulty)
	{
		if (category is not (59 or 60)) throw new ArgumentOutOfRangeException(nameof(category));
		_ = master.Require("labyrinth_difficulty_data", ("difficulty", difficulty));
		_ = master.Require("labyrinth_quest_data", ("quest_id", questId));
		var ids = new List<int> { 770999901 };
		var difficultyRow = master.Require("labyrinth_difficulty_enemy_data", ("difficulty", difficulty));
		for (int i = 1; i <= 5; i++) if (difficultyRow.Int("enemy_id_" + i) != 0) ids.Add(difficultyRow.Int("enemy_id_" + i));
		foreach (var row in master.Rows("extra_effect_data").Where(row => row.Int("content_type") == category && row.Int("target_value_1") == questId && row.Int("target_value_2") == 0))
			for (int i = 1; i <= 5; i++) if (row.Int("enemy_id_" + i) != 0) ids.Add(row.Int("enemy_id_" + i));
		var enemies = ids.Distinct().Select(id => master.Find("enemy_parameter", ("enemy_id", id)) is not null ? CalculateEnemy(id) : CalculateLabyrinthEnemy(id)).ToArray();
		return PrepareParameters(new[] { questId }, enemies, allowLabyrinth: true);
	}
}
