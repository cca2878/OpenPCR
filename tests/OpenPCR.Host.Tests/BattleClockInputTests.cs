using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenPCR.Host;
using OpenPCR.Runtime.Impl;
using Elements;

internal static class BattleClockInputTests
{
	internal static void Run(Action<bool, string> check)
	{
		long large = 9007199254740993;
		BattleClockFrame Frame(int frame, long time, long delta) => new(new(time, 60, 1), new(delta, 60, 1), new(time, 60, 1), new(1, 60, 1), frame);
		var input = new BattleClockInput(BattleClockInput.CurrentSchema, Frame(100, large, 0), 0.5f,
			new[] { Frame(101, large + 1, 1), Frame(102, large + 1, 0), Frame(103, large + 3, 2) });
		string json = input.ToJson();
		var loaded = BattleClockInput.Parse(json);
		var clock = loaded.Prepare();
		check(loaded.InitialFrame.Time.Count == large && clock.InitialFrame.Time.Count == large && clock.InitialTimeScale == 0.5f, "Clock transport rounded its 64-bit integer origin or lost initial scale.");
		check(clock.ReadUpdate(2).DeltaTime.Count == 0 && clock.ReadUpdate(2).UnscaledDeltaTime.Count == 1, "Clock transport conflated zero scaled and nonzero unscaled deltas.");
		loaded.Updates[0] = Frame(999, 999, 99);
		check(clock.ReadUpdate(1).FrameCount == 101 && clock.ReadUpdate(1).Time.Count == large + 1, "Prepared clock retained mutable input array storage.");
		var empty = input with { Updates = Array.Empty<BattleClockFrame>() };
		bool exhausted = false;
		try { BattleClockInput.Parse(empty.ToJson()).Prepare().ReadUpdate(1); } catch (InvalidOperationException) { exhausted = true; }
		check(exhausted, "Finite empty input extended itself instead of exhausting.");
		void Reject(string value)
		{
			bool rejected = false;
			try { BattleClockInput.Parse(value); }
			catch (Exception error) when (error is InvalidDataException or JsonException or ArgumentException) { rejected = true; }
			check(rejected, "Malformed or incomplete clock input was accepted.");
		}
		foreach (var edit in new Action<JsonNode>[]
		{
			n => n["Schema"] = "unknown", n => n["FrameRate"] = 30, n => n.AsObject().Remove("Schema"),
			n => n.AsObject().Remove("InitialTimeScale"), n => n.AsObject().Remove("Updates"), n => n["Updates"] = null,
			n => n["InitialFrame"] = null, n => n["InitialFrame"]!.AsObject().Remove("FrameCount"),
			n => n["InitialFrame"]!["FrameCount"] = -1, n => n["InitialTimeScale"] = -1,
			n => n["Updates"]![0] = null, n => n["Updates"]![0]!["Time"] = null,
			n => n["Updates"]![0]!.AsObject().Remove("UnscaledTime"),
			n => n["Updates"]![0]!["Time"]!["Seconds"] = 123,
			n => n["Updates"]![0]!["Time"]!.AsObject().Remove("Count"),
			n => n["Updates"]![0]!["Time"]!["Count"] = "9007199254740993",
			n => n["Updates"]![0]!["Time"]!["Count"] = -1,
			n => n["Updates"]![0]!["Time"]!["Numerator"] = 0,
			n => n["Updates"]![0]!["Time"]!["Denominator"] = 0,
			n => n["Updates"]![0]!["Time"]!["Numerator"] = -1,
			n => n["Updates"]![0]!["FrameCount"] = 100,
			n => n["Updates"]![0]!["Time"]!["Count"] = large - 1,
			n => n["Updates"]![0]!["UnscaledTime"]!["Count"] = large - 1
		})
		{
			var node = JsonNode.Parse(json)!; edit(node); Reject(node.ToJsonString());
		}
		Reject("null");
		Reject("{\"Schema\":\"openpcr-battle-clock/v1\"," + json.TrimStart()[1..]);
		Reject(json.Replace("\"Count\": 9007199254740993", "\"Count\": 9007199254740993, \"Count\": 9007199254740993", StringComparison.Ordinal));
		// A v2 example's account values, assembled without masterdata (no content-release lookup is needed for the clock).
		var example = BattleRequest.Read(Path.Combine("examples", "battles", "trial.json"));
		var account0 = new BattleAccountEnvironment(example.Player.UserInfo.TeamLevel, BattlePlanner.PlannedNow,
			new BattleEnvironment(BattlePlanner.PlayerViewerId, example.Player.Errm, 0, BattlePlanner.ReleaseTime(example.Player.Releases.SlipDamageLogBarrier), -1, System.Array.Empty<BattleQuest>()));
		var definition = new BattleDefinition(example.MasterVersion, example.Battles[0].Party, null,
			new BattleScene(32, new[] { 1 }, 90f, 0, example.Battles[0].Seed, new[] { 1 }, null, new BattleTrial(example.ClanBattle!.ClanBattleId, 1, 1, 0, 0)),
			account0, BattlePlanner.Settings(example.Settings, example.Battles[0].Opening, false, null), BattlePlanner.Logging(example.Player));
		foreach (int origin in new[] { 0, 1000, int.MaxValue })
		{
			var account = (UserData)RuntimeHelpers.GetUninitializedObject(typeof(UserData));
			BattleEnvironmentInputs.BindAccountBattleInputs(account, definition, origin);
			var zero = new EngineRationalTime(0, 60, 1);
			ScriptingTime.Initialize(new(zero, zero, zero, zero, origin), 1f);
			try { check(account.NowUnixTime == definition.Account.NowUnixTime && account.TimeData is null, "Battle startup refreshed an explicit Unix-time input instead of using its matching Native frame cache."); }
			finally { ScriptingTime.EndSession(); }
		}
	}
}
