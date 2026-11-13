using System;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Session-owned product factories; runtime callbacks consume detached products only.</summary>
public static class PreparedParameters
{
	private sealed record Binding(Func<object, int, int, bool, bool, object> Summon, Func<int, object> Enemy, Func<object, bool, object>? Limited);
	private static readonly ConditionalWeakTable<object, Binding> bindings = new();
	public static void Bind(object manager, Func<object, int, int, bool, bool, object> summon, Func<int, object> enemy, Func<object, bool, object>? limited = null)
	{
		ArgumentNullException.ThrowIfNull(manager); ArgumentNullException.ThrowIfNull(summon); ArgumentNullException.ThrowIfNull(enemy);
		bindings.Add(manager, new(summon, enemy, limited));
	}
	private static Binding Require(object manager) => bindings.TryGetValue(manager, out var binding) ? binding :
		throw new InvalidOperationException("Missing prepared parameter products or templates.");
	public static object Summon(object manager, object owner, int id, int level, bool equipment, bool evolved) =>
		Require(manager).Summon(owner, id, level, equipment, evolved) ?? throw new InvalidOperationException("Prepared summon producer returned null.");
	/// <summary>Prepared UpdateLimitedUnitData result for this unique input; a fresh copy each call.</summary>
	public static object Limited(object manager, object unique, bool isSupportOrReplay) =>
		(Require(manager).Limited ?? throw new InvalidOperationException("Missing prepared Limited unit products."))(unique, isSupportOrReplay)
		?? throw new InvalidOperationException("Prepared Limited producer returned null.");
	public static object Enemy(object manager, int id) => Require(manager).Enemy(id) ?? throw new InvalidOperationException("Prepared enemy producer returned null.");
	public static void Release(object manager) => bindings.Remove(manager);
}
