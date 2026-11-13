using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Elements;
using OpenPCR.Growth;

namespace OpenPCR.Host;

/// <summary>One normalized battle: the scene definition, typed growth inputs and starting execution inputs.</summary>
internal sealed record PlannedBattle(BattleDefinition Definition, GrowthInputs Growth, BattleExecutionInputs Execution);

/// <summary>
/// Normalizes a v2 request into the internal representation. Release switches become the values the Native
/// consumers read: a fixed current time with release times before or after it, and the content-release quest record.
/// </summary>
internal static class BattlePlanner
{
	/// <summary>Current Unix time the release values are built around; release times are 1 (released) or 3 (not released).</summary>
	internal const long PlannedNow = 2;
	/// <summary>The player's own viewer ID; viewer IDs are identity keys only.</summary>
	internal const long PlayerViewerId = 1;
	/// <summary>Owner of the selected training-ground support.</summary>
	internal const long SupportOwnerViewerId = 2;
	/// <summary>Knight enhancement content release (eSystemId.KNIGHT_ENHANCE).</summary>
	private const int KnightEnhanceSystemId = 129;

	internal static long ReleaseTime(bool released) => released ? 1 : 3;

	internal static PlannedBattle PlanClan(BattleRequest request, GrowthMasterData master)
	{
		var clan = request.ClanBattle ?? throw new InvalidOperationException("Not a clan battle request.");
		var battle = request.Battles[0];
		var map = ClanMapRow(master, clan.ClanBattleId, clan.Phase, clan.LapNum);
		int waveGroup = map.Int("wave_group_id_" + clan.OrderNum);
		// wave_group_data is keyed by id; drop variants repeat other wave_group_id values, a clan wave group has one row.
		var waves = master.Rows("wave_group_data").Where(row => row.Int("wave_group_id") == waveGroup).ToArray() is { Length: 1 } single ? single[0]
			: throw new InvalidDataException("Clan battle wave group must have exactly one row: " + waveGroup);
		int[] enemies = Enumerable.Range(1, 5).Select(i => waves.Int("enemy_id_" + i)).Where(id => id != 0).ToArray();
		if (enemies.Length == 0) throw new InvalidDataException("Clan battle wave group has no enemies: " + waveGroup);
		float timeLimit = master.Rows("clan_battle_period").Where(row => row.Int("clan_battle_id") == clan.ClanBattleId).OrderBy(row => row.Int("period")).Select(row => (float)row.Int("limit_time")).DefaultIfEmpty(-1).First();
		if (timeLimit <= 0) throw new InvalidDataException("Clan battle has no period: " + clan.ClanBattleId);
		int background = master.Require("clan_battle_2_boss_data", ("boss_id", map.Int("boss_id_" + clan.OrderNum))).Int("background");
		int lap = clan.LapNum ?? map.Int("lap_num_from");
		BattleClan? clanScene = null; BattleTrial? trialScene = null;
		if (clan.IsTrial) trialScene = new BattleTrial(clan.ClanBattleId, clan.OrderNum, clan.Phase, clan.InfinityHp ? 1 : 0, 0);
		else
		{
			long maxHp = clan.BossHp?.MaxHp ?? master.Require("enemy_parameter", ("enemy_id", enemies[0])).Long("hp");
			long currentHp = clan.BossHp?.CurrentHp ?? maxHp;
			clanScene = new BattleClan(0, enemies[0], maxHp, currentHp, clan.QuadSpeed, false, clan.ClanBattleId, lap, clan.Phase, clan.OrderNum, 0, 0, false, 0, 0);
		}
		var scene = new BattleScene(clan.IsTrial ? 32 : 9, enemies, timeLimit, 0, battle.Seed, new[] { background }, clanScene, trialScene);
		var support = clan.Support is { } selected ? Support(selected.UnitData) : null;
		var settings = Settings(request.Settings, battle.Opening, bossInfinityHp: clan.InfinityHp, clanRehearsalMaxSpeed: clan.IsTrial ? null : clan.QuadSpeed ? 4 : 2);
		var definition = new BattleDefinition(request.MasterVersion, battle.Party.ToArray(), clan.Support is { } s ? new BattleSupportIdentity(s.UnitId, SupportOwnerViewerId) : null,
			scene, Account(request.Player, master), settings, Logging(request.Player));
		return new PlannedBattle(definition, Growth(request.MasterVersion, request.Player, support), Execution(battle));
	}

