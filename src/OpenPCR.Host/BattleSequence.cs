using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenPCR.Host;

/// <summary>One battle of a run: its common result, the mode settlement once completed, and the next battle's start state.</summary>
/// <param name="Settlement">Clan or training-ground settlement, Labyrinth settlement, or Labyrinth EX party settlement; null until completed.</param>
/// <param name="Next">Start state of the next EX party; null when this battle ended the run or did not complete.</param>
public sealed record BattleRunEntry(int Index, BattleResult Battle, object? Settlement, LabyrinthExState? Next);

/// <summary>Result <c>openpcr-battle-result/v2</c> of a whole request.</summary>
/// <param name="Completed">False when a battle exhausted its engine-frame budget before the run ended.</param>
/// <param name="Outcome">Summary of a multi-battle run once it ended (Labyrinth EX: the final enemies, waves and clear type).</param>
public sealed record BattleRunResult(string Schema, bool Completed, IReadOnlyList<BattleRunEntry> Battles, object? Outcome)
{
	public const string CurrentSchema = "openpcr-battle-result/v2";
}

/// <summary>
/// Runs a request's battle sequence in one process. A clan or category 59 Labyrinth request is one battle; a Labyrinth EX
/// request runs party by party, each party starting from the previous party's settlement. Only one battle is active at a time;
/// checkpoints taken on <see cref="Current"/> are valid until <see cref="Advance"/>.
/// </summary>
public sealed class BattleSequence : IDisposable
{
	private readonly BattleRequest request;
	private readonly BattleRunOptions options;
	private readonly Action<BattleProgress>? progress;
	private readonly BattleUpdateClock? clock;
	private readonly List<BattleRunEntry> finished = new();
	private bool disposed;

	/// <summary>The active battle.</summary>
	public BattleSession Current { get; private set; }
	/// <summary>Zero-based index of the active battle in Battles.</summary>
	public int Index { get; private set; }
	/// <summary>For Labyrinth EX, the state the active party started from; null otherwise.</summary>
	public LabyrinthExState? State { get; private set; }

	private BattleSequence(BattleRequest request, BattleRunOptions options, Action<BattleProgress>? progress, BattleUpdateClock? clock)
	{
		this.request = request; this.options = options; this.progress = progress; this.clock = clock;
		Current = Create(request.Labyrinth?.StartState);
	}

	/// <summary>Prepare and create the first battle (for EX: party 0, or Labyrinth.start_state).</summary>
	public static BattleSequence Start(BattleRequest request, BattleRunOptions options, Action<BattleProgress>? progress = null, BattleUpdateClock? clock = null)
	{
		ArgumentNullException.ThrowIfNull(request); ArgumentNullException.ThrowIfNull(options);
		request.Validate();
		return new(request, options, progress, clock);
	}

	/// <summary>Run every battle of the request and return the result.</summary>
	public static BattleRunResult Run(BattleRequest request, BattleRunOptions options, Action<BattleProgress>? progress = null, BattleUpdateClock? clock = null)
	{
		using var sequence = Start(request, options, progress, clock);
		return sequence.RunToEnd();
	}

	private BattleSession Create(LabyrinthExState? state)
	{
		if (request.ClanBattle is not null)
		{
			var (inputs, execution) = BattlePreparer.Prepare(request, options.Resources);
			Index = 0; State = null;
			return BattleSession.Create(inputs, options, execution, progress, clock: clock);
		}
		var prepared = LabyrinthBattlePreparer.Prepare(request, options.Resources, state);
		State = prepared.State; Index = prepared.State?.PartyIndex ?? 0;
		return BattleSession.Create(prepared.Inputs, options, prepared.Execution, progress, clock: clock);
	}

	private bool IsEx => State is not null;
	/// <summary>True when the active battle completed and another EX party follows.</summary>
	public bool CanAdvance { get { RequireActive(); return IsEx && Current.Completed && !Current.ReadLabyrinthExSettlement().IsFinalTeam; } }
	/// <summary>True when the active battle completed and ended the run.</summary>
	public bool IsComplete { get { RequireActive(); return Current.Completed && (!IsEx || Current.ReadLabyrinthExSettlement().IsFinalTeam); } }

	/// <summary>The active battle's entry; the settlement is present once it completed.</summary>
	public BattleRunEntry ReadEntry()
	{
		RequireActive();
		var battle = Current.ReadBattleResult();
		if (!battle.Completed) return new(Index, battle, null, null);
		if (request.ClanBattle is not null) return new(Index, battle, Current.ReadClanSettlement(), null);
		if (!IsEx) return new(Index, battle, Current.ReadLabyrinthSettlement(), null);
		var settlement = Current.ReadLabyrinthExSettlement();
		return new(Index, battle, settlement, settlement.IsFinalTeam ? null : LabyrinthExState.FromSettlement(settlement));
	}

	/// <summary>Finish the active EX party and create the next party's battle from its settlement.</summary>
	public void Advance()
	{
		if (!CanAdvance) throw new InvalidOperationException("Battle " + Index + " has not completed or ended the run.");
		var entry = ReadEntry();
		Current.Dispose();
		finished.Add(entry);
		Current = Create(entry.Next!);
	}

	/// <summary>Run the active and remaining battles until the run ends or a battle exhausts its frame budget.</summary>
	public BattleRunResult RunToEnd()
	{
		RequireActive();
		while (true)
		{
			Current.RunToEnd();
			if (!Current.Completed || !CanAdvance) return ReadResult();
			Advance();
		}
	}

	/// <summary>Battles finished so far plus the active one; the outcome is present once the run ended.</summary>
	public BattleRunResult ReadResult()
	{
		RequireActive();
		var entry = ReadEntry();
		var battles = finished.Append(entry).ToArray();
		bool complete = entry.Battle.Completed && IsComplete;
		object? outcome = complete && entry.Settlement is LabyrinthExBattleSettlement ex ? ex.FinishRequest : null;
		return new(BattleRunResult.CurrentSchema, complete, battles, outcome);
	}

	private void RequireActive() { if (disposed) throw new ObjectDisposedException(nameof(BattleSequence)); }
	public void Dispose()
	{
		if (disposed) return;
		disposed = true;
		Current.Dispose();
	}
}
