using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Elements;
using Elements.Battle;
using OpenPCR;
using OpenPCR.BattleInputs;
using OpenPCR.Checkpoint;
using OpenPCR.Host;

internal static class ScriptCallbackTests
{
	internal static void Run(Action<bool, string> check)
	{
		var manager = new CoroutineManager();
		manager.Init(new List<UnitCtrl>());
		int sentinel = 0, entered = 0, returned = 0, outside = 0, accumulator = 10;
		var errors = new List<Exception>();
		IEnumerator Earlier() { while (true) { sentinel++; yield return null; } }
		IEnumerator Disappear() => (IEnumerator)RuntimeBinding.Call(new ChangeColorEffect(), "updateDisappear")!;
		manager.AppendCoroutine(Earlier(), ePauseType.IGNORE_BLACK_OUT);
		manager.AppendCoroutine(Disappear(), ePauseType.IGNORE_BLACK_OUT);
		manager.AppendCoroutine(Disappear(), ePauseType.IGNORE_BLACK_OUT);
		void Update() { entered++; manager._Update(); returned++; accumulator--; }
		void Frame() { BattleScriptCallbacks.Update(Update, errors.Add); outside++; }
		Frame();
		check(errors.Count == 0 && sentinel == 1 && returned == 1, "Initial coroutine yields changed.");
		Frame();
		check(errors.Count == 1 && sentinel == 1 && returned == 1 && accumulator == 9 && outside == 2,
			"Script exception resumed the game tick, advanced earlier coroutines, or aborted the outer frame.");
		Frame();
		check(errors.Count == 2 && sentinel == 1 && returned == 1 && outside == 3,
			"A second script failure was lost, or the failed iterator was restarted.");
		Frame();
		check(entered == 4 && returned == 2 && accumulator == 8 && outside == 4 && sentinel == 2 &&
			errors.TrueForAll(error => error is NullReferenceException),
			"Normal traversal did not retire the failed iterators and continue on the next Update.");

		void Reject(Action action, Type type)
		{
			Exception? caught = null;
			try { BattleScriptCallbacks.Update(action, errors.Add); } catch (Exception error) { caught = error; }
			check(caught?.GetType() == type && errors.Count == 2, "Host/control error was recorded as a script exception: " + type.Name);
		}
		var uninitialized = (BattleManager)RuntimeHelpers.GetUninitializedObject(typeof(BattleManager));
		Reject(uninitialized.StartCoroutineBossLose, typeof(OutOfBoundaryException));
		Reject(() => throw new MigrationPendingException("fixture", "missing"), typeof(MigrationPendingException));
		Reject(() => RuntimeBinding.Get(manager, "missing_member"), typeof(MissingMemberException));
		Reject(() => ThrowAdapterFailure(manager), typeof(NullReferenceException));
		Reject(() => throw new OperationCanceledException(), typeof(OperationCanceledException));
		var stopType = typeof(BattleSession).GetNestedType("UbProbeStop", BindingFlags.NonPublic)!;
		var stop = (Exception)Activator.CreateInstance(stopType, 136001)!;
		Reject(() => throw stop, stopType);
		int notifications = 0;
		Reject(() => BattleScriptCallbacks.Update(() =>
		{
			var iterator = Disappear(); iterator.MoveNext(); iterator.MoveNext();
		}, _ => { notifications++; throw new InvalidOperationException("diagnostic receiver failed"); }), typeof(InvalidOperationException));
		check(notifications == 1, "Diagnostic receiver failed silently or was called repeatedly.");
		Action fail = () => { var iterator = Disappear(); iterator.MoveNext(); iterator.MoveNext(); };
		string? firstStack = null, secondStack = null;
		void FirstCaller() => BattleScriptCallbacks.Update(fail, error => firstStack = BattleScriptCallbacks.ReadStackTrace(error));
		void SecondCaller() => BattleScriptCallbacks.Update(fail, error => secondStack = BattleScriptCallbacks.ReadStackTrace(error));
		FirstCaller(); SecondCaller();
		check(firstStack == secondStack && firstStack!.Contains("ChangeColorEffect", StringComparison.Ordinal) &&
			!firstStack.Contains("FirstCaller", StringComparison.Ordinal), "Host caller leaked into the script stack or changed checkpoint diagnostics.");

		var diagnostics = new List<BattleScriptException>();
		using var context = new CheckpointContext();
		context.AddRoot("diagnostics", diagnostics);
		using var before = context.Capture();
		diagnostics.Add(new(2, 10, 60, "Update", "System.NullReferenceException", "fixture", "stack"));
		using var after = context.Capture();
		diagnostics.Add(diagnostics[0] with { EngineFrame = 3 });
		var detached = diagnostics.ToArray();
		context.Restore(before);
		check(diagnostics.Count == 0 && detached.Length == 2, "Checkpoint restore retained abandoned script failures or changed a detached result.");
		context.Restore(after);
		check(diagnostics.Count == 1 && diagnostics[0].EngineFrame == 2, "Checkpoint after an exception lost its diagnostic prefix.");
		var result = new BattleResult(false, 0, 0, 90, "INVALID_VALUE", Array.Empty<BattleUnitResult>(), 60, 136, "", default);
		check(!JsonSerializer.Serialize(result).Contains("ScriptExceptions", StringComparison.Ordinal), "Normal result JSON gained an empty diagnostics field.");
		check(JsonSerializer.Serialize(result with { ScriptExceptions = detached }).Contains("ScriptExceptions", StringComparison.Ordinal), "Repeated script diagnostics are absent from the result JSON.");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void ThrowAdapterFailure(CoroutineManager manager)
	{
		IEnumerator Adapter() { yield return null; throw new NullReferenceException("adapter failure"); }
		manager.AppendCoroutine(Adapter(), ePauseType.IGNORE_BLACK_OUT);
		manager._Update();
		manager._Update();
	}
}
