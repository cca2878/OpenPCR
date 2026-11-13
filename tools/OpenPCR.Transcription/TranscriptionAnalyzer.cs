using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OpenPCR.Transcription
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class TranscriptionAnalyzer : DiagnosticAnalyzer
	{
		private static readonly ImmutableArray<DiagnosticDescriptor> Rules = ImmutableArray.Create(
			Rule("PCR001", "Invalid or missing member registration"),
			Rule(Optimization.Code, "Invalid OPENPCR_OPTIMIZED region"));
		private static DiagnosticDescriptor Rule(string id, string title) => new DiagnosticDescriptor(id, title, "{0}", "OpenPCR.Transcription", DiagnosticSeverity.Error, true);
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Rules;
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
			context.EnableConcurrentExecution();
			context.RegisterCompilationAction(c =>
			{
				try
				{
					var built = SymbolIdentity.WithMetadataProjectReferences(c.Compilation, c.CancellationToken);
					// Reviews always judge the reference view; the optimized view only adds registered regions.
					bool optimizedBuild = Optimization.Defined(built);
					var compilation = optimizedBuild ? Optimization.View(built, defined: false) : built;
					var optimized = optimizedBuild ? built : Optimization.View(built, defined: true);
					AuditIds.RequireSourceOnlyMetadata(compilation, optimized);
					var options = c.Options.AnalyzerConfigOptionsProvider.GlobalOptions;
					string mode = options.TryGetValue("build_property.OpenPcrAuditMode", out var configured) ? configured : "Private";
					if (mode != "Public") throw new InvalidOperationException("This source candidate supports Public checks only; explicitly select -p:OpenPcrAuditMode=Public. Full audit validation requires the private repository.");
					var problems = AuditIds.Check(compilation);
					problems.AddRange(AuditIds.Check(optimized));
					problems.AddRange(Optimization.Check(compilation, optimized));

					// Both views share each file's text, so a span in either view maps onto the compiled tree of that file.
					var trees = c.Compilation.SyntaxTrees.ToDictionary(t => t.FilePath);
					Location Map(Location location) => location.SourceTree is { } tree && !c.Compilation.ContainsSyntaxTree(tree) && trees.TryGetValue(tree.FilePath, out var compiled)
						? Location.Create(compiled, location.SourceSpan) : location;
					foreach (var p in problems) c.ReportDiagnostic(Diagnostic.Create(Rules.Single(x => x.Id == p.Code), Map(p.Location), p.Member + ": " + p.Message));
				}
				catch (Exception e) { c.ReportDiagnostic(Diagnostic.Create(Rules[0], Location.None, "Cannot validate transcription inputs: " + e.Message)); }
			});
		}
	}
}
