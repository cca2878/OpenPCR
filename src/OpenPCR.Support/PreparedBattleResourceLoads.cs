using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Explicit, already prepared unit resources. Load callbacks remain ordered at StartLoad.</summary>
public static class PreparedBattleResourceLoads
{
	private sealed class Session
	{
		internal readonly Func<int, bool, bool, object> Produce;
		internal readonly Func<int, long, bool, object?>? Resource;
		internal readonly Queue<Action> Pending = new();
		internal readonly Queue<Action> Completions = new();
		internal bool Flushing;
		internal Session(Func<int, bool, bool, object> produce, Func<int, long, bool, object?>? resource) { Produce = produce; Resource = resource; }
	}
	private static readonly ConditionalWeakTable<object, Session> Loaders = new();
	private static readonly ConditionalWeakTable<object, Session> Managers = new();
	public static void Bind(object loader, object manager, Func<int, bool, bool, object> producer, Func<int, long, bool, object?>? resources = null)
	{
		ArgumentNullException.ThrowIfNull(producer);
		if (Loaders.TryGetValue(loader, out _) || Managers.TryGetValue(manager, out _)) throw new InvalidOperationException("A prepared resource load session is already bound.");
		var session = new Session(producer, resources); Loaders.Add(loader, session); Managers.Add(manager, session);
	}
	public static object? ReadResource(object manager, int resourceId, long index, bool nothingToError)
	{
		if (!Managers.TryGetValue(manager, out var session) || session.Resource is null) throw new InvalidOperationException("Prepared resource catalog is missing.");
		return session.Resource(resourceId, index, nothingToError);
	}
	public static void AddResource(object manager, int resourceId, long index, Action<object?>? callback, bool required)
	{
		if (!Managers.TryGetValue(manager, out var session)) throw new InvalidOperationException("Prepared resource catalog is missing.");
		session.Pending.Enqueue(() => { var value = ReadResource(manager, resourceId, index, required); callback?.Invoke(value); });
	}
	/// <summary>RequestInstantiate: the prepared resource instance is produced and handed to the callback in request order.</summary>
	public static void AddInstantiate(object manager, int resourceId, long index, object? parent, Action<object>? callback)
	{
		if (!Managers.TryGetValue(manager, out var session)) throw new InvalidOperationException("Prepared resource catalog is missing.");
		session.Pending.Enqueue(() =>
		{
			var instance = PreparedResourceInputBinding.Instantiate(manager, resourceId, index, parent);
			callback?.Invoke(instance);
		});
	}
	public static void Add(object loader, int prefabId, Action<object>? callback, bool waveEnemy, bool instantiate)
	{
		if (!Loaders.TryGetValue(loader, out var session)) throw new InvalidOperationException("Prepared unit resource loader is missing.");
		session.Pending.Enqueue(() =>
		{
			var value = session.Produce(prefabId, waveEnemy, instantiate) ?? throw new InvalidOperationException("Prepared unit resource producer returned null.");
			callback?.Invoke(value);
		});
	}
	public static void Flush(object manager, Action? complete)
	{
		if (!Managers.TryGetValue(manager, out var session)) throw new InvalidOperationException("Prepared unit resource manager is missing.");
		if (complete is not null) session.Completions.Enqueue(complete);
		if (session.Flushing) return;
		session.Flushing = true;
		// Native loadCoroutine selects the first non-STARTLOAD request and runs
		// completion only after that set is empty, including callback-added work.
		try { while (session.Pending.TryDequeue(out var callback) || session.Completions.TryDequeue(out callback)) callback(); }
		finally { session.Flushing = false; }
	}
	public static void Release(object loader, object manager) { Loaders.Remove(loader); Managers.Remove(manager); }
}
