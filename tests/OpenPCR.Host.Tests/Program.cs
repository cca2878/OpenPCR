using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

if (args.Length == 4 && args[1] == "--preparation-worker")
{
	BattlePreparationWorkerTests.Run(Path.GetFullPath(args[0]), Path.GetFullPath(args[2]), Path.GetFullPath(args[3]));
	return 0;
}

// Each suite runs to its end: a failed assertion is recorded and the remaining suites still run.
int assertions = 0;
var failures = new List<string>();
string current = "";
void Check(bool valid, string message) { assertions++; if (!valid) failures.Add($"{current}: {message}"); }
var suites = new (string Name, Action Run)[]
{
	("SceneActivity", () => assertions += SceneActivityTests.Run()),
	("CoroutineExecution", () => CoroutineExecutionTests.Run(Check)),
	("ScriptCallback", () => ScriptCallbackTests.Run(Check)),
	("BattleUpdateClock", () => BattleUpdateClockTests.Run(Check)),
	("BattleClockInput", () => BattleClockInputTests.Run(Check)),
	("SpeedSettings", () => SpeedSettingsTests.Run(Check)),
	("TerminalTrace", () => TerminalTraceTests.Run(Check)),
	("AnimationDispatch", () => AnimationDispatchTests.Run(Check)),
	("PrincessMovie", () => PrincessMovieTests.Run(Check)),
	("ClanOutput", () => ClanOutputTests.Run(Check)),
	("SkillEffectResourceIdentity", () => SkillEffectResourceIdentityTests.Run(Check)),
	("PreparedResourceLoad", () => PreparedResourceLoadTests.Run(Check)),
	("SlowEffectInput", () => SlowEffectInputTests.Run(Check)),
	("MultiTargetAppearance", () => MultiTargetAppearanceTests.Run(Check)),
	("LabyrinthRequest", () => LabyrinthRequestTests.Run(Check)),
	("PlayerWire", () => PlayerWireTests.Run(Check)),
};

string[] selected;
if (args.Length == 0) selected = suites.Select(suite => suite.Name).ToArray();
else if (args.Length == 1 && args[0] == "--activity") selected = new[] { "SceneActivity" };
else if (args.Length == 1 && args[0] == "--list") { foreach (var suite in suites) Console.WriteLine(suite.Name); return 0; }
else if (args.Length == 2 && args[0] == "--only") selected = args[1].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
else
{
	Console.Error.WriteLine("Usage: OpenPCR.Host.Tests [--list | --only <suite>[,<suite>...] | --activity]\n       OpenPCR.Host.Tests <resources> --preparation-worker <runner.dll> <battle.json>");
	return 2;
}
var unknown = selected.Where(name => suites.All(suite => suite.Name != name)).ToArray();
if (selected.Length == 0 || unknown.Length != 0)
{
	Console.Error.WriteLine(selected.Length == 0 ? "No suite selected." : "Unknown suite: " + string.Join(", ", unknown) + ". Use --list.");
	return 2;
}

foreach (var (name, run) in suites.Where(suite => selected.Contains(suite.Name)))
{
	current = name;
	try { run(); }
	catch (Exception exception) { failures.Add($"{name}: stopped by {exception.GetType().Name}: {exception.Message}"); }
}
if (failures.Count != 0)
{
	foreach (string failure in failures) Console.Error.WriteLine("FAILED " + failure);
	Console.Error.WriteLine($"Host synthetic inputs and lifecycle: {failures.Count} failures in {selected.Length} suites ({assertions} assertions run).");
	return 1;
}
Console.WriteLine($"Host synthetic inputs and lifecycle: {assertions} assertions passed in {selected.Length} suites.");
return 0;
