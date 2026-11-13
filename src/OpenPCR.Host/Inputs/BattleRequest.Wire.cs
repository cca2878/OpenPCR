using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenPCR.Host;

// Field names follow the game's server responses only to ease adoption; OpenPCR declares the fields it consumes.
// Undeclared fields of these objects are ignored. Missing optional fields take the client UnitData parse defaults.

/// <summary>Skill entry of a unit (<c>union_burst</c>, <c>main_skill</c>, <c>ex_skill</c>, <c>free_skill</c>).</summary>
public sealed record WireSkill(
	[property: JsonPropertyName("skill_id"), JsonRequired] int SkillId,
	[property: JsonPropertyName("skill_level"), JsonRequired] int SkillLevel,
	[property: JsonPropertyName("slot_number")] int SlotNumber = 0);

/// <summary>Normal or unique equipment slot.</summary>
public sealed record WireEquipSlot(
	[property: JsonPropertyName("is_slot"), JsonRequired] JsonElement IsSlot,
	[property: JsonPropertyName("id")] int Id = 0,
	[property: JsonPropertyName("enhancement_level")] int EnhancementLevel = 0,
	[property: JsonPropertyName("enhancement_pt")] int EnhancementPt = 0,
	[property: JsonPropertyName("rank")] int Rank = 0);

/// <summary>EX equipment sub-status entry.</summary>
public sealed record WireExSubStatus(
	[property: JsonPropertyName("status"), JsonRequired] int Status,
	[property: JsonPropertyName("step"), JsonRequired] int Step,
	[property: JsonPropertyName("slot_number")] int SlotNumber = 0,
	[property: JsonPropertyName("is_lock")] JsonElement? IsLock = null);

/// <summary>EX equipment slot of a unit.</summary>
public sealed record WireExEquipSlot(
	[property: JsonPropertyName("serial_id")] int SerialId = 0,
	[property: JsonPropertyName("ex_equipment_id")] int ExEquipmentId = 0,
	[property: JsonPropertyName("enhancement_pt")] int EnhancementPt = 0,
	[property: JsonPropertyName("sub_status")] WireExSubStatus[]? SubStatus = null);

/// <summary>Skin selection.</summary>
public sealed record WireSkin(
	[property: JsonPropertyName("icon_skin_id")] int IconSkinId = 0,
	[property: JsonPropertyName("sd_skin_id")] int SdSkinId = 0,
	[property: JsonPropertyName("still_skin_id")] int StillSkinId = 0,
	[property: JsonPropertyName("motion_id")] int MotionId = 0);

/// <summary>Rarity-6 unlock slots.</summary>
public sealed record WireUnlockRarity6(
	[property: JsonPropertyName("quest_clear")] int QuestClear = 0,
	[property: JsonPropertyName("slot_1")] int Slot1 = 0,
	[property: JsonPropertyName("slot_2")] int Slot2 = 0,
	[property: JsonPropertyName("slot_3")] int Slot3 = 0,
	[property: JsonPropertyName("status_1")] int Status1 = 0,
	[property: JsonPropertyName("status_2")] int Status2 = 0,
	[property: JsonPropertyName("status_3")] int Status3 = 0);

/// <summary>Raw growth of one unit, as in <c>load/index</c> <c>unit_list</c> or <c>labyrinth/resume</c> <c>unit_data</c>.</summary>
public sealed record WireUnit(
	[property: JsonPropertyName("id"), JsonRequired] int Id,
	[property: JsonPropertyName("unit_rarity"), JsonRequired] int UnitRarity,
	[property: JsonPropertyName("unit_level"), JsonRequired] int UnitLevel,
	[property: JsonPropertyName("promotion_level"), JsonRequired] int PromotionLevel,
	[property: JsonPropertyName("union_burst"), JsonRequired] WireSkill[] UnionBurst,
	[property: JsonPropertyName("main_skill"), JsonRequired] WireSkill[] MainSkill,
	[property: JsonPropertyName("ex_skill"), JsonRequired] WireSkill[] ExSkill,
	[property: JsonPropertyName("free_skill"), JsonRequired] WireSkill[] FreeSkill,
	[property: JsonPropertyName("equip_slot"), JsonRequired] WireEquipSlot[] EquipSlot,
	[property: JsonPropertyName("battle_rarity")] int BattleRarity = 0,
	[property: JsonPropertyName("exceed_stage")] int ExceedStage = 0,
	[property: JsonPropertyName("unique_equip_slot")] WireEquipSlot[]? UniqueEquipSlot = null,
	[property: JsonPropertyName("ex_equip_slot")] WireExEquipSlot[]? ExEquipSlot = null,
	[property: JsonPropertyName("cb_ex_equip_slot")] WireExEquipSlot[]? CbExEquipSlot = null,
	[property: JsonPropertyName("skin_data")] WireSkin? SkinData = null,
	[property: JsonPropertyName("unlock_rarity_6_item")] WireUnlockRarity6? UnlockRarity6Item = null,
	[property: JsonPropertyName("resist_status_id")] int ResistStatusId = 0,
	[property: JsonPropertyName("resist_variation_id")] int ResistVariationId = 0,
	[property: JsonPropertyName("bonus_param")] JsonElement? BonusParam = null);

/// <summary>Light skill entry of a support payload: only the level.</summary>
public sealed record WireLightSkill([property: JsonPropertyName("skill_level"), JsonRequired] int SkillLevel);

/// <summary>Light equipment slot of a support payload.</summary>
public sealed record WireLightEquipSlot(
	[property: JsonPropertyName("is_slot"), JsonRequired] JsonElement IsSlot,
	[property: JsonPropertyName("enhancement_pt")] int EnhancementPt = 0,
	[property: JsonPropertyName("rank")] int Rank = 0);

