using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elements;

namespace OpenPCR.Growth;

/// <summary>Skill slot of a raw unit.</summary>
public sealed record RawSkill(int SkillId, int SkillLevel, int SlotNumber = 0);

/// <summary>Normal or unique equipment slot of a raw unit.</summary>
public sealed record RawEquipSlot(int Id, bool IsSlot, int EnhancementLevel, int EnhancementPt = 0, int Rank = 0);

/// <summary>EX equipment sub-status entry.</summary>
public sealed record RawExSubStatus(int SlotNumber, int Status, int Step, int IsLock = 0);

/// <summary>EX equipment slot of a raw unit; the equipment itself is resolved through the serial or the direct ID.</summary>
public sealed record RawExEquipSlot(int SerialId, int ExEquipmentId, int EnhancementPt, IReadOnlyList<RawExSubStatus>? SubStatus, int AlcesFlag = 0);

/// <summary>Rarity-6 unlock slots.</summary>
public sealed record RawUnlockRarity6(int QuestClear, int Slot1, int Slot2, int Slot3, int Status1, int Status2, int Status3);

/// <summary>Skin selection.</summary>
public sealed record RawSkin(int IconSkinId, int SdSkinId, int StillSkinId, int MotionId);

/// <summary>Server-supplied short status (BonusParam) of an NPC or support unit.</summary>
public sealed record RawStatusShort(long Hp, int Atk, int Def, int Matk, int Mdef, int Crt, int Mcrt, int Hrec, int Erec, int HrecRate,
	int Pnt, int Mpnt, int LifeSteal, int Dodge, int ErecRate, int EredRate, int Accuracy)
{
	internal StatusParamShort ToNative()
	{
		var result = new StatusParamShort();
		foreach (var property in typeof(RawStatusShort).GetProperties())
		{
			var target = typeof(StatusParamShort).GetProperty(property.Name) ?? throw new InvalidDataException("Unknown StatusParamShort." + property.Name);
			RuntimeBinding.Set(result, property.Name, RuntimeBinding.Wrap(Convert.ToInt64(property.GetValue(this)), target.PropertyType));
		}
		return result;
	}
}

/// <summary>
/// Raw growth of one unit: the fields the calculators consume. Owned units, Labyrinth NPCs, expanded supports and
/// opponents share this type. Calculated products (parameters, power, bonus from stories) are never part of it.
/// </summary>
public sealed record RawUnit(int Id, int UnitLevel, int UnitRarity, int BattleRarity, int PromotionLevel, int ExceedStage,
	int ResistStatusId, int ResistVariationId, RawUnlockRarity6? UnlockRarity6,
	IReadOnlyList<RawEquipSlot>? EquipSlots, IReadOnlyList<RawEquipSlot>? UniqueEquipSlots,
	IReadOnlyList<RawExEquipSlot>? ExEquipSlots, IReadOnlyList<RawExEquipSlot>? CbExEquipSlots,
	IReadOnlyList<RawSkill> UnionBurst, IReadOnlyList<RawSkill> MainSkill, IReadOnlyList<RawSkill> ExSkill, IReadOnlyList<RawSkill> FreeSkill,
	RawSkin? Skin)
{
	/// <summary>A fresh mutable Native UnitData; every call returns an independent object graph.</summary>
	internal UnitData ToNative()
	{
		var result = new UnitData();
		result.SetId(Id);
		result.SetUnitLevel(UnitLevel);
		result.SetUnitRarity(UnitRarity);
		result.SetBattleRarity(BattleRarity);
		result.SetPromotionLevel((ePromotionLevel)PromotionLevel);
		result.SetExceedStage(ExceedStage);
		result.SetResistStatusId(ResistStatusId);
		result.SetResistVariationId(ResistVariationId);
		result.SetEquipSlot(EquipSlots?.Select(Native).ToList()!);
		result.SetUniqueEquipSlot(UniqueEquipSlots?.Select(Native).ToList()!);
		result.SetExEquipSlot(ExEquipSlots?.Select(Native).ToList()!);
		result.SetCbExEquipSlot(CbExEquipSlots?.Select(Native).ToList()!);
		RuntimeBinding.Set(result, "UnionBurst", UnionBurst.Select(Native).ToList());
		RuntimeBinding.Set(result, "MainSkill", MainSkill.Select(Native).ToList());
		RuntimeBinding.Set(result, "ExSkill", ExSkill.Select(Native).ToList());
		RuntimeBinding.Set(result, "FreeSkill", FreeSkill.Select(Native).ToList());
		if (UnlockRarity6 is { } unlock)
			RuntimeBinding.Set(result, "UnlockRarity6Item", Build<UnlockRarity6Slot>(("QuestClear", unlock.QuestClear), ("Slot1", unlock.Slot1), ("Slot2", unlock.Slot2),
				("Slot3", unlock.Slot3), ("Status1", unlock.Status1), ("Status2", unlock.Status2), ("Status3", unlock.Status3)));
		result.SetSkinData(Skin is { } skin ? Build<SkinData>(("IconSkinId", skin.IconSkinId), ("SdSkinId", skin.SdSkinId), ("StillSkinId", skin.StillSkinId), ("MotionId", skin.MotionId)) : null!);
		return result;
	}
	internal void Validate()
	{
		if (Id <= 0 || UnitLevel <= 0 || UnitRarity <= 0) throw new InvalidDataException("Raw unit requires positive ID, level and rarity: " + Id);
		if (UnionBurst is null || MainSkill is null || ExSkill is null || FreeSkill is null) throw new InvalidDataException("Raw unit skill lists cannot be null: " + Id);
	}
	private static EquipSlot Native(RawEquipSlot slot) => Build<EquipSlot>(("Id", slot.Id), ("IsSlot", slot.IsSlot), ("EnhancementLevel", slot.EnhancementLevel),
		("EnhancementPt", slot.EnhancementPt), ("Rank", slot.Rank));
	internal static ExtraEquipSlot Native(RawExEquipSlot slot)
	{
		var result = Build<ExtraEquipSlot>(("SerialId", slot.SerialId), ("ExEquipmentId", slot.ExEquipmentId), ("EnhancementPt", slot.EnhancementPt), ("AlcesFlag", slot.AlcesFlag));
		RuntimeBinding.Set(result, "SubStatus", slot.SubStatus?.Select(Native).ToList());
		return result;
	}
	internal static ExtraEquipSubStatus Native(RawExSubStatus sub) => Build<ExtraEquipSubStatus>(("SlotNumber", sub.SlotNumber), ("Status", sub.Status), ("Step", sub.Step), ("IsLock", sub.IsLock));
	private static SkillLevelInfo Native(RawSkill skill) => Build<SkillLevelInfo>(("SkillId", skill.SkillId), ("SkillLevel", skill.SkillLevel), ("SlotNumber", skill.SlotNumber));
	private static T Build<T>(params (string Name, object Value)[] values) where T : new()
	{
		var result = new T();
		foreach (var (name, value) in values)
		{
			var property = typeof(T).GetProperty(name) ?? throw new InvalidDataException("Unknown " + typeof(T).Name + "." + name);
			RuntimeBinding.Set(result, name, RuntimeBinding.Wrap(value is bool flag ? flag : Convert.ToInt64(value), property.PropertyType));
		}
		return result;
	}
}

