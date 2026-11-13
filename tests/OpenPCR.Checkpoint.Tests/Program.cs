using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using OpenPCR.Checkpoint;

int assertions = 0;
void Check(bool condition) { assertions++; if (!condition) throw new Exception("Assertion " + assertions); }
void Reject<T>(Action action) where T : Exception { assertions++; try { action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T)); }
var first = new Node { Value = 7 };
first.Next = first;
var root = new Root(first);
root.List.Add(first); root.Map.Add("first", first);
root.Nested = new Nested { Item = first, Number = 3 };
root.Array = new[,] { { first, first } };
var iterator = Sequence(root).GetEnumerator();
Check(iterator.MoveNext() && iterator.Current == 7);
root.Iterator = iterator;
var enumeration = root.List.GetEnumerator();
Check(enumeration.MoveNext()); root.Enumerator = enumeration;
var weak = new ConditionalWeakTable<Node, Node>(); weak.Add(first, first);
var receiver = new Receiver(root);
root.Callback = receiver.Invoke;
using var context = new CheckpointContext();
context.AddRoot("root", root); context.AddRoot("weak", weak);
using var a = context.Capture();
for (int round = 0; round < 3; round++)
{
	first.Value = 90; first.Next = new Node(); root.Nested = default;
	root.Array[0, 0] = new Node(); root.Map.Clear();
	for (int i = 0; i < 100; i++) root.List.Add(new Node());
	weak.Clear(); root.Callback();
	typeof(Root).GetField(nameof(Root.First))!.SetValue(root, new Node { Value = 555 });
	Check(iterator.MoveNext());
	context.Restore(a);
	Check(first.Value == 7 && ReferenceEquals(first.Next, first));
	Check(ReferenceEquals(root.First, first));
	Check(root.List.Count == 1 && ReferenceEquals(root.List[0], first) && ReferenceEquals(root.Map["first"], first));
	Check(root.Nested.Number == 3 && ReferenceEquals(root.Nested.Item, first));
	Check(ReferenceEquals(root.Array[0, 0], first) && ReferenceEquals(root.Array[0, 1], first));
	Check(weak.TryGetValue(first, out var target) && ReferenceEquals(target, first));
	Check(root.Counter == 0 && receiver.Count == 0);
	Check(!root.Enumerator.MoveNext());
	Check(iterator.MoveNext() && iterator.Current == 8);
	context.Restore(a);
}
first.Value = 15;
using var b = context.Capture();
context.Restore(a); Check(first.Value == 7);
context.Restore(b); Check(first.Value == 15);
context.Restore(a); Check(first.Value == 7);
using var foreign = new CheckpointContext();
using (var retained = context.Retain(b))
{
	b.Dispose(); context.Restore(retained); Check(first.Value == 15);
	Reject<ArgumentException>(() => context.Retain(b));
	Reject<ArgumentException>(() => foreign.Retain(retained));
}
context.Restore(a);
Reject<ArgumentException>(() => foreign.Restore(a));
b.Dispose(); Reject<ArgumentException>(() => context.Restore(b));
Reject<InvalidOperationException>(() => context.AddRoot("late", new Node()));
using var unsupported = new CheckpointContext(supportedType: t => t != typeof(Node));
unsupported.AddRoot("unsupported", first); Reject<NotSupportedException>(() => unsupported.Capture());
using var lifetime = context.Retain(a);
context.Dispose(); Reject<ObjectDisposedException>(() => context.Restore(a));
Reject<ObjectDisposedException>(() => { _ = lifetime.ObjectCount; });
Reject<ObjectDisposedException>(() => context.Retain(lifetime));
using (var nested = new CheckpointContext())
{
	var sequence = Sequence(root).GetEnumerator();
	Check(sequence.MoveNext() && sequence.MoveNext());
	nested.AddRoot("nested iterator", sequence);
	using var saved = nested.Capture();
	Check(sequence.MoveNext() && sequence.Current == 9);
	Check(!sequence.MoveNext());
	nested.Restore(saved);
	Check(sequence.MoveNext() && sequence.Current == 9);
}
var map = new ConcurrentDictionary<int, Node>(); map[1] = first;
using (var resources = new CheckpointContext())
{
	resources.KeepConcurrentMapMembership(map); resources.AddRoot("handles", map);
	using var saved = resources.Capture();
	first.Value = 99; map[2] = new Node { Value = 22 };
	resources.Restore(saved);
	Check(first.Value == 7 && map.Count == 2 && map[2].Value == 22);
}
var deadKey = new Node();
var liveKey = new Node();
var ephemerons = new ConditionalWeakTable<Node, object>();
ephemerons.Add(deadKey, new System.Threading.Thread(() => { }));
ephemerons.Add(liveKey, first);
using (var roots = new CheckpointContext(weakTableKeysAreRoots: false))
{
	roots.AddRoot("weak table", ephemerons); roots.AddRoot("live key", liveKey);
	using var saved = roots.Capture();
	first.Value = 50; roots.Restore(saved); Check(first.Value == 7);
}
using (var boxed = new CheckpointContext())
{
	IEnumerator iteratorRoot = new List<int> { 10, 20 }.GetEnumerator();
	Check(iteratorRoot.MoveNext()); boxed.AddRoot("boxed iterator", iteratorRoot);
	using var saved = boxed.Capture();
	Check(iteratorRoot.MoveNext() && (int)iteratorRoot.Current == 20);
	Check(!iteratorRoot.MoveNext()); boxed.Restore(saved);
	Check(iteratorRoot.MoveNext() && (int)iteratorRoot.Current == 20);
}
var concurrent = new ConcurrentDictionary<Node, Node>(new CallbackComparer());
concurrent[first] = first;
using (var concurrentState = new CheckpointContext())
{
	concurrentState.AddRoot("concurrent", concurrent);
	using var saved = concurrentState.Capture();
	concurrent[new Node()] = new Node(); first.Value = 30; CallbackComparer.Calls = 0;
	concurrentState.Restore(saved);
	Check(CallbackComparer.Calls == 0 && concurrent.Count == 1 && first.Value == 7);
}
using (var statics = new CheckpointContext())
{
	StaticStorage.Number = 4; StaticStorage.Item = first;
	statics.AddStatic(typeof(StaticStorage).GetField(nameof(StaticStorage.Number))!);
	statics.AddStatic(typeof(StaticStorage).GetField(nameof(StaticStorage.Item))!);
	using var saved = statics.Capture();
	StaticStorage.Number = 9; StaticStorage.Item = new Node();
	statics.Restore(saved); Check(StaticStorage.Number == 4 && ReferenceEquals(StaticStorage.Item, first));
}
var numbers = new[] { new Numbers(1.25f, 5), new Numbers(float.NaN, 6) };
using (var scalars = new CheckpointContext())
{
	scalars.AddRoot("numeric structs", numbers);
	using var saved = scalars.Capture();
	numbers[0] = default; numbers[1] = default;
	scalars.Restore(saved);
	Check(numbers[0].Time == 1.25f && numbers[0].Count == 5 && float.IsNaN(numbers[1].Time) && numbers[1].Count == 6);
}
var finalizable = new Finalizable();
using (var finalizers = new CheckpointContext())
{
	finalizers.AddRoot("finalizable", finalizable);
	using (var saved = finalizers.Capture()) { }
}
GC.Collect(); GC.WaitForPendingFinalizers();
Check(Finalizable.Finalized == 0);
GC.KeepAlive(finalizable);
{
	// A rejection names the whole route from the root: fields, array elements and delegate targets.
	var holder = new Holder { Items = new[] { new Slot { Callback = new System.Threading.Thread(() => { }).Join } } };
	using var routes = new CheckpointContext();
	routes.AddRoot("route", holder);
	string message = "";
	try { routes.Capture(); } catch (NotSupportedException exception) { message = exception.Message; }
	Check(message == "Unsupported checkpoint object at route.Items[].Callback.Target: System.Threading.Thread");
	// Type predicates are asked once per type, however many objects of that type the graph holds.
	int asked = 0;
	using var counted = new CheckpointContext(supportedType: t => { if (t == typeof(Node)) asked++; return true; });
	counted.AddRoot("nodes", new[] { new Node(), new Node(), new Node() });
	using (counted.Capture()) { }
	using (counted.Capture()) { }
	Check(asked == 1);
}
Console.WriteLine($"Checkpoint object graphs: {assertions} assertions passed.");