/// <summary>Light support unit, as in <c>trial_battle/support_unit_list</c> <c>support_unit_list[].unit_data</c>.</summary>
public sealed record WireSupportUnit(
	[property: JsonPropertyName("id"), JsonRequired] int Id,
	[property: JsonPropertyName("unit_rarity"), JsonRequired] int UnitRarity,
	[property: JsonPropertyName("unit_level"), JsonRequired] int UnitLevel,
	[property: JsonPropertyName("promotion_level"), JsonRequired] int PromotionLevel,
	[property: JsonPropertyName("union_burst"), JsonRequired] WireLightSkill[] UnionBurst,
	[property: JsonPropertyName("main_skill"), JsonRequired] WireLightSkill[] MainSkill,
	[property: JsonPropertyName("ex_skill"), JsonRequired] WireLightSkill[] ExSkill,
	[property: JsonPropertyName("equip_slot"), JsonRequired] WireLightEquipSlot[] EquipSlot,
	[property: JsonPropertyName("battle_rarity")] int BattleRarity = 0,
	[property: JsonPropertyName("unique_equip_slot")] WireLightEquipSlot[]? UniqueEquipSlot = null,
	[property: JsonPropertyName("ex_equip_slot")] WireExEquipSlot[]? ExEquipSlot = null,
	[property: JsonPropertyName("cb_ex_equip_slot")] WireExEquipSlot[]? CbExEquipSlot = null,
	[property: JsonPropertyName("skin_data")] WireSkin? SkinData = null,
	[property: JsonPropertyName("unlock_rarity_6_item")] WireUnlockRarity6? UnlockRarity6Item = null,
	[property: JsonPropertyName("resist_status_id")] int ResistStatusId = 0,
	[property: JsonPropertyName("resist_variation_id")] int ResistVariationId = 0,
	[property: JsonPropertyName("bonus_param")] JsonElement? BonusParam = null);

/// <summary>EX equipment inventory entry, as in <c>load/index</c> <c>user_ex_equip</c>.</summary>
public sealed record WireExEquipment(
	[property: JsonPropertyName("serial_id"), JsonRequired] int SerialId,
	[property: JsonPropertyName("ex_equipment_id"), JsonRequired] int ExEquipmentId,
	[property: JsonPropertyName("enhancement_pt")] int EnhancementPt = 0,
	[property: JsonPropertyName("rank")] int Rank = 0,
	[property: JsonPropertyName("sub_status")] WireExSubStatus[]? SubStatus = null);

/// <summary>Princess-knight talent level.</summary>
public sealed record WireTalentLevel(
	[property: JsonPropertyName("talent_id"), JsonRequired] int TalentId,
	[property: JsonPropertyName("total_point"), JsonRequired] long TotalPoint);

/// <summary>Talent or team skill node.</summary>
public sealed record WireSkillNode(
	[property: JsonPropertyName("node_id"), JsonRequired] int NodeId,
	[property: JsonPropertyName("enhance_level"), JsonRequired] int EnhanceLevel);

/// <summary>Princess-knight investment, as in <c>load/index</c> <c>princess_knight_info</c>.</summary>
public sealed record WirePrincessKnight(
	[property: JsonPropertyName("talent_level_info_list"), JsonRequired] WireTalentLevel[] TalentLevelInfoList,
	[property: JsonPropertyName("talent_skill_last_enhanced_page_node_list"), JsonRequired] WireSkillNode[] TalentSkillLastEnhancedPageNodeList,
	[property: JsonPropertyName("team_skill_latest_node")] WireSkillNode? TeamSkillLatestNode = null);

/// <summary>Role mastery of one role, as in <c>load/index</c> <c>unit_role_list</c>.</summary>
public sealed record WireUnitRole(
	[property: JsonPropertyName("unit_role_id"), JsonRequired] int UnitRoleId,
	[property: JsonPropertyName("slot_level_1"), JsonRequired] int SlotLevel1,
	[property: JsonPropertyName("enhance_level_1"), JsonRequired] int EnhanceLevel1,
	[property: JsonPropertyName("slot_level_2"), JsonRequired] int SlotLevel2,
	[property: JsonPropertyName("enhance_level_2"), JsonRequired] int EnhanceLevel2,
	[property: JsonPropertyName("slot_level_3"), JsonRequired] int SlotLevel3,
	[property: JsonPropertyName("enhance_level_3"), JsonRequired] int EnhanceLevel3,
	[property: JsonPropertyName("slot_level_4"), JsonRequired] int SlotLevel4,
	[property: JsonPropertyName("enhance_level_4"), JsonRequired] int EnhanceLevel4);

/// <summary>Log barrier coefficient and threshold, as in <c>ini_setting.log_barrier</c>.</summary>
public sealed record WireLogBarrier(
	[property: JsonPropertyName("coefficient"), JsonRequired] int Coefficient,
	[property: JsonPropertyName("threshold"), JsonRequired] int Threshold);

/// <summary>The consumed part of <c>load/index</c> <c>ini_setting</c>.</summary>
public sealed record WireIniSetting(
	[property: JsonPropertyName("battle_log_type"), JsonRequired] Dictionary<string, int> BattleLogType,
	[property: JsonPropertyName("log_barrier"), JsonRequired] WireLogBarrier LogBarrier);

/// <summary>The consumed part of <c>load/index</c> <c>user_info</c>.</summary>
public sealed record WireUserInfo([property: JsonPropertyName("team_level"), JsonRequired] int TeamLevel);
