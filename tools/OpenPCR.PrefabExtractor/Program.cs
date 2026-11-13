using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenPCR.PrefabExtractor;

internal static class Program
{
	public static async Task<int> Main(string[] args)
	{
		string? temporary = null;
		try
		{
			if (args.Length > 0 && args[0] == "equipment-skills") return EquipmentSkillExporter.Run(args[1..]);
			var options = Options.Parse(args);
			if (options.Help) { Console.WriteLine(Options.Usage); return 0; }
			if (options.List) { foreach (var (source, tables) in Table.Sources) Console.WriteLine(source + ": " + string.Join(", ", tables.Select(t => t.Name))); return 0; }
			if (options.BuildManifest != null) { await ManifestBuilder.Build(options.BuildManifest, options.Output!, options.ResourceRoot!); return 0; }
			if (options.Compare != null) return DatabaseComparison.Compare(options.Output!, options.Compare);
			var resources = new Resources(options.Manifest!, options.Bundles!, options.Download, options.ResourceRoot);
			string output = Path.GetFullPath(options.Output!); Directory.CreateDirectory(Path.GetDirectoryName(output)!);
			if (Path.GetFullPath(options.Manifest!) == output) throw new ArgumentException("Output cannot overwrite the manifest");
			temporary = output + "." + Guid.NewGuid().ToString("N") + ".part";
			if (options.Update && !File.Exists(output)) throw new ArgumentException("--update requires an existing output database");
			if (File.Exists(output) && (!options.All || options.Merge))
			{
				using var original = new Microsoft.Data.Sqlite.SqliteConnection(new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder { DataSource = output, Mode = Microsoft.Data.Sqlite.SqliteOpenMode.ReadOnly, Pooling = false }.ToString());
				using var staging = new Microsoft.Data.Sqlite.SqliteConnection(new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder { DataSource = temporary, Pooling = false }.ToString());
				original.Open(); staging.Open(); original.BackupDatabase(staging);
			}
			using (var database = new Database(temporary))
			{
				foreach (string source in Sources.Names.Where(n => options.All || options.Update && options.Sources.Count == 0 || options.Sources.Contains(n)))
				{
					var keys = (options.Keys.Count != 0 ? options.Keys : Sources.Keys(source, resources, database).ToList());
					if (keys.Count == 0) throw new InvalidDataException(source + ": no keys selected");
					if (keys.Distinct(StringComparer.Ordinal).Count() != keys.Count) throw new ArgumentException(source + ": duplicate keys");
					if (options.Merge)
					{
						var table = Table.Sources[source][0];
						if (table.Scope.Length == 0) throw new ArgumentException("--merge unsupported for " + source);
						var have = database.Keys(table.Name, table.Scope[0]).ToHashSet(StringComparer.Ordinal); keys = keys.Where(k => !have.Contains(k)).ToList();
					}
					if (options.Update)
					{
						var plan = Update.For(source, keys, database.SourceBundles(source), resources);
						foreach (string key in plan.Removed) database.Write(source, key, new Rows(source));
						Console.WriteLine(source + ": " + plan.Changed.Count + " changed, " + plan.Removed.Count + " removed, " + (keys.Count - plan.Changed.Count) + " unchanged");
						keys = plan.Changed;
						if (keys.Count == 0) continue;
					}
					else Console.WriteLine(source + ": " + keys.Count + " keys");
					await Extract(source, keys, resources, database, options.Jobs);
				}
				if (options.All) database.RequireNonempty();
				database.Commit();
			}
			File.Move(temporary, output, true); temporary = null; Console.WriteLine("Saved " + output); return 0;
		}
		catch (Exception ex) { Console.Error.WriteLine(ex.Message); return 1; }
		finally { if (temporary != null && File.Exists(temporary)) File.Delete(temporary); }
	}

	/// <summary>
	/// At most <paramref name="jobs"/> parses run at once inside a window of <paramref name="jobs"/> * 4 started keys. The
	/// writer consumes results in key order as each becomes ready, so a slow key does not idle the other workers and writes
	/// overlap parsing. Each worker owns its AssetsManager; key-order writes keep shared projectile identities stable.
	/// </summary>
	private static async Task Extract(string source, List<string> keys, Resources resources, Database database, int jobs)
	{
		using var slots = new System.Threading.SemaphoreSlim(jobs);
		var pending = new Task<Rows>[keys.Count];
		Task<Rows> Start(string key) => Task.Run(async () =>
		{
			await slots.WaitAsync();
			try
			{
				using var reads = Resources.Track();
				var rows = Sources.Parse(source, key, resources);
				if (reads.Bundles.Count == 0) throw new InvalidDataException("No manifest bundle was read");
				rows.Bundles.AddRange(reads.Bundles.Values);
				rows.Pack(source);
				return rows;
			}
			catch (Exception ex) { throw new InvalidDataException(source + "/" + key + ": " + ex.Message, ex); }
			finally { slots.Release(); }
		});
		int started = 0;
		for (; started < Math.Min(jobs * 4, keys.Count); started++) pending[started] = Start(keys[started]);
		for (int i = 0; i < keys.Count; i++)
		{
			var rows = await pending[i]; pending[i] = null!;
			if (started < keys.Count) { pending[started] = Start(keys[started]); started++; }
			database.Write(source, keys[i], rows);
			int done = i + 1;
			if (done % (jobs * 25) == 0 || done == keys.Count) Console.WriteLine("  " + done + "/" + keys.Count);
		}
	}
}

