using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;
using System.Text.Json.Serialization.Metadata;

namespace OpenPCR.Host;

/// <summary>
/// Schemas generated from reader records and serializer options. Server-shaped objects accept undeclared fields;
/// readers also check masterdata references and cross-field consistency.
/// </summary>
public static class InputSchemas
{
	/// <summary>File name → schema document.</summary>
	public static IReadOnlyDictionary<string, JsonNode> Export() => new SortedDictionary<string, JsonNode>(StringComparer.Ordinal)
	{
		["battle-request.schema.json"] = Schema<BattleRequest>(BattleRequest.JsonOptions(), BattleRequest.CurrentSchema,
			"Battle request: the player side, device settings, the battle sequence and exactly one mode section (ClanBattle or Labyrinth). "
			+ "Enemies, time limit and background are derived from the mode section and masterdata."),
		["battle-clock.schema.json"] = Schema<BattleClockInput>(BattleClockInput.Options(), BattleClockInput.CurrentSchema,
			"Optional explicit Unity update clock."),
	};

	private static JsonNode Schema<T>(JsonSerializerOptions options, string? schemaName, string description)
	{
		// Readers reject null where the records are non-nullable; export the same rule on a private copy of the shared options.
		options = new JsonSerializerOptions(options);
		options.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();
		options.RespectNullableAnnotations = true;
		var node = JsonSchemaExporter.GetJsonSchemaAsNode(options, typeof(T), new JsonSchemaExporterOptions { TreatNullObliviousAsNonNullable = true });
		var root = new JsonObject
		{
			["$schema"] = "https://json-schema.org/draft/2020-12/schema",
			["title"] = schemaName ?? typeof(T).Name,
			["description"] = description,
		};
		foreach (var (key, value) in node.AsObject().ToArray()) root[key] = value?.DeepClone();
		if (schemaName is not null) root["properties"]!["Schema"] = new JsonObject { ["const"] = schemaName };
		return root;
	}
}
