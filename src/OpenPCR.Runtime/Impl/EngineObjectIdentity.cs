using global::OpenPCR.Runtime.Api;
using global::OpenPCR.Runtime.Impl;
#nullable enable
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenPCR.Runtime.Impl
{

	/// <summary>
	/// Binds a managed Unity wrapper to the scene owner's opaque engine pointer. The owner supplies and clears the
	/// pointer at its real allocation/destruction boundaries; this API neither allocates engine objects nor synthesizes
	/// resources or lifecycle callbacks. Target player identity uses this pointer only for null/liveness tests.
	/// </summary>
	public static class EngineObjectIdentity
	{
		private sealed record Accessor(Func<object, IntPtr> Read, Action<object, IntPtr> Write);

		// m_CachedPtr is declared once on OpenPCR.Runtime.Api.Object, so one accessor serves every wrapper; a wrapper type is
		// valid exactly when it derives from that Object type.
		private static Accessor? shared;

		public static void Bind(object wrapper, IntPtr enginePointer)
		{
			CachedPointer(wrapper).Write(wrapper, enginePointer);
			SceneScripting.StructureChanged(); // Liveness that scene validation and cached world reads check.
		}
		private static Accessor CachedPointer(object wrapper)
		{
			if (wrapper is null) throw new ArgumentNullException(nameof(wrapper));
			if (wrapper is not global::OpenPCR.Runtime.Api.Object) throw new ArgumentException("A target OpenPCR.Runtime.Api.Object wrapper is required.", "wrapper");
			return shared ??= Create(typeof(global::OpenPCR.Runtime.Api.Object));
		}
		private static Accessor Create(Type wrapperType)
		{
			Type? type = wrapperType;
			while (type is not null && type != typeof(global::OpenPCR.Runtime.Api.Object)) type = type.BaseType;
			if (type is null || type.Assembly != typeof(global::OpenPCR.Runtime.Api.Object).Assembly)
				throw new ArgumentException("A target OpenPCR.Runtime.Api.Object wrapper is required.", "wrapper");
			var field = type.GetField("m_CachedPtr", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
			if (field is null || field.FieldType != typeof(IntPtr))
				throw new InvalidOperationException("The target player cached-pointer declaration is missing.");
			var target = Expression.Parameter(typeof(object));
			var value = Expression.Parameter(typeof(IntPtr));
			var member = Expression.Field(Expression.Convert(target, type), field);
			return new Accessor(
				Expression.Lambda<Func<object, IntPtr>>(member, target).Compile(),
				Expression.Lambda<Action<object, IntPtr>>(Expression.Assign(member, value), target, value).Compile());
		}
		internal static IntPtr ReadPointer(object wrapper) => CachedPointer(wrapper).Read(wrapper);
		public static void Unbind(object wrapper) => Bind(wrapper, IntPtr.Zero);
	}

}
