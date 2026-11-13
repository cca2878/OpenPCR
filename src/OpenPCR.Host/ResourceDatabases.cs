using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace OpenPCR.Host;

/// <summary>Explicit deployment root. Databases are supplied manually; no search or automatic refresh.</summary>
internal sealed class ResourceDatabases : IDisposable
{
	internal SqliteConnection Master { get; }
	internal SqliteConnection Prefab { get; }
	public string MasterVersion { get; }
	public string Root { get; }
	public PrefabInputs Prefabs { get; }
	public MasterInputs MasterData { get; }

	public ResourceDatabases(string resourceRoot, string masterVersion)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(resourceRoot);
		if (masterVersion is null || !Regex.IsMatch(masterVersion, @"\A[0-9]{12}\z"))
			throw new ArgumentException("Specify the twelve-digit masterdata deployment version.", nameof(masterVersion));
		Root = Path.GetFullPath(resourceRoot); MasterVersion = masterVersion;
		Master = Open(Path.Combine(Root, "masterdata", masterVersion + "_unhashed.db"));
		try
		{
			Prefab = Open(Path.Combine(Root, "prefab", "unit_prefab_native.sqlite3"));
			try
			{
				RequireMeta("schema_version", "10"); RequireMeta("unity_version", "6000.0.58f2");
				Prefabs = new PrefabInputs(Prefab); MasterData = new MasterInputs(Master);
			}
			catch { Prefab.Dispose(); throw; }
		}
		catch { Master.Dispose(); throw; }
	}

	internal static SqliteConnection Open(string path)
	{
		if (!File.Exists(path) || new FileInfo(path).Length == 0)
			throw new InvalidDataException("Resource database is missing or empty: " + path);
		var connection = new SqliteConnection(new SqliteConnectionStringBuilder
		{
			DataSource = path,
			Mode = SqliteOpenMode.ReadOnly,
			Pooling = false
		}.ToString());
		try { connection.Open(); return connection; }
		catch { connection.Dispose(); throw; }
	}
	private void RequireMeta(string key, string expected)
	{
		using var command = Prefab.CreateCommand();
		command.CommandText = "SELECT value FROM prefab_meta WHERE key=$key";
		command.Parameters.AddWithValue("$key", key);
		if (command.ExecuteScalar() is not string actual || actual != expected)
			throw new InvalidDataException("Unsupported prefab " + key + "; expected " + expected + ".");
	}
	public void Dispose() { Prefab.Dispose(); Master.Dispose(); }
}