static IEnumerable<int> Sequence(Root root)
{
	yield return root.First.Value;
	foreach (var value in Inner()) yield return root.First.Value + value;
}
static IEnumerable<int> Inner() { yield return 1; yield return 2; }
sealed class Node { public int Value; public Node? Next; }
sealed class Holder { public Slot[] Items = Array.Empty<Slot>(); }
sealed class Slot { public Action? Callback; }
struct Nested { public Node? Item; public int Number; }
sealed class Root
{
	public readonly Node First;
	public readonly List<Node> List = new();
	public readonly Dictionary<string, Node> Map = new();
	public Node[,] Array = new Node[0, 0];
	public Nested Nested;
	public IEnumerator? Iterator;
	public List<Node>.Enumerator Enumerator;
	public Action Callback = null!;
	public int Counter;
	public Root(Node first) => First = first;
}

sealed class Receiver(Root root) { public int Count; public void Invoke() { root.Counter++; Count++; } }

sealed class Finalizable { public static int Finalized; ~Finalizable() => Finalized++; }

sealed class CallbackComparer : IEqualityComparer<Node>
{
	public static int Calls;
	public bool Equals(Node? left, Node? right) { Calls++; return ReferenceEquals(left, right); }
	public int GetHashCode(Node value) { Calls++; return RuntimeHelpers.GetHashCode(value); }
}

static class StaticStorage { public static int Number; public static Node? Item; }

readonly record struct Numbers(float Time, long Count);
