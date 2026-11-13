using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Elements;

namespace OpenPCR.Growth;

public sealed partial class GrowthCalculator
{
	/// <summary>Selected support as the trial support list holds it: original identity, Limited from UpdateLimitedUnitData(true, true).</summary>
	public PreparedUnit CalculateSupport(int id)
	{
		var payload = ActiveInput.Support ?? throw new InvalidDataException("Missing selected support server payload.");
		if (payload.Id != id) throw new InvalidDataException("Selected support identity differs.");
		// Expand the light support payload before calculating parameters and skills.
		var skills = master.Require("unit_skill_data", ("unit_id", id));
		List<(int Id, int Level)> Expand(IReadOnlyList<int> levels, int limit, Func<int, string> column)
		{
			var result = new List<(int, int)>();
			for (int i = 0; i < Math.Min(levels.Count, limit); i++)
			{
				int skillId = skills.Int(column(i)), level = levels[i];
				// Native setSkillLevelInfo omits both zero IDs and zero levels.
				if (skillId != 0 && level != 0) result.Add((skillId, level));
			}
			return result;
		}
		var unionBurst = Expand(payload.UnionBurstLevels, 1, _ => "union_burst");
		var mainSkill = Expand(payload.MainSkillLevels, 10, i => "main_skill_" + (i + 1));
		var exSkill = Expand(payload.ExSkillLevels, 5, i => "ex_skill_" + (i + 1));
		if (unionBurst.Count > 0 && payload.UnitRarity >= 6) unionBurst[0] = (skills.Int("union_burst_evolution"), unionBurst[0].Level);
		var promotion = master.Require("unit_promotion", ("unit_id", id), ("promotion_level", payload.PromotionLevel));
		var equipment = new List<RawEquipSlot>();
		for (int i = 0; i < payload.EquipSlots.Count; i++)
		{
			var light = payload.EquipSlots[i];
			int equipId = promotion.Int("equip_slot_" + (i + 1));
			var row = master.Find("equipment_data", ("equipment_id", equipId));
			int point = light.EnhancementPt;
			int level = row is null ? 0 : master.Rows("equipment_enhance_data").Where(r => r.Int("promotion_level") == row.Int("promotion_level") - 1 && r.Int("total_point") <= point).Select(r => r.Int("equipment_enhance_level")).Append(0).Max();
			equipment.Add(new RawEquipSlot(equipId, row is not null && light.IsSlot, level, light.EnhancementPt, light.Rank));
		}
		var unique = new List<RawEquipSlot>();
		for (int i = 0; i < payload.UniqueEquipSlots.Count; i++)
		{
			var row = master.Find("unit_unique_equip", ("unit_id", id), ("equip_slot", i + 1));
			if (row is null) continue;
			var light = payload.UniqueEquipSlots[i]; int point = light.EnhancementPt;
			int level = master.Rows("unique_equipment_enhance_data").Where(r => r.Int("equip_slot") == i + 1 && r.Int("total_point") <= point).Select(r => r.Int("enhance_level")).Append(i == 1 ? 0 : 1).Max();
			if (light.IsSlot)
			{
				if (i < mainSkill.Count && i < 2) mainSkill[i] = (skills.Int("main_skill_evolution_" + (i + 1)), mainSkill[i].Level);
				unique.Add(new RawEquipSlot(row.Int("equip_id"), true, level, light.EnhancementPt, light.Rank));
			}
			else unique.Add(new RawEquipSlot(row.Int("equip_id"), false, 0, 0, 0));
		}
		static RawSkill[] Skills(List<(int Id, int Level)> list) => list.Select(skill => new RawSkill(skill.Id, skill.Level)).ToArray();
		// Native support construction chooses the guild EX slots before CalcParamAndSkill.
		var raw = new RawUnit(payload.Id, payload.UnitLevel, payload.UnitRarity, payload.BattleRarity, payload.PromotionLevel, 0,
			payload.ResistStatusId, payload.ResistVariationId, payload.UnlockRarity6, equipment, unique, payload.CbExEquipSlots, payload.CbExEquipSlots,
			Skills(unionBurst), Skills(mainSkill), Skills(exSkill), Array.Empty<RawSkill>(), payload.Skin);
		var data = raw.ToNative();
		if (Rarity(data) != 5) RuntimeBinding.Set(data, "UnlockRarity6Item", null);
		if (payload.BonusParam is { } bonus) data.SetBonusParam(bonus.ToNative());
		return CalculateUnit(data, true);
	}
}
