using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;

namespace OpenPCR.Growth;

/// <summary>Read-only, version-specific formula tables. No singleton, resource search or Python dependency.</summary>
internal sealed class GrowthMasterData
{
	private readonly SqliteConnection connection;
	private readonly Tables data;
	private sealed class Tables
	{
		internal readonly Dictionary<string, IReadOnlyList<Row>> Rows = new(StringComparer.Ordinal);
		internal readonly Dictionary<IndexKey, Dictionary<(int, int), Row>> Indexes = new();
		internal readonly Dictionary<string, Dictionary<string, Row>> OtherIndexes = new(StringComparer.Ordinal);
	}
	private readonly record struct IndexKey(string Table, string First, string? Second, bool Composite);
	private static readonly object sharedGate = new();
	private static string? sharedPath;
	private static Tables? sharedTables;

	internal GrowthMasterData(SqliteConnection connection)
	{
		this.connection = connection;
		data = SharedTables(connection);
	}
	private static Tables SharedTables(SqliteConnection connection)
	{
		// Deployed read-only resources remain fixed for the worker's lifetime; updating them requires a restart.
		// Writable/in-memory databases retain per-calculator caches.
		if (connection.State != System.Data.ConnectionState.Open ||
			new SqliteConnectionStringBuilder(connection.ConnectionString).Mode != SqliteOpenMode.ReadOnly)
			return new Tables();
		string source = connection.DataSource;
		if (source.Length == 0 || source == ":memory:") return new Tables();
		string path = Path.GetFullPath(source);
		lock (sharedGate)
		{
			// Bound retention to the latest database identity; active calculators can finish with their own snapshot.
			// No connection, account input, calculator or mutable UnitData is retained here.
			if (sharedPath != path) { sharedPath = path; sharedTables = new Tables(); }
			return sharedTables!;
		}
	}
	internal IReadOnlyList<Row> Rows(string table)
	{
		lock (data)
		{
			if (data.Rows.TryGetValue(table, out var rows)) return rows;
			using var command = connection.CreateCommand();
			command.CommandText = "SELECT * FROM \"" + table + "\"";
			using var reader = command.ExecuteReader();
			var columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToArray();
			var ordinals = new Dictionary<string, int>(columns.Length, StringComparer.Ordinal);
			for (int i = 0; i < columns.Length; i++) ordinals.Add(columns[i], i);
			var result = new List<Row>();
			while (reader.Read())
			{
				// Column names/ordinals belong to the table; rows retain only their immutable SQLite values, equal values sharing one box.
				var values = new object[columns.Length];
				for (int i = 0; i < columns.Length; i++) values[i] = SharedBoxes.Share(reader.GetValue(i))!;
				result.Add(new Row(table, ordinals, values));
			}
			return data.Rows[table] = result.AsReadOnly();
		}
	}
	internal Row? Find(string table, params (string Column, int Value)[] keys)
	{
		lock (data)
		{
			if (keys.Length is 1 or 2)
			{
				var key = new IndexKey(table, keys[0].Column, keys.Length == 2 ? keys[1].Column : null, keys.Length == 2);
				if (!data.Indexes.TryGetValue(key, out var index))
				{
					index = new Dictionary<(int, int), Row>();
					foreach (var row in Rows(table)) index.Add((row.Int(key.First), key.Composite ? row.Int(key.Second!) : 0), row);
					data.Indexes.Add(key, index);
				}
				return index.GetValueOrDefault((keys[0].Value, keys.Length == 2 ? keys[1].Value : 0));
			}
			string indexKey = table + ":" + string.Join(",", keys.Select(key => key.Column));
			if (!data.OtherIndexes.TryGetValue(indexKey, out var other))
			{
				other = new Dictionary<string, Row>(StringComparer.Ordinal);
				foreach (var row in Rows(table)) other.Add(string.Join(",", keys.Select(key => row.Int(key.Column).ToString(CultureInfo.InvariantCulture))), row);
				data.OtherIndexes.Add(indexKey, other);
			}
			return other.GetValueOrDefault(string.Join(",", keys.Select(key => key.Value.ToString(CultureInfo.InvariantCulture))));
		}
	}
	internal Row Require(string table, params (string Column, int Value)[] keys)
		=> Find(table, keys) ?? throw new InvalidDataException("Missing growth row: " + table + " " + string.Join(",", keys));
	internal sealed class Row
	{
		private readonly string table;
		private readonly Dictionary<string, int> columns;
		private readonly object[] values;
		internal Row(string table, Dictionary<string, int> columns, object[] values) { this.table = table; this.columns = columns; this.values = values; }
		internal int Int(string column) => checked((int)Long(column));
		internal long Long(string column) => Convert.ToInt64(Value(column), CultureInfo.InvariantCulture);
		internal float Float(string column) => Convert.ToSingle(Value(column), CultureInfo.InvariantCulture);
		internal double Double(string column) => Convert.ToDouble(Value(column), CultureInfo.InvariantCulture);
		private object Value(string column) => columns.TryGetValue(column, out int ordinal) && values[ordinal] is not DBNull
			? values[ordinal] : throw new InvalidDataException("Missing/null growth column: " + table + "." + column);
	}
}
