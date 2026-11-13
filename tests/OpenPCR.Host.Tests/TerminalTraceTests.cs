using System;
using System.Linq;
using OpenPCR;

internal static class TerminalTraceTests
{
	internal static void Run(Action<bool, string> check)
	{
		var owner = new object(); int frame = 100, reads = 0;
		PreparedBattleResultOutput.Observation Sample() { reads++; return new(10, frame, 1, 1, 1, 90000); }
		void Reject(Action action)
		{
			int before = reads; bool rejected = false;
			try { action(); } catch (Exception error) when (error is InvalidOperationException or ArgumentNullException) { rejected = true; }
			check(rejected && reads == before, "Invalid terminal operation sampled or changed an unbound output.");
		}
		Reject(() => PreparedBattleResultOutput.BindObservationSource(owner, Sample));
		Reject(() => PreparedBattleResultOutput.ReadObservations(owner));
		PreparedBattleResultOutput.Bind(owner);
		try
		{
			check(PreparedBattleResultOutput.ReadObservations(owner).Length == 0, "Fresh trace contains an event.");
			Reject(() => PreparedBattleResultOutput.BindObservationSource(owner, null!));
			PreparedBattleResultOutput.BindObservationSource(owner, Sample);
			Reject(() => PreparedBattleResultOutput.BindObservationSource(owner, Sample));
			Reject(() => PreparedBattleResultOutput.Complete(owner, 59));
			PreparedBattleResultOutput.CaptureLabyrinth(owner, new(1, 2, 3, 90000, new object(), 0), null);
			frame = 104;
			Reject(() => PreparedBattleResultOutput.Complete(owner, 32));
			PreparedBattleResultOutput.Complete(owner, 59);
			var events = PreparedBattleResultOutput.ReadObservations(owner);
			check(events.Select(e => e.Kind).SequenceEqual(new[] { "request-captured", "send-requested" }) && events[0].Point.BattleFrame == 100 && events[1].Point.BattleFrame == 104, "Terminal point was reconstructed at read time instead of sampled at the producer.");
			events[0] = events[1];
			check(PreparedBattleResultOutput.ReadObservations(owner)[0].Point.BattleFrame == 100 && reads == 2, "Trace read exposed mutable storage or sampled again.");
			Reject(() => PreparedBattleResultOutput.Complete(owner, 59));
		}
		finally { PreparedBattleResultOutput.Release(owner); }
		foreach (bool sendFirst in new[] { false, true })
		{
			PreparedBattleResultOutput.BindLabyrinthEx(owner);
			try
			{
				PreparedBattleResultOutput.BindObservationSource(owner, Sample);
				if (sendFirst) PreparedBattleResultOutput.RequestLabyrinthExSend(owner);
				else PreparedBattleResultOutput.CompleteLabyrinthEx(owner, false);
				check(!PreparedBattleResultOutput.IsLabyrinthExComplete(owner), "Observing one EX boundary completed the other gate.");
				frame++;
				if (sendFirst) PreparedBattleResultOutput.CompleteLabyrinthEx(owner, false);
				else PreparedBattleResultOutput.RequestLabyrinthExSend(owner);
				check(PreparedBattleResultOutput.IsLabyrinthExComplete(owner) && PreparedBattleResultOutput.ReadObservations(owner).Select(e => e.Kind).SequenceEqual(sendFirst ? new[] { "send-requested", "finish-returned" } : new[] { "finish-returned", "send-requested" }), "EX observations reordered the Native send and data gates.");
			}
			finally { PreparedBattleResultOutput.Release(owner); }
		}
		PreparedBattleResultOutput.Bind(owner);
		try
		{
			bool fail = true;
			PreparedBattleResultOutput.BindObservationSource(owner, () => fail ? throw new InvalidOperationException("sampler failure") : Sample());
			Reject(() => PreparedBattleResultOutput.CaptureLabyrinth(owner, new(1, 2, 3, 90000, new object(), 0), null));
			check(PreparedBattleResultOutput.ReadObservations(owner).Length == 0, "Failed sampler left a phantom event.");
			fail = false;
			PreparedBattleResultOutput.CaptureLabyrinth(owner, new(1, 2, 3, 90000, new object(), 0), null);
			check(PreparedBattleResultOutput.ReadObservations(owner).Length == 1, "Failed sampler consumed the original output slot.");
		}
		finally { PreparedBattleResultOutput.Release(owner); }
		PreparedBattleResultOutput.Bind(owner);
		try
		{
			PreparedBattleResultOutput.CaptureLabyrinth(owner, new(1, 2, 3, 90000, new object(), 0), null);
			Reject(() => PreparedBattleResultOutput.BindObservationSource(owner, Sample));
			PreparedBattleResultOutput.Complete(owner, 59);
			check(PreparedBattleResultOutput.ReadObservations(owner).Length == 0, "Legacy receiver without a sampler changed behavior.");
		}
		finally { PreparedBattleResultOutput.Release(owner); }
	}
}
