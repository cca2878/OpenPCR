using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>The player TimeManager's 16-byte RationalTime, without rounding it to a float first.</summary>
	public readonly struct EngineRationalTime
	{
		public readonly long Count;
		public readonly uint Numerator, Denominator;
		public EngineRationalTime(long count, uint numerator, uint denominator)
		{
			Count = count; Numerator = numerator; Denominator = denominator;
		}
		public float Seconds => (float)((double)Count * (Numerator == 0 ? 0d : (double)Denominator / Numerator));
	}

	/// <summary>Update-phase TimeManager fields. Clock production belongs to the frame driver.</summary>
	public readonly struct EngineFrameTime
	{
		public readonly EngineRationalTime Time, DeltaTime, UnscaledTime, UnscaledDeltaTime;
		public readonly int FrameCount;
		public EngineFrameTime(EngineRationalTime time, EngineRationalTime deltaTime,
			EngineRationalTime unscaledTime, EngineRationalTime unscaledDeltaTime, int frameCount)
		{
			Time = time; DeltaTime = deltaTime; UnscaledTime = unscaledTime;
			UnscaledDeltaTime = unscaledDeltaTime; FrameCount = frameCount;
		}
	}

	/// <summary>
	/// Process-global TimeManager consumption boundary. The host explicitly initializes and publishes the native
	/// rational fields at each Update boundary. It must produce them from its engine frame driver, not captured battle
	/// snapshots. No wall clock, fixed timestep, accumulated float clock or default scale is synthesized here.
	/// </summary>
	public static class ScriptingTime
	{
		private static EngineFrameTime frame;
		private static float scale;
		private static bool initialized;
		public static bool IsInitialized => initialized;

		public static void Initialize(EngineFrameTime initialFrame, float initialTimeScale)
		{
			if (initialized) throw new InvalidOperationException("TimeManager is already initialized; end the previous engine session explicitly.");
			frame = initialFrame;
			scale = initialTimeScale;
			initialized = true;
		}
		public static void PublishUpdate(EngineFrameTime nextFrame)
		{
			RequireInitialized();
			frame = nextFrame;
		}
		public static void EndSession() { initialized = false; frame = default; scale = default; }
		private static void RequireInitialized()
		{
			if (!initialized) throw new InvalidOperationException("TimeManager has no engine frame input. Initialize the engine clock before reading OpenPCR.Runtime.Api.Time.");
		}
		public static float Time { get { RequireInitialized(); return frame.Time.Seconds; } }
		public static float DeltaTime { get { RequireInitialized(); return frame.DeltaTime.Seconds; } }
		public static float UnscaledTime { get { RequireInitialized(); return frame.UnscaledTime.Seconds; } }
		public static float UnscaledDeltaTime { get { RequireInitialized(); return frame.UnscaledDeltaTime.Seconds; } }
		public static int FrameCount { get { RequireInitialized(); return frame.FrameCount; } }
		public static float TimeScale
		{
			get { RequireInitialized(); return scale; }
			set
			{
				RequireInitialized();
				// Locked player setter logs NaN/negative input and leaves the previous field unchanged.
				// Diagnostic rendering and profiler hooks are outside the static battle projection.
				if (!float.IsNaN(value) && value >= 0f) scale = value;
			}
		}
	}

}
