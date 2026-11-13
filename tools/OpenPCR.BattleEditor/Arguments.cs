using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace OpenPCR.BattleEditor;

internal sealed class Arguments
{
	private readonly Dictionary<string, string> values = new(StringComparer.Ordinal);
	internal Arguments(string[] args)
	{
		for (int i = 0; i < args.Length; i += 2)
		{
			if (!args[i].StartsWith("--", StringComparison.Ordinal) || i + 1 == args.Length || args[i + 1].StartsWith("--", StringComparison.Ordinal))
				throw new ArgumentException("Expected --option value: " + args[i]);
			if (!values.TryAdd(args[i], args[i + 1])) throw new ArgumentException("Duplicate option: " + args[i]);
		}
	}
	internal string? Take(string key) => values.Remove("--" + key, out var value) ? value : null;
	internal string Require(string key) => Take(key) ?? throw new ArgumentException("Required: --" + key);
	internal void RequirePresent(params string[] keys)
	{
		foreach (string key in keys) if (!values.ContainsKey("--" + key)) throw new ArgumentException("Required: --" + key);
	}
	internal int? Integer(string key) => Take(key) is { } value ? int.Parse(value, CultureInfo.InvariantCulture) : null;
	internal long? Long(string key) => Take(key) is { } value ? long.Parse(value, CultureInfo.InvariantCulture) : null;
	internal float? Single(string key) => Take(key) is { } value ? float.Parse(value, CultureInfo.InvariantCulture) : null;
	internal bool? Boolean(string key) => Take(key) is { } value ? bool.Parse(value) : null;
	internal void Complete()
	{
		if (values.Count > 0) throw new ArgumentException("Unknown or inapplicable option: " + string.Join(", ", values.Keys));
	}
	internal static int Positive(int value, string name)
	{
		if (value <= 0) throw new InvalidDataException(name + " must be positive.");
		return value;
	}
}
