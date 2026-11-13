using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>
/// Offline decoder input: one pending query, then a completed
/// segment while battle frames are frozen. The positive marker is dimensionless;
/// it does not describe a video duration or advance any game clock.
/// </summary>
public static class PreparedPrincessMoviePlayback
{
	private sealed class Segment
	{
		internal int DurationQueries;
		internal bool EndTypeSet;
	}
	private sealed class Session
	{
		internal readonly Func<int, long, int, bool> IsPreparedSegment;
		internal readonly Dictionary<(int Type, long Id, int Index), Segment> Segments = new();
		internal Session(Func<int, long, int, bool> isPreparedSegment) => IsPreparedSegment = isPreparedSegment;
	}
	private static readonly ConditionalWeakTable<object, Session> sessions = new();
	public static void Bind(object owner, Func<int, long, int, bool> isPreparedSegment)
	{
		ArgumentNullException.ThrowIfNull(owner);
		ArgumentNullException.ThrowIfNull(isPreparedSegment);
		sessions.Add(owner, new(isPreparedSegment));
	}
	public static bool IsBound(object? owner) => owner is not null && sessions.TryGetValue(owner, out _);
	public static void Start(object owner, int type, long id, int index)
	{
		if (!sessions.TryGetValue(owner, out var session) || !session.IsPreparedSegment(type, id, index)) throw new InvalidOperationException("Unprepared princess movie receiver or segment.");
		var key = (type, id, index);
		if (session.Segments.TryGetValue(key, out var previous) && !previous.EndTypeSet) throw new InvalidOperationException("Princess movie segment restarted before completion.");
		session.Segments[key] = new();
	}
	private static Segment Require(object owner, int type, long id, long index)
	{
		if (!sessions.TryGetValue(owner, out var session) || index <= 0 || index > int.MaxValue || !session.Segments.TryGetValue((type, id, (int)index), out var segment)) throw new InvalidOperationException("Princess movie query requires an actual started segment.");
		return segment;
	}
	public static float Duration(object owner, int type, long id, long index)
	{
		var segment = Require(owner, type, id, index);
		if (segment.DurationQueries == 0) { segment.DurationQueries = 1; return 0f; }
		segment.DurationQueries = 2;
		return 1f;
	}
	public static float Seek(object owner, int type, long id, long index)
	{
		if (Require(owner, type, id, index).DurationQueries != 2) throw new InvalidOperationException("Princess movie position queried before completion input.");
		return 1f;
	}
	public static void SetEndTypeDestroy(object owner, int type, long id, long index)
	{
		var segment = Require(owner, type, id, index);
		if (segment.DurationQueries != 2 || segment.EndTypeSet) throw new InvalidOperationException("Out-of-order princess movie end output.");
		segment.EndTypeSet = true;
	}
	public static void Release(object owner) => sessions.Remove(owner);
}
