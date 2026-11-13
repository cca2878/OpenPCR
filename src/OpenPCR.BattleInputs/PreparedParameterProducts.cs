using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Elements;

namespace OpenPCR.BattleInputs;

/// <summary>Detached parameter products and summon rules. Contains no calculator, JSON, database or callbacks.</summary>
public sealed class PreparedParameterProducts
{
	private readonly bool changeRarity;
	private readonly Dictionary<int, SummonDefinition> definitions;
	private readonly Dictionary<int, SummonActionSpec[]> actions;
	private readonly Dictionary<int, PreparedUnit> enemies;
	internal PreparedParameterProducts(bool changeRarity, Dictionary<int, SummonDefinition> definitions,
		Dictionary<int, SummonActionSpec[]> actions, Dictionary<int, PreparedUnit> enemies)
	{ this.changeRarity = changeRarity; this.definitions = definitions; this.actions = actions; this.enemies = enemies; }
	public IReadOnlyList<int> ResourceUnitIds => definitions.Keys.Concat(enemies.Keys).ToArray();
	public PreparedUnit Enemy(int id) => enemies.TryGetValue(id, out var product) ? Fresh(product) : throw new InvalidDataException("Missing prepared enemy definition: " + id);
	/// <summary>Native consumer-time assembly from prepared rules and the actual owner; no raw growth lookup.</summary>
	public PreparedUnit CalculateSummon(object owner, int id, int level, bool equipment, bool evolved)
	{
		if (owner is not UnitParameter parameter) throw new InvalidDataException("Summon owner is not a UnitParameter.");
		var source = parameter.UniqueData;
		int rarity = (int)source.UnitRarity == 5 && changeRarity && (int)source.BattleRarity != 0 ? (int)source.BattleRarity : (int)source.UnitRarity;
		var groups = new[] { source.UnionBurst, source.MainSkill, source.ExSkill,
			parameter.SkillData.SpSkillIds.Select(skill => Skill(skill, 0)).ToList(), source.FreeSkill };
		(int Level, int Rarity, int Rank)? selected = null;
		foreach (var group in groups)
		{
			foreach (var skill in group)
			{
				if ((int)skill.SkillId == 0 || !actions.TryGetValue(skill.SkillId, out var entries)) continue;
				foreach (var action in entries)
				{
					if (action.UnitId != id) continue;
					int value = action.EncodedRarityRank;
					selected = ((int)skill.SkillLevel, value / 100 == 0 ? rarity : value / 100, value % 100 == 0 ? (int)source.PromotionLevel : value % 100);
					break;
				}
				if (selected is not null) break;
			}
			if (selected is not null) break;
		}
		if (selected is not { } spec || !definitions.TryGetValue(id, out var definition)) throw new InvalidDataException("No prepared summon rule on owner for unit " + id);
		var data = new UnitData(); data.SetId(id); data.SetUnitLevel(level); data.SetUnitRarity(source.UnitRarity); data.SetBattleRarity(source.BattleRarity);
		data.SetMainSkill(definition.Main.Select((skill, index) => (Skill: skill, Index: index)).Where(entry => entry.Skill.Id != 0).Select(entry => Skill(entry.Index == 0 && evolved && definition.EvolvedMain != 0 ? definition.EvolvedMain : entry.Skill.Id,
			entry.Skill.UnlockRank is { } rank && spec.Rank >= rank ? level : 0)).ToList());
		data.SetUnionBurst(definition.UnionBurst == 0 ? new List<SkillLevelInfo>() : new List<SkillLevelInfo> { Skill(definition.UnionBurst, level) });
		if (equipment && definition.ExUnlockRank is { } exRank && spec.Rank >= exRank)
			data.SetExSkill((rarity >= 5 ? new[] { definition.EvolvedEx } : definition.Ex).Where(skill => skill != 0).Select(skill => Skill(skill, level)).ToList());
		var part = new UnitParam();
		if (!definition.Curves.TryGetValue((spec.Rarity, spec.Rank), out var curves)) throw new InvalidDataException("Missing prepared summon growth curve: " + id + "/" + spec.Rarity + "/" + spec.Rank);
		part.SetBaseParam(Status(type => curves[type - 1].At(spec.Level))); part.SetEquipParam(new StatusParam()); data.SetUnitParam(part);
		if (data.ExSkill.Count > 0 && (int)data.ExSkill[0].SkillLevel > 0) data.ExSkill[0].SetSkillId(rarity >= 5 ? definition.EvolvedEx : definition.Ex[0]);
		if ((int)source.UnitRarity >= 6 && definition.Conversion is { } conversion)
		{
			data.UnionBurst[0].SetSkillId(conversion.UnionBurst);
			while (data.MainSkill.Count < 2) data.MainSkill.Add(new SkillLevelInfo());
			data.MainSkill[0].SetSkillId(conversion.Main1); data.MainSkill[0].SetSlotNumber(0);
			data.MainSkill[1].SetSkillId(conversion.Main2); data.MainSkill[1].SetSlotNumber(1);
			if (data.ExSkill.Count == 0) data.ExSkill.Add(new SkillLevelInfo());
			data.ExSkill[0].SetSkillId(conversion.Ex); data.ExSkill[0].SetSlotNumber(0);
		}
		if (equipment) { data.SetBonusParam(source.BonusParam); data.UnitParam.SetEquipParam(source.UnitParam.EquipParam); }
		var totals = Enumerable.Range(1, 17).Select(type =>
		{
			long value = unchecked(Read(data.UnitParam.BaseParam, type) + Read(data.UnitParam.EquipParam, type));
			if (data.BonusParam is not null) value = unchecked(value + Read(data.BonusParam, type, true));
			return Math.Max(type == 1 ? value : unchecked((int)value), 0);
		}).ToArray();
		var inputs = new PreparedUnitInputs(totals, totals[0], totals[2], totals[4]);
		return new PreparedUnit(data, Clone(data), inputs, inputs, definition.PrefabId);
	}
	private static PreparedUnit Fresh(PreparedUnit product)
	{
		var unique = Clone(product.Unique); var limited = Clone(product.Limited);
		UnitParam CopyPart(UnitParam original)
		{ var result = new UnitParam(); result.SetBaseParam(Status(type => Read(original.BaseParam, type))); result.SetEquipParam(Status(type => Read(original.EquipParam, type))); return result; }
		unique.SetUnitParam(CopyPart(product.Unique.UnitParam));
		limited.SetUnitParam(ReferenceEquals(product.Unique.UnitParam, product.Limited.UnitParam) ? unique.UnitParam : CopyPart(product.Limited.UnitParam));
		return product with { Unique = unique, Limited = limited };
	}
	/// <summary>Native CreateCopyData: shallow copy with fresh skill-level and extra-equipment lists.</summary>
	internal static UnitData Clone(UnitData source)
	{
		var result = RuntimeBinding.ShallowClone(source);
		foreach (string name in new[] { "UnionBurst", "MainSkill", "ExSkill" })
		{
			var skills = (List<SkillLevelInfo>?)RuntimeBinding.Get(source, name);
			if (skills is not null) RuntimeBinding.Set(result, name, skills.Select(skill => Skill(skill.SkillId, skill.SkillLevel, skill.SlotNumber)).ToList());
		}
		if (source.ExEquipSlot is not null) result.SetExEquipSlot(source.ExEquipSlot.Select(slot => new ExtraEquipSlot(slot)).ToList());
		return result;
	}
	private static SkillLevelInfo Skill(int id, int level, int slot = 0)
	{ var result = new SkillLevelInfo(); result.SetSkillId(id); result.SetSkillLevel(level); result.SetSlotNumber(slot); return result; }
	private static readonly string[] fields = { "Hp", "Atk", "Def", "MagicStr", "MagicDef", "PhysicalCritical", "MagicCritical", "Dodge", "LifeSteal", "WaveHpRecovery", "WaveEnergyRecovery", "PhysicalPenetrate", "MagicPenetrate", "EnergyRecoveryRate", "HpRecoveryRate", "EnergyReduceRate", "Accuracy" };
	private static readonly string[] shortFields = { "Hp", "Atk", "Def", "Matk", "Mdef", "Crt", "Mcrt", "Dodge", "LifeSteal", "Hrec", "Erec", "Pnt", "Mpnt", "ErecRate", "HrecRate", "EredRate", "Accuracy" };
	private static long Read(object value, int type, bool shortName = false)
	{
		var scalar = value.GetType().GetProperty((shortName ? shortFields : fields)[type - 1])!.GetValue(value)!;
		return type == 1 ? (long)scalar : (int)scalar;
	}
	private static StatusParam Status(Func<int, long> value)
	{
		var result = new StatusParam();
		for (int type = 1; type <= 17; type++) RuntimeBinding.Call(result, "Set" + fields[type - 1], type == 1 ? (object)value(type) : checked((int)value(type)));
		return result;
	}
}

internal sealed record SummonActionSpec(int UnitId, int EncodedRarityRank);
internal sealed record SummonSkillSpec(int Id, int? UnlockRank);
internal sealed record SummonConversion(int UnionBurst, int Main1, int Main2, int Ex);
internal sealed record SummonDefinition(int PrefabId, int UnionBurst, SummonSkillSpec[] Main, int FirstMain, int EvolvedMain,
	int[] Ex, int EvolvedEx, int? ExUnlockRank, SummonConversion? Conversion, Dictionary<(int Rarity, int Rank), PreparedGrowthCurve[]> Curves);
internal sealed record PreparedGrowthCurve(double Promotion, double Growth, double Initial, double RankGrowth, double PromotionBonus)
{
	internal int At(int level)
	{
		double value = Promotion + Growth * level + Initial + RankGrowth + PromotionBonus;
		int truncated = NativeNumeric.ToInt32(value);
		bool approximate = Math.Abs(value - (truncated + 0.5)) < Math.Max(1E-14 * Math.Max(Math.Abs(value), Math.Abs(truncated + 0.5)), 2.220446049250313E-16);
		return value - truncated >= 0.5 || approximate ? unchecked(truncated + 1) : truncated;
	}
}
