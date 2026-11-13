using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using OpenPCR.Host;

if (args.Length == 1 && args[0] == "worker") return InteractiveWorker.Serve();
if (args.Length > 0 && args[0] == "run") return Run(args[1..]);
if (args.Length == 3 && args[0] == "schema" && args[1] is "--write" or "--check") return Schemas(args[1] == "--write", args[2]);
Console.Error.WriteLine("""
run --resources <root> --request <battle.json> --max-engine-frames <int> [--csv-dir <directory>] [--clock <clock.json>]
    Runs every battle of an openpcr-battle/v2 request; stdout is the openpcr-battle-result/v2 result.
    --csv-dir writes battle-<index>.csv for each battle.
worker
    JSON Lines on stdin/stdout.
schema --write|--check <directory>
    JSON Schemas of the request and clock documents.
""");
return 2;

static int Run(string[] args)
{
	try
	{
		var values = new Dictionary<string, string>();
		for (int i = 0; i < args.Length; i += 2)
			if (i + 1 >= args.Length || args[i] is not ("--resources" or "--request" or "--max-engine-frames" or "--csv-dir" or "--clock") || !values.TryAdd(args[i], args[i + 1]))
				throw new ArgumentException("Unknown, duplicate or incomplete option: " + args[i]);
		string Required(string name) => values.TryGetValue(name, out var value) ? value : throw new ArgumentException("Required option: " + name);
		var request = BattleRequest.Read(Required("--request"));
		var options = new BattleRunOptions(Required("--resources"), int.Parse(Required("--max-engine-frames"), CultureInfo.InvariantCulture));
		var clock = values.TryGetValue("--clock", out var clockPath) ? BattleClockInput.Read(clockPath).Prepare() : null;
		var result = BattleRunner.Run(request, options, progress => Console.Error.WriteLine(JsonSerializer.Serialize(progress)), clock);
		if (values.TryGetValue("--csv-dir", out var directory))
		{
			Directory.CreateDirectory(directory);
			foreach (var battle in result.Battles)
				File.WriteAllText(Path.Combine(directory, "battle-" + battle.Index.ToString(CultureInfo.InvariantCulture) + ".csv"), battle.Battle.Csv);
		}
		Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { IncludeFields = true, WriteIndented = true }));
		return result.Completed ? 0 : 3;
	}
	catch (Exception exception) { Console.Error.WriteLine(exception); return 1; }
}

static int Schemas(bool write, string directory)
{
	var options = new JsonSerializerOptions { WriteIndented = true };
	int stale = 0;
	if (write) Directory.CreateDirectory(directory);
	foreach (var (name, schema) in InputSchemas.Export())
	{
		string path = Path.Combine(directory, name), text = schema.ToJsonString(options) + "\n";
		if (write) File.WriteAllText(path, text);
		else if (!File.Exists(path) || File.ReadAllText(path) != text) { stale++; Console.Error.WriteLine("Schema differs from the code: " + path); }
	}
	if (!write && Directory.Exists(directory))
		foreach (string path in Directory.GetFiles(directory, "*.schema.json").Where(path => !InputSchemas.Export().ContainsKey(Path.GetFileName(path))))
		{ stale++; Console.Error.WriteLine("Schema file has no source: " + path); }
	return stale == 0 ? 0 : 1;
}
