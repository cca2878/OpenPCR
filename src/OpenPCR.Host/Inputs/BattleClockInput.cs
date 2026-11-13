using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenPCR.Runtime.Impl;

namespace OpenPCR.Host;

/// <summary>Independent finite Update input. Every rational field is supplied, never inferred from battle FPS.</summary>
public sealed record BattleClockInput([property: JsonRequired] string Schema,
	[property: JsonRequired] BattleClockFrame InitialFrame, [property: JsonRequired] float InitialTimeScale,
	[property: JsonRequired] BattleClockFrame[] Updates)
{
	public const string CurrentSchema = "openpcr-battle-clock/v1";
	public static BattleClockInput Read(string path) => Parse(File.ReadAllText(path));
	public static BattleClockInput Parse(string json)
	{
		using var document = JsonDocument.Parse(json);
		JsonInputValidation.RejectDuplicateProperties(document.RootElement);
		var input = document.RootElement.Deserialize<BattleClockInput>(Options()) ?? throw new InvalidDataException("Clock input cannot be null.");
		_ = input.Prepare();
		return input;
	}
	public string ToJson() { _ = Prepare(); return JsonSerializer.Serialize(this, Options()); }
	public BattleUpdateClock Prepare()
	{
		if (Schema != CurrentSchema || InitialFrame is null || Updates is null || Updates.Any(frame => frame is null))
			throw new InvalidDataException("Unsupported clock schema or missing initial frame/updates.");
		return BattleUpdateClock.Timeline(InitialFrame.Prepare(), InitialTimeScale, Updates.Select(frame => frame.Prepare()));
	}
	internal static JsonSerializerOptions Options() => options;
	private static readonly JsonSerializerOptions options = new() { WriteIndented = true, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
}

public sealed record BattleClockValue([property: JsonRequired] long Count,
	[property: JsonRequired] uint Numerator, [property: JsonRequired] uint Denominator)
{
	internal EngineRationalTime Prepare() => new(Count, Numerator, Denominator);
}
public sealed record BattleClockFrame([property: JsonRequired] BattleClockValue Time,
	[property: JsonRequired] BattleClockValue DeltaTime, [property: JsonRequired] BattleClockValue UnscaledTime,
	[property: JsonRequired] BattleClockValue UnscaledDeltaTime, [property: JsonRequired] int FrameCount)
{
	internal EngineFrameTime Prepare()
	{
		if (Time is null || DeltaTime is null || UnscaledTime is null || UnscaledDeltaTime is null)
			throw new InvalidDataException("All four rational clock fields are required.");
		return new(Time.Prepare(), DeltaTime.Prepare(), UnscaledTime.Prepare(), UnscaledDeltaTime.Prepare(), FrameCount);
	}
}
