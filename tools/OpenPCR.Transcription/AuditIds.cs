using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OpenPCR.Transcription
{
	public static class AuditIds
	{
		public const string MigrationName = "OpenPCR.MigrationAttribute", ImplicitName = "OpenPCR.ImplicitMigrationAttribute";
		private static bool ValidId(string value) => Guid.TryParseExact(value, "D", out var id) && id.ToString("D") == value && value[14] == '4' && "89ab".IndexOf(value[19]) >= 0;
		public static void RequireSourceOnlyMetadata(params Compilation[] views)
		{
			const string symbol = "OPENPCR_EMIT_AUDIT_METADATA";
			foreach (var compilation in views)
				foreach (var tree in compilation.SyntaxTrees)
					if (tree.Options.PreprocessorSymbolNames.Contains(symbol) || tree.GetRoot().DescendantTrivia(descendIntoTrivia: true)
						.Any(trivia => trivia.GetStructure() is DefineDirectiveTriviaSyntax { IsActive: true } directive && directive.Name.ValueText == symbol))
						throw new InvalidOperationException("Audit attributes must not be emitted in mirror metadata");
		}
		public static List<AuditProblem> Check(Compilation compilation)
		{
			var problems = new List<AuditProblem>();
			var used = new Dictionary<string, string>(StringComparer.Ordinal);
			foreach (var tree in compilation.SyntaxTrees)
			{
				var model = compilation.GetSemanticModel(tree);
				foreach (var syntax in tree.GetRoot().DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax>())
				{
					var type = (model.GetSymbolInfo(syntax).Symbol as IMethodSymbol)?.ContainingType?.ToDisplayString();
					if (type != MigrationName && type != ImplicitName && type != Optimization.AttributeName) continue;
					var args = syntax.ArgumentList?.Arguments;
					var value = args?.Count == 1 ? model.GetConstantValue(args.Value[0].Expression) : default;
					if (!value.HasValue || value.Value is not string id || !ValidId(id) || args!.Value[0].NameEquals != null)
					{ problems.Add(new AuditProblem("PCR001", "", "Audit attributes require exactly one random UUID v4", syntax.GetLocation())); continue; }
					var site = tree.FilePath + ":" + syntax.SpanStart;
					if (used.TryGetValue(id, out var previous) && previous != site) problems.Add(new AuditProblem("PCR001", "", "Duplicate audit registration ID", syntax.GetLocation())); else used[id] = site;
				}
			}
			return problems;
		}
	}
}
