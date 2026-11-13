using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Explicit prepared-resource instantiation. Identity belongs to the scene producer, not a shadow prefab.</summary>
public static class PreparedResourceInputBinding
{
	private sealed class Binding
	{
		internal readonly Func<int, long, object?, object> Create;
		internal readonly Func<int, long, bool>? BundleExists;
		internal readonly HashSet<object> Instances = new(ReferenceEqualityComparer.Instance);
		internal Binding(Func<int, long, object?, object> create, Func<int, long, bool>? bundleExists) { Create = create; BundleExists = bundleExists; }
	}
	private static readonly ConditionalWeakTable<object, Binding> bindings = new();
	public static void Bind(object manager, Func<int, long, object?, object> create, Func<int, long, bool>? bundleExists = null)
	{
		ArgumentNullException.ThrowIfNull(manager); ArgumentNullException.ThrowIfNull(create);
		bindings.Add(manager, new Binding(create, bundleExists));
	}
	public static bool HasBundle(object manager, int bundleId, long index) => bindings.TryGetValue(manager, out var binding) && binding.BundleExists is not null
		? binding.BundleExists(bundleId, index) : throw new InvalidOperationException("Bundle lookup requires an explicit prepared catalog.");
	public static object Instantiate(object manager, int resourceId, long index, object? parent)
	{
		if (!bindings.TryGetValue(manager, out var binding)) throw new InvalidOperationException("Resource instantiation requires an explicit prepared producer.");
		lock (binding)
		{
			var result = binding.Create(resourceId, index, parent) ?? throw new InvalidOperationException("Prepared resource producer returned no instance.");
			if (!binding.Instances.Add(result)) throw new InvalidOperationException("Resource instantiation must produce a fresh instance.");
			return result;
		}
	}
	public static void Release(object manager) => bindings.Remove(manager);
}