/// <summary>Light support payload: skill levels and equipment points only; IDs and levels come from masterdata.</summary>
public sealed record RawSupportUnit(int Id, int UnitLevel, int UnitRarity, int BattleRarity, int PromotionLevel,
	int ResistStatusId, int ResistVariationId, RawUnlockRarity6? UnlockRarity6,
	IReadOnlyList<int> UnionBurstLevels, IReadOnlyList<int> MainSkillLevels, IReadOnlyList<int> ExSkillLevels,
	IReadOnlyList<RawSupportEquipSlot> EquipSlots, IReadOnlyList<RawSupportEquipSlot> UniqueEquipSlots,
	IReadOnlyList<RawExEquipSlot>? ExEquipSlots, IReadOnlyList<RawExEquipSlot>? CbExEquipSlots, RawSkin? Skin, RawStatusShort? BonusParam);

/// <summary>Light equipment slot of a support payload.</summary>
public sealed record RawSupportEquipSlot(bool IsSlot, int EnhancementPt, int Rank);

/// <summary>EX equipment inventory entry.</summary>
public sealed record RawExEquipment(int SerialId, int ExEquipmentId, int EnhancementPt, int Rank, IReadOnlyList<RawExSubStatus>? SubStatus);

/// <summary>Princess-knight talent level.</summary>
public sealed record RawTalentLevel(int TalentId, long TotalPoint);

/// <summary>Talent or team skill node and its enhancement level.</summary>
public sealed record RawSkillNode(int NodeId, int EnhanceLevel);

/// <summary>Princess-knight investment.</summary>
public sealed record RawPrincessKnight(IReadOnlyList<RawTalentLevel> TalentLevels, IReadOnlyList<RawSkillNode> LastEnhancedPageNodes, RawSkillNode? TeamSkillLatestNode);

/// <summary>Role mastery investment of one role: slot and enhancement levels of slots 1 to 4.</summary>
public sealed record RawUnitRole(int UnitRoleId, IReadOnlyList<int> SlotLevels, IReadOnlyList<int> EnhanceLevels);

/// <summary>Team enhancement investment of one side.</summary>
public sealed record RawTeamEnhance(RawPrincessKnight? PrincessKnight, IReadOnlyList<RawUnitRole> Roles);

/// <summary>Typed growth inputs of one player side plus the optional selected support.</summary>
public sealed record GrowthInputs(string MasterVersion, int TeamLevel, bool ChangeRarityReleased, bool SkillLevelSyncReleased,
	IReadOnlyList<int> ReadStoryIds, IReadOnlyDictionary<int, RawUnit> Units, IReadOnlyDictionary<int, RawExEquipment> ExEquipment,
	RawTeamEnhance? Team, RawTeamEnhance? EnemyTeam, RawSupportUnit? Support = null)
{
	internal void Validate()
	{
		if (string.IsNullOrEmpty(MasterVersion)) throw new InvalidDataException("Growth inputs require a master version.");
		if (TeamLevel <= 0) throw new InvalidDataException("TeamLevel must be positive.");
		if (ReadStoryIds is null || Units is null || ExEquipment is null) throw new InvalidDataException("Story, unit and EX inventory inputs must be explicit.");
		foreach (var (id, unit) in Units)
		{
			if (unit is null || unit.Id != id) throw new InvalidDataException("Raw unit key/Id differ: " + id);
			unit.Validate();
		}
		foreach (var (serial, item) in ExEquipment)
			if (item is null || item.SerialId != serial) throw new InvalidDataException("EX inventory key/serial differ: " + serial);
	}
}
