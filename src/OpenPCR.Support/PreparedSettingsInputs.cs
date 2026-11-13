using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Supplied local settings. Missing keys never take a product/persistence default.</summary>
public static class PreparedSettingsInputs
{
	private sealed class Session
	{
		internal readonly Dictionary<string, object> Values;
		internal int Saves;
		internal Session(IReadOnlyDictionary<string, object> values) { Values = new(values, StringComparer.Ordinal); }
	}
	private static readonly ConditionalWeakTable<object, Session> Values = new();
	public static void Bind(object owner, IReadOnlyDictionary<string, bool> values)
	{
		ArgumentNullException.ThrowIfNull(values);
		var converted = new Dictionary<string, object>(StringComparer.Ordinal);
		foreach (var pair in values) converted.Add(pair.Key, pair.Value);
		Bind(owner, converted);
	}
	public static void Bind(object owner, IReadOnlyDictionary<string, object> values)
	{
		ArgumentNullException.ThrowIfNull(values);
		Values.Add(owner, new Session(values));
	}
	public static bool ReadBool(object owner, string key) => Read<bool>(owner, key);
	public static T Read<T>(object owner, string key) => Values.TryGetValue(owner, out var session) && session.Values.TryGetValue(key, out object? value) && value is T result
		? result : throw new InvalidOperationException("Missing prepared local setting or wrong type: " + key);
	public static void Write<T>(object owner, string key, T value) where T : notnull
	{
		_ = Read<T>(owner, key);
		Values.GetValue(owner, _ => throw new InvalidOperationException("Settings owner is unbound.")).Values[key] = value;
	}
	public static void Save(object owner)
	{
		if (!Values.TryGetValue(owner, out var session)) throw new InvalidOperationException("Local settings save output is unbound.");
		session.Saves++;
	}
	public static int SaveCount(object owner) => Values.TryGetValue(owner, out var session) ? session.Saves : throw new InvalidOperationException("Local settings owner is unbound.");
	public static void Release(object owner) => Values.Remove(owner);
}
