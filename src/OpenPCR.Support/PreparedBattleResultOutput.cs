using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Offline terminal boundary: collect the original finish payload without sending API requests or simulating responses.</summary>
public static class PreparedBattleResultOutput
{
	public sealed record Observation(int UpdateIndex, int BattleFrame, int Wave, int GameState, int Result, int MiliLimitTime);
	public sealed record ObservedBoundary(string Kind, Observation Point);
	public sealed record LabyrinthFinish(int EnterId, int BlockId, int QuestId, int RemainTime, object Units, int AutoClear);
	public sealed record LabyrinthExFinish(int EnterId, int BlockId, int QuestId, object Waves, object Enemies, int AutoClear);
	public abstract record Finish(int Phase, int OrderNum, int BattleLogId, object Units, long TotalDamage);
	public sealed record TrialFinish(int ClanBattleId, int Phase, int OrderNum, int BattleLogId, object Units, long TotalDamage) : Finish(Phase, OrderNum, BattleLogId, Units, TotalDamage);
	public abstract record ClanFinish(int ClanId, int Phase, int OrderNum, int BattleLogId, object Units, long BossHp, long BossDamage, int RemainTime, long TotalDamage, int IsAuto, object Timeline, int BattleTime, int StartRemainTime) : Finish(Phase, OrderNum, BattleLogId, Units, TotalDamage);
	public sealed record ClanBattleFinish(int ClanId, int ClanBattleId, int LapNum, int Phase, int OrderNum, int BattleLogId, object Units, long BossHp, long BossDamage, int RemainTime, long TotalDamage, string BattleLog, int IsAuto, object Timeline, int BattleTime, int StartRemainTime) : ClanFinish(ClanId, Phase, OrderNum, BattleLogId, Units, BossHp, BossDamage, RemainTime, TotalDamage, IsAuto, Timeline, BattleTime, StartRemainTime);
	public sealed record ClanRehearsalFinish(int ClanId, int ClanBattleId, int LapNum, int Phase, int OrderNum, int BattleLogId, object Units, long BossHp, long BossDamage, int RemainTime, long TotalDamage, int IsActualBossStatus, int IsAuto, int StockIndex, object Timeline, int BattleTime, int StartRemainTime) : ClanFinish(ClanId, Phase, OrderNum, BattleLogId, Units, BossHp, BossDamage, RemainTime, TotalDamage, IsAuto, Timeline, BattleTime, StartRemainTime);
	public sealed record ClanTrainingFinish(int ClanId, int TrainingId, int ClanBattleMode, int Phase, int OrderNum, int BattleLogId, object Units, long BossHp, long BossDamage, int RemainTime, long TotalDamage, int IsAuto, object Timeline, int BattleTime, int StartRemainTime) : ClanFinish(ClanId, Phase, OrderNum, BattleLogId, Units, BossHp, BossDamage, RemainTime, TotalDamage, IsAuto, Timeline, BattleTime, StartRemainTime);
	private sealed class Session
	{
		internal Finish? Finish;
		internal LabyrinthFinish? Labyrinth;
		internal LabyrinthExFinish? LabyrinthEx;
		internal bool IsLabyrinthEx;
		internal bool ExSendRequested;
		internal bool ExFinishReturned;
		internal Delegate? ResponseCallback;
		internal bool RefreshRequested;
		internal bool Completed;
		internal Func<Observation>? ObservationSource;
		internal readonly List<ObservedBoundary> Observations = new();
	}
	private static readonly ConditionalWeakTable<object, Session> sessions = new();
	public static void Bind(object owner) { ArgumentNullException.ThrowIfNull(owner); sessions.Add(owner, new Session()); }
	public static void BindLabyrinthEx(object owner) { ArgumentNullException.ThrowIfNull(owner); sessions.Add(owner, new Session { IsLabyrinthEx = true }); }
	public static bool IsBound(object? owner) => owner is not null && sessions.TryGetValue(owner, out _);
	/// <summary>Bind a read-only sampler before output starts. Sampling does not advance the battle or complete a send.</summary>
	public static void BindObservationSource(object owner, Func<Observation> source)
	{
		ArgumentNullException.ThrowIfNull(source);
		if (!sessions.TryGetValue(owner, out var session) || session.ObservationSource is not null || session.Finish is not null || session.Labyrinth is not null || session.LabyrinthEx is not null || session.ExSendRequested || session.ExFinishReturned || session.RefreshRequested || session.Completed)
			throw new InvalidOperationException("Terminal observation source is missing, duplicate or late.");
		session.ObservationSource = source;
	}
	public static ObservedBoundary[] ReadObservations(object owner) => sessions.TryGetValue(owner, out var session)
		? session.Observations.ToArray() : throw new InvalidOperationException("Terminal observation owner is unbound.");
	private static ObservedBoundary? Observe(Session session, string kind) => session.ObservationSource is null ? null
		: new(kind, session.ObservationSource() ?? throw new InvalidOperationException("Terminal observation source returned null."));
	private static void Record(Session session, ObservedBoundary? observation) { if (observation is not null) session.Observations.Add(observation); }
	public static void CaptureLabyrinthEx(object owner, LabyrinthExFinish finish, Delegate? responseCallback)
	{
		if (!sessions.TryGetValue(owner, out var session) || !session.IsLabyrinthEx || session.LabyrinthEx is not null || session.ExSendRequested || session.ExFinishReturned || session.Completed)
			throw new InvalidOperationException("Missing, duplicate or out-of-order EX finish request.");
		ArgumentNullException.ThrowIfNull(finish); ArgumentNullException.ThrowIfNull(finish.Waves); ArgumentNullException.ThrowIfNull(finish.Enemies);
		var observation = Observe(session, "request-captured");
		session.LabyrinthEx = finish; session.ResponseCallback = responseCallback;
		Record(session, observation);
	}
	public static void RequestLabyrinthExSend(object owner)
	{
		if (!sessions.TryGetValue(owner, out var session) || !session.IsLabyrinthEx || session.ExSendRequested || session.Completed)
			throw new InvalidOperationException("Missing or duplicate EX result send.");
		var observation = Observe(session, "send-requested");
		session.ExSendRequested = true;
		session.Completed = session.ExFinishReturned;
		Record(session, observation);
	}
	public static void CompleteLabyrinthEx(object owner, bool finalTeam)
	{
		if (!sessions.TryGetValue(owner, out var session) || !session.IsLabyrinthEx || session.ExFinishReturned || session.Completed || finalTeam != (session.LabyrinthEx is not null))
			throw new InvalidOperationException("Missing or inconsistent EX terminal output.");
		var observation = Observe(session, "finish-returned");
		session.ExFinishReturned = true;
		session.Completed = session.ExSendRequested;
		Record(session, observation);
	}
	public static bool IsLabyrinthExComplete(object owner) => sessions.TryGetValue(owner, out var session) && session.IsLabyrinthEx && session.Completed;
	public static LabyrinthExFinish? ReadLabyrinthEx(object owner) => sessions.TryGetValue(owner, out var session) && session.IsLabyrinthEx && session.Completed ? session.LabyrinthEx : null;
	public static void CaptureLabyrinth(object owner, LabyrinthFinish finish, Delegate? responseCallback)
	{
		if (!sessions.TryGetValue(owner, out var session)) throw new InvalidOperationException("Offline result output is unbound.");
		if (session.IsLabyrinthEx || session.Finish is not null || session.Labyrinth is not null || session.Completed) throw new InvalidOperationException("Duplicate battle finish output.");
		ArgumentNullException.ThrowIfNull(finish); ArgumentNullException.ThrowIfNull(finish.Units);
		var observation = Observe(session, "request-captured");
		session.Labyrinth = finish; session.ResponseCallback = responseCallback;
		Record(session, observation);
	}
	public static void Capture(object owner, int clanBattleId, int phase, int orderNum, int battleLogId, object units, long totalDamage)
	{
		if (!sessions.TryGetValue(owner, out var session)) throw new InvalidOperationException("Offline result output is unbound.");
		if (session.IsLabyrinthEx || session.Finish is not null || session.Labyrinth is not null || session.Completed) throw new InvalidOperationException("Duplicate battle finish output.");
		ArgumentNullException.ThrowIfNull(units);
		var observation = Observe(session, "request-captured");
		session.Finish = new TrialFinish(clanBattleId, phase, orderNum, battleLogId, units, totalDamage);
		Record(session, observation);
	}
	public static void CaptureClan(object owner, ClanFinish finish, Delegate? responseCallback)
	{
		if (!sessions.TryGetValue(owner, out var session)) throw new InvalidOperationException("Offline result output is unbound.");
		if (session.IsLabyrinthEx || session.Finish is not null || session.Labyrinth is not null || session.Completed) throw new InvalidOperationException("Duplicate battle finish output.");
		ArgumentNullException.ThrowIfNull(finish);
		ArgumentNullException.ThrowIfNull(finish.Units);
		ArgumentNullException.ThrowIfNull(finish.Timeline);
		var observation = Observe(session, "request-captured");
		session.Finish = finish;
		// Original API stores this delegate for a future response. This boundary ends
		// before that response, so the delegate is retained but never invoked.
		session.ResponseCallback = responseCallback;
		Record(session, observation);
	}
	public static void RequestRefresh(object owner)
	{
		if (!sessions.TryGetValue(owner, out var session) || session.Finish is not TrialFinish || session.RefreshRequested || session.Completed)
			throw new InvalidOperationException("Out-of-order result refresh output.");
		var observation = Observe(session, "refresh-requested");
		session.RefreshRequested = true;
		Record(session, observation);
	}
	public static void Complete(object owner, int category)
	{
		if (category == 59)
		{
			if (!sessions.TryGetValue(owner, out var labyrinth) || labyrinth.Labyrinth is null || labyrinth.Finish is not null || labyrinth.Completed)
				throw new InvalidOperationException("Missing or duplicate Labyrinth finish output.");
			var observation = Observe(labyrinth, "send-requested");
			labyrinth.Completed = true;
			Record(labyrinth, observation);
			return;
		}
		if (!sessions.TryGetValue(owner, out var session) || session.Finish is null || (session.Finish is TrialFinish && !session.RefreshRequested)) throw new InvalidOperationException("Missing original finish/refresh output.");
		if ((category == 32 && session.Finish is not TrialFinish) || (category == 9 && session.Finish is not ClanFinish) || (category != 9 && category != 32)) throw new InvalidOperationException("Finish protocol differs from the original battle category.");
		if (session.Completed) throw new InvalidOperationException("Duplicate terminal output.");
		var send = Observe(session, "send-requested");
		session.Completed = true;
		Record(session, send);
	}
	public static Finish? Read(object owner) => sessions.TryGetValue(owner, out var session) && session.Completed ? session.Finish : null;
	public static LabyrinthFinish? ReadLabyrinth(object owner) => sessions.TryGetValue(owner, out var session) && session.Completed ? session.Labyrinth : null;
	public static void Release(object owner) => sessions.Remove(owner);
}
