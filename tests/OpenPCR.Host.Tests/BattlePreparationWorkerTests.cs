using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using OpenPCR.Host;

internal static class BattlePreparationWorkerTests
{
	internal static void Run(string resources, string runner, string example)
	{
		var request = JsonSerializer.Deserialize<JsonElement>(File.ReadAllText(example));
		var invalidNode = JsonNode.Parse(request.GetRawText())!; invalidNode["Battles"]![0]!["Opening"]!["SetUnits"] = new JsonArray(999999);
		var invalid = JsonSerializer.Deserialize<JsonElement>(invalidNode.ToJsonString());
		var start = new ProcessStartInfo("dotnet") { RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true };
		start.ArgumentList.Add(runner); start.ArgumentList.Add("worker");
		using var process = Process.Start(start)!;
		var errors = process.StandardError.ReadToEndAsync();
		int sequence = 0;
		JsonElement Send(string command, object? arguments = null, bool ok = true)
		{
			int id = ++sequence;
			process.StandardInput.WriteLine(JsonSerializer.Serialize(new { id, command, arguments })); process.StandardInput.Flush();
			string line = process.StandardOutput.ReadLineAsync().WaitAsync(TimeSpan.FromSeconds(60)).GetAwaiter().GetResult() ?? throw new Exception("Worker exited unexpectedly.");
			using var document = JsonDocument.Parse(line); var root = document.RootElement;
			if (root.GetProperty("id").GetInt32() != id || root.GetProperty("ok").GetBoolean() != ok) throw new Exception(line);
			if (!ok && root.GetProperty("error").GetProperty("faulted").GetBoolean()) throw new Exception("Rejected command faulted a healthy worker: " + line);
			return root.Clone();
		}
		var options = new BattleRunOptions(resources, 5000);
		try
		{
			Send("create", new { preparedId = -1, options }, ok: false);
			Send("prepare", new { request = new { Schema = "invalid" }, resources }, ok: false);
			Send("prepare", new { request = invalid, resources }, ok: false);
			Send("create-battle", new { request, options }, ok: false);
			long a = Send("prepare", new { request, resources }).GetProperty("result").GetProperty("preparedId").GetInt64();
			long b = Send("prepare", new { request, resources }).GetProperty("result").GetProperty("preparedId").GetInt64();
			Send("create", new { preparedId = a, options });
			if (Send("inputs").GetProperty("result").GetProperty("Operations").GetArrayLength() != 0) throw new Exception("Preparation imported an operation plan.");
			Send("prepare", new { request, resources }, ok: false);
			Send("create", new { preparedId = b, options }, ok: false);
			long initial = Send("capture").GetProperty("result").GetProperty("checkpointId").GetInt64();
			Send("step", new { frames = 400 }); Send("run");
			string expected = Send("result").GetProperty("result").GetRawText();
			Send("restore", new { checkpointId = initial }); Send("run");
			if (Send("result").GetProperty("result").GetRawText() != expected) throw new Exception("Prepared worker startup restore changed the result.");
			Send("sequence-state", null, ok: false);
			Send("dispose");
			Send("create", new { preparedId = a, options }, ok: false);
			Send("create", new { preparedId = b, options }); Send("run");
			if (Send("result").GetProperty("result").GetRawText() != expected) throw new Exception("Independently prepared worker products shared mutable state.");
			Send("dispose");
			long drop = Send("prepare", new { request, resources }).GetProperty("result").GetProperty("preparedId").GetInt64();
			Send("drop-prepared", new { preparedId = drop });
			Send("create", new { preparedId = drop, options }, ok: false);
			long both = Send("prepare", new { request, resources }).GetProperty("result").GetProperty("preparedId").GetInt64();
			Send("create", new { request, preparedId = both, options }, ok: false);
			Send("drop-prepared", new { preparedId = both });
			Send("create", new { request, options }); Send("run");
			if (Send("result").GetProperty("result").GetRawText() != expected) throw new Exception("A request sequence and a prepared handle produced different results.");
			if (Send("battle-result").GetProperty("result").GetProperty("Csv").GetString()!.Length == 0) throw new Exception("battle-result lost the battle log.");
			Send("shutdown");
			if (!process.WaitForExit(30000) || process.ExitCode != 0) throw new Exception("Worker shutdown failed: " + errors.GetAwaiter().GetResult());
			Console.WriteLine($"Prepared worker lifecycle: {sequence} protocol commands passed, including rejection and checkpoint paths.");
		}
		finally { if (!process.HasExited) { process.Kill(entireProcessTree: true); process.WaitForExit(); } }
	}
}
