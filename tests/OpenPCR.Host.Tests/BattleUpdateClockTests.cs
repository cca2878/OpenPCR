using System;
using OpenPCR.Host;
using OpenPCR.Runtime.Impl;

internal static class BattleUpdateClockTests
{
	internal static void Run(Action<bool, string> check)
	{
		void Reject(Action action, string message)
		{
			bool rejected = false;
			try { action(); } catch (ArgumentException) { rejected = true; }
			check(rejected, message);
		}
		EngineFrameTime Frame(int count, long time, long delta) => new(new(time, 120, 1), new(delta, 120, 1), new(time, 120, 1), new(delta, 120, 1), count);
		var fixedClock = BattleUpdateClock.FixedRate(60);
		check(fixedClock.InitialFrame.FrameCount == 0 && fixedClock.InitialFrame.Time.Count == 0 && fixedClock.InitialTimeScale == 1f, "Fixed clock changed the default origin or scale.");
		check(fixedClock.ReadUpdate(123).Time.Seconds == (float)(123d / 60) && fixedClock.ReadUpdate(123).DeltaTime.Seconds == 1f / 60f, "Fixed clock rounded before rational conversion.");
		check(BattleUpdateClock.FixedRate(30).ReadUpdate(1).DeltaTime.Seconds == 1f / 30f, "Update clock did not accept an independent render frequency.");
		Reject(() => BattleUpdateClock.FixedRate(0), "Zero render rate accepted.");
		Reject(() => fixedClock.ReadUpdate(0), "Initial frame accepted as an Update index.");
		var updates = new[] { Frame(101, 1002, 2), Frame(102, 1002, 0), Frame(103, 1007, 5) };
		var clock = BattleUpdateClock.Timeline(Frame(100, 1000, 0), 0.5f, updates);
		updates[0] = Frame(999, 9999, 9);
		check(clock.InitialFrame.FrameCount == 100 && clock.InitialTimeScale == 0.5f && clock.ReadUpdate(1).FrameCount == 101, "Timeline retained caller storage or discarded its origin.");
		check(clock.ReadUpdate(2).DeltaTime.Count == 0 && clock.ReadUpdate(3).DeltaTime.Count == 5, "Timeline replaced zero or variable deltas.");
		check(clock.ReadUpdate(1).Time.Count == 1002, "Clock reads retained a mutable cursor rather than selecting the requested update.");
		bool exhausted = false;
		try { clock.ReadUpdate(4); } catch (InvalidOperationException) { exhausted = true; }
		check(exhausted, "Missing Update input silently extended the clock.");
		Reject(() => BattleUpdateClock.Timeline(Frame(0, 0, 0), float.NaN, updates), "Nonfinite scale accepted.");
		Reject(() => BattleUpdateClock.Timeline(Frame(0, 0, 0), -1f, updates), "Negative scale accepted.");
		Reject(() => BattleUpdateClock.Timeline(Frame(1, 4, 0), 1f, new[] { Frame(1, 5, 1) }), "Repeated frame count accepted.");
		Reject(() => BattleUpdateClock.Timeline(Frame(1, 4, 0), 1f, new[] { Frame(2, 3, 1) }), "Reversed time accepted.");
		Reject(() => BattleUpdateClock.Timeline(Frame(1, long.MaxValue, 0), 1f, new[] { Frame(2, long.MaxValue - 1, 0) }), "Rational time comparison lost an integer reversal to float rounding.");
		var asymmetric = new EngineFrameTime(new(1, 30, 1), new(0, 30, 1), new(7, 60, 1), new(2, 60, 1), 1);
		var paused = BattleUpdateClock.Timeline(Frame(0, 0, 0), 0f, new[] { asymmetric }).ReadUpdate(1);
		check(paused.DeltaTime.Count == 0 && paused.UnscaledDeltaTime.Count == 2 && paused.UnscaledTime.Count == 7, "Timeline conflated scaled and unscaled clock inputs.");
		Reject(() => BattleUpdateClock.Timeline(Frame(0, 0, 0), 1f, new[] { Frame(1, 1, -1) }), "Negative delta accepted.");
		Reject(() => BattleUpdateClock.Timeline(default, 1f, Array.Empty<EngineFrameTime>()), "Invalid rational time accepted.");
		Reject(() => BattleUpdateClock.Timeline(Frame(0, 0, 0), 1f, null!), "Null timeline accepted.");
		check(CheckpointSharing.IsPreparedReadOnly(typeof(BattleUpdateClock)), "Immutable clock input was not classified for checkpoint sharing.");
	}
}
