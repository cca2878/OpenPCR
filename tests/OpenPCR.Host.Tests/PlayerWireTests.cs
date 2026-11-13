using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenPCR.Host;

/// <summary>Nested Player and support wire records: required fields reject, explicit zero/false/null/empty stay valid.</summary>
internal static class PlayerWireTests
{
	internal static void Run(Action<bool, string> check)
	{
		string json = BattleRequest.Read(Path.Combine("examples", "battles", "trial.json")).ToJson();
		JsonNode Source() => JsonNode.Parse(json)!;
		void Reject(JsonNode value, string message)
		{
			bool rejected = false;
			try { BattleRequest.Parse(value.ToJsonString()); }
			catch (Exception error) when (error is InvalidDataException or JsonException) { rejected = true; }
			check(rejected, message);
		}
		JsonObject Unit(JsonNode root) => root["Player"]!["unit_list"]![0]!.AsObject();

		// Explicit zero, false, null and empty values are inputs, not missing data.
		var minimal = Source(); var unit = Unit(minimal);
		foreach (string name in new[] { "unique_equip_slot", "ex_equip_slot", "cb_ex_equip_slot", "skin_data", "unlock_rarity_6_item" }) unit[name] = null;
		unit["main_skill"] = new JsonArray(new JsonObject { ["skill_id"] = 0, ["skill_level"] = 0 });
		unit["equip_slot"] = new JsonArray(new JsonObject { ["is_slot"] = false }, new JsonObject { ["is_slot"] = 0, ["id"] = 999999 });
		unit["ex_equip_slot"] = new JsonArray(new JsonObject { ["serial_id"] = 0, ["sub_status"] = null });
		var parsed = BattlePlanner.Unit(BattleRequest.Parse(minimal.ToJsonString()).Player.UnitList[0]);
		check(parsed.Skin is null && parsed.UnlockRarity6 is null && parsed.UniqueEquipSlots!.Count == 0 && parsed.CbExEquipSlots!.Count == 0,
			"Null optional unit objects and lists must stay valid: skin and unlock null, lists empty.");
		check(parsed.MainSkill.Single().SkillLevel == 0 && parsed.MainSkill.Single().SlotNumber == 0, "An explicit zero skill level and an omitted slot number must stay valid.");
		check(parsed.EquipSlots!.Select(slot => slot.IsSlot).SequenceEqual(new[] { false, false }) && parsed.EquipSlots![1].Id == 999999, "is_slot accepts false and 0.");
		check(parsed.ExEquipSlots!.Single().SubStatus!.Count == 0, "A null EX sub-status list must stay valid.");

		// Required fields of every nested record.
		foreach (string name in new[] { "id", "unit_rarity", "unit_level", "promotion_level", "union_burst", "main_skill", "ex_skill", "free_skill", "equip_slot" })
		{
			var value = Source(); Unit(value).Remove(name); Reject(value, "unit_list entry without " + name + " was accepted.");
		}
		foreach (string name in new[] { "skill_id", "skill_level" })
		{
			var value = Source(); Unit(value)["union_burst"]![0]!.AsObject().Remove(name); Reject(value, "Skill without " + name + " was accepted.");
		}
		var noSlotFlag = Source(); Unit(noSlotFlag)["equip_slot"] = new JsonArray(new JsonObject { ["id"] = 101011 }); Reject(noSlotFlag, "Equip slot without is_slot was accepted.");
		foreach (string name in new[] { "status", "step" })
		{
			var value = Source(); var sub = new JsonObject { ["status"] = 1, ["step"] = 1 }; sub.Remove(name);
			Unit(value)["ex_equip_slot"] = new JsonArray(new JsonObject { ["serial_id"] = 1, ["sub_status"] = new JsonArray(sub) });
			Reject(value, "EX sub-status without " + name + " was accepted.");
		}
		foreach (string name in new[] { "user_info", "unit_list", "read_story_ids", "unit_role_list", "ini_setting", "errm", "Releases" })
		{
			var value = Source(); value["Player"]!.AsObject().Remove(name); Reject(value, "Player without " + name + " was accepted.");
		}
		var inventory = Source(); inventory["Player"]!["user_ex_equip"] = new JsonArray(new JsonObject { ["serial_id"] = 1 }); Reject(inventory, "EX inventory entry without ex_equipment_id was accepted.");
		var role = Source(); role["Player"]!["unit_role_list"] = new JsonArray(new JsonObject { ["unit_role_id"] = 1 }); Reject(role, "Role entry without its slot levels was accepted.");
		var duplicate = Source(); var units = duplicate["Player"]!["unit_list"]!.AsArray(); units.Add(units[0]!.DeepClone()); Reject(duplicate, "Duplicate unit_list ids were accepted.");

		// The server light support payload carries skill levels without IDs or slot numbers.
		var supported = Source();
		var light = new JsonObject
		{
			["id"] = 100201,
			["unit_rarity"] = 3,
			["unit_level"] = 90,
			["promotion_level"] = 7,
			["union_burst"] = new JsonArray(new JsonObject { ["skill_level"] = 90 }),
			["main_skill"] = new JsonArray(new JsonObject { ["skill_level"] = 1 }),
			["ex_skill"] = new JsonArray(),
			["equip_slot"] = new JsonArray(new JsonObject { ["is_slot"] = 1, ["enhancement_pt"] = 0 }),
			["bonus_param"] = new JsonArray()
		};
		supported["ClanBattle"]!["support"] = new JsonObject { ["unit_id"] = 100201, ["unit_data"] = light };
		supported["Battles"]![0]!["Party"]![4] = 1;
		var support = BattlePlanner.Support(BattleRequest.Parse(supported.ToJsonString()).ClanBattle!.Support!.UnitData);
		check(support.MainSkillLevels.SequenceEqual(new[] { 1 }) && support.EquipSlots.Single().IsSlot && support.BonusParam is { Hp: 0, Atk: 0 },
			"The light support shape must keep levels and slot flags; an empty bonus_param is an all-zero bonus.");
		light["main_skill"]![0]!.AsObject().Remove("skill_level"); Reject(supported, "Light support skill without skill_level was accepted.");
	}
}
