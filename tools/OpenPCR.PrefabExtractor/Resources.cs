using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace OpenPCR.PrefabExtractor;

internal sealed record BundleEntry(string Name, string Category, string PoolKey, string Md5, long Size);
internal sealed class Resources
{
	private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(60) };
	private readonly Dictionary<string, BundleEntry> entries = new(StringComparer.Ordinal);
	private readonly string cache;
	private readonly bool download;
	private readonly string? resourceRoot;
	public IEnumerable<string> Names => entries.Keys.Order(StringComparer.Ordinal);
	public Resources(string manifest, string cache, bool download, string? resourceRoot)
	{
		if (download && (!Uri.TryCreate(resourceRoot, UriKind.Absolute, out var uri) || uri.Scheme != "https")) throw new ArgumentException("Downloads require an explicit HTTPS resource root");
		this.cache = Path.GetFullPath(cache); this.download = download; this.resourceRoot = resourceRoot?.TrimEnd('/');
		using var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = manifest, Mode = SqliteOpenMode.ReadOnly, Pooling = false }.ToString());
		connection.Open();
		using var command = connection.CreateCommand();
		command.CommandText = "SELECT url,category,pool_key,md5,size FROM manifest_entries WHERE kind='asset' ORDER BY url";
		using var reader = command.ExecuteReader();
		while (reader.Read())
		{
			string url = reader.GetString(0);
			if (!url.StartsWith("a/", StringComparison.Ordinal)) continue;
			string name = url[2..];
			if (Path.GetFileName(name) != name || name.Contains('\\')) throw new InvalidDataException("Unsafe manifest bundle name: " + name);
			entries.Add(name, new BundleEntry(name, reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt64(4)));
		}
		if (entries.Count == 0) throw new InvalidDataException("Manifest has no asset entries");
	}
	public bool Contains(string name) => entries.ContainsKey(name);
	public BundleEntry? Entry(string name) => entries.GetValueOrDefault(name);
	private static readonly System.Threading.AsyncLocal<SortedDictionary<string, BundleEntry>?> reads = new();
	/// <summary>Record every bundle <see cref="Fetch"/> returns in this asynchronous flow until disposed.</summary>
	public static ReadScope Track() => new();
	internal sealed class ReadScope : IDisposable
	{
		private readonly SortedDictionary<string, BundleEntry>? previous = reads.Value;
		public SortedDictionary<string, BundleEntry> Bundles { get; } = new(StringComparer.Ordinal);
		public ReadScope() => reads.Value = Bundles;
		public void Dispose() => reads.Value = previous;
	}
	public byte[] Fetch(string name)
	{
		if (!entries.TryGetValue(name, out var entry)) throw new FileNotFoundException("Bundle absent from manifest: " + name);
		reads.Value?.TryAdd(name, entry);
		string path = Path.Combine(cache, name);
		byte[] bytes;
		if (File.Exists(path)) bytes = File.ReadAllBytes(path);
		else
		{
			if (!download) throw new FileNotFoundException("Bundle absent from offline cache: " + path);
			if (entry.PoolKey.Length < 2 || entry.PoolKey.Any(c => !Uri.IsHexDigit(c)) || entry.Category.Contains("..") || entry.Category.Contains('\\')) throw new InvalidDataException("Invalid manifest pool address");
			bytes = Client.GetByteArrayAsync(resourceRoot + "/pool/" + entry.Category + "/" + entry.PoolKey[..2] + "/" + entry.PoolKey).GetAwaiter().GetResult();
			Validate(entry, bytes);
			Directory.CreateDirectory(cache);
			string temporary = path + "." + Guid.NewGuid().ToString("N") + ".part";
			try { File.WriteAllBytes(temporary, bytes); File.Move(temporary, path, true); }
			finally { if (File.Exists(temporary)) File.Delete(temporary); }
		}
		Validate(entry, bytes);
		return bytes;
	}
	private static void Validate(BundleEntry entry, byte[] bytes)
	{
		if (bytes.LongLength != entry.Size || !Convert.ToHexStringLower(MD5.HashData(bytes)).Equals(entry.Md5, StringComparison.OrdinalIgnoreCase))
			throw new InvalidDataException(entry.Name + ": manifest size/MD5 mismatch");
	}
}