	/// <summary>
	/// The clan map row of a scene: the row whose lap range contains <paramref name="lap"/>, or without a lap the row
	/// with the largest lap_num_from in the phase (the row the training ground uses).
	/// </summary>
	internal static GrowthMasterData.Row ClanMapRow(GrowthMasterData master, int clanBattleId, int phase, int? lap)
	{
		var rows = master.Rows("clan_battle_2_map_data").Where(row => row.Int("clan_battle_id") == clanBattleId && row.Int("phase") == phase).OrderBy(row => row.Int("lap_num_from")).ToArray();
		if (rows.Length == 0) throw new InvalidDataException("No clan battle map for " + clanBattleId + " phase " + phase + ".");
		if (lap is not { } value) return rows[^1];
		return rows.SingleOrDefault(row => row.Int("lap_num_from") <= value && (row.Int("lap_num_to") == -1 || row.Int("lap_num_to") >= value))
			?? throw new InvalidDataException("Lap " + value + " is outside the map of " + clanBattleId + " phase " + phase + ".");
	}

	internal static BattleExecutionInputs Execution(BattleEntryInput battle) => new(battle.Opening.Auto, battle.Opening.SetUnits.ToArray(), battle.OperationList.ToArray());

	/// <summary>Device settings plus one battle's opening: speed 1/2/4 is speed-up off, on, on with the quadruple preference.</summary>
	internal static BattleSettings Settings(BattleDeviceSettings device, BattleOpening opening, bool bossInfinityHp, int? clanRehearsalMaxSpeed)
		=> new(device.FrameRate, device.ApplyOtherUserSkins, bossInfinityHp, true, device.ResetSkillReserveEveryTime,
			device.AdvantageAndBreakdownBonusShows, device.MultiTargetFirst, device.TalentFormationBonusFirst, clanRehearsalMaxSpeed,
			new BattleAutoSpeed(SystemSettingDefine.eTypeButtonState.ON, opening.Auto, false, opening.Speed > 1), device.PlayMultiTargetAppearance, opening.Speed == 4);

	internal static BattleAccountEnvironment Account(BattlePlayerInput player, GrowthMasterData master)
	{
		var release = master.Require("content_release_data", ("system_id", KnightEnhanceSystemId));
		int questId = release.Int("quest_id"), teamLevel = release.Int("team_level");
		bool teamReached = teamLevel <= 0 || player.UserInfo.TeamLevel >= teamLevel;
		if (player.Releases.KnightEnhance && !teamReached) throw new InvalidDataException("Releases.KnightEnhance needs team level " + teamLevel + ".");
		if (!player.Releases.KnightEnhance && teamReached && questId <= 0) throw new InvalidDataException("Knight enhancement has no quest condition at this team level; Releases.KnightEnhance must be true.");
		var quests = player.Releases.KnightEnhance && questId > 0 ? new[] { new BattleQuest(questId, 1, 0, 0, 0) } : Array.Empty<BattleQuest>();
		// Largest cleared normal quest ID, or -1 when none are available.
		int arrival = quests.Length > 0 && unchecked((uint)(questId - 11000000)) < 1000000u ? questId : -1;
		var values = new BattleEnvironment(PlayerViewerId, player.Errm, 0, ReleaseTime(player.Releases.SlipDamageLogBarrier), arrival, quests);
		return new BattleAccountEnvironment(player.UserInfo.TeamLevel, PlannedNow, values);
	}

