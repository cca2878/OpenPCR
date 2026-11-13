using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;

namespace OpenPCR.Transcription
{
	public sealed class AuditProblem
	{
		public string Code, Member, Message;
		public Location Location;
		public AuditProblem(string code, string member, string message, Location location) { Code = code; Member = member; Message = message; Location = location; }
	}
	public static class SymbolIdentity
	{
		public static Compilation WithMetadataProjectReferences(Compilation compilation, CancellationToken cancellationToken = default)
		{
			// IDE workspaces use source project references; build/CLI use emitted metadata.
			// Emit the current dependency in memory so compiler-generated contracts are identical.
			foreach (var reference in compilation.References.OfType<CompilationReference>().ToArray())
			{
				var dependency = WithMetadataProjectReferences(reference.Compilation, cancellationToken);
				using (var image = new MemoryStream())
				{
					var result = dependency.Emit(image, cancellationToken: cancellationToken);
					if (!result.Success) throw new InvalidOperationException("Cannot emit project reference " + dependency.AssemblyName + ": " + string.Join("; ", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)));
					compilation = compilation.ReplaceReference(reference, MetadataReference.CreateFromImage(image.ToArray(), reference.Properties));
				}
			}
			return compilation;
		}
		public static string TypeKey(ITypeSymbol t)
		{
			if (t is INamedTypeSymbol n)
			{
				if (!n.IsDefinition && n.TypeArguments.Length != 0) return TypeKey(n.OriginalDefinition) + "<" + string.Join(",", n.TypeArguments.Select(TypeKey)) + ">";
				var names = new Stack<string>(); for (var x = n; x != null; x = x.ContainingType) names.Push(x.MetadataName);
				return (n.ContainingNamespace.IsGlobalNamespace ? "" : n.ContainingNamespace.ToDisplayString() + ".") + string.Join("+", names);
			}
			if (t is IArrayTypeSymbol a) return TypeKey(a.ElementType) + "[" + new string(',', a.Rank - 1) + "]";
			if (t is IPointerTypeSymbol p) return TypeKey(p.PointedAtType) + "*";
			if (t is ITypeParameterSymbol g) return (g.TypeParameterKind == TypeParameterKind.Method ? "!!" : "!") + g.Ordinal;
			return t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "");
		}
		private static string Parameter(IParameterSymbol p) => (p.RefKind == RefKind.None ? "" : p.RefKind.ToString().ToLowerInvariant() + " ") + TypeKey(p.Type) + (p.IsParams ? " params" : "");
		public static string Signature(ISymbol s)
		{
			if (s is IMethodSymbol m)
			{
				var tp = m.TypeParameters; var ps = m.Parameters; var rt = m.ReturnType;
				return (m.MethodKind == MethodKind.Constructor ? ".ctor" : m.MetadataName + (tp.Length == 0 ? "" : "`" + tp.Length)) + "(" + string.Join(",", ps.Select(Parameter)) + ")" + (m.MethodKind == MethodKind.Constructor ? "" : ":" + TypeKey(rt));
			}
			if (s is IPropertySymbol p) return "property " + p.MetadataName + "(" + string.Join(",", p.Parameters.Select(Parameter)) + "):" + TypeKey(p.Type);
			if (s is IFieldSymbol f) return "field " + f.Name + ":" + TypeKey(f.Type);
			if (s is IEventSymbol e) return "event " + e.Name + ":" + TypeKey(e.Type);
			return s.Kind.ToString().ToLowerInvariant() + " " + s.Name;
		}
		public static string Key(ISymbol s) => TypeKey(s.ContainingType) + "::" + Signature(s);
	}
}
