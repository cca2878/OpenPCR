using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace OpenPCR.PrefabExtractor;

internal static class ManifestBuilder
{
	internal sealed record Entry(string Url, string Md5, string PoolKey, string Type, long Size);
	internal static Entry? ParseLine(string line)
	{
		if (string.IsNullOrWhiteSpace(line)) return null;
		string[] p = line.Trim().Split(',');
		if (p.Length < 4) throw new InvalidDataException("Malformed manifest row");
		bool extended = p.Length > 5;
		string md5 = p[1], pool = extended ? p[2] : p[1];
		if (md5.Length != 32 || pool.Length < 2 || !System.Linq.Enumerable.All(md5 + pool, Uri.IsHexDigit)) throw new InvalidDataException("Malformed manifest hash");
		long size = long.Parse(p[extended ? 4 : 3], CultureInfo.InvariantCulture);
		if (size < 0) throw new InvalidDataException("Negative manifest size");
		if (p[0].Contains("..", StringComparison.Ordinal) || p[0].Contains('\\') || p[0].StartsWith('/')) throw new InvalidDataException("Unsafe manifest URL");
		return new Entry(p[0], md5, pool, p[extended ? 3 : 2], size);
	}
	public static async Task Build(string version, string output, string root, Func<string, Task<string>>? fetch = null)
	{
		if (!Uri.TryCreate(root, UriKind.Absolute, out var address) || address.Scheme != "https") throw new ArgumentException("Manifest downloads require an explicit HTTPS resource root");
		if (version.Length == 0 || !System.Linq.Enumerable.All(version, char.IsAsciiDigit)) throw new ArgumentException("Manifest version must contain digits only");
		output = Path.GetFullPath(output); Directory.CreateDirectory(Path.GetDirectoryName(output)!);
		string temporary = output + "." + Guid.NewGuid().ToString("N") + ".part";
		try
		{
			using (var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = temporary, Pooling = false }.ToString()))
			{
				connection.Open(); using var transaction = connection.BeginTransaction();
				using var command = connection.CreateCommand(); command.Transaction = transaction;
				command.CommandText = "CREATE TABLE manifest_entries(url TEXT PRIMARY KEY,md5 TEXT,pool_key TEXT,type TEXT,category TEXT,size INTEGER,parent_url TEXT,kind TEXT NOT NULL); CREATE TABLE manifest_meta(ver TEXT,fetched_at TEXT,manifest_nodes INTEGER,asset_leaves INTEGER,total_size INTEGER);";
				command.ExecuteNonQuery();
				const string category = "AssetBundles/Android", rootKey = "manifest/manifest_assetmanifest";
				void Insert(string url, string? md5, string? pool, string type, long size, string? parent, string kind)
				{
					command.Parameters.Clear();
					command.CommandText = "INSERT OR REPLACE INTO manifest_entries VALUES($url,$md5,$pool,$type,$category,$size,$parent,$kind)";
					foreach (var (name, value) in new (string, object?)[] { ("url", url), ("md5", md5), ("pool", pool), ("type", type), ("category", category), ("size", size), ("parent", parent), ("kind", kind) }) command.Parameters.AddWithValue("$" + name, value ?? DBNull.Value);
					command.ExecuteNonQuery();
				}
				Insert(rootKey, null, null, "every", 0, null, "manifest");
				using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
				fetch ??= client.GetStringAsync;
				var stack = new Stack<(string Url, bool Exit)>(); stack.Push((rootKey, false)); var active = new HashSet<string>(StringComparer.Ordinal); int fetched = 0;
				while (stack.Count != 0)
				{
					var (parent, exit) = stack.Pop();
					if (exit) { active.Remove(parent); continue; }
					if (!active.Add(parent)) throw new InvalidDataException("Manifest cycle: " + parent);
					stack.Push((parent, true));
					string content = await fetch(root.TrimEnd('/') + "/Manifest/" + category + "/" + version + "/" + parent); fetched++;
					foreach (string line in content.Split('\n'))
					{
						var row = ParseLine(line); if (row == null) continue;
						string kind = row.Url.StartsWith("manifest/", StringComparison.Ordinal) ? "manifest" : "asset";
						Insert(row.Url, row.Md5, row.PoolKey, row.Type, row.Size, parent, kind);
						if (kind == "manifest") stack.Push((row.Url, false));
					}
					if (fetched % 100 == 0) Console.WriteLine("Manifest nodes: " + fetched);
				}
				command.Parameters.Clear(); command.Parameters.AddWithValue("$ver", version);
				command.CommandText = "INSERT INTO manifest_meta SELECT $ver,datetime('now'),SUM(kind='manifest'),SUM(kind='asset'),SUM(CASE WHEN kind='asset' THEN size ELSE 0 END) FROM manifest_entries; SELECT COUNT(*) FROM manifest_entries WHERE kind='asset';";
				if (Convert.ToInt64(command.ExecuteScalar()) <= 0) throw new InvalidDataException("Manifest contains no asset leaves");
				transaction.Commit();
			}
			File.Move(temporary, output, true); Console.WriteLine("Saved manifest " + version + " to " + output);
		}
		finally { if (File.Exists(temporary)) File.Delete(temporary); }
	}
}
