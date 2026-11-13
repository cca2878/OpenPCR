using global::OpenPCR.Runtime.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using Elements;
using Elements.Battle;
using SavedCheckpoint = OpenPCR.Checkpoint.Checkpoint;

namespace OpenPCR.Host;

public sealed partial class BattleSession
{
	// Ownership, immutable node descriptions and instrumentation are deliberately outside the memento.
	private readonly Dictionary<long, UbDecision> ubDecisions = new();
	private UbBattleManager? ubBridge;
	private SavedCheckpoint? ubAnchor;
	private int ubAnchorFrame;
	private int? ubReplayChoice;
	private long nextUbDecision, ubCaptures, ubProbedFrames, ubReplayedFrames;

	public UbExplorationStatus ReadUbExploration()
	{
		RequireAvailable();
		UbDecision? decision = state.UbDecisionId == 0 ? null : ubDecisions[state.UbDecisionId];
		return new(state.UbExploration, decision is null ? null : decision with { UnitIds = decision.UnitIds.ToArray() },
			readProgress(), isCompleted(), state.EngineFrame >= options.MaxEngineFrames, ubCaptures, ubProbedFrames, ubReplayedFrames);
	}
	/// <summary>Replace future player controls by explicit UB decisions. Enemy/summon automation remains live.</summary>
	public void BeginUbExploration()
	{
		RequireAvailable();
		if (state.UbExploration) throw new InvalidOperationException("UB exploration is already enabled.");
		if (Completed || Exhausted) throw new InvalidOperationException("Restore an earlier checkpoint before beginning UB exploration.");
		busy = true;
		try
		{
			state.PlayerInputs.ReplaceFuture(Array.Empty<PlayerOperation>());
			state.UbExploration = true; state.UbDecisionId = 0;
			SetUbAnchor(CaptureUbAnchor());
		}
		catch { faulted = true; throw; }
		finally { busy = false; }
	}
	/// <summary>Resume ordinary frame stepping with the current controls and an empty future input queue.</summary>
	public void EndUbExploration()
	{
		RequireAvailable(); state.UbExploration = false; state.UbDecisionId = 0;
		ClearUbAnchor();
	}
	/// <summary>Return an independent handle to the cached decision image; no new graph capture is performed.</summary>
	public SavedCheckpoint CaptureUbDecision()
	{
		RequireAvailable();
		if (!state.UbExploration || state.UbDecisionId == 0 || ubAnchor is null || ubAnchorFrame != state.EngineFrame)
			throw new InvalidOperationException("Advance to a UB decision before capturing an exploration checkpoint.");
		return checkpoints.Retain(ubAnchor);
	}
	/// <summary>Stop at the complete frame preceding the next legal UB, or after the requested frame budget.</summary>
	public UbExplorationStatus AdvanceToUbDecision(int maxEngineFrames)
	{
		RequireAvailable(); RequireUbExploration();
		if (maxEngineFrames <= 0) throw new ArgumentOutOfRangeException(nameof(maxEngineFrames));
		if (state.UbDecisionId != 0 || Completed || Exhausted) return ReadUbExploration();
		int fromFrame = state.EngineFrame;
		int limit = (int)Math.Min(options.MaxEngineFrames, (long)fromFrame + maxEngineFrames);
		var observer = ScriptingRandom.Observer;
		busy = true;
		try
		{
			// Ordinary/manual snapshots may have been restored between decisions. Keep one independent root handle.
			if (ubAnchor is null) SetUbAnchor(CaptureUbAnchor());
			var participantState = checkpointParticipants.Select(p => p.Capture()).ToArray();
			ScriptingRandom.Observer = null;
			int found = 0, triggerFrame = 0;
			while (state.EngineFrame < limit && !isCompleted())
			{
				found = ProbeUbFrame(-1);
				if (found != 0) { triggerFrame = state.EngineFrame; break; }
			}
			int destination = found == 0 ? state.EngineFrame : triggerFrame - 1;
			RestoreUbAnchor();
			// Reconstruct already committed frames silently. Restore observers' cursors at the entry boundary.
			ReplayUbFrames(fromFrame, false);
			for (int i = 0; i < checkpointParticipants.Count; i++) checkpointParticipants[i].Restore(participantState[i]);
			ScriptingRandom.Observer = observer;
			ReplayUbFrames(destination, true);
			if (found != 0)
			{
				long id = ++nextUbDecision;
				state.UbDecisionId = id;
				SetUbAnchor(CaptureUbAnchor()); // One image at the actual decision, never one per traversed frame.
				ScriptingRandom.Observer = null;
				var candidates = new List<int>();
				foreach (int unitId in ubBridge!.PlayerIdsInEngineOrder())
				{
					if (unitId == found || ProbeUbFrame(unitId) == unitId) candidates.Add(unitId);
					RestoreUbAnchor();
				}
				ubDecisions.Add(id, new(id, triggerFrame, candidates.ToArray()));
			}
		}
		catch { faulted = true; throw; }
		finally { ScriptingRandom.Observer = observer; ubBridge!.ResetProbe(); busy = false; }
		return ReadUbExploration();
	}
	/// <summary>Resolve this decision for exactly one engine frame. UnitId 0 waits and keeps later timing choices.</summary>
	public UbExplorationStatus ChooseUb(long decisionId, int unitId)
	{
		RequireAvailable(); RequireUbExploration();
		if (state.UbDecisionId == 0 || decisionId != state.UbDecisionId) throw new ArgumentException("Unknown or stale UB decisionId.", nameof(decisionId));
		if (unitId != 0 && !ubDecisions[decisionId].UnitIds.Contains(unitId)) throw new ArgumentException("Unit is not a legal choice at this UB decision.", nameof(unitId));
		busy = true;
		try
		{
			ubReplayChoice = unitId; state.UbDecisionId = 0;
			CommitUbFrame(unitId, true);
		}
		catch { faulted = true; throw; }
		finally { busy = false; }
		return ReadUbExploration();
	}
	private void RequireUbExploration()
	{
		if (!state.UbExploration) throw new InvalidOperationException("Begin UB exploration first.");
	}
	private SavedCheckpoint CaptureUbAnchor()
	{
		checkpoints ??= CreateCheckpointContext();
		var saved = checkpoints.Capture(); ubCaptures++; return saved;
	}
	private void SetUbAnchor(SavedCheckpoint saved)
	{
		ubAnchor?.Dispose(); ubAnchor = saved; ubAnchorFrame = state.EngineFrame; ubReplayChoice = null;
	}
	private void ClearUbAnchor() { ubAnchor?.Dispose(); ubAnchor = null; ubReplayChoice = null; }
	private void RestoreUbAnchor()
	{
		SceneScripting.EndSession(); checkpoints.Restore(ubAnchor!);
	}
	private void ReplayUbFrames(int destination, bool notify)
	{
		while (state.EngineFrame < destination)
		{
			int request = state.EngineFrame == ubAnchorFrame ? ubReplayChoice ?? 0 : 0;
			state.UbDecisionId = 0;
			CommitUbFrame(request, notify); ubReplayedFrames++;
		}
	}
	private int ProbeUbFrame(int unitId)
	{
		ubBridge!.Probe = true; ubBridge.Request = unitId; ubProbedFrames++;
		try { stepFrame(); return 0; }
		catch (UbProbeStop stop) { return stop.UnitId; }
		finally { ubBridge.ResetProbe(); }
	}
	private void CommitUbFrame(int unitId, bool notify)
	{
		ubBridge!.Request = unitId;
		try
		{
			stepFrame();
			if (unitId != 0 && ubBridge.Selected != unitId) throw new InvalidOperationException("Validated UB choice did not reach the original engine commit.");
			if (notify && state.EngineFrame % 60 == 0) progress?.Invoke(readProgress());
		}
		finally { ubBridge.ResetProbe(); }
	}
	private sealed class UbProbeStop(int unitId) : Exception
	{
		internal int UnitId { get; } = unitId;
	}
	/// <summary>Interpose only on the existing interface. Eligibility, targeting and ordering run in Engine.</summary>
	private sealed class UbBattleManager(BattleSession session, BattleManager manager, int[] playerIds) : IBattleManagerForBattleUnionBurstController
	{
		internal bool Probe;
		internal int Request, Selected;
		private UnitCtrl? current;
		internal void ResetProbe() { Probe = false; Request = Selected = 0; current = null; }
		private bool IsControllable(UnitCtrl unit) => playerIds.Contains(unit.UnitId) &&
			manager.UnitUiCtrl.UnitCtrls.Any(slot => ReferenceEquals(slot, unit));
		internal int[] PlayerIdsInEngineOrder() => manager.UnitList.Where(IsControllable).Select(u => u.UnitId).ToArray();
		internal void NormalizeControls()
		{
			manager.UnitUiCtrl.SetAutoMode(false);
			foreach (int id in playerIds) manager.UnitUiCtrl.SkillReserves[id] = false;
		}
		public eChargeSkillTurn ChargeSkillTurn
		{
			get => manager.ChargeSkillTurn;
			set
			{
				if (session.state.UbExploration && value == eChargeSkillTurn.PLAYER && current is not null && IsControllable(current))
				{
					if (Probe) throw new UbProbeStop(current.UnitId); // Before the original commit, never exposed as a safe point.
					Selected = current.UnitId;
				}
				manager.ChargeSkillTurn = value;
			}
		}
		public UnitCtrl GetUnitCtrl(int index)
		{
			current = manager.GetUnitCtrl(index);
			if (session.state.UbExploration && IsControllable(current))
				current.ComponentUnionBurst.SetIsUbExecTrying(Request == -1 || Request == current.UnitId);
			return current;
		}
		public UnitCtrl UnionBurstUseUnit { get => manager.UnionBurstUseUnit; set => manager.UnionBurstUseUnit = value; }
		public UnitUiCtrl UnitUiCtrl => manager.UnitUiCtrl;
		public bool IsDefenseReplayMode => manager.IsDefenseReplayMode;
		public eBattleGameState GameState => manager.GameState;
		public int GetEnemyCtrlLength() => manager.GetEnemyCtrlLength();
		public UnitCtrl GetEnemyCtrl(int index) => manager.GetEnemyCtrl(index);
		public bool GetIsPlayCutin() => manager.GetIsPlayCutin();
		public int GetUnitCtrlLength() => manager.GetUnitCtrlLength();
	}
}
