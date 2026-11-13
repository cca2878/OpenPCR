using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;
using System.Collections.Generic;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>Explicit resource and lifecycle producers for a prepared scene.</summary>
	public static class PreparedSceneOperations
	{
		private static Dictionary<object, string>? names;
		private static Func<object, object>? clone;
		private static Action<object, bool>? activate;
		private static Action<object>? createEmpty;
		private static Func<object, bool>? readEnabled;
		private static Action<object, bool>? writeEnabled;
		public static bool IsInitialized => names is not null;
		public static void Initialize(Func<object, object> cloneResource, Action<object, bool> setActivity, Action<object>? createEmptyObject = null, Func<object, bool>? getEnabled = null, Action<object, bool>? setEnabled = null)
		{
			if (names is not null) throw new InvalidOperationException("Prepared scene operations are already active.");
			clone = cloneResource ?? throw new ArgumentNullException(nameof(cloneResource));
			activate = setActivity ?? throw new ArgumentNullException(nameof(setActivity));
			createEmpty = createEmptyObject;
			readEnabled = getEnabled; writeEnabled = setEnabled;
			names = new(ReferenceEqualityComparer.Instance);
		}
		public static void CreateEmpty(object gameObject, string? name)
		{
			ArgumentNullException.ThrowIfNull(gameObject);
			if (name is not null) throw new NotSupportedException("Prepared creation supports the original unnamed GameObject constructor only.");
			if (EngineObjectIdentity.ReadPointer(gameObject) != IntPtr.Zero) throw new InvalidOperationException("Cannot create an already registered GameObject.");
			(createEmpty ?? throw new InvalidOperationException("No empty GameObject producer."))(gameObject);
			if (!ReferenceEquals(SceneScripting.GameObject(SceneScripting.Transform(gameObject)), gameObject)) throw new InvalidOperationException("Creation must register the supplied GameObject identity.");
		}
		public static void BindName(object gameObject, string name)
		{
			if (!ReferenceEquals(SceneScripting.GameObject(SceneScripting.Transform(gameObject)), gameObject)) throw new ArgumentException("Name requires the actual GameObject.");
			if (names is null || !names.TryAdd(gameObject, name)) throw new InvalidOperationException("Resource name must be supplied once per live GameObject.");
		}
		public static string Name(object wrapper)
		{
			var owner = SceneScripting.GameObject(SceneScripting.Transform(wrapper));
			return names is not null && names.TryGetValue(owner, out string? name) ? name : throw new InvalidOperationException("Missing authored resource name.");
		}
		public static object Clone(object original)
		{
			ArgumentNullException.ThrowIfNull(original);
			SceneScripting.Transform(original);
			var result = (clone ?? throw new InvalidOperationException("No resource clone producer."))(original);
			if (result is null || ReferenceEquals(result, original) || result.GetType() != original.GetType()) throw new InvalidOperationException("Resource clone must be a fresh exact wrapper.");
			SceneScripting.Transform(result); return result;
		}
		public static void SetActive(object gameObject, bool value)
		{
			SceneScripting.ActiveSelf(gameObject);
			(activate ?? throw new InvalidOperationException("No activity/lifecycle producer."))(gameObject, value);
			if (SceneScripting.ActiveSelf(gameObject) != value) throw new InvalidOperationException("Activity producer did not publish the requested state.");
		}
		public static bool Enabled(object behaviour)
		{
			SceneScripting.GameObject(behaviour);
			return (readEnabled ?? throw new InvalidOperationException("No Behaviour enabled-state producer."))(behaviour);
		}
		public static void SetEnabled(object behaviour, bool value)
		{
			SceneScripting.GameObject(behaviour);
			(writeEnabled ?? throw new InvalidOperationException("No Behaviour enabled-state producer."))(behaviour, value);
			if (Enabled(behaviour) != value || SceneScripting.IsActiveAndEnabled(behaviour) != (value && SceneScripting.ActiveInHierarchy(SceneScripting.GameObject(behaviour))))
				throw new InvalidOperationException("Enabled-state producer did not publish the requested state.");
		}
		public static void EndSession() { names = null; clone = null; activate = null; createEmpty = null; readEnabled = null; writeEnabled = null; }
	}

}
