using System;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Authored particle lifetime input, independent of particle rendering.</summary>
public static class PreparedEffectInputs
{
	private sealed record Input(float? Duration);
	private static readonly ConditionalWeakTable<object, Input> inputs = new();
	private static readonly ConditionalWeakTable<object, Func<bool>> rendererOnlyTweeners = new();
	/// <summary>Register a renderer-only tweener; check that it has no callbacks on binding and every use.</summary>
	public static void BindRendererOnlyTweener(object tweener, Func<bool> callbackFree)
	{
		ArgumentNullException.ThrowIfNull(tweener); ArgumentNullException.ThrowIfNull(callbackFree);
		if (!callbackFree()) throw new InvalidOperationException("Effect tweener has completion callbacks.");
		rendererOnlyTweeners.Add(tweener, callbackFree);
	}
	public static bool IsRendererOnlyTweener(object? tweener)
	{
		if (tweener is null || !rendererOnlyTweeners.TryGetValue(tweener, out var callbackFree)) return false;
		if (!callbackFree()) throw new InvalidOperationException("Prepared effect tweener acquired completion callbacks; renderer projection is invalid.");
		return true;
	}
	public static void Bind(object effect, float? duration)
	{
		ArgumentNullException.ThrowIfNull(effect);
		if (duration is float value && (!float.IsFinite(value) || value < 0)) throw new ArgumentOutOfRangeException(nameof(duration));
		inputs.Add(effect, new(duration));
	}
	public static float ParticleDuration(object effect) =>
		inputs.TryGetValue(effect, out var input) && input.Duration is float value ? value : throw new InvalidOperationException("Effect has no authored particle duration input.");
	public static bool HasParticle(object effect) => inputs.TryGetValue(effect, out var input) ? input.Duration.HasValue : throw new InvalidOperationException("Effect has no authored particle input.");
	public static void Release(object effect) => inputs.Remove(effect);
}
