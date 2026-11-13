using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using Elements;

namespace OpenPCR.Growth;

public sealed partial class GrowthCalculator
{
	internal int getEquipmentParam(List<ExtraEquipSlot> slots, int index, int type)
	{
		if (slots.Count == 0) return 0;
		var slot = slots[index];
		if ((int)slot.SerialId != 0)
		{
			if (!ActiveInput.ExEquipment.TryGetValue(slot.SerialId, out var info)) throw new InvalidDataException("Missing EX inventory serial: " + slot.SerialId);
			int id = info.ExEquipmentId;
			var row = master.Require("ex_equipment_data", ("ex_equipment_id", id));
			int value = GetExEquipmentIntValue(row, type, CalcExtraEquipEnhanceLv(row.Int("rarity"), info.EnhancementPt));
			if (row.Int("rarity") == 5)
				foreach (var sub in info.SubStatus ?? throw new InvalidDataException("EX inventory sub-status list is null: " + info.SerialId))
					if (sub.Status == type)
						value = unchecked(value + SubValue(master.Require("ex_equipment_sub_status_group", ("ex_equipment_id", id)).Int("group_id"), type, sub.Step));
			return value;
		}
		if ((int)slot.ExEquipmentId == 0) return 0;
		var other = master.Find("ex_equipment_data", ("ex_equipment_id", slot.ExEquipmentId));
		if (other is null) return 0;
		int result = GetExEquipmentIntValue(other, type, CalcExtraEquipEnhanceLv(other.Int("rarity"), slot.EnhancementPt));
		if (other.Int("rarity") > 4)
		{
			var group = master.Find("ex_equipment_sub_status_group", ("ex_equipment_id", slot.ExEquipmentId));
			if (group is not null && slot.SubStatus is not null)
				foreach (var sub in slot.SubStatus) if ((int)sub.Status == type) result = unchecked(result + SubValue(group.Int("group_id"), type, sub.Step));
		}
		return result;
	}
	private Dictionary<int, GrowthMasterData.Row[]>? enhanceRows;
	/// <summary>ex_equipment_enhance_data rows of one rarity in table order, grouped once per calculator.</summary>
	private GrowthMasterData.Row[] EnhanceRows(int rarity)
	{
		enhanceRows ??= master.Rows("ex_equipment_enhance_data").GroupBy(row => row.Int("rarity")).ToDictionary(group => group.Key, group => group.ToArray());
		return enhanceRows.TryGetValue(rarity, out var rows) ? rows : System.Array.Empty<GrowthMasterData.Row>();
	}
	private int CalcExtraEquipEnhanceLv(int rarity, int point)
	{
		if (rarity > 4) return 0;
		var rows = EnhanceRows(rarity);
		foreach (var row in rows) if (row.Int("total_point") > point) return row.Int("enhance_level") - 1;
		return rows.Last().Int("enhance_level");
	}
	private int GetExEquipmentIntValue(GrowthMasterData.Row row, int type, int enhance)
	{
		int value = row.Int("default_" + Columns[type - 1]);
		if (row.Int("rarity") == 5) return value;
		int max = EnhanceRows(row.Int("rarity")).Length;
		if (max == 0 || enhance < 0 || enhance > max) throw new InvalidDataException("EX enhancement outside Native cache range.");
		return OpenPCR.NativeNumeric.ToInt32(Math.Ceiling(((double)enhance / max) * unchecked(row.Int("max_" + Columns[type - 1]) - value) + value));
	}
	private int SubValue(int group, int type, int step) => step is >= 1 and <= 5 ? master.Require("ex_equipment_sub_status", ("group_id", group), ("status", type)).Int("value_" + step) : 0;
}
