using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Data.Sqlite;
using OpenPCR.Growth;

internal static class MasterDataCacheTests
{
	internal static int Run()
	{
		int assertions = 0;
		void Check(bool value, string message) { assertions++; if (!value) throw new Exception(message); }
		void Reject<T>(Action action) where T : Exception
		{
			assertions++;
			try { action(); } catch (T) { return; }
			throw new Exception("Expected " + typeof(T).Name);
		}
		SqliteConnection Open(string path, bool readOnly = true)
		{
			var connection = new SqliteConnection(new SqliteConnectionStringBuilder
			{
				DataSource = path,
				Mode = readOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWriteCreate,
				Pooling = false
			}.ToString());
			connection.Open(); return connection;
		}
		void Write(string path, int offset)
		{
			using var connection = Open(path, false);
			using var command = connection.CreateCommand();
			command.CommandText = "DROP TABLE IF EXISTS sample; CREATE TABLE sample (id INTEGER, other INTEGER, value REAL, nullable TEXT);" +
				"INSERT INTO sample VALUES (7,2,$first,NULL),(7,3,$second,'x'),(-1,5,9.5,'');";
			command.Parameters.AddWithValue("$first", 1.5 + offset); command.Parameters.AddWithValue("$second", 2.5 + offset);
			command.ExecuteNonQuery();
		}
		string folder = Path.Combine(Path.GetTempPath(), "openpcr-master-cache-" + Guid.NewGuid());
		Directory.CreateDirectory(folder);
		try
		{
			string path = Path.Combine(folder, "master.db"), other = Path.Combine(folder, "other.db");
			Write(path, 0); Write(other, 10);
			IReadOnlyList<GrowthMasterData.Row> initial;
			using (var first = Open(path))
			{
				var master = new GrowthMasterData(first);
				initial = master.Rows("sample");
				Check(initial.Select(row => row.Int("other")).SequenceEqual(new[] { 2, 3, 5 }), "Cached rows must retain SQLite enumeration order.");
				Check(master.Require("sample", ("id", 7), ("other", 2)).Double("value") == 1.5, "Composite keys must preserve both values.");
				Check(master.Require("sample", ("other", 3), ("id", 7)).Float("value") == 2.5f, "Reversed column order must identify its own index.");
				Check(master.Require("sample", ("other", 5)).Int("id") == -1, "Single-column numeric keys must preserve signed values.");
				Check(master.Find("sample", ("id", 7), ("other", 999)) is null, "A missing composite key must remain a miss.");
				Reject<InvalidDataException>(() => master.Require("sample", ("other", 999)));
				Reject<ArgumentException>(() => master.Find("sample", ("id", 7)));
				Reject<ArgumentException>(() => master.Find("sample", ("id", 7)));
				Reject<InvalidDataException>(() => master.Find("sample", ("missing", 1)));
				Reject<InvalidDataException>(() => initial[0].Int("nullable"));
				Reject<NotSupportedException>(() => ((IList<GrowthMasterData.Row>)initial)[0] = initial[1]);
				Check(master.Require("sample", ("id", 7), ("other", 2), ("value", 2)).Double("value") == 1.5, "Multi-column fallback must preserve existing numeric conversions.");
				Check(master.Find("sample", ("id", 7), ("other", 2), ("value", 3)) is null, "Multi-column fallback must preserve misses.");
			}
			using (var second = Open(path))
			{
				var master = new GrowthMasterData(second);
				Check(ReferenceEquals(initial, master.Rows("sample")), "A new calculator must reuse immutable rows after the original connection is disposed.");
				Check(master.Require("sample", ("other", 2)).Double("value") == 1.5, "A reused index must not retain its original connection.");
			}
			using (var different = Open(other))
			{
				var master = new GrowthMasterData(different);
				Check(master.Require("sample", ("other", 2)).Double("value") == 11.5, "Different resource identities must never share table values.");
				Check(!ReferenceEquals(initial, master.Rows("sample")), "A different database must receive a separate immutable snapshot.");
			}
			using (var revisited = Open(path))
				Check(!ReferenceEquals(initial, new GrowthMasterData(revisited).Rows("sample")), "The shared cache must retain only the most recent database identity.");
			string writablePath = Path.Combine(folder, "writable.db");
			Write(writablePath, 20);
			using (var writable = Open(writablePath, false))
			{
				var before = new GrowthMasterData(writable).Rows("sample");
				using var command = writable.CreateCommand(); command.CommandText = "UPDATE sample SET value=31.5 WHERE other=2"; command.ExecuteNonQuery();
				var master = new GrowthMasterData(writable);
				Check(!ReferenceEquals(before, master.Rows("sample")) && master.Require("sample", ("other", 2)).Double("value") == 31.5, "Writable connections must retain per-calculator isolation.");
			}
			using (var memory = Open(":memory:", false))
			{
				using var command = memory.CreateCommand();
				command.CommandText = "CREATE TABLE sample (id INTEGER); INSERT INTO sample VALUES (1)"; command.ExecuteNonQuery();
				var before = new GrowthMasterData(memory).Rows("sample");
				command.CommandText = "UPDATE sample SET id=2"; command.ExecuteNonQuery();
				var master = new GrowthMasterData(memory);
				Check(!ReferenceEquals(before, master.Rows("sample")) && master.Require("sample", ("id", 2)).Int("id") == 2, "In-memory connections must retain per-calculator isolation.");
			}
		}
		finally { Directory.Delete(folder, true); }
		return assertions;
	}
}
