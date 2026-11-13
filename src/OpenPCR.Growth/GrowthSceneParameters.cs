using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elements;

namespace OpenPCR.Growth;

public sealed partial class GrowthCalculator
{
	internal IEnumerable<int> ResourceUnitIds() => master.Rows("skill_action").Where(row => row.Int("action_type") == 15 && row.Int("action_detail_2") < 1000000).Select(row => row.Int("action_detail_2"));
	/// <summary>Enemy product from locked masterdata, with fresh mutable skills and parameter objects.</summary>
	public PreparedUnit CalculateEnemy(int enemyId)
		=> CalculateEnemy(master.Require("enemy_parameter", ("enemy_id", enemyId)), enemyId);
	/// <summary>Labyrinth enemy/hidden actor product from its dedicated table, before battle initialization.</summary>
	public PreparedUnit CalculateLabyrinthEnemy(int enemyId)
		=> CalculateEnemy(master.Require("labyrinth_enemy_parameter", ("enemy_id", enemyId)), enemyId);
	private PreparedUnit CalculateEnemy(GrowthMasterData.Row enemy, int enemyId)
	{
		int resourceId = enemy.Int("unit_id");
		var skills = master.Require("unit_skill_data", ("unit_id", resourceId));
		var data = new UnitData();
		data.SetId(enemyId); data.SetUnitLevel(enemy.Int("level")); data.SetUnitRarity(enemy.Int("rarity"));
		data.SetPromotionLevel((ePromotionLevel)enemy.Int("promotion_level"));
		data.SetResistStatusId(enemy.Int("resist_status_id")); data.SetResistVariationId(enemy.Int("resist_variation_id"));
		var parameter = new UnitParam();
		parameter.SetBaseParam(Status(type => enemy.Long(Columns[type - 1]))); parameter.SetEquipParam(new StatusParam()); data.SetUnitParam(parameter);
		data.SetMainSkill(Enumerable.Range(1, 10).Where(i => skills.Int("main_skill_" + i) != 0).Select(i => Skill(
			i <= 2 && enemy.Int("unique_equipment_flag_1") >= i ? skills.Int("main_skill_evolution_" + i) : skills.Int("main_skill_" + i), enemy.Int("main_skill_lv_" + i))).ToList());
		data.SetExSkill(Enumerable.Range(1, 5).Where(i => skills.Int("ex_skill_" + i) != 0).Select(i => Skill(skills.Int("ex_skill_" + i), enemy.Int("ex_skill_lv_" + i))).ToList());
		data.SetUnionBurst(skills.Int("union_burst") == 0 ? new List<SkillLevelInfo>() : new List<SkillLevelInfo> { Skill(skills.Int(enemy.Int("rarity") == 6 ? "union_burst_evolution" : "union_burst"), enemy.Int("union_burst_level")) });
		var totals = Prepare(data, false);
		var metadata = master.Find("unit_data", ("unit_id", resourceId)) ?? master.Require("unit_enemy_data", ("unit_id", resourceId));
		return new PreparedUnit(data, CopyForGrowth(data), totals, totals, metadata.Int("prefab_id"));
	}
}
