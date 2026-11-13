using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenPCR.Host;

/// <summary>Opening AUTO/SET and ordered in-battle operations. Reserve IDs use original unit identities.</summary>
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record BattleExecutionInputs([property: JsonRequired] bool Auto, [property: JsonRequired] int[] ReserveUnitIds, [property: JsonRequired] PlayerOperation[] Operations)
{
	public static BattleExecutionInputs Manual() => new(false, Array.Empty<int>(), Array.Empty<PlayerOperation>());
	/// <summary>Parse an execution document; enum values may be names or numbers.</summary>
	public static BattleExecutionInputs Parse(string json)
	{
		using var document = JsonDocument.Parse(json);
		JsonInputValidation.RejectDuplicateProperties(document.RootElement);
		return document.RootElement.Deserialize<BattleExecutionInputs>(Options()) ?? throw new InvalidDataException("Execution inputs cannot be null.");
	}
	internal static JsonSerializerOptions Options() => options;
	private static readonly JsonSerializerOptions options = new()
	{
		WriteIndented = true,
		IncludeFields = true,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
		Converters = { new JsonStringEnumConverter() }
	};
	internal BattleExecutionInputs Copy()
	{
		ArgumentNullException.ThrowIfNull(ReserveUnitIds); ArgumentNullException.ThrowIfNull(Operations);
		return this with { ReserveUnitIds = (int[])ReserveUnitIds.Clone(), Operations = (PlayerOperation[])Operations.Clone() };
	}
}