internal sealed class Options
{
	public const string Usage = """
		OpenPCR.PrefabExtractor --manifest <manifest_VERSION.db> --bundles <version-cache-directory>
		  --output <unit_prefab_native.sqlite3> (--all | --only <source> [--only <source> ...])
		  [--keys <key> ...] [--jobs <1..64>] [--merge] [--download --resource-root <https-url>]
		OpenPCR.PrefabExtractor --manifest <manifest_VERSION.db> --bundles <version-cache-directory>
		  --output <existing unit_prefab_native.sqlite3> --update [--only <source> ...] [--jobs <1..64>] [--download ...]
		OpenPCR.PrefabExtractor --list | --help
		OpenPCR.PrefabExtractor --build-manifest <version> --output <manifest_VERSION.db> --resource-root <https-url>
		OpenPCR.PrefabExtractor --compare <reference.sqlite3> --output <extracted.sqlite3>

		Offline by default. An explicit manifest selects the resource version. --all rebuilds from
		scratch; partial runs copy existing outputs and replace selected scopes in a temporary
		database. Only a successful run publishes it.
		--keys requires one --only. --merge skips existing primary scopes without refreshing them.
		--update compares the bundles each scope recorded (source_bundle) with the manifest, re-extracts
		changed or new scopes and removes scopes the manifest no longer produces; --only limits sources.
		--download and --build-manifest require an explicit HTTPS --resource-root. No server is preset.
		""";
	public string? Manifest, Bundles, Output;
	public string? BuildManifest;
	public string? Compare;
	public string? ResourceRoot;
	public bool All, List, Help, Download, Merge, Update;
	public int Jobs = 4;
	public List<string> Sources { get; } = new();
	public List<string> Keys { get; } = new();
	public static Options Parse(string[] args)
	{
		var result = new Options();
		for (int i = 0; i < args.Length; i++)
		{
			string Value() => ++i < args.Length && !args[i].StartsWith("--", StringComparison.Ordinal) ? args[i] : throw new ArgumentException("Missing value for " + args[i - 1]);
			switch (args[i])
			{
				case "--manifest": result.Manifest = Value(); break;
				case "--bundles": result.Bundles = Value(); break;
				case "--output": result.Output = Value(); break;
				case "--all": result.All = true; break;
				case "--list": result.List = true; break;
				case "--help": result.Help = true; break;
				case "--download": result.Download = true; break;
				case "--merge": result.Merge = true; break;
				case "--update": result.Update = true; break;
				case "--build-manifest": result.BuildManifest = Value(); break;
				case "--compare": result.Compare = Value(); break;
				case "--jobs": result.Jobs = int.Parse(Value(), System.Globalization.CultureInfo.InvariantCulture); break;
				case "--resource-root": result.ResourceRoot = Value(); break;
				case "--only": result.Sources.Add(Value()); break;
				case "--keys":
					result.Keys.Add(Value()); while (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal)) result.Keys.Add(args[++i]); break;
				default: throw new ArgumentException("Unknown option: " + args[i]);
			}
		}
		if (result.List || result.Help) return result;
		if (result.Compare != null)
		{
			if (result.Output == null || result.BuildManifest != null || result.All || result.Sources.Count != 0 || result.Manifest != null || result.Bundles != null || result.Keys.Count != 0 || result.Merge || result.Update || result.Download) throw new ArgumentException("--compare requires --output and cannot be combined with extraction");
			return result;
		}
		if (result.BuildManifest != null)
		{
			if (result.Output == null || result.All || result.Sources.Count != 0 || result.Keys.Count != 0 || result.Merge || result.Update || result.Manifest != null || result.Bundles != null) throw new ArgumentException("--build-manifest requires --output and cannot be combined with extraction");
			RequireRoot(result.ResourceRoot);
			return result;
		}
		if (result.Manifest == null || result.Bundles == null || result.Output == null) throw new ArgumentException(Usage);
		if (result.Update && (result.All || result.Merge || result.Keys.Count != 0)) throw new ArgumentException("--update cannot be combined with --all, --merge or --keys");
		if (!result.Update && result.All == (result.Sources.Count != 0)) throw new ArgumentException("Choose --all or --only");
		if (result.Sources.Any(s => !Table.Sources.ContainsKey(s))) throw new ArgumentException("Unknown source");
		if (result.Sources.Distinct(StringComparer.Ordinal).Count() != result.Sources.Count) throw new ArgumentException("Duplicate --only source");
		if (result.Keys.Count != 0 && (result.All || result.Sources.Count != 1)) throw new ArgumentException("--keys requires exactly one --only");
		if (result.Jobs is < 1 or > 64) throw new ArgumentException("--jobs must be in 1..64");
		if (result.Download || result.ResourceRoot != null) RequireRoot(result.ResourceRoot);
		return result;
	}
	private static void RequireRoot(string? root)
	{
		if (!Uri.TryCreate(root, UriKind.Absolute, out var uri) || uri.Scheme != "https") throw new ArgumentException("An explicit HTTPS --resource-root is required for network operations");
	}
}
