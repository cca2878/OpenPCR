using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Explicit results of the ordered enemy-table lookup, established during preparation.</summary>
public static class PreparedEnemyLookupInputs
{
	private static readonly ConditionalWeakTable<object, Dictionary<int, Func<object?>>> results = new();
	public static void BindAbsent(object master, IEnumerable<int> ids)
	{
		ArgumentNullException.ThrowIfNull(master); ArgumentNullException.ThrowIfNull(ids);
		BindResults(master, ids.Distinct().ToDictionary(id => id, _ => (object?)null));
	}
	public static void BindResults(object master, IReadOnlyDictionary<int, object?> values)
	{
		ArgumentNullException.ThrowIfNull(master); ArgumentNullException.ThrowIfNull(values);
		BindFactories(master, values.ToDictionary(entry => entry.Key, entry => (Func<object?>)(() => entry.Value)));
	}
	public static void BindFactories(object master, IReadOnlyDictionary<int, Func<object?>> factories)
	{
		ArgumentNullException.ThrowIfNull(master); ArgumentNullException.ThrowIfNull(factories);
		if (factories.Values.Any(value => value is null)) throw new ArgumentException("Prepared lookup factories cannot be null.", nameof(factories));
		results.Add(master, factories.ToDictionary(entry => entry.Key, entry => entry.Value));
	}
	public static object? RequireResult(object master, int id)
	{
		if (!results.TryGetValue(master, out var values) || !values.TryGetValue(id, out var value))
			throw new InvalidOperationException("Enemy-table lookup has no prepared all-kind result: " + id);
		return value();
	}
	public static object? RequireAbsent(object master, int id)
	{
		if (RequireResult(master, id) is not null) throw new InvalidOperationException("Enemy-table result is not absent: " + id);
		return null;
	}
	public static void Release(object master) => results.Remove(master);
}
