using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using OpenPCR.Transcription;

const string id = "870b7aa5-ae3f-4e1b-b183-1581f613b3ce";
const string other = "3c1e5ccb-d362-4232-98f2-e683d73bf578";
const string definitions = """
namespace OpenPCR
{
    [System.Diagnostics.Conditional("OPENPCR_EMIT_AUDIT_METADATA")]
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
    public sealed class MigrationAttribute : System.Attribute
    {
        public MigrationAttribute(string id) { }
        public MigrationAttribute(string id, string extra) { }
        public string Reason { get; set; }
    }
    [System.Diagnostics.Conditional("OPENPCR_EMIT_AUDIT_METADATA")]
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
    public sealed class ImplicitMigrationAttribute : System.Attribute
    {
        public ImplicitMigrationAttribute(string id) { }
    }
    [System.Diagnostics.Conditional("OPENPCR_EMIT_AUDIT_METADATA")]
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = true)]
    public sealed class OptimizationAttribute : System.Attribute
    {
        public OptimizationAttribute(string id) { }
        public string Reference { get; set; }
    }
}
""";
var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
	.Select(path => MetadataReference.CreateFromFile(path)).ToArray();
int assertions = 0;
string Mark(string name = "Migration", string? value = null) => $"[OpenPCR.{name}(\"{value ?? id}\")]";
string ReplaceBody(string attribute, string optimizedBody = "return 2;", string referenceBody = "return 1;", string condition = "OPENPCR_OPTIMIZED") => $$"""
class Sample
{
    {{attribute}} public int Run()
    {
#if {{condition}}
        {{optimizedBody}}
#else
        {{referenceBody}}
#endif
    }
}
""";
string AddMember(string member) => "class Sample {\n#if OPENPCR_OPTIMIZED\n" + member + "\n#endif\n}";
CSharpCompilation Compile(IEnumerable<string> sources, string[] defines)
{
	var options = new CSharpParseOptions(LanguageVersion.CSharp9, preprocessorSymbols: defines);
	var trees = sources.Prepend(definitions).Select((text, index) => CSharpSyntaxTree.ParseText(text, options, "fixture" + index + ".cs"));
	var compilation = CSharpCompilation.Create("PublicFixture", trees, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
	var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
	if (errors.Length != 0) throw new Exception("Invalid test compilation: " + string.Join("; ", errors.Select(d => d.ToString())));
	return compilation;
}
void Test(string name, string source, string expected = "", string? mode = "Public", string[]? extra = null, string[]? defines = null)
{
	foreach (bool optimized in new[] { false, true })
	{
		var symbols = (defines ?? []).Concat(optimized ? ["OPENPCR_OPTIMIZED"] : Array.Empty<string>()).ToArray();
		var compilation = Compile(new[] { source }.Concat(extra ?? []), symbols);
		var diagnostics = compilation.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new TranscriptionAnalyzer()),
			new AnalyzerOptions(ImmutableArray<AdditionalText>.Empty, new ModeProvider(mode))).GetAnalyzerDiagnosticsAsync().GetAwaiter().GetResult();
		string actual = string.Join(",", diagnostics.Select(d => d.Id).Distinct().Order());
		if (actual != expected) throw new Exception($"{name} (optimized={optimized}): expected [{expected}], actual [{actual}]: {string.Join("; ", diagnostics.Select(d => d.ToString()))}");
		foreach (var diagnostic in diagnostics)
			if (diagnostic.Location.SourceTree is { } tree && !compilation.ContainsSyntaxTree(tree))
				throw new Exception("Diagnostic was not mapped to the original source: " + name);
		assertions++;
	}
}

Test("valid explicit ID", "class Sample { " + Mark() + " int Run()=>1; }");
Test("valid implicit ID", Mark("ImplicitMigration") + " class Sample { }");
Test("named constructor ID", "class Sample { [OpenPCR.Migration(id: \"" + id + "\")] int Run()=>1; }");
foreach (string bad in new[] { "bad", "", id.ToUpperInvariant(), id.Replace("-4e1b-", "-1e1b-"), id.Replace("-b183-", "-7183-"), "00000000-0000-0000-0000-000000000000" })
	Test("invalid ID " + bad, "class Sample { " + Mark(value: bad) + " int Run()=>1; }", "PCR001");
