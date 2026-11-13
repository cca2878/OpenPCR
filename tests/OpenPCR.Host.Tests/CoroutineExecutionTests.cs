using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using OpenPCR.Host;

internal static class CoroutineExecutionTests
{
	internal static void Run(Action<bool, string> check)
	{
		var random = ScriptingRandom.Current;
		var root = (GameObject)RuntimeHelpers.GetUninitializedObject(typeof(GameObject));
		var transform = (Transform)Activator.CreateInstance(typeof(Transform), true)!;
		var owner = new MonoBehaviour();
		EngineSceneActivity Activity(bool active) => new(root, active, false, byte.MaxValue, new[] { new EngineBehaviourRegistration(owner, false) });
		SceneScripting.Initialize(new[] { new EngineSceneNode(new(root, (IntPtr)1), new[] { new EngineSceneObject(transform, (IntPtr)2), new EngineSceneObject(owner, (IntPtr)3) }, null, Activity(false)) });
		try
		{
			using var driver = new PreparedCoroutineDriver();
			int steps = 0;
			IEnumerator Work() { steps++; yield return null; steps++; }
			var iterator = Work();
			check(owner.StartCoroutine(iterator) is null && steps == 0, "Inactive owner created or advanced a coroutine instead of Native null.");
			driver.BeginFrame(1); driver.Update(); driver.EndFrame();
			check(steps == 0, "Rejected inactive coroutine was queued for a later frame.");
			SceneScripting.PublishActivity(new[] { Activity(true) });
			check(!owner.isActiveAndEnabled && owner.StartCoroutine(iterator) is not null && steps == 1, "Coroutine gate incorrectly uses Behaviour registration, or inactive request consumed the iterator.");
			driver.BeginFrame(2); driver.Update(); driver.EndFrame();
			check(steps == 2, "Active coroutine lost the original immediate/next-frame execution order.");
			SceneScripting.PublishActivity(new[] { Activity(false) });
			check(owner.StartCoroutine(Work()) is null && steps == 2, "Later deactivated owner still started an iterator.");
		}
		finally { SceneScripting.EndSession(); }
		check(random.Equals(ScriptingRandom.Current) && !PreparedCoroutineOperations.IsInitialized && !SceneScripting.IsInitialized, "Coroutine activation fixture leaked RNG or session ownership.");
	}
}
