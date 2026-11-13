using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using OpenPCR.Host;

internal static class SceneActivityTests
{
	internal static int Run()
	{
		int assertions = 0;
		long handle = 1;
		var enabled = new Dictionary<object, bool>(ReferenceEqualityComparer.Instance);
		EngineSceneNode Node(Transform? parent, params MonoBehaviour[] components)
		{
			var go = (GameObject)RuntimeHelpers.GetUninitializedObject(typeof(GameObject));
			var transform = (Transform)Activator.CreateInstance(typeof(Transform), true)!;
			var bindings = new List<EngineSceneObject> { new(transform, (IntPtr)handle++) };
			var registrations = new List<EngineBehaviourRegistration>();
			foreach (var component in components)
			{
				bindings.Add(new(component, (IntPtr)handle++)); enabled.Add(component, true);
				registrations.Add(new(component, true));
			}
			return new(new(go, (IntPtr)handle++), bindings, parent, new(go, true, false, byte.MaxValue, registrations));
		}
		void Check(bool value, string message) { assertions++; if (!value) throw new Exception(message); }
		void Reject<T>(Action action) where T : Exception
		{
			assertions++;
			try { action(); } catch (T) { return; }
			throw new Exception("Expected " + typeof(T).Name);
		}
		var rootBehaviour = new MonoBehaviour(); var childBehaviour = new MonoBehaviour(); var disabled = new MonoBehaviour();
		var root = Node(null, rootBehaviour);
		var child = Node((Transform)root.Components[0].Wrapper, childBehaviour, disabled);
		var other = Node(null);
		enabled[disabled] = false;
		// Deliberately publish the child before its parent: traversal order is not a topology guarantee.
		SceneScripting.Initialize(new[] { child, root, other });
		try
		{
			var publisher = new SceneActivityPublisher(enabled);
			publisher.Publish(root.GameObject.Wrapper, false);
			Check(!rootBehaviour.isActiveAndEnabled && !childBehaviour.isActiveAndEnabled && !disabled.isActiveAndEnabled, "Parent deactivation must update every Behaviour registration.");
			Check(SceneScripting.ActiveSelf(child.GameObject.Wrapper) && !SceneScripting.ActiveInHierarchy(child.GameObject.Wrapper), "Child activeSelf must survive an inactive ancestor.");
			publisher.Publish(root.GameObject.Wrapper, true);
			publisher.Publish(root.GameObject.Wrapper, true);
			Check(rootBehaviour.isActiveAndEnabled && childBehaviour.isActiveAndEnabled && !disabled.isActiveAndEnabled, "Repeated activation must preserve explicit component enabled values.");
			enabled[childBehaviour] = false;
			publisher.Publish(root.GameObject.Wrapper, true);
			Check(!childBehaviour.isActiveAndEnabled, "An enabled-state change must invalidate cached registration records.");
			enabled[childBehaviour] = true;
			publisher.Publish(root.GameObject.Wrapper, false);
			SceneScripting.Reparent(child.Components[0].Wrapper, other.Components[0].Wrapper);
			publisher.Publish(root.GameObject.Wrapper, false);
			Check(childBehaviour.isActiveAndEnabled && SceneScripting.ActiveInHierarchy(child.GameObject.Wrapper), "Live reparenting must replace the previous ancestor chain.");
			var addedBehaviour = new MonoBehaviour();
			var added = Node((Transform)root.Components[0].Wrapper, addedBehaviour);
			SceneScripting.RegisterNodes(new[] { added });
			publisher.Publish(root.GameObject.Wrapper, false);
			Check(!addedBehaviour.isActiveAndEnabled, "A newly registered resource node must enter the next full activity snapshot.");
			SceneScripting.ConfigureComponentProducer((owner, type) => new(new MonoBehaviour(), true, value => enabled.Add(value, true)));
			var dynamicBehaviour = (MonoBehaviour)SceneScripting.AddComponent(child.GameObject.Wrapper, typeof(MonoBehaviour));
			publisher.Publish(root.GameObject.Wrapper, true);
			Check(dynamicBehaviour.isActiveAndEnabled && addedBehaviour.isActiveAndEnabled, "Dynamically added Behaviour registration must be published.");
			SceneScripting.CompleteComponentDestruction(childBehaviour);
			enabled.Remove(childBehaviour);
			publisher.Publish(root.GameObject.Wrapper, true);
			Check(dynamicBehaviour.isActiveAndEnabled && !disabled.isActiveAndEnabled, "Removed components must not leave stale or shifted registrations.");
			enabled.Remove(dynamicBehaviour);
			Reject<KeyNotFoundException>(() => publisher.Publish(root.GameObject.Wrapper, false));
			Check(SceneScripting.ActiveSelf(root.GameObject.Wrapper), "A rejected host snapshot must not partially change scene activity.");
			enabled.Add(dynamicBehaviour, true);
			publisher.Publish(root.GameObject.Wrapper, false);
			Check(!addedBehaviour.isActiveAndEnabled && dynamicBehaviour.isActiveAndEnabled, "Retry after rejection must rebuild activity from the live scene.");
			EngineObjectIdentity.Unbind(rootBehaviour);
			Reject<InvalidOperationException>(() => publisher.Publish(root.GameObject.Wrapper, true));
			EngineObjectIdentity.Bind(rootBehaviour, root.Components[1].Handle);
			Check(!SceneScripting.ActiveSelf(root.GameObject.Wrapper), "Cached records must still pass the Runtime's live-pointer validation.");
			publisher.Publish(root.GameObject.Wrapper, true);
			Check(rootBehaviour.isActiveAndEnabled && addedBehaviour.isActiveAndEnabled, "A valid retry must recover after Runtime validation rejects publication.");
			SceneScripting.CompleteComponentDestruction(dynamicBehaviour);
			enabled.Remove(dynamicBehaviour);
			publisher.Publish(root.GameObject.Wrapper, true);
			Check(!disabled.isActiveAndEnabled, "Removing the last Behaviour must truncate an otherwise unchanged registration prefix.");
			SceneScripting.CompleteComponentDestruction(disabled);
			enabled.Remove(disabled);
			publisher.Publish(root.GameObject.Wrapper, true);
			foreach (var node in SceneScripting.SnapshotNodes())
				if (ReferenceEquals(node.GameObject.Wrapper, child.GameObject.Wrapper))
					Check(node.Activity.Registrations.Count == 0, "Removing all Behaviours must publish an empty registration list.");
		}
		finally { SceneScripting.EndSession(); }
		return assertions + RunVersioned() + RunRegistration();
	}

