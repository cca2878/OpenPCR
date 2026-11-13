using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenPCR.Host;
using Elements;

internal static class LabyrinthRequestTests
{
	internal static void Run(Action<bool, string> check)
	{
		string json = BattleRequest.Read(Path.Combine("examples", "battles", "labyrinth-normal.json")).ToJson();
		var source = JsonNode.Parse(json)!.AsObject();
		int id = source["Battles"]![0]!["Party"]![0]!.GetValue<int>();
		source["Battles"]![0]!["Opening"] = new JsonObject { ["Auto"] = true, ["Speed"] = 4, ["SetUnits"] = new JsonArray(id) };
		source["Labyrinth"]!["alpha_rupee"] = 123;
		json = source.ToJsonString();
		var request = BattleRequest.Parse(json);
		var reloaded = BattleRequest.Parse(request.ToJson());
		var opening = reloaded.Battles[0].Opening;
		check(opening.Auto && opening.Speed == 4 && opening.SetUnits.SequenceEqual(new[] { id }) && reloaded.Labyrinth!.AlphaRupee == 123,
			"Labyrinth roundtrip lost the opening or the explicit inventory.");
		var mapped = BattlePlanner.Settings(request.Settings, opening, false, null);
		var execution = BattlePlanner.Execution(request.Battles[0]);
		check(mapped.UseUbSetMode && mapped.SpeedQuadruple && mapped.AutoSpeed.IsSpeedUp && execution.Auto && execution.ReserveUnitIds.SequenceEqual(new[] { id }),
			"Opening AUTO, speed and SET were not mapped to battle settings and execution inputs.");
		check(!BattlePlanner.Settings(request.Settings, opening with { Speed = 1, SetUnits = Array.Empty<int>() }, false, null).AutoSpeed.IsSpeedUp
			&& !BattlePlanner.Settings(request.Settings, opening with { Speed = 2 }, false, null).SpeedQuadruple
			&& BattlePlanner.Settings(request.Settings, opening with { SetUnits = Array.Empty<int>() }, false, null).UseUbSetMode,
			"Speed 1 and 2 must map to speed-up off and the double preference; the UB SET switch stays on.");
		void Reject(string value, string message)
		{
			bool rejected = false;
			try { BattleRequest.Parse(value); }
			catch (Exception error) when (error is InvalidDataException or JsonException or ArgumentException or NotSupportedException) { rejected = true; }
			check(rejected, message);
		}
		foreach (var edit in new Action<JsonNode>[]
		{
			n => n["Schema"] = "unknown", n => n["frame0"] = new JsonObject(), n => n["Labyrinth"] = null,
			n => n["ClanBattle"] = new JsonObject { ["clan_battle_id"] = 1001, ["phase"] = 1, ["order_num"] = 1, ["kind"] = "trial" },
			n => n["Labyrinth"]!["unit_list"] = new JsonArray(), n => n["Labyrinth"]!["unit_list"]![0]!["unit_type"] = 9,
			n => n["Labyrinth"]!["unit_list"]![0]!["unit_type"] = 2, n => n["Labyrinth"]!.AsObject().Remove("alpha_rupee"),
			n => n["Labyrinth"]!["relic_list"] = new JsonArray(new JsonObject { ["relic_id"] = 50001, ["effected_count"] = 0 }),
			n => n["Battles"]![0]!["Opening"]!["Speed"] = 3, n => n["Battles"]![0]!["Opening"]!["SetUnits"] = new JsonArray(id + 1),
			n => n["Battles"]![0]!["Party"] = new JsonArray(0, 0, 0, 0, 0), n => n["Battles"]![0]!["Party"] = new JsonArray(1, 0, 0, 0, 0),
			n => n["Battles"]![0]!["Operations"] = new JsonArray(new JsonObject { ["Anchor"] = "BattleClock", ["At"] = 1, ["Kind"] = "auto_mode" }),
			n => n["Battles"]![0]!["Opponent"] = new JsonObject(), n => n["Battles"] = new JsonArray(),
			n => n["Player"]!["Releases"]!["Extra"] = true, n => n["Settings"]!["AutoSpeed"] = new JsonObject()
		})
		{
			var value = JsonNode.Parse(json)!; edit(value); Reject(value.ToJsonString(), "Invalid or missing Labyrinth input was accepted.");
		}
		Reject("{\"Schema\":\"openpcr-battle/v2\"," + json.TrimStart()[1..], "Duplicate schema property was accepted.");
		// NPC unit_data: bonus_param absent or null is no bonus, an empty array is an all-zero bonus; computed fields are ignored.
		var npc = JsonNode.Parse(json)!;
		var unit = npc["Player"]!["unit_list"]![0]!.DeepClone();
		npc["Labyrinth"]!["unit_list"]![0]!["unit_type"] = 2; npc["Labyrinth"]!["unit_list"]![0]!["unit_data"] = unit;
		unit["unit_param"] = new JsonObject(); unit["power"] = 1;
		var npcRequest = BattleRequest.Parse(npc.ToJsonString());
		check(BattlePlanner.Bonus(npcRequest.Labyrinth!.UnitList[0].UnitData!.BonusParam) is null, "Absent NPC bonus must be no bonus.");
		unit["bonus_param"] = new JsonArray();
		check(BattlePlanner.Bonus(BattleRequest.Parse(npc.ToJsonString()).Labyrinth!.UnitList[0].UnitData!.BonusParam) is { Hp: 0, Atk: 0 }, "Empty NPC bonus array must be an all-zero bonus.");
		unit["bonus_param"] = new JsonObject { ["atk"] = 12, ["hrec_rate"] = 3 };
		check(BattlePlanner.Bonus(BattleRequest.Parse(npc.ToJsonString()).Labyrinth!.UnitList[0].UnitData!.BonusParam) is { Atk: 12, HrecRate: 3, Hp: 0 }, "NPC bonus fields were not read by their server names.");
		// EX start state keeps int64 HP and skill counters; missing required state fields are rejected.
		long hp = 9007199254740993L;
		var ex = JsonNode.Parse(File.ReadAllText(Path.Combine("examples", "battles", "labyrinth-ex.json")))!;
		var counters = new[] { new LabyrinthExCounterInput(123, 7) };
		int enemy = 401010101;
		var state = new LabyrinthExState(1,
			new[] { new LabyrinthExEnemyInput(enemy, 1, hp, 0, counters, hp, hp, 1, 1, 1, 0, null) }, Array.Empty<LabyrinthExDeadInput>(),
			new[] { new LabyrinthExWaveInput(1, Array.Empty<LabyrinthExWaveUnitInput>(), 1000) },
			new[] { new Dictionary<int, LabyrinthExUnitResultInput> { [id] = new(1, 2, 3.25) } }, new[] { eBattleClearType.MANUAL },
			new[] { new[] { new LabyrinthExContinuousInput(enemy, 1, hp, 0, counters) } });
		ex["Labyrinth"]!["start_state"] = JsonSerializer.SerializeToNode(state);
		var loadedState = BattleRequest.Parse(ex.ToJsonString()).Labyrinth!.StartState!;
		check(loadedState.Enemies[0].Hp == hp && loadedState.EnemyContinuation![0][0].SkillLimitCounter[0].counter == 7 && loadedState.UnitResults[0][id].Energy == 3.25,
			"EX start state lost int64 HP, skill counters or prior results.");
		ex["Labyrinth"]!["start_state"]!["UnitResults"]![0]![id.ToString()]!.AsObject().Remove("Hp");
		Reject(ex.ToJsonString(), "Missing EX prior HP silently became zero.");
		var four = JsonNode.Parse(File.ReadAllText(Path.Combine("examples", "battles", "labyrinth-ex.json")))!;
		four["Battles"]!.AsArray().Add(four["Battles"]![0]!.DeepClone());
		Reject(four.ToJsonString(), "More than three EX parties were accepted.");
	}
}
