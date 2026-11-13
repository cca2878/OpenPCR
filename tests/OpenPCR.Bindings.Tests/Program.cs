using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// RuntimeBinding.Set/Get/Call name members of Engine and Runtime types by string. This check binds every call
// site in the projects below and resolves each literal name against the receiver's type, private members included.
string[] projects = { "src/OpenPCR.BattleInputs", "src/OpenPCR.Growth", "src/OpenPCR.Host" };
string[] checkedAssemblies = { "OpenPCR.BattleInputs", "OpenPCR.Growth", "OpenPCR.Host" };

string? root = null;
if (args.Length == 2 && args[0] == "--root") root = Path.GetFullPath(args[1]);
else if (args.Length != 0) { Console.Error.WriteLine("Usage: OpenPCR.Bindings.Tests [--root <repository root>]"); return 2; }
for (var directory = new DirectoryInfo(AppContext.BaseDirectory); root is null && directory is not null; directory = directory.Parent)
	if (File.Exists(Path.Combine(directory.FullName, "OpenPCR.slnx"))) root = directory.FullName;
if (root is null || !File.Exists(Path.Combine(root, "OpenPCR.slnx"))) { Console.Error.WriteLine("Repository root not found; pass --root."); return 2; }

var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
	.Where(path => !checkedAssemblies.Contains(Path.GetFileNameWithoutExtension(path)) && Path.GetFileNameWithoutExtension(path) != "OpenPCR.Bindings.Tests")
	.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path)).ToArray();
var parse = new CSharpParseOptions(LanguageVersion.Preview);
var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, nullableContextOptions: NullableContextOptions.Enable,
	metadataImportOptions: MetadataImportOptions.All);

// Rejection self-check: the same resolver must report a missing member and accept present ones.
var probe = Analyze(new[] { CSharpSyntaxTree.ParseText("""
	using OpenPCR;
	class Target { private int hidden; int Shown { get; set; } void Run(int value) { } }
	sealed class Derived : Target { private int late; }
	static class Probe
	{
		static void Run(Target target, object anything)
		{
			RuntimeBinding.Set(target, "hidden", 1); RuntimeBinding.Get(target, "Shown"); RuntimeBinding.Call(target, "Run", 1);
			RuntimeBinding.Set(target, "late", 1); RuntimeBinding.Get(typeof(Target), "hidden");
			foreach (string name in new[] { "hidden", "Shown" }) RuntimeBinding.Get(target, name);
			void Forward(object o, string n, object v) => RuntimeBinding.Set(o, n, v);
			Forward(target, "hidden", 1);
			RuntimeBinding.Set(target, "missing", 1); RuntimeBinding.Call(target, "Run"); RuntimeBinding.Get(new Derived(), "absent");
			Forward(target, "gone", 1); RuntimeBinding.Get(anything, "noSuchMemberAnywhere");
			RuntimeBinding.Get(target, anything.ToString()!);
		}
	}
	""", parse, "probe.cs") });
if (probe.Resolved != 8 || probe.Dynamic != 1 || probe.Missing.Count != 5)
{
	Console.Error.WriteLine($"Resolver self-check failed: resolved {probe.Resolved}, dynamic {probe.Dynamic}, missing {probe.Missing.Count}.");
	foreach (string line in probe.Missing) Console.Error.WriteLine("  " + line);
	return 1;
}

var trees = projects.SelectMany(project => Directory.EnumerateFiles(Path.Combine(root, project), "*.cs", SearchOption.AllDirectories))
	.Where(path => !path.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar) && !path.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar))
	.OrderBy(path => path, StringComparer.Ordinal)
	.Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path), parse, Path.GetRelativePath(root, path))).ToArray();
var result = Analyze(trees);
foreach (string line in result.Missing) Console.Error.WriteLine("MISSING " + line);
if (result.Missing.Count != 0 || result.Resolved == 0)
{
	Console.Error.WriteLine($"RuntimeBinding member names: {result.Missing.Count} unresolved, {result.Resolved} resolved, {result.Dynamic} computed at run time.");
	return 1;
}
Console.WriteLine($"RuntimeBinding member names: {result.Resolved} resolved in {trees.Length} files; {result.Dynamic} call sites compute the name at run time and are not checked.");
return 0;

