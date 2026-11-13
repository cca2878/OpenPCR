using System;
using System.Runtime.CompilerServices;

namespace OpenPCR;

public static class PreparedColliderInputs
{
	private sealed record Binding(Func<object, object, bool, bool, float, object> Create);
	private static readonly ConditionalWeakTable<object, Binding> inputs = new();
	public static void Bind(object gameObject, Func<object, object, bool, bool, float, object> create) => inputs.Add(gameObject, new(create));
	public static object Create(object box, object slot, object gameObject, bool trigger, bool kinematic, float gravity)
	{
		if (!inputs.TryGetValue(gameObject, out var input)) throw new InvalidOperationException("Skeletal collision resource has no prepared geometry producer.");
		return input.Create(box, slot, trigger, kinematic, gravity) ?? throw new InvalidOperationException("Skeletal geometry producer returned no collider.");
	}
	public static void Release(object gameObject) => inputs.Remove(gameObject);
}
