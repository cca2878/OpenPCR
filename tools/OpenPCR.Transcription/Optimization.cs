using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace OpenPCR.Transcription
{
	/// <summary>
	/// <c>OPENPCR_OPTIMIZED</c> regions. The reference view (symbol undefined) is the audited mirror. The optimized view may
	/// differ only inside registered regions: a whole replaced body (<c>#if</c> optimized <c>#else</c> reference
	/// <c>#endif</c>) or whole added members (<c>#if</c> ... <c>#endif</c>). Violations are reported as PCR005.
	/// </summary>
	public static class Optimization
	{
		public const string Symbol = "OPENPCR_OPTIMIZED", AttributeName = "OpenPCR.OptimizationAttribute", Code = "PCR005";

		public sealed class Site
		{
			public ISymbol Symbol = null!;
			public ISymbol? ReferenceSymbol;
			public AttributeData Attribute = null!;
			public string Key = "", Kind = "";
			public Location Location = Location.None;
		}

		private sealed class Region
		{
			public SyntaxTree Tree = null!;
			public IfDirectiveTriviaSyntax If = null!;
			public ElseDirectiveTriviaSyntax? Else;
			public EndIfDirectiveTriviaSyntax EndIf = null!;
		}

		public static bool Defined(Compilation c) => c.SyntaxTrees.Any(t => t.Options.PreprocessorSymbolNames.Contains(Symbol));

		private static bool UsesSymbol(ExpressionSyntax condition) => condition.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Any(name => name.Identifier.ValueText == Symbol);

		/// <summary>The same compilation with the symbol defined or undefined in every tree.</summary>
		public static Compilation View(Compilation c, bool defined)
		{
			var trees = c.SyntaxTrees.Select(tree =>
			{
				var options = (CSharpParseOptions)tree.Options;
				var symbols = options.PreprocessorSymbolNames.Where(s => s != Symbol);
				if (defined) symbols = symbols.Concat(new[] { Symbol });
				return CSharpSyntaxTree.ParseText(tree.GetText(), options.WithPreprocessorSymbols(symbols), tree.FilePath);
			}).ToArray();
			return c.RemoveAllSyntaxTrees().AddSyntaxTrees(trees);
		}

		public static List<AuditProblem> Check(Compilation reference, Compilation optimized, Action<Site>? validate = null)
		{
			var problems = new List<AuditProblem>();
			void Problem(string member, string message, Location location) => problems.Add(new AuditProblem(Code, member, message, location));
			var referenceTrees = reference.SyntaxTrees.ToDictionary(t => t.FilePath);
			var registrations = new HashSet<(string Path, TextSpan Span)>();
			void Cover(AttributeData attribute)
			{
				if (attribute.ApplicationSyntaxReference is { } syntax) registrations.Add((syntax.SyntaxTree.FilePath, syntax.Span));
			}
			var replaced = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
			var added = new Dictionary<INamedTypeSymbol, HashSet<string>>(SymbolEqualityComparer.Default);
			foreach (var tree in optimized.SyntaxTrees)
			{
				var model = optimized.GetSemanticModel(tree); var root = tree.GetRoot(); var text = tree.GetText();
				bool Blank(int start, int end) => start <= end && text.ToString(TextSpan.FromBounds(start, end)).All(char.IsWhiteSpace);
				foreach (var region in Regions(tree, problems))
				{
					var location = Location.Create(tree, region.If.Span);
					if (region.Else is not null)
					{
						var block = root.FindToken(region.If.FullSpan.End).Parent?.AncestorsAndSelf().OfType<BlockSyntax>()
							.FirstOrDefault(b => b.Parent is BaseMethodDeclarationSyntax or AccessorDeclarationSyntax);
						if (block is null || !Blank(block.OpenBraceToken.Span.End, region.If.FullSpan.Start) || !Blank(region.EndIf.FullSpan.End, block.CloseBraceToken.SpanStart))
						{ Problem("", "A region with #else must replace one whole method or accessor body.", location); continue; }
						var symbol = model.GetDeclaredSymbol(block.Parent!);
						var referenceTree = referenceTrees[tree.FilePath];
						var referenceNode = referenceTree.GetRoot().FindNode(block.Parent!.Span);
						var referenceSymbol = reference.GetSemanticModel(referenceTree).GetDeclaredSymbol(referenceNode);
						if (symbol is null || referenceSymbol is null) { Problem("", "Replaced body has no declared member.", location); continue; }
						var key = SymbolIdentity.Key(referenceSymbol);
						var attribute = Attribute(symbol);
						replaced.Add(symbol);
						if (attribute is null) { Problem(key, "A replaced body requires [Optimization].", location); continue; }
						Cover(attribute);
						validate?.Invoke(new Site { Symbol = symbol, ReferenceSymbol = referenceSymbol, Attribute = attribute, Key = key, Kind = "replaced", Location = location });
						continue;
					}
					var start = region.If.FullSpan.End; var end = region.EndIf.FullSpan.Start;
					var members = root.DescendantTokens(TextSpan.FromBounds(start, end)).Where(t => t.SpanStart >= start && t.Span.End <= end)
						.Select(t => t.Parent?.AncestorsAndSelf().OfType<MemberDeclarationSyntax>().FirstOrDefault(m => m.Parent is TypeDeclarationSyntax)).ToList();
					if (members.Count == 0 || members.Any(m => m is null || m.SpanStart < start || m.Span.End > end) || members.Select(m => m!.Parent).Distinct().Count() != 1)
					{ Problem("", "A region without #else must contain only whole member declarations of one type.", location); continue; }
					foreach (var member in members.Distinct())
					{
						var symbols = member is BaseFieldDeclarationSyntax field
							? field.Declaration.Variables.Select(v => model.GetDeclaredSymbol(v)).ToList()
							: new List<ISymbol?> { model.GetDeclaredSymbol(member!) };
						foreach (var symbol in symbols)
						{
							if (symbol is null) { Problem("", "Added member has no declared symbol.", location); continue; }
							var key = SymbolIdentity.Key(symbol);
							if (!added.TryGetValue(symbol.ContainingType, out var keys)) added.Add(symbol.ContainingType, keys = new HashSet<string>());
							keys.Add(key);
							if (symbol is IPropertySymbol property)
							{
								foreach (var accessor in new[] { property.GetMethod, property.SetMethod }.Where(method => method is not null && !method.IsImplicitlyDeclared))
									keys.Add(SymbolIdentity.Key(accessor!));
							}
							bool allowed = symbol switch
							{
								IFieldSymbol f => !f.IsStatic && !f.IsConst,
								IMethodSymbol m => m.MethodKind == MethodKind.Ordinary && !m.IsVirtual && !m.IsAbstract && !m.IsOverride && !m.IsExtern,
								IPropertySymbol p => !p.IsStatic && !p.IsVirtual && !p.IsAbstract && !p.IsOverride,
								_ => false,
							};
							if (!allowed) Problem(key, "Added members are limited to instance fields, non-virtual methods and instance non-virtual properties; static state would change the checkpoint layout.", location);
							if (symbol.DeclaredAccessibility is not (Accessibility.Private or Accessibility.Internal or Accessibility.ProtectedAndInternal))
								Problem(key, "Added members must be private, internal or private protected.", location);
							var attribute = Attribute(symbol);
							if (attribute is null) { Problem(key, "An added member requires [Optimization].", location); continue; }
							Cover(attribute);
							validate?.Invoke(new Site { Symbol = symbol, Attribute = attribute, Key = key, Kind = "added", Location = location });
						}
					}
				}
			}
			// Registrations must belong to a region, in both views.
			foreach (var compilation in new[] { reference, optimized })
				foreach (var tree in compilation.SyntaxTrees)
				{
					var model = compilation.GetSemanticModel(tree);
					foreach (var node in tree.GetRoot().DescendantNodes().OfType<AttributeSyntax>())
					{
						if ((model.GetSymbolInfo(node).Symbol as IMethodSymbol)?.ContainingType.ToDisplayString() != AttributeName) continue;
						if (registrations.Contains((tree.FilePath, node.Span))) continue;
						var owner = node.Ancestors().FirstOrDefault(parent => parent is AccessorDeclarationSyntax or MemberDeclarationSyntax);
						var symbol = owner is BaseFieldDeclarationSyntax field ? model.GetDeclaredSymbol(field.Declaration.Variables.First()) : owner is null ? null : model.GetDeclaredSymbol(owner);
						Problem(symbol is null ? "" : SymbolIdentity.Key(symbol), "[Optimization] marks a member without an OPENPCR_OPTIMIZED region.", node.GetLocation());
					}
				}
			// The optimized member table is the reference table plus the declared additions.
			foreach (var type in replaced.Select(s => s.ContainingType).Concat(added.Keys).Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default))
			{
				var referenceType = FindIn(reference, type) as INamedTypeSymbol;
				var optimizedKeys = new HashSet<string>(type.GetMembers().Where(m => !m.IsImplicitlyDeclared).Select(SymbolIdentity.Key));
				var referenceKeys = referenceType is null ? new HashSet<string>() : new HashSet<string>(referenceType.GetMembers().Where(m => !m.IsImplicitlyDeclared).Select(SymbolIdentity.Key));
				var additions = added.TryGetValue(type, out var declared) ? declared : new HashSet<string>();
				if (!optimizedKeys.SetEquals(referenceKeys.Concat(additions)))
					Problem(SymbolIdentity.TypeKey(type), "The optimized member table differs from the reference table plus the declared additions.", type.Locations.FirstOrDefault() ?? Location.None);
			}
			return problems;
		}

		private static IEnumerable<Region> Regions(SyntaxTree tree, List<AuditProblem> problems)
		{
			var stack = new Stack<(bool Ours, Region? Region)>();
			foreach (var trivia in tree.GetRoot().DescendantTrivia(descendIntoTrivia: false).Where(t => t.IsDirective))
			{
				var directive = (DirectiveTriviaSyntax)trivia.GetStructure()!;
				var location = Location.Create(tree, directive.Span);
				void Problem(string message) => problems.Add(new AuditProblem(Code, "", message, location));
				switch (directive)
				{
					case DefineDirectiveTriviaSyntax define when define.Name.ValueText == Symbol: Problem("Do not #define " + Symbol + "; the build configuration defines it."); break;
					case UndefDirectiveTriviaSyntax undef when undef.Name.ValueText == Symbol: Problem("Do not #undef " + Symbol + "."); break;
					case IfDirectiveTriviaSyntax @if:
						{
							bool exact = @if.Condition is IdentifierNameSyntax name && name.Identifier.ValueText == Symbol;
							if (!exact && UsesSymbol(@if.Condition)) Problem("The condition must be exactly #if " + Symbol + ".");
							if (stack.Any(s => s.Ours)) Problem("Conditional directives cannot be nested inside an " + Symbol + " region.");
							else if (exact && stack.Count != 0) Problem("An " + Symbol + " region cannot be nested inside another conditional.");
							stack.Push((exact, exact ? new Region { Tree = tree, If = @if } : null));
							break;
						}
					case ElifDirectiveTriviaSyntax elif:
						if (stack.Count != 0 && stack.Peek().Ours) Problem("#elif is not allowed in an " + Symbol + " region.");
						else if (UsesSymbol(elif.Condition)) Problem(Symbol + " may only appear in #if " + Symbol + ".");
						break;
					case ElseDirectiveTriviaSyntax @else when stack.Count != 0 && stack.Peek().Ours:
						stack.Peek().Region!.Else = @else; break;
					case EndIfDirectiveTriviaSyntax endIf when stack.Count != 0:
						{
							var (ours, region) = stack.Pop();
							if (ours) { region!.EndIf = endIf; yield return region; }
							break;
						}
				}
			}
		}

		private static AttributeData? Attribute(ISymbol symbol)
		{
			var direct = symbol.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == AttributeName);
			return direct ?? (symbol is IMethodSymbol { AssociatedSymbol: { } owner } ? owner.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == AttributeName) : null);
		}

		private static ISymbol? FindIn(Compilation compilation, ISymbol symbol)
		{
			var tree = symbol.Locations.FirstOrDefault(l => l.IsInSource)?.SourceTree;
			var target = tree is null ? null : compilation.SyntaxTrees.FirstOrDefault(t => t.FilePath == tree.FilePath);
			if (target is null) return null;
			var node = target.GetRoot().FindNode(symbol.Locations.First(l => l.IsInSource).SourceSpan);
			return compilation.GetSemanticModel(target).GetDeclaredSymbol(node);
		}
	}
}
