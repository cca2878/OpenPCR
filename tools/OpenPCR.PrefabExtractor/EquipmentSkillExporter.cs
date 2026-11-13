using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using AssetsTools.NET.Extra;
using Microsoft.Data.Sqlite;

namespace OpenPCR.PrefabExtractor;

internal static class EquipmentSkillExporter
{
	internal static int Run(string[] args)
	{
		var options = new Dictionary<string, string>(StringComparer.Ordinal); bool download = false;
		for (int i = 0; i < args.Length; i++)
		{
			if (args[i] == "--download") { if (download) throw new ArgumentException("Duplicate --download"); download = true; continue; }
			if (args[i] is not ("--manifest" or "--bundles" or "--output" or "--resource-root") || i + 1 == args.Length || !options.TryAdd(args[i], args[++i])) throw new ArgumentException("Invalid equipment-skills option.");
		}
		string Required(string key) => options.TryGetValue(key, out var value) ? value : throw new ArgumentException("Required: " + key);
		string output = Path.GetFullPath(Required("--output"));
		if (File.Exists(output)) throw new IOException("Equipment skill output already exists: " + output);
		string manifest = Path.GetFullPath(Required("--manifest"));
		var resources = new Resources(manifest, Required("--bundles"), download, options.GetValueOrDefault("--resource-root"));
		using var sql = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = manifest, Mode = SqliteOpenMode.ReadOnly }.ToString()); sql.Open();
		using var versionQuery = sql.CreateCommand(); versionQuery.CommandText = "SELECT ver FROM manifest_meta";
		string version;
		using (var versions = versionQuery.ExecuteReader())
		{
			if (!versions.Read()) throw new InvalidDataException("Manifest version is absent.");
			version = versions.GetString(0);
			if (versions.Read() || version.Length != 12 || version.Any(c => c is < '0' or > '9')) throw new InvalidDataException("Manifest requires one twelve-digit version.");
		}
		var assets = new List<object>();
		foreach (string name in resources.Names.Where(name => Regex.IsMatch(name, @"^all_equipment_skill_\d+\.unity3d$", RegexOptions.CultureInvariant)))
		{
			byte[] bytes = resources.Fetch(name);
			using var bundle = new UnityBundle(bytes, name);
			var entry = bundle.Container.Single(pair => pair.Value.Type == AssetClassID.MonoBehaviour && pair.Value.ClassName == "ExSkillScriptable");
			var tree = entry.Value.Tree;
			foreach (string field in new[] { "KnockAnimationCurve", "KnockDownAnimationCurve", "EffectType" })
				if (!tree.ContainsKey(field)) throw new InvalidDataException(name + ": missing " + field);
			assets.Add(new
			{
				skill_id = int.Parse(name[20..^8], System.Globalization.CultureInfo.InvariantCulture),
				bundle = "a/" + name,
				asset_path = entry.Key,
				source_sha256 = Convert.ToHexStringLower(SHA256.HashData(bytes)),
				data = tree
			});
		}
		if (assets.Count == 0) throw new InvalidDataException("Manifest contains no equipment skill assets.");
		Directory.CreateDirectory(Path.GetDirectoryName(output)!);
		string temporary = output + "." + Guid.NewGuid().ToString("N") + ".part";
		try
		{
			File.WriteAllText(temporary, JsonSerializer.Serialize(new
			{
				schema = "openpcr-equipment-skills/v1",
				unity_version = UnitPrefabBuilder.UnityVersion,
				manifest_version = version,
				manifest_sha256 = Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(manifest))),
				assets
			}, new JsonSerializerOptions { WriteIndented = true }));
			File.Move(temporary, output);
		}
		finally { if (File.Exists(temporary)) File.Delete(temporary); }
		Console.WriteLine($"Equipment skill assets: {assets.Count}; {output}");
		return 0;
	}
}
