using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using Elements;

namespace OpenPCR.Growth;

/// <summary>Native 11.7.2 parameter producers with explicit account and database inputs.</summary>
public sealed partial class GrowthCalculator : IDisposable
{
	private GrowthInputs input;
	private GrowthMasterData master;
	private GrowthInputs ActiveInput => input ?? throw new ObjectDisposedException(nameof(GrowthCalculator));
	internal static readonly string[] Columns = { "hp", "atk", "def", "magic_str", "magic_def", "physical_critical", "magic_critical", "dodge", "life_steal", "wave_hp_recovery", "wave_energy_recovery", "physical_penetrate", "magic_penetrate", "energy_recovery_rate", "hp_recovery_rate", "energy_reduce_rate", "accuracy" };
	internal static readonly string[] Fields = { "Hp", "Atk", "Def", "MagicStr", "MagicDef", "PhysicalCritical", "MagicCritical", "Dodge", "LifeSteal", "WaveHpRecovery", "WaveEnergyRecovery", "PhysicalPenetrate", "MagicPenetrate", "EnergyRecoveryRate", "HpRecoveryRate", "EnergyReduceRate", "Accuracy" };
	internal static readonly string[] ShortFields = { "Hp", "Atk", "Def", "Matk", "Mdef", "Crt", "Mcrt", "Dodge", "LifeSteal", "Hrec", "Erec", "Pnt", "Mpnt", "ErecRate", "HrecRate", "EredRate", "Accuracy" };
	public GrowthCalculator(GrowthInputs input, Microsoft.Data.Sqlite.SqliteConnection database, string masterVersion)
	{
		ArgumentNullException.ThrowIfNull(input);
		input.Validate();
		this.input = input;
		if (masterVersion != input.MasterVersion) throw new InvalidDataException("Growth input/masterdata version differs.");
		master = new GrowthMasterData(database);
	}
	public void Dispose() { input = null!; master = null!; }
	public PreparedUnit CalculateUnit(int originalUnitId, bool isClanSupportOrReplay = false)
	{
		var unique = (ActiveInput.Units.TryGetValue(originalUnitId, out var raw) ? raw : throw new KeyNotFoundException("Unit is not in the growth inputs: " + originalUnitId)).ToNative();
		AddCharaStoryBonusParamEachUnit(unique);
		return CalculateUnit(unique, isClanSupportOrReplay);
	}
	private PreparedUnit CalculateUnit(UnitData unique, bool isClanSupportOrReplay)
	{
		int originalUnitId = unique.Id;
		CalcParamAndSkill(unique);
		var limited = Limited(unique, isClanSupportOrReplay);
		// Login state keeps the original identity; UnitUiCtrl.Initialize applies the rarity-6 conversion.
		int battleId = Conversion(originalUnitId, unique.UnitRarity);
		var metadata = master.Require("unit_data", ("unit_id", battleId));
		int prefabId = metadata.Int("prefab_id");
		return new PreparedUnit(unique, limited, Prepare(unique, true), Prepare(limited, true), prefabId, isClanSupportOrReplay);
	}
	/// <summary>UnitParameter.UpdateLimitedUnitData: copy, team-level cap, skill-level sync, CalcParamAndSkill.</summary>
	private UnitData Limited(UnitData unique, bool isClanSupportOrReplay)
	{
		var limited = CopyForGrowth(unique);
		limited.SetExEquipSlot(new List<ExtraEquipSlot>(unique.CbExEquipSlot));
		int level = unique.UnitLevel;
		if (!isClanSupportOrReplay)
		{
			level = Math.Min(level, ActiveInput.TeamLevel);
			limited.SetUnitLevel(level);
		}
		bool sync = ActiveInput.SkillLevelSyncReleased;
		var ub = limited.UnionBurst[0];
		ub.SetSkillLevel(sync ? level : Math.Min(ub.SkillLevel, level));
		for (int i = 0; i < Math.Min(limited.MainSkill.Count, 2); i++) Sync(limited.MainSkill[i]);
		if (limited.ExSkill.Count > 0) Sync(limited.ExSkill[0]);
		void Sync(SkillLevelInfo skill) => skill.SetSkillLevel(sync && (int)skill.SkillLevel >= 1 ? level : Math.Min(skill.SkillLevel, level));
		CalcParamAndSkill(limited);
		return limited;
	}
	/// <summary>
	/// UpdateLimitedUnitData on the copy MightConvertRarity6Unit makes of <paramref name="unique"/>: the copy carries the
	/// converted identity, so the refresh runs on that identity. Null when the unit has no rarity-6 conversion.
	/// </summary>
	public (UnitData Limited, OpenPCR.PreparedUnitInputs Inputs)? CalculateConvertedLimited(UnitData unique, bool isClanSupportOrReplay)
	{
		int converted = Conversion(unique.Id, unique.UnitRarity);
		if (converted == (int)unique.Id) return null;
		var copy = CopyForGrowth(unique); copy.SetId(converted);
		var limited = Limited(copy, isClanSupportOrReplay);
		return (limited, Prepare(limited, true));
	}
	internal int Original(int id)
	{
		if ((unchecked((uint)(id - 170300)) >> 2) >= 0x979U) return id;
		return master.Find("unit_conversion", ("unit_id", id))?.Int("original_unit_id") ?? id;
	}
	internal int Conversion(int id, int rarity)
	{
		if (rarity < 6) return id;
		var row = master.Find("unit_conversion", ("original_unit_id", id));
		return row?.Int("unit_id") ?? id;
	}
	internal int Rarity(UnitData unit) => (int)unit.UnitRarity != 5 || !ActiveInput.ChangeRarityReleased || (int)unit.BattleRarity == 0 ? (int)unit.UnitRarity : (int)unit.BattleRarity;
	// UnitUtility.round: truncate, half threshold, relative double epsilon.
	internal static int round(double value)
	{
		int truncated = OpenPCR.NativeNumeric.ToInt32(value);
		bool approximate = Math.Abs(value - (truncated + 0.5)) < Math.Max(1E-14 * Math.Max(Math.Abs(value), Math.Abs(truncated + 0.5)), 2.220446049250313E-16);
		return value - truncated >= 0.5 || approximate ? unchecked(truncated + 1) : truncated;
	}
	private static double DecimalDouble(float value) => (double)(decimal)value;
	// Native CreateCopyData, before any immutable prepared binding exists.
	internal static UnitData CopyForGrowth(UnitData source) => PreparedParameterProducts.Clone(source);
	internal static SkillLevelInfo Skill(int id, int level, int slot = 0)
	{
		var result = new SkillLevelInfo(); result.SetSkillId(id); result.SetSkillLevel(level); result.SetSlotNumber(slot); return result;
	}
	private double Float(GrowthMasterData.Row? row, int type) => row?.Double(Columns[type - 1]) ?? 0;
	internal int CalcUnitBaseParameter(int type, int id, int level, int rarity, int rank)
	{
		if (type is < 1 or > 17) return 0;
		id = Original(id);
		double promotion = rank > 1 ? Float(master.Require("unit_promotion_status", ("unit_id", id), ("promotion_level", rank)), type) : 0;
		var row = master.Find("unit_rarity", ("unit_id", id), ("rarity", rarity));
		double growth = row?.Double(Columns[type - 1] + "_growth") ?? 0;
		double initial = Float(row, type);
		double rankGrowth = type <= 6 ? growth * rank : 0;
		return round(promotion + growth * level + initial + rankGrowth + DecimalDouble((float)Float(master.Find("promotion_bonus", ("unit_id", id), ("promotion_level", rank)), type)));
	}
	internal int calcUnitBaseParameterWithoutPromotionBonusRank(int type, int id, int level, int rarity, int rank)
	{
		if (type is < 1 or > 17) return 0;
		id = Original(id);
		int v = rank + 1; bool has;
		do { has = master.Find("promotion_bonus", ("unit_id", id), ("promotion_level", v - 2)) is not null; --v; } while (has);
		double promotion = v >= 2 ? Float(master.Require("unit_promotion_status", ("unit_id", id), ("promotion_level", v)), type) : 0;
		var row = master.Find("unit_rarity", ("unit_id", id), ("rarity", rarity));
		double growth = row?.Double(Columns[type - 1] + "_growth") ?? 0;
		double initial = Float(row, type);
		double rankGrowth = type <= 6 ? growth * v : 0;
		return round(promotion + growth * level + initial + rankGrowth);
	}
	private double calcEquipParameterWithoutUniqueEquip(int type, List<EquipSlot>? slots)
	{
		double sum = 0;
		foreach (var slot in slots ?? new List<EquipSlot>())
		{
			if (!slot.IsSlot || (int)slot.Id == 999999) continue;
			var row = master.Find("equipment_data", ("equipment_id", slot.Id));
			if (row is null) continue;
			double baseValue = DecimalDouble(row.Float(Columns[type - 1]));
			var rate = master.Find("equipment_enhance_rate", ("equipment_id", slot.Id));
			double enhancement = rate is null ? 0 : DecimalDouble(rate.Float(Columns[type - 1])) * (int)slot.EnhancementLevel;
			// Parentheses follow the three FADD D instructions, not an associative sum.
			sum = sum + (baseValue + Math.Ceiling(enhancement));
		}
		return sum;
	}
	internal int CalcEquipParameter(int type, UnitData unit)
	{
		double sum = calcEquipParameterWithoutUniqueEquip(type, unit.EquipSlot);
		foreach (var slot in unit.UniqueEquipSlot ?? new List<EquipSlot>())
		{
			if (!slot.IsSlot || (int)slot.Id == 999999) continue;
			int id = slot.Id, level = slot.EnhancementLevel;
			var row = master.Find("unique_equipment_data", ("equipment_id", id));
			if (row is null) continue;
			double baseValue = DecimalDouble(row.Float(Columns[type - 1]));
			var normal = master.Find("equipment_enhance_rate", ("equipment_id", id));
			var segments = master.Rows("unique_equip_enhance_rate").Where(r => r.Int("equipment_id") == id).ToArray();
			var segment = normal is null ? segments.OrderByDescending(r => r.Int("min_lv")).FirstOrDefault(r => r.Int("min_lv") - 1 <= level) : null;
			double addition;
			if (segment is not null)
			{
				double start = 0;
				foreach (var previous in segments)
					if (previous.Int("max_lv") != -1 && previous.Int("max_lv") < segment.Int("min_lv"))
						start += DecimalDouble(previous.Float(Columns[type - 1]) * (previous.Int("max_lv") - previous.Int("min_lv") + 1));
				var bonus = master.Rows("unique_equipment_bonus").Where(r => r.Int("equipment_id") == id).OrderByDescending(r => r.Int("min_lv")).FirstOrDefault(r => r.Int("min_lv") <= level);
				float product = segment.Float(Columns[type - 1]) * (level - segment.Int("min_lv") + 1);
				float inner = (bonus?.Float(Columns[type - 1]) ?? 0f) + product;
				addition = Math.Ceiling(start + DecimalDouble(inner));
			}
			else
			{
				var rate = normal ?? master.Find("unique_equipment_enhance_rate", ("equipment_id", id));
				addition = Math.Ceiling(rate is null ? 0 : DecimalDouble(rate.Float(Columns[type - 1])) * (level + (normal is null ? -1 : 0)));
			}
			sum = sum + (baseValue + addition);
		}
		return round(sum);
	}
	private void CalcParamAndSkill(UnitData unit, int? baseLevel = null, int? baseRarity = null, int? baseRank = null)
	{
		var parameter = new UnitParam();
		parameter.SetBaseParam(Status(type => CalcUnitBaseParameter(type, unit.Id, baseLevel ?? (int)unit.UnitLevel, baseRarity ?? Rarity(unit), baseRank ?? (int)unit.PromotionLevel)));
		parameter.SetEquipParam(Status(type => CalcEquipParameter(type, unit)));
		unit.SetUnitParam(parameter);
		var skills = master.Require("unit_skill_data", ("unit_id", unit.Id));
		if (unit.ExSkill.Count > 0 && (int)unit.ExSkill[0].SkillLevel > 0) unit.ExSkill[0].SetSkillId(skills.Int(Rarity(unit) >= 5 ? "ex_skill_evolution_1" : "ex_skill_1"));
		int converted = Conversion(unit.Id, unit.UnitRarity);
		if (converted != (int)unit.Id) SetConversionSkill(unit, converted);
	}
	private void SetConversionSkill(UnitData unit, int id)
	{
		var skills = master.Require("unit_skill_data", ("unit_id", id));
		unit.UnionBurst[0].SetSkillId(skills.Int("union_burst_evolution"));
		while (unit.MainSkill.Count < 2) unit.MainSkill.Add(new SkillLevelInfo());
		unit.MainSkill[0].SetSkillId(skills.Int("main_skill_evolution_1")); unit.MainSkill[0].SetSlotNumber(0);
		bool second = unit.UniqueEquipSlot is { Count: > 1 } && unit.UniqueEquipSlot[1].IsSlot;
		unit.MainSkill[1].SetSkillId(skills.Int(second ? "main_skill_evolution_2" : "main_skill_2")); unit.MainSkill[1].SetSlotNumber(1);
		if (unit.ExSkill.Count == 0) unit.ExSkill.Add(new SkillLevelInfo());
		unit.ExSkill[0].SetSkillId(skills.Int("ex_skill_evolution_1")); unit.ExSkill[0].SetSlotNumber(0);
	}
	private void AddCharaStoryBonusParamEachUnit(UnitData unit)
	{
		unit.SetBonusParam(null!);
		int chara = (int)unit.Id / 100;
		foreach (var story in ViewedStories())
		{
			if (System.Array.IndexOf(story.Charas, chara) < 0) { story.Failure?.Throw(); continue; }
			var row = story.Row;
			for (int i = 1; i <= 5; i++)
			{
				int type = row.Int("status_type_" + i), value = row.Int("status_rate_" + i);
				if (type == 0 || value < 1) continue;
				if (unit.BonusParam is null) unit.SetBonusParam(new StatusParamShort());
				if (type is < 1 or > 17) continue;
				long old = ReadStatus(unit.BonusParam!, type, true);
				var property = typeof(StatusParamShort).GetProperty(ShortFields[type - 1])!;
				RuntimeBinding.Set(unit.BonusParam!, property.Name, RuntimeBinding.Wrap(type == 1 ? unchecked(old + value) : unchecked((int)old + value), property.PropertyType));
			}
		}
	}
	/// <summary>A viewed story row with the chara_id_1..20 values read in column order, up to the first column that fails to read.</summary>
	private sealed record ViewedStory(GrowthMasterData.Row Row, int[] Charas, ExceptionDispatchInfo? Failure);
	private static readonly string[] CharaColumns = Enumerable.Range(1, 20).Select(i => "chara_id_" + i).ToArray();
	private ViewedStory[]? viewedStories;
	/// <summary>
	/// ViewingStoryIdList rows, read once per calculator in list order. A unit matches a row when its chara appears before the
	/// failing column; otherwise that column's error is raised where the per-unit column scan would have raised it.
	/// </summary>
	private ViewedStory[] ViewedStories()
	{
		if (viewedStories is not null) return viewedStories;
		var stories = new List<ViewedStory>();
		foreach (int id in ActiveInput.ReadStoryIds)
		{
			var row = master.Find("chara_story_status", ("story_id", id));
			if (row is null) continue;
			var charas = new List<int>(CharaColumns.Length); ExceptionDispatchInfo? failure = null;
			foreach (string column in CharaColumns)
			{
				try { charas.Add(row.Int(column)); }
				catch (Exception error) { failure = ExceptionDispatchInfo.Capture(error); break; }
			}
			stories.Add(new(row, charas.ToArray(), failure));
		}
		return viewedStories = stories.ToArray();
	}
	internal static StatusParam Status(Func<int, long> value)
	{
		var result = new StatusParam();
		for (int type = 1; type <= 17; type++) RuntimeBinding.Call(result, "Set" + Fields[type - 1], type == 1 ? (object)value(type) : checked((int)value(type)));
		return result;
	}
	internal static long ReadStatus(object value, int type, bool shortNames = false)
	{
		object scalar = value.GetType().GetProperty((shortNames ? ShortFields : Fields)[type - 1])!.GetValue(value)!;
		return type == 1 ? (long)scalar : (int)scalar;
	}
	private long Unlock(UnitData unit, int type, bool damaged)
	{
		if (unit.UnlockRarity6Item is null || Rarity(unit) != 5) return 0;
		long sum = 0;
		for (int slot = 1; slot <= 3; slot++)
		{
			int level = (global::System.Int32)RuntimeBinding.Get(unit.UnlockRarity6Item, "Slot" + slot)!;
			if (damaged && level < 1) continue;
			var row = master.Find("unlock_rarity_6", ("unit_id", unit.Id), ("slot_id", slot), ("unlock_level", level));
			if (damaged && row is null) throw new InvalidDataException("Missing damaged-energy unlock row.");
			if (row is not null) sum += row.Int(Columns[type - 1]);
		}
		return sum;
	}
	internal long GetTotalParam(UnitData unit, int type)
	{
		long sum = unchecked(ReadStatus(unit.UnitParam.BaseParam, type) + ReadStatus(unit.UnitParam.EquipParam, type));
		if (unit.BonusParam is not null) sum = unchecked(sum + ReadStatus(unit.BonusParam, type, true));
		sum = unchecked(sum + Unlock(unit, type, false));
		if (type != 1) sum = unchecked((int)sum);
		if (unit.ExEquipSlot is not null)
		{
			int addition = 0;
			for (int slot = 0; slot < 3; slot++)
			{
				int ex = getEquipmentParam(unit.ExEquipSlot, slot, type);
				addition = unchecked(addition + (type <= 7 ? OpenPCR.NativeNumeric.ToInt32(Math.Ceiling((double)unchecked(sum * ex) / 10000.0)) : ex));
			}
			sum = type == 1 ? unchecked(sum + addition) : unchecked((int)sum + addition);
		}
		return Math.Max(sum, 0);
	}
	internal long CalcUnitParameterForDamagedEnergy(UnitData unit, int type)
	{
		long value = calcUnitBaseParameterWithoutPromotionBonusRank(type, unit.Id, unit.UnitLevel, Rarity(unit), (int)unit.PromotionLevel);
		if (master.Find("promotion_bonus", ("unit_id", Original(unit.Id)), ("promotion_level", (int)unit.PromotionLevel)) is null) value += round(calcEquipParameterWithoutUniqueEquip(type, unit.EquipSlot));
		return value + Unlock(unit, type, true);
	}
	internal OpenPCR.PreparedUnitInputs Prepare(UnitData unit, bool playable) => new(
		Enumerable.Range(1, 17).Select(type => GetTotalParam(unit, type)).ToArray(),
		playable ? CalcUnitParameterForDamagedEnergy(unit, 1) : GetTotalParam(unit, 1), playable ? CalcUnitParameterForDamagedEnergy(unit, 3) : GetTotalParam(unit, 3), playable ? CalcUnitParameterForDamagedEnergy(unit, 5) : GetTotalParam(unit, 5));
}
