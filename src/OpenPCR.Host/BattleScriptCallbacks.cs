using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Elements.Battle;

namespace OpenPCR.Host;

/// <summary>Unity message isolation at the Host-to-script call, outside the game's own tick and coroutine loops.</summary>
internal static class BattleScriptCallbacks
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	internal static void Update(Action callback, Action<Exception> report)
	{
		try { callback(); }
		catch (Exception error) when (IsScriptException(error)) { report(error); }
	}

	// The Unity message is the external boundary. Runner/Step/UB-probe callers below it are not game
	// frames, and must not change a diagnostic when the same checkpoint is replayed through another API.
	internal static string ReadStackTrace(Exception error) => new StackTrace(new StackTrace(error, true).GetFrames()
		.TakeWhile(frame => frame.GetMethod()?.DeclaringType != typeof(BattleScriptCallbacks))).ToString();

	private static bool IsScriptException(Exception error)
	{
		// Missing implementations, deployment/binding failures and process/control failures are Host errors.
		if (error is OutOfBoundaryException or MigrationPendingException or NotSupportedException or NotImplementedException or
			IOException or UnauthorizedAccessException or MissingMemberException or TypeLoadException or
			OperationCanceledException or OutOfMemoryException or AccessViolationException)
			return false;
		if (error.InnerException is not null && !IsScriptException(error.InnerException)) return false;
		if (error is AggregateException aggregate)
			foreach (var inner in aggregate.InnerExceptions)
				if (!IsScriptException(inner)) return false;
		// Library throw helpers may precede the script. The first OpenPCR implementation frame must belong
		// to the mirrored game, not a resource/input/runtime adapter or a Host control signal (UbProbeStop).
		foreach (var frame in new StackTrace(error, false).GetFrames())
		{
			var assembly = frame.GetMethod()?.DeclaringType?.Assembly;
			if (assembly == typeof(BattleManager).Assembly) return true;
			if (assembly?.GetName().Name?.StartsWith("OpenPCR.", StringComparison.Ordinal) == true) return false;
		}
		return false;
	}
}