	// RegisterNodes appends a fresh subtree: every rejected import must leave the scene and wrapper identity untouched,
	// and an accepted one must leave exactly the existing scene plus the input's records.
	private static int RunRegistration()
	{
		int assertions = 0;
		long handle = 1;
		EngineSceneNode Node(Transform? parent, params MonoBehaviour[] components)
		{
			var go = (GameObject)RuntimeHelpers.GetUninitializedObject(typeof(GameObject));
			var transform = (Transform)Activator.CreateInstance(typeof(Transform), true)!;
			var bindings = new List<EngineSceneObject> { new(transform, (IntPtr)handle++) };
			var registrations = new List<EngineBehaviourRegistration>();
			foreach (var component in components) { bindings.Add(new(component, (IntPtr)handle++)); registrations.Add(new(component, true)); }
			return new(new(go, (IntPtr)handle++), bindings, parent, new(go, true, false, byte.MaxValue, registrations));
		}
		void Check(bool value, string message) { assertions++; if (!value) throw new Exception(message); }
		string Describe(IReadOnlyList<EngineSceneNode> nodes)
		{
			var text = new System.Text.StringBuilder();
			foreach (var node in nodes)
			{
				text.Append(RuntimeHelpers.GetHashCode(node.GameObject.Wrapper)).Append('/').Append(node.GameObject.Handle).Append('<')
					.Append(node.ParentTransform is null ? 0 : RuntimeHelpers.GetHashCode(node.ParentTransform)).Append(':')
					.Append(node.Activity.ActiveSelf).Append(node.Activity.HierarchySuppressed).Append(node.Activity.HierarchyCache);
				foreach (var component in node.Components) text.Append('|').Append(RuntimeHelpers.GetHashCode(component.Wrapper)).Append('/').Append(component.Handle);
				foreach (var field in node.Activity.Registrations) text.Append(',').Append(RuntimeHelpers.GetHashCode(field.Wrapper)).Append('=').Append(field.Registered);
				text.Append(';');
			}
			return text.ToString();
		}
		var rootBehaviour = new MonoBehaviour(); var childBehaviour = new MonoBehaviour();
		var root = Node(null, rootBehaviour);
		var child = Node((Transform)root.Components[0].Wrapper, childBehaviour);
		SceneScripting.Initialize(new[] { root, child });
		try
		{
			Transform RootTransform() => (Transform)root.Components[0].Wrapper;
			var cases = new List<(string Name, Func<EngineSceneNode[]> Input)>
			{
				("empty components", () => { var n = Node(null); return new[] { n with { Components = Array.Empty<EngineSceneObject>() } }; }),
				("existing GameObject", () => { var n = Node(null); return new[] { n with { GameObject = root.GameObject } }; }),
				("existing handle", () => { var n = Node(null); return new[] { n with { GameObject = new(n.GameObject.Wrapper, root.GameObject.Handle) } }; }),
				("foreign parent", () => new[] { Node((Transform)Activator.CreateInstance(typeof(Transform), true)!) }),
				("activity names an existing GameObject", () => { var n = Node(RootTransform()); return new[] { n with { Activity = root.Activity } }; }),
				("registration of another node's Behaviour", () => { var b = new MonoBehaviour(); var n = Node(RootTransform(), b); return new[] { n with { Activity = n.Activity with { Registrations = new[] { new EngineBehaviourRegistration(b, true), new EngineBehaviourRegistration(rootBehaviour, true) } } } }; }),
				("missing registration", () => { var n = Node(RootTransform(), new MonoBehaviour()); return new[] { n with { Activity = n.Activity with { Registrations = Array.Empty<EngineBehaviourRegistration>() } } }; }),
				("already bound component", () => { var b = new MonoBehaviour(); var n = Node(RootTransform(), b); EngineObjectIdentity.Bind(b, (IntPtr)999999); return new[] { n }; }),
				("wrapper shared by two inputs", () => { var a = Node(RootTransform()); var b = Node(RootTransform()); return new[] { a, b with { GameObject = a.GameObject } }; }),
			};
			foreach (var (name, make) in cases)
			{
				var input = make(); string before = Describe(SceneScripting.SnapshotNodes());
				assertions++;
				try { SceneScripting.RegisterNodes(input); throw new Exception("Expected ArgumentException: " + name); } catch (ArgumentException) { }
				Check(before == Describe(SceneScripting.SnapshotNodes()), "A rejected import changed the scene: " + name);
				foreach (var node in input)
					if (!SceneScripting.SnapshotNodes().Any(existing => ReferenceEquals(existing.GameObject.Wrapper, node.GameObject.Wrapper)))
						Check(node.GameObject.Wrapper as global::OpenPCR.Runtime.Api.Object == null, "A rejected import bound a wrapper: " + name);
			}
			string existingScene = Describe(SceneScripting.SnapshotNodes());
			var leafBehaviour = new MonoBehaviour(); var hiddenBehaviour = new MonoBehaviour();
			var branch = Node(RootTransform());
			var leaf = Node((Transform)branch.Components[0].Wrapper, leafBehaviour, hiddenBehaviour);
			leaf = leaf with { Activity = leaf.Activity with { ActiveSelf = false, HierarchyCache = 0, Registrations = new[] { new EngineBehaviourRegistration(leafBehaviour, true), new EngineBehaviourRegistration(hiddenBehaviour, false) } } };
			SceneScripting.RegisterNodes(new[] { branch, leaf });
			Check(Describe(SceneScripting.SnapshotNodes()) == existingScene + Describe(new[] { branch, leaf }), "An accepted import must append exactly its own records.");
			Check(ReferenceEquals(SceneScripting.Parent(leaf.Components[0].Wrapper), branch.Components[0].Wrapper) && ReferenceEquals(SceneScripting.Parent(branch.Components[0].Wrapper), RootTransform()), "Imported parents must resolve within the import and the scene.");
			Check(leafBehaviour.isActiveAndEnabled && !hiddenBehaviour.isActiveAndEnabled && !SceneScripting.ActiveInHierarchy(leaf.GameObject.Wrapper), "Imported raw fields must be published as given.");
			Check(leafBehaviour != null && leaf.GameObject.Wrapper as global::OpenPCR.Runtime.Api.Object != null, "Imported wrappers must be bound.");
			Check(SceneScripting.AllocateHandle().ToInt64() >= handle, "Handle allocation must continue after every imported handle.");
		}
		finally { SceneScripting.EndSession(); }
		return assertions;
	}

