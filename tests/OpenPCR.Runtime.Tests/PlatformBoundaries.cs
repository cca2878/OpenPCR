using global::OpenPCR.AudioStubs;
using global::OpenPCR.DbStubs;
using global::OpenPCR.Runtime.Api;
using System;
using System.Linq;
using System.Reflection;

namespace OpenPCR.Runtime.Tests;

internal static class PlatformBoundaries
{
	internal static void Run(Action<bool, string> check)
	{
		Assembly platform = typeof(AudioAtomSource).Assembly;
		check(platform.GetName().Name == "OpenPCR.Platform", "Boundary types must come from the product platform assembly.");
		check(typeof(global::OpenPCR.Runtime.Api.Object).Assembly.GetName().Name == "OpenPCR.Runtime", "Platform must use the actual Runtime identity.");
		Type[] unityBoundaries = { typeof(Animation), typeof(AnimationState), typeof(Animator), typeof(AnimatorStateInfo), typeof(AssetBundle), typeof(JsonUtility), typeof(ParticleSystem), typeof(ParticleSystem.MainModule), typeof(ParticleSystem.EmissionModule), typeof(BoxCollider), typeof(Collider), typeof(Font), typeof(TrackedReference) };
		check(unityBoundaries.All(t => t.Assembly == typeof(global::OpenPCR.Runtime.Api.Object).Assembly), "Unity boundary declarations must share Runtime with the other Unity APIs.");
		check(platform.GetTypes().All(t => t.Namespace != typeof(global::OpenPCR.Runtime.Api.Object).Namespace), "Unity declarations must not remain in Platform.");
		check(platform.GetReferencedAssemblies().All(a => a.Name is "OpenPCR.Runtime" or "OpenPCR.Support" || a.Name!.StartsWith("System.", StringComparison.Ordinal)), "A boundary references an assembly outside the product and framework.");
		foreach (Type type in platform.GetTypes().Concat(unityBoundaries))
		{
			check(type.GetCustomAttributesData().All(a => a.AttributeType.Assembly == typeof(Attribute).Assembly || a.AttributeType.Assembly == typeof(global::OpenPCR.Runtime.Api.Object).Assembly), "A boundary type carries metadata outside the framework and Runtime.");
			foreach (MethodBase method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Cast<MethodBase>().Concat(type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)))
			{
				if (method.IsAbstract) continue;
				byte[]? body = method.GetMethodBody()?.GetILAsByteArray();
				check(body is { Length: > 0 } && body[^1] == 0x7a, "An unsupported runtime boundary must end in an explicit throw: " + method);
			}
		}
		bool Rejected(Action action) { try { action(); return false; } catch (InvalidOperationException) { return true; } }
		check(Rejected(() => _ = new Animation()), "Animation must not become an implicit implementation.");
		check(Rejected(() => _ = new AudioAtomSource()), "Audio must not become an implicit implementation.");
		check(Rejected(() => _ = new global::OpenPCR.DbStubs.Connection()), "Legacy database boundary must fail explicitly.");
		check(Rejected(() => _ = default(AnimatorStateInfo).normalizedTime), "Value-type boundary must fail explicitly.");
	}
}
