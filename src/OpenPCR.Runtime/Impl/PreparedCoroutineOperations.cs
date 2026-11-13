using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;
using System.Collections;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>Prepared frame owner's coroutine operations. No implicit scheduler or clock.</summary>
	public static class PreparedCoroutineOperations
	{
		private static Func<object, IEnumerator, object>? start;
		private static Action<object, object>? stop;
		private static Action<object>? stopAll;
		public static bool IsInitialized => start is not null;
		public static void Initialize(Func<object, IEnumerator, object> startRoutine, Action<object, object> stopRoutine, Action<object> stopOwner)
		{
			if (start is not null) throw new InvalidOperationException("Coroutine operations are already initialized.");
			if (startRoutine is null) throw new ArgumentNullException(nameof(startRoutine));
			if (stopRoutine is null) throw new ArgumentNullException(nameof(stopRoutine));
			if (stopOwner is null) throw new ArgumentNullException(nameof(stopOwner));
			start = startRoutine;
			stop = stopRoutine;
			stopAll = stopOwner;
		}
		public static object Start(object owner, IEnumerator routine)
		{
			ArgumentNullException.ThrowIfNull(routine);
			SceneScripting.GameObject(owner);
			return (start ?? throw new InvalidOperationException("No coroutine frame owner."))(owner, routine);
		}
		public static void Stop(object owner, object routine)
		{
			ArgumentNullException.ThrowIfNull(routine); SceneScripting.GameObject(owner);
			(stop ?? throw new InvalidOperationException("No coroutine frame owner."))(owner, routine);
		}
		public static void StopAll(object owner)
		{
			SceneScripting.GameObject(owner);
			(stopAll ?? throw new InvalidOperationException("No coroutine frame owner."))(owner);
		}
		public static void EndSession() { start = null; stop = null; stopAll = null; }
	}

}