	/// <summary>ini_setting log types (missing types are 0) and the effective switches BattleLog.GetLogEnable derives from them.</summary>
	internal static BattleLogging Logging(BattlePlayerInput player)
	{
		var types = typeof(BattleLogType).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Select(property => property.Name);
		var account = types.ToDictionary(name => name, name => (long)player.IniSetting.BattleLogType.GetValueOrDefault(Snake(name)));
		var effective = new Dictionary<int, bool> { [-1] = false };
		foreach (eBattleLogType type in Enum.GetValues<eBattleLogType>())
		{
			if ((int)type == -1) continue;
			string name = Pascal(type.ToString());
			if (account.TryGetValue(name, out long value)) effective[(int)type] = value == 1;
		}
		return new BattleLogging(account, effective, player.IniSetting.LogBarrier.Coefficient, player.IniSetting.LogBarrier.Threshold);
	}

	internal static GrowthInputs Growth(string masterVersion, BattlePlayerInput player, RawSupportUnit? support)
	{
		var units = player.UnitList.ToDictionary(unit => unit.Id, Unit);
		var inventory = (player.UserExEquip ?? Array.Empty<WireExEquipment>()).ToDictionary(item => item.SerialId,
			item => new RawExEquipment(item.SerialId, item.ExEquipmentId, item.EnhancementPt, item.Rank, SubStatuses(item.SubStatus)));
		var team = new RawTeamEnhance(player.PrincessKnightInfo is { } knight ? new RawPrincessKnight(
				knight.TalentLevelInfoList.Select(level => new RawTalentLevel(level.TalentId, level.TotalPoint)).ToArray(),
				knight.TalentSkillLastEnhancedPageNodeList.Select(Node).ToArray(), knight.TeamSkillLatestNode is { } latest ? Node(latest) : null) : null,
			player.UnitRoleList.Select(role => new RawUnitRole(role.UnitRoleId,
				new[] { role.SlotLevel1, role.SlotLevel2, role.SlotLevel3, role.SlotLevel4 },
				new[] { role.EnhanceLevel1, role.EnhanceLevel2, role.EnhanceLevel3, role.EnhanceLevel4 })).ToArray());
		return new GrowthInputs(masterVersion, player.UserInfo.TeamLevel, player.Releases.ChangeRarity, player.Releases.SkillLevelSync,
			player.ReadStoryIds.ToArray(), units, inventory, team, null, support);
	}

	/// <summary>Client UnitData parse defaults: missing lists are empty, missing skin and unlock slots are null.</summary>
	internal static RawUnit Unit(WireUnit unit) => new(unit.Id, unit.UnitLevel, unit.UnitRarity, unit.BattleRarity, unit.PromotionLevel, unit.ExceedStage,
		unit.ResistStatusId, unit.ResistVariationId, Unlock(unit.UnlockRarity6Item),
		unit.EquipSlot.Select(Equip).ToArray(), (unit.UniqueEquipSlot ?? Array.Empty<WireEquipSlot>()).Select(Equip).ToArray(),
		ExSlots(unit.ExEquipSlot), ExSlots(unit.CbExEquipSlot),
		Skills(unit.UnionBurst), Skills(unit.MainSkill), Skills(unit.ExSkill), Skills(unit.FreeSkill), Skin(unit.SkinData));

	internal static RawSupportUnit Support(WireSupportUnit unit) => new(unit.Id, unit.UnitLevel, unit.UnitRarity, unit.BattleRarity, unit.PromotionLevel,
		unit.ResistStatusId, unit.ResistVariationId, Unlock(unit.UnlockRarity6Item),
		unit.UnionBurst.Select(skill => skill.SkillLevel).ToArray(), unit.MainSkill.Select(skill => skill.SkillLevel).ToArray(), unit.ExSkill.Select(skill => skill.SkillLevel).ToArray(),
		unit.EquipSlot.Select(Light).ToArray(), (unit.UniqueEquipSlot ?? Array.Empty<WireLightEquipSlot>()).Select(Light).ToArray(),
		ExSlots(unit.ExEquipSlot), ExSlots(unit.CbExEquipSlot), Skin(unit.SkinData), Bonus(unit.BonusParam));

