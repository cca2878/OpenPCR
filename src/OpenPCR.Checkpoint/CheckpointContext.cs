using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace OpenPCR.Checkpoint;

/// <summary>Capture returns a private, reusable memento. Restore must not run simulation callbacks.</summary>
public interface ICheckpointParticipant
{
	object Capture();
	void Restore(object memento);
}

/// <summary>In-process, identity-preserving field mementos. The caller provides an exclusive safe point.</summary>
public sealed class CheckpointContext : IDisposable
{
	private readonly List<(string Name, Func<object?> Read, Action<object?>? Write, bool CopiedValue)> roots = new();
	private readonly List<ICheckpointParticipant> participants = new();
	private readonly HashSet<object> shared = new(ReferenceEqualityComparer.Instance);
	private readonly HashSet<Checkpoint> checkpoints = new();
	private readonly Func<Type, bool> shareType;
	private readonly Func<Type, bool> supportedType;
	private readonly Func<FieldInfo, bool> ignoreField;
	private readonly bool weakTableKeysAreRoots;
	private readonly HashSet<object> retainedMapMembership = new(ReferenceEqualityComparer.Instance);
	private bool disposed, sealedLayout;
	private static readonly ConcurrentDictionary<Type, FieldInfo[]> layouts = new();
	private static readonly ConcurrentDictionary<Type, bool> references = new();
	private readonly Dictionary<Type, FieldInfo[]> captureLayouts = new();
	private readonly Dictionary<Type, Action<object, object>> captureCopiers = new();
	private readonly Dictionary<Type, FieldInfo[]> referenceFields = new();
	private static readonly ConcurrentDictionary<(Type, string), Action<object, object>> copiers = new();
	private static Action<object, object> Copier(Type type, FieldInfo[] fields)
	{
		string signature = string.Join(",", fields.Select(f => $"{f.Module.ModuleVersionId}:{f.MetadataToken}"));
		return copiers.GetOrAdd((type, signature), _ =>
		{
			var method = new DynamicMethod("RestoreCheckpoint", typeof(void), new[] { typeof(object), typeof(object) }, typeof(CheckpointContext).Module, true);
			var il = method.GetILGenerator();
			foreach (var field in fields)
			{
				il.Emit(OpCodes.Ldarg_0); il.Emit(type.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, type);
				il.Emit(OpCodes.Ldarg_1); il.Emit(type.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, type);
				il.Emit(OpCodes.Ldfld, field); il.Emit(OpCodes.Stfld, field);
			}
			il.Emit(OpCodes.Ret);
			return method.CreateDelegate<Action<object, object>>();
		});
	}
	private static readonly Func<object, object> shallowClone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!.CreateDelegate<Func<object, object>>();
	private FieldInfo[] CaptureFields(Type type)
	{
		if (!captureLayouts.TryGetValue(type, out var fields)) captureLayouts.Add(type, fields = Fields(type).Where(f => f.FieldType != typeof(CheckpointContext) && f.FieldType != typeof(Checkpoint) && !ignoreField(f)).ToArray());
		return fields;
	}
	private FieldInfo[] ReferenceFields(Type type)
	{
		if (!referenceFields.TryGetValue(type, out var fields))
		{
			fields = CaptureFields(type).Where(f => HasReferences(f.FieldType)).ToArray();
			referenceFields.Add(type, fields);
		}
		return fields;
	}