Test("extra constructor field", "class Sample { [OpenPCR.Migration(\"" + id + "\", \"extra\")] int Run()=>1; }", "PCR001");
Test("extra named field", "class Sample { [OpenPCR.Migration(\"" + id + "\", Reason=\"extra\")] int Run()=>1; }", "PCR001");
Test("duplicate method IDs", "class Sample { " + Mark() + " int First()=>1; " + Mark() + " int Second()=>2; }", "PCR001");
Test("duplicate IDs across files", "class First { " + Mark() + " int Run()=>1; }", "PCR001", extra: ["class Second { " + Mark() + " int Run()=>1; }"]);
Test("duplicate IDs across kinds", Mark("ImplicitMigration") + " class Sample { " + Mark() + " int Run()=>1; }", "PCR001");
Test("unrelated attribute", "class MigrationAttribute : System.Attribute { public MigrationAttribute(string text) {} } [Migration(\"not-an-id\")] class Sample { }");
Test("both branch IDs checked", "class Sample {\n#if OPENPCR_OPTIMIZED\n" + Mark(value: "bad") + " int Other()=>2;\n#else\n" + Mark() + " int Run()=>1;\n#endif\n}", "PCR001,PCR005");
Test("whole body replacement", ReplaceBody(Mark("Optimization")));
Test("replacement requires registration", ReplaceBody(""), "PCR005");
Test("replacement ID checked", ReplaceBody(Mark("Optimization", "bad")), "PCR001");
Test("whole body required", ReplaceBody(Mark("Optimization")).Replace("#if OPENPCR_OPTIMIZED", "int unused = 0;\n#if OPENPCR_OPTIMIZED"), "PCR005");
Test("exact condition required", ReplaceBody(Mark("Optimization"), condition: "OPENPCR_OPTIMIZED && true"), "PCR005");
Test("no nested directives", ReplaceBody(Mark("Optimization"), "#if OTHER\nreturn 2;\n#else\nreturn 3;\n#endif"), "PCR005");
Test("no elif", ReplaceBody(Mark("Optimization")).Replace("#else", "#elif OTHER\nreturn 3;\n#else"), "PCR005");
Test("no local define", "#define OPENPCR_OPTIMIZED\nclass Sample {}", "PCR005");
Test("no local undef", "#undef OPENPCR_OPTIMIZED\nclass Sample {}", "PCR005");
Test("added instance field", AddMember(Mark("Optimization") + " private int value;"));
Test("added nonvirtual method", AddMember(Mark("Optimization") + " private int Run()=>1;"));
Test("added property registration covers its accessors", AddMember(Mark("Optimization") + " private int Value { get; set; }"));
Test("added member requires registration", AddMember("private int value;"), "PCR005");
Test("added member cannot be public", AddMember(Mark("Optimization") + " public int value;"), "PCR005");
Test("added member cannot add static state", AddMember(Mark("Optimization") + " private static int value;"), "PCR005");
Test("added member cannot be virtual", AddMember(Mark("Optimization") + " internal virtual int Run()=>1;"), "PCR005");
Test("registration needs a region", "class Sample { " + Mark("Optimization") + " int Run()=>1; }", "PCR005");
Test("distinct added IDs", AddMember(Mark("Optimization") + " private int left; " + Mark("Optimization", other) + " private int right;"));
Test("duplicate added IDs", AddMember(Mark("Optimization") + " private int left; " + Mark("Optimization") + " private int right;"), "PCR001");
Test("audit metadata cannot be emitted", "class Sample { " + Mark() + " int Run()=>1; }", "PCR001", defines: ["OPENPCR_EMIT_AUDIT_METADATA"]);
string registered = "class Sample { " + Mark() + " int Run()=>1; }";
Test("file-local audit metadata rejected", "#define OPENPCR_EMIT_AUDIT_METADATA\n" + registered, "PCR001");
Test("audit metadata rejected in another file", registered, "PCR001", extra: ["#define OPENPCR_EMIT_AUDIT_METADATA\nclass Other {}"]);
Test("optimized-only audit metadata rejected", "#if OPENPCR_OPTIMIZED\n#define OPENPCR_EMIT_AUDIT_METADATA\n#endif\n" + registered, "PCR001");
Test("reference-only audit metadata rejected", "#if !OPENPCR_OPTIMIZED\n#define OPENPCR_EMIT_AUDIT_METADATA\n#endif\n" + registered, "PCR001");
Test("local undef does not permit a forbidden define", "#define OPENPCR_EMIT_AUDIT_METADATA\n#undef OPENPCR_EMIT_AUDIT_METADATA\n" + registered, "PCR001");
Test("global metadata definition rejected despite local undef", "#undef OPENPCR_EMIT_AUDIT_METADATA\n" + registered, "PCR001", defines: ["OPENPCR_EMIT_AUDIT_METADATA"]);
Test("inactive audit metadata definition allowed", "#if false\n#define OPENPCR_EMIT_AUDIT_METADATA\n#endif\n" + registered);
Test("audit metadata text is not a directive", "// #define OPENPCR_EMIT_AUDIT_METADATA\nclass Sample { const string text=\"#define OPENPCR_EMIT_AUDIT_METADATA\"; " + Mark() + " int Run()=>1; }");
Test("private mode rejected", "class Sample {}", "PCR001", mode: "Private");
Test("unknown mode rejected", "class Sample {}", "PCR001", mode: "Misspelled");
Test("missing mode rejected", "class Sample {}", "PCR001", mode: null);
foreach (string symbol in new[] { "OPENPCR_OPTIMIZED", @"OPENPCR_OPTIMIZ\u0045D", @"OPENPCR_OPTIMIZ\U00000045D", @"\u004fPENPCR_OPTIMIZED", "OPENPCR_OPTI\u200cMIZED", @"OPENPCR_OPTI\u200cMIZED" })
{
	Test("unregistered condition " + symbol, ReplaceBody("", condition: symbol), "PCR005");
	Test("registered condition " + symbol, ReplaceBody(Mark("Optimization"), condition: symbol));
	Test("complex condition " + symbol, ReplaceBody(Mark("Optimization"), condition: symbol + " && true"), "PCR005");
	Test("negated condition " + symbol, ReplaceBody("", condition: "!" + symbol), "PCR005");
	Test("parenthesized condition " + symbol, ReplaceBody("", condition: "(" + symbol + ")"), "PCR005");
	Test("elif condition " + symbol, "class Sample { int Run(){\n#if OTHER\nreturn 0;\n#elif " + symbol + "\nreturn 2;\n#else\nreturn 1;\n#endif\n} }", "PCR005");
	Test("local define " + symbol, "#define " + symbol + "\nclass Sample {}", "PCR005");
	Test("local undef " + symbol, "#undef " + symbol + "\nclass Sample {}", "PCR005");
	Test("cross-file optimization " + symbol, "class First {}", "PCR005", extra: [ReplaceBody("", condition: symbol)]);
}
foreach (string name in new[] { @"OpenPCR.Optimiz\u0061tion", @"OpenPCR.Optimiz\U00000061tionAttribute", "O" })
{
	string alias = name == "O" ? "using O=OpenPCR.OptimizationAttribute;\n" : "";
	string attribute = "[" + name + "(\"" + id + "\")]";
	Test("orphan registration " + name, alias + "class Sample { " + attribute + " int Run()=>1; }", "PCR005");
	Test("registered replacement " + name, alias + ReplaceBody(attribute));
	Test("registered addition " + name, alias + AddMember(attribute + " private int value;"));
}
Test("implicit backing field cannot carry an orphan registration", "class Sample { [field:OpenPCR.Optimization(\"" + id + "\")] public int Value {get;set;} }", "PCR005");
Test("event backing field cannot carry an orphan registration", "class Sample { [field:OpenPCR.Optimization(\"" + id + "\")] public event System.Action Value; }", "PCR005");
Test("event accessors cannot carry an orphan registration", "class Sample { [method:OpenPCR.Optimization(\"" + id + "\")] public event System.Action Value; }", "PCR005");
Test("property registration covers getter replacement", "class Sample { " + Mark("Optimization") + " private int Value { get {\n#if OPENPCR_OPTIMIZED\nreturn 2;\n#else\nreturn 1;\n#endif\n} } }");
Test("accessor registration covers its own replacement", "class Sample { private int Value { " + Mark("Optimization") + " get {\n#if OPENPCR_OPTIMIZED\nreturn 2;\n#else\nreturn 1;\n#endif\n} } }");
Test("added expression property", AddMember(Mark("Optimization") + " private int Value=>1;"));
Test("similar symbol is independent", ReplaceBody("", condition: "OPENPCR_OPTIMIZED_HELPER"));
Console.WriteLine($"Public transcription: {assertions} reference/optimized checks passed.");
Console.WriteLine($"Public attribute API: {AuditAttributeApiTests.Run()} checks passed.");

sealed class ModeProvider(string? mode) : AnalyzerConfigOptionsProvider
{
	private sealed class Options(string? value) : AnalyzerConfigOptions
	{
		public override bool TryGetValue(string key, out string result)
		{
			result = value ?? "";
			return key == "build_property.OpenPcrAuditMode" && value != null;
		}
	}
	public override AnalyzerConfigOptions GlobalOptions => new Options(mode);
	public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => new Options(null);
	public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => new Options(null);
}
