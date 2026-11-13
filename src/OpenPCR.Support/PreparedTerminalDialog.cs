using System;
using System.Runtime.CompilerServices;

namespace OpenPCR;

/// <summary>Explicit terminal presentation policy, after the battle caller has sealed gameplay.</summary>
public static class PreparedTerminalDialog
{
	private sealed class Session
	{
		internal readonly Action<Action> Complete;
		internal bool Closed;
		internal Session(Action<Action> complete) => Complete = complete;
	}
	private static readonly ConditionalWeakTable<object, Session> sessions = new();
	public static void Bind(object owner, Action<Action> complete)
	{
		ArgumentNullException.ThrowIfNull(owner);
		ArgumentNullException.ThrowIfNull(complete);
		sessions.Add(owner, new Session(complete));
	}
	public static void CloseTimeUp(object owner, Action callback)
	{
		ArgumentNullException.ThrowIfNull(callback);
		if (!sessions.TryGetValue(owner, out var session)) throw new InvalidOperationException("Terminal dialog completion is unbound.");
		if (session.Closed) throw new InvalidOperationException("Duplicate TIME_UP completion.");
		session.Closed = true;
		session.Complete(callback);
	}
	public static void Release(object owner) => sessions.Remove(owner);
}
