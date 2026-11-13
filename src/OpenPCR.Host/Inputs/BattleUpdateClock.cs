using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using OpenPCR.Runtime.Impl;

namespace OpenPCR.Host;

/// <summary>Immutable external Update clock. Indices are session updates, independent of battle frames.</summary>
public sealed class BattleUpdateClock
{
	private readonly uint rate;
	private readonly EngineFrameTime[]? updates;
	public EngineFrameTime InitialFrame { get; }
	public float InitialTimeScale { get; }
	private BattleUpdateClock(uint rate)
	{
		if (rate == 0) throw new ArgumentOutOfRangeException(nameof(rate));
		this.rate = rate;
		InitialFrame = new(new(0, rate, 1), new(0, rate, 1), new(0, rate, 1), new(0, rate, 1), 0);
		InitialTimeScale = 1f;
	}
	private BattleUpdateClock(EngineFrameTime initialFrame, float initialTimeScale, IEnumerable<EngineFrameTime> updates)
	{
		ArgumentNullException.ThrowIfNull(updates);
		if (!float.IsFinite(initialTimeScale) || initialTimeScale < 0f) throw new ArgumentOutOfRangeException(nameof(initialTimeScale));
		Validate(initialFrame);
		this.updates = updates.ToArray();
		var previous = initialFrame;
		foreach (var frame in this.updates)
		{
			Validate(frame);
			if (frame.FrameCount <= previous.FrameCount || Before(frame.Time, previous.Time) || Before(frame.UnscaledTime, previous.UnscaledTime))
				throw new ArgumentException("Update clock must advance frame counts and must not reverse time.", nameof(updates));
			previous = frame;
		}
		InitialFrame = initialFrame;
		InitialTimeScale = initialTimeScale;
	}
	/// <summary>Scaled and unscaled clocks both advance by 1/rate; no wall clock is read.</summary>
	public static BattleUpdateClock FixedRate(uint rate) => new(rate);
	/// <summary>Copies a finite sequence. Exhaustion fails rather than extending the last delta.</summary>
	public static BattleUpdateClock Timeline(EngineFrameTime initialFrame, float initialTimeScale, IEnumerable<EngineFrameTime> updates)
		=> new(initialFrame, initialTimeScale, updates);
	internal EngineFrameTime ReadUpdate(int update)
	{
		if (update <= 0) throw new ArgumentOutOfRangeException(nameof(update));
		if (updates is null) return new(new(update, rate, 1), new(1, rate, 1), new(update, rate, 1), new(1, rate, 1), update);
		if (update > updates.Length) throw new InvalidOperationException($"Update clock has no input for session update {update}.");
		return updates[update - 1];
	}
	private static bool Before(EngineRationalTime value, EngineRationalTime previous)
		=> (BigInteger)value.Count * value.Denominator * previous.Numerator < (BigInteger)previous.Count * previous.Denominator * value.Numerator;
	private static void Validate(EngineFrameTime frame)
	{
		if (frame.FrameCount < 0) throw new ArgumentException("Update clock frame count must be nonnegative.");
		foreach (var value in new[] { frame.Time, frame.DeltaTime, frame.UnscaledTime, frame.UnscaledDeltaTime })
			if (value.Count < 0 || value.Numerator == 0 || value.Denominator == 0 || !float.IsFinite(value.Seconds))
				throw new ArgumentException("Update clock requires finite nonnegative rational times with positive rates.");
	}
}
