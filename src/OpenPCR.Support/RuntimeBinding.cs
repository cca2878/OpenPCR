using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace OpenPCR;

/// <summary>Access existing members without extending mirrored declarations.</summary>
public static class RuntimeBinding
{
	public const BindingFlags Members = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	// Cache successful lookups by type, member name and runtime argument types; failures throw without being cached.
	private static readonly ConcurrentDictionary<(Type, string), MemberInfo> setters = new();
	// Fields use direct loads; properties, constants and mismatched receiver kinds retain reflection behavior.
	private static readonly ConcurrentDictionary<(Type, string, bool), Func<object?, object?>> getters = new();
	private static readonly ConcurrentDictionary<CallKey, MethodInfo> methods = new();
	private readonly record struct CallKey(Type Type, string Name, int Count, Type? A0, Type? A1, Type? A2, Type? A3);

	public static void Set(object target, string name, object? value)
	{
		var type = target is Type declaration ? declaration : target.GetType();
		if (!setters.TryGetValue((type, name), out var member))
		{
			for (Type? search = type; search is not null && member is null; search = search.BaseType)
			{
				var property = search.GetProperty(name, Members | BindingFlags.DeclaredOnly);
				if (property?.SetMethod is not null) member = property;
				else member = search.GetField(name, Members | BindingFlags.DeclaredOnly);
			}
			if (member is null) throw new MissingMemberException(target.GetType().FullName, name);
			setters[(type, name)] = member;
		}
		if (member is PropertyInfo p) p.SetValue(target is Type ? null : target, value);
		else ((FieldInfo)member).SetValue(target is Type ? null : target, value);
	}
	public static object? Get(object target, string name)
	{
		var type = target is Type declaration ? declaration : target.GetType();
		bool isStatic = target is Type;
		if (!getters.TryGetValue((type, name, isStatic), out var read))
		{
			MemberInfo? member = null;
			for (Type? search = type; search is not null && member is null; search = search.BaseType)
				member = (MemberInfo?)search.GetProperty(name, Members | BindingFlags.DeclaredOnly) ?? search.GetField(name, Members | BindingFlags.DeclaredOnly);
			if (member is null) throw new MissingMemberException(target.GetType().FullName, name);
			read = Reader(member, isStatic);
			getters[(type, name, isStatic)] = read;
		}
		return read(isStatic ? null : target);
	}
	private static Func<object?, object?> Reader(MemberInfo member, bool isStatic)
	{
		if (member is PropertyInfo property) return receiver => property.GetValue(receiver);
		var field = (FieldInfo)member;
		if (field.IsLiteral || field.IsStatic != isStatic) return receiver => field.GetValue(receiver);
		var receiver = System.Linq.Expressions.Expression.Parameter(typeof(object));
		var load = System.Linq.Expressions.Expression.Field(field.IsStatic ? null : System.Linq.Expressions.Expression.Convert(receiver, field.DeclaringType!), field);
		return System.Linq.Expressions.Expression.Lambda<Func<object?, object?>>(System.Linq.Expressions.Expression.Convert(load, typeof(object)), receiver).Compile();
	}
	public static object? Call(object target, string name, params object?[] arguments)
	{
		var type = target is Type declaration ? declaration : target.GetType();
		bool cacheable = arguments.Length <= 4;
		var key = cacheable ? new CallKey(type, name, arguments.Length, Arg(arguments, 0), Arg(arguments, 1), Arg(arguments, 2), Arg(arguments, 3)) : default;
		if (!cacheable || !methods.TryGetValue(key, out var method))
		{
			method = Resolve(type, target, name, arguments);
			if (cacheable) methods[key] = method;
		}
		try { return method.Invoke(target is Type ? null : target, arguments); }
		catch (TargetInvocationException exception) when (exception.InnerException is not null)
		{
			ExceptionDispatchInfo.Capture(exception.InnerException).Throw(); throw;
		}
	}
	public static object Wrap(object value, Type type)
	{
		if (type.IsEnum) return Enum.ToObject(type, value);
		if (type.IsPrimitive || type == typeof(string)) return Convert.ChangeType(value, type, System.Globalization.CultureInfo.InvariantCulture);
		var conversion = System.Linq.Enumerable.Single(type.GetMethods(BindingFlags.Public | BindingFlags.Static), method => method.Name == "op_Implicit" && method.ReturnType == type);
		return conversion.Invoke(null, new[] { Convert.ChangeType(value, conversion.GetParameters()[0].ParameterType, System.Globalization.CultureInfo.InvariantCulture) })!;
	}
	private static Type? Arg(object?[] arguments, int index) => index < arguments.Length ? arguments[index]?.GetType() : null;
	private static MethodInfo Resolve(Type start, object target, string name, object?[] arguments)
	{
		MethodInfo? method = null;
		for (Type? type = start; type is not null && method is null; type = type.BaseType)
			foreach (var candidate in type.GetMethods(Members | BindingFlags.DeclaredOnly))
			{
				if (candidate.Name != name || candidate.ContainsGenericParameters) continue;
				var parameters = candidate.GetParameters(); if (parameters.Length != arguments.Length) continue;
				bool matches = true;
				for (int index = 0; index < parameters.Length; index++)
					if (arguments[index] is null ? parameters[index].ParameterType.IsValueType : !parameters[index].ParameterType.IsInstanceOfType(arguments[index])) matches = false;
				if (!matches) continue;
				if (method is not null) throw new AmbiguousMatchException(name);
				method = candidate;
			}
		return method ?? throw new MissingMethodException(target.GetType().FullName, name);
	}
	private static readonly Func<object, object> shallowClone = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!.CreateDelegate<Func<object, object>>();
	/// <summary>Field-for-field copy of the receiver's runtime type; reference fields stay shared.</summary>
	public static T ShallowClone<T>(T source) where T : class => (T)shallowClone(source ?? throw new ArgumentNullException(nameof(source)));
	public static void Instance(Type generic, Type argument, object? value) =>
		generic.MakeGenericType(argument).GetProperty("Instance", Members)!.SetValue(null, value);
}
