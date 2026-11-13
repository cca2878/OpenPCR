using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace OpenPCR.PrefabExtractor;

internal sealed record Column(string Name, string Kind);
internal sealed record Table(string Name, Column[] Columns, string[] PrimaryKey, string[] Scope, string[][] Indexes)
{
	public static readonly Dictionary<string, Table[]> Sources = Load();
	public static readonly Table Metadata = new("prefab_meta", [new Column("key", "t"), new Column("value", "t")], ["key"], [], []);
	/// <summary>The manifest bundles each extracted scope read: what <c>--update</c> compares with a newer manifest.</summary>
	public static readonly Table SourceBundles = new("source_bundle", [new Column("source", "t"), new Column("scope_key", "t"), new Column("bundle", "t"), new Column("md5", "t"), new Column("size", "i")], ["source", "scope_key", "bundle"], [], []);
	public static IEnumerable<Table> All => new[] { Metadata, SourceBundles }.Concat(Sources.Values.SelectMany(t => t));
	private static Dictionary<string, Table[]> Load()
	{
		using var input = typeof(Table).Assembly.GetManifestResourceStream("OpenPCR.PrefabExtractor.Tables.json")!;
		return JsonSerializer.Deserialize<Dictionary<string, Table[]>>(input, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
	}
	public static string Quote(string name) => "\"" + name.Replace("\"", "\"\"") + "\"";
	public string Ddl()
	{
		string ColumnSql(Column c)
		{
			string type = c.Kind switch { "i" or "i?" => "INTEGER", "t" => "TEXT", "f32" or "f64" or "blob" or "zjson" or "zproto" => "BLOB", _ => throw new InvalidDataException(c.Kind) };
			string sql = Quote(c.Name) + " " + type + (c.Kind == "i?" ? "" : " NOT NULL");
			if (c.Kind is "f32" or "f64") sql += " CHECK(length(" + Quote(c.Name) + ")=" + (c.Kind == "f32" ? 4 : 8) + ")";
			return sql;
		}
		return "CREATE TABLE IF NOT EXISTS " + Quote(Name) + "(" + string.Join(",", Columns.Select(ColumnSql)) + ",PRIMARY KEY(" + string.Join(",", PrimaryKey.Select(Quote)) + "));" +
			string.Concat(Indexes.Select((cols, i) => "CREATE INDEX IF NOT EXISTS " + Quote("ix_" + Name + "_" + i) + " ON " + Quote(Name) + "(" + string.Join(",", cols.Select(Quote)) + ");"));
	}
}

/// <summary>A zjson value already compressed by <see cref="Rows.Pack"/>.</summary>
internal sealed record PackedJson(byte[] Bytes);
/// <summary>A zproto value already compressed by <see cref="Rows.Pack"/>.</summary>
internal sealed record PackedProto(byte[] Bytes);

/// <summary>Column kind <c>zjson</c>: UTF-8 JSON text stored as a zlib stream (RFC 1950).</summary>
internal static class ZJson
{
	public static byte[] Pack(string json) => ZProto.Pack(System.Text.Encoding.UTF8.GetBytes(json));
	public static byte[] Unpack(byte[] packed) => ZProto.Unpack(packed);
}

/// <summary>Column kind <c>zproto</c>: a serialized protobuf message stored as a zlib stream (RFC 1950).</summary>
internal static class ZProto
{
	public static byte[] Pack(byte[] message)
	{
		using var output = new MemoryStream();
		using (var zlib = new System.IO.Compression.ZLibStream(output, System.IO.Compression.CompressionLevel.Optimal, leaveOpen: true)) zlib.Write(message);
		return output.ToArray();
	}
	public static byte[] Unpack(byte[] packed)
	{
		using var zlib = new System.IO.Compression.ZLibStream(new MemoryStream(packed), System.IO.Compression.CompressionMode.Decompress);
		using var output = new MemoryStream(); zlib.CopyTo(output); return output.ToArray();
	}
}

internal sealed class Rows : Dictionary<string, List<object?[]>>
{
	public Rows(string source) { foreach (var table in Table.Sources[source]) Add(table.Name, new()); }
	public void AddRow(string table, params object?[] values) => this[table].Add(values);
	/// <summary>Manifest bundles read while producing these rows.</summary>
	public List<BundleEntry> Bundles { get; } = new();
	/// <summary>Compress zjson text and zproto messages on the parsing thread, so the single writer stores packed bytes.</summary>
	public void Pack(string source)
	{
		foreach (var table in Table.Sources[source])
			for (int i = 0; i < table.Columns.Length; i++)
				if (table.Columns[i].Kind == "zjson")
				{ foreach (var row in this[table.Name]) if (row[i] is string text) row[i] = new PackedJson(ZJson.Pack(text)); }
				else if (table.Columns[i].Kind == "zproto")
				{ foreach (var row in this[table.Name]) if (row[i] is byte[] message) row[i] = new PackedProto(ZProto.Pack(message)); }
	}
}

internal sealed class Database : IDisposable
{
	public SqliteConnection Connection { get; }
	private readonly SqliteTransaction transaction;
	public Database(string path)
	{
		bool initialize = !File.Exists(path) || new FileInfo(path).Length == 0;
		Connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path, Pooling = false }.ToString());
		Connection.Open();
		transaction = Connection.BeginTransaction();
		try
		{
			foreach (var table in Table.All) Execute(table.Ddl());
			foreach (var (key, expected) in new[] { ("schema_version", "10"), ("unity_version", UnitPrefabBuilder.UnityVersion) })
			{
				using var command = Connection.CreateCommand(); command.Transaction = transaction;
				command.CommandText = "SELECT value FROM prefab_meta WHERE key=$key"; command.Parameters.AddWithValue("$key", key);
				object? actual = command.ExecuteScalar();
				if (actual == null && !initialize) throw new InvalidDataException("Missing existing prefab " + key);
				if (actual != null && !Equals(actual, expected)) throw new InvalidDataException("Unsupported existing prefab " + key + ": " + actual);
				if (actual != null) continue;
				command.CommandText = "INSERT INTO prefab_meta VALUES($key,$value)"; command.Parameters.AddWithValue("$value", expected); command.ExecuteNonQuery();
			}
		}
		catch { Dispose(); throw; }
	}
	private void Execute(string sql)
	{
		using var command = Connection.CreateCommand(); command.Transaction = transaction; command.CommandText = sql; command.ExecuteNonQuery();
	}
	public List<string> Keys(string table, string column)
	{
		using var command = Connection.CreateCommand(); command.Transaction = transaction;
		command.CommandText = "SELECT DISTINCT " + Table.Quote(column) + " FROM " + Table.Quote(table) + " ORDER BY " + Table.Quote(column);
		using var reader = command.ExecuteReader();
		var result = new List<string>(); while (reader.Read()) result.Add(Convert.ToString(reader.GetValue(0), System.Globalization.CultureInfo.InvariantCulture)!);
		return result;
	}
	public void Write(string source, string key, Rows rows)
	{
		// Validate the entire result before replacing any scoped rows. An empty successful scope still clears old data.
		foreach (var table in Table.Sources[source])
		{
			var identities = new HashSet<string>(StringComparer.Ordinal);
			foreach (var row in rows[table.Name])
			{
				if (row.Length != table.Columns.Length) throw new InvalidDataException(table.Name + ": row width mismatch");
				for (int i = 0; i < row.Length; i++)
				{
					if (table.Columns[i].Kind == "zjson" && row[i] is not (string or PackedJson)) throw new InvalidDataException(table.Name + "." + table.Columns[i].Name + ": zjson column requires JSON text");
					if (table.Columns[i].Kind == "zproto" && row[i] is not (byte[] or PackedProto)) throw new InvalidDataException(table.Name + "." + table.Columns[i].Name + ": zproto column requires a serialized message");
				}
				string identity = Tree.Json(table.PrimaryKey.Select(name => row[Array.FindIndex(table.Columns, c => c.Name == name)]).ToArray());
				if (!identities.Add(identity)) throw new InvalidDataException(table.Name + ": duplicate primary key " + identity);
			}
		}
		foreach (var table in Table.Sources[source])
		{
			if (table.Scope.Length != 0)
			{
				if (table.Scope.Length != 1) throw new InvalidDataException("Unsupported composite scope: " + table.Name);
				string scope = table.Scope[0] == "bundle" && source is "mode_spine" or "skill_spine" ? Sources.BundleNames(source, key, null).Single() : key;
				using var delete = Connection.CreateCommand(); delete.Transaction = transaction;
				delete.CommandText = "DELETE FROM " + Table.Quote(table.Name) + " WHERE " + Table.Quote(table.Scope[0]) + "=$scope";
				delete.Parameters.AddWithValue("$scope", scope); delete.ExecuteNonQuery();
			}
			using var insert = Connection.CreateCommand(); insert.Transaction = transaction;
			insert.CommandText = "INSERT OR REPLACE INTO " + Table.Quote(table.Name) + " VALUES(" + string.Join(",", table.Columns.Select((_, i) => "$p" + i)) + ")";
			foreach (var (column, i) in table.Columns.Select((c, i) => (c, i))) insert.Parameters.AddWithValue("$p" + i, DBNull.Value);
			foreach (var row in rows[table.Name])
			{
				for (int i = 0; i < row.Length; i++) insert.Parameters[i].Value = Pack(table.Columns[i].Kind, row[i]);
				insert.ExecuteNonQuery();
			}
		}
		using (var clear = Connection.CreateCommand())
		{
			clear.Transaction = transaction; clear.CommandText = "DELETE FROM source_bundle WHERE source=$source AND scope_key=$key";
			clear.Parameters.AddWithValue("$source", source); clear.Parameters.AddWithValue("$key", key); clear.ExecuteNonQuery();
		}
		using var record = Connection.CreateCommand(); record.Transaction = transaction;
		record.CommandText = "INSERT INTO source_bundle VALUES($source,$key,$bundle,$md5,$size)";
		record.Parameters.AddWithValue("$source", source); record.Parameters.AddWithValue("$key", key);
		var bundle = record.Parameters.Add("$bundle", SqliteType.Text); var md5 = record.Parameters.Add("$md5", SqliteType.Text); var size = record.Parameters.Add("$size", SqliteType.Integer);
		foreach (var entry in rows.Bundles) { bundle.Value = entry.Name; md5.Value = entry.Md5; size.Value = entry.Size; record.ExecuteNonQuery(); }
	}
	/// <summary>Recorded manifest bundles per scope key of one source.</summary>
	public Dictionary<string, List<BundleEntry>> SourceBundles(string source)
	{
		using var command = Connection.CreateCommand(); command.Transaction = transaction;
		command.CommandText = "SELECT scope_key,bundle,md5,size FROM source_bundle WHERE source=$source ORDER BY scope_key,bundle";
		command.Parameters.AddWithValue("$source", source);
		using var reader = command.ExecuteReader();
		var result = new Dictionary<string, List<BundleEntry>>(StringComparer.Ordinal);
		while (reader.Read())
		{
			string key = reader.GetString(0);
			if (!result.TryGetValue(key, out var list)) result[key] = list = new();
			list.Add(new BundleEntry(reader.GetString(1), "", "", reader.GetString(2), reader.GetInt64(3)));
		}
		return result;
	}
	private static object Pack(string kind, object? value)
	{
		if (value == null) return DBNull.Value;
		if (kind == "f32") { var bytes = new byte[4]; BinaryPrimitives.WriteSingleLittleEndian(bytes, Convert.ToSingle(value)); return bytes; }
		if (kind == "f64") { var bytes = new byte[8]; BinaryPrimitives.WriteDoubleLittleEndian(bytes, Convert.ToDouble(value)); return bytes; }
		if (kind == "zjson") return value is PackedJson packed ? packed.Bytes : ZJson.Pack(value as string ?? throw new InvalidDataException("zjson column requires JSON text"));
		if (kind == "zproto") return value is PackedProto message ? message.Bytes : ZProto.Pack(value as byte[] ?? throw new InvalidDataException("zproto column requires a serialized message"));
		return value is bool flag ? (flag ? 1 : 0) : value;
	}
	public void Commit() => transaction.Commit();
	public void RequireNonempty()
	{
		// Optional tables (for example authored tweeners) may be empty; these are required full-build inputs.
		string[] required = ["prefab_tree", "spine_bone", "spine_pose_setup", "spine_bounding_box", "spine_bounding_box_vertex", "firearm_prefab", "battle_manager_field", "battle_manager_transform", "battle_spine_transform", "cysp_animation", "cysp_pose", "cysp_event", "circle_effect_prefab", "circle_effect_child", "skill_effect_component", "skill_effect_particle", "count_down_effect", "count_down_end_effect", "skill_spine_animation", "skill_spine_data", "skill_spine_timeline", "mode_spine_animation", "mode_spine_pose", "mode_spine_setup", "mode_spine_data", "unit_spine_data"];
		foreach (string name in required)
		{
			using var command = Connection.CreateCommand(); command.Transaction = transaction; command.CommandText = "SELECT COUNT(*) FROM " + Table.Quote(name);
			long count = (long)command.ExecuteScalar()!; Console.WriteLine(name + ": " + count + " rows");
			if (count == 0) throw new InvalidDataException("Required full-build table is empty: " + name);
		}
	}
	public void Dispose() { transaction.Dispose(); Connection.Dispose(); }
}