(int Resolved, int Dynamic, List<string> Missing) Analyze(SyntaxTree[] sources)
{
	var compilation = CSharpCompilation.Create("OpenPCR.Bindings.Probe", sources, references, options);
	var binding = compilation.GetTypeByMetadataName("OpenPCR.RuntimeBinding") ?? throw new InvalidOperationException("OpenPCR.RuntimeBinding is not referenced.");
	Dictionary<string, List<INamedTypeSymbol>>? declaring = null;
	int resolved = 0, dynamic = 0; var missing = new List<string>();

	foreach (var tree in sources)
	{
		var model = compilation.GetSemanticModel(tree);
		foreach (var call in tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
		{
			string? kind = Kind(model, call);
			if (kind is null || call.ArgumentList.Arguments.Count < 2) continue;
			var arguments = call.ArgumentList.Arguments;
			if (Forwarder(call) is not null) continue; // The forwarding body itself; its callers are checked instead.
			var receiver = arguments[0].Expression;
			ITypeSymbol? type = receiver is TypeOfExpressionSyntax typeOf ? model.GetTypeInfo(typeOf.Type).Type : model.GetTypeInfo(receiver).Type;
			var names = Names(model, arguments[1].Expression);
			if (type is null || type.TypeKind == TypeKind.Error) { missing.Add($"{Where(call)}: receiver type is unbound"); continue; }
			if (names is null) { dynamic++; continue; }
			int? count = kind != "Call" ? null : arguments.Count == 3 && model.GetTypeInfo(arguments[2].Expression).Type is IArrayTypeSymbol ? null : arguments.Count - 2;
			foreach (string name in names)
			{
				if (Has(type, name, kind, count) || Derived(type, name, kind, count)) resolved++;
				else missing.Add($"{Where(call)}: {kind} \"{name}\" on {type.ToDisplayString()}");
			}
		}
	}
	return (resolved, dynamic, missing);

	string Where(SyntaxNode node) { var span = node.GetLocation().GetLineSpan(); return $"{span.Path}:{span.StartLinePosition.Line + 1}"; }

	// RuntimeBinding.Set/Get/Call, or a local function / method whose expression body forwards its first two parameters to one.
	string? Kind(SemanticModel model, InvocationExpressionSyntax call)
	{
		var info = model.GetSymbolInfo(call);
		if ((info.Symbol ?? info.CandidateSymbols.FirstOrDefault()) is not IMethodSymbol method) return null;
		if (SymbolEqualityComparer.Default.Equals(method.ContainingType, binding)) return method.Name is "Set" or "Get" or "Call" ? method.Name : null;
		if (method.Parameters.Length < 2) return null;
		foreach (var reference in method.DeclaringSyntaxReferences)
		{
			var body = reference.GetSyntax() switch
			{
				LocalFunctionStatementSyntax local => local.ExpressionBody?.Expression,
				MethodDeclarationSyntax declared => declared.ExpressionBody?.Expression,
				_ => null
			};
			if (body is not InvocationExpressionSyntax inner || inner.ArgumentList.Arguments.Count < 2) continue;
			string? forwarded = Kind(compilation.GetSemanticModel(inner.SyntaxTree), inner);
			if (forwarded is not null && Forwarder(inner) is not null) return forwarded;
		}
		return null;
	}

	// Non-null when the call passes the enclosing function's own first two parameters straight through.
	SyntaxNode? Forwarder(InvocationExpressionSyntax call)
	{
		var owner = call.Parent is ArrowExpressionClauseSyntax arrow ? arrow.Parent : null;
		var parameters = owner switch { LocalFunctionStatementSyntax local => local.ParameterList, MethodDeclarationSyntax declared => declared.ParameterList, _ => null };
		var arguments = call.ArgumentList.Arguments;
		if (parameters is null || parameters.Parameters.Count < 2) return null;
		return arguments[0].Expression is IdentifierNameSyntax first && first.Identifier.ValueText == parameters.Parameters[0].Identifier.ValueText
			&& arguments[1].Expression is IdentifierNameSyntax second && second.Identifier.ValueText == parameters.Parameters[1].Identifier.ValueText ? owner : null;
	}

	// A literal, or a foreach variable ranging over an array of literals.
	string[]? Names(SemanticModel model, ExpressionSyntax expression)
	{
		if (model.GetConstantValue(expression) is { HasValue: true, Value: string literal }) return new[] { literal };
		if (expression is not IdentifierNameSyntax || model.GetSymbolInfo(expression).Symbol is not ILocalSymbol local) return null;
		if (local.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not ForEachStatementSyntax loop) return null;
		var initializer = loop.Expression switch
		{
			ImplicitArrayCreationExpressionSyntax array => array.Initializer,
			ArrayCreationExpressionSyntax array => array.Initializer,
			_ => null
		};
		if (initializer is null) return null;
		var values = initializer.Expressions.Select(item => model.GetConstantValue(item)).ToArray();
		return values.All(value => value.HasValue && value.Value is string) ? values.Select(value => (string)value.Value!).ToArray() : null;
	}

	bool Has(ITypeSymbol start, string name, string kind, int? count)
	{
		for (ITypeSymbol? type = start is ITypeParameterSymbol parameter ? parameter.ConstraintTypes.FirstOrDefault() ?? compilation.ObjectType : start; type is not null; type = type.BaseType)
			if (Declares(type, name, kind, count)) return true;
		return false;
	}
	bool Declares(ITypeSymbol type, string name, string kind, int? count) => type.GetMembers(name).Any(member => kind switch
	{
		// A field-like event is reached through its backing field, which carries the event's name.
		"Set" => member is IFieldSymbol or IEventSymbol || member is IPropertySymbol { SetMethod: not null },
		"Get" => member is IFieldSymbol or IPropertySymbol or IEventSymbol,
		_ => member is IMethodSymbol { MethodKind: MethodKind.Ordinary, IsGenericMethod: false } method && (count is null || method.Parameters.Length == count)
	});

	// The run-time receiver may be any subtype of the static type (object, a base class behind an `is` test).
	bool Derived(ITypeSymbol start, string name, string kind, int? count)
	{
		if (start.IsSealed || start.IsValueType) return false;
		if (declaring is null)
		{
			declaring = new Dictionary<string, List<INamedTypeSymbol>>();
			var pending = new Stack<INamespaceOrTypeSymbol>(compilation.References.Select(compilation.GetAssemblyOrModuleSymbol).OfType<IAssemblySymbol>()
				.Append(compilation.Assembly).Select(assembly => (INamespaceOrTypeSymbol)assembly.GlobalNamespace));
			while (pending.Count != 0)
			{
				var current = pending.Pop();
				foreach (var member in current.GetMembers())
				{
					if (member is INamespaceOrTypeSymbol nested) pending.Push(nested);
					if (current is INamedTypeSymbol owner && member is IFieldSymbol or IPropertySymbol or IMethodSymbol or IEventSymbol)
					{
						if (!declaring.TryGetValue(member.Name, out var owners)) declaring.Add(member.Name, owners = new List<INamedTypeSymbol>());
						if (owners.Count == 0 || !SymbolEqualityComparer.Default.Equals(owners[^1], owner)) owners.Add(owner);
					}
				}
			}
		}
		if (!declaring.TryGetValue(name, out var candidates)) return false;
		bool any = start.SpecialType == SpecialType.System_Object || start is ITypeParameterSymbol;
		return candidates.Any(candidate => Declares(candidate, name, kind, count) && (any || Inherits(candidate, start)));
	}
	bool Inherits(INamedTypeSymbol type, ITypeSymbol ancestor)
	{
		var target = ancestor.OriginalDefinition;
		if (type.AllInterfaces.Any(item => SymbolEqualityComparer.Default.Equals(item.OriginalDefinition, target))) return true;
		for (INamedTypeSymbol? current = type.BaseType; current is not null; current = current.BaseType)
			if (SymbolEqualityComparer.Default.Equals(current.OriginalDefinition, target)) return true;
		return false;
	}
}
