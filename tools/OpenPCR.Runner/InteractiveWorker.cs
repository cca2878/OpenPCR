using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenPCR.Host;

/// <summary>One command/response JSON line. Commands run serially at complete-frame boundaries.</summary>
internal static class InteractiveWorker
{
	internal static int Serve()
	{
		using var protocol = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
		Console.SetOut(Console.Error);
		var json = new JsonSerializerOptions { IncludeFields = true, Converters = { new JsonStringEnumConverter() } };
		// A request runs as a sequence; a prepared handle runs as one battle.
		BattleSequence? sequence = null;
		BattleSession? session = null;
		bool labyrinth = false;
		var checkpoints = new Dictionary<long, OpenPCR.Checkpoint.Checkpoint>();
		var preparations = new Dictionary<long, (PreparedBattleInputs Inputs, BattleExecutionInputs Execution, bool Labyrinth)>();
		long nextPreparation = 0;
		long nextCheckpoint = 0;
		void DisposeSession()
		{
			DropCheckpoints();
			if (sequence is not null) sequence.Dispose(); else session?.Dispose();
			session = null; sequence = null; labyrinth = false;
		}
		void DropCheckpoints()
		{
			foreach (var checkpoint in checkpoints.Values) checkpoint.Dispose();
			checkpoints.Clear();
		}
		// A sequence moves to its next battle when the active one completed; checkpoints belong to the finished battle.
		void AdvanceIfReady()
		{
			if (sequence is not null && sequence.Current.Completed && sequence.CanAdvance) { DropCheckpoints(); sequence.Advance(); }
		}
		void RequireOperations()
		{
			if (labyrinth) throw new NotSupportedException("Labyrinth battles take AUTO, speed and SET in the request; player operations and UB exploration are not supported.");
		}
		void RequireNone() { if (session is not null || sequence is not null) throw new InvalidOperationException("Dispose the current session first."); }
		BattleUpdateClock? Clock(JsonElement arguments) => arguments.TryGetProperty("clock", out var value)
			? BattleClockInput.Parse(value.GetRawText()).Prepare() : null;
		BattleSession RequireSession() => sequence?.Current ?? session ?? throw new InvalidOperationException("Create a session first.");
		object Status()
		{
			var current = RequireSession();
			if (current.IsFaulted) return new { faulted = true };
			return new
			{
				faulted = false,
				progress = current.Progress,
				completed = current.Completed,
				exhausted = current.Exhausted,
				index = sequence?.Index ?? 0,
				runCompleted = sequence is null ? current.Completed : current.Completed && sequence.IsComplete
			};
		}
		BattleRunResult Result()
		{
			if (sequence is not null) return sequence.ReadResult();
			var current = RequireSession(); var battle = current.ReadBattleResult();
			object? settlement = !battle.Completed ? null : labyrinth ? current.ReadLabyrinthSettlement() : current.ReadClanSettlement();
			return new(BattleRunResult.CurrentSchema, battle.Completed, new[] { new BattleRunEntry(0, battle, settlement, null) }, null);
		}
		try
		{
			string? line;
			while ((line = Console.In.ReadLine()) is not null)
			{
				JsonElement? id = null;
				bool shutdown = false;
				object response;
				try
				{
					using var document = JsonDocument.Parse(line);
					var request = document.RootElement;
					id = request.GetProperty("id").Clone();
					if (id.Value.ValueKind is not (JsonValueKind.String or JsonValueKind.Number)) throw new ArgumentException("id must be a string or number.");
					OpenPCR.JsonInputValidation.RejectDuplicateProperties(request);
					string command = request.GetProperty("command").GetString() ?? throw new ArgumentException("command is required.");
					request.TryGetProperty("arguments", out var arguments);
					if (request.TryGetProperty("clock", out _) || arguments.ValueKind == JsonValueKind.Object && arguments.TryGetProperty("clock", out _) && command != "create")
						throw new ArgumentException("clock belongs in the arguments of create.");
					object result;
					switch (command)
					{
						case "prepare":
							RequireNone();
							var toPrepare = BattleRequest.Parse(arguments.GetProperty("request").GetRawText());
							string prepareResources = arguments.GetProperty("resources").GetString() ?? throw new ArgumentException("resources is required.");
							(PreparedBattleInputs Inputs, BattleExecutionInputs Execution, bool Labyrinth) ready;
							if (toPrepare.ClanBattle is not null) { var clan = BattlePreparer.Prepare(toPrepare, prepareResources); ready = (clan.Inputs, clan.Execution, false); }
							else
							{
								var lab = LabyrinthBattlePreparer.Prepare(toPrepare, prepareResources);
								if (lab.State is not null) throw new NotSupportedException("A Labyrinth EX request is a battle sequence; run it with create.");
								ready = (lab.Inputs, lab.Execution, true);
							}
							long preparationId = ++nextPreparation; preparations.Add(preparationId, ready);
							result = new { preparedId = preparationId, masterVersion = ready.Inputs.Definition.MasterVersion }; break;
						case "create":
							RequireNone();
							var runOptions = arguments.GetProperty("options").Deserialize<BattleRunOptions>(json) ?? throw new ArgumentException("options are required.");
							if (arguments.TryGetProperty("preparedId", out var preparedElement))
							{
								if (arguments.TryGetProperty("request", out _)) throw new ArgumentException("create takes either request or preparedId.");
								var clock = Clock(arguments);
								if (!preparations.Remove(preparedElement.GetInt64(), out var handle)) throw new ArgumentException("Unknown or consumed preparedId.");
								session = BattleSession.Create(handle.Inputs, runOptions, handle.Execution, clock: clock); labyrinth = handle.Labyrinth;
							}
							else
							{
								var toRun = BattleRequest.Parse(arguments.GetProperty("request").GetRawText());
								sequence = BattleSequence.Start(toRun, runOptions, clock: Clock(arguments)); labyrinth = toRun.Labyrinth is not null;
							}
							result = Status(); break;
						case "drop-prepared":
							long dropPrepared = arguments.GetProperty("preparedId").GetInt64();
							if (!preparations.Remove(dropPrepared)) throw new ArgumentException("Unknown or consumed preparedId.");
							result = new { dropped = dropPrepared }; break;
						case "step": AdvanceIfReady(); RequireSession().Step(arguments.GetProperty("frames").GetInt32()); result = Status(); break;
						case "run":
							if (sequence is not null)
							{
								int before = sequence.Index;
								sequence.RunToEnd();
								if (sequence.Index != before) DropCheckpoints();
							}
							else RequireSession().RunToEnd();
							result = Status(); break;
						case "status": result = Status(); break;
						case "result": result = Result(); break;
						case "battle-result": result = RequireSession().ReadBattleResult(); break;
						case "sequence-state": result = sequence?.State ?? throw new InvalidOperationException("sequence-state requires a Labyrinth EX sequence."); break;
						case "terminal-trace": result = RequireSession().ReadTerminalTrace(); break;
						case "inputs": result = RequireSession().ReadPlayerInputs(); break;
						case "begin-ub-exploration": RequireOperations(); RequireSession().BeginUbExploration(); result = RequireSession().ReadUbExploration(); break;
						case "end-ub-exploration": RequireOperations(); RequireSession().EndUbExploration(); result = RequireSession().ReadUbExploration(); break;
						case "ub-status": result = RequireSession().ReadUbExploration(); break;
						case "advance-ub-decision": RequireOperations(); result = RequireSession().AdvanceToUbDecision(arguments.GetProperty("frames").GetInt32()); break;
						case "choose-ub": RequireOperations(); result = RequireSession().ChooseUb(arguments.GetProperty("decisionId").GetInt64(), arguments.GetProperty("unitId").GetInt32()); break;
						case "replace-operations":
							RequireOperations();
							var operations = arguments.GetProperty("operations").Deserialize<PlayerOperation[]>(json) ?? throw new ArgumentException("operations must be an array.");
							RequireSession().ReplaceFutureOperations(operations); result = RequireSession().ReadPlayerInputs(); break;
						case "capture":
							var currentSession = RequireSession();
							var checkpoint = currentSession.ReadUbExploration().Enabled ? currentSession.CaptureUbDecision() : currentSession.Capture();
							long checkpointId = ++nextCheckpoint; checkpoints.Add(checkpointId, checkpoint);
							result = new { checkpointId, objectCount = checkpoint.ObjectCount, progress = currentSession.Progress }; break;
						case "restore":
							long restoreId = arguments.GetProperty("checkpointId").GetInt64();
							if (!checkpoints.TryGetValue(restoreId, out var saved)) throw new ArgumentException("Unknown or expired checkpointId.");
							RequireSession().Restore(saved); result = Status(); break;
						case "drop-checkpoint":
							long dropId = arguments.GetProperty("checkpointId").GetInt64();
							if (!checkpoints.Remove(dropId, out var dropped)) throw new ArgumentException("Unknown or expired checkpointId.");
							dropped.Dispose(); result = new { dropped = dropId }; break;
						case "dispose": DisposeSession(); result = new { disposed = true }; break;
						case "shutdown": DisposeSession(); shutdown = true; result = new { shutdown = true }; break;
						default: throw new ArgumentException("Unknown command: " + command);
					}
					response = new { id, ok = true, result };
				}
				catch (Exception exception)
				{
					response = new { id, ok = false, error = new { code = exception.GetType().Name, message = exception.Message, faulted = (sequence?.Current ?? session)?.IsFaulted ?? false } };
				}
				protocol.WriteLine(JsonSerializer.Serialize(response, json));
				if (shutdown) return 0;
			}
			return 0;
		}
		finally { DisposeSession(); preparations.Clear(); }
	}
}
