using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;
using System.Collections.Generic;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>A real managed wrapper and the scene owner's nonzero opaque live handle.</summary>
	public sealed record EngineSceneObject(object Wrapper, IntPtr Handle);
	public sealed record EngineBehaviourRegistration(object Wrapper, bool Registered);
	public sealed record EngineSceneActivity(object GameObject, bool ActiveSelf, bool HierarchySuppressed, byte HierarchyCache,
		IReadOnlyList<EngineBehaviourRegistration> Registrations);
	public sealed record EngineSceneNode(EngineSceneObject GameObject, IReadOnlyList<EngineSceneObject> Components,
		object? ParentTransform, EngineSceneActivity Activity);
	public sealed record EngineComponentCreation(object Wrapper, bool? Registered, Action<object> Initialize);

	/// <summary>
	/// Owns prepared scene bindings and the native field readouts used by game logic. It does not create prefabs,
	/// allocate Unity GameObjects, produce lifecycle state, or dispatch activation/destruction/coroutine callbacks.
	/// Those producers must supply real wrappers, ownership, ordered components and complete activity snapshots.
	/// Only explicitly implemented mutations are supported; publishing a snapshot does not implement missing setters.
	/// </summary>
	public static class SceneScripting
	{
		private sealed class Node
		{
			internal readonly EngineSceneObject GameObject;
			internal readonly List<EngineSceneObject> Components;
			internal Node? Parent;
			internal bool ActiveSelf, HierarchySuppressed;
			internal byte HierarchyCache;
			internal Node(EngineSceneNode input)
			{
				GameObject = input.GameObject;
				Components = new List<EngineSceneObject>(input.Components);
			}
		}
		private sealed class Session
		{
			// Scene state is single-threaded like the engine it serves: it belongs to the thread that initialized it.
			internal int OwnerThread = Environment.CurrentManagedThreadId;
			internal readonly Dictionary<object, Node> Owners = new(ReferenceEqualityComparer.Instance);
			internal readonly Dictionary<object, bool> Registered = new(ReferenceEqualityComparer.Instance);
			internal readonly Dictionary<object, IntPtr> Handles = new(ReferenceEqualityComparer.Instance);
			internal readonly HashSet<IntPtr> HandleValues = new();
			internal readonly List<Node> Nodes = new();
			internal Func<object, Type, EngineComponentCreation>? ComponentProducer;
			internal Action<object>? DestructionProducer;
			internal long NextHandle = 1;
			// The last activity publication: its enabled source and version, and the structure epoch it saw. A later
			// publication with the same source, version and epoch differs only in the published GameObject's subtree.
			internal Func<object, bool>? ActivityEnabled;
			internal long ActivityVersion, ActivityEpoch = -1;
			internal Dictionary<Node, HashSet<Node>>? Children;
			internal long ChildrenEpoch = -1;
			// Roots of subtrees reparented since the last activity publication; their ancestor chains changed.
			internal readonly List<Node> ActivityDirty = new();
		}
		private static Session? session;
		public static bool IsInitialized { get { return session is not null; } }
		private static readonly System.Collections.Concurrent.ConcurrentDictionary<(Type, string), bool> baseTypes = new();
		private static bool HasBase(object wrapper, string name) => baseTypes.GetOrAdd((wrapper.GetType(), name), static key =>
		{
			Type? expected = key.Item2 switch { "GameObject" => typeof(global::OpenPCR.Runtime.Api.GameObject), "Transform" => typeof(global::OpenPCR.Runtime.Api.Transform), "Component" => typeof(global::OpenPCR.Runtime.Api.Component), _ => null };
			for (Type? type = key.Item1; type is not null; type = type.BaseType)
				if (type == expected) return true;
			return false;
		});
		private static void Add(Session state, EngineSceneObject binding, Node node, string type, HashSet<IntPtr> handles)
		{
			if (binding is null || binding.Wrapper is null || binding.Handle == IntPtr.Zero || !HasBase(binding.Wrapper, type))
				throw new ArgumentException("Prepared scene requires a real " + type + " wrapper and nonzero live handle.");
			if (!state.Owners.TryAdd(binding.Wrapper, node) || !handles.Add(binding.Handle))
				throw new ArgumentException("Prepared scene wrapper/handle ownership must be unique.");
			state.Handles.Add(binding.Wrapper, binding.Handle); state.HandleValues.Add(binding.Handle);
		}
		public static void Initialize(IReadOnlyList<EngineSceneNode> input)
		{
			ArgumentNullException.ThrowIfNull(input);
			using var mutation = MutateStructure();
			if (session is not null) throw new InvalidOperationException("Scene bindings are already initialized.");
			if (input.Count == 0) throw new ArgumentException("A prepared scene must contain real resource nodes.", nameof(input));
			var state = new Session(); var handles = new HashSet<IntPtr>();
			foreach (var item in input)
			{
				if (item is null || item.Components is null || item.Components.Count == 0 || item.Activity is null)
					throw new ArgumentException("Every node requires its ordered component table and activity state.", nameof(input));
				var node = new Node(item); state.Nodes.Add(node);
				Add(state, item.GameObject, node, "GameObject", handles);
				for (int i = 0; i < node.Components.Count; i++) Add(state, node.Components[i], node, i == 0 ? "Transform" : "Component", handles);
				for (int i = 1; i < node.Components.Count; i++)
					if (node.Components[i].Wrapper is global::OpenPCR.Runtime.Api.Transform) throw new ArgumentException("Transform must be the unique component at slot zero.");
			}
			for (int i = 0; i < input.Count; i++)
			{
				var parent = input[i].ParentTransform;
				if (parent is not null)
				{
					if (!state.Owners.TryGetValue(parent, out var owner) || !ReferenceEquals(owner.Components[0].Wrapper, parent))
						throw new ArgumentException("Parent must be an actual prepared Transform.");
					state.Nodes[i].Parent = owner;
				}
			}
			foreach (var node in state.Nodes)
			{
				var ancestors = new HashSet<Node>();
				for (Node? ancestor = node; ancestor is not null; ancestor = ancestor.Parent)
					if (!ancestors.Add(ancestor)) throw new ArgumentException("Prepared Transform hierarchy contains a cycle.");
			}
			var activities = new EngineSceneActivity[input.Count];
			for (int i = 0; i < input.Count; i++) activities[i] = input[i].Activity;
			Publish(state, activities);
			// Validate all declarations before changing any wrapper; a rejected import binds nothing.
			foreach (var wrapper in state.Handles.Keys)
				if (EngineObjectIdentity.ReadPointer(wrapper) != IntPtr.Zero) throw new ArgumentException("Prepared wrapper is already owned by a live engine object.");
			foreach (var handle in state.Handles.Values) state.NextHandle = Math.Max(state.NextHandle, checked(handle.ToInt64() + 1));
			foreach (var pair in state.Handles) EngineObjectIdentity.Bind(pair.Key, pair.Value);
			session = state;
		}
		// Owner validity of every handle depends only on the scene structure and object identity, which change only in the
		// mutations below and EngineObjectIdentity.Bind; a successful full validation holds until the structure epoch moves.
		private static long structureEpoch, validatedEpoch = -1;
		/// <summary>Scene structure or object identity changed: revalidate handles and drop cached world reads.</summary>
		internal static void StructureChanged() { structureEpoch++; PreparedTransformInputs.InvalidateWorld(); }
		/// <summary>Marks a structure mutation on entry and exit, so a read during the mutation is not reused after it.</summary>
		private static StructureMutation MutateStructure() { StructureChanged(); return default; }
		private readonly struct StructureMutation : IDisposable { public void Dispose() => StructureChanged(); }
		private static Session Get()
		{
			var state = session ?? throw new InvalidOperationException("Scene readouts require explicit prepared scene bindings.");
			if (state.OwnerThread != Environment.CurrentManagedThreadId) throw new InvalidOperationException("Prepared scene bindings belong to the thread that initialized them.");
			return state;
		}
		private static Node Owner(Session state, object wrapper)
		{
			if (wrapper is null || !state.Owners.TryGetValue(wrapper, out var node) || EngineObjectIdentity.ReadPointer(wrapper) != Handle(state, node, wrapper))
				throw new InvalidOperationException("The wrapper has no live object in the current prepared scene.");
			return node;
		}
		// The owner's binding records hold the same handles as Session.Handles; scanning them avoids a second lookup.
		private static IntPtr Handle(Session state, Node node, object wrapper)
		{
			if (ReferenceEquals(node.GameObject.Wrapper, wrapper)) return node.GameObject.Handle;
			foreach (var component in node.Components)
				if (ReferenceEquals(component.Wrapper, wrapper)) return component.Handle;
			return state.Handles[wrapper];
		}
		private static Node GameObjectNode(Session state, object wrapper)
		{
			var node = Owner(state, wrapper);
			return ReferenceEquals(node.GameObject.Wrapper, wrapper) ? node : throw new ArgumentException("GameObject fields require the actual GameObject wrapper.");
		}
		public static object GameObject(object component)
		{
			if (component is not global::OpenPCR.Runtime.Api.Component) throw new ArgumentException("An actual Component wrapper is required.");
			return Owner(Get(), component).GameObject.Wrapper;
		}
		public static object Transform(object wrapper) { return Owner(Get(), wrapper).Components[0].Wrapper; }
		public static object? GetComponent(object gameObject, Type type)
		{
			ArgumentNullException.ThrowIfNull(type);
			if (type.IsInterface) throw new NotSupportedException("Interface component search requires the Native scripting fallback; only class queries are wired.");
			var node = GameObjectNode(Get(), gameObject);
			foreach (var component in node.Components)
				if (type.IsInstanceOfType(component.Wrapper)) return component.Wrapper;
			return null;
		}
		public static object? GetComponentInChildren(object gameObject, Type type)
		{
			ArgumentNullException.ThrowIfNull(type);
			if (type.IsInterface) throw new NotSupportedException("Interface component search requires the Native scripting fallback.");
			var state = Get(); var root = GameObjectNode(state, gameObject);
			object? Search(Node node, bool self)
			{
				if (self || Hierarchy(node))
					foreach (var component in node.Components)
						if (type.IsInstanceOfType(component.Wrapper)) return component.Wrapper;
				foreach (var child in state.Nodes)
					if (ReferenceEquals(child.Parent, node)) { var found = Search(child, false); if (found is not null) return found; }
				return null;
			}
			return Search(root, true);
		}
		public static void ConfigureComponentProducer(Func<object, Type, EngineComponentCreation> producer)
		{
			ArgumentNullException.ThrowIfNull(producer);
			var state = Get();
			if (state.ComponentProducer is not null) throw new InvalidOperationException("The component producer is already configured.");
			state.ComponentProducer = producer;
		}
		public static object AddComponent(object gameObject, Type type)
		{
			ArgumentNullException.ThrowIfNull(type);
			using var mutation = MutateStructure();
			var state = Get(); var node = GameObjectNode(state, gameObject);
			var create = state.ComponentProducer ?? throw new InvalidOperationException("Component allocation and lifecycle require an explicit host producer.");
			var input = create(gameObject, type);
			if (input is null || input.Wrapper is null || input.Initialize is null || input.Wrapper.GetType() != type ||
				input.Wrapper is not global::OpenPCR.Runtime.Api.Component || input.Wrapper is global::OpenPCR.Runtime.Api.Transform || state.Owners.ContainsKey(input.Wrapper) ||
				EngineObjectIdentity.ReadPointer(input.Wrapper) != IntPtr.Zero || (input.Wrapper is global::OpenPCR.Runtime.Api.Behaviour) != input.Registered.HasValue)
				throw new InvalidOperationException("Producer must supply a fresh exact Component type, explicit Behaviour registration and its initialization callback.");
			var binding = new EngineSceneObject(input.Wrapper, AllocateHandle());
			state.Owners.Add(input.Wrapper, node); state.Handles.Add(input.Wrapper, binding.Handle); state.HandleValues.Add(binding.Handle); node.Components.Add(binding);
			if (input.Registered is bool registered) state.Registered.Add(input.Wrapper, registered);
			EngineObjectIdentity.Bind(input.Wrapper, binding.Handle);
			input.Initialize(input.Wrapper);
			return input.Wrapper;
		}
		public static void ConfigureDestructionProducer(Action<object> producer)
		{
			ArgumentNullException.ThrowIfNull(producer);
			var state = Get();
			if (state.DestructionProducer is not null) throw new InvalidOperationException("The destruction producer is already configured.");
			state.DestructionProducer = producer;
		}
		public static void RequestDestroy(object? wrapper)
		{
			if (wrapper is null) return;
			using var mutation = MutateStructure();
			var state = Get(); Owner(state, wrapper);
			(state.DestructionProducer ?? throw new InvalidOperationException("Destruction requires the frame owner's lifecycle producer."))(wrapper);
		}
		public static void CompleteComponentDestruction(object component)
		{
			using var mutation = MutateStructure();
			var state = Get(); var node = Owner(state, component);
			if (ReferenceEquals(node.GameObject.Wrapper, component) || ReferenceEquals(node.Components[0].Wrapper, component)) throw new NotSupportedException("GameObject/Transform destruction requires the full scene lifecycle producer.");
			node.Components.RemoveAll(binding => ReferenceEquals(binding.Wrapper, component));
			state.Registered.Remove(component); state.HandleValues.Remove(state.Handles[component]); state.Handles.Remove(component); state.Owners.Remove(component); EngineObjectIdentity.Unbind(component);
		}
		public static object? Parent(object transform)
		{
			var node = Owner(Get(), transform);
			if (!ReferenceEquals(node.Components[0].Wrapper, transform)) throw new ArgumentException("An actual Transform wrapper is required.");
			return node.Parent?.Components[0].Wrapper;
		}
		/// <summary>One readout of <see cref="Transform"/>'s identity check and <see cref="Parent"/>: returns whether
		/// the wrapper is its node's actual Transform, and that node's parent Transform.</summary>
		public static bool TransformParent(object transform, out object? parent)
		{
			var node = Owner(Get(), transform);
			parent = node.Parent?.Components[0].Wrapper;
			return ReferenceEquals(node.Components[0].Wrapper, transform);
		}
		/// <summary>Publish a fresh resource subtree without rebinding existing scene objects.</summary>
		/// <remarks>
		/// Equivalent to publishing the complete scene snapshot extended by <paramref name="input"/>, computed in place:
		/// every check of the complete publication runs, in its order, against the live scene plus the input before any
		/// field changes, so a rejected import binds and changes nothing. Existing nodes republish their own fields and
		/// registered values, which every Behaviour already has, so only the input's records are added.
		/// </remarks>
		public static void RegisterNodes(IReadOnlyList<EngineSceneNode> input)
		{
			ArgumentNullException.ThrowIfNull(input);
			using var mutation = MutateStructure();
			var state = Get();
			var owners = new Dictionary<object, Node>(ReferenceEqualityComparer.Instance);
			var handles = new HashSet<IntPtr>();
			var added = new List<Node>();
			void Claim(EngineSceneObject binding, Node node, string type)
			{
				if (binding is null || binding.Wrapper is null || binding.Handle == IntPtr.Zero || !HasBase(binding.Wrapper, type))
					throw new ArgumentException("Prepared scene requires a real " + type + " wrapper and nonzero live handle.");
				if (state.Owners.ContainsKey(binding.Wrapper) || !owners.TryAdd(binding.Wrapper, node) || state.HandleValues.Contains(binding.Handle) || !handles.Add(binding.Handle))
					throw new ArgumentException("Prepared scene wrapper/handle ownership must be unique.");
			}
			bool Find(object wrapper, out Node node) => state.Owners.TryGetValue(wrapper, out node!) || owners.TryGetValue(wrapper, out node!);
			foreach (var item in input)
			{
				if (item.Components is null || item.Components.Count == 0 || item.Activity is null) throw new ArgumentException("Fresh nodes require components and activity.");
				var node = new Node(item); added.Add(node);
				Claim(item.GameObject, node, "GameObject");
				for (int index = 0; index < item.Components.Count; index++)
				{
					Claim(item.Components[index], node, index == 0 ? "Transform" : "Component");
					if (index != 0 && item.Components[index].Wrapper is global::OpenPCR.Runtime.Api.Transform) throw new ArgumentException("A node has exactly one Transform.");
				}
			}
			for (int index = 0; index < input.Count; index++)
			{
				if (input[index].ParentTransform is object parent)
				{
					if (!Find(parent, out var owner) || !ReferenceEquals(owner.Components[0].Wrapper, parent)) throw new ArgumentException("Parent must belong to this scene.");
					added[index].Parent = owner;
				}
				var ancestors = new HashSet<Node>();
				for (Node? node = added[index]; node is not null; node = node.Parent)
					if (!ancestors.Add(node)) throw new ArgumentException("Resource hierarchy contains a cycle.");
			}
			foreach (var node in added)
			{
				if (EngineObjectIdentity.ReadPointer(node.GameObject.Wrapper) != IntPtr.Zero) throw new ArgumentException("New GameObject is already bound.");
				foreach (var component in node.Components)
					if (EngineObjectIdentity.ReadPointer(component.Wrapper) != IntPtr.Zero) throw new ArgumentException("New component is already bound.");
			}
			// The complete snapshot lists every existing node first, so an input item may name only a fresh GameObject.
			var seen = new HashSet<Node>(); var registered = new Dictionary<object, bool>(ReferenceEqualityComparer.Instance);
			foreach (var item in input)
			{
				var activity = item.Activity;
				if (activity is null || activity.GameObject is null || !owners.TryGetValue(activity.GameObject, out var node) && !state.Owners.ContainsKey(activity.GameObject) ||
					node is null || !ReferenceEquals(node.GameObject.Wrapper, activity.GameObject) || !seen.Add(node) || activity.Registrations is null)
					throw new ArgumentException("Activity must identify each actual GameObject exactly once.");
				foreach (var field in activity.Registrations)
				{
					if (field is null || field.Wrapper is null || field.Wrapper is not global::OpenPCR.Runtime.Api.Behaviour ||
						!Find(field.Wrapper, out var owner) || !ReferenceEquals(owner, node) || !registered.TryAdd(field.Wrapper, field.Registered))
						throw new ArgumentException("Registered fields must belong to each actual Behaviour exactly once.");
				}
			}
			foreach (var wrapper in owners.Keys)
				if (wrapper is global::OpenPCR.Runtime.Api.Behaviour && !registered.ContainsKey(wrapper)) throw new ArgumentException("Missing explicit Behaviour registered field.");
			long nextHandle = state.NextHandle;
			foreach (var handle in handles) nextHandle = Math.Max(nextHandle, checked(handle.ToInt64() + 1));
			// Validation is complete; publish the fresh records.
			for (int index = 0; index < input.Count; index++)
			{
				var node = added[index]; var activity = input[index].Activity;
				state.Nodes.Add(node);
				state.Owners.Add(node.GameObject.Wrapper, node); state.Handles.Add(node.GameObject.Wrapper, node.GameObject.Handle); state.HandleValues.Add(node.GameObject.Handle);
				foreach (var component in node.Components)
				{
					state.Owners.Add(component.Wrapper, node); state.Handles.Add(component.Wrapper, component.Handle); state.HandleValues.Add(component.Handle);
				}
				node.ActiveSelf = activity.ActiveSelf; node.HierarchySuppressed = activity.HierarchySuppressed; node.HierarchyCache = activity.HierarchyCache;
			}
			foreach (var field in registered) state.Registered.Add(field.Key, field.Value);
			foreach (var node in added)
			{
				EngineObjectIdentity.Bind(node.GameObject.Wrapper, node.GameObject.Handle);
				foreach (var component in node.Components) EngineObjectIdentity.Bind(component.Wrapper, component.Handle);
			}
			state.NextHandle = nextHandle;
		}
		public static IntPtr AllocateHandle()
		{
			var state = Get(); return new IntPtr(checked(state.NextHandle++));
		}
		public static IReadOnlyList<EngineSceneNode> SnapshotNodes()
		{
			var state = Get(); var result = new List<EngineSceneNode>();
			foreach (var node in state.Nodes)
			{
				var registrations = new List<EngineBehaviourRegistration>();
				foreach (var component in node.Components)
					if (state.Registered.TryGetValue(component.Wrapper, out bool registered)) registrations.Add(new(component.Wrapper, registered));
				result.Add(new(node.GameObject, node.Components.ToArray(), node.Parent?.Components[0].Wrapper,
					new(node.GameObject.Wrapper, node.ActiveSelf, node.HierarchySuppressed, node.HierarchyCache, registrations)));
			}
			return result;
		}
		// A reparent changes no ownership, handle or object identity, so handle validation stays current and the
		// structure epoch does not move. It moves one subtree: world reads are dropped on entry and exit, the children
		// index follows the move, and the subtree is marked for the next versioned activity publication.
		public static void Reparent(object transform, object? parent)
		{
			PreparedTransformInputs.InvalidateWorld();
			try
			{
				var state = Get(); var node = Owner(state, transform);
				if (!ReferenceEquals(node.Components[0].Wrapper, transform)) throw new ArgumentException("Reparent requires a Transform.");
				Node? target = parent is null ? null : Owner(state, parent);
				if (target is not null && !ReferenceEquals(target.Components[0].Wrapper, parent)) throw new ArgumentException("Parent must be a Transform.");
				for (Node? ancestor = target; ancestor is not null; ancestor = ancestor.Parent)
					if (ReferenceEquals(ancestor, node)) throw new ArgumentException("Reparent would create a cycle.");
				if (state.Children is not null && state.ChildrenEpoch == structureEpoch)
				{
					if (node.Parent is not null) state.Children[node.Parent].Remove(node);
					if (target is not null)
					{
						if (!state.Children.TryGetValue(target, out var set)) state.Children.Add(target, set = new HashSet<Node>());
						set.Add(node);
					}
				}
				node.Parent = target;
				foreach (var item in state.Nodes) item.HierarchyCache = byte.MaxValue;
				if (state.ActivityEpoch == structureEpoch) state.ActivityDirty.Add(node);
			}
			finally { PreparedTransformInputs.InvalidateWorld(); }
		}
		public static bool ActiveSelf(object gameObject) { return GameObjectNode(Get(), gameObject).ActiveSelf; }
		public static bool ActiveInHierarchy(object gameObject)
		{
			return Hierarchy(GameObjectNode(Get(), gameObject));
		}
		private static bool Hierarchy(Node node)
		{
			if (node.HierarchyCache != byte.MaxValue) return node.HierarchyCache != 0;
			bool value = node.ActiveSelf && !node.HierarchySuppressed;
			node.HierarchyCache = value ? (byte)1 : (byte)0;
			if (node.Parent is not null) node.HierarchyCache = value && Hierarchy(node.Parent) ? (byte)1 : (byte)0;
			return node.HierarchyCache != 0;
		}
		public static bool IsActiveAndEnabled(object behaviour)
		{
			var state = Get(); Owner(state, behaviour);
			return state.Registered.TryGetValue(behaviour, out bool value) ? value : throw new ArgumentException("A registered Behaviour field is required.");
		}
		private static void Publish(Session state, IReadOnlyList<EngineSceneActivity> activity)
		{
			if (activity.Count != state.Nodes.Count) throw new ArgumentException("Publish the complete activity snapshot, without defaults or partial merging.");
			var seen = new HashSet<Node>(); var registered = new Dictionary<object, bool>(ReferenceEqualityComparer.Instance);
			foreach (var item in activity)
			{
				if (item is null || item.GameObject is null || !state.Owners.TryGetValue(item.GameObject, out var node) ||
					!ReferenceEquals(node.GameObject.Wrapper, item.GameObject) || !seen.Add(node) || item.Registrations is null)
					throw new ArgumentException("Activity must identify each actual GameObject exactly once.");
				foreach (var field in item.Registrations)
				{
					if (field is null || field.Wrapper is null || field.Wrapper is not global::OpenPCR.Runtime.Api.Behaviour ||
						!state.Owners.TryGetValue(field.Wrapper, out var owner) || !ReferenceEquals(owner, node) || !registered.TryAdd(field.Wrapper, field.Registered))
						throw new ArgumentException("Registered fields must belong to each actual Behaviour exactly once.");
				}
			}
			foreach (var wrapper in state.Owners.Keys)
				if (wrapper is global::OpenPCR.Runtime.Api.Behaviour && !registered.ContainsKey(wrapper)) throw new ArgumentException("Missing explicit Behaviour registered field.");
			// Validation finishes before any field changes. Registered state is not inferred from activeSelf or ancestry.
			foreach (var item in activity)
			{
				var node = state.Owners[item.GameObject]; node.ActiveSelf = item.ActiveSelf; node.HierarchySuppressed = item.HierarchySuppressed;
				node.HierarchyCache = item.HierarchyCache;
			}
			state.Registered.Clear(); foreach (var field in registered) state.Registered.Add(field.Key, field.Value);
		}
		/// <summary>
		/// Equivalent of publishing the complete host snapshot for one activeSelf change, computed in place: every node
		/// keeps its activeSelf except <paramref name="gameObject"/>, hierarchy suppression clears, and every Behaviour's
		/// registered field becomes <paramref name="enabled"/> and active in hierarchy. Host lookups and live-pointer
		/// validation run in the complete publication's order before any field changes; the registered table is rebuilt
		/// in node and component order and hierarchy caches are left invalidated, as a complete publication leaves them.
		/// </summary>
		public static void PublishActivity(object gameObject, bool activeSelf, Func<object, bool> enabled)
		{
			ArgumentNullException.ThrowIfNull(enabled);
			var state = Get();
			state.ActivityEpoch = -1; state.ActivityDirty.Clear();
			var owner = state.Owners.TryGetValue(gameObject, out var found) && ReferenceEquals(found.GameObject.Wrapper, gameObject) ? found : null;
			foreach (var node in state.Nodes)
				foreach (var component in node.Components)
					if (component.Wrapper is global::OpenPCR.Runtime.Api.Behaviour) enabled(component.Wrapper);
			if (validatedEpoch != structureEpoch)
			{
				foreach (var wrapper in state.Handles.Keys) Owner(state, wrapper);
				validatedEpoch = structureEpoch;
			}
			// A wrapper outside the scene changes no node, as in the host's complete snapshot.
			if (owner is not null) owner.ActiveSelf = activeSelf;
			foreach (var node in state.Nodes) { node.HierarchySuppressed = false; node.HierarchyCache = byte.MaxValue; }
			state.Registered.Clear();
			foreach (var node in state.Nodes)
			{
				bool active = true;
				for (Node? ancestor = node; ancestor is not null; ancestor = ancestor.Parent) active &= ancestor.ActiveSelf;
				foreach (var component in node.Components)
					if (component.Wrapper is global::OpenPCR.Runtime.Api.Behaviour) state.Registered.Add(component.Wrapper, enabled(component.Wrapper) && active);
			}
		}
		/// <summary>
		/// <see cref="PublishActivity(object, bool, Func{object, bool})"/> for an enabled source that versions its
		/// changes: equal <paramref name="enabledVersion"/>s promise unchanged keys and values, and a negative version is
		/// never reused. When the previous publication used the same source and version and the scene structure has not
		/// changed since, every node outside <paramref name="gameObject"/>'s subtree and the subtrees reparented since
		/// already holds the values a complete publication would compute (and every lookup of the complete publication
		/// succeeded then), so only those subtrees' registered fields are recomputed, in place, in any order; hierarchy
		/// suppression and caches are reset for every node as in the complete publication. Otherwise the complete
		/// publication runs.
		/// </summary>
		public static void PublishActivity(object gameObject, bool activeSelf, Func<object, bool> enabled, long enabledVersion)
		{
			ArgumentNullException.ThrowIfNull(enabled);
			var state = Get();
			if (enabledVersion < 0 || !ReferenceEquals(state.ActivityEnabled, enabled) || state.ActivityVersion != enabledVersion ||
				state.ActivityEpoch != structureEpoch || validatedEpoch != structureEpoch)
			{
				PublishActivity(gameObject, activeSelf, enabled);
			}
			else
			{
				var owner = state.Owners.TryGetValue(gameObject, out var found) && ReferenceEquals(found.GameObject.Wrapper, gameObject) ? found : null;
				if (owner is not null) owner.ActiveSelf = activeSelf;
				foreach (var node in state.Nodes) { node.HierarchySuppressed = false; node.HierarchyCache = byte.MaxValue; }
				if (owner is not null || state.ActivityDirty.Count != 0)
				{
					if (state.ChildrenEpoch != structureEpoch)
					{
						state.Children = new Dictionary<Node, HashSet<Node>>();
						foreach (var node in state.Nodes)
							if (node.Parent is not null)
							{
								if (!state.Children.TryGetValue(node.Parent, out var set)) state.Children.Add(node.Parent, set = new HashSet<Node>());
								set.Add(node);
							}
						state.ChildrenEpoch = structureEpoch;
					}
				}
				if (owner is not null) state.ActivityDirty.Add(owner);
				foreach (var root in state.ActivityDirty)
				{
					bool active = true;
					for (Node? ancestor = root.Parent; ancestor is not null; ancestor = ancestor.Parent) active &= ancestor.ActiveSelf;
					RefreshRegistered(state, root, active, enabled);
				}
				state.ActivityDirty.Clear();
			}
			state.ActivityEnabled = enabled; state.ActivityVersion = enabledVersion; state.ActivityEpoch = structureEpoch;
		}
		private static void RefreshRegistered(Session state, Node node, bool ancestorsActive, Func<object, bool> enabled)
		{
			bool active = ancestorsActive && node.ActiveSelf;
			foreach (var component in node.Components)
				if (component.Wrapper is global::OpenPCR.Runtime.Api.Behaviour) state.Registered[component.Wrapper] = enabled(component.Wrapper) && active;
			if (state.Children!.TryGetValue(node, out var children))
				foreach (var child in children) RefreshRegistered(state, child, active, enabled);
		}
		public static void PublishActivity(IReadOnlyList<EngineSceneActivity> activity)
		{
			ArgumentNullException.ThrowIfNull(activity);
			var state = Get(); state.ActivityEpoch = -1; foreach (var wrapper in state.Handles.Keys) Owner(state, wrapper);
			Publish(state, activity);
		}
		public static void EndSession()
		{
			using var mutation = MutateStructure();
			if (session is null) return;
			Get();
			foreach (var pair in session.Handles)
			{
				var pointer = EngineObjectIdentity.ReadPointer(pair.Key);
				if (pointer != IntPtr.Zero && pointer != pair.Value) throw new InvalidOperationException("A wrapper was rebound to a different engine owner.");
			}
			foreach (var wrapper in session.Handles.Keys) EngineObjectIdentity.Unbind(wrapper);
			session = null;
		}
	}

}
