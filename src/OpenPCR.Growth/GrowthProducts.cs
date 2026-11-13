using System;
using System.Collections.Generic;
using System.Linq;
using Elements;

namespace OpenPCR.Growth;

public sealed partial class GrowthCalculator
{
	/// <summary>Compile detached summon rules from masterdata for runtime parameter construction.</summary>
	/// <param name="stageEnemyIds">Stage enemies; extra-effect rows they key (target_value_1) get enemy products.</param>
	public PreparedParameterProducts PrepareParameters(IEnumerable<int> stageEnemyIds)
		=> PrepareParameters(stageEnemyIds, Array.Empty<PreparedUnit>());
	private PreparedParameterProducts PrepareParameters(IEnumerable<int> stageEnemyIds, IEnumerable<PreparedUnit> additionalEnemies, bool allowLabyrinth = false)
	{
		var relations = new Dictionary<int, SummonActionSpec[]>();
		foreach (var skill in master.Rows("skill_data"))
		{
			var values = new List<SummonActionSpec>();
			for (int index = 1; index <= 10; index++)
			{
				var action = master.Find("skill_action", ("action_id", skill.Int("action_" + index)));
				if (action is not null && action.Int("action_type") == 15)
					values.Add(new(action.Int("action_detail_2"), NativeNumeric.ToInt32(action.Float("action_value_4"))));
			}
			if (values.Count != 0) relations.Add(skill.Int("skill_id"), values.ToArray());
		}
		var definitions = new Dictionary<int, SummonDefinition>(); var enemies = new Dictionary<int, PreparedUnit>();
		foreach (int id in relations.Values.SelectMany(values => values).Select(value => value.UnitId).Distinct())
		{
			if (id >= 1000000)
			{
				if (master.Find("enemy_parameter", ("enemy_id", id)) is not null) enemies.Add(id, CalculateEnemy(id));
				else if (allowLabyrinth && master.Find("labyrinth_enemy_parameter", ("enemy_id", id)) is not null) enemies.Add(id, CalculateLabyrinthEnemy(id));
				continue;
			}
			if (id == 0 || master.Find("unit_data", ("unit_id", id)) is null) continue;
			int original = Original(id); var skills = master.Require("unit_skill_data", ("unit_id", id));
			var curves = new Dictionary<(int Rarity, int Rank), PreparedGrowthCurve[]>();
			var ranks = master.Rows("unit_promotion_status").Where(row => row.Int("unit_id") == original).Select(row => row.Int("promotion_level")).Append(1).Distinct();
			foreach (int rank in ranks)
				for (int rarity = 1; rarity <= 6; rarity++)
				{
					var rarityRow = master.Find("unit_rarity", ("unit_id", original), ("rarity", rarity));
					var promotion = rank > 1 ? master.Require("unit_promotion_status", ("unit_id", original), ("promotion_level", rank)) : null;
					var bonus = master.Find("promotion_bonus", ("unit_id", original), ("promotion_level", rank));
					curves.Add((rarity, rank), Enumerable.Range(1, 17).Select(type =>
					{
						double growth = rarityRow?.Double(Columns[type - 1] + "_growth") ?? 0;
						return new PreparedGrowthCurve(Float(promotion, type), growth, Float(rarityRow, type), type <= 6 ? growth * rank : 0, DecimalDouble((float)Float(bonus, type)));
					}).ToArray());
				}
			var main = Enumerable.Range(1, 10).Select(index => new SummonSkillSpec(skills.Int("main_skill_" + index), master.Find("unlock_skill_data", ("unlock_skill", 200 + index))?.Int("promotion_level"))).ToArray();
			var conversion = master.Find("unit_conversion", ("original_unit_id", id));
			SummonConversion? converted = null;
			if (conversion is not null)
			{
				var convertedSkills = master.Require("unit_skill_data", ("unit_id", conversion.Int("unit_id")));
				converted = new(convertedSkills.Int("union_burst_evolution"), convertedSkills.Int("main_skill_evolution_1"), convertedSkills.Int("main_skill_2"), convertedSkills.Int("ex_skill_evolution_1"));
			}
			definitions.Add(id, new(master.Require("unit_data", ("unit_id", id)).Int("prefab_id"), skills.Int("union_burst"), main,
				skills.Int("main_skill_1"), skills.Int("main_skill_evolution_1"), Enumerable.Range(1, 5).Select(index => skills.Int("ex_skill_" + index)).ToArray(), skills.Int("ex_skill_evolution_1"),
				master.Find("unlock_skill_data", ("unlock_skill", 301))?.Int("promotion_level"), converted, curves));
		}
		// Prepare extra_effect_data rows keyed by stage enemies; the processor selects by category and wave at setup.
		var stage = stageEnemyIds.ToHashSet();
		foreach (var row in master.Rows("extra_effect_data").Where(row => stage.Contains(row.Int("target_value_1"))))
			for (int index = 1; index <= 5; index++)
			{
				int id = row.Int("enemy_id_" + index);
				if (id != 0 && !enemies.ContainsKey(id) && master.Find("enemy_parameter", ("enemy_id", id)) is not null) enemies.Add(id, CalculateEnemy(id));
			}
		// Summon, extra-effect and difficulty rows share templates; Enemy(id) returns a fresh product for each birth.
		foreach (var enemy in additionalEnemies) enemies.TryAdd(enemy.Unique.Id, enemy);
		return new(ActiveInput.ChangeRarityReleased, definitions, relations, enemies);
	}
	public PreparedAccountGrowth PrepareAccountGrowth()
	{
		var inventory = ActiveInput.ExEquipment.Values.Select(entry => new PreparedExtraEquipmentIdentity(entry.SerialId, entry.ExEquipmentId)).ToArray();
		return new(ActiveInput.ChangeRarityReleased, inventory);
	}
}