	/// <summary>Server bonus_param: absent or null is no bonus; an empty array or object is an all-zero bonus.</summary>
	internal static RawStatusShort? Bonus(JsonElement? bonus)
	{
		if (bonus is not { } value || value.ValueKind == JsonValueKind.Null) return null;
		if (value.ValueKind == JsonValueKind.Array)
		{
			if (value.GetArrayLength() != 0) throw new InvalidDataException("bonus_param must be an object or an empty array.");
			return new RawStatusShort(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
		}
		if (value.ValueKind != JsonValueKind.Object) throw new InvalidDataException("bonus_param must be an object or an empty array.");
		long Get(string name) => value.TryGetProperty(name, out var field) ? field.GetInt64() : 0;
		return new RawStatusShort(Get("hp"), (int)Get("atk"), (int)Get("def"), (int)Get("matk"), (int)Get("mdef"), (int)Get("crt"), (int)Get("mcrt"), (int)Get("hrec"),
			(int)Get("erec"), (int)Get("hrec_rate"), (int)Get("pnt"), (int)Get("mpnt"), (int)Get("life_steal"), (int)Get("dodge"), (int)Get("erec_rate"), (int)Get("ered_rate"), (int)Get("accuracy"));
	}

	private static RawSkillNode Node(WireSkillNode node) => new(node.NodeId, node.EnhanceLevel);
	private static bool Flag(JsonElement value) => value.ValueKind switch
	{
		JsonValueKind.True => true,
		JsonValueKind.False => false,
		JsonValueKind.Number when value.GetInt32() is 0 or 1 => value.GetInt32() == 1,
		_ => throw new InvalidDataException("is_slot must be a boolean, 0 or 1.")
	};
	private static RawEquipSlot Equip(WireEquipSlot slot) => new(slot.Id, Flag(slot.IsSlot), slot.EnhancementLevel, slot.EnhancementPt, slot.Rank);
	private static RawSupportEquipSlot Light(WireLightEquipSlot slot) => new(Flag(slot.IsSlot), slot.EnhancementPt, slot.Rank);
	private static RawExEquipSlot[] ExSlots(WireExEquipSlot[]? slots) => (slots ?? Array.Empty<WireExEquipSlot>()).Select(slot =>
		new RawExEquipSlot(slot.SerialId, slot.ExEquipmentId, slot.EnhancementPt, SubStatuses(slot.SubStatus))).ToArray();
	private static RawExSubStatus[] SubStatuses(WireExSubStatus[]? subs) => (subs ?? Array.Empty<WireExSubStatus>()).Select(sub => new RawExSubStatus(sub.SlotNumber, sub.Status, sub.Step, sub.IsLock is { } lockValue && Flag(lockValue) ? 1 : 0)).ToArray();
	private static RawSkill[] Skills(WireSkill[] skills) => skills.Select(skill => new RawSkill(skill.SkillId, skill.SkillLevel, skill.SlotNumber)).ToArray();
	private static RawSkin? Skin(WireSkin? skin) => skin is null ? null : new RawSkin(skin.IconSkinId, skin.SdSkinId, skin.StillSkinId, skin.MotionId);
	private static RawUnlockRarity6? Unlock(WireUnlockRarity6? unlock) => unlock is null ? null
		: new RawUnlockRarity6(unlock.QuestClear, unlock.Slot1, unlock.Slot2, unlock.Slot3, unlock.Status1, unlock.Status2, unlock.Status3);

	/// <summary>PascalCase property name to the server's snake_case key (ButtonTap → button_tap).</summary>
	internal static string Snake(string pascal)
	{
		var result = new StringBuilder();
		for (int i = 0; i < pascal.Length; i++)
		{
			if (char.IsUpper(pascal[i]) && i > 0) result.Append('_');
			result.Append(char.ToLowerInvariant(pascal[i]));
		}
		return result.ToString();
	}
	/// <summary>UPPER_SNAKE enum name to the PascalCase property name (BUTTON_TAP → ButtonTap).</summary>
	internal static string Pascal(string upperSnake) => string.Concat(upperSnake.Split('_').Select(part => part.Length == 0 ? part : part[0] + part[1..].ToLowerInvariant()));
}