	private Action<object, object> CaptureCopier(Type type, FieldInfo[] fields)
	{
		if (!captureCopiers.TryGetValue(type, out var copy)) captureCopiers.Add(type, copy = Copier(type, fields));
		return copy;
	}
	private static bool HasReferences(Type type) => references.GetOrAdd(type, t => !t.IsValueType || (!t.IsPrimitive && !t.IsEnum && Fields(t).Any(f => HasReferences(f.FieldType))));
	public CheckpointContext(Func<Type, bool>? shareType = null, Func<Type, bool>? supportedType = null, Func<FieldInfo, bool>? ignoreField = null, bool weakTableKeysAreRoots = true)
	{
		// Both predicates classify a type, not an object: each type is asked once per context.
		this.shareType = Memoize(shareType ?? (_ => false));
		this.supportedType = Memoize(supportedType ?? (_ => true));
		this.ignoreField = ignoreField ?? (_ => false);
		this.weakTableKeysAreRoots = weakTableKeysAreRoots;
	}
	private static Func<Type, bool> Memoize(Func<Type, bool> predicate)
	{
		var answers = new Dictionary<Type, bool>();
		return type =>
		{
			if (!answers.TryGetValue(type, out bool answer)) answers.Add(type, answer = predicate(type));
			return answer;
		};
	}
	private void Configure() { ObjectDisposedException.ThrowIf(disposed, this); if (sealedLayout) throw new InvalidOperationException("Checkpoint layout is frozen after its first capture."); }
	public void AddRoot(string name, object? value) { Configure(); roots.Add((name, () => value, null, false)); }
	public void AddStatic(FieldInfo field)
	{
		Configure();
		if (!field.IsStatic || field.IsLiteral) throw new ArgumentException("An actual static storage field is required.", nameof(field));
		roots.Add(($"{field.DeclaringType}.{field.Name}", () => field.GetValue(null), field.IsInitOnly ? null : value => field.SetValue(null, value), field.FieldType.IsValueType));
	}
	/// <summary>Keep lifetime-owned resource handles monotonic; captured value objects still rewind.</summary>
	public void KeepConcurrentMapMembership(object map)
	{
		Configure(); ArgumentNullException.ThrowIfNull(map);
		if (!map.GetType().IsGenericType || map.GetType().GetGenericTypeDefinition() != typeof(ConcurrentDictionary<,>)) throw new ArgumentException("A ConcurrentDictionary resource registry is required.", nameof(map));
		retainedMapMembership.Add(map);
	}
	public void Share(object value) { Configure(); shared.Add(value); }
	public void AddParticipant(ICheckpointParticipant participant) { Configure(); ArgumentNullException.ThrowIfNull(participant); participants.Add(participant); shared.Add(participant); }
	public Checkpoint Capture()
	{
		ObjectDisposedException.ThrowIf(disposed, this); sealedLayout = true;
		var builder = new Builder(this);
		var values = new object?[roots.Count];
		for (int i = 0; i < roots.Count; i++) { values[i] = roots[i].Read(); builder.Visit(values[i], roots[i].Name, roots[i].CopiedValue); }
		builder.Drain();
		var mementos = participants.Select(p => p.Capture()).ToArray();
		var checkpoint = new Checkpoint(this, values, builder.Nodes.ToArray(), mementos);
		checkpoints.Add(checkpoint);
		return checkpoint;
	}
	public void Validate(Checkpoint checkpoint)
	{
		ObjectDisposedException.ThrowIf(disposed, this);
		ArgumentNullException.ThrowIfNull(checkpoint);
		if (!ReferenceEquals(checkpoint.Owner, this) || !checkpoints.Contains(checkpoint)) throw new ArgumentException("Checkpoint is disposed or belongs to a different context/session.", nameof(checkpoint));
	}
	/// <summary>Own an independent handle to the same immutable image without traversing the graph again.</summary>
	public Checkpoint Retain(Checkpoint checkpoint)
	{
		Validate(checkpoint);
		var retained = new Checkpoint(this, checkpoint.Roots!, checkpoint.Nodes!, checkpoint.Mementos!);
		checkpoints.Add(retained);
		return retained;
	}
	public void Restore(Checkpoint checkpoint)
	{
		Validate(checkpoint);
		foreach (var node in checkpoint.Nodes!) node.Restore();
		for (int i = 0; i < roots.Count; i++) roots[i].Write?.Invoke(checkpoint.Roots![i]);
		for (int i = 0; i < participants.Count; i++) participants[i].Restore(checkpoint.Mementos![i]);
	}
	internal void Drop(Checkpoint checkpoint) => checkpoints.Remove(checkpoint);
	public void Dispose()
	{
		if (disposed) return;
		disposed = true;
		foreach (var checkpoint in checkpoints.ToArray()) checkpoint.Dispose();
		roots.Clear(); participants.Clear(); shared.Clear(); retainedMapMembership.Clear();
	}
	private static FieldInfo[] Fields(Type type) => layouts.GetOrAdd(type, t =>
	{
		var result = new List<FieldInfo>();
		for (Type? current = t; current is not null; current = current.BaseType)
			result.AddRange(current.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly));
		return result.ToArray();
	});
	private sealed class Builder
	{
		private readonly CheckpointContext context;
		private readonly HashSet<object> seen = new(ReferenceEqualityComparer.Instance);
		private readonly Queue<(object Value, Path Path)> pending = new();
		internal readonly List<INode> Nodes = new();
		private readonly List<(IWeakNode Node, Path Path)> weakTables = new();
		internal Builder(CheckpointContext context) => this.context = context;
		/// <summary>Where an object was reached from; only rendered when a capture is rejected.</summary>
		private sealed class Path
		{
			private readonly Path? parent; private readonly string segment; private readonly bool member;
			internal Path(Path? parent, string segment, bool member) { this.parent = parent; this.segment = segment; this.member = member; }
			public override string ToString() => parent is null ? segment : parent + (member ? "." : "") + segment;
		}
		internal void Visit(object? value, string root, bool copiedValue = false) => Visit(value, null, root, copiedValue);
		private void Visit(object? value, Path? parent, string segment, bool copiedValue = false, bool member = false)
		{
			if (value is null) return;
			var type = value.GetType();
			if (type.IsPrimitive || type.IsEnum || value is string or decimal or DateTime or TimeSpan or Guid || value is Type or MemberInfo or CheckpointContext or Checkpoint || context.shared.Contains(value) || context.shareType(type)) { seen.Add(value); return; }
			if (copiedValue && type.IsValueType)
			{
				Path? path = null;
				if (HasReferences(type))
					foreach (var field in context.ReferenceFields(type))
					{
						if (field.FieldType.IsPointer || field.FieldType.IsByRefLike) throw new NotSupportedException($"Unsupported checkpoint field at {new Path(parent, segment, member)}.{field.Name}");
						Visit(field.GetValue(value), path ??= new Path(parent, segment, member), field.Name, field.FieldType.IsValueType, member: true);
					}
				return;
			}
			if (seen.Add(value)) pending.Enqueue((value, new Path(parent, segment, member)));
		}
		internal void Drain()
		{
			do
			{
				while (pending.TryDequeue(out var item))
				{
					var (value, path) = item;
					var type = value.GetType();
					if (value is Delegate callback)
					{
						foreach (var entry in callback.GetInvocationList()) Visit(entry.Target, path, ".Target");
						continue;
					}
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ConditionalWeakTable<,>))
					{
						var node = (IWeakNode)Activator.CreateInstance(typeof(WeakTableNode<,>).MakeGenericType(type.GetGenericArguments()), value)!;
						Nodes.Add(node);
						if (context.weakTableKeysAreRoots) foreach (var child in node.Children()) Visit(child, path, ".Entries");
						else weakTables.Add((node, path));
						continue;
					}
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ConcurrentDictionary<,>) && context.retainedMapMembership.Contains(value))
					{
						var node = (INode)Activator.CreateInstance(typeof(ConcurrentMapNode<,>).MakeGenericType(type.GetGenericArguments()), value, context.retainedMapMembership.Contains(value))!;
						Nodes.Add(node);
						foreach (var child in node.Children()) Visit(child, path, ".Entries");
						continue;
					}
					if (value is Array array)
					{
						var copy = (Array)array.Clone();
						Nodes.Add(new ArrayNode(array, copy));
						if (HasReferences(type.GetElementType()!))
							foreach (var child in copy) Visit(child, path, "[]", type.GetElementType()!.IsValueType);
						continue;
					}
					if (!context.supportedType(type) || value is System.Runtime.InteropServices.SafeHandle or System.Threading.Thread or System.Threading.Tasks.Task || type.IsCOMObject)
						throw new NotSupportedException($"Unsupported checkpoint object at {path}: {type.FullName}");
					var fields = context.CaptureFields(type);
					var image = shallowClone(value);
					GC.SuppressFinalize(image); // A field image must never release the original object's resources.
					foreach (var field in context.ReferenceFields(type))
					{
						if (field.FieldType.IsPointer || field.FieldType.IsByRefLike) throw new NotSupportedException($"Unsupported checkpoint field at {path}.{field.Name}");
						Visit(field.GetValue(image), path, field.Name, field.FieldType.IsValueType, member: true);
					}
					Nodes.Add(new FieldsNode(value, fields, image, context.CaptureCopier(type, fields)));
				}
				foreach (var table in weakTables)
					foreach (var child in table.Node.LiveValues(seen)) Visit(child, table.Path, ".Value");
			} while (pending.Count > 0);
		}
	}
	internal interface INode { void Restore(); IEnumerable<object?> Children(); }
	private sealed record FieldsNode(object Target, FieldInfo[] Fields, object Image, Action<object, object> Copy) : INode
	{
		public void Restore() => Copy(Target, Image);
		public IEnumerable<object?> Children() => Fields.Select(f => f.GetValue(Image));
	}
	private sealed record ArrayNode(Array Target, Array Values) : INode
	{
		public void Restore() => Array.Copy(Values, Target, Values.Length);
		public IEnumerable<object?> Children() => Values.Cast<object?>();
	}
	private interface IWeakNode : INode { IEnumerable<object?> LiveValues(HashSet<object> seen); }
	private sealed class ConcurrentMapNode<TKey, TValue> : INode where TKey : notnull
	{
		private readonly ConcurrentDictionary<TKey, TValue> target;
		private readonly KeyValuePair<TKey, TValue>[] entries;
		private readonly bool retainMembership;
		public ConcurrentMapNode(object target, bool retainMembership) { this.retainMembership = retainMembership; this.target = (ConcurrentDictionary<TKey, TValue>)target; entries = this.target.ToArray(); }
		public void Restore() { if (retainMembership) return; target.Clear(); foreach (var entry in entries) target.TryAdd(entry.Key, entry.Value); }
		public IEnumerable<object?> Children() { foreach (var entry in entries) { yield return entry.Key; yield return entry.Value; } }
	}
	private sealed class WeakTableNode<TKey, TValue> : IWeakNode where TKey : class where TValue : class
	{
		private readonly ConditionalWeakTable<TKey, TValue> target;
		private readonly KeyValuePair<TKey, TValue>[] entries;
		public WeakTableNode(object target) { this.target = (ConditionalWeakTable<TKey, TValue>)target; entries = this.target.ToArray(); }
		public IEnumerable<object?> LiveValues(HashSet<object> seen) { foreach (var entry in entries) if (seen.Contains(entry.Key)) yield return entry.Value; }
		public void Restore() { target.Clear(); foreach (var entry in entries) target.Add(entry.Key, entry.Value); }
		public IEnumerable<object?> Children() { foreach (var entry in entries) { yield return entry.Key; yield return entry.Value; } }
	}
}

/// <summary>A reusable memento retaining original object identities until disposed.</summary>
public sealed class Checkpoint : IDisposable
{
	internal CheckpointContext? Owner;
	internal object?[]? Roots;
	internal CheckpointContext.INode[]? Nodes;
	internal object[]? Mementos;
	internal Checkpoint(CheckpointContext owner, object?[] roots, CheckpointContext.INode[] nodes, object[] mementos) { Owner = owner; Roots = roots; Nodes = nodes; Mementos = mementos; }
	public int ObjectCount => Nodes?.Length ?? throw new ObjectDisposedException(nameof(Checkpoint));
	public void Dispose() { Owner?.Drop(this); Owner = null; Roots = null; Nodes = null; Mementos = null; }
}