	// With a publisher-owned enabled table, a publication that may take the Runtime's subtree path must leave exactly
	// the scene a complete publication of the same change leaves.
	private static int RunVersioned()
	{
		int assertions = 0;
		long handle = 1;
		var publisher = new SceneActivityPublisher();
		EngineSceneNode Node(Transform? parent, params MonoBehaviour[] components)
		{
			var go = (GameObject)RuntimeHelpers.GetUninitializedObject(typeof(GameObject));
			var transform = (Transform)Activator.CreateInstance(typeof(Transform), true)!;
			var bindings = new List<EngineSceneObject> { new(transform, (IntPtr)handle++) };
			var registrations = new List<EngineBehaviourRegistration>();
			foreach (var component in components)
			{
				bindings.Add(new(component, (IntPtr)handle++)); publisher.Add(component, true);
				registrations.Add(new(component, true));
			}
			return new(new(go, (IntPtr)handle++), bindings, parent, new(go, true, false, byte.MaxValue, registrations));
		}
		void Check(bool value, string message) { assertions++; if (!value) throw new Exception(message); }
		string Describe(IReadOnlyList<EngineSceneNode> nodes)
		{
			var text = new System.Text.StringBuilder();
			foreach (var node in nodes)
			{
				text.Append(RuntimeHelpers.GetHashCode(node.GameObject.Wrapper)).Append(':').Append(node.Activity.ActiveSelf).Append(node.Activity.HierarchySuppressed).Append(node.Activity.HierarchyCache);
				foreach (var field in node.Activity.Registrations) text.Append(',').Append(RuntimeHelpers.GetHashCode(field.Wrapper)).Append('=').Append(field.Registered);
				text.Append(';');
			}
			return text.ToString();
		}
		var rootBehaviour = new MonoBehaviour(); var childBehaviour = new MonoBehaviour(); var disabled = new MonoBehaviour();
		var leafBehaviour = new MonoBehaviour(); var otherBehaviour = new MonoBehaviour();
		var root = Node(null, rootBehaviour);
		var child = Node((Transform)root.Components[0].Wrapper, childBehaviour, disabled);
		var leaf = Node((Transform)child.Components[0].Wrapper, leafBehaviour);
		var other = Node(null, otherBehaviour);
		publisher.Set(disabled, false);
		SceneScripting.Initialize(new[] { leaf, child, root, other });
		try
		{
			var outside = (GameObject)RuntimeHelpers.GetUninitializedObject(typeof(GameObject));
			object Go(EngineSceneNode node) => node.GameObject.Wrapper;
			var steps = new (object Prep, bool PrepActive, Action? Between, object Target, bool Active)[]
			{
				(Go(root), true, null, Go(root), false),
				(Go(root), false, null, Go(root), true),
				(Go(root), true, null, Go(child), false),
				(Go(child), false, null, Go(leaf), false),
				(Go(leaf), false, null, Go(root), false),
				(Go(root), false, null, Go(child), true),
				(Go(root), true, null, Go(other), false),
				(Go(other), false, () => publisher.Set(childBehaviour, false), Go(child), true),
				(Go(child), true, null, outside, true),
				(Go(root), true, () => SceneScripting.Reparent(leaf.Components[0].Wrapper, other.Components[0].Wrapper), Go(other), true),
				(Go(other), true, null, Go(root), false),
				(Go(root), false, () => publisher.Set(childBehaviour, true), Go(leaf), true),
				(Go(leaf), true, null, Go(leaf), true),
				// A reparent outside the published subtree: only the moved subtree's ancestry changed.
				(Go(root), false, () => SceneScripting.Reparent(leaf.Components[0].Wrapper, root.Components[0].Wrapper), Go(other), true),
				(Go(other), false, () => SceneScripting.Reparent(leaf.Components[0].Wrapper, other.Components[0].Wrapper), Go(child), true),
				(Go(root), true, () => { SceneScripting.Reparent(leaf.Components[0].Wrapper, null); SceneScripting.Reparent(child.Components[0].Wrapper, other.Components[0].Wrapper); }, outside, false),
			};
			foreach (var step in steps)
			{
				publisher.Publish(step.Prep, step.PrepActive);
				step.Between?.Invoke();
				publisher.Publish(step.Target, step.Active);
				string versioned = Describe(SceneScripting.SnapshotNodes());
				SceneScripting.PublishActivity(step.Target, step.Active, publisher.Enabled);
				Check(versioned == Describe(SceneScripting.SnapshotNodes()), "A versioned publication must leave the scene a complete publication leaves.");
			}
			Check(!childBehaviour.isActiveAndEnabled || SceneScripting.ActiveInHierarchy(Go(child)), "Registered state must follow the published hierarchy.");
			var external = new SceneActivityPublisher(new Dictionary<object, bool>(ReferenceEqualityComparer.Instance));
			assertions++;
			try { external.Set(rootBehaviour, false); throw new Exception("Expected InvalidOperationException"); } catch (InvalidOperationException) { }
		}
		finally { SceneScripting.EndSession(); }
		return assertions;
	}
}
