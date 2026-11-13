using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

/// <summary>Check the real Support contract separately from the permissive analyzer-negative fixtures.</summary>
internal static class AuditAttributeApiTests
{
	internal static int Run()
	{
		int checks = 0;
		void Check(bool valid, string message)
		{
			if (!valid) throw new Exception("Public attribute API: " + message);
			checks++;
		}
		var assembly = typeof(OpenPCR.MigrationAttribute).Assembly;
		const BindingFlags declared = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
		foreach (var (type, targets, multiple) in new[]
		{
			(typeof(OpenPCR.MigrationAttribute), AttributeTargets.All, false),
			(typeof(OpenPCR.ImplicitMigrationAttribute), AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Event | AttributeTargets.Property, true),
			(typeof(OpenPCR.OptimizationAttribute), AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Property | AttributeTargets.Field, false)
		})
		{
			Check(type.IsSealed && type.BaseType == typeof(Attribute), type.Name + " must remain a sealed attribute.");
			var constructors = type.GetConstructors();
			Check(constructors.Length == 1 && constructors[0].GetParameters() is var parameters && parameters.Length == 1 && parameters[0].ParameterType == typeof(string) && parameters[0].Name == "id", type.Name + " must expose only the string id constructor.");
			var properties = type.GetProperties(declared);
			Check(properties.Length == 1 && properties[0].Name == "Id" && properties[0].PropertyType == typeof(string) && properties[0].GetMethod is not null && properties[0].SetMethod is null, type.Name + " must expose only a read-only Id.");
			Check(type.GetFields(declared).Length == 0 && type.GetEvents(declared).Length == 0 && type.GetMethods(declared).Select(m => m.Name).SequenceEqual(new[] { "get_Id" }), type.Name + " exposes unexpected public members.");
			var usage = type.GetCustomAttribute<AttributeUsageAttribute>()!;
			Check(usage.ValidOn == targets && usage.AllowMultiple == multiple && !usage.Inherited, type.Name + " changed its registration targets or multiplicity.");
			Check(type.GetCustomAttribute<ConditionalAttribute>()?.ConditionString == "OPENPCR_EMIT_AUDIT_METADATA", type.Name + " lost conditional metadata suppression.");
			var instance = constructors[0].Invoke(new object[] { "870b7aa5-ae3f-4e1b-b183-1581f613b3ce" });
			Check((string?)properties[0].GetValue(instance) == "870b7aa5-ae3f-4e1b-b183-1581f613b3ce", type.Name + " does not preserve its ID.");
		}
		foreach (string name in new[] { "MigrationScope", "MigrationReview", "MigrationOrigin", "MigrationContext" })
			Check(assembly.GetType("OpenPCR." + name) is null, "Legacy audit enum remains public: " + name);

		var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator)
			.Append(assembly.Location).Distinct(StringComparer.Ordinal).Select(path => MetadataReference.CreateFromFile(path)).ToArray();
		void Compile(string source, bool valid)
		{
			foreach (bool emitted in new[] { false, true })
			{
				var parse = new CSharpParseOptions(LanguageVersion.CSharp9, preprocessorSymbols: emitted ? new[] { "OPENPCR_EMIT_AUDIT_METADATA" } : Array.Empty<string>());
				var tree = CSharpSyntaxTree.ParseText(source, parse, "real-attribute-api.cs");
				var compilation = CSharpCompilation.Create("RealAttributeFixture", new[] { tree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
				var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
				Check((errors.Length == 0) == valid, $"Unexpected compiler result (metadata={emitted}): {source}: {string.Join("; ", errors.Select(d => d.ToString()))}");
			}
		}
		const string id = "\"870b7aa5-ae3f-4e1b-b183-1581f613b3ce\"";
		foreach (string name in new[] { "Migration", "ImplicitMigration", "Optimization" })
		{
			string Source(string arguments) => name == "ImplicitMigration"
				? "[OpenPCR." + name + "(" + arguments + ")] class Sample { }"
				: "class Sample { [OpenPCR." + name + "(" + arguments + ")] void Run() { } }";
			Compile(Source(id), valid: true);
			Compile(Source("id: " + id), valid: true);
			Compile(Source(""), valid: false);
			Compile(Source(id + ", \"legacy\""), valid: false);
			foreach (string property in new[] { "Scope", "Review", "Contexts", "Origin" }) Compile(Source(id + ", " + property + " = 0"), valid: false);
			foreach (string property in new[] { "Signature", "Evidence", "Reason", "OriginRef", "Reference" }) Compile(Source(id + ", " + property + " = \"legacy\""), valid: false);
		}
		Compile("class Sample { [OpenPCR.Migration(0, 0)] void Run() { } }", valid: false);
		Compile("[OpenPCR.ImplicitMigration(\".ctor()\", 0, 0)] class Sample { }", valid: false);
		return checks;
	}
}
