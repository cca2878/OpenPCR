using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Object = global::OpenPCR.Runtime.Api.Object;

namespace OpenPCR.Runtime.Tests;

internal static class Program
{
	private static int assertions;
	private static void Check(bool value, string message)
	{
		if (!value) throw new InvalidOperationException(message);
		assertions++;
	}
	private static void Throws<T>(Action action) where T : Exception
	{
		try { action(); }
		catch (T) { assertions++; return; }
		throw new InvalidOperationException("Expected " + typeof(T).Name);
	}
	private static float Float(JsonElement x) => BitConverter.Int32BitsToSingle(unchecked((int)x.GetUInt32()));
	private static uint Bits(float value) => unchecked((uint)BitConverter.SingleToInt32Bits(value));
	private static JsonDocument Fixture(string file) => JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "fixtures", file)));
	private static void Main()
	{
		global::OpenPCR.Runtime.Api.Random.InitState(1172);
		var state = global::OpenPCR.Runtime.Api.Random.state;
		RuntimeAssembly(); PoseValueContracts(); Identity(); Clock(); Waits(); Curves(); Scenes(); WorldCache();
		InitializationFailureTests.Run(Check);
		PlatformBoundaries.Run(Check);
		Check(state.Equals(global::OpenPCR.Runtime.Api.Random.state), "Infrastructure unexpectedly consumed RNG.");
		Console.WriteLine($"Unity infrastructure: {assertions} assertions passed. Native fixtures certify the stated scalar subsets; they do not certify resource construction, activation callbacks, clock production or whole-battle replay.");
	}
	private static void RuntimeAssembly()
	{
		var runtime = typeof(Object).Assembly;
		Check(runtime.GetName().Name == "OpenPCR.Runtime", "Runtime assembly identity.");
		Check(typeof(Collider2D).Assembly == runtime && typeof(ScriptingRandom).Assembly == runtime && typeof(ScriptingTime).Assembly == runtime, "Unity APIs and libunity state must share one executable assembly.");
		Check(!Array.Exists(runtime.GetReferencedAssemblies(), a => a.Name is "UnityEngine.CoreModule" or "UnityEngine.Physics2DModule" or "OpenPCR.UnityNative"), "Runtime retains an obsolete split dependency.");
	}
	private readonly record struct ReferenceEngineVector(float X, float Y, float Z);
	private readonly record struct ReferenceEngineQuaternion(float X, float Y, float Z, float W);
	private static void PoseValueContracts()
	{
		foreach (var scalar in new[] { 0f, -0f, 1.25f, float.NaN, float.PositiveInfinity })
		{
			var vector = new EngineVector(scalar, 2f, -3f);
			var referenceVector = new ReferenceEngineVector(scalar, 2f, -3f);
			Check(vector.GetHashCode() == referenceVector.GetHashCode(), "C# 9 vector carrier must retain record hash semantics.");
			Check(vector.ToString() == referenceVector.ToString().Replace("ReferenceEngineVector", "EngineVector"), "Vector diagnostic representation changed.");
			Check(vector == new EngineVector(scalar, 2f, -3f), "Vector value equality changed, including NaN.");
			var rotation = new EngineQuaternion(scalar, 2f, -3f, 1f);
			var referenceRotation = new ReferenceEngineQuaternion(scalar, 2f, -3f, 1f);
			Check(rotation.GetHashCode() == referenceRotation.GetHashCode(), "C# 9 quaternion carrier must retain record hash semantics.");
			Check(rotation.ToString() == referenceRotation.ToString().Replace("ReferenceEngineQuaternion", "EngineQuaternion"), "Quaternion diagnostic representation changed.");
			Check(rotation == new EngineQuaternion(scalar, 2f, -3f, 1f), "Quaternion value equality changed, including NaN.");
		}
	}
	private static bool FakeNull(Object value) => value == null;
	private static void Identity()
	{
		var a = new Object(); var b = new Object();
		Check(FakeNull(a) && FakeNull(b) && !a && !b, "Fresh managed wrappers must be fake-null.");
		Check(a != b && !a.Equals(b), "Distinct unbound wrappers must remain distinct.");
		Check(a.Equals((object?)null) && !a!.Equals(new object()), "Equals must reject non-Unity objects.");
		int hash = a.GetHashCode();
		EngineObjectIdentity.Bind(a, (IntPtr)123); EngineObjectIdentity.Bind(b, (IntPtr)123);
		Check(a && b && !FakeNull(a) && !FakeNull(b), "Bound wrappers must be alive.");
		Check(a != b && !a.Equals(b), "Pointer sharing must not replace target managed identity.");
		Object alias = a;
		Check(a == alias && a.Equals(alias) && a.GetHashCode() == hash, "Managed identity/hash changed.");
		EngineObjectIdentity.Unbind(a); EngineObjectIdentity.Unbind(b);
		Check(!a && FakeNull(a) && a != b && a.GetHashCode() == hash, "Unbinding must change liveness only.");
		// The wrapper type is warm: identity writes must not allocate cache factory delegates.
		long allocated = GC.GetAllocatedBytesForCurrentThread();
		for (int i = 0; i < 10000; i++)
		{
			EngineObjectIdentity.Bind(a, (IntPtr)123);
			EngineObjectIdentity.Unbind(a);
		}
		Check(GC.GetAllocatedBytesForCurrentThread() == allocated, "Cached identity access allocated per-call objects.");
		Throws<ArgumentNullException>(() => EngineObjectIdentity.Bind(null!, (IntPtr)1));
		Throws<ArgumentException>(() => EngineObjectIdentity.Bind(new object(), (IntPtr)1));
	}
	private static void Clock()
	{
		ScriptingTime.EndSession();
		Throws<InvalidOperationException>(() => Time.time.ToString());
		Throws<InvalidOperationException>(() => Time.timeScale = 1);
		using var doc = Fixture("time-native.json");
		foreach (var row in doc.RootElement.GetProperty("cases").EnumerateArray())
		{
			var rational = new EngineRationalTime(row.GetProperty("count").GetInt64(), row.GetProperty("numerator").GetUInt32(), row.GetProperty("denominator").GetUInt32());
			ScriptingTime.Initialize(new EngineFrameTime(rational, rational, rational, rational, int.MinValue), 1);
			foreach (float value in new[] { Time.time, Time.deltaTime, Time.unscaledTime, Time.unscaledDeltaTime })
				Check(Bits(value) == row.GetProperty("valueBits").GetUInt32(), "Native rational-time readout differs.");
			Check(Time.frameCount == int.MinValue, "Frame counter signedness changed.");
			ScriptingTime.EndSession();
		}
		var r = new EngineRationalTime(1, 30, 1); var frame = new EngineFrameTime(r, r, r, r, 0);
		ScriptingTime.Initialize(frame, 2);
		Throws<InvalidOperationException>(() => ScriptingTime.Initialize(frame, 1));
		Time.timeScale = float.NaN; Check(Time.timeScale == 2, "NaN scale must keep old state.");
		Time.timeScale = -1; Check(Time.timeScale == 2, "Negative scale must keep old state.");
		Time.timeScale = BitConverter.Int32BitsToSingle(unchecked((int)0x80000000)); Check(Bits(Time.timeScale) == 0x80000000, "Signed zero scale was lost.");
		Time.timeScale = float.PositiveInfinity; Check(float.IsPositiveInfinity(Time.timeScale), "Positive infinite scale changed.");
		ScriptingTime.PublishUpdate(new EngineFrameTime(new EngineRationalTime(2, 30, 1), r, new EngineRationalTime(3, 30, 1), r, int.MaxValue));
		Check(Time.frameCount == int.MaxValue && float.IsPositiveInfinity(Time.timeScale), "Publishing a frame reset scale.");
		Check(Time.time == 2f / 30 && Time.unscaledTime == 3f / 30, "Scaled/unscaled time was merged.");
		ScriptingTime.EndSession(); Throws<InvalidOperationException>(() => Time.deltaTime.ToString());
	}
	private sealed class DynamicWait : CustomYieldInstruction
	{
		internal bool Value;
		internal int Calls;
		public override bool keepWaiting { get { Calls++; return Value; } }
	}
	private static void Waits()
	{
		bool value = false; int calls = 0;
		var until = new WaitUntil(() => { calls++; return value; });
		var whileWait = new WaitWhile(() => { calls++; return value; });
		Check(calls == 0 && until.Current == null, "Waiting constructor evaluated predicate.");
		Check(until.MoveNext() && !whileWait.MoveNext() && calls == 2, "Waiting predicate polarity/count differs.");
		value = true; Check(!until.MoveNext() && whileWait.MoveNext() && calls == 4, "Waiting result was cached.");
		until.Reset(); Check(calls == 4 && whileWait.Current == null, "Current/Reset evaluated predicate.");
		var dynamicWait = new DynamicWait { Value = true }; Check(dynamicWait.MoveNext(), "Missing virtual waiting dispatch.");
		dynamicWait.Value = false; Check(!dynamicWait.MoveNext() && dynamicWait.Calls == 2, "Virtual waiting result was cached.");
		var nullWait = new WaitUntil(null!); Throws<NullReferenceException>(() => nullWait.MoveNext());
		var sentinel = new InvalidOperationException("predicate");
		try { new WaitWhile(() => throw sentinel).MoveNext(); throw new Exception("Missing predicate exception."); }
		catch (InvalidOperationException ex) { Check(ReferenceEquals(ex, sentinel), "Predicate exception was replaced."); }
		var field = typeof(WaitForSeconds).GetField("m_Seconds", BindingFlags.NonPublic | BindingFlags.Instance)!;
		foreach (uint bits in new uint[] { 0, 0x80000000, 0x3ee66666, 0xbf800000, 0x7f800000, 0xff800000, 0x7fc12345 })
			Check(Bits((float)field.GetValue(new WaitForSeconds(BitConverter.Int32BitsToSingle(unchecked((int)bits))))!) == bits, "WaitForSeconds payload changed.");
	}
	private static void Curves()
	{
		using var doc = Fixture("curve-native.json");
		foreach (var row in doc.RootElement.GetProperty("curves").EnumerateArray())
		{
			var raw = row.GetProperty("keys"); var keys = new Keyframe[raw.GetArrayLength()]; int i = 0;
			foreach (var key in raw.EnumerateArray()) keys[i++] = new Keyframe(Float(key[0]), Float(key[1]), Float(key[2]), Float(key[3]));
			var curve = new AnimationCurve(keys);
			foreach (var sample in row.GetProperty("samples").EnumerateArray())
				Check(Bits(curve.Evaluate(Float(sample[0]))) == sample[1].GetUInt32(), "Cached Native curve polynomial differs.");
			GC.KeepAlive(curve);
		}
		var original = new[] { new Keyframe(0, 0, 0, 0), new Keyframe(1, 1, 0, 0) };
		var copy = new AnimationCurve(original); original[1] = new Keyframe(1, 999, 0, 0);
		Check(copy.Evaluate(.5f) == .5f, "Curve must own its key copy.");
		Throws<NotSupportedException>(() => copy.Evaluate(float.NaN));
		Throws<NotSupportedException>(() => new AnimationCurve(new Keyframe(0, 0, 0, 0), new Keyframe(0, 1, 0, 0)));
		var weighted = new[] { new EngineCurveKey { Time = 0, Value = 0, WeightedMode = 1 } };
		Throws<NotSupportedException>(() => CurveScripting.Create(MemoryMarshal.AsBytes(weighted.AsSpan()), 2, 2));
		Throws<NotSupportedException>(() => CurveScripting.Create(ReadOnlySpan<byte>.Empty, 1, 2));
		var handle = CurveScripting.Create(ReadOnlySpan<byte>.Empty, 2, 2); CurveScripting.Destroy(handle);
		Throws<InvalidOperationException>(() => CurveScripting.Evaluate(handle, 0));
		Check(new AnimationCurve().Evaluate(0) == 0, "Empty curve differs.");
		var filtered = new AnimationCurve(new Keyframe(float.NaN, 2, 0, 0), new Keyframe(0, 5, 0, 0));
		Check(filtered.Evaluate(float.NaN) == 5, "Single-key/filtering branch differs.");
	}
	private static GameObject ResourceWrapper() => (GameObject)RuntimeHelpers.GetUninitializedObject(typeof(GameObject));
	private static Transform TransformWrapper() => (Transform)Activator.CreateInstance(typeof(Transform), nonPublic: true)!;
	// Cached axis-aligned world reads equal the uncached chain walk and keep its failures after every mutation they depend on.
	private static void WorldCache()
	{
		var objects = new GameObject[3]; var transforms = new Transform[3]; var nodes = new EngineSceneNode[3];
		for (int i = 0; i < 3; i++) { objects[i] = ResourceWrapper(); transforms[i] = TransformWrapper(); }
		for (int i = 0; i < 3; i++)
			nodes[i] = new EngineSceneNode(new EngineSceneObject(objects[i], (IntPtr)(30 + i)), new[] { new EngineSceneObject(transforms[i], (IntPtr)(40 + i)) },
				i == 0 ? null : transforms[i - 1], new EngineSceneActivity(objects[i], true, false, 255, Array.Empty<EngineBehaviourRegistration>()));
		SceneScripting.Initialize(nodes);
		// Scene bindings are single-threaded: another thread fails explicitly instead of racing the owner.
		bool rejected = false;
		var other = new System.Threading.Thread(() => { try { SceneScripting.Parent(transforms[2]); } catch (InvalidOperationException) { rejected = true; } });
		other.Start(); other.Join();
		Check(rejected && ReferenceEquals(SceneScripting.Parent(transforms[2]), transforms[1]), "Scene bindings were readable from a thread that does not own them.");
		var identity = new EngineQuaternion(0, 0, 0, 1);
		PreparedTransformInputs.Initialize(new[]
		{
			new EngineTransformPose(transforms[0], new(0.1f, -2.3f, 0.7f), new(1.3f, 0.7f, 1f), identity),
			new EngineTransformPose(transforms[1], new(3.3f, 0.9f, -0.2f), new(0.3f, 2.1f, 1f), identity),
			new EngineTransformPose(transforms[2], new(-1.7f, 0.4f, 0.1f), new(1.9f, 0.6f, 1f), identity),
		});
		var point = new EngineVector(0.37f, -1.1f, 0.25f);
		bool Same(EngineVector a, EngineVector b) => Bits(a.X) == Bits(b.X) && Bits(a.Y) == Bits(b.Y) && Bits(a.Z) == Bits(b.Z);
		// Unity's uncached recursion from local poses only: TransformPoint(t, v) = Position(t) + lossyScale(t) * v.
		EngineVector Scale(object transform)
		{
			var scale = new EngineVector(1, 1, 1);
			for (object? current = transform; current is not null; current = SceneScripting.Parent(current))
			{
				var local = PreparedTransformInputs.LocalScale(current); scale = new(scale.X * local.X, scale.Y * local.Y, scale.Z * local.Z);
			}
			return scale;
		}
		EngineVector Point(object transform, EngineVector value)
		{
			var parent = SceneScripting.Parent(transform); var local = PreparedTransformInputs.LocalPosition(transform);
			var position = parent is null ? local : Point(parent, local); var scale = Scale(transform);
			return new(position.X + scale.X * value.X, position.Y + scale.Y * value.Y, position.Z + scale.Z * value.Z);
		}
		void Matches(string after)
		{
			var expected = Point(transforms[2], point); var scale = Scale(transforms[2]);
			for (int read = 0; read < 2; read++)
				Check(Same(PreparedTransformInputs.TransformPoint(transforms[2], point), expected) && Same(PreparedTransformInputs.Scale(transforms[2]), scale), "Cached world read differs " + after + ".");
		}
		Matches("on first use");
		PreparedTransformInputs.LocalPosition(transforms[1], new(-0.6f, 1.4f, 0.3f)); Matches("after an ancestor position write");
		PreparedTransformInputs.LocalScale(transforms[0], new(0.9f, 1.7f, 1f)); Matches("after a root scale write");
		PreparedTransformInputs.SetParent(transforms[1], null, false); Matches("after reparenting");
		// Activity publication validates every handle; a validated scene is reused only until its identity changes.
		SceneScripting.PublishActivity(objects[0], true, _ => true); SceneScripting.PublishActivity(objects[0], true, _ => true);
		EngineObjectIdentity.Unbind(transforms[1]);
		Throws<InvalidOperationException>(() => PreparedTransformInputs.TransformPoint(transforms[2], point));
		Throws<InvalidOperationException>(() => PreparedTransformInputs.Scale(transforms[2]));
		Throws<InvalidOperationException>(() => SceneScripting.PublishActivity(objects[0], true, _ => true));
		EngineObjectIdentity.Bind(transforms[1], (IntPtr)41); Matches("after rebinding");
		SceneScripting.PublishActivity(objects[0], true, _ => true);
		PreparedTransformInputs.EndSession();
		Throws<InvalidOperationException>(() => PreparedTransformInputs.TransformPoint(transforms[2], point));
		SceneScripting.EndSession();
	}
	private static void Scenes()
	{
		SceneScripting.EndSession();
		var component = new Component(); var behaviour = new MonoBehaviour(); var transform = TransformWrapper();
		Check(FakeNull(component) && FakeNull(behaviour) && FakeNull(transform), "Managed constructors allocated engine objects.");
		Throws<InvalidOperationException>(() => component.gameObject.ToString());
		using var doc = Fixture("scene-native.json");
		foreach (var row in doc.RootElement.GetProperty("cases").EnumerateArray())
		{
			var objects = new GameObject[3]; var transforms = new Transform[3]; var nodes = new EngineSceneNode[3];
			for (int i = 0; i < 3; i++) { objects[i] = ResourceWrapper(); transforms[i] = TransformWrapper(); }
			for (int i = 0; i < 3; i++)
				nodes[i] = new EngineSceneNode(new EngineSceneObject(objects[i], (IntPtr)(10 + i)),
					new[] { new EngineSceneObject(transforms[i], (IntPtr)(20 + i)) }, i == 0 ? null : transforms[i - 1],
					new EngineSceneActivity(objects[i], row.GetProperty("active")[i].GetBoolean(), row.GetProperty("suppressed")[i].GetBoolean(), row.GetProperty("cache").GetByte(), Array.Empty<EngineBehaviourRegistration>()));
			SceneScripting.Initialize(nodes);
			for (int i = 0; i < 3; i++) Check(ReferenceEquals(transforms[i].parent, i == 0 ? null : transforms[i - 1]), "Numeric parent reference changed.");
			for (int i = 2; i >= 0; i--) Check(objects[i].activeInHierarchy == row.GetProperty("leafToRoot")[2 - i].GetBoolean(), "Native hierarchy/cache readout differs.");
			Check(ReferenceEquals(transforms[2].gameObject, objects[2]) && ReferenceEquals(objects[2].transform, transforms[2]) && ReferenceEquals(transforms[2].transform, transforms[2]), "Prepared component ownership/reference changed.");
			SceneScripting.EndSession();
			Check(FakeNull(objects[2]) && FakeNull(transforms[2]), "Scene teardown did not clear its live bindings.");
		}
		var gameObject = ResourceWrapper(); var root = TransformWrapper();
		var components = new[] { new EngineSceneObject(root, (IntPtr)2), new EngineSceneObject(behaviour, (IntPtr)3) };
		var initial = new EngineSceneActivity(gameObject, true, false, 255, new[] { new EngineBehaviourRegistration(behaviour, false) });
		var node = new EngineSceneNode(new EngineSceneObject(gameObject, (IntPtr)1), components, null, initial);
		Throws<ArgumentException>(() => SceneScripting.Initialize(new[] { node with { Activity = initial with { Registrations = Array.Empty<EngineBehaviourRegistration>() } } }));
		Check(FakeNull(gameObject) && FakeNull(root), "Rejected resource binding changed wrapper identity.");
		Throws<ArgumentException>(() => SceneScripting.Initialize(new[] { node with { ParentTransform = root } }));
		Throws<ArgumentException>(() => SceneScripting.Initialize(new[] { node, node }));
		SceneScripting.Initialize(new[] { node });
		components[0] = new EngineSceneObject(TransformWrapper(), (IntPtr)99);
		Check(ReferenceEquals(gameObject.transform, root), "Scene retained caller-owned component array.");
		Check(gameObject.activeSelf && gameObject.activeInHierarchy && !behaviour.isActiveAndEnabled, "Registered byte was inferred from hierarchy.");
		Throws<ArgumentException>(() => SceneScripting.PublishActivity(Array.Empty<EngineSceneActivity>()));
		Check(gameObject.activeSelf && !behaviour.isActiveAndEnabled, "Rejected snapshot changed engine fields.");
		SceneScripting.PublishActivity(new[] { initial with { ActiveSelf = false, HierarchyCache = 1, Registrations = new[] { new EngineBehaviourRegistration(behaviour, true) } } });
		Check(!gameObject.activeSelf && gameObject.activeInHierarchy && behaviour.isActiveAndEnabled, "Raw hierarchy cache/registered fields were recomputed prematurely.");
		SceneScripting.PublishActivity(new[] { initial with { ActiveSelf = false, HierarchyCache = 255 } });
		Check(!gameObject.activeInHierarchy && !behaviour.isActiveAndEnabled, "Explicit cache invalidation did not re-read activity.");
		Throws<InvalidOperationException>(() => gameObject.SetActive(true));
		Check(!gameObject.activeSelf, "Unbound activation silently mutated readouts.");
		PreparedSceneOperations.Initialize(_ => throw new InvalidOperationException("No clone producer in this fixture."), (_, _) => { });
		Throws<InvalidOperationException>(() => gameObject.SetActive(true));
		Check(!gameObject.activeSelf, "A producer that did not publish activity was accepted.");
		PreparedSceneOperations.EndSession();
		int activityCalls = 0;
		PreparedSceneOperations.Initialize(_ => throw new InvalidOperationException("No clone producer in this fixture."), (owner, value) =>
		{
			Check(ReferenceEquals(owner, gameObject), "Activation changed its bound owner.");
			activityCalls++;
			SceneScripting.PublishActivity(new[] { initial with { ActiveSelf = value, HierarchyCache = 255 } });
		});
		gameObject.SetActive(true);
		Check(activityCalls == 1 && gameObject.activeSelf && gameObject.activeInHierarchy, "Prepared activation failed to invoke and publish its lifecycle producer.");
		PreparedSceneOperations.EndSession();
		SceneScripting.EndSession();
		Check(ReferenceEquals(gameObject.gameObject, gameObject), "GameObject.gameObject must return this even when unbound.");
		Throws<InvalidOperationException>(() => gameObject.activeSelf.ToString());
	}
}
