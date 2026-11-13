using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace OpenPCR.PrefabExtractor;

internal static class DatabaseComparison
{
	public static int Compare(string actualPath, string expectedPath)
	{
		using var actual = Open(actualPath); using var expected = Open(expectedPath);
		long checkedRows = 0, differences = 0;
		foreach (var table in Table.All)
		{
			using var a = actual.CreateCommand(); using var b = expected.CreateCommand();
			string sql = "SELECT " + string.Join(",", table.Columns.Select(c => Table.Quote(c.Name))) + " FROM " + Table.Quote(table.Name) + " ORDER BY " + string.Join(",", table.PrimaryKey.Select(Table.Quote));
			a.CommandText = sql; b.CommandText = sql; using var ar = a.ExecuteReader(); using var br = b.ExecuteReader();
			long rows = 0, failed = 0;
			bool hasA = ar.Read(), hasB = br.Read();
			int[] keys = table.PrimaryKey.Select(name => Array.FindIndex(table.Columns, c => c.Name == name)).ToArray();
			while (hasA || hasB)
			{
				rows++; int order = hasA ? (hasB ? 0 : -1) : 1;
				if (hasA && hasB)
					foreach (int key in keys)
					{
						object av = ar.GetValue(key), bv = br.GetValue(key);
						order = av is string astr && bv is string bstr ? string.CompareOrdinal(astr, bstr) : ((IComparable)av).CompareTo(bv);
						if (order != 0) break;
					}
				if (order != 0)
				{
					if (differences + failed < 10) Console.Error.WriteLine(table.Name + ": key missing from " + (order < 0 ? "reference" : "extraction"));
					failed++;
					if (order < 0) hasA = ar.Read(); else hasB = br.Read();
					continue;
				}
				for (int i = 0; i < table.Columns.Length; i++)
				{
					object av = ar.GetValue(i), bv = br.GetValue(i);
					bool equal = av is byte[] ab && bv is byte[] bb ? ab.AsSpan().SequenceEqual(bb) : Equals(av, bv);
					if (table.Columns[i].Kind == "zjson")
					{
						if (av is not byte[] apacked || bv is not byte[] bpacked) equal = false;
						else { using var aj = JsonDocument.Parse(ZJson.Unpack(apacked)); using var bj = JsonDocument.Parse(ZJson.Unpack(bpacked)); equal = JsonEqual(aj.RootElement, bj.RootElement); }
					}
					else if (table.Columns[i].Kind == "zproto")
						equal = av is byte[] amessage && bv is byte[] bmessage && ZProto.Unpack(amessage).AsSpan().SequenceEqual(ZProto.Unpack(bmessage));
					else if (table.Columns[i].Name is "data" or "properties" && av is string astr && bv is string bstr)
					{
						using var aj = JsonDocument.Parse(astr); using var bj = JsonDocument.Parse(bstr); equal = JsonEqual(aj.RootElement, bj.RootElement);
					}
					if (equal) continue;
					if (differences + failed < 10) Console.Error.WriteLine(table.Name + " row " + rows + " column " + table.Columns[i].Name + " differs");
					failed++; break;
				}
				hasA = ar.Read(); hasB = br.Read();
			}
			checkedRows += rows; differences += failed; Console.WriteLine(table.Name + ": " + rows + " rows, " + failed + " differences");
		}
		if (checkedRows == 0) throw new InvalidDataException("Comparison denominator is zero");
		Console.WriteLine("Compared " + checkedRows + " rows; differences=" + differences); return differences == 0 ? 0 : 1;
	}
	private static SqliteConnection Open(string path)
	{
		var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = path, Mode = SqliteOpenMode.ReadOnly, Pooling = false }.ToString()); connection.Open(); return connection;
	}
	internal static bool JsonEqual(JsonElement a, JsonElement b)
	{
		ValidateJson(a); ValidateJson(b);
		return EqualValue(a, b);
	}
	private static void ValidateJson(JsonElement value)
	{
		if (value.ValueKind == JsonValueKind.Object)
		{
			var names = new HashSet<string>(StringComparer.Ordinal);
			foreach (var property in value.EnumerateObject())
			{
				if (!names.Add(property.Name)) throw new InvalidDataException("Duplicate JSON property: " + property.Name);
				ValidateJson(property.Value);
			}
		}
		else if (value.ValueKind == JsonValueKind.Array)
			foreach (var item in value.EnumerateArray()) ValidateJson(item);
		else if (value.ValueKind == JsonValueKind.Number && !JsonNumber.Parse(value).IsInteger)
			FiniteDouble(value);
	}
	private static bool EqualValue(JsonElement a, JsonElement b)
	{
		if (a.ValueKind != b.ValueKind) return false;
		return a.ValueKind switch
		{
			JsonValueKind.Object => a.EnumerateObject().Count() == b.EnumerateObject().Count() && a.EnumerateObject().All(p => b.TryGetProperty(p.Name, out var value) && EqualValue(p.Value, value)),
			JsonValueKind.Array => a.GetArrayLength() == b.GetArrayLength() && a.EnumerateArray().Zip(b.EnumerateArray()).All(p => EqualValue(p.First, p.Second)),
			JsonValueKind.Number => EqualNumber(a, b),
			JsonValueKind.String => a.GetString() == b.GetString(),
			_ => a.GetRawText() == b.GetRawText()
		};
	}
	private static bool EqualNumber(JsonElement a, JsonElement b)
	{
		var left = JsonNumber.Parse(a); var right = JsonNumber.Parse(b);
		// Integer spellings must not lose identity through binary64 rounding. Other
		// numbers retain the binary64 contract used by the resource JSON writers.
		if (left.IsInteger || right.IsInteger) return left == right;
		return BitConverter.DoubleToInt64Bits(FiniteDouble(a)) == BitConverter.DoubleToInt64Bits(FiniteDouble(b));
	}
	private static double FiniteDouble(JsonElement value)
	{
		double number = value.GetDouble();
		if (!double.IsFinite(number) || number == 0)
			throw new InvalidDataException("JSON fraction is outside the nonzero finite double range: " + value.GetRawText());
		return number;
	}
	private readonly record struct JsonNumber(bool Negative, string Digits, BigInteger Exponent)
	{
		public bool IsInteger => Digits == "0" || Exponent.Sign >= 0;
		public static JsonNumber Parse(JsonElement value)
		{
			string text = value.GetRawText();
			bool negative = text[0] == '-';
			int start = negative ? 1 : 0, exponentAt = text.IndexOfAny(['e', 'E']);
			int end = exponentAt < 0 ? text.Length : exponentAt;
			int dot = text.IndexOf('.', start, end - start);
			BigInteger exponent = exponentAt < 0 ? BigInteger.Zero : BigInteger.Parse(text.AsSpan(exponentAt + 1), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
			if (dot >= 0) exponent -= end - dot - 1;
			string digits = text[start..end].Replace(".", "", StringComparison.Ordinal).TrimStart('0');
			if (digits.Length == 0) return new JsonNumber(negative, "0", BigInteger.Zero);
			int significant = digits.Length;
			while (digits[significant - 1] == '0') significant--;
			return new JsonNumber(negative, digits[..significant], exponent + digits.Length - significant);
		}
	}
}
