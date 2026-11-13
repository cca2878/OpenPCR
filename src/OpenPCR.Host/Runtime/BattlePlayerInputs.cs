using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Elements;
using Elements.Battle;

namespace OpenPCR.Host;

/// <summary>Session-owned operation cursors, preserving the original clock/frame dispatch order.</summary>
internal sealed class BattlePlayerInputs
{
	private sealed record Operation(string Kind, int UnitId, bool Value);
	private Dictionary<int, List<Operation>> clocks = new(), frames = new();
	private readonly List<Operation> initialControls = new();
	private readonly int[] playerIds;
	private readonly float start;
	private int lastClock = -1, lastFrame = -1;
	private int minimumClock, minimumFrame;
	private static void Add(Dictionary<int, List<Operation>> target, int frame, Operation operation)
	{
		if (frame < 0) throw new InvalidDataException("Player operation precedes the prepared battle start.");
		if (!target.TryGetValue(frame, out var list)) target.Add(frame, list = new());
		list.Add(operation);
	}
	internal BattlePlayerInputs(float startTime, IReadOnlyList<int> playerIds, BattleExecutionInputs timeline)
	{
		start = startTime;
		this.playerIds = playerIds.ToArray();
		if (this.playerIds.Distinct().Count() != this.playerIds.Length) throw new InvalidDataException("Duplicate converted player identity.");
		initialControls.Add(new("auto_mode", 0, timeline.Auto));
		var originals = playerIds.Select(UnitUtility.GetOriginalUnitId).ToHashSet();
		if (timeline.ReserveUnitIds.Except(originals).Any()) throw new InvalidDataException("Initial reserve target is absent from party.");
		foreach (int id in playerIds) initialControls.Add(new("ub_set_mode", id, timeline.ReserveUnitIds.Contains(UnitUtility.GetOriginalUnitId(id))));
		ReplaceFuture(timeline.Operations);
	}
	internal PlayerInputState Read()
	{
		IEnumerable<PlayerOperation> Pending(Dictionary<int, List<Operation>> source, PlayerOperationAnchor anchor, int minimum) =>
			source.OrderBy(pair => pair.Key).Where(pair => pair.Key >= minimum).SelectMany(pair => pair.Value.Select(op => new PlayerOperation(anchor, pair.Key, op.Kind, op.UnitId, op.Value)));
		return new(start, lastFrame, lastClock, minimumFrame, minimumClock, playerIds.ToArray(),
			Pending(frames, PlayerOperationAnchor.BattleFrame, minimumFrame).Concat(Pending(clocks, PlayerOperationAnchor.BattleClock, minimumClock)).ToArray());
	}
	internal void ReplaceFuture(IReadOnlyList<PlayerOperation> operations)
	{
		var nextClocks = new Dictionary<int, List<Operation>>();
		var nextFrames = new Dictionary<int, List<Operation>>();
		foreach (var operation in operations)
		{
			if (operation is null) throw new ArgumentException("An operation cannot be null.", nameof(operations));
			if (!Enum.IsDefined(operation.Anchor)) throw new ArgumentException("Unknown operation anchor.", nameof(operations));
			int minimum = operation.Anchor == PlayerOperationAnchor.BattleFrame ? minimumFrame : minimumClock;
			if (operation.At < minimum) throw new ArgumentException($"Operation anchor {operation.At} precedes the editable {operation.Anchor} boundary {minimum}.", nameof(operations));
			if (operation.Kind is not ("auto_mode" or "ub_set_mode" or "manual_ub")) throw new ArgumentException("Unknown player operation: " + operation.Kind, nameof(operations));
			if (operation.Kind == "auto_mode" ? operation.UnitId != 0 : !playerIds.Contains(operation.UnitId)) throw new ArgumentException("Operation target must be a prepared player battle identity (AUTO uses 0): " + operation.UnitId, nameof(operations));
			if (operation.Kind == "manual_ub" && operation.Value) throw new ArgumentException("manual_ub has no boolean value; use false.", nameof(operations));
			Add(operation.Anchor == PlayerOperationAnchor.BattleFrame ? nextFrames : nextClocks, operation.At, new(operation.Kind, operation.UnitId, operation.Value));
		}
		// Publish only after validating the entire batch. No controls or RNG are touched by editing.
		clocks = nextClocks; frames = nextFrames;
	}
	internal void UpdateBoundary(BattleManager manager)
	{
		// Startup coroutines may not have initialized TimeLimit yet. No inputs have been consumed then.
		minimumFrame = lastFrame < 0 ? 0 : Math.Max(lastFrame + 1, (int)manager.FrameCount);
		minimumClock = lastClock < 0 ? 0 : Math.Max(lastClock + 1, Clock(manager));
	}
	private int Clock(BattleManager manager) => (int)(((double)start - manager.TimeLimit) / manager.DeltaTime + 0.5);
	public void Apply(BattleManager manager, UnitUiCtrl ui)
	{
		int frame = (int)manager.FrameCount;
		int clock = Clock(manager);
		void ApplyOperations(IEnumerable<Operation> operations)
		{
			foreach (var operation in operations)
			{
				if (operation.Kind == "auto_mode") { ui.SetAutoMode(operation.Value); continue; }
				var unit = manager.UnitList.SingleOrDefault(value => value.UnitId == operation.UnitId) ?? throw new InvalidDataException("Operation target is not a prepared player: " + operation.UnitId);
				if (operation.Kind == "ub_set_mode") ui.SkillReserves[unit.UnitId] = operation.Value;
				else
				{
					// Native ExecReserveSKill uses the footer's actual slot, not UnitCtrl.Index.
					int slot = Array.IndexOf(ui.UnitCtrls, unit);
					if (slot < 0) throw new InvalidDataException("Operation target has no player footer slot: " + operation.UnitId);
					RuntimeBinding.Call(ui, "performTapPanelAction", slot, true, false);
				}
			}
		}
		if (frame != lastFrame) { lastFrame = frame; if (frames.TryGetValue(frame, out var inputs)) ApplyOperations(inputs); }
		if (clock != lastClock)
		{
			lastClock = clock;
			if (clock == 0) ApplyOperations(initialControls);
			if (clocks.TryGetValue(clock, out var inputs)) ApplyOperations(inputs);
		}
	}
}
