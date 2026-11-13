using System;
using System.Collections;
using System.Runtime.CompilerServices;
using OpenPCR.Runtime.Api;
using OpenPCR.Runtime.Impl;
using Object = OpenPCR.Runtime.Api.Object;

namespace OpenPCR.Runtime.Tests;

internal static class InitializationFailureTests
{
	internal static void Run(Action<bool, string> check)
	{
		void Reject<T>(Action action) where T : Exception
		{
			try { action(); }
			catch (T) { check(true, "Expected rejected initialization input."); return; }
			throw new InvalidOperationException("Expected " + typeof(T).Name);
		}
		Func<object, IEnumerator, object> start = (_, _) => new object();
		Action<object, object> stop = (_, _) => { };
		Action<object> stopAll = _ => { };
		for (int argument = 0; argument < 3; argument++)
		{
			try
			{
				Reject<ArgumentNullException>(() => PreparedCoroutineOperations.Initialize(argument == 0 ? null! : start, argument == 1 ? null! : stop, argument == 2 ? null! : stopAll));
				check(!PreparedCoroutineOperations.IsInitialized, "Rejected coroutine callbacks left a partial active session.");
				PreparedCoroutineOperations.Initialize(start, stop, stopAll);
				check(PreparedCoroutineOperations.IsInitialized, "Valid coroutine initialization could not follow rejected callbacks.");
			}
			finally { PreparedCoroutineOperations.EndSession(); }
		}
		if (IntPtr.Size != sizeof(long)) return;
		EngineSceneNode Node(long objectHandle, long transformHandle)
		{
			var gameObject = (GameObject)RuntimeHelpers.GetUninitializedObject(typeof(GameObject));
			var transform = (Transform)RuntimeHelpers.GetUninitializedObject(typeof(Transform));
			return new(new(gameObject, new IntPtr(objectHandle)), new[] { new EngineSceneObject(transform, new IntPtr(transformHandle)) }, null,
				new(gameObject, true, false, byte.MaxValue, Array.Empty<EngineBehaviourRegistration>()));
		}
		bool Unbound(EngineSceneNode node) => (Object)node.GameObject.Wrapper == null && (Object)node.Components[0].Wrapper == null;
		EngineSceneNode Handles(EngineSceneNode node, long objectHandle, long transformHandle) => node with
		{
			GameObject = new(node.GameObject.Wrapper, new IntPtr(objectHandle)),
			Components = new[] { new EngineSceneObject(node.Components[0].Wrapper, new IntPtr(transformHandle)) }
		};
		for (int slot = 0; slot < 2; slot++)
		{
			var rejected = Node(slot == 0 ? long.MaxValue : 1, slot == 1 ? long.MaxValue : 2);
			try
			{
				Reject<OverflowException>(() => SceneScripting.Initialize(new[] { rejected }));
				check(!SceneScripting.IsInitialized && Unbound(rejected), "Overflowing initial handles published a scene or bound wrappers.");
				var valid = Handles(rejected, 1, 2);
				SceneScripting.Initialize(new[] { valid });
				check(SceneScripting.SnapshotNodes().Count == 1 && !Unbound(valid), "A valid scene could not reuse wrappers from rejected initialization.");
			}
			finally { SceneScripting.EndSession(); }
		}
		for (int slot = 0; slot < 2; slot++)
		{
			var root = Node(1, 2);
			var rejected = Node(slot == 0 ? long.MaxValue : 3, slot == 1 ? long.MaxValue : 4);
			try
			{
				SceneScripting.Initialize(new[] { root });
				Reject<OverflowException>(() => SceneScripting.RegisterNodes(new[] { rejected }));
				check(SceneScripting.SnapshotNodes().Count == 1 && Unbound(rejected), "Overflowing incremental handles partly registered the subtree.");
				check(ReferenceEquals(SceneScripting.Transform(root.GameObject.Wrapper), root.Components[0].Wrapper) && SceneScripting.ActiveSelf(root.GameObject.Wrapper), "Rejected subtree changed the original scene.");
				check(SceneScripting.AllocateHandle() == new IntPtr(3), "Rejected subtree advanced the live handle allocator.");
				var valid = Handles(rejected, 3, 4);
				SceneScripting.RegisterNodes(new[] { valid });
				check(SceneScripting.SnapshotNodes().Count == 2 && !Unbound(valid), "A valid subtree could not reuse wrappers from rejected registration.");
			}
			finally { SceneScripting.EndSession(); }
		}
	}
}
