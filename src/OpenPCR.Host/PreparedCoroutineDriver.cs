using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenPCR.Host;

/// <summary>Ordered Update/end-of-frame driver for explicitly prepared resource coroutines.</summary>
internal sealed class PreparedCoroutineDriver : IDisposable
{
	private sealed class Routine
	{
		internal readonly MonoBehaviour Owner;
		// A component never changes its GameObject; Start read it through the live-ownership check.
		internal readonly GameObject OwnerObject;
		internal readonly Coroutine Handle;
		internal readonly IEnumerator Iterator;
		// Unity runs a yielded IEnumerator as a nested coroutine; the parent resumes when it finishes.
		internal readonly Stack<IEnumerator> Parents = new();
		internal IEnumerator Current;
		internal object? Yield;
		internal int StartedFrame;
		internal float Remaining;
		internal bool Removed;
		internal Routine(MonoBehaviour owner, GameObject ownerObject, IEnumerator iterator, int frame)
		{
			Owner = owner; OwnerObject = ownerObject; Iterator = iterator; Current = iterator; StartedFrame = frame;
			Handle = (Coroutine)RuntimeHelpers.GetUninitializedObject(typeof(Coroutine));
		}
	}
	private readonly List<Routine> routines = new();
	private int frame;
	public PreparedCoroutineDriver() => PreparedCoroutineOperations.Initialize(Start, Stop, StopAll);
	private object Start(object owner, IEnumerator iterator)
	{
		var component = (MonoBehaviour)owner;
		// Inactive owners return null without advancing the supplied iterator.
		var gameObject = component.gameObject;
		if (!gameObject.activeInHierarchy) return null!;
		var routine = new Routine(component, gameObject, iterator, frame);
		routines.Add(routine);
		Advance(routine);
		return routine.Handle;
	}
	private void Stop(object owner, object token)
	{
		foreach (var routine in routines)
			if (ReferenceEquals(owner, routine.Owner) && (ReferenceEquals(token, routine.Handle) || ReferenceEquals(token, routine.Iterator))) routine.Removed = true;
	}
	private void StopAll(object owner)
	{
		foreach (var routine in routines) if (ReferenceEquals(owner, routine.Owner)) routine.Removed = true;
	}
	private void Advance(Routine routine)
	{
		while (!routine.Current.MoveNext())
		{
			if (routine.Parents.Count == 0) { routine.Removed = true; return; }
			routine.Current = routine.Parents.Pop();
		}
		routine.Yield = routine.Current.Current;
		if (routine.Yield is IEnumerator nested && routine.Yield is not CustomYieldInstruction)
		{
			routine.Parents.Push(routine.Current);
			routine.Current = nested;
			Advance(routine);
			return;
		}
		if (routine.Yield is WaitForSeconds seconds) routine.Remaining = (float)RuntimeBinding.Get(seconds, "m_Seconds")!;
		else if (routine.Yield is not null && routine.Yield is not CustomYieldInstruction && routine.Yield is not WaitForEndOfFrame)
			throw new NotSupportedException("Coroutine yield requires its Native scheduling producer: " + routine.Yield.GetType().FullName);
	}
	public void BeginFrame(int nextFrame)
	{
		if (nextFrame <= frame) throw new ArgumentOutOfRangeException(nameof(nextFrame));
		frame = nextFrame;
	}
	public void Update()
	{
		// Routines started during this pass are appended past the initial count and removals wait for RemoveAll, so
		// iterating that prefix preserves order and keeps them from receiving a second MoveNext.
		for (int index = 0, count = routines.Count; index < count; index++)
		{
			var routine = routines[index];
			if (routine.Removed) continue;
			if (!routine.Owner || !routine.OwnerObject.activeInHierarchy) { routine.Removed = true; continue; }
			if (routine.StartedFrame == frame || routine.Yield is WaitForEndOfFrame) continue;
			if (routine.Yield is WaitForSeconds)
			{
				routine.Remaining -= Time.deltaTime;
				if (routine.Remaining > 0) continue;
			}
			if (routine.Yield is CustomYieldInstruction condition && condition.keepWaiting) continue;
			Advance(routine);
		}
		routines.RemoveAll(routine => routine.Removed);
	}
	public void EndFrame()
	{
		for (int index = 0, count = routines.Count; index < count; index++)
		{
			var routine = routines[index];
			if (!routine.Removed && routine.Yield is WaitForEndOfFrame && routine.Owner && routine.OwnerObject.activeInHierarchy) Advance(routine);
		}
		routines.RemoveAll(routine => routine.Removed);
	}
	public void Dispose() { routines.Clear(); PreparedCoroutineOperations.EndSession(); }
}
