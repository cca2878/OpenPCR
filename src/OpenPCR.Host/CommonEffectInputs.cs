using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Elements;

namespace OpenPCR.Host;

/// <summary>Maps supported effect paths to the deployed prefab asset catalog.</summary>
internal sealed class CommonEffectInputs
{
	private readonly ResourceDatabases databases;
	private readonly Dictionary<int, string> paths = new();
	public CommonEffectInputs(ResourceDatabases databases)
	{
		this.databases = databases;
		using var stream = typeof(CommonEffectInputs).Assembly.GetManifestResourceStream("OpenPCR.Host.Inputs.common-effect-paths.json") ?? throw new InvalidDataException("Common effect path input is absent.");
		using var document = JsonDocument.Parse(stream);
		foreach (var property in document.RootElement.GetProperty("paths").EnumerateObject())
			paths.Add((int)Enum.Parse<eResourceId>(property.Name), property.Value.GetString()!);
	}
	public string? FieldName(int resourceId, long index)
	{
		if ((eResourceId)resourceId is not (eResourceId.HEAL_FIELD_EFFECT_170 or eResourceId.DEBUFF_FIELD_EFFECT_170 or eResourceId.FX_ENERGY_FIELD_MAGICCIRCLE or eResourceId.FX_ENERGY_DAMAGEFIELD_MAGICCIRCLE)) return null;
		if (!paths.TryGetValue(resourceId, out var format)) throw new InvalidDataException("Missing field resource path: " + (eResourceId)resourceId);
		string path = string.Format(CultureInfo.InvariantCulture, format, index);
		return path[(path.LastIndexOf('/') + 1)..];
	}
	public long? Resolve(int resourceId, long index, bool nothingToError)
	{
		// Indexed strike-back paths use inferred templates.
		if ((eResourceId)resourceId is eResourceId.FX_STRIKE_BACK or eResourceId.FX_STRIKE_BACK_LEFT)
		{
			if (index < 5 || index > 8) throw new NotSupportedException("Unprepared indexed strike-back identity: " + index);
			string name = "fx_strike_back_" + index.ToString(CultureInfo.InvariantCulture) + ((eResourceId)resourceId == eResourceId.FX_STRIKE_BACK_LEFT ? "_l" : "");
			using var lookup = databases.Prefab.CreateCommand();
			lookup.CommandText = "SELECT DISTINCT prefab_path_id FROM skill_effect_component WHERE bundle='a/all_effectprefab_3.unity3d' AND prefab_name=$name";
			lookup.Parameters.AddWithValue("$name", name);
			using var identities = lookup.ExecuteReader();
			if (!identities.Read()) throw new InvalidDataException("Missing indexed strike-back resource input: " + name);
			long identity = identities.GetInt64(0);
			if (identities.Read()) throw new InvalidDataException("Ambiguous strike-back resource input: " + name);
			return identity;
		}
		if (!paths.TryGetValue(resourceId, out var format)) throw new NotSupportedException("Unprepared resource path: " + (eResourceId)resourceId);
		string path = string.Format(CultureInfo.InvariantCulture, format, index);
		string? folder = Array.Find(path.Split('/'), value => value.EndsWith("_BUNDLE", StringComparison.Ordinal));
		if (folder is null)
		{
			// Field resources are supplied as authored components by the input catalog.
			// Positive exact identity is sufficient; this catalog cannot prove asset absence.
			string name = path[(path.LastIndexOf('/') + 1)..];
			if (!path.Split('/')[^2].EndsWith("_BUNDLENAMECUT", StringComparison.Ordinal) || !name.Contains("_CUT_", StringComparison.Ordinal))
				throw new InvalidDataException("Common resource has no prepared bundle identity: " + path);
			string fieldBundle = "a/all_" + name[..name.IndexOf("_CUT_", StringComparison.Ordinal)].ToLowerInvariant() + ".unity3d";
			using var field = databases.Prefab.CreateCommand();
			field.CommandText = "SELECT DISTINCT prefab_path_id FROM skill_effect_component WHERE bundle=$bundle AND prefab_name=$name";
			field.Parameters.AddWithValue("$bundle", fieldBundle);
			field.Parameters.AddWithValue("$name", name);
			using var rows = field.ExecuteReader();
			if (!rows.Read()) throw new InvalidDataException("Unextracted common field effect: " + path);
			long id = rows.GetInt64(0);
			if (rows.Read()) throw new InvalidDataException("Ambiguous common field effect: " + path);
			return id;
		}
		string bundle = "a/all_" + folder[..^7].ToLowerInvariant() + ".unity3d";
		using var coverage = databases.Prefab.CreateCommand();
		coverage.CommandText = "SELECT COUNT(*) FROM skill_effect_bundle WHERE bundle=$bundle";
		coverage.Parameters.AddWithValue("$bundle", bundle);
		if ((long)coverage.ExecuteScalar()! != 1) throw new InvalidDataException("Unextracted common effect bundle: " + bundle);
		using var command = databases.Prefab.CreateCommand();
		command.CommandText = "SELECT prefab_path_id FROM skill_effect_asset WHERE bundle=$bundle AND asset_path=$path";
		command.Parameters.AddWithValue("$bundle", bundle);
		command.Parameters.AddWithValue("$path", "assets/_elementsresources/resources/" + path.ToLowerInvariant() + ".prefab");
		object? result = command.ExecuteScalar();
		// Native returns null for an absent asset even when diagnostic reporting is enabled.
		// Catalog coverage above proves absence; the Core consumer retains its original null branch.
		return result is null ? null : (long)result;
	}
}
